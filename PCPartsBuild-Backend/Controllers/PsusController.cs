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
                query = query.Where(p => (p.Brand ?? "").ToLower().Contains(search) || (p.ProductName ?? "").ToLower().Contains(search));
            }

            if (!string.IsNullOrEmpty(request.Brand)) {
                var brands = request.Brand.Split(',').Select(b => b.Trim().ToLowerInvariant()).ToList();
                query = query.Where(p => brands.Contains((p.Brand ?? "").ToLower().Trim()));
            }

            if (!string.IsNullOrEmpty(request.WattageW)) {
                var vals = request.WattageW.Split(',').Select(s => int.TryParse(s, out int n) ? n : (int?)null).Where(n => n != null).Select(n => n.Value).ToList();
                if (vals.Any()) query = query.Where(p => vals.Contains(p.WattageW));
            }

            if (!string.IsNullOrEmpty(request.IsModular)) {
                var modTypes = request.IsModular.Split(',').Select(s => s.Trim().ToLowerInvariant()).ToList();
                query = query.Where(p => modTypes.Contains((p.IsModular ?? "").ToLower().Trim()));
            }

            if (!string.IsNullOrEmpty(request.Certification)) {
                var certs = request.Certification.Split(',').Select(r => r.Trim().ToLowerInvariant()).ToList();
                query = query.Where(p => certs.Contains((p.Certification ?? "").ToLower().Trim()));
            }

            if (!string.IsNullOrEmpty(request.PsuFormFactor)) {
                var formFactors = request.PsuFormFactor.Split(',').Select(s => s.Trim().ToLowerInvariant()).ToList();
                query = query.Where(p => formFactors.Contains((p.FormFactor ?? "").ToLower().Trim()));
            }

            if (!string.IsNullOrEmpty(request.ATXVersion)) {
                var versions = request.ATXVersion.Split(',').Select(s => s.Trim().ToLowerInvariant()).ToList();
                query = query.Where(p => versions.Contains((p.ATXVersion ?? "").ToLower().Trim()));
            }

            if (request.MinPrice.HasValue)
                query = query.Where(p => p.Price >= request.MinPrice.Value);
            if (request.MaxPrice.HasValue)
                query = query.Where(p => p.Price <= request.MaxPrice.Value);

            string cacheKey = $"TotalCount_Psus_V11_{request.SearchTerm?.ToLowerInvariant()}_{request.Brand?.ToLowerInvariant()}_{request.WattageW}_{request.IsModular?.ToLowerInvariant()}_{request.Certification?.ToLowerInvariant()}_{request.PsuFormFactor?.ToLowerInvariant()}_{request.ATXVersion?.ToLowerInvariant()}_{request.MinPrice}_{request.MaxPrice}_{request.CompatibleCpuTdp}_{request.CompatibleGpuTdp}";

            if (!_cache.TryGetValue(cacheKey, out int totalCount))
            {
                totalCount = await query.CountAsync();
                _cache.Set(cacheKey, totalCount, TimeSpan.FromHours(1));
            }

            int pageNum = request.PageNumber > 0 ? request.PageNumber : 1;
            int pageSize = request.PageSize > 0 ? request.PageSize : 25;
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            
            // Compatibility Sorting
            if (request.CompatibleCpuTdp > 0 || request.CompatibleGpuTdp > 0)
            {
                var totalTdp = (request.CompatibleCpuTdp ?? 0) + (request.CompatibleGpuTdp ?? 0);
                var requiredWattage = (int)Math.Ceiling(totalTdp * 1.25);
                query = query.OrderByDescending(p => p.WattageW >= requiredWattage).ThenByDescending(p => p.WattageW).ThenBy(p => p.Id);
            }
            else
            {
                query = query.OrderBy(p => p.Id);
            }

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