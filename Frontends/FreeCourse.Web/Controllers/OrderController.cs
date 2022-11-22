using FreeCourse.Web.Models.Order;
using FreeCourse.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace FreeCourse.Web.Controllers
{
    public class OrderController : Controller
    {
        private readonly IBasketService _basketService;
        private readonly IOrderService _orderService;
        public OrderController(IBasketService basketService, IOrderService orderService)
        {
            _basketService = basketService;
            _orderService = orderService;
        }

        public async Task<IActionResult> Checkout()
        {
            var basket = await _basketService.Get();
            ViewBag.Basket = basket;

            return View(new CheckoutInput());
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(CheckoutInput checkoutInput)
        {
            //Senkron iletişim (1.YOL)
            //var orderStatus = await _orderService.CreateOrder(checkoutInput);

            //Asenkron İletişim (2.YOL)
            var orderSuspend = await _orderService.SuspendOrder(checkoutInput);

            if (!orderSuspend.IsSuccessful)
            {
                var basket = await _basketService.Get();
                ViewBag.Basket = basket;

                ViewBag.Error = orderSuspend.Error;

                return View();
            }

            //Senkron iletişim (1.YOL)
            //return RedirectToAction(nameof(SuccessfulCheckout), new {orderId= orderSuspend.OrderId});

            //Asenkron İletişim (2.YOL)
            //Fakepayment da id dönmediğimiz için random değer atadık değiştirilebilir.
            return RedirectToAction(nameof(SuccessfulCheckout), new { orderId = new Random().Next(1, 1000) });
        }

        public IActionResult SuccessfulCheckout(int orderId)
        {
            ViewBag.OrderId = orderId;
            return View();
        }

        public async Task<IActionResult> CheckoutHistory()
        {
            return View(await _orderService.GetOrder());

        }


    }
}
