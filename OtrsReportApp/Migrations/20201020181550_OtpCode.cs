using Microsoft.EntityFrameworkCore.Migrations;

namespace OtrsReportApp.Migrations
{
    public partial class OtpCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OtpCode",
                table: "otpcodes");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "otpcodes",
                nullable: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Code",
                table: "otpcodes");

            migrationBuilder.AddColumn<string>(
                name: "OtpCode",
                table: "otpcodes",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: false,
                defaultValue: "");
        }
    }
}
