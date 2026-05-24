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
    public class BuildsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BuildsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("save")]
        public IActionResult SaveBuild([FromBody] SavedBuilds build)
        {
            if (string.IsNullOrEmpty(build.UserId))
                return BadRequest("Kullanıcı girişi yapılmamış.");

            build.CreatedAt = System.DateTime.UtcNow;
            _context.SavedBuilds.Add(build);
            _context.SaveChanges();

            return Ok(new { Message = "Sistem başarıyla kaydedildi!", BuildId = build.Id });
        }

        [HttpGet("{userId}")]
        public IActionResult GetUserBuilds(string userId)
        {
            var builds = _context.SavedBuilds
                                 .Where(b => b.UserId == userId)
                                 .OrderByDescending(b => b.CreatedAt)
                                 .ToList();

            var buildDtos = new List<SavedBuildDto>();

            foreach (var build in builds)
            {
                var dto = new SavedBuildDto
                {
                    Id = build.Id,
                    BuildName = build.BuildName,
                    CreatedAt = build.CreatedAt,
                    TotalPrice = build.TotalPrice
                };

                if (build.CpuId != null)
                {
                    var p = _context.Processors.Find(build.CpuId);
                    dto.CpuName = p != null ? $"{p.Brand} {p.ProductName}" : "İşlemci Bulunamadı";
                    dto.CpuImage = p?.ImageUrl;
                }

                if (build.GpuId != null)
                {
                    var p = _context.Gpus.Find(build.GpuId);
                    dto.GpuName = p != null ? $"{p.Brand} {p.ProductName}" : "GPU Bulunamadı";
                    dto.GpuImage = p?.ImageUrl;
                }

                if (build.RamId != null)
                {
                    var p = _context.Rams.Find(build.RamId);
                    dto.RamName = p != null ? $"{p.Brand} {p.ProductName} ({p.CapacityGB}GB)" : "RAM Bulunamadı";
                }

                if (build.MotherboardId != null)
                {
                    var p = _context.Motherboards.Find(build.MotherboardId);
                    dto.MotherboardName = p != null ? $"{p.Brand} {p.ProductName}" : "Anakart Bulunamadı";
                }

                if (build.StorageId != null)
                {
                    var p = _context.Storages.Find(build.StorageId);
                    dto.StorageName = p != null ? $"{p.Brand} {p.ProductName}" : "Disk Bulunamadı";
                }

                if (build.CaseId != null)
                {
                    var p = _context.Cases.Find(build.CaseId);
                    dto.CaseName = p != null ? $"{p.Brand} {p.ProductName}" : "Kasa Bulunamadı";
                    dto.CaseImage = p?.ImageUrl;
                }

                if (build.PsuId != null)
                {
                    var p = _context.Psus.Find(build.PsuId);
                    dto.PsuName = p != null ? $"{p.Brand} {p.ProductName} ({p.WattageW}W)" : "PSU Bulunamadı";
                }

                if (build.CpuCoolerId != null)
                {
                    var p = _context.CpuCoolers.Find(build.CpuCoolerId);
                    dto.CoolerName = p != null ? $"{p.Brand} {p.ProductName}" : "Soğutucu Bulunamadı";
                }

                buildDtos.Add(dto);
            }

            return Ok(buildDtos);
        }

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