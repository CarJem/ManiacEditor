using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManiacEditor.Classes.Editor;
using System.IO;
using Newtonsoft.Json;

namespace ManiacEditor.Core.Options
{
    public class InputPreferences
    {

        public System.Collections.Specialized.StringCollection GetInput(string name)
        {
            return (System.Collections.Specialized.StringCollection)this.GetType().GetField(name).GetValue(this);
        }

        public void SetInput(string name, System.Collections.Specialized.StringCollection value)
        {
            this.GetType().GetField(name).SetValue(this, value);
        }

        public List<string> GetInputs()
        {
            List<string> PossibleInputs = new List<string>();
            var feilds = this.GetType().GetFields();
            foreach (var entry in feilds)
            {
                PossibleInputs.Add(entry.Name);
            }
            return PossibleInputs;
        }


        public System.Collections.Specialized.StringCollection NudgeFaster { get; set; } = new System.Collections.Specialized.StringCollection() { "Ctrl+F1" };
        public System.Collections.Specialized.StringCollection ScrollLock { get; set; } = new System.Collections.Specialized.StringCollection() { "Ctrl+F2" };
        public System.Collections.Specialized.StringCollection ScrollLockTypeSwitch { get; set; } = new System.Collections.Specialized.StringCollection() { "Ctrl+F3" };
        public System.Collections.Specialized.StringCollection Cut { get; set; } = new System.Collections.Specialized.StringCollection() { "Ctrl+X" };
        public System.Collections.Specialized.StringCollection Copy { get; set; } = new System.Collections.Specialized.StringCollection() { "Ctrl+C" };
        public System.Collections.Specialized.StringCollection Paste { get; set; } = new System.Collections.Specialized.StringCollection() { "Ctrl+V" };
        public System.Collections.Specialized.StringCollection Delete { get; set; } = new System.Collections.Specialized.StringCollection() { "Delete" };
        public System.Collections.Specialized.StringCollection Duplicate { get; set; } = new System.Collections.Specialized.StringCollection() { "Ctrl+D" };
        public System.Collections.Specialized.StringCollection SelectAll { get; set; } = new System.Collections.Specialized.StringCollection() { "Ctrl+A" };
        public System.Collections.Specialized.StringCollection Undo { get; set; } = new System.Collections.Specialized.StringCollection() { "Ctrl+Z" };
        public System.Collections.Specialized.StringCollection Redo { get; set; } = new System.Collections.Specialized.StringCollection() { "Ctrl+Y" };
        public System.Collections.Specialized.StringCollection New { get; set; } = new System.Collections.Specialized.StringCollection() { "Ctrl+N" };
        public System.Collections.Specialized.StringCollection Open { get; set; } = new System.Collections.Specialized.StringCollection() { "Ctrl+O" };
        public System.Collections.Specialized.StringCollection OpenDataDir { get; set; } = new System.Collections.Specialized.StringCollection() { "Ctrl+Alt+O" };
        public System.Collections.Specialized.StringCollection _Save { get; set; } = new System.Collections.Specialized.StringCollection() { "Ctrl+S" };
        public System.Collections.Specialized.StringCollection SaveAs { get; set; } = new System.Collections.Specialized.StringCollection() { "Ctrl+Alt+S" };
        public System.Collections.Specialized.StringCollection ResetZoomLevel { get; set; } = new System.Collections.Specialized.StringCollection() { "Ctrl+NumPad0", "Ctrl+0" };
        public System.Collections.Specialized.StringCollection UnloadScene { get; set; } = new System.Collections.Specialized.StringCollection() { "Ctrl+U" };
        public System.Collections.Specialized.StringCollection ShowPathA { get; set; } = new System.Collections.Specialized.StringCollection() { "Ctrl+1" };
        public System.Collections.Specialized.StringCollection ShowPathB { get; set; } = new System.Collections.Specialized.StringCollection() { "Ctrl+2" };
        public System.Collections.Specialized.StringCollection BackupNormal { get; set; } = new System.Collections.Specialized.StringCollection() { "None" };
        public System.Collections.Specialized.StringCollection BackupStageConfig { get; set; } = new System.Collections.Specialized.StringCollection() { "None" };
        public System.Collections.Specialized.StringCollection Recover { get; set; } = new System.Collections.Specialized.StringCollection() { "None" };
        public System.Collections.Specialized.StringCollection RunScene { get; set; } = new System.Collections.Specialized.StringCollection() { "Ctrl+R" };
        public System.Collections.Specialized.StringCollection RefreshResources { get; set; } = new System.Collections.Specialized.StringCollection() { "F5" };
        public System.Collections.Specialized.StringCollection ShowGrid { get; set; } = new System.Collections.Specialized.StringCollection() { "Ctrl+G" };
        public System.Collections.Specialized.StringCollection ShowTileID { get; set; } = new System.Collections.Specialized.StringCollection() { "Shift+3" };
        public System.Collections.Specialized.StringCollection FlipH { get; set; } = new System.Collections.Specialized.StringCollection() { "M" };
        public System.Collections.Specialized.StringCollection FlipV { get; set; } = new System.Collections.Specialized.StringCollection() { "F" };
        public System.Collections.Specialized.StringCollection FlipVTiles { get; set; } = new System.Collections.Specialized.StringCollection() { "ShiftKey" };
        public System.Collections.Specialized.StringCollection FlipHTiles { get; set; } = new System.Collections.Specialized.StringCollection() { "ControlKey" };
        public System.Collections.Specialized.StringCollection FlipHIndv { get; set; } = new System.Collections.Specialized.StringCollection() { "Ctrl+M" };
        public System.Collections.Specialized.StringCollection FlipVIndv { get; set; } = new System.Collections.Specialized.StringCollection() { "Ctrl+F" };
        public System.Collections.Specialized.StringCollection StatusBoxToggle { get; set; } = new System.Collections.Specialized.StringCollection() { "F3" };
        public System.Collections.Specialized.StringCollection PasteToChunk { get; set; } = new System.Collections.Specialized.StringCollection() { "Ctrl+Shift+V" };
        public System.Collections.Specialized.StringCollection ForceOpenOnStartup { get; set; } = new System.Collections.Specialized.StringCollection() { "None" };
        public System.Collections.Specialized.StringCollection DeveloperInterface { get; set; } = new System.Collections.Specialized.StringCollection() { "None" };
        public System.Collections.Specialized.StringCollection CopyAirTiles { get; set; } = new System.Collections.Specialized.StringCollection() { "Ctrl+P" };
        public System.Collections.Specialized.StringCollection PointerTooltipToggle { get; set; } = new System.Collections.Specialized.StringCollection() { "None" };
        public System.Collections.Specialized.StringCollection PointerTool { get; set; } = new System.Collections.Specialized.StringCollection() { "1" };
        public System.Collections.Specialized.StringCollection SelectTool { get; set; } = new System.Collections.Specialized.StringCollection() { "2" };
        public System.Collections.Specialized.StringCollection DrawTool { get; set; } = new System.Collections.Specialized.StringCollection() { "3" };
        public System.Collections.Specialized.StringCollection SplineTool { get; set; } = new System.Collections.Specialized.StringCollection() { "4" };
        public System.Collections.Specialized.StringCollection StampTool { get; set; } = new System.Collections.Specialized.StringCollection() { "5" };
        public System.Collections.Specialized.StringCollection MagnetTool { get; set; } = new System.Collections.Specialized.StringCollection() { "6" };
        public System.Collections.Specialized.StringCollection TileManiacNewInstance { get; set; } = new System.Collections.Specialized.StringCollection() { "Ctrl+N" };
        public System.Collections.Specialized.StringCollection TileManiacOpen { get; set; } = new System.Collections.Specialized.StringCollection() { "Ctrl+O" };
        public System.Collections.Specialized.StringCollection TileManiacSave { get; set; } = new System.Collections.Specialized.StringCollection() { "Ctrl+S" };
        public System.Collections.Specialized.StringCollection TileManiacSaveAs { get; set; } = new System.Collections.Specialized.StringCollection() { "Ctrl+Alt+S" };
        public System.Collections.Specialized.StringCollection TileManiacSaveAsUncompressed { get; set; } = new System.Collections.Specialized.StringCollection() { "Ctrl+Alt+U" };
        public System.Collections.Specialized.StringCollection TileManiacSaveUncompressed { get; set; } = new System.Collections.Specialized.StringCollection() { "Ctrl+U" };
        public System.Collections.Specialized.StringCollection TileManiacbackupConfig { get; set; }
        public System.Collections.Specialized.StringCollection TileManiacbackupImage { get; set; }
        public System.Collections.Specialized.StringCollection TileManiacImportFromOlderRSDK { get; set; }
        public System.Collections.Specialized.StringCollection TileManiacOpenSingleColMask { get; set; }
        public System.Collections.Specialized.StringCollection TileManiacExportColMask { get; set; }
        public System.Collections.Specialized.StringCollection TileManiacCopy { get; set; } = new System.Collections.Specialized.StringCollection() { "Ctrl+C" };
        public System.Collections.Specialized.StringCollection TileManiacPaste { get; set; } = new System.Collections.Specialized.StringCollection() { "Ctrl+V" };
        public System.Collections.Specialized.StringCollection TileManiacPastetoOther { get; set; } = new System.Collections.Specialized.StringCollection() { "Ctrl+Alt+V" };
        public System.Collections.Specialized.StringCollection TileManiacMirrorMode { get; set; } = new System.Collections.Specialized.StringCollection() { "Ctrl+M" };
        public System.Collections.Specialized.StringCollection TileManiacRestorePathA { get; set; }
        public System.Collections.Specialized.StringCollection TileManiacRestorePathB { get; set; }
        public System.Collections.Specialized.StringCollection TileManiacRestorePaths { get; set; }
        public System.Collections.Specialized.StringCollection TileManiacShowPathB { get; set; } = new System.Collections.Specialized.StringCollection() { "Ctrl+B" };
        public System.Collections.Specialized.StringCollection TileManiacShowGrid { get; set; } = new System.Collections.Specialized.StringCollection() { "Ctrl+G" };
        public System.Collections.Specialized.StringCollection TileManiacClassicMode { get; set; }
        public System.Collections.Specialized.StringCollection TileManiacSplitFile { get; set; }
        public System.Collections.Specialized.StringCollection TileManiacFlipTileH { get; set; }
        public System.Collections.Specialized.StringCollection TileManiacFlipTileV { get; set; }
        public System.Collections.Specialized.StringCollection TileManiacHomeFolderOpen { get; set; }
        public System.Collections.Specialized.StringCollection TileManiacAbout { get; set; }
        public System.Collections.Specialized.StringCollection TileManiacSettings { get; set; }
        public System.Collections.Specialized.StringCollection TileManiacWindowAlwaysOnTop { get; set; } = new System.Collections.Specialized.StringCollection() { "Ctrl+T" };
        public System.Collections.Specialized.StringCollection TileManiacCut { get; set; } = new System.Collections.Specialized.StringCollection() { "Ctrl+X" };

        #region Accessors
        public static void Init()
        {
            Reload();
        }
        private static InputPreferences DefaultInstance;
        public static InputPreferences Default
        {
            get
            {
                return DefaultInstance;
            }
        }
        public static void Save()
        {
            try
            {
                string json = JsonConvert.SerializeObject(DefaultInstance);
                File.WriteAllText(Constants.InputPreferencesFilePath, json);
            }
            catch (Exception ex)
            {
                Methods.ProgramBase.Log.ErrorFormat("Failed to Write InputPreferences to File! Reason: {0}", ex.Message);
            }
        }
        public static void Reload()
        {
            try
            {
                if (!File.Exists(Constants.InputPreferencesFilePath)) File.Create(Constants.InputPreferencesFilePath).Close();
                string json = File.ReadAllText(Constants.InputPreferencesFilePath);
                try
                {
                    InputPreferences result = JsonConvert.DeserializeObject<InputPreferences>(json);
                    if (result != null) DefaultInstance = result;
                    else DefaultInstance = new InputPreferences();
                }
                catch
                {
                    DefaultInstance = new InputPreferences();
                }
            }
            catch (Exception ex)
            {
                Methods.ProgramBase.Log.ErrorFormat("Failed to Load InputPreferences! Reason: {0}", ex.Message);
                Methods.ProgramBase.Log.InfoFormat("Creating a new InputPreferences in Memory...");
                DefaultInstance = new InputPreferences();
            }

        }
        public static void Reset()
        {
            DefaultInstance = new InputPreferences();
            Save();
            Reload();
        }
        public static void Upgrade()
        {

        }
        #endregion
    }
}
