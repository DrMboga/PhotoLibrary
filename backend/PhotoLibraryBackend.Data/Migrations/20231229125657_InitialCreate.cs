using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PhotoLibraryBackend.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Address",
                columns: table => new
                {
                    AddressId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Latitude = table.Column<decimal>(type: "numeric(7,4)", nullable: false),
                    Longitude = table.Column<decimal>(type: "numeric(7,4)", nullable: false),
                    AddressReadDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Country = table.Column<string>(type: "text", nullable: true),
                    Region = table.Column<string>(type: "text", nullable: true),
                    Locality = table.Column<string>(type: "text", nullable: true),
                    AddressName = table.Column<string>(type: "text", nullable: true),
                    AddressLabel = table.Column<string>(type: "text", nullable: true),
                    VenueName = table.Column<string>(type: "text", nullable: true),
                    VenueLabel = table.Column<string>(type: "text", nullable: true),
                    VenueDistance = table.Column<decimal>(type: "numeric(5,3)", nullable: true),
                    AddressDistance = table.Column<decimal>(type: "numeric(5,3)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Address", x => x.AddressId);
                });

            migrationBuilder.CreateTable(
                name: "Album",
                columns: table => new
                {
                    AlbumId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MediaId = table.Column<long>(type: "bigint", nullable: false),
                    MarkedAsFavorite = table.Column<bool>(type: "boolean", nullable: false),
                    MarkedAsImportant = table.Column<bool>(type: "boolean", nullable: false),
                    MarkedAsPrint = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Album", x => x.AlbumId);
                });

            migrationBuilder.CreateTable(
                name: "ImporterReport",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Timestamp = table.Column<int>(type: "integer", nullable: false),
                    Severity = table.Column<int>(type: "integer", nullable: false),
                    Message = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImporterReport", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Media",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FullPath = table.Column<string>(type: "text", nullable: false),
                    FileName = table.Column<string>(type: "text", nullable: false),
                    FileExt = table.Column<string>(type: "text", nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    FileHash = table.Column<string>(type: "text", nullable: false),
                    DateTimeOriginalUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PictureMaker = table.Column<string>(type: "text", nullable: true),
                    Width = table.Column<int>(type: "integer", nullable: true),
                    Height = table.Column<int>(type: "integer", nullable: true),
                    ThumbnailWidth = table.Column<int>(type: "integer", nullable: true),
                    ThumbnailHeight = table.Column<int>(type: "integer", nullable: true),
                    Thumbnail = table.Column<byte[]>(type: "bytea", nullable: true),
                    VideoDurationSec = table.Column<int>(type: "integer", nullable: true),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false),
                    TagLabel = table.Column<string>(type: "text", nullable: true),
                    MediaAddressId = table.Column<long>(type: "bigint", nullable: true),
                    AlbumId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Media", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Media_Address_MediaAddressId",
                        column: x => x.MediaAddressId,
                        principalTable: "Address",
                        principalColumn: "AddressId");
                    table.ForeignKey(
                        name: "FK_Media_Album_AlbumId",
                        column: x => x.AlbumId,
                        principalTable: "Album",
                        principalColumn: "AlbumId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Address_Latitude_Longitude",
                table: "Address",
                columns: new[] { "Latitude", "Longitude" });

            migrationBuilder.CreateIndex(
                name: "IX_ImporterReport_Timestamp",
                table: "ImporterReport",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_Media_AlbumId",
                table: "Media",
                column: "AlbumId");

            migrationBuilder.CreateIndex(
                name: "IX_Media_DateTimeOriginalUtc",
                table: "Media",
                column: "DateTimeOriginalUtc");

            migrationBuilder.CreateIndex(
                name: "IX_Media_FullPath",
                table: "Media",
                column: "FullPath",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Media_MediaAddressId",
                table: "Media",
                column: "MediaAddressId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ImporterReport");

            migrationBuilder.DropTable(
                name: "Media");

            migrationBuilder.DropTable(
                name: "Address");

            migrationBuilder.DropTable(
                name: "Album");
        }
    }
}
