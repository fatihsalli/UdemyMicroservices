using System.ComponentModel.DataAnnotations;

namespace FreeCourse.Web.Models.Order
{
    public class CheckoutInput
    {
        [Display(Name ="Province")]
        public string Province { get; set; }

        [Display(Name = "District")]
        public string District { get; set; }

        [Display(Name = "Street")]
        public string Street { get; set; }

        [Display(Name = "ZipCode")]
        public string ZipCode { get; set; }

        [Display(Name = "Line")]
        public string Line { get; set; }


        //Kart bilgileri => Payment

        [Display(Name = "CardName")]
        public string CardName { get; set; }

        [Display(Name = "CardNumber")]
        public string CardNumber { get; set; }

        [Display(Name = "Expiration")]
        public string Expiration { get; set; }

        [Display(Name = "CVV/CVC2")]
        public string CVV { get; set; }
    }
}
