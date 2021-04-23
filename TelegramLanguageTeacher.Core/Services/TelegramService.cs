using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramLanguageTeacher.Core.Services
{
    public interface ITelegramService
    {
        Task<Update> GetUpdate(int lastUpdateId);
        Task SendTextMessage(int userId, string text);
        Task SendInlineButtonMessage(int userId, string text, InlineKeyboardMarkup markup);
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

        public async Task SendTextMessage(int userId, string text)
        {
            await _bot.SendTextMessageAsync(new ChatId(userId), text, ParseMode.Html);
        }

        public async Task SendInlineButtonMessage(int userId, string text, InlineKeyboardMarkup markup)
        {
            await _bot.SendTextMessageAsync(new ChatId(userId), text,
                replyMarkup: markup, parseMode: ParseMode.Markdown);
        }

        public async Task SetWebHook(string url)
        {
            await _bot.SetWebhookAsync(url);
        }
    }
}