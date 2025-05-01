using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace NB.URLShortener.API.Models;

/// <summary>
/// Wrapper model for url to shorten
/// </summary>
public class ShortenRequest
{
    /// <summary>
    /// Url to shorten stored in property
    /// </summary>
    [Required] public string OriginalUrl { get; set; }
}