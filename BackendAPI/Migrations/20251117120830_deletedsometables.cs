using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PCPartsAPI.Migrations
{
    /// <inheritdoc />
    public partial class deletedsometables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // BURAYI TAMAMEN BOŞALTIYORUZ.
            // Çünkü siz bu sütunları zaten pgAdmin'den elle sildiniz.
            // Burası boş kalınca EF Core veritabanına dokunmayacak, hata vermeyecek.
            // Sadece "Bu işlem yapıldı" diye not düşecek.
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Burası kalsın. Eğer ileride "vazgeçtim geri alayım" derseniz
            // bu kodlar çalışır ve sütunları geri ekler.
            migrationBuilder.AddColumn<bool>(
                name: "HeatsinkIncluded",
                table: "Storages",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "RadiatorThickness",
                table: "CpuCoolers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RamClearance",
                table: "CpuCoolers",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}