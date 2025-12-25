namespace PCPartsAPI.Models
{
    public class Motherboard
    {
        public int Id { get; set; }
        public string Brand { get; set; }
        public string ModelName { get; set; }
        public string Socket { get; set; }
        public string Chipset { get; set; }
        public string FormFactor { get; set; } // ATX, Micro-ATX, Mini-ITX (Kasa uyumu için)

        // RAM Detayları
        public string MemoryType { get; set; } // DDR4, DDR5
        public int MemorySlots { get; set; } // 2 veya 4
        public int MaxMemory { get; set; }
        // ÖNEMLİ: Postgres Array olarak tutulacak. Örn: {2133, 2400, ... , 6000, 6400}
        public List<int> SupportedMemoryFrequencies { get; set; }

        // Güç ve Bağlantı (PSU Uyumu)
        public string CpuPowerConnectors { get; set; } // Örn: "8+4 pin" veya "8 pin" (PSU'da bu kablo var mı?)
        public int AtxPowerConnector { get; set; } = 24; // Genelde 24 pin

        // Genişleme Yuvaları
        public string Pcie16xVersion { get; set; } // PCIe 5.0, 4.0 (GPU tam performans için)
        public int M2SlotCount { get; set; }
        public int SataPortCount { get; set; }

        // Kasa Bağlantıları (Case IO Uyumu)
        public int InternalUsb2Headers { get; set; } // Kasa ön panel USB 2.0 için
        public int InternalUsb3Headers { get; set; } // Kasa ön panel USB 3.0 için
        public int InternalTypeCHeader { get; set; } // Kasa ön panel Type-C için
        public int FanHeaders { get; set; } // Kaç tane fan takılabilir?
        public bool ArgbSupport { get; set; } // anakartta argb pini var mı destekliyor mu

        public bool IntegratedWifi { get; set; }
        public bool IntegratedBluetooth { get; set; }
        public string ImageUrl { get; set; } = ""; // Resim linki 
    }
}