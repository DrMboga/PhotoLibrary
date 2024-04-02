using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace PhotoLibraryBackend.Data;

public class ImporterDataAccessMessagesHandler :
    INotificationHandler<SaveImporterStepToDbNotification>,
    INotificationHandler<SaveMediaFileInfoToDbNotification>,
    IRequestHandler<SaveNewFolderInfoRequest, FolderInfo>,
    IRequestHandler<GetLibraryInfoRequest, LibraryInfo?>,
    IRequestHandler<GetImporterLogsRequest, ImporterReport[]?>,
    IRequestHandler<GetAllVideosRequest, MediaFileInfo[]>,
    INotificationHandler<UpdateVideoDateNotification>,
    INotificationHandler<UpdateVideoThumbnailNotification>,
    IRequestHandler<GetAllQuickTimeVideosRequest, MediaFileInfo[]>
{
    private readonly IDbContextFactory<PhotoLibraryBackendDbContext> _dbContextFactory;
    private readonly ILogger<ImporterDataAccessMessagesHandler> _logger;

    public ImporterDataAccessMessagesHandler(IDbContextFactory<PhotoLibraryBackendDbContext> dbContextFactory, ILogger<ImporterDataAccessMessagesHandler> logger)
    {
        _dbContextFactory = dbContextFactory;
        _logger = logger;
    }

    public async Task Handle(SaveImporterStepToDbNotification notification, CancellationToken cancellationToken)
    {
        using (var context = _dbContextFactory.CreateDbContext())
        {
            await context.ImporterReport.AddAsync(notification.ReportMessage);
            await context.SaveChangesAsync();
        }
    }

    public async Task Handle(SaveMediaFileInfoToDbNotification notification, CancellationToken cancellationToken)
    {
        using (var context = _dbContextFactory.CreateDbContext())
        {
            var mediaAddress = notification.MediaFileInfoPayload.MediaAddress;
            if (mediaAddress != null)
            {
                var existingCoordinates = await context
                    .Address
                    .Where(a => a.Latitude == mediaAddress.Latitude && a.Longitude == mediaAddress.Longitude)
                    .FirstOrDefaultAsync();
                if (existingCoordinates != null)
                {
                    notification.MediaFileInfoPayload.MediaAddress = existingCoordinates;
                }
            }

            await context.Media.AddAsync(notification.MediaFileInfoPayload);
            await context.SaveChangesAsync();
        }
    }




    public async Task<FolderInfo> Handle(SaveNewFolderInfoRequest request, CancellationToken cancellationToken)
    {
        using (var context = _dbContextFactory.CreateDbContext())
        {
            var existingFolder = await context.Folder.AsNoTracking()
                .Where(f => f.FullName == request.FullPath)
                .FirstOrDefaultAsync();

            if (existingFolder != null) {
                return existingFolder;
            }

            var newFolder = new FolderInfo
            {
                ParentFolderId = request.ParentId,
                FolderName = request.Name,
                FullName = request.FullPath,
            };

            await context.Folder.AddAsync(newFolder);
            await context.SaveChangesAsync();

            return newFolder;
        }
    }

    public async Task<LibraryInfo?> Handle(GetLibraryInfoRequest request, CancellationToken cancellationToken)
    {
        string query = "select Count(m.\"Id\"), Min(m.\"DateTimeOriginalUtc\"), Max(m.\"DateTimeOriginalUtc\") from \"Media\" m";
        using (var context = _dbContextFactory.CreateDbContext())
        using(var connection = context.Database.GetDbConnection())
        using (var command = connection.CreateCommand())
        {
            // seems that this is a first db query, so it should run migrations
            await context.Database.MigrateAsync();
            command.CommandText = query;
            await connection.OpenAsync();
            var reader = await command.ExecuteReaderAsync();
            if (reader.Read())
            {
                var mediaCount = reader.GetInt64(0);
                var minDate = reader.IsDBNull(1) ? null : (DateTime?) reader.GetDateTime(1);
                var maxDate = reader.IsDBNull(2) ? null : (DateTime?) reader.GetDateTime(2);
                return new LibraryInfo(mediaCount, minDate, maxDate);
            }
            return null;
        }
        
    }

    public async Task<ImporterReport[]?> Handle(GetImporterLogsRequest request, CancellationToken cancellationToken)
    {
        using (var context = _dbContextFactory.CreateDbContext())
        {
            return await context.ImporterReport
                .AsNoTracking()
                .OrderByDescending(r => r.Timestamp)
                .Take(request.PageSize)
                .ToArrayAsync();
        }
    }


    public async Task<MediaFileInfo[]> Handle(GetAllVideosRequest request, CancellationToken cancellationToken)
    {
        using (var context = _dbContextFactory.CreateDbContext())
        {
                    /*
select m.*
from "Media" m
where lower(m."FileExt") in ('.avi', '.mov', '.mp4')
order by m."FileExt"
== 2777
        */
            string[] videoExtensions = [".avi", ".mov", ".mp4"];
            var videos = await context.Media
                .AsNoTracking()
                .Where(m => videoExtensions.Contains(m.FileExt.ToLower()))
                .ToArrayAsync();
            return videos ?? [];
        }
    }

    public async Task Handle(UpdateVideoDateNotification notification, CancellationToken cancellationToken)
    {
        using (var context = _dbContextFactory.CreateDbContext())
        {
            var media = await context.Media
                .Where(m => m.Id == notification.MediaId)
                .FirstOrDefaultAsync();
            if (media != null)
            {
                media.DateTimeOriginalUtc = notification.NewDate;
                await context.SaveChangesAsync();
            }
        }
    }

    public async Task Handle(UpdateVideoThumbnailNotification notification, CancellationToken cancellationToken)
    {
        using (var context = _dbContextFactory.CreateDbContext())
        {
            var media = await context.Media
                .Where(m => m.Id == notification.MediaId)
                .FirstOrDefaultAsync();
            if (media != null)
            {
                media.Thumbnail = notification.Thumbnail;
                await context.SaveChangesAsync();
            }
        }
    }

    public async Task<MediaFileInfo[]> Handle(GetAllQuickTimeVideosRequest request, CancellationToken cancellationToken)
    {
        using (var context = _dbContextFactory.CreateDbContext())
        {
            string[] videoExtensions = [".mov"];
            var videos = await context.Media
                .AsNoTracking()
                .Where(m => videoExtensions.Contains(m.FileExt.ToLower()))
                .ToArrayAsync();
            return videos ?? [];
        }
    }
}
