using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramLanguageTeacher.DomainModels;

namespace TelegramLanguageTeacher.Core
{
    public interface ITelegramService
    {
        Task<Update> GetUpdate(int lastUpdateId);
        Task SendMessage(int userId, string text);
        Task SendMessageWithReplyButton(int userId, string text, Word word);
        Task SendMessageWithQualityButtons(int userId, string text, Word word);
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

        public async Task SendMessage(int userId, string text)
        {
            await _bot.SendTextMessageAsync(new ChatId(userId), text);
        }

        public async Task SendMessageWithReplyButton(int userId, string text, Word word)
        {
            var reply = new InlineKeyboardMarkup(new[]
            {
                new InlineKeyboardButton() { CallbackData = $"reply_{word.Id}", Text = "Show translation" }
            });

            await _bot.SendTextMessageAsync(new ChatId(userId), text,
                replyMarkup: reply, parseMode: ParseMode.Markdown);
        }

        public async Task SendMessageWithQualityButtons(int userId, string text, Word word)
        {
            var reply = new InlineKeyboardMarkup(new[]
            {
                new InlineKeyboardButton() { CallbackData = $"3_{word.Id}", Text = "Easy" },
                new InlineKeyboardButton() { CallbackData = $"2_{word.Id}", Text = "Good" },
                new InlineKeyboardButton() { CallbackData = $"1_{word.Id}", Text = "Bad" }
            });

            await _bot.SendTextMessageAsync(new ChatId(userId), text,
                replyMarkup: reply, parseMode: ParseMode.Markdown);
        }
    }
}