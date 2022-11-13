using FreeCourse.IdentityServer.Models;
using IdentityModel;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FreeCourse.IdentityServer.Services
{
    //Bu classı user ile token alma durumu için oluşturduk. Biz MVC tarafında kullanıcı login yaptığında eğerki IdentityServer sayfasına yönlendirseydik o zaman bu class'a gerek yoktu. Ama biz burada kullanıcı MVC sitesinden ayrılmadan doğrulama işlemini yapmak için "IResourceOwnerPasswordValidator" interface'inden miras aldık.
    public class IdentityResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public IdentityResourceOwnerPasswordValidator(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            //Biz UserName olarak email göndereceğiz.
            var existUser = await _userManager.FindByEmailAsync(context.UserName);

            if (existUser == null)
            {
                //IdentityServer ın döneceği Response'a ilave yapmak için aşağıdaki düzenlemeyi yaptık.
                var errors = new Dictionary<string, object>();
                errors.Add("errors", new List<string> { "Email or password is wrong!" });

                context.Result.CustomResponse = errors;

                return;
            }

            var passwordCheck = await _userManager.CheckPasswordAsync(existUser, context.Password);

            if (passwordCheck == false)
            {
                //IdentityServer ın döneceği Response'a ilave yapmak için aşağıdaki düzenlemeyi yaptık.
                var errors = new Dictionary<string, object>();
                errors.Add("errors", new List<string> { "Email or password is wrong!" });
                context.Result.CustomResponse = errors;

                return;
            }

            //Result içerisinde bunu döndüğümüzde IdentityServer kullanıcının olduğunu ve giriş yaptığını anlayarak token üretecek.
            context.Result = new GrantValidationResult(existUser.Id.ToString(), OidcConstants.AuthenticationMethods.Password);





        }
    }
}
