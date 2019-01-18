using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;
using ManiacEditor;
using Microsoft.Xna.Framework;
using RSDKv5;
using Xceed.Wpf.Toolkit;

namespace ManiacEditor
{
    public static class Extensions
    {
        public static void EnableButtonList(object[] allItems)
        {
            foreach (var item in allItems)
            {
                if (item is ToggleButton)
                {
                    var button = item as ToggleButton;
                    button.IsEnabled = true;
                }
                else if (item is UIElement)
                {
                    var button = item as UIElement;
                    button.IsEnabled = true;
                }
                else if (item is Button)
                {
                    var button = item as Button;
                    button.IsEnabled = true;
                }
                else if (item is SplitButton)
                {
                    var button = item as SplitButton;
                    button.IsEnabled = true;
                }

            }
        }
    }

    public static class ExtensionMethods
    {

        private static Action EmptyDelegate = delegate () { };


        public static void Refresh(this UIElement uiElement)
        {
            uiElement.Dispatcher.Invoke(DispatcherPriority.Render, EmptyDelegate);
        }
    }
}
