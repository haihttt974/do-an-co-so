using doan3.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Globalization;

namespace doan3
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews()
                .AddViewLocalization()
                .AddDataAnnotationsLocalization();

            // Localization setup
            builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
            builder.Services.AddHostedService<KhoaHocStatusUpdater>();

            builder.Services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[]
                {
                    new CultureInfo("vi"),      // Vietnamese
                    new CultureInfo("en"),      // English
                    new CultureInfo("zh-Hans"), // Simplified Chinese
                    new CultureInfo("ja"),      // Japanese
                    new CultureInfo("ko"),      // Korean
                    new CultureInfo("es"),      // Spanish
                    new CultureInfo("de"),      // German
                    new CultureInfo("fr"),      // French
                    new CultureInfo("it")       // Italian
                };

                options.DefaultRequestCulture = new RequestCulture("vi");  // Default language
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;

                // Use cookie for culture storage
                options.RequestCultureProviders.Insert(0, new CookieRequestCultureProvider());
            });


            // Database context
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<DacsGplxContext>(options =>
                options.UseSqlServer(connectionString));

            // Authentication
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Account/Login";
                    options.LogoutPath = "/Account/Logout";
                    options.AccessDeniedPath = "/Account/AccessDenied";
                });

            builder.Services.AddSession();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            // Localization middleware
            var locOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(locOptions.Value);

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSession();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
