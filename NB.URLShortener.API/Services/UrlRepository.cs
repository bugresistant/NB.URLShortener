using Microsoft.EntityFrameworkCore;
using NB.URLShortener.API.DbContexts;
using NB.URLShortener.API.Models;

namespace NB.URLShortener.API.Services;

public class UrlRepository : IUrlRepository
{
    private readonly AppDbContext _context;

    public UrlRepository(AppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<ShortUrl?> GetBySlugAsync(string slug)
    {
        return await _context.ShortUrls.FirstOrDefaultAsync(s => s.Slug == slug);
    }

    public async Task<ShortUrl> AddAsync(ShortUrl shortUrl)
    {
        _context.ShortUrls.Add(shortUrl);
        await _context.SaveChangesAsync();
        return shortUrl;
    }

    public async Task IncrementClickCounterAsync(string slug)
    {
        await _context.ShortUrls
            .Where(s => s.Slug == slug)
            .ExecuteUpdateAsync(s => 
                s.SetProperty(p => p.ClickCounter, p => p.ClickCounter + 1));
    }
} 