using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types.Enums;
using TelegramLanguageTeacher.Core;
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
            while (true)
            {
                ApplicationDbContext.CreateDb();

                var update = await TelegramService.GetUpdate(_lastUpdateId);
                if (update == null)
                    continue;

                _lastUpdateId = update.Id + 1;
                var userId = update.Message.From.Id;

                if (update.Type == UpdateType.Message && update.Message.Type == MessageType.Text && !update.Message.From.IsBot)
                {
                    var translated = await TranslatorService.Translate(update.Message.Text);
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
                else if(update.Type == UpdateType.CallbackQuery)
                {
                    var nextWord = await WordService.GetNextWord(userId);
                    if (nextWord != null)
                    {
                        await TelegramService.SendMessage(userId, nextWord.Original);
                    }
                    else
                    {
                        await TelegramService.SendMessage(userId, "You don't have any words for today.");
                    }
                }
            }
        }
    }
}
