using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using IniParser;
using IniParser.Model;
using ManiacEditor.Classes.Internal;
using Newtonsoft.Json;

namespace ManiacEditor.Methods.Prefrences
{
    public static class SceneCurrentSettings
	{
		private static Controls.Editor.MainEditor Instance;
        public static string ManiacINIPath;
        public static SceneSettings ManiacINIData { get; set; } = new SceneSettings();

        public static void UpdateInstance(Controls.Editor.MainEditor instance)
		{
            Instance = instance;
        }

        public static void ClearSettings()
        {
            if (ManiacINIData != null) ManiacINIData.Reset();
        }

        public static void AddSavedCoordinates(string name, int x, int y, bool tilesMode)
        {
            if (ManiacINIData == null) CreateFile();
            if (ManiacINIData == null) return;

            if (ManiacINIData.Positions == null) ManiacINIData.Positions = new List<Tuple<string, string>>();
            Tuple<string, string> newEntry = new Tuple<string, string>(name, string.Format("{0},{1}", x.ToString(), y.ToString()));
            ManiacINIData.Positions.Add(newEntry);
            SaveFile();
        }

        public static void UpdateFilePath()
        {
            ManiacINIPath = Path.Combine(Instance.Paths.SceneFile_Source.SourceDirectory, "Maniac.json");
        }

        public static string GetFilePath()
        {
            return Path.Combine(Instance.Paths.SceneFile_Source.SourceDirectory, "Maniac.json");
        }

        public static void CreateFile()
        {
            UpdateFilePath();
            if (GetFile() == false)
            {
                var ModListFile = File.Create(ManiacINIPath);
                ModListFile.Close();
                if (GetFile() == false) return;
            }
            InterpretInformation();
        }

        public static void InterpretInformation()
        {
            var path = GetFilePath();
            string data = File.ReadAllText(path);
            SceneSettings Options = JsonConvert.DeserializeObject<SceneSettings>(data);
            ManiacINIData = Options;
        }

        public static void LoadFile()
		{
			if (GetFile() == false)
			{
				var ModListFile = File.Create(ManiacINIPath);
				ModListFile.Close();
				if (GetFile() == false) return;
			}
			InterpretInformation();
		}

		public static void SaveFile()
		{
            var path = GetFilePath();
            string data = JsonConvert.SerializeObject(ManiacINIData, Formatting.Indented);
            File.WriteAllText(path, data);
        }

		public static bool GetFile()
		{
            if (!File.Exists(GetFilePath()))
            {
                return false;
            }
			else
			{
                InterpretInformation();
            }
			return true;
		}
    }
}
