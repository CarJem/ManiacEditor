using System;
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
using System.Windows.Shapes;

namespace ManiacEditor.Controls.Utility.Technical
{
    /// <summary>
    /// Interaction logic for SceneBackgroundColorPickerTool.xaml
    /// </summary>
    public partial class SceneBGColorPickerTool : Window
    {
        public RSDKv5.Color BackgroundColor1 { get; set; }
        public RSDKv5.Color BackgroundColor2 { get; set; }

        public bool ColorsChanged { get; set; } = false;

        public SceneBGColorPickerTool()
        {
            InitializeComponent();
        }
        public SceneBGColorPickerTool(RSDKv5.Color A, RSDKv5.Color B) : this()
        {
            BackgroundColor1 = A;
            BackgroundColor2 = B;

            ColorPickerA.SelectedColor = ToMediaColor(A);
            ColorPickerB.SelectedColor = ToMediaColor(B);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            BackgroundColor1 = ToRSDKv5Color(ColorPickerA.SelectedColor.Value);
            BackgroundColor2 = ToRSDKv5Color(ColorPickerB.SelectedColor.Value);
            DialogResult = true;
            ColorsChanged = true;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DialogResult = false;
        }

        private RSDKv5.Color ToRSDKv5Color(System.Windows.Media.Color value)
        {
            return new RSDKv5.Color
            {
                R = value.R,
                A = value.A,
                B = value.B,
                G = value.G
            };
        }
        private System.Windows.Media.Color ToMediaColor(RSDKv5.Color value)
        {
            return new System.Windows.Media.Color
            {
                R = value.R,
                A = value.A,
                B = value.B,
                G = value.G
            };
        }
    }
}
