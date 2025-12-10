using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory; // Cache için gerekli
using PCPartsAPI.DTOs;
using System;
using System.Threading.Tasks;

namespace PCPartsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly IMemoryCache _memoryCache; // Kodları geçici tutmak için

        public AuthController(UserManager<IdentityUser> userManager,
                              SignInManager<IdentityUser> signInManager,
                              IEmailSender emailSender,
                              IMemoryCache memoryCache)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _memoryCache = memoryCache;
        }

        // --- 1. KAYIT OLMA ---
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var user = new IdentityUser
            {
                UserName = registerDto.Email,
                Email = registerDto.Email
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok(new { Message = "Kayıt başarılı! Giriş yapabilirsiniz." });
        }

        // --- 2. GİRİŞ YAPMA ---
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var user = await _userManager.FindByNameAsync(loginDto.LoginIdentifier)
                       ?? await _userManager.FindByEmailAsync(loginDto.LoginIdentifier);

            if (user == null)
            {
                return Unauthorized(new { Message = "Kullanıcı adı veya şifre hatalı." });
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (!result.Succeeded)
            {
                return Unauthorized(new { Message = "Kullanıcı adı veya şifre hatalı." });
            }

            return Ok(new { Message = "Giriş başarılı!", Username = user.UserName });
        }

        // --- 3. ADIM: ŞİFRE SIFIRLAMA KODU GÖNDER ---
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            // Kullanıcı yoksa bile güvenlik gereği "Gönderildi" diyoruz.
            if (user == null)
            {
                return Ok(new { Message = "Doğrulama kodu e-posta adresinize gönderildi." });
            }

            // 6 Haneli Rastgele Kod Üret
            var verificationCode = new Random().Next(100000, 999999).ToString();

            // Kodu MemoryCache'e kaydet (15 dakika geçerli olsun)
            // Anahtar: "ResetCode_user@email.com", Değer: "123456"
            _memoryCache.Set($"ResetCode_{model.Email}", verificationCode, TimeSpan.FromMinutes(15));

            // Mail Gönder
            await _emailSender.SendEmailAsync(model.Email, "Şifre Sıfırlama Kodu",
                $"<h3>Şifre Sıfırlama Talebi</h3>" +
                $"<p>Hesabınızın şifresini sıfırlamak için aşağıdaki kodu kullanın:</p>" +
                $"<h2 style='color:#16a3b2'>{verificationCode}</h2>" +
                $"<p>Bu kod 15 dakika geçerlidir.</p>");

            return Ok(new { Message = "Doğrulama kodu e-posta adresinize gönderildi." });
        }

        // --- 4. ADIM: KODU DOĞRULA VE TOKEN AL ---
        [HttpPost("verify-code")]
        public async Task<IActionResult> VerifyCode([FromBody] VerifyCodeDto model)
        {
            // Cache'den kodu kontrol et
            if (!_memoryCache.TryGetValue($"ResetCode_{model.Email}", out string storedCode))
            {
                return BadRequest(new { Message = "Kodun süresi dolmuş veya hatalı." });
            }

            if (storedCode != model.Code)
            {
                return BadRequest(new { Message = "Girilen kod hatalı." });
            }

            // Kod doğruysa, şimdi gerçek şifre sıfırlama token'ını üretiyoruz
            var user = await _userManager.FindByEmailAsync(model.Email);
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            // İşlem tamamlandığı için kodu silebiliriz (tek kullanımlık olsun)
            _memoryCache.Remove($"ResetCode_{model.Email}");

            // Token'ı frontend'e dönüyoruz ki diğer sayfaya geçebilsin
            return Ok(new { Message = "Kod doğrulandı.", Token = token, Email = model.Email });
        }

        // --- 5. ADIM: ŞİFREYİ GÜNCELLE ---
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return BadRequest("Hatalı istek.");

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);

            if (result.Succeeded)
            {
                return Ok(new { Message = "Şifreniz başarıyla değiştirildi. Yeni şifrenizle giriş yapabilirsiniz." });
            }

            return BadRequest(result.Errors);
        }
    }
}