using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Soundy.CatalogService.Migrations
{
    /// <inheritdoc />
    public partial class AddIsFavoriteProp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "Playlists",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "Playlists");
        }
    }
}
