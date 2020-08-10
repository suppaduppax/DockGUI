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

        public DockLayout DockLayoutParent => (DockLayout)parent;
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
            
            RegisterCallback<MouseEnterEvent>(OnMouseEnter);
        }

        private void OnMouseEnter(MouseEnterEvent evt)
        {
            Debug.Log("ENTER TABLAYOUT");
        }

        public void AddTab(DockPanel targetPanel)
        {
            DockTab tab = new DockTab(this);
            tab.text = targetPanel.Title;
            
            _panelToTabDict.Add(targetPanel, tab);
            _tabToPanelDict.Add(tab, targetPanel);
            
            tabs.Add(tab);
            
            Deselect(tab);
            Add(tab);
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
            Remove(tab);
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

        public DockPanel GetPanel(DockTab tab)
        {
            return _tabToPanelDict[tab];
        }

    }
}