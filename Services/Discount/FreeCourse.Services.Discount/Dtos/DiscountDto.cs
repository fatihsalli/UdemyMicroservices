using System;

namespace FreeCourse.Services.Discount.Dtos
{
    public class DiscountDto
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int Rate { get; set; }
        public string DiscountCode { get; set; }
        public DateTime CreatedTime { get; set; }


    }
}
