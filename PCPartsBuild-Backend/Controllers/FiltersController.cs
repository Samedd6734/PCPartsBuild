using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using PCPartsAPI.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PCPartsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FiltersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _cache;

        public FiltersController(ApplicationDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        [HttpGet("{category}")]
        public async Task<IActionResult> GetFilters(string category)
        {
            string cacheKey = $"Filters_Epey_V10_{category.ToLowerInvariant()}";

            var filters = await _cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24);
                
                return category.ToLowerInvariant() switch
                {
                    "processors" or "cpu" or "processor" => (object)new
                    {
                        Brand = await _context.Processors.Select(p => p.Brand).Distinct().Where(x => !string.IsNullOrEmpty(x)).ToListAsync(),
                        SocketType = await _context.Processors.Select(p => p.SocketType).Distinct().Where(x => !string.IsNullOrEmpty(x)).ToListAsync(),
                        CoreCount = await _context.Processors.Select(p => p.CoreCount).Distinct().OrderBy(x => x).ToListAsync(),
                        ThreadCount = await _context.Processors.Select(p => p.ThreadCount).Distinct().OrderBy(x => x).ToListAsync(),
                        HasIntegratedGraphics = new List<string> { "true", "false" },
                        TDP = await _context.Processors.Select(p => p.TDP).Distinct().OrderBy(x => x).ToListAsync(),
                        SupportedMemoryType = await _context.Processors.Select(p => p.SupportedMemoryType).Distinct().Where(x => !string.IsNullOrEmpty(x)).ToListAsync(),
                        PCIeVersion = await _context.Processors.Select(p => p.PCIeVersion).Distinct().Where(x => !string.IsNullOrEmpty(x)).ToListAsync()
                    },
                    "motherboards" or "mobo" or "motherboard" => new
                    {
                        Brand = await _context.Motherboards.Select(m => m.Brand).Distinct().Where(x => !string.IsNullOrEmpty(x)).ToListAsync(),
                        SocketType = await _context.Motherboards.Select(m => m.SocketType).Distinct().Where(x => !string.IsNullOrEmpty(x)).ToListAsync(),
                        FormFactor = await _context.Motherboards.Select(m => m.FormFactor).Distinct().Where(x => !string.IsNullOrEmpty(x)).ToListAsync(),
                        MemoryType = await _context.Motherboards.Select(m => m.MemoryType).Distinct().Where(x => !string.IsNullOrEmpty(x)).ToListAsync(),
                        M2SlotCount = await _context.Motherboards.Select(m => m.M2SlotCount).Distinct().OrderBy(x => x).ToListAsync(),
                        MemorySlotCount = await _context.Motherboards.Select(m => m.MemorySlotCount).Distinct().OrderBy(x => x).ToListAsync(),
                        SataPortCount = await _context.Motherboards.Select(m => m.SataPortCount).Distinct().OrderBy(x => x).ToListAsync(),
                        PCIex16SlotCount = await _context.Motherboards.Select(m => m.PCIex16SlotCount).Distinct().OrderBy(x => x).ToListAsync(),
                        SupportsOverclock = new List<string> { "true", "false" }
                    },
                    "gpus" or "vga" or "gpu" => new
                    {
                        Brand = await _context.Gpus.Select(g => g.Brand).Distinct().Where(x => !string.IsNullOrEmpty(x)).ToListAsync(),
                        ChipManufacturer = await _context.Gpus.Select(g => g.ChipManufacturer).Distinct().Where(x => !string.IsNullOrEmpty(x)).ToListAsync(),
                        VRAMGB = await _context.Gpus.Select(g => g.VRAMGB).Distinct().OrderBy(x => x).ToListAsync(),
                        GpuMemoryType = await _context.Gpus.Select(g => g.MemoryType).Distinct().Where(x => !string.IsNullOrEmpty(x)).ToListAsync(),
                        PCIeInterface = await _context.Gpus.Select(g => g.PCIeInterface).Distinct().Where(x => !string.IsNullOrEmpty(x)).ToListAsync(),
                        FanCount = await _context.Gpus.Select(g => g.FanCount).Distinct().OrderBy(x => x).ToListAsync(),
                        TDPWatt = await _context.Gpus.Select(g => g.TDPWatt).Distinct().OrderBy(x => x).ToListAsync()
                    },
                    "rams" or "memory" or "ram" => new
                    {
                        Brand = await _context.Rams.Select(r => r.Brand).Distinct().Where(x => !string.IsNullOrEmpty(x)).ToListAsync(),
                        MemoryType = await _context.Rams.Select(r => r.MemoryType).Distinct().Where(x => !string.IsNullOrEmpty(x)).ToListAsync(),
                        SpeedMHz = await _context.Rams.Select(r => r.SpeedMHz).Distinct().OrderBy(x => x).ToListAsync(),
                        CapacityGB = await _context.Rams.Select(r => r.CapacityGB).Distinct().OrderBy(x => x).ToListAsync(),
                        CasLatency = await _context.Rams.Select(r => r.CasLatency).Distinct().OrderBy(x => x).ToListAsync(),
                        ModuleConfig = await _context.Rams.Select(r => r.ModuleConfig).Distinct().Where(x => !string.IsNullOrEmpty(x)).ToListAsync()
                    },
                    "psus" or "power" or "psu" => new
                    {
                        Brand = await _context.Psus.Select(p => p.Brand).Distinct().Where(x => !string.IsNullOrEmpty(x)).ToListAsync(),
                        WattageW = await _context.Psus.Select(p => p.WattageW).Distinct().OrderBy(x => x).ToListAsync(),
                        Certification = await _context.Psus.Select(p => p.Certification).Distinct().Where(x => !string.IsNullOrEmpty(x)).ToListAsync(),
                        IsModular = await _context.Psus.Select(p => p.IsModular).Distinct().Where(x => !string.IsNullOrEmpty(x)).ToListAsync(),
                        PsuFormFactor = await _context.Psus.Select(p => p.FormFactor).Distinct().Where(x => !string.IsNullOrEmpty(x)).ToListAsync(),
                        ATXVersion = await _context.Psus.Select(p => p.ATXVersion).Distinct().Where(x => !string.IsNullOrEmpty(x)).ToListAsync()
                    },
                    "storages" or "ssd" or "hdd" or "storage" => new
                    {
                        Brand = await _context.Storages.Select(s => s.Brand).Distinct().Where(x => !string.IsNullOrEmpty(x)).ToListAsync(),
                        StorageFormFactor = await _context.Storages.Select(s => s.FormFactor).Distinct().Where(x => !string.IsNullOrEmpty(x)).ToListAsync(),
                        Interface = await _context.Storages.Select(s => s.Interface).Distinct().Where(x => !string.IsNullOrEmpty(x)).ToListAsync(),
                        CapacityGB = await _context.Storages.Select(s => s.CapacityGB).Distinct().OrderBy(x => x).ToListAsync()
                    },
                    "cases" or "chassis" or "case" => new
                    {
                        Brand = await _context.Cases.Where(c => !string.IsNullOrEmpty(c.Brand)).Select(c => c.Brand).Distinct().ToListAsync(),
                        SupportedMotherboardFormFactors = new List<string> { "E-ATX", "ATX", "Micro ATX", "Mini ITX" },
                        HasBuiltInPSU = new List<string> { "true", "false" }
                    },
                    "cpucoolers" or "cooler" or "cpucooler" => new
                    {
                        Brand = await _context.CpuCoolers.Select(c => c.Brand).Distinct().Where(x => !string.IsNullOrEmpty(x)).ToListAsync(),
                        CoolerType = await _context.CpuCoolers.Select(c => c.CoolerType).Distinct().Where(x => !string.IsNullOrEmpty(x)).ToListAsync(),
                        RadiatorSizeMm = await _context.CpuCoolers.Select(c => c.RadiatorSizeMm).Distinct().OrderBy(x => x).ToListAsync(),
                        TDPCapacityW = await _context.CpuCoolers.Select(c => c.TDPCapacityW).Distinct().OrderBy(x => x).ToListAsync(),
                        FanSizeMm = await _context.CpuCoolers.Select(c => c.FanSizeMm).Distinct().OrderBy(x => x).ToListAsync()
                    },
                    _ => (object)null
                };
            });

            return filters == null ? BadRequest("Invalid category") : Ok(filters);
        }
    }
}
