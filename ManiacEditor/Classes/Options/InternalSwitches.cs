using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManiacEditor.Methods.Editor;
using System.IO;
using Newtonsoft.Json;

namespace ManiacEditor.Classes.Options
{
    public class InternalSwitches
    {
        public static bool TestPortableModeEligibilty()
        {
            if (GenerationsLib.Core.FileHelpers.CanWrite(Methods.ProgramPaths.SettingsPortableDirectory))
            {
                return true;
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Can't Activate Portable Mode! The place where Maniac Editor is installed is unabled to be written to! Install Maniac Editor somewhere besides the Program Files folder or somewhere with writing permisions for this user to use this functionality!");
                return false;
            }
        }
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
                File.WriteAllText(Methods.ProgramPaths.InternalSwitchesFilePath, json);
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
                if (!File.Exists(Methods.ProgramPaths.InternalSwitchesFilePath)) File.Create(Methods.ProgramPaths.InternalSwitchesFilePath).Close();
                string json = File.ReadAllText(Methods.ProgramPaths.InternalSwitchesFilePath);
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
