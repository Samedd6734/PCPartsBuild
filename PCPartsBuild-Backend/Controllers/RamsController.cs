using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using PCPartsAPI.Data;
using PCPartsAPI.Models;

namespace PCPartsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RamsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _cache;

        public RamsController(ApplicationDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        [HttpGet]
        public async Task<ActionResult<PCPartsAPI.Dtos.PagedResponse<Ram>>> GetRams([FromQuery] PCPartsAPI.Dtos.PaginationRequestParams request)
        {
            var query = _context.Rams.AsQueryable().AsNoTracking();

            if (!string.IsNullOrEmpty(request.SearchTerm)) {
                var search = request.SearchTerm.ToLowerInvariant();
                query = query.Where(r => (r.Brand ?? "").ToLower().Contains(search) || (r.ModelName ?? "").ToLower().Contains(search));
            }

            if (!string.IsNullOrEmpty(request.Brand)) {
                var brands = request.Brand.Split(',').Select(b => b.Trim().ToLowerInvariant()).ToList();
                query = query.Where(r => brands.Contains((r.Brand ?? "").ToLower().Trim()));
            }

            if (!string.IsNullOrEmpty(request.MemoryType)) {
                var memTypes = request.MemoryType.Split(',').Select(s => s.Trim().ToLowerInvariant()).ToList();
                query = query.Where(r => memTypes.Contains((r.MemoryType ?? "").ToLower().Trim()));
            }

            if (!string.IsNullOrEmpty(request.TotalCapacity)) {
                var vals = request.TotalCapacity.Split(',').Select(s => int.TryParse(s, out int n) ? n : (int?)null).Where(n => n != null).Select(n => n.Value).ToList();
                if (vals.Any()) query = query.Where(r => vals.Contains(r.TotalCapacity));
            }

            if (!string.IsNullOrEmpty(request.Speed)) {
                var vals = request.Speed.Split(',').Select(s => int.TryParse(s, out int n) ? n : (int?)null).Where(n => n != null).Select(n => n.Value).ToList();
                if (vals.Any()) query = query.Where(r => vals.Contains(r.Speed));
            }

            if (!string.IsNullOrEmpty(request.CasLatency)) {
                var vals = request.CasLatency.Split(',').Select(s => int.TryParse(s, out int n) ? n : (int?)null).Where(n => n != null).Select(n => n.Value).ToList();
                if (vals.Any()) query = query.Where(r => vals.Contains(r.CasLatency));
            }

            if (!string.IsNullOrEmpty(request.ModuleCount)) {
                var vals = request.ModuleCount.Split(',').Select(s => int.TryParse(s, out int n) ? n : (int?)null).Where(n => n != null).Select(n => n.Value).ToList();
                if (vals.Any()) query = query.Where(r => vals.Contains(r.ModuleCount));
            }

            string cacheKey = $"TotalCount_Rams_V8_{request.SearchTerm?.ToLowerInvariant()}_{request.Brand?.ToLowerInvariant()}_{request.MemoryType?.ToLowerInvariant()}_{request.TotalCapacity}_{request.Speed}_{request.CasLatency}_{request.ModuleCount}";

            if (!_cache.TryGetValue(cacheKey, out int totalCount))
            {
                totalCount = await query.CountAsync();
                _cache.Set(cacheKey, totalCount, TimeSpan.FromHours(1));
            }

            int pageNum = request.PageNumber > 0 ? request.PageNumber : 1;
            int pageSize = request.PageSize > 0 ? request.PageSize : 25;
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            
            // Compatibility Sorting
            if (!string.IsNullOrEmpty(request.CompatibleMotherboardMemoryType))
            {
                var moboMemType = request.CompatibleMotherboardMemoryType.ToLowerInvariant().Trim();
                query = query.OrderByDescending(r => r.MemoryType != null && r.MemoryType.ToLower() == moboMemType).ThenBy(r => r.Id);
            }
            else
            {
                query = query.OrderBy(r => r.Id);
            }

            var rams = await query.Skip((pageNum - 1) * pageSize).Take(pageSize).ToListAsync();

            return Ok(new PCPartsAPI.Dtos.PagedResponse<Ram> { Data = rams, TotalCount = totalCount, TotalPages = totalPages, HasNextPage = pageNum < totalPages });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Ram>> GetRam(int id)
        {
            var ram = await _context.Rams.FindAsync(id);
            return ram == null ? NotFound() : Ok(ram);
        }
    }
}