using Pulse.API.Services;
using MediatR;

namespace Pulse.API.Features.Upload;

public class UploadHandler(IFileService fileService) : IRequestHandler<UploadCommand, string>
{
    public async Task<string> Handle(UploadCommand request, CancellationToken ct)
    {
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
        var ext = Path.GetExtension(request.File.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(ext))
            throw new BadHttpRequestException("Only image files are allowed (jpg, jpeg, png, webp, gif)");

        return await fileService.SaveBusinessImageAsync(request.File);
    }
}
