using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Projeli.WikiService.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddedWikiStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MemberPage");

            migrationBuilder.DropTable(
                name: "MemberPageVersion");

            migrationBuilder.DropColumn(
                name: "IsCreated",
                table: "Wikis");

            migrationBuilder.DropColumn(
                name: "IsPublished",
                table: "Wikis");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Wikis",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "PageVersionWikiMember",
                columns: table => new
                {
                    EditorsId = table.Column<Guid>(type: "uuid", nullable: false),
                    PageVersionsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PageVersionWikiMember", x => new { x.EditorsId, x.PageVersionsId });
                    table.ForeignKey(
                        name: "FK_PageVersionWikiMember_Members_EditorsId",
                        column: x => x.EditorsId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PageVersionWikiMember_PageVersions_PageVersionsId",
                        column: x => x.PageVersionsId,
                        principalTable: "PageVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PageWikiMember",
                columns: table => new
                {
                    EditorsId = table.Column<Guid>(type: "uuid", nullable: false),
                    PagesId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PageWikiMember", x => new { x.EditorsId, x.PagesId });
                    table.ForeignKey(
                        name: "FK_PageWikiMember_Members_EditorsId",
                        column: x => x.EditorsId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PageWikiMember_Pages_PagesId",
                        column: x => x.PagesId,
                        principalTable: "Pages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PageVersionWikiMember_PageVersionsId",
                table: "PageVersionWikiMember",
                column: "PageVersionsId");

            migrationBuilder.CreateIndex(
                name: "IX_PageWikiMember_PagesId",
                table: "PageWikiMember",
                column: "PagesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PageVersionWikiMember");

            migrationBuilder.DropTable(
                name: "PageWikiMember");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Wikis");

            migrationBuilder.AddColumn<bool>(
                name: "IsCreated",
                table: "Wikis",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublished",
                table: "Wikis",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "MemberPage",
                columns: table => new
                {
                    EditorsId = table.Column<Guid>(type: "uuid", nullable: false),
                    PagesId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberPage", x => new { x.EditorsId, x.PagesId });
                    table.ForeignKey(
                        name: "FK_MemberPage_Members_EditorsId",
                        column: x => x.EditorsId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MemberPage_Pages_PagesId",
                        column: x => x.PagesId,
                        principalTable: "Pages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MemberPageVersion",
                columns: table => new
                {
                    EditorsId = table.Column<Guid>(type: "uuid", nullable: false),
                    PageVersionsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberPageVersion", x => new { x.EditorsId, x.PageVersionsId });
                    table.ForeignKey(
                        name: "FK_MemberPageVersion_Members_EditorsId",
                        column: x => x.EditorsId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MemberPageVersion_PageVersions_PageVersionsId",
                        column: x => x.PageVersionsId,
                        principalTable: "PageVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MemberPage_PagesId",
                table: "MemberPage",
                column: "PagesId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberPageVersion_PageVersionsId",
                table: "MemberPageVersion",
                column: "PageVersionsId");
        }
    }
}
