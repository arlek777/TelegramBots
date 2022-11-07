using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using MediatR;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBots.Common.Extensions;
using TelegramBots.Common.MessageHandling;
using TelegramBots.Common.MessageHandling.Requests;
using TelegramBots.Common.Services;
using TelegramBots.Common.Services.Interfaces;

namespace NewYearMovies.Core.MessageHandlers.Commands
{
    public class GetMoviesMessageRequest : BaseRequest
    {
        public override bool CanHandle(Update update)
        {
            Update = update;
            return update.IsCommand(Core.Commands.GetAllMovies) || update.IsCallback(CallbackCommands.LoadNextMoviesPage);
        }
    }

    /// <summary>
    /// It supports both command and callback handler for get movies by pages.
    /// </summary>
    public class GetMoviesMessageHandler : IRequestHandler<GetMoviesMessageRequest, bool>
    {
        private readonly ITelegramBotClientService<NewYearMoviesBot> _telegramService;

        private const int PerPage = 8;

        public GetMoviesMessageHandler(ITelegramBotClientService<NewYearMoviesBot> telegramService)
        {
            _telegramService = telegramService;
        }

        public async Task<bool> Handle(GetMoviesMessageRequest request, CancellationToken cancellationToken)
        {
            Update update = request.Update;

            var isCommand = update.IsCommand(Core.Commands.GetAllMovies);
            var userId = isCommand
                ? update.Message.From.Id
                : update.CallbackQuery.From.Id;

            int page = isCommand ? 0 : int.Parse(update.CallbackQuery.Data.Split('_')[1]);

            var allMovies = NewYearMoviesStore.Movies;
            var sortedMovies = allMovies.OrderByDescending(m => m.Rating).ToList().Skip(page * PerPage).Take(PerPage).ToList();
            var nextPage = allMovies.Skip((page + 1) * PerPage).Take(PerPage).ToList();

            string message = string.Join("\n\n", sortedMovies.Select(m => $"<a href='https://www.google.com/search?q={HttpUtility.UrlEncode(m.Name + " дивитися онлайн на українській", Encoding.UTF8)}'>{m.Name}</a>").ToList());

            if (!nextPage.Any())
            {
                await _telegramService.SendTextMessage(userId, message);
                return true;
            }
           
            await _telegramService.SendInlineButtonMessage(userId, message, new InlineKeyboardMarkup(new InlineKeyboardButton("Завантажити більше")
            {
                CallbackData = CallbackCommands.LoadNextMoviesPage + (page + 1),
                Text = "Завантажити більше"
            }));

            return true;
        }
    }
}