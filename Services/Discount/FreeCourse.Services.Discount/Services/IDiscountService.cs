using FreeCourse.Services.Discount.Dtos;
using FreeCourse.Shared.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FreeCourse.Services.Discount.Services
{
    public interface IDiscountService
    {
        Task<Response<List<DiscountDto>>> GetAll();
        Task<Response<DiscountDto>> GetById(int id);
        Task<Response<NoContent>> Save(DiscountDto discountDto);
        Task<Response<NoContent>> Update(DiscountDto discountDto);
        Task<Response<NoContent>> Delete(int id);
        Task<Response<DiscountDto>> GetByCodeAndUserId(string code,string userId);


    }
}
