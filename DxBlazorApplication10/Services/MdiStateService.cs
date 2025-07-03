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

        /// <summary>
        /// 인덱스 대신 고유 ID를 사용하여 탭을 닫습니다.
        /// </summary>
        /// <param name="tabId">닫을 탭의 고유 ID</param>
        public void CloseTabID(string tabId)
        {
            var tabToClose = OpenTabs.FirstOrDefault(t => t.Id == tabId);
            if (tabToClose == null) return;

            int indexToClose = OpenTabs.IndexOf(tabToClose);

            // 탭을 리스트에서 제거
            OpenTabs.Remove(tabToClose);

            // 활성 탭 인덱스 조정
            if (OpenTabs.Count == 0)
            {
                ActiveTabIndex = 0;
            }
            else if (ActiveTabIndex >= indexToClose)
            {
                // 닫힌 탭이 현재 활성 탭이거나 그 앞 탭이었다면, 인덱스를 하나 줄임
                // 단, 0보다 작아지지 않도록 보정
                ActiveTabIndex = Math.Max(0, ActiveTabIndex - 1);
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
