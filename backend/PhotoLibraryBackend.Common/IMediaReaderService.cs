using PhotoLibraryBackend.Common.Messages;

namespace PhotoLibraryBackend.Common;

public interface IMediaReaderService
{
    /// <summary>
    /// Returns a bunch of photos after the date from
    /// </summary>
    Task<MediaInfo[]> GetNextPhotosChunk(double dateFrom);

    /// <summary>
    /// Returns a bunch of photos before the date to
    /// </summary>
    Task<MediaInfo[]> GetPreviousPhotosChunk(double dateTo);
}
