using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using PCPartsAPI.Data;
using PCPartsAPI.Models;

namespace PCPartsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PsusController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _cache;

        public PsusController(ApplicationDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        [HttpGet]
        public async Task<ActionResult<PCPartsAPI.Dtos.PagedResponse<Psu>>> GetPsus([FromQuery] PCPartsAPI.Dtos.PaginationRequestParams request)
        {
            var query = _context.Psus.AsQueryable().AsNoTracking();

            if (!string.IsNullOrEmpty(request.SearchTerm)) {
                var search = request.SearchTerm.ToLowerInvariant();
                query = query.Where(p => (p.Brand ?? "").ToLower().Contains(search) || (p.ModelName ?? "").ToLower().Contains(search));
            }

            if (!string.IsNullOrEmpty(request.Brand)) {
                var brands = request.Brand.Split(',').Select(b => b.Trim().ToLowerInvariant()).ToList();
                query = query.Where(p => brands.Contains((p.Brand ?? "").ToLower().Trim()));
            }

            if (!string.IsNullOrEmpty(request.Wattage)) {
                var vals = request.Wattage.Split(',').Select(s => int.TryParse(s, out int n) ? n : (int?)null).Where(n => n != null).Select(n => n.Value).ToList();
                if (vals.Any()) query = query.Where(p => p.Wattage >= vals.Min());
            }

            if (!string.IsNullOrEmpty(request.IsModular)) {
                var boolVals = request.IsModular.Split(',').Select(s => bool.TryParse(s, out bool b) ? b : (bool?)null).Where(b => b != null).Select(b => b.Value).ToList();
                if (boolVals.Any()) query = query.Where(p => boolVals.Contains(p.IsModular));
            }

            if (!string.IsNullOrEmpty(request.Rating)) {
                var ratings = request.Rating.Split(',').Select(r => r.Trim().ToLowerInvariant()).ToList();
                query = query.Where(p => ratings.Contains((p.Rating ?? "").ToLower().Trim()));
            }

            string cacheKey = $"TotalCount_Psus_V8_{request.SearchTerm?.ToLowerInvariant()}_{request.Brand?.ToLowerInvariant()}_{request.Wattage}_{request.IsModular}_{request.Rating?.ToLowerInvariant()}";

            if (!_cache.TryGetValue(cacheKey, out int totalCount))
            {
                totalCount = await query.CountAsync();
                _cache.Set(cacheKey, totalCount, TimeSpan.FromHours(1));
            }

            int pageNum = request.PageNumber > 0 ? request.PageNumber : 1;
            int pageSize = request.PageSize > 0 ? request.PageSize : 25;
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            
            var psus = await query.Skip((pageNum - 1) * pageSize).Take(pageSize).ToListAsync();

            return Ok(new PCPartsAPI.Dtos.PagedResponse<Psu> { Data = psus, TotalCount = totalCount, TotalPages = totalPages, HasNextPage = pageNum < totalPages });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Psu>> GetPsu(int id)
        {
            var psu = await _context.Psus.FindAsync(id);
            return psu == null ? NotFound() : Ok(psu);
        }
    }
}