using FreeCourse.Shared.Dtos;
using FreeCourse.Shared.Services;
using FreeCourse.Web.Models.Basket;
using FreeCourse.Web.Services.Interfaces;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace FreeCourse.Web.Services
{
    public class BasketService : IBasketService
    {
        private readonly HttpClient _httpClient;
        private readonly IDiscountService _discountService;
        public BasketService(HttpClient httpClient, IDiscountService discountService)
        {
            _httpClient = httpClient;
            _discountService = discountService;
        }

        public async Task AddBasketItem(BasketItemVM basketItemVM)
        {
            var basketVM = await Get();

            if (basketVM != null)
            {
                if (!basketVM.BasketItems.Any(x=> x.CourseId==basketItemVM.CourseId))
                {
                    basketVM.BasketItems.Add(basketItemVM);
                }
            }
            else //sepette basket oluşmamış olma durumu (ilk defa ekleme)
            {
                basketVM = new BasketVM();
                basketVM.BasketItems.Add(basketItemVM);
            }
            await SaveOrUpdate(basketVM);
            return;
        }

        //Discount service den sonra
        public async Task<bool> ApplyDiscount(string discountCode)
        {
            await CancelApplyDiscount();

            var basket = await Get();

            if (basket == null)
            {
                return false;
            }

            var hasDiscount=await _discountService.GetDiscount(discountCode);

            if (hasDiscount==null)
            {
                return false;
            }

            basket.ApplyDiscount(hasDiscount.DiscountCode, hasDiscount.Rate);
            await SaveOrUpdate(basket);
            return true;
        }

        //Discount service den sonra
        public async Task<bool> CancelApplyDiscount()
        {
            var basket = await Get();

            if (basket==null || basket.DiscountCode==null)
            {
                return false;
            }

            basket.CancelDiscount();
            await SaveOrUpdate(basket);
            return true;
        }

        public async Task<bool> Delete()
        {
            var response = await _httpClient.DeleteAsync("baskets");

            return response.IsSuccessStatusCode;
        }

        public async Task<BasketVM> Get()
        {
            //Sepet dolu mu değil mi onun için datayı çekiyoruz.
            var response = await _httpClient.GetAsync("baskets");

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var basketVM = await response.Content.ReadFromJsonAsync<Response<BasketVM>>();
            return basketVM.Data;
        }

        public async Task<bool> RemoveBasketItem(string courseId)
        {
            var basketVM = await Get();

            if (basketVM==null)
            {
                return false;
            }
            var deleteBasketItem = basketVM.BasketItems.FirstOrDefault(x => x.CourseId == courseId);

            if (deleteBasketItem == null) return false;

            var deleteResult=basketVM.BasketItems.Remove(deleteBasketItem);

            if (!deleteResult)
            {
                return false;
            }

            //Sepetteki son ürünü silme durumunda indirimi null'a çekmek için
            if (!basketVM.BasketItems.Any())
            {
                basketVM.DiscountCode=null;
            }
            return await SaveOrUpdate(basketVM);
        }

        public async Task<bool> SaveOrUpdate(BasketVM basketVM)
        {
            var response = await _httpClient.PostAsJsonAsync<BasketVM>("baskets",basketVM);

            return response.IsSuccessStatusCode;
        }
    }
}
