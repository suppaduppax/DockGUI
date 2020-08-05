using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UIElements;

namespace DockGUI
{

    public class DockTabLayout : VisualElement
    {
        public List<DockTab> tabs;

        private Dictionary<DockPanel, DockTab> _panelToTabDict;
        private Dictionary<DockTab, DockPanel> _tabToPanelDict;

        private DockTab _selectedTab;

        private VisualElement _flexSpace;
        
        public DockTabLayout()
        {
            tabs = new List<DockTab>();
            _panelToTabDict = new Dictionary<DockPanel, DockTab>();
            _tabToPanelDict = new Dictionary<DockTab, DockPanel>();
            
            style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);
            
            
            _flexSpace = new VisualElement();
            _flexSpace.name = "TabFlexSpace";
            _flexSpace.style.flexGrow = 1;
            
            Add(_flexSpace);
        }

        public void AddTab(DockPanel targetPanel)
        {
            DockTab tab = new DockTab(this);
            tab.text = targetPanel.Title;
            
            _panelToTabDict.Add(targetPanel, tab);
            _tabToPanelDict.Add(tab, targetPanel);
            
            tabs.Add(tab);
            
            Deselect(tab);
            Insert(childCount - 1, tab);
        }

        public void RemoveTab(DockPanel targetPanel)
        {
            RemoveTab(_panelToTabDict[targetPanel]);
        }

        public void RemoveTab(DockTab tab)
        {
            if (_selectedTab == tab && tabs.Count > 1)
            {
                Select(tabs[0]);
            }

            DockPanel dockPanel = _tabToPanelDict[tab];
            
            _panelToTabDict.Remove(dockPanel);
            _tabToPanelDict.Remove(tab);
            
            tabs.Remove(tab);
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
            // tab.style.backgroundColor = new StyleColor(Color.white);
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
            _tabToPanelDict[tab].style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
        }

        private new void Add(VisualElement element)
        {
            base.Add(element);
        }
    }
}