namespace PCPartsAPI.Models
{
    public class Psu
    {
        public int Id { get; set; }
        public string Brand { get; set; }
        public string ModelName { get; set; }
        public int Wattage { get; set; }
        public string Rating { get; set; } // 80+ Gold, Bronze
        public string FormFactor { get; set; } // ATX, SFX
        public bool IsModular { get; set; }
        public int Length { get; set; } // mm (Kasa uyumu için)

        // Kablo Sayıları (Uyumluluk Kontrolü İçin Kritik)
        public int Eps8PinCount { get; set; } // CPU Güç Kablosu Sayısı (Anakartın istediği kadar olmalı)
        public int Pcie8PinCount { get; set; } // Ekran Kartı Güç Kablosu Sayısı (6+2 pin)
        public bool Has12VHPWR { get; set; } // Yeni nesil Nvidia kartlar için tekli 16 pin kablo var mı?
        public int SataCount { get; set; } // HDD/SSD için
        public string ImageUrl { get; set; } = ""; // Resim linki 
    }
}