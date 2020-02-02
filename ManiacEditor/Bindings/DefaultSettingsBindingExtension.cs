using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;
using System.Configuration;
using System.Windows.Data;

namespace ManiacEditor.Bindings
{
    public class DefaultSettingsBindingExtension : Binding
    {
        public DefaultSettingsBindingExtension()
        {
            Initialize();
        }

        public DefaultSettingsBindingExtension(string path)
            : base(path)
        {
            Initialize();
        }

        private void Initialize()
        {
            this.Source = ManiacEditor.Properties.Defaults.Default;
            this.Mode = BindingMode.TwoWay;
        }
    }
}
