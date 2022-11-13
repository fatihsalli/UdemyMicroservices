using FreeCourse.Services.PhotoStock.Dtos;
using FreeCourse.Shared.ControllerBases;
using FreeCourse.Shared.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FreeCourse.Services.PhotoStock.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhotosController : CustomBaseController
    {
        //Cancellation tokenı neden aldık? Buraya bir fotoğraf geldiğinde örneğin 20 sn sürüyor diyelim isteği gönderen işlem tamamlanmadan iptal ederse işlem devam etmesin sonlansın diye. Asenkron başlayan bir işlemi hata fırlatarak sonlandırabilirsiniz. Cancellation Token da hata fırlatarak işlemi sonlandırır.
        [HttpPost]
        public async Task<IActionResult> PhotoSave(IFormFile photo, CancellationToken cancellationToken)
        {
            if (photo != null && photo.Length > 0)
            {
                //Path ini oluşturuyoruz.
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/photos", photo.FileName);

                using var stream = new FileStream(path, FileMode.Create);
                //Photo yu kaydederken tarayıcı kapanır veya işlem sonlanırsa diye cancellation tokenı da veriyoruz. (İşlemi sonlandırması için)
                await photo.CopyToAsync(stream, cancellationToken);
                //İsteği yapana pathi dönüyoruz. //http://www.photostock.api.com/photos/imagesbackground.jpg
                var returnPath = "photos/" + photo.FileName;
                //var photoDto= new PhotoDto { Url= returnPath };
                PhotoDto photoDto = new() { Url = returnPath };

                return CreateActionResultInstance(Response<PhotoDto>.Success(photoDto, 200));
            }
            return CreateActionResultInstance(Response<NoContent>.Fail("Photo is empty!", 400));
        }

        [HttpDelete]
        public IActionResult PhotoDelete(string photoUrl)
        {
            //Buradaki ifade yan yana aşağıda verdiğimiz pathleri birleştirir.
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/photos", photoUrl);

            //Path var mı yok mu kontrol ediyoruz.
            if (!System.IO.File.Exists(path))
            {
                return CreateActionResultInstance(Response<NoContent>.Fail("Photo is not found!", 404));
            }
            //Var ise siliyoruz.
            System.IO.File.Delete(path);

            return CreateActionResultInstance(Response<NoContent>.Success(204));
        }



    }
}
