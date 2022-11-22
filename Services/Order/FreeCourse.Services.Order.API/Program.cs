using FreeCourse.Services.Order.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FreeCourse.Services.Order.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            //Ayağa kalktığında migration işlemlerinin database'e yansıması için yaptık yoksa hata alırız. Ya da conatinerı her yeniden yüklediğimizde update-database dememiz gerekir Önce migration yapıyor yok ise sonrasında database 'i update ediyor.
            using (var scope = host.Services.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                var orderDbContext = serviceProvider.GetRequiredService<OrderDbContext>();
                orderDbContext.Database.Migrate();
            }
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
