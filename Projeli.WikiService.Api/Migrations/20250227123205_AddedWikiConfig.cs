using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Projeli.WikiService.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddedWikiConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Config",
                table: "Wikis",
                type: "jsonb",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Content",
                table: "Wikis",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsCreated",
                table: "Wikis",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Wikis",
                type: "character varying(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProjectName",
                table: "Wikis",
                type: "character varying(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProjectSlug",
                table: "Wikis",
                type: "character varying(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsOwner",
                table: "Members",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Config",
                table: "Wikis");

            migrationBuilder.DropColumn(
                name: "Content",
                table: "Wikis");

            migrationBuilder.DropColumn(
                name: "IsCreated",
                table: "Wikis");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Wikis");

            migrationBuilder.DropColumn(
                name: "ProjectName",
                table: "Wikis");

            migrationBuilder.DropColumn(
                name: "ProjectSlug",
                table: "Wikis");

            migrationBuilder.DropColumn(
                name: "IsOwner",
                table: "Members");
        }
    }
}
