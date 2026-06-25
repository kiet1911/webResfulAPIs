using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace webResfulAPIs.Migrations
{
    /// <inheritdoc />
    public partial class dbBoardgameUPdateReservation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "address",
                table: "Orders",
                type: "nvarchar(100)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "name_reciptient",
                table: "Orders",
                type: "nvarchar(100)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "note",
                table: "Orders",
                type: "nvarchar(256)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "phone",
                table: "Orders",
                type: "nvarchar(20)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "reservation_quantity",
                table: "BoardGames",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddCheckConstraint(
                name: "CK_BoardGames_Reservation",
                table: "BoardGames",
                sql: "reservation_quantity >=0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_BoardGames_Reservation",
                table: "BoardGames");

            migrationBuilder.DropColumn(
                name: "address",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "name_reciptient",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "note",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "phone",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "reservation_quantity",
                table: "BoardGames");
        }
    }
}
