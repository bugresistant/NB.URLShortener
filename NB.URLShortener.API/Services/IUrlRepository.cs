using NB.URLShortener.API.Models;

namespace NB.URLShortener.API.Services;

public interface IUrlRepository
{
    Task<ShortUrl?> GetBySlugAsync(string slug);
    Task<ShortUrl> AddAsync(ShortUrl shortUrl);
    Task IncrementClickCounterAsync(string slug);
} 