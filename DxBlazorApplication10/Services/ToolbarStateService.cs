namespace BlazorApp.Services
{
    // 설명: 레이아웃의 툴바와 현재 페이지의 기능을 연결하는 서비스입니다.
    public class ToolbarStateService
    {
        // 각 버튼 클릭 시 실행될 액션을 담는 델리게이트입니다.
        public Action? OnSearchClick { get; private set; }
        public Action? OnSaveClick { get; private set; }
        public Action? OnDeleteClick { get; private set; }

        // 툴바 버튼의 활성화/비활성화 상태
        public bool IsSaveEnabled { get; private set; } = false;

        // 서비스 상태가 변경되었음을 레이아웃에 알리기 위한 이벤트입니다.
        public event Action? OnChange;

        /// <summary>
        /// 현재 페이지로부터 실행할 액션들을 등록받습니다.
        /// </summary>
        public void RegisterToolbarActions(Action? search = null, Action? save = null, Action? delete = null)
        {
            OnSearchClick = search;
            OnSaveClick = save;
            OnDeleteClick = delete;
            NotifyStateChanged(); // 상태 변경 알림
        }

        /// <summary>
        /// '저장' 버튼 같은 특정 버튼의 상태를 변경합니다.
        /// </summary>
        public void SetSaveButtonState(bool isEnabled)
        {
            if (IsSaveEnabled != isEnabled)
            {
                IsSaveEnabled = isEnabled;
                NotifyStateChanged();
            }
        }

        /// <summary>
        /// 페이지가 닫힐 때 등록된 액션들을 초기화합니다.
        /// </summary>
        public void ResetToolbar()
        {
            RegisterToolbarActions(); // 모든 액션을 null로 설정
            SetSaveButtonState(false);
        }

        // 레이아웃(UI)에 변경 사항을 알리는 메서드
        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
