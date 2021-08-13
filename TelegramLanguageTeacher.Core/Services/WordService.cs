using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TelegramBots.DataAccess;
using TelegramBots.DataAccess.LanguageTeacher;
using TelegramBots.DomainModels;
using TelegramBots.DomainModels.LanguageTeacher;

namespace TelegramLanguageTeacher.Core.Services
{
    public interface IWordService
    {
        Task<Word> AddWord(int userId, Word word);
        Task<Word> GetNextWord(int userId);
        Task<CachedWord> GetWordFromCache(string original);
        Task RateWord(int userId, Guid wordId, int rate);
        Task<Word> GetWord(int userId, Guid wordId);
        Task<Word> FindWordInUserDict(int userId, string word);
        Task<List<Word>> GetAllWords(int userId);
        Task RemoveWord(int userId, Guid wordId);
        Task RemoveAllWords(int userId);
        Task AddWordToCache(CachedWord cachedWord);
        Task<int> GetTodayRepeatWordsCount(int userId);
    }

    public class WordService : IWordService
    {
        private readonly IGenericRepository _repository;

        public WordService(IGenericRepository repository)
        {
            _repository = repository;
        }

        public async Task<Word> AddWord(int userId, Word word)
        {
            var user = await _repository.Find<User>(u => u.TelegramUserId == userId);
            if (user == null)
                return null;

            var defaultDict = await _repository.Find<Dict>(d => d.UserId == user.Id);
            if (defaultDict == null || word.Original == null)
                throw new ArgumentException("Dictionary is not found or translation is null");

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

        public async Task RateWord(int userId, Guid wordId, int rate)
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

        public async Task<Word> GetWord(int userId, Guid wordId)
        {
            var user = await _repository.FindUserInclude(u => u.TelegramUserId == userId);
            var dbWord = user.Dicts.FirstOrDefault()?.Words.Where(w => w.Original != null)
                .FirstOrDefault(w => w.Id == wordId);

            return dbWord;
        }

        public async Task<Word> FindWordInUserDict(int userId, string word)
        {
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

        public async Task<List<Word>> GetAllWords(int userId)
        {
            var user = await _repository.FindUserInclude(u => u.TelegramUserId == userId);
            var words = user.Dicts.FirstOrDefault()?.Words.Where(w => w.Original != null).OrderByDescending(w => w.AddedDate).ToList();

            return words;
        }

        public async Task RemoveWord(int userId, Guid wordId)
        {
            var word = await _repository.Find<Word>(w => w.Id == wordId);
            _repository.Remove(word);
            await _repository.SaveChanges();
        }

        public async Task RemoveAllWords(int userId)
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

        public async Task<int> GetTodayRepeatWordsCount(int userId)
        {
            var user = await _repository.FindUserInclude(u => u.TelegramUserId == userId);

            var wordCount = user?.Dicts.FirstOrDefault()?.Words.Where(w => w.Original != null).Count(IsWordToRepeatToday);

            return wordCount ?? 0;
        }

        public async Task<Word> GetNextWord(int userId)
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