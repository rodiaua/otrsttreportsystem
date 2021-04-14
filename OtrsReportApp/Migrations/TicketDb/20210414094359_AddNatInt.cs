using Microsoft.EntityFrameworkCore.Migrations;

namespace OtrsReportApp.Migrations.TicketDb
{
    public partial class AddNatInt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NatInt",
                table: "acknowledged_ticket",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);
            migrationBuilder.Sql("UPDATE acknowledged_ticket SET NatInt = 'NationalKey'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NatInt",
                table: "acknowledged_ticket");
        }
    }
}
