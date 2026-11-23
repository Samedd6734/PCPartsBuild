using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCPartsAPI.Models
{
    public class Case
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

        // Anakart Uyumluluğu
        [MaxLength(200)]
        public string SupportedMotherboardFormFactors { get; set; } = string.Empty;

        // GPU Uyumluluğu
        public int MaxGPULengthMm { get; set; }

        // Soğutucu Uyumluluğu
        public int MaxCPUCoolerHeightMm { get; set; }

        public int TopRadiatorSupportMm { get; set; }

        public int FrontRadiatorSupportMm { get; set; }

        // Fan Uyumluluğu
        public int FanCapacity { get; set; }

        public int MaxFanSizeMm { get; set; }

        // PSU Uyumluluğu
        public bool HasBuiltInPSU { get; set; }

        public int? BuiltInPSUWattage { get; set; }

        // Depolama Yuvası
        public int Drive25Bays { get; set; }

        public int Drive35Bays { get; set; }

        // Ham Epey Verisi
        [Column(TypeName = "jsonb")]
        public string RawEpeyData { get; set; } = "{}";
    }
}