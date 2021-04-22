using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OtrsReportApp.Migrations.TicketDb
{
    public partial class PendingTicketRestrictedQueue : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "pending_ticket_restricted_queue",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    QueueId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pending_ticket_restricted_queue", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "pending_ticket_restricted_queue",
                columns: new[] { "Id", "Name", "QueueId" },
                values: new object[,]
                {
                    { 1, "test_But", 32 },
                    { 2, "testing_vas", 38 },
                    { 3, "Testing_operators", 39 },
                    { 4, "TestSurvey", 42 },
                    { 5, "daily testing", 52 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_pending_ticket_restricted_queue_QueueId",
                table: "pending_ticket_restricted_queue",
                column: "QueueId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "pending_ticket_restricted_queue");
        }
    }
}
