using FreeCourse.Web.Models.Basket;
using FreeCourse.Web.Services.Interfaces;
using System.Threading.Tasks;

namespace FreeCourse.Web.Services
{
    public class BasketService : IBasketService
    {
        public Task AddBasketItem(BasketItemVM basketItemVM)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> ApplyDiscount(string discountCode)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> CancelApplyDiscount()
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> Delete()
        {
            throw new System.NotImplementedException();
        }

        public Task<BasketVM> Get()
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> RemoveBasketItem(string courseId)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> SaveOrUpdate(BasketVM basketVM)
        {
            throw new System.NotImplementedException();
        }
    }
}
