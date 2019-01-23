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
