using MediatR;

namespace PhotoLibraryBackend.Common;

public record GetPathOfConvertedHeicRequest(string OriginalFilePath): IRequest<string>;
