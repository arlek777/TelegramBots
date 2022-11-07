using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBots.Common.Services.Interfaces;

public interface ITelegramBotClientService<T> where T: ITelegramBot
{
    Task<Update> GetUpdate(int lastUpdateId);

    Task<Message> SendTextMessage(long userId, string text, ParseMode parseMode = ParseMode.Html);

    Task<Message> SendInlineButtonMessage(long userId, string text, InlineKeyboardMarkup markup);

    Task SendAudioMessage(long userId, string audioLink, string name);

    Task SetWebHook(string url);
}