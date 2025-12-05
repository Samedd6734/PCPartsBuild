namespace PCPartsAPI.Models
{
    public class Gpu
    {
        public int Id { get; set; }
        public string Brand { get; set; } // Asus, MSI
        public string ChipsetBrand { get; set; } // NVIDIA, AMD, Intel
        public string ModelName { get; set; }

        // Fiziksel Boyutlar (Kasa Uyumu)
        public int Length { get; set; } // mm
        public int Height { get; set; } // 

        // Güç (PSU Uyumu - ÇOK ÖNEMLİ)
        public int Tdp { get; set; } // Harcadığı güç
        public int RecommendedPsu { get; set; } // Önerilen PSU
        // Örn: "1x16pin" (12VHPWR) veya "3x8pin ekran kartı güç girişinin kaç pin olduğu"
        public string PowerConnectors { get; set; }

        // Performans
        public string Interface { get; set; } // PCIe 4.0 x16
        public int VRAMMemorySize { get; set; } // GB
        public string MemoryType { get; set; } // GDDR6X
        public int BoostClock { get; set; } // MHz
        public string ImageUrl { get; set; } = ""; // Resim linki 
    }
}