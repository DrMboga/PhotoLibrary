using MediatR;

namespace PhotoLibraryBackend.Common;

public record class GetMediaListByAlbumDataBaseRequest(bool? IsFavorite, bool? IsImportant, bool? IsToPrint): IRequest<MediaFileInfo[]>;
