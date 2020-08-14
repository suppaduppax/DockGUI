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
    public class DockLayout : VisualElement, IDroppable
    {
        public enum State
        {
            None,
            Docked,
            Floating
        }
        
        public enum Direction
        {
            Vertical,
            Horizontal,
            ReverseVertical,
            ReverseHorizontal,
        }

        public State state;
        public bool isExpanded = false;
        public bool hasDockPanels = false;


        private Direction _expandDirection;
        private StyleSheet _styleSheet;

        private Dictionary<Border, DockPanel> _borderToPanelDict;
        private List<Border> _availableBorders;
        private DockPanel _currentPanel;
        private DockTabLayout _dockTabLayout;

        private VisualElement _rootElement;

        public DockTabLayout DockTabLayout => _dockTabLayout;
        public bool IsFloating => state == State.Floating;
        public DockLayout DockLayoutParent => (DockLayout) parent;

        public VisualElement TargetElement => this;
        
        public List<DockPanel> DockPanels
        {
            get
            {
                List<DockPanel> result = new List<DockPanel>();
                foreach (var child in Children())
                {
                    if (child.GetType() == typeof(DockPanel))
                    {
                        result.Add((DockPanel)child);
                    }
                }

                return result;
            }
        }
        
        public DockLayout() : this(DockGUIStyles.DefaultStyle) {}

        public DockLayout(StyleSheet styleSheet)
        {
            styleSheets.Add(styleSheet);
            _availableBorders = new List<Border>();
            AddToClassList("DockLayoutBg");
        }

        public void SetExpandDirection(Direction direction)
        {
            _expandDirection = direction;

            if (isExpanded)
            {
                Debug.LogWarning("Layout is already expanded when trying to set expand direction.");
                return;
            }
            
            switch (direction)
            {
                case Direction.Horizontal:
                    SetDockableBorder(Border.Left);
                    SetDockableBorder(Border.Right);
                    style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);
                    Debug.Log("SHOULD HAVE DON IT");
                    break;
                
                case Direction.Vertical:
                    SetDockableBorder(Border.Top);
                    SetDockableBorder(Border.Bottom);
                    style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Column);
                    break;
            }
        }
        
        public void SetDockableBorder(Border border)
        {
            if (!_availableBorders.Contains(border))
            {
                _availableBorders.Add(border);
            }

            switch (border)
            {
                case Border.Top:
                    style.borderTopColor = new StyleColor(Color.magenta);
                    style.borderTopWidth = new StyleFloat(2);
                    break;

                case Border.Left:
                    style.borderLeftColor = new StyleColor(Color.magenta);
                    style.borderLeftWidth = new StyleFloat(2);
                    break;

                case Border.Right:
                    style.borderRightColor = new StyleColor(Color.magenta);
                    style.borderRightWidth = new StyleFloat(2);
                    break;

                case Border.Bottom:
                    style.borderBottomColor = new StyleColor(Color.magenta);
                    style.borderBottomWidth = new StyleFloat(2);
                    break;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="dockPanel"></param>
        /// <param name="index">The border index, left to right or top to bottom</param>
        public void Expand(DockPanel dockPanel, int index)
        {
            if (hasDockPanels)
            {
                // move all dockpanels to a new docklayout to expand
                DockLayout moveToLayout = new DockLayout();
                DockGUIStyles.SetDefaultLayoutSize(ref moveToLayout, _expandDirection);
                
                var children = new List<VisualElement>(Children());
                foreach (var element in children)
                {
                    moveToLayout.Add(element);
                }
                
                Add(moveToLayout);
                
                hasDockPanels = false;
            }
            DockLayout dockLayoutParent = dockPanel.DockLayoutParent;
            if (dockLayoutParent != null)
            {
                Debug.Log("Removing " + dockPanel.name + " from " + dockLayoutParent);
                dockPanel.DockLayoutParent.Remove(dockPanel);
            }
            
            DockLayout dockLayout = new DockLayout();
            DockGUIStyles.SetDefaultLayoutSize(ref dockLayout, _expandDirection);
            
            Add(dockLayout);

            Insert(index, dockLayout);
            dockLayout.Add(dockPanel);

            CalculateSize();
            
            isExpanded = true;
        }

        private void Remove(DockPanel dockPanel)
        {
            base.Remove(dockPanel);
            return;

            if (childCount > 0)
            {
                Debug.Log("Calculating size for " + name);
                CalculateSize();
            }
            else
            {
                DockLayoutParent.Remove(this);
            }
        }

        private void Remove(DockLayout dockLayout)
        {
            Debug.Log("Removing " + dockLayout.name + " from " + name);
            base.Remove(dockLayout);
            
            return;
            
            if (childCount == 0)
            {
                Debug.Log("Removing " + name);
                DockLayoutParent.Remove(this);
            }
            else
            {
                
                Debug.Log("Calculating size for " + name);
                CalculateSize();
            }
        }
        
        private void CalculateSize()
        {
            switch (_expandDirection)
            {
                case Direction.Horizontal:
                    // increase the layout size to expand the new panel
                    style.width = new StyleLength(StyleKeyword.Auto);
                    break;
                
                case Direction.Vertical:
                    style.height = new StyleLength(StyleKeyword.Auto);
                    break;
            }
            // switch (_expandDirection)
            // {
            //     case Direction.Horizontal:
            //         // increase the layout size to expand the new panel
            //         style.width = new StyleLength(new Length(DockGUIStyles.DEFAULT_LAYOUT_SIZE * childCount, DockGUIStyles.DEFAULT_LAYOUT_SIZE_UNIT));
            //         break;
            //     
            //     case Direction.Vertical:
            //         style.height = new StyleLength(new Length(DockGUIStyles.DEFAULT_LAYOUT_SIZE * childCount, DockGUIStyles.DEFAULT_LAYOUT_SIZE_UNIT));
            //         break;
            // }
        }

        public void Float()
        {
            
        }
        
        public void DetachAndFloatPanel(DockPanel dockPanel)
        {
            
        }

        public VisualElement GetRootElement()
        {
            if (parent == null)
            {
                return null;
            }

            VisualElement nextParent = parent;
            VisualElement curParent = null;
            
            while (nextParent != null)
            {
                curParent = nextParent;
                if (curParent != null && curParent.GetType() != typeof(DockLayout))
                {
                    break;
                }
                nextParent = nextParent.parent;
            }

            return curParent;
        }
       
        public void Add(DockPanel panel)
        {
            hasDockPanels = true;

            if (panel.DockLayoutParent != null)
            {
                if (panel.DockLayoutParent.state == State.Floating && panel.DockLayoutParent.DockPanels.Count == 1)
                {
                    panel.DockLayoutParent.parent.Remove(panel.DockLayoutParent);
                }

                panel.DockLayoutParent.Remove(panel);
                
            }
            
            base.Add(panel);
            
            if (_dockTabLayout == null)
            {
                style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Column);
                CreateTabLayout(panel);
                SetCurrentPanel(panel);
            }
            else
            {
                _dockTabLayout.AddTab(panel);
                HidePanel(panel);
            }
            
            
        }

        public void CreateTabLayout(DockPanel dockPanel)
        {
            if (_dockTabLayout != null)
            {
                throw new Exception("DockTabLayout already exists for " + name);
            }
            
            _dockTabLayout = new DockTabLayout();
            _dockTabLayout.AddTab(dockPanel);
            _dockTabLayout.Select(dockPanel);

            _dockTabLayout.name = dockPanel.Title + "_TabLayout";
            
            Insert(0, _dockTabLayout);
            
        }

        private new void Add(VisualElement element)
        {
            throw  new Exception();
        }

        private void SetCurrentPanel(DockPanel dockPanel)
        {
            if (_currentPanel != null)
            {
                HidePanel(_currentPanel);
            }

            dockPanel.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
            _currentPanel = dockPanel;
        }

        private void HidePanel(DockPanel dockPanel)
        {
            dockPanel.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
        }

        public bool HasDockLayoutParent()
        {
            if (parent == null)
            {
                return false;
            }

            return parent.GetType() == typeof(DockLayout);
        }
        

        //
        // private void Remove(DockPanel dockPanel)
        // {
        //     // DockLayout dockLayoutParent = dockPanel.DockLayoutParent;
        //     throw new NotImplementedException();
        //     
        // }

        //
        // public void AddDockPanel(DockPanel dockPanel, Border border)
        // {
        //     if (!_availableBorders.Contains(border))
        //     {
        //         throw new Exception("Trying to dock unavailable border: " + border);
        //     }
        //     
        //     switch (border)
        //     {
        //         case Border.Left:
        //             _borderToPanelDict.Add(border, dockPanel);
        //             _dockedPanels.Add(dockPanel);
        //             Add(dockPanel);
        //             break;
        //         
        //         default:
        //             throw new Exception("Invalid border to dock: " + border);
        //     }
        // }

        // public void AddDockLayout(DockLayout dockLayout, Border border)
        // {
        //     if (hasDockPanels)
        //     {
        //         throw new Exception("Cannot dock a layout to another that already has DockPanels as children");
        //     }
        //
        //     if (!_availableBorders.Contains(border))
        //     {
        //         throw new Exception("Cannot dock layout to unavailable border: " + border);
        //     }
        //
        //     switch (border)
        //     {
        //         case Border.Top:
        //             DockGUIStyles.DockLayoutTop(ref dockLayout);
        //             break;
        //     }
        //     
        //     Add(dockLayout);
        // }

    }
}