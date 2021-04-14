using System;
using System.Linq;
using System.Threading.Tasks;
using TelegramLanguageTeacher.DataAccess;
using TelegramLanguageTeacher.DomainModels;

namespace TelegramLanguageTeacher.Core.Services
{
    public interface IWordService
    {
        Task<bool> AddWord(int userId, Word word);
        Task<Word> GetNextWord(int userId);
        Task RateWord(int userId, Guid wordId, int rate);
        Task<Word> GetWord(int userId, Guid wordId);
    }

    public class WordService : IWordService
    {
        private readonly IGenericRepository _repository;

        public WordService(IGenericRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> AddWord(int userId, Word word)
        {
            var user = await _repository.Find<User>(u => u.TelegramUserId == userId);
            if (user == null)
                return false;

            var defaultDict = await _repository.Find<Dict>(d => d.UserId == user.Id);
            if (defaultDict == null)
                throw new ArgumentException("Dictionary is not found");

            var isDuplicateWork = defaultDict.Words.Any(
                w => w.Original.Equals(word.Original, StringComparison.InvariantCultureIgnoreCase));

            if (isDuplicateWork)
                return true;

            var now = DateTime.UtcNow;
            word.DictId = defaultDict.Id;
            word.NextRepeat = now;
            word.AddedDate = now;
            word.LastRepeat = now;

            _repository.Add(word);

            await _repository.SaveChanges();

            return true;
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

        public async Task<Word> GetNextWord(int userId)
        {
            var user = await _repository.FindUserInclude(u => u.TelegramUserId == userId);

            var word = user?.Dicts.FirstOrDefault()?.Words
                .Where(w => IsTodayDate(w.NextRepeat))
                .OrderBy(w => w.NextRepeat)
                .ThenBy(w => w.Rate)
                .FirstOrDefault();
            return word;
        }

        private bool IsTodayDate(DateTime dt)
        {
            var now = DateTime.UtcNow;
            return new DateTime(dt.Year, dt.Month, dt.Day) == new DateTime(now.Year, now.Month, now.Day);
        }

        private DateTime GetNextRepeatDateByRate(Word word, int rate, DateTime now)
        {
            return word.RepeatCount < 1 || rate == 1 ? now.AddDays(1) : now.AddDays(word.RepeatCount * rate);
        }
    }
}