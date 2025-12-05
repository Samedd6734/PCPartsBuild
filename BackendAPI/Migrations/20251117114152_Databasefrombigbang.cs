using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PCPartsAPI.Migrations
{
    /// <inheritdoc />
    public partial class Databasefrombigbang : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ==============================================================================
            // 1. PROCESSORS (İŞLEMCİLER)
            // ==============================================================================
            // Tablo yoksa oluştur (Boş şablon)
            migrationBuilder.Sql("CREATE TABLE IF NOT EXISTS \"Processors\" (\"Id\" SERIAL PRIMARY KEY);");

            // Sütunları kontrol et ve ekle
            migrationBuilder.Sql("ALTER TABLE \"Processors\" ADD COLUMN IF NOT EXISTS \"Brand\" text NULL;");
            migrationBuilder.Sql("ALTER TABLE \"Processors\" ADD COLUMN IF NOT EXISTS \"ModelName\" text NULL;");
            migrationBuilder.Sql("ALTER TABLE \"Processors\" ADD COLUMN IF NOT EXISTS \"Socket\" text NULL;");
            migrationBuilder.Sql("ALTER TABLE \"Processors\" ADD COLUMN IF NOT EXISTS \"CoreCount\" integer NOT NULL DEFAULT 0;");
            migrationBuilder.Sql("ALTER TABLE \"Processors\" ADD COLUMN IF NOT EXISTS \"ThreadCount\" integer NOT NULL DEFAULT 0;");
            migrationBuilder.Sql("ALTER TABLE \"Processors\" ADD COLUMN IF NOT EXISTS \"BaseClockSpeed\" double precision NOT NULL DEFAULT 0;");
            migrationBuilder.Sql("ALTER TABLE \"Processors\" ADD COLUMN IF NOT EXISTS \"BoostClockSpeed\" double precision NOT NULL DEFAULT 0;");
            migrationBuilder.Sql("ALTER TABLE \"Processors\" ADD COLUMN IF NOT EXISTS \"L3Cache\" integer NOT NULL DEFAULT 0;");
            migrationBuilder.Sql("ALTER TABLE \"Processors\" ADD COLUMN IF NOT EXISTS \"Tdp\" integer NOT NULL DEFAULT 0;");
            migrationBuilder.Sql("ALTER TABLE \"Processors\" ADD COLUMN IF NOT EXISTS \"IntegratedGraphics\" boolean NOT NULL DEFAULT FALSE;");
            migrationBuilder.Sql("ALTER TABLE \"Processors\" ADD COLUMN IF NOT EXISTS \"SupportedMemoryTypes\" text NULL;");
            migrationBuilder.Sql("ALTER TABLE \"Processors\" ADD COLUMN IF NOT EXISTS \"MaxMemoryCapacity\" integer NOT NULL DEFAULT 0;");

            // ==============================================================================
            // 2. MOTHERBOARDS (ANAKARTLAR)
            // ==============================================================================
            migrationBuilder.Sql("CREATE TABLE IF NOT EXISTS \"Motherboards\" (\"Id\" SERIAL PRIMARY KEY);");

            migrationBuilder.Sql("ALTER TABLE \"Motherboards\" ADD COLUMN IF NOT EXISTS \"Brand\" text NULL;");
            migrationBuilder.Sql("ALTER TABLE \"Motherboards\" ADD COLUMN IF NOT EXISTS \"ModelName\" text NULL;");
            migrationBuilder.Sql("ALTER TABLE \"Motherboards\" ADD COLUMN IF NOT EXISTS \"Socket\" text NULL;");
            migrationBuilder.Sql("ALTER TABLE \"Motherboards\" ADD COLUMN IF NOT EXISTS \"Chipset\" text NULL;");
            migrationBuilder.Sql("ALTER TABLE \"Motherboards\" ADD COLUMN IF NOT EXISTS \"FormFactor\" text NULL;");
            migrationBuilder.Sql("ALTER TABLE \"Motherboards\" ADD COLUMN IF NOT EXISTS \"MemoryType\" text NULL;");
            migrationBuilder.Sql("ALTER TABLE \"Motherboards\" ADD COLUMN IF NOT EXISTS \"MemorySlots\" integer NOT NULL DEFAULT 0;");
            migrationBuilder.Sql("ALTER TABLE \"Motherboards\" ADD COLUMN IF NOT EXISTS \"MaxMemory\" integer NOT NULL DEFAULT 0;");
            // Liste tipi (Array)
            migrationBuilder.Sql("ALTER TABLE \"Motherboards\" ADD COLUMN IF NOT EXISTS \"SupportedMemoryFrequencies\" integer[] NULL;");

            migrationBuilder.Sql("ALTER TABLE \"Motherboards\" ADD COLUMN IF NOT EXISTS \"CpuPowerConnectors\" text NULL;");
            migrationBuilder.Sql("ALTER TABLE \"Motherboards\" ADD COLUMN IF NOT EXISTS \"AtxPowerConnector\" integer NOT NULL DEFAULT 24;");
            migrationBuilder.Sql("ALTER TABLE \"Motherboards\" ADD COLUMN IF NOT EXISTS \"Pcie16xVersion\" text NULL;");
            migrationBuilder.Sql("ALTER TABLE \"Motherboards\" ADD COLUMN IF NOT EXISTS \"M2SlotCount\" integer NOT NULL DEFAULT 0;");
            migrationBuilder.Sql("ALTER TABLE \"Motherboards\" ADD COLUMN IF NOT EXISTS \"SataPortCount\" integer NOT NULL DEFAULT 0;");
            migrationBuilder.Sql("ALTER TABLE \"Motherboards\" ADD COLUMN IF NOT EXISTS \"InternalUsb2Headers\" integer NOT NULL DEFAULT 0;");
            migrationBuilder.Sql("ALTER TABLE \"Motherboards\" ADD COLUMN IF NOT EXISTS \"InternalUsb3Headers\" integer NOT NULL DEFAULT 0;");
            migrationBuilder.Sql("ALTER TABLE \"Motherboards\" ADD COLUMN IF NOT EXISTS \"InternalTypeCHeader\" integer NOT NULL DEFAULT 0;");
            migrationBuilder.Sql("ALTER TABLE \"Motherboards\" ADD COLUMN IF NOT EXISTS \"FanHeaders\" integer NOT NULL DEFAULT 0;");
            migrationBuilder.Sql("ALTER TABLE \"Motherboards\" ADD COLUMN IF NOT EXISTS \"ArgbSupport\" boolean NOT NULL DEFAULT FALSE;");
            migrationBuilder.Sql("ALTER TABLE \"Motherboards\" ADD COLUMN IF NOT EXISTS \"IntegratedWifi\" boolean NOT NULL DEFAULT FALSE;");
            migrationBuilder.Sql("ALTER TABLE \"Motherboards\" ADD COLUMN IF NOT EXISTS \"IntegratedBluetooth\" boolean NOT NULL DEFAULT FALSE;");

            // ==============================================================================
            // 3. STORAGES (DEPOLAMA)
            // ==============================================================================
            migrationBuilder.Sql("CREATE TABLE IF NOT EXISTS \"Storages\" (\"Id\" SERIAL PRIMARY KEY);");

            migrationBuilder.Sql("ALTER TABLE \"Storages\" ADD COLUMN IF NOT EXISTS \"Brand\" text NULL;");
            migrationBuilder.Sql("ALTER TABLE \"Storages\" ADD COLUMN IF NOT EXISTS \"ModelName\" text NULL;");
            migrationBuilder.Sql("ALTER TABLE \"Storages\" ADD COLUMN IF NOT EXISTS \"StorageType\" text NULL;");
            migrationBuilder.Sql("ALTER TABLE \"Storages\" ADD COLUMN IF NOT EXISTS \"Capacity\" integer NOT NULL DEFAULT 0;");
            // CapacityFormatted veritabanına eklenmez ([NotMapped] olduğu için)
            migrationBuilder.Sql("ALTER TABLE \"Storages\" ADD COLUMN IF NOT EXISTS \"ReadSpeed\" integer NOT NULL DEFAULT 0;");
            migrationBuilder.Sql("ALTER TABLE \"Storages\" ADD COLUMN IF NOT EXISTS \"WriteSpeed\" integer NOT NULL DEFAULT 0;");
            migrationBuilder.Sql("ALTER TABLE \"Storages\" ADD COLUMN IF NOT EXISTS \"FormFactor\" text NULL;");
            migrationBuilder.Sql("ALTER TABLE \"Storages\" ADD COLUMN IF NOT EXISTS \"Interface\" text NULL;");
            migrationBuilder.Sql("ALTER TABLE \"Storages\" ADD COLUMN IF NOT EXISTS \"IsNvme\" boolean NOT NULL DEFAULT FALSE;");
            migrationBuilder.Sql("ALTER TABLE \"Storages\" ADD COLUMN IF NOT EXISTS \"HasDramCache\" boolean NOT NULL DEFAULT FALSE;");
            migrationBuilder.Sql("ALTER TABLE \"Storages\" ADD COLUMN IF NOT EXISTS \"NandType\" text NULL;");
            migrationBuilder.Sql("ALTER TABLE \"Storages\" ADD COLUMN IF NOT EXISTS \"Tbw\" integer NOT NULL DEFAULT 0;");
            migrationBuilder.Sql("ALTER TABLE \"Storages\" ADD COLUMN IF NOT EXISTS \"HeatsinkIncluded\" boolean NOT NULL DEFAULT FALSE;");
            migrationBuilder.Sql("ALTER TABLE \"Storages\" ADD COLUMN IF NOT EXISTS \"Rpm\" integer NOT NULL DEFAULT 0;");
            migrationBuilder.Sql("ALTER TABLE \"Storages\" ADD COLUMN IF NOT EXISTS \"CacheSizeMB\" integer NOT NULL DEFAULT 0;");

            // ==============================================================================
            // 4. GPUS (EKRAN KARTLARI)
            // ==============================================================================
            migrationBuilder.Sql("CREATE TABLE IF NOT EXISTS \"Gpus\" (\"Id\" SERIAL PRIMARY KEY);");

            migrationBuilder.Sql("ALTER TABLE \"Gpus\" ADD COLUMN IF NOT EXISTS \"Brand\" text NULL;");
            migrationBuilder.Sql("ALTER TABLE \"Gpus\" ADD COLUMN IF NOT EXISTS \"ChipsetBrand\" text NULL;");
            migrationBuilder.Sql("ALTER TABLE \"Gpus\" ADD COLUMN IF NOT EXISTS \"ModelName\" text NULL;");
            migrationBuilder.Sql("ALTER TABLE \"Gpus\" ADD COLUMN IF NOT EXISTS \"Length\" integer NOT NULL DEFAULT 0;");
            migrationBuilder.Sql("ALTER TABLE \"Gpus\" ADD COLUMN IF NOT EXISTS \"Height\" integer NOT NULL DEFAULT 0;");
            migrationBuilder.Sql("ALTER TABLE \"Gpus\" ADD COLUMN IF NOT EXISTS \"Tdp\" integer NOT NULL DEFAULT 0;");
            migrationBuilder.Sql("ALTER TABLE \"Gpus\" ADD COLUMN IF NOT EXISTS \"RecommendedPsu\" integer NOT NULL DEFAULT 0;");
            migrationBuilder.Sql("ALTER TABLE \"Gpus\" ADD COLUMN IF NOT EXISTS \"PowerConnectors\" text NULL;");
            migrationBuilder.Sql("ALTER TABLE \"Gpus\" ADD COLUMN IF NOT EXISTS \"Interface\" text NULL;");
            migrationBuilder.Sql("ALTER TABLE \"Gpus\" ADD COLUMN IF NOT EXISTS \"VRAMMemorySize\" integer NOT NULL DEFAULT 0;");
            migrationBuilder.Sql("ALTER TABLE \"Gpus\" ADD COLUMN IF NOT EXISTS \"MemoryType\" text NULL;");
            migrationBuilder.Sql("ALTER TABLE \"Gpus\" ADD COLUMN IF NOT EXISTS \"BoostClock\" integer NOT NULL DEFAULT 0;");

            // ==============================================================================
            // 5. CASES (KASALAR)
            // ==============================================================================
            migrationBuilder.Sql("CREATE TABLE IF NOT EXISTS \"Cases\" (\"Id\" SERIAL PRIMARY KEY);");

            migrationBuilder.Sql("ALTER TABLE \"Cases\" ADD COLUMN IF NOT EXISTS \"Brand\" text NULL;");
            migrationBuilder.Sql("ALTER TABLE \"Cases\" ADD COLUMN IF NOT EXISTS \"ModelName\" text NULL;");
            migrationBuilder.Sql("ALTER TABLE \"Cases\" ADD COLUMN IF NOT EXISTS \"CaseType\" text NULL;");
            migrationBuilder.Sql("ALTER TABLE \"Cases\" ADD COLUMN IF NOT EXISTS \"SupportedMotherboards\" text NULL;");
            migrationBuilder.Sql("ALTER TABLE \"Cases\" ADD COLUMN IF NOT EXISTS \"MaxGpuLength\" integer NOT NULL DEFAULT 0;");
            migrationBuilder.Sql("ALTER TABLE \"Cases\" ADD COLUMN IF NOT EXISTS \"MaxCpuCoolerHeight\" integer NOT NULL DEFAULT 0;");
            migrationBuilder.Sql("ALTER TABLE \"Cases\" ADD COLUMN IF NOT EXISTS \"MaxPsuLength\" integer NOT NULL DEFAULT 0;");
            migrationBuilder.Sql("ALTER TABLE \"Cases\" ADD COLUMN IF NOT EXISTS \"RadiatorSupportFront\" text NULL;");
            migrationBuilder.Sql("ALTER TABLE \"Cases\" ADD COLUMN IF NOT EXISTS \"RadiatorSupportTop\" text NULL;");
            migrationBuilder.Sql("ALTER TABLE \"Cases\" ADD COLUMN IF NOT EXISTS \"HasTypeC\" boolean NOT NULL DEFAULT FALSE;");
            migrationBuilder.Sql("ALTER TABLE \"Cases\" ADD COLUMN IF NOT EXISTS \"Usb3Count\" integer NOT NULL DEFAULT 0;");

            // ==============================================================================
            // 6. CPU COOLERS (İŞLEMCİ SOĞUTUCULARI)
            // ==============================================================================
            migrationBuilder.Sql("CREATE TABLE IF NOT EXISTS \"CpuCoolers\" (\"Id\" SERIAL PRIMARY KEY);");

            migrationBuilder.Sql("ALTER TABLE \"CpuCoolers\" ADD COLUMN IF NOT EXISTS \"Brand\" text NULL;");
            migrationBuilder.Sql("ALTER TABLE \"CpuCoolers\" ADD COLUMN IF NOT EXISTS \"ModelName\" text NULL;");
            migrationBuilder.Sql("ALTER TABLE \"CpuCoolers\" ADD COLUMN IF NOT EXISTS \"CoolerType\" text NULL;");
            migrationBuilder.Sql("ALTER TABLE \"CpuCoolers\" ADD COLUMN IF NOT EXISTS \"TdpRating\" integer NOT NULL DEFAULT 0;");
            migrationBuilder.Sql("ALTER TABLE \"CpuCoolers\" ADD COLUMN IF NOT EXISTS \"SupportedSockets\" text NULL;");
            migrationBuilder.Sql("ALTER TABLE \"CpuCoolers\" ADD COLUMN IF NOT EXISTS \"Height\" integer NOT NULL DEFAULT 0;");
            migrationBuilder.Sql("ALTER TABLE \"CpuCoolers\" ADD COLUMN IF NOT EXISTS \"RadiatorSize\" integer NOT NULL DEFAULT 0;");
            migrationBuilder.Sql("ALTER TABLE \"CpuCoolers\" ADD COLUMN IF NOT EXISTS \"RadiatorThickness\" integer NOT NULL DEFAULT 0;");
            migrationBuilder.Sql("ALTER TABLE \"CpuCoolers\" ADD COLUMN IF NOT EXISTS \"RamClearance\" integer NOT NULL DEFAULT 0;");
            migrationBuilder.Sql("ALTER TABLE \"CpuCoolers\" ADD COLUMN IF NOT EXISTS \"FanSize\" integer NOT NULL DEFAULT 0;");
            migrationBuilder.Sql("ALTER TABLE \"CpuCoolers\" ADD COLUMN IF NOT EXISTS \"FanCount\" integer NOT NULL DEFAULT 0;");
            migrationBuilder.Sql("ALTER TABLE \"CpuCoolers\" ADD COLUMN IF NOT EXISTS \"MaxFanSpeed\" integer NOT NULL DEFAULT 0;");
            migrationBuilder.Sql("ALTER TABLE \"CpuCoolers\" ADD COLUMN IF NOT EXISTS \"NoiseLevel\" double precision NOT NULL DEFAULT 0;");
            migrationBuilder.Sql("ALTER TABLE \"CpuCoolers\" ADD COLUMN IF NOT EXISTS \"MaxAirflow\" double precision NOT NULL DEFAULT 0;");
            migrationBuilder.Sql("ALTER TABLE \"CpuCoolers\" ADD COLUMN IF NOT EXISTS \"HasRgb\" boolean NOT NULL DEFAULT FALSE;");
            migrationBuilder.Sql("ALTER TABLE \"CpuCoolers\" ADD COLUMN IF NOT EXISTS \"RgbConnectorType\" text NULL;");
            migrationBuilder.Sql("ALTER TABLE \"CpuCoolers\" ADD COLUMN IF NOT EXISTS \"RequiresInternalUsbHeader\" boolean NOT NULL DEFAULT FALSE;");

            // ==============================================================================
            // 7. PSUS (GÜÇ KAYNAKLARI)
            // ==============================================================================
            migrationBuilder.Sql("CREATE TABLE IF NOT EXISTS \"Psus\" (\"Id\" SERIAL PRIMARY KEY);");

            migrationBuilder.Sql("ALTER TABLE \"Psus\" ADD COLUMN IF NOT EXISTS \"Brand\" text NULL;");
            migrationBuilder.Sql("ALTER TABLE \"Psus\" ADD COLUMN IF NOT EXISTS \"ModelName\" text NULL;");
            migrationBuilder.Sql("ALTER TABLE \"Psus\" ADD COLUMN IF NOT EXISTS \"Wattage\" integer NOT NULL DEFAULT 0;");
            migrationBuilder.Sql("ALTER TABLE \"Psus\" ADD COLUMN IF NOT EXISTS \"Rating\" text NULL;");
            migrationBuilder.Sql("ALTER TABLE \"Psus\" ADD COLUMN IF NOT EXISTS \"FormFactor\" text NULL;");
            migrationBuilder.Sql("ALTER TABLE \"Psus\" ADD COLUMN IF NOT EXISTS \"IsModular\" boolean NOT NULL DEFAULT FALSE;");
            migrationBuilder.Sql("ALTER TABLE \"Psus\" ADD COLUMN IF NOT EXISTS \"Length\" integer NOT NULL DEFAULT 0;");
            migrationBuilder.Sql("ALTER TABLE \"Psus\" ADD COLUMN IF NOT EXISTS \"Eps8PinCount\" integer NOT NULL DEFAULT 0;");
            migrationBuilder.Sql("ALTER TABLE \"Psus\" ADD COLUMN IF NOT EXISTS \"Pcie8PinCount\" integer NOT NULL DEFAULT 0;");
            migrationBuilder.Sql("ALTER TABLE \"Psus\" ADD COLUMN IF NOT EXISTS \"Has12VHPWR\" boolean NOT NULL DEFAULT FALSE;");
            migrationBuilder.Sql("ALTER TABLE \"Psus\" ADD COLUMN IF NOT EXISTS \"SataCount\" integer NOT NULL DEFAULT 0;");

            // ==============================================================================
            // 8. RAMS (BELLEKLER)
            // ==============================================================================
            migrationBuilder.Sql("CREATE TABLE IF NOT EXISTS \"Rams\" (\"Id\" SERIAL PRIMARY KEY);");
            migrationBuilder.Sql("ALTER TABLE \"Rams\" ADD COLUMN IF NOT EXISTS \"Brand\" text NULL;");
            migrationBuilder.Sql("ALTER TABLE \"Rams\" ADD COLUMN IF NOT EXISTS \"ModelName\" text NULL;");
            migrationBuilder.Sql("ALTER TABLE \"Rams\" ADD COLUMN IF NOT EXISTS \"MemoryType\" text NULL;");
            migrationBuilder.Sql("ALTER TABLE \"Rams\" ADD COLUMN IF NOT EXISTS \"Speed\" integer NOT NULL DEFAULT 0;");
            migrationBuilder.Sql("ALTER TABLE \"Rams\" ADD COLUMN IF NOT EXISTS \"ModuleCount\" integer NOT NULL DEFAULT 0;");
            migrationBuilder.Sql("ALTER TABLE \"Rams\" ADD COLUMN IF NOT EXISTS \"CapacityPerModule\" integer NOT NULL DEFAULT 0;");
            migrationBuilder.Sql("ALTER TABLE \"Rams\" ADD COLUMN IF NOT EXISTS \"CasLatency\" integer NOT NULL DEFAULT 0;");
            migrationBuilder.Sql("ALTER TABLE \"Rams\" ADD COLUMN IF NOT EXISTS \"HasRgb\" boolean NOT NULL DEFAULT FALSE;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Cases");

            migrationBuilder.DropTable(
                name: "CpuCoolers");

            migrationBuilder.DropTable(
                name: "Gpus");

            migrationBuilder.DropTable(
                name: "Motherboards");

            migrationBuilder.DropTable(
                name: "Processors");

            migrationBuilder.DropTable(
                name: "Psus");

            migrationBuilder.DropTable(
                name: "Rams");

            migrationBuilder.DropTable(
                name: "Storages");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
