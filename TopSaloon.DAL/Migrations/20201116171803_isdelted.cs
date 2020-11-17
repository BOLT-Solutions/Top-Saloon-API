using Microsoft.EntityFrameworkCore.Migrations;

namespace TopSaloon.DAL.Migrations
{
    public partial class isdelted : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isDeleted",
                table: "Services",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isDeleted",
                table: "Customers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isDeleted",
                table: "Barbers",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isDeleted",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "isDeleted",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "isDeleted",
                table: "Barbers");
        }
    }
}
