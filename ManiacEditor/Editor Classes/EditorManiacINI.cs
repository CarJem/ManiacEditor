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
	public class EditorManiacINI
	{
		private Editor Instance;
		IniData ManiacINIData;
		public List<Tuple<string, List<Tuple<string, string>>>> ManiacINISettings;
        public string ManiacINIPath;
		public EditorManiacINI(Editor instance)
		{
            Instance = instance;
        }

        public void ClearSettings()
        {
            if (ManiacINISettings != null) ManiacINISettings.Clear();
        }

        public Dictionary<String, String> ReturnPrefrences()
        {
            Dictionary<string, string> Preferences = new Dictionary<string, string>();

            if (ManiacINISettings == null || ManiacINISettings.Count == 0) return null;
            if (ManiacINISettings.Where(x => x.Item1 == "Settings") == null) return null;

            Tuple<string, List<Tuple<string,string>>> classicSettingsList = ManiacINISettings.Where(x => x.Item1 == "Settings").FirstOrDefault();
            if (classicSettingsList == null) return null;
            var Settings = classicSettingsList.Item2;

            foreach (var tuple in Settings)
            {
                Preferences.Add(tuple.Item1, tuple.Item2);
            }

            return Preferences;
        }

        public List<Tuple<string, string>> ReturnEntityRenderSwaps()
        {
            Dictionary<string, string> Preferences = new Dictionary<string, string>();

            if (ManiacINISettings == null || ManiacINISettings.Count == 0) return null;
            if (ManiacINISettings.Where(x => x.Item1 == "EntityRenderSwaps") == null) return null;

            Tuple<string, List<Tuple<string, string>>> EntityRenderSwapsList = ManiacINISettings.Where(x => x.Item1 == "EntityRenderSwaps").FirstOrDefault();
            if (EntityRenderSwapsList == null) return null;
            var SwapList = EntityRenderSwapsList.Item2;



            return SwapList;
        }

        public List<Tuple<string,string>> ReturnPositionMarkers()
        {
            List<Tuple<string, string>> Positions;
            if (ManiacINISettings != null)
            {
                var PositionSettingsList = ManiacINISettings.Where(x => x.Item1 == "Positions").FirstOrDefault();

                if (PositionSettingsList != null)
                {
                    Positions = PositionSettingsList.Item2;
                }
                else
                {
                    Positions = new List<Tuple<string, string>>();
                }
            }
            else Positions = new List<Tuple<string, string>>();



            return Positions;
        }

        public void AddSavedCoordinates(string name, int x, int y, bool tilesMode)
        {
            if (ManiacINIData == null) CreateFile();
            if (ManiacINIData == null || ManiacINISettings == null) return;

            if (ManiacINISettings.Where(e => e.Item1 == "Positions").FirstOrDefault() != null)
            {
                AddCoords();
            }
            else
            {
                ManiacINISettings.Add(new Tuple<string, List<Tuple<string, string>>>("Positions", new List<Tuple<string, string>>()));
                AddCoords();
            }

            void AddCoords()
            {
                Tuple<string, string> newEntry = new Tuple<string, string>(name, string.Format("{0},{1}", x.ToString(), y.ToString()));
                ManiacINISettings.Where(e => e.Item1 == "Positions").FirstOrDefault().Item2.Add(newEntry);
            }
        }

        public void SetINIEntityRenderSwaps()
        {
            var RenderSwaps = ReturnEntityRenderSwaps();
            if (RenderSwaps == null) return;

            foreach(var pair in RenderSwaps)
            {
                Instance.userDefinedEntityRenderSwaps.Add(pair.Item1, pair.Item2);
            }

            /* Old Code for Render Swapping
            if (ManiacINIClassicSettings.ContainsKey("SwapEntityRenderNames"))
            {
                ManiacINIClassicSettings.TryGetValue("SwapEntityRenderNames", out value);
                List<string> list = new List<string>(value.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries));
                if (list.Count % 2 == 0 && list.Count != 0)
                {
                    for (int i = 0; i < list.Count;)
                    {
                        string toBeSwapped = list[i];
                        string toSet = list[i + 1];
                        //RSDKrU.MessageBox.Show(toBeSwapped + "-> " + toSet);
                        Instance.userDefinedEntityRenderSwaps.Add(toBeSwapped, toSet);
                        i = i + 2;
                    }
                }
                else
                {
                    RSDKrU.MessageBox.Show("There is an odd number of swaps for entity names, please double check your maniac.ini file");
                }


            }*/
        }

        public void SetINIDefaultPrefrences()
        {
            SetINClassicPrefrences();
            SetINIEntityRenderSwaps();
        }

        public void SetINClassicPrefrences()
        {
            string value;
            Dictionary<String, String> ManiacINIClassicSettings = ReturnPrefrences();
            if (ManiacINIClassicSettings == null) return;
            if (ManiacINIClassicSettings.ContainsKey("LevelID"))
            {
                ManiacINIClassicSettings.TryGetValue("LevelID", out value);
                Int32.TryParse(value, out int resultingInt);
                if (resultingInt >= -1)
                {
                    Instance.LevelID = resultingInt;
                }

            }
            if (ManiacINIClassicSettings.ContainsKey("FGLower"))
            {
                ManiacINIClassicSettings.TryGetValue("FGLower", out value);
                Instance.INILayerNameLower = value;
            }
            if (ManiacINIClassicSettings.ContainsKey("FGHigher"))
            {
                ManiacINIClassicSettings.TryGetValue("FGHigher", out value);
                Instance.INILayerNameHigher = value;
            }
            if (ManiacINIClassicSettings.ContainsKey("WaterColor"))
            {
                ManiacINIClassicSettings.TryGetValue("WaterColor", out value);
                System.Drawing.Color color = System.Drawing.ColorTranslator.FromHtml(value);

                if (ManiacINIClassicSettings.ContainsKey("WaterColorAlpha"))
                {
                    ManiacINIClassicSettings.TryGetValue("WaterColorAlpha", out string value2);
                    Int32.TryParse(value2, out int alpha);
                    color = System.Drawing.Color.FromArgb(alpha, color.R, color.G, color.B);
                }
                Instance.waterColor = color;
            }
            if (ManiacINIClassicSettings.ContainsKey("SpritePaths"))
            {
                ManiacINIClassicSettings.TryGetValue("SpritePaths", out value);
                List<string> list = new List<string>(value.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries));
                Instance.userDefinedSpritePaths = list;
            }
            if (ManiacINIClassicSettings.ContainsKey("EncoreACTFile"))
            {
                ManiacINIClassicSettings.TryGetValue("EncoreACTFile", out value);
                value = value.Replace("\"", "");
                Instance.SetEncorePallete(null, value);
            }
            if (ManiacINIClassicSettings.ContainsKey("CustomMenuFontText"))
            {
                ManiacINIClassicSettings.TryGetValue("CustomMenuFontText", out value);
                Instance.MenuChar = value.ToCharArray();
            }
            if (ManiacINIClassicSettings.ContainsKey("CustomLSelectFontText"))
            {
                ManiacINIClassicSettings.TryGetValue("CustomLSelectFontText", out value);
                Instance.LevelSelectChar = value.ToCharArray();
            }
            if (ManiacINIClassicSettings.ContainsKey("CustomMenuSmallFontText"))
            {
                ManiacINIClassicSettings.TryGetValue("CustomMenuSmallFontText", out value);
                Instance.MenuChar_Small = value.ToCharArray();
            }


        }

        public void UpdateFilePath()
        {
            ManiacINIPath = Path.Combine(Instance.EditorPath.SceneFile_Directory, "Maniac.ini");
        }

        public string GetFilePath()
        {
            return Path.Combine(Instance.EditorPath.SceneFile_Directory, "Maniac.ini");
        }

        public List<string> DataPackNamesToList()
        {
            List<string> PackNames = new List<string>();
            foreach (var config in ManiacINISettings)
            {
                PackNames.Add(config.Item1);
            }
            return PackNames;
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

		public void InterpretInformation()
		{
			ManiacINISettings = new List<Tuple<string, List<Tuple<string, string>>>>();
			foreach (var section in ManiacINIData.Sections)
			{
				List<Tuple<string, string>> Keys = new List<Tuple<string, string>>();
				foreach (var key in section.Keys)
				{
					Keys.Add(new Tuple<string, string>(key.KeyName, key.Value));
				}
				ManiacINISettings.Add(new Tuple<string, List<Tuple<string, string>>>(section.SectionName, Keys));
			}

        }

		public void PrintInformation()
		{
			var n = Environment.NewLine;
			string fullInfo = "";
			foreach(var pair in ManiacINISettings)
			{
				fullInfo += String.Format("[{0}]", pair.Item1) + n;
				foreach (var key in pair.Item2)
				{
					fullInfo += String.Format("   {0}={1}", key.Item1, key.Item2) + n;
				}
			}
			RSDKrU.MessageBox.Show(fullInfo);
		}

		public void SaveFile()
		{
			IniData SaveData = new IniData();
			foreach (var pair in ManiacINISettings)
			{
				SectionData section = new SectionData(pair.Item1);
				foreach (var key in pair.Item2)
				{
					section.Keys.AddKey(key.Item1, key.Item2);
				}
				SaveData.Sections.Add(section);
			}
            string path = GetFilePath();

            var parser = new FileIniDataParser();
			parser.WriteFile(path, SaveData);
		}

		public static FileStream GetModPackList(string path)
		{
			if (!File.Exists(path)) return null;
			return new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
		}

		public bool GetFile()
		{
			var parser = new FileIniDataParser();
            if (!File.Exists(GetFilePath()))
            {
                return false;
            }
			else
			{
                IniData file = parser.ReadFile(GetFilePath());
                ManiacINIData = file;
			}
			return true;
		}
	}
}
