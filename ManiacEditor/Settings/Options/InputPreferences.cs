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
