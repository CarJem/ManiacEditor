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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using ManiacEditor.Controls.Editor;

namespace ManiacEditor.Controls.Editor
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
        }

        public void UpdateInstance(MainEditor editor)
        {
            Instance = editor;
            SharpPanel.UpdateInstance(editor);
        }
    }
}
