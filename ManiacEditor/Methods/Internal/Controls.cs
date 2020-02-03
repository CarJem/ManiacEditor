using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using RSDKv5;
using ManiacEditor.Actions;
using System.Windows.Controls;
using ManiacEditor.Controls.Base;
using ManiacEditor.Controls.Base.Toolbars;

namespace ManiacEditor.Methods.Internal
{
    public static class Controls
    {
        private static MainEditor Instance;
        public static void UpdateInstance(MainEditor _instance)
        {
            Instance = _instance;
        }
    }
}
