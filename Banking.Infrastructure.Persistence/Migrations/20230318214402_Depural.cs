using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Banking.Infrastructure.Persistence.Migrations
{
    public partial class Depural : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transacciones_Productos_ProductID",
                table: "Transacciones");

            migrationBuilder.DropIndex(
                name: "IX_Transacciones_ProductID",
                table: "Transacciones");

            migrationBuilder.DropColumn(
                name: "ProductID",
                table: "Transacciones");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductID",
                table: "Transacciones",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transacciones_ProductID",
                table: "Transacciones",
                column: "ProductID");

            migrationBuilder.AddForeignKey(
                name: "FK_Transacciones_Productos_ProductID",
                table: "Transacciones",
                column: "ProductID",
                principalTable: "Productos",
                principalColumn: "id");
        }
    }
}
