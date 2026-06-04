using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElectRa_BackEnd.Migrations
{
    /// <inheritdoc />
    public partial class categoryIcons : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Icon",
                table: "SubCategories",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Icon",
                table: "SubCategories");
        }
    }
}
