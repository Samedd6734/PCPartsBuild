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
    public class CpuCoolersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _cache;

        public CpuCoolersController(ApplicationDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        [HttpGet]
        public async Task<ActionResult<PCPartsAPI.Dtos.PagedResponse<CpuCooler>>> GetCpuCoolers([FromQuery] PCPartsAPI.Dtos.PaginationRequestParams request)
        {
            var query = _context.CpuCoolers.AsQueryable().AsNoTracking();

            if (!string.IsNullOrEmpty(request.SearchTerm)) {
                var search = request.SearchTerm.ToLowerInvariant();
                query = query.Where(c => (c.Brand ?? "").ToLower().Contains(search) || (c.ProductName ?? "").ToLower().Contains(search));
            }

            if (!string.IsNullOrEmpty(request.Brand)) {
                var brands = request.Brand.Split(',').Select(b => b.Trim().ToLowerInvariant()).ToList();
                query = query.Where(c => brands.Contains((c.Brand ?? "").ToLower().Trim()));
            }

            if (!string.IsNullOrEmpty(request.CoolerType)) {
                var types = request.CoolerType.Split(',').Select(s => s.Trim().ToLowerInvariant()).ToList();
                query = query.Where(c => types.Contains((c.CoolerType ?? "").ToLower().Trim()));
            }

            if (!string.IsNullOrEmpty(request.SocketType)) {
                var sockets = request.SocketType.Split(',').Select(s => s.Trim().ToLowerInvariant()).ToList();
                query = query.Where(c => sockets.Any(s => (c.SupportedSockets ?? "").ToLower().Contains(s)));
            }

            if (!string.IsNullOrEmpty(request.TDPCapacityW)) {
                var vals = request.TDPCapacityW.Split(',').Select(s => int.TryParse(s, out int n) ? n : (int?)null).Where(n => n != null).Select(n => n.Value).ToList();
                if (vals.Any()) query = query.Where(c => c.TDPCapacityW >= vals.Min());
            }

            if (!string.IsNullOrEmpty(request.RadiatorSizeMm)) {
                var vals = request.RadiatorSizeMm.Split(',').Select(s => int.TryParse(s, out int n) ? n : (int?)null).Where(n => n != null).Select(n => n.Value).ToList();
                if (vals.Any()) query = query.Where(c => vals.Contains(c.RadiatorSizeMm));
            }

            if (!string.IsNullOrEmpty(request.FanSizeMm)) {
                var vals = request.FanSizeMm.Split(',').Select(s => int.TryParse(s, out int n) ? n : (int?)null).Where(n => n != null).Select(n => n.Value).ToList();
                if (vals.Any()) query = query.Where(c => vals.Contains(c.FanSizeMm));
            }

            if (request.MinPrice.HasValue)
                query = query.Where(c => c.Price >= request.MinPrice.Value);
            if (request.MaxPrice.HasValue)
                query = query.Where(c => c.Price <= request.MaxPrice.Value);

            string cacheKey = $"TotalCount_CpuCoolers_V10_{request.SearchTerm?.ToLowerInvariant()}_{request.Brand?.ToLowerInvariant()}_{request.CoolerType?.ToLowerInvariant()}_{request.SocketType?.ToLowerInvariant()}_{request.TDPCapacityW}_{request.RadiatorSizeMm}_{request.FanSizeMm}_{request.MinPrice}_{request.MaxPrice}";

            if (!_cache.TryGetValue(cacheKey, out int totalCount))
            {
                totalCount = await query.CountAsync();
                _cache.Set(cacheKey, totalCount, TimeSpan.FromHours(1));
            }

            int pageNum = request.PageNumber > 0 ? request.PageNumber : 1;
            int pageSize = request.PageSize > 0 ? request.PageSize : 25;
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            
            // Compatibility Sorting
            if (!string.IsNullOrEmpty(request.CompatibleCpuSocket) || request.CompatibleCpuTdp > 0 || request.CompatibleCaseMaxCoolerHeight > 0 || request.CompatibleCaseFrontRad > 0 || request.CompatibleCaseTopRad > 0)
            {
                var socket = request.CompatibleCpuSocket?.ToLowerInvariant()?.Trim() ?? "";
                var tdp = request.CompatibleCpuTdp ?? 0;
                var maxHeight = request.CompatibleCaseMaxCoolerHeight ?? 0;
                var frontRad = request.CompatibleCaseFrontRad ?? 0;
                var topRad = request.CompatibleCaseTopRad ?? 0;

                query = query.OrderByDescending(c =>
                    (string.IsNullOrEmpty(socket) || (c.SupportedSockets != null && c.SupportedSockets.ToLower().Contains(socket))) &&
                    (tdp == 0 || c.TDPCapacityW == 0 || c.TDPCapacityW >= tdp) &&
                    (maxHeight == 0 || (c.CoolerType != null && c.CoolerType.ToLower().Contains("liquid")) || c.HeightMm == 0 || c.HeightMm <= maxHeight) &&
                    ((frontRad == 0 && topRad == 0) || !(c.CoolerType != null && c.CoolerType.ToLower().Contains("liquid")) || c.RadiatorSizeMm == 0 || (c.RadiatorSizeMm <= frontRad || c.RadiatorSizeMm <= topRad))
                ).ThenBy(c => c.Id);
            }
            else
            {
                query = query.OrderBy(c => c.Id);
            }

            var cpuCoolers = await query.Skip((pageNum - 1) * pageSize).Take(pageSize).ToListAsync();

            return Ok(new PCPartsAPI.Dtos.PagedResponse<CpuCooler> { Data = cpuCoolers, TotalCount = totalCount, TotalPages = totalPages, HasNextPage = pageNum < totalPages });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CpuCooler>> GetCpuCooler(int id)
        {
            var cpuCooler = await _context.CpuCoolers.FindAsync(id);
            return cpuCooler == null ? NotFound() : Ok(cpuCooler);
        }
    }
}