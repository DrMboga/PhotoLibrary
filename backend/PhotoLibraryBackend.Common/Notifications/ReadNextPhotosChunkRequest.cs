using MediatR;
using PhotoLibraryBackend.Common.Messages;

namespace PhotoLibraryBackend.Common;

public record ReadNextPhotosChunkRequest(double DateFrom): IRequest<MediaInfo[]>;
