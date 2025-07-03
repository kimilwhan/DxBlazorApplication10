using DevExpress.Blazor;
using BlazorApp.Models;

namespace BlazorApp.Services
{
    public class MdiStateService
    {
        public event Action? OnChange;

        public List<MDITab2> OpenTabs { get; } = new();
        public int ActiveTabIndex { get; private set; }

        public void OpenTab(string title, Type componentType)
        {
            var tabId = componentType.Name;
            var existingTab = OpenTabs.FindIndex(t => t.Id == tabId);

            if (existingTab != -1)
            {
                ActiveTabIndex = existingTab;
            }
            else
            {
                OpenTabs.Add(new MDITab2
                {
                    Id = tabId,
                    Title = title,
                    ComponentType = componentType,
                    PageNM = ""
                });
                ActiveTabIndex = OpenTabs.Count - 1;
            }
            NotifyStateHasChanged();
        }

        public void OpenTab(string tabId, string title, string componentPath)
        {
            //var tabId = title;
            var existingTab = OpenTabs.FindIndex(t => t.Id == tabId);

            if (existingTab != -1)
            {
                ActiveTabIndex = existingTab;
            }
            else
            {
                OpenTabs.Add(new MDITab2
                {
                    Id = tabId,
                    Title = title,
                    ComponentType = typeof(Nullable),
                    PageNM = componentPath
                });
                ActiveTabIndex = OpenTabs.Count - 1;
            }
            NotifyStateHasChanged();
        }

        public void CloseTab(int index)
        {
            if (index < 0 || index >= OpenTabs.Count) return;

            OpenTabs.RemoveAt(index);
            if (ActiveTabIndex >= index && ActiveTabIndex > 0)
            {
                ActiveTabIndex--;
            }
            else if (OpenTabs.Count == 0)
            {
                ActiveTabIndex = 0;
            }
            NotifyStateHasChanged();
        }

        public void CloseAllTabs()
        {
            OpenTabs.ForEach((t) => OpenTabs.Remove(t));
        }

        public void SetActiveTab(int index)
        {
            ActiveTabIndex = index;
            NotifyStateHasChanged();
        }

        public string? GetTabTextByTabInfo(ITabInfo tabInfo)
        {
            //return tabs.FirstOrDefault(t => t.Text == tabInfo.Text)?.Text;
            return OpenTabs.FirstOrDefault(t => t.Title == tabInfo.Text)?.Title;
        }

        //public int GetVisibleIndexByTabText(string? text)
        //{
        //    //return OpenTabs.Find(t => t.Title == text)?.VisibleIndex ?? -1;
        //}

        //public bool GetVisibleByTabText(string? text)
        //{
        //    return tabs.Find(t => t.Text == text)?.Visible ?? true;
        //}

        private void NotifyStateHasChanged() => OnChange?.Invoke();
    }
}
