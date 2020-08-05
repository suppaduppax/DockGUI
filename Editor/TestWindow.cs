using System;
using System.Collections;
using System.Collections.Generic;
using DockGUI;
using PocketGuild.Test.ThirdTest;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class DockWindow : EditorWindow
{

    private Dictionary<Border, DockLayout> _borderToLayoutDict;

    private VisualElement _flexLayout;
    
    [MenuItem("DockGUI/Test Window")]
    static void ShowWindow()
    {
        DockWindow window = GetWindow<DockWindow>();
        window.Show();
        
    }

    private void OnEnable()
    {
        _borderToLayoutDict = new Dictionary<Border, DockLayout>();
        _flexLayout = new VisualElement();
        _flexLayout.name = "OriginalFlexLayout";
        
        DockGUIStyles.FlexLayout(ref _flexLayout, FlexDirection.Column);
        rootVisualElement.Add(_flexLayout);

        DockPanel panel = new DockPanel ("Panel 1"){name = "panel1"};
        panel.Add(new Label("Panel 1"));

        DockLayout firstLayout = Dock(panel, Border.Right);
        firstLayout.name = "FirstLayout";

        DockPanel panel2 = new DockPanel("panel2");
        // panel2.;
        firstLayout.Add(panel2);
        panel2.Add(new Label("Panel 2"));


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
                
                _flexLayout.parent.Add(oldFlexLayout);
                                   
                oldFlexLayout.Add(dockLayout);
                oldFlexLayout.Add(_flexLayout);
                
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
                
                _flexLayout.parent.Add(oldFlexLayout);
                                   
                oldFlexLayout.Add(dockLayout);
                oldFlexLayout.Add(_flexLayout);
                
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
                
                
                _flexLayout.parent.Add(oldFlexLayout);
                                   
                oldFlexLayout.Add(dockLayout);
                oldFlexLayout.Add(_flexLayout);
                
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
                
                _flexLayout.parent.Add(oldFlexLayout);
                                   
                oldFlexLayout.Add(dockLayout);
                oldFlexLayout.Add(_flexLayout);
                
                return dockLayout;

        }

        return null;
    }
}
