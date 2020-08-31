using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UIElements;

namespace DockGUI
{

    public class TabLayout : VisualElement, IDroppable
    {
        public List<Tab> tabs;

        private Dictionary<DockPanel, Tab> _panelToTabDict;
        private Dictionary<Tab, DockPanel> _tabToPanelDict;

        private Tab _selectedTab;

        private VisualElement _flexSpace;
        private VisualElement _tabLayoutBg;

        public DockPanelLayout DockPanelLayoutParent => (DockPanelLayout) parent;

        public VisualElement TargetElement => this;

        public TabLayout()
        {
            tabs = new List<Tab>();
            _panelToTabDict = new Dictionary<DockPanel, Tab>();
            _tabToPanelDict = new Dictionary<Tab, DockPanel>();

            styleSheets.Add(DockGUIStyles.DefaultStyle);
            AddToClassList("TabLayoutHeight");
            AddToClassList("TabLayout");

            _tabLayoutBg = new VisualElement();
            _tabLayoutBg.styleSheets.Add(DockGUIStyles.DefaultStyle);

            _tabLayoutBg.AddToClassList("TabLayoutBg");
            Add(_tabLayoutBg);

            RegisterCallback<MouseDownEvent>(OnMouseDown);
        }

        private void OnMouseDown(MouseDownEvent evt)
        {
        }

        public bool ContainsTab(DockPanel dockPanel)
        {
            return _panelToTabDict.ContainsKey(dockPanel);
        }

        public Tab GetTab(DockPanel dockPanel)
        {
            return _panelToTabDict[dockPanel];
        }

        public Tab AddTab(DockPanel targetPanel)
        {
            Tab tab = new Tab(targetPanel,this);
            tab.text = targetPanel.Title;
            
            _panelToTabDict.Add(targetPanel, tab);
            _tabToPanelDict.Add(tab, targetPanel);

            if (targetPanel.DockPanelLayoutParent != null)
            {
                // if (targetPanel.DockLayoutParent.parent != null)
                // {
                //     targetPanel.DockLayoutParent.DockLayoutParent.Remove(targetPanel.DockLayoutParent);
                // }

                // targetPanel.DockLayoutParent.Remove(targetPanel);
            }
            
            tabs.Add(tab);
            Deselect(tab);
            Add(tab);

            return tab;
        }

        public void MoveTab(int index, Tab tab)
        {
            int curIndex = tabs.IndexOf(tab);
            if (curIndex == index)
            {
                return;
            }
            
            tabs.Remove(tab);
            if (index > tabs.Count)
            {
                tabs.Add(tab);
            }
            else
            {
                tabs.Insert(index, tab);
            }

            Remove(tab);
            Insert(index + 1, tab);
        }
        
        public void RemoveTab(DockPanel targetPanel)
        {
            RemoveTab(_panelToTabDict[targetPanel]);
        }

        public void RemoveTab(Tab tab)
        {
            DockPanel dockPanel = _tabToPanelDict[tab];
            
            _panelToTabDict.Remove(dockPanel);
            _tabToPanelDict.Remove(tab);
            
            tabs.Remove(tab);
            Remove(tab);
            
            // select the first tab if we remove the currently selected tab
            if (_selectedTab == tab && tabs.Count > 0)
            {
                _selectedTab = null;
                Select(tabs[0]);
            }
        }

        public void OnTabClicked(Tab tab)
        {
            if (_selectedTab == tab)
            {
                return;
            }
            
            Select(tab);
        }

        public void Select(DockPanel dockPanel)
        {
            Select(_panelToTabDict[dockPanel]);
        }
        
        private void Select(Tab tab)
        {
            if (_selectedTab != null)
            {
                Deselect(_selectedTab);
            }

            _tabToPanelDict[tab].style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
            
            tab.Highlight(true);
            _selectedTab = tab;
        }

        public void Deselect(DockPanel dockPanel)
        {
            Deselect(_panelToTabDict[dockPanel]);
        }

        private void Deselect(Tab tab)
        {
            tab.Highlight(false);
            if (!_tabToPanelDict.ContainsKey(tab))
            {
                Debug.Log(tab.text);
                foreach (var key in _tabToPanelDict.Keys)
                {
                    Debug.Log(key.text);
                }
            }

            _tabToPanelDict[tab].style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
        }

        private new void Add(VisualElement element)
        {
            base.Add(element);
        }

        public DockPanel GetPanel(Tab tab)
        {
            if (!_tabToPanelDict.ContainsKey(tab))
            {
                Debug.Log("Looking for" + tab.text);
                foreach (var kvp in _tabToPanelDict)
                {
                    Debug.Log(kvp.Key.text);
                    Debug.Log(kvp.Key == tab);
                }
            }
            return _tabToPanelDict[tab];
        }

    }
}