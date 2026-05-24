using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PCPartsAPI.Models;

namespace PCPartsAPI.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Bileşen Tabloları (epey şeması — scraping ile paylaşımlı)
        public DbSet<Processor> Processors { get; set; }
        public DbSet<Motherboard> Motherboards { get; set; }
        public DbSet<Gpu> Gpus { get; set; }
        public DbSet<Ram> Rams { get; set; }
        public DbSet<Case> Cases { get; set; }
        public DbSet<Psu> Psus { get; set; }
        public DbSet<CpuCooler> CpuCoolers { get; set; }
        public DbSet<Storage> Storages { get; set; }

        // Kullanıcı Tabloları (public şema)
        public DbSet<Favorites> Favorites { get; set; }
        public DbSet<SavedBuilds> SavedBuilds { get; set; }

        // Asistan Oturumları
        public DbSet<AssistantSession> AssistantSessions { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // ─── Bileşen tabloları: PCPartsDB şeması ─────────────────────────────
            // Scraping projesi bu tablolara yazar, backend sadece okur.

            // Processor
            builder.Entity<Processor>(entity =>
            {
                entity.ToTable("Processors", "PCPartsDB");
                entity.HasIndex(e => e.EpeyUrl).IsUnique();
                entity.Property(e => e.RawEpeyData).HasColumnType("jsonb");
                entity.Property(e => e.Price).HasPrecision(18, 2);
                entity.Property(e => e.BaseClockGHz).HasPrecision(5, 2);
                entity.Property(e => e.BoostClockGHz).HasPrecision(5, 2);
            });

            // Motherboard
            builder.Entity<Motherboard>(entity =>
            {
                entity.ToTable("Motherboards", "PCPartsDB");
                entity.HasIndex(e => e.EpeyUrl).IsUnique();
                entity.Property(e => e.RawEpeyData).HasColumnType("jsonb");
                entity.Property(e => e.Price).HasPrecision(18, 2);
            });

            // GPU
            builder.Entity<Gpu>(entity =>
            {
                entity.ToTable("Gpus", "PCPartsDB");
                entity.HasIndex(e => e.EpeyUrl).IsUnique();
                entity.Property(e => e.RawEpeyData).HasColumnType("jsonb");
                entity.Property(e => e.Price).HasPrecision(18, 2);
            });

            // RAM
            builder.Entity<Ram>(entity =>
            {
                entity.ToTable("Rams", "PCPartsDB");
                entity.HasIndex(e => e.EpeyUrl).IsUnique();
                entity.Property(e => e.RawEpeyData).HasColumnType("jsonb");
                entity.Property(e => e.Price).HasPrecision(18, 2);
                entity.Property(e => e.Voltage).HasPrecision(4, 2);
            });

            // Case
            builder.Entity<Case>(entity =>
            {
                entity.ToTable("Cases", "PCPartsDB");
                entity.HasIndex(e => e.EpeyUrl).IsUnique();
                entity.Property(e => e.RawEpeyData).HasColumnType("jsonb");
                entity.Property(e => e.Price).HasPrecision(18, 2);
            });

            // PSU
            builder.Entity<Psu>(entity =>
            {
                entity.ToTable("Psus", "PCPartsDB");
                entity.HasIndex(e => e.EpeyUrl).IsUnique();
                entity.Property(e => e.RawEpeyData).HasColumnType("jsonb");
                entity.Property(e => e.Price).HasPrecision(18, 2);
            });

            // CpuCooler
            builder.Entity<CpuCooler>(entity =>
            {
                entity.ToTable("CpuCoolers", "PCPartsDB");
                entity.HasIndex(e => e.EpeyUrl).IsUnique();
                entity.Property(e => e.RawEpeyData).HasColumnType("jsonb");
                entity.Property(e => e.Price).HasPrecision(18, 2);
            });

            // Storage
            builder.Entity<Storage>(entity =>
            {
                entity.ToTable("Storages", "PCPartsDB");
                entity.HasIndex(e => e.EpeyUrl).IsUnique();
                entity.Property(e => e.RawEpeyData).HasColumnType("jsonb");
                entity.Property(e => e.Price).HasPrecision(18, 2);
            });

            // ─── Kullanıcı tabloları: public şema ───────────────────────────

            // AssistantSession
            builder.Entity<AssistantSession>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.SelectedComponentsJson).HasColumnType("jsonb");
                entity.Property(e => e.CurrentStep).HasConversion<int>();
                entity.HasIndex(e => e.UserId);
            });
        }
    }
}