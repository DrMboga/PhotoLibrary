
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace PhotoLibraryBackend.Data;

public class MediaGeolocationDataAccessMessagesHandler : 
    IRequestHandler<GetMediaGeoLocationSummaryRequest, MediaGeoSummary[]>,
    IRequestHandler<GetRandomMediaByRegionRequest, MediaFileInfo?>
{
    private readonly IDbContextFactory<PhotoLibraryBackendDbContext> _dbContextFactory;

    public MediaGeolocationDataAccessMessagesHandler(IDbContextFactory<PhotoLibraryBackendDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<MediaGeoSummary[]> Handle(GetMediaGeoLocationSummaryRequest request, CancellationToken cancellationToken)
    {
        using (var context = _dbContextFactory.CreateDbContext())
        {
            var mediaGeoSummary = await context.Address.AsNoTracking()
                .Where(a => a.Region != null)
                .Join(context.Media, m => m.AddressId, a => a.MediaAddressId, (address, media) => new {address.Region, address.Country, media.Id})
                .GroupBy(j => new {j.Region, j.Country})
                .Select(g => new MediaGeoSummary(g.Key.Region!, g.Key.Country!, g.Count()))
                .ToArrayAsync();
            return mediaGeoSummary ?? [];
        }
    }

    public async Task<MediaFileInfo?> Handle(GetRandomMediaByRegionRequest request, CancellationToken cancellationToken)
    {
        string query = @"
SELECT m.*
FROM ""Media"" m
	INNER JOIN ""Address"" a ON m.""MediaAddressId"" = a.""AddressId""
WHERE m.""TagLabel"" = 'People' AND a.""Region"" = @Region
ORDER BY RANDOM() LIMIT 1
        ";

        var regionParameter = new NpgsqlParameter("Region", request.Region);

        using (var context = _dbContextFactory.CreateDbContext())
        {
            return await context.Media
                .FromSqlRaw(query, regionParameter)
                .FirstOrDefaultAsync();
        }
    }
}
