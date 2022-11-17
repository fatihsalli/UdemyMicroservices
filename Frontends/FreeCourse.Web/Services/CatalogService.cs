using FreeCourse.Shared.Dtos;
using FreeCourse.Web.Helpers;
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
        private readonly IPhotoStockService _photoStockService;
        private readonly PhotoHelper _photoHelper;

        public CatalogService(HttpClient httpClient, IPhotoStockService photoStockService,PhotoHelper photoHelper)
        {
            _httpClient = httpClient;
            _photoStockService = photoStockService; 
            _photoHelper = photoHelper;
        }

        public async Task<bool> CreateCourseAsync(CourseCreateVM courseCreateVM)
        {
            var resultPhotoService = await _photoStockService.UploadPhoto(courseCreateVM.PhotoFormFile);

            if (resultPhotoService!=null)
            {
                courseCreateVM.Picture = resultPhotoService.Url;
            }

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

            //Fotoğrafları Url olarak ekliyoruz. PhotoStockdan istek yapacak şekilde.
            responseSuccess.Data.ForEach(x =>
            {
                x.StockPictureUrl = _photoHelper.GetPhotoStockUrl(x.Picture);
            });

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

            //Fotoğrafları Url olarak ekliyoruz. PhotoStockdan istek yapacak şekilde.
            responseSuccess.Data.ForEach(x =>
            {
                x.StockPictureUrl = _photoHelper.GetPhotoStockUrl(x.Picture);
            });

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

            responseSuccess.Data.StockPictureUrl = _photoHelper.GetPhotoStockUrl(responseSuccess.Data.Picture);

            return responseSuccess.Data;
        }

        // TODO: Fotoğrafı silerken hata alıyoruz.
        public async Task<bool> UpdateCourseAsync(CourseUpdateVM courseUpdateVM)
        {
            var resultPhotoService = await _photoStockService.UploadPhoto(courseUpdateVM.PhotoFormFile);

            if (resultPhotoService != null)
            {
                //eskisini silmek için
                await _photoStockService.DeletePhoto(courseUpdateVM.Picture);
                courseUpdateVM.Picture = resultPhotoService.Url;
            }

            var response = await _httpClient.PutAsJsonAsync<CourseUpdateVM>("courses", courseUpdateVM);
            return response.IsSuccessStatusCode;
        }
    }
}
