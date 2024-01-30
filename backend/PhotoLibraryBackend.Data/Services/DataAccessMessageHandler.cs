using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace PhotoLibraryBackend.Data;

public class DataAccessMessageHandler :
    INotificationHandler<SaveImporterStepToDbNotification>,
    INotificationHandler<SaveMediaFileInfoToDbNotification>,
    IRequestHandler<GetMediaFileHashRequest, string?>,
    IRequestHandler<GetNextPhotosChunkRequest, MediaFileInfo[]>,
    IRequestHandler<GetPreviousPhotosChunkRequest, MediaFileInfo[]>,
    IRequestHandler<SaveNewFolderInfoRequest, FolderInfo>,
    IRequestHandler<GetLibraryInfoRequest, LibraryInfo?>,
    IRequestHandler<GetImporterLogsRequest, ImporterReport[]?>,
    IRequestHandler<GetMediaFullPathByIdRequest, string>,
    INotificationHandler<MarkMediaAsDeletedNotification>,
    INotificationHandler<ChangeMediaAlbumNotification>,
    IRequestHandler<GetMediaListByAlbumDataBaseRequest, MediaFileInfo[]>,
    IRequestHandler<GetAllVideosRequest, MediaFileInfo[]>,
    INotificationHandler<UpdateVideoDateNotification>,
    INotificationHandler<UpdateVideoThumbnailNotification>,
    IRequestHandler<GetAddressesListRequest, MediaAddress[]>,
    INotificationHandler<SaveAddressInfoNotification>
{
    private readonly IDbContextFactory<PhotoLibraryBackendDbContext> _dbContextFactory;
    private readonly ILogger<DataAccessMessageHandler> _logger;

    public DataAccessMessageHandler(IDbContextFactory<PhotoLibraryBackendDbContext> dbContextFactory, ILogger<DataAccessMessageHandler> logger)
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
                .Where(m => m.DateTimeOriginalUtc <= request.DateFrom && m.Deleted == false)
                .OrderByDescending(m => m.DateTimeOriginalUtc)
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
                .Where(m => m.DateTimeOriginalUtc > request.DateTo && m.Deleted == false)
                .OrderBy(m => m.DateTimeOriginalUtc)
                .Take(request.ChunkSize)
                .ToArrayAsync();
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
        string query = "select Count(m.*), Min(m.\"DateTimeOriginalUtc\"), Max(m.\"DateTimeOriginalUtc\") from \"Media\" m";
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

    public async Task<MediaAddress[]> Handle(GetAddressesListRequest request, CancellationToken cancellationToken)
    {
        using (var context = _dbContextFactory.CreateDbContext())
        {
            var addressesQuery = context.Address
                .AsNoTracking();
            if (request.ReadDateIsEmpty.HasValue && request.ReadDateIsEmpty.Value)
            {
                addressesQuery = addressesQuery.Where(a => a.AddressReadDate == null);
            }
            if (request.ReadDateIsEmpty.HasValue && !request.ReadDateIsEmpty.Value)
            {
                addressesQuery = addressesQuery.Where(a => a.AddressReadDate != null);
            }
            addressesQuery = addressesQuery.Take(request.TopRows);

            var addresses = await addressesQuery.ToArrayAsync();

            return addresses ?? [];
        }
    }

    public async Task Handle(SaveAddressInfoNotification notification, CancellationToken cancellationToken)
    {
        using (var context = _dbContextFactory.CreateDbContext())
        {
            var address = await context.Address
                .Where(a => a.AddressId == notification.Address.AddressId)
                .FirstOrDefaultAsync();
            if (address != null)
            {
                address.Country = notification.Address.Country;
                address.Region = notification.Address.Region;
                address.Locality = notification.Address.Locality;
                address.AddressName = notification.Address.AddressName;
                address.AddressLabel = notification.Address.AddressLabel;
                address.VenueName = notification.Address.VenueName;
                address.VenueLabel = notification.Address.VenueLabel;
                address.AddressDistance = notification.Address.AddressDistance;
                address.AddressReadDate = notification.Address.AddressReadDate;
                await context.SaveChangesAsync();
            }
        }
    }
}
