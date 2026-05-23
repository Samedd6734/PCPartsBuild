using System;
using Microsoft.EntityFrameworkCore;
using PCPartsAPI.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services; // 1. EKLENDİ: Mail arayüzü
using PCPartsAPI.Services; // 2. EKLENDİ: Senin EmailSender sınıfın
using PCPartsAPI.Services.Interfaces; // Asistan servisleri
using PCPartsAPI.Models; // AppUser için gerekli
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;


namespace PCPartsAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddTransient<IEmailSender, EmailSender>();
            builder.Services.AddMemoryCache();

            // --- ASISTAN SERVİSLERİ ---
            builder.Services.AddScoped<IDynamicBudgetService, DynamicBudgetService>();
            builder.Services.AddScoped<ICompatibilityEngine, CompatibilityEngine>();
            builder.Services.AddScoped<IAiPromptService, AiPromptService>();
            builder.Services.AddScoped<ISessionManager, SessionManager>();

            // Named HttpClient for LLM API (Groq/Gemini)
            builder.Services.AddHttpClient("LlmClient", client =>
            {
                client.Timeout = TimeSpan.FromSeconds(60);
            });


            // --- 1. CORS AYARLARI ---
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    b => b.SetIsOriginAllowed(origin => 
                        {
                            var host = new Uri(origin).Host;
                            return host == "localhost" || host == "127.0.0.1";
                        })
                         .AllowAnyHeader()
                         .AllowAnyMethod()
                         .AllowCredentials());
            });

            // --- VERİTABANI BAĞLANTISI ---
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            // --- JWT KİMLİK DOĞRULAMA AYARLARI ---
            var jwtSettings = builder.Configuration.GetSection("Jwt");
            var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false; // Geliştirme ortamında false, canlıda true yapılabilir
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidateAudience = true,
                    ValidAudience = jwtSettings["Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

            // --- IDENTITY (KULLANICI) AYARLARI ---
            builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
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
            builder.Services.AddControllers()
                .AddJsonOptions(options => {
                    options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
                });
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // --- MIDDLEWARE (UYGULAMA AKIŞI) ---

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            else
            {
                app.UseHttpsRedirection();
            }

            // Frontend ve Backend ayrı olduğu için StaticFiles'a (HTML sunmaya) gerek kalmadı.
            // Sadece API hizmeti vereceğiz.

            // 5. CORS'U AKTİF ET (Auth'dan önce olmalı)
            app.UseRouting();
            app.UseCors("AllowAll");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.Run();
        }
    }
}