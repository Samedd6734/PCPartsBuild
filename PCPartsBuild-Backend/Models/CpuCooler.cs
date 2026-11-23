using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCPartsAPI.Models
{
    public class CpuCooler
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(500)]
        public string EpeyUrl { get; set; } = string.Empty;

        [Required, MaxLength(300)]
        public string ProductName { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Brand { get; set; } = string.Empty;

        public decimal? Price { get; set; }

        [MaxLength(500)]
        public string ImageUrl { get; set; } = string.Empty;

        public int? EpeyScore { get; set; }

        // Soğutucu Tipi
        [MaxLength(20)]
        public string CoolerType { get; set; } = string.Empty;

        // Performans Uyumluluğu (TDP)
        public int TDPCapacityW { get; set; }

        // Soket Uyumluluğu
        [MaxLength(300)]
        public string SupportedSockets { get; set; } = string.Empty;

        // Fiziksel Boyutlar / Kasa Uyumluluğu
        public int HeightMm { get; set; }

        public int RadiatorSizeMm { get; set; }

        // Fan Detayları
        public int FanSizeMm { get; set; }

        public int FanCount { get; set; }

        // Ham Epey Verisi
        [Column(TypeName = "jsonb")]
        public string RawEpeyData { get; set; } = "{}";
    }
}