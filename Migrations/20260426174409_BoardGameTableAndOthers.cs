using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace webResfulAPIs.Migrations
{
    /// <inheritdoc />
    public partial class BoardGameTableAndOthers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BoardGames",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    name = table.Column<string>(type: "nvarchar(256)", nullable: false, defaultValue: ""),
                    base_price = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    stock_quantity = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    sold_quantity = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "GETDATE()"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "GETDATE()"),
                    status = table.Column<string>(type: "nvarchar(100)", nullable: false, defaultValue: "Active"),
                    weight = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    size_x = table.Column<decimal>(type: "decimal(8,2)", nullable: false, defaultValue: 0m),
                    size_y = table.Column<decimal>(type: "decimal(8,2)", nullable: false, defaultValue: 0m),
                    size_z = table.Column<decimal>(type: "decimal(8,2)", nullable: false, defaultValue: 0m),
                    min_player = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    max_player = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    min_time = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    max_time = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    prefer_player = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    complexity = table.Column<decimal>(type: "decimal(4,2)", nullable: false, defaultValue: 0m),
                    rating = table.Column<decimal>(type: "decimal(4,2)", nullable: false, defaultValue: 0m),
                    age_requirement = table.Column<int>(type: "int", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoardGames", x => x.id);
                    table.CheckConstraint("CK_BoardGames_Age", "age_requirement >= 0");
                    table.CheckConstraint("CK_BoardGames_Prefer", "prefer_player >= 0");
                    table.CheckConstraint("CK_BoardGames_Price", "base_price >= 0");
                    table.CheckConstraint("CK_BoardGames_Rating", "rating >= 0 and rating <=10");
                    table.CheckConstraint("CK_BoardGames_SizeX", "size_x >= 0");
                    table.CheckConstraint("CK_BoardGames_SizeY", "size_y >= 0");
                    table.CheckConstraint("CK_BoardGames_SizeZ", "size_z >= 0");
                    table.CheckConstraint("CK_BoardGames_Sold", "sold_quantity >= 0");
                    table.CheckConstraint("CK_BoardGames_Stock", "stock_quantity >= 0");
                    table.CheckConstraint("CK_BoardGames_ValidPlayer", "max_player >= min_player and min_player >=0");
                    table.CheckConstraint("CK_BoardGames_ValidTime", "max_time >= min_time and min_time >=0");
                    table.CheckConstraint("CK_BoardGames_Weight", "weight >= 0");
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(256)", nullable: false, defaultValue: ""),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: ""),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "GETDATE()"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "GETDATE()"),
                    status = table.Column<string>(type: "varchar(100)", nullable: false, defaultValue: "Active")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.id);
                    table.CheckConstraint("CK_Category_Status", "status IN ('Active', 'Inactive', 'Banned')");
                });

            migrationBuilder.CreateTable(
                name: "Creators",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    name = table.Column<string>(type: "nvarchar(256)", nullable: false, defaultValue: "empty"),
                    bio = table.Column<string>(type: "nvarchar(256)", nullable: false, defaultValue: ""),
                    type = table.Column<string>(type: "nvarchar(100)", nullable: false, defaultValue: "Author"),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "GETDATE()"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "GETDATE()"),
                    status = table.Column<string>(type: "nvarchar(100)", nullable: false, defaultValue: "Active")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Creators", x => x.id);
                    table.CheckConstraint("CK_Creators_Status", "status IN ('Active', 'Inactive', 'OutStock')");
                    table.CheckConstraint("CK_Creators_Type", "type IN ('Active','Inactive','Banned')");
                });

            migrationBuilder.CreateTable(
                name: "BoardGameImages",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    bg_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    img_url = table.Column<string>(type: "nvarchar(256)", nullable: false, defaultValue: ""),
                    alt = table.Column<string>(type: "nvarchar(256)", nullable: false, defaultValue: ""),
                    is_thumbnail = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "GETDATE()"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoardGameImages", x => x.id);
                    table.ForeignKey(
                        name: "FK_BoardGameImages_BoardGames_bg_id",
                        column: x => x.bg_id,
                        principalTable: "BoardGames",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BoardGameImages_bg_id",
                table: "BoardGameImages",
                column: "bg_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BoardGameImages");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Creators");

            migrationBuilder.DropTable(
                name: "BoardGames");
        }
    }
}
