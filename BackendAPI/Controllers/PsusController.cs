using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PCPartsAPI.Data;
using PCPartsAPI.Models;

namespace PCPartsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PsusController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PsusController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Psu>>> GetPsus()
        {
            return await _context.Psus.ToListAsync();
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Psu>> GetPsu(int id)
        {
            var psu = await _context.Psus.FindAsync(id);

            if (psu == null)
            {
                return NotFound();
            }

            return psu;
        }
    }
}