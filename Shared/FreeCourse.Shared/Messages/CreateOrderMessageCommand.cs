using System;
using System.Collections.Generic;

namespace FreeCourse.Shared.Messages
{
    //FakePayment mesajı gönderen => mesajı alan Order. Command 'i oluşturuyoruz.
    public class CreateOrderMessageCommand
    {
        public CreateOrderMessageCommand()
        {
            OrderItems = new List<OrderItem>();
        }

        public string BuyerId { get; set; }
        public List<OrderItem> OrderItems { get; set; }

        //Address Dto ya karşılık olarak aşağıdaki propertyleri tanımladık.
        public string Province { get; set; }
        public string District { get; set; }
        public string Street { get; set; }
        public string ZipCode { get; set; }
        public string Line { get; set; }
    }

    //Order.Api deki OrderController bizden ne bekliyorsa onu oluşturuyoruz.
    public class OrderItem
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string PictureUrl { get; set; }
        public Decimal Price { get; set; }
    }


}
