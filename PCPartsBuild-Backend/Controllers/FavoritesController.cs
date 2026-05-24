using Microsoft.AspNetCore.Mvc;
using PCPartsAPI.Data;
using PCPartsAPI.Models;
using PCPartsAPI.DTOs;
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

                switch (fav.ComponentType.ToLower())
                {
                    case "cpu":
                        var cpu = _context.Processors.Find(fav.ComponentId);
                        if (cpu != null)
                        {
                            dto.Brand = cpu.Brand;
                            dto.Name = cpu.ProductName;
                            dto.ImageUrl = cpu.ImageUrl;
                            dto.Specs = $"{cpu.CoreCount} Çekirdek, {cpu.SocketType}";
                            dto.Price = cpu.Price ?? 0;
                        }
                        break;

                    case "motherboard":
                        var mobo = _context.Motherboards.Find(fav.ComponentId);
                        if (mobo != null)
                        {
                            dto.Brand = mobo.Brand;
                            dto.Name = mobo.ProductName;
                            dto.ImageUrl = mobo.ImageUrl;
                            dto.Specs = $"{mobo.FormFactor}, {mobo.SocketType}";
                            dto.Price = mobo.Price ?? 0;
                        }
                        break;

                    case "ram":
                        var ram = _context.Rams.Find(fav.ComponentId);
                        if (ram != null)
                        {
                            dto.Brand = ram.Brand;
                            dto.Name = ram.ProductName;
                            dto.ImageUrl = ram.ImageUrl;
                            dto.Specs = $"{ram.MemoryType}, {ram.SpeedMHz}MHz, {ram.CapacityGB}GB";
                            dto.Price = ram.Price ?? 0;
                        }
                        break;

                    case "gpu":
                        var gpu = _context.Gpus.Find(fav.ComponentId);
                        if (gpu != null)
                        {
                            dto.Brand = gpu.Brand;
                            dto.Name = gpu.ProductName;
                            dto.ImageUrl = gpu.ImageUrl;
                            dto.Specs = $"{gpu.ChipManufacturer}, {gpu.VRAMGB}GB VRAM";
                            dto.Price = gpu.Price ?? 0;
                        }
                        break;

                    case "storage":
                        var ssd = _context.Storages.Find(fav.ComponentId);
                        if (ssd != null)
                        {
                            dto.Brand = ssd.Brand;
                            dto.Name = ssd.ProductName;
                            dto.ImageUrl = ssd.ImageUrl;
                            string cap = ssd.CapacityGB >= 1000 ? $"{ssd.CapacityGB/1000} TB" : $"{ssd.CapacityGB} GB";
                            dto.Specs = $"{cap}, {ssd.Interface}, {ssd.ReadSpeedMBs}/{ssd.WriteSpeedMBs} MB/s";
                            dto.Price = ssd.Price ?? 0;
                        }
                        break;

                    case "case":
                        var kase = _context.Cases.Find(fav.ComponentId);
                        if (kase != null)
                        {
                            dto.Brand = kase.Brand;
                            dto.Name = kase.ProductName;
                            dto.ImageUrl = kase.ImageUrl;
                            dto.Specs = $"{kase.SupportedMotherboardFormFactors}";
                            dto.Price = kase.Price ?? 0;
                        }
                        break;

                    case "psu":
                        var psu = _context.Psus.Find(fav.ComponentId);
                        if (psu != null)
                        {
                            dto.Brand = psu.Brand;
                            dto.Name = psu.ProductName;
                            dto.ImageUrl = psu.ImageUrl;
                            dto.Specs = $"{psu.WattageW}W, {psu.Certification}, {psu.FormFactor}";
                            dto.Price = psu.Price ?? 0;
                        }
                        break;

                    case "cooler":
                    case "cpucooler":
                        var cooler = _context.CpuCoolers.Find(fav.ComponentId);
                        if (cooler != null)
                        {
                            dto.Brand = cooler.Brand;
                            dto.Name = cooler.ProductName;
                            dto.ImageUrl = cooler.ImageUrl;
                            dto.Specs = $"{cooler.CoolerType}, {cooler.FanSizeMm}mm Fan";
                            dto.Price = cooler.Price ?? 0;
                        }
                        break;
                }

                if (!string.IsNullOrEmpty(dto.Name))
                {
                    dtoList.Add(dto);
                }
            }

            return Ok(dtoList);
        }
    }
}