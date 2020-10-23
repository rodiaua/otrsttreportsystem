using Microsoft.EntityFrameworkCore.Migrations;

namespace OtrsReportApp.Migrations
{
    public partial class OtpString : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "OtpCode",
                table: "otpcodes",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "OtpCode",
                table: "otpcodes",
                type: "int",
                nullable: false,
                oldClrType: typeof(string));
        }
    }
}
