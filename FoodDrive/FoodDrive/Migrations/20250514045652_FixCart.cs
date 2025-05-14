using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodDrive.Migrations
{
    /// <inheritdoc />
    public partial class FixCart : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CartId2",
                table: "CartItems");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CartId2",
                table: "CartItems",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
