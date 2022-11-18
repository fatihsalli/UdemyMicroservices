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
            //ClientId ve ClientSecret deðerleri için "ClientSettings" classýný oluþturduk. Options pattern üzerinden dolduracaðýz.
            services.Configure<ClientSettings>(Configuration.GetSection("ClientSettings"));

            //Options pattern ile appsettings deki ayarlarýmýzý "ServiceApiSettings" classý üzerinden okuyacaðýz.
            services.Configure<ServiceApiSettings>(Configuration.GetSection("ServiceApiSettings"));

            //IdentityService üzerinden kullanabilmek için.
            services.AddHttpContextAccessor();

            //IClientAccessTokenCache kullanmamazý saðlýyor. IdentityModel kütüphanesi
            services.AddAccessTokenManagement();

            //PhotoHelper DI Contanier a ekledik.
            services.AddSingleton<PhotoHelper>();

            //ISharedIdentityService nesne türettik. UserId için.
            services.AddScoped<ISharedIdentityService, SharedIdentityService>();

            //ResourceOwnerPasswordTokenHandler DI Containere ekledik
            services.AddScoped<ResourceOwnerPasswordTokenHandler>();

            //ClientCredentialTokenHandler DI Containere ekledik
            services.AddScoped<ClientCredentialTokenHandler>();

            //Extension metot
            services.AddHttpClientServices(Configuration);

            //Cookie oluþturuyoruz. Þemayý verdik servis tarafýnda yazdýðýmýz.
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, opt =>
            {
                opt.LoginPath = "/Auth/SignIn";
                //Refresh token 60 gün olduðu için burada da 60 gün verdik.
                opt.ExpireTimeSpan=TimeSpan.FromDays(60);
                //60 gün içinde giriþ yaptýðýnda süre uzasýn mý=> true dedik
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
