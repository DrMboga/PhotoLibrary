using MediatR;
using PhotoLibraryBackend.Common.Messages;

namespace PhotoLibraryBackend.Common;

public record ReadPreviousPhotosChunkRequest(double DateTo): IRequest<MediaInfo[]>;
