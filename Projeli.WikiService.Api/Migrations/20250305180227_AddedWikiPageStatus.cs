using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Projeli.WikiService.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddedWikiPageStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPublished",
                table: "Pages");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Pages",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Pages");

            migrationBuilder.AddColumn<bool>(
                name: "IsPublished",
                table: "Pages",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
