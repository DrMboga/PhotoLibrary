using MediatR;
using PhotoLibraryBackend.Common.Messages;

namespace PhotoLibraryBackend.Common;

public record class GetMediaListByAlbumRequest(bool? IsFavorite, bool? IsImportant, bool? IsToPrint): IRequest<MediaInfo[]>;

