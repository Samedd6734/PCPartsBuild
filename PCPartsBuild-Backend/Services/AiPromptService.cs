using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PCPartsAPI.Services.Interfaces;

namespace PCPartsAPI.Services
{
    public class AiPromptService : IAiPromptService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AiPromptService> _logger;

        public AiPromptService(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ILogger<AiPromptService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<string> GenerateWelcomeMessageAsync(decimal totalBudget, string purpose, decimal cpuBudget)
        {
            var systemPrompt = @"Sen PCPartsBuild platformunun eğitici asistanısın. Kullanıcı yeni bir bilgisayar toplamak istiyor ve sen ona adım adım yardım edeceksin. İlk adım olarak İŞLEMCİ (CPU) seçmesi gerekiyor.
KURALLAR:
1. Doğrudan marka veya model önerme. Seçimi kullanıcıya bırak.
2. Sol menüdeki filtreleri nasıl kullanacağını açıkla (soket tipi, çekirdek sayısı, bütçe aralığı gibi).
3. Kullanım amacına göre nelere dikkat etmesi gerektiğini kısaca belirt.
4. Maksimum 4 cümle kur, samimi ol. Asla maddeleme (bullet point) kullanma.";

            var userPrompt = $@"- Toplam Bütçe: {totalBudget:N0} TL
- Kullanım Amacı: {purpose}
- İşlemci İçin Ayrılan Bütçe: {cpuBudget:N0} TL";

            return await CallLlmAsync(systemPrompt, userPrompt);
        }

        public async Task<string> GenerateNextStepMessageAsync(string nextStepName, decimal allocatedBudget, string previousComponentSpecs)
        {
            var systemPrompt = @"Sen PCPartsBuild platformunun eğitici asistanısın. Kullanıcı bir parçayı başarıyla seçti. Ona BİR SONRAKİ adım için sol taraftaki filtreleri nasıl kullanması gerektiğini söyleyeceksin.
KURALLAR:
1. Doğrudan marka/model önerme. Seçimi kullanıcıya bırak.
2. Bir önceki parçanın teknik özelliklerini baz alarak, sol menüden tam olarak neleri filtrelemesi gerektiğini net belirt.
3. Kalan bütçeyi hatırlat.
4. Maksimum 4 cümle kur, samimi ol. Asla maddeleme (bullet point) kullanma.";

            var userPrompt = $@"- Yeni Seçilecek Parça: {nextStepName}
- Bu Adım İçin Ayrılan Bütçe: {allocatedBudget:N0} TL
- Bir Önceki Seçilen Parça Özellikleri: {previousComponentSpecs}";

            return await CallLlmAsync(systemPrompt, userPrompt);
        }

        public async Task<string> GenerateErrorMessageAsync(string attemptedComponentName, string existingComponentName, string errorReason)
        {
            var systemPrompt = @"Sen PCPartsBuild karar destek sisteminin asistanısın. Kullanıcı UYUMSUZ bir parça seçti ve sistem işlemi durdurdu.
KURALLAR:
1. Kullanıcıyı azarlama, eğitici bir dil kullan.
2. Neden hata aldığını teknik olarak (soket, boyut vb.) kısaca açıkla.
3. Sol menüdeki filtrelerden neyi düzeltmesi gerektiğini net bir şekilde söyle.
4. Onay verme, işlemi tekrar yapmasını iste. Maksimum 3 cümle kur. Asla maddeleme kullanma.";

            var userPrompt = $@"- Seçmeye Çalıştığı Parça: {attemptedComponentName}
- Sistemindeki Mevcut Parça: {existingComponentName}
- Hata Nedeni: {errorReason}";

            return await CallLlmAsync(systemPrompt, userPrompt);
        }

        private async Task<string> CallLlmAsync(string systemPrompt, string userPrompt)
        {
            try
            {
                var section = _configuration.GetSection("AiAssistant");
                var apiKey = section["ApiKey"] ?? "";
                var model = section["Model"] ?? "llama-3.3-70b-versatile";
                var baseUrl = section["BaseUrl"] ?? "https://api.groq.com/openai/v1/chat/completions";

                if (string.IsNullOrWhiteSpace(apiKey))
                {
                    _logger.LogWarning("AiAssistant:ApiKey boş — fallback mesaj döndürülüyor.");
                    return GenerateFallbackMessage(systemPrompt);
                }

                var client = _httpClientFactory.CreateClient("LlmClient");

                var requestBody = new
                {
                    model = model,
                    messages = new[]
                    {
                        new { role = "system", content = systemPrompt },
                        new { role = "user", content = userPrompt }
                    },
                    temperature = 0.7,
                    max_tokens = 300,
                    stream = false // Yanıtın parça parça gelmesini engelle
                };

                var json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

                var response = await client.PostAsync(baseUrl, content);
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorBody = await response.Content.ReadAsStringAsync();
                    _logger.LogError("LLM API hatası: {StatusCode} — {Body}", response.StatusCode, errorBody);
                    return GenerateFallbackMessage(systemPrompt);
                }

                var responseBody = await response.Content.ReadAsStringAsync();

                // OpenAI-compatible response parse
                using var doc = JsonDocument.Parse(responseBody);
                var aiMessage = doc.RootElement
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString();

                return aiMessage ?? GenerateFallbackMessage(systemPrompt);
            }
            catch (TaskCanceledException ex)
            {
                // Hatanın gerçekten timeout mu yoksa başka bir iptal mi olduğunu logla
                _logger.LogWarning("LLM API isteği iptal edildi veya zaman aşımına uğradı. Mesaj: {Message}", ex.Message);
                return GenerateFallbackMessage(systemPrompt);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "LLM API çağrısında beklenmeyen bir hata oluştu: {Message}", ex.Message);
                return GenerateFallbackMessage(systemPrompt);
            }
        }

        /// <summary>
        /// LLM erişilemez olduğunda statik fallback mesajlar döner.
        /// </summary>
        private static string GenerateFallbackMessage(string systemPrompt)
        {
            if (systemPrompt.Contains("UYUMSUZ"))
                return "Seçtiğin parça mevcut sistemle uyumlu değil. Sol menüdeki filtreleri kontrol edip tekrar denemelisin.";

            if (systemPrompt.Contains("BİR SONRAKİ"))
                return "Harika, parça başarıyla eklendi! Şimdi bir sonraki adıma geçebilirsin. Sol menüdeki filtreleri kullanarak bütçene uygun parçaları listele.";

            return "PC toplama asistanına hoş geldin! Sol menüdeki filtreleri kullanarak bütçene ve kullanım amacına uygun parçaları listeleyebilirsin. İlk adım olarak bir işlemci seçerek başla.";
        }
    }
}
