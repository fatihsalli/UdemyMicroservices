using FreeCourse.Web.Models.Order;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FreeCourse.Web.Services.Interfaces
{
    public interface IOrderService
    {
        /// <summary>
        /// Synchronous communication - The request will be made directly to the microservice
        /// </summary>
        /// <param name="checkoutInput"></param>
        /// <returns></returns>
        Task<OrderCreatedVM> CreateOrder(CheckoutInput checkoutInput);
        /// <summary>
        /// Asynchronous communication - The order information will be sent to RabbitMQ
        /// </summary>
        /// <param name="checkoutInput"></param>
        /// <returns></returns>
        Task SuspendOrder(CheckoutInput checkoutInput);

        Task<List<OrderVM>> GetOrder();


    }
}
