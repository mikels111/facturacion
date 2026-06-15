using Facturacion.Server.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddEntityFrameworkSqlite().AddDbContext<AppDbContext>();
var WhiteListOrigins = "whiteListOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: WhiteListOrigins,
                      policy =>
                      {
                          policy.WithOrigins("https://localhost:56439")
                          .AllowCredentials()
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                      });
});

var app = builder.Build();
using (var client = new AppDbContext())
{
    client.Database.EnsureCreated();
    client.Database.Migrate();
}


app.UseDefaultFiles();
app.MapStaticAssets();

// Configure the HTTP request pipeline.
app.UseCors(WhiteListOrigins);

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
