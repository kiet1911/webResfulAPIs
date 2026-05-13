using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace webResfulAPIs.Migrations
{
    /// <inheritdoc />
    public partial class fixCreatortypeconfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Creators_Status",
                table: "Creators");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Creators_Type",
                table: "Creators");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Creators_Status",
                table: "Creators",
                sql: "status IN ('Active', 'Inactive', 'Banned')");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Creators_Type",
                table: "Creators",
                sql: "type IN ('Author','Artist')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Creators_Status",
                table: "Creators");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Creators_Type",
                table: "Creators");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Creators_Status",
                table: "Creators",
                sql: "status IN ('Active', 'Inactive', 'OutStock')");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Creators_Type",
                table: "Creators",
                sql: "type IN ('Active','Inactive','Banned')");
        }
    }
}
