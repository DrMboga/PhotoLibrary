using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace PhotoLibraryBackend;

public class IdentityDbContext: IdentityDbContext<IdentityUser>
{
        // TODO: For migrations uncomment this constructor and comment out another one. Then run:
    // dotnet ef migrations add InitialCreate
    // dotnet ef database update
    // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    // {
    // optionsBuilder.UseNpgsql("Host=localhost;Database=photo-identity-db;Username=postgres;Password=MyDocker6");
    // }
    

    public IdentityDbContext(DbContextOptions<IdentityDbContext> options) :
        base(options) { }
}
