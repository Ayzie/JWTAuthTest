using Microsoft.AspNetCore.Identity;

namespace JWTAuthTest.DBModels
{
    public class User : IdentityUser
    {
        public string SignUpSecret { get; set; }
    }
}
