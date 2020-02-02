using System;
using System.Collections.Generic;
using System.IO;
using IniParser;
using IniParser.Model;

namespace ManiacEditor.Methods.Prefrences
{
    public static class DataPackStorage
	{
		public static Controls.Base.MainEditor Instance;
		static IniData ModPackInfo;
		public static List<Tuple<string, List<Tuple<string, string>>>> ModListInformation;
        private static string SettingsFolder { get => GetDataPackDirectory(); }

        private static string GetDataPackDirectory()
        {
            return (Properties.Internal.Default.PortableMode ? Classes.Editor.Constants.SettingsPortableDirectory : Classes.Editor.Constants.SettingsStaticDirectory);
        }

		public static void Initilize(Controls.Base.MainEditor instance)
		{
			UpdateInstance(instance);
			LoadFile();
		}

		public static void UpdateInstance(Controls.Base.MainEditor instance)
		{
			Instance = instance;
		}

        public static List<string> DataPackNamesToList()
        {
            List<string> PackNames = new List<string>();
            foreach (var config in ModListInformation)
            {
                PackNames.Add(config.Item1);
            }
            return PackNames;
        }

		public static void LoadFile()
		{
			if (GetFile() == false)
			{
				var ModListFile = File.Create(Path.Combine(SettingsFolder, "ModPackLists.ini"));
				ModListFile.Close();
				if (GetFile() == false) return;
			}
			InterpretInformation();
		}

		public static void InterpretInformation()
		{
			ModListInformation = new List<Tuple<string, List<Tuple<string, string>>>>();
			foreach (var section in ModPackInfo.Sections)
			{
				List<Tuple<string, string>> Keys = new List<Tuple<string, string>>();
				foreach (var key in section.Keys)
				{
					Keys.Add(new Tuple<string, string>(key.KeyName, key.Value));
				}
				ModListInformation.Add(new Tuple<string, List<Tuple<string, string>>>(section.SectionName, Keys));
			}

        }

		public static void PrintInformation()
		{
			var n = Environment.NewLine;
			string fullInfo = "";
			foreach(var pair in ModListInformation)
			{
				fullInfo += String.Format("[{0}]", pair.Item1) + n;
				foreach (var key in pair.Item2)
				{
					fullInfo += String.Format("   {0}={1}", key.Item1, key.Item2) + n;
				}
			}
			System.Windows.MessageBox.Show(fullInfo);
		}

		public static void SaveFile()
		{
			IniData SaveData = new IniData();
			foreach (var pair in ModListInformation)
			{
				SectionData section = new SectionData(pair.Item1);
				foreach (var key in pair.Item2)
				{
					section.Keys.AddKey(key.Item1, key.Item2);
				}
				SaveData.Sections.Add(section);
			}
			string path = Path.Combine(SettingsFolder, "ModPackLists.ini");
			var parser = new FileIniDataParser();
			parser.WriteFile(path, SaveData);
		}

		public static FileStream GetModPackList(string path)
		{
			if (!File.Exists(path)) return null;
			return new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
		}

		public static bool GetFile()
		{
			var parser = new FileIniDataParser();
            if (!File.Exists(Path.Combine(SettingsFolder, "ModPackLists.ini")))
            {
                return false;
            }
			else
			{
                IniData file = parser.ReadFile(Path.Combine(SettingsFolder, "ModPackLists.ini"));
                ModPackInfo = file;
			}
			return true;
		}
	}
}
