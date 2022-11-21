using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TelegramBots.DataAccess;
using TelegramBots.DomainModels.LanguageTeacher;
using TelegramLanguageTeacher.Core.Extensions;
using TelegramLanguageTeacher.Core.Services.Interfaces;

namespace TelegramLanguageTeacher.Core.Services
{
    public class WordService : IWordService
    {
        private readonly IGenericRepository _repository;

        public WordService(IGenericRepository repository)
        {
            _repository = repository;
        }

        public async Task<Word> AddWord(long userId, Word word)
        {
            var user = await _repository.Find<User>(u => u.TelegramUserId == userId);
            if (user == null)
                return null;

            var defaultDict = await _repository.Find<Dict>(d => d.UserId == user.Id);

            if (defaultDict == null || word.Original == null)
            {
                throw new ArgumentException("Dictionary is not found or translation is null.");
            }

            var duplicatedWord = defaultDict.Words.Where(w => w?.Original != null).FirstOrDefault(
                w => w.Original.Equals(word.Original, StringComparison.InvariantCultureIgnoreCase));

            if (duplicatedWord != null)
                return duplicatedWord;

            var now = DateTime.UtcNow;
            word.DictId = defaultDict.Id;
            word.NextRepeat = now;
            word.AddedDate = now;
            word.LastRepeat = now;

            _repository.Add(word);

            await _repository.SaveChanges();

            return word;
        }

        public async Task RateWord(long userId, Guid wordId, int rate)
        {
            var user = await _repository.FindUserInclude(u => u.TelegramUserId == userId);
            var dbWord = user.Dicts.FirstOrDefault()?.Words.Where(w => w?.Original != null)
                .FirstOrDefault(w => w.Id == wordId);

            var now = DateTime.UtcNow;
            if (dbWord != null)
            {
                dbWord.Rate = rate;
                dbWord.LastRepeat = now;
                dbWord.NextRepeat = GetNextRepeatDateByRate(dbWord, rate, now);
                dbWord.RepeatCount += 1;
                await _repository.SaveChanges();
            }
        }

        public async Task<Word> GetWord(long userId, Guid wordId)
        {
            var user = await _repository.FindUserInclude(u => u.TelegramUserId == userId);
            var dbWord = user.Dicts.FirstOrDefault()?.Words.Where(w => w.Original != null)
                .FirstOrDefault(w => w.Id == wordId);

            return dbWord;
        }

        public async Task<Word> FindWordInUserDict(long userId, string word)
        {
            var existingUser = await _repository.Find<User>(u => u.TelegramUserId == userId);
            if (existingUser == null)
            {
                return null;
            }

            var user = await _repository.FindUserInclude(u => u.TelegramUserId == userId);
            var dbWord = user.Dicts.FirstOrDefault()?.Words
                .Where(w => w?.Original != null)
                .FirstOrDefault(w => w.Original.Equals(word, StringComparison.InvariantCultureIgnoreCase));

            return dbWord;
        }

        public async Task<CachedWord> GetWordFromCache(string original)
        {
            var word = await _repository.Find<CachedWord>(cw => cw.Original.Equals(original));
            return word;
        }

        public async Task<List<Word>> GetAllWords(long userId)
        {
            var user = await _repository.FindUserInclude(u => u.TelegramUserId == userId);
            var words = user.Dicts.FirstOrDefault()?.Words.Where(w => w.Original != null).OrderByDescending(w => w.AddedDate).ToList();

            return words;
        }

        public async Task RemoveWord(long userId, Guid wordId)
        {
            var word = await _repository.Find<Word>(w => w.Id == wordId);
            _repository.Remove(word);
            await _repository.SaveChanges();
        }

        public async Task RemoveAllWords(long userId)
        {
            var user = await _repository.FindUserInclude(u => u.TelegramUserId == userId);
            user.Dicts.FirstOrDefault()?.Words.Clear();

            await _repository.SaveChanges();
        }

        public async Task AddWordToCache(CachedWord cachedWord)
        {
            _repository.Add(cachedWord);
            await _repository.SaveChanges();
        }

        public async Task<int> GetTodayRepeatWordsCount(long userId)
        {
            var user = await _repository.FindUserInclude(u => u.TelegramUserId == userId);

            var wordCount = user?.Dicts.FirstOrDefault()?.Words.Where(w => w.Original != null).Count(IsWordToRepeatToday);

            return wordCount ?? 0;
        }

        public async Task<Word> GetNextWord(long userId)
        {
            var user = await _repository.FindUserInclude(u => u.TelegramUserId == userId);

            var word = user?.Dicts.FirstOrDefault()?.Words
                .Where(IsWordToRepeatToday)
                .OrderByDescending(w => w.NextRepeat)
                .ThenBy(w => w.Rate)
                .FirstOrDefault();

            return word;
        }

        private DateTime GetNextRepeatDateByRate(Word word, int rate, DateTime now)
        {
            if (word.RepeatCount >= 4 && rate == 3 && word.Rate >= 2)
            {
                return now.AddDays(60);
            }

            return word.RepeatCount < 1 || rate == 1 ? now.AddDays(1) : now.AddDays(word.RepeatCount * rate);
        }

        private bool IsWordToRepeatToday(Word word)
        {
            if (word.Original == null)
                return false;

            if (word.RepeatCount >= 6 && word.Rate == 3)
            {
                return false;
            }

            var now = DateTime.UtcNow;
            var dt = word.NextRepeat;
            var pastOrToday = new DateTime(now.Year, now.Month, now.Day) >= new DateTime(dt.Year, dt.Month, dt.Day);

            return pastOrToday;
        }
    }
}