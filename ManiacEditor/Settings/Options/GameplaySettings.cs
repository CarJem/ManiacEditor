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
    public class GameplaySettings
    {
        public System.Boolean RunGameOnly { get; set; } = false;
        public System.Boolean GameQuitOnMenu { get; set; } = false;
        public System.Boolean GameAutoInput { get; set; } = false;
        public System.Boolean RealTimeObjectMovementMode { get; set; } = true;
        public System.Boolean EnableDevMode { get; set; } = true;
        public System.Boolean DisableBackgroundPausing { get; set; } = true;
        public System.Boolean EnableDebugMode { get; set; } = true;

        #region Accessors
        public static void Init()
        {
            Reload();
        }
        private static GameplaySettings DefaultInstance;
        public static GameplaySettings Default
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
                File.WriteAllText(Methods.ProgramPaths.GameplaySettingsFilePath, json);
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
                if (!File.Exists(Methods.ProgramPaths.GameplaySettingsFilePath)) File.Create(Methods.ProgramPaths.GameplaySettingsFilePath).Close();
                string json = File.ReadAllText(Methods.ProgramPaths.GameplaySettingsFilePath);
                try
                {
                    JsonSerializerSettings settings = new JsonSerializerSettings();
                    settings.ObjectCreationHandling = ObjectCreationHandling.Replace;
                    GameplaySettings result = JsonConvert.DeserializeObject<GameplaySettings>(json, settings);
                    if (result != null) DefaultInstance = result;
                    else DefaultInstance = new GameplaySettings();
                }
                catch
                {
                    DefaultInstance = new GameplaySettings();
                }
            }
            catch (Exception ex)
            {
                Methods.ProgramBase.Log.ErrorFormat("Failed to Load GameplaySettings! Reason: {0}", ex.Message);
                Methods.ProgramBase.Log.InfoFormat("Creating a new GameplaySettings in Memory...");
                DefaultInstance = new GameplaySettings();
            }

        }
        public static void Reset()
        {
            DefaultInstance = new GameplaySettings();
            Save();
            Reload();
        }
        public static void Upgrade()
        {

        }
        #endregion
    }
}
