using Microsoft.EntityFrameworkCore;

namespace PhotoLibraryBackend.Data;

public class MediaDataAccessMessagesHandler:
    IRequestHandler<GetMediaFileHashRequest, string?>,
    IRequestHandler<GetNextPhotosChunkRequest, MediaFileInfo[]>,
    IRequestHandler<GetPreviousPhotosChunkRequest, MediaFileInfo[]>,
    IRequestHandler<GetMediaFullPathByIdRequest, string>,
    INotificationHandler<MarkMediaAsDeletedNotification>,
    INotificationHandler<ChangeMediaAlbumNotification>,
    IRequestHandler<GetMediaListByAlbumDataBaseRequest, MediaFileInfo[]>,
    IRequestHandler<GetMediasOfTheDayRequest, MediaFileInfo[]>,
    IRequestHandler<GetMediaByIdRequest, MediaFileInfo>,
    IRequestHandler<GetBunchOfMediasWithEmptyLabelRequest, (long MediaId, string FullFileName)[]>
{
    private readonly IDbContextFactory<PhotoLibraryBackendDbContext> _dbContextFactory;

    public MediaDataAccessMessagesHandler(IDbContextFactory<PhotoLibraryBackendDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<string?> Handle(GetMediaFileHashRequest request, CancellationToken cancellationToken)
    {
        using (var context = _dbContextFactory.CreateDbContext())
        {
            return await context.Media
                .AsNoTracking()
                .Where(m => m.FullPath == request.FullPath)
                .Select(m => m.FileHash)
                .FirstOrDefaultAsync();
        }
    }

        public async Task<MediaFileInfo[]> Handle(GetNextPhotosChunkRequest request, CancellationToken cancellationToken)
    {
        using (var context = _dbContextFactory.CreateDbContext())
        {
            return await context.Media
                .AsNoTracking()
                .Include(m => m.MediaAddress)
                .AsNoTracking()
                .Include(m => m.Album)
                .AsNoTracking()
                .Where(m => m.DateTimeOriginalUtc >= request.DateFrom && m.Deleted == false)
                .OrderBy(m => m.DateTimeOriginalUtc)
                .Take(request.ChunkSize)
                .ToArrayAsync();
        }
    }

    public async Task<MediaFileInfo[]> Handle(GetPreviousPhotosChunkRequest request, CancellationToken cancellationToken)
    {
        using (var context = _dbContextFactory.CreateDbContext())
        {
            return await context.Media
                .AsNoTracking()
                .Include(m => m.MediaAddress)
                .AsNoTracking()
                .Include(m => m.Album)
                .AsNoTracking()
                .Where(m => m.DateTimeOriginalUtc <= request.DateTo && m.Deleted == false)
                .OrderByDescending(m => m.DateTimeOriginalUtc)
                .Take(request.ChunkSize)
                .ToArrayAsync();
        }
    }
    public async Task<string> Handle(GetMediaFullPathByIdRequest request, CancellationToken cancellationToken)
    {
        using (var context = _dbContextFactory.CreateDbContext())
        {
            var fullPath = await context.Media
                .AsNoTracking()
                .Where(m => m.Id == request.MediaId)
                .Select(m => m.FullPath)
                .FirstOrDefaultAsync();
            return fullPath ?? string.Empty;
        }
    }

    public async Task Handle(MarkMediaAsDeletedNotification notification, CancellationToken cancellationToken)
    {
        using (var context = _dbContextFactory.CreateDbContext())
        {
            var media = await context.Media
                .Where(m => m.Id == notification.MediaId)
                .FirstOrDefaultAsync();
            if (media != null)
            {
                media.Deleted = true;
            }
            await context.SaveChangesAsync();
        }
    }

    public async Task Handle(ChangeMediaAlbumNotification notification, CancellationToken cancellationToken)
    {
        using (var context = _dbContextFactory.CreateDbContext())
        {
            var media = await context.Media
                .Where(m => m.Id == notification.MediaId)
                .Include(m => m.Album)
                .FirstOrDefaultAsync();
            if (media != null)
            {
                if(media.Album != null) {
                    if (notification.IsFavorite.HasValue)
                    {
                        media.Album.MarkedAsFavorite = notification.IsFavorite.Value;
                    }
                    if (notification.IsImportant.HasValue)
                    {
                        media.Album.MarkedAsImportant = notification.IsImportant.Value;
                    }
                    if (notification.IsToPrint.HasValue)
                    {
                        media.Album.MarkedAsPrint = notification.IsToPrint.Value;
                    }
                }
                else {
                    media.Album = new Album {
                        MarkedAsFavorite = notification.IsFavorite ?? false,
                        MarkedAsImportant = notification.IsImportant ?? false,
                        MarkedAsPrint = notification.IsToPrint ?? false
                    };
                }
            }
            await context.SaveChangesAsync();
        }
    }

    public async Task<MediaFileInfo[]> Handle(GetMediaListByAlbumDataBaseRequest request, CancellationToken cancellationToken)
    {
        using (var context = _dbContextFactory.CreateDbContext())
        {
            var medias = await context.Media
                .AsNoTracking()
                .Include(m => m.MediaAddress)
                .AsNoTracking()
                .Include(m => m.Album)
                .AsNoTracking()
                .Where(m => m.Album != null 
                    && ((request.IsFavorite.HasValue && m.Album.MarkedAsFavorite == request.IsFavorite) || !request.IsFavorite.HasValue)
                    && ((request.IsImportant.HasValue && m.Album.MarkedAsImportant == request.IsImportant) || !request.IsImportant.HasValue)
                    && ((request.IsToPrint.HasValue && m.Album.MarkedAsPrint == request.IsToPrint) || !request.IsToPrint.HasValue)
                )
                .ToArrayAsync();
            return medias ?? [];
        }
    }

        public async Task<MediaFileInfo[]> Handle(GetMediasOfTheDayRequest request, CancellationToken cancellationToken)
    {
                /*
select *
from "Media"
where date_part('month', "DateTimeOriginalUtc") = 4 
	AND date_part('day', "DateTimeOriginalUtc") = 1
	AND "PictureMaker" is not null AND "PictureMaker" != ' '
order by "DateTimeOriginalUtc"
        */
        using (var context = _dbContextFactory.CreateDbContext())
        {
            var media = await context.Media
                .AsNoTracking()
                .Include(m => m.MediaAddress)
                .AsNoTracking()
                .Include(m => m.Album)
                .AsNoTracking()
                .Where(m => 
                    m.DateTimeOriginalUtc.Month == request.Month && 
                    m.DateTimeOriginalUtc.Day == request.Day &&
                    m.PictureMaker != null &&
                    m.PictureMaker != " " &&
                    m.Deleted == false)
                .ToArrayAsync();
            return media ?? [];
        }
    }

    public async Task<MediaFileInfo> Handle(GetMediaByIdRequest request, CancellationToken cancellationToken)
    {
        using (var context = _dbContextFactory.CreateDbContext())
        {
            var media = await context.Media
                .AsNoTracking()
                .Include(m => m.MediaAddress)
                .AsNoTracking()
                .Where(m => m.Id == request.MediaId)
                .SingleAsync();
            return media;
        }
    }

    public async Task<(long MediaId, string FullFileName)[]> Handle(GetBunchOfMediasWithEmptyLabelRequest request, CancellationToken cancellationToken)
    {
        string[] videoExtensions = [".avi", ".mov", ".mp4"];
        using (var context = _dbContextFactory.CreateDbContext())
        {
            var medias = await context.Media
                .AsNoTracking()
                .Where(m => m.TagLabel == null && !videoExtensions.Contains(m.FileExt.ToLower()))
                .Select(m => new { m.Id, m.FullPath })
                .Take(request.BunchSize)
                .ToArrayAsync();
            var result = new List<(long MediaId, string FullFileName)>();
            foreach (var media in medias)
            {
                result.Add((MediaId: media.Id, FullFileName: media.FullPath));
            }

            return [..result];
        }
    }
}
