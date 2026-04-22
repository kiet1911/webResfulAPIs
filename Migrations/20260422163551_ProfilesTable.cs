using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace webResfulAPIs.Migrations
{
    /// <inheritdoc />
    public partial class ProfilesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Profiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    full_name = table.Column<string>(type: "nvarchar(256)", nullable: false),
                    display_name = table.Column<string>(type: "nvarchar(256)", nullable: false),
                    img_url = table.Column<string>(type: "nvarchar(256)", nullable: true),
                    birth = table.Column<DateTime>(type: "dateTime", nullable: false, defaultValue: new DateTime(1999, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified)),
                    gender = table.Column<string>(type: "varchar(20)", nullable: true, defaultValue: "Male"),
                    address = table.Column<string>(type: "nvarchar(256)", nullable: true),
                    created_at = table.Column<DateTime>(type: "dateTime", nullable: false, defaultValueSql: "GETDATE()"),
                    updated_at = table.Column<DateTime>(type: "dateTime", nullable: false, defaultValueSql: "GETDATE()"),
                    user_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Profiles_Users_user_id",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Profiles_User_Id",
                table: "Profiles",
                column: "user_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Profiles");
        }
    }
}
