using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace webResfulAPIs.Migrations
{
    /// <inheritdoc />
    public partial class CategoryAndCreators : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BoardGameCategories",
                columns: table => new
                {
                    boardgame_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    category_id = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "GETDATE()"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoardGameCategories", x => new { x.boardgame_id, x.category_id });
                    table.ForeignKey(
                        name: "FK_BoardGameCategories_BoardGames_boardgame_id",
                        column: x => x.boardgame_id,
                        principalTable: "BoardGames",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BoardGameCategories_Categories_category_id",
                        column: x => x.category_id,
                        principalTable: "Categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BoardGameCreators",
                columns: table => new
                {
                    creator_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    boardgame_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "GETDATE()"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoardGameCreators", x => new { x.creator_id, x.boardgame_id });
                    table.ForeignKey(
                        name: "FK_BoardGameCreators_BoardGames_boardgame_id",
                        column: x => x.boardgame_id,
                        principalTable: "BoardGames",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BoardGameCreators_Creators_creator_id",
                        column: x => x.creator_id,
                        principalTable: "Creators",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BoardGameDescription",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    bg_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    short_description = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: ""),
                    full_description = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: ""),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "GETDATE()"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoardGameDescription", x => x.id);
                    table.ForeignKey(
                        name: "FK_BoardGameDescription_BoardGames_bg_id",
                        column: x => x.bg_id,
                        principalTable: "BoardGames",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BoardGameCategories_category_id",
                table: "BoardGameCategories",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "IX_BoardGameCreators_boardgame_id",
                table: "BoardGameCreators",
                column: "boardgame_id");

            migrationBuilder.CreateIndex(
                name: "IX_BoardGameDescription_bg_id",
                table: "BoardGameDescription",
                column: "bg_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BoardGameCategories");

            migrationBuilder.DropTable(
                name: "BoardGameCreators");

            migrationBuilder.DropTable(
                name: "BoardGameDescription");
        }
    }
}
