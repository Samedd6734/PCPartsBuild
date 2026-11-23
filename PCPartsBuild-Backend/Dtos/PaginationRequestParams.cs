namespace PCPartsAPI.Dtos
{
    public class PaginationRequestParams
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 25;

        // Generic (tüm bileşenler)
        public string? SearchTerm { get; set; }
        public string? Brand { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }

        // Processor
        public string? SocketType { get; set; }
        public string? CoreCount { get; set; }
        public string? ThreadCount { get; set; }
        public string? IntegratedGraphics { get; set; }
        public string? TDP { get; set; }
        public string? SupportedMemoryType { get; set; }
        public string? PCIeVersion { get; set; }

        // Motherboard
        public string? FormFactor { get; set; }
        public string? MemoryType { get; set; }
        public string? M2SlotCount { get; set; }
        public string? MemorySlotCount { get; set; }
        public string? SataPortCount { get; set; }
        public string? PCIex16SlotCount { get; set; }
        public string? SupportsOverclock { get; set; }

        // GPU
        public string? ChipManufacturer { get; set; }
        public string? VRAMGB { get; set; }
        public string? PCIeInterface { get; set; }
        public string? GpuMemoryType { get; set; }
        public string? FanCount { get; set; }
        public string? TDPWatt { get; set; }

        // RAM
        public string? SpeedMHz { get; set; }
        public string? CapacityGB { get; set; }
        public string? CasLatency { get; set; }
        public string? ModuleConfig { get; set; }

        // PSU
        public string? WattageW { get; set; }
        public string? Certification { get; set; }
        public string? IsModular { get; set; }
        public string? PsuFormFactor { get; set; }
        public string? ATXVersion { get; set; }

        // Case
        public string? SupportedMotherboardFormFactors { get; set; }
        public string? HasBuiltInPSU { get; set; }

        // Storage
        public string? Interface { get; set; }
        public string? StorageFormFactor { get; set; }

        // Cooler
        public string? CoolerType { get; set; }
        public string? TDPCapacityW { get; set; }
        public string? RadiatorSizeMm { get; set; }
        public string? FanSizeMm { get; set; }

        // Compatibility Sorting Parameters
        public string? CompatibleCpuSocket { get; set; }
        public int? CompatibleCpuTdp { get; set; }

        public string? CompatibleMotherboardSocket { get; set; }
        public string? CompatibleMotherboardMemoryType { get; set; }
        public string? CompatibleMotherboardFormFactor { get; set; }
        public int? CompatibleMotherboardM2Slots { get; set; }
        public int? CompatibleMotherboardSataPorts { get; set; }
        public int? CompatibleMotherboardMemorySlots { get; set; }

        public string? CompatibleRamMemoryType { get; set; }
        public int? CompatibleRamModuleCount { get; set; }

        public string? CompatibleCaseSupportedFormFactors { get; set; }
        public int? CompatibleCaseMaxGpuLength { get; set; }
        public int? CompatibleCaseMaxCoolerHeight { get; set; }
        public int? CompatibleCaseFrontRadiator { get; set; }
        public int? CompatibleCaseTopRadiator { get; set; }

        public int? CompatibleGpuLength { get; set; }
        public int? CompatibleGpuTdp { get; set; }

        public string? CompatibleCoolerSupportedSockets { get; set; }
        public int? CompatibleCoolerHeight { get; set; }
        public int? CompatibleCoolerRadiatorSize { get; set; }
        
        // Psu required wattage
        public int? CompatiblePsuRequiredWattage { get; set; }
        
        public int? CompatibleMoboM2Slots { get; set; }
        public int? CompatibleMotherboardId { get; set; }
        public int? CompatiblePsuWattage { get; set; }
        public int? CompatibleCaseFrontRad { get; set; }
        public int? CompatibleCaseTopRad { get; set; }
    }
}
