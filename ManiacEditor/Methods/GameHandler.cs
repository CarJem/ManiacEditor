using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using Point = System.Drawing.Point;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;

namespace ManiacEditor.Methods
{
    public static class GameHandler
    {
        // Stuff Used for Command Line Tool to Fix Duplicate Object ID's
        #region DLL Import Stuff
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
        private static extern bool ShowWindow(IntPtr hWnd, ShowWindowEnum flags);

        [DllImport("USER32.DLL")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        private enum ShowWindowEnum
        {
            Hide = 0,
            ShowNormal = 1, ShowMinimized = 2, ShowMaximized = 3,
            Maximize = 3, ShowNormalNoActivate = 4, Show = 5,
            Minimize = 6, ShowMinNoActivate = 7, ShowNoActivate = 8,
            Restore = 9, ShowDefault = 10, ForceMinimized = 11
        };

        public static void ShowConsoleWindow()
        {
            var handle = GetConsoleWindow();

            if (handle == IntPtr.Zero)
            {
                AllocConsole();
            }
            else
            {
                ShowWindow(handle, SW_SHOW);
            }
        }

        public static void HideConsoleWindow()
        {
            var handle = GetConsoleWindow();

            ShowWindow(handle, SW_HIDE);
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool AllocConsole();

        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(int hProcess, int lpBaseAddress, byte[] buffer, int size, int lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool AttachConsole(int dwProcessId);

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;
        #endregion

        public static bool GameRunning = false; //Tells us if the game is running
        public static string GamePath = ""; //Tells us where the game is located
        public static int P1_X = 0;
        public static int P1_Y = 0;
        public static int P2_X = 0;
        public static int P2_Y = 0;
        public static int P3_X = 0;
        public static int P3_Y = 0;
        public static int P4_X = 0;
        public static int P4_Y = 0;
        public static int SP_X = 0;
        public static int SP_Y = 0;
        public static int SelectedPlayer = 0;
        public static bool PlayerSelected = false;
        public static bool CheckpointSelected = false;

        public static IList<string> GameVersion = new List<string> { "1.3", "1.4", "1.6", "N/A" };
        public static string SelectedGameVersion = "1.6";

        public static IList<bool> IsOffset = new List<bool> { false, false, true, false };
        public static IList<int> ObjectStart = new List<int> { 0x00A5DCC0, 0x0086FFA0, 0x0047B010, 0x00 };
        public static IList<int> ObjectSize = new List<int> { 0x458, 0x458, 0x458, 0x00 };

        public static IList<int> CheckpointBase = new List<int> { 0x00, 0x00EBB6C4, 0x00AC672C, 0x00 }; // TODO: Find CheckpointBase for 1.02 (Pre-Plus)
        public static IList<int> PlayerBase = new List<int> { 0x00, 0x85E9A0, 0x00469A10, 0x00 }; //TODO: Find PlayerBase for 1.02 (Pre-Plus)
        public static IList<int> CurrentSceneAddress = new List<int> { 0x00CCF6F8, 0x00E48758, 0x00A535C4, 0x00 };
        public static IList<int> GameStateAddress = new List<int> { 0x00, 0x00E48776, 0x00A535E2, 0x00 }; //TODO: Find GameStateAddress for 1.02 (Pre-Plus)
        public static int Player1Base { get => PlayerBase[GameVersion.IndexOf(SelectedGameVersion)] + (0x458 * 0); } 
        public static int Player2Base { get => PlayerBase[GameVersion.IndexOf(SelectedGameVersion)] + (0x458 * 1); }
        public static int Player3Base { get => PlayerBase[GameVersion.IndexOf(SelectedGameVersion)] + (0x458 * 2); }
        public static int Player4Base { get => PlayerBase[GameVersion.IndexOf(SelectedGameVersion)] + (0x458 * 3); }

        public static IList<int> CurrentScene_ptr = new List<int> { 0x00CCF6F8, 0x00E48758, 0x00A535C4, 0x00 };
        public static IList<int> GameState_ptr = new List<int> { 0x00, 0x00E48776, 0x00A535E2, 0x00 }; //TODO: Find GameState_ptr for 1.02 (Pre-Plus)
        public static IList<int> IsGameRunning_ptr = new List<int> { 0x00628094, 0x0065D1C8, 0x002681D8, 0x00 };
        public static IList<int> Player1_ControllerID_ptr = new List<int> { 0x00A4C860, 0x0085EB44, 0x00469BB4, 0x00 };
        public static IList<int> Player2_ControllerID_ptr = new List<int> { 0x00, 0x0085EF9C, 0x0046A00C, 0x00 }; //TODO: Find Player2_ControllerID_ptr for 1.02 (Pre-Plus)

        public static IList<int> EnableDebugMode = new List<int> { 0x00CCF708, 0x00E48768, 0x00A535D4, 0x00 };
        public static IList<int> EnableDevMenu = new List<int> { 0x00630D74, 0x006F1806, 0x002FC867, 0x00 };
        public static IList<int> DisableBackgroundPausing = new List<int> { 0x005CAD65, 0x005FDD00, 0x001FE5F0, 0x00 };

        public static IList<byte> EnableDebugMode_Values = new List<byte> { 0x01, 0x01, 0x01, 0x00 };
        public static IList<byte> EnableDevMenu_Values = new List<byte> { 0x01, 0x01, 0x01, 0x00 };
        public static IList<byte> DisableBackgroundPausing_Values = new List<byte> { 0xEB, 0xEB, 0xEB, 0x00 };

        private static Controls.Base.MainEditor Editor;

        #region Addresses
        public static short Player1_State { get { return Editor.GameMemory.ReadShort(Player1Base + 0xC0); } set { Editor.GameMemory.WriteShort(Player1Base + 0xC0, value); } }
        public static short Player1_X { get { return Editor.GameMemory.ReadShort(Player1Base + 0x02); } set { Editor.GameMemory.WriteShort(Player1Base + 0x02, value); } }
        public static short Player1_Y { get { return Editor.GameMemory.ReadShort(Player1Base + 0x06); } set { Editor.GameMemory.WriteShort(Player1Base + 0x06, value); } }

        public static short Player4_State { get { return Editor.GameMemory.ReadShort(Player4Base + 0xC0); } set { Editor.GameMemory.WriteShort(Player4Base + 0xC0, value); } }
        public static short Player4_X { get { return Editor.GameMemory.ReadShort(Player4Base + 0x02); } set { Editor.GameMemory.WriteShort(Player4Base + 0x02, value); } }
        public static short Player4_Y { get { return Editor.GameMemory.ReadShort(Player4Base + 0x06); } set { Editor.GameMemory.WriteShort(Player4Base + 0x06, value); } }

        public static short Player3_State { get { return Editor.GameMemory.ReadShort(Player3Base + 0xC0); } set { Editor.GameMemory.WriteShort(Player3Base + 0xC0, value); } }
        public static short Player3_X { get { return Editor.GameMemory.ReadShort(Player3Base + 0x02); } set { Editor.GameMemory.WriteShort(Player3Base + 0x02, value); } }
        public static short Player3_Y { get { return Editor.GameMemory.ReadShort(Player3Base + 0x06); } set { Editor.GameMemory.WriteShort(Player3Base + 0x06, value); } }

        public static short Player2_State { get { return Editor.GameMemory.ReadShort(Player2Base + 0xC0); } set { Editor.GameMemory.WriteShort(Player2Base + 0xC0, value); } }
        public static short Player2_X { get { return Editor.GameMemory.ReadShort(Player2Base + 0x02); } set { Editor.GameMemory.WriteShort(Player2Base + 0x02, value); } }
        public static short Player2_Y { get { return Editor.GameMemory.ReadShort(Player2Base + 0x06); } set { Editor.GameMemory.WriteShort(Player2Base + 0x06, value); } }

        public static byte StarPostEnable { get { return Editor.GameMemory.ReadByte(Editor.GameMemory.ReadInt32(CheckpointBase[GameVersion.IndexOf(SelectedGameVersion)]) + 0x34); } set { Editor.GameMemory.WriteByte(Editor.GameMemory.ReadByte(CheckpointBase[GameVersion.IndexOf(SelectedGameVersion)]) + 0x34, value); } }
        public static int StarPostX { get { return Editor.GameMemory.ReadInt32(Editor.GameMemory.ReadInt32(CheckpointBase[GameVersion.IndexOf(SelectedGameVersion)]) + 0x12); } set { Editor.GameMemory.WriteInt32(Editor.GameMemory.ReadInt32(CheckpointBase[GameVersion.IndexOf(SelectedGameVersion)]) + 0x12, value); } }
        public static int StarPostY { get { return Editor.GameMemory.ReadInt32(Editor.GameMemory.ReadInt32(CheckpointBase[GameVersion.IndexOf(SelectedGameVersion)]) + 0x16); } set { Editor.GameMemory.WriteInt32(Editor.GameMemory.ReadInt32(CheckpointBase[GameVersion.IndexOf(SelectedGameVersion)]) + 0x16, value); } }

        public static byte CurrentScene { get { return Editor.GameMemory.ReadByte(CurrentSceneAddress[GameVersion.IndexOf(SelectedGameVersion)]); } set { Editor.GameMemory.WriteByte(CurrentSceneAddress[GameVersion.IndexOf(SelectedGameVersion)], value); } }
        public static byte GameState { get { return Editor.GameMemory.ReadByte(GameStateAddress[GameVersion.IndexOf(SelectedGameVersion)]); } set { Editor.GameMemory.WriteByte(GameStateAddress[GameVersion.IndexOf(SelectedGameVersion)], value); } }
        #endregion

        public static void UpdateInstance(Controls.Base.MainEditor instance)
        {
            Editor = instance;
        }

        #region In Game Functions
        public static int GetPlayerAt(Point point)
        {
            for (int i = 1; i < 5; i++)
                if (PlayerContainsPoint(point, i))
                    return i;
            return -1;
        }

        public static bool PlayerContainsPoint(Point point, int player)
        {
            return GetDimensions(player).Contains(point);
        }
        public static bool CheckpointEnabled { get { return StarPostEnable >= (byte)65; } }
        public static Rectangle GetDimensions(int player)
        {
            return new Rectangle(GetX(player), GetY(player), 20, 20);
        }
        public static int GetX(int player)
        {
            int X = 0;
            switch (player)
            {
                case 1:
                    X = P1_X;
                    break;
                case 2:
                    X = P2_X;
                    break;
                case 3:
                    X = P3_X;
                    break;
                case 4:
                    X = P4_X;
                    break;
            }
            return X;
        }
        public static int GetY(int player)
        {
            int Y = 0;
            switch (player)
            {
                case 1:
                    Y = P1_Y;
                    break;
                case 2:
                    Y = P2_Y;
                    break;
                case 3:
                    Y = P3_Y;
                    break;
                case 4:
                    Y = P4_Y;
                    break;
            }
            return Y;
        }
        public static void MovePlayer(Point p, double Zoom, int player)
        {
            if (GameRunning)
            {
                // TODO: Find out if this is constent
                switch (player)
                {
                    case 0:
                        Player1_X = (short)(p.X / Zoom);
                        Player1_Y = (short)(p.Y / Zoom);
                        break;
                    case 1:
                        Player2_X = (short)(p.X / Zoom);
                        Player2_Y = (short)(p.Y / Zoom);
                        break;
                    case 2:
                        Player3_X = (short)(p.X / Zoom);
                        Player3_Y = (short)(p.Y / Zoom);
                        break;
                    case 3:
                        Player4_X = (short)(p.X / Zoom);
                        Player4_Y = (short)(p.Y / Zoom);
                        break;
                }

            }
        }
        public static void DrawSinglePlayer(DevicePanel d, int playerID)
        {

            int x = 0;
            int y = 0;
            int ID = 0;
            switch (playerID)
            {
                case 1:
                    P1_X = Player1_X;
                    P1_Y = Player1_Y;
                    x = Player1_X;
                    y = Player1_Y;
                    ID = Player1_State;
                    break;
                case 2:
                    P2_X = Player2_X;
                    P2_Y = Player2_Y;
                    x = Player2_X;
                    y = Player2_Y;
                    ID = Player2_State;
                    break;
                case 3:
                    P3_X = Player3_X;
                    P3_Y = Player3_Y;
                    x = Player3_X;
                    y = Player3_Y;
                    ID = Player3_State;
                    break;
                case 4:
                    P4_X = Player4_X;
                    P4_Y = Player4_Y;
                    x = Player4_X;
                    y = Player4_Y;
                    ID = Player4_State;
                    break;

            }

            if (playerID <= 0 || playerID >= 5) return;

            if (playerID == Classes.Editor.SolutionState.PlayerBeingTracked) Classes.Editor.EditorActions.GoToPosition(x, y);

            int Transparency = 0xff;
            string name = "Player " + playerID;

            System.Drawing.Color color2 = System.Drawing.Color.DarkBlue;
            int frameID = 0;
            bool showFrame = false;

            switch (ID)
            {
                case 1:
                    color2 = System.Drawing.Color.SkyBlue;
                    frameID = 0;
                    break;
                case 2:
                    color2 = System.Drawing.Color.Orange;
                    frameID = 1;
                    break;
                case 4:
                    color2 = System.Drawing.Color.Red;
                    frameID = 2;
                    break;
                case 8:
                    color2 = System.Drawing.Color.Yellow;
                    frameID = 3;
                    break;
                case 16:
                    color2 = System.Drawing.Color.DarkRed;
                    frameID = 4;
                    break;
                default:
                    color2 = System.Drawing.Color.White;
                    frameID = 0;
                    showFrame = true;
                    break;

            }




            if (x != 0 || y != 0)
            {
                if (showFrame)
                {
                    d.DrawRectangle(x, y, x + Classes.Editor.Constants.ENTITY_NAME_BOX_WIDTH, y + Classes.Editor.Constants.ENTITY_NAME_BOX_HEIGHT, System.Drawing.Color.FromArgb(0x00, System.Drawing.Color.MediumPurple));
                    d.DrawLine(x, y, x + Classes.Editor.Constants.ENTITY_NAME_BOX_WIDTH, y, System.Drawing.Color.FromArgb(Transparency, color2));
                    d.DrawLine(x, y, x, y + Classes.Editor.Constants.ENTITY_NAME_BOX_HEIGHT, System.Drawing.Color.FromArgb(Transparency, color2));
                    d.DrawLine(x, y + Classes.Editor.Constants.ENTITY_NAME_BOX_HEIGHT, x + Classes.Editor.Constants.ENTITY_NAME_BOX_WIDTH, y + Classes.Editor.Constants.ENTITY_NAME_BOX_HEIGHT, System.Drawing.Color.FromArgb(Transparency, color2));
                    d.DrawLine(x + Classes.Editor.Constants.ENTITY_NAME_BOX_WIDTH, y, x + Classes.Editor.Constants.ENTITY_NAME_BOX_WIDTH, y + Classes.Editor.Constants.ENTITY_NAME_BOX_HEIGHT, System.Drawing.Color.FromArgb(Transparency, color2));
                    if (Classes.Editor.SolutionState.Zoom >= 1) d.DrawTextSmall(name, x + 2, y + 2, Classes.Editor.Constants.ENTITY_NAME_BOX_WIDTH - 4, System.Drawing.Color.FromArgb(Transparency, System.Drawing.Color.Black), true);
                }
                else
                {
                    var editorAnim = Editor.EntityDrawing.LoadAnimation2("HUD", d, 2, frameID, false, false, false);
                    if (editorAnim != null && editorAnim.Frames.Count != 0 && ID != 0)
                    {
                        var frame = editorAnim.Frames[0];
                        d.DrawBitmap(frame.Texture, x, y, frame.Frame.Width, frame.Frame.Height, false, Transparency);
                    }
                }


            }
        }
        public static void UpdateCheckpoint(Point p, bool enabled = true, bool forceStatus = false)
        {

            byte status = StarPostEnable;
            if (enabled == true)
            {
                status = 0x01;
            }
            StarPostX = p.X;
            StarPostY = p.Y;
            StarPostEnable = status;

        }
        public static void AssetReset()
        {
            int x = Player1_X;
            int y = Player1_Y;

            byte oldScene = CurrentScene;
            CurrentScene = 0x03;
            GameState = 0;

            MessageBox.Show("Click OK to Finish Asset Reset");
            CurrentScene = oldScene;
            GameState = 0;
            UpdateCheckpoint(new Point(x, y), true, true);
        }
        public static void RestartScene()
        {
            GameState = 0;
        }
        public static void DrawCheckpoint(DevicePanel d)
        {
            // TODO: Find out if this is constent
            int x = StarPostX;
            int y = StarPostY;
            bool isEnabled = (StarPostEnable >= 1 ? true : false);


            int Transparency = 0xff;

            System.Drawing.Color color2 = System.Drawing.Color.DarkBlue;
            bool showFrame = false;
            string name = "Checkpoint";




            if (isEnabled)
            {
                if (showFrame)
                {
                    d.DrawRectangle(x, y, x + Classes.Editor.Constants.ENTITY_NAME_BOX_WIDTH, y + Classes.Editor.Constants.ENTITY_NAME_BOX_HEIGHT, System.Drawing.Color.FromArgb(0x00, System.Drawing.Color.MediumPurple));
                    d.DrawLine(x, y, x + Classes.Editor.Constants.ENTITY_NAME_BOX_WIDTH, y, System.Drawing.Color.FromArgb(Transparency, color2));
                    d.DrawLine(x, y, x, y + Classes.Editor.Constants.ENTITY_NAME_BOX_HEIGHT, System.Drawing.Color.FromArgb(Transparency, color2));
                    d.DrawLine(x, y + Classes.Editor.Constants.ENTITY_NAME_BOX_HEIGHT, x + Classes.Editor.Constants.ENTITY_NAME_BOX_WIDTH, y + Classes.Editor.Constants.ENTITY_NAME_BOX_HEIGHT, System.Drawing.Color.FromArgb(Transparency, color2));
                    d.DrawLine(x + Classes.Editor.Constants.ENTITY_NAME_BOX_WIDTH, y, x + Classes.Editor.Constants.ENTITY_NAME_BOX_WIDTH, y + Classes.Editor.Constants.ENTITY_NAME_BOX_HEIGHT, System.Drawing.Color.FromArgb(Transparency, color2));
                    if (Classes.Editor.SolutionState.Zoom >= 1) d.DrawTextSmall(name, x + 2, y + 2, Classes.Editor.Constants.ENTITY_NAME_BOX_WIDTH - 4, System.Drawing.Color.FromArgb(Transparency, System.Drawing.Color.Black), true);
                }
                else
                {
                    var editorAnim = Editor.EntityDrawing.LoadAnimation2("StarPost", d, 1, 0, false, false, false);
                    if (editorAnim != null && editorAnim.Frames.Count != 0)
                    {
                        var frame = editorAnim.Frames[0];
                        d.DrawBitmap(frame.Texture, x, y, frame.Frame.Width, frame.Frame.Height, false, Transparency);
                    }
                }


            }
        }
        public static byte[] ReadMemory(int adress, int processSize, int processHandle)
        {
            byte[] buffer = new byte[processSize];
            Extensions.ExternalExtensions.ReadProcessMemory(processHandle, adress, buffer, processSize, 0);
            return buffer;
        }
        public static void DrawGameElements(DevicePanel d)
        {

            DrawSinglePlayer(d, 1);
            DrawSinglePlayer(d, 2);
            DrawSinglePlayer(d, 3);
            DrawSinglePlayer(d, 4);

            DrawCheckpoint(d);


        }
        #endregion

        #region Game Linking

        public static void RunScene()
        {
            GetManiaWindow();
            /*
            if (sender == Editor.RunSceneButton && Editor.GameRunning)
            {
                Editor.RunSceneDropDown.IsSubmenuOpen = true;
                return;
            }
            */
        }
        private static void GetManiaWindow()
        {
            IntPtr hWnd = FindWindow("SonicMania", null); // this gives you the handle of the window you need.
            Process processes = Process.GetProcessesByName("SonicMania").FirstOrDefault();
            if (processes != null) RestoreManiaWindow(processes);
            else StartSonicMania();
        }
        private static string GetModLoaderPath(string path)
        {
            string ManiaDir = Path.GetDirectoryName(path);
            string WorkingDirectory = "";
            // Check if the mod loader is installed
            string modLoaderDLL = ManiaDir + "//d3d9.dll";
            if (File.Exists(modLoaderDLL))
                WorkingDirectory = ManiaDir;
            else
                WorkingDirectory = Path.GetDirectoryName(Editor.DataDirectory);

            return WorkingDirectory;
        }
        private static void RestoreManiaWindow(Process processes)
        {
            // check if the window is hidden / minimized
            if (processes.MainWindowHandle == IntPtr.Zero) ShowWindow(processes.Handle, ShowWindowEnum.Restore); // the window is hidden so try to restore it before setting focus.
            // set user the focus to the window
            SetForegroundWindow(processes.MainWindowHandle);
            if (!GameRunning) Editor.Dispatcher.Invoke(new Action(() => RunSequence(null, null, true)));
        }
        private static void StartSonicMania()
        {
            if (!GameRunning)
            {
                if (ManiacEditor.Core.Settings.MyDefaults.ModLoaderPath != null && ManiacEditor.Core.Settings.MySettings.ModLoaderConfigs?.Count > 0)
                {
                    string ConfigPath = ManiacEditor.Core.Settings.MyDefaults.SonicManiaPath;
                    System.Windows.Controls.MenuItem dropDownItem = Editor.EditorToolbar.selectConfigToolStripMenuItem.Items[0] as System.Windows.Controls.MenuItem;
                    ConfigPath = ConfigPath.Replace('/', '\\');
                    ConfigPath = ConfigPath.Replace("SonicMania.exe", "//mods//ManiaModLoader.ini");
                    var allItems = Editor.EditorToolbar.selectConfigToolStripMenuItem.Items.Cast<System.Windows.Controls.MenuItem>().ToArray();
                    foreach (System.Windows.Controls.MenuItem item in allItems)
                    {
                        if (item.IsChecked)
                        {
                            dropDownItem = item;
                        }
                    }
                    File.WriteAllText(ConfigPath, dropDownItem.Tag.ToString());
                }
                //TO-DO: Fix No Attach on First Start
                //Editor.Dispatcher.Invoke(new Action(() => RunSequence(null, null)));
                //Work Around Below

                string path = GetSonicManiaPath();
                if (path == "") return;
                ProcessStartInfo psi = new ProcessStartInfo(path);
                psi.WorkingDirectory = GetModLoaderPath(path);
                Process GameProcess;

                GameProcess = Process.Start(psi);

            }
        }

        public static void RunSequence(object sender, EventArgs e, bool attachMode = false)
        {
            //TO-DO: Fix No Attach on First Start
            string path = GetSonicManiaPath();
            if (path == "") return;
            bool GameReady = false;
            ProcessStartInfo psi = new ProcessStartInfo(path);
            psi.WorkingDirectory = GetModLoaderPath(path);
            Process GameProcess;

            if (!attachMode) GameProcess = Process.Start(psi);
            else GameProcess = Process.GetProcessesByName("SonicMania").FirstOrDefault();

            if (GameRunning == false && attachMode)
            {
                GameRunning = true;

                #region Setup Sequence
                new Thread(() =>
                {
                    Editor.Dispatcher.Invoke(new Action(() =>
                    {
                        // Attach and Apply Cheats
                        UseCheatCodes(GameProcess);
                        Methods.Internal.UserInterface.UpdateControls();
                        GameReady = true;

                        // Wait until there is a Running Scene.
                        while (Editor.GameMemory.ReadByte(GameState_ptr[GameVersion.IndexOf(SelectedGameVersion)]) == 0x00) Thread.Sleep(1);

                        // Swap the Scene
                        if (Classes.Editor.SolutionState.LevelID != -1)
                        {
                            Editor.GameMemory.WriteByte(CurrentScene_ptr[GameVersion.IndexOf(SelectedGameVersion)], (byte)Classes.Editor.SolutionState.LevelID);
                            // Restart the Scene
                            Editor.GameMemory.WriteByte(GameState_ptr[GameVersion.IndexOf(SelectedGameVersion)], 0);
                        }




                    }));
                }).Start();
                #endregion

                #region Main Sequence
                new Thread(() =>
                {
                    while (!GameReady)
                        Thread.Sleep(10);
                    /* Level != Main Menu*/
                    while (Editor.GameMemory.ReadByte(CurrentScene_ptr[GameVersion.IndexOf(SelectedGameVersion)]) != 0x02 || ManiacEditor.Core.Settings.MyGameOptions.GameQuitOnMenu == true)
                    {
                        // Check if the user closed the game
                        if (GameProcess.HasExited || !GameRunning)
                        {
                            GameRunning = false;
                            if (Editor.IsVisible)
                            {
                                Editor.Dispatcher.Invoke(new Action(() => Methods.Internal.UserInterface.UpdateControls()));
                            }
                            return;
                        }
                        UseCheatCodes(GameProcess);
                        // Makes sure the process is attached and patches are applied
                        // Set Player 1 Controller Set to 1 (If we set it to AnyController (0x00) we can't use Debug Mode In-Game)
                        if (Editor.GameMemory.ReadByte(Player1_ControllerID_ptr[GameVersion.IndexOf(SelectedGameVersion)]) != 0x01 && ManiacEditor.Core.Settings.MyGameOptions.GameAutoInput == false)
                        {
                            Editor.GameMemory.WriteByte(Player1_ControllerID_ptr[GameVersion.IndexOf(SelectedGameVersion)], 0x01); //setting this to 0x00 causes the inability to use debug mode
                            Editor.GameMemory.WriteByte(Player2_ControllerID_ptr[GameVersion.IndexOf(SelectedGameVersion)], 0xFF);
                        }
                        Thread.Sleep(300);
                    }
                    // User is on the Main Menu
                    // Close the game
                    Editor.GameMemory.WriteByte(IsGameRunning_ptr[GameVersion.IndexOf(SelectedGameVersion)], 0);
                    GameRunning = false;
                    Editor.Dispatcher.Invoke(new Action(() => Methods.Internal.UserInterface.UpdateControls()));
                }).Start();
                #endregion
            }

        }
        private static string GetSonicManiaPath()
        {
            //"steam://run/584400"
            if (string.IsNullOrEmpty(ManiacEditor.Core.Settings.MyDefaults.SonicManiaPath))
            {
                var ofd = new OpenFileDialog
                {
                    Title = "Select SonicMania.exe",
                    Filter = "Windows PE Executable|*.exe"
                };
                if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    ManiacEditor.Core.Settings.MyDefaults.SonicManiaPath = ofd.FileName;
            }
            else
            {
                if (!File.Exists(ManiacEditor.Core.Settings.MyDefaults.SonicManiaPath))
                {
                    ManiacEditor.Core.Settings.MyDefaults.SonicManiaPath = "";
                    return "";
                }
            }
            return ManiacEditor.Core.Settings.MyDefaults.SonicManiaPath;
        }
        public static void UseCheatCodes(Process p)
        {
            Editor.GameMemory.Attach(p);
            if (IsOffset[GameVersion.IndexOf(SelectedGameVersion)])
                Editor.GameMemory.Offset = (int)p.MainModule.BaseAddress;
            else
                Editor.GameMemory.Offset = 0;

            // Mania Plus Patches
            if (ManiacEditor.Core.Settings.MyGameOptions.EnableDebugMode) Editor.GameMemory.WriteByte(EnableDebugMode[GameVersion.IndexOf(SelectedGameVersion)], EnableDebugMode_Values[GameVersion.IndexOf(SelectedGameVersion)]); // Enable Debug
            if (ManiacEditor.Core.Settings.MyGameOptions.EnableDevMode) Editor.GameMemory.WriteByte(EnableDevMenu[GameVersion.IndexOf(SelectedGameVersion)], EnableDevMenu_Values[GameVersion.IndexOf(SelectedGameVersion)]); // Allow DevMenu
            if (ManiacEditor.Core.Settings.MyGameOptions.DisableBackgroundPausing) Editor.GameMemory.WriteByte(DisableBackgroundPausing[GameVersion.IndexOf(SelectedGameVersion)], DisableBackgroundPausing_Values[GameVersion.IndexOf(SelectedGameVersion)]); // Disable Background Pausing
        }


        #endregion

        #region Editor Events

        public static void MoveThePlayerToHere()
        {
            if (GameRunning)
            {
                int ObjectAddress = PlayerBase[GameVersion.IndexOf(SelectedGameVersion)];
                Editor.GameMemory.WriteInt16(ObjectAddress + 2, (short)(Classes.Editor.SolutionState.LastX / Classes.Editor.SolutionState.Zoom));
                Editor.GameMemory.WriteInt16(ObjectAddress + 6, (short)(Classes.Editor.SolutionState.LastY / Classes.Editor.SolutionState.Zoom));
            }
        }

        public static void SetPlayerRespawnToHere()
        {
            Point clicked_point = new Point((int)(Classes.Editor.SolutionState.LastX / Classes.Editor.SolutionState.Zoom), (int)(Classes.Editor.SolutionState.LastY / Classes.Editor.SolutionState.Zoom));
            if (GameRunning)
            {
                UpdateCheckpoint(clicked_point);
            }
        }

        public static void TrackthePlayer(object sender, System.Windows.RoutedEventArgs e)
        {
            System.Windows.Controls.MenuItem item = sender as System.Windows.Controls.MenuItem;
            if (item != null)
            {

                if (!item.IsChecked)
                {
                    UncheckAllPlayers();
                    item.IsChecked = true;
                    int.TryParse(item.Tag.ToString(), out int player);
                    Classes.Editor.SolutionState.PlayerBeingTracked = player;
                }
                else
                {
                    item.IsChecked = false;
                    Classes.Editor.SolutionState.PlayerBeingTracked = -1;
                }


            }
            void UncheckAllPlayers()
            {
                Editor.EditorToolbar.trackP1ToolStripMenuItem.IsChecked = false;
                Editor.EditorToolbar.trackP2ToolStripMenuItem.IsChecked = false;
                Editor.EditorToolbar.trackP3ToolStripMenuItem.IsChecked = false;
                Editor.EditorToolbar.trackP4ToolStripMenuItem.IsChecked = false;
            }
        }

        public static void UpdateRunSceneDropdown()
        {
            Editor.EditorToolbar.trackThePlayerToolStripMenuItem.IsEnabled = GameRunning;
            Editor.EditorToolbar.assetResetToolStripMenuItem1.IsEnabled = GameRunning;
            Editor.moveThePlayerToHereToolStripMenuItem.IsEnabled = GameRunning;
            Editor.EditorToolbar.restartSceneToolStripMenuItem1.IsEnabled = GameRunning;
            Editor.EditorToolbar.selectConfigToolStripMenuItem.IsEnabled = !GameRunning;
        }


        #endregion
    }
}
