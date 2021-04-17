using Microsoft.EntityFrameworkCore.Migrations;

namespace OtrsReportApp.Migrations.TicketDb
{
    public partial class c : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_pended_ticket_ticket_comment_CommentId",
                table: "pended_ticket");

            migrationBuilder.AlterColumn<int>(
                name: "CommentId",
                table: "pended_ticket",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_pended_ticket_ticket_comment_CommentId",
                table: "pended_ticket",
                column: "CommentId",
                principalTable: "ticket_comment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_pended_ticket_ticket_comment_CommentId",
                table: "pended_ticket");

            migrationBuilder.AlterColumn<int>(
                name: "CommentId",
                table: "pended_ticket",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_pended_ticket_ticket_comment_CommentId",
                table: "pended_ticket",
                column: "CommentId",
                principalTable: "ticket_comment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
