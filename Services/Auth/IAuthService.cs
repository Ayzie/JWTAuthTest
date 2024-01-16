using JWTAuthTest.RequestModels;
using JWTAuthTest.Utils;

namespace JWTAuthTest.Services.Auth
{
    public interface IAuthService
    {
        public Task<BusinessLogicMessage> Register(RegisterModel model);
        public Task<BusinessLogicMessage> RegisterAdmin(RegisterModel model);
        public Task<BusinessLogicMessage<string>> Login(LoginModel model);
    }
}
