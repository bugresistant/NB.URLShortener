namespace NB.URLShortener.API.Services;

public interface IUrlGenerator
{
    Task<string> GenerateUniqueSlugAsync();
}