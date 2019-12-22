using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Linq;
using System.Collections.Generic;
using ManiacEditor.Structures;

namespace ManiacEditor
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);

            DiscordRP.InitDiscord();
            SetRuntimeRules();
            DisableDPIScaling();

            Environment.CurrentDirectory = GetExecutingDirectoryName();

            GatherObjectsAndAttributes();
            SetupRenderingOptions();
            SetupSettingFiles();
            StartApp();
            DiscordRP.DisposeDiscord();
        }

        private static void StartApp()
        {
            if (Settings.MySettings.ShowUnhandledExceptions)
            {
                try
                {
                    Load();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    throw ex;
                }
            }
            else Load();


            void Load()
            {
                var application = new ManiacEditor.App();
                application.Load();
            }

        }

        #region Initilization

        private static void GatherObjectsAndAttributes()
        {
            try
            {
                string resource_folder = Environment.CurrentDirectory + @"\Resources\";
                string definitions_file = Path.Combine(resource_folder, "entity_definitions.json");

                Console.WriteLine("Setting up Object & Attribute Definitions");

                string data = File.ReadAllText(definitions_file);
                Structures.EntityDefinitions definitions = Newtonsoft.Json.JsonConvert.DeserializeObject<EntityDefinitions>(data);

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

        private static void DisplayLoadFailure(string message)
        {
            MessageBox.Show(message, "Unable to start.", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private static void SetupRenderingOptions()
        {
            EntityRenderingOptions.GetExternalData(ref ManiacEditor.EditorEntityDrawing.RenderingSettings);

        }

        private static void SetupSettingFiles()
        {
            if (!File.Exists(Path.Combine(GetExecutingDirectoryName(), "Internal.settings"))) File.Create(Path.Combine(GetExecutingDirectoryName(), "Internal.settings"));
            bool PortableExists = System.IO.Directory.Exists(EditorConstants.SettingsPortableDirectory);
            if (!PortableExists) System.IO.Directory.CreateDirectory(EditorConstants.SettingsPortableDirectory);
            bool FixedExists = System.IO.Directory.Exists(EditorConstants.SettingsStaticDirectory);
            if (!FixedExists) System.IO.Directory.CreateDirectory(EditorConstants.SettingsStaticDirectory);

            if (Properties.Internal.Default.PortableMode)
            {
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

        private static void DisableDPIScaling()
        {
            string appPath = string.Format(@"{0}\{1}.exe", GetExecutingDirectoryName(), Assembly.GetExecutingAssembly().GetName().Name);
            Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows NT\CurrentVersion\AppCompatFlags\Layers", appPath, "~ PERPROCESSSYSTEMDPIFORCEON DPIUNAWARE");
        }

        private static void SetRuntimeRules()
        {
#if DEBUG
            System.Diagnostics.PresentationTraceSources.DataBindingSource.Switch.Level = System.Diagnostics.SourceLevels.Critical;
#endif
        }

        #endregion



        private static string GetExecutingDirectoryName()
        {
            string exeLocationUrl = Assembly.GetEntryAssembly().GetName().CodeBase;
            string exeLocation = new Uri(exeLocationUrl).LocalPath;
            return new FileInfo(exeLocation).Directory.FullName;
        }


    }
}
