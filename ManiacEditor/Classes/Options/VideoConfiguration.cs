using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManiacEditor.Methods.Solution;
using System.IO;
using Newtonsoft.Json;

namespace ManiacEditor.Classes.Options
{
    public class VideoConfiguration
    {
        public System.Boolean ReduceZoom { get; set; } = false;
        public System.Boolean DisableEntityRendering { get; set; } = false;
        public System.Boolean DisableEntitySelectionBoxText { get; set; } = true;
        public System.Boolean UseSimplifedWaterRendering { get; set; } = false;
        public System.Boolean DisableRendererExclusions { get; set; } = true;
        public System.Boolean UseAlternativeRenderingMode { get; set; } = true;
        public System.Boolean NeverLoadEntityTextures { get; set; } = false;

        public System.Boolean ShowEditLayerBackground { get; set; } = false;
        public System.Boolean HideNormalBackground { get; set; } = false;


        #region Accessors
        public static void Init()
        {
            Reload();
        }
        private static VideoConfiguration DefaultInstance;
        public static VideoConfiguration Default
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
                File.WriteAllText(Methods.ProgramPaths.VideoConfigurationFilePath, json);
            }
            catch (Exception ex)
            {
                Methods.ProgramBase.Log.ErrorFormat("Failed to Write VideoConfiguration to File! Reason: {0}", ex.Message);
            }
        }
        public static void Reload()
        {
            try
            {
                if (!File.Exists(Methods.ProgramPaths.VideoConfigurationFilePath)) File.Create(Methods.ProgramPaths.VideoConfigurationFilePath).Close();
                string json = File.ReadAllText(Methods.ProgramPaths.VideoConfigurationFilePath);
                try
                {
                    JsonSerializerSettings settings = new JsonSerializerSettings();
                    settings.ObjectCreationHandling = ObjectCreationHandling.Replace;
                    VideoConfiguration result = JsonConvert.DeserializeObject<VideoConfiguration>(json, settings);
                    if (result != null) DefaultInstance = result;
                    else DefaultInstance = new VideoConfiguration();
                }
                catch
                {
                    DefaultInstance = new VideoConfiguration();
                }
            }
            catch (Exception ex)
            {
                Methods.ProgramBase.Log.ErrorFormat("Failed to Load VideoConfiguration! Reason: {0}", ex.Message);
                Methods.ProgramBase.Log.InfoFormat("Creating a new VideoConfiguration in Memory...");
                DefaultInstance = new VideoConfiguration();
            }

        }
        public static void Reset()
        {
            DefaultInstance = new VideoConfiguration();
            Save();
            Reload();
        }
        public static void Upgrade()
        {

        }
        #endregion
    }
}
