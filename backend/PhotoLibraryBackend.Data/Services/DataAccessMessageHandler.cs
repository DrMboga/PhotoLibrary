using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace PhotoLibraryBackend.Data;

public class DataAccessMessageHandler :
    INotificationHandler<SaveImporterStepToDbNotification>,
    INotificationHandler<SaveMediaFileInfoToDbNotification>,
    IRequestHandler<GetMediaFileHashRequest, string?>,
    IRequestHandler<GetNextPhotosChunkRequest, MediaFileInfo[]>,
    IRequestHandler<GetPreviousPhotosChunkRequest, MediaFileInfo[]>,
    IRequestHandler<SaveNewFolderInfoRequest, FolderInfo>
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
                .Where(m => m.DateTimeOriginalUtc <= request.DateFrom)
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
                .Where(m => m.DateTimeOriginalUtc > request.DateTo)
                .OrderBy(m => m.DateTimeOriginalUtc)
                .Take(request.ChunkSize)
                .ToArrayAsync();
        }
    }

    public async Task<FolderInfo> Handle(SaveNewFolderInfoRequest request, CancellationToken cancellationToken)
    {
        using (var context = _dbContextFactory.CreateDbContext())
        {
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
}

        /*
-- select * from "ImporterReport" r
-- order by r."Timestamp" desc

select m.*
from "Media" m
-- where 
-- m."FileName" = 'IMG_5843.JPG'
-- m."MediaAddressId" = 966 --is not null
order by m."DateTimeOriginalUtc" desc

-- select * from "Address" a

-- truncate table "Media"
        */