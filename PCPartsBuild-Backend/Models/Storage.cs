using System.ComponentModel.DataAnnotations.Schema; // NotMapped için gerekli

namespace PCPartsAPI.Models
{
    public class Storage
    {
        public int Id { get; set; }
        public string Brand { get; set; } = string.Empty; // Örn: "Samsung", "Seagate"
        public string ModelName { get; set; } = string.Empty; // Örn: "990 Pro", "BarraCuda"

        public string StorageType { get; set; } = string.Empty; // "SSD" veya "HDD"

        // --- 1. KAPASİTE VE PERFORMANS ---

        // Veritabanında GB cinsinden tutulur. (Gerçek 2'lik sistem değeri)
        // Örn: 1 TB için 1024, 2 TB için 2048, 500 GB için 500 veya 512 girilmeli.
        public int Capacity { get; set; }

        // BU YENİ ÖZELLİK: Veritabanında sütun oluşturmaz, otomatik hesaplar.
        // Frontend'de "1 TB", "512 GB" gibi düzgün göstermek için kullanılır.
        [NotMapped]
        public string CapacityFormatted
        {
            get
            {
                if (Capacity >= 1024 && Capacity % 1024 == 0)
                {
                    return $"{Capacity / 1024} TB";
                }
                else
                {
                    return $"{Capacity} GB";
                }
            }
        }

        public int ReadSpeed { get; set; } // MB/s (Örn: 7450)
        public int WriteSpeed { get; set; } // MB/s (Örn: 6900)

        // --- 2. BAĞLANTI VE FORM FAKTÖRÜ ---
        public string FormFactor { get; set; } = string.Empty; // "M.2", "2.5 Inch"
        public string Interface { get; set; } = string.Empty; // "PCIe 4.0 x4", "SATA III"
        public bool IsNvme { get; set; }

        // --- 3. TEKNİK DETAYLAR ---
        public bool HasDramCache { get; set; }
        public string NandType { get; set; } = string.Empty; // "TLC", "QLC"
        public int Tbw { get; set; } // Ömür (Total Bytes Written)

        // --- 5. HDD ÖZEL ---
        public int Rpm { get; set; }
        public int CacheSizeMB { get; set; }
        public string ImageUrl { get; set; } = ""; // Resim linki 
    }
}