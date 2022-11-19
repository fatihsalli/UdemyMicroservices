using FreeCourse.Web.Models.Order;
using FreeCourse.Web.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FreeCourse.Web.Services
{
    public class OrderService : IOrderService
    {
        public Task<OrderCreatedVM> CreateOrder(CheckoutInput checkoutInput)
        {
            throw new System.NotImplementedException();
        }

        public Task<List<OrderVM>> GetOrder()
        {
            throw new System.NotImplementedException();
        }

        public Task SuspendOrder(CheckoutInput checkoutInput)
        {
            throw new System.NotImplementedException();
        }
    }
}
