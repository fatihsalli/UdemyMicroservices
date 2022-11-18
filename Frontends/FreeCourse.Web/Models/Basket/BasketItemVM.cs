namespace FreeCourse.Web.Models.Basket
{
    public class BasketItemVM
    {
        //Kurs 1 tane alınabileceği için 1'e set ettik. Eticaret olsaydı değiştirecektik.
        public int Quantity { get; set; } = 1;
        public string CourseId { get; set; }
        public string CourseName { get; set; }
        public decimal Price { get; set; }


        //İndirim kodu uygulandığında fiyatı tutmak için
        private decimal? DiscountAppliedPrice { get; set; }
        public decimal GetCurrentPrice
        {
            get => DiscountAppliedPrice != null ? DiscountAppliedPrice.Value : Price;
        }
        public void AppliedDiscount(decimal disCountPrice)
        {
            DiscountAppliedPrice = disCountPrice;
        }

    }
}
