using Microsoft.EntityFrameworkCore;

namespace PhotoLibraryBackend.Data;

public class AddressDataAccessMessagesHandler:
    IRequestHandler<GetAddressesListRequest, MediaAddress[]>,
    INotificationHandler<SaveAddressInfoNotification>,
    IRequestHandler<GetMediaAddressesCountRequest, int>

{
    private readonly IDbContextFactory<PhotoLibraryBackendDbContext> _dbContextFactory;

    public AddressDataAccessMessagesHandler(IDbContextFactory<PhotoLibraryBackendDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
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

    public async Task<int> Handle(GetMediaAddressesCountRequest request, CancellationToken cancellationToken)
    {
        using (var context = _dbContextFactory.CreateDbContext())
        {
            var addressesQuery = context.Address
                .AsNoTracking();
            if (request.EmptyAddresses)
            {
                addressesQuery = addressesQuery.Where(a => a.AddressReadDate == null);
            } else
            {
                addressesQuery = addressesQuery.Where(a => a.AddressReadDate != null);
            }

            return await addressesQuery.CountAsync();
        }
    }
}
