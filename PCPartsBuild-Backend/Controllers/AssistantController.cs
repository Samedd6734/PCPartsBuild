using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PCPartsAPI.Data;
using PCPartsAPI.Dtos;
using PCPartsAPI.Enums;
using PCPartsAPI.Services.Interfaces;

namespace PCPartsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssistantController : ControllerBase
    {
        private readonly IDynamicBudgetService _budgetService;
        private readonly ICompatibilityEngine _compatibilityEngine;
        private readonly IAiPromptService _aiPromptService;
        private readonly ISessionManager _sessionManager;
        private readonly ApplicationDbContext _context;

        public AssistantController(
            IDynamicBudgetService budgetService,
            ICompatibilityEngine compatibilityEngine,
            IAiPromptService aiPromptService,
            ISessionManager sessionManager,
            ApplicationDbContext context)
        {
            _budgetService = budgetService;
            _compatibilityEngine = compatibilityEngine;
            _aiPromptService = aiPromptService;
            _sessionManager = sessionManager;
            _context = context;
        }

        /// <summary>
        /// Asistan oturumunu başlatır. İlk adım (CPU) bütçesini hesaplar ve karşılama mesajı üretir.
        /// </summary>
        [HttpPost("start")]
        public async Task<IActionResult> Start([FromBody] AssistantStartRequestDto request)
        {
            if (request.Budget <= 0)
                return BadRequest(new { message = "Bütçe sıfırdan büyük olmalıdır." });

            if (string.IsNullOrWhiteSpace(request.Purpose))
                return BadRequest(new { message = "Kullanım amacı belirtilmelidir." });

            // UserId — JWT varsa al, yoksa null (anonim)
            string? userId = User.Identity?.IsAuthenticated == true
                ? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                : null;

            var session = await _sessionManager.CreateSessionAsync(request.Budget, request.Purpose, userId);
            var allocatedBudget = _budgetService.CalculateAllocation(session, ComponentStep.CPU);

            var aiMessage = await _aiPromptService.GenerateWelcomeMessageAsync(
                request.Budget,
                request.Purpose,
                allocatedBudget);

            return Ok(new AssistantResponseDto
            {
                SessionId = session.Id,
                Status = "success",
                CanProceed = true,
                AiMessage = aiMessage,
                CurrentStep = "CPU",
                NextStep = null,
                AllocatedBudget = allocatedBudget,
                RemainingBudget = session.RemainingBudget
            });
        }

        /// <summary>
        /// Seçilen parçayı uyumluluk motoruyla test eder. Uyumluysa build'e ekler ve sonraki adıma geçer.
        /// </summary>
        [HttpPost("process-selection")]
        public async Task<IActionResult> ProcessSelection([FromBody] AssistantSelectionRequestDto request)
        {
            // 1. Session'ı getir
            var session = await _sessionManager.GetSessionAsync(request.SessionId);
            if (session == null)
                return BadRequest(new { message = "Oturum bulunamadı." });

            if (session.CurrentStep == ComponentStep.Completed)
                return BadRequest(new { message = "Tüm adımlar zaten tamamlandı." });

            // 2. CurrentStep'e göre doğru tablodan parçayı bul
            var stepName = session.CurrentStep.ToString();
            var (component, componentSpecs) = await FindComponentAsync(session.CurrentStep, request.ComponentId);

            if (component == null)
                return BadRequest(new { message = $"{stepName} ID:{request.ComponentId} bulunamadı." });

            // 3. Uyumluluk kontrolü
            var compatibility = await _compatibilityEngine.CheckCompatibilityAsync(
                session.SelectedComponentsJson,
                stepName,
                component);

            if (!compatibility.IsCompatible)
            {
                // Uyumsuz — LLM hata mesajı üret, kaydetme
                var errorMessage = await _aiPromptService.GenerateErrorMessageAsync(
                    compatibility.AttemptedComponentName ?? stepName,
                    compatibility.ExistingComponentName ?? "Mevcut Parça",
                    compatibility.Reason);

                return Ok(new AssistantResponseDto
                {
                    SessionId = session.Id,
                    Status = "error",
                    CanProceed = false,
                    AiMessage = errorMessage,
                    CurrentStep = stepName,
                    NextStep = null,
                    AllocatedBudget = _budgetService.CalculateAllocation(session, session.CurrentStep),
                    RemainingBudget = session.RemainingBudget
                });
            }

            // 4. Uyumlu — SelectedComponents'a ekle
            var selectedJson = JsonNode.Parse(session.SelectedComponentsJson) as JsonObject ?? new JsonObject();
            selectedJson[stepName] = JsonNode.Parse(componentSpecs);
            session.SelectedComponentsJson = selectedJson.ToJsonString();

            // 5. Bütçeyi güncelle (Price alanı yoksa 0 varsay)
            var price = GetComponentPrice(component);
            var nextStep = (ComponentStep)((int)session.CurrentStep + 1);
            var nextAllocated = _budgetService.RecalculateBudget(session, price, nextStep);

            // 6. Adımı ilerlet
            await _sessionManager.AdvanceStepAsync(session);

            // 7. Tamamlandıysa veya sonraki adım için AI mesajı üret
            string aiMessage;
            string? nextStepName = null;

            if (session.CurrentStep == ComponentStep.Completed)
            {
                aiMessage = "Tebrikler! Tüm parçalar başarıyla seçildi. Sisteminiz eksiksiz ve uyumlu bir şekilde tamamlandı. Kaydetmeyi unutma!";
            }
            else
            {
                nextStepName = session.CurrentStep.ToString();
                aiMessage = await _aiPromptService.GenerateNextStepMessageAsync(
                    nextStepName,
                    nextAllocated,
                    componentSpecs);
            }

            return Ok(new AssistantResponseDto
            {
                SessionId = session.Id,
                Status = "success",
                CanProceed = true,
                AiMessage = aiMessage,
                CurrentStep = stepName,
                NextStep = nextStepName,
                AllocatedBudget = nextAllocated,
                RemainingBudget = session.RemainingBudget
            });
        }

        /// <summary>
        /// CurrentStep'e göre doğru DbSet'ten parçayı bulur ve JSON spec özetini döndürür.
        /// </summary>
        private async Task<(object? component, string specs)> FindComponentAsync(ComponentStep step, int componentId)
        {
            switch (step)
            {
                case ComponentStep.CPU:
                {
                    var p = await _context.Processors.FindAsync(componentId);
                    if (p == null) return (null, "{}");
                    var specs = JsonSerializer.Serialize(new
                    {
                        id = p.Id,
                        name = $"{p.Brand} {p.ModelName}",
                        socket = p.Socket,
                        tdp = p.Tdp,
                        memoryTypes = p.SupportedMemoryTypes,
                        integratedGraphics = p.IntegratedGraphics
                    });
                    return (p, specs);
                }
                case ComponentStep.Motherboard:
                {
                    var m = await _context.Motherboards.FindAsync(componentId);
                    if (m == null) return (null, "{}");
                    var specs = JsonSerializer.Serialize(new
                    {
                        id = m.Id,
                        name = $"{m.Brand} {m.ModelName}",
                        socket = m.Socket,
                        chipset = m.Chipset,
                        formFactor = m.FormFactor,
                        memoryType = m.MemoryType,
                        memorySlots = m.MemorySlots,
                        m2SlotCount = m.M2SlotCount,
                        maxCpuCoolerHeight = 0  // Anakart bu bilgiyi tutmuyor, Kasa tutuyor
                    });
                    return (m, specs);
                }
                case ComponentStep.RAM:
                {
                    var r = await _context.Rams.FindAsync(componentId);
                    if (r == null) return (null, "{}");
                    var specs = JsonSerializer.Serialize(new
                    {
                        id = r.Id,
                        name = $"{r.Brand} {r.ModelName}",
                        memoryType = r.MemoryType,
                        speed = r.Speed,
                        totalCapacity = r.TotalCapacity,
                        moduleCount = r.ModuleCount
                    });
                    return (r, specs);
                }
                case ComponentStep.GPU:
                {
                    var g = await _context.Gpus.FindAsync(componentId);
                    if (g == null) return (null, "{}");
                    var specs = JsonSerializer.Serialize(new
                    {
                        id = g.Id,
                        name = $"{g.Brand} {g.ModelName}",
                        tdp = g.Tdp,
                        length = g.Length,
                        powerConnectors = g.PowerConnectors,
                        vram = g.VRAMMemorySize,
                        recommendedPsu = g.RecommendedPsu
                    });
                    return (g, specs);
                }
                case ComponentStep.Storage:
                {
                    var s = await _context.Storages.FindAsync(componentId);
                    if (s == null) return (null, "{}");
                    var specs = JsonSerializer.Serialize(new
                    {
                        id = s.Id,
                        name = $"{s.Brand} {s.ModelName}",
                        formFactor = s.FormFactor,
                        storageType = s.StorageType,
                        capacity = s.Capacity,
                        isNvme = s.IsNvme
                    });
                    return (s, specs);
                }
                case ComponentStep.Case:
                {
                    var c = await _context.Cases.FindAsync(componentId);
                    if (c == null) return (null, "{}");
                    var specs = JsonSerializer.Serialize(new
                    {
                        id = c.Id,
                        name = $"{c.Brand} {c.ModelName}",
                        supportedMotherboards = c.SupportedMotherboards,
                        maxGpuLength = c.MaxGpuLength,
                        maxCpuCoolerHeight = c.MaxCpuCoolerHeight,
                        maxPsuLength = c.MaxPsuLength,
                        radiatorSupportFront = c.RadiatorSupportFront,
                        radiatorSupportTop = c.RadiatorSupportTop
                    });
                    return (c, specs);
                }
                case ComponentStep.PSU:
                {
                    var ps = await _context.Psus.FindAsync(componentId);
                    if (ps == null) return (null, "{}");
                    var specs = JsonSerializer.Serialize(new
                    {
                        id = ps.Id,
                        name = $"{ps.Brand} {ps.ModelName}",
                        wattage = ps.Wattage,
                        rating = ps.Rating,
                        has12VHPWR = ps.Has12VHPWR,
                        length = ps.Length
                    });
                    return (ps, specs);
                }
                case ComponentStep.Cooler:
                {
                    var co = await _context.CpuCoolers.FindAsync(componentId);
                    if (co == null) return (null, "{}");
                    var specs = JsonSerializer.Serialize(new
                    {
                        id = co.Id,
                        name = $"{co.Brand} {co.ModelName}",
                        coolerType = co.CoolerType,
                        tdpRating = co.TdpRating,
                        supportedSockets = co.SupportedSockets,
                        height = co.Height,
                        radiatorSize = co.RadiatorSize
                    });
                    return (co, specs);
                }
                default:
                    return (null, "{}");
            }
        }

        /// <summary>
        /// Parçanın fiyatını alır. Model'larda Price alanı yoksa 0 döner.
        /// </summary>
        private static decimal GetComponentPrice(object component)
        {
            var priceProp = component.GetType().GetProperty("Price");
            if (priceProp != null)
            {
                var val = priceProp.GetValue(component);
                if (val is decimal d) return d;
                if (val is double dbl) return (decimal)dbl;
                if (val is int i) return i;
            }
            return 0;
        }
    }
}
