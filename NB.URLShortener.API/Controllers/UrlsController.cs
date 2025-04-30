using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        [HttpPost("shorten")]
        public async Task<IActionResult> ShortenUrl([FromBody] string originalUrl)
        {
            var slug = await _slugGenerator.GenerateUniqueSlugAsync();
            // HACK: should provide support for link expiration date
            var shortenedUrl = new ShortUrl(originalUrl, slug, null);
            _context.ShortUrls.Add(shortenedUrl);
            await _context.SaveChangesAsync();
            
            var shortUrl = $"{Request.Scheme}://{Request.Host}/r/{slug}";

            return Ok(new { shortUrl });
        }
        
    }
}
