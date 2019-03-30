using System;
using System.IO;
using System.Reflection;
using System.Windows;

namespace ManiacEditor
{
    static class Program
    {
        public static string DataDir = "";
        public static string ScenePath = "";
        public static string ModPath = "";
        public static int LevelID = -1;
        public static int X = 0;
        public static int Y = 0;
        public static bool isEncoreMode = false;
        public static bool launchAsShortcut = false;
        public static int shortcutMode = 0;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);


            string appPath = string.Format(@"{0}\{1}.exe", GetExecutingDirectoryName(), Assembly.GetExecutingAssembly().GetName().Name);
            Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows NT\CurrentVersion\AppCompatFlags\Layers", appPath, "~ PERPROCESSSYSTEMDPIFORCEON DPIUNAWARE");

            foreach (string argument in args)
            {
                if (argument.StartsWith("DataDir="))
                {
                    DataDir = argument.Substring(8);
                }
                else if (argument.StartsWith("ScenePath="))
                {
                    ScenePath = argument.Substring(10);
                }
                else if (argument.StartsWith("ModPath="))
                {
                    ModPath = argument.Substring(8);
                }
                else if (argument.StartsWith("LevelID="))
                {
                    Int32.TryParse(argument.Substring(8), out LevelID);
                }
                else if (argument.Equals("EncoreMode=TRUE"))
                {
                    isEncoreMode = true;
                }
                else if (argument.StartsWith("X="))
                {
                    Int32.TryParse(argument.Substring(2), out X);
                }
                else if (argument.StartsWith("Y="))
                {
                    Int32.TryParse(argument.Substring(2), out Y);
                }
            }
            //Remove the "" from the arguments to prevent errors.
            DataDir.Replace("\"", "");
            ScenePath.Replace("\"", "");
            ModPath.Replace("\"", "");

            if (DataDir != "" && ScenePath != "")
            {
                launchAsShortcut = true;
                shortcutMode = 1;
            }
            else if (DataDir != "")
            {
                launchAsShortcut = true;
                shortcutMode = 0;
            }

            Environment.CurrentDirectory = GetExecutingDirectoryName();

            bool allowedToLoad = false;
            try
            {
                Console.WriteLine("Setting up Objects");
                string objIni = Environment.CurrentDirectory + @"\Resources\objects.ini";
                string attribIni = Environment.CurrentDirectory + @"\Resources\attributes.ini";
                RSDKv5.Objects.InitObjectNames(new StreamReader(File.OpenRead(objIni)));
                RSDKv5.Objects.InitAttributeNames(new StreamReader(File.OpenRead(attribIni)));
                allowedToLoad = true;
            }
            catch (FileNotFoundException fnfe)
            {
                DisplayLoadFailure($@"{fnfe.Message}
Missing file: {fnfe.FileName}");
            }
            catch (Exception e)
            {
                DisplayLoadFailure(e.Message);
            }
            Console.WriteLine("Finished Objects");
            SetupSettingFiles();
            if (allowedToLoad)
            {
                if (Settings.MySettings.ShowUnhandledExceptions)
                {
                    try
                    {
                        StartApp();
                    }
                    catch (Exception ex)
                    {
                       MessageBox.Show(ex.ToString());
                       throw ex;
                    }
                }
                else
                {
                    StartApp();
                }


            }
        }

        private static void StartApp()
        {
            var application = new ManiacEditor.App();
            application.InitializeComponent();
            application.Load(DataDir, ScenePath, ModPath, LevelID, launchAsShortcut, shortcutMode, isEncoreMode, X, Y);
        }

        private static void SetupSettingFiles()
        {
            if (!File.Exists(Path.Combine(GetExecutingDirectoryName(), "Internal.settings"))) File.Create(Path.Combine(GetExecutingDirectoryName(), "Internal.settings"));

            if (Properties.Internal.Default.PortableMode)
            {
                bool exists = System.IO.Directory.Exists(EditorConstants.SettingsPortableDirectory);
                if (!exists) System.IO.Directory.CreateDirectory(EditorConstants.SettingsPortableDirectory);
                string settings1 = Path.Combine(EditorConstants.SettingsPortableDirectory, string.Format("{0}.settings", "Performance"));
                string settings2 = Path.Combine(EditorConstants.SettingsPortableDirectory, string.Format("{0}.settings", "DevOptions"));
                string settings3 = Path.Combine(EditorConstants.SettingsPortableDirectory, string.Format("{0}.settings", "Defaults"));
                string settings4 = Path.Combine(EditorConstants.SettingsPortableDirectory, string.Format("{0}.settings", "Keybinds"));
                string settings5 = Path.Combine(EditorConstants.SettingsPortableDirectory, string.Format("{0}.settings", "Settings"));
                string settings6 = Path.Combine(EditorConstants.SettingsPortableDirectory, string.Format("{0}.settings", "GameOptions"));
                if (!File.Exists(settings1)) File.Create(settings1).Close();
                if (!File.Exists(settings2)) File.Create(settings2).Close();
                if (!File.Exists(settings3)) File.Create(settings3).Close(); 
                if (!File.Exists(settings4)) File.Create(settings4).Close();
                if (!File.Exists(settings5)) File.Create(settings5).Close();
                if (!File.Exists(settings6)) File.Create(settings6).Close();
            }
            else
            {
                bool exists = System.IO.Directory.Exists(EditorConstants.SettingsStaticDirectory);
                if (!exists) System.IO.Directory.CreateDirectory(EditorConstants.SettingsStaticDirectory);
                string settings1 = Path.Combine(EditorConstants.SettingsStaticDirectory, string.Format("{0}.settings", "Performance"));
                string settings2 = Path.Combine(EditorConstants.SettingsStaticDirectory, string.Format("{0}.settings", "DevOptions"));
                string settings3 = Path.Combine(EditorConstants.SettingsStaticDirectory, string.Format("{0}.settings", "Defaults"));
                string settings4 = Path.Combine(EditorConstants.SettingsStaticDirectory, string.Format("{0}.settings", "Keybinds"));
                string settings5 = Path.Combine(EditorConstants.SettingsStaticDirectory, string.Format("{0}.settings", "Settings"));
                string settings6 = Path.Combine(EditorConstants.SettingsStaticDirectory, string.Format("{0}.settings", "GameOptions"));
                if (!File.Exists(settings1)) File.Create(settings1).Close();
                if (!File.Exists(settings2)) File.Create(settings2).Close();
                if (!File.Exists(settings3)) File.Create(settings3).Close();
                if (!File.Exists(settings4)) File.Create(settings4).Close();
                if (!File.Exists(settings5)) File.Create(settings5).Close();
                if (!File.Exists(settings6)) File.Create(settings6).Close();
            }


        }

        private static void DisplayLoadFailure(string message)
        {
            MessageBox.Show(message,
                            "Unable to start.",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
        }

        private static string GetExecutingDirectoryName()
        {
            string exeLocationUrl = Assembly.GetEntryAssembly().GetName().CodeBase;
            string exeLocation = new Uri(exeLocationUrl).LocalPath;
            return new FileInfo(exeLocation).Directory.FullName;
        }
    }
}
