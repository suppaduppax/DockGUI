using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace DockGUI
{
    public class DockPanel : VisualElement
    {
        private string _title;
        
        public DockLayout DockLayoutParent => (DockLayout) parent;

        public DockPanel(string title): this(title, DockGUIStyles.DefaultStyle) { }

        public List<DockPanel> dockedPanels;
        private StyleSheet _styleSheet;

        public string Title => _title;

        public DockPanel(string title, StyleSheet styleSheet)
        {
            _title = title;
            _styleSheet = styleSheet;
            // style.width = DockGUIStyles.DefaultStyleLength;
            style.flexGrow = 1;
        }

    }

}