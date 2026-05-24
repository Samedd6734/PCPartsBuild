using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCPartsAPI.Models
{
    public class Motherboard
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(500)]
        public string EpeyUrl { get; set; } = string.Empty;

        [Required]
        [MaxLength(300)]
        public string ProductName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Brand { get; set; } = string.Empty;

        public decimal? Price { get; set; }

        [MaxLength(500)]
        public string ImageUrl { get; set; } = string.Empty;

        public int? EpeyScore { get; set; }

        // CPU Uyumluluğu
        [MaxLength(50)]
        public string SocketType { get; set; } = string.Empty;

        // Kasa Uyumluluğu
        [MaxLength(50)]
        public string FormFactor { get; set; } = string.Empty;

        // RAM Uyumluluğu
        [MaxLength(50)]
        public string MemoryType { get; set; } = string.Empty;

        public int MaxMemorySpeedMHz { get; set; }

        public int MaxMemoryCapacityGB { get; set; }

        public int MemorySlotCount { get; set; }

        // SSD Uyumluluğu
        public int M2SlotCount { get; set; }

        [MaxLength(200)]
        public string M2PCIeVersions { get; set; } = string.Empty;

        public int SataPortCount { get; set; }

        [MaxLength(50)]
        public string SataVersion { get; set; } = string.Empty;

        // GPU Uyumluluğu
        [MaxLength(50)]
        public string GpuPCIeVersion { get; set; } = string.Empty;

        public int PCIex16SlotCount { get; set; }

        // Genel Özellikler
        public bool SupportsOverclock { get; set; }

        // Ham Epey Verisi
        [Column(TypeName = "jsonb")]
        public string RawEpeyData { get; set; } = "{}";
    }
}