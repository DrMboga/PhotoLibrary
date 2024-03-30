using MediatR;

namespace PhotoLibraryBackend.Common;

public record class GetPreviousPhotosChunkRequest(DateTime DateTo, int ChunkSize): IRequest<MediaFileInfo[]>;
