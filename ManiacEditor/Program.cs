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
            ManiacEditor.Methods.ProgramBase.StartLogging();
            ManiacEditor.Methods.ProgramBase.Log.InfoFormat("Starting Maniac Editor...");
            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);

            ManiacEditor.Methods.ProgramBase.Log.InfoFormat("Initilizing Discord RPC Support...");
            DiscordRP.InitDiscord();

            ManiacEditor.Methods.ProgramBase.Log.InfoFormat("Optimizing Maniac Editor Program Launch Properties...");
            ManiacEditor.Methods.ProgramBase.SetRuntimeRules();
            ManiacEditor.Methods.ProgramBase.DisableDPIScaling();

            Environment.CurrentDirectory = ManiacEditor.Methods.ProgramBase.GetExecutingDirectoryName();

            ManiacEditor.Methods.ProgramBase.Log.InfoFormat("Gathering Objects And Attributes...");
            ManiacEditor.Methods.ProgramBase.GatherObjectsAndAttributes();
            ManiacEditor.Methods.ProgramBase.Log.InfoFormat("Setting Up Rendering Options...");
            ManiacEditor.Methods.ProgramBase.SetupRenderingOptions();
            ManiacEditor.Methods.ProgramBase.Log.InfoFormat("Setting Up Options Files...");
            ManiacEditor.Methods.ProgramBase.SetupSettingFiles();
            StartApp();
            ManiacEditor.Methods.ProgramBase.Log.InfoFormat("Disposing Discord RPC Support...");
            DiscordRP.DisposeDiscord();
            ManiacEditor.Methods.ProgramBase.Log.InfoFormat("Closing Maniac Editor...");
            ManiacEditor.Methods.ProgramBase.EndLogging();
        }

        private static void StartApp()
        {
            ManiacEditor.Methods.ProgramBase.Log.InfoFormat("Launching the Map Editor...");
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
