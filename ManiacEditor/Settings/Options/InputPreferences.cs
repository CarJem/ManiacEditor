using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManiacEditor.Methods.Solution;
using System.IO;
using Newtonsoft.Json;
using System.Reflection;
using System.Runtime;
using System.Runtime.Serialization;

namespace ManiacEditor.Classes.Options
{
    [DataContract]
    public class InputPreferences
    {
        public List<string> GetInput(string name)
        {
            BindingFlags bindingFlags = BindingFlags.Public |
                            BindingFlags.NonPublic |
                            BindingFlags.Instance |
                            BindingFlags.Static;

            var feilds = typeof(InputPreferences).GetProperties(bindingFlags);
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

        public void SetInput(string name, List<string> value)
        {
            this.GetType().GetField(name).SetValue(this, value);
        }

        public List<string> GetInputs()
        {
            BindingFlags bindingFlags = BindingFlags.Public |
                            BindingFlags.NonPublic |
                            BindingFlags.Instance |
                            BindingFlags.Static;

            List<string> PossibleInputs = new List<string>();
            var feilds = typeof(InputPreferences).GetProperties(bindingFlags);
            foreach (var entry in feilds)
            {
                PossibleInputs.Add(entry.Name);
            }
            return PossibleInputs;
        }

        [DataMember] 
        public List<string> NudgeFaster { get; set; } = new List<string>() { "Ctrl+F1" };
        [DataMember] 
        public List<string> ScrollLock { get; set; } = new List<string>() { "Ctrl+F2" };
        [DataMember] 
        public List<string> ScrollLockTypeSwitch { get; set; } = new List<string>() { "Ctrl+F3" };
        [DataMember] 
        public List<string> Cut { get; set; } = new List<string>() { "Ctrl+X" };
        [DataMember] 
        public List<string> Copy { get; set; } = new List<string>() { "Ctrl+C" };
        [DataMember] 
        public List<string> Paste { get; set; } = new List<string>() { "Ctrl+V" };
        [DataMember] 
        public List<string> Delete { get; set; } = new List<string>() { "Delete" };
        [DataMember] 
        public List<string> Duplicate { get; set; } = new List<string>() { "Ctrl+D" };
        [DataMember] 
        public List<string> SelectAll { get; set; } = new List<string>() { "Ctrl+A" };
        [DataMember] 
        public List<string> Undo { get; set; } = new List<string>() { "Ctrl+Z" };
        [DataMember] 
        public List<string> Redo { get; set; } = new List<string>() { "Ctrl+Y" };
        [DataMember] 
        public List<string> New { get; set; } = new List<string>() { "Ctrl+N" };
        [DataMember] 
        public List<string> Open { get; set; } = new List<string>() { "Ctrl+O" };
        [DataMember] 
        public List<string> OpenDataDir { get; set; } = new List<string>() { "Ctrl+Alt+O" };
        [DataMember] 
        public List<string> _Save { get; set; } = new List<string>() { "Ctrl+S" };
        [DataMember] 
        public List<string> SaveAs { get; set; } = new List<string>() { "Ctrl+Alt+S" };
        [DataMember] 
        public List<string> ResetZoomLevel { get; set; } = new List<string>() { "Ctrl+NumPad0", "Ctrl+0" };
        [DataMember] 
        public List<string> UnloadScene { get; set; } = new List<string>() { "Ctrl+U" };
        [DataMember] 
        public List<string> ShowPathA { get; set; } = new List<string>() { "Ctrl+1" };
        [DataMember] 
        public List<string> ShowPathB { get; set; } = new List<string>() { "Ctrl+2" };
        [DataMember] 
        public List<string> BackupNormal { get; set; } = new List<string>() { "None" };
        [DataMember] 
        public List<string> BackupStageConfig { get; set; } = new List<string>() { "None" };
        [DataMember] 
        public List<string> Recover { get; set; } = new List<string>() { "None" };
        [DataMember] 
        public List<string> RunScene { get; set; } = new List<string>() { "Ctrl+R" };
        [DataMember] 
        public List<string> RefreshResources { get; set; } = new List<string>() { "F5" };
        [DataMember] 
        public List<string> ShowGrid { get; set; } = new List<string>() { "Ctrl+G" };
        [DataMember] 
        public List<string> ShowTileID { get; set; } = new List<string>() { "Shift+3" };
        [DataMember] 
        public List<string> FlipH { get; set; } = new List<string>() { "M" };
        [DataMember] 
        public List<string> FlipV { get; set; } = new List<string>() { "F" };
        [DataMember] 
        public List<string> FlipVTiles { get; set; } = new List<string>() { "ShiftKey" };
        [DataMember] 
        public List<string> FlipHTiles { get; set; } = new List<string>() { "ControlKey" };
        [DataMember] 
        public List<string> FlipHIndv { get; set; } = new List<string>() { "Ctrl+M" };
        [DataMember] 
        public List<string> FlipVIndv { get; set; } = new List<string>() { "Ctrl+F" };
        [DataMember] 
        public List<string> StatusBoxToggle { get; set; } = new List<string>() { "F3" };
        [DataMember] 
        public List<string> PasteToChunk { get; set; } = new List<string>() { "Ctrl+Shift+V" };
        [DataMember] 
        public List<string> ForceOpenOnStartup { get; set; } = new List<string>() { "None" };
        [DataMember] 
        public List<string> DeveloperInterface { get; set; } = new List<string>() { "None" };
        [DataMember] 
        public List<string> CopyAirTiles { get; set; } = new List<string>() { "Ctrl+P" };
        [DataMember] 
        public List<string> PointerTooltipToggle { get; set; } = new List<string>() { "None" };
        [DataMember] 
        public List<string> PointerTool { get; set; } = new List<string>() { "1" };
        [DataMember] 
        public List<string> SelectTool { get; set; } = new List<string>() { "2" };
        [DataMember] 
        public List<string> DrawTool { get; set; } = new List<string>() { "3" };
        [DataMember] 
        public List<string> SplineTool { get; set; } = new List<string>() { "4" };
        [DataMember] 
        public List<string> StampTool { get; set; } = new List<string>() { "5" };
        [DataMember] 
        public List<string> MagnetTool { get; set; } = new List<string>() { "6" };
        [DataMember] 
        public List<string> TileManiacNewInstance { get; set; } = new List<string>() { "Ctrl+N" };
        [DataMember] 
        public List<string> TileManiacOpen { get; set; } = new List<string>() { "Ctrl+O" };
        [DataMember] 
        public List<string> TileManiacSave { get; set; } = new List<string>() { "Ctrl+S" };
        [DataMember] 
        public List<string> TileManiacSaveAs { get; set; } = new List<string>() { "Ctrl+Alt+S" };
        [DataMember] 
        public List<string> TileManiacSaveAsUncompressed { get; set; } = new List<string>() { "Ctrl+Alt+U" };
        [DataMember] 
        public List<string> TileManiacSaveUncompressed { get; set; } = new List<string>() { "Ctrl+U" };
        [DataMember] 
        public List<string> TileManiacbackupConfig { get; set; }
        [DataMember] 
        public List<string> TileManiacbackupImage { get; set; }
        [DataMember] 
        public List<string> TileManiacImportFromOlderRSDK { get; set; }
        [DataMember] 
        public List<string> TileManiacOpenSingleColMask { get; set; }
        [DataMember] 
        public List<string> TileManiacExportColMask { get; set; }
        [DataMember] 
        public List<string> TileManiacCopy { get; set; } = new List<string>() { "Ctrl+C" };
        [DataMember] 
        public List<string> TileManiacPaste { get; set; } = new List<string>() { "Ctrl+V" };
        [DataMember] 
        public List<string> TileManiacPastetoOther { get; set; } = new List<string>() { "Ctrl+Alt+V" };
        [DataMember] 
        public List<string> TileManiacMirrorMode { get; set; } = new List<string>() { "Ctrl+M" };
        [DataMember] 
        public List<string> TileManiacRestorePathA { get; set; }
        [DataMember] 
        public List<string> TileManiacRestorePathB { get; set; }
        [DataMember] 
        public List<string> TileManiacRestorePaths { get; set; }
        [DataMember] 
        public List<string> TileManiacShowPathB { get; set; } = new List<string>() { "Ctrl+B" };
        [DataMember] 
        public List<string> TileManiacShowGrid { get; set; } = new List<string>() { "Ctrl+G" };
        [DataMember] 
        public List<string> TileManiacClassicMode { get; set; }
        [DataMember] 
        public List<string> TileManiacSplitFile { get; set; }
        [DataMember] 
        public List<string> TileManiacFlipTileH { get; set; }
        [DataMember] 
        public List<string> TileManiacFlipTileV { get; set; }
        [DataMember] 
        public List<string> TileManiacHomeFolderOpen { get; set; }
        [DataMember] 
        public List<string> TileManiacAbout { get; set; }
        [DataMember] 
        public List<string> TileManiacSettings { get; set; }
        [DataMember] 
        public List<string> TileManiacWindowAlwaysOnTop { get; set; } = new List<string>() { "Ctrl+T" };
        [DataMember] 
        public List<string> TileManiacCut { get; set; } = new List<string>() { "Ctrl+X" };

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
                File.WriteAllText(Methods.ProgramPaths.InputPreferencesFilePath, json);
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
                if (!File.Exists(Methods.ProgramPaths.InputPreferencesFilePath)) File.Create(Methods.ProgramPaths.InputPreferencesFilePath).Close();
                string json = File.ReadAllText(Methods.ProgramPaths.InputPreferencesFilePath);
                try
                {
                    JsonSerializerSettings settings = new JsonSerializerSettings();
                    settings.ObjectCreationHandling = ObjectCreationHandling.Replace;
                    InputPreferences result = JsonConvert.DeserializeObject<InputPreferences>(json, settings);
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
