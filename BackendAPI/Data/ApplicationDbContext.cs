using Microsoft.AspNetCore.Identity.EntityFrameworkCore; // <-- 1. BU SATIRI EKLE
using Microsoft.EntityFrameworkCore;
using PCPartsAPI.Models;

namespace PCPartsAPI.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Tüm PC Parçası tabloların
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