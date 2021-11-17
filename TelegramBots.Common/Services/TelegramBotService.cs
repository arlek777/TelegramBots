using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBots.Common.Services
{
    public interface ITelegramBotService<T> where T: TelegramBotInstance
    {
        Task<Update> GetUpdate(int lastUpdateId);
        Task<Message> SendTextMessage(int userId, string text, ParseMode parseMode = ParseMode.Html);
        Task<Message> SendInlineButtonMessage(int userId, string text, InlineKeyboardMarkup markup);
        Task SendAudioMessage(int userId, string audioLink, string name);
        Task SetWebHook(string url);
    }

    public class TelegramBotService<T>: ITelegramBotService<T> where T : TelegramBotInstance
    {
        private readonly TelegramBotClient _bot;

        public TelegramBotService(T bot)
        {
            _bot = new TelegramBotClient(bot.Token);
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

        public async Task<Message> SendTextMessage(int userId, string text, ParseMode parseMode = ParseMode.Html)
        {
            return await _bot.SendTextMessageAsync(new ChatId(userId), text, parseMode);
        }

        public async Task SendAudioMessage(int userId, string audioLink, string name)
        {
            try
            {
                var file = await new HttpClient().GetStreamAsync(audioLink.Replace("//", "https://"));
                await _bot.SendAudioAsync(new ChatId(userId), new InputOnlineFile(file, name), caption: "\U0001F3A7",
                    disableNotification: true);
            }
            catch (Exception e)
            {
                
            }
        }

        public async Task<Message> SendInlineButtonMessage(int userId, string text, InlineKeyboardMarkup markup)
        {
            return await _bot.SendTextMessageAsync(new ChatId(userId), text,
                replyMarkup: markup, parseMode: ParseMode.Html);
        }

        public async Task SetWebHook(string url)
        {
            await _bot.SetWebhookAsync(url);
        }
    }
}