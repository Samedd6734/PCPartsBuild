using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using PCPartsAPI.Data;
using PCPartsAPI.Models;

namespace PCPartsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoragesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _cache;

        public StoragesController(ApplicationDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        [HttpGet]
        public async Task<ActionResult<PCPartsAPI.Dtos.PagedResponse<Storage>>> GetStorages([FromQuery] PCPartsAPI.Dtos.PaginationRequestParams request)
        {
            var query = _context.Storages.AsQueryable().AsNoTracking();

            if (!string.IsNullOrEmpty(request.SearchTerm)) {
                var search = request.SearchTerm.ToLowerInvariant();
                query = query.Where(s => (s.Brand ?? "").ToLower().Contains(search) || (s.ModelName ?? "").ToLower().Contains(search));
            }

            if (!string.IsNullOrEmpty(request.Brand)) {
                var brands = request.Brand.Split(',').Select(b => b.Trim().ToLowerInvariant()).ToList();
                query = query.Where(s => brands.Contains((s.Brand ?? "").ToLower().Trim()));
            }

            if (!string.IsNullOrEmpty(request.StorageType)) {
                var types = request.StorageType.Split(',').Select(s => s.Trim().ToLowerInvariant()).ToList();
                query = query.Where(s => types.Contains((s.StorageType ?? "").ToLower().Trim()));
            }

            if (!string.IsNullOrEmpty(request.TotalCapacity)) {
                var vals = request.TotalCapacity.Split(',').Select(s => int.TryParse(s, out int n) ? n : (int?)null).Where(n => n != null).Select(n => n.Value).ToList();
                if (vals.Any()) query = query.Where(s => vals.Contains(s.Capacity));
            }

            if (!string.IsNullOrEmpty(request.IsNvme)) {
                var boolVals = request.IsNvme.Split(',').Select(s => bool.TryParse(s, out bool b) ? b : (bool?)null).Where(b => b != null).Select(b => b.Value).ToList();
                if (boolVals.Any()) query = query.Where(s => boolVals.Contains(s.IsNvme));
            }

            if (!string.IsNullOrEmpty(request.FormFactor)) {
                var formFactors = request.FormFactor.Split(',').Select(s => s.Trim().ToLowerInvariant()).ToList();
                query = query.Where(s => formFactors.Contains((s.FormFactor ?? "").ToLower().Trim()));
            }

            if (!string.IsNullOrEmpty(request.HasDramCache)) {
                var boolVals = request.HasDramCache.Split(',').Select(s => bool.TryParse(s, out bool b) ? b : (bool?)null).Where(b => b != null).Select(b => b.Value).ToList();
                if (boolVals.Any()) query = query.Where(s => boolVals.Contains(s.HasDramCache));
            }

            if (!string.IsNullOrEmpty(request.Interface)) {
                var interfaces = request.Interface.Split(',').Select(s => s.Trim().ToLowerInvariant()).ToList();
                query = query.Where(s => interfaces.Contains((s.Interface ?? "").ToLower().Trim()));
            }

            if (!string.IsNullOrEmpty(request.NandType)) {
                var nandTypes = request.NandType.Split(',').Select(s => s.Trim().ToLowerInvariant()).ToList();
                query = query.Where(s => nandTypes.Contains((s.NandType ?? "").ToLower().Trim()));
            }

            string cacheKey = $"TotalCount_Storages_V8_{request.SearchTerm?.ToLowerInvariant()}_{request.Brand?.ToLowerInvariant()}_{request.StorageType?.ToLowerInvariant()}_{request.TotalCapacity}_{request.FormFactor?.ToLowerInvariant()}_{request.IsNvme}_{request.HasDramCache}_{request.Interface?.ToLowerInvariant()}_{request.NandType?.ToLowerInvariant()}";

            if (!_cache.TryGetValue(cacheKey, out int totalCount))
            {
                totalCount = await query.CountAsync();
                _cache.Set(cacheKey, totalCount, TimeSpan.FromHours(1));
            }

            int pageNum = request.PageNumber > 0 ? request.PageNumber : 1;
            int pageSize = request.PageSize > 0 ? request.PageSize : 25;
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            
            var storages = await query.Skip((pageNum - 1) * pageSize).Take(pageSize).ToListAsync();

            return Ok(new PCPartsAPI.Dtos.PagedResponse<Storage> { Data = storages, TotalCount = totalCount, TotalPages = totalPages, HasNextPage = pageNum < totalPages });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Storage>> GetStorage(int id)
        {
            var storage = await _context.Storages.FindAsync(id);
            return storage == null ? NotFound() : Ok(storage);
        }
    }
}