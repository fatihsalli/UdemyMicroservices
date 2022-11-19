using FreeCourse.Web.Handler;
using FreeCourse.Web.Models;
using FreeCourse.Web.Services.Interfaces;
using FreeCourse.Web.Services;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.Extensions.Configuration;

namespace FreeCourse.Web.Extensions
{
    public static class ServiceExtension
    {
        public static void AddHttpClientServices(this IServiceCollection services,IConfiguration Configuration)
        {
            //ServiceApiSettings classına ulaştık.
            var serviceApiSettings = Configuration.GetSection("ServiceApiSettings").Get<ServiceApiSettings>();

            //HttpClient kullandığımız için ekledik.BaseUrl i "discovery üzerinden kendimiz alıyoruz."
            services.AddHttpClient<IClientCredentialTokenService, ClientCredentialTokenService>();

            //Basket.Api => HttpClient kullandığımız için ekledik. Kullanıcı olduğu için "ResourceOwnerPasswordTokenHandler" kullandık.
            services.AddHttpClient<IBasketService, BasketService>(opt =>
            {
                opt.BaseAddress = new Uri($"{serviceApiSettings.GatewayBaseUri}/{serviceApiSettings.Basket.Path}");
            }).AddHttpMessageHandler<ResourceOwnerPasswordTokenHandler>();

            //Catalog.Api için nesne türetip bir de pathi belirliyoruz."ClientCredentialTokenHandler" ekliyoruz.
            //Buradaki işlem=> Catalog Service içerisinde HttpClient kullanıldığında baseaddress'e gidecek."ClientCredentialTokenHandler" gelen isteği bölecek memorydeki tokenı ekleyip gönderecek memory de yoksa sıfırdan tokenı ekleyip memory'ye ekleyecek. Sonrasında request headerına tokenımızı ekleyecek.
            services.AddHttpClient<ICatalogService, CatalogService>(opt =>
            {
                opt.BaseAddress = new Uri($"{serviceApiSettings.GatewayBaseUri}/{serviceApiSettings.Catalog.Path}");
            }).AddHttpMessageHandler<ClientCredentialTokenHandler>();

            //PhotoStock.Api için HttpClient kullanacağımızdan burada DI Containera ekliyoruz. Client bazlı olduğu için "ClientCredentialTokenHandler" ekledik.
            services.AddHttpClient<IPhotoStockService, PhotoStockService>(opt =>
            {
                opt.BaseAddress = new Uri($"{serviceApiSettings.GatewayBaseUri}/{serviceApiSettings.PhotoStock.Path}");
            }).AddHttpMessageHandler<ClientCredentialTokenHandler>();

            //IdentityService de uygulama bize uygun bir HttpClient dönsün diye yazdık.
            services.AddHttpClient<IIdentityService, IdentityService>();
            //UserService de uygulama bize uygun bir HttpClient dönsün diye yazdık. Exception ekledik burada da belirtiyoruz => "AddHttpMessageHandler<ResourceOwnerPasswordTokenHandler>();"
            services.AddHttpClient<IUserService, UserService>(opt =>
            {
                opt.BaseAddress = new Uri(serviceApiSettings.IdentityBaseUri);
            }).AddHttpMessageHandler<ResourceOwnerPasswordTokenHandler>();

            //Discount.Api => HttpClient kullandığımız için ekledik. Kullanıcı olduğu için "ResourceOwnerPasswordTokenHandler" kullandık.
            services.AddHttpClient<IDiscountService, DiscountService>(opt =>
            {
                opt.BaseAddress = new Uri($"{serviceApiSettings.GatewayBaseUri}/{serviceApiSettings.Discount.Path}");
            }).AddHttpMessageHandler<ResourceOwnerPasswordTokenHandler>();

            //FakePayment.Api => HttpClient kullandığımız için ekledik. Kullanıcı olduğu için "ResourceOwnerPasswordTokenHandler" kullandık.
            services.AddHttpClient<IPaymentService, PaymentService>(opt =>
            {
                opt.BaseAddress = new Uri($"{serviceApiSettings.GatewayBaseUri}/{serviceApiSettings.Payment.Path}");
            }).AddHttpMessageHandler<ResourceOwnerPasswordTokenHandler>();

            //Order.Api => HttpClient kullandığımız için ekledik. Kullanıcı olduğu için "ResourceOwnerPasswordTokenHandler" kullandık.
            services.AddHttpClient<IOrderService, OrderService>(opt =>
            {
                opt.BaseAddress = new Uri($"{serviceApiSettings.GatewayBaseUri}/{serviceApiSettings.Order.Path}");
            }).AddHttpMessageHandler<ResourceOwnerPasswordTokenHandler>();



        }
    }
}
