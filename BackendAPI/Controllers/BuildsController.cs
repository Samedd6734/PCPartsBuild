using Microsoft.AspNetCore.Mvc;
using PCPartsAPI.Data;
using PCPartsAPI.Models;
using System.Linq;

namespace PCPartsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BuildsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BuildsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. Sistemi Kaydet
        [HttpPost("save")]
        public IActionResult SaveBuild([FromBody] SavedBuilds build)
        {
            if (string.IsNullOrEmpty(build.UserId))
                return BadRequest("Kullanıcı girişi yapılmamış.");

            build.CreatedAt = System.DateTime.Now;
            _context.SavedBuilds.Add(build);
            _context.SaveChanges();

            return Ok(new { Message = "Sistem başarıyla kaydedildi!", BuildId = build.Id });
        }

        // 2. Kullanıcının Kayıtlı Sistemlerini Getir
        [HttpGet("{userId}")]
        public IActionResult GetUserBuilds(string userId)
        {
            var builds = _context.SavedBuilds
                                 .Where(b => b.UserId == userId)
                                 .OrderByDescending(b => b.CreatedAt) // En yeniler üstte
                                 .ToList();
            return Ok(builds);
        }

        // 3. Sistemi Sil
        [HttpDelete("{id}")]
        public IActionResult DeleteBuild(int id)
        {
            var build = _context.SavedBuilds.Find(id);
            if (build == null) return NotFound();

            _context.SavedBuilds.Remove(build);
            _context.SaveChanges();
            return Ok(new { Message = "Sistem silindi." });
        }
    }
}