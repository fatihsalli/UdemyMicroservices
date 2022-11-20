using FreeCourse.Services.Order.Application.Consumer;
using FreeCourse.Services.Order.Application.Handlers;
using FreeCourse.Services.Order.Infrastructure;
using FreeCourse.Shared.Services;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;

namespace FreeCourse.Services.Order.API
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
            //Asenkron iletiþim dinlemek için kullanýyoruz. FakeOrder.Api den kopyaladýk.
            //Order ile FakePayment asenkron iletiþim olacak. Buradaki düzenlemeler bunun için.
            //Default port: 5672
            services.AddMassTransit(x =>
            {
                //Consumerý ekliyoruz
                x.AddConsumer<CreateOrderMessageCommandConsumer>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(Configuration["RabbitMQUrl"], "/", host =>
                    {
                        //Rabbit Mq ile default olarak gelen deðeri kullanýyoruz.
                        host.Username("guest");
                        host.Password("guest");
                    });

                    //Haberdar ediyoruz. Bu kuyruðu FakePayment controller tarafýnda ismini verdik. Hangi endpionti okuyacaðýný belirttik.
                    cfg.ReceiveEndpoint("create-order-service", e =>
                    {
                        e.ConfigureConsumer<CreateOrderMessageCommandConsumer>(context);
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
                options.Audience = "resource_order";
                //Https bekleyeceði için onu belirttik
                options.RequireHttpsMetadata = false;
            });

            //Database ekledik.
            services.AddDbContext<OrderDbContext>(opt =>
            {
                opt.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"), configure =>
                {
                    //Migrationýn nereye eklendiðini gösteriyoruz.
                    configure.MigrationsAssembly("FreeCourse.Services.Order.Infrastructure");
                });
            });

            //MediatR DI Containera ekledik. Dosya konumunu istediði için aþaðýdaki gibi verdik.
            services.AddMediatR(typeof(CreateOrderCommandHandler).Assembly);

            //Tokendan UserId alabilmek için ISharedIdentityService nesnesini DI Containera ekledik.
            services.AddScoped<ISharedIdentityService, SharedIdentityService>();
            //HttpContextAccessor ýrýmýzýda eklemeliyiz. "SharedIdentityService" DI Containerdan bekliyor.
            services.AddHttpContextAccessor();

            //Jwt ile user üzerinden koruma altýna almak için aþaðýdaki filterý ekledik.
            services.AddControllers(opt =>
            {
                opt.Filters.Add(new AuthorizeFilter(requireAuthorizePolicy));
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "FreeCourse.Services.Order.API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "FreeCourse.Services.Order.API v1"));
            }

            app.UseRouting();

            //Authentication ekledik.
            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
