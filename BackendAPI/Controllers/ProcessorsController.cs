using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Entity Framework'ü kullanabilmek için bu satır gerekli
using PCPartsAPI.Data; // Veritabanı köprümüz (ApplicationDbContext) burada
using PCPartsAPI.Models; // Processor modelimiz (kimlik kartı) burada

namespace PCPartsAPI.Controllers
{
    [Route("api/[controller]")] // Bu Controller'a nasıl erişileceğinin adresi
    [ApiController]
    public class ProcessorsController : ControllerBase
    {
        // Veritabanıyla konuşmamızı sağlayacak olan "köprüyü" (DbContext) buraya çağırıyoruz.
        private readonly ApplicationDbContext _context;

        // Bu özel bir metottur (Constructor).
        // Proje çalıştığında, ASP.NET Core otomatik olarak veritabanı köprümüzü (ApplicationDbContext) oluşturur ve bize hazır olarak verir.
        public ProcessorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/processors
        // Bu adrese bir GET isteği geldiğinde aşağıdaki kod çalışacak.
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Processor>>> GetProcessors()
        {
            // Köprüyü kullanarak veritabanına git, Processors tablosundaki tüm verileri bul
            // ve bir liste halinde geri döndür.
            var processors = await _context.Processors.ToListAsync();

            return Ok(processors); // Bulunan listeyi "Başarılı" (200 OK) koduyla birlikte cevap olarak gönder.
        }

        // --- AŞAĞIDAKİ KISIM EKLENDİ (Tek bir işlemciyi bulmak için) ---
        // GET: api/processors/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Processor>> GetProcessor(int id)
        {
            var processor = await _context.Processors.FindAsync(id);

            if (processor == null)
            {
                return NotFound();
            }

            return processor;
        }
        // --- EKLENEN KISIM BİTTİ ---
    }
}