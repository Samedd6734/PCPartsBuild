using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using PCPartsAPI.Data;
using PCPartsAPI.Models;

namespace PCPartsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MotherboardsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _cache;

        public MotherboardsController(ApplicationDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        [HttpGet]
        public async Task<ActionResult<PCPartsAPI.Dtos.PagedResponse<Motherboard>>> GetMotherboards([FromQuery] PCPartsAPI.Dtos.PaginationRequestParams request)
        {
            var query = _context.Motherboards.AsQueryable().AsNoTracking();

            if (!string.IsNullOrEmpty(request.SearchTerm)) {
                var search = request.SearchTerm.ToLowerInvariant();
                query = query.Where(m => (m.Brand ?? "").ToLower().Contains(search) || (m.ModelName ?? "").ToLower().Contains(search));
            }

            if (!string.IsNullOrEmpty(request.Brand)) {
                var brands = request.Brand.Split(',').Select(b => b.Trim().ToLowerInvariant()).ToList();
                query = query.Where(m => brands.Contains((m.Brand ?? "").ToLower().Trim()));
            }

            if (!string.IsNullOrEmpty(request.Socket)) {
                var sockets = request.Socket.Split(',').Select(s => s.Trim().ToLowerInvariant()).ToList();
                query = query.Where(m => sockets.Contains((m.Socket ?? "").ToLower().Trim()));
            }

            if (!string.IsNullOrEmpty(request.Chipset)) {
                var chipsets = request.Chipset.Split(',').Select(s => s.Trim().ToLowerInvariant()).ToList();
                query = query.Where(m => chipsets.Contains((m.Chipset ?? "").ToLower().Trim()));
            }

            if (!string.IsNullOrEmpty(request.FormFactor)) {
                var formFactors = request.FormFactor.Split(',').Select(s => s.Trim().ToLowerInvariant()).ToList();
                query = query.Where(m => formFactors.Contains((m.FormFactor ?? "").ToLower().Trim()));
            }

            if (!string.IsNullOrEmpty(request.MemoryType)) {
                var memTypes = request.MemoryType.Split(',').Select(s => s.Trim().ToLowerInvariant()).ToList();
                query = query.Where(m => memTypes.Contains((m.MemoryType ?? "").ToLower().Trim()));
            }

            if (!string.IsNullOrEmpty(request.IntegratedWifi)) {
                var boolVals = request.IntegratedWifi.Split(',').Select(s => bool.TryParse(s, out bool b) ? b : (bool?)null).Where(b => b != null).Select(b => b.Value).ToList();
                if (boolVals.Any()) query = query.Where(m => boolVals.Contains(m.IntegratedWifi));
            }

            if (!string.IsNullOrEmpty(request.IntegratedBluetooth)) {
                var boolVals = request.IntegratedBluetooth.Split(',').Select(s => bool.TryParse(s, out bool b) ? b : (bool?)null).Where(b => b != null).Select(b => b.Value).ToList();
                if (boolVals.Any()) query = query.Where(m => boolVals.Contains(m.IntegratedBluetooth));
            }

            if (!string.IsNullOrEmpty(request.ArgbSupport)) {
                var boolVals = request.ArgbSupport.Split(',').Select(s => bool.TryParse(s, out bool b) ? b : (bool?)null).Where(b => b != null).Select(b => b.Value).ToList();
                if (boolVals.Any()) query = query.Where(m => boolVals.Contains(m.ArgbSupport));
            }

            if (!string.IsNullOrEmpty(request.M2SlotCount)) {
                var counts = request.M2SlotCount.Split(',').Select(s => int.TryParse(s, out int n) ? n : (int?)null).Where(n => n != null).Select(n => n.Value).ToList();
                if (counts.Any()) query = query.Where(m => counts.Contains(m.M2SlotCount));
            }

            string cacheKey = $"TotalCount_Motherboards_V9_{request.SearchTerm?.ToLowerInvariant()}_{request.Brand?.ToLowerInvariant()}_{request.Socket?.ToLowerInvariant()}_{request.Chipset?.ToLowerInvariant()}_{request.FormFactor?.ToLowerInvariant()}_{request.MemoryType?.ToLowerInvariant()}_{request.IntegratedWifi}_{request.IntegratedBluetooth}_{request.ArgbSupport}_{request.M2SlotCount}";

            if (!_cache.TryGetValue(cacheKey, out int totalCount))
            {
                totalCount = await query.CountAsync();
                _cache.Set(cacheKey, totalCount, TimeSpan.FromHours(1));
            }

            int pageNum = request.PageNumber > 0 ? request.PageNumber : 1;
            int pageSize = request.PageSize > 0 ? request.PageSize : 25;
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            
            // Compatibility Sorting
            if (!string.IsNullOrEmpty(request.CompatibleCpuSocket) || !string.IsNullOrEmpty(request.CompatibleRamMemoryType))
            {
                var cpuSocket = request.CompatibleCpuSocket?.ToLowerInvariant()?.Trim();
                var ramMemType = request.CompatibleRamMemoryType?.ToLowerInvariant()?.Trim();

                query = query.OrderByDescending(m => 
                    (string.IsNullOrEmpty(cpuSocket) || (m.Socket != null && m.Socket.ToLower() == cpuSocket)) &&
                    (string.IsNullOrEmpty(ramMemType) || (m.MemoryType != null && m.MemoryType.ToLower() == ramMemType))
                ).ThenBy(m => m.Id);
            }
            else
            {
                query = query.OrderBy(m => m.Id);
            }

            var motherboards = await query.Skip((pageNum - 1) * pageSize).Take(pageSize).ToListAsync();

            return Ok(new PCPartsAPI.Dtos.PagedResponse<Motherboard> { Data = motherboards, TotalCount = totalCount, TotalPages = totalPages, HasNextPage = pageNum < totalPages });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Motherboard>> GetMotherboard(int id)
        {
            var motherboard = await _context.Motherboards.FindAsync(id);
            return motherboard == null ? NotFound() : Ok(motherboard);
        }
    }
}