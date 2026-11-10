using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShipNotes.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Applications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IconUrl = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    ApiKeyHash = table.Column<string>(type: "char(64)", unicode: false, fixedLength: true, maxLength: 64, nullable: false),
                    ApiKeyPrefix = table.Column<string>(type: "varchar(12)", unicode: false, maxLength: 12, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Applications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReleaseNotes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Version = table.Column<string>(type: "varchar(128)", unicode: false, maxLength: 128, nullable: false),
                    VersionSortKey = table.Column<string>(type: "varchar(400)", unicode: false, maxLength: 400, nullable: false, collation: "Latin1_General_100_BIN2"),
                    IsPreRelease = table.Column<bool>(type: "bit", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ContentHtml = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PublishedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReleaseNotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReleaseNotes_Applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "Applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Applications_ApiKeyHash",
                table: "Applications",
                column: "ApiKeyHash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Applications_Name",
                table: "Applications",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReleaseNotes_ApplicationId_PublishedAt_VersionSortKey",
                table: "ReleaseNotes",
                columns: new[] { "ApplicationId", "PublishedAt", "VersionSortKey" });

            migrationBuilder.CreateIndex(
                name: "IX_ReleaseNotes_ApplicationId_VersionSortKey",
                table: "ReleaseNotes",
                columns: new[] { "ApplicationId", "VersionSortKey" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReleaseNotes");

            migrationBuilder.DropTable(
                name: "Applications");
        }
    }
}
