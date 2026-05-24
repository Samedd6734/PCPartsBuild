using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCPartsAPI.Models
{
    public class Psu
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

        // Güç Kapasitesi
        public int WattageW { get; set; }

        // Kasa Uyumluluğu
        [MaxLength(20)]
        public string FormFactor { get; set; } = string.Empty;

        // Verimlilik ve Standart
        [MaxLength(50)]
        public string Certification { get; set; } = string.Empty;

        [MaxLength(50)]
        public string ATXVersion { get; set; } = string.Empty;

        [Required, MaxLength(30)]
        public string IsModular { get; set; } = string.Empty;

        // Ham Epey Verisi
        [Column(TypeName = "jsonb")]
        public string RawEpeyData { get; set; } = "{}";
    }
}