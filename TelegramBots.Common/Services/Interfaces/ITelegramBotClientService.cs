using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBots.Common.Services.Interfaces;

public interface ITelegramBotClientService<T> where T: ITelegramBot
{
    Task<Update> GetUpdate(int lastUpdateId);

    Task DeleteMessages(long userId, List<int> ids);

	Task<Message> SendTextMessage(long userId, string text, ParseMode parseMode = ParseMode.Html);

    Task<Message> SendInlineButtonMessage(long userId, string text, InlineKeyboardMarkup markup);

    Task SendAudioMessage(long userId, string audioLink, string name);

    Task<Message> SendImageMessage(long userId, InputOnlineFile photo);

	Task SetWebHook(string url);
}