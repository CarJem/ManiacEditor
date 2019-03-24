using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using IniParser;
using IniParser.Model;

namespace ManiacEditor
{
    public class EditorNewSettings
    {
        public Editor Instance;
        IniData SettingsInfo;
        public List<Tuple<string, List<Tuple<string, string>>>> SettingsInformation;
        public List<Tuple<string, List<Tuple<string, string, SettingVarriableTypes, string, string>>>> PrasedSettings;
        string MyDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        string ManiacDefaultsPath;

        public static string GetSuggestedName(string name)
        {
            foreach (var pair in SuggestedNames)
            {
                if (pair.Item1 == name) return pair.Item2;
            }
            return name;
        }

        public static string GetSuggestedValueName(string value)
        {
            return value;
        }

        public EditorNewSettings(Editor instance)
        {
            Instance = instance;
            ManiacDefaultsPath = Path.Combine(MyDocuments, "ManiacEditor Prefrences", "Defaults.ini");
            LoadFile();
        }

        public void LoadFile()
        {
            if (GetFile() == false)
            {
                if (!Directory.Exists(Path.Combine(MyDocuments, "ManiacEditor Prefrences"))) Directory.CreateDirectory(Path.Combine(MyDocuments, "ManiacEditor Prefrences"));
                var ModListFile = File.Create(ManiacDefaultsPath);
                ModListFile.Close();
                if (GetFile() == false) return;
            }
            InterpretInformation();
        }

        public void InterpretInformation()
        {
            SettingsInformation = new List<Tuple<string, List<Tuple<string, string>>>>();
            foreach (var section in SettingsInfo.Sections)
            {
                List<Tuple<string, string>> Keys = new List<Tuple<string, string>>();
                foreach (var key in section.Keys)
                {
                    Keys.Add(new Tuple<string, string>(key.KeyName, key.Value));
                }
                SettingsInformation.Add(new Tuple<string, List<Tuple<string, string>>>(section.SectionName, Keys));
            }
            UpdateInformation();
        }

        public enum SettingVarriableTypes : int
        {
            Boolean = 1,
            ToolbarSlider = 2,
            String = 3,
            Interger = 4,
            MenuButtons = 5,
            MenuLanguage = 6,
            Color = 7,
            GridSize = 8,
            CollisionColor = 9,
            SceneSelectView = 10
        }

        List<string> MenuButtonTags = new List<string>()
        {
            "Xbox",
            "PS4",
            "Switch",
            "Switch Joy R",
            "Switch Joy L",
            "Saturn Black",
            "Saturn White",
            "PC EN/JP",
            "PC FR",
            "PC IT",
            "PC GE",
            "PC SP",
        };

        List<string> MenuLanguageTags = new List<string>()
        {
            "EN",
            "FR",
            "GE",
            "IT",
            "JP",
            "KO",
            "SC",
            "SP",
            "TC",
        };

        public int GetTagIndex(string tag)
        {
            foreach (string item in MenuLanguageTags)
            {
                if (item == tag) return MenuLanguageTags.IndexOf(item);
            }
            foreach (string item in MenuButtonTags)
            {
                if (item == tag) return MenuLanguageTags.IndexOf(item);
            }
            return -1;
        }

        public bool IsValidValue(string value, SettingVarriableTypes type)
        {
            if (type == SettingVarriableTypes.Boolean)
            {
                if (value != "F" && value != "T") return false;
                else return true;
            }
            else if (type == SettingVarriableTypes.ToolbarSlider)
            {
                if (value != "0" && value != "1" && value != "2" && value != "3") return false;
                else return true;
            }
            else if (type == SettingVarriableTypes.Interger)
            {
                if (!Int32.TryParse(value, out int result) && result >= 0) return false;
                else return true;
            }
            else if (type == SettingVarriableTypes.MenuButtons)
            {
                if (!MenuButtonTags.Exists(x => x == value)) return false;
                else return true;
            }
            else if (type == SettingVarriableTypes.MenuLanguage)
            {
                if (!MenuLanguageTags.Exists(x => x == value)) return false;
                else return true;
            }
            else if (type == SettingVarriableTypes.Color)
            {
                if (System.Windows.Media.ColorConverter.ConvertFromString(value).Equals(null)) return false;
                return true;
            }
            else if (type == SettingVarriableTypes.GridSize)
            {
                if (value != "16" && value != "128" && value != "256" && value != "Custom") return false;
                return true;
            }
            else if (type == SettingVarriableTypes.String)
            {
                return true;
            }
            else return false;
        }

        public static List<Tuple<string,string>> SuggestedNames = new List<Tuple<string, string>>()
        {
            new Tuple<string, string>("TilesToolbar", "Tiles Toolbar Defaults"),
            new Tuple<string, string>("CustomFGLayers", "Custom FG Layer Names"),
            new Tuple<string, string>("EntitiesToolbar", "Entities Toolbar Defaults"),
            new Tuple<string, string>("Layers", "Layer Toolbar Defaults"),
            new Tuple<string, string>("Numerical", "Numerical Defaults"),
            new Tuple<string, string>("MenuButtonsAndLanguage", "Menu Buttons / Language Defaults"),
            new Tuple<string, string>("CustomColors", "Custom Color Defaults"),
            new Tuple<string, string>("OtherDefaults", "Various Defaults"),
        };

        List<Tuple<string, string, SettingVarriableTypes, string, string>> TilesToolbarDefaults = new List<Tuple<string, string, SettingVarriableTypes, string, string>>
        {
            new Tuple<string, string, SettingVarriableTypes, string, string>("SolidTop", "F", SettingVarriableTypes.Boolean, "Solid Top", ""),
            new Tuple<string, string, SettingVarriableTypes, string, string>("SolidAllButTop", "F", SettingVarriableTypes.Boolean, "Solid All But Top", ""),
            new Tuple<string, string, SettingVarriableTypes, string, string>("SolidTop2", "F", SettingVarriableTypes.Boolean, "Solid Top (Plane 2)", ""),
            new Tuple<string, string, SettingVarriableTypes, string, string>("SolidAllButTop2", "F", SettingVarriableTypes.Boolean, "Solid All But Top (Plane 2)", ""),
            new Tuple<string, string, SettingVarriableTypes, string, string>("TilesDefaultZoomLevel", "1", SettingVarriableTypes.ToolbarSlider, "Toolbar Default Zoom Level (Tiles)", ""),
            new Tuple<string, string, SettingVarriableTypes, string, string>("ChunksDefaultZoomLevel", "1", SettingVarriableTypes.ToolbarSlider, "Toolbar Default Zoom Level (Chunks)", "")
        };

        List<Tuple<string, string, SettingVarriableTypes, string, string>> CustomFGLayersDefault = new List<Tuple<string, string, SettingVarriableTypes, string, string>>
        {
            new Tuple<string, string, SettingVarriableTypes, string, string>("LowerLayer", "", SettingVarriableTypes.String, "Lower Layer", ""),
            new Tuple<string, string, SettingVarriableTypes, string, string>("HigherLayer", "", SettingVarriableTypes.String, "Higher Layer", "")

        };

        List<Tuple<string, string, SettingVarriableTypes, string, string>> EntitiesToolbarDefault = new List<Tuple<string, string, SettingVarriableTypes, string, string>>
        {
            new Tuple<string, string, SettingVarriableTypes, string, string>("ManiaFilter", "T", SettingVarriableTypes.Boolean, "Mania Filter", ""),
            new Tuple<string, string, SettingVarriableTypes, string, string>("EncoreFilter", "T", SettingVarriableTypes.Boolean, "Encore Filter", ""),
            new Tuple<string, string, SettingVarriableTypes, string, string>("BothFilter", "T", SettingVarriableTypes.Boolean, "Both Filter", ""),
            new Tuple<string, string, SettingVarriableTypes, string, string>("OtherFilter", "T", SettingVarriableTypes.Boolean, "Other Filter", ""),
            new Tuple<string, string, SettingVarriableTypes, string, string>("PinballFilter", "T", SettingVarriableTypes.Boolean, "Pinball Filter", ""),
            new Tuple<string, string, SettingVarriableTypes, string, string>("FilterlessFilter", "T", SettingVarriableTypes.Boolean, "Filterless Filter", ""),
            new Tuple<string, string, SettingVarriableTypes, string, string>("UseBitOperators", "F", SettingVarriableTypes.Boolean, "Use Bit Operators", "")

        };

        List<Tuple<string, string, SettingVarriableTypes, string, string>> LayersDefault = new List<Tuple<string, string, SettingVarriableTypes, string, string>>
        {
            new Tuple<string, string, SettingVarriableTypes, string, string>("FGLower", "T", SettingVarriableTypes.Boolean, "FG Lower", ""),
            new Tuple<string, string, SettingVarriableTypes, string, string>("FGLow", "T", SettingVarriableTypes.Boolean, "FG Low", ""),
            new Tuple<string, string, SettingVarriableTypes, string, string>("FGHigh", "T", SettingVarriableTypes.Boolean, "FG High", ""),
            new Tuple<string, string, SettingVarriableTypes, string, string>("FGHigher", "T", SettingVarriableTypes.Boolean, "FG Higher", ""),
            new Tuple<string, string, SettingVarriableTypes, string, string>("Animations", "T", SettingVarriableTypes.Boolean, "Animations", ""),
            new Tuple<string, string, SettingVarriableTypes, string, string>("AnimationsSprites", "T", SettingVarriableTypes.Boolean, "Sprite Animations", ""),
            new Tuple<string, string, SettingVarriableTypes, string, string>("AnimationsPlatforms", "F", SettingVarriableTypes.Boolean, "Platform Animations", "")

        };

        List<Tuple<string, string, SettingVarriableTypes, string, string>> NumericalDefaults = new List<Tuple<string, string, SettingVarriableTypes, string, string>>
        {
            new Tuple<string, string, SettingVarriableTypes, string, string>("GridSize", "16", SettingVarriableTypes.GridSize, "Default Grid Size", ""),
            new Tuple<string, string, SettingVarriableTypes, string, string>("CustomGridSize", "16", SettingVarriableTypes.Interger, "Custom Grid Size", ""),
            new Tuple<string, string, SettingVarriableTypes, string, string>("FasterNudgeAmount", "5", SettingVarriableTypes.Interger, "Faster Nudge Increment Amount", ""),

        };

        List<Tuple<string, string, SettingVarriableTypes, string, string>> ButtonLangDefaults = new List<Tuple<string, string, SettingVarriableTypes, string, string>>
        {
            new Tuple<string, string, SettingVarriableTypes, string, string>("ButtonLayout", "Xbox", SettingVarriableTypes.MenuButtons, "Default Button Type", ""),
            new Tuple<string, string, SettingVarriableTypes, string, string>("MenuLanguage", "EN", SettingVarriableTypes.MenuLanguage, "Default Menu Language", ""),
        };

        List<Tuple<string, string, SettingVarriableTypes, string, string>> CustomColorDefaults = new List<Tuple<string, string, SettingVarriableTypes, string, string>>
        {
            new Tuple<string, string, SettingVarriableTypes, string, string>("CollisionSolidAll", "White", SettingVarriableTypes.Color, "Collision Solid (All) Color", ""),
            new Tuple<string, string, SettingVarriableTypes, string, string>("CollisionTopOnlySolid", "Yellow", SettingVarriableTypes.Color, "Collision Solid (Top Only) Color", ""),
            new Tuple<string, string, SettingVarriableTypes, string, string>("CollisionSolidLRD", "Red", SettingVarriableTypes.Color, "Collision Solid (LRD) Color", ""),
            new Tuple<string, string, SettingVarriableTypes, string, string>("WaterLevelColor", "Blue", SettingVarriableTypes.Color, "Water Level Color", ""),
            new Tuple<string, string, SettingVarriableTypes, string, string>("GridColor", "Black", SettingVarriableTypes.Color, "Grid Color", ""),
        };

        List<Tuple<string, string, SettingVarriableTypes, string, string>> OtherDefaults = new List<Tuple<string, string, SettingVarriableTypes, string, string>>
        {
            new Tuple<string, string, SettingVarriableTypes, string, string>("FullParallaxSprites", "T", SettingVarriableTypes.Boolean, "Show Full Parallax Sprites", ""),
            new Tuple<string, string, SettingVarriableTypes, string, string>("WarpDoorPaths", "T", SettingVarriableTypes.Boolean, "Show Entity Path Arrows", ""),
            new Tuple<string, string, SettingVarriableTypes, string, string>("EditEntitiesTransparentLayers", "F", SettingVarriableTypes.Boolean, "Edit Entities with Semi-Transparent Layers", ""),
            new Tuple<string, string, SettingVarriableTypes, string, string>("PixelMode", "F", SettingVarriableTypes.Boolean, "Use Pixel Mode", ""),
            new Tuple<string, string, SettingVarriableTypes, string, string>("CollisionColor", "1", SettingVarriableTypes.CollisionColor, "Default Collision Color", ""),
            new Tuple<string, string, SettingVarriableTypes, string, string>("ScrollLock", "T", SettingVarriableTypes.Boolean, "Enable Scroll Lock", ""),
            new Tuple<string, string, SettingVarriableTypes, string, string>("ShowEntitySelectionBoxes", "T", SettingVarriableTypes.Boolean, "Show Entity Selection Boxes", ""),
            new Tuple<string, string, SettingVarriableTypes, string, string>("ShowWaterLevel", "F", SettingVarriableTypes.Boolean, "Show Water Level", ""),
            new Tuple<string, string, SettingVarriableTypes, string, string>("AlwaysShowWaterLevel", "F", SettingVarriableTypes.Boolean, "Always Show Water Level", ""),
            new Tuple<string, string, SettingVarriableTypes, string, string>("WaterLevelSizeWithBoundsWhenUnSelected", "F", SettingVarriableTypes.Boolean, "Size Water Level with Bounds when not selected", ""),
            new Tuple<string, string, SettingVarriableTypes, string, string>("ShowStatsViewer", "F", SettingVarriableTypes.Boolean, "Show Stats Viewer", ""),
            new Tuple<string, string, SettingVarriableTypes, string, string>("UseLargeStatsViewerText", "F", SettingVarriableTypes.Boolean, "Use Large Debug HUD Text", ""),
        };

        public void UpdateInformation()
        {
            if (PrasedSettings == null) PrasedSettings = new List<Tuple<string, List<Tuple<string, string, SettingVarriableTypes, string, string>>>>();
            UpdateCategory("TilesToolbar", TilesToolbarDefaults);
            UpdateCategory("CustomFGLayers", CustomFGLayersDefault);
            UpdateCategory("EntitiesToolbar", EntitiesToolbarDefault);
            UpdateCategory("Layers", LayersDefault);
            UpdateCategory("Numerical", NumericalDefaults);
            UpdateCategory("MenuButtonsAndLanguage", ButtonLangDefaults);
            UpdateCategory("CustomColors", CustomColorDefaults);
            UpdateCategory("OtherDefaults", OtherDefaults);

            SaveFile();
        }

        public void UpdateCategory(string category, List<Tuple<string,string, SettingVarriableTypes, string, string>> subItems)
        {
            if (!SettingsInformation.Any(m => m.Item1 == category)) SettingsInformation.Add(new Tuple<string, List<Tuple<string, string>>>(category, new List<Tuple<string, string>>()));
            int index = SettingsInformation.FindIndex(x => x.Item1 == category);
            UpdateCategoryValues(index, subItems);
        }

        public void UpdateCategoryValues(int index, List<Tuple<string, string, SettingVarriableTypes, string, string>> subItems)
        {
            List<Tuple<string, string, SettingVarriableTypes, string, string>> CategoryValues = new List<Tuple<string, string, SettingVarriableTypes, string, string>>();
            foreach (var keypair in subItems)
            {
                if (!SettingsInformation[index].Item2.Any(x => x.Item1 == keypair.Item1)) SettingsInformation[index].Item2.Add(new Tuple<string, string>(keypair.Item1, keypair.Item2));
                int valueIndex = SettingsInformation[index].Item2.FindIndex(x => x.Item1 == keypair.Item1);
                if (!IsValidValue(SettingsInformation[index].Item2[valueIndex].Item2, keypair.Item3)) SettingsInformation[index].Item2[valueIndex] = new Tuple<string, string>(keypair.Item1, keypair.Item2);
                CategoryValues.Add(new Tuple<string, string, SettingVarriableTypes, string, string>(keypair.Item1, keypair.Item2, keypair.Item3, keypair.Item4, keypair.Item5));
            }
            PrasedSettings.Add(new Tuple<string, List<Tuple<string, string, SettingVarriableTypes, string, string>>>(SettingsInformation[index].Item1, CategoryValues));
        }


        public void PrintInformation()
		{
			var n = Environment.NewLine;
			string fullInfo = "";
			foreach(var pair in SettingsInformation)
			{
				fullInfo += String.Format("[{0}]", pair.Item1) + n;
				foreach (var key in pair.Item2)
				{
					fullInfo += String.Format("   {0}={1}", key.Item1, key.Item2) + n;
				}
			}
			System.Windows.MessageBox.Show(fullInfo);
		}

		public void SaveFile()
		{
			IniData SaveData = new IniData();
			foreach (var pair in SettingsInformation)
			{
				SectionData section = new SectionData(pair.Item1);
				foreach (var key in pair.Item2)
				{
					section.Keys.AddKey(key.Item1, key.Item2);
				}
				SaveData.Sections.Add(section);
			}
			var parser = new FileIniDataParser();
			parser.WriteFile(ManiacDefaultsPath, SaveData);
		}

		public static FileStream GetModPackList(string path)
		{
			if (!File.Exists(path)) return null;
			return new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
		}

		public bool GetFile()
		{
			var parser = new FileIniDataParser();
            if (!File.Exists(ManiacDefaultsPath))
            {
                return false;
            }
			else
			{
                IniData file = parser.ReadFile(ManiacDefaultsPath);
                SettingsInfo = file;
			}
			return true;
		}
	}
}
