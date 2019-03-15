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
                using (var stream = GetObjectsIniResource())
                {
                   RSDKv5.Objects.InitObjects(stream);
                    allowedToLoad = true;
                }
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

            if (allowedToLoad)
            {
                var application = new ManiacEditor.App();
				application.InitializeComponent();
                application.Load(DataDir, ScenePath, ModPath, LevelID, launchAsShortcut, shortcutMode, isEncoreMode, X, Y);
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

        private static FileStream GetObjectsIniResource()
        {
            string executingDirectory = GetExecutingDirectoryName();
            string fullPathToIni = executingDirectory + @"\Resources\objects_attributes.ini";
            if (!File.Exists(fullPathToIni))
            {
                throw new FileNotFoundException("Unable to find the required file for naming objects and attributes.",
                                                @"\Resources\objects_attributes.ini");
            }

            return new FileStream(fullPathToIni,
                                  FileMode.Open,
                                  FileAccess.Read,
                                  FileShare.Read);
        }
    }
}
