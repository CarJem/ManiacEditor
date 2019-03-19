using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.Windows.Forms;
using RSDKv5;
using System.Drawing;
using Point = System.Drawing.Point;
using ManiacEditor.Interfaces;
using Cyotek.Windows.Forms;
using Color = System.Drawing.Color;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;
using System.Windows.Controls;

namespace ManiacEditor
{
    public class EditorLaunch
    {
        private Editor Editor;
        public ManiacED_ManiaPal.Connector ManiaPalConnector;
        #region DLL Import Stuff
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
        #endregion
        public EditorLaunch(Editor instance)
        {
            Editor = instance;
        }

        #region Apps Tab Buttons
        public void TileManiacNormal()
        {
            if (Editor.TileManiacInstance == null || Editor.TileManiacInstance.IsClosed) Editor.TileManiacInstance = new TileManiac.MainWindow();
            Editor.TileManiacInstance.Show();
            if (Editor.TileConfig != null && Editor.EditorTiles.StageTiles != null)
            {
                if (Editor.TileManiacInstance.Visibility != Visibility.Visible || Editor.TileManiacInstance.tcf == null)
                {
                    Editor.TileManiacInstance.LoadTileConfigViaIntergration(Editor.TileConfig, Editor.Paths.TileConfig_Source);
                }
                else
                {
                    Editor.TileManiacInstance.Activate();
                }

            }

        }
        public void InsanicManiac()
        {
            //Sanic2Maniac sanic = new Sanic2Maniac(null, this);
            //sanic.Show();
        }
        public void RSDKAnnimationEditor()
        {
            String aniProcessName = Path.GetFileNameWithoutExtension(Settings.mySettings.RunAniEdPath);
            IntPtr hWnd = FindWindow(aniProcessName, null); // this gives you the handle of the window you need.
            Process processes = Process.GetProcessesByName(aniProcessName).FirstOrDefault();
            if (processes != null)
            {
                // check if the window is hidden / minimized
                if (processes.MainWindowHandle == IntPtr.Zero)
                {
                    // the window is hidden so try to restore it before setting focus.
                    ShowWindow(processes.Handle, ShowWindowEnum.Restore);
                }

                // set user the focus to the window
                SetForegroundWindow(processes.MainWindowHandle);
            }
            else
            {

                // Ask where RSDK Annimation Editor is located when not set
                if (string.IsNullOrEmpty(Settings.mySettings.RunAniEdPath))
                {
                    var ofd = new OpenFileDialog
                    {
                        Title = "Select RSDK Animation Editor.exe",
                        Filter = "Windows Executable|*.exe"
                    };
                    if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        Settings.mySettings.RunAniEdPath = ofd.FileName;
                }
                else
                {
                    if (!File.Exists(Settings.mySettings.RunGamePath))
                    {
                        Settings.mySettings.RunAniEdPath = "";
                        return;
                    }
                }

                ProcessStartInfo psi;
                psi = new ProcessStartInfo(Settings.mySettings.RunAniEdPath);
                Process.Start(psi);
            }
        }
        public void RenderListManager()
        {
            RenderListEditor editor = new RenderListEditor(Editor);
            editor.Owner = Editor.Owner;
            editor.ShowDialog();
        }
        public void ManiaPal(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.MenuItem button = sender as System.Windows.Controls.MenuItem;
            bool isGameConfig = false;
            bool GC_NULL = false;
            bool SC_NULL = false;
            string SC_Path = "";
            string GC_Path = "";

            if (button != null && button == Editor.maniaPalGameConfigToolStripMenuItem)
            {
                if (Editor.GameConfig == null) GC_NULL = true;
                else GC_Path = Editor.GameConfig.FilePath;
                isGameConfig = true;
            }
            else
            {
                if (Editor.StageConfig == null) SC_NULL = true;
                else SC_Path = Editor.StageConfig.FilePath;
                isGameConfig = false;
            }


            if (ManiaPalConnector == null) ManiaPalConnector = new ManiacED_ManiaPal.Connector();

            ManiaPalConnector.SetLoadingInformation(GC_Path, SC_Path, SC_NULL, GC_NULL);
            ManiaPalConnector.Activate(isGameConfig);



        }
        public void ManiaPalSubmenuOpened(object sender, RoutedEventArgs e)
        {
            Editor.maniaPalHint.Header = "HINT: The Button that houses this dropdown" + Environment.NewLine + "will focus ManiaPal if it is opened already" + Environment.NewLine + "(without reloading the currently loaded colors)";
        }
        public void DuplicateObjectIDHealer()
        {
            MessageBoxResult result = RSDKrU.MessageBox.Show("WARNING: Once you do this the editor will restart immediately, make sure your progress is closed and saved!", "WARNING", MessageBoxButton.OKCancel, MessageBoxImage.Information);
            if (result == MessageBoxResult.OK)
            {
                Editor.RepairScene();
            }
        }

        #endregion

        #region Folders Tab Buttons
        public void OpenSceneFolder()
        {
            if (Editor.Paths.SceneFile_Directory != null && Editor.Paths.SceneFile_Directory != "")
            {
                string SceneFilename_mod = Editor.Paths.SceneFile_Directory.Replace('/', '\\');
                Process.Start("explorer.exe", "/select, " + SceneFilename_mod);
            }
            else
            {
                RSDKrU.MessageBox.Show("Scene File does not exist or simply isn't loaded!", "ERROR");
            }

        }
        public void OpenDataDirectory()
        {
            string DataDirectory_mod = Editor.DataDirectory.Replace('/', '\\');
            if (DataDirectory_mod != null && DataDirectory_mod != "" && Directory.Exists(DataDirectory_mod))
            {
                Process.Start("explorer.exe", "/select, " + DataDirectory_mod);
            }
            else
            {
                RSDKrU.MessageBox.Show("Data Directory does not exist or simply isn't loaded!", "ERROR");
            }

        }

        public void OpenSonicManiaFolder()
        {
            if (Settings.mySettings.RunGamePath != null && Settings.mySettings.RunGamePath != "" && File.Exists(Settings.mySettings.RunGamePath))
            {
                string GameFolder = Settings.mySettings.RunGamePath;
                string GameFolder_mod = GameFolder.Replace('/', '\\');
                Process.Start("explorer.exe", "/select, " + GameFolder_mod);
            }
            else
            {
                RSDKrU.MessageBox.Show("Game Folder does not exist or isn't set!", "ERROR");
            }

        }
        public void OpenASavedPlaceDropDownOpening(object sender, RoutedEventArgs e)
        {
            if (Settings.mySettings.SavedPlaces != null && Settings.mySettings.SavedPlaces.Count > 0)
            {
                Editor.openASavedPlaceToolStripMenuItem.Items.Clear();
                var allItems = Editor.openASavedPlaceToolStripMenuItem.Items.Cast<System.Windows.Controls.MenuItem>().ToArray();
                foreach (string savedPlace in Settings.mySettings.SavedPlaces)
                {
                    var savedPlaceItem = new System.Windows.Controls.MenuItem()
                    {
                        Header = savedPlace,
                        Tag = savedPlace
                    };
                    savedPlaceItem.Click += OpenASavedPlaceTrigger;
                    Editor.openASavedPlaceToolStripMenuItem.Items.Add(savedPlaceItem);
                }
            }

        }

        public void OpenASavedPlaceTrigger(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.MenuItem item = sender as System.Windows.Controls.MenuItem;
            string savedPlaceDir = item.Header.ToString().Replace('/', '\\');
            Process.Start("explorer.exe", "/select, " + savedPlaceDir);
        }

        public void OpenASavedPlaceDropDownClosed(object sender, RoutedEventArgs e)
        {
            Editor.openASavedPlaceToolStripMenuItem.Items.Clear();
            Editor.openASavedPlaceToolStripMenuItem.Items.Add("No Saved Places");
        }

        #endregion
    }
}
