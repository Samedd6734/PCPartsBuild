namespace PCPartsAPI.Services.Interfaces
{
    public interface IAiPromptService
    {
        /// <summary>
        /// Başarılı seçim sonrası yönlendirme mesajı üretir.
        /// LLM'e System + User prompt gönderir.
        /// </summary>
        Task<string> GenerateNextStepMessageAsync(
            string nextStepName,
            decimal allocatedBudget,
            string previousComponentSpecs);

        /// <summary>
        /// Uyumsuz parça seçimi sonrası hata mesajı üretir.
        /// </summary>
        Task<string> GenerateErrorMessageAsync(
            string attemptedComponentName,
            string existingComponentName,
            string errorReason);

        /// <summary>
        /// İlk adım (CPU) için giriş mesajı üretir.
        /// </summary>
        Task<string> GenerateWelcomeMessageAsync(
            decimal totalBudget,
            string purpose,
            decimal cpuBudget);
    }
}
