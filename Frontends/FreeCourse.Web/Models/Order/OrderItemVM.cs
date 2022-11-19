using System;

namespace FreeCourse.Web.Models.Order
{
    public class OrderItemVM
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string PictureUrl { get; set; }
        public Decimal Price { get; set; }

    }
}
