using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using LemmaSharp;
using Telegram.Bot.Types.Enums;
using TelegramLanguageTeacher.Core;
using TelegramLanguageTeacher.Core._3rdPartyServices;
using TelegramLanguageTeacher.Core.Services;
using TelegramLanguageTeacher.DataAccess;
using TelegramLanguageTeacher.DomainModels;

namespace TelegramLanguageTeacher
{
    class Program
    {
        private static readonly ITelegramService TelegramService = new TelegramService("1716552741:AAFXAUHKsmdLP_P5JoQZ0YvvGjplRe5IScE");
        private static readonly ApplicationDbContext ApplicationDbContext = new ApplicationDbContext("Data Source=DESKTOP-56J8H8U;Initial Catalog=TelegramLanguageTeacher;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");

        private static readonly IGenericRepository Repository = new EntityFrameworkRepository(ApplicationDbContext);

        private static readonly IWordService WordService = new WordService(Repository);
        private static readonly IUserService UserService = new UserService(Repository);
        private static readonly ITranslatorService TranslatorService = new TranslatorService();

        private static int _lastUpdateId;

        static async Task Main(string[] args)
        {
            var dataFilepath = "Data/full7z-mlteast-en.lem";
            var stream = File.OpenRead(dataFilepath);
            var lemmatizer = new Lemmatizer(stream);

            while (true)
            {
                ApplicationDbContext.CreateDb();

                var update = await TelegramService.GetUpdate(_lastUpdateId);
                if (update == null)
                    continue;

                _lastUpdateId = update.Id + 1;
               
                var isTextToTranslate = update.Type == UpdateType.Message
                                        && update.Message.Type == MessageType.Text
                                        && !update.Message.From.IsBot
                                        && !update.Message.Text.Contains("/");

                var isCommand = update.Type == UpdateType.Message
                                        && update.Message.Type == MessageType.Text
                                        && !update.Message.From.IsBot
                                        && update.Message.Text.Contains("/");

                if (isTextToTranslate)
                {
                    var userId = update.Message.From.Id;

                    var result = lemmatizer.Lemmatize(update.Message.Text);

                    var translated = await TranslatorService.Translate(result);
                    if (!string.IsNullOrWhiteSpace(translated))
                    {
                        var wordModel = new Word() { Original = update.Message.Text, Translate = translated };

                        if (!await WordService.AddWord(userId, wordModel))
                        {
                            await UserService.CreateNewUser(new User()
                            {
                                UserName = update.Message.From.FirstName,
                                TelegramUserId = update.Message.From.Id
                            });

                            await WordService.AddWord(userId, wordModel);
                        }

                        await TelegramService.SendMessage(userId, translated);
                    }
                }
                else if (isCommand && update.Message.Text.Equals(TelegramCommands.StartLearn, StringComparison.InvariantCultureIgnoreCase))
                {
                    var userId = update.Message.From.Id;
                    var nextWord = await WordService.GetNextWord(userId);
                    if (nextWord != null)
                    {
                        await TelegramService.SendMessage(userId, TextConstants.StartLearningGreeting);
                        await TelegramService.SendMessageWithReplyButton(userId, nextWord.Original, nextWord);
                    }
                    else
                    {
                        await TelegramService.SendMessage(userId, TextConstants.EmptyVocabulary);
                    }
                }
                else if(update.Type == UpdateType.CallbackQuery)
                {
                    var userId = update.CallbackQuery.From.Id;

                    if (update.CallbackQuery.Data.Contains("reply"))
                    {
                        string[] callbackData = update.CallbackQuery.Data.Split('_');
                        Guid wordId = Guid.Parse(callbackData[1]);

                        var originalWord = await WordService.GetWord(userId, wordId);
                        await TelegramService.SendMessageWithQualityButtons(userId, originalWord.Translate, originalWord);
                    }
                    else if(update.CallbackQuery.Data.Contains("_"))
                    {
                        string[] callbackData = update.CallbackQuery.Data.Split('_');
                        Guid wordId = Guid.Parse(callbackData[1]);
                        int rate = int.Parse(callbackData[0]);

                        await WordService.RateWord(userId, wordId, rate);

                        var nextWord = await WordService.GetNextWord(userId);
                        if (nextWord != null)
                        {
                            await TelegramService.SendMessageWithReplyButton(userId, nextWord.Original, nextWord);
                        }
                        else
                        {
                            await TelegramService.SendMessage(userId, TextConstants.EmptyVocabulary);
                        }
                    }
                }
            }
        }
    }
}
