using System;
using System.ComponentModel.DataAnnotations;

namespace PCPartsAPI.Models
{
    public class SavedBuilds
    {
        [Key]
        public int Id { get; set; }

        public string UserId { get; set; } // Sistemi kaydeden kullanıcı

        public string BuildName { get; set; } // Örn: "Hayalimdeki PC", "Ofis PC"

        public DateTime CreatedAt { get; set; } = DateTime.Now; // Kayıt tarihi

        // Seçili parçaların ID'lerini tutuyoruz. 
        // Eğer parça seçilmediyse null olabilir (int?)
        public int? CpuId { get; set; }
        public int? MotherboardId { get; set; }
        public int? RamId { get; set; }
        public int? GpuId { get; set; }
        public int? StorageId { get; set; }
        public int? CaseId { get; set; }
        public int? PsuId { get; set; }
        public int? CpuCoolerId { get; set; }

        // Frontend'de toplam fiyatı hesaplatıp buraya da kaydedebiliriz (Opsiyonel)
        public decimal TotalPrice { get; set; }
    }
}