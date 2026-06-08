using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace webResfulAPIs.Migrations
{
    /// <inheritdoc />
    public partial class fix_order_status : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Orders_Status",
                table: "Orders");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Orders_Status",
                table: "Orders",
                sql: "status in ('Pending','Confirmed','Cancelled','Refunded','Shipping', 'Delivered') ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Orders_Status",
                table: "Orders");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Orders_Status",
                table: "Orders",
                sql: "status in ('Pending','Paid','Cancel','Refund','Delivering') ");
        }
    }
}
