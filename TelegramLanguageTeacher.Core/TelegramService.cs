using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramLanguageTeacher.Core
{
    public interface ITelegramService
    {
        Task<Update> GetUpdate(int lastUpdateId);
        Task SendMessage(int username, string text);
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

        public async Task SendMessage(int username, string text)
        {
            await _bot.SendTextMessageAsync(new ChatId(username), text);
        }
    }
}