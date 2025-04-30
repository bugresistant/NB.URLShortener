using System.Text;
using Microsoft.EntityFrameworkCore;
using NB.URLShortener.API.DbContexts;

namespace NB.URLShortener.API.Services;

public class SlugGenerator: IUrlGenerator
{
    private readonly AppDbContext _context;
    private const string VALID_CHARS = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_-";
    private const int DEFAULT_LENGTH = 6;
    private static readonly Random _random = new();

    public SlugGenerator(AppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }
    private string GenerateRandomSlug(int length)
    {
        var builder = new StringBuilder(length);
        for (int i = 0; i < length; i += 1)
        {
            var index = _random.Next(VALID_CHARS.Length);
            builder.Append(index);
        }

        return builder.ToString();
    }

    public async Task<string> GenerateUniqueSlugAsync()
    {
        string slug;
        // Checks whether the slug is unique. I don't know if it's the best implementation but at least it should work.
        do
        {
            slug = GenerateRandomSlug(DEFAULT_LENGTH);
        } while (await _context.ShortUrls.AnyAsync(s => s.Slug == slug));
        
        return slug;
    }
}