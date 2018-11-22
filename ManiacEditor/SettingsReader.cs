using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.Configuration;
using IniParser;
using IniParser.Model;


namespace ManiacEditor
{
    [Serializable]
    class SettingsReader
    {
        static Dictionary<String, String> SceneINISettings = new Dictionary<string, string> { };

        public static void exportSettings()
        {
            bool stillMore = true;
            int i = 0;
            int optionCount = 0;
            while (stillMore == true)
            {
                if (ConfigurationManager.AppSettings[i] != null)
                {
                    optionCount++;
                }
                else
                {
                    stillMore = false;
                }
                i++;
            }
            MessageBox.Show(optionCount.ToString(), "hey");

            /*for (int j = 0; j < optionCount; j++)
            {

            }*/


        }
        public static void importSettings()
        {

        }

        public static FileStream GetSceneIniResource(string path)
        {
            if (!File.Exists(path))
            {
                return null;
            }

            return new FileStream(path,
                                  FileMode.Open,
                                  FileAccess.Read,
                                  FileShare.Read);
        }

        public static void GetSceneINISettings(Stream stream)
        {
            var parser = new IniParser.StreamIniDataParser();
            IniData data = parser.ReadData(new StreamReader(stream));

            foreach (var section in data.Sections)
            {
                foreach (var key in section.Keys)
                {
                    SceneINISettings.Add(key.KeyName, key.Value);
                }
            }


        }


        public static Dictionary<String,String> ReturnPrefrences()
        {
            return SceneINISettings;
        }

        public static void CleanPrefrences()
        {
            SceneINISettings.Clear();
        }
    }

}
