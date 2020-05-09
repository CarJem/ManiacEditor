using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManiacEditor.Methods.Solution;
using System.IO;
using Newtonsoft.Json;
using GenerationsLib.WPF.Themes;

namespace ManiacEditor.Classes.Options
{
    public class GeneralSettings
    {
        public bool IsMaximized { get; set; } = false;
        public bool UpgradeRequired { get; set; } = true;
        public bool KeepLayersVisible { get; set; } = false;
        public bool RemoveObjectImportLock { get; set; } = false;
        public bool DisableSaveWarnings { get; set; } = false;
        public bool ShowDiscordRPC { get; set; } = true;
        public bool NeverShowThisAgain { get; set; } = false;
        public bool UseBitOperators { get; set; } = false;
        public bool ShowFirstTimeSetup { get; set; } = true;
        public Skin UserTheme { get; set; } = Skin.Light;
        public string LastModConfig { get; set; }
        public bool ScrollerPressReleaseMode { get; set; } = false;

        #region Accessors
        public static void Init()
        {
            Reload();
        }
        private static GeneralSettings DefaultInstance;
        public static GeneralSettings Default
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
                File.WriteAllText(Methods.ProgramPaths.GeneralSettingsFilePath, json);
            }
            catch (Exception ex)
            {
                Methods.ProgramBase.Log.ErrorFormat("Failed to Write GeneralSettings to File! Reason: {0}", ex.Message);
            }
        }
        public static void Reload()
        {
            try
            {
                if (!File.Exists(Methods.ProgramPaths.GeneralSettingsFilePath)) File.Create(Methods.ProgramPaths.GeneralSettingsFilePath).Close();
                string json = File.ReadAllText(Methods.ProgramPaths.GeneralSettingsFilePath);
                try
                {
                    JsonSerializerSettings settings = new JsonSerializerSettings();
                    settings.ObjectCreationHandling = ObjectCreationHandling.Replace;
                    GeneralSettings result = JsonConvert.DeserializeObject<GeneralSettings>(json, settings);
                    if (result != null) DefaultInstance = result;
                    else DefaultInstance = new GeneralSettings();
                }
                catch
                {
                    DefaultInstance = new GeneralSettings();
                }
            }
            catch (Exception ex)
            {
                Methods.ProgramBase.Log.ErrorFormat("Failed to Load GeneralSettings! Reason: {0}", ex.Message);
                Methods.ProgramBase.Log.InfoFormat("Creating a new GeneralSettings in Memory...");
                DefaultInstance = new GeneralSettings();
            }

        }
        public static void Reset()
        {
            DefaultInstance = new GeneralSettings();
            Save();
            Reload();
        }
        public static void Upgrade()
        {

        }
        #endregion
    }
}
