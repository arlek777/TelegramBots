﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramBots.Common.MessageHandling;
using TelegramBots.DataAccess;
using TelegramBots.DomainModels;

namespace TelegramBots.Common.Services
{
    public interface ITelegramBotsStatisticService
    {
        Task CheckAndTrackIfNewUserJoined(Update update, Type botType);
        Task<IEnumerable<BotsStatistic>> GetStats();
    }

    public class TelegramBotsStatisticService : ITelegramBotsStatisticService
    {
        private readonly IGenericRepository _genericRepository;

        public TelegramBotsStatisticService(IGenericRepository genericRepository)
        {
            _genericRepository = genericRepository;
        }

        public async Task CheckAndTrackIfNewUserJoined(Update update, Type botType)
        {
            if (!update.IsTextMessage())
                return;

            var existingStat = await _genericRepository.Find<BotsStatistic>(s => s.UserId == update.Message.From.Id);
            if (existingStat == null)
            {
                _genericRepository.Add(new BotsStatistic()
                {
                    BotType = botType.Name,
                    JoinedDate = DateTime.UtcNow,
                    UserId = update.Message.From.Id,
                    UserName = $"{update.Message.From.Username} {update.Message.From.FirstName} {update.Message.From.LastName}"
                });

                await _genericRepository.SaveChanges();
            }
        }

        public async Task<IEnumerable<BotsStatistic>> GetStats()
        {
            return await _genericRepository.GetAll<BotsStatistic>();
        }
    }
}