using System;
using System.Linq;
using System.Threading.Tasks;
using TelegramLanguageTeacher.DataAccess;
using TelegramLanguageTeacher.DomainModels;

namespace TelegramLanguageTeacher.Core
{
    public interface IWordService
    {
        Task<bool> AddWord(int userId, Word word);
        Task<Word> GetNextWord(int userId);
        Task RateWord(int userId, string word, int rate);
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
            var now = DateTime.UtcNow;
            word.NextRepeat = now.AddDays(1);
            word.AddedDate = now;
            word.LastRepeat = now;

            var user = await _repository.Find<User>(u => u.TelegramUserId == userId);
            if (user == null)
                return false;

            user.Dicts.FirstOrDefault()?.Words.Add(word);

            await _repository.SaveChanges();

            return true;
        }

        public async Task RateWord(int userId, string word, int rate)
        {
            var user = await _repository.Find<User>(u => u.TelegramUserId == userId);
            var dbWord = user.Dicts.FirstOrDefault()?.Words
                .FirstOrDefault(w => w.Original.Equals(word, StringComparison.InvariantCultureIgnoreCase));

            var now = DateTime.UtcNow;
            if (dbWord != null)
            {
                dbWord.LastRepeat = now;
                dbWord.NextRepeat = GetNextRepeatDateByRate(dbWord, rate, now);
                dbWord.RepeatCount += 1;
                await _repository.SaveChanges();
            }
        }

        public async Task<Word> GetNextWord(int userId)
        {
            var user = await _repository.Find<User>(u => u.TelegramUserId == userId);

            var word = user?.Dicts.FirstOrDefault()?.Words.OrderByDescending(w => w.NextRepeat).FirstOrDefault();
            return word;
        }

        private DateTime GetNextRepeatDateByRate(Word word, int rate, DateTime now)
        {
            return word.RepeatCount < 1 || rate == 1 ? now.AddDays(1) : now.AddDays(word.RepeatCount * rate);
        }
    }
}