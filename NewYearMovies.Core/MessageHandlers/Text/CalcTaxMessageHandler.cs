using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Telegram.Bot.Types;
using TelegramBots.Common.MessageHandling;
using TelegramBots.Common.Services;

namespace NewYearMovies.Core.MessageHandlers.Text
{
    public class CalcTaxMessageRequest : BaseRequest
    {
        public override bool AcceptUpdate(Update update)
        {
            Update = update;
            return update.IsTextMessage();
        }
    }

    public class CalcTaxMessageHandler : IRequestHandler<CalcTaxMessageRequest, bool>
    {
        private readonly ITelegramService<IndoTaxHelperBot> _telegramService;

        public CalcTaxMessageHandler(ITelegramService<IndoTaxHelperBot> telegramService)
        {
            _telegramService = telegramService;
        }

        public async Task<bool> Handle(CalcTaxMessageRequest request, CancellationToken cancellationToken)
        {
            Update update = request.Update;

            int userId = update.Message.From.Id;
            string messageText = update.Message.Text.Trim().ToLowerInvariant();

            const double tax = 0.1;

            try
            {
                double finalSum = 0;
                if (messageText.Contains(" "))
                {
                    double initialSum = double.Parse(messageText.Split(' ')[0]);
                    double serviceFee = double.Parse(messageText.Split(' ')[1]) / 100;

                    finalSum += initialSum + (initialSum * serviceFee);
                }
                else
                {
                    finalSum = double.Parse(messageText.Split(' ')[0]);
                }

                finalSum += finalSum * tax;

                await _telegramService.SendTextMessage(userId, finalSum.ToString("N"));
            }
            catch
            {
                await _telegramService.SendTextMessage(userId, "Invalid input data, please correct your numbers.");
            }

            return true;
        }
    }
}