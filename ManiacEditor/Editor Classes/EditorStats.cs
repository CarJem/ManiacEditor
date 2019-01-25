using ManiacEditor.Entity_Renders;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ManiacEditor
{
    public partial class StatusBox
    {
        string newLine = Environment.NewLine;
        System.Windows.Forms.Timer t;
        public Editor EditorInstance;
        public bool panelMode = false;
        public StatusBox(Editor instance)
        {
            EditorInstance = instance;
            t = new System.Windows.Forms.Timer();
            t.Interval = 10;
            t.Tick += new EventHandler(setText);
            t.Start();
        }

        private void setText(object sender, EventArgs e)
        {
            ApplyText();
        }



        private void ApplyText()
        {
            //informationLabel.Text = GetStatsText();
        }


        public string GetStatsText()
        {
            string statsText = GetDataFolder();
            statsText = statsText + newLine + GetModDataFolder();
            statsText = statsText + newLine + GetMasterDataFolder();
            statsText = statsText + newLine + GetZoom();
            statsText = statsText + newLine + GetSceneFilePath();
            statsText = statsText + newLine + GetScenePath();
            statsText = statsText + newLine + GetSelectedZone();
            statsText = statsText + newLine +  GetSceneTileConfigPath();
            statsText = statsText + newLine + GetSetupObject();
            return statsText;
        }

        public string GetSceneTileConfigPath()
        {
            if (EditorInstance.SceneFilepath != null && EditorInstance.SceneFilepath != "") return "Scene TileConfig Path: " + Path.Combine(EditorInstance.SceneFilepath, "TileConfig.bin").ToString();         
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

        public string GetSceneFilePath()
        {
            if (EditorInstance.SceneFilepath != null && EditorInstance.SceneFilepath != "") return "Scene Filepath: " + EditorInstance.SceneFilepath;
            else return "Scene Filepath: N/A";
        }

        public string GetZoom()
        {
            return "Zoom Level: " + EditorInstance.GetZoom();
        }

        public string GetSelectedZone()
        {
            if (EditorInstance.SelectedZone != null && EditorInstance.SelectedZone != "") return "Selected Zone: " + EditorInstance.SelectedZone;
            else return "Selected Zone: N/A";
        }

        public string GetScenePath()
        {
            if (EditorInstance.Discord.ScenePath != null && EditorInstance.Discord.ScenePath != "") return "Scene Path: " + EditorInstance.Discord.ScenePath;
            else return "Scene Path: N/A";
        }

        public string GetModDataFolder()
        {
            if (EditorInstance.ModDataDirectory != null && EditorInstance.ModDataDirectory != "") return "Mod Data Directory: " + EditorInstance.ModDataDirectory;
            else return "Mod Data Directory: N/A";
        }

        public string GetDataFolder()
        {
            if (EditorInstance.DataDirectory != null && EditorInstance.DataDirectory != "") return "Data Directory: " + EditorInstance.DataDirectory;
            else return "Data Directory: N/A";
        }
        public string GetMasterDataFolder()
        {
            if (EditorInstance.MasterDataDirectory != null && EditorInstance.MasterDataDirectory != "") return "Master Data Directory: " + EditorInstance.MasterDataDirectory;
            else return "Master Data Directory: N/A";
        }

        public string GetSetupObject()
        {
            if (EditorInstance.entities != null && EditorInstance.entities.SetupObject != null && EditorInstance.entities.SetupObject != "")
            {
                return "Setup Object: " + EditorInstance.entities.SetupObject;
            }
            else
            {
                return "Setup Object: N/A";
            }

        }


        private void controlBox_Quit(object sender, EventArgs e)
        {
            EditorInstance.controlWindowOpen = false;
        }
    }
}
