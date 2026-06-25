using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace webResfulAPIs.Migrations
{
    /// <inheritdoc />
    public partial class updateOrderaddurlandmerchantcode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "merchant_refno",
                table: "Orders",
                type: "nvarchar(100)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "url_vnpay",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "merchant_refno",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "url_vnpay",
                table: "Orders");
        }
    }
}
