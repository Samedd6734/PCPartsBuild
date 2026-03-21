using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using PCPartsAPI.Models;
using PCPartsAPI.Data;

namespace PCPartsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CasesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CasesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/storages
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Case>>> GetCases()
        {
            // Veritabanındaki tüm Storage'ları çekip döndürür
            return await _context.Cases.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Case>> GetCase(int id)
        {
            var caseItem = await _context.Cases.FindAsync(id);

            if (caseItem == null)
            {
                return NotFound();
            }

            return caseItem;
        }
    }
}
        