using FreeCourse.Web.Handler;
using FreeCourse.Web.Models;
using FreeCourse.Web.Services;
using FreeCourse.Web.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            //IdentityService �zerinden kullanabilmek i�in.
            services.AddHttpContextAccessor();

            //
            var serviceApiSettings=Configuration.GetSection("ServiceApiSettings").Get<ServiceApiSettings>();

            //IdentityService de uygulama bize uygun bir HttpClient d�ns�n diye yazd�k.
            services.AddHttpClient<IIdentityService, IdentityService>();
            //UserService de uygulama bize uygun bir HttpClient d�ns�n diye yazd�k. Exception ekledik burada da belirtiyoruz => "AddHttpMessageHandler<ResourceOwnerPasswordTokenHandler>();"
            services.AddHttpClient<IUserService, UserService>(opt =>
            {
                opt.BaseAddress = new Uri(serviceApiSettings.IdentityBaseUri);
            }).AddHttpMessageHandler<ResourceOwnerPasswordTokenHandler>();

            //Options pattern ile appsettings deki ayarlar�m�z� "ServiceApiSettings" class� �zerinden okuyaca��z.
            services.Configure<ServiceApiSettings>(Configuration.GetSection("ServiceApiSettings"));

            //ClientId ve ClientSecret de�erleri i�in "ClientSettings" class�n� olu�turduk. Options pattern �zerinden dolduraca��z.
            services.Configure<ClientSettings>(Configuration.GetSection("ClientSettings"));

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
