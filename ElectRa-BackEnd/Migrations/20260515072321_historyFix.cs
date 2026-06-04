using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElectRa_BackEnd.Migrations
{
    /// <inheritdoc />
    public partial class historyFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VisitHistories_Products_productId",
                table: "VisitHistories");

            migrationBuilder.RenameColumn(
                name: "productId",
                table: "VisitHistories",
                newName: "ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_VisitHistories_productId",
                table: "VisitHistories",
                newName: "IX_VisitHistories_ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_VisitHistories_Products_ProductId",
                table: "VisitHistories",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VisitHistories_Products_ProductId",
                table: "VisitHistories");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "VisitHistories",
                newName: "productId");

            migrationBuilder.RenameIndex(
                name: "IX_VisitHistories_ProductId",
                table: "VisitHistories",
                newName: "IX_VisitHistories_productId");

            migrationBuilder.AddForeignKey(
                name: "FK_VisitHistories_Products_productId",
                table: "VisitHistories",
                column: "productId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
