namespace PCPartsAPI.DTOs
{
    public class FavoriteDetailDto
    {
        public int Id { get; set; } // Favori Kayıt ID'si (Silmek için lazım)
        public int ComponentId { get; set; } // Parçanın kendi ID'si
        public string ComponentType { get; set; } // "cpu", "gpu" vs.

        // Ekranda Gözükecekler
        public string Name { get; set; } // Örn: Intel Core i9-14900K
        public string Brand { get; set; } // Örn: Intel
        public string ImageUrl { get; set; } // Resim
        public string Specs { get; set; } // Örn: "3.2 GHz, 24 Çekirdek" (Özet bilgi)
        public decimal Price { get; set; } = 0; // Fiyat (Şimdilik 0 veya veritabanında varsa o)
    }
}