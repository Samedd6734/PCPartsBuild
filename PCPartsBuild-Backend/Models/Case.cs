namespace PCPartsAPI.Models
{
    public class Case
    {
        public int Id { get; set; }
        public string Brand { get; set; }
        public string ModelName { get; set; }
        public string CaseType { get; set; } // Mid Tower, Full Tower

        // Desteklediği Anakartlar (Array veya Virgüllü String)
        public string SupportedMotherboards { get; set; } // "ATX, Micro-ATX, Mini-ITX"

        // Fiziksel Sınırlar (Uyumluluk)
        public int MaxGpuLength { get; set; } // mm
        public int MaxCpuCoolerHeight { get; set; } // mm
        public int MaxPsuLength { get; set; } // mm

        // Radyatör Desteği (Sıvı Soğutma)
        public string RadiatorSupportFront { get; set; } // "120, 240, 360"
        public string RadiatorSupportTop { get; set; } // "120, 240"

        // Ön Panel Portları (Anakart Header uyumu için)
        public bool HasTypeC { get; set; }
        public int Usb3Count { get; set; }
        public string ImageUrl { get; set; } = ""; // Resim linki
    }
}