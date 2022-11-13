using Microsoft.AspNetCore.Http;

namespace FreeCourse.Shared.Services
{
    public class SharedIdentityService : ISharedIdentityService
    {
        //HttpContext üzerinden userId yi alabilmemiz için aşağıdaki interface'i kullandık. "IHttpContextAccessor" aslında uygulamanın kalbi bu interface ile hem response hem de request e erişebilirim.
        private IHttpContextAccessor _httpContextAccessor;

        public SharedIdentityService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        //Token üzerinden çıkarttığımız userId ye IHttpContextAccessor üzerinden eriştik.
        public string GetUserId => _httpContextAccessor.HttpContext.User.FindFirst("sub").Value;


    }
}
