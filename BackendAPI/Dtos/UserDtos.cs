namespace PCPartsAPI.DTOs
{
    // Kayıt olurken istenecek veriler
    public class RegisterDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    // Şifremi unuttum derken istenecek veri
    public class ForgotPasswordDto
    {
        public string Email { get; set; }
    }

    // Şifre sıfırlarken istenecek veriler
    public class ResetPasswordDto
    {
        public string Email { get; set; }
        public string Token { get; set; } 
        public string NewPassword { get; set; }
    }

    // --- İŞTE EKSİK OLAN KISIM BURASIYDI ---
    // Giriş yaparken istenecek veriler
    public class LoginDto
    {
        // Kullanıcı buraya E-mail de yazabilir, Kullanıcı Adı da.
        public string LoginIdentifier { get; set; } 
        public string Password { get; set; }
    }
}