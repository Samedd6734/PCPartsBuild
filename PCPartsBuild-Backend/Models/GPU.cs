using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCPartsAPI.Models
{
    public class Gpu
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

        // Chip / Performans
        [MaxLength(150)]
        public string GpuChip { get; set; } = string.Empty;

        [MaxLength(50)]
        public string ChipManufacturer { get; set; } = string.Empty;

        public int VRAMGB { get; set; }

        [MaxLength(50)]
        public string MemoryType { get; set; } = string.Empty;

        [MaxLength(20)]
        public string PCIeInterface { get; set; } = string.Empty;

        public int? PassMarkScore { get; set; }

        // Fiziksel Boyutlar (Kasa Uyumluluğu)
        public int LengthMm { get; set; }

        public int ThicknessMm { get; set; }

        public int FanCount { get; set; }

        // Güç / PSU Uyumluluğu
        public int RecommendedPSUW { get; set; }

        public int TDPWatt { get; set; }

        [MaxLength(200)]
        public string PowerConnectors { get; set; } = string.Empty;

        // Ham Epey Verisi
        [Column(TypeName = "jsonb")]
        public string RawEpeyData { get; set; } = "{}";
    }
}