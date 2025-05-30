﻿using Microsoft.EntityFrameworkCore;

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
    IRequestHandler<GetBunchOfMediasWithEmptyLabelRequest, (long MediaId, string FullFileName)[]>,
    INotificationHandler<SetMediaLabelNotification>,
    IRequestHandler<GetLabeledMediasInfoRequest, (long labeledMediaCount, long totalMediaCount)>,
    IRequestHandler<GetMediasByLabelDataRequest, MediaFileInfo[]>,
    IRequestHandler<GetDeletedMediasFromDbRequest, MediaFileInfo[]>
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
                media.Deleted = notification.Deleted;
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
                .Where(m => m.TagLabel == null && m.Deleted == false && !videoExtensions.Contains(m.FileExt.ToLower()))
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

    public async Task Handle(SetMediaLabelNotification notification, CancellationToken cancellationToken)
    {
        using (var context = _dbContextFactory.CreateDbContext())
        {
            var media = await context.Media.Where(m => m.Id == notification.MediaId).FirstOrDefaultAsync();
            if (media != null)
            {
                media.TagLabel = notification.Label;
                await context.SaveChangesAsync();
            }
        }
    }

    public async Task<(long labeledMediaCount, long totalMediaCount)> Handle(GetLabeledMediasInfoRequest request, CancellationToken cancellationToken)
    {
        long total = 0;
        long labeled = 0;

        string countAllQuery = "select count(*) from \"Media\"";
        string countLabeledQuery = "select count(*) from \"Media\" where \"TagLabel\" is not null";

        using (var context = _dbContextFactory.CreateDbContext())
        using(var connection = context.Database.GetDbConnection())
        using (var command = connection.CreateCommand())
        {
            command.CommandText = countAllQuery;
            await connection.OpenAsync();
            total = (long)((await command.ExecuteScalarAsync()) ?? 0);

        }

        using (var context = _dbContextFactory.CreateDbContext())
        using(var connection = context.Database.GetDbConnection())
        using (var command = connection.CreateCommand())
        {
            command.CommandText = countLabeledQuery;
            await connection.OpenAsync();
            labeled = (long)((await command.ExecuteScalarAsync()) ?? 0);
        }

        return (labeledMediaCount: labeled, totalMediaCount: total);
    }

    public async Task<MediaFileInfo[]> Handle(GetMediasByLabelDataRequest request, CancellationToken cancellationToken)
    {
        using (var context = _dbContextFactory.CreateDbContext())
        {
            var media = await context.Media
                .AsNoTracking()
                .Include(m => m.MediaAddress)
                .AsNoTracking()
                .Include(m => m.Album)
                .AsNoTracking()
                .Where(m => 
                    m.DateTimeOriginalUtc >= request.DateFrom && 
                    m.DateTimeOriginalUtc <= request.DateTo &&
                    m.TagLabel != null &&
                    m.TagLabel == request.LabelName &&
                    m.Deleted == false)
                .OrderBy(m => m.DateTimeOriginalUtc)
                .ToArrayAsync();
            return media ?? [];
        }
    }

    public async Task<MediaFileInfo[]> Handle(GetDeletedMediasFromDbRequest request, CancellationToken cancellationToken)
    {
        using (var context = _dbContextFactory.CreateDbContext())
        {
            var media = await context.Media
                .AsNoTracking()
                .Where(m => m.Deleted == true)
                .ToArrayAsync();
            return media ?? [];
        }
    }
}
