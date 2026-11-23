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
                query = query.Where(s => (s.Brand ?? "").ToLower().Contains(search) || (s.ProductName ?? "").ToLower().Contains(search));
            }

            if (!string.IsNullOrEmpty(request.Brand)) {
                var brands = request.Brand.Split(',').Select(b => b.Trim().ToLowerInvariant()).ToList();
                query = query.Where(s => brands.Contains((s.Brand ?? "").ToLower().Trim()));
            }

            if (!string.IsNullOrEmpty(request.FormFactor)) {
                var formFactors = request.FormFactor.Split(',').Select(s => s.Trim().ToLowerInvariant()).ToList();
                query = query.Where(s => formFactors.Contains((s.FormFactor ?? "").ToLower().Trim()));
            }

            if (!string.IsNullOrEmpty(request.CapacityGB)) {
                var vals = request.CapacityGB.Split(',').Select(s => int.TryParse(s, out int n) ? n : (int?)null).Where(n => n != null).Select(n => n.Value).ToList();
                if (vals.Any()) query = query.Where(s => vals.Contains(s.CapacityGB));
            }

            if (!string.IsNullOrEmpty(request.Interface)) {
                var interfaces = request.Interface.Split(',').Select(s => s.Trim().ToLowerInvariant()).ToList();
                query = query.Where(s => interfaces.Contains((s.Interface ?? "").ToLower().Trim()));
            }

            if (!string.IsNullOrEmpty(request.StorageFormFactor)) {
                var formFactors = request.StorageFormFactor.Split(',').Select(s => s.Trim().ToLowerInvariant()).ToList();
                query = query.Where(s => formFactors.Contains((s.FormFactor ?? "").ToLower().Trim()));
            }

            if (request.MinPrice.HasValue)
                query = query.Where(s => s.Price >= request.MinPrice.Value);
            if (request.MaxPrice.HasValue)
                query = query.Where(s => s.Price <= request.MaxPrice.Value);

            string cacheKey = $"TotalCount_Storages_V10_{request.SearchTerm?.ToLowerInvariant()}_{request.Brand?.ToLowerInvariant()}_{request.FormFactor?.ToLowerInvariant()}_{request.StorageFormFactor?.ToLowerInvariant()}_{request.CapacityGB}_{request.Interface?.ToLowerInvariant()}_{request.MinPrice}_{request.MaxPrice}";

            if (!_cache.TryGetValue(cacheKey, out int totalCount))
            {
                totalCount = await query.CountAsync();
                _cache.Set(cacheKey, totalCount, TimeSpan.FromHours(1));
            }

            int pageNum = request.PageNumber > 0 ? request.PageNumber : 1;
            int pageSize = request.PageSize > 0 ? request.PageSize : 25;
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            
            // Compatibility Sorting
            if (request.CompatibleMoboM2Slots == 0 && request.CompatibleMotherboardId != null) // Mobo selected but no M2 slots
            {
                query = query.OrderByDescending(s => !(s.FormFactor != null && s.FormFactor.ToLower().Contains("m.2"))).ThenBy(s => s.Id);
            }
            else
            {
                query = query.OrderBy(s => s.Id);
            }

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