using Microsoft.AspNetCore.Mvc;
using PCPartsAPI.Data;
using PCPartsAPI.Models;
using PCPartsAPI.DTOs; // DTO kullanımı için
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

        // 1. Sistemi Kaydet (AYNEN KALIYOR)
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

        // 2. Kullanıcının Kayıtlı Sistemlerini Getir (8 PARÇA DESTEKLİ)
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

                // 1. İŞLEMCİ
                if (build.CpuId != null)
                {
                    var p = _context.Processors.Find(build.CpuId);
                    dto.CpuName = p != null ? $"{p.Brand} {p.ModelName}" : "İşlemci Bulunamadı";
                    dto.CpuImage = p?.ImageUrl;
                }

                // 2. EKRAN KARTI
                if (build.GpuId != null)
                {
                    var p = _context.Gpus.Find(build.GpuId);
                    dto.GpuName = p != null ? $"{p.Brand} {p.ModelName}" : "GPU Bulunamadı";
                    dto.GpuImage = p?.ImageUrl;
                }

                // 3. RAM
                if (build.RamId != null)
                {
                    var p = _context.Rams.Find(build.RamId);
                    dto.RamName = p != null ? $"{p.Brand} {p.ModelName} ({p.TotalCapacity}GB)" : "RAM Bulunamadı";
                }

                // 4. ANAKART (YENİ)
                if (build.MotherboardId != null)
                {
                    var p = _context.Motherboards.Find(build.MotherboardId);
                    dto.MotherboardName = p != null ? $"{p.Brand} {p.ModelName}" : "Anakart Bulunamadı";
                }

                // 5. DEPOLAMA (YENİ)
                if (build.StorageId != null)
                {
                    var p = _context.Storages.Find(build.StorageId);
                    dto.StorageName = p != null ? $"{p.Brand} {p.ModelName}" : "Disk Bulunamadı";
                }

                // 6. KASA
                if (build.CaseId != null)
                {
                    var p = _context.Cases.Find(build.CaseId);
                    dto.CaseName = p != null ? $"{p.Brand} {p.ModelName}" : "Kasa Bulunamadı";
                    dto.CaseImage = p?.ImageUrl;
                }

                // 7. PSU (YENİ)
                if (build.PsuId != null)
                {
                    var p = _context.Psus.Find(build.PsuId);
                    dto.PsuName = p != null ? $"{p.Brand} {p.ModelName} ({p.Wattage}W)" : "PSU Bulunamadı";
                }

                // 8. SOĞUTUCU (YENİ)
                if (build.CpuCoolerId != null)
                {
                    var p = _context.CpuCoolers.Find(build.CpuCoolerId);
                    dto.CoolerName = p != null ? $"{p.Brand} {p.ModelName}" : "Soğutucu Bulunamadı";
                }

                buildDtos.Add(dto);
            }

            return Ok(buildDtos);
        }

        // 3. Sistemi Sil (AYNEN KALIYOR)
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