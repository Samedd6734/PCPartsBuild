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
            string cacheKey = $"Filters_Exhaustive_V9_{category.ToLowerInvariant()}";

            var filters = await _cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24);
                
                return category.ToLowerInvariant() switch
                {
                    "processors" or "cpu" or "processor" => (object)new
                    {
                        Brand = await _context.Processors.Select(p => p.Brand).Distinct().Where(x => !string.IsNullOrEmpty(x)).ToListAsync(),
                        Socket = await _context.Processors.Select(p => p.Socket).Distinct().Where(x => !string.IsNullOrEmpty(x)).ToListAsync(),
                        CoreCount = await _context.Processors.Select(p => p.CoreCount).Distinct().OrderBy(x => x).ToListAsync(),
                        ThreadCount = await _context.Processors.Select(p => p.ThreadCount).Distinct().OrderBy(x => x).ToListAsync(),
                        Tdp = await _context.Processors.Select(p => p.Tdp).Distinct().OrderBy(x => x).ToListAsync(),
                        IntegratedGraphics = new List<string> { "true", "false" }
                    },
                    "motherboards" or "mobo" or "motherboard" => new
                    {
                        Brand = await _context.Motherboards.Select(m => m.Brand).Distinct().Where(x => !string.IsNullOrEmpty(x)).ToListAsync(),
                        Socket = await _context.Motherboards.Select(m => m.Socket).Distinct().Where(x => !string.IsNullOrEmpty(x)).ToListAsync(),
                        Chipset = await _context.Motherboards.Select(m => m.Chipset).Distinct().Where(x => !string.IsNullOrEmpty(x)).ToListAsync(),
                        FormFactor = await _context.Motherboards.Select(m => m.FormFactor).Distinct().Where(x => !string.IsNullOrEmpty(x)).ToListAsync(),
                        MemoryType = await _context.Motherboards.Select(m => m.MemoryType).Distinct().Where(x => !string.IsNullOrEmpty(x)).ToListAsync(),
                        M2SlotCount = await _context.Motherboards.Select(m => m.M2SlotCount).Distinct().OrderBy(x => x).ToListAsync(),
                        ArgbSupport = new List<string> { "true", "false" },
                        IntegratedWifi = new List<string> { "true", "false" },
                        IntegratedBluetooth = new List<string> { "true", "false" }
                    },
                    "gpus" or "vga" or "gpu" => new
                    {
                        Brand = await _context.Gpus.Select(g => g.Brand).Distinct().Where(x => !string.IsNullOrEmpty(x)).ToListAsync(),
                        ChipsetBrand = await _context.Gpus.Select(g => g.ChipsetBrand).Distinct().Where(x => !string.IsNullOrEmpty(x)).ToListAsync(),
                        VRAMMemorySize = await _context.Gpus.Select(g => g.VRAMMemorySize).Distinct().OrderBy(x => x).ToListAsync(),
                        MemoryType = await _context.Gpus.Select(g => g.MemoryType).Distinct().Where(x => !string.IsNullOrEmpty(x)).ToListAsync(),
                        Interface = await _context.Gpus.Select(g => g.Interface).Distinct().Where(x => !string.IsNullOrEmpty(x)).ToListAsync(),
                        RecommendedPsu = await _context.Gpus.Select(g => g.RecommendedPsu).Distinct().OrderBy(x => x).ToListAsync()
                    },
                    "rams" or "memory" or "ram" => new
                    {
                        Brand = await _context.Rams.Select(r => r.Brand).Distinct().Where(x => !string.IsNullOrEmpty(x)).ToListAsync(),
                        MemoryType = await _context.Rams.Select(r => r.MemoryType).Distinct().Where(x => !string.IsNullOrEmpty(x)).ToListAsync(),
                        Speed = await _context.Rams.Select(r => r.Speed).Distinct().OrderBy(x => x).ToListAsync(),
                        ModuleCount = await _context.Rams.Select(r => r.ModuleCount).Distinct().OrderBy(x => x).ToListAsync(),
                        TotalCapacity = await _context.Rams.Select(r => r.TotalCapacity).Distinct().OrderBy(x => x).ToListAsync(),
                        CasLatency = await _context.Rams.Select(r => r.CasLatency).Distinct().OrderBy(x => x).ToListAsync()
                    },
                    "psus" or "power" or "psu" => new
                    {
                        Brand = await _context.Psus.Select(p => p.Brand).Distinct().Where(x => !string.IsNullOrEmpty(x)).ToListAsync(),
                        Wattage = await _context.Psus.Select(p => p.Wattage).Distinct().OrderBy(x => x).ToListAsync(),
                        Rating = await _context.Psus.Select(p => p.Rating).Distinct().Where(x => !string.IsNullOrEmpty(x)).ToListAsync(),
                        IsModular = new List<string> { "true", "false" }
                    },
                    "storages" or "ssd" or "hdd" or "storage" => new
                    {
                        Brand = await _context.Storages.Select(s => s.Brand).Distinct().Where(x => !string.IsNullOrEmpty(x)).ToListAsync(),
                        StorageType = await _context.Storages.Select(s => s.StorageType).Distinct().Where(x => !string.IsNullOrEmpty(x)).ToListAsync(),
                        FormFactor = await _context.Storages.Select(s => s.FormFactor).Distinct().Where(x => !string.IsNullOrEmpty(x)).ToListAsync(),
                        Interface = await _context.Storages.Select(s => s.Interface).Distinct().Where(x => !string.IsNullOrEmpty(x)).ToListAsync(),
                        NandType = await _context.Storages.Select(s => s.NandType).Distinct().Where(x => !string.IsNullOrEmpty(x)).ToListAsync(),
                        IsNvme = new List<string> { "true", "false" },
                        HasDramCache = new List<string> { "true", "false" }
                    },
                    "cases" or "chassis" or "case" => new
                    {
                        Brand = await _context.Cases.Select(c => c.Brand).Distinct().Where(x => !string.IsNullOrEmpty(x)).ToListAsync(),
                        CaseType = await _context.Cases.Select(c => c.CaseType).Distinct().Where(x => !string.IsNullOrEmpty(x)).ToListAsync(),
                        SupportedMotherboards = await _context.Cases.Select(c => c.SupportedMotherboards).Distinct().Where(x => !string.IsNullOrEmpty(x)).ToListAsync(),
                        HasTypeC = new List<string> { "true", "false" },
                        Usb3Count = await _context.Cases.Select(c => c.Usb3Count).Distinct().OrderBy(x => x).ToListAsync()
                    },
                    "cpucoolers" or "cooler" or "cpucooler" => new
                    {
                        Brand = await _context.CpuCoolers.Select(c => c.Brand).Distinct().Where(x => !string.IsNullOrEmpty(x)).ToListAsync(),
                        CoolerType = await _context.CpuCoolers.Select(c => c.CoolerType).Distinct().Where(x => !string.IsNullOrEmpty(x)).ToListAsync(),
                        RadiatorSize = await _context.CpuCoolers.Select(c => c.RadiatorSize).Distinct().OrderBy(x => x).ToListAsync(),
                        HasRgb = new List<string> { "true", "false" },
                        TdpRating = await _context.CpuCoolers.Select(c => c.TdpRating).Distinct().OrderBy(x => x).ToListAsync()
                    },
                    _ => (object)null
                };
            });

            return filters == null ? BadRequest("Invalid category") : Ok(filters);
        }
    }
}
