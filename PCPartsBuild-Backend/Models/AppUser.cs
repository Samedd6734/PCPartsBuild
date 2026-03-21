using Microsoft.AspNetCore.Identity;

namespace PCPartsAPI.Models
{
    // IdentityUser'dan miras alıyoruz, böylece onun tüm özelliklerine (Email, Password vs.) sahip oluyoruz.
    // Artı olarak FullName ekliyoruz.
    public class AppUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
    }
}