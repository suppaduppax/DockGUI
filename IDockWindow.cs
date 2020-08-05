using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DockGUI
{
    public interface IDockableWindow
    {
        DockPanel WindowPanel { get; }
    }
}