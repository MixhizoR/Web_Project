using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PropPulse.Data;
using PropPulse.Data;
using Microsoft.EntityFrameworkCore;

namespace PropPulse
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ApplicationDbContext'i ekliyoruz ve do�ru ba�lant� dizesini kullan�yoruz.
            builder.Services.AddDbContext<PropPulseContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection") 
                                      ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.")));

            // Hizmetleri ekliyoruz (Controllers & Views)
            builder.Services.AddControllersWithViews();

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
