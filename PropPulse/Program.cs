using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PropPulse.Data;
using PropPulse.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.Features;

namespace PropPulse
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<PropPulseContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("PropPulseContext") ?? throw new InvalidOperationException("Connection string 'PropPulseContext' not found.")));
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(
                options => { options.LoginPath = "/Account/Login";
                    options.LogoutPath = "/Account/Logout";
                    options.AccessDeniedPath = "/Account/AccessDenied";
                    }
                );
            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = 52428800; // 50 MB
            });

            var app = builder.Build();

            // HTTP istekleri yap�land�rmas�n� yap�yoruz.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            // HTTPS y�nlendirmesi ve statik dosyalar.
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            // Routing ve authorization yap�land�rmas�
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            // Varsay�lan rota ayarl�yoruz
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            // Uygulamay� �al��t�r�yoruz.
            app.Run();
        }
    }
}
