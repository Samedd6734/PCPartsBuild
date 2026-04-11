using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using PCPartsAPI.Models;
using PCPartsAPI.Data;

namespace PCPartsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CasesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _cache;

        public CasesController(ApplicationDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        [HttpGet]
        public async Task<ActionResult<PCPartsAPI.Dtos.PagedResponse<Case>>> GetCases([FromQuery] PCPartsAPI.Dtos.PaginationRequestParams request)
        {
            var query = _context.Cases.AsQueryable().AsNoTracking();

            if (!string.IsNullOrEmpty(request.SearchTerm)) {
                var search = request.SearchTerm.ToLowerInvariant();
                query = query.Where(c => (c.Brand ?? "").ToLower().Contains(search) || (c.ModelName ?? "").ToLower().Contains(search));
            }

            if (!string.IsNullOrEmpty(request.Brand)) {
                var brands = request.Brand.Split(',').Select(b => b.Trim().ToLowerInvariant()).ToList();
                query = query.Where(c => brands.Contains((c.Brand ?? "").ToLower().Trim()));
            }

            if (!string.IsNullOrEmpty(request.CaseType)) {
                var caseTypes = request.CaseType.Split(',').Select(s => s.Trim().ToLowerInvariant()).ToList();
                query = query.Where(c => caseTypes.Contains((c.CaseType ?? "").ToLower().Trim()));
            }

            if (!string.IsNullOrEmpty(request.HasTypeC)) {
                var boolVals = request.HasTypeC.Split(',').Select(s => bool.TryParse(s, out bool b) ? b : (bool?)null).Where(b => b != null).Select(b => b.Value).ToList();
                if (boolVals.Any()) query = query.Where(c => boolVals.Contains(c.HasTypeC));
            }

            if (!string.IsNullOrEmpty(request.Usb3Count)) {
                var vals = request.Usb3Count.Split(',').Select(s => int.TryParse(s, out int n) ? n : (int?)null).Where(n => n != null).Select(n => n.Value).ToList();
                if (vals.Any()) query = query.Where(c => vals.Contains(c.Usb3Count));
            }

            if (!string.IsNullOrEmpty(request.SupportedMotherboards)) {
                var supported = request.SupportedMotherboards.Split(',').Select(s => s.Trim().ToLowerInvariant()).ToList();
                query = query.Where(c => supported.Any(s => (c.SupportedMotherboards ?? "").ToLower().Contains(s)));
            }

            string cacheKey = $"TotalCount_Cases_V8_{request.SearchTerm?.ToLowerInvariant()}_{request.Brand?.ToLowerInvariant()}_{request.CaseType?.ToLowerInvariant()}_{request.HasTypeC}_{request.Usb3Count}_{request.SupportedMotherboards?.ToLowerInvariant()}";

            if (!_cache.TryGetValue(cacheKey, out int totalCount))
            {
                totalCount = await query.CountAsync();
                _cache.Set(cacheKey, totalCount, TimeSpan.FromHours(1));
            }

            int pageNum = request.PageNumber > 0 ? request.PageNumber : 1;
            int pageSize = request.PageSize > 0 ? request.PageSize : 25;
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            
            var cases = await query.Skip((pageNum - 1) * pageSize).Take(pageSize).ToListAsync();

            return Ok(new PCPartsAPI.Dtos.PagedResponse<Case> { Data = cases, TotalCount = totalCount, TotalPages = totalPages, HasNextPage = pageNum < totalPages });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Case>> GetCase(int id)
        {
            var caseItem = await _context.Cases.FindAsync(id);
            return caseItem == null ? NotFound() : Ok(caseItem);
        }
    }
}