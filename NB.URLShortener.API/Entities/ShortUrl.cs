using System.ComponentModel.DataAnnotations;

namespace NB.URLShortener.API.Models;

public class ShortUrl
{
    [Key]
    public int Id { get; init; } 
    
    [Required]
    [RegularExpression(@"^[a-zA-Z0-9_-]+$", 
        ErrorMessage = "Only letters, numbers, hyphens, and underscores allowed")]
    public string Slug { get; init; }
    
    [Required]
    [MaxLength(100, ErrorMessage = "Maximum length is 100 characters")]
    [Url(ErrorMessage = "Invalid URL format")]
    public string OriginalUrl { get; init; }
    [Required]
    public DateTimeOffset CreationDate { get; init; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? ExpirationDate { get; set; }

    [Required] public int ClickCounter { get; set; } = 0;

    public ShortUrl(string originalUrl, string slug, DateTimeOffset? expirationDate = null)
    {
        OriginalUrl = originalUrl;
        Slug = slug;
        ExpirationDate = expirationDate;
    }
}