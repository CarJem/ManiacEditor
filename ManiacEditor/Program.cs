using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Linq;
using System.Collections.Generic;
using ManiacEditor.Classes.Internal;

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
            ManiacEditor.Methods.ProgramBase.SetRuntimeRules();
            ManiacEditor.Methods.ProgramBase.DisableDPIScaling();

            Environment.CurrentDirectory = ManiacEditor.Methods.ProgramBase.GetExecutingDirectoryName();

            ManiacEditor.Methods.ProgramBase.GatherObjectsAndAttributes();
            ManiacEditor.Methods.ProgramBase.SetupRenderingOptions();
            ManiacEditor.Methods.ProgramBase.SetupSettingFiles();
            StartApp();
            DiscordRP.DisposeDiscord();
        }

        private static void StartApp()
        {
            if (Core.Settings.MySettings.ShowUnhandledExceptions)
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
    }
}
