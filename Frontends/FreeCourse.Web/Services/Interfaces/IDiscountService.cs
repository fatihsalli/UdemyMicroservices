using FreeCourse.Web.Models.Discount;
using System.Threading.Tasks;

namespace FreeCourse.Web.Services.Interfaces
{
    public interface IDiscountService
    {
        Task<DiscountVM> GetDiscount(string discountCode);

    }
}
