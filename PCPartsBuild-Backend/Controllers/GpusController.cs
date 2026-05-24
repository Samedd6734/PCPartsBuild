using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using PCPartsAPI.Data;
using PCPartsAPI.Models;

namespace PCPartsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GpusController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _cache;

        public GpusController(ApplicationDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        [HttpGet]
        public async Task<ActionResult<PCPartsAPI.Dtos.PagedResponse<Gpu>>> GetGpus([FromQuery] PCPartsAPI.Dtos.PaginationRequestParams request)
        {
            var query = _context.Gpus.AsQueryable().AsNoTracking();

            if (!string.IsNullOrEmpty(request.SearchTerm)) {
                var search = request.SearchTerm.ToLowerInvariant();
                query = query.Where(g => (g.Brand ?? "").ToLower().Contains(search) || (g.ProductName ?? "").ToLower().Contains(search));
            }

            if (!string.IsNullOrEmpty(request.Brand)) {
                var brands = request.Brand.Split(',').Select(b => b.Trim().ToLowerInvariant()).ToList();
                query = query.Where(g => brands.Contains((g.Brand ?? "").ToLower().Trim()));
            }

            if (!string.IsNullOrEmpty(request.ChipManufacturer)) {
                var chips = request.ChipManufacturer.Split(',').Select(s => s.Trim().ToLowerInvariant()).ToList();
                query = query.Where(g => chips.Contains((g.ChipManufacturer ?? "").ToLower().Trim()));
            }

            if (!string.IsNullOrEmpty(request.VRAMGB)) {
                var vals = request.VRAMGB.Split(',').Select(s => int.TryParse(s, out int n) ? n : (int?)null).Where(n => n != null).Select(n => n.Value).ToList();
                if (vals.Any()) query = query.Where(g => vals.Contains(g.VRAMGB));
            }

            if (!string.IsNullOrEmpty(request.MemoryType)) {
                var memTypes = request.MemoryType.Split(',').Select(s => s.Trim().ToLowerInvariant()).ToList();
                query = query.Where(g => memTypes.Contains((g.MemoryType ?? "").ToLower().Trim()));
            }

            if (!string.IsNullOrEmpty(request.PCIeInterface)) {
                var interfaces = request.PCIeInterface.Split(',').Select(s => s.Trim().ToLowerInvariant()).ToList();
                query = query.Where(g => interfaces.Contains((g.PCIeInterface ?? "").ToLower().Trim()));
            }

            if (!string.IsNullOrEmpty(request.FanCount)) {
                var counts = request.FanCount.Split(',').Select(s => int.TryParse(s, out int n) ? n : (int?)null).Where(n => n != null).Select(n => n.Value).ToList();
                if (counts.Any()) query = query.Where(g => counts.Contains(g.FanCount));
            }

            if (!string.IsNullOrEmpty(request.TDPWatt)) {
                var tdps = request.TDPWatt.Split(',').Select(s => int.TryParse(s, out int n) ? n : (int?)null).Where(n => n != null).Select(n => n.Value).ToList();
                if (tdps.Any()) query = query.Where(g => tdps.Contains(g.TDPWatt));
            }

            if (request.MinPrice.HasValue)
                query = query.Where(g => g.Price >= request.MinPrice.Value);
            if (request.MaxPrice.HasValue)
                query = query.Where(g => g.Price <= request.MaxPrice.Value);

            string cacheKey = $"TotalCount_Gpus_V10_{request.SearchTerm?.ToLowerInvariant()}_{request.Brand?.ToLowerInvariant()}_{request.ChipManufacturer?.ToLowerInvariant()}_{request.VRAMGB}_{request.MemoryType?.ToLowerInvariant()}_{request.PCIeInterface?.ToLowerInvariant()}_{request.FanCount}_{request.TDPWatt}_{request.MinPrice}_{request.MaxPrice}";

            if (!_cache.TryGetValue(cacheKey, out int totalCount))
            {
                totalCount = await query.CountAsync();
                _cache.Set(cacheKey, totalCount, TimeSpan.FromHours(1));
            }

            int pageNum = request.PageNumber > 0 ? request.PageNumber : 1;
            int pageSize = request.PageSize > 0 ? request.PageSize : 25;
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            
            // Compatibility Sorting
            if (request.CompatibleCaseMaxGpuLength > 0 || request.CompatiblePsuWattage > 0)
            {
                var maxLen = request.CompatibleCaseMaxGpuLength ?? 0;
                var psuWatt = request.CompatiblePsuWattage ?? 0;
                var cpuTdp = request.CompatibleCpuTdp ?? 0;

                query = query.OrderByDescending(g =>
                    (maxLen == 0 || g.LengthMm == 0 || g.LengthMm <= maxLen) &&
                    (psuWatt == 0 || (int)Math.Ceiling((cpuTdp + g.TDPWatt) * 1.25) <= psuWatt)
                ).ThenBy(g => g.Id);
            }
            else
            {
                query = query.OrderBy(g => g.Id);
            }

            var gpus = await query.Skip((pageNum - 1) * pageSize).Take(pageSize).ToListAsync();

            return Ok(new PCPartsAPI.Dtos.PagedResponse<Gpu> { Data = gpus, TotalCount = totalCount, TotalPages = totalPages, HasNextPage = pageNum < totalPages });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Gpu>> GetGpu(int id)
        {
            var gpu = await _context.Gpus.FindAsync(id);
            return gpu == null ? NotFound() : Ok(gpu);
        }
    }
}