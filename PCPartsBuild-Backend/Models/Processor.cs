using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCPartsAPI.Models
{
    public class Processor
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

        // Uyumluluk Kritik Alanlar
        [MaxLength(50)]
        public string SocketType { get; set; } = string.Empty;

        [MaxLength(100)]
        public string SupportedMemoryType { get; set; } = string.Empty;

        public int MaxMemorySpeedMHz { get; set; }

        [MaxLength(50)]
        public string PCIeVersion { get; set; } = string.Empty;

        public int TDP { get; set; }

        public bool HasIntegratedGraphics { get; set; }

        // Performans / Darboğaz Analizi
        public int CoreCount { get; set; }

        public int ThreadCount { get; set; }

        public decimal BaseClockGHz { get; set; }

        public decimal BoostClockGHz { get; set; }

        public int L3CacheMB { get; set; }

        public int? PassMarkScoreMulti { get; set; }

        public int? PassMarkScoreSingle { get; set; }

        // Ham Epey Verisi
        [Column(TypeName = "jsonb")]
        public string RawEpeyData { get; set; } = "{}";
    }
}