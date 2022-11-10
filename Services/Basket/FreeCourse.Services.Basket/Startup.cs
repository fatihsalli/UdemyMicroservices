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
            //Json web token payloadýnda "sub" tipinde bir user olduðu için policy oluþturarak bunu kontrol ediyoruz.
            var requireAuthorizePolicy=new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
            //Json web tokenda yer alan sub baþlýðýný (User id'nin tutulduðu) nameidentifier olarak çeviriyor bunu deðiþtiriyoruz. Burada söylediðimiz herbir claimi maplerken sub tipinde olaný mapleme diyoruz.
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("sub");


            //Jwt ile kimlik doðrulama için aþaðýdaki kodlarý tanýmladýk.
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                //Tokený kimin daðýttýðý bilgisini veriyoruz.
                options.Authority = Configuration["IdentityServerURL"];
                //Audience ý belirttik. Bu rastgele bir deðer deðil IdentityServer=>Config dosyasý üzerinden gelir.
                options.Audience = "resource_basket";
                //Https bekleyeceði için onu belirttik
                options.RequireHttpsMetadata = false;
            });

            //appsettings'i okuyarak "RedisSettings" classýndaki propertyleri set ediyoruz.
            services.Configure<RedisSettings>(Configuration.GetSection("RedisSettings"));
            //SharedLibrary-Services içerisinde SharedIdentityService "IHttpContextAccessor" interface ini kullanabilmek için burada eklememiz gerekmektedir.
            services.AddHttpContextAccessor();
            //Shared Librarydeki "SharedIdentityService" classýný kullanabilmek için burada DI containerda ekledik.
            services.AddScoped<ISharedIdentityService, SharedIdentityService>();
            //Basket service nesne türettik
            services.AddScoped<IBasketService, BasketService>();

            //Metotlarýn çalýþabilmesi için içine girerek ayarlamayý yaptýk. <RedisService> bu ifadeyi singletonda siledebiliriz. Zaten içine girerek nesneyi veriyoruz.
            services.AddSingleton<RedisService>(sp =>
            {
                //Redissetting içindeki ayarlamalarý okumak için
                var redisSettings = sp.GetRequiredService<IOptions<RedisSettings>>().Value;
                //RedisService nesnesini türettik
                var redis=new RedisService(redisSettings.Host, redisSettings.Port);
                //RedisService Connect metotunu tetikledik.
                redis.Connect();
                //Geriye nesneyi döndük.
                return redis;
            });

            //Jwt ile user üzerinden koruma altýna almak için aþaðýdaki filterý ekledik.
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
