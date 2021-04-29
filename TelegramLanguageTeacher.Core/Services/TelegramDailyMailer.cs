using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramLanguageTeacher.Core.MessageHandlers.PlainTextHandlers;

namespace TelegramLanguageTeacher.Core.Services
{
    public interface ITelegramDailyMailer
    {
        Task Mail();
    }

    public class TelegramDailyMailer : ITelegramDailyMailer
    {
        private readonly ITelegramService _telegramService;
        private readonly IUserService _userService;
        private readonly TranslateAndAddWordMessageHandler _translateMessageHandler;

        public TelegramDailyMailer(ITelegramService telegramService, IUserService userService, TranslateAndAddWordMessageHandler translateMessageHandler)
        {
            _telegramService = telegramService;
            _userService = userService;
            _translateMessageHandler = translateMessageHandler;
        }

        public async Task Mail()
        {
            var now = DateTime.UtcNow;
            if (now.Hour == CommonConstants.TimeToRepeatUtcHour)
            {
                var users = await _userService.GetAllUsers();

                foreach (var user in users)
                {
                    await _telegramService.SendInlineButtonMessage(user.TelegramUserId,
                        TelegramMessageTexts.RepeatReminderText,
                        new InlineKeyboardMarkup(new InlineKeyboardButton()
                        {
                            CallbackData = $"{TelegramCallbackCommands.StartRepeating}_{user.TelegramUserId}",
                            Text = TelegramMessageTexts.StartRepeating
                        }));
                }
            }
            else if (now.Hour == CommonConstants.TimeToShowWordOfTheDayUtcHour)
            {
                var users = await _userService.GetAllUsers();

                foreach (var user in users)
                {
                    if (!DailyWordsToMailing.WordsOfTheDays.ContainsKey(now.Day))
                        break;

                    await _telegramService.SendTextMessage(user.TelegramUserId, TelegramMessageTexts.WordOfTheDayText);

                    await _translateMessageHandler.Handle(new Update()
                    {
                        Message = new Message()
                        {
                            Text = DailyWordsToMailing.WordsOfTheDays[now.Day],
                            From = new User()
                            {
                                FirstName = user.UserName,
                                Id = user.TelegramUserId,
                                IsBot = false
                            }
                        }
                    });
                }
            }
        }
    }
}