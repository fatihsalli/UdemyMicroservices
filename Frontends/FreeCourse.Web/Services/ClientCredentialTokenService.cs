using FreeCourse.Web.Models;
using FreeCourse.Web.Services.Interfaces;
using IdentityModel.AspNetCore.AccessTokenManagement;
using IdentityModel.Client;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Threading.Tasks;

namespace FreeCourse.Web.Services
{
    //Identity tarafında token işlemi için -> client için
    public class ClientCredentialTokenService : IClientCredentialTokenService
    {
        //Url leri tuttuğumuz class üzerinden
        private readonly ServiceApiSettings _serviceApiSettings;

        //clientları tuttuğumuz class üzerinden
        private readonly ClientSettings _clientSettings;

        //"IdentityModel.AspNetCore" modelden gelen IClientAccessTokenCache implemente ediyoruz.
        private readonly IClientAccessTokenCache _clientAccessTokenCache;
        private readonly HttpClient _httpClient;

        //startup tarafında belirtmediğimiz için burada IOptions pattern ile geçiyoruz.
        public ClientCredentialTokenService(IOptions<ServiceApiSettings> serviceApiSettings, IOptions<ClientSettings> clientSettings, IClientAccessTokenCache clientAccessTokenCache, HttpClient httpClient)
        {
            _serviceApiSettings = serviceApiSettings.Value;
            _clientSettings = clientSettings.Value;
            _clientAccessTokenCache = clientAccessTokenCache;
            _httpClient = httpClient;
        }

        //IdentityModel ile ilgili başka bir paket yüklüyoruz. "IdentityModel.AspNetCore"
        public async Task<string> GetToken()
        {
            //Cache den kontrol ediyoruz.
            var currentToken = await _clientAccessTokenCache.GetAsync("WebClientToken");

            //Var ise direkt dönüyoruz.
            if (currentToken != null)
            {
                return currentToken.AccessToken;
            }

            //Identity model paketinden geliyor. Https ile istek yapmaması için yazdık.
            var discovery = await _httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = _serviceApiSettings.IdentityBaseUri,
                //Https ile istek yapmaması için yazdık.
                Policy = new DiscoveryPolicy { RequireHttps = false }
            });

            //Discovery de bir hata var mı diye kontrol ediyoruz.
            if (discovery.IsError)
            {
                throw discovery.Exception;
            }

            var clientCredentialTokenRequest = new ClientCredentialsTokenRequest
            {
                ClientId = _clientSettings.WebClient.ClientId,
                ClientSecret = _clientSettings.WebClient.ClientSecret,
                Address = discovery.TokenEndpoint
            };

            var newToken = await _httpClient.RequestClientCredentialsTokenAsync(clientCredentialTokenRequest);

            if (newToken.IsError)
            {
                throw newToken.Exception;
            }

            //Önce Cache'e kayededip sonra geriye döneceğiz. Cache'e kaydetmek için hazır sunulan bir metot.
            await _clientAccessTokenCache.SetAsync("WebClientToken", newToken.AccessToken, newToken.ExpiresIn);

            return newToken.AccessToken;
        }
    }
}
