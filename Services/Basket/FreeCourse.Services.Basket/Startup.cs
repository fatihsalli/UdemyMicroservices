using FreeCourse.Services.Basket.Services;
using FreeCourse.Services.Basket.Settings;
using FreeCourse.Shared.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace FreeCourse.Services.Basket
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //Json web token payload�nda "sub" tipinde bir user oldu�u i�in policy olu�turarak bunu kontrol ediyoruz.
            var requireAuthorizePolicy=new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
            //Json web tokenda yer alan sub ba�l���n� (User id'nin tutuldu�u) nameidentifier olarak �eviriyor bunu de�i�tiriyoruz. Burada s�yledi�imiz herbir claimi maplerken sub tipinde olan� mapleme diyoruz.
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("sub");


            //Jwt ile kimlik do�rulama i�in a�a��daki kodlar� tan�mlad�k.
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                //Token� kimin da��tt��� bilgisini veriyoruz.
                options.Authority = Configuration["IdentityServerURL"];
                //Audience � belirttik. Bu rastgele bir de�er de�il IdentityServer=>Config dosyas� �zerinden gelir.
                options.Audience = "resource_basket";
                //Https bekleyece�i i�in onu belirttik
                options.RequireHttpsMetadata = false;
            });

            //appsettings'i okuyarak "RedisSettings" class�ndaki propertyleri set ediyoruz.
            services.Configure<RedisSettings>(Configuration.GetSection("RedisSettings"));
            //SharedLibrary-Services i�erisinde SharedIdentityService "IHttpContextAccessor" interface ini kullanabilmek i�in burada eklememiz gerekmektedir.
            services.AddHttpContextAccessor();
            //Shared Librarydeki "SharedIdentityService" class�n� kullanabilmek i�in burada DI containerda ekledik.
            services.AddScoped<ISharedIdentityService, SharedIdentityService>();
            //Basket service nesne t�rettik
            services.AddScoped<IBasketService, BasketService>();

            //Metotlar�n �al��abilmesi i�in i�ine girerek ayarlamay� yapt�k. <RedisService> bu ifadeyi singletonda siledebiliriz. Zaten i�ine girerek nesneyi veriyoruz.
            services.AddSingleton<RedisService>(sp =>
            {
                //Redissetting i�indeki ayarlamalar� okumak i�in
                var redisSettings = sp.GetRequiredService<IOptions<RedisSettings>>().Value;
                //RedisService nesnesini t�rettik
                var redis=new RedisService(redisSettings.Host, redisSettings.Port);
                //RedisService Connect metotunu tetikledik.
                redis.Connect();
                //Geriye nesneyi d�nd�k.
                return redis;
            });

            //Jwt ile user �zerinden koruma alt�na almak i�in a�a��daki filter� ekledik.
            services.AddControllers(opt=>
            {
                opt.Filters.Add(new AuthorizeFilter(requireAuthorizePolicy));
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "FreeCourse.Services.Basket", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "FreeCourse.Services.Basket v1"));
            }

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
