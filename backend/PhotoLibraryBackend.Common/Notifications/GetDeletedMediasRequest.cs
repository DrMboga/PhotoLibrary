using MediatR;
using PhotoLibraryBackend.Common.Messages;

namespace PhotoLibraryBackend.Common;

public record GetDeletedMediasRequest(): IRequest<MediaInfo[]>;

