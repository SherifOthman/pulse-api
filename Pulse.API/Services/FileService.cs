using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

namespace Pulse.API.Services;

public class FileService(IWebHostEnvironment env) : IFileService
{
    private const string ProfileDir = "uploads/profiles";
    private const string BusinessDir = "uploads/businesses";

    // Profile images: smaller max dimension, higher compression
    private const int ProfileMaxDimension = 400;
    private const int ProfileJpegQuality = 75;

    // Business images (profile + cover): wider, moderate compression
    private const int BusinessMaxDimension = 1200;
    private const int BusinessJpegQuality = 80;

    public Task<string> SaveProfileImageAsync(IFormFile file) =>
        SaveImageAsync(file, ProfileDir, ProfileMaxDimension, ProfileJpegQuality);

    public Task<string> SaveBusinessImageAsync(IFormFile file) =>
        SaveImageAsync(file, BusinessDir, BusinessMaxDimension, BusinessJpegQuality);

    private string WebRoot =>
        env.WebRootPath ?? Path.Combine(env.ContentRootPath, "wwwroot");

    private async Task<string> SaveImageAsync(
        IFormFile file,
        string subDir,
        int maxDimension,
        int jpegQuality)
    {
        var dir = Path.Combine(WebRoot, subDir);
        Directory.CreateDirectory(dir);

        // Always save as WebP for best compression; fall back to JPEG for GIFs
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        var isGif = ext == ".gif";
        var outputExt = isGif ? ".gif" : ".webp";
        var fileName = $"{Guid.NewGuid()}{outputExt}";
        var path = Path.Combine(dir, fileName);

        await using var inputStream = file.OpenReadStream();

        if (isGif)
        {
            // Save GIFs as-is to preserve animation
            await using var outStream = new FileStream(path, FileMode.Create);
            await inputStream.CopyToAsync(outStream);
        }
        else
        {
            using var image = await Image.LoadAsync(inputStream);

            // Resize if larger than the max dimension (preserves aspect ratio)
            if (image.Width > maxDimension || image.Height > maxDimension)
            {
                image.Mutate(ctx => ctx.Resize(new ResizeOptions
                {
                    Size = new Size(maxDimension, maxDimension),
                    Mode = ResizeMode.Max,
                }));
            }

            // Encode as WebP (lossy) for the best size-to-quality ratio
            var webpEncoder = new WebpEncoder
            {
                Quality = jpegQuality,
                FileFormat = WebpFileFormatType.Lossy,
            };

            await using var outStream = new FileStream(path, FileMode.Create);
            await image.SaveAsWebpAsync(outStream, webpEncoder);
        }

        return $"/{subDir}/{fileName}";
    }

    public Task DeleteFileAsync(string? filePath)
    {
        if (string.IsNullOrEmpty(filePath)) return Task.CompletedTask;
        var fullPath = Path.Combine(WebRoot, filePath.TrimStart('/'));
        if (File.Exists(fullPath)) File.Delete(fullPath);
        return Task.CompletedTask;
    }
}
