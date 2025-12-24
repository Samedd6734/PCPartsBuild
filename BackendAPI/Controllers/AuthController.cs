using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using PCPartsAPI.DTOs;
using PCPartsAPI.Models; // AppUser için gerekli
using System;
using System.Threading.Tasks;

namespace PCPartsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        // ARTIK IdentityUser DEĞİL, AppUser KULLANIYORUZ
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly IMemoryCache _memoryCache;

        public AuthController(UserManager<AppUser> userManager,
                              SignInManager<AppUser> signInManager,
                              IEmailSender emailSender,
                              IMemoryCache memoryCache)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _memoryCache = memoryCache;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            // BURASI DEĞİŞTİ: AppUser oluşturuyoruz ve FullName'i aktarıyoruz
            var user = new AppUser
            {
                UserName = registerDto.Username,
                Email = registerDto.Email,
                FullName = registerDto.FullName // <-- Yeni eklediğin alan
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok(new { Message = "Kayıt başarılı! Giriş yapabilirsiniz." });
        }

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

            return Ok(new
            {
                Message = "Giriş başarılı!",
                Username = user.UserName,
                UserId = user.Id,
                FullName = user.FullName // İstersen girişte ismini de dönebilirsin
            });
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                return Ok(new { Message = "Doğrulama kodu e-posta adresinize gönderildi." });
            }

            var verificationCode = new Random().Next(100000, 999999).ToString();
            _memoryCache.Set($"ResetCode_{model.Email}", verificationCode, TimeSpan.FromMinutes(15));

            await _emailSender.SendEmailAsync(model.Email, "Şifre Sıfırlama Kodu",
                $"<h3>Şifre Sıfırlama Talebi</h3>" +
                $"<p>Hesabınızın şifresini sıfırlamak için aşağıdaki kodu kullanın:</p>" +
                $"<h2 style='color:#16a3b2'>{verificationCode}</h2>" +
                $"<p>Bu kod 15 dakika geçerlidir.</p>");

            return Ok(new { Message = "Doğrulama kodu e-posta adresinize gönderildi." });
        }

        [HttpPost("verify-code")]
        public async Task<IActionResult> VerifyCode([FromBody] VerifyCodeDto model)
        {
            if (!_memoryCache.TryGetValue($"ResetCode_{model.Email}", out string storedCode))
            {
                return BadRequest(new { Message = "Kodun süresi dolmuş veya hatalı." });
            }

            if (storedCode != model.Code)
            {
                return BadRequest(new { Message = "Girilen kod hatalı." });
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            _memoryCache.Remove($"ResetCode_{model.Email}");

            return Ok(new { Message = "Kod doğrulandı.", Token = token, Email = model.Email });
        }

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
        // AuthController.cs içine, diğer metodların altına ekle:

        [HttpGet("get-user/{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound(new { Message = "Kullanıcı bulunamadı." });
            }

            return Ok(new
            {
                Username = user.UserName,
                Email = user.Email,
                FullName = user.FullName, // Ad Soyad buradan gidecek
                PhoneNumber = user.PhoneNumber
            });
        }
        [HttpPut("update-profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null) return NotFound(new { Message = "Kullanıcı bulunamadı." });

            // --- 1. KULLANICI ADI KONTROLÜ VE GÜNCELLEME ---
            if (model.Username != user.UserName)
            {
                var existingUserByName = await _userManager.FindByNameAsync(model.Username);
                if (existingUserByName != null)
                {
                    return BadRequest(new { Message = "Bu kullanıcı adı zaten kullanılıyor." });
                }
                user.UserName = model.Username;
            }

            // --- 2. DİĞER BİLGİLERİ GÜNCELLE ---
            user.FullName = model.FullName;
            user.PhoneNumber = model.PhoneNumber;

            // --- 3. E-POSTA DEĞİŞİKLİĞİ KONTROLÜ ---
            bool emailChangeRequested = false;

            // Eğer yeni mail girilmişse ve mevcut mailden farklıysa
            if (!string.IsNullOrEmpty(model.Email) && model.Email != user.Email)
            {
                // A) Format Kontrolü (Saçma yazı engelleme)
                if (!IsValidEmail(model.Email))
                {
                    return BadRequest(new { Message = "Geçersiz e-posta formatı." });
                }

                // B) Başkası kullanıyor mu?
                var existingUserByEmail = await _userManager.FindByEmailAsync(model.Email);
                if (existingUserByEmail != null)
                {
                    return BadRequest(new { Message = "Bu e-posta adresi başka bir hesapta kayıtlı." });
                }

                // C) Hemen güncelleme! Onay süreci başlat.
                emailChangeRequested = true;

                // Token oluştur (Rastgele güvenli kod)
                var token = Guid.NewGuid().ToString();

                // Cache'e kaydet (15 Dakika ömürlü)
                // Anahtar: "EmailChange_TOKEN", Değer: "UserId|YeniEmail"
                string cacheKey = $"EmailChange_{token}";
                string cacheValue = $"{user.Id}|{model.Email}";
                _memoryCache.Set(cacheKey, cacheValue, TimeSpan.FromMinutes(15));

                // D) Onay Linki Oluştur (Frontend sayfasına yönlendirecek)
                // Kullanıcı bu linke tıkladığında token ile birlikte siteye gidecek.
                string callbackUrl = $"https://pcpartsbuild.com/hesabim.html?emailToken={token}";
               
                // --- GÜNCELLENMİŞ MODERN E-POSTA TASARIMI ---
                string emailTemplate = $@"
                <div style='font-family:Segoe UI, Helvetica, Arial, sans-serif; max-width: 600px; margin: 0 auto; background-color: #f3f4f6; padding: 40px 20px;'>
                    <div style='background-color: #ffffff; border-radius: 12px; box-shadow: 0 4px 6px rgba(0,0,0,0.05); overflow: hidden;'>
                
                        <div style='background-color: #1f2937; padding: 30px; text-align: center;'>
                            <h1 style='color: #16a3b2; margin: 0; font-size: 24px; letter-spacing: 1px;'>PCPartsBuild</h1>
                            <p style='color: #9ca3af; margin: 5px 0 0 0; font-size: 14px;'>Bilgisayar Toplama Sihirbazı</p>
                        </div>

                        <div style='padding: 40px 30px;'>
                            <h2 style='color: #111827; margin-top: 0; font-size: 20px; text-align: center;'>E-Posta Değişikliği Talebi</h2>
                    
                            <p style='color: #4b5563; font-size: 16px; line-height: 1.6; margin-top: 20px;'>
                                Merhaba,
                            </p>
                            <p style='color: #4b5563; font-size: 16px; line-height: 1.6;'>
                                PCPartsBuild hesabınızın e-posta adresini <strong style='color: #16a3b2;'>{model.Email}</strong> olarak değiştirmek üzere bir talep aldık.
                            </p>
                            <p style='color: #4b5563; font-size: 16px; line-height: 1.6;'>
                                Hesabınızın güvenliğini sağlamak için bu işlemi onaylamanız gerekmektedir. Aşağıdaki butona tıklayarak değişikliği tamamlayabilirsiniz.
                            </p>

                            <div style='text-align: center; margin: 35px 0;'>
                                <a href='{callbackUrl}' style='background-color: #16a3b2; color: #ffffff; padding: 14px 32px; text-decoration: none; border-radius: 8px; font-weight: 600; font-size: 16px; display: inline-block; box-shadow: 0 4px 6px rgba(22, 163, 178, 0.25);'>
                                    Değişikliği Onayla
                                </a>
                            </div>

                            <div style='background-color: #fff1f2; border-left: 4px solid #e11d48; padding: 15px; border-radius: 4px; margin-top: 30px;'>
                                <p style='color: #9f1239; font-size: 13px; margin: 0; line-height: 1.5;'>
                                    <strong>Dikkat:</strong> Eğer bu değişikliği siz talep etmediyseniz, lütfen bu e-postayı dikkate almayın ve hesabınızın güvenliği için şifrenizi değiştirin. Linke tıklamadığınız sürece e-posta adresiniz değişmeyecektir.
                                </p>
                            </div>
                        </div>

                        <div style='background-color: #f9fafb; padding: 20px; text-align: center; border-top: 1px solid #e5e7eb;'>
                            <p style='color: #6b7280; font-size: 12px; margin: 0;'>
                                © 2025 PCPartsBuild. Tüm Hakları Saklıdır.
                            </p>
                            <p style='color: #9ca3af; font-size: 11px; margin-top: 5px;'>
                                Bu e-posta otomatik olarak gönderilmiştir, lütfen yanıtlamayınız.
                            </p>
                        </div>
                    </div>
                </div>";

                // E) ESKİ (Mevcut) Maile Link Gönder
                await _emailSender.SendEmailAsync(user.Email, "PCPartsBuild - E-Posta Değişikliği Onayı", emailTemplate);
            }

            // Veritabanını güncelle (İsim, Telefon, Username değiştiyse işlenir)
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                if (emailChangeRequested)
                {
                    // Frontend'e "Başarılı ama mailine bakman lazım" mesajı dönüyoruz
                    return Ok(new
                    {
                        Message = "Profil güncellendi. E-posta değişikliği için mevcut adresinize gönderilen onay linkine tıklayın.",
                        EmailPending = true
                    });
                }

                return Ok(new { Message = "Profil başarıyla güncellendi.", EmailPending = false });
            }

            return BadRequest(result.Errors);
        }

        [HttpPost("confirm-email-change")]
        public async Task<IActionResult> ConfirmEmailChange([FromBody] VerifyCodeDto model)
        {
            // Not: VerifyCodeDto içinde "Code" ve "Email" var. Biz Code alanına Token'ı göndereceğiz.
            // Email alanını kullanmayacağız ama DTO yapısı bozulmasın diye onu kullanabiliriz.
            // YA DA sadece string alan yeni bir DTO yapabilirsin ama VerifyCodeDto iş görür.

            string token = model.Code; // Frontend token'ı buraya koyacak

            // 1. Cache'den Token'ı Sorgula
            if (!_memoryCache.TryGetValue($"EmailChange_{token}", out string cacheValue))
            {
                return BadRequest(new { Message = "Onay linkinin süresi dolmuş veya hatalı." });
            }

            // 2. Değeri Parçala (UserId | YeniEmail)
            var parts = cacheValue.Split('|');
            var userId = parts[0];
            var newEmail = parts[1];

            // 3. Kullanıcıyı Bul
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return BadRequest(new { Message = "Kullanıcı bulunamadı." });

            // 4. E-Postayı Son Kez Kontrol Et (Arada başkası almış mı?)
            var checkEmail = await _userManager.FindByEmailAsync(newEmail);
            if (checkEmail != null) return BadRequest(new { Message = "Bu e-posta adresi kullanımda." });

            // 5. E-Postayı ve Kullanıcı Adını (Genelde identity username=email tutar ama sen ayırdın) GÜNCELLE
            // Token ile mail değişimi yaparken Identity'nin kendi token yapısı yerine 
            // manuel set edip update ediyoruz çünkü kendi doğrulama sistemimizi kurduk.
            user.Email = newEmail;
            user.NormalizedEmail = _userManager.NormalizeEmail(newEmail);
            user.EmailConfirmed = true; // Onay linkine tıkladığı için true yapabiliriz

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                // İşlem bitince Cache'i temizle
                _memoryCache.Remove($"EmailChange_{token}");
                return Ok(new { Message = "E-posta adresiniz başarıyla değiştirildi!" });
            }

            return BadRequest(result.Errors);
        }

        // AuthController.cs içine en alta ekle
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}