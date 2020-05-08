using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using ManiacEditor.Methods.Solution;
using System.IO;
using Newtonsoft.Json;

namespace ManiacEditor.Classes.Options
{
    public class DefaultPrefrences
    {
        public bool FGLowerDefault { get; set; } = true;
        public bool FGLowDefault { get; set; } = true;
        public bool FGHighDefault { get; set; } = true;
        public bool FGHigherDefault { get; set; } = true;
        public bool SolidTopADefault { get; set; } = false;
        public bool SolidAllButTopADefault { get; set; } = false;
        public bool SolidTopBDefault { get; set; } = false;
        public bool SolidAllButTopBDefault { get; set; } = false;
        public bool EntitiesDefault { get; set; } = true;
        public Int32 TilesDefaultZoom { get; set; } = 1;
        public Int32 ChunksDefaultZoom { get; set; } = 1;
        public Int32 DefaultGridSizeOption { get; set; } = 0;
        public Int32 DefaultCollisionColors { get; set; } = 0;
        public bool SceneSelectFilesViewDefault { get; set; } = false;
        public Color DefaultGridColor { get; set; } = Color.Black;
        public bool EditEntitiesTransparentLayersDefault { get; set; } = false;
        public bool ScrollLockDefault { get; set; } = true;
        public bool EnablePixelModeDefault { get; set; } = false;
        public bool ShowEntityArrowPathsDefault { get; set; } = false;
        public bool ShowWaterEntityLevelDefault { get; set; } = false;
        public bool AlwaysShowWaterLevelDefault { get; set; } = false;
        public bool SizeWaterLevelWithBoundsDefault { get; set; } = false;
        public bool ShowFullParallaxSpritesDefault { get; set; } = false;
        public bool ShowEntitySelectionBoxesDefault { get; set; } = true;
        public bool ShowDebugStatsDefault { get; set; } = false;
        public bool ScrollLockDirectionDefault { get; set; } = false;
        public String MenuButtonLayoutDefault { get; set; } = "Xbox";
        public String MenuLanguageDefault { get; set; } = "EN";
        public Color WaterEntityColorDefault { get; set; } = Color.Blue;
        public bool ShowPinballEntities { get; set; } = true;
        public bool ShowFilterlessEntities { get; set; } = true;
        public bool ShowManiaEntities { get; set; } = true;
        public bool ShowEncoreEntities { get; set; } = true;
        public bool ShowBothEntities { get; set; } = true;
        public bool ShowOtherEntities { get; set; } = true;
        public String DefaultFilter { get; set; } = "E";
        public Color CollisionSAColour { get; set; } = Color.White;
        public Color CollisionTOColour { get; set; } = Color.Yellow;
        public Color CollisionLRDColour { get; set; } = Color.Red;
        public Int32 CustomGridSizeValue { get; set; } = 16;
        public Int32 FasterNudgeValue { get; set; } = 5;
        public String ModLoaderPath { get; set; }
        public String SonicManiaPath { get; set; }
        public String AnimationEditorPath { get; set; }
        public String CustomFGHigher { get; set; }
        public String CustomFGLower { get; set; }
        public String CheatEnginePath { get; set; }
        public String ImageEditorPath { get; set; }
        public String ImageEditorArguments { get; set; }
        public Int32 TileManiacListSetting { get; set; } = 0;
        public Int32 TileManiacRenderViewerSetting { get; set; } = 0;
        public bool TileManiacShowGrid { get; set; } = false;
        public bool TileManiacMirrorMode { get; set; } = false;
        public bool TileManiacAllowDirect16x16TilesGIFEditing { get; set; } = false;
        public bool TileManiacPromptForChoiceOnImageWrite { get; set; } = false;
        public bool TileManiacEnableWindowsClipboard { get; set; } = false;

        #region Accessors
        public static void Init()
        {
            Reload();
        }
        private static DefaultPrefrences DefaultInstance;
        public static DefaultPrefrences Default
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
                string json = JsonConvert.SerializeObject(DefaultInstance, Formatting.Indented);
                File.WriteAllText(Methods.ProgramPaths.DefaultPrefrencesFilePath, json);
            }
            catch (Exception ex)
            {
                Methods.ProgramBase.Log.ErrorFormat("Failed to Write GameplaySettings to File! Reason: {0}", ex.Message);
            }
        }
        public static void Reload()
        {
            try
            {
                if (!File.Exists(Methods.ProgramPaths.DefaultPrefrencesFilePath)) File.Create(Methods.ProgramPaths.DefaultPrefrencesFilePath).Close();
                string json = File.ReadAllText(Methods.ProgramPaths.DefaultPrefrencesFilePath);
                try
                {
                    JsonSerializerSettings settings = new JsonSerializerSettings();
                    settings.ObjectCreationHandling = ObjectCreationHandling.Replace;
                    DefaultPrefrences result = JsonConvert.DeserializeObject<DefaultPrefrences>(json, settings);
                    if (result != null) DefaultInstance = result;
                    else DefaultInstance = new DefaultPrefrences();
                }
                catch
                {
                    DefaultInstance = new DefaultPrefrences();
                }
            }
            catch (Exception ex)
            {
                Methods.ProgramBase.Log.ErrorFormat("Failed to Load DefaultPrefrences! Reason: {0}", ex.Message);
                Methods.ProgramBase.Log.InfoFormat("Creating a new DefaultPrefrences in Memory...");
                DefaultInstance = new DefaultPrefrences();
            }

        }
        public static void Reset()
        {
            DefaultInstance = new DefaultPrefrences();
            Save();
            Reload();
        }
        public static void Upgrade()
        {

        }
        #endregion
    }
}
