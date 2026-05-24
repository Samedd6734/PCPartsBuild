using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCPartsAPI.Models
{
    public class Ram
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

        // Bellek Özellikleri
        [MaxLength(20)]
        public string MemoryType { get; set; } = string.Empty;

        public int CapacityGB { get; set; }

        [MaxLength(30)]
        public string ModuleConfig { get; set; } = string.Empty;

        public int SpeedMHz { get; set; }

        public int CasLatency { get; set; }

        public decimal Voltage { get; set; }

        // Fiziksel Boyut (Soğutucu Uyumluluğu)
        public int HeightMm { get; set; }

        // Ham Epey Verisi
        [Column(TypeName = "jsonb")]
        public string RawEpeyData { get; set; } = "{}";
    }
}