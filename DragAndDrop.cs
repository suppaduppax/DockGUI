using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace DockGUI
{

    public static class DragAndDrop
    {
        
        private static IDraggable _draggingObject;
        private static IDroppable _droppingObject;
        
        private static DragAndDropState _state = DragAndDropState.None;
        
        private static List<IDroppable> _droppables;
        
        public static IDraggable DraggingObject => _draggingObject;
        public static IDroppable DroppingObject => _droppingObject;
        public static DragAndDropState State => _state;
        
        public enum DragAndDropState
        {
            None,
            Dragging,
            Dropping
        }

        public static void StartDrag(IDraggable target)
        {
            _draggingObject = target;
            _draggingObject.TargetElement.CaptureMouse();
            RefreshDroppables();
        }

        private static VisualElement GetRootElement(VisualElement refElement)
        {
            VisualElement nextParent = refElement.parent;
            VisualElement parent = nextParent ?? refElement;
            
            while (nextParent != null)
            {
                parent = nextParent;
                nextParent = nextParent.parent;
            }

            return parent;
        }

        public static void DroppableEntered(IDroppable target)
        {
            if (_state == DragAndDropState.Dragging)
            {
                // _draggingObject.OnDroppableEntered(_draggingObject);
                _droppingObject = target;
            }
        }

        public static void DragUpdate()
        {
            // _draggingObject.OnDrag();
        }

        /// <summary>
        /// Unsuccessful drag
        /// </summary>
        public static void EndDrag()
        {
            // _draggingObject.OnDragEnd();
            _draggingObject = null;
        }
        
        public static List<IDroppable> GetAllDroppables(VisualElement rootElement)
        {
            List<IDroppable> result = new List<IDroppable>();
            var children = rootElement.Children();
            foreach (var child in rootElement.Children())
            {
                if (child.visible)
                {
                    if (child is IDroppable droppable)
                    {
                        result.Add(droppable);
                    }

                    result.AddRange(GetAllDroppables(child));
                }
            }
            return result;
        }

        public static List<IDroppable> GetDroppablesUnderMouse(Vector2 mousePos)
        {
            List<IDroppable> result = new List<IDroppable>();
            foreach (var droppable in _droppables)
            {
                if (droppable.TargetElement.worldBound.Contains(mousePos))
                {
                    result.Add(droppable);
                }
            }

            return result;
        }

        public static void RefreshDroppables()
        {
            _droppables = GetAllDroppables(GetRootElement(_draggingObject.TargetElement));
        }
    }

    public interface IDraggable
    {
        VisualElement TargetElement { get; }
        
        // void OnDrag();
        // void OnDroppableEntered(IDraggable source);
        // void OnDragEnd();

    }

    public interface IDroppable
    {    
        VisualElement TargetElement { get; }
    }
    

}