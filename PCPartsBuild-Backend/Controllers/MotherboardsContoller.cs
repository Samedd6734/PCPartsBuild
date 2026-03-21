using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PCPartsAPI.Data;
using PCPartsAPI.Models;

namespace PCPartsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MotherboardsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MotherboardsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/motherboards
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Motherboard>>> GetMotherboards()
        {
            // Veritabanındaki tüm Motherboard'ları çekip döndürür
            return await _context.Motherboards.ToListAsync();
        }

        // GET: api/motherboards/5 (Tek bir Anakart'ın detayları için)
        [HttpGet("{id}")]
        public async Task<ActionResult<Motherboard>> GetMotherboard(int id)
        {
            var motherboard = await _context.Motherboards.FindAsync(id);

            if (motherboard == null)
            {
                return NotFound();
            }

            return motherboard;
        }
    }
}