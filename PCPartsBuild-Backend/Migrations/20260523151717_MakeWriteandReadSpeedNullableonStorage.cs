using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PCPartsAPI.Migrations
{
    /// <inheritdoc />
    public partial class MakeWriteandReadSpeedNullableonStorage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "WriteSpeedMBs",
                schema: "PCPartsDB",
                table: "Storages",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "ReadSpeedMBs",
                schema: "PCPartsDB",
                table: "Storages",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "WriteSpeedMBs",
                schema: "PCPartsDB",
                table: "Storages",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ReadSpeedMBs",
                schema: "PCPartsDB",
                table: "Storages",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);
        }
    }
}
