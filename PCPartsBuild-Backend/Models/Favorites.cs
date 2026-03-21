using System.ComponentModel.DataAnnotations;

namespace PCPartsAPI.Models
{
    public class Favorites
    {
        [Key]
        public int Id { get; set; }

        public string UserId { get; set; } // Kullanıcı ID'si (String çünkü AspNetUsers Id string)

        public string ComponentType { get; set; } // Örn: "cpu", "gpu", "motherboard"

        public int ComponentId { get; set; } // Parçanın ID'si (Örn: Ryzen 5 5600'ün ID'si)
    }
}