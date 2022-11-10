using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreeCourse.Services.PhotoStock
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
            //Jwt ile kimlik doðrulama için aþaðýdaki kodlarý tanýmladýk.
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                //Tokený kimin daðýttýðý bilgisini veriyoruz.
                options.Authority = Configuration["IdentityServerURL"];
                //Audience ý belirttik. Bu rastgele bir deðer deðil IdentityServer=>Config dosyasý üzerinden gelir.
                options.Audience = "photo_stock_catalog";
                //Https bekleyeceði için onu belirttik
                options.RequireHttpsMetadata = false;
            });

            //Tüm Controller üzerine [Authorize] yazmak yerine aþaðýdaki düzenlemeyi Startup tarafýnda yaptýk. AddController içerisine opt ile girerek düzenlemeyi yapýyoruz.
            services.AddControllers(opt =>
            {
                opt.Filters.Add(new AuthorizeFilter());
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "FreeCourse.Services.PhotoStock", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "FreeCourse.Services.PhotoStock v1"));
            }
            //wwwroot oluþturduktan sonra bu klasörü dýþ dünyaya açmak için.
            app.UseStaticFiles();

            app.UseRouting();

            //Authentication ekledik
            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
