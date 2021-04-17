using Microsoft.EntityFrameworkCore.Migrations;

namespace OtrsReportApp.Migrations.TicketDb
{
    public partial class TicketIdUnique : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_pended_ticket_TicketId",
                table: "pended_ticket",
                column: "TicketId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_pended_ticket_TicketId",
                table: "pended_ticket");
        }
    }
}
