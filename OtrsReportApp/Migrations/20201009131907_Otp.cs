using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OtrsReportApp.Migrations
{
    public partial class Otp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "otpcodes",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    OtpCode = table.Column<int>(nullable: false),
                    Expires = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_otpcodes", x => x.UserId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "otpcodes");
        }
    }
}
