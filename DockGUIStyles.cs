using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using DockGUI;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;

[InitializeOnLoad]
public static class DockGUIStyles
{
    public static StyleSheet DefaultStyle;
    
    public const float DEFAULT_LAYOUT_SIZE = 250f;
    public const LengthUnit DEFAULT_LAYOUT_SIZE_UNIT = LengthUnit.Pixel;

    public static StyleLength DefaultStyleLength;

    public static IStyle FlexLayoutStyle;

    private const string _defaultStyleFilename = "DefaultStyle2.uss";
    
    static DockGUIStyles()
    {
        Refresh();
    }

    public static string[] DockingStyleNames()
    {
        return new string[]{"DockLayoutTop", "DockLayoutBottom", "DockLayoutLeft", "DockLayoutRight"};
    }

    public static string[] DockingGhostStyleNames()
    {
        return new string[]{DockingGhostTop, DockingGhostBottom, DockingGhostLeft, DockingGhostRight};
    }

    public static string DockingGhostTop => "DockingGhostTop";
    public static string DockingGhostBottom => "DockingGhostBottom";
    public static string DockingGhostRight => "DockingGhostRight";
    public static string DockingGhostLeft => "DockingGhostLeft";

    public static void Refresh()
    {
        Debug.Log("Refreshing...");
        var files = Directory.GetFiles(Application.dataPath, _defaultStyleFilename, SearchOption.AllDirectories);
        if (files == null)
        {
            throw new Exception("Could not find default style...");
        }
        
        var folderGuid = AssetDatabase.FindAssets("DockGUI,t:folder");
        var folderPath = AssetDatabase.GUIDToAssetPath(folderGuid[0]);

        var defaultStyleGuid = AssetDatabase.FindAssets("DefaultStyle,t:StyleSheet", new string[]{folderPath + "/Styles"}); 
        var defaultStylePath = AssetDatabase.GUIDToAssetPath(defaultStyleGuid[0]);

        DefaultStyle = AssetDatabase.LoadAssetAtPath<StyleSheet>(defaultStylePath);
        DefaultStyleLength = new StyleLength(new Length(DEFAULT_LAYOUT_SIZE, DEFAULT_LAYOUT_SIZE_UNIT));
    }

    public static void FlexLayout(ref VisualElement element, FlexDirection direction)
    {
        element.style.flexGrow = 1;
        element.style.backgroundColor = new StyleColor(Color.blue);
        element.style.flexDirection = direction;
    }

    public static void DockLayoutLeft(ref DockLayout element)
    {
        element.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Column);
        element.style.flexGrow = 0;
        element.style.backgroundColor = new StyleColor(Color.red);
        element.style.paddingLeft = 3;
        element.style.paddingRight = 3;
        element.style.paddingTop = 3;
        element.style.paddingBottom = 3;
    }
    
    public static void DockLayoutTop(ref DockLayout element)
    {
        element.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);
        element.style.flexGrow = 0;
        element.style.backgroundColor = new StyleColor(Color.white);
        element.style.paddingLeft = 3;
        element.style.paddingRight = 3;
        element.style.paddingTop = 3;
        element.style.paddingBottom = 3;
    }

   

    public static void DockLayoutBottom(ref DockLayout element)
    {
        element.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Column);
        element.style.flexGrow = 0;
        element.style.backgroundColor = new StyleColor(Color.green);
        element.style.paddingLeft = 3;
        element.style.paddingRight = 3;
        element.style.paddingTop = 3;
        element.style.paddingBottom = 3;
        
    }

    public static void DockLayoutRight(ref DockLayout element)
    {
        element.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Column);
        element.style.flexGrow = 0;
        element.style.backgroundColor = new StyleColor(Color.red);
        element.style.paddingLeft = 3;
        element.style.paddingRight = 3;
        element.style.paddingTop = 3;
        element.style.paddingBottom = 3;
    }
}
