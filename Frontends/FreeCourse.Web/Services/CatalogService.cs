using FreeCourse.Shared.Dtos;
using FreeCourse.Web.Models;
using FreeCourse.Web.Models.Catalog;
using FreeCourse.Web.Services.Interfaces;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace FreeCourse.Web.Services
{
    public class CatalogService : ICatalogService
    {
        //İstek yapmak için
        private readonly HttpClient _httpClient;

        public CatalogService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> CreateCourseAsync(CourseCreateVM courseCreateVM)
        {
            var response = await _httpClient.PostAsJsonAsync<CourseCreateVM>("courses",courseCreateVM);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteCourseAsycn(string courseId)
        {
            var response = await _httpClient.DeleteAsync($"courses/{courseId}");
            return response.IsSuccessStatusCode;
        }

        public async Task<List<CategoryVM>> GetAllCategoryAsync()
        {
            //http:localhost:500/services/catalog/categories
            var response = await _httpClient.GetAsync("categories");

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var responseSuccess = await response.Content.ReadFromJsonAsync<Response<List<CategoryVM>>>();

            return responseSuccess.Data;
        }

        public async Task<List<CourseVM>> GetAllCourseAsync()
        {
            //http:localhost:500/services/catalog/courses
            var response = await _httpClient.GetAsync("courses");

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var responseSuccess=await response.Content.ReadFromJsonAsync<Response<List<CourseVM>>>();

            return responseSuccess.Data;
        }

        public async Task<List<CourseVM>> GetAllCourseByUserIdAsync(string userId)
        {
            //http:localhost:500/services/catalog/courses/GetAllByUserId/{userId}
            var response = await _httpClient.GetAsync($"courses/GetAllByUserId/{userId}");

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var responseSuccess = await response.Content.ReadFromJsonAsync<Response<List<CourseVM>>>();

            return responseSuccess.Data;
        }

        public async Task<CourseVM> GetByCourseIdAsycn(string courseId)
        {
            //http:localhost:500/services/catalog/courses/{courseId}
            var response = await _httpClient.GetAsync($"courses/{courseId}");

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var responseSuccess = await response.Content.ReadFromJsonAsync<Response<CourseVM>>();

            return responseSuccess.Data;
        }

        public async Task<bool> UpdateCourseAsync(CourseUpdateVM courseUpdateVM)
        {
            var response = await _httpClient.PutAsJsonAsync<CourseUpdateVM>("courses", courseUpdateVM);
            return response.IsSuccessStatusCode;
        }
    }
}
