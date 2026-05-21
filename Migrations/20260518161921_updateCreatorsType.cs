using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace webResfulAPIs.Migrations
{
    /// <inheritdoc />
    public partial class updateCreatorsType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Creators_Type",
                table: "Creators");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Creators_Type",
                table: "Creators",
                sql: "type IN ('Author','Artist', 'Designer', 'Publisher')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Creators_Type",
                table: "Creators");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Creators_Type",
                table: "Creators",
                sql: "type IN ('Author','Artist')");
        }
    }
}
