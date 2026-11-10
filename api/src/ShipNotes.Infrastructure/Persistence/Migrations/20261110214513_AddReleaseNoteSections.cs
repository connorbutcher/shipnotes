using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShipNotes.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddReleaseNoteSections : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ContentHtml",
                table: "ReleaseNotes",
                newName: "SummaryHtml");

            migrationBuilder.CreateTable(
                name: "ReleaseNoteSections",
                columns: table => new
                {
                    Category = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    ReleaseNoteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContentHtml = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReleaseNoteSections", x => new { x.ReleaseNoteId, x.Category });
                    table.ForeignKey(
                        name: "FK_ReleaseNoteSections_ReleaseNotes_ReleaseNoteId",
                        column: x => x.ReleaseNoteId,
                        principalTable: "ReleaseNotes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReleaseNoteSections");

            migrationBuilder.RenameColumn(
                name: "SummaryHtml",
                table: "ReleaseNotes",
                newName: "ContentHtml");
        }
    }
}
