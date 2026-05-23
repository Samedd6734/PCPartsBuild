using System;

namespace PCPartsAPI.Dtos
{
    public class AssistantResponseDto
    {
        public Guid SessionId { get; set; }

        // "success" | "error"
        public string Status { get; set; } = "success";

        public bool CanProceed { get; set; } = true;

        // LLM'den gelen Markdown mesaj
        public string AiMessage { get; set; } = string.Empty;

        // Mevcut adım (Örn: "CPU", "Motherboard")
        public string CurrentStep { get; set; } = "CPU";

        // Sonraki adım (null ise tamamlandı)
        public string? NextStep { get; set; }

        // Bu adım için ayrılan bütçe
        public decimal AllocatedBudget { get; set; }

        // Toplam kalan bütçe
        public decimal RemainingBudget { get; set; }
    }
}
