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
    public class EditorState
    {
        string newLine = Environment.NewLine;
        public Editor EditorInstance;
        public bool panelMode = false;
        public EditorState(Editor instance)
        {
            EditorInstance = instance;
            //t = new System.Windows.Forms.Timer();
            //t.Interval = 10;
            //t.Tick += new EventHandler(setText);
            //t.Start();
        }

        #region Debug HUD Information

        public string GetSceneTileConfigPath()
        {
            if (EditorInstance.EditorPath.TileConfig_Source != null && EditorInstance.EditorPath.TileConfig_Source != "") return "Scene TileConfig Path: " + Path.Combine(EditorInstance.EditorPath.TileConfig_Source, "TileConfig.bin").ToString();         
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
            return "Zoom Level: " + EditorInstance.GetZoom();
        }

        public string GetSelectedZone()
        {
            if (EditorInstance.EditorPath.CurrentZone != null && EditorInstance.EditorPath.CurrentZone != "") return "Selected Zone: " + EditorInstance.EditorPath.CurrentZone;
            else return "Selected Zone: N/A";
        }

		public string GetSceneFilePath()
		{
			if (EditorInstance.EditorPath.SceneFile_Source != null && EditorInstance.EditorPath.SceneFile_Source != "") return "Scene File: " + EditorInstance.EditorPath.SceneFile_Source;
			else return "Scene File: N/A";
		}

		public string GetScenePath()
        {

            if (EditorInstance.EditorPath.SceneFile_Directory != null && EditorInstance.EditorPath.SceneFile_Directory != "") return "Scene Path: " + EditorInstance.EditorPath.SceneFile_Directory;
            else return "Scene Path: N/A";
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

        #endregion

        #region Editing States 
        //Editor Editing States
        public bool dragged { get; set; }
        public bool startDragged { get; set; }
        public int lastX { get; set; }
        public int lastY { get; set; }
        public int draggedX { get; set; }
        public int draggedY { get; set; }
        public int ShiftX { get; set; } = 0;
        public int ShiftY { get; set; } = 0;
        public int ScreenWidth { get; set; }
        public int ScreenHeight { get; set; }
        public int CustomX { get; set; } = 0;
        public int CustomY { get; set; } = 0;
        public int select_x1 { get; set; }
        public int select_x2 { get; set; }
        public int select_y1 { get; set; }
        public int select_y2 { get; set; }
        public int ClickedX { get; set; }
        public int ClickedY { get; set; }
        public bool draggingSelection { get; set; } //Determines if we are dragging a selection
        public int selectingX { get; set; }
        public int selectingY { get; set; }
        public bool zooming { get; set; } //Detects if we are zooming
        public double Zoom { get; set; } = 1; //Double Value for Zoom Levels
        public int ZoomLevel { get; set; } = 0; //Interger Value for Zoom Levels
        public int SelectedTilesCount; //Used to get the Amount of Selected Tiles in a Selection
        public int DeselectTilesCount; //Used in combination with SelectedTilesCount to get the definitive amount of Selected Tiles
        public int SelectedTileX { get; set; } = 0; //Used to get a single Selected Tile's X
        public int SelectedTileY { get; set; } = 0; //Used to get a single Selected Tile's Y
        public bool scrolling { get; set; } = false; //Determines if the User is Scrolling
        public bool scrollingDragged { get; set; } = false;
        public bool wheelClicked { get; set; } = false; //Dermines if the mouse wheel was clicked or is the user is drag-scrolling.
        public Point scrollPosition { get; set; } //For Getting the Scroll Position
        #endregion
    }
}
