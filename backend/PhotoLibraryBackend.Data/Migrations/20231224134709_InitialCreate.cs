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
                    AddressId = table.Column<int>(type: "integer", nullable: false)
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
                name: "ImporterReport",
                columns: table => new
                {
                    Timestamp = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Severity = table.Column<int>(type: "integer", nullable: false),
                    Message = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImporterReport", x => x.Timestamp);
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
                    DateTimeOriginal = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PictureMaker = table.Column<string>(type: "text", nullable: true),
                    Width = table.Column<int>(type: "integer", nullable: true),
                    Height = table.Column<int>(type: "integer", nullable: true),
                    ThumbnailWidth = table.Column<int>(type: "integer", nullable: true),
                    ThumbnailHeight = table.Column<int>(type: "integer", nullable: true),
                    Thumbnail = table.Column<byte[]>(type: "bytea", nullable: true),
                    VideoDurationSec = table.Column<int>(type: "integer", nullable: true),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false),
                    TagLabel = table.Column<string>(type: "text", nullable: true),
                    MediaAddressId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Media", x => x.Id);
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
                    table.ForeignKey(
                        name: "FK_Album_Media_MediaId",
                        column: x => x.MediaId,
                        principalTable: "Media",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Album_MediaId",
                table: "Album",
                column: "MediaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Media_DateTimeOriginal",
                table: "Media",
                column: "DateTimeOriginal",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Media_FullPath",
                table: "Media",
                column: "FullPath",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Address");

            migrationBuilder.DropTable(
                name: "Album");

            migrationBuilder.DropTable(
                name: "ImporterReport");

            migrationBuilder.DropTable(
                name: "Media");
        }
    }
}
