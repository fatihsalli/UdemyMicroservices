using System;
using System.Collections.Generic;

namespace FreeCourse.Services.FakePayment.Dtos
{
    //Asenkron haberleşmek için mesajı FakePayment göndereceği için burada OrderDto oluşturduk.
    public class OrderDto
    {
        public OrderDto()
        {
            OrderItems = new List<OrderItemDto>();
        }

        //Order microservisinin siparişi kaydetmek için istediği modeli oluşturduk."CreateOrderCommand"
        public string BuyerId { get; set; }
        public List<OrderItemDto> OrderItems { get; set; }
        public AddressDto Address { get; set; }

    }

    public class AddressDto
    {
        public string Province { get; set; }
        public string District { get; set; }
        public string Street { get; set; }
        public string ZipCode { get; set; }
        public string Line { get; set; }
    }

    public class OrderItemDto
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string PictureUrl { get; set; }
        public Decimal Price { get; set; }

    }



}
