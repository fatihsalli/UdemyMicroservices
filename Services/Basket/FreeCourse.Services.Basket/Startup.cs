using FreeCourse.Services.Basket.Services;
using FreeCourse.Services.Basket.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
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
            //appsettings'i okuyarak "RedisSettings" classýndaki propertyleri set ediyoruz.
            services.Configure<RedisSettings>(Configuration.GetSection("RedisSettings"));

            //Metotlarýn çalýþabilmesi için içine girerek ayarlamayý yaptýk.
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


            services.AddControllers();
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

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
