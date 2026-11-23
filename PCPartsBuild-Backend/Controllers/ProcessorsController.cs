using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using PCPartsAPI.Data;
using PCPartsAPI.Models;

namespace PCPartsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProcessorsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _cache;

        public ProcessorsController(ApplicationDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        [HttpGet]
        public async Task<ActionResult<PCPartsAPI.Dtos.PagedResponse<Processor>>> GetProcessors([FromQuery] PCPartsAPI.Dtos.PaginationRequestParams request)
        {
            var query = _context.Processors.AsQueryable().AsNoTracking();

            if (!string.IsNullOrEmpty(request.SearchTerm)) {
                var search = request.SearchTerm.ToLowerInvariant();
                query = query.Where(p => (p.Brand ?? "").ToLower().Contains(search) || (p.ProductName ?? "").ToLower().Contains(search));
            }

            if (!string.IsNullOrEmpty(request.Brand)) {
                var brands = request.Brand.Split(',').Select(b => b.Trim().ToLowerInvariant()).ToList();
                query = query.Where(p => brands.Contains((p.Brand ?? "").ToLower().Trim()));
            }

            if (!string.IsNullOrEmpty(request.SocketType)) {
                var sockets = request.SocketType.Split(',').Select(s => s.Trim().ToLowerInvariant()).ToList();
                query = query.Where(p => sockets.Contains((p.SocketType ?? "").ToLower().Trim()));
            }

            if (!string.IsNullOrEmpty(request.CoreCount)) {
                var coreCounts = request.CoreCount.Split(',')
                    .Select(s => int.TryParse(s, out int n) ? n : (int?)null)
                    .Where(n => n != null).Select(n => n.Value).ToList();
                if (coreCounts.Any()) query = query.Where(p => coreCounts.Contains(p.CoreCount));
            }

            if (!string.IsNullOrEmpty(request.ThreadCount)) {
                var threadCounts = request.ThreadCount.Split(',')
                    .Select(s => int.TryParse(s, out int n) ? n : (int?)null)
                    .Where(n => n != null).Select(n => n.Value).ToList();
                if (threadCounts.Any()) query = query.Where(p => threadCounts.Contains(p.ThreadCount));
            }

            if (!string.IsNullOrEmpty(request.IntegratedGraphics)) {
                var boolVals = request.IntegratedGraphics.Split(',')
                    .Select(s => bool.TryParse(s, out bool b) ? b : (bool?)null)
                    .Where(b => b != null).Select(b => b.Value).ToList();
                if (boolVals.Any()) query = query.Where(p => boolVals.Contains(p.HasIntegratedGraphics));
            }

            if (!string.IsNullOrEmpty(request.TDP)) {
                var tdps = request.TDP.Split(',')
                    .Select(s => int.TryParse(s, out int n) ? n : (int?)null)
                    .Where(n => n != null).Select(n => n.Value).ToList();
                if (tdps.Any()) query = query.Where(p => tdps.Contains(p.TDP));
            }

            if (!string.IsNullOrEmpty(request.SupportedMemoryType)) {
                var memTypes = request.SupportedMemoryType.Split(',').Select(s => s.Trim().ToLowerInvariant()).ToList();
                query = query.Where(p => memTypes.Contains((p.SupportedMemoryType ?? "").ToLower().Trim()));
            }

            if (!string.IsNullOrEmpty(request.PCIeVersion)) {
                var pcies = request.PCIeVersion.Split(',').Select(s => s.Trim().ToLowerInvariant()).ToList();
                query = query.Where(p => pcies.Contains((p.PCIeVersion ?? "").ToLower().Trim()));
            }

            if (request.MinPrice.HasValue)
                query = query.Where(p => p.Price >= request.MinPrice.Value);
            if (request.MaxPrice.HasValue)
                query = query.Where(p => p.Price <= request.MaxPrice.Value);

            string cacheKey = $"TotalCount_Processors_V10_{request.SearchTerm?.ToLowerInvariant()}_{request.Brand?.ToLowerInvariant()}_{request.SocketType?.ToLowerInvariant()}_{request.CoreCount}_{request.ThreadCount}_{request.IntegratedGraphics}_{request.TDP}_{request.SupportedMemoryType}_{request.PCIeVersion}_{request.MinPrice}_{request.MaxPrice}";

            if (!_cache.TryGetValue(cacheKey, out int totalCount))
            {
                totalCount = await query.CountAsync();
                _cache.Set(cacheKey, totalCount, TimeSpan.FromHours(1));
            }

            int pageNum = request.PageNumber > 0 ? request.PageNumber : 1;
            int pageSize = request.PageSize > 0 ? request.PageSize : 25;
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            
            // Compatibility Sorting
            if (!string.IsNullOrEmpty(request.CompatibleMotherboardSocket))
            {
                var moboSocket = request.CompatibleMotherboardSocket.ToLowerInvariant().Trim();
                query = query.OrderByDescending(p => p.SocketType != null && p.SocketType.ToLower() == moboSocket).ThenBy(p => p.Id);
            }
            else
            {
                query = query.OrderBy(p => p.Id);
            }

            var processors = await query
                .Skip((pageNum - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new PCPartsAPI.Dtos.PagedResponse<Processor>
            {
                Data = processors,
                TotalCount = totalCount,
                TotalPages = totalPages,
                HasNextPage = pageNum < totalPages
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Processor>> GetProcessor(int id)
        {
            var processor = await _context.Processors.FindAsync(id);
            return processor == null ? NotFound() : Ok(processor);
        }
    }
}