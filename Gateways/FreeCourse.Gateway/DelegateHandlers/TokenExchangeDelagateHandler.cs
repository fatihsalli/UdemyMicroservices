using IdentityModel.Client;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace FreeCourse.Gateway.DelegateHandlers
{
    //Token exchange için oluşturduk. İstek geldiğinde handler ile araya girip token exchange yapacak.
    public class TokenExchangeDelagateHandler:DelegatingHandler
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private string _accessToken;

        public TokenExchangeDelagateHandler(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        //Mevcut tokenı almak için
        private async Task<string> GetToken(string requestToken)
        {
            if (!string.IsNullOrEmpty(_accessToken))
            {
                return _accessToken;
            }

            //Identity model paketinden geliyor. Https ile istek yapmaması için yazdık.
            var discovery = await _httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = _configuration["IdentityServerURL"],
                //Https ile istek yapmaması için yazdık.
                Policy = new DiscoveryPolicy { RequireHttps = false }
            });

            //Discovery de bir hata var mı diye kontrol ediyoruz.
            if (discovery.IsError)
            {
                throw discovery.Exception;
            }

            TokenExchangeTokenRequest tokenExchangeTokenRequest = new TokenExchangeTokenRequest()
            { 
                Address=discovery.TokenEndpoint,
                ClientId = "TokenExchangeClient",
                ClientSecret = "secret",
                GrantType= "urn:ietf:params:oauth:grant-type:token-exchange",
                SubjectToken=requestToken,
                SubjectTokenType= "urn:ietf:params:oauth:token-type:access-token",
                Scope= "openid discount_fullpermission payment_fullpermission"
            };

            var tokenResponse=await _httpClient.RequestTokenExchangeTokenAsync(tokenExchangeTokenRequest);

            if (tokenResponse.IsError)
            {
                throw tokenResponse.Exception;
            }

            _accessToken=tokenResponse.AccessToken;

            return _accessToken;
        }

        //Yeni tokenı almak için.
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var requestToken = request.Headers.Authorization.Parameter;

            var newToken = await GetToken(requestToken);

            request.SetBearerToken(newToken);

            return await base.SendAsync(request, cancellationToken);
        }


    }
}
