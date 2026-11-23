using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PCPartsAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialNeon : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "PCPartsDB");

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    FullName = table.Column<string>(type: "text", nullable: false),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AssistantSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: true),
                    TotalBudget = table.Column<decimal>(type: "numeric", nullable: false),
                    RemainingBudget = table.Column<decimal>(type: "numeric", nullable: false),
                    Purpose = table.Column<string>(type: "text", nullable: false),
                    CurrentStep = table.Column<int>(type: "integer", nullable: false),
                    SelectedComponentsJson = table.Column<string>(type: "jsonb", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssistantSessions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cases",
                schema: "PCPartsDB",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EpeyUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ProductName = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Brand = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Price = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    ImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    EpeyScore = table.Column<int>(type: "integer", nullable: true),
                    SupportedMotherboardFormFactors = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    MaxGPULengthMm = table.Column<int>(type: "integer", nullable: false),
                    MaxCPUCoolerHeightMm = table.Column<int>(type: "integer", nullable: false),
                    TopRadiatorSupportMm = table.Column<int>(type: "integer", nullable: false),
                    FrontRadiatorSupportMm = table.Column<int>(type: "integer", nullable: false),
                    FanCapacity = table.Column<int>(type: "integer", nullable: false),
                    MaxFanSizeMm = table.Column<int>(type: "integer", nullable: false),
                    HasBuiltInPSU = table.Column<bool>(type: "boolean", nullable: false),
                    BuiltInPSUWattage = table.Column<int>(type: "integer", nullable: true),
                    Drive25Bays = table.Column<int>(type: "integer", nullable: false),
                    Drive35Bays = table.Column<int>(type: "integer", nullable: false),
                    RawEpeyData = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cases", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CpuCoolers",
                schema: "PCPartsDB",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EpeyUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ProductName = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Brand = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Price = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    ImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    EpeyScore = table.Column<int>(type: "integer", nullable: true),
                    CoolerType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    TDPCapacityW = table.Column<int>(type: "integer", nullable: false),
                    SupportedSockets = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    HeightMm = table.Column<int>(type: "integer", nullable: false),
                    RadiatorSizeMm = table.Column<int>(type: "integer", nullable: false),
                    FanSizeMm = table.Column<int>(type: "integer", nullable: false),
                    FanCount = table.Column<int>(type: "integer", nullable: false),
                    RawEpeyData = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CpuCoolers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Favorites",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    ComponentType = table.Column<string>(type: "text", nullable: false),
                    ComponentId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Favorites", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Gpus",
                schema: "PCPartsDB",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EpeyUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ProductName = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Brand = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Price = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    ImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    EpeyScore = table.Column<int>(type: "integer", nullable: true),
                    GpuChip = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    ChipManufacturer = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    VRAMGB = table.Column<int>(type: "integer", nullable: false),
                    MemoryType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PCIeInterface = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PassMarkScore = table.Column<int>(type: "integer", nullable: true),
                    LengthMm = table.Column<int>(type: "integer", nullable: false),
                    ThicknessMm = table.Column<int>(type: "integer", nullable: false),
                    FanCount = table.Column<int>(type: "integer", nullable: false),
                    RecommendedPSUW = table.Column<int>(type: "integer", nullable: false),
                    TDPWatt = table.Column<int>(type: "integer", nullable: false),
                    PowerConnectors = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    RawEpeyData = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gpus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Motherboards",
                schema: "PCPartsDB",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EpeyUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ProductName = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Brand = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Price = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    ImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    EpeyScore = table.Column<int>(type: "integer", nullable: true),
                    SocketType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    FormFactor = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    MemoryType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    MaxMemorySpeedMHz = table.Column<int>(type: "integer", nullable: false),
                    MaxMemoryCapacityGB = table.Column<int>(type: "integer", nullable: false),
                    MemorySlotCount = table.Column<int>(type: "integer", nullable: false),
                    M2SlotCount = table.Column<int>(type: "integer", nullable: false),
                    M2PCIeVersions = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    SataPortCount = table.Column<int>(type: "integer", nullable: false),
                    SataVersion = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    GpuPCIeVersion = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PCIex16SlotCount = table.Column<int>(type: "integer", nullable: false),
                    SupportsOverclock = table.Column<bool>(type: "boolean", nullable: false),
                    RawEpeyData = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Motherboards", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Processors",
                schema: "PCPartsDB",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EpeyUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ProductName = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Brand = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Price = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    ImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    EpeyScore = table.Column<int>(type: "integer", nullable: true),
                    SocketType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    SupportedMemoryType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    MaxMemorySpeedMHz = table.Column<int>(type: "integer", nullable: false),
                    PCIeVersion = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    TDP = table.Column<int>(type: "integer", nullable: false),
                    HasIntegratedGraphics = table.Column<bool>(type: "boolean", nullable: false),
                    CoreCount = table.Column<int>(type: "integer", nullable: false),
                    ThreadCount = table.Column<int>(type: "integer", nullable: false),
                    BaseClockGHz = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    BoostClockGHz = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    L3CacheMB = table.Column<int>(type: "integer", nullable: false),
                    PassMarkScoreMulti = table.Column<int>(type: "integer", nullable: true),
                    PassMarkScoreSingle = table.Column<int>(type: "integer", nullable: true),
                    RawEpeyData = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Processors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Psus",
                schema: "PCPartsDB",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EpeyUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ProductName = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Brand = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Price = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    ImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    EpeyScore = table.Column<int>(type: "integer", nullable: true),
                    WattageW = table.Column<int>(type: "integer", nullable: false),
                    FormFactor = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Certification = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ATXVersion = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IsModular = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    RawEpeyData = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Psus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Rams",
                schema: "PCPartsDB",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EpeyUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ProductName = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Brand = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Price = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    ImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    EpeyScore = table.Column<int>(type: "integer", nullable: true),
                    MemoryType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CapacityGB = table.Column<int>(type: "integer", nullable: false),
                    ModuleConfig = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    SpeedMHz = table.Column<int>(type: "integer", nullable: false),
                    CasLatency = table.Column<int>(type: "integer", nullable: false),
                    Voltage = table.Column<decimal>(type: "numeric(4,2)", precision: 4, scale: 2, nullable: false),
                    HeightMm = table.Column<int>(type: "integer", nullable: false),
                    RawEpeyData = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rams", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SavedBuilds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    BuildName = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CpuId = table.Column<int>(type: "integer", nullable: true),
                    MotherboardId = table.Column<int>(type: "integer", nullable: true),
                    RamId = table.Column<int>(type: "integer", nullable: true),
                    GpuId = table.Column<int>(type: "integer", nullable: true),
                    StorageId = table.Column<int>(type: "integer", nullable: true),
                    CaseId = table.Column<int>(type: "integer", nullable: true),
                    PsuId = table.Column<int>(type: "integer", nullable: true),
                    CpuCoolerId = table.Column<int>(type: "integer", nullable: true),
                    TotalPrice = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavedBuilds", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Storages",
                schema: "PCPartsDB",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EpeyUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ProductName = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Brand = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Price = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    ImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    EpeyScore = table.Column<int>(type: "integer", nullable: true),
                    FormFactor = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Interface = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CapacityGB = table.Column<int>(type: "integer", nullable: false),
                    ReadSpeedMBs = table.Column<int>(type: "integer", nullable: false),
                    WriteSpeedMBs = table.Column<int>(type: "integer", nullable: false),
                    RawEpeyData = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Storages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    RoleId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssistantSessions_UserId",
                table: "AssistantSessions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Cases_EpeyUrl",
                schema: "PCPartsDB",
                table: "Cases",
                column: "EpeyUrl",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CpuCoolers_EpeyUrl",
                schema: "PCPartsDB",
                table: "CpuCoolers",
                column: "EpeyUrl",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Gpus_EpeyUrl",
                schema: "PCPartsDB",
                table: "Gpus",
                column: "EpeyUrl",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Motherboards_EpeyUrl",
                schema: "PCPartsDB",
                table: "Motherboards",
                column: "EpeyUrl",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Processors_EpeyUrl",
                schema: "PCPartsDB",
                table: "Processors",
                column: "EpeyUrl",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Psus_EpeyUrl",
                schema: "PCPartsDB",
                table: "Psus",
                column: "EpeyUrl",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Rams_EpeyUrl",
                schema: "PCPartsDB",
                table: "Rams",
                column: "EpeyUrl",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Storages_EpeyUrl",
                schema: "PCPartsDB",
                table: "Storages",
                column: "EpeyUrl",
                unique: true);
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
                name: "AssistantSessions");

            migrationBuilder.DropTable(
                name: "Cases",
                schema: "PCPartsDB");

            migrationBuilder.DropTable(
                name: "CpuCoolers",
                schema: "PCPartsDB");

            migrationBuilder.DropTable(
                name: "Favorites");

            migrationBuilder.DropTable(
                name: "Gpus",
                schema: "PCPartsDB");

            migrationBuilder.DropTable(
                name: "Motherboards",
                schema: "PCPartsDB");

            migrationBuilder.DropTable(
                name: "Processors",
                schema: "PCPartsDB");

            migrationBuilder.DropTable(
                name: "Psus",
                schema: "PCPartsDB");

            migrationBuilder.DropTable(
                name: "Rams",
                schema: "PCPartsDB");

            migrationBuilder.DropTable(
                name: "SavedBuilds");

            migrationBuilder.DropTable(
                name: "Storages",
                schema: "PCPartsDB");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
