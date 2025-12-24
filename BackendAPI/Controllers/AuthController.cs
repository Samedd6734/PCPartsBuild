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
    }
}