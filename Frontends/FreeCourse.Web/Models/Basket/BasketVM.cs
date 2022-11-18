using System;
using System.Collections.Generic;
using System.Linq;

namespace FreeCourse.Web.Models.Basket
{
    public class BasketVM
    {
        public BasketVM()
        {
            _basketItems = new List<BasketItemVM>();
        }

        public string UserId { get; set; }
        public string DiscountCode { get; set; }

        //İndirim oranı için
        public int? DiscountRate { get; set; }

        private List<BasketItemVM> _basketItems;

        //İndirimi fiyatlara uyguluyoruz.
        public List<BasketItemVM> BasketItems 
        { 
            get 
            {
                if (HasDiscount)
                {
                    //Örnek kurs fiyat 100 TL indirim oranı %10
                    _basketItems.ForEach(x =>
                    {
                        var discountPrice = x.Price * ((decimal)DiscountRate.Value / 100); //10 TL gelecek
                        x.AppliedDiscount(Math.Round(x.Price - discountPrice, 2)); //Kursun indirimli fiyatı
                    });
                }
                return _basketItems;
            }
            set
            {
                _basketItems = value;
            }
        }

        public decimal TotalPrice
        {
            //Çarpıp toplayarak tüm miktarı verecek hazır bir metot.
            get => _basketItems.Sum(x => x.GetCurrentPrice * x.Quantity);
        }

        //Kupon kodu var mı yok mu öğrenmek için yardımcı metot tanımladık.
        public bool HasDiscount
        {
            get => !string.IsNullOrEmpty(DiscountCode);
        }


    }
}
