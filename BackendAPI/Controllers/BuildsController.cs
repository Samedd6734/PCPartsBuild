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

        // 2. Kullanıcının Kayıtlı Sistemlerini Getir (GÜNCELLENMİŞ)
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

                // İŞLEMCİ
                if (build.CpuId != null)
                {
                    var cpu = _context.Processors.Find(build.CpuId);
                    dto.CpuName = cpu != null ? $"{cpu.Brand} {cpu.ModelName}" : "İşlemci Bulunamadı";
                    dto.CpuImage = cpu?.ImageUrl;
                }
                else dto.CpuName = "İşlemci Seçilmedi"; // Boşsa bunu yaz

                // EKRAN KARTI
                if (build.GpuId != null)
                {
                    var gpu = _context.Gpus.Find(build.GpuId);
                    dto.GpuName = gpu != null ? $"{gpu.Brand} {gpu.ModelName}" : "Ekran Kartı Bulunamadı";
                    dto.GpuImage = gpu?.ImageUrl;
                }
                else dto.GpuName = "Ekran Kartı Seçilmedi";

                // RAM
                if (build.RamId != null)
                {
                    var ram = _context.Rams.Find(build.RamId);
                    dto.RamName = ram != null ? $"{ram.Brand} {ram.ModelName} ({ram.TotalCapacity}GB)" : "RAM Bulunamadı";
                }
                else dto.RamName = "RAM Seçilmedi";

                // KASA
                if (build.CaseId != null)
                {
                    var kase = _context.Cases.Find(build.CaseId);
                    dto.CaseName = kase != null ? $"{kase.Brand} {kase.ModelName}" : "Kasa Bulunamadı";
                    dto.CaseImage = kase?.ImageUrl;
                }
                else dto.CaseName = "Kasa Seçilmedi";

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