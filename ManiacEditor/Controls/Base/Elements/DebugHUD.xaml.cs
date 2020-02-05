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

namespace ManiacEditor.Controls.Base.Elements
{
    /// <summary>
    /// Interaction logic for EditorHUD.xaml
    /// </summary>
    public partial class DebugHUD : UserControl
    {
        private MainEditor Instance { get; set; }
        private System.Windows.Forms.Timer t { get; set; }
        public DebugHUD()
        {
            InitializeComponent();
            t = new System.Windows.Forms.Timer();
            t.Tick += T_Tick;
            t.Start();
        }

        private void T_Tick(object sender, EventArgs e)
        {
            UpdateHUDInfo();
        }

        public void UpdateInstance(MainEditor editor)
        {
            Instance = editor;
        }

        private void UpdateHUDInfo()
        {
            MemoryUsage.Text = GetMemoryUsage();
            PhysicalMemoryUsage.Text = GetPhysicalMemoryUsage();
        }


        #region Debug HUD Information

        public string GetSceneTileConfigPath()
        {
            if (Instance.Paths.TileConfig_Source != null && Instance.Paths.TileConfig_Source != "") return "Scene TileConfig Path: " + System.IO.Path.Combine(Instance.Paths.TileConfig_Source, "TileConfig.bin").ToString();
            else return "Scene TileConfig Path: N/A";
        }

        public string GetMemoryUsage()
        {
            Process proc = Process.GetCurrentProcess();
            long memory = proc.PrivateMemorySize64;
            double finalMem = ConvertBytesToMegabytes(memory);
            return "Memory Usage: " + finalMem.ToString() + " MB";
        }

        public string GetPhysicalMemoryUsage()
        {
            Process proc = Process.GetCurrentProcess();
            long memoryWorkSet = proc.WorkingSet64;
            double finalMem = ConvertBytesToMegabytes(memoryWorkSet);
            return "Physical Memory Usage: " + finalMem.ToString() + " MB";
        }

        public string GetDeviceType()
        {
            Process proc = Process.GetCurrentProcess();
            long memoryWorkSet = proc.WorkingSet64;
            double finalMem = ConvertBytesToMegabytes(memoryWorkSet);
            return "Physical Memory Usage: " + finalMem.ToString() + " MB";
        }

        public string GetDevicePramaters()
        {
            Process proc = Process.GetCurrentProcess();
            long memoryWorkSet = proc.WorkingSet64;
            double finalMem = ConvertBytesToMegabytes(memoryWorkSet);
            return "Physical Memory Usage: " + finalMem.ToString() + " MB";
        }

        static double ConvertBytesToMegabytes(long bytes)
        {
            return (bytes / 1024f) / 1024f;
        }

        public string GetZoom()
        {
            return "Zoom Level: " + Classes.Editor.SolutionState.Zoom;
        }

        public string GetSelectedZone()
        {
            if (Instance.Paths.CurrentZone != null && Instance.Paths.CurrentZone != "") return "Selected Zone: " + Instance.Paths.CurrentZone;
            else return "Selected Zone: N/A";
        }

        public string GetSceneFilePath()
        {
            if (Instance.Paths.SceneFile_Source != null && Instance.Paths.SceneFile_Source != "") return "Scene File: " + Instance.Paths.SceneFile_Source;
            else return "Scene File: N/A";
        }

        public string GetScenePath()
        {

            if (Instance.Paths.SceneFile_Directory != null && Instance.Paths.SceneFile_Directory != "") return "Scene Path: " + Instance.Paths.SceneFile_Directory;
            else return "Scene Path: N/A";
        }

        public string GetDataFolder()
        {
            if (Instance.DataDirectory != null && Instance.DataDirectory != "") return "Data Directory: " + Instance.DataDirectory;
            else return "Data Directory: N/A";
        }
        public string GetMasterDataFolder()
        {
            if (Instance.MasterDataDirectory != null && Instance.MasterDataDirectory != "") return "Master Data Directory: " + Instance.MasterDataDirectory;
            else return "Master Data Directory: N/A";
        }

        public string GetSetupObject()
        {
            if (Classes.Editor.Solution.Entities != null && Classes.Editor.Solution.Entities.SetupObject != null && Classes.Editor.Solution.Entities.SetupObject != "")
            {
                return "Setup Object: " + Classes.Editor.Solution.Entities.SetupObject;
            }
            else
            {
                return "Setup Object: N/A";
            }

        }

        public string GetPosition()
        {
            int x = (int)((Classes.Editor.SolutionState.ViewPositionX / Classes.Editor.SolutionState.Zoom) / 16);
            int y = (int)((Classes.Editor.SolutionState.ViewPositionY / Classes.Editor.SolutionState.Zoom) / 16);
            return string.Format("Position: {0}, {1}", x, y);
        }

        #endregion


    }
}
