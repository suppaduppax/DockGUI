using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace DockGUI
{
    public class DockPanelGhost : VisualElement, IDraggable
    {
        private VisualElement _floatingWindow;
        private VisualElement _floatingTab;

        private VisualElement _rootElement;
        private Vector2 _mouseOffset;

        // private DockTabLayout _tabLayout;
        private DockPanel _targetPanel;
        
        private bool _isDragging = false;
        private List<IDroppable> _droppables;

        public VisualElement TargetElement => this;

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

            _droppables = DragAndDrop.GetAllDroppables(_rootElement);
            Debug.Log(_droppables.Count);

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

            _floatingWindow.style.visibility = new StyleEnum<Visibility>(Visibility.Hidden);
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

        public void OnMouseMove(MouseMoveEvent evt)
        {
            if (DragAndDrop.State != DragAndDrop.DragAndDropState.Dragging && DragAndDrop.DraggingObject != this)
            {
                return;
            }

            List<IDroppable> droppablesUnderMouse = DragAndDrop.GetDroppablesUnderMouse(evt.mousePosition);

            Debug.Log("DRAG!?");
            
            float offsetX = -3;
            float offsetY = -22;

            float x = evt.mousePosition.x + offsetX - _mouseOffset.x;
            float y = evt.mousePosition.y + offsetY - _mouseOffset.y;

            float minX = 0;
            float minY = 0;
            
            float maxX = _rootElement.worldBound.size.x;
            float maxY = _rootElement.worldBound.size.y;

            ShowGhostPanel();
            
            if (droppablesUnderMouse.Count > 0)
            {
                if (droppablesUnderMouse[0].GetType() == typeof(DockTabLayout) && droppablesUnderMouse[0].TargetElement.visible == true)
                {
                    ShowGhostTab();

                    minX = droppablesUnderMouse[0].TargetElement.worldBound.x;
                    minY = droppablesUnderMouse[0].TargetElement.worldBound.y + offsetY;
                    
                    maxX = minX + droppablesUnderMouse[0].TargetElement.worldBound.width;
                    maxY = minY + droppablesUnderMouse[0].TargetElement.worldBound.height;
                }
            }
            
            
            if (x < minX)
            {
                x = minX;
            }
            else if (x + worldBound.size.x > maxX)
            {
                x = maxX - worldBound.size.x;
            }

            if (y < minY)
            {
                y = minY;
            }
            else
            {
                if (y + worldBound.size.y > maxY)
                {
                    y = maxY - worldBound.size.y;
                }
            }

            transform.position = new Vector3(x, y, transform.position.z);
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
            _targetPanel.DockLayoutParent.style.visibility = new StyleEnum<Visibility>(Visibility.Visible);
            _targetPanel.DockLayoutParent.transform.position = transform.position;
            _rootElement.Remove(this);
            
            EndDrag();
        }

        public void StartDrag()
        {
            _isDragging = true;
            this.CaptureMouse();
        }

        public void EndDrag()
        {
            _isDragging = false;
            this.ReleaseMouse();
        }
        
        
    }
    
}