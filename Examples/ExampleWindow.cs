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
            
            DockPanel panel1 = new DockPanel("Flex");
            panel1.Add(new Label("Flex"));

            DockPanel panel2 = new DockPanel("Panel 2");
            panel2.Add(new Label("PAnel 2 babehz"));

            DockPanel panel3 = new DockPanel("Docking");
            panel3.Add(new Label("Docking"));
            
            AddPanel(panel1);
            panel1.DockPanelLayoutParent.AddPanel(panel2);
            panel1.DockPanelLayoutParent.AddPanel(panel3);
            
            // panel1.DockPanelLayoutParent.Dock(panel3, Border.Left);
            // panel1.DockPanelLayoutParent.Dock(panel2, Border.Left);
            // panel1.name = "Panel 1";
            // panel1.DockPanelLayoutParent.name = "Panel 1 - DLP";
            // panel1.DockPanelLayoutParent.DockLayoutParent.name = "Panel 3 - DL";

            // panel1.DockPanelLayoutParent.AddPanel(panel2);


        }
    }
}