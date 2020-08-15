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
            panel1.Add(new Label("PAnel 1!"));

            DockPanel panel2 = new DockPanel("Panel 2");
            panel2.Add(new Label("PAnel 2 babehz"));

            DockPanel panel3 = new DockPanel("Panel 3");
            panel3.Add(new Label("PAnel THREE"));
            
            Add(panel1);
            Add(panel2);
                
            panel1.DockLayoutParent.Expand(panel3, Border.Left);
            panel3.DockLayoutParent.RemovePanel(panel3);

        }
    }
}