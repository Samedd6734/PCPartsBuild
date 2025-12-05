namespace PCPartsAPI.Models
{
    public class Processor
    {
        public int Id { get; set; }
        public string Brand { get; set; } // Intel, AMD
        public string ModelName { get; set; }
        public string Socket { get; set; } // AM5, LGA1700
        public int CoreCount { get; set; }
        public int ThreadCount { get; set; }
        public double BaseClockSpeed { get; set; }
        public double BoostClockSpeed { get; set; }
        public int L3Cache { get; set; }
        public int Tdp { get; set; } // Soğutucu seçimi ve PSU hesaplaması için kritik
        public bool IntegratedGraphics { get; set; } // Ekran kartsız görüntü verir mi
        public string SupportedMemoryTypes { get; set; } // Örn: "DDR4, DDR5"
        public int MaxMemoryCapacity { get; set; } // GB (Örn: 128)
        public string ImageUrl { get; set; } = ""; // Resim linki 
    }
}