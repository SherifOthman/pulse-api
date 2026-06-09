namespace Pulse.API.Services;

public interface IFileService
{
    Task<string> SaveProfileImageAsync(IFormFile file);
    Task<string> SaveBusinessImageAsync(IFormFile file);
    Task DeleteFileAsync(string? filePath);
}
