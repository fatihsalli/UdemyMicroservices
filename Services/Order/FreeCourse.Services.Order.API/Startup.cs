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
            //Asenkron ileti�im dinlemek i�in kullan�yoruz. FakeOrder.Api den kopyalad�k.
            //Order ile FakePayment asenkron ileti�im olacak. Buradaki d�zenlemeler bunun i�in.
            //Default port: 5672
            services.AddMassTransit(x =>
            {
                //Consumer� ekliyoruz
                x.AddConsumer<CreateOrderMessageCommandConsumer>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(Configuration["RabbitMQUrl"], "/", host =>
                    {
                        //Rabbit Mq ile default olarak gelen de�eri kullan�yoruz.
                        host.Username("guest");
                        host.Password("guest");
                    });

                    //Haberdar ediyoruz. Bu kuyru�u FakePayment controller taraf�nda ismini verdik. Hangi endpionti okuyaca��n� belirttik.
                    cfg.ReceiveEndpoint("create-order-service", e =>
                    {
                        e.ConfigureConsumer<CreateOrderMessageCommandConsumer>(context);
                    });
                });
            });
            //Sonras�nda "AddMassTransitHostedService" ekliyoruz.
            services.AddMassTransitHostedService();

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
                options.Audience = "resource_order";
                //Https bekleyece�i i�in onu belirttik
                options.RequireHttpsMetadata = false;
            });

            //Database ekledik.
            services.AddDbContext<OrderDbContext>(opt =>
            {
                opt.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"), configure =>
                {
                    //Migration�n nereye eklendi�ini g�steriyoruz.
                    configure.MigrationsAssembly("FreeCourse.Services.Order.Infrastructure");
                });
            });

            //MediatR DI Containera ekledik. Dosya konumunu istedi�i i�in a�a��daki gibi verdik.
            services.AddMediatR(typeof(CreateOrderCommandHandler).Assembly);

            //Tokendan UserId alabilmek i�in ISharedIdentityService nesnesini DI Containera ekledik.
            services.AddScoped<ISharedIdentityService, SharedIdentityService>();
            //HttpContextAccessor �r�m�z�da eklemeliyiz. "SharedIdentityService" DI Containerdan bekliyor.
            services.AddHttpContextAccessor();

            //Jwt ile user �zerinden koruma alt�na almak i�in a�a��daki filter� ekledik.
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
