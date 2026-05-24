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

        [HttpPost("start")]
        public async Task<IActionResult> Start([FromBody] AssistantStartRequestDto request)
        {
            if (request.Budget <= 0)
                return BadRequest(new { message = "Bütçe sıfırdan büyük olmalıdır." });

            if (string.IsNullOrWhiteSpace(request.Purpose))
                return BadRequest(new { message = "Kullanım amacı belirtilmelidir." });

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

        [HttpPost("process-selection")]
        public async Task<IActionResult> ProcessSelection([FromBody] AssistantSelectionRequestDto request)
        {
            var session = await _sessionManager.GetSessionAsync(request.SessionId);
            if (session == null)
                return BadRequest(new { message = "Oturum bulunamadı." });

            if (session.CurrentStep == ComponentStep.Completed)
                return BadRequest(new { message = "Tüm adımlar zaten tamamlandı." });

            var stepName = session.CurrentStep.ToString();
            var (component, componentSpecs) = await FindComponentAsync(session.CurrentStep, request.ComponentId);

            if (component == null)
                return BadRequest(new { message = $"{stepName} ID:{request.ComponentId} bulunamadı." });

            var compatibility = await _compatibilityEngine.CheckCompatibilityAsync(
                session.SelectedComponentsJson,
                stepName,
                component);

            if (!compatibility.IsCompatible)
            {
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

            var selectedJson = JsonNode.Parse(session.SelectedComponentsJson) as JsonObject ?? new JsonObject();
            selectedJson[stepName] = JsonNode.Parse(componentSpecs);
            session.SelectedComponentsJson = selectedJson.ToJsonString();

            var price = GetComponentPrice(component);
            var nextStep = (ComponentStep)((int)session.CurrentStep + 1);
            var nextAllocated = _budgetService.RecalculateBudget(session, price, nextStep);

            await _sessionManager.AdvanceStepAsync(session);

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
                        name = p.ProductName,
                        socket = p.SocketType,
                        tdp = p.TDP,
                        memoryTypes = p.SupportedMemoryType,
                        integratedGraphics = p.HasIntegratedGraphics,
                        price = p.Price
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
                        name = m.ProductName,
                        socket = m.SocketType,
                        formFactor = m.FormFactor,
                        memoryType = m.MemoryType,
                        memorySlots = m.MemorySlotCount,
                        m2SlotCount = m.M2SlotCount,
                        price = m.Price
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
                        name = r.ProductName,
                        memoryType = r.MemoryType,
                        speed = r.SpeedMHz,
                        totalCapacity = r.CapacityGB,
                        moduleConfig = r.ModuleConfig,
                        price = r.Price
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
                        name = g.ProductName,
                        tdp = g.TDPWatt,
                        length = g.LengthMm,
                        powerConnectors = g.PowerConnectors,
                        vram = g.VRAMGB,
                        recommendedPsu = g.RecommendedPSUW,
                        price = g.Price
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
                        name = s.ProductName,
                        formFactor = s.FormFactor,
                        interfaceType = s.Interface,
                        capacity = s.CapacityGB,
                        price = s.Price
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
                        name = c.ProductName,
                        supportedMotherboards = c.SupportedMotherboardFormFactors,
                        maxGpuLength = c.MaxGPULengthMm,
                        maxCpuCoolerHeight = c.MaxCPUCoolerHeightMm,
                        topRadiatorSupport = c.TopRadiatorSupportMm,
                        frontRadiatorSupport = c.FrontRadiatorSupportMm,
                        price = c.Price
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
                        name = ps.ProductName,
                        wattage = ps.WattageW,
                        certification = ps.Certification,
                        isModular = ps.IsModular,
                        formFactor = ps.FormFactor,
                        price = ps.Price
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
                        name = co.ProductName,
                        coolerType = co.CoolerType,
                        tdpCapacity = co.TDPCapacityW,
                        supportedSockets = co.SupportedSockets,
                        height = co.HeightMm,
                        radiatorSize = co.RadiatorSizeMm,
                        price = co.Price
                    });
                    return (co, specs);
                }
                default:
                    return (null, "{}");
            }
        }

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
