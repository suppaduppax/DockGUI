using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace DockGUI
{
    public class DockPanel : VisualElement
    {
        public List<DockPanel> dockedPanels;
        
        private string _title;

        public string Title => _title;
        public DockLayout DockLayoutParent => (DockLayout) parent;
        public bool IsFloating => DockLayoutParent.IsFloating;

        public DockPanel(string title) : this(title, DockGUIStyles.DefaultStyle) { }

        public DockPanel(string title, StyleSheet styleSheet)
        {
            _title = title;
            styleSheets.Add(styleSheet);
        }

        public DockLayout FreeFloat(float x, float y)
        {
            DockLayout dockLayoutParent = DockLayoutParent;
            List<DockPanel> dockPanels = dockLayoutParent.DockPanels;

            DockLayout floatingLayout = null;

            if (dockPanels.Count > 1)
            {
                VisualElement rootElement = dockLayoutParent.GetRootElement();
                
                dockLayoutParent.DockTabLayout.RemoveTab(this);

                // create a new floating layout for this panel
                floatingLayout = new DockLayout();
                floatingLayout.state = DockLayout.State.Floating;
                // floatingLayout.CreateTabLayout(this);

                floatingLayout.AddPanel(this);
                
                rootElement.Add(floatingLayout);
            }
            else
            {
                Debug.Log("OAKY THIS ISWHATS GOING ON");
                floatingLayout = dockLayoutParent;
            }

            floatingLayout.AddToClassList("FloatingLayout");

            floatingLayout.transform.position = new Vector3(x,y, floatingLayout.transform.position.z);

            return floatingLayout;
        }
        
        private new void Add(VisualElement element)
        {
            throw new Exception();
        }
    }
}