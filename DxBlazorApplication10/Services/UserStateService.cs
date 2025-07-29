using BlazorApp.Models;

namespace BlazorApp.Services
{
    /// <summary>
    /// 현재 로그인한 사용자의 상태를 관리하는 서비스 (Scoped)
    /// </summary>
    public class UserStateService
    {
        /// <summary>
        /// 현재 로그인한 사용자 정보
        /// </summary>
        public SCU100? CurrentUser { get; private set; }

        /// <summary>
        /// 사용자 정보가 변경되었을 때 UI를 갱신하기 위한 이벤트
        /// </summary>
        public event Action? OnChange;

        /// <summary>
        /// 로그인 성공 시 사용자 정보를 설정하고 변경 이벤트를 발생시킵니다.
        /// </summary>
        public void SetCurrentUser(SCU100 user)
        {
            CurrentUser = user;
            NotifyStateChanged();
        }

        /// <summary>
        /// 로그아웃 시 사용자 정보를 초기화합니다.
        /// </summary>
        public void ClearCurrentUser()
        {
            CurrentUser = null;
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();

    }
}
