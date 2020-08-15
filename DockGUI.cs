using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace DockGUI
{

    public static class DockGUI
    {
        public const float REARRANGE_TAB_DISTANCE = 10.0f;
        public const float WORLD_WINDOW_OFFSET_X = -3.0f;
        public const float WORLD_WINDOW_OFFSET_Y = -22.0f;
        public const float DOCK_LAYOUT_RATIO = 0.3f;
        
        public static VisualElement CreateVisualElement(params string[] styleClasses)
        {
            return CreateVisualElement(DockGUIStyles.DefaultStyle, styleClasses);
        }

        public static VisualElement CreateVisualElement(StyleSheet styleSheet, params string[] styleClasses)
        {
            VisualElement result = new VisualElement();
            result.styleSheets.Add(styleSheet);

            foreach (var styleClass in styleClasses)
            {
                result.AddToClassList(styleClass);
            }

            return result;
        }

        public static VisualElement CreateVisualElement(VisualElement parent, StyleSheet styleSheet, params string[] styleClasses)
        {
            VisualElement element = CreateVisualElement(styleSheet, styleClasses);
            parent.Add(element);
            return element;
        }

        public static VisualElement CreateVisualElement(VisualElement parent,
            params string[] styleClasses)
        {
            return CreateVisualElement(parent, DockGUIStyles.DefaultStyle);
        }
        
    }
}