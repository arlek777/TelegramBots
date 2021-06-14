//using System;
//using System.Threading.Tasks;
//using Telegram.Bot.Types;
//using Telegram.Bot.Types.ReplyMarkups;
//using TelegramLanguageTeacher.Core.MessageHandlers.PlainTextHandlers;

//namespace TelegramLanguageTeacher.Core.Services
//{
//    public interface ITelegramDailyMailer
//    {
//        Task Mail();
//    }

//    public class TelegramDailyMailer : ITelegramDailyMailer
//    {
//        private readonly ITelegramService _telegramService;
//        private readonly IUserService _userService;
//        private readonly TranslateAndAddWordMessageHandler _translateMessageHandler;

//        private static bool IsRepeatAlreadySent = false;
//        private static bool IsWordOfTheDayAlreadySent = false;

//        public TelegramDailyMailer(ITelegramService telegramService, IUserService userService, TranslateAndAddWordMessageHandler translateMessageHandler)
//        {
//            _telegramService = telegramService;
//            _userService = userService;
//            _translateMessageHandler = translateMessageHandler;
//        }

//        public async Task Mail()
//        {
//            var now = DateTime.UtcNow;
//            if (now.Hour >= LanguageTeacherConstants.TimeToRepeatUtcHour
//                && !IsRepeatAlreadySent)
//            {
//                var users = await _userService.GetAllUsers();

//                foreach (var user in users)
//                {
//                    try
//                    {
//                        await _telegramService.SendInlineButtonMessage(user.TelegramUserId,
//                            TelegramMessageTexts.RepeatReminderText,
//                            new InlineKeyboardMarkup(new InlineKeyboardButton()
//                            {
//                                CallbackData = $"{TelegramCallbackCommands.StartRepeating}_{user.TelegramUserId}",
//                                Text = TelegramMessageTexts.StartRepeating
//                            }));
//                    }
//                    catch (Exception e)
//                    {
//                    }
//                }

//                IsRepeatAlreadySent = true;
//            }
//            else if (now.Hour >= LanguageTeacherConstants.TimeToShowWordOfTheDayUtcHour
//                     && !IsWordOfTheDayAlreadySent)
//            {
//                var users = await _userService.GetAllUsers();

//                foreach (var user in users)
//                {
//                    try
//                    {
//                        if (!DailyWordsToMailing.WordsOfTheDays.ContainsKey(now.Day))
//                            break;

//                        await _telegramService.SendTextMessage(user.TelegramUserId,
//                            TelegramMessageTexts.WordOfTheDayText);

//                        await _translateMessageHandler.Handle(new Update()
//                        {
//                            Message = new Message()
//                            {
//                                Text = DailyWordsToMailing.WordsOfTheDays[now.Day],
//                                From = new User()
//                                {
//                                    FirstName = user.UserName,
//                                    Id = user.TelegramUserId,
//                                    IsBot = false
//                                }
//                            }
//                        });
//                    }
//                    catch (Exception e)
//                    {
//                    }
//                }

//                IsWordOfTheDayAlreadySent = true;
//            }
//        }
//    }
//}