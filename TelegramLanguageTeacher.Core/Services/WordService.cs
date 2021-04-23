using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TelegramLanguageTeacher.DataAccess;
using TelegramLanguageTeacher.DomainModels;

namespace TelegramLanguageTeacher.Core.Services
{
    public interface IWordService
    {
        Task<Word> AddWord(int userId, Word word);
        Task<Word> GetNextWord(int userId);
        Task RateWord(int userId, Guid wordId, int rate);
        Task<Word> GetWord(int userId, Guid wordId);
        Task<List<Word>> GetAllWords(int userId);
        Task RemoveWord(int userId, Guid wordId);
        Task RemoveAllWords(int userId);
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
            if (defaultDict == null)
                throw new ArgumentException("Dictionary is not found");

            var duplicatedWord = defaultDict.Words.FirstOrDefault(
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
            var dbWord = user.Dicts.FirstOrDefault()?.Words
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
            var dbWord = user.Dicts.FirstOrDefault()?.Words
                .FirstOrDefault(w => w.Id == wordId);

            return dbWord;
        }

        public async Task<List<Word>> GetAllWords(int userId)
        {
            var user = await _repository.FindUserInclude(u => u.TelegramUserId == userId);
            var words = user.Dicts.FirstOrDefault()?.Words.OrderByDescending(w => w.AddedDate).ToList();

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

        public async Task<Word> GetNextWord(int userId)
        {
            var user = await _repository.FindUserInclude(u => u.TelegramUserId == userId);

            var word = user?.Dicts.FirstOrDefault()?.Words
                .Where(w => IsTodayOrPastDate(w.NextRepeat))
                .OrderBy(w => w.NextRepeat)
                .ThenBy(w => w.Rate)
                .FirstOrDefault();
            return word;
        }

        private DateTime GetNextRepeatDateByRate(Word word, int rate, DateTime now)
        {
            return word.RepeatCount < 1 || rate == 1 ? now.AddDays(1) : now.AddDays(word.RepeatCount * rate);
        }

        private bool IsTodayOrPastDate(DateTime dt)
        {
            var now = DateTime.UtcNow;
            return new DateTime(now.Year, now.Month, now.Day) >= new DateTime(dt.Year, dt.Month, dt.Day);
        }
    }
}