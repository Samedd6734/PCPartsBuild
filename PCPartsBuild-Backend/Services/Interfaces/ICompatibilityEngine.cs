namespace PCPartsAPI.Services.Interfaces
{
    public class CompatibilityResult
    {
        public bool IsCompatible { get; set; } = true;
        public string Reason { get; set; } = string.Empty;

        // Uyumluluk hatası detayları (LLM prompt için)
        public string? ExistingComponentName { get; set; }
        public string? AttemptedComponentName { get; set; }
    }

    public interface ICompatibilityEngine
    {
        /// <summary>
        /// Seçilen parçanın mevcut build ile uyumluluğunu kontrol eder.
        /// Session'daki SelectedComponentsJson verilerini baz alarak kontrol yapar.
        /// </summary>
        Task<CompatibilityResult> CheckCompatibilityAsync(
            string selectedComponentsJson,
            string currentStepName,
            object component);
    }
}
