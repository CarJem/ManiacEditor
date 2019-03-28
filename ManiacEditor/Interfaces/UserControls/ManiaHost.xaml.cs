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
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ManiacEditor
{
    /// <summary>
    /// Interaction logic for ManiaHost.xaml
    /// </summary>
    public partial class ManiaHost : UserControl
    {
        private System.Windows.Forms.Panel _panel;
        public Process _process;
        //private ManiacED_CodenameWalla.Form1 WallaInstance;

        public ManiaHost()
        {
            InitializeComponent();
            _panel = new System.Windows.Forms.Panel();
            _panel.Anchor = System.Windows.Forms.AnchorStyles.None;
            _panel.Dock = System.Windows.Forms.DockStyle.Fill;
            windowsFormsHost1.Child = _panel;
            this.Unloaded += OnGameClosing;
        }

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32")]
        private static extern IntPtr SetParent(IntPtr hWnd, IntPtr hWndParent);

        [DllImport("user32")]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, int uFlags);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
        private static extern bool ShowWindow(IntPtr hWnd, ShowWindowEnum flags);

        [DllImport("USER32.DLL")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        private enum ShowWindowEnum
        {
            Hide = 0,
            ShowNormal = 1, ShowMinimized = 2, ShowMaximized = 3,
            Maximize = 3, ShowNormalNoActivate = 4, Show = 5,
            Minimize = 6, ShowMinNoActivate = 7, ShowNoActivate = 8,
            Restore = 9, ShowDefault = 10, ForceMinimized = 11
        };

        private const int SWP_NOZORDER = 0x0004;
        private const int SWP_NOACTIVATE = 0x0010;
        private const int GWL_STYLE = -16;
        private const int WS_CAPTION = 0x00C00000;
        private const int WS_THICKFRAME = 0x00040000;

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            IntPtr hWnd = FindWindow("SonicMania", null); // this gives you the handle of the window you need.
            Process processes = Process.GetProcessesByName("SonicMania").FirstOrDefault();
            if (processes != null)
            {
                _process = processes;
                SetParent(processes.MainWindowHandle, _panel.Handle);

                /*
                // remove control box
                int style = GetWindowLong(_process.MainWindowHandle, GWL_STYLE);
                style = style & ~WS_CAPTION & ~WS_THICKFRAME;
                SetWindowLong(_process.MainWindowHandle, GWL_STYLE, style);*/

                // resize embedded application & refresh
                ResizeEmbeddedApp();
            }


        }

        private void LockWallaToHost(object sender, RoutedEventArgs e)
        {
            /*if (WallaInstance != null)
            {
                WallaInstance.FrmOverlay.TopLevel = false;
                WallaInstance.FrmOverlay.Parent = _panel.Parent;
            }*/
        }
        
        private void OnGameClosing(object sender, RoutedEventArgs e)
        {
            if (_process != null)
            {
                _process.Refresh();
                _process.Close();
            }
        }

        private void ResizeEmbeddedApp()
        {
            if (_process == null)
                return;

            SetWindowPos(_process.MainWindowHandle, IntPtr.Zero, 0, 0, (int)_panel.ClientSize.Width, (int)_panel.ClientSize.Height, SWP_NOZORDER | SWP_NOACTIVATE);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            Size size = base.MeasureOverride(availableSize);
            ResizeEmbeddedApp();
            return size;
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ForceKillMania_Click(object sender, RoutedEventArgs e)
        {
            ForceKillSonicMania();
        }

        public void ForceKillSonicMania()
        {
            foreach (var process in Process.GetProcessesByName("SonicMania"))
            {
                process.Kill();
            }
        }

        private void OptionsButton_Click(object sender, RoutedEventArgs e)
        {
            OptionsButton.IsOpen = true;
        }

        private void DevWalla_Click(object sender, RoutedEventArgs e)
        {
            /*if (WallaInstance == null)
            {
                WallaInstance = new ManiacED_CodenameWalla.Form1();
                WallaInstance.checkBox1.Enabled = false;
                WallaInstance.TopMost = true;
                WallaInstance.Show();
            }*/

        }

        private void LaunchModLoader_Click(object sender, RoutedEventArgs e)
        {
            Editor.Instance.Launcher.ManiaModManager();
        }

        private void LaunchSM_Click(object sender, RoutedEventArgs e)
        {
            Editor.Instance.Launcher.SonicManiaHeadless();
        }

        private void LauncherButton_Click(object sender, RoutedEventArgs e)
        {
            LauncherButton.IsOpen = true;
        }

        private void LaunchCheatEngine_Click(object sender, RoutedEventArgs e)
        {
            Editor.Instance.Launcher.CheatEngine();
        }
    }
}
