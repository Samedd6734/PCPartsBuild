using Microsoft.AspNetCore.Mvc;
using PCPartsAPI.Data;
using PCPartsAPI.Models;
using System.Linq;

namespace PCPartsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FavoritesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FavoritesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. Favoriye Ekleme
        [HttpPost("add")]
        public IActionResult AddFavorite([FromBody] Favorites favorite)
        {
            // Zaten ekli mi kontrol et
            var exists = _context.Favorites.Any(f =>
                f.UserId == favorite.UserId &&
                f.ComponentType == favorite.ComponentType &&
                f.ComponentId == favorite.ComponentId);

            if (exists)
                return BadRequest("Bu parça zaten favorilerinizde.");

            _context.Favorites.Add(favorite);
            _context.SaveChanges();
            return Ok(new { Message = "Favorilere eklendi." });
        }

        // 2. Favoriden Çıkarma
        [HttpPost("remove")]
        public IActionResult RemoveFavorite([FromBody] Favorites favorite)
        {
            var item = _context.Favorites.FirstOrDefault(f =>
                f.UserId == favorite.UserId &&
                f.ComponentType == favorite.ComponentType &&
                f.ComponentId == favorite.ComponentId);

            if (item == null)
                return NotFound("Favori bulunamadı.");

            _context.Favorites.Remove(item);
            _context.SaveChanges();
            return Ok(new { Message = "Favorilerden kaldırıldı." });
        }

        // 3. Kullanıcının Favorilerini Getir
        [HttpGet("{userId}")]
        public IActionResult GetUserFavorites(string userId)
        {
            var favorites = _context.Favorites.Where(f => f.UserId == userId).ToList();
            return Ok(favorites);
        }
    }
}