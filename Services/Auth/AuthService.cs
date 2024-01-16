using JWTAuthTest.DBModels;
using JWTAuthTest.RequestModels;
using JWTAuthTest.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace JWTAuthTest.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AuthService(
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        public async Task<BusinessLogicMessage<string>> Login(LoginModel model)
        {
            User? user = await _userManager.FindByNameAsync(model.Username!);
            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password!))
                return new BusinessLogicMessage<string>(HttpStatusCode.Unauthorized, "Login credentials are wrong.", null);

            IList<string> userRoles = await _userManager.GetRolesAsync(user);

            List<Claim> authClaims = new()
            {
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
            };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            JwtSecurityToken token = GetToken(authClaims);

            string stringToken = new JwtSecurityTokenHandler().WriteToken(token);

            return new BusinessLogicMessage<string>(HttpStatusCode.OK, "Token has been issued successfully.", stringToken);
        }

        public async Task<BusinessLogicMessage> Register(RegisterModel model)
        {
            BusinessLogicMessage<User> createUserResponse = await CreateBaseUserFromModelAsync(model);

            if (!createUserResponse.GetValid())
                return new BusinessLogicMessage(HttpStatusCode.InternalServerError, "Failed to create a user.");

            User user = createUserResponse.Result!;
            await CheckAndAddRoleAsync(user, UserRoles.User);

            return new BusinessLogicMessage(HttpStatusCode.OK, $"User {user.UserName} has been successfully created");
        }

        public async Task<BusinessLogicMessage> RegisterAdmin(RegisterModel model)
        {
            BusinessLogicMessage<User> createUserResponse = await CreateBaseUserFromModelAsync(model);

            if (!createUserResponse.GetValid())
                return new BusinessLogicMessage(HttpStatusCode.NotFound, "Failed to create a user."); ;

            User user = createUserResponse.Result!;
            await CheckAndAddRoleAsync(user, UserRoles.User);
            await CheckAndAddRoleAsync(user, UserRoles.Admin);

            return new BusinessLogicMessage(HttpStatusCode.OK, $"User {user.UserName} has been successfully created");
        }

        private async Task<BusinessLogicMessage<User>> CreateBaseUserFromModelAsync(RegisterModel model)
        {
            var userExists = await _userManager.FindByNameAsync(model.Username!);
            if (userExists != null)
                return new BusinessLogicMessage<User>(HttpStatusCode.InternalServerError, "User already exists.");

            User user = new()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username,
                SignUpSecret = model.SignUpSecret
            };
            IdentityResult result = await _userManager.CreateAsync(user, model.Password!);

            if (!result.Succeeded)
            {
                List<IdentityError> errors = result.Errors.ToList();
                string errorString = "The user-creation failed with the following errors: \n";
                errorString += string.Join("\n ", errors);

                return new BusinessLogicMessage<User>(HttpStatusCode.InternalServerError, errorString);
            }

            return new BusinessLogicMessage<User>(HttpStatusCode.OK, "User has been created.", user);
        }

        private async Task CheckAndAddRoleAsync(User user, string role)
        {
            if (!await _roleManager.RoleExistsAsync(role))
                await _roleManager.CreateAsync(new IdentityRole(role));

            if (await _roleManager.RoleExistsAsync(role))
            {
                await _userManager.AddToRoleAsync(user, role);
            }
        }

        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            SymmetricSecurityKey authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]!));

            JwtSecurityToken token = new(
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }
    }
}
