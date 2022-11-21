using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using NewYearMovies.Core.Services.Interfaces;
using TelegramBots.DataAccess;
using TelegramBots.DomainModels.NewYearMovies;

namespace NewYearMovies.Core.Services;

public class MoviesService : IMoviesService
{
    private const string MoviesCacheKey = "movies";
    private readonly TimeSpan _cacheExpiration = TimeSpan.FromDays(30);

    private readonly IMemoryCache _memoryCache;
    private readonly IGenericRepository _repository;

    public MoviesService(IMemoryCache memoryCache, IGenericRepository repository)
    {
        _memoryCache = memoryCache;
        _repository = repository;
    }

    public async Task<IList<Movie>> GetAsync()
    {
        List<Movie> movies;

        if(_memoryCache.TryGetValue(MoviesCacheKey, out string cachedMovies))
        {
            movies = JsonConvert.DeserializeObject<List<Movie>>(cachedMovies).ToList();
            return movies;
        }

        movies = (await _repository.GetAllAsync<Movie>())?.ToList();
        _memoryCache.Set(MoviesCacheKey, JsonConvert.SerializeObject(movies), _cacheExpiration);

        return movies;
    }
}
