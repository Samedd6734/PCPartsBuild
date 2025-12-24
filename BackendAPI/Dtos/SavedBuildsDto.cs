using System;

namespace PCPartsAPI.DTOs
{
    public class SavedBuildDto
    {
        public int Id { get; set; }
        public string BuildName { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal TotalPrice { get; set; }

        // --- PARÇA İSİMLERİ (8 Bileşen) ---
        public string CpuName { get; set; } = "İşlemci Seçilmedi";
        public string GpuName { get; set; } = "Ekran Kartı Seçilmedi";
        public string RamName { get; set; } = "RAM Seçilmedi";
        public string MotherboardName { get; set; } = "Anakart Seçilmedi";
        public string StorageName { get; set; } = "Depolama Seçilmedi";
        public string CaseName { get; set; } = "Kasa Seçilmedi";
        public string PsuName { get; set; } = "PSU Seçilmedi";
        public string CoolerName { get; set; } = "Soğutucu Seçilmedi";

        // --- PARÇA RESİMLERİ (Ana resim seçimi için) ---
        public string CpuImage { get; set; }
        public string GpuImage { get; set; }
        public string CaseImage { get; set; }
    }
}