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
    }
}
