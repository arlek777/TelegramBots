using System.Collections.Generic;
using System.Threading.Tasks;
using TelegramBots.DomainModels.NewYearMovies;

namespace NewYearMovies.Core.Services.Interfaces;

public interface IMoviesService
{
    Task<IList<Movie>> GetMoviesAsync();
}