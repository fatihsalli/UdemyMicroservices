using System.Collections.Generic;
using System;

namespace FreeCourse.Web.Models.Order
{
    public class OrderVM
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }

        //Ödeme geçmişinde adres alanına ihtiyaç olmadığı için alınmadı
        //public AddressDto Address { get; set; }
        public string BuyerId { get; set; }
        public List<OrderItemVM> OrderItems { get; set; }

    }
}
