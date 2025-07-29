using BlazorApp.Components.Layout;
using BlazorApp.Models;
using DevExpress.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Routing;
using Microsoft.JSInterop;
using System.Collections.Generic;
using System.Linq;

namespace BlazorApp.Services
{
    public class MdiStateService
    {
        private readonly RouteTableService _routeTable;

        public event Action? OnChange;

        private readonly IJSRuntime _jsRuntime;
        private readonly NavigationManager _navigationManager;
        private readonly ThemeService _themeService;

        // --- 추가된 부분: 새 창으로 열린 탭의 전체 정보를 추적합니다. ---
        private readonly Dictionary<string, MDITab2> _detachedWindows = new();

        public MdiStateService(IJSRuntime jsRuntime, NavigationManager navigationManager, ThemeService themeService, RouteTableService routeTable)
        {
            _jsRuntime = jsRuntime;
            _navigationManager = navigationManager;
            _themeService = themeService;
            _routeTable = routeTable;
        }

        public List<MDITab2> OpenTabs { get; } = new();
        public List<MDITab2> PopupWindows { get; } = new();
        public int ActiveTabIndex { get; private set; }

        //public void OpenTab(string title, Type componentType)
        //{
        //    var tabId = componentType.Name;
        //    var existingTab = OpenTabs.FindIndex(t => t.Id == tabId);

        //    if (existingTab != -1)
        //    {
        //        ActiveTabIndex = existingTab;
        //    }
        //    else
        //    {
        //        OpenTabs.Add(new MDITab2
        //        {
        //            Id = tabId,
        //            Title = title,
        //            ComponentType = componentType,
        //            PageNM = ""
        //        });
        //        ActiveTabIndex = OpenTabs.Count - 1;
        //    }
        //    NotifyStateHasChanged();
        //}

        public void OpenTab(string tabId, string title, string componentPath)
        {
            if (OpenTabs.Any(t => t.Id == tabId))
            {
                SetActiveTab(OpenTabs.FindIndex(t => t.Id == tabId));
                return;
            }

            if (_detachedWindows.ContainsKey(tabId))
            {
                _jsRuntime.InvokeVoidAsync("alert", $"'{title}'은(는) 이미 새 창으로 열려 있습니다.");
                return;
            }

            //var componentType = Type.GetType(componentPath);
            // 1. routePath로 Component Type을 조회
            var componentType = _routeTable.GetTypeForRoute(componentPath);

            if (componentType == null)
            {
                // 컴포넌트 타입을 찾을 수 없는 경우 처리
                System.Console.WriteLine($"Error: Component type not found for '{componentPath}'");
                return;
            }

            var newTab = new MDITab2
            {
                Id = tabId,
                Title = title,
                ComponentType = typeof(Nullable),
                PageNM = componentPath,
                Content = builder => { } // 임시 초기화
            };

            // RenderFragment를 동적으로 생성합니다.
            // 이 RenderFragment는 PageLayout으로 감싸진 실제 페이지 컴포넌트를 포함합니다.
            newTab.Content = builder =>
            {
                builder.OpenComponent(0, typeof(PageLayout));
                builder.AddAttribute(1, "ChildContent", (RenderFragment)(builder2 => {
                    builder2.OpenComponent(0, componentType);
                    // 페이지에 파라미터를 전달해야 할 경우 여기에 추가
                    // builder2.AddAttribute(1, "ParameterName", parameterValue);

                    // @ref를 통해 컴포넌트 인스턴스를 받아와서 newTab 모델에 저장합니다.
                    builder2.AddComponentReferenceCapture(2, inst => { newTab.ComponentInstance = inst; });
                    builder2.CloseComponent();
                }));
                builder.CloseComponent();
            };

            OpenTabs.Add(newTab);
            SetActiveTab(OpenTabs.Count - 1);

            NotifyStateHasChanged();
        }

        // --- 추가된 메서드: 새 창을 다시 탭으로 전환합니다. ---
        public void AttachWindowToTab(string tabId)
        {
            if (_detachedWindows.TryGetValue(tabId, out var tabToAttach))
            {
                _detachedWindows.Remove(tabId);
                OpenTabs.Add(tabToAttach);
                SetActiveTab(OpenTabs.Count - 1);
            }
        }

        // --- 수정된 메서드: 탭을 새 창으로 전환합니다. ---
        public async void DetachTabToNewWindow(string tabId)
        {
            var tabToDetach = OpenTabs.FirstOrDefault(t => t.Id == tabId);
            if (tabToDetach == null || string.IsNullOrEmpty(tabToDetach.PageNM)) return;

            _detachedWindows[tabId] = tabToDetach; // 추적 목록에 추가
            OpenTabs.Remove(tabToDetach);

            // 새 창에서 탭 복원을 위해 필요한 정보를 URL 파라미터로 전달합니다.
            var componentName = tabToDetach.PageNM;
            var url = _navigationManager.ToAbsoluteUri($"/PopupPage?componentName={componentName}&tabId={tabId}&title={tabToDetach.Title}").ToString();

            await _jsRuntime.InvokeVoidAsync("openAsPopup", url, tabId);

            NotifyStateHasChanged();
        }

        // --- 추가된 메서드: 새 창이 닫혔을 때 추적 정보를 삭제합니다. ---
        public void ClearDetachedWindowInfo(string tabId)
        {
            if (_detachedWindows.ContainsKey(tabId))
            {
                _detachedWindows.Remove(tabId);
                // 이 변경 사항은 UI에 직접적인 영향을 주지 않으므로 NotifyStateHasChanged() 호출은 선택 사항입니다.
            }
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
            //else
            //{
            //    var popupToClose = PopupWindows.FirstOrDefault(p => p.Id == tabId);
            //    if (popupToClose != null)
            //    {
            //        PopupWindows.Remove(popupToClose);
            //    }
            //}

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
