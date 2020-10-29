using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Linq;
using System.Collections.Generic;
using ManiacEditor.Structures;
using System.Diagnostics;

namespace ManiacEditor
{
    static class Program
    {
        private static bool UseDebuggerForErrors
        {
            get
            {
                return Debugger.IsAttached;
            }
        }
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
            Load();

            void Load()
            {
                var application = new ManiacEditor.App();
                var mainWindow = new Controls.Editor.MainEditor();
                application.MainWindow = mainWindow;
                application.ShutdownMode = ShutdownMode.OnMainWindowClose;
                application.Run(mainWindow);
            }

        }
    }
}
