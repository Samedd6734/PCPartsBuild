using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services; // IEmailSender için gerekli
using Microsoft.AspNetCore.Mvc;
using PCPartsAPI.DTOs; // DTO'lar için

namespace PCPartsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IEmailSender _emailSender; // YENİ: Mail servisini ekledik

        // Constructor (Yapıcı Metot) güncellendi: Artık EmailSender da istiyor
        public AuthController(UserManager<IdentityUser> userManager, 
                              SignInManager<IdentityUser> signInManager,
                              IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }

        // --- 1. KAYIT OLMA (GÜNCELLENDİ: Mail Gönderme Eklendi) ---
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var user = new IdentityUser
            {
                UserName = registerDto.Email, // Genelde username olarak email kullanılır, karışıklığı önler
                Email = registerDto.Email
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            // --- YENİ EKLENEN KISIM BAŞLANGICI ---
            
            // Token üret
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            // Doğrulama Linki Oluştur (API'ye geri döner)
            var confirmationLink = Url.Action(nameof(ConfirmEmail), "Auth", 
                new { userId = user.Id, token = token }, Request.Scheme);

            // Mail Gönder
            await _emailSender.SendEmailAsync(user.Email, "Hoşgeldiniz! Mailinizi Doğrulayın",
                $"Lütfen hesabınızı doğrulamak için <a href='{confirmationLink}'>buraya tıklayınız</a>.");

            // --- YENİ EKLENEN KISIM BİTİŞİ ---

            return Ok(new { Message = "Kayıt başarılı! Lütfen e-posta kutunuzu kontrol edip hesabınızı doğrulayın." });
        }

        // --- 2. GİRİŞ YAPMA (SENİN ESKİ KODUN + KÜÇÜK BİR KONTROL) ---
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            // Kullanıcıyı bul (Email veya Kullanıcı Adı ile)
            var user = await _userManager.FindByNameAsync(loginDto.LoginIdentifier);
            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(loginDto.LoginIdentifier);
            }

            if (user == null)
            {
                return Unauthorized(new { Message = "Kullanıcı adı veya şifre hatalı." });
            }

            // EKSTRA GÜVENLİK: Eğer maili doğrulamadıysa giriş yapamasın (İsteğe bağlı)
            // if (!await _userManager.IsEmailConfirmedAsync(user))
            // {
            //     return BadRequest("Lütfen önce e-posta adresinizi doğrulayın.");
            // }

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (!result.Succeeded)
            {
                return Unauthorized(new { Message = "Kullanıcı adı veya şifre hatalı." });
            }

            return Ok(new { Message = "Giriş başarılı!", Username = user.UserName });
        }

        // --- 3. MAİL DOĞRULAMA (YENİ) ---
        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null) return BadRequest("Hatalı link.");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return BadRequest("Kullanıcı bulunamadı.");

            var result = await _userManager.ConfirmEmailAsync(user, token);
            
            if (result.Succeeded)
            {
                // Başarılı olursa kullanıcıya basit bir mesaj dönüyoruz.
                // İstersen burada "Tebrikler.html" sayfasına yönlendirme (Redirect) de yapabilirsin.
                return Ok("E-posta başarıyla doğrulandı! Artık giriş yapabilirsiniz.");
            }
            
            return BadRequest("Doğrulama başarısız.");
        }

        // --- 4. ŞİFREMİ UNUTTUM (YENİ) ---
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            
            // Kullanıcı yoksa veya mail doğrulanmamışsa güvenlik gereği renk vermiyoruz
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                return Ok(new { Message = "Şifre sıfırlama linki e-posta adresinize gönderildi." });
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            // BURASI ÖNEMLİ: Link, senin oluşturduğun HTML sayfasına gidiyor!
            // wwwroot içindeki 'sifre-sifirla.html' dosyasına yönlendiriyoruz.
            var resetLink = $"{Request.Scheme}://{Request.Host}/sifre-sifirla.html?token={System.Net.WebUtility.UrlEncode(token)}&email={model.Email}";

            await _emailSender.SendEmailAsync(model.Email, "Şifre Sıfırlama Talebi",
                $"Şifrenizi yenilemek için <a href='{resetLink}'>buraya tıklayın</a>.");

            return Ok(new { Message = "Şifre sıfırlama linki e-posta adresinize gönderildi." });
        }

        // --- 5. ŞİFRE SIFIRLAMA İŞLEMİ (YENİ) ---
        // HTML sayfasındaki form buraya veri gönderecek
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return BadRequest("Hatalı istek.");

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);

            if (result.Succeeded)
            {
                return Ok(new { Message = "Şifreniz başarıyla değiştirildi." });
            }

            return BadRequest(result.Errors);
        }
    }
}