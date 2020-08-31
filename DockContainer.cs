using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace DockGUI
{
    public class DockContainer : VisualElement
    {
        private StyleSheet _styleSheet;

        public DockContainer DockContainerParent => (DockContainer) parent;

        public bool HasContainerParent
        {
            get
            {
                if (parent == null)
                {
                    return false;
                }

                Type parentType = parent.GetType();
                return parentType == typeof(DockContainer) || parentType.IsSubclassOf(typeof(DockContainer));
            }
        }

        public DockContainer() : this(DockGUIStyles.DefaultStyle) { }

        public DockContainer(StyleSheet styleSheet)
        {
            styleSheets.Add(styleSheet);
        }
        
        public void SetDockingClass(string styleClass)
        {
            foreach (var styleName in DockGUIStyles.DockingStyleNames())
            {
                if (styleName == styleClass)
                {
                    continue;
                }
                
                if (ClassListContains(styleName))
                {
                    RemoveFromClassList(styleName);
                }
            }

            if (!ClassListContains(styleClass))
            {
                AddToClassList(styleClass);
            }
        }
        
        public void UnsetDockingClass()
        {
            foreach (var styleName in DockGUIStyles.DockingStyleNames())
            {
                if (ClassListContains(styleName))
                {
                    RemoveFromClassList(styleName);
                }
            }
        }

        public DockLayout GetRootDockLayout()
        {
            if (parent == null)
            {
                return null;
            }
            
            DockContainer nextParent = DockContainerParent;
            DockContainer curParent = null;
            while (nextParent != null)
            {
                curParent = nextParent;
                nextParent = nextParent.HasContainerParent ? nextParent.DockContainerParent : null;
            }

            return (DockLayout)curParent;
        }
        

        // public VisualElement GetRootElement()
        // {
        //     if (parent == null)
        //     {
        //         return null;
        //     }
        //
        //     VisualElement nextParent = parent;
        //     VisualElement curParent = null;
        //     
        //     while (nextParent != null)
        //     {
        //         curParent = nextParent;
        //         if (curParent.GetType() != typeof(DockLayout))
        //         {
        //             break;
        //         }
        //         nextParent = nextParent.parent;
        //     }
        //
        //     return curParent;
        // }
    }
}