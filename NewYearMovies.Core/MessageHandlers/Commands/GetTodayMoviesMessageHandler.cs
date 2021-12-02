using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Telegram.Bot.Types;
using TelegramBots.Common.Extensions;
using TelegramBots.Common.MessageHandling;
using TelegramBots.Common.Services;
using TelegramBots.DomainModels.NewYearMovies;

namespace NewYearMovies.Core.MessageHandlers.Commands
{
    public class GetTodayMoviesMessageRequest : BaseRequest
    {
        public bool IsDailySend { get; set; }

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

            var now = DateTime.UtcNow.AddHours(2); // new DateTime(2022, 12, 7);
            var isDecember = now.Month == 12;

            // Don't send movies for today if it's not start time yet
            if (update.IsCommand(TelegramCommands.Start))
            {
                await _telegramService.SendTextMessage(userId, TelegramMessageTexts.StartText + TelegramMessageTexts.StartText2);

                if (now.TimeOfDay < NewYearMoviesBotConfig.DailyStart)
                {
                    return true;
                }
            }

            var movies = isDecember
                ? NewYearMoviesStore.Movies.Where(m => m.Day == now.Day && m.IsDecember).ToList()
                : NewYearMoviesStore.Movies.Where(m => m.Day == now.Day && !m.IsDecember).ToList();

            // No movies
            if (!movies.Any())
            {
                // Don't send empty message if it's daily sending and not time yet
                if (!request.IsDailySend && !update.IsCommand(TelegramCommands.Start))
                {
                    await _telegramService.SendTextMessage(userId, TelegramMessageTexts.NoTodayMovies);
                }

                return true;
            }

            await SendMovies(userId, movies);
            await SendGreetings(userId, movies.Count, now, isDecember);

            return true;
        }

        private async Task SendGreetings(long userId, int moviesCount, DateTime now, bool isDecember)
        {
            var day = now.Day;

            if (isDecember && TelegramMessageTexts.DecDailyMessages.ContainsKey(day))
            {
                await _telegramService.SendTextMessage(userId, TelegramMessageTexts.DecDailyMessages[day]);
            }
            else if (now.Month == 1 && TelegramMessageTexts.JanDailyMessages.ContainsKey(day))
            {
                await _telegramService.SendTextMessage(userId, TelegramMessageTexts.JanDailyMessages[day]);
            }
            else
            {
                var message = moviesCount > 1 ? TelegramMessageTexts.TodayMovies : TelegramMessageTexts.TodayMovie;
                await _telegramService.SendTextMessage(userId, $"{message}\n\n");
            }
        }

        private async Task SendMovies(long userId, List<Movie> movies)
        {
            foreach (var m in movies)
            {
                await _telegramService.SendTextMessage(userId, $"<a href='{m.Url}'>{m.Name}</a>");
            }
        }

        private async Task SendMoviesInOneMessage(long userId, List<Movie> movies)
        {
            string mess = "";
            foreach (var m in movies)
            {
                mess += $"<a href='{m.Url}'>{m.Name}</a>\n\n";
            }
            await _telegramService.SendTextMessage(userId, mess);
        }
    }
}