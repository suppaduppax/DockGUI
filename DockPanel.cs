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
        public DockPanelLayout DockPanelLayoutParent => (DockPanelLayout) parent;
        public bool HasDockLayoutParent => parent is DockPanelLayout;
        public bool IsFloating => DockPanelLayoutParent.IsFloating;
        
        public bool HasDockPanelLayoutParent {
            get
            {
                if (parent == null)
                {
                    return false;
                }

                return parent.GetType() == typeof(DockPanelLayout);
            }
        }

        public DockPanel(string title) : this(title, DockGUIStyles.DefaultStyle) { }

        public DockPanel(string title, StyleSheet styleSheet)
        {
            _title = title;
            styleSheets.Add(styleSheet);
            AddToClassList("DockPanel");
        }

        public DockPanelLayout FreeFloat(float x, float y)
        {
            DockPanelLayout dockPanelLayoutParent = DockPanelLayoutParent;
            List<DockPanel> dockPanels = dockPanelLayoutParent.DockPanels;

            DockPanelLayout floatingLayout = null;
            DockLayout rootElement = dockPanelLayoutParent.GetRootDockLayout();

            if (dockPanels.Count > 1)
            {
                // just detach this panel from the panelLayout 
                dockPanelLayoutParent.RemovePanel(this);
            }
            else
            {
                // no panels left in the panelLayout, destroy it
                if (dockPanelLayoutParent.IsFloating)
                {
                    // if it's floating, it's not a flex/docked container, so just remove it normally 
                    dockPanelLayoutParent.DockLayoutParent.Remove(dockPanelLayoutParent);
                }
                else
                {
                    dockPanelLayoutParent.DockLayoutParent.RemoveContainer(dockPanelLayoutParent);
                }
            }
            
            // create a new floating layout for this panel
            floatingLayout = new DockPanelLayout {IsFloating = true};
            floatingLayout.AddPanel(this);
            floatingLayout.AddToClassList("FloatingLayout");
            floatingLayout.transform.position = new Vector3(x,y, floatingLayout.transform.position.z);

            rootElement.Add(floatingLayout);

            return floatingLayout;
        }
        
        private new void Add(VisualElement element)
        {
            throw new Exception();
        }
    }
}