using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Runtime.Remoting.Messaging;
using Priority_Queue;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Random = System.Random;

namespace DockGUI
{
    public class DockLayout : DockContainer
    {
        protected DockContainer _flexContainer;
        protected DockContainer _dockedContainer;

        public DockLayout DockLayoutParent => (DockLayout)parent;
        
        public DockContainer DockedContainer => _dockedContainer;
        public DockContainer FlexContainer => _flexContainer;
        
        public bool IsDockedLayout => HasDockLayoutParent && DockLayoutParent.DockedContainer == this;
        public bool IsFlexLayout => HasDockLayoutParent && DockLayoutParent.FlexContainer == this;

        public bool HasDockLayoutParent {
            get
            {
                if (parent == null)
                {
                    return false;
                }

                return parent.GetType() == typeof(DockLayout);
            }
        }
        
        // public List<DockPanel> DockPanels
        // {
        //     get
        //     {
        //         List<DockPanel> result = new List<DockPanel>();
        //         foreach (var child in Children())
        //         {
        //             if (child.GetType() == typeof(DockPanel))
        //             {
        //                 result.Add((DockPanel)child);
        //             }
        //         }
        //
        //         return result;
        //     }
        // }

        public DockLayout() //: base(DockGUIStyles.DefaultStyle)
        {
            AddToClassList("DockLayout");
        }

        // public DockLayout(DockPanelLayout flexPanelLayout) : this (flexPanelLayout, DockGUIStyles.DefaultStyle) { }
        //
        // public DockLayout(DockPanelLayout flexPanelLayout, StyleSheet styleSheet) : this(styleSheet)
        // {
        //     _flexLayout = flexPanelLayout;
        // }
        //
        // public DockLayout(DockPanelLayout dockedPanelLayout, DockPanelLayout flexPanelLayout, StyleSheet styleSheet) : this(styleSheet)
        // {
        //     _flexLayout = flexPanelLayout;
        //     _dockedLayout = dockedPanelLayout;
        // }
        
        public void CreatePanelLayout(DockPanel dockPanel)
        {
            DockPanelLayout dockPanelLayout = new DockPanelLayout();
            dockPanelLayout.AddPanel(dockPanel);
            SetFlexContainer(dockPanelLayout);
            
            Add(dockPanelLayout);
        }
        
        private void ConvertDockedToFlexLayout()
        {
            if (_dockedContainer == null)
            {
                throw new Exception("Cannot convert docked to flex layout, it is null");
            }
            
            UnsetDockingClass();
        
            _flexContainer = _dockedContainer;
            _flexContainer.AddToClassList("DockLayoutFlex");
        }

        public void RemoveContainer(DockContainer container)
        {
            if (container == _flexContainer)
            {
                _flexContainer = null;
                Remove(container);
                
                if (_dockedContainer != null)
                {
                    SetFlexContainer(_dockedContainer);
                    _dockedContainer.UnsetDockingClass();
                    _dockedContainer.AddToClassList("DockLayoutFlex");
                    _dockedContainer = null;
                }
            }
            else if (container == _dockedContainer)
            {
                Remove(container);
                _dockedContainer = null;
            }
            else
            {
                throw new Exception("DockPanelLayout being removed is not a dockedContainer nor a flexContainer!");
            }

            if (_dockedContainer == null && _flexContainer == null)
            {
                DockLayoutParent?.RemoveContainer(this);
            }
        }

        public void RemoveLayout(DockLayout dockLayout)
        {
            if (dockLayout == _flexContainer)
            {
                if (_dockedContainer != null)
                {
                    ConvertDockedToFlexLayout();
                }
                else
                {
                    _flexContainer = null;
                }
            }
            else if (dockLayout == _dockedContainer)
            {
                _dockedContainer = null;
            }
            
            Debug.Log("Removing " + dockLayout.name + " from " + name);
            
            base.Remove(dockLayout);
            
            if (HasDockLayoutParent)
            {
                DockLayout dockLayoutParent = DockLayoutParent;

                if (_dockedContainer == null && _flexContainer == null)
                {
                    dockLayoutParent.RemoveLayout(this);
                }
                
                // if (dockLayoutParent.childCount == 1)
                // {
                //     foreach (var dockPanel in DockPanels)
                //     {
                //         dockLayoutParent.AddPanel(dockPanel);
                //         dockLayoutParent.RemoveLayout(this);
                //     }
                // }
            }
        }

        public void Expand(DockPanelLayout dockPanelLayout, Border border)
        {
            // DockPanelLayout dockLayoutParent = dockPanel.DockPanelLayoutParent;
            //
            // if (dockLayoutParent != null) 
            // {
            //     // remove the panel from a docklayout that has at least one other panel inside  
            //     if (dockLayoutParent.DockPanels.Count > 1)
            //     {
            //         dockLayoutParent.RemovePanel(dockPanel);
            //     }
            //
            //     if (IsDockedLayout || IsFlexLayout)
            //     {
            //         dockLayoutParent.RemovePanel(dockPanel);
            //     }
            // }
            
                        

            // VisualElement oldParent = parent;
            //
            // DockLayout newDockLayout = new DockLayout {name = "FlexLayout"};
            // DockPanelLayout dockedPanelLayout = dockPanel.HasDockLayoutParent ? dockPanel.DockPanelLayoutParent : new DockPanelLayout();
            //
            // newDockLayout.SetContainers(dockedPanelLayout, this);
            //
            // DockPanelLayout dockingLayout = new DockPanelLayout();
            // dockingLayout.AddPanel(dockPanel);
            // dockingLayout.RemoveFromClassList("DockLayoutFlex");
            //
            // if (!(oldParent is DockLayout))
            // {
            //     DockLayout newDockLayoutParent = new DockLayout();
            //     newDockLayoutParent.name = "NewDockLayoutParent";
            //     parent.Add(newDockLayoutParent);
            //     newDockLayoutParent.SetFlexContainer(newDockLayout);
            // }
            // else
            // {
            //     Debug.Log("Replacing");
            //     DockLayout oldDockLayoutParent = (DockLayout) oldParent;
            //     oldDockLayoutParent.SetFlexContainer(newDockLayout);
            // }

            switch (border)
            {
                case Border.Top:
                    style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Column);
                    // newPanelLayout.style.height = new StyleLength(new Length(DockGUI.DOCK_LAYOUT_RATIO * 100, LengthUnit.Percent));
                    // dockingLayout.SetDockingClass("DockLayoutTop");
                    break;
                
                case Border.Left:
                    style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);
                    // newPanelLayout.style.width = new StyleLength(new Length(DockGUI.DOCK_LAYOUT_RATIO * 100, LengthUnit.Percent));
                    // dockingLayout.SetDockingClass("DockLayoutLeft");
                    break;
            }
            
            // dockingLayout.RemoveFromClassList("DockLayoutFlex");
            // Add(dockingLayout);
            // Add(newDockLayout);

            // return dockingLayout;
        }

        private void SetContainers(DockContainer dockedContainer, DockContainer flexContainer)
        {
            if (_dockedContainer != null || _flexContainer != null)
            {
                Debug.LogWarning("This DockLayout already contains DockPanelLayouts. Removing them.");
            }

            SetDockedContainer(dockedContainer);
            SetFlexContainer(flexContainer);
        }

        public void SetDockedContainer(DockContainer dockedContainer)
        {
            if (_dockedContainer != null)
            {
                Debug.LogWarning("This DockLayout already contains a dockedPanelLayout. Replacing it.");
            }

            _dockedContainer = dockedContainer;
            Insert(0, _dockedContainer);
        }
        
        public void SetFlexContainer(DockContainer flexContainer)
        {
            if (_flexContainer != null)
            {
                Debug.LogWarning("This DockLayout already contains a flexContainer. Replacing.");
            }

            _flexContainer = flexContainer;
            Add(_flexContainer);
        }

        public void ReplaceContainer(DockContainer original, DockContainer newContainer)
        {
            int index = IndexOf(original);
            Insert(index, newContainer);
            Remove(original);
            
            if (_flexContainer == original)
            {
                _flexContainer = newContainer;
            }
            else if (_dockedContainer == original)
            {
                _dockedContainer = newContainer;
            }
            else
            {
                throw new Exception("Cannot replace container, it isn't currently a flex or docked container");
            }
        }

        
    }
}