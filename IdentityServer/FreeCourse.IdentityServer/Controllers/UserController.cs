using FreeCourse.IdentityServer.Dtos;
using FreeCourse.IdentityServer.Models;
using FreeCourse.Shared.ControllerBases;
using FreeCourse.Shared.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
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
            var user = new ApplicationUser
            {
                UserName = signUpDto.UserName,
                Email = signUpDto.Email,
                City = signUpDto.City
            };
            var result = await _userManager.CreateAsync(user, signUpDto.Password);

            if (!result.Succeeded)
            {
                return CreateActionResultInstance(Response<NoContent>.Fail(result.Errors.Select(x => x.Description).ToList(), 400));
            }
            return CreateActionResultInstance(Response<NoContent>.Success(204));
        }

        [HttpGet]
        public async Task<IActionResult> GetUser()
        {
            var userIdClaim = User.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub);

            if (userIdClaim == null) return CreateActionResultInstance(Response<NoContent>.Fail("Claim not found!", 400));

            var user = await _userManager.FindByIdAsync(userIdClaim.Value);

            if (user == null) return CreateActionResultInstance(Response<NoContent>.Fail("User not found!", 400));

            var userDto = new UserForClientDto { Id = user.Id, UserName = user.UserName, Email = user.Email, City = user.City };
            //Bu şekilde yazınca Mvc tarafı tanımıyor.
            return CreateActionResultInstance(Response<UserForClientDto>.Success(userDto, 200));

            //return Ok(new { Id = user.Id, UserName = user.UserName, Email = user.Email, City = user.City });

        }





    }
}
