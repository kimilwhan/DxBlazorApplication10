using Microsoft.AspNetCore.Components;

namespace BlazorApp.Models
{
    public class MDITab {
        public string Text { get; set; }
        public int VisibleIndex { get; set; }
        public bool Visible { get; set; }
        public string? PageNM { get; set; }

        public MDITab(string text, int visibleIndex, bool visible, string? pageNM = null)
        {
            Text = text;
            VisibleIndex = visibleIndex;
            Visible = visible;
            PageNM = pageNM;
        }
    }

    public class MDITab2
    {
        public required string Id { get; init; }
        public required string Title { get; init; }
        public required Type ComponentType { get; init; }
        public required string? PageNM { get; init; }

        // 이 탭이 팝업 상태인지 여부를 나타냅니다.
        public bool IsPopup { get; set; } = false;

        // ComponentType 대신 RenderFragment를 사용하여 탭의 UI 콘텐츠를 저장합니다.
        public required RenderFragment Content { get; set; }

        // IChangable 인터페이스 등을 확인하기 위해 실제 컴포넌트 인스턴스를 저장합니다.
        public object? ComponentInstance { get; set; }
    }
}
