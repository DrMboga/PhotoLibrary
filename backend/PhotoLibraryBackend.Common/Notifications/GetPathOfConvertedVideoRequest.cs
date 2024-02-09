using MediatR;

namespace PhotoLibraryBackend.Common;

public record GetPathOfConvertedVideoRequest(string OriginalFilePath): IRequest<string>;
