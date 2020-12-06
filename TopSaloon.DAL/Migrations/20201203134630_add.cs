using Microsoft.EntityFrameworkCore.Migrations;

namespace TopSaloon.DAL.Migrations
{
    public partial class add : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "position",
                table: "Services",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "position",
                table: "Services");
        }
    }
}
