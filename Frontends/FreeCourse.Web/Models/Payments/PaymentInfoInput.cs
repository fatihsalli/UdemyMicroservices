﻿using FreeCourse.Web.Models.Order;

namespace FreeCourse.Web.Models.Payments
{
    public class PaymentInfoInput
    {
        public string CardName { get; set; }
        public string CardNumber { get; set; }
        public string Expiration { get; set; }
        public string CVV { get; set; }
        public decimal TotalPrice { get; set; }

        //Asenkron iletişim için
        public OrderCreateInput Order { get; set; }




    }
}
