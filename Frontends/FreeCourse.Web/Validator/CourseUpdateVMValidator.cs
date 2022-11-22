using FluentValidation;
using FreeCourse.Web.Models.Catalog;

namespace FreeCourse.Web.Validator
{
    public class CourseUpdateVMValidator : AbstractValidator<CourseUpdateVM>
    {
        public CourseUpdateVMValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required!");
            RuleFor(x => x.Description).NotEmpty().WithMessage("Description is required!");
            RuleFor(x => x.Feature.Duration).InclusiveBetween(1, int.MaxValue).WithMessage("Duration is invalid!");
            // 1234,55 => ScalePrecision(2,6)
            RuleFor(x => x.Price).NotEmpty().WithMessage("Price is required!").ScalePrecision(2, 6).WithMessage("Price is invalid format!");
        }

    }
}
