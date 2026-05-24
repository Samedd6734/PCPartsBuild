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
                query = query.Where(c => (c.Brand ?? "").ToLower().Contains(search) || (c.ProductName ?? "").ToLower().Contains(search));
            }

            if (!string.IsNullOrEmpty(request.Brand)) {
                var brands = request.Brand.Split(',').Select(b => b.Trim().ToLowerInvariant()).ToList();
                query = query.Where(c => brands.Contains((c.Brand ?? "").ToLower().Trim()));
            }

            if (!string.IsNullOrEmpty(request.SupportedMotherboardFormFactors)) {
                var supported = request.SupportedMotherboardFormFactors.Split(',')
                    .Select(s => s.Trim().ToLowerInvariant())
                    .ToList();
                
                int requiredSize = 0;
                if (supported.Contains("e-atx") || supported.Contains("eatx")) requiredSize = 4;
                else if (supported.Contains("atx")) requiredSize = 3;
                else if (supported.Contains("micro atx") || supported.Contains("matx") || supported.Contains("micro-atx")) requiredSize = 2;
                else if (supported.Contains("mini itx") || supported.Contains("mitx") || supported.Contains("mini-itx")) requiredSize = 1;

                if (requiredSize > 0)
                {
                    if (requiredSize == 4) // E-ATX required
                    {
                        query = query.Where(c => (c.SupportedMotherboardFormFactors ?? "").ToLower().Contains("e-atx") 
                                              || (c.SupportedMotherboardFormFactors ?? "").ToLower().Contains("eatx")
                                              || (c.SupportedMotherboardFormFactors ?? "").ToLower().Contains("extended"));
                    }
                    else if (requiredSize == 3) // ATX required (E-ATX or ATX cases)
                    {
                        query = query.Where(c => (c.SupportedMotherboardFormFactors ?? "").ToLower().Contains("e-atx") 
                                              || (c.SupportedMotherboardFormFactors ?? "").ToLower().Contains("eatx")
                                              || (c.SupportedMotherboardFormFactors ?? "").ToLower().Contains("extended")
                                              || (c.SupportedMotherboardFormFactors ?? "").ToLower().Contains("atx"));
                    }
                    else if (requiredSize == 2) // Micro ATX required (E-ATX, ATX, or Micro ATX cases)
                    {
                        query = query.Where(c => (c.SupportedMotherboardFormFactors ?? "").ToLower().Contains("e-atx") 
                                              || (c.SupportedMotherboardFormFactors ?? "").ToLower().Contains("eatx")
                                              || (c.SupportedMotherboardFormFactors ?? "").ToLower().Contains("extended")
                                              || (c.SupportedMotherboardFormFactors ?? "").ToLower().Contains("atx")
                                              || (c.SupportedMotherboardFormFactors ?? "").ToLower().Contains("micro")
                                              || (c.SupportedMotherboardFormFactors ?? "").ToLower().Contains("matx"));
                    }
                    // Mini ITX required matches all cases, so no filter is needed.
                }
            }

            // Compatibility Filtering - seçilen anakart form faktörünü desteklemeyen kasaları filtrele
            if (!string.IsNullOrEmpty(request.CompatibleMotherboardFormFactor))
            {
                var moboForm = request.CompatibleMotherboardFormFactor.ToLowerInvariant().Trim();
                int moboSize = 0;
                if (moboForm.Contains("e-atx") || moboForm.Contains("eatx") || moboForm.Contains("extended")) moboSize = 4;
                else if (moboForm.Contains("atx")) moboSize = 3;
                else if (moboForm.Contains("micro") || moboForm.Contains("matx") || moboForm.Contains("m-atx")) moboSize = 2;
                else if (moboForm.Contains("mini") || moboForm.Contains("mitx") || moboForm.Contains("m-itx") || moboForm.Contains("itx")) moboSize = 1;

                if (moboSize == 4) // E-ATX anakart -> sadece E-ATX destekleyen kasalar
                {
                    query = query.Where(c => (c.SupportedMotherboardFormFactors ?? "").ToLower().Contains("e-atx") 
                                          || (c.SupportedMotherboardFormFactors ?? "").ToLower().Contains("eatx")
                                          || (c.SupportedMotherboardFormFactors ?? "").ToLower().Contains("extended"));
                }
                else if (moboSize == 3) // ATX anakart -> ATX veya daha büyük kasalar
                {
                    query = query.Where(c => (c.SupportedMotherboardFormFactors ?? "").ToLower().Contains("e-atx") 
                                          || (c.SupportedMotherboardFormFactors ?? "").ToLower().Contains("eatx")
                                          || (c.SupportedMotherboardFormFactors ?? "").ToLower().Contains("extended")
                                          || (c.SupportedMotherboardFormFactors ?? "").ToLower().Contains("atx"));
                }
                else if (moboSize == 2) // Micro ATX anakart -> Micro ATX, ATX veya daha büyük kasalar
                {
                    query = query.Where(c => (c.SupportedMotherboardFormFactors ?? "").ToLower().Contains("e-atx") 
                                          || (c.SupportedMotherboardFormFactors ?? "").ToLower().Contains("eatx")
                                          || (c.SupportedMotherboardFormFactors ?? "").ToLower().Contains("extended")
                                          || (c.SupportedMotherboardFormFactors ?? "").ToLower().Contains("atx")
                                          || (c.SupportedMotherboardFormFactors ?? "").ToLower().Contains("micro")
                                          || (c.SupportedMotherboardFormFactors ?? "").ToLower().Contains("matx"));
                }
                // Mini ITX (moboSize == 1) tüm kasalara uyar, filtre gerekmez
            }

            if (!string.IsNullOrEmpty(request.HasBuiltInPSU)) {
                var boolVals = request.HasBuiltInPSU.Split(',')
                    .Select(s => bool.TryParse(s, out bool b) ? b : (bool?)null)
                    .Where(b => b != null).Select(b => b.Value).ToList();
                if (boolVals.Any()) query = query.Where(c => boolVals.Contains(c.HasBuiltInPSU));
            }

            if (request.MinPrice.HasValue)
                query = query.Where(c => c.Price >= request.MinPrice.Value);
            if (request.MaxPrice.HasValue)
                query = query.Where(c => c.Price <= request.MaxPrice.Value);

            string cacheKey = $"TotalCount_Cases_V11_{request.SearchTerm?.ToLowerInvariant()}_{request.Brand?.ToLowerInvariant()}_{request.SupportedMotherboardFormFactors?.ToLowerInvariant()}_{request.CompatibleMotherboardFormFactor?.ToLowerInvariant()}_{request.HasBuiltInPSU}_{request.MinPrice}_{request.MaxPrice}";

            if (!_cache.TryGetValue(cacheKey, out int totalCount))
            {
                totalCount = await query.CountAsync();
                _cache.Set(cacheKey, totalCount, TimeSpan.FromHours(1));
            }

            int pageNum = request.PageNumber > 0 ? request.PageNumber : 1;
            int pageSize = request.PageSize > 0 ? request.PageSize : 25;
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            
            // Compatibility Sorting
            if (!string.IsNullOrEmpty(request.CompatibleMotherboardFormFactor) || request.CompatibleGpuLength > 0 || request.CompatibleCoolerHeight > 0 || request.CompatibleCoolerRadiatorSize > 0)
            {
                var moboForm = request.CompatibleMotherboardFormFactor?.ToLowerInvariant()?.Trim();
                var gpuLen = request.CompatibleGpuLength ?? 0;
                var coolerHeight = request.CompatibleCoolerHeight ?? 0;
                var radSize = request.CompatibleCoolerRadiatorSize ?? 0;

                int requiredSize = 0;
                if (!string.IsNullOrEmpty(moboForm)) {
                    if (moboForm.Contains("e-atx") || moboForm.Contains("eatx")) requiredSize = 4;
                    else if (moboForm.Contains("atx")) requiredSize = 3;
                    else if (moboForm.Contains("micro") || moboForm.Contains("matx")) requiredSize = 2;
                    else if (moboForm.Contains("mini") || moboForm.Contains("itx")) requiredSize = 1;
                }

                query = query.OrderByDescending(c =>
                    (requiredSize == 0 || (
                        requiredSize == 4 ? (c.SupportedMotherboardFormFactors ?? "").ToLower().Contains("e-atx") || (c.SupportedMotherboardFormFactors ?? "").ToLower().Contains("eatx") || (c.SupportedMotherboardFormFactors ?? "").ToLower().Contains("extended") :
                        requiredSize == 3 ? (c.SupportedMotherboardFormFactors ?? "").ToLower().Contains("e-atx") || (c.SupportedMotherboardFormFactors ?? "").ToLower().Contains("eatx") || (c.SupportedMotherboardFormFactors ?? "").ToLower().Contains("extended") || (c.SupportedMotherboardFormFactors ?? "").ToLower().Contains("atx") :
                        requiredSize == 2 ? (c.SupportedMotherboardFormFactors ?? "").ToLower().Contains("e-atx") || (c.SupportedMotherboardFormFactors ?? "").ToLower().Contains("eatx") || (c.SupportedMotherboardFormFactors ?? "").ToLower().Contains("extended") || (c.SupportedMotherboardFormFactors ?? "").ToLower().Contains("atx") || (c.SupportedMotherboardFormFactors ?? "").ToLower().Contains("micro") || (c.SupportedMotherboardFormFactors ?? "").ToLower().Contains("matx") : true
                    )) &&
                    (gpuLen == 0 || c.MaxGPULengthMm == 0 || c.MaxGPULengthMm >= gpuLen) &&
                    (coolerHeight == 0 || c.MaxCPUCoolerHeightMm == 0 || c.MaxCPUCoolerHeightMm >= coolerHeight) &&
                    (radSize == 0 || (c.FrontRadiatorSupportMm >= radSize || c.TopRadiatorSupportMm >= radSize))
                ).ThenBy(c => c.Id);
            }
            else
            {
                query = query.OrderBy(c => c.Id);
            }

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