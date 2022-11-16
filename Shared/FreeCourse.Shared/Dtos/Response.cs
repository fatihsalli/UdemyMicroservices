using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace FreeCourse.Shared.Dtos
{
    public class Response<T>
    {
        //Client'a response olarak tek bir model dönüyoruz. Successdto ve Errordto olarak ikiye ayırmadık. Tek model dönüyoruz. Json tarafında deserialize ederken hata aldık o sebeple private ları kaldırdık.

        public T Data { get; set; }

        //Bir API'ya yani bir endpointe istek yaptığımzda response da zaten status code oluyor. Ama yazılım içerisinde ihtiyacımız olduğu için bu şekilde [JsonIgnore] ile tanımladık.
        [JsonIgnore]
        public int StatusCode { get; set; }

        //Yazılım içerisinde kullanmak için tanımladık Response Body'sinde görünmesini istemediğimiz için [JsonIgnore] dedik.
        [JsonIgnore]
        public bool IsSuccessful { get; set; }
        public List<string> Errors { get; set; }

        //Static factory metotlar ile nesne türetiyoruz.
        public static Response<T> Success(T data, int statusCode)
        {
            return new Response<T> { Data = data, StatusCode = statusCode, IsSuccessful = true };
        }
        //Başarılı ama data dönmeyen metotumuz. Update,delete gibi.
        public static Response<T> Success(int statusCode)
        {
            return new Response<T> { Data = default(T), StatusCode = statusCode, IsSuccessful = true };
        }
        //Fail olma durumunda birden çok hata gelmesi halinde
        public static Response<T> Fail(List<string> errors, int statusCode)
        {
            return new Response<T> { Errors = errors, StatusCode = statusCode, IsSuccessful = false };
        }
        //Fail olma durumunda bir hata gelmesi halinde
        public static Response<T> Fail(string error, int statusCode)
        {
            return new Response<T> { Errors = new List<string>() { error }, StatusCode = statusCode, IsSuccessful = false };
        }

    }
}
