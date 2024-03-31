using MediatR;

namespace PhotoLibraryBackend.Common;

public record ReadVideoMetadataRequest(string FilePath): IRequest<VideoMetadata?>;
