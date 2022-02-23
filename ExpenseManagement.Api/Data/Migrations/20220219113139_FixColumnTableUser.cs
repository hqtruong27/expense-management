using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpenseManagement.Api.Data.Migrations
{
    public partial class FixColumnTableUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LasUpdatedDate",
                table: "AspNetUsers",
                newName: "LastUpdatedDate");

            migrationBuilder.RenameColumn(
                name: "LasUpdatedBy",
                table: "AspNetUsers",
                newName: "LastUpdatedBy");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LastUpdatedDate",
                table: "AspNetUsers",
                newName: "LasUpdatedDate");

            migrationBuilder.RenameColumn(
                name: "LastUpdatedBy",
                table: "AspNetUsers",
                newName: "LasUpdatedBy");
        }
    }
}
