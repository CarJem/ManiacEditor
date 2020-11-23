using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using IniParser;
using IniParser.Model;
using ManiacEditor.Structures;
using Newtonsoft.Json;
using Xe.Tools.Wpf;

namespace ManiacEditor.Classes.Prefrences
{

public static class SceneCurrentSettings
	{
        public class SceneSettings : BaseNotifyPropertyChanged
        {
            private string _ForegroundLower;
            private string _ForegroundHigher;
            private System.Drawing.Color _WaterColor;
            private List<string> _SpritePaths;
            private string _RedirectSpriteDataFolder;
            private string _EncoreACTFile;
            private List<string> _ObjectHashes;
            private List<string> _AttributeHashes;
            private Dictionary<string, string> _EntityRenderSwaps;
            private List<Tuple<string, string>> _Positions;

            public string RedirectSpriteDataFolder
            {
                get
                {
                    return _RedirectSpriteDataFolder;
                }
                set
                {
                    _RedirectSpriteDataFolder = value;
                    OnPropertyChanged(nameof(RedirectSpriteDataFolder));
                }
            }
            public string ForegroundLower
            {
                get
                {
                    return _ForegroundLower;
                }
                set
                {
                    _ForegroundLower = value;
                    OnPropertyChanged(nameof(ForegroundLower));
                }
            }
            public string ForegroundHigher
            {
                get
                {
                    return _ForegroundHigher;
                }
                set
                {
                    _ForegroundHigher = value;
                    OnPropertyChanged(nameof(ForegroundHigher));
                }
            }
            public System.Drawing.Color WaterColor
            {
                get
                {
                    return _WaterColor;
                }
                set
                {
                    _WaterColor = value;
                    OnPropertyChanged(nameof(WaterColor));
                }
            }
            public List<string> SpritePaths
            {
                get
                {
                    return _SpritePaths;
                }
                set
                {
                    _SpritePaths = value;
                    OnPropertyChanged(nameof(SpritePaths));
                }
            }
            public string EncoreACTFile
            {
                get
                {
                    return _EncoreACTFile;
                }
                set
                {
                    _EncoreACTFile = value;
                    OnPropertyChanged(nameof(EncoreACTFile));
                }
            }
            public Dictionary<string, string> EntityRenderSwaps
            {
                get
                {
                    return _EntityRenderSwaps;
                }
                set
                {
                    _EntityRenderSwaps = value;
                    OnPropertyChanged(nameof(EntityRenderSwaps));
                }
            }
            public List<Tuple<string, string>> Positions
            {
                get
                {
                    return _Positions;
                }
                set
                {
                    _Positions = value;
                    OnPropertyChanged(nameof(Positions));
                }
            }
            public List<string> ObjectHashes
            {
                get
                {
                    return _ObjectHashes;
                }
                set
                {
                    _ObjectHashes = value;
                    OnPropertyChanged(nameof(ObjectHashes));
                }
            }

            public List<string> AttributeHashes
            {
                get
                {
                    return _AttributeHashes;
                }
                set
                {
                    _AttributeHashes = value;
                    OnPropertyChanged(nameof(AttributeHashes));
                }
            }

            public void Reset()
            {

            }
        }

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

        public static void AddCustomObjectHashNames(string name)
        {
            if (ManiacINIData == null) CreateFile();
            if (ManiacINIData == null) return;

            if (ManiacINIData.ObjectHashes == null) ManiacINIData.ObjectHashes = new List<string>();

            if (Methods.ProgramBase.EntityDefinitions.Objects.Contains(name)) return;

            if (!ManiacINIData.ObjectHashes.Contains(name))
            {
                ManiacINIData.ObjectHashes.Add(name);
                SaveFile();
            }
        }

        public static void AddCustomAttributeHashNames(string name)
        {
            if (ManiacINIData == null) CreateFile();
            if (ManiacINIData == null) return;

            if (ManiacINIData.AttributeHashes == null) ManiacINIData.AttributeHashes = new List<string>();

            if (Methods.ProgramBase.EntityDefinitions.Attributes.Contains(name)) return;

            if (!ManiacINIData.AttributeHashes.Contains(name))
            {
                ManiacINIData.AttributeHashes.Add(name);
                SaveFile();
            }
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
            ManiacINIPath = Path.Combine(ManiacEditor.Methods.Solution.SolutionPaths.SceneFile_Source.SourceDirectory, "Maniac.json");
        }

        public static string GetFilePath()
        {
            return Path.Combine(ManiacEditor.Methods.Solution.SolutionPaths.SceneFile_Source.SourceDirectory, "Maniac.json");
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
