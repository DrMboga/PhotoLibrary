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

    public Task Handle(SaveMediaFileInfoToDbNotification notification, CancellationToken cancellationToken)
    {
        /*
select * from "ImporterReport" r

-- select * from "Media" m
        */
        return Task.CompletedTask;
    }
}
