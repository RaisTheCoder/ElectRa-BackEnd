using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElectRa_BackEnd.Migrations
{
    /// <inheritdoc />
    public partial class favoritesRatesSales : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ReviewHelpfuls",
                table: "ReviewHelpfuls");

            migrationBuilder.DropIndex(
                name: "IX_ReviewHelpfuls_ReviewId_UserId",
                table: "ReviewHelpfuls");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ReviewHelpfuls");

            migrationBuilder.DropColumn(
                name: "Rating",
                table: "Products");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "OrderItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReviewHelpfuls",
                table: "ReviewHelpfuls",
                columns: new[] { "ReviewId", "UserId" });

            migrationBuilder.CreateTable(
                name: "ProductFavorites",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    ProductId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductFavorites", x => new { x.UserId, x.ProductId });
                    table.ForeignKey(
                        name: "FK_ProductFavorites_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductFavorites_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductFavorites_ProductId",
                table: "ProductFavorites",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductFavorites");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReviewHelpfuls",
                table: "ReviewHelpfuls");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "OrderItems");

            migrationBuilder.AddColumn<long>(
                name: "Id",
                table: "ReviewHelpfuls",
                type: "bigint",
                nullable: false,
                defaultValue: 0L)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<decimal>(
                name: "Rating",
                table: "Products",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReviewHelpfuls",
                table: "ReviewHelpfuls",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewHelpfuls_ReviewId_UserId",
                table: "ReviewHelpfuls",
                columns: new[] { "ReviewId", "UserId" },
                unique: true);
        }
    }
}
