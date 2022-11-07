using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBots.Common.Services.Interfaces;

namespace TelegramBots.Common.Services
{
    public class TelegramBotClientService<T>: ITelegramBotClientService<T> where T : ITelegramBot
    {
        private readonly TelegramBotClient _bot;

        public TelegramBotClientService(T bot)
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

        public async Task<Message> SendTextMessage(long userId, string text, ParseMode parseMode = ParseMode.Html)
        {
            return await _bot.SendTextMessageAsync(new ChatId(userId), text, parseMode);
        }

        public async Task SendAudioMessage(long userId, string audioLink, string name)
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

        public async Task<Message> SendInlineButtonMessage(long userId, string text, InlineKeyboardMarkup markup)
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