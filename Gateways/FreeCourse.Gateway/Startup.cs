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

            //Gateway.API JWT token ile koruma alt�na ald�k.
            services.AddAuthentication().AddJwtBearer("GatewayAuthenticationScheme",options=>
            {
                //Token� kimin da��tt��� bilgisini veriyoruz.
                options.Authority = Configuration["IdentityServerURL"];
                //Audience � belirttik. Bu rastgele bir de�er de�il IdentityServer=>Config dosyas� �zerinden gelir.
                options.Audience = "resource_gateway";
                //Https bekleyece�i i�in onu belirttik
                options.RequireHttpsMetadata = false;
            });

            //Ocelot k�t�phanesini ekledik. ve Delegate ekledik token exhange i�in
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
