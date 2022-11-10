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
            //Jwt ile kimlik do�rulama i�in a�a��daki kodlar� tan�mlad�k.
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                //Token� kimin da��tt��� bilgisini veriyoruz.
                options.Authority = Configuration["IdentityServerURL"];
                //Audience � belirttik. Bu rastgele bir de�er de�il IdentityServer=>Config dosyas� �zerinden gelir.
                options.Audience = "photo_stock_catalog";
                //Https bekleyece�i i�in onu belirttik
                options.RequireHttpsMetadata = false;
            });

            //T�m Controller �zerine [Authorize] yazmak yerine a�a��daki d�zenlemeyi Startup taraf�nda yapt�k. AddController i�erisine opt ile girerek d�zenlemeyi yap�yoruz.
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
            //wwwroot olu�turduktan sonra bu klas�r� d�� d�nyaya a�mak i�in.
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
