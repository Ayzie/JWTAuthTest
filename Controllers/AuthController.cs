using JWTAuthTest.RequestModels;
using JWTAuthTest.Services.Auth;
using JWTAuthTest.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Net;

namespace JWTAuthTest.Controllers
{
    public class AuthController : ControllerExtension
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return ConsumeResult(new BusinessLogicMessage(HttpStatusCode.BadRequest, GetModelErrorMessage()));
            }

            BusinessLogicMessage result = await _authService.Register(model);

            return ConsumeResult(result);
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost("register-admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return ConsumeResult(new BusinessLogicMessage(HttpStatusCode.BadRequest, GetModelErrorMessage()));
            }

            BusinessLogicMessage result = await _authService.RegisterAdmin(model);

            return ConsumeResult(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return ConsumeResult(new BusinessLogicMessage(HttpStatusCode.BadRequest, GetModelErrorMessage()));
            }

            BusinessLogicMessage<string> result = await _authService.Login(model);

            return ConsumeResult(result);
        }
    }
}
