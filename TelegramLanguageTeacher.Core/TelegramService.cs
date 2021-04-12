using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramLanguageTeacher.Core
{
    public interface ITelegramService
    {
        Task<Update> GetUpdate(int lastUpdateId);
        Task SendMessage(int username, string text, bool addReplyButton);
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
            return updates.OrderByDescending(u => u.Id).FirstOrDefault();
        }

        public async Task SendMessage(int username, string text, bool addReplyButton)
        {
            if (addReplyButton)
            {
                await _bot.SendTextMessageAsync(new ChatId(username), text,
                    replyMarkup: new ReplyKeyboardMarkup(new[]
                    {
                        new KeyboardButton("Stop Repeating")
                    }));
            }
            else
            {
                await _bot.SendTextMessageAsync(new ChatId(username), text,
                    replyMarkup: new ReplyKeyboardRemove());
            }
        }
    }
}