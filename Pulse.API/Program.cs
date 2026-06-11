using Microsoft.EntityFrameworkCore;
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

// CORS for mobile dev
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

app.UseAppExceptionMiddleware();

app.UseStaticFiles();

// Auto-apply migrations and create database on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
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
