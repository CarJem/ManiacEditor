﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace ManiacEditor.Controls.Editor.Elements
{
    /// <summary>
    /// Interaction logic for ViewPanel.xaml
    /// </summary>
    public partial class ViewPanel : UserControl
    {
        private MainEditor Instance { get; set; } = null;
        public ViewPanel()
        {
            InitializeComponent();
            SplitContainer.MyEvent += SplitContainer_MyEvent;
        }

        private void SplitContainer_MyEvent(object sender, EventArgs e)
        {
            if (Instance != null)
            {
                //Instance.DeviceModel.ResetViewSize(true);
            }
        }

        public void UpdateInstance(MainEditor editor)
        {
            Instance = editor;
            SharpPanel.UpdateInstance(editor);
        }
    }
}
