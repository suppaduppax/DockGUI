using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace DockGUI
{
    public class DockPanelLayout : DockContainer, IDroppable
    {
        // public DockLayout DockLayoutParent => (DockLayout) parent;
        
        private List<DockPanel> _dockPanels;
        private TabLayout _tabLayout;

        private bool _isFloating;


        public DockLayout DockLayoutParent => (DockLayout) parent;
        public TabLayout TabLayout => _tabLayout;
        public List<DockPanel> DockPanels => _dockPanels;

        public bool IsFloating
        {
            get => _isFloating;
            set => _isFloating = value;
        }

        public bool HasDockLayoutParent => parent is DockLayout;

        public VisualElement TargetElement => this;
        
        public DockPanelLayout() : this(DockGUIStyles.DefaultStyle) { }

        public DockPanelLayout(StyleSheet styleSheet) : base(styleSheet)
        {
            _dockPanels = new List<DockPanel>();
            AddToClassList("DockPanelLayout");
        }
              
        public void Dock(DockPanel sourcePanel, Border border)
        {
            if (sourcePanel.HasDockPanelLayoutParent)
            {
                Debug.Log("Removing panel from its original panelLayout");
                sourcePanel.DockPanelLayoutParent.RemovePanel(sourcePanel);
            }

            // create the new panelLayout for the docked panel 
            DockPanelLayout dockingPanelParent = new DockPanelLayout();
            dockingPanelParent.AddPanel(sourcePanel, false);

            // add a new docklayout as the parent of this DockPanelLayout
            DockLayout newParent = CreateDockLayoutParent();
            newParent.SetDockedContainer(dockingPanelParent);
            newParent.AddToClassList("DockLayoutFlex");
            
            switch (border)
            {
                case Border.Left:
                    newParent.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);
                    dockingPanelParent.AddToClassList("DockLayoutLeft");
                    break;
                
                case Border.Top:
                    newParent.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Column);
                    dockingPanelParent.AddToClassList("DockLayoutTop");
                    break;
                    
            }
            
            SetFlex();
        }

        private void SetFlex()
        {
            UnsetDockingClass();
            AddToClassList("DockLayoutFlex");
        }

        private void UnsetFlex()
        {
            if (ClassListContains("DockLayoutFlex"))
            {
                RemoveFromClassList("DockLayoutFlex");
            }
        }

        public DockLayout CreateDockLayoutParent()
        {
            DockLayout oldParent = DockLayoutParent;
            DockLayout newDockLayoutParent = new DockLayout();
            oldParent.ReplaceContainer(this, newDockLayoutParent);
            newDockLayoutParent.SetFlexContainer(this);

            return newDockLayoutParent;
        }
        
        public void AddPanel(DockPanel panel, bool flex = true)
        {
            _dockPanels.Add(panel);
            
            Add(panel);
            
            if (_tabLayout == null)
            {
                // adding to fullsize window and not a floating window
                style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Column);
                if (flex)
                {
                    AddToClassList("DockLayoutFlex");
                }

                CreateTabLayout(panel);
            }
            else
            {
                _tabLayout.AddTab(panel);
                // HidePanel(panel);
            }
        }

        public bool ContainsPanel(DockPanel dockPanel)
        {
            return _dockPanels.Contains(dockPanel);
        }
        
        private void CreateTabLayout(DockPanel dockPanel)
        {
            if (_tabLayout != null)
            {
                throw new Exception("DockTabLayout already exists for " + name);
            }
            
            _tabLayout = new TabLayout();
            _tabLayout.AddTab(dockPanel);
            _tabLayout.Select(dockPanel);

            _tabLayout.name = dockPanel.Title + "_TabLayout";
            
            Insert(0, _tabLayout);
        }
  
        
        public void RemovePanel(DockPanel dockPanel)
        {
                
            // remove the tab of this panel from the tabLayout
            if (_tabLayout.ContainsTab(dockPanel))
            {
                _tabLayout.RemoveTab(dockPanel);
                if (_tabLayout.tabs.Count == 0)
                {
                    Remove(_tabLayout);
                    _tabLayout = null;
                }
            }

            Remove(dockPanel);
            _dockPanels.Remove(dockPanel);
            
            if (_dockPanels.Count == 0 && HasDockLayoutParent)
            {
                Debug.Log("Removing dockPanelLayout from its dockLayout");
                DockLayoutParent.RemoveContainer(this);
            }
        }
    }

}