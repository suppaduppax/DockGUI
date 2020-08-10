using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace DockGUI
{
    public class ExampleWindow : DockWindow
    {
        [MenuItem("DockGUI/Examples/Example Window")]
        static void ShowWindow()
        {
            EditorWindow exampleWindow = GetWindow<ExampleWindow>();
            exampleWindow.Show();
        }

        private void OnEnable()
        {
            DockGUIStyles.Refresh();
            
            DockPanel panel1 = new DockPanel("Panel 1");
            DockPanel panel2 = new DockPanel("Panel 2");
            DockPanel panel3 = new DockPanel("Panel 3");
            panel2.Add(new Label("PAnel 2 babehz"));
            panel3.Add(new Label("PAnel THREE"));
            Add(panel1);
            Add(panel2);
            Add(panel3);
            
            panel2.FreeFloat(200, 200);
            panel3.FreeFloat(100, 0);
            // panel2.DockLayoutParent.Add(panel3);

        }
    }
}