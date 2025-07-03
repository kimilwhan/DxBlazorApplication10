using DevExpress.Blazor;
using BlazorApp.Models;
using System.Collections;
using System.Text.Json.Serialization;

namespace BlazorApp.Services
{
    public class MDITabCollection {
        [JsonInclude]
        public List<MDITab> tabs;

        public int Count => tabs.Count;

        public MDITabCollection() {
            tabs = new List<MDITab>();
        }

        public MDITabCollection(IEnumerable<ITabInfo> tabs) {
            this.tabs = tabs.Select(t => new MDITab(t.Text,  
                t.VisibleIndex == -1 ? tabs.ToList().IndexOf(t) : t.VisibleIndex,
                t.Visible)).ToList();
        }

        public void SetVisibleAllTabs(bool visible) {
            tabs.ForEach((t) => t.Visible = visible);
        }

        public string? GetTabTextByTabInfo(ITabInfo tabInfo) {
            return tabs.FirstOrDefault(t => t.Text == tabInfo.Text)?.Text;
        }

        public int GetVisibleIndexByTabText(string? text) {
            return tabs.Find(t => t.Text == text)?.VisibleIndex ?? -1;
        }

        public bool GetVisibleByTabText(string? text) {
            return tabs.Find(t => t.Text == text)?.Visible ?? true;
        }

        public void SetVisibleByTabText(string? text, bool visible) {
            var tab = tabs.Find(t => t.Text == text);
            if(tab != null)
            {
                tab.Visible = visible;
            }
        }

        public void SetVisibleIndexByTabText(string? text, int visibleIndex) {
            var tab = tabs.Find(t => t.Text == text);
            if(tab != null)
            {
                tab.VisibleIndex = visibleIndex;
            }
        }

        public void OpenTab(string title, string? componentTypeName = null)
        {
            var existing = tabs.FirstOrDefault(t => t.Text == title);

            if (existing == null)
            {
                tabs.Add(new MDITab(title, tabs.Count, true, componentTypeName));
                //ActiveTabIndex = Tabs.Count - 1;
            }
            //else
            //{
            //    activeTabIndex = tabs.IndexOf(existing);
            //}

            //OnTabsChanged?.Invoke();
        }

        public void CloseTab(string iD)
        {
            var tab = tabs.FirstOrDefault(t => t.Text == iD);

            if (tab != null)
            {
                tabs.Remove(tab);
                //ActiveTabIndex = Math.Max(0, Tabs.Count - 1);
                //OnTabsChanged?.Invoke();
            }
        }

        public void CloseAllTabs()
        {
            tabs.ForEach((t) => tabs.Remove(t));
        }
    }
}
