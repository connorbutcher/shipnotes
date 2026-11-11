using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShipNotes.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class StoreReleaseNotesAsOneDocument : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SummaryHtml",
                table: "ReleaseNotes",
                newName: "ContentHtml");

            // Fold each note's sections into its HTML, in display order, wrapped the way the
            // editor writes them, so no content is lost when the sections table goes.
            migrationBuilder.Sql(
                """
                UPDATE n
                SET ContentHtml = n.ContentHtml + s.Html
                FROM ReleaseNotes AS n
                CROSS APPLY (
                    SELECT STRING_AGG(
                        CAST(N'<section class="note-section note-section--' + LOWER(x.Category) + N'">'
                            + N'<h2 class="note-section__title">'
                            + CASE x.Category WHEN 'Breaking' THEN N'Breaking changes' ELSE CAST(x.Category AS nvarchar(20)) END
                            + N'</h2>' + x.ContentHtml + N'</section>' AS nvarchar(max)),
                        N'') WITHIN GROUP (ORDER BY CASE x.Category
                            WHEN 'Breaking' THEN 0 WHEN 'Added' THEN 1 WHEN 'Changed' THEN 2 WHEN 'Deprecated' THEN 3
                            WHEN 'Removed' THEN 4 WHEN 'Fixed' THEN 5 WHEN 'Security' THEN 6 ELSE 7 END) AS Html
                    FROM ReleaseNoteSections AS x
                    WHERE x.ReleaseNoteId = n.Id
                ) AS s
                WHERE s.Html IS NOT NULL;
                """);

            migrationBuilder.DropTable(
                name: "ReleaseNoteSections");
        }

        /// <inheritdoc />
        /// <remarks>The table comes back empty: sections stay inside each note's HTML.</remarks>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ContentHtml",
                table: "ReleaseNotes",
                newName: "SummaryHtml");

            migrationBuilder.CreateTable(
                name: "ReleaseNoteSections",
                columns: table => new
                {
                    ReleaseNoteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Category = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
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
    }
}
