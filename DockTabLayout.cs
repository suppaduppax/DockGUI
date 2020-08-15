using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UIElements;

namespace DockGUI
{

    public class DockTabLayout : VisualElement, IDroppable
    {
        public List<DockTab> tabs;

        private Dictionary<DockPanel, DockTab> _panelToTabDict;
        private Dictionary<DockTab, DockPanel> _tabToPanelDict;

        private DockTab _selectedTab;

        private VisualElement _flexSpace;
        private VisualElement _tabLayoutBg;

        public DockLayout DockLayoutParent => (DockLayout) parent;
        public bool IsFloating => DockLayoutParent.IsFloating;

        public VisualElement TargetElement => this;

        public DockTabLayout()
        {
            tabs = new List<DockTab>();
            _panelToTabDict = new Dictionary<DockPanel, DockTab>();
            _tabToPanelDict = new Dictionary<DockTab, DockPanel>();

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

        public DockTab GetTab(DockPanel dockPanel)
        {
            return _panelToTabDict[dockPanel];
        }

        public DockTab AddTab(DockPanel targetPanel)
        {
            DockTab tab = new DockTab(targetPanel,this);
            tab.text = targetPanel.Title;
            
            _panelToTabDict.Add(targetPanel, tab);
            _tabToPanelDict.Add(tab, targetPanel);

            if (targetPanel.DockLayoutParent != null)
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

        public void MoveTab(int index, DockTab tab)
        {
            int curIndex = tabs.IndexOf(tab);
            if (curIndex == index)
            {
                return;
            }
            
            tabs.Remove(tab);
            tabs.Insert(index, tab);
            
            
            Remove(tab);
            Insert(index + 1, tab);
        }
        
        public void RemoveTab(DockPanel targetPanel)
        {
            RemoveTab(_panelToTabDict[targetPanel]);
        }

        public void RemoveTab(DockTab tab)
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

        public void OnTabClicked(DockTab dockTab)
        {
            if (_selectedTab == dockTab)
            {
                return;
            }
            
            Select(dockTab);
        }

        public void Select(DockPanel dockPanel)
        {
            Select(_panelToTabDict[dockPanel]);
        }
        
        private void Select(DockTab tab)
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

        private void Deselect(DockTab tab)
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

        public DockPanel GetPanel(DockTab tab)
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