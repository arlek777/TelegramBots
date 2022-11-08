using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using NewYearMovies.Core.Services.Interfaces;
using Telegram.Bot.Types;
using TelegramBots.Common.Extensions;
using TelegramBots.Common.MessageHandling.Requests;
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
        private readonly IMoviesService _moviesService;

        public GetTodayMoviesMessageHandler(ITelegramBotClientService<NewYearMoviesBot> telegramService, IMoviesService moviesService)
        {
            _telegramService = telegramService;
            _moviesService = moviesService;
        }

        public async Task<bool> Handle(GetTodayMoviesMessageRequest request, CancellationToken cancellationToken)
        {
            Update update = request.Update;
            var userId = update.Message.From.Id;

            var movies = await _moviesService.GetMoviesAsync();

            await SendMovies(userId, movies.Where(m => !m.IsDecember).OrderByDescending(m => m.Day).ToList(), false);
            await SendMovies(userId, movies.Where(m => m.IsDecember).OrderByDescending(m => m.Day).ToList());

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

            movies = isDecember
                ? movies.Where(m => m.Day == now.Day && m.IsDecember).ToList()
                : movies.Where(m => m.Day == now.Day && !m.IsDecember).ToList();

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

        private async Task SendMovies(long userId, IList<Movie> movies, bool isDecember = true)
        {
            var month = isDecember ? MessageTexts.DecemberMonth : MessageTexts.JanuaryMonth;
            foreach (var group in movies.GroupBy(m => m.Day).DistinctBy(m => m.Key))
            {
                var moviesTitles = new StringBuilder();
                foreach (var m in group)
                {
                    moviesTitles.AppendLine($"{EmojiCodes.XTree} <a href='{m.Url}'>{m.Name}</a> {EmojiCodes.XTree}");
                }

                await _telegramService.SendTextMessage(userId,
                    $"{EmojiCodes.Snow} {MessageTexts.MoviesText} {group.Key} {month} {EmojiCodes.Snow}\n\n{moviesTitles}");
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