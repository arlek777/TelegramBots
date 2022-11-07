using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TelegramBots.DomainModels.LanguageTeacher;

namespace TelegramLanguageTeacher.Core.Services.Interfaces;

public interface IWordService
{
    Task<Word> AddWord(long userId, Word word);

    Task<Word> GetNextWord(long userId);

    Task<CachedWord> GetWordFromCache(string original);

    Task RateWord(long userId, Guid wordId, int rate);

    Task<Word> GetWord(long userId, Guid wordId);

    Task<Word> FindWordInUserDict(long userId, string word);

    Task<List<Word>> GetAllWords(long userId);

    Task RemoveWord(long userId, Guid wordId);

    Task RemoveAllWords(long userId);

    Task AddWordToCache(CachedWord cachedWord);

    Task<int> GetTodayRepeatWordsCount(long userId);
}