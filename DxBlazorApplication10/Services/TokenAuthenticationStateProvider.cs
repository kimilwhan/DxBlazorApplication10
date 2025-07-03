using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BlazorApp.Services
{
    public class TokenAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly IConfiguration _configuration;
        private string? _currentJwt;

        public TokenAuthenticationStateProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// 현재 인증 상태를 가져옵니다. JWT가 유효하면 인증된 사용자로, 아니면 익명 사용자로 처리합니다.
        /// </summary>
        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            if (string.IsNullOrEmpty(_currentJwt))
            {
                return Task.FromResult(CreateAnonymousState());
            }

            try
            {
                var user = ParseJwt(_currentJwt);
                var authState = new AuthenticationState(user);
                return Task.FromResult(authState);
            }
            catch
            {
                // 토큰 파싱/검증 실패 시 익명 상태 반환
                return Task.FromResult(CreateAnonymousState());
            }
        }

        /// <summary>
        /// 사용자를 인증된 상태로 표시하고, 상태 변경을 앱에 알립니다.
        /// </summary>
        /// <param name="jwt">로그인 시 발급받은 JWT</param>
        public void MarkUserAsAuthenticated(string jwt)
        {
            try
            {
                var user = ParseJwt(jwt);
                _currentJwt = jwt;
                NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
            }
            catch
            {
                // 토큰이 유효하지 않으면 아무 작업도 하지 않음
            }
        }

        /// <summary>
        /// 사용자를 로그아웃 상태로 표시하고, 상태 변경을 앱에 알립니다.
        /// </summary>
        public void MarkUserAsLoggedOut()
        {
            _currentJwt = null;
            NotifyAuthenticationStateChanged(Task.FromResult(CreateAnonymousState()));
        }

        /// <summary>
        /// JWT 문자열을 파싱하고 검증하여 ClaimsPrincipal 객체를 생성합니다.
        /// </summary>
        private ClaimsPrincipal ParseJwt(string jwt)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtSettings = _configuration.GetSection("Jwt");
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!)),
                ValidateIssuer = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidateAudience = true,
                ValidAudience = jwtSettings["Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero // 만료 시간 즉시 검증
            };

            var principal = tokenHandler.ValidateToken(jwt, validationParameters, out _);
            return principal;
        }

        /// <summary>
        /// 익명 사용자 상태를 생성합니다.
        /// </summary>
        private AuthenticationState CreateAnonymousState()
        {
            var anonymousIdentity = new ClaimsIdentity();
            var anonymousUser = new ClaimsPrincipal(anonymousIdentity);
            return new AuthenticationState(anonymousUser);
        }
    }
}
