using System;

namespace PCPartsAPI.DTOs
{
    public class SavedBuildDto
    {
        public int Id { get; set; }
        public string BuildName { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal TotalPrice { get; set; }

        // Parça İsimleri (Ekranda göstermek için)
        public string CpuName { get; set; } = "İşlemci Seçilmedi";
        public string GpuName { get; set; } = "Ekran Kartı Seçilmedi";
        public string RamName { get; set; } = "RAM Seçilmedi";
        public string CaseName { get; set; } = "Kasa Seçilmedi";

        // Parça Resimleri (Kartta göstermek için)
        public string CpuImage { get; set; }
        public string GpuImage { get; set; }
        public string CaseImage { get; set; }
    }
}