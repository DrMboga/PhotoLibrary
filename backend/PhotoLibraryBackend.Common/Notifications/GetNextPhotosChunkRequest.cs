using MediatR;

namespace PhotoLibraryBackend.Common;

public record class GetNextPhotosChunkRequest(DateTime DateFrom, int ChunkSize): IRequest<MediaFileInfo[]>;
