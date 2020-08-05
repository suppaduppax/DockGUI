using System;
using System.Collections;
using System.Collections.Generic;
using Priority_Queue;
using UnityEngine;
using UnityEngine.UIElements;

namespace DockGUI
{
    public class DockLayout : VisualElement
    {
        public enum DockType
        {
            Fixed,
            Stretch
        }
        
        public enum Direction
        {
            Vertical,
            Horizontal,
            ReverseVertical,
            ReverseHorizontal,
        }

        public bool isExpanded = false;

        private Direction _expandDirection;
        
        private StyleSheet _styleSheet;
        
        private List<DockPanel> _dockedPanels;
        private Dictionary<Border, DockPanel> _borderToPanelDict;

        private List<Border> _availableBorders;

        public bool hasDockPanels = false;
        private DockPanel _currentPanel;

        private DockTabLayout _dockTabLayout;

        public DockLayout DockLayoutParent => (DockLayout) parent;

        public DockLayout() : this(DockGUIStyles.DefaultStyle) { }
        
        public DockLayout(StyleSheet styleSheet)
        {
            _styleSheet = styleSheet;
            _availableBorders = new List<Border>();
            
            styleSheets.Add(styleSheet);
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
            Debug.Log("Removing panel: " + dockPanel + " from " + name);
            
            base.Remove(dockPanel);

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

        public void Add(DockPanel panel)
        {
            hasDockPanels = true;
            base.Add(panel);

            if (childCount == 1)
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

        private void CreateTabLayout(DockPanel dockPanel)
        {
            if (_dockTabLayout != null)
            {
                throw new Exception("DockTabLayout already exists for " + name);
            }
            
            _dockTabLayout = new DockTabLayout();
            _dockTabLayout.AddTab(dockPanel);
            _dockTabLayout.Select(dockPanel);
            
            Insert(0, _dockTabLayout);
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