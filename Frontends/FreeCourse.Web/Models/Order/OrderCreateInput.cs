using System.Collections.Generic;

namespace FreeCourse.Web.Models.Order
{
    public class OrderCreateInput
    {
        public OrderCreateInput()
        {
            OrderItems = new List<OrderItemCreateInput>();
        }

        //Order microservisinin siparişi kaydetmek için istediği modeli oluşturduk."CreateOrderCommand"
        public string BuyerId { get; set; }
        public List<OrderItemCreateInput> OrderItems { get; set; }
        public AddressCreateInput Address { get; set; }


    }
}
