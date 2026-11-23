using System;
using System.ComponentModel.DataAnnotations;
using PCPartsAPI.Enums;

namespace PCPartsAPI.Models
{
    public class AssistantSession
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public string? UserId { get; set; }

        public decimal TotalBudget { get; set; }

        public decimal RemainingBudget { get; set; }

        // "Gaming", "Render", "Office"
        public string Purpose { get; set; } = string.Empty;

        public ComponentStep CurrentStep { get; set; } = ComponentStep.CPU;

        // PostgreSQL JSONB — seçilen parçaların ID + kritik spec'leri
        // Örn: { "CPU": { "id": 5, "socket": "AM5", "tdp": 105 }, "Motherboard": { ... } }
        public string SelectedComponentsJson { get; set; } = "{}";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
