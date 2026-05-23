namespace PCPartsAPI.Dtos
{
    public class PaginationRequestParams
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 25;

        // Generic
        public string? SearchTerm { get; set; }
        public string? Brand { get; set; } 

        // 1. GPU Specific
        public string? VRAMMemorySize { get; set; }
        public string? MemoryType { get; set; } // Shared with RAM
        public string? RecommendedPsu { get; set; }
        public string? ChipsetBrand { get; set; } // NVIDIA/AMD
        public string? Interface { get; set; } // Added for GPU/Storage

        // 2. Motherboard Specific
        public string? FormFactor { get; set; }
        public string? Chipset { get; set; }
        public string? M2SlotCount { get; set; }
        public string? ArgbSupport { get; set; }
        public string? IntegratedWifi { get; set; }
        public string? IntegratedBluetooth { get; set; }
        public string? Socket { get; set; } // Shared with CPU

        // 3. Processor Specific
        public string? CoreCount { get; set; }
        public string? IntegratedGraphics { get; set; }
        public string? Tdp { get; set; }
        public string? ThreadCount { get; set; } // Added for CPU

        // 4. RAM Specific
        public string? Speed { get; set; }
        public string? TotalCapacity { get; set; }
        public string? CasLatency { get; set; }
        public string? ModuleCount { get; set; } // Added for RAM

        // 5. PSU Specific
        public string? Wattage { get; set; }
        public string? Rating { get; set; }
        public string? IsModular { get; set; }

        // 6. Case Specific
        public string? CaseType { get; set; }
        public string? HasTypeC { get; set; }
        public string? Usb3Count { get; set; }
        public string? SupportedMotherboards { get; set; } // Added for Case

        // 7. Storage Specific
        public string? StorageType { get; set; }
        public string? IsNvme { get; set; }
        public string? HasDramCache { get; set; }
        public string? NandType { get; set; } // Added for Storage

        // 8. Cooler Specific
        public string? CoolerType { get; set; }
        public string? TdpRating { get; set; }
        public string? HasRgb { get; set; }
        public string? RadiatorSize { get; set; }

        // 9. Compatibility Sorting Parameters
        public string? CompatibleCpuSocket { get; set; }
        public string? CompatibleMotherboardSocket { get; set; }
        public string? CompatibleMotherboardMemoryType { get; set; }
        public string? CompatibleRamMemoryType { get; set; }
    }
}
