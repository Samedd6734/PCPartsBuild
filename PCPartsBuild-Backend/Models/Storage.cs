using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCPartsAPI.Models
{
    public class Storage
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

        // Form Faktör ve Arayüz
        [MaxLength(50)]
        public string FormFactor { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Interface { get; set; } = string.Empty;

        // Kapasite
        public int CapacityGB { get; set; }

        // Performans
        public int? ReadSpeedMBs { get; set; }

        public int? WriteSpeedMBs { get; set; }

        // Ham Epey Verisi
        [Column(TypeName = "jsonb")]
        public string RawEpeyData { get; set; } = "{}";
    }
}