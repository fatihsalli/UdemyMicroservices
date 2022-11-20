using FreeCourse.Shared.Dtos;
using FreeCourse.Shared.Services;
using FreeCourse.Web.Models.Order;
using FreeCourse.Web.Models.Payments;
using FreeCourse.Web.Services.Interfaces;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace FreeCourse.Web.Services
{
    public class OrderService : IOrderService
    {
        private readonly HttpClient _httpClient;
        private readonly IPaymentService _paymentService;
        private readonly IBasketService _basketService;
        private readonly ISharedIdentityService _sharedIdentityService;

        public OrderService(HttpClient httpClient, IPaymentService paymentService, IBasketService basketService, ISharedIdentityService sharedIdentityService)
        {
            _httpClient = httpClient;
            _paymentService = paymentService;
            _basketService = basketService;
            _sharedIdentityService = sharedIdentityService;
        }

        public async Task<OrderCreatedVM> CreateOrder(CheckoutInput checkoutInput)
        {
            var basket = await _basketService.Get();

            var payment = new PaymentInfoInput()
            {
                CardName = checkoutInput.CardName,
                CardNumber = checkoutInput.CardNumber,
                Expiration = checkoutInput.Expiration,
                CVV = checkoutInput.CVV,
                TotalPrice = basket.TotalPrice
            };

            var responsePayment = await _paymentService.ReceivePayment(payment);

            if (!responsePayment)
            {
                return new OrderCreatedVM { Error = "Payment is failed", IsSuccessful = false };
            }

            //Gelen data üzerinden "OrderCreateInput" oluşturduk.
            var orderCreateInput = new OrderCreateInput()
            {
                BuyerId = _sharedIdentityService.GetUserId,
                Address = new AddressCreateInput
                {
                    Province = checkoutInput.Province,
                    District = checkoutInput.District,
                    Line = checkoutInput.Line,
                    Street = checkoutInput.Street,
                    ZipCode = checkoutInput.ZipCode
                }
            };

            //"OrderCreateInput"a basketitemlerini ekledik.
            basket.BasketItems.ForEach(x =>
            {
                var orderItem = new OrderItemCreateInput
                {
                    ProductId = x.CourseId,
                    Price=x.GetCurrentPrice,
                    //Catalog servis üzerinden alabiliriz. Önemli olmadığı için böyle bıraktık.
                    PictureUrl="",
                    ProductName=x.CourseName
                };
                orderCreateInput.OrderItems.Add(orderItem);
            });

            var response = await _httpClient.PostAsJsonAsync<OrderCreateInput>("orders", orderCreateInput);

            if (!response.IsSuccessStatusCode)
            {
                return new OrderCreatedVM { Error = "Payment failed", IsSuccessful = false };
            }

            var responseSuccess = await response.Content.ReadFromJsonAsync<Response<OrderCreatedVM>>();

            //sepeti boşaltmak için
            await _basketService.Delete();

            return new OrderCreatedVM { OrderId=responseSuccess.Data.OrderId,Error =null, IsSuccessful = true };
        }

        public async Task<List<OrderVM>> GetOrder()
        {
            var response = await _httpClient.GetFromJsonAsync<Response<List<OrderVM>>>("orders");
            return response.Data;            
        }

        //Asenkron iletişim
        public async Task<OrderSuspendVM> SuspendOrder(CheckoutInput checkoutInput)
        {
            var basket = await _basketService.Get();

            //Gelen data üzerinden "OrderCreateInput" oluşturduk.
            var orderCreateInput = new OrderCreateInput()
            {
                BuyerId = _sharedIdentityService.GetUserId,
                Address = new AddressCreateInput
                {
                    Province = checkoutInput.Province,
                    District = checkoutInput.District,
                    Line = checkoutInput.Line,
                    Street = checkoutInput.Street,
                    ZipCode = checkoutInput.ZipCode
                }
            };

            //"OrderCreateInput"a basketitemlerini ekledik.
            basket.BasketItems.ForEach(x =>
            {
                var orderItem = new OrderItemCreateInput
                {
                    ProductId = x.CourseId,
                    Price = x.GetCurrentPrice,
                    //Catalog servis üzerinden alabiliriz. Önemli olmadığı için böyle bıraktık.
                    PictureUrl = "",
                    ProductName = x.CourseName
                };
                orderCreateInput.OrderItems.Add(orderItem);
            });

            var payment = new PaymentInfoInput()
            {
                CardName = checkoutInput.CardName,
                CardNumber = checkoutInput.CardNumber,
                Expiration = checkoutInput.Expiration,
                CVV = checkoutInput.CVV,
                TotalPrice = basket.TotalPrice,
                Order=orderCreateInput
            };

            var responsePayment = await _paymentService.ReceivePayment(payment);

            if (!responsePayment)
            {
                return new OrderSuspendVM { Error = "Payment is failed", IsSuccessful = false };
            }

            //sepeti boşaltmak için
            await _basketService.Delete();

            return new OrderSuspendVM { IsSuccessful= true};
        }
    }
}
