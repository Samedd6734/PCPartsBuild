using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PCPartsAPI.Data;
using PCPartsAPI.Models;

namespace PCPartsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GpusController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public GpusController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/gpus
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Gpu>>> GetGpus()
        {
            // Veritabanındaki tüm Gpu'ları çekip döndürür
            return await _context.Gpus.ToListAsync();
        }

        // GET: api/gpus/5 (Tek bir GPU'nun detayları için)
        [HttpGet("{id}")]
        public async Task<ActionResult<Gpu>> GetGpu(int id)
        {
            var gpu = await _context.Gpus.FindAsync(id);

            if (gpu == null)
            {
                return NotFound();
            }

            return gpu;
        }
    }
}