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
    public class InternalSwitches
    {
        public bool PortableMode { get; set; } = false;

        #region Accessors
        public static void Init()
        {
            Reload();
        }
        private static InternalSwitches DefaultInstance;
        public static InternalSwitches Default
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
                File.WriteAllText(Constants.InternalSwitchesFilePath, json);
            }
            catch (Exception ex)
            {
                Methods.ProgramBase.Log.ErrorFormat("Failed to Write InternalSwitches to File! Reason: {0}", ex.Message);
            }
        }
        public static void Reload()
        {
            try
            {
                if (!File.Exists(Constants.InternalSwitchesFilePath)) File.Create(Constants.InternalSwitchesFilePath).Close();
                string json = File.ReadAllText(Constants.InternalSwitchesFilePath);
                try
                {
                    JsonSerializerSettings settings = new JsonSerializerSettings();
                    settings.ObjectCreationHandling = ObjectCreationHandling.Replace;
                    InternalSwitches result = JsonConvert.DeserializeObject<InternalSwitches>(json, settings);
                    if (result != null) DefaultInstance = result;
                    else DefaultInstance = new InternalSwitches();
                }
                catch
                {
                    DefaultInstance = new InternalSwitches();
                }

            }
            catch (Exception ex)
            {
                Methods.ProgramBase.Log.ErrorFormat("Failed to Load InternalSwitches! Reason: {0}", ex.Message);
                Methods.ProgramBase.Log.InfoFormat("Creating a new InternalSwitches in Memory...");
                DefaultInstance = new InternalSwitches();
            }

        }
        public static void Reset()
        {
            DefaultInstance = new InternalSwitches();
            Save();
            Reload();
        }
        public static void Upgrade()
        {

        }
        #endregion
    }
}
