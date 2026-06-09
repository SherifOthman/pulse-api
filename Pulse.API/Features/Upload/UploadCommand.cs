using MediatR;

namespace Pulse.API.Features.Upload;

public record UploadCommand(IFormFile File) : IRequest<string>;
