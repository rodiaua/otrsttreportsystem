using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OtrsReportApp.Migrations
{
    public partial class OtpId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_otpcodes",
                table: "otpcodes");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "otpcodes",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255) CHARACTER SET utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "otpcodes",
                nullable: false,
                defaultValue: 0)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_otpcodes",
                table: "otpcodes",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_otpcodes",
                table: "otpcodes");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "otpcodes");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "otpcodes",
                type: "varchar(255) CHARACTER SET utf8mb4",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AddPrimaryKey(
                name: "PK_otpcodes",
                table: "otpcodes",
                column: "UserId");
        }
    }
}
