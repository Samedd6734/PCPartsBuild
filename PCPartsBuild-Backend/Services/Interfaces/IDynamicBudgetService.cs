using PCPartsAPI.Enums;
using PCPartsAPI.Models;

namespace PCPartsAPI.Services.Interfaces
{
    public interface IDynamicBudgetService
    {
        /// <summary>
        /// Verilen adım için ayrılması gereken bütçeyi hesaplar.
        /// Purpose'a göre ağırlık tablosunu kullanır.
        /// </summary>
        decimal CalculateAllocation(AssistantSession session, ComponentStep step);

        /// <summary>
        /// Parça seçildikten sonra kalan bütçeyi yeniden hesaplar.
        /// Surplus/deficit'i kalan adımlara oransal olarak dağıtır.
        /// </summary>
        decimal RecalculateBudget(AssistantSession session, decimal spentAmount, ComponentStep nextStep);
    }
}
