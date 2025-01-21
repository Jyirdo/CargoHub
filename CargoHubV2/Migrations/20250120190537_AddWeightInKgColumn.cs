using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CargohubV2.Migrations
{
    /// <inheritdoc />
    public partial class AddWeightInKgColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "WeightInKg",
                table: "Items",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WeightInKg",
                table: "Items");
        }
    }
}
