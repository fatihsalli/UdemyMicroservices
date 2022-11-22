using FreeCourse.Web.Exceptions;
using FreeCourse.Web.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace FreeCourse.Web.Handler
{
    public class ResourceOwnerPasswordTokenHandler : DelegatingHandler
    {
        private readonly IIdentityService _identityService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ResourceOwnerPasswordTokenHandler> _logger;
        public ResourceOwnerPasswordTokenHandler(IIdentityService identityService, IHttpContextAccessor httpContextAccessor, ILogger<ResourceOwnerPasswordTokenHandler> logger)
        {
            _identityService = identityService;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            //Access tokenı cookiden okuduk
            var accessToken = await _httpContextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);

            //Requestın headırına ekledik
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var response = await base.SendAsync(request, cancellationToken);

            //401 gelmesi durumunda Access token ömrünü kontrol ediyoruz.
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                //Elimdeki refresh token ile access token almayı deniyorum.
                var tokenResponse = await _identityService.GetAccessTokenByRefreshToken();

                //Alabilirsem tekrar gönderiyorum alamadıysam buraya girmeyecek
                if (tokenResponse != null)
                {
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);
                    //Aynı isteği tekrar gönderiyoruz.
                    response = await base.SendAsync(request, cancellationToken);
                }
            }

            //Yukarıdaki if bloğunda kontrol ettik kullanıcı response tokenı geçersiz olduğu için access token alamadığında buraya girecek ve kullanıcı login ekranına göndereceğiz. Bu sınıftan başka bir actiona gönderemeyeceğimiz için hata fırlatacağız. Bu hatayı middleware ile yakalayıp oradan yönlendireceğiz. "UnAuthorizeException" ile.
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new UnAuthorizeException();
            }

            return response;
        }

    }
}
