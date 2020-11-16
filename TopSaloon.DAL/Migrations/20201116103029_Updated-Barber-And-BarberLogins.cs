using Microsoft.EntityFrameworkCore.Migrations;

namespace TopSaloon.DAL.Migrations
{
    public partial class UpdatedBarberAndBarberLogins : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumberOfCompleteOrders",
                table: "BarberLogins");

            migrationBuilder.AddColumn<string>(
                name: "BarberFingerPrintId",
                table: "Barbers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BarberFingerPrintId",
                table: "Barbers");

            migrationBuilder.AddColumn<int>(
                name: "NumberOfCompleteOrders",
                table: "BarberLogins",
                type: "int",
                nullable: true);
        }
    }
}
