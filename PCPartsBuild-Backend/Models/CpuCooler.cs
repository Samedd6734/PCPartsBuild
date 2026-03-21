using System.ComponentModel.DataAnnotations;

namespace PCPartsAPI.Models
{
    public class CpuCooler
    {
        public int Id { get; set; }
        public string Brand { get; set; } = string.Empty; // Örn: "Noctua", "Corsair"
        public string ModelName { get; set; } = string.Empty;
        public string CoolerType { get; set; } = string.Empty; // "Air" (Hava) veya "Liquid" (Sıvı/AIO)

        // --- 1. PERFORMANS UYUMLULUĞU (TDP) ---
        // Bu değer CPU'nun TDP değerinden BÜYÜK olmalı.
        // Örn: i9-14900K (253W) için 150W'lık bir soğutucu yetersiz kalır ve sistem kapanır.
        public int TdpRating { get; set; } // Watt cinsinden soğutma kapasitesi (Örn: 250)

        // --- 2. SOKET UYUMLULUĞU ---
        // Virgülle ayrılmış string veya JSON olarak tutulabilir.
        // Örn: "LGA1700, AM5, AM4, LGA1200, ..."
        public string SupportedSockets { get; set; } = string.Empty;

        // --- 3. FİZİKSEL BOYUTLAR & KASA UYUMLULUĞU ---

        // Kule Tipi İçin:
        // Kasanın "MaxCpuCoolerHeight" değerinden küçük olmalı.
        public int Height { get; set; } // mm (Tabandan tepeye yükseklik)

        // Sıvı Soğutma İçin:
        // Kasanın radyatör desteği (Örn: "Front 360mm") ile eşleşmeli.
        public int RadiatorSize { get; set; } // mm (120, 240, 280, 360, 420)

        // KRİTİK DETAY: Radyatör Kalınlığı (+Fan)
        // Bazı kasalarda anakartın VRM soğutucularına veya RAM'e çarpabilir

        // --- 5. FAN VE GÜRÜLTÜ DETAYLARI ---
        public int FanSize { get; set; } // mm (120, 140)
        public int FanCount { get; set; } // Radyatör veya blok üzerindeki fan sayısı
        public int MaxFanSpeed { get; set; } // RPM
        public double NoiseLevel { get; set; } // dBA (Kullanıcı sessiz sistem istiyorsa filtrelemek için)
        public double MaxAirflow { get; set; } // CFM

        // --- 6. BAĞLANTI DETAYLARI (Anakart Header Uyumu) ---
        public bool HasRgb { get; set; } // Işıklı mı?
        public string RgbConnectorType { get; set; } = string.Empty; // "3-pin 5V ARGB", "4-pin 12V RGB" veya "Yok"

        // YENİ NESİL DETAY: LCD Ekran
        // LCD ekranlı soğutucular (Örn: Kraken Elite) anakartta dahili USB 2.0 portu ister.
        // Anakartta yeterli port yoksa kullanıcı "USB Hub" almalıdır.
        public bool RequiresInternalUsbHeader { get; set; }
        public string ImageUrl { get; set; } = ""; // Resim linki 
    }
}