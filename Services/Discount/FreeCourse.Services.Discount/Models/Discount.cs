﻿using System;

namespace FreeCourse.Services.Discount.Models
{
    //Mapleme işlemini gerçekleştirdik.
    [Dapper.Contrib.Extensions.Table("discount")]
    public class Discount
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int Rate { get; set; }
        public string DiscountCode { get; set; }
        public DateTime CreatedTime { get; set; }



    }
}
