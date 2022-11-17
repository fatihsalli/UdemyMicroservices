using FreeCourse.Shared.Dtos;
using FreeCourse.Web.Models;
using FreeCourse.Web.Services.Interfaces;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace FreeCourse.Web.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ClientSettings _clientSettings;
        private readonly ServiceApiSettings _serviceApiSettings;

        public IdentityService(HttpClient client, IHttpContextAccessor httpContextAccessor, IOptions<ClientSettings> clientSettings, IOptions<ServiceApiSettings> serviceApiSettings)
        {
            _httpClient = client;
            _httpContextAccessor = httpContextAccessor;
            _clientSettings = clientSettings.Value;
            _serviceApiSettings = serviceApiSettings.Value;
        }

        public async Task<TokenResponse> GetAccessTokenByRefreshToken()
        {
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

            //Cookie den refresh tokenı alıyoruz. //new AuthenticationToken{Name=OpenIdConnectParameterNames.RefreshToken,Value=token.RefreshToken}, kaydederken bu isimle kaydettiğimiz için yine aynı isimle çağırıyoruz.
            var refreshToken =await _httpContextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);

            RefreshTokenRequest refreshTokenRequest = new()
            { 
                ClientId=_clientSettings.WebClientForUser.ClientId,
                ClientSecret=_clientSettings.WebClientForUser.ClientSecret,
                RefreshToken= refreshToken,
                Address= discovery.TokenEndpoint
            };

            var token=await _httpClient.RequestRefreshTokenAsync(refreshTokenRequest);

            if (token.IsError)
            {
                //Loglama yapılınca buraya ekleyebiliriz. Ya da hata da fırlatabiliriz.
                return null;
            }

            //OpenIdConnect kütüphanesini yükledik. Access token,refresh token ve süreyi tuttuk.
            var authenticationTokens=new List<AuthenticationToken> {
                new AuthenticationToken{Name=OpenIdConnectParameterNames.AccessToken,Value=token.AccessToken},
                new AuthenticationToken{Name=OpenIdConnectParameterNames.RefreshToken,Value=token.RefreshToken},
                new AuthenticationToken{Name=OpenIdConnectParameterNames.ExpiresIn,Value=DateTime.Now.AddSeconds(token.ExpiresIn).ToString("o",CultureInfo.InvariantCulture)}
            };

            //Cookieden bulup set edeceğiz.
            var authenticationResult = await _httpContextAccessor.HttpContext.AuthenticateAsync();

            var properties = authenticationResult.Properties;

            //Var olanı cookieden aldığı bilgilerle dolduracak.
            properties.StoreTokens(authenticationTokens);

            await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,authenticationResult.Principal,properties);

            return token;
        }

        //Kullanıcı çıkış yaptığında refresh tokenı silmek için
        public async Task RevokeRefreshToken()
        {
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

            var refreshToken = await _httpContextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);

            TokenRevocationRequest tokenRevocationRequest = new TokenRevocationRequest
            {
                ClientId=_clientSettings.WebClientForUser.ClientId,
                ClientSecret=_clientSettings.WebClientForUser.ClientSecret,
                Address=discovery.RevocationEndpoint,
                Token=refreshToken,
                TokenTypeHint="refresh token"
            };

            await _httpClient.RevokeTokenAsync(tokenRevocationRequest);
        }

        public async Task<Response<bool>> SignIn(SignInVM signInInput)
        {
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

            //Resource owner passwordu oluşturuyoruz."PasswordTokenRequest" hazır classını kullanıyoruz.
            var passwordTokenRequest = new PasswordTokenRequest
            {
                ClientId = _clientSettings.WebClientForUser.ClientId,
                ClientSecret = _clientSettings.WebClientForUser.ClientSecret,
                UserName = signInInput.Email,
                Password = signInInput.Password,
                Address = discovery.TokenEndpoint
            };

            var token = await _httpClient.RequestPasswordTokenAsync(passwordTokenRequest);

            if (token.IsError)
            {
                var responseContent = await token.HttpResponse.Content.ReadAsStringAsync();
                //PropertyNameCaseInsensitive json serialize-deserialize ederken büyük küçük harfe bakar bu ayarı kapattık.
                var errodDto = JsonSerializer.Deserialize<ErrorDto>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return Response<bool>.Fail(errodDto.Errors, 400);
            }

            //Elimizdeki tokenla user hakkında bilgi almak için UserEndpointine istek yapıyoruz.
            var userInfoRequest = new UserInfoRequest
            {
                Token = token.AccessToken,
                Address = discovery.UserInfoEndpoint
            };

            //Kullanıcı bilgilerini alıyoruz. Bu sayede kullanıcı bazlı yetkilendirme vs. yapabiliriz.
            var userInfo = await _httpClient.GetUserInfoAsync(userInfoRequest);

            if (userInfo.IsError)
            {
                throw userInfo.Exception;
            }

            //Cookie oluşturuyoruz.Key-value şeklinde. IdentityModel sayesinde Claimleri verdik."CookieAuthenticationDefaults.AuthenticationScheme" sabit bir şema aldık "Cookies" olarak.
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(userInfo.Claims, CookieAuthenticationDefaults.AuthenticationScheme, "name", "role");

            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            //Access token ve refresh tokenı tutmak için
            var authenticationProperties = new AuthenticationProperties();

            //OpenIdConnect kütüphanesini yükledik. Access token,refresh token ve süreyi tuttuk.
            authenticationProperties.StoreTokens(new List<AuthenticationToken> {
                new AuthenticationToken{Name=OpenIdConnectParameterNames.AccessToken,Value=token.AccessToken},
                new AuthenticationToken{Name=OpenIdConnectParameterNames.RefreshToken,Value=token.RefreshToken},
                new AuthenticationToken{Name=OpenIdConnectParameterNames.ExpiresIn,Value=DateTime.Now.AddSeconds(token.ExpiresIn).ToString("o",CultureInfo.InvariantCulture)}
            });

            //Cookie kalıcı olması için
            authenticationProperties.IsPersistent = signInInput.IsRemember;

            await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal, authenticationProperties);

            return Response<bool>.Success(200);
        }
    }
}
