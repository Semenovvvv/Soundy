using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Soundy.CatalogService.Migrations
{
    /// <inheritdoc />
    public partial class Fix2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Playlists",
                newName: "Title");

            migrationBuilder.AlterColumn<string>(
                name: "AvatarUrl",
                table: "Playlists",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Playlists",
                newName: "Name");

            migrationBuilder.AlterColumn<string>(
                name: "AvatarUrl",
                table: "Playlists",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
