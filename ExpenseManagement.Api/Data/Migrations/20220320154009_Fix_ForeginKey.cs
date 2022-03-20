using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpenseManagement.Api.Data.Migrations
{
    public partial class Fix_ForeginKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserExpenses_ExpenseId",
                table: "UserExpenses");

            migrationBuilder.CreateIndex(
                name: "IX_UserExpenses_ExpenseId",
                table: "UserExpenses",
                column: "ExpenseId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserChats_ChatId",
                table: "UserChats",
                column: "ChatId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserChats_AspNetUsers_UserId",
                table: "UserChats",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserChats_Chats_ChatId",
                table: "UserChats",
                column: "ChatId",
                principalTable: "Chats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserChats_AspNetUsers_UserId",
                table: "UserChats");

            migrationBuilder.DropForeignKey(
                name: "FK_UserChats_Chats_ChatId",
                table: "UserChats");

            migrationBuilder.DropIndex(
                name: "IX_UserExpenses_ExpenseId",
                table: "UserExpenses");

            migrationBuilder.DropIndex(
                name: "IX_UserChats_ChatId",
                table: "UserChats");

            migrationBuilder.CreateIndex(
                name: "IX_UserExpenses_ExpenseId",
                table: "UserExpenses",
                column: "ExpenseId");
        }
    }
}
