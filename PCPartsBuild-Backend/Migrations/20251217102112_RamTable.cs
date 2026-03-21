using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PCPartsAPI.Migrations
{
    /// <inheritdoc />
    public partial class RamTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasRgb",
                table: "Rams");

            migrationBuilder.RenameColumn(
                name: "CapacityPerModule",
                table: "Rams",
                newName: "TotalCapacity");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalCapacity",
                table: "Rams",
                newName: "CapacityPerModule");

            migrationBuilder.AddColumn<bool>(
                name: "HasRgb",
                table: "Rams",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
