using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Runtime.Remoting.Messaging;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace DockGUI
{
    public class DockPanelGhost : VisualElement, IDraggable
    {
        public enum GhostState
        {
            None,
            TabLayout,
            Window,
            DockPanelLayout,
        }

        public enum DockingState
        {
            None,
            Left,
            Right,
            Top,
            Bottom
        }

        private GhostState _state = GhostState.None;
        private DockingState _dockingState = DockingState.None;
        
        private VisualElement _floatingWindow;
        private VisualElement _floatingTab;
        private VisualElement _proxyTab;
        
        private DockLayout _rootDockLayout;
        private Vector2 _mouseOffset;

        // private DockTabLayout _tabLayout;
        private DockPanel _targetDockPanel;
        
        private TabLayout _dropTabLayout;
        private DockPanelLayout _dropPanelLayout;
        private DockPanelLayout _dockingPanelLayout;
        
        private List<IDroppable> _droppables;

        public VisualElement TargetElement => this;

        private IDroppable _oldDroppable;
        private Color _oldColour;

        public DockPanelGhost(DockPanel dockPanel, Vector2 mouseOffset)
            : this(dockPanel, mouseOffset, DockGUIStyles.DefaultStyle)
        {
        }

        public DockPanelGhost(DockPanel dockPanel, Vector2 mouseOffset,
            StyleSheet styleSheet)
        {
            _targetDockPanel = dockPanel;           
            _rootDockLayout = dockPanel.DockPanelLayoutParent.GetRootDockLayout();
            _mouseOffset = mouseOffset;

            _targetDockPanel.DockPanelLayoutParent.TabLayout.GetTab(dockPanel).visible = false;
            _targetDockPanel.DockPanelLayoutParent.TabLayout.GetTab(dockPanel).style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);

            // _targetPanel.DockLayoutParent.visible = false;
            
            _droppables = DragAndDrop.GetAllDroppables(_rootDockLayout);

            // create ghost panel ----
            styleSheets.Add(styleSheet);
            AddToClassList("GhostLayout");

            _floatingWindow = DockGUI.CreateVisualElement(this, styleSheet, "FloatingLayout", "DockLayoutBg");

            VisualElement floatingWindowTabLayout =
                DockGUI.CreateVisualElement(_floatingWindow, styleSheet, "TabLayoutHeight", "TabLayout");
            
            DockGUI.CreateVisualElement(floatingWindowTabLayout, styleSheet, "TabLayoutBg");
            VisualElement floatingWindowTab =
                DockGUI.CreateVisualElement(floatingWindowTabLayout, styleSheet, "TabHighlight", "TabPadding");
            
            floatingWindowTab.Add(new Label(dockPanel.Title));
            DockGUI.CreateVisualElement(_floatingWindow, styleSheet).style.flexGrow = new StyleFloat(1);

            _floatingWindow.style.opacity = new StyleFloat(0.5f);
            _floatingWindow.visible = false;
            // -----------------------

            // create ghost tab ------
            _floatingTab = DockGUI.CreateVisualElement(this, styleSheet, "TabLayoutHeight", "TabLayout");
            VisualElement _floatingTabButton =
                DockGUI.CreateVisualElement(_floatingTab, styleSheet, "TabHighlight", "TabPadding");
            _floatingTabButton.Add(new Label(dockPanel.Title));
            // -----------------------
            
            
            // create proxy tab
            _proxyTab = DockGUI.CreateVisualElement(this, styleSheet, "TabLayoutHeight", "TabLayout");
            VisualElement _proxyTabButton =
                DockGUI.CreateVisualElement(_proxyTab, styleSheet, "TabHighlight", "TabPadding");
            _proxyTabButton.Add(new Label(dockPanel.Title));

            Tab tab = dockPanel.DockPanelLayoutParent.TabLayout.GetTab(dockPanel);
            
            _proxyTab.visible = false;
            _proxyTab.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
            
            dockPanel.DockPanelLayoutParent.TabLayout.Insert(dockPanel.DockPanelLayoutParent.TabLayout.IndexOf(tab), _proxyTab);
            
            RegisterCallback<MouseUpEvent>(OnMouseUp);
            RegisterCallback<MouseMoveEvent>(OnMouseMove);
        }

        private void OnMouseMove(MouseMoveEvent evt)
        {
            if (DragAndDrop.State != DragAndDrop.DragAndDropState.Dragging && DragAndDrop.DraggingObject != this)
            {
                return;
            }
            
            float x = evt.mousePosition.x - _mouseOffset.x + DockGUI.WORLD_WINDOW_OFFSET_X;
            float y = evt.mousePosition.y -  _mouseOffset.y + DockGUI.WORLD_WINDOW_OFFSET_Y;
            
            List<IDroppable> droppablesUnderMouse = DragAndDrop.GetDroppablesUnderMouse(evt.mousePosition);
            IDroppable droppable = droppablesUnderMouse.Count > 0 ? droppablesUnderMouse[droppablesUnderMouse.Count - 1] : null;

            if (_oldDroppable != null)
            {
                _oldDroppable.TargetElement.style.backgroundColor = new StyleColor(_oldColour);
            }

            _oldDroppable = droppable;

            if (droppable != null)
            {
                droppable.TargetElement.style.backgroundColor = new StyleColor(Color.magenta);
            }
            
            switch (_state)
            {
                case GhostState.TabLayout:
                    DragAndDrop.RefreshDroppables();

                    if (droppable != null && droppable == _dropTabLayout)
                    {
                        OnTabLayoutUpdate(new Vector2(x, y));
                    }
                    else
                    {
                        OnTabLayoutLeave(droppable, evt);
                    }
                    break;
                
                case GhostState.DockPanelLayout:
                    if (droppable != null && droppable == _dropPanelLayout)
                    {
                        OnDockPanelLayoutUpdate(new Vector2(x, y));
                    }
                    else
                    {
                        OnDockPanelLayoutLeave();
                    }

                    break;
                
                default:
                    float minX = 0;
                    float minY = 0;
            
                    float maxX = _rootDockLayout.worldBound.size.x;
                    float maxY = _rootDockLayout.worldBound.size.y;         
                   
                    // List<IDroppable> droppablesUnderMouse = DragAndDrop.GetDroppablesUnderMouse(evt.mousePosition);
                    
                    if (droppable != null)
                    {
                        if (droppable.GetType() == typeof(TabLayout))
                        {
                            OnTabLayoutEnter((TabLayout)droppable);
                            return;
                        }
                        
                        if (droppable.GetType() == typeof(DockPanelLayout))
                        {
                            OnDockPanelLayoutEnter((DockPanelLayout) droppable);
                            return;
                        }
                        
                        _state = GhostState.None;
                        
                    }

                    UpdatePosition(new Vector2(x, y), new Vector2(minX, minY), new Vector2(maxX, maxY));
                    break;
            }
        }

        private void OnDockPanelLayoutLeave()
        {
            _state = GhostState.None;
            _dockingState = DockingState.None;
        }

        private void FreeFloat()
        {
            _floatingWindow.style.height = 200;
            _floatingWindow.style.width = 200;
            _dockingState = DockingState.None;
            
            UnsetDockingClass();
        }
        
        private void DockLeft()
        {
            _floatingWindow.style.height = new StyleLength(_dropPanelLayout.worldBound.height);
            _floatingWindow.style.width = new StyleLength(_dropPanelLayout.worldBound.width * 0.3f);
            _floatingWindow.style.opacity = new StyleFloat(1);
            _floatingWindow.transform.position = new Vector3(0, 0, 0);

            transform.position = _dropPanelLayout.worldBound.position + new Vector2(0, DockGUI.WORLD_WINDOW_OFFSET_Y);

            _dockingState = DockingState.Left;
            
            SetDockingClass(_floatingWindow, DockGUIStyles.DockingGhostLeft);
        }

        private void SetDockingClass(VisualElement element, string styleClass)
        {
            foreach (var styleName in DockGUIStyles.DockingGhostStyleNames())
            {
                if (styleName == styleClass)
                {
                    continue;
                }
                
                if (element.ClassListContains(styleName))
                {
                    element.RemoveFromClassList(styleName);
                }
            }

            if (!element.ClassListContains(styleClass))
            {
                element.AddToClassList(styleClass);
            }
        }

        public void UnsetDockingClass()
        {
            foreach (var styleName in DockGUIStyles.DockingStyleNames())
            {
                if (ClassListContains(styleName))
                {
                    RemoveFromClassList(styleName);
                }
            }
        }

        
        private void OnDockPanelLayoutUpdate(Vector2 mouseWithOffsetPosition)
        {
            if (mouseWithOffsetPosition.x < _dropPanelLayout.worldBound.position.x + (_dropPanelLayout.worldBound.size.x * DockGUI.DOCK_LAYOUT_RATIO))
            {
                if (_dockingState != DockingState.Left)
                {
                    DockLeft();
                }
            }
            else
            {
                if (_dockingState != DockingState.None)
                {
                    FreeFloat();
                }

                UpdatePosition(mouseWithOffsetPosition, Vector2.zero, _rootDockLayout.worldBound.size);
            }
        }

        private void OnDockPanelLayoutEnter(DockPanelLayout dockPanelLayout)
        {
            ShowGhostPanel();
            
            // if (dockPanelLayout != _targetPanel.DockPanelLayoutParent)
            // {
                _dropPanelLayout = dockPanelLayout;
                _state = GhostState.DockPanelLayout;
            // }
        }

        private void OnTabLayoutEnter(TabLayout tabLayout)
        {
            _state = GhostState.TabLayout;
            _dropTabLayout = tabLayout;

            if (!_dropTabLayout.Contains(_proxyTab))
            {
                // add the panel with an invisible tab to the docklayout for inserting between already existing tabs
                _dropTabLayout.Add(_proxyTab);
                _proxyTab.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
            }
            
            ShowGhostTab();
        }

        private void OnTabLayoutUpdate(Vector2 mousePositionWithOffset)
        {
            Vector2 min;
            Vector2 max;
            
            min.x = _dropTabLayout.worldBound.x;
            min.y = _dropTabLayout.worldBound.y + DockGUI.WORLD_WINDOW_OFFSET_Y;
                
            max.x = min.x + _dropTabLayout.TargetElement.worldBound.width;
            max.y = min.y + _dropTabLayout.TargetElement.worldBound.height;
            
            UpdatePosition(new Vector2(mousePositionWithOffset.x, mousePositionWithOffset.y), new Vector2(min.x, min.y), new Vector2(max.x, max.y));

            int i = -1;
           
            // check if the mouse position is over another tab and insert the targetPanel tab there instead
            foreach (var tab in _dropTabLayout.tabs)
            {
                i++;
                
                // if (tab.DockPanelTarget == _targetDockPanel)
                // {
                //     continue;
                // }


                // use minX instead of center for the first tab
                // float tabX = i > 0 ? tab.worldBound.center.x : tab.worldBound.x;
                // float floatX = i > 0 ? _floatingTab.worldBound.center.x : _floatingTab.worldBound.x;

                float tabX = tab.worldBound.x;
                float floatX = _floatingTab.worldBound.x;

                if (Mathf.Abs(tabX - floatX) < DockGUI.REARRANGE_TAB_DISTANCE)
                {
                    if (tab.DockPanelTarget != _targetDockPanel)
                    {
                        _dropTabLayout.Insert(_dropTabLayout.IndexOf(tab), _proxyTab);
                    }
                    break;
                }
            }
        }
        
        private void OnTabLayoutLeave(IDroppable droppable, MouseMoveEvent evt)
        {
            _state = GhostState.None;    

            var x = evt.mousePosition.x;
            var y = evt.mousePosition.y;

            // _targetPanel.FreeFloat(x, y);
            // _targetPanel.DockPanelLayoutParent.visible = false;
            _dropTabLayout = null;
            
            DragAndDrop.RefreshDroppables();
            
            ShowGhostPanel();
        }

        private void UpdatePosition(Vector2 position, Vector2 min, Vector2 max)
        {
            if (position.x < min.x)
            {
                position.x = min.x;
            }
            else if (position.x + worldBound.size.x > max.x)
            {
                position.x = max.x - worldBound.size.x;
            }

            if (position.y < min.y)
            {
                position.y = min.y;
            }
            else if (position.y + worldBound.size.y > max.y)
            {
                    position.y = max.y - worldBound.size.y;
            }

            transform.position = new Vector3(position.x, position.y, transform.position.z);
        }

        private void ShowGhostTab()
        {
            _floatingWindow.style.visibility = new StyleEnum<Visibility>(Visibility.Hidden);
            _floatingTab.style.visibility = new StyleEnum<Visibility>(Visibility.Visible);
        }
        
        private void ShowGhostPanel()
        {
            _floatingWindow.style.visibility = new StyleEnum<Visibility>(Visibility.Visible);
            _floatingTab.style.visibility = new StyleEnum<Visibility>(Visibility.Hidden);
        }

        private void HideAll()
        {
            _floatingWindow.style.visibility = new StyleEnum<Visibility>(Visibility.Hidden);
            _floatingTab.style.visibility = new StyleEnum<Visibility>(Visibility.Hidden);
        }

        
        private void OnMouseUp(MouseUpEvent evt)
        {
            Debug.Log(_state);

            switch (_state)
            {
                case GhostState.TabLayout:
                    int index = _proxyTab.parent.IndexOf(_proxyTab) - 1;

                    if (!_dropTabLayout.DockPanelLayoutParent.ContainsPanel(_targetDockPanel))
                    {
                        _dropTabLayout.DockPanelLayoutParent.AddPanel(_targetDockPanel);
                    }

                    Tab tab = _dropTabLayout.GetTab(_targetDockPanel);
                    tab.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
                    _dropTabLayout.MoveTab(index, tab);
                    _dropTabLayout.Select(_targetDockPanel);
                    break;

                case GhostState.DockPanelLayout:
                    if (_dockingState == DockingState.None)
                    {
                        _targetDockPanel.DockPanelLayoutParent.transform.position = transform.position;
                    }
                    else
                    {
                        Debug.Log("SHOULD HAVE DOCKED!?");
                        _targetDockPanel.DockPanelLayoutParent.transform.position = Vector3.zero;
                        Dock(_targetDockPanel, _dropPanelLayout, _dockingState);
                    }

                    break;

                default:
                    _targetDockPanel.DockPanelLayoutParent.transform.position = transform.position;
                    break;
            }

            _dockingPanelLayout = null;

            _targetDockPanel.DockPanelLayoutParent.TabLayout.GetTab(_targetDockPanel).visible = true;
            _targetDockPanel.DockPanelLayoutParent.visible = true;
            _rootDockLayout.Remove(this);
            
            _proxyTab?.parent?.Remove(_proxyTab);

            _targetDockPanel = null;
            _dropTabLayout = null;
            
            if (_oldDroppable != null)
            {
                _oldDroppable.TargetElement.style.backgroundColor = _oldColour;
            }
            
            
            EndDrag();
        }

        private void Dock(DockPanel dockPanel, DockPanelLayout dockPanelLayout, DockingState dockingState)
        {
            Debug.Log("Docking: " + dockingState);
            
            
            switch (dockingState)
            {
                case DockingState.Left:
                    dockPanelLayout.Dock(dockPanel, Border.Left);
                    break;
                
                case DockingState.Right:
                    dockPanelLayout.Dock(dockPanel, Border.Right);
                    break;
                
                case DockingState.Top:
                    dockPanelLayout.Dock(dockPanel, Border.Top);
                    break;

                case DockingState.Bottom:
                    dockPanelLayout.Dock(dockPanel, Border.Bottom);
                    break;
                
                default:
                    throw new Exception("Invalid docking state for dock: " + dockingState);
            }
        }
      
        public void EndDrag()
        {
            this.ReleaseMouse();
        }
        
        
    }
    
}