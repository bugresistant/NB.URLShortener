using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NB.URLShortener.API.Models;
using NB.URLShortener.API.Services;

namespace NB.URLShortener.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UrlsController : ControllerBase
    {
        private readonly IUrlRepository _urlRepository;
        private readonly IUrlGenerator _slugGenerator;

        public UrlsController(IUrlRepository urlRepository, IUrlGenerator slugGenerator)
        {
            _urlRepository = urlRepository ?? throw new ArgumentNullException(nameof(urlRepository));
            _slugGenerator = slugGenerator ?? throw new ArgumentNullException(nameof(slugGenerator));
        }

        /// <summary>
        /// Shorten provided url if it is correct.
        /// </summary>
        /// <param name="request">The original url to shorten.</param>
        /// <response code="201">JSON with short url in body.</response>
        /// <response code="400">Bad request status code if url is not valid.</response>
        [HttpPost("shorten")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> ShortenUrl([FromBody] ShortenRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var slug = await _slugGenerator.GenerateUniqueSlugAsync();
            var normalizedUrl = NormalizeUrl(request.OriginalUrl);
            var shortenedUrl = new ShortUrl(normalizedUrl, slug);
            await _urlRepository.AddAsync(shortenedUrl);
            
            var shortUrl = $"{Request.Scheme}://{Request.Host}/r/{slug}";

            return CreatedAtAction(
                nameof(RedirectToOriginalUrl),
                new { slug },
                new { shortUrl }
            );
        }

        /// <summary>
        /// Redirects to original url using provided slug.
        /// </summary>
        /// <param name="slug">The unique identifier of the shortened url.</param>
        /// <response code="302">Redirection to original page if found.</response>
        /// <response code="404">Not found status code if slug is not found.</response>
        [HttpGet("/r/{slug}", Name = "RedirectRoute")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status302Found)]
        public async Task<IActionResult> RedirectToOriginalUrl(string slug)
        {
            var shortUrl = await _urlRepository.GetBySlugAsync(slug);
            if (shortUrl == null)
            {
                return NotFound();
            }
            
            await _urlRepository.IncrementClickCounterAsync(slug);
            return Redirect(shortUrl.OriginalUrl);
        }
        
        private string NormalizeUrl(string url)
        {
            if (!url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
                !url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                return "https://" + url;
            }

            return url;
        }
    }
}
