using DevExpress.Blazor;
using BlazorApp.Models;

namespace BlazorApp.Services
{
    public class MdiStateService
    {
        public event Action? OnChange;

        public List<MDITab2> OpenTabs { get; } = new();
        public List<MDITab2> PopupWindows { get; } = new();
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
            var existingTab = OpenTabs.Find(t => t.Id == tabId) ?? PopupWindows.Find(t => t.Id == tabId);

            if (existingTab != null)
            {
                if (existingTab.IsPopup)
                    AttachPopupToTab(existingTab.Id);
                else
                    SetActiveTab(OpenTabs.IndexOf(existingTab));
            }
            else
            {
                var newTab = new MDITab2
                {
                    Id = tabId,
                    Title = title,
                    ComponentType = typeof(Nullable),
                    PageNM = componentPath
                };
                OpenTabs.Add(newTab);
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
            if (tabToClose != null)
            {
                int indexToClose = OpenTabs.IndexOf(tabToClose);
                OpenTabs.Remove(tabToClose);
                if (ActiveTabIndex >= indexToClose && ActiveTabIndex > 0)
                {
                    ActiveTabIndex--;
                }
            }
            else
            {
                var popupToClose = PopupWindows.FirstOrDefault(p => p.Id == tabId);
                if (popupToClose != null)
                {
                    PopupWindows.Remove(popupToClose);
                }
            }

            NotifyStateHasChanged();
        }

        // 탭을 팝업으로 전환
        public void DetachTabToPopup(string tabId)
        {
            var tabToDetach = OpenTabs.FirstOrDefault(t => t.Id == tabId);
            if (tabToDetach == null) return;

            tabToDetach.IsPopup = true;
            OpenTabs.Remove(tabToDetach);
            PopupWindows.Add(tabToDetach);

            if (ActiveTabIndex >= OpenTabs.Count && OpenTabs.Count > 0)
            {
                ActiveTabIndex = OpenTabs.Count - 1;
            }
            NotifyStateHasChanged();
        }

        // 팝업을 탭으로 전환
        public void AttachPopupToTab(string tabId)
        {
            var popupToAttach = PopupWindows.FirstOrDefault(p => p.Id == tabId);
            if (popupToAttach == null) return;

            popupToAttach.IsPopup = false;
            PopupWindows.Remove(popupToAttach);
            OpenTabs.Add(popupToAttach);
            ActiveTabIndex = OpenTabs.Count - 1; // 새로 추가된 탭을 활성화
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
