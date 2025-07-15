namespace BlazorApp.Services
{
    // 설명: 메시지 박스의 상태를 제어하고,
    // 사용자의 응답을 비동기적으로 반환하는 중앙 서비스입니다.
    public class MessageBoxService
    {
        // 메시지 박스 UI에 변경 사항을 알리기 위한 이벤트
        public event Action? OnChange;

        // 메시지 박스의 표시 여부
        public bool IsVisible { get; set; }
        // 메시지 박스의 제목
        public string Title { get; set; } = "";
        // 메시지 박스의 내용
        public string Message { get; set; } = "";
        // '확인' 버튼만 표시할지, '확인/취소' 버튼을 모두 표시할지 결정
        public bool ShowOkCancelButtons { get; set; }

        // 사용자의 응답을 기다리는 Task를 관리하는 객체
        private TaskCompletionSource<bool>? _taskCompletionSource;

        /// <summary>
        /// 확인/취소 버튼이 있는 메시지 박스를 표시하고 사용자의 응답을 기다립니다.
        /// </summary>
        /// <param name="title">표시할 제목</param>
        /// <param name="message">표시할 메시지</param>
        /// <returns>사용자가 '확인'을 누르면 true, '취소'를 누르면 false를 반환합니다.</returns>
        public Task<bool> Confirm(string title, string message)
        {
            Title = title;
            Message = message;
            ShowOkCancelButtons = true; // 확인/취소 버튼 표시
            IsVisible = true;
            NotifyStateChanged();

            _taskCompletionSource = new TaskCompletionSource<bool>();
            return _taskCompletionSource.Task;
        }

        /// <summary>
        /// 확인 버튼만 있는 알림 메시지 박스를 표시합니다.
        /// </summary>
        /// <param name="title">표시할 제목</param>
        /// <param name="message">표시할 메시지</param>
        public Task ShowAlert(string title, string message)
        {
            Title = title;
            Message = message;
            ShowOkCancelButtons = false; // 확인 버튼만 표시
            IsVisible = true;
            NotifyStateChanged();

            _taskCompletionSource = new TaskCompletionSource<bool>();
            return _taskCompletionSource.Task;
        }


        /// <summary>
        /// 메시지 박스에서 '확인' 또는 '닫기' 버튼을 클릭했을 때 호출됩니다.
        /// </summary>
        internal void Close(bool result)
        {
            IsVisible = false;
            NotifyStateChanged();
            _taskCompletionSource?.SetResult(result);
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
