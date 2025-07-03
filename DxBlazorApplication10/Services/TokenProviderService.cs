using System.Collections.Concurrent;
using System.Security.Claims;

namespace BlazorApp.Services
{
    public class TokenProviderService
    {
        // 스레드로부터 안전한 ConcurrentDictionary를 사용하여 토큰을 저장합니다.
        private readonly ConcurrentDictionary<string, ClaimsPrincipal> _tokens = new();

        /// <summary>
        /// 사용자 정보를 저장하고 고유한 토큰을 생성하여 반환합니다.
        /// </summary>
        /// <param name="user">저장할 사용자의 ClaimsPrincipal 객체</param>
        /// <returns>생성된 고유 토큰 문자열</returns>
        public string AddUser(ClaimsPrincipal user)
        {
            var token = Guid.NewGuid().ToString();
            _tokens.TryAdd(token, user);
            return token;
        }

        /// <summary>
        /// 토큰을 사용하여 저장된 사용자 정보를 가져옵니다.
        /// </summary>
        /// <param name="token">조회할 토큰</param>
        /// <returns>토큰에 해당하는 ClaimsPrincipal 객체, 없으면 null</returns>
        public ClaimsPrincipal? GetUserFromToken(string token)
        {
            _tokens.TryGetValue(token, out var user);
            return user;
        }

        /// <summary>
        /// 저장소에서 토큰을 제거합니다. (로그아웃 시 사용)
        /// </summary>
        /// <param name="token">제거할 토큰</param>
        public void RemoveToken(string token)
        {
            _tokens.TryRemove(token, out _);
        }
    }
}
