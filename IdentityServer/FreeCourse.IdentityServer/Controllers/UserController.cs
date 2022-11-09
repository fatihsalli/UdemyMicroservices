using FreeCourse.IdentityServer.Dtos;
using FreeCourse.IdentityServer.Models;
using FreeCourse.Shared.ControllerBases;
using FreeCourse.Shared.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using static IdentityServer4.IdentityServerConstants;

namespace FreeCourse.IdentityServer.Controllers
{
    //Identity Server static classdan yararlanarak Authorization veriyoruz.Claim bazlı startup tarafında eklenen metot sayesinde biz PolicyName kullanarak autorize işlemini gerçekleştirdik.
    [Authorize(LocalApi.PolicyName)]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : CustomBaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public UserController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpDto signUpDto)
        {
            var user=new ApplicationUser 
            { 
                UserName=signUpDto.UserName, 
                Email=signUpDto.Email,
                City=signUpDto.City
            };
            var result=await _userManager.CreateAsync(user, signUpDto.Password);

            if (!result.Succeeded)
            {
                return CreateActionResultInstance(Response<NoContent>.Fail(result.Errors.Select(x => x.Description).ToList(), 400));
            }
            return CreateActionResultInstance(Response<NoContent>.Success(204));
        }



    }
}
