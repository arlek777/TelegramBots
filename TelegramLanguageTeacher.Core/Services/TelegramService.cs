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

        public async Task SendMessageWithReplyButton(int userId, string text, Word word)
        {
            var reply = new InlineKeyboardMarkup(new[]
            {
                new InlineKeyboardButton() { CallbackData = $"{TelegramCommands.ShowTranslate}_{word.Id}", Text = TelegramMessageTexts.ShowTranslation }
            });

            await _bot.SendTextMessageAsync(new ChatId(userId), text,
                replyMarkup: reply, parseMode: ParseMode.Markdown);
        }

        public async Task SendMessageWithQualityButtons(int userId, string text, Word word)
        {
            var reply = new InlineKeyboardMarkup(new[]
            {
                new InlineKeyboardButton() { CallbackData = $"{TelegramCommands.Rate}_1_{word.Id}", Text = TelegramMessageTexts.HardRate },
                new InlineKeyboardButton() { CallbackData = $"{TelegramCommands.Rate}_2_{word.Id}", Text = TelegramMessageTexts.NormalRate  },
                new InlineKeyboardButton() { CallbackData = $"{TelegramCommands.Rate}_3_{word.Id}", Text = TelegramMessageTexts.EasyRate }
            });

            await _bot.SendTextMessageAsync(new ChatId(userId), text,
                replyMarkup: reply, parseMode: ParseMode.Markdown);
        }
    }
}