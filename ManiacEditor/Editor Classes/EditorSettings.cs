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
    class EditorSettings
    {
        static Dictionary<String, String> SceneINISettings = new Dictionary<string, string> { };

        public static void exportSettings()
        {
            foreach (SettingsProperty currentProperty in Properties.Settings.Default.Properties)
            {
                //Properties.Settings.Default[currentProperty.Name]
            }
        }
        public static void importSettings()
        {

        }

		public static void SetINIDefaultPrefrences(Editor instance)
		{
			string value;
			Dictionary<String, String> ListedPrefrences = EditorSettings.ReturnPrefrences();
			if (ListedPrefrences.ContainsKey("LevelID"))
			{
				ListedPrefrences.TryGetValue("LevelID", out value);
				Int32.TryParse(value, out int resultingInt);
				if (resultingInt >= -1)
				{
					instance.LevelID = resultingInt;
				}

			}
			if (ListedPrefrences.ContainsKey("FGLower"))
			{
				ListedPrefrences.TryGetValue("FGLower", out value);
				instance.INILayerNameLower = value;
			}
			if (ListedPrefrences.ContainsKey("FGHigher"))
			{
				ListedPrefrences.TryGetValue("FGHigher", out value);
				instance.INILayerNameHigher = value;
			}
			if (ListedPrefrences.ContainsKey("WaterColor"))
			{
				ListedPrefrences.TryGetValue("WaterColor", out value);
				Color color = System.Drawing.ColorTranslator.FromHtml(value);

				if (ListedPrefrences.ContainsKey("WaterColorAlpha"))
				{
					ListedPrefrences.TryGetValue("WaterColorAlpha", out string value2);
					Int32.TryParse(value2, out int alpha);
					color = Color.FromArgb(alpha, color.R, color.G, color.B);
				}
				instance.waterColor = color;
			}
			if (ListedPrefrences.ContainsKey("SpritePaths"))
			{
				ListedPrefrences.TryGetValue("SpritePaths", out value);
				List<string> list = new List<string>(value.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries));
				foreach (string item in list)
				{
					System.Windows.MessageBox.Show(item);
				}
				instance.userDefinedSpritePaths = list;
			}
			if (ListedPrefrences.ContainsKey("SwapEntityRenderNames"))
			{
				ListedPrefrences.TryGetValue("SwapEntityRenderNames", out value);
				List<string> list = new List<string>(value.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries));
				if (list.Count % 2 == 0 && list.Count != 0)
				{
					for (int i = 0; i < list.Count;)
					{
						string toBeSwapped = list[i];
						string toSet = list[i + 1];
						System.Windows.MessageBox.Show(toBeSwapped + "-> " + toSet);
						instance.userDefinedEntityRenderSwaps.Add(toBeSwapped, toSet);
						i = i + 2;
					}
				}
				else
				{
					System.Windows.MessageBox.Show("There is an odd number of swaps for entity names, please double check your maniac.ini file");
				}


			}
			if (ListedPrefrences.ContainsKey("EncoreACTFile"))
			{
				ListedPrefrences.TryGetValue("EncoreACTFile", out value);
				value = value.Replace("\"", "");
				instance.SetEncorePallete(null, value);
			}
			if (ListedPrefrences.ContainsKey("CustomMenuFontText"))
			{
				ListedPrefrences.TryGetValue("CustomMenuFontText", out value);
				instance.MenuChar = value.ToCharArray();
			}
			if (ListedPrefrences.ContainsKey("CustomLSelectFontText"))
			{
				ListedPrefrences.TryGetValue("CustomLSelectFontText", out value);
				instance.LevelSelectChar = value.ToCharArray();
			}
			if (ListedPrefrences.ContainsKey("CustomMenuSmallFontText"))
			{
				ListedPrefrences.TryGetValue("CustomMenuSmallFontText", out value);
				instance.MenuChar_Small = value.ToCharArray();
			}


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

        public static void ApplyPreset(int state)
        {
            switch (state)
            {
                case 0:
                    ApplyMinimalPreset();
                    break;
                case 1:
                    ApplyBasicPreset();
                    break;
                case 2:
                    ApplySuperPreset();
                    break;
                case 3:
                    ApplyHyperPreset();
                    break;
            }
        }

        public static void ApplyPreset(string state)
        {
            switch (state)
            {
                case "Minimal":
                    ApplyMinimalPreset();
                    break;
                case "Basic":
                    ApplyBasicPreset();
                    break;
                case "Super":
                    ApplySuperPreset();
                    break;
                case "Hyper":
                    ApplyHyperPreset();
                    break;
            }
        }

        public static void ApplyMinimalPreset()
        {
            Settings.mySettings.allowForSmoothSelection = false;
            Settings.mySettings.AllowMoreRenderUpdates = false;
            Settings.mySettings.ShowEditLayerBackground = false;
            Settings.mySettings.SimplifiedWaterLevelRendering = true;
            Settings.mySettings.PrioritizedObjectRendering = false;
            Settings.mySettings.DisableRenderExlusions = true;
            Settings.mySettings.NeverLoadEntityTextures = true;
            Settings.mySettings.preRenderTURBOMode = false;
        }

        public static void ApplyBasicPreset()
        {
            Settings.mySettings.allowForSmoothSelection = false;
            Settings.mySettings.AllowMoreRenderUpdates = false;
            Settings.mySettings.ShowEditLayerBackground = true;
            Settings.mySettings.PrioritizedObjectRendering = false;
            Settings.mySettings.SimplifiedWaterLevelRendering = true;
            Settings.mySettings.DisableRenderExlusions = true;
            Settings.mySettings.NeverLoadEntityTextures = false;
            Settings.mySettings.preRenderTURBOMode = false;
        }

        public static void ApplySuperPreset()
        {
            Settings.mySettings.allowForSmoothSelection = true;
            Settings.mySettings.AllowMoreRenderUpdates = true;
            Settings.mySettings.ShowEditLayerBackground = true;
            Settings.mySettings.PrioritizedObjectRendering = true;
            Settings.mySettings.SimplifiedWaterLevelRendering = false;
            Settings.mySettings.DisableRenderExlusions = false;
            Settings.mySettings.NeverLoadEntityTextures = false;
            Settings.mySettings.preRenderTURBOMode = false;
        }

        public static void ApplyHyperPreset()
        {
            Settings.mySettings.allowForSmoothSelection = true;
            Settings.mySettings.AllowMoreRenderUpdates = true;
            Settings.mySettings.ShowEditLayerBackground = true;
            Settings.mySettings.preRenderTURBOMode = true;
            Settings.mySettings.PrioritizedObjectRendering = true;
            Settings.mySettings.SimplifiedWaterLevelRendering = false;
            Settings.mySettings.DisableRenderExlusions = false;
            Settings.mySettings.NeverLoadEntityTextures = false;
        }



        public static bool isMinimalPreset()
        {
            bool isMinimal = false;
            if (Settings.mySettings.allowForSmoothSelection == false)
            {
                if (Settings.mySettings.AllowMoreRenderUpdates == false)
                    if (Settings.mySettings.ShowEditLayerBackground == false)
                        if (Settings.mySettings.PrioritizedObjectRendering == false)
                            if (Settings.mySettings.SimplifiedWaterLevelRendering == true)
                            if (Settings.mySettings.DisableRenderExlusions == true)
                                if (Settings.mySettings.NeverLoadEntityTextures == true)
                                    if (Settings.mySettings.preRenderTURBOMode == false)
                                        isMinimal = true;
            }
            return isMinimal;

        }

        public static bool isBasicPreset()
        {
            bool isBasic = false;
            if (Settings.mySettings.allowForSmoothSelection == false)
            {
                if (Settings.mySettings.AllowMoreRenderUpdates == false)
                    if (Settings.mySettings.ShowEditLayerBackground == true)
                        if (Settings.mySettings.PrioritizedObjectRendering == false)
                            if (Settings.mySettings.SimplifiedWaterLevelRendering == true)
                            if (Settings.mySettings.DisableRenderExlusions == true)
                                if (Settings.mySettings.NeverLoadEntityTextures == false)
                                    if (Settings.mySettings.preRenderTURBOMode == false)
                                        isBasic = true;
            }
            return isBasic;
        }

        public static bool isSuperPreset()
        {
            bool isSuper = false;
            if (Settings.mySettings.allowForSmoothSelection == true)
            {
                if (Settings.mySettings.AllowMoreRenderUpdates == true)
                    if (Settings.mySettings.ShowEditLayerBackground == true)
                        if (Settings.mySettings.PrioritizedObjectRendering == true)
                            if (Settings.mySettings.SimplifiedWaterLevelRendering == false)
                            if (Settings.mySettings.DisableRenderExlusions == false)
                                if (Settings.mySettings.NeverLoadEntityTextures == false)
                                    if (Settings.mySettings.preRenderTURBOMode == false)
                                        isSuper = true;
            }
            return isSuper;
        }

        public static bool isHyperPreset()
        {
            bool isHyper = false;
            if (Settings.mySettings.allowForSmoothSelection == true)
            {
                if (Settings.mySettings.AllowMoreRenderUpdates == true)
                    if (Settings.mySettings.ShowEditLayerBackground == true)
                        if (Settings.mySettings.preRenderTURBOMode == true)
                            if (Settings.mySettings.PrioritizedObjectRendering == true)
                                if (Settings.mySettings.SimplifiedWaterLevelRendering == false)
                                    if (Settings.mySettings.DisableRenderExlusions == false)
                                        if (Settings.mySettings.NeverLoadEntityTextures == false)
                                            isHyper = true;
            }
            return isHyper;
        }
    }

}
