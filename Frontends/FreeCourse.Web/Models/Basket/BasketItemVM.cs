namespace FreeCourse.Web.Models.Basket
{
    public class BasketItemVM
    {
        public int Quantity { get; set; }
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
