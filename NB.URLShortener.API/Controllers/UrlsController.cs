using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NB.URLShortener.API.DbContexts;
using NB.URLShortener.API.Models;
using NB.URLShortener.API.Services;

namespace NB.URLShortener.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UrlsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IUrlGenerator _slugGenerator;

        // I decided to keep the app simple, as it's quite small therefore I use DbContext directly, without repository.
        // Implementing repository here is a little bit overkill.
        // Maybe I'll reconsider my choice and do it later, in case if a code turns into a mess.
        public UrlsController(AppDbContext context, IUrlGenerator slugGenerator)
        {
            _context = context;
            _slugGenerator = slugGenerator;
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
            _context.ShortUrls.Add(shortenedUrl);
            await _context.SaveChangesAsync();
            
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
            var shortUrl = await _context.ShortUrls.FirstOrDefaultAsync(s => s.Slug == slug);
            if (shortUrl == null)
            {
                return NotFound();
            }
            
            var originalUrl = shortUrl.OriginalUrl;
            
            // Old way, not thread-safe
            // shortUrl.ClickCounter += 1;
            // await

            // Now thread-safe increment to ClickCounter property, as when a couple of people could click on link simultaneously
            // Not sure if it works because I'm not so advanced with LINQ yet and found the ready solution, but we'll find out eventually xD
            await _context.ShortUrls
                .Where(s => s.Slug == slug)
                .ExecuteUpdateAsync(s => 
                    s.SetProperty(p => p.ClickCounter, p => p.ClickCounter + 1));

            return Redirect(originalUrl);
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
