using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using PCPartsAPI.Models;
using PCPartsAPI.Data;

namespace PCPartsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CpuCoolersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CpuCoolersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/cpucoolers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CpuCooler>>> GetCpuCoolers()
        {
            // Yorumu da düzelttim
            // Veritabanındaki tüm CpuCooler'ları çekip döndürür
            return await _context.CpuCoolers.ToListAsync();
        }

        // GET: api/cpucoolers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CpuCooler>> GetCpuCooler(int id)
        {
            // Değişken adını daha anlaşılır yaptım (CpuCoolers -> cpuCooler)
            var cpuCooler = await _context.CpuCoolers.FindAsync(id);

            if (cpuCooler == null)
            {
                return NotFound();
            }

            return cpuCooler;
        }
    }
}