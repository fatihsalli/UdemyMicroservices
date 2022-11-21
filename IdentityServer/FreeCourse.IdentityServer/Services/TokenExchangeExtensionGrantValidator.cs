using IdentityServer4.Validation;
using System.Linq;
using System.Threading.Tasks;

namespace FreeCourse.IdentityServer.Services
{
    //Token exchange için oluşturduk.
    public class TokenExchangeExtensionGrantValidator : IExtensionGrantValidator
    {
        //Akış ismini veriyoruz.
        public string GrantType => "urn:ietf:params:oauth:grant-type:token-exchange";

        private readonly ITokenValidator _tokenValidator;

        public TokenExchangeExtensionGrantValidator(ITokenValidator tokenValidator)
        {
            _tokenValidator = tokenValidator;
        }

        public async Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            var requestRaw = context.Request.Raw.ToString();

            //Tokenı alıyoruz
            var token = context.Request.Raw.Get("subject_token");

            //Token yok ise hata veriyoruz.
            if (string.IsNullOrEmpty(token))
            {
                context.Result = new GrantValidationResult(IdentityServer4.Models.TokenRequestErrors.InvalidRequest, "token missing");
                return;
            }

            var tokenValidateResult = await _tokenValidator.ValidateAccessTokenAsync(token);

            //Tokenın geçerliliğini bilgilerini kontrol ediyoruz.
            if (tokenValidateResult.IsError)
            {
                context.Result = new GrantValidationResult(IdentityServer4.Models.TokenRequestErrors.InvalidGrant, "token invalid");

                return;
            }

            //Bu aşamada token geçerli. Kullanıcıyı buluyoruz.
            var subjectClaim = tokenValidateResult.Claims.FirstOrDefault(c => c.Type == "sub");

            if (subjectClaim == null)
            {
                context.Result = new GrantValidationResult(IdentityServer4.Models.TokenRequestErrors.InvalidGrant, "token must contain sub value");

                return;
            }

            //Bu aşamaya geldiyse kullanıcı da vardır. Artık bu noktada yeni token verebiliriz.
            //subjectClaim.Value => kullanıcı id
            context.Result = new GrantValidationResult(subjectClaim.Value, "access_token", tokenValidateResult.Claims);

            return;
        }
    }
}
