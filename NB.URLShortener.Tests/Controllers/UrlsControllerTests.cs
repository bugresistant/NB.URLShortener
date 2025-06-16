using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NB.URLShortener.API.Controllers;
using NB.URLShortener.API.Models;
using NB.URLShortener.API.Services;

namespace NB.URLShortener.Tests.Controllers;

public class UrlsControllerTests
{
    private readonly Mock<IUrlRepository> _mockUrlRepository;
    private readonly Mock<IUrlGenerator> _mockSlugGenerator;
    private readonly UrlsController _controller;

    public UrlsControllerTests()
    {
        _mockUrlRepository = new Mock<IUrlRepository>();
        _mockSlugGenerator = new Mock<IUrlGenerator>();
        _controller = new UrlsController(_mockUrlRepository.Object, _mockSlugGenerator.Object);

        // Set up the controller context
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                Request =
                {
                    Scheme = "https",
                    Host = new HostString("example.com")
                }
            }
        };
    }

    [Fact]
    public async Task ShortenUrl_WithValidUrl_ReturnsCreatedResult()
    {
        // Arrange
        var request = new ShortenRequest { OriginalUrl = "https://example.com" };
        var expectedSlug = "abc123";
        _mockSlugGenerator.Setup(x => x.GenerateUniqueSlugAsync())
            .ReturnsAsync(expectedSlug);

        // Act
        var result = await _controller.ShortenUrl(request);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(UrlsController.RedirectToOriginalUrl), createdResult.ActionName);
        Assert.Equal(expectedSlug, createdResult.RouteValues["slug"]);

        // Verifying that the repository was called
        _mockUrlRepository.Verify(x => x.AddAsync(It.IsAny<ShortUrl>()), Times.Once);
    }

    [Fact]
    public async Task RedirectToOriginalUrl_WithValidSlug_ReturnsRedirectResult()
    {
        // Arrange
        var slug = "abc123";
        var originalUrl = "https://example.com";
        var shortUrl = new ShortUrl(originalUrl, slug);
        _mockUrlRepository.Setup(x => x.GetBySlugAsync(slug))
            .ReturnsAsync(shortUrl);

        // Act
        var result = await _controller.RedirectToOriginalUrl(slug);

        // Assert
        var redirectResult = Assert.IsType<RedirectResult>(result);
        Assert.Equal(originalUrl, redirectResult.Url);
    }

    [Fact]
    public async Task RedirectToOriginalUrl_WithInvalidSlug_ReturnsNotFound()
    {
        // Arrange
        var slug = "nonexistent";
        _mockUrlRepository.Setup(x => x.GetBySlugAsync(slug))
            .ReturnsAsync((ShortUrl)null);

        // Act
        var result = await _controller.RedirectToOriginalUrl(slug);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
}