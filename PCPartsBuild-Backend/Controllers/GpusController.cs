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
                query = query.Where(g => (g.Brand ?? "").ToLower().Contains(search) || (g.ModelName ?? "").ToLower().Contains(search));
            }

            if (!string.IsNullOrEmpty(request.Brand)) {
                var brands = request.Brand.Split(',').Select(b => b.Trim().ToLowerInvariant()).ToList();
                query = query.Where(g => brands.Contains((g.Brand ?? "").ToLower().Trim()));
            }

            if (!string.IsNullOrEmpty(request.ChipsetBrand)) {
                var chipsets = request.ChipsetBrand.Split(',').Select(s => s.Trim().ToLowerInvariant()).ToList();
                query = query.Where(g => chipsets.Contains((g.ChipsetBrand ?? "").ToLower().Trim()));
            }

            if (!string.IsNullOrEmpty(request.VRAMMemorySize)) {
                var vals = request.VRAMMemorySize.Split(',').Select(s => int.TryParse(s, out int n) ? n : (int?)null).Where(n => n != null).Select(n => n.Value).ToList();
                if (vals.Any()) query = query.Where(g => vals.Contains(g.VRAMMemorySize));
            }

            if (!string.IsNullOrEmpty(request.MemoryType)) {
                var memTypes = request.MemoryType.Split(',').Select(s => s.Trim().ToLowerInvariant()).ToList();
                query = query.Where(g => memTypes.Contains((g.MemoryType ?? "").ToLower().Trim()));
            }

            if (!string.IsNullOrEmpty(request.RecommendedPsu)) {
                var vals = request.RecommendedPsu.Split(',').Select(s => int.TryParse(s, out int n) ? n : (int?)null).Where(n => n != null).Select(n => n.Value).ToList();
                if (vals.Any()) query = query.Where(g => g.RecommendedPsu >= vals.Min());
            }

            if (!string.IsNullOrEmpty(request.Interface)) {
                var interfaces = request.Interface.Split(',').Select(s => s.Trim().ToLowerInvariant()).ToList();
                query = query.Where(g => interfaces.Contains((g.Interface ?? "").ToLower().Trim()));
            }

            string cacheKey = $"TotalCount_Gpus_V8_{request.SearchTerm?.ToLowerInvariant()}_{request.Brand?.ToLowerInvariant()}_{request.ChipsetBrand?.ToLowerInvariant()}_{request.VRAMMemorySize}_{request.MemoryType?.ToLowerInvariant()}_{request.RecommendedPsu}_{request.Interface?.ToLowerInvariant()}";

            if (!_cache.TryGetValue(cacheKey, out int totalCount))
            {
                totalCount = await query.CountAsync();
                _cache.Set(cacheKey, totalCount, TimeSpan.FromHours(1));
            }

            int pageNum = request.PageNumber > 0 ? request.PageNumber : 1;
            int pageSize = request.PageSize > 0 ? request.PageSize : 25;
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            
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