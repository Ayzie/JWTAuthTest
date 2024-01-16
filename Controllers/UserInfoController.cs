using System.Net;
using JWTAuthTest.DBModels;
using JWTAuthTest.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using JWTAuthTest.Dto;

namespace JWTAuthTest.Controllers
{
    [Authorize]
    public class UserInfoController : ControllerExtension
    {
        private readonly UserManager<User> _userManager;

        public UserInfoController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        [HttpPost("GetUserInfo")]
        public async Task<IActionResult> GetUserInfo()
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            string? userName = User.FindFirstValue(ClaimTypes.NameIdentifier);
            string[] roles = User.FindAll(ClaimTypes.Role).Select(x => x.Value).ToArray();

            if (userId == null || userName == null)
            {
                return ConsumeResult(BusinessLogicMessage.NotAuthenticated);
            }

            User? user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return ConsumeResult(BusinessLogicMessage.NotAuthenticated);
            }

            string? signUpSecret = user.SignUpSecret;

            if (signUpSecret == null)
            {
                return ConsumeResult(new BusinessLogicMessage(HttpStatusCode.NotFound, "SignUpSecret was not found."));
            }

            UserInfoDto userInfoDto = new (userName, userId, roles, signUpSecret);

            return ConsumeResult(new BusinessLogicMessage<UserInfoDto>(HttpStatusCode.OK, "Here is the userInfo.", userInfoDto));
        }
    }
}
