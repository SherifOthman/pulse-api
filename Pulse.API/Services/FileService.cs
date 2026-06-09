namespace Pulse.API.Services;

public class FileService(IWebHostEnvironment env) : IFileService
{
    private const string ProfileDir = "uploads/profiles";
    private const string BusinessDir = "uploads/businesses";

    public Task<string> SaveProfileImageAsync(IFormFile file) =>
        SaveImageAsync(file, ProfileDir);

    public Task<string> SaveBusinessImageAsync(IFormFile file) =>
        SaveImageAsync(file, BusinessDir);

    private string WebRoot =>
        env.WebRootPath ?? Path.Combine(env.ContentRootPath, "wwwroot");

    private async Task<string> SaveImageAsync(IFormFile file, string subDir)
    {
        var dir = Path.Combine(WebRoot, subDir);
        Directory.CreateDirectory(dir);
        var fileName = $"{Guid.CreateVersion7()}_{file.FileName}";
        var path = Path.Combine(dir, fileName);
        await using var stream = new FileStream(path, FileMode.Create);
        await file.CopyToAsync(stream);
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
