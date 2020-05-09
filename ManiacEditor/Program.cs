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
            Classes.Options.InternalSwitches.Init();
            ManiacEditor.Methods.ProgramBase.StartLogging();
            Extensions.ConsoleExtensions.PrintWithLog("Setting Up Options Files...");
            ManiacEditor.Properties.Settings.Init();
            Extensions.ConsoleExtensions.PrintWithLog("Starting Maniac Editor...");
            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);

            Extensions.ConsoleExtensions.PrintWithLog("Initilizing Discord RPC Support...");
            DiscordRP.InitDiscord();

            Extensions.ConsoleExtensions.PrintWithLog("Optimizing Maniac Editor Program Launch Properties...");
            ManiacEditor.Methods.ProgramBase.SetRuntimeRules();
            ManiacEditor.Methods.ProgramBase.DisableDPIScaling();

            Environment.CurrentDirectory = ManiacEditor.Methods.ProgramBase.GetExecutingDirectoryName();

            Extensions.ConsoleExtensions.PrintWithLog("Gathering Objects And Attributes...");
            ManiacEditor.Methods.ProgramBase.GatherObjectsAndAttributes();
            StartApp();
            Extensions.ConsoleExtensions.PrintWithLog("Disposing Discord RPC Support...");
            DiscordRP.DisposeDiscord();
            Extensions.ConsoleExtensions.PrintWithLog("Closing Maniac Editor...");
            ManiacEditor.Methods.ProgramBase.EndLogging();
        }

        private static void StartApp()
        {
            Extensions.ConsoleExtensions.PrintWithLog("Launching the Map Editor...");
            try
            {
                Load();
            }
            catch (Exception ex)
            {
                Extensions.ConsoleExtensions.PrintWithLog(ex.Message);
                Extensions.ConsoleExtensions.ShowManiacConsole(true);
            }


            void Load()
            {
                var application = new ManiacEditor.App();
                application.Load();
            }

        }
    }
}
