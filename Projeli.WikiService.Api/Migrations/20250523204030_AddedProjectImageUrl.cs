using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Projeli.WikiService.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddedProjectImageUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProjectImageUrl",
                table: "Wikis",
                type: "character varying(128)",
                maxLength: 128,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProjectImageUrl",
                table: "Wikis");
        }
    }
}
