using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace DockGUI
{
    public class DockTab : TextElement
    {
        private DockTabLayout _tabLayout;
        
        public DockTab(DockTabLayout tabLayout)
        {
            _tabLayout = tabLayout;
            RegisterCallback<MouseDownEvent>(OnMouseDown);
            // RegisterCallback<DragLeaveEvent>(OnMouseDrag);

            DragAndDrop.PrepareStartDrag();
            DragAndDrop.SetGenericData("test", this);
            DragAndDrop.StartDrag("test");
        }

        private void OnMouseDrag(DragUpdatedEvent evt)
        {
            Debug.Log("DRAG");
        }

        private void OnMouseDown(MouseDownEvent evt)
        {
            Debug.Log("Click");
            _tabLayout.OnTabClicked(this);
            DragAndDrop.PrepareStartDrag();
            DragAndDrop.SetGenericData("test", this);
            DragAndDrop.StartDrag("test");
        }



        public void Highlight(bool state)
        {
            style.backgroundColor = state ? new StyleColor(Color.white) : new StyleColor(Color.gray);
        }
    }
}