using System.Collections;
using System.Collections.Generic;
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
            DockLeft,
            DockRight,
            Docktop,
            DockBottom
        }

        private GhostState _state = GhostState.None;
        
        private VisualElement _floatingWindow;
        private VisualElement _floatingTab;

        private VisualElement _rootElement;
        private Vector2 _mouseOffset;

        // private DockTabLayout _tabLayout;
        private DockPanel _targetPanel;
        
        private DockTabLayout _dropTabLayout;
        private DockTab _dropTab;
        
        private bool _isDragging = false;
        private List<IDroppable> _droppables;

        public VisualElement TargetElement => this;

        private IDroppable _oldDroppable;
        private Color _oldColour;

        public DockPanelGhost(DockPanel panel, Vector2 mouseOffset)
            : this(panel, mouseOffset, DockGUIStyles.DefaultStyle)
        {
        }

        public DockPanelGhost(DockPanel panel, Vector2 mouseOffset,
            StyleSheet styleSheet)
        {
            _targetPanel = panel;            
            _rootElement = panel.DockLayoutParent.GetRootElement();
            _mouseOffset = mouseOffset;

            _targetPanel.DockLayoutParent.DockTabLayout.GetTab(panel).visible = false;
            // _targetPanel.DockLayoutParent.visible = false;
            
            _droppables = DragAndDrop.GetAllDroppables(_rootElement);

            // create ghost panel ----
            styleSheets.Add(styleSheet);
            AddToClassList("GhostLayout");

            _floatingWindow = DockGUI.CreateVisualElement(this, styleSheet, "FloatingLayout", "DockLayoutBg");

            VisualElement floatingWindowTabLayout =
                DockGUI.CreateVisualElement(_floatingWindow, styleSheet, "TabLayoutHeight", "TabLayout");
            
            DockGUI.CreateVisualElement(floatingWindowTabLayout, styleSheet, "TabLayoutBg");
            VisualElement floatingWindowTab =
                DockGUI.CreateVisualElement(floatingWindowTabLayout, styleSheet, "TabHighlight", "TabPadding");
            
            floatingWindowTab.Add(new Label(panel.Title));
            DockGUI.CreateVisualElement(_floatingWindow, styleSheet).style.flexGrow = new StyleFloat(1);

            _floatingWindow.visible = false;
            // -----------------------

            // create ghost tab ------
            _floatingTab = DockGUI.CreateVisualElement(this, styleSheet, "TabLayoutHeight", "TabLayout");
            VisualElement _floatingTabButton =
                DockGUI.CreateVisualElement(_floatingTab, styleSheet, "TabHighlight", "TabPadding");
            _floatingTabButton.Add(new Label(panel.Title));
            // -----------------------
            
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
                
                case GhostState.DockLeft:
                    break;
                
                // case GhostState.Tab:
                //     if (droppable != null && droppable == _dropTab)
                //     {
                //         OnTabUpdate(new Vector2(x, y));
                //     }
                //     else
                //     {
                //         OnTabLeave(evt);
                //     }
                //     break;
                    
                default:
                    float minX = 0;
                    float minY = 0;
            
                    float maxX = _rootElement.worldBound.size.x;
                    float maxY = _rootElement.worldBound.size.y;         
                   
                    // List<IDroppable> droppablesUnderMouse = DragAndDrop.GetDroppablesUnderMouse(evt.mousePosition);
                    
                    if (droppable != null)
                    {
                        if (droppable.GetType() == typeof(DockTabLayout))
                        {
                            OnTabLayoutEnter((DockTabLayout)droppable);
                        }
                        else
                        {
                            _state = GhostState.None;
                        }
                    }

                    UpdatePosition(new Vector2(x, y), new Vector2(minX, minY), new Vector2(maxX, maxY));
                    break;
            }
        }

        private void OnTabLayoutEnter(DockTabLayout tabLayout)
        {
            _state = GhostState.TabLayout;
            _dropTabLayout = tabLayout;
            
            ShowGhostTab();
        }

        private void OnTabLayoutUpdate(Vector2 mousePositionWithOffset)
        {

            Vector2 min;
            Vector2 max;
  
            if (!_dropTabLayout.ContainsTab(_targetPanel))
            {
                // add the panel with an invisible tab to the docklayout for inserting between already existing tabs
                _dropTabLayout.DockLayoutParent.Add(_targetPanel);
                DockTab tab = _dropTabLayout.GetTab(_targetPanel);
                tab.visible = false;
            }

            foreach (var tab in _dropTabLayout.tabs)
            {
                if (tab.DockPanelTarget == _targetPanel)
                {
                    continue;
                }
                
                if (Mathf.Abs(tab.worldBound.center.x - _floatingTab.worldBound.center.x) < DockGUI.REARRANGE_TAB_DISTANCE)
                {
                    _dropTabLayout.MoveTab(_dropTabLayout.tabs.IndexOf(tab), _dropTabLayout.GetTab(_targetPanel));
                    break;
                }
            }
            
            min.x = _dropTabLayout.worldBound.x;
            min.y = _dropTabLayout.worldBound.y + DockGUI.WORLD_WINDOW_OFFSET_Y;
                
            max.x = min.x + _dropTabLayout.TargetElement.worldBound.width;
            max.y = min.y + _dropTabLayout.TargetElement.worldBound.height;
            
            UpdatePosition(new Vector2(mousePositionWithOffset.x, mousePositionWithOffset.y), new Vector2(min.x, min.y), new Vector2(max.x, max.y));
        }
        
        private void OnTabLayoutLeave(IDroppable droppable, MouseMoveEvent evt)
        {
            _state = GhostState.None;    

            var x = evt.mousePosition.x;
            var y = evt.mousePosition.y;

            if (droppable != null)
            {
                if (droppable.GetType() == typeof(DockTab) &&
                    _dropTabLayout == ((DockTab) droppable).DockTabLayoutParent)
                {
                    return;
                }
            }

            _targetPanel.FreeFloat(x, y);
            _targetPanel.DockLayoutParent.visible = false;
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

        private void OnMouseUp(MouseUpEvent evt)
        {
            switch (_state)
            {
                case GhostState.Tab:
                case GhostState.TabLayout:
                    _dropTabLayout.GetTab(_targetPanel).visible = true;
                    _dropTabLayout.Select(_targetPanel);
                    break;
                
                default:
                    _targetPanel.DockLayoutParent.transform.position = transform.position;
                    break;
            }

            _targetPanel.DockLayoutParent.DockTabLayout.GetTab(_targetPanel).visible = true;
            _targetPanel.DockLayoutParent.visible = true;
            _rootElement.Remove(this);

            _targetPanel = null;
            _dropTab = null;
            _dropTabLayout = null;

            if (_oldDroppable != null)
            {
                _oldDroppable.TargetElement.style.backgroundColor = _oldColour;
            }
            
            EndDrag();
        }

      
        public void EndDrag()
        {
            _isDragging = false;
            this.ReleaseMouse();
        }
        
        
    }
    
}