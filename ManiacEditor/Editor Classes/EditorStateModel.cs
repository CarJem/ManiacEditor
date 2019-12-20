using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;

namespace ManiacEditor
{
    public class EditorStateModel
    {
        public Editor EditorInstance;
        public EditorStateModel(Editor instance)
        {
            EditorInstance = instance;
        }

        #region Debug HUD Information

        public string GetSceneTileConfigPath()
        {
            if (EditorInstance.Paths.TileConfig_Source != null && EditorInstance.Paths.TileConfig_Source != "") return "Scene TileConfig Path: " + Path.Combine(EditorInstance.Paths.TileConfig_Source, "TileConfig.bin").ToString();         
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
            if (EditorInstance.Paths.CurrentZone != null && EditorInstance.Paths.CurrentZone != "") return "Selected Zone: " + EditorInstance.Paths.CurrentZone;
            else return "Selected Zone: N/A";
        }

		public string GetSceneFilePath()
		{
			if (EditorInstance.Paths.SceneFile_Source != null && EditorInstance.Paths.SceneFile_Source != "") return "Scene File: " + EditorInstance.Paths.SceneFile_Source;
			else return "Scene File: N/A";
		}

		public string GetScenePath()
        {

            if (EditorInstance.Paths.SceneFile_Directory != null && EditorInstance.Paths.SceneFile_Directory != "") return "Scene Path: " + EditorInstance.Paths.SceneFile_Directory;
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
            if (EditorSolution.Entities != null && EditorSolution.Entities.SetupObject != null && EditorSolution.Entities.SetupObject != "")
            {
                return "Setup Object: " + EditorSolution.Entities.SetupObject;
            }
            else
            {
                return "Setup Object: N/A";
            }

        }

        #endregion

        public static int LastX { get; set; }
        public static int LastY { get; set; }
        public static int DraggedX { get; set; }
        public static int DraggedY { get; set; }

        public static int ScreenWidth { get; set; }
        public static int ScreenHeight { get; set; }
        public static int CustomX { get; set; } = 0;
        public static int CustomY { get; set; } = 0;

        #region View Position Controls

        public static int ViewPositionX { get => GetViewPositionX(); set => SetViewPositionX(value); }
        public static int ViewPositionY { get => GetViewPositionY(); set => SetViewPositionY(value); }


        public static int GetViewPositionX()
        {
            if (Editor.Instance.FormsModel.hScrollBar1 != null)
            {
                return (int)Editor.Instance.FormsModel.hScrollBar1.Value;
            }
            else return 0;
        }

        public static int GetViewPositionY()
        {
            if (Editor.Instance.FormsModel.vScrollBar1 != null)
            {
                return (int)Editor.Instance.FormsModel.vScrollBar1.Value;
            }
            else return 0;
        }

        public static void SetViewPositionX(int value)
        {
            if (Editor.Instance.FormsModel.hScrollBar1 != null)
            {
                Editor.Instance.FormsModel.hScrollBar1.Value = value;
            }
        }

        public static void SetViewPositionY(int value)
        {
            if (Editor.Instance.FormsModel.vScrollBar1 != null)
            {
                Editor.Instance.FormsModel.vScrollBar1.Value = value;
            }
        }

        #endregion

        public static int RegionX1 { get; set; } = -1;
        public static int RegionY1 { get; set; } = -1;
        public static int RegionX2 { get; set; }
        public static int RegionY2 { get; set; }

        public static int select_x1 { get; set; }
        public static int select_x2 { get; set; }
        public static int select_y1 { get; set; }
        public static int select_y2 { get; set; }


        public static bool DraggingSelection { get; set; } = false; //Determines if we are dragging a selection
        public static bool Dragged { get; set; } = false;
        public static bool StartDragged { get; set; } = false;
        public static bool Zooming { get; set; } = false;  //Detects if we are zooming
        public static double Zoom { get; set; } = 1; //Double Value for Zoom Levels
        public static int ZoomLevel { get; set; } = 0; //Interger Value for Zoom Levels
        public static bool Scrolling { get; set; } = false; //Determines if the User is Scrolling
        public static bool ScrollingDragged { get; set; } = false;
        public static bool WheelClicked { get; set; } = false; //Dermines if the mouse wheel was clicked or is the user is drag-scrolling.
        public static Point ScrollPosition { get; set; } //For Getting the Scroll Position


        public static int SelectedTilesCount; //Used to get the Amount of Selected Tiles in a Selection
        public static int DeselectTilesCount; //Used in combination with SelectedTilesCount to get the definitive amount of Selected Tiles
        public static int SelectedTileX { get; set; } = 0; //Used to get a single Selected Tile's X
        public static int SelectedTileY { get; set; } = 0; //Used to get a single Selected Tile's Y


        public static bool isTileDrawing { get; set; } = false;
    }
}
