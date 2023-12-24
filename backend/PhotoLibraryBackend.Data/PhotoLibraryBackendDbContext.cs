using Microsoft.EntityFrameworkCore;
using PhotoLibraryBackend.Common;

namespace PhotoLibraryBackend.Data;

public class PhotoLibraryBackendDbContext: DbContext
{
    // TODO: For migrations
    // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    // {
    // optionsBuilder.UseNpgsql("Host=localhost;Database=photo;Username=postgres;Password=MyDocker6");
    // }
    

    public PhotoLibraryBackendDbContext(DbContextOptions options): base(options)
    {
    }
    public virtual DbSet<MediaFileInfo> Media { get; set; }

    public virtual DbSet<MediaAddress> Address { get; set; }

    public virtual DbSet<Album> Album { get; set; }

    public virtual DbSet<ImporterReport> ImporterReport { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MediaFileInfo>()
            .Property(m => m.Id)
            .ValueGeneratedOnAdd();
        modelBuilder.Entity<MediaFileInfo>()
            .Property(m => m.Thumbnail)
            .HasColumnType("bytea");
        modelBuilder.Entity<MediaFileInfo>()
            .Ignore(i => i.Orientation)
            .Ignore(i => i.MediaAddress)
            .HasKey(m => m.Id);
        modelBuilder.Entity<MediaFileInfo>()
            .HasIndex(m => m.FullPath)
            .IsUnique();
        modelBuilder.Entity<MediaFileInfo>()
            .HasIndex(m => m.DateTimeOriginal)
            .IsUnique();
        
        modelBuilder.Entity<MediaAddress>()
            .Property(a => a.AddressId)
            .ValueGeneratedOnAdd();
        modelBuilder.Entity<MediaAddress>()
            .HasKey(i => i.AddressId);
        modelBuilder.Entity<MediaAddress>()
            .Property(i => i.Latitude).HasColumnType("NUMERIC(7,4)");
        modelBuilder.Entity<MediaAddress>()
            .Property(i => i.Longitude).HasColumnType("NUMERIC(7,4)");
        modelBuilder.Entity<MediaAddress>()
            .Property(i => i.VenueDistance).HasColumnType("NUMERIC(5,3)");
        modelBuilder.Entity<MediaAddress>()
            .Property(i => i.AddressDistance).HasColumnType("NUMERIC(5,3)");

        // Media -> Address One Address can have many photos
        modelBuilder.Entity<MediaAddress>()
            .HasMany(e => e.MediaFiles)
            .WithOne(e => e.MediaAddress)
            .HasForeignKey(e => e.MediaAddressId);

        modelBuilder.Entity<Album>()
            .Property(a => a.AlbumId)
            .ValueGeneratedOnAdd();
        modelBuilder.Entity<Album>()
            .HasKey(a => a.AlbumId);

        // One to many. One album can have many photos
        modelBuilder.Entity<Album>()
            .HasMany(a => a.MediaFiles)
            .WithOne(m => m.Album)
            .HasForeignKey(m => m.AlbumId);

        modelBuilder.Entity<ImporterReport>()
            .Property(r => r.Id)
            .ValueGeneratedOnAdd();
        modelBuilder.Entity<ImporterReport>()
            .HasKey(r => r.Id);
    
    }
}