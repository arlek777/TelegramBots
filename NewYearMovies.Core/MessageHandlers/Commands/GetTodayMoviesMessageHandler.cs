using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Telegram.Bot.Types;
using TelegramBots.Common.Extensions;
using TelegramBots.Common.MessageHandling;
using TelegramBots.Common.MessageHandling.Requests;
using TelegramBots.Common.Services;
using TelegramBots.Common.Services.Interfaces;
using TelegramBots.DomainModels.NewYearMovies;

namespace NewYearMovies.Core.MessageHandlers.Commands
{
    public class GetTodayMoviesMessageRequest : BaseRequest
    {
        public bool IsDailySend { get; set; }

        public override bool CanHandle(Update update)
        {
            Update = update;
            return update.IsCommand(Core.Commands.GetTodayMovies) || update.IsCommand(Core.Commands.Start);
        }
    }

    public class GetTodayMoviesMessageHandler : IRequestHandler<GetTodayMoviesMessageRequest, bool>
    {
        private readonly ITelegramBotClientService<NewYearMoviesBot> _telegramService;

        public GetTodayMoviesMessageHandler(ITelegramBotClientService<NewYearMoviesBot> telegramService)
        {
            _telegramService = telegramService;
        }

        public async Task<bool> Handle(GetTodayMoviesMessageRequest request, CancellationToken cancellationToken)
        {
            Update update = request.Update;
            var userId = update.Message.From.Id;

            await SendMovies(userId, NewYearMoviesStore.Movies.Where(m => m.IsDecember).OrderBy(m => m.Day).ToList());
            await SendMovies(userId, NewYearMoviesStore.Movies.Where(m => !m.IsDecember).OrderBy(m => m.Day).ToList(), false);

            return true;

            var now = DateTime.UtcNow.AddHours(2); // new DateTime(2022, 12, 7);
            var isDecember = now.Month == 12;

            // Don't send movies for today if it's not start time yet
            if (update.IsCommand(Core.Commands.Start))
            {
                await _telegramService.SendTextMessage(userId, MessageTexts.StartText + MessageTexts.StartText2);

                if (now.TimeOfDay < Constants.DailyStart)
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
                if (!request.IsDailySend && !update.IsCommand(Core.Commands.Start))
                {
                    await _telegramService.SendTextMessage(userId, MessageTexts.NoTodayMovies);
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

            if (isDecember && MessageTexts.DecDailyMessages.ContainsKey(day))
            {
                await _telegramService.SendTextMessage(userId, MessageTexts.DecDailyMessages[day]);
            }
            else if (now.Month == 1 && MessageTexts.JanDailyMessages.ContainsKey(day))
            {
                await _telegramService.SendTextMessage(userId, MessageTexts.JanDailyMessages[day]);
            }
            else
            {
                var message = moviesCount > 1 ? MessageTexts.TodayMovies : MessageTexts.TodayMovie;
                await _telegramService.SendTextMessage(userId, $"{message}\n");
            }
        }

        private async Task SendMovies(long userId, List<Movie> movies, bool isDecember = true)
        {
            var month = isDecember ? "Грудня" : "Січня";
            foreach (var group in movies.GroupBy(m => m.Day))
            {
                var moviesTitles = new StringBuilder();
                foreach (var m in group)
                {
                    moviesTitles.AppendLine($"<a href='{m.Url}'>{m.Name}</a>");
                }

                await _telegramService.SendTextMessage(userId, $"{EmojiCodes.Snow} Фільми на {group.Key} {month} {EmojiCodes.Snow}\n{moviesTitles}");
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