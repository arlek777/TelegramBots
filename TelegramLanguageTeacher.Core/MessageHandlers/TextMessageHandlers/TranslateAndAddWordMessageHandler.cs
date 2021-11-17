﻿using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBots.Common.Extensions;
using TelegramBots.Common.MessageHandling;
using TelegramBots.Common.Services;
using TelegramBots.DomainModels.LanguageTeacher;
using TelegramLanguageTeacher.Core.Helpers;
using TelegramLanguageTeacher.Core.Models;
using TelegramLanguageTeacher.Core.Models.Responses;
using TelegramLanguageTeacher.Core.Services;
using User = TelegramBots.DomainModels.LanguageTeacher.User;

namespace TelegramLanguageTeacher.Core.MessageHandlers.TextMessageHandlers
{
    public class TranslateAndAddWordMessageRequest : BaseRequest
    {
        public override bool AcceptUpdate(Update update)
        {
            Update = update;
            return update.IsTextMessage();
        }
    }

    public class TranslateAndAddWordMessageHandler : IRequestHandler<TranslateAndAddWordMessageRequest, bool>
    {
        private readonly IWordService _wordService;
        private readonly IUserService _userService;
        private readonly ITranslatorService _translatorService;
        private readonly ITelegramBotService<LanguageTeacherBot> _telegramService;
        private readonly IWordNormalizationService _normalizationService;
        private readonly IDefaultLogger _logger;

        public TranslateAndAddWordMessageHandler(IWordService wordService, 
            IUserService userService, 
            ITranslatorService translatorService, 
            ITelegramBotService<LanguageTeacherBot> telegramService,
            IWordNormalizationService normalizationService, 
            IDefaultLogger logger)
        {
            _wordService = wordService;
            _userService = userService;
            _translatorService = translatorService;
            _telegramService = telegramService;
            _normalizationService = normalizationService;
            _logger = logger;
        }

        public async Task<bool> Handle(TranslateAndAddWordMessageRequest request, CancellationToken token)
        {
            var update = request.Update;

            // Get message and lemmatize it
            var userId = update.Message.From.Id;
            var messageText = update.Message.Text.Trim().ToLowerInvariant();

            if (messageText.Length > 500)
                return false;

            bool toNormalize = messageText.Split(' ').Length == 1;
            messageText = toNormalize ? _normalizationService.Normalize(messageText) : CustomNormalize(messageText);

            // Translate or get from cache
            Word translatedWord = await Translate(userId, messageText);

            await _logger.Log("Transalted: " + JsonConvert.SerializeObject(translatedWord));

            if (string.IsNullOrWhiteSpace(translatedWord.Translate) && string.IsNullOrWhiteSpace(translatedWord.Definition))
            {
                await _telegramService.SendTextMessage(userId, TelegramMessageTexts.NoTranslationFound + "\n\n" 
                                                                                                       + TelegramMessageTexts.AddCustomTranslation);
                return true;
            }

            // Save to DB
            var word = await AddWordToDb(update, translatedWord);

            // Send Telegram Message
            await SendTranslationData(userId, word);

            return true;
        }

        private string CustomNormalize(string word)
        {
            if (word.StartsWith("to ", StringComparison.InvariantCultureIgnoreCase))
            {
                word = word.Remove(0, 3);
            }

            if (word.StartsWith("a ", StringComparison.InvariantCultureIgnoreCase))
            {
                word = word.Remove(0, 2);
            }

            if (word.StartsWith("an ", StringComparison.InvariantCultureIgnoreCase))
            {
                word = word.Remove(0, 3);
            }

            return word;
        }

        // Tries to get word from cache, if failed, translate it and add word to cache
        private async Task<Word> Translate(int userId, string text)
        {
            Word word = await _wordService.FindWordInUserDict(userId, text);

            if (word != null)
            {
                return word;
            }
            else
            {
                WordTranslationResponse translationResponse = await _translatorService.Translate(text);

                var translations = translationResponse.Translations.Select(t => t.Translation).Take(LanguageTeacherConstants.TranslationCounts);
                var examples = translationResponse.Examples.Take(LanguageTeacherConstants.ExamplesCount);
                var definitions = translationResponse.Definitions.Where(d => d != null).Select(d => $"{d.PartOfSpeech}-{d.Definition}");

                var joinedTranslations = string.Join('\n', translations);
                var joinedExamples = string.Join('\n', examples);
                var joinedDefinitions = string.Join('\n', definitions);

                word = new Word()
                {
                    Original = translationResponse.Word,
                    Translate = joinedTranslations,
                    Examples = joinedExamples,
                    Definition = joinedDefinitions,
                    AudioLink = translationResponse.AudioLink
                };
            }

            return word;
        }

        private async Task<Word> AddWordToDb(Update update, Word wordModel)
        {
            var userId = update.Message.From.Id;

            var word = await _wordService.AddWord(userId, wordModel);

            if (word == null)
            {
                await _userService.CreateNewUser(new User()
                {
                    UserName = update.Message.From.FirstName,
                    TelegramUserId = update.Message.From.Id
                });

                word = await _wordService.AddWord(userId, wordModel);
            }

            await _logger.Log("Added to db Word: " + JsonConvert.SerializeObject(wordModel));

            return word;
        }

        private async Task SendTranslationData(int userId, Word word)
        {
            var button = GetButton(word.Id);
            var formattedTranslation = EmojiTextFormatter.FormatFinalTranslationMessage(word);

            if (!string.IsNullOrWhiteSpace(word.AudioLink))
            {
                await _telegramService.SendAudioMessage(userId, word.AudioLink, word.Original);
            }

            await _telegramService.SendInlineButtonMessage(userId, formattedTranslation, button);

            var randomReminder = new Random().Next(0, 3);
            if (randomReminder == 2)
            {
                await _telegramService.SendTextMessage(userId, "Let's /repeat your words. Just click /repeat.");
            }

            await _logger.Log("SendTextMessage with translation: " + formattedTranslation);
        }

        private InlineKeyboardMarkup GetButton(Guid wordId)
        {
            return new InlineKeyboardMarkup(new[]
            {
                new InlineKeyboardButton()
                {
                    CallbackData = $"{TelegramCallbackCommands.RemoveWord}_{wordId}",
                    Text = TelegramMessageTexts.RemoveWord
                },
                new InlineKeyboardButton()
                {
                    CallbackData = $"{TelegramCallbackCommands.AddYourTranslation}_{wordId}",
                    Text = TelegramMessageTexts.AddYourTranslation
                }
            });
        }
    }
}
