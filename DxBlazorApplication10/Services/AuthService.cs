using BlazorApp.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net; 

namespace BlazorApp.Services
{
    /// <summary>
    /// 사용자 로그인 비즈니스 로직을 처리하고 JWT를 생성합니다.
    /// </summary>
    public class AuthService
    {
        private readonly IConfiguration _configuration;
        private readonly IDapperService _dapperService; // DapperService 주입
        private readonly UserStateService _userStateService; // ✨ 1. UserStateService 주입

        //    // 실제 애플리케이션에서는 데이터베이스에서 사용자 정보를 확인해야 합니다.
        //    private readonly Dictionary<string, string> _users = new()
        //{
        //    { "admin", "1234" },
        //    { "user", "1234" }
        //};

        public AuthService(IConfiguration configuration, IDapperService dapperService, UserStateService userStateService)
        {
            _configuration = configuration;
            _dapperService = dapperService;
            _userStateService = userStateService;   // ✨ 2. 주입받은 서비스 할당
        }

        /// <summary>
        /// 사용자 로그인을 시도하고 성공 시 JWT를 생성하여 반환합니다.
        /// </summary>
        /// <param name="username">사용자 이름</param>
        /// <param name="password">비밀번호</param>
        /// <returns>로그인 성공 시 JWT 문자열, 실패 시 null</returns>
        public async Task<string?> LoginAsync(string username, string password)
        {
            //// 사용자 정보 확인
            //if (!_users.TryGetValue(username, out var storedPassword) || storedPassword != password)
            //{
            //    return Task.FromResult<string?>(null);
            //}

            // 1. DapperService를 사용하여 데이터베이스에서 사용자 정보 조회
            const string sql = @"
                SELECT 
                    reg_id, id, pwd, pda_pwd, nm, usr_ty 
                FROM SCU100 
                WHERE id = @Id";

            // 주입받은 _dapperService를 통해 쿼리 실행
            var user = await _dapperService.QuerySingleOrDefaultAsync<SCU100>(sql, new { Id = username });

            // 사용자가 없거나 비밀번호가 일치하지 않으면 null 반환
            if (user == null || !VerifyPassword(password, user.Pwd))
            {
                return null;
            }

            // ✨ 3. 로그인 성공 시 UserStateService에 사용자 정보 저장
            _userStateService.SetCurrentUser(user);


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

            //return Task.FromResult<string?>(jwt);
            return jwt;
        }

        /// <summary>
        /// ✨ 3. BCrypt.Verify를 사용하여 비밀번호를 안전하게 검증합니다.
        /// </summary>
        /// <param name="enteredPassword">사용자가 로그인 시 입력한 평문 비밀번호</param>
        /// <param name="storedHashedPassword">DB에 저장된 해시된 비밀번호</param>
        private bool VerifyPassword(string enteredPassword, string? storedHashedPassword)
        {
            if (string.IsNullOrEmpty(enteredPassword) || string.IsNullOrEmpty(storedHashedPassword))
            {
                return false;
            }

            // BCrypt가 평문 비밀번호와 해시값을 비교하여 일치 여부를 반환합니다.
            try
            {
                return BCrypt.Net.BCrypt.Verify(enteredPassword, storedHashedPassword);

                //// 사용자가 입력한 평문 비밀번호
                //string plainPassword = "user_entered_password123";

                //// BCrypt를 사용하여 비밀번호를 해시합니다. (솔트가 자동으로 생성 및 포함됨)
                //string hashedPassword = BCrypt.Net.BCrypt.HashPassword(plainPassword);
            }
            catch (BCrypt.Net.SaltParseException)
            {
                // DB에 저장된 Pwd 값이 유효한 BCrypt 해시 형식이 아닐 경우 예외 발생
                // 이 경우는 평문 비밀번호가 저장되어 있다는 의미일 수 있습니다.
                return false;
            }
        }

        // ✨ 현재 비밀번호 검증을 위한 메서드 추가
        public async Task<bool> VerifyCredentialsAsync(string username, string password)
        {
            const string sql = "SELECT Pwd FROM SCU100 WHERE Id = @Username --AND UseYn = 'Y'";

            var userHashedPassword = await _dapperService.QuerySingleOrDefaultAsync<string>(sql, new { Username = username });

            if (userHashedPassword == null)
            {
                return false;
            }

            return VerifyPassword(password, userHashedPassword);
        }
    }
}
