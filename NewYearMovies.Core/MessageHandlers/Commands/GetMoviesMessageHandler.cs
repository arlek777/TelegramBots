using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBots.Common.MessageHandling;
using TelegramBots.Common.Services;
using TelegramBots.DataAccess;
using TelegramBots.DomainModels.NewYearMovies;

namespace NewYearMovies.Core.MessageHandlers.Commands
{
    public class GetMoviesMessageRequest : BaseRequest
    {
        public override bool AcceptUpdate(Update update)
        {
            Update = update;
            return update.IsCommand(TelegramCommands.GetAllMovies) || update.IsCallback(TelegramCallbackCommands.LoadNextMoviesPage);
        }
    }

    /// <summary>
    /// It supports both command and callback handler for get movies by pages.
    /// </summary>
    public class GetMoviesMessageHandler : IRequestHandler<GetMoviesMessageRequest, bool>
    {
        private readonly ITelegramService<NewYearMoviesBot> _telegramService;
        private readonly IGenericRepository _repository;

        public GetMoviesMessageHandler(ITelegramService<NewYearMoviesBot> telegramService, IGenericRepository repository)
        {
            _telegramService = telegramService;
            _repository = repository;
        }

        public async Task<bool> Handle(GetMoviesMessageRequest request, CancellationToken cancellationToken)
        {
            Update update = request.Update;

            var isCommand = update.IsCommand(TelegramCommands.GetAllMovies);
            var userId = isCommand
                ? update.Message.From.Id
                : update.CallbackQuery.From.Id;

            int page = isCommand ? 0 : int.Parse(update.CallbackQuery.Data.Split('_')[0]);

            var random = new Random();
            var movies = await _repository.GetAll<Movie>();
            movies = movies.OrderBy(m => random.Next()).Skip(page * 10).Take(10);
            var nextPage = movies.Skip(page * 10).Take(10);

            string message = string.Join("<br/>", movies);

            if (!nextPage.Any())
            {
                await _telegramService.SendTextMessage(userId, message);
            }
           
            await _telegramService.SendInlineButtonMessage(userId, message, new InlineKeyboardMarkup(new InlineKeyboardButton()
            {
                CallbackData = TelegramCallbackCommands.LoadNextMoviesPage + (page + 1),
                Text = "Загрузить больше"
            }));

            return true;
        }
    }
}