using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PCPartsAPI.Data;
using PCPartsAPI.Models;

namespace PCPartsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RamsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RamsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/rams
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Ram>>> GetRams()
        {
            // Veritabanındaki tüm Ram'leri çekip döndürür
            return await _context.Rams.ToListAsync();
        }

        // GET: api/rams/5 (Tek bir RAM'in detayları için)
        [HttpGet("{id}")]
        public async Task<ActionResult<Ram>> GetRam(int id)
        {
            var ram = await _context.Rams.FindAsync(id);

            if (ram == null)
            {
                return NotFound();
            }

            return ram;
        }
    }
}