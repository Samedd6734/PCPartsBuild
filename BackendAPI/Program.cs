using Microsoft.EntityFrameworkCore;
using PCPartsAPI.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services; // 1. EKLENDİ: Mail arayüzü
using PCPartsAPI.Services; // 2. EKLENDİ: Senin EmailSender sınıfın

namespace PCPartsAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddTransient<IEmailSender, EmailSender>();
            builder.Services.AddMemoryCache();

            // --- 1. CORS AYARLARI ---
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    b => b.WithOrigins(
                            "https://pcpartsbuild.com",
                            "https://www.pcpartsbuild.com",
                            "http://localhost:5500",       // VS Code Live Server (Localhost)
                            "http://127.0.0.1:5500"        // VS Code Live Server (IP versiyonu - Genelde bu çalışır)
                         )
                         .AllowAnyHeader()
                         .AllowAnyMethod()
                         .AllowCredentials());
            });

            // --- VERİTABANI BAĞLANTISI ---
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            // --- IDENTITY (KULLANICI) AYARLARI ---
            builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                // Şifre Kuralları (Senin ayarların)
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 4;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;

                // Güvenlik Ayarları
                options.User.RequireUniqueEmail = true; // Aynı maille 2 kayıt olamasın
                options.SignIn.RequireConfirmedEmail = false; // Test için false, bitince true yapabilirsin
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders(); // 3. EKLENDİ: Şifre sıfırlama kodu (Token) üretmek için ŞART!

            // 4. EKLENDİ: Mail Gönderme Servisini Tanıtma
            // Sistem "IEmailSender" istendiğinde senin "EmailSender" sınıfını kullanacak.
            builder.Services.AddScoped<EmailSender>();
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // --- MIDDLEWARE (UYGULAMA AKIŞI) ---

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            // Frontend ve Backend ayrı olduğu için StaticFiles'a (HTML sunmaya) gerek kalmadı.
            // Sadece API hizmeti vereceğiz.

            // 5. CORS'U AKTİF ET (Auth'dan önce olmalı)
            app.UseCors("AllowAll");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.Run();
        }
    }
}