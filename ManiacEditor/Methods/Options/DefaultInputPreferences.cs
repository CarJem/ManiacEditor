using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace ManiacEditor.Methods.Options
{
    public class DefaultInputPreferences
    {
        public List<string> GetInput(string name)
        {
            BindingFlags bindingFlags = BindingFlags.Public |
                            BindingFlags.NonPublic |
                            BindingFlags.Instance |
                            BindingFlags.Static;

            var feilds = typeof(DefaultInputPreferences).GetProperties(bindingFlags);
            foreach (var entry in feilds)
            {
                if (entry.Name == name)
                {
                    object value = entry.GetValue(this, null);
                    if (value != null && value is List<string>) return (List<string>)value;
                    else return null;

                }
            }
            return null;
        }

        public static List<string> NudgeFaster { get; set; } = new List<string>() { "Ctrl+F1" };
        public static List<string> ScrollLock { get; set; } = new List<string>() { "Ctrl+F2" };
        public static List<string> ScrollLockTypeSwitch { get; set; } = new List<string>() { "Ctrl+F3" };
        public static List<string> Cut { get; set; } = new List<string>() { "Ctrl+X" };
        public static List<string> Copy { get; set; } = new List<string>() { "Ctrl+C" };
        public static List<string> Paste { get; set; } = new List<string>() { "Ctrl+V" };
        public static List<string> Delete { get; set; } = new List<string>() { "Delete" };
        public static List<string> Duplicate { get; set; } = new List<string>() { "Ctrl+D" };
        public static List<string> SelectAll { get; set; } = new List<string>() { "Ctrl+A" };
        public static List<string> Undo { get; set; } = new List<string>() { "Ctrl+Z" };
        public static List<string> Redo { get; set; } = new List<string>() { "Ctrl+Y" };
        public static List<string> New { get; set; } = new List<string>() { "Ctrl+N" };
        public static List<string> Open { get; set; } = new List<string>() { "Ctrl+O" };
        public static List<string> OpenDataDir { get; set; } = new List<string>() { "Ctrl+Alt+O" };
        public static List<string> _Save { get; set; } = new List<string>() { "Ctrl+S" };
        public static List<string> SaveAs { get; set; } = new List<string>() { "Ctrl+Alt+S" };
        public static List<string> ResetZoomLevel { get; set; } = new List<string>() { "Ctrl+NumPad0", "Ctrl+0" };
        public static List<string> UnloadScene { get; set; } = new List<string>() { "Ctrl+U" };
        public static List<string> ShowPathA { get; set; } = new List<string>() { "Ctrl+1" };
        public static List<string> ShowPathB { get; set; } = new List<string>() { "Ctrl+2" };
        public static List<string> BackupNormal { get; set; } = new List<string>() { "None" };
        public static List<string> BackupStageConfig { get; set; } = new List<string>() { "None" };
        public static List<string> Recover { get; set; } = new List<string>() { "None" };
        public static List<string> RunScene { get; set; } = new List<string>() { "Ctrl+R" };
        public static List<string> RefreshResources { get; set; } = new List<string>() { "F5" };
        public static List<string> ShowGrid { get; set; } = new List<string>() { "Ctrl+G" };
        public static List<string> ShowTileID { get; set; } = new List<string>() { "Shift+3" };
        public static List<string> FlipH { get; set; } = new List<string>() { "M" };
        public static List<string> FlipV { get; set; } = new List<string>() { "F" };
        public static List<string> FlipVTiles { get; set; } = new List<string>() { "ShiftKey" };
        public static List<string> FlipHTiles { get; set; } = new List<string>() { "ControlKey" };
        public static List<string> FlipHIndv { get; set; } = new List<string>() { "Ctrl+M" };
        public static List<string> FlipVIndv { get; set; } = new List<string>() { "Ctrl+F" };
        public static List<string> StatusBoxToggle { get; set; } = new List<string>() { "F3" };
        public static List<string> PasteToChunk { get; set; } = new List<string>() { "Ctrl+Shift+V" };
        public static List<string> ForceOpenOnStartup { get; set; } = new List<string>() { "None" };
        public static List<string> DeveloperInterface { get; set; } = new List<string>() { "None" };
        public static List<string> CopyAirTiles { get; set; } = new List<string>() { "Ctrl+P" };
        public static List<string> PointerTooltipToggle { get; set; } = new List<string>() { "None" };
        public static List<string> PointerTool { get; set; } = new List<string>() { "1" };
        public static List<string> SelectTool { get; set; } = new List<string>() { "2" };
        public static List<string> DrawTool { get; set; } = new List<string>() { "3" };
        public static List<string> SplineTool { get; set; } = new List<string>() { "4" };
        public static List<string> StampTool { get; set; } = new List<string>() { "5" };
        public static List<string> MagnetTool { get; set; } = new List<string>() { "6" };
        public static List<string> TileManiacNewInstance { get; set; } = new List<string>() { "Ctrl+N" };
        public static List<string> TileManiacOpen { get; set; } = new List<string>() { "Ctrl+O" };
        public static List<string> TileManiacSave { get; set; } = new List<string>() { "Ctrl+S" };
        public static List<string> TileManiacSaveAs { get; set; } = new List<string>() { "Ctrl+Alt+S" };
        public static List<string> TileManiacSaveAsUncompressed { get; set; } = new List<string>() { "Ctrl+Alt+U" };
        public static List<string> TileManiacSaveUncompressed { get; set; } = new List<string>() { "Ctrl+U" };
        public static List<string> TileManiacbackupConfig { get; set; }
        public static List<string> TileManiacbackupImage { get; set; }
        public static List<string> TileManiacImportFromOlderRSDK { get; set; }
        public static List<string> TileManiacOpenSingleColMask { get; set; }
        public static List<string> TileManiacExportColMask { get; set; }
        public static List<string> TileManiacCopy { get; set; } = new List<string>() { "Ctrl+C" };
        public static List<string> TileManiacPaste { get; set; } = new List<string>() { "Ctrl+V" };
        public static List<string> TileManiacPastetoOther { get; set; } = new List<string>() { "Ctrl+Alt+V" };
        public static List<string> TileManiacMirrorMode { get; set; } = new List<string>() { "Ctrl+M" };
        public static List<string> TileManiacRestorePathA { get; set; }
        public static List<string> TileManiacRestorePathB { get; set; }
        public static List<string> TileManiacRestorePaths { get; set; }
        public static List<string> TileManiacShowPathB { get; set; } = new List<string>() { "Ctrl+B" };
        public static List<string> TileManiacShowGrid { get; set; } = new List<string>() { "Ctrl+G" };
        public static List<string> TileManiacClassicMode { get; set; }
        public static List<string> TileManiacSplitFile { get; set; }
        public static List<string> TileManiacFlipTileH { get; set; }
        public static List<string> TileManiacFlipTileV { get; set; }
        public static List<string> TileManiacHomeFolderOpen { get; set; }
        public static List<string> TileManiacAbout { get; set; }
        public static List<string> TileManiacSettings { get; set; }
        public static List<string> TileManiacWindowAlwaysOnTop { get; set; } = new List<string>() { "Ctrl+T" };
        public static List<string> TileManiacCut { get; set; } = new List<string>() { "Ctrl+X" };
    }
}
