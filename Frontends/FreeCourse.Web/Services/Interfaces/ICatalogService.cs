using FreeCourse.Web.Models.Catalog;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FreeCourse.Web.Services.Interfaces
{
    //Catalog.Api ile iletişimi servis üzerinden sağlayacağız.
    public interface ICatalogService
    {
        //Bize bir Json formatında data gelecek. Onlar için ViewModellar oluşturduk.
        Task<List<CourseVM>> GetAllCourseAsync();
        Task<List<CategoryVM>> GetAllCategoryAsync();
        Task<List<CourseVM>> GetAllCourseByUserIdAsync(string userId);
        Task<CourseVM> GetByCourseIdAsycn(string courseId);
        Task<bool> CreateCourseAsync(CourseCreateVM courseCreateVM);
        Task<bool> UpdateCourseAsync(CourseUpdateVM courseUpdateVM);
        Task<bool> DeleteCourseAsycn(string courseId);

    }
}
