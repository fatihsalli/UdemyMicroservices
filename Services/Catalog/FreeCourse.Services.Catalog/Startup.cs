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
            //Jwt ile kimlik do�rulama i�in a�a��daki kodlar� tan�mlad�k.
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                //Token� kimin da��tt��� bilgisini veriyoruz.
                options.Authority = Configuration["IdentityServerURL"];
                //Audience � belirttik. Bu rastgele bir de�er de�il IdentityServer=>Config dosyas� �zerinden gelir.
                options.Audience = "resource_catalog";
                //Https bekleyece�i i�in onu belirttik
                options.RequireHttpsMetadata = false;
            });

            //ICategoryService-CategoryService
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<ICourseService, CourseService>();

            //Automapper ekledik. A�a��da belirtilen teknik ile �unu s�ylemek istiyoruz Startup'�n yer ald��� Assembly'de IProfileExpression ya da IProfileConfiguration'dan miras alan classlar� maplemeye dahil ediyor.
            services.AddAutoMapper(typeof(Startup));

            //appsettings'i okuyarak "DatabaseSettings" class�ndaki propertyleri set ediyoruz.
            //�ncelikle appsettings deki datalar�m�z� "DatabaseSettings" e ba�lamak i�in. Bunu yazd�ktan sonra herhangi bir class�n constructor�nda IOptions<DatabaseSettings> options diyerek bu de�erleri okuyabiliriz. Biz bunun yerine direkt olarak bir interface �zerinden almak i�in iki a�a��daki kodu yazd�k.
            services.Configure<DatabaseSettings>(Configuration.GetSection("DatabaseSettings"));
            //==> Interface �zerinden almak i�in a�a��daki kodu yazd�k. A�a��daki kodda IOptions ile DatabaseSettingsi tan�mlad�k. Herhangi bir class�n contructor�nda IDatabaseSettings'i �a��rd���mda bana DatabaseSettings appsettingsdeki ayarlar ile doldurulmu� �ekilde gelecektir.
            services.AddSingleton<IDatabaseSettings>(sp =>
            {
                return sp.GetRequiredService<IOptions<DatabaseSettings>>().Value;
            });

            //T�m Controller �zerine [Authorize] yazmak yerine a�a��daki d�zenlemeyi Startup taraf�nda yapt�k. AddController i�erisine opt ile girerek d�zenlemeyi yap�yoruz.
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
