using FluentValidation;
using FreeCourse.Web.Models.Discount;

namespace FreeCourse.Web.Validator
{
    public class DiscountApplyVMValidator:AbstractValidator<DiscountApplyVM>
    {
        public DiscountApplyVMValidator()
        {
            RuleFor(x=> x.DiscountCode).NotEmpty().WithMessage("Coupon Code is required!");
        }


    }
}
