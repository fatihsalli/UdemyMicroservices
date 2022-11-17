using FreeCourse.Shared.Dtos;
using FreeCourse.Web.Models.PhotoStock;
using FreeCourse.Web.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace FreeCourse.Web.Services
{
    public class PhotoStockService : IPhotoStockService
    {
        private readonly HttpClient _httpClient;
        public PhotoStockService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> DeletePhoto(string photoUrl)
        {
            //Path içinde nokta olduğu için bu şekilde gösterdik.
            var response= await _httpClient.DeleteAsync($"photos?photoUrl={photoUrl}");
            return response.IsSuccessStatusCode;
        }

        public async Task<PhotoStockVM> UploadPhoto(IFormFile photo)
        {
            if (photo==null || photo.Length<=0)
            {
                return null;
            }
            // => 05105050adwad0a6f50a6f.jpg
            var randomFilename=$"{Guid.NewGuid().ToString()}{Path.GetExtension(photo.FileName)}";

            using var ms=new MemoryStream();
            await photo.CopyToAsync(ms);

            var multipartContent = new MultipartFormDataContent();
            //photo ismini PhotoStock.Api beklediği için bu ismi verdik. Save metotunda.
            multipartContent.Add(new ByteArrayContent(ms.ToArray()),"photo",randomFilename);

            //Base Url startup tarafından gelecek
            var response = await _httpClient.PostAsync("photos", multipartContent);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var responseSuccess= await response.Content.ReadFromJsonAsync<Response<PhotoStockVM>>();

            return responseSuccess.Data;
        }
    }
}
