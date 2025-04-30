using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace NB.URLShortener.API.Models;

public class ShortenRequest
{
    [Required] public string OriginalUrl { get; set; } = string.Empty;
}