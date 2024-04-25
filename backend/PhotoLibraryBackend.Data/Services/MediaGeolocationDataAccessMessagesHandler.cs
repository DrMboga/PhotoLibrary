
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace PhotoLibraryBackend.Data;

public class MediaGeolocationDataAccessMessagesHandler : 
    IRequestHandler<GetMediaGeoLocationSummaryRequest, MediaGeoSummary[]>,
    IRequestHandler<GetRandomMediaByRegionRequest, MediaFileInfo?>,
    IRequestHandler<GetMediaGeoLocationRegionSummaryRequest, MediaGeoLocationRegionSummaryDto[]>,
    IRequestHandler<GetMediaFilesByRegionAndDateRequest, MediaFileInfo[]>
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
                .Where(a => a.Region != null && a.Region != "")
                .Join(context.Media, m => m.AddressId, a => a.MediaAddressId, (address, media) => new {address.Region, address.Country, media.Id, media.DateTimeOriginalUtc})
                .GroupBy(j => new {j.Region, j.Country})
                .Select(g => new MediaGeoSummary(g.Key.Region!, g.Key.Country!, g.Count(), g.Max(j => j.DateTimeOriginalUtc)))
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
WHERE m.""TagLabel"" = @TagLabel AND a.""Region"" = @Region  AND m.""Deleted"" = false
ORDER BY RANDOM() LIMIT 1
        ";

        var tagParameter = new NpgsqlParameter("TagLabel", request.TagLabel);
        var regionParameter = new NpgsqlParameter("Region", request.Region);

        using (var context = _dbContextFactory.CreateDbContext())
        {
            return await context.Media
                .FromSqlRaw(query, tagParameter, regionParameter)
                .FirstOrDefaultAsync();
        }
    }

    public async Task<MediaGeoLocationRegionSummaryDto[]> Handle(GetMediaGeoLocationRegionSummaryRequest request, CancellationToken cancellationToken)
    {
        string query = @"
SELECT 
	CONCAT(CAST(DATE_PART('year', m.""DateTimeOriginalUtc"") AS VARCHAR(4)), '-', RIGHT(CONCAT('00', CAST(DATE_PART('month', m.""DateTimeOriginalUtc"") AS VARCHAR(2))), 2)) monthly,
	DATE_PART('year', m.""DateTimeOriginalUtc"") yearpart,
	DATE_PART('month', m.""DateTimeOriginalUtc"") monthpart,
	COUNT(m.""Id"") photoscount
FROM ""Media"" m
	INNER JOIN ""Address"" a ON a.""AddressId"" = m.""MediaAddressId""
WHERE a.""Region"" = @Region AND m.""Deleted"" = false
GROUP BY CONCAT(CAST(DATE_PART('year', m.""DateTimeOriginalUtc"") AS VARCHAR(4)), '-', RIGHT(CONCAT('00', CAST(DATE_PART('month', m.""DateTimeOriginalUtc"") AS VARCHAR(2))), 2)),
	DATE_PART('year', m.""DateTimeOriginalUtc""),
	DATE_PART('month', m.""DateTimeOriginalUtc"")
ORDER BY CONCAT(CAST(DATE_PART('year', m.""DateTimeOriginalUtc"") AS VARCHAR(4)), '-', RIGHT(CONCAT('00', CAST(DATE_PART('month', m.""DateTimeOriginalUtc"") AS VARCHAR(2))), 2))       
        ";
        var regionParameter = new NpgsqlParameter("Region", request.Region);

        var result = new List<MediaGeoLocationRegionSummaryDto>();

        using (var context = _dbContextFactory.CreateDbContext())
        using(var connection = context.Database.GetDbConnection())
        using (var command = connection.CreateCommand())
        {
            command.CommandText = query;
            command.Parameters.Add(regionParameter);
            await connection.OpenAsync();
            var reader = await command.ExecuteReaderAsync();
            while (reader.Read())
            {
                var monthly = reader.GetString(0);
                var yearPart = Convert.ToInt32(reader.GetDouble(1));
                var monthPart = Convert.ToInt32(reader.GetDouble(2));
                var mediasCount = Convert.ToInt32(reader.GetInt64(3));
                result.Add(new MediaGeoLocationRegionSummaryDto(monthly, yearPart, monthPart, mediasCount));
            }
        }

        return [.. result];
    }

    public async Task<MediaFileInfo[]> Handle(GetMediaFilesByRegionAndDateRequest request, CancellationToken cancellationToken)
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
                    m.DateTimeOriginalUtc.Year == request.Year && 
                    m.DateTimeOriginalUtc.Month == request.Month &&
                    m.Deleted == false &&
                    m.MediaAddress != null &&
                    m.MediaAddress.Region == request.Region)
                .OrderBy(m => m.DateTimeOriginalUtc)
                .ToArrayAsync();
            return media ?? [];
        }
    }
}
