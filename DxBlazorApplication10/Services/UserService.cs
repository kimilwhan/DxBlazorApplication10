using BlazorApp.Models;

namespace BlazorApp.Services
{
    public class UserService
    {
        private readonly IDapperService _dapperService;

        public UserService(IDapperService dapperService)
        {
            _dapperService = dapperService;
        }

        // 사용자 아이디로 사용자 정보 가져오기
        public async Task<SCU100?> GetUserByIdAsync(string userId)
        {
            const string sql = "SELECT * FROM SCU100 WHERE Id = @UserId";
            return await _dapperService.QuerySingleOrDefaultAsync<SCU100>(sql, new { UserId = userId });
        }

        // 사용자 정보 업데이트 (이름, 이메일 등)
        public async Task<bool> UpdateUserProfileAsync(SCU100 user)
        {
            // ID, 암호 등 민감한 정보는 업데이트에서 제외
            const string sql = @"
            UPDATE SCU100 SET
                Nm = @Nm,
                emp_no = @Emp_No,
                dsc = @Dsc 
            WHERE Reg_Id = @Reg_Id";

            var result = await _dapperService.ExecuteAsync(sql, user);
            return result > 0;
        }

        // 암호 변경
        public async Task<bool> ChangePasswordAsync(string userId, string newHashedPassword)
        {
            const string sql = "UPDATE SCU100 SET Pwd = @Password, Pwd_Dt = GETDATE() WHERE Id = @UserId";
            var result = await _dapperService.ExecuteAsync(sql, new { Password = newHashedPassword, UserId = userId });
            return result > 0;
        }

        // ✨ 1. 비밀번호 재설정 토큰 생성
        public async Task<string?> GeneratePasswordResetTokenAsync(string userId)
        {
            var user = await GetUserByIdAsync(userId);
            if (user == null) return null;

            var token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            var expires = DateTime.UtcNow.AddHours(1); // 토큰 유효 시간: 1시간

            const string sql = @"
            UPDATE SCU100 
            SET ResetToken = @Token, ResetTokenExpires = @Expires
            WHERE Id = @UserId";

            await _dapperService.ExecuteAsync(sql, new { Token = token, Expires = expires, UserId = userId });

            // 실제 앱에서는 이 토큰을 포함한 링크를 이메일로 발송합니다.
            return token;
        }

        // ✨ 2. 토큰으로 사용자 정보 조회 (검증)
        public async Task<SCU100?> GetUserByResetTokenAsync(string token)
        {
            const string sql = @"
            SELECT * FROM SCU100
            WHERE ResetToken = @Token AND ResetTokenExpires > GETUTCDATE()"; // UTC 시간으로 비교

            return await _dapperService.QuerySingleOrDefaultAsync<SCU100>(sql, new { Token = token });
        }

        // ✨ 3. 비밀번호 최종 재설정
        public async Task<bool> ResetPasswordAsync(string token, string newPassword)
        {
            var user = await GetUserByResetTokenAsync(token);
            if (user == null) return false;

            var newHashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);

            const string sql = @"
            UPDATE SCU100 SET
                Pwd = @HashedPassword,
                ResetToken = NULL,          -- 토큰 재사용 방지
                ResetTokenExpires = NULL,
                Pwd_Dt = GETDATE()
            WHERE Reg_Id = @Reg_Id";

            var result = await _dapperService.ExecuteAsync(sql, new { HashedPassword = newHashedPassword, Reg_Id = user.Reg_Id });
            return result > 0;
        }

    }
}
