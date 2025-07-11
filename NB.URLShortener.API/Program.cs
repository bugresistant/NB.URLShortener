using Microsoft.EntityFrameworkCore;
using NB.URLShortener.API.DbContexts;
using NB.URLShortener.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(dbContextOptions =>
    dbContextOptions.UseSqlite(
        builder.Configuration["ConnectionStrings:AppDBConnectionString"]));

builder.Services.AddScoped<IUrlGenerator, SlugGenerator>();
builder.Services.AddScoped<IUrlRepository, UrlRepository>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(setupAction =>
{
    var xmlCommentsFullPath = Path.Combine(AppContext.BaseDirectory, "NB.URLShortener.API.xml");
    setupAction.IncludeXmlComments(xmlCommentsFullPath);
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "URL Shortener API v1");
        c.RoutePrefix = "docs";
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseDefaultFiles(); // Serves index.html by default
app.UseStaticFiles();  // Serves /wwwroot content

app.Run();
