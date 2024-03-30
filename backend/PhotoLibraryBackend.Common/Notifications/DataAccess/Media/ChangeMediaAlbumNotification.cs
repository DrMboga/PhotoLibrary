using MediatR;

namespace PhotoLibraryBackend.Common;

public record class ChangeMediaAlbumNotification(long MediaId, bool? IsFavorite, bool? IsImportant, bool? IsToPrint): INotification;
