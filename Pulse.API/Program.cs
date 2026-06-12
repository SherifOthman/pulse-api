using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.HttpOverrides;
using Pulse.API.DependencyInjections;
using Pulse.API.Infrastructure;
using Pulse.API.Persistence;
using Pulse.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Force Kestrel to listen on all interfaces so physical devices can reach the API
builder.WebHost.UseUrls("http://0.0.0.0:5170");

// Ensure wwwroot exists for file uploads
var webRoot = builder.Environment.WebRootPath
    ?? Path.Combine(builder.Environment.ContentRootPath, "wwwroot");
Directory.CreateDirectory(webRoot);

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddAuthorization();

builder.Services.AddEntityFrameworkDependencies(builder.Configuration);
builder.Services.AddJWTAuth(builder.Configuration);

builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddNewsServices(builder.Configuration);

// Trust the reverse proxy (runasp.net, etc.) so X-Forwarded-Proto is respected.
// This is required for UseHttpsRedirection and Secure cookies to work correctly
// when Kestrel sits behind an HTTPS-terminating proxy.
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    // Trust all proxies — safe for runasp.net shared hosting where we can't whitelist IPs
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

// CORS — allow the dashboard origin and mobile
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.SetIsOriginAllowed(_ => true)
              .AllowCredentials()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// MUST be first — so all subsequent middleware sees the correct scheme/host
app.UseForwardedHeaders();

app.UseAppExceptionMiddleware();

app.UseStaticFiles();

// Auto-apply pending migrations on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors();

app.UseAuthentication();

app.UseAuthorization();

app.MapEndpoints();

// Seed master data on startup
var seedScope = app.Services.CreateScope();
var seedDb = seedScope.ServiceProvider.GetRequiredService<AppDbContext>();
await GovernorateSeeder.SeedAsync(seedDb);
await CitySeeder.SeedAsync(seedDb);
await AdminSeeder.SeedAsync(app.Services);
seedScope.Dispose();

app.Run();
