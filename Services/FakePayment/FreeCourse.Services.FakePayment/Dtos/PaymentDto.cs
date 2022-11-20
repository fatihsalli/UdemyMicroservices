namespace FreeCourse.Services.FakePayment.Dtos
{
    public class PaymentDto
    {
        public string CardName { get; set; }
        public string CardNumber { get; set; }
        public string Expiration { get; set; }
        public string CVV { get; set; }
        public decimal TotalPrice { get; set; }

        //Asenkron iletişim için
        public OrderDto Order { get; set; }

    }
}
