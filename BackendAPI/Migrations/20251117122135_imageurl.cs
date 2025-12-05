using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PCPartsAPI.Migrations
{
    /// <inheritdoc />
    public partial class imageurl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Storages",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Rams",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Psus",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Processors",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Motherboards",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Gpus",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "CpuCoolers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Cases",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Storages");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Rams");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Psus");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Processors");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Motherboards");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Gpus");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "CpuCoolers");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Cases");
        }
    }
}
