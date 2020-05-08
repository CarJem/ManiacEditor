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
    public class DevelopmentStates
    {
        public System.Decimal DevInt1 { get; set; }
        public System.Decimal DevInt2 { get; set; }
        public System.Decimal DevInt3 { get; set; }
        public System.Decimal DevInt4 { get; set; }
        public System.Decimal DevInt5 { get; set; }
        public System.Decimal DevInt6 { get; set; }
        public System.Decimal DevInt7 { get; set; }
        public System.Decimal DevInt8 { get; set; }
        public System.Decimal DevInt9 { get; set; }
        public System.Decimal DevInt10 { get; set; }
        public System.Decimal DevInt11 { get; set; }
        public System.Decimal DevInt12 { get; set; }
        public System.Decimal DevInt13 { get; set; }
        public System.Decimal DevInt14 { get; set; }
        public System.Decimal DevInt15 { get; set; }
        public System.Decimal DevInt16 { get; set; }
        public System.Decimal DevInt17 { get; set; }
        public System.Decimal DevInt18 { get; set; }
        public System.Decimal DevInt19 { get; set; }
        public System.Decimal DevInt20 { get; set; }
        public System.Decimal DevInt21 { get; set; }
        public System.Decimal DevInt22 { get; set; }
        public System.Decimal DevInt23 { get; set; }
        public System.Decimal DevInt24 { get; set; }
        public System.Decimal DevInt25 { get; set; }
        public System.Decimal DevInt26 { get; set; }
        public System.Decimal DevInt27 { get; set; }
        public System.String DevForceRestartData { get; set; }
        public System.Boolean DevForceRestartIsEncore { get; set; } = false;
        public System.String DevForceRestartScene { get; set; }
        public System.Int32 DevForceRestartX { get; set; }
        public System.Int32 DevForceRestartY { get; set; }
        public System.Int32 DevForceRestartID { get; set; }
        public System.Int32 DevForceRestartZoomLevel { get; set; }
        public System.Boolean DevAutoStart { get; set; } = false;
        public System.Boolean UseAutoForcefulStartup { get; set; } = false;
        public System.Boolean DevForceRestartIsBrowsed { get; set; } = false;
        public System.String DevForceRestartSceneID { get; set; }
        public System.String DevForceRestartCurrentName { get; set; }
        public System.String DevForceRestartCurrentZone { get; set; }
        public System.Boolean ExperimentalPropertyGridView { get; set; } = false;
        public System.Collections.Specialized.StringCollection DevForceRestartResourcePacks { get; set; } = new System.Collections.Specialized.StringCollection() { };

        #region Accessors
        public static void Init()
        {
            Reload();
        }
        private static DevelopmentStates DefaultInstance;
        public static DevelopmentStates Default
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
                File.WriteAllText(Methods.ProgramPaths.DevelopmentStatesFilePath, json);
            }
            catch (Exception ex)
            {
                Methods.ProgramBase.Log.ErrorFormat("Failed to Write DevelopmentStates to File! Reason: {0}", ex.Message);
            }
        }
        public static void Reload()
        {
            try
            {
                if (!File.Exists(Methods.ProgramPaths.DevelopmentStatesFilePath)) File.Create(Methods.ProgramPaths.DevelopmentStatesFilePath).Close();
                string json = File.ReadAllText(Methods.ProgramPaths.DevelopmentStatesFilePath);
                try
                {
                    JsonSerializerSettings settings = new JsonSerializerSettings();
                    settings.ObjectCreationHandling = ObjectCreationHandling.Replace;
                    DevelopmentStates result = JsonConvert.DeserializeObject<DevelopmentStates>(json, settings);
                    if (result != null) DefaultInstance = result;
                    else DefaultInstance = new DevelopmentStates();
                }
                catch
                {
                    DefaultInstance = new DevelopmentStates();
                }
            }
            catch (Exception ex)
            {
                Methods.ProgramBase.Log.ErrorFormat("Failed to Load DevelopmentStates! Reason: {0}", ex.Message);
                Methods.ProgramBase.Log.InfoFormat("Creating a new DevelopmentStates in Memory...");
                DefaultInstance = new DevelopmentStates();
            }

        }
        public static void Reset()
        {
            DefaultInstance = new DevelopmentStates();
            Save();
            Reload();
        }
        public static void Upgrade()
        {

        }
        #endregion
    }
}
