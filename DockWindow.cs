using System;
using System.Collections;
using System.Collections.Generic;
using DockGUI;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace DockGUI
{

    public class DockWindow : EditorWindow
    {

        private Dictionary<Border, DockLayout> _borderToLayoutDict;

        private DockLayout _rootDockLayout;

        public virtual void AddPanel(DockPanel dockPanel)
        {
            if (_rootDockLayout == null)
            {
                _rootDockLayout = new DockLayout();
                _rootDockLayout.AddToClassList("DockLayoutFlex");
                rootVisualElement.Add(_rootDockLayout);
            }
            
            _rootDockLayout.CreatePanelLayout(dockPanel);
        }
    }
}