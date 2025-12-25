using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PCPartsAPI.Data;
using PCPartsAPI.Models;

namespace PCPartsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoragesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public StoragesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/storages
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Storage>>> GetStorages()
        {
            // Veritabanındaki tüm Storage'ları çekip döndürür
            return await _context.Storages.ToListAsync();
        }

        // GET: api/storages/5 (Tek bir Depolama'nın detayları için)
        [HttpGet("{id}")]
        public async Task<ActionResult<Storage>> GetStorage(int id)
        {
            var storage = await _context.Storages.FindAsync(id);

            if (storage == null)
            {
                return NotFound();
            }

            return storage;
        }
    }
}