using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using IniParser;
using IniParser.Model;
using ManiacEditor.Structures;
using Newtonsoft.Json;

namespace ManiacEditor
{
    public class EditorManiacINI
	{
		private Interfaces.Base.MainEditor Instance;
        public string ManiacINIPath;
        public LoadPrefrences ManiacINIData { get; set; } = new LoadPrefrences();

        public EditorManiacINI(Interfaces.Base.MainEditor instance)
		{
            Instance = instance;
        }

        public void ClearSettings()
        {
            if (ManiacINIData != null) ManiacINIData.Reset();
        }

        public void AddSavedCoordinates(string name, int x, int y, bool tilesMode)
        {
            if (ManiacINIData == null) CreateFile();
            if (ManiacINIData == null) return;

            if (ManiacINIData.Positions == null) ManiacINIData.Positions = new List<Tuple<string, string>>();
            Tuple<string, string> newEntry = new Tuple<string, string>(name, string.Format("{0},{1}", x.ToString(), y.ToString()));
            ManiacINIData.Positions.Add(newEntry);
            SaveFile();
        }

        public void UpdateFilePath()
        {
            ManiacINIPath = Path.Combine(Instance.Paths.SceneFile_Directory, "Maniac.json");
        }

        public string GetFilePath()
        {
            return Path.Combine(Instance.Paths.SceneFile_Directory, "Maniac.json");
        }

        public void CreateFile()
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

        public void InterpretInformation()
        {
            var path = GetFilePath();
            string data = File.ReadAllText(path);
            LoadPrefrences Options = JsonConvert.DeserializeObject<LoadPrefrences>(data);
            ManiacINIData = Options;
        }

        public void LoadFile()
		{
			if (GetFile() == false)
			{
				var ModListFile = File.Create(ManiacINIPath);
				ModListFile.Close();
				if (GetFile() == false) return;
			}
			InterpretInformation();
		}

		public void SaveFile()
		{
            var path = GetFilePath();
            string data = JsonConvert.SerializeObject(ManiacINIData, Formatting.Indented);
            File.WriteAllText(path, data);
        }

		public bool GetFile()
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
