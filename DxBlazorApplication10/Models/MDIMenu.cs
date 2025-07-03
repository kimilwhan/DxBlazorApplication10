namespace BlazorApp.Models
{
    public class MDIMenu
    {
        public int MenuID { get; set; }
        public string MenuCD { get; set; } = string.Empty;
        public string? MenuNM { get; set; }
        public string? PageNM { get; set; }
        public string? CssClass { get; set; }
        public string? IconCssClass { get; set; }
        public int? ParentID { get; set; }
    }

    public class MenuItem
    {
        public string Text { get; set; } = "";
        public string IconCssClass { get; set; } = "";
        public Type? PageComponentType { get; set; }
        public List<MenuItem>? Items { get; set; }
    }
}
