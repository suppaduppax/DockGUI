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

        public virtual void Add(DockPanel dockPanel)
        {
            if (_rootDockLayout == null)
            {
                _rootDockLayout = new DockLayout();
                rootVisualElement.Add(_rootDockLayout);
            }
            
            _rootDockLayout.Add(dockPanel);
        }

        private DockLayout Dock(DockPanel panel, Border border)
        {
            if (_borderToLayoutDict.ContainsKey(border))
            {
                throw new Exception("Layout already exists for border: " + border);
            }

            DockLayout dockLayout = CreateAndDockLayout(border);
            dockLayout.hasDockPanels = true;
            _borderToLayoutDict.Add(border, dockLayout);

            dockLayout.Add(panel);
            panel.style.backgroundColor = new StyleColor(Color.cyan);

            return dockLayout;
        }

        private DockLayout CreateAndDockLayout(Border border)
        {
            DockLayout dockLayout;

            VisualElement oldFlexLayout;
            switch (border)
            {
                case Border.Left:
                    dockLayout = new DockLayout();
                    DockGUIStyles.DockLayoutLeft(ref dockLayout);
                    dockLayout.SetExpandDirection(DockLayout.Direction.Horizontal);
                    dockLayout.style.width = DockGUIStyles.DefaultStyleLength;

                    dockLayout.name = "WindowLeft";
                    oldFlexLayout = new VisualElement();
                    oldFlexLayout.name = "OldFlexLayout";
                    DockGUIStyles.FlexLayout(ref oldFlexLayout, FlexDirection.Row);

                    _rootDockLayout.parent.Add(oldFlexLayout);

                    oldFlexLayout.Add(dockLayout);
                    oldFlexLayout.Add(_rootDockLayout);

                    return dockLayout;

                case Border.Right:
                    dockLayout = new DockLayout();
                    DockGUIStyles.DockLayoutRight(ref dockLayout);
                    dockLayout.SetExpandDirection(DockLayout.Direction.Horizontal);
                    dockLayout.style.width = DockGUIStyles.DefaultStyleLength;

                    dockLayout.name = "WindowRight";
                    oldFlexLayout = new VisualElement();
                    oldFlexLayout.name = "OldFlexLayout";
                    DockGUIStyles.FlexLayout(ref oldFlexLayout, FlexDirection.RowReverse);

                    _rootDockLayout.parent.Add(oldFlexLayout);

                    oldFlexLayout.Add(dockLayout);
                    oldFlexLayout.Add(_rootDockLayout);

                    return dockLayout;

                case Border.Top:
                    dockLayout = new DockLayout();
                    DockGUIStyles.DockLayoutTop(ref dockLayout);
                    dockLayout.SetExpandDirection(DockLayout.Direction.Vertical);
                    dockLayout.style.height = DockGUIStyles.DefaultStyleLength;

                    dockLayout.name = "WindowTop";
                    oldFlexLayout = new VisualElement();
                    oldFlexLayout.name = "OldFlexLayout";
                    DockGUIStyles.FlexLayout(ref oldFlexLayout, FlexDirection.Column);


                    _rootDockLayout.parent.Add(oldFlexLayout);

                    oldFlexLayout.Add(dockLayout);
                    oldFlexLayout.Add(_rootDockLayout);

                    return dockLayout;


                case Border.Bottom:
                    dockLayout = new DockLayout();
                    DockGUIStyles.DockLayoutBottom(ref dockLayout);
                    dockLayout.SetExpandDirection(DockLayout.Direction.Vertical);
                    // dockLayout.style.height = DockGUIStyles.DefaultStyleLength;

                    dockLayout.name = "WindowBottom";
                    oldFlexLayout = new VisualElement();
                    oldFlexLayout.name = "OldFlexLayout";
                    // oldFlexLayout.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);
                    DockGUIStyles.FlexLayout(ref oldFlexLayout, FlexDirection.ColumnReverse);

                    _rootDockLayout.parent.Add(oldFlexLayout);

                    oldFlexLayout.Add(dockLayout);
                    oldFlexLayout.Add(_rootDockLayout);

                    return dockLayout;

            }

            return null;
        }
    }
}