using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace PhotoLibraryBackend.Data;

public static class PhotoLibraryDbContextRegister 
{
    public static void RegisterPhotoLibraryDbContext (this IServiceCollection services, string connectionString) {
        services.AddDbContextFactory<PhotoLibraryBackendDbContext>(
            options =>
                options.UseNpgsql(connectionString));
    }
}
