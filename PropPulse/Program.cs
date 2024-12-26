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

            // ApplicationDbContext'i ekliyoruz ve doðru baðlantý dizesini kullanýyoruz.
            builder.Services.AddDbContext<PropPulseContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection") 
                                      ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.")));

            // Hizmetleri ekliyoruz (Controllers & Views)
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // HTTP istekleri yapýlandýrmasýný yapýyoruz.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            // HTTPS yönlendirmesi ve statik dosyalar.
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            // Routing ve authorization yapýlandýrmasý
            app.UseRouting();
            app.UseAuthorization();

            // Varsayýlan rota ayarlýyoruz
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            // Uygulamayý çalýþtýrýyoruz.
            app.Run();
        }
    }
}
