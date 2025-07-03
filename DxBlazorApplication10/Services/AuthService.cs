using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BlazorApp.Services
{
    /// <summary>
    /// 사용자 로그인 비즈니스 로직을 처리하고 JWT를 생성합니다.
    /// </summary>
    public class AuthService
    {
        private readonly IConfiguration _configuration;

        // 실제 애플리케이션에서는 데이터베이스에서 사용자 정보를 확인해야 합니다.
        private readonly Dictionary<string, string> _users = new()
    {
        { "admin", "1234" },
        { "user", "1234" }
    };

        public AuthService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// 사용자 로그인을 시도하고 성공 시 JWT를 생성하여 반환합니다.
        /// </summary>
        /// <param name="username">사용자 이름</param>
        /// <param name="password">비밀번호</param>
        /// <returns>로그인 성공 시 JWT 문자열, 실패 시 null</returns>
        public Task<string?> LoginAsync(string username, string password)
        {
            // 사용자 정보 확인
            if (!_users.TryGetValue(username, out var storedPassword) || storedPassword != password)
            {
                return Task.FromResult<string?>(null);
            }

            // JWT에 포함될 클레임(Claims) 생성
            var claims = new[]
            {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, username == "admin" ? "Administrator" : "User"),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // JWT ID
        };

            // appsettings.json에서 JWT 설정값 가져오기
            var jwtSettings = _configuration.GetSection("Jwt");
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // JWT 생성
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(30), // 토큰 만료 시간
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var jwt = tokenHandler.WriteToken(securityToken);

            return Task.FromResult<string?>(jwt);
        }
    }
}
