using Microsoft.AspNetCore.Mvc;
using PCPartsAPI.Data;
using PCPartsAPI.Models;
using PCPartsAPI.DTOs; // DTO klasörünü eklemeyi unutma
using System.Linq;
using System.Collections.Generic;

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

        // 1. Ekleme ve 2. Silme metodları aynen kalsın...
        // (Buraya ekleme/silme kodlarını tekrar yapıştırmıyorum, onlar değişmedi)
        [HttpPost("add")]
        public IActionResult AddFavorite([FromBody] Favorites favorite)
        {
            var exists = _context.Favorites.Any(f =>
               f.UserId == favorite.UserId &&
               f.ComponentType == favorite.ComponentType &&
               f.ComponentId == favorite.ComponentId);

            if (exists) return BadRequest("Bu parça zaten favorilerinizde.");

            _context.Favorites.Add(favorite);
            _context.SaveChanges();
            return Ok(new { Message = "Favorilere eklendi." });
        }

        [HttpPost("remove")]
        public IActionResult RemoveFavorite([FromBody] Favorites favorite)
        {
            var item = _context.Favorites.FirstOrDefault(f =>
                f.UserId == favorite.UserId &&
                f.ComponentType == favorite.ComponentType &&
                f.ComponentId == favorite.ComponentId);

            if (item == null) return NotFound("Favori bulunamadı.");

            _context.Favorites.Remove(item);
            _context.SaveChanges();
            return Ok(new { Message = "Favorilerden kaldırıldı." });
        }

        // --- GÜNCELLENEN KISIM: DETAYLI FAVORİ GETİRME ---
        [HttpGet("{userId}")]
        public IActionResult GetUserFavorites(string userId)
        {
            var favorites = _context.Favorites.Where(f => f.UserId == userId).ToList();
            var dtoList = new List<FavoriteDetailDto>();

            foreach (var fav in favorites)
            {
                var dto = new FavoriteDetailDto
                {
                    Id = fav.Id,
                    ComponentId = fav.ComponentId,
                    ComponentType = fav.ComponentType
                };

                // Türüne göre detayları bul
                switch (fav.ComponentType.ToLower())
                {
                    case "cpu":
                        var cpu = _context.Processors.Find(fav.ComponentId);
                        if (cpu != null)
                        {
                            dto.Brand = cpu.Brand;
                            dto.Name = $"{cpu.Brand} {cpu.ModelName}";
                            dto.ImageUrl = cpu.ImageUrl;
                            dto.Specs = $"{cpu.CoreCount} Çekirdek, {cpu.Socket}";
                        }
                        break;

                    case "motherboard":
                        var mobo = _context.Motherboards.Find(fav.ComponentId);
                        if (mobo != null)
                        {
                            dto.Brand = mobo.Brand;
                            dto.Name = $"{mobo.Brand} {mobo.ModelName}";
                            dto.ImageUrl = mobo.ImageUrl;
                            dto.Specs = $"{mobo.FormFactor}, {mobo.Socket}, {mobo.Chipset}";
                        }
                        break;

                    case "ram":
                        var ram = _context.Rams.Find(fav.ComponentId);
                        if (ram != null)
                        {
                            dto.Brand = ram.Brand;
                            dto.Name = $"{ram.Brand} {ram.ModelName}";
                            dto.ImageUrl = ram.ImageUrl;
                            dto.Specs = $"{ram.MemoryType}, {ram.Speed}MHz, {ram.TotalCapacity}GB";
                        }
                        break;

                    case "gpu":
                        var gpu = _context.Gpus.Find(fav.ComponentId);
                        if (gpu != null)
                        {
                            dto.Brand = gpu.Brand;
                            dto.Name = $"{gpu.Brand} {gpu.ModelName}";
                            dto.ImageUrl = gpu.ImageUrl;
                            dto.Specs = $"{gpu.ChipsetBrand}, {gpu.VRAMMemorySize}GB VRAM";
                        }
                        break;

                    case "storage":
                        var ssd = _context.Storages.Find(fav.ComponentId);
                        if (ssd != null)
                        {
                            dto.Brand = ssd.Brand;
                            dto.Name = $"{ssd.Brand} {ssd.ModelName}";
                            dto.ImageUrl = ssd.ImageUrl;
                            // Kapasiteyi formatla
                            string cap = ssd.Capacity >= 1024 ? $"{ssd.Capacity/1024} TB" : $"{ssd.Capacity} GB";
                            dto.Specs = $"{cap}, {ssd.StorageType}, {ssd.ReadSpeed}/{ssd.WriteSpeed} MB/s";
                        }
                        break;

                    case "case":
                        var kase = _context.Cases.Find(fav.ComponentId);
                        if (kase != null)
                        {
                            dto.Brand = kase.Brand;
                            dto.Name = $"{kase.Brand} {kase.ModelName}";
                            dto.ImageUrl = kase.ImageUrl;
                            dto.Specs = $"{kase.CaseType}, {kase.SupportedMotherboards}";
                        }
                        break;

                    case "psu":
                        var psu = _context.Psus.Find(fav.ComponentId);
                        if (psu != null)
                        {
                            dto.Brand = psu.Brand;
                            dto.Name = $"{psu.Brand} {psu.ModelName}";
                            dto.ImageUrl = psu.ImageUrl;
                            dto.Specs = $"{psu.Wattage}W, {psu.Rating}, {psu.FormFactor}";
                        }
                        break;

                    case "cooler": // Frontend'de "cooler" veya "cpucooler" kullanıyorsan ikisini de kontrol et
                    case "cpucooler":
                        var cooler = _context.CpuCoolers.Find(fav.ComponentId);
                        if (cooler != null)
                        {
                            dto.Brand = cooler.Brand;
                            dto.Name = $"{cooler.Brand} {cooler.ModelName}";
                            dto.ImageUrl = cooler.ImageUrl;
                            dto.Specs = $"{cooler.CoolerType}, {cooler.FanSize}mm Fan";
                        }
                        break;
                }

                // Eğer parça silinmişse listeye ekleme
                if (!string.IsNullOrEmpty(dto.Name))
                {
                    dtoList.Add(dto);
                }
            }

            return Ok(dtoList);
        }
    }
}