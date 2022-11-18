using System.Collections.Generic;
using System.Linq;

namespace FreeCourse.Services.Basket.Dtos
{
    public class BasketDto
    {
        public string UserId { get; set; }
        public string DiscountCode { get; set; }

        //İndirim oranı için
        public int? DiscountRate { get; set; }
        public List<BasketItemDto> BasketItems { get; set; }
        public decimal TotalPrice
        {
            //Çarpıp toplayarak tüm miktarı verecek hazır bir metot.
            get => BasketItems.Sum(x => x.Price * x.Quantity);
        }



    }
}
