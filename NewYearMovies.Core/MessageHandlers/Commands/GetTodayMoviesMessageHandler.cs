using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Telegram.Bot.Types;
using TelegramBots.Common.Extensions;
using TelegramBots.Common.MessageHandling;
using TelegramBots.Common.Services;

namespace NewYearMovies.Core.MessageHandlers.Commands
{
    public class GetTodayMoviesMessageRequest : BaseRequest
    {
        public override bool AcceptUpdate(Update update)
        {
            Update = update;
            return update.IsCommand(TelegramCommands.GetTodayMovies) || update.IsCommand(TelegramCommands.Start);
        }
    }

    public class GetTodayMoviesMessageHandler : IRequestHandler<GetTodayMoviesMessageRequest, bool>
    {
        private readonly ITelegramBotService<NewYearMoviesBot> _telegramService;

        public GetTodayMoviesMessageHandler(ITelegramBotService<NewYearMoviesBot> telegramService)
        {
            _telegramService = telegramService;
        }

        public async Task<bool> Handle(GetTodayMoviesMessageRequest request, CancellationToken cancellationToken)
        {
            Update update = request.Update;
            var userId = update.Message.From.Id;
            var startMessage = update.IsCommand(TelegramCommands.Start) ? TelegramMessageTexts.StartText : string.Empty;

            var today = DateTime.UtcNow.AddHours(2);
            var todayDay = today.Day;

            var movies = NewYearMoviesStore.Movies.Where(m => m.Day == todayDay).ToList();

            if (!movies.Any())
            {
                await _telegramService.SendTextMessage(userId, TelegramMessageTexts.NoTodayMovies);
                return true;
            }

            foreach (var m in movies)
            {
                await _telegramService.SendTextMessage(userId, $"<a href='{m.Url}'>{m.Name}</a>");
            }

            if (TelegramMessageTexts.DailyMessages.ContainsKey(todayDay))
            {
                await _telegramService.SendTextMessage(userId, TelegramMessageTexts.DailyMessages[todayDay]);
            }

            await _telegramService.SendTextMessage(userId, $"{startMessage}{TelegramMessageTexts.TodayMovie}\n\n");

            return true;
        }
    }
}