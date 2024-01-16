namespace JWTAuthTest.Dto
{
    public class UserInfoDto
    {
        public string Username { get; }
        public string UserId { get; }
        public string[] Roles { get; }
        public string SignUpSecret { get; }

        public UserInfoDto(string username, string userId, string[] roles, string signUpSecret)
        {
            Username = username;
            UserId = userId;
            Roles = roles;
            SignUpSecret = signUpSecret;
        }
    }
}
