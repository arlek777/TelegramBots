using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Telegram.Bot.Types;
using TelegramBots.Common.Extensions;
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
        private readonly ITelegramBotService<NewYearMoviesBot> _telegramService;
        private readonly IGenericRepository _repository;

        public GetTodayMoviesMessageHandler(ITelegramBotService<NewYearMoviesBot> telegramService, IGenericRepository repository)
        {
            _telegramService = telegramService;
            _repository = repository;
        }

        public async Task<bool> Handle(GetTodayMoviesMessageRequest request, CancellationToken cancellationToken)
        {
            Update update = request.Update;
            var userId = update.Message.From.Id;
            var startMessage = update.IsCommand(TelegramCommands.Start) ? TelegramMessageTexts.StartText : string.Empty;

            var todayDay = DateTime.Now.Day;

            var movies = (await _repository.GetList<Movie>(m => m.Day == todayDay))?.ToList();

            if (movies == null || !movies.Any())
            {
                await _telegramService.SendTextMessage(userId, TelegramMessageTexts.NoTodayMovies);
            }
            else
            {
                string moviesMessage = string
                    .Join("\n\n", movies.Select(m => $"<a href='https://www.kinopoisk.ru/film/48162/'>{m.Name}</a>").ToList());

                string message = $"{startMessage}{TelegramMessageTexts.TodayMovie}\n\n{moviesMessage}";

                await _telegramService.SendTextMessage(userId, message);
            }

            return true;
        }
    }
}