using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace PhotoLibraryBackend.Data;

public class DataAccessMessageHandler :
    INotificationHandler<SaveImporterStepToDbNotification>,
    INotificationHandler<SaveMediaFileInfoToDbNotification>
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
}

        /*
-- select * from "ImporterReport" r

-- select * 
-- from "Media" m
-- where 

-- m."MediaAddressId" is null
-- order by m."DateTimeOriginalUtc"

-- select * from "Address" a

-- truncate table "Media"
        */