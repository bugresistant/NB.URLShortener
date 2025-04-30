using Microsoft.EntityFrameworkCore;
using NB.URLShortener.API.Models;

namespace NB.URLShortener.API.DbContexts;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<ShortUrl> ShortUrls { get; set; }
}