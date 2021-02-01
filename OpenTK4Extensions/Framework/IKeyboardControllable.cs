using OpenTK.Windowing.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenTKExtensions.Framework
{
    public interface IKeyboardControllable
    {
        int KeyboardPriority { get; set; }
        bool ProcessKeyDown(KeyboardKeyEventArgs e);
        bool ProcessKeyUp(KeyboardKeyEventArgs e);
    }
}
