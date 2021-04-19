using System;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramLanguageTeacher.DomainModels;

namespace TelegramLanguageTeacher.Core.Services
{
    public interface ITelegramService
    {
        Task<Update> GetUpdate(int lastUpdateId);
        Task SendPlanTextMessage(int userId, string text);
        Task SendMessageTranslateButton(int userId, string text, Word word);
        Task SendRateButtonsMessage(int userId, string text, Word word);
        Task SendRemoveButtonMessage(int userId, string text, Word word);
        Task SetWebHook(string url);
    }

    public class TelegramService : ITelegramService
    {
        private readonly TelegramBotClient _bot;

        public TelegramService(string token)
        {
            _bot = new TelegramBotClient(token);
        }

        public async Task<Update> GetUpdate(int lastUpdateId)
        {
            var updates = await _bot.GetUpdatesAsync(timeout: 2000, offset: lastUpdateId);
            return updates.Where(u => (u.Type == UpdateType.Message
                                      && u.Message.Type == MessageType.Text
                                      && !u.Message.From.IsBot) || u.Type == UpdateType.CallbackQuery)
                .OrderByDescending(u => u.Id)
                .FirstOrDefault();
        }

        public async Task SendPlanTextMessage(int userId, string text)
        {
            await _bot.SendTextMessageAsync(new ChatId(userId), text, ParseMode.Html);
        }

        public async Task SendMessageTranslateButton(int userId, string text, Word word)
        {
            var reply = new InlineKeyboardMarkup(new[]
            {
                new InlineKeyboardButton()
                {
                    CallbackData = $"{TelegramCallbackCommands.ShowTranslate}_{word.Id}", 
                    Text = TelegramMessageTexts.ShowTranslation
                }
            });

            await _bot.SendTextMessageAsync(new ChatId(userId), text,
                replyMarkup: reply, parseMode: ParseMode.Markdown);
        }

        public async Task SendRemoveButtonMessage(int userId, string text, Word word)
        {
            var reply = new InlineKeyboardMarkup(new[]
            {
                new InlineKeyboardButton()
                {
                    CallbackData = $"{TelegramCallbackCommands.RemoveWord}_{word.Id}",
                    Text = TelegramMessageTexts.RemoveWord
                }
            });

            await _bot.SendTextMessageAsync(new ChatId(userId), text,
                replyMarkup: reply, parseMode: ParseMode.Markdown);
        }

        public async Task SendRateButtonsMessage(int userId, string text, Word word)
        {
            var reply = new InlineKeyboardMarkup(new[]
            {
                new InlineKeyboardButton() { CallbackData = FormatCallbackRateData(0, word.Id), Text = TelegramMessageTexts.RemoveWord },
                new InlineKeyboardButton() { CallbackData = FormatCallbackRateData(1, word.Id), Text = TelegramMessageTexts.HardRate },
                new InlineKeyboardButton() { CallbackData = FormatCallbackRateData(2, word.Id), Text = TelegramMessageTexts.NormalRate  },
                new InlineKeyboardButton() { CallbackData = FormatCallbackRateData(3, word.Id), Text = TelegramMessageTexts.EasyRate }
            });

            await _bot.SendTextMessageAsync(new ChatId(userId), text,
                replyMarkup: reply, parseMode: ParseMode.Markdown);
        }

        public async Task SetWebHook(string url)
        {
            await _bot.SetWebhookAsync(url);
        }

        private string FormatCallbackRateData(int index, Guid wordId)
        {
            return $"{TelegramCallbackCommands.Rate}_{index}_{wordId}";
        }
    }
}