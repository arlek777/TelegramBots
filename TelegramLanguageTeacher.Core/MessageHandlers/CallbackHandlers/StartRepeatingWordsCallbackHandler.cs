using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramLanguageTeacher.Core.Helpers;
using TelegramLanguageTeacher.Core.MessageHandlers.CommandHandlers;

namespace TelegramLanguageTeacher.Core.MessageHandlers.CallbackHandlers
{
    public class StartRepeatingWordsCallbackHandler : ITelegramMessageHandler
    {
        private readonly StartRepeatingWordsCommandMessageHandler _repeatingWordsCommand;

        public StartRepeatingWordsCallbackHandler(StartRepeatingWordsCommandMessageHandler repeatingWordsCommand)
        {
            _repeatingWordsCommand = repeatingWordsCommand;
        }

        public async Task<bool> Handle(Update update)
        {
            if (!update.IsUserCallback(TelegramCallbackCommands.StartRepeating))
                return false;

            var userId = update.CallbackQuery.From.Id;

            await _repeatingWordsCommand.Handle(new Update()
            {
                Message = new Message()
                {
                    Text = TelegramCommands.Repeat,
                    From = new User()
                    {
                        Id = userId,
                        IsBot = false
                    }
                }
            });

            return true;
        }
    }
}