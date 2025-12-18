namespace PCPartsAPI.Models
{
    public class Ram
    {
        public int Id { get; set; }
        public string Brand { get; set; }
        public string ModelName { get; set; }
        public string MemoryType { get; set; } // DDR4, DDR5
        public int Speed { get; set; } // MHz
        public int ModuleCount { get; set; } // Kit adedi (1, 2, 4)
        public int CapacityPerModule { get; set; } // GB
        public int CasLatency { get; set; } // CL değeri (Düşük iyi
        public bool HasRgb { get; set; }
        public string ImageUrl { get; set; } = ""; // Resim linki 
    }
}