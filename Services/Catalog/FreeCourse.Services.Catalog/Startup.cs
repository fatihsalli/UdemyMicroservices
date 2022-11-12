using FreeCourse.Services.Catalog.Services;
using FreeCourse.Services.Catalog.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

namespace FreeCourse.Services.Catalog
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public void ConfigureServices(IServiceCollection services)
        {
            //Jwt ile kimlik doðrulama için aþaðýdaki kodlarý tanýmladýk.
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                //Tokený kimin daðýttýðý bilgisini veriyoruz.
                options.Authority = Configuration["IdentityServerURL"];
                //Audience ý belirttik. Bu rastgele bir deðer deðil IdentityServer=>Config dosyasý üzerinden gelir.
                options.Audience = "resource_catalog";
                //Https bekleyeceði için onu belirttik
                options.RequireHttpsMetadata = false;
            });

            //ICategoryService-CategoryService
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<ICourseService, CourseService>();

            //Automapper ekledik. Aþaðýda belirtilen teknik ile þunu söylemek istiyoruz Startup'ýn yer aldýðý Assembly'de IProfileExpression ya da IProfileConfiguration'dan miras alan classlarý maplemeye dahil ediyor.
            services.AddAutoMapper(typeof(Startup));

            //appsettings'i okuyarak "DatabaseSettings" classýndaki propertyleri set ediyoruz.
            //Öncelikle appsettings deki datalarýmýzý "DatabaseSettings" e baðlamak için. Bunu yazdýktan sonra herhangi bir classýn constructorýnda IOptions<DatabaseSettings> options diyerek bu deðerleri okuyabiliriz. Biz bunun yerine direkt olarak bir interface üzerinden almak için iki aþaðýdaki kodu yazdýk.
            services.Configure<DatabaseSettings>(Configuration.GetSection("DatabaseSettings"));
            //==> Interface üzerinden almak için aþaðýdaki kodu yazdýk. Aþaðýdaki kodda IOptions ile DatabaseSettingsi tanýmladýk. Herhangi bir classýn contructorýnda IDatabaseSettings'i çaðýrdýðýmda bana DatabaseSettings appsettingsdeki ayarlar ile doldurulmuþ þekilde gelecektir.
            services.AddSingleton<IDatabaseSettings>(sp =>
            {
                return sp.GetRequiredService<IOptions<DatabaseSettings>>().Value;
            });

            //Tüm Controller üzerine [Authorize] yazmak yerine aþaðýdaki düzenlemeyi Startup tarafýnda yaptýk. AddController içerisine opt ile girerek düzenlemeyi yapýyoruz.
            services.AddControllers(opt=>
            {
                opt.Filters.Add(new AuthorizeFilter());
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "FreeCourse.Services.Catalog", Version = "v1" });
            });

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "FreeCourse.Services.Catalog v1"));
            }

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
