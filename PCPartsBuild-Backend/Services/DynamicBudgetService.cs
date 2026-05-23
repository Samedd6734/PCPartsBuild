using PCPartsAPI.Enums;
using PCPartsAPI.Models;
using PCPartsAPI.Services.Interfaces;

namespace PCPartsAPI.Services
{
    public class DynamicBudgetService : IDynamicBudgetService
    {
        // Purpose'a göre ağırlık tabloları
        private static readonly Dictionary<string, Dictionary<ComponentStep, decimal>> WeightProfiles = new()
        {
            ["Gaming"] = new()
            {
                [ComponentStep.CPU] = 0.20m,
                [ComponentStep.Motherboard] = 0.10m,
                [ComponentStep.RAM] = 0.10m,
                [ComponentStep.GPU] = 0.40m,
                [ComponentStep.Storage] = 0.07m,
                [ComponentStep.Case] = 0.05m,
                [ComponentStep.PSU] = 0.05m,
                [ComponentStep.Cooler] = 0.03m
            },
            ["Render"] = new()
            {
                [ComponentStep.CPU] = 0.30m,
                [ComponentStep.Motherboard] = 0.10m,
                [ComponentStep.RAM] = 0.15m,
                [ComponentStep.GPU] = 0.35m,
                [ComponentStep.Storage] = 0.05m,
                [ComponentStep.Case] = 0.02m,
                [ComponentStep.PSU] = 0.02m,
                [ComponentStep.Cooler] = 0.01m
            },
            ["Office"] = new()
            {
                [ComponentStep.CPU] = 0.20m,
                [ComponentStep.Motherboard] = 0.15m,
                [ComponentStep.RAM] = 0.10m,
                [ComponentStep.GPU] = 0.15m,
                [ComponentStep.Storage] = 0.15m,
                [ComponentStep.Case] = 0.10m,
                [ComponentStep.PSU] = 0.10m,
                [ComponentStep.Cooler] = 0.05m
            }
        };

        public decimal CalculateAllocation(AssistantSession session, ComponentStep step)
        {
            var weights = GetWeightsForPurpose(session.Purpose);
            if (!weights.ContainsKey(step))
                return 0;

            return Math.Round(session.RemainingBudget * weights[step] / GetRemainingWeightSum(weights, step), 2);
        }

        public decimal RecalculateBudget(AssistantSession session, decimal spentAmount, ComponentStep nextStep)
        {
            session.RemainingBudget -= spentAmount;
            session.UpdatedAt = DateTime.UtcNow;

            if (nextStep == ComponentStep.Completed || session.RemainingBudget <= 0)
                return 0;

            return CalculateAllocation(session, nextStep);
        }

        private Dictionary<ComponentStep, decimal> GetWeightsForPurpose(string purpose)
        {
            if (WeightProfiles.TryGetValue(purpose, out var profile))
                return profile;

            // Bilinmeyen purpose → Gaming profili varsayılan
            return WeightProfiles["Gaming"];
        }

        /// <summary>
        /// Henüz seçilmemiş adımların toplam ağırlığını hesaplar.
        /// Bu sayede kalan bütçe oransal dağıtılır.
        /// </summary>
        private decimal GetRemainingWeightSum(Dictionary<ComponentStep, decimal> weights, ComponentStep currentStep)
        {
            decimal sum = 0;
            foreach (var kvp in weights)
            {
                if ((int)kvp.Key >= (int)currentStep)
                    sum += kvp.Value;
            }
            return sum > 0 ? sum : 1; // sıfıra bölme koruması
        }
    }
}
