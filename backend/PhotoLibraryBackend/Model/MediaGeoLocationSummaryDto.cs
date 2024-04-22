namespace PhotoLibraryBackend;

public record class MediaGeoLocationSummaryDto(string Region, string Country, int MediasCount, int? RandomPhotoThumbnailWidth, int? RandomPhotoThumbnailHeight, byte[]? Thumbnail);
