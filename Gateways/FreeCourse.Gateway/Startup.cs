using FreeCourse.Gateway.DelegateHandlers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

namespace FreeCourse.Gateway
{
    public class Startup
    {
        private readonly IConfiguration Configuration;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            //TokenExchangeDelagateHandler HttpClient belirtiyoruz.
            services.AddHttpClient<TokenExhangeDelegateHandler>();

            //Gateway.API JWT token ile koruma altýna aldýk.
            services.AddAuthentication().AddJwtBearer("GatewayAuthenticationScheme",options=>
            {
                //Tokený kimin daðýttýðý bilgisini veriyoruz.
                options.Authority = Configuration["IdentityServerURL"];
                //Audience ý belirttik. Bu rastgele bir deðer deðil IdentityServer=>Config dosyasý üzerinden gelir.
                options.Audience = "resource_gateway";
                //Https bekleyeceði için onu belirttik
                options.RequireHttpsMetadata = false;
            });

            //Ocelot kütüphanesini ekledik. ve Delegate ekledik token exhange için
            services.AddOcelot().AddDelegatingHandler<TokenExhangeDelegateHandler>();
        }

        public async void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //Asenkron olarak Ocelotu middleware olarak ekledik.
            await app.UseOcelot();

        }
    }
}
