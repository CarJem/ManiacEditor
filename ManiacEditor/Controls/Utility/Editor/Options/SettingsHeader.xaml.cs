using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Configuration;
using System.ComponentModel;
using System.Data;
using System.Collections.Specialized;

namespace ManiacEditor.Controls.Utility.Editor.Options
{
    /// <summary>
    /// Interaction logic for SettingsHeader.xaml
    /// </summary>
    public partial class SettingsHeader : UserControl
    {
        public SettingsHeader()
        {
            InitializeComponent();
            DataContext = this;
        }

        public string HeaderName
        {
            get { return (string)GetValue(TextBlockTextProperty); }
            set { SetValue(TextBlockTextProperty, value); }
        }

        public static readonly DependencyProperty TextBlockTextProperty =
DependencyProperty.Register("Text", typeof(string), typeof(SettingsHeader), new UIPropertyMetadata(""));
    }
}
