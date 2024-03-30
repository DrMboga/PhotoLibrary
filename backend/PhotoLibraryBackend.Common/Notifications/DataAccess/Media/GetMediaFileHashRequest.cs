using MediatR;

namespace PhotoLibraryBackend.Common;

public record class GetMediaFileHashRequest(string FullPath): IRequest<string?>;
