using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace DockGUI
{
    public class DockTab : TextElement
    {
        private DockTabLayout _tabLayout;
        private DockPanel _dockPanelTarget;

        private VisualElement _tabLayoutBg;
        private Vector2 _originalMousePos;
        private bool _isDragging = false;
        private bool _mouseDown = false;
        private Vector2 _mouseOffset;
        private VisualElement _rootElement;
        private DockPanelGhost _ghost;


        public DockTabLayout DockTabLayoutParent => _tabLayout;
        public DockPanel DockPanelTarget => _dockPanelTarget;

        // public VisualElement TargetElement => this;

        public DockTab(DockPanel dockPanelTarget, DockTabLayout tabLayout)
        {
            _dockPanelTarget = dockPanelTarget;
            
            _tabLayout = tabLayout;
            RegisterCallback<MouseDownEvent>(OnMouseDown);
            RegisterCallback<MouseUpEvent>(OnMouseUp);
            RegisterCallback<MouseMoveEvent>(OnMouseMove);
            styleSheets.Add(DockGUIStyles.DefaultStyle);
            AddToClassList("TabNormal");
            AddToClassList("TabPadding");
            AddToClassList("TabLabelNormal");
        }

        private void OnMouseUp(MouseUpEvent evt)
        {
            _mouseDown = false;
        }

        private void OnMouseMove(MouseMoveEvent evt)
        {
            // does the user want to drag
            if (_mouseDown && Vector2.Distance(_originalMousePos, evt.mousePosition) > DockGUISettings.DRAG_THRESHOLD)
            {
                if (_tabLayout.DockLayoutParent.state == DockLayout.State.Floating || _tabLayout.tabs.Count > 1 )
                {
                    float x = evt.mousePosition.x - _mouseOffset.x + DockGUI.WORLD_WINDOW_OFFSET_X;
                    float y = evt.mousePosition.y - _mouseOffset.y + DockGUI.WORLD_WINDOW_OFFSET_Y;

                    _rootElement = _tabLayout.DockLayoutParent.GetRootElement();

                    _ghost = new DockPanelGhost(_dockPanelTarget, _mouseOffset);
                    _ghost.transform.position = new Vector3(x, y, 0);

                    _rootElement.Add(_ghost);

                    DragAndDrop.StartDrag(_ghost);
                    
                    // reset mouse down so it doesnt drag again after switcing mouse capture
                    _mouseDown = false;
                    // }
                }
            }
        }

        private void OnMouseDown(MouseDownEvent evt)
        {
            _tabLayout.OnTabClicked(this);

            _originalMousePos = evt.originalMousePosition;
            _mouseOffset = evt.localMousePosition;
            _mouseDown = true;
        
        }
        
        public void Highlight(bool state)
        {
            // style.backgroundColor = state ? new StyleColor(Color.white) : new StyleColor(Color.gray);
            RemoveFromClassList(state ? "TabNormal" : "TabHighlight");
            AddToClassList(state ? "TabHighlight" : "TabNormal");
        }

    }
}