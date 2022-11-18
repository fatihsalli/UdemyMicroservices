using FreeCourse.Shared.Services;
using FreeCourse.Web.Extensions;
using FreeCourse.Web.Handler;
using FreeCourse.Web.Helpers;
using FreeCourse.Web.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace FreeCourse.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            //ClientId ve ClientSecret de�erleri i�in "ClientSettings" class�n� olu�turduk. Options pattern �zerinden dolduraca��z.
            services.Configure<ClientSettings>(Configuration.GetSection("ClientSettings"));

            //Options pattern ile appsettings deki ayarlar�m�z� "ServiceApiSettings" class� �zerinden okuyaca��z.
            services.Configure<ServiceApiSettings>(Configuration.GetSection("ServiceApiSettings"));

            //IdentityService �zerinden kullanabilmek i�in.
            services.AddHttpContextAccessor();

            //IClientAccessTokenCache kullanmamaz� sa�l�yor. IdentityModel k�t�phanesi
            services.AddAccessTokenManagement();

            //PhotoHelper DI Contanier a ekledik.
            services.AddSingleton<PhotoHelper>();

            //ISharedIdentityService nesne t�rettik. UserId i�in.
            services.AddScoped<ISharedIdentityService, SharedIdentityService>();

            //ResourceOwnerPasswordTokenHandler DI Containere ekledik
            services.AddScoped<ResourceOwnerPasswordTokenHandler>();

            //ClientCredentialTokenHandler DI Containere ekledik
            services.AddScoped<ClientCredentialTokenHandler>();

            //Extension metot
            services.AddHttpClientServices(Configuration);

            //Cookie olu�turuyoruz. �emay� verdik servis taraf�nda yazd���m�z.
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, opt =>
            {
                opt.LoginPath = "/Auth/SignIn";
                //Refresh token 60 g�n oldu�u i�in burada da 60 g�n verdik.
                opt.ExpireTimeSpan=TimeSpan.FromDays(60);
                //60 g�n i�inde giri� yapt���nda s�re uzas�n m�=> true dedik
                opt.SlidingExpiration = true;
                opt.Cookie.Name = "udemywebcookie";
            });

            services.AddControllersWithViews();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();                
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseRouting();

            //Authentication ekledik.
            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
