using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using System.Windows;
using ManiacEditor.Classes.Internal;


namespace ManiacEditor.Methods
{
    public static class ProgramBase
    {
        public static bool IsDebug { get; private set; } = false;

        public static Version RuntimeVersion { get; set; } = null;

        public static Version GetVersion()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            if (IsDebug)
            {
                if (RuntimeVersion == null)
                {
                    string runtimeVer = AssemblyAttributeAccessors.GetBuildDate.ToString("y.M.d.0");
                    RuntimeVersion = new Version(runtimeVer);
                }
                return RuntimeVersion;
            }

            return version;
        }

        public static string GetCasualVersion()
        {
            string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            if (IsDebug)
            {
                if (RuntimeVersion == null)
                {
                    string runtimeVer = AssemblyAttributeAccessors.GetBuildDate.ToString("y.M.d.0");
                    RuntimeVersion = new Version(runtimeVer);
                }

                string verString = RuntimeVersion.ToString();
                string devVersion = verString.TrimEnd(verString[verString.Length - 1]) + "DEV";
                return devVersion;
            }

            return version;
        }

        public static string GetExecutingDirectoryName()
        {
            string exeLocationUrl = Assembly.GetEntryAssembly().GetName().CodeBase;
            string exeLocation = new Uri(exeLocationUrl).LocalPath;
            return new FileInfo(exeLocation).Directory.FullName;
        }

        public static class AssemblyAttributeAccessors
        {
            public static string AssemblyTitle
            {
                get
                {
                    object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                    if (attributes.Length > 0)
                    {
                        AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                        if (titleAttribute.Title != "")
                        {
                            return titleAttribute.Title;
                        }
                    }
                    return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
                }
            }

            public static string GetProgramType
            {
                get
                {
                    if (Environment.Is64BitProcess)
                    {
                        return "x64";
                    }
                    else
                    {
                        return "x86";
                    }
                }
            }

            public static string AssemblyProduct
            {
                get
                {
                    object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                    if (attributes.Length == 0)
                    {
                        return "";
                    }
                    return ((AssemblyProductAttribute)attributes[0]).Product;
                }
            }

            public static string AssemblyCopyright
            {
                get
                {
                    object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                    if (attributes.Length == 0)
                    {
                        return "";
                    }
                    return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
                }
            }

            public static string GetBuildTime
            {
                get
                {
                    DateTime buildDate = new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).LastWriteTime;
                    String buildTimeString = buildDate.ToString();
                    return buildTimeString;
                }

            }

            public static DateTime GetBuildDate
            {
                get 
                {
                    var assembly = Assembly.GetExecutingAssembly();
                    return GetLinkerTime(assembly);
                }
            }

            private static DateTime GetLinkerTime(Assembly assembly, TimeZoneInfo target = null)
            {
                var filePath = assembly.Location;
                const int c_PeHeaderOffset = 60;
                const int c_LinkerTimestampOffset = 8;

                var buffer = new byte[2048];

                using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                    stream.Read(buffer, 0, 2048);

                var offset = BitConverter.ToInt32(buffer, c_PeHeaderOffset);
                var secondsSince1970 = BitConverter.ToInt32(buffer, offset + c_LinkerTimestampOffset);
                var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

                var linkTimeUtc = epoch.AddSeconds(secondsSince1970);

                var tz = target ?? TimeZoneInfo.Local;
                var localTime = TimeZoneInfo.ConvertTimeFromUtc(linkTimeUtc, tz);

                return localTime;
            }
        }


        #region Initilization

        public static void GatherObjectsAndAttributes()
        {
            try
            {
                string resource_folder = Environment.CurrentDirectory + @"\Resources\";
                string definitions_file = Path.Combine(resource_folder, "entity_definitions.json");

                Console.WriteLine("Setting up Object & Attribute Definitions");

                string data = File.ReadAllText(definitions_file);
                EntityDefinitions definitions = Newtonsoft.Json.JsonConvert.DeserializeObject<EntityDefinitions>(data);

                foreach (string attribute in definitions.Attributes) RSDKv5.Objects.AddAttributeName(attribute);
                foreach (string entityObject in definitions.Objects) RSDKv5.Objects.AddObjectName(entityObject);

                Console.WriteLine("Finished Object & Attribute Definitions");

            }
            catch (FileNotFoundException fnfe)
            {
                DisplayLoadFailure($@"{fnfe.Message} Missing file: {fnfe.FileName}");
            }
            catch (Exception e)
            {
                DisplayLoadFailure(e.Message);
            }

        }

        public static void DisplayLoadFailure(string message)
        {
            MessageBox.Show(message, "Unable to start.", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static void SetupRenderingOptions()
        {
            EntityRenderingOptions.GetExternalData(ref ManiacEditor.Methods.Entities.EntityDrawing.RenderingSettings);
        }

        public static void SetupSettingFiles()
        {
            string currentDirectory = ManiacEditor.Methods.ProgramBase.GetExecutingDirectoryName();
            if (!File.Exists(Path.Combine(currentDirectory, "Internal.settings"))) File.Create(Path.Combine(currentDirectory, "Internal.settings"));
            bool PortableExists = System.IO.Directory.Exists(Classes.Editor.Constants.SettingsPortableDirectory);
            if (!PortableExists) System.IO.Directory.CreateDirectory(Classes.Editor.Constants.SettingsPortableDirectory);
            bool FixedExists = System.IO.Directory.Exists(Classes.Editor.Constants.SettingsStaticDirectory);
            if (!FixedExists) System.IO.Directory.CreateDirectory(Classes.Editor.Constants.SettingsStaticDirectory);

            if (Properties.Internal.Default.PortableMode)
            {
                string settings1 = Path.Combine(Classes.Editor.Constants.SettingsPortableDirectory, string.Format("{0}.settings", "Performance"));
                string settings2 = Path.Combine(Classes.Editor.Constants.SettingsPortableDirectory, string.Format("{0}.settings", "DevOptions"));
                string settings3 = Path.Combine(Classes.Editor.Constants.SettingsPortableDirectory, string.Format("{0}.settings", "Defaults"));
                string settings4 = Path.Combine(Classes.Editor.Constants.SettingsPortableDirectory, string.Format("{0}.settings", "Keybinds"));
                string settings5 = Path.Combine(Classes.Editor.Constants.SettingsPortableDirectory, string.Format("{0}.settings", "Settings"));
                string settings6 = Path.Combine(Classes.Editor.Constants.SettingsPortableDirectory, string.Format("{0}.settings", "GameOptions"));
                if (!File.Exists(settings1)) File.Create(settings1).Close();
                if (!File.Exists(settings2)) File.Create(settings2).Close();
                if (!File.Exists(settings3)) File.Create(settings3).Close();
                if (!File.Exists(settings4)) File.Create(settings4).Close();
                if (!File.Exists(settings5)) File.Create(settings5).Close();
                if (!File.Exists(settings6)) File.Create(settings6).Close();
            }
            else
            {

                string settings1 = Path.Combine(Classes.Editor.Constants.SettingsStaticDirectory, string.Format("{0}.settings", "Performance"));
                string settings2 = Path.Combine(Classes.Editor.Constants.SettingsStaticDirectory, string.Format("{0}.settings", "DevOptions"));
                string settings3 = Path.Combine(Classes.Editor.Constants.SettingsStaticDirectory, string.Format("{0}.settings", "Defaults"));
                string settings4 = Path.Combine(Classes.Editor.Constants.SettingsStaticDirectory, string.Format("{0}.settings", "Keybinds"));
                string settings5 = Path.Combine(Classes.Editor.Constants.SettingsStaticDirectory, string.Format("{0}.settings", "Settings"));
                string settings6 = Path.Combine(Classes.Editor.Constants.SettingsStaticDirectory, string.Format("{0}.settings", "GameOptions"));
                if (!File.Exists(settings1)) File.Create(settings1).Close();
                if (!File.Exists(settings2)) File.Create(settings2).Close();
                if (!File.Exists(settings3)) File.Create(settings3).Close();
                if (!File.Exists(settings4)) File.Create(settings4).Close();
                if (!File.Exists(settings5)) File.Create(settings5).Close();
                if (!File.Exists(settings6)) File.Create(settings6).Close();
            }


        }

        public static void DisableDPIScaling()
        {
            string currentDirectory = ManiacEditor.Methods.ProgramBase.GetExecutingDirectoryName();
            string appPath = string.Format(@"{0}\{1}.exe", currentDirectory, Assembly.GetExecutingAssembly().GetName().Name);
            Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows NT\CurrentVersion\AppCompatFlags\Layers", appPath, "~ PERPROCESSSYSTEMDPIFORCEON DPIUNAWARE");
        }

        public static void SetRuntimeRules()
        {
            #if DEBUG
            System.Diagnostics.PresentationTraceSources.DataBindingSource.Switch.Level = System.Diagnostics.SourceLevels.Critical;
            IsDebug = true;
            #endif
        }

        #endregion
    }
}
