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
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace FreeCourse.Services.Discount
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
            var requireAuthorizePolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
            //Json web tokenda yer alan sub ba�l���n� (User id'nin tutuldu�u) nameidentifier olarak �eviriyor bunu de�i�tiriyoruz. Burada s�yledi�imiz herbir claimi maplerken sub tipinde olan� mapleme diyoruz.
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("sub");

            //Jwt ile kimlik do�rulama i�in a�a��daki kodlar� tan�mlad�k.
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                //Token� kimin da��tt��� bilgisini veriyoruz.
                options.Authority = Configuration["IdentityServerURL"];
                //Audience � belirttik. Bu rastgele bir de�er de�il IdentityServer=>Config dosyas� �zerinden gelir.
                options.Audience = "resource_discount";
                //Https bekleyece�i i�in onu belirttik
                options.RequireHttpsMetadata = false;
            });

            //Automapper ekledik. A�a��da belirtilen teknik ile �unu s�ylemek istiyoruz Startup'�n yer ald��� Assembly'de IProfileExpression ya da IProfileConfiguration'dan miras alan classlar� maplemeye dahil ediyor.
            services.AddAutoMapper(typeof(Startup));

            //Jwt ile user �zerinden koruma alt�na almak i�in a�a��daki filter� ekledik.
            services.AddControllers(opt =>
            {
                opt.Filters.Add(new AuthorizeFilter(requireAuthorizePolicy));
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "FreeCourse.Services.Discount", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "FreeCourse.Services.Discount v1"));
            }

            app.UseRouting();

            //Authentication for json
            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
