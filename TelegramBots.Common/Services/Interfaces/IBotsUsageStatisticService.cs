using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramBots.DomainModels;

namespace TelegramBots.Common.Services.Interfaces;

public interface IBotsUsageStatisticService
{
    Task CheckAndTrackIfNewUserJoined(Update update, Type botType);

    Task<IEnumerable<BotsStatistic>> GetStats();
}