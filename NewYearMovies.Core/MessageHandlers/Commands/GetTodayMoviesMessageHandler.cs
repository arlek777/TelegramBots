using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Telegram.Bot.Types;
using TelegramBots.Common.MessageHandling;
using TelegramBots.Common.Services;
using TelegramBots.DataAccess;
using TelegramBots.DomainModels.NewYearMovies;

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
        private readonly ITelegramService<NewYearMoviesBot> _telegramService;
        private readonly IGenericRepository _repository;

        public GetTodayMoviesMessageHandler(ITelegramService<NewYearMoviesBot> telegramService, IGenericRepository repository)
        {
            _telegramService = telegramService;
            _repository = repository;
        }

        public async Task<bool> Handle(GetTodayMoviesMessageRequest request, CancellationToken cancellationToken)
        {
            Update update = request.Update;
            var userId = update.Message.From.Id;

            var todayDay = DateTime.Now.Day;

            var movies = await _repository.GetList<Movie>(m => m.Day == todayDay);

            if (movies == null || !movies.Any())
            {
                await _telegramService.SendTextMessage(userId, TelegramMessageTexts.NoTodayMovies);
            }
            else
            {
                string message = $"{TelegramMessageTexts.TodayMovie}\n\n";
                foreach (var movie in movies)
                {
                    message += $"{movie.Name}\n\n";
                }

                await _telegramService.SendTextMessage(userId, message);
            }

            return true;
        }
    }
}