using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;

namespace FreeCourse.Services.FakePayment
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
            //Order ile FakePayment asenkron iletiþim olacak. Buradaki düzenlemeler bunun için.
            //Default port: 5672
            services.AddMassTransit(x =>
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(Configuration["RabbitMQUrl"], "/", host =>
                    {
                        //Rabbit Mq ile default olarak gelen deðeri kullanýyoruz.
                        host.Username("guest");
                        host.Password("guest");
                    });
                });
            });
            //Sonrasýnda "AddMassTransitHostedService" ekliyoruz.
            services.AddMassTransitHostedService();

            //Json web token payloadýnda "sub" tipinde bir user olduðu için policy oluþturarak bunu kontrol ediyoruz.
            var requireAuthorizePolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
            //Json web tokenda yer alan sub baþlýðýný (User id'nin tutulduðu) nameidentifier olarak çeviriyor bunu deðiþtiriyoruz. Burada söylediðimiz herbir claimi maplerken sub tipinde olaný mapleme diyoruz.
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("sub");

            //Jwt ile kimlik doðrulama için aþaðýdaki kodlarý tanýmladýk.
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                //Tokený kimin daðýttýðý bilgisini veriyoruz.
                options.Authority = Configuration["IdentityServerURL"];
                //Audience ý belirttik. Bu rastgele bir deðer deðil IdentityServer=>Config dosyasý üzerinden gelir.
                options.Audience = "resource_payment";
                //Https bekleyeceði için onu belirttik
                options.RequireHttpsMetadata = false;
            });

            //Jwt ile user üzerinden koruma altýna almak için aþaðýdaki filterý ekledik.
            services.AddControllers(opt =>
            {
                opt.Filters.Add(new AuthorizeFilter(requireAuthorizePolicy));
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "FreeCourse.Services.FakePayment", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "FreeCourse.Services.FakePayment v1"));
            }

            app.UseRouting();

            //Authenticationý tanýmladýk.
            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
