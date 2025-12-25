using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PCPartsAPI.Models; // AppUser burada olduğu için ekliyoruz

namespace PCPartsAPI.Data
{
    // DİKKAT: Buraya <AppUser> ekledik
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Diğer tabloların aynen kalıyor
        public DbSet<Processor> Processors { get; set; }
        public DbSet<Motherboard> Motherboards { get; set; }
        public DbSet<Gpu> Gpus { get; set; }
        public DbSet<Ram> Rams { get; set; }
        public DbSet<Storage> Storages { get; set; }
        public DbSet<Psu> Psus { get; set; }
        public DbSet<Case> Cases { get; set; }
        public DbSet<CpuCooler> CpuCoolers { get; set; }
        public DbSet<Favorites> Favorites { get; set; }
        public DbSet<SavedBuilds> SavedBuilds { get; set; }
    }
}