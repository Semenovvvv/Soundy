using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Soundy.CatalogService.Migrations
{
    /// <inheritdoc />
    public partial class AddPlaylistTrack : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlaylistTrack");

            migrationBuilder.CreateTable(
                name: "PlaylistsTracks",
                columns: table => new
                {
                    PlaylistId = table.Column<Guid>(type: "uuid", nullable: false),
                    TrackId = table.Column<Guid>(type: "uuid", nullable: false),
                    AddedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlaylistsTracks", x => new { x.PlaylistId, x.TrackId });
                    table.ForeignKey(
                        name: "FK_PlaylistsTracks_Playlists_PlaylistId",
                        column: x => x.PlaylistId,
                        principalTable: "Playlists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlaylistsTracks_Tracks_TrackId",
                        column: x => x.TrackId,
                        principalTable: "Tracks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlaylistsTracks_TrackId",
                table: "PlaylistsTracks",
                column: "TrackId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlaylistsTracks");

            migrationBuilder.CreateTable(
                name: "PlaylistTrack",
                columns: table => new
                {
                    PlaylistsId = table.Column<Guid>(type: "uuid", nullable: false),
                    TracksId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlaylistTrack", x => new { x.PlaylistsId, x.TracksId });
                    table.ForeignKey(
                        name: "FK_PlaylistTrack_Playlists_PlaylistsId",
                        column: x => x.PlaylistsId,
                        principalTable: "Playlists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlaylistTrack_Tracks_TracksId",
                        column: x => x.TracksId,
                        principalTable: "Tracks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlaylistTrack_TracksId",
                table: "PlaylistTrack",
                column: "TracksId");
        }
    }
}
