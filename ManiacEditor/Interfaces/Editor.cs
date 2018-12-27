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
using ManiacEditor.Actions;
using ManiacEditor.Enums;
using RSDKv5;
using SharpDX.Direct3D9;
using Color = System.Drawing.Color;
using System.Reflection;
using ManiacEditor.Interfaces;
using Cyotek.Windows.Forms;
using Microsoft.Scripting.Utils;
using TileManiac;
using Microsoft.Win32;
using ManiacEditor.Entity_Renders;
using IWshRuntimeLibrary;
using File = System.IO.File;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Net.Sockets;

namespace ManiacEditor
{
    public partial class Editor : Form, IDrawArea
    {
        #region Definitions
        //Editor Editing States
        bool dragged;
        bool startDragged;
        int lastX, lastY, draggedX, draggedY;
        int ShiftX = 0, ShiftY = 0, ScreenWidth, ScreenHeight;
        int CustomX = 0, CustomY = 0;
        public int select_x1, select_x2, select_y1, select_y2;
        int ClickedX = -1, ClickedY = -1;
        bool draggingSelection; //Determines if we are dragging a selection
        int selectingX, selectingY;
        bool zooming; //Detects if we are zooming
        double Zoom = 1; //Double Value for Zoom Levels
        int ZoomLevel = 0; //Interger Value for Zoom Levels
        public int SelectedTilesCount; //Used to get the Amount of Selected Tiles in a Selection
        public int DeselectTilesCount; //Used in combination with SelectedTilesCount to get the definitive amount of Selected Tiles
        internal int SelectedTileX; //Used to get a single Selected Tile's X
        internal int SelectedTileY; //Used to get a single Selected Tile's Y
        bool scrolling = false; //Determines if the User is Scrolling
        bool scrollingDragged = false, wheelClicked = false; //Dermines if the mouse wheel was clicked or is the user is drag-scrolling.
        Point scrollPosition; //For Getting the Scroll Position

        //Editor Toggles
        public bool showTileID; // Show Tile ID Mode Status
        public bool showGrid; // Show Grid Mode Status
        public bool useEncoreColors = false; // Show Encore Color Status
        public bool showCollisionA; //Show Collision Path A Status
        public bool showCollisionB; //Show Collision Path B Status
        public int backupType = 0; //Determines What Kind of Backup to Make
        bool UseMagnetMode = false; // Determines the state of Magnet Mode
        bool useMagnetXAxis = true; //Determines if the Magnet should use the X Axis
        bool useMagnetYAxis = true; //Determines if the Magnet should use the Y Axis
        public bool showEntityPathArrows = true; //Determines if we want to see Object Arrow Paths
        public bool showWaterLevel = false; //Determines if the Water Object should show it's Water Level.
        public bool alwaysShowWaterLevel = false; //Determines if the Water Level Should be Shown at all times regardless of the object being selected
        public bool sizeWaterLevelwithBounds = false; //Determines if the water level width should match those of the object's bounds
        public bool extraLayersMoveToFront = false; //Determines if we should render the extra layers in front of everything on behind everything
        public bool showFlippedTileHelper = false; //Determines if we should Show Flip Assist or Not
        public bool showingDataDirectory = false; //Determines who's turn it is when swaping the label's entry to display ethier the Data Directory and Mod Folder.

        //Editor Status States (Like are we pre-loading a scene)
        public bool importingObjects = false; //Determines if we are importing objects so we can disable all the other Scene Select Options
        public static bool isPreRending = false; //Determines if we are Preloading a Scene
        bool AllowSceneChange = false; // For the Save Warning Dialog
        bool encorePaletteExists = false; // Determines if an Encore Pallete Exists
        int SelectedTileID = -1; //For Tile Maniac Intergration via Right Click in Editor View Panel
        public string CurrentLanguage = "EN"; //Current Selected Language
        Point TempWarpCoords = new Point(0, 0); //Temporary Warp Position for Shortcuts and Force Open
        public bool ForceWarp = false; //For Shortcuts and Force Open.
        public int PlayerBeingTracked = -1;


        //Editor Variable States (Like Scroll Lock is in the X Direction)
        string scrollDirection = "X"; //Determines Scroll Lock Direction
        int magnetSize = 16; //Determines the Magnets Size
        public static int EncoreSetupType; //Used to determine what kind of encore setup the stage uses
        public string ToolbarSelectedTile; //Used to display the selected tile in the tiles toolbar
        internal bool controlWindowOpen; //Used somewhere in the Layer Manager (Unkown)
        public int selectPlayerObject_GoTo = 0; //Used to determine which player object to go to
        public bool cooldownDone = false; // For waiting on functions
        public Color waterColor = new Color(); // The color used for the Water Entity
        public string INILayerNameLower = ""; //Reserved String for INI Default Layer Prefrences
        public string INILayerNameHigher = ""; //Reserved String for INI Default Layer Prefrences
        public string entitiesTextFilter = ""; //Used to hide objects that don't match the discription
        public int entityVisibilityType = 0; // Used to determine how to display entities
        string MenuCharS = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ*+,-./: \'\"";
        public char[] MenuChar;

        //Editor Paths
        public static string DataDirectory; //Used to get the current Data Directory
        public static string MasterDataDirectory = Environment.CurrentDirectory + "\\Data"; //Used as a way of allowing mods to not have to lug all the files in their folder just to load in Maniac.
        public static string ModDataDirectory = ""; //Used as a way of allowing mods to not have to lug all the files in their folder just to load in Maniac.
        public string SelectedZone; //Used to get the Selected zone
        string SelectedScene; //Used to get the Scene zone
        public static string[] EncorePalette = new string[6]; //Used to store the location of the encore palletes
        string SceneFilename = null; //Used for fetching the scene's file name
        public string SceneFilepath = null; //Used for fetching the folder that contains the scene file
        string StageConfigFileName = null; //Used for fetch the scene's stage config file name

        // Extra Layer Buttons
        private IList<ToolStripButton> _extraLayerEditButtons; //Used for Extra Layer Edit Buttons
        private IList<ToolStripButton> _extraLayerViewButtons; //Used for Extra Layer View Buttons
        private IList<ToolStripSeparator> _extraLayerSeperators; //Used for Adding Extra Seperators along side Extra Edit/View Layer Buttons

        // Editor Collections
        public List<string> ObjectList = new List<string>(); //All Gameconfig + Stageconfig Object names (Unused)
        public List<Bitmap> CollisionLayerA = new List<Bitmap>(); //Collection of Collision Type A for the Loaded Scene
        public List<Bitmap> CollisionLayerB = new List<Bitmap>(); //Collection of Collision Type B for the Loaded Scene
        public Stack<IAction> undo = new Stack<IAction>(); //Undo Actions Stack
        public Stack<IAction> redo = new Stack<IAction>(); //Redo Actions Stack
        public List<string> entityRenderingObjects = EditorEntity_ini.GetSpecialRenderList(1); //Used to get the Render List for Objects
        public List<string> renderOnScreenExlusions = EditorEntity_ini.GetSpecialRenderList(0); //Used to get the Always Render List for Objects
        public IList<ToolStripMenuItem> _recentDataItems; //Used to get items for the Data Directory Toolstrip Area
        private IList<ToolStripMenuItem> _recentDataItems_Button; //Used to get items for the Data Directory Button Toolstrip
        public IList<SceneEntity> playerObjectPosition = new List<SceneEntity> { }; //Used to store the scenes current playerObjectPositions
        public List<string> userDefinedSpritePaths = new List<string>();
        public Dictionary<string, string> userDefinedEntityRenderSwaps = new Dictionary<string, string>();

        //Used for Get Common Layers
        internal EditorLayer FGHigher => EditorScene?.HighDetails;
        internal EditorLayer FGHigh => EditorScene?.ForegroundHigh;
        internal EditorLayer FGLow => EditorScene?.ForegroundLow;
        internal EditorLayer FGLower => EditorScene?.LowDetails;
        internal EditorLayer ScratchLayer => EditorScene?.Scratch;

        //internal IEnumerable<EditorLayer> AllLayers => EditorScene?.AllLayers;
        //Used to Get the Maximum Layer Height and Width
        internal int SceneWidth => EditorScene.Layers.Max(sl => sl.Width) * 16;
        internal int SceneHeight => EditorScene.Layers.Max(sl => sl.Height) * 16;

        //Used for "Run Scene"
        public static ProcessMemory GameMemory = new ProcessMemory(); //Allows us to write hex codes like cheats, etc.
        public static bool GameRunning = false; //Tells us if the game is running
        public static string GamePath = ""; //Tells us where the game is located
        public int P1_X = 0;
        public int P1_Y = 0;
        public int P2_X = 0;
        public int P2_Y = 0;
        public int P3_X = 0;
        public int P3_Y = 0;
        public int P4_X = 0;
        public int P4_Y = 0;
        public int SP_X = 0;
        public int SP_Y = 0;
        public int selectedPlayer = 0;
        public bool playerSelected = false;
        public bool checkpointSelected = false;

        //Used to store information to Clipboards
        public Dictionary<Point, ushort> TilesClipboard;
        public Dictionary<Point, ushort> FindReplaceClipboard;
        public Dictionary<Point, ushort> TilesClipboardEditable;
        private List<EditorEntity> entitiesClipboard;

        //Used For Discord Rich Pressence
        public SharpPresence.Discord.RichPresence RPCcontrol = new SharpPresence.Discord.RichPresence();
        public SharpPresence.Discord.EventHandlers RPCEventHandler = new SharpPresence.Discord.EventHandlers();
        public string ScenePath = "";

        //Collision Colours
        public Color CollisionAllSolid = Color.FromArgb(255, 255, 255, 255);
        public Color CollisionTopOnlySolid = Color.FromArgb(255, 255, 255, 255);
        public Color CollisionLRDSolid = Color.FromArgb(255, 255, 255, 0);
        public int collisionPreset = 0; //For Collision Presets

        //Internal/Public/Vital Classes
        public StageTiles StageTiles;
        public EditorScene EditorScene;
        public StageConfig StageConfig;
        public ObjectManager objectRemover;
        public ConfigManager configLists;
        public GameConfig GameConfig;
        public EditorControls EditorControls;
        public static EditorEntities entities;
        public static Editor Instance; //Used the access this class easier
        internal EditorBackground Background;
        internal EditorLayer EditLayer;
        internal TilesToolbar TilesToolbar = null;
        internal ChunksToolbar ChunksToolbar = null;
        private EntitiesToolbar entitiesToolbar = null;
        public EditorUpdater Updater = new EditorUpdater();
        public TilesConfig TilesConfig;
        public EditorInGame EditorGame = new EditorInGame();

        //Tile Maniac Instance
        public TileManiac.Mainform mainform = new Mainform();

        //Editor Misc. Variables
        System.Windows.Forms.Timer t;

        //Dark Theme
        public Color darkTheme1 = Color.FromArgb(255, 50, 50, 50);
        public Color darkTheme2 = Color.FromArgb(255, 70, 70, 70);
        public Color darkTheme3 = Color.White;

        //Shorthanding Setting Files
        Properties.Settings mySettings = Properties.Settings.Default;
        Properties.EditorState myEditorState = Properties.EditorState.Default;
        Properties.KeyBinds myKeyBinds = Properties.KeyBinds.Default;


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

        #endregion
        public Editor(string dataDir = "", string scenePath = "", string modPath = "", int levelID = 0, bool shortcutLaunch = false, int shortcutLaunchMode = 0, bool isEncoreMode = false, int X = 0, int Y = 0)
        {
            SystemEvents.PowerModeChanged += CheckDeviceState;

            Instance = this;
            UseDarkTheme(mySettings.NightMode);
            InitializeComponent();
            SetupButtonColors();
            AllocConsole();
            HideConsoleWindow();
            try
            {
                InitDiscord();
            }
            catch
            {

            }


            RefreshCollisionColours();

            this.Text = String.Format("Maniac Editor - Generations Edition {0}", Updater.GetVersion());
            this.splitContainer1.Panel2MinSize = 254;

            GraphicPanel.Width = SystemInformation.PrimaryMonitorSize.Width;
            GraphicPanel.Height = SystemInformation.PrimaryMonitorSize.Height;

            _extraLayerEditButtons = new List<ToolStripButton>();
            _extraLayerViewButtons = new List<ToolStripButton>();
            _extraLayerSeperators = new List<ToolStripSeparator>();
            _recentDataItems = new List<ToolStripMenuItem>();
            _recentDataItems_Button = new List<ToolStripMenuItem>();
            EditorControls = new EditorControls();
            MenuChar = MenuCharS.ToCharArray();

            SetViewSize();


            UpdateControls();

            TryLoadSettings();

            if (mySettings.UseForcefulStartup)
            {
                OpenSceneForceFully();
            }
            if (shortcutLaunch)
            {
                try
                {
                    if (dataDir != "")
                    {
                        if (scenePath != "")
                        {
                            OpenSceneForceFully(dataDir, scenePath, modPath, levelID, isEncoreMode, X, Y);
                        }
                        else
                        {
                            OpenSceneForceFully(dataDir);
                        }
                    }
                }
                catch
                {
                    Debug.Print("Couldn't Force Open!");
                }

            }

            if (!Updater.GetVersion().Contains("DEV") && mySettings.checkForUpdatesAuto)
            {
                Updater.CheckforUpdates();
            }

        }

        #region Discord Rich Presence

        public void InitDiscord()
        {
            if (!Environment.Is64BitProcess)
            {
                SharpPresence.Discord.Initialize("484279851830870026", RPCEventHandler);

                if (mySettings.ShowDiscordRPC)
                {
                    RPCcontrol.state = "Maniac Editor";
                    RPCcontrol.details = "Idle";

                    RPCcontrol.largeImageKey = "maniac";
                    RPCcontrol.largeImageText = "maniac-small";

                    TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
                    int secondsSinceEpoch = (int)t.TotalSeconds;

                    RPCcontrol.startTimestamp = secondsSinceEpoch;

                    SharpPresence.Discord.RunCallbacks();
                    SharpPresence.Discord.UpdatePresence(RPCcontrol);
                }
                else
                {
                    RPCcontrol.state = "Maniac Editor";
                    RPCcontrol.details = "";

                    RPCcontrol.largeImageKey = "maniac";
                    RPCcontrol.largeImageText = "Maniac Editor";

                    TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
                    int secondsSinceEpoch = (int)t.TotalSeconds;

                    RPCcontrol.startTimestamp = secondsSinceEpoch;

                    SharpPresence.Discord.RunCallbacks();
                    SharpPresence.Discord.UpdatePresence(RPCcontrol);
                }
            }

        }

        public void UpdateDiscord(string Details = null)
        {
            if (!Environment.Is64BitProcess)
            {
                try
                {
                    if (mySettings.ShowDiscordRPC)
                    {
                        SharpPresence.Discord.RunCallbacks();
                        if (Details != null)
                        {
                            RPCcontrol.details = Details;
                        }
                        else
                        {
                            RPCcontrol.details = "Idle";
                        }
                        SharpPresence.Discord.UpdatePresence(RPCcontrol);
                    }
                    else
                    {
                        RPCcontrol.state = "Maniac Editor";
                        RPCcontrol.details = "";

                        RPCcontrol.largeImageKey = "maniac";
                        RPCcontrol.largeImageText = "Maniac Editor";

                        SharpPresence.Discord.RunCallbacks();
                        SharpPresence.Discord.UpdatePresence(RPCcontrol);
                    }
                }
                catch
                {

                }
            }
        }

        public void DisposeDiscord()
        {
            RPCcontrol.startTimestamp = 0;
            SharpPresence.Discord.Shutdown();
        }
        #endregion

        #region Defaults and Presets
        /// <summary>
        /// Try to load settings from the Application Settings file(s).
        /// This includes User specific settings.
        /// </summary>
        private void TryLoadSettings()
        {
            try
            {
                if (mySettings.UpgradeRequired)
                {
                    mySettings.Upgrade();
                    mySettings.UpgradeRequired = false;
                    mySettings.Save();
                }

                WindowState = mySettings.IsMaximized ? FormWindowState.Maximized : WindowState;
                GamePath = mySettings.GamePath;
                ApplyDefaults();

                if (mySettings.DataDirectories?.Count > 0)
                {
                    RefreshDataDirectories(mySettings.DataDirectories);
                }

                if (mySettings.modConfigs?.Count > 0)
                {
                    selectConfigToolStripMenuItem.DropDownItems.Clear();
                    for (int i = 0; i < mySettings.modConfigs.Count; i++)
                    {
                        selectConfigToolStripMenuItem.DropDownItems.Add(CreateModConfigMenuItem(i));
                        
                    }
                }
                //mySettings.LastModConfig
                

            }
            catch (Exception ex)
            {
                Debug.Write("Failed to load settings: " + ex);
            }
        }

        private void ApplyDefaults()
        {
            // These Prefrences are applied on Editor Load
            editEntitesTransparencyToolStripMenuItem.Checked = myEditorState.editEntitiesTransparency;
            transparentLayersForEditingEntitiesToolStripMenuItem.Checked = myEditorState.editEntitiesTransparency;

            mySettings.scrollLock = mySettings.ScrollLockDefault;
            statusNAToolStripMenuItem.Checked = mySettings.ScrollLockDefault;
            scrollLockButton.Checked = mySettings.ScrollLockDefault;


            xToolStripMenuItem.Checked = mySettings.ScrollLockDirection;
            yToolStripMenuItem.Checked = !mySettings.ScrollLockDirection;

            pixelModeButton.Checked = mySettings.EnablePixelModeDefault;
            pixelModeToolStripMenuItem.Checked = mySettings.EnablePixelModeDefault;
            mySettings.pixelCountMode = mySettings.EnablePixelModeDefault;

            showEntityPathArrowsToolstripItem.Checked = mySettings.ShowEntityArrowPathsDefault;
            showEntityPathArrows = mySettings.ShowEntityArrowPathsDefault;

            showWaterLevelToolStripMenuItem.Checked = mySettings.showWaterLevelDefault;
            showWaterLevel = mySettings.showWaterLevelDefault;
            alwaysShowWaterLevel = mySettings.AlwaysShowWaterLevelDefault;
            sizeWaterLevelwithBounds = mySettings.SizeWaterLevelWithBoundsDefault;
            waterLevelAlwaysShowItem.Checked = mySettings.AlwaysShowWaterLevelDefault;
            sizeWithBoundsWhenNotSelectedToolStripMenuItem.Checked = mySettings.SizeWaterLevelWithBoundsDefault;

            showParallaxSpritesToolStripMenuItem.Checked = mySettings.ShowFullParallaxEntityRenderDefault;
            myEditorState.ShowParallaxSprites = mySettings.ShowFullParallaxEntityRenderDefault;
            foreach (ToolStripMenuItem item in menuLanguageToolStripMenuItem.DropDownItems) if (item.Tag.ToString() == mySettings.LangDefault)
                {
                    item.Checked = true;
                    CurrentLanguage = item.Tag.ToString();
                }
        }
        void UseDefaultPrefrences()
        {
            //These Prefrences are applied on Stage Load

            //Default Layer Visibility Preferences
            if (!mySettings.FGLowerDefault) ShowFGLower.Checked = false;            
            else ShowFGLower.Checked = true;
            if (!mySettings.FGLowDefault) ShowFGLow.Checked = false;
            else ShowFGLow.Checked = true;
            if (!mySettings.FGHighDefault) ShowFGHigh.Checked = false;
            else ShowFGHigh.Checked = true;
            if (!mySettings.FGHigherDefault) ShowFGHigher.Checked = false;
            else ShowFGHigher.Checked = true;
            if (!mySettings.EntitiesDefault) ShowEntities.Checked = false;
            else ShowEntities.Checked = true;           
            if (!mySettings.AnimationsDefault) ShowAnimations.Checked = false;       
            else ShowAnimations.Checked = true;

            //Default Enabled Annimation Preferences
            movingPlatformsObjectsToolStripMenuItem.Checked = mySettings.MovingPlatformsDefault;
            myEditorState.movingPlatformsChecked = mySettings.MovingPlatformsDefault;

            spriteFramesToolStripMenuItem.Checked = mySettings.AnimatedSpritesDefault;
            myEditorState.annimationsChecked = mySettings.AnimatedSpritesDefault;

            waterColor = mySettings.WaterColorDefault;


            //Default Grid Preferences
            if (!mySettings.x16Default) x16ToolStripMenuItem.Checked = false;
            else x16ToolStripMenuItem.Checked = true;
            if (!mySettings.x128Default) x128ToolStripMenuItem.Checked = false;
            else x128ToolStripMenuItem.Checked = true;
            if (!mySettings.x256Default) x256ToolStripMenuItem.Checked = false;
            else x256ToolStripMenuItem.Checked = true;
            if (!mySettings.CustomGridDefault) customToolStripMenuItem.Checked = false;
            else customToolStripMenuItem.Checked = true;

            //Collision Color Presets
            defaultToolStripMenuItem.Checked = mySettings.CollisionColorsDefault == 0;
            invertedToolStripMenuItem.Checked = mySettings.CollisionColorsDefault == 1;
            customToolStripMenuItem1.Checked = mySettings.CollisionColorsDefault == 2;
            collisionPreset = mySettings.CollisionColorsDefault;
            RefreshCollisionColours();

        }
        void SetINIDefaultPrefrences()
        {
            string value;
            Dictionary<String,String> ListedPrefrences = SettingsReader.ReturnPrefrences();
            if (ListedPrefrences.ContainsKey("LevelID"))
            {   
                ListedPrefrences.TryGetValue("LevelID", out value);
                Int32.TryParse(value, out int resultingInt);
                if (resultingInt >= -1)
                {
                    myEditorState.Level_ID = resultingInt;
                }

            }
            if (ListedPrefrences.ContainsKey("FGLower"))
            {
                ListedPrefrences.TryGetValue("FGLower", out value);
                INILayerNameLower = value;
            }
            if (ListedPrefrences.ContainsKey("FGHigher"))
            {
                ListedPrefrences.TryGetValue("FGHigher", out value);
                INILayerNameHigher = value;
            }
            if (ListedPrefrences.ContainsKey("WaterColor"))
            {
                ListedPrefrences.TryGetValue("WaterColor", out value);
                Color color = System.Drawing.ColorTranslator.FromHtml(value);
                
                if (ListedPrefrences.ContainsKey("WaterColorAlpha"))
                {
                    ListedPrefrences.TryGetValue("WaterColorAlpha", out string value2);
                    Int32.TryParse(value2, out int alpha);
                    color = Color.FromArgb(alpha, color.R, color.G, color.B);
                }
                waterColor = color;
            }
            if (ListedPrefrences.ContainsKey("SpritePaths"))
            {
                ListedPrefrences.TryGetValue("SpritePaths", out value);
                List<string> list = new List<string>(value.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries));
                foreach (string item in list)
                {
                    MessageBox.Show(item);
                }
                userDefinedSpritePaths = list;
            }
            if (ListedPrefrences.ContainsKey("SwapEntityRenderNames"))
            {
                ListedPrefrences.TryGetValue("SwapEntityRenderNames", out value);
                List<string> list = new List<string>(value.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries));
                if (list.Count % 2 == 0 && list.Count != 0)
                {
                    for (int i = 0; i < list.Count;)
                    {
                        string toBeSwapped = list[i];
                        string toSet = list[i+1];
                        MessageBox.Show(toBeSwapped + "-> " + toSet);
                        userDefinedEntityRenderSwaps.Add(toBeSwapped, toSet);
                        i = i + 2;
                    }
                }
                else
                {
                    MessageBox.Show("There is an odd number of swaps for entity names, please double check your maniac.ini file");
                }


            }
            if (ListedPrefrences.ContainsKey("EncoreACTFile"))
            {
                ListedPrefrences.TryGetValue("EncoreACTFile", out value);
                value = value.Replace("\"", "");
                SetEncorePalete(null, value);
            }
            if (ListedPrefrences.ContainsKey("CustomMenuFontText"))
            {
                ListedPrefrences.TryGetValue("CustomMenuFontText", out value);
                MenuChar = value.ToCharArray();
            }


        }
        #endregion

        #region Mod Config List Stuff
        private ToolStripMenuItem CreateModConfigMenuItem(int i)
        {
            ToolStripMenuItem newItem = new ToolStripMenuItem(mySettings.modConfigsNames[i])
            {
                Tag = mySettings.modConfigs[i]
            };
            newItem.Click += ModConfigItemClicked;
            if (newItem.Tag.ToString() == mySettings.LastModConfig) newItem.Checked = true;
            return newItem;
        }

        private void ModConfigItemClicked(object sender, EventArgs e)
        {
            var modConfig_CheckedItem = (sender as ToolStripMenuItem);
            SelectConfigToolStripMenuItem_Click(modConfig_CheckedItem);
            mySettings.LastModConfig = modConfig_CheckedItem.Tag.ToString();
        }

        public void EditConfigsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigManager configManager = new ConfigManager();
            configManager.ShowDialog();

            // TODO: Fix NullReferenceException on mySettings.modConfigs
            selectConfigToolStripMenuItem.DropDownItems.Clear();
            for (int i = 0; i < mySettings.modConfigs.Count; i++)
            {
                selectConfigToolStripMenuItem.DropDownItems.Add(CreateModConfigMenuItem(i));
            }
        }

        private void SelectConfigToolStripMenuItem_Click(ToolStripMenuItem modConfig_CheckedItem)
        {
            foreach (ToolStripMenuItem item in selectConfigToolStripMenuItem.DropDownItems)
            {
                item.Checked = false;
            }
            modConfig_CheckedItem.Checked = true;

        }

        #endregion

        #region Boolean States

        public bool IsEditing()
        {
            return IsTilesEdit() || IsEntitiesEdit();
        }

        public bool IsSceneLoaded()
        {
            if (EditorScene != null)
                return true;
            else
                return false;
        }

        public bool IsTilesEdit()
        {
            return EditLayer != null;
        }

        public bool IsChunksEdit()
        {
            return ChunksToolButton.Checked;
        }

        public bool IsEntitiesEdit()
        {
            return EditEntities.Checked;
        }

        public bool IsSelected()
        {
            if (IsTilesEdit())
            {
                return EditLayer.SelectedTiles.Count > 0 || EditLayer.TempSelectionTiles.Count > 0;
            }
            else if (IsEntitiesEdit())
            {
                return entities.IsSelected();
            }
            return false;
        }

        private bool CtrlPressed()
        {
            return ModifierKeys.HasFlag(Keys.Control);
        }

        private bool ShiftPressed()
        {
            return ModifierKeys.HasFlag(Keys.Alt);
        }

        public bool IsTileUnused(int tile)
        {
            List<ushort> listValue = new List<ushort> { };
            List<Point> listPoint = new List<Point> { };
            List<Point> listLocations = new List<Point> { };
            IEnumerable<EditorLayer> AllLayers = EditorScene.AllLayers;

            foreach (var editorLayer in EditorScene.AllLayers)
            {
                EditLayer = editorLayer;
                EditLayer.Select(new Rectangle(0, 0, 32768, 32768), true, false);
                UpdateEditLayerActions();
                Dictionary<Point, ushort> copyData = EditLayer.CopyToClipboard(true);
                FindReplaceClipboard = copyData;


                if (FindReplaceClipboard != null)
                {
                    foreach (var item in FindReplaceClipboard)
                    {
                        listPoint.Add(item.Key);
                    }
                    foreach (var item in FindReplaceClipboard)
                    {
                        listValue.Add(item.Value);
                    }
                    for (int i = 0; i < listValue.Count; i++)
                    {
                        if ((listValue[i] & 0x3ff) == (ushort)(tile & 0x3ff))
                        {
                            listLocations.Add(listPoint[i]);
                        }
                    }
                    FindReplaceClipboard.Clear();
                    Deselect();
                }

            }

            if (listLocations.Count != 0)
            {
                cooldownDone = true;
                return false;
            }
            else
            {
                cooldownDone = true;
                return true;
            }

        }

        #endregion

        #region Enable And Disable Editor Buttons

        private void SetSceneOnlyButtonsState(bool enabled, bool stageLoad = false)
        {
            saveToolStripMenuItem.Enabled = enabled;
            saveAsToolStripMenuItem.Enabled = enabled;
            backupToolStripMenuItem.Enabled = enabled;
            unloadSceneToolStripMenuItem.Enabled = enabled;
            goToToolStripMenuItem1.Enabled = enabled;
            specificPlaceToolStripMenuItem.Enabled = enabled;
            playerSpawnToolStripMenuItem.Enabled = enabled;

            ShowFGHigh.Enabled = enabled && FGHigh != null;
            ShowFGLow.Enabled = enabled && FGLow != null;
            ShowFGHigher.Enabled = enabled && FGHigher != null;
            ShowFGLower.Enabled = enabled && FGLower != null;
            ShowEntities.Enabled = enabled;
            ShowAnimations.Enabled = enabled;
            animationsSplitButton.Enabled = enabled;
            ReloadButton.Enabled = enabled;
            PreloadSceneButton.Enabled = enabled;
            newShortcutToolStripMenuItem.Enabled = Directory.Exists(DataDirectory);
            withoutCurrentCoordinatesToolStripMenuItem.Enabled = EditorScene != null;
            withCurrentCoordinatesToolStripMenuItem.Enabled = EditorScene != null;
            changeEncorePaleteToolStripMenuItem.Enabled = enabled;

            Save.Enabled = enabled;

            if (mySettings.ReduceZoom)
            {
                ZoomInButton.Enabled = enabled && ZoomLevel < 5;
                ZoomOutButton.Enabled = enabled && ZoomLevel > -2;
            }
            else
            {
                ZoomInButton.Enabled = enabled && ZoomLevel < 5;
                ZoomOutButton.Enabled = enabled && ZoomLevel > -5;
            }



            RunSceneButton.Enabled = enabled;

            SetEditButtonsState(enabled);
            UpdateTooltips();

            if (mySettings.preRenderSceneOption == 3 && enabled && stageLoad)
            {
                PreLoadSceneButton_Click(null, null);
            }
            else if (mySettings.preRenderSceneOption == 2 && enabled && stageLoad)
            {
                DialogResult result = MessageBox.Show("Do you wish to Pre-Render this scene?", "Requesting to Pre-Render the Scene", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (result == DialogResult.Yes)
                {
                    PreLoadSceneButton_Click(null, null);
                }
            }
            else if (mySettings.preRenderSceneOption == 1 && Properties.EditorState.Default.preRenderSceneSelectCheckbox && enabled && stageLoad)
            {
                PreLoadSceneButton_Click(null, null);
            }
        }

        private void SetSelectOnlyButtonsState(bool enabled = true)
        {
            enabled &= IsSelected();
            deleteToolStripMenuItem.Enabled = enabled;
            copyToolStripMenuItem.Enabled = enabled;
            cutToolStripMenuItem.Enabled = enabled;
            duplicateToolStripMenuItem.Enabled = enabled;

            flipHorizontalToolStripMenuItem.Enabled = enabled && IsTilesEdit();
            flipVerticalToolStripMenuItem.Enabled = enabled && IsTilesEdit();
            flipHorizontalIndvidualToolStripMenuItem.Enabled = enabled && IsTilesEdit();
            flipVerticalIndvidualToolStripMenuItem.Enabled = enabled && IsTilesEdit();

            selectAllToolStripMenuItem.Enabled = IsTilesEdit() || IsEntitiesEdit();

            if (IsEntitiesEdit())
            {
                entitiesToolbar.SelectedEntities = entities.SelectedEntities.Select(x => x.Entity).ToList();
            }
        }

        private void SetEditButtonsState(bool enabled)
        {
            bool windowsClipboardState;
            bool windowsEntityClipboardState;
            EditFGLow.Enabled = enabled && FGLow != null;
            EditFGHigh.Enabled = enabled && FGHigh != null;
            EditFGLower.Enabled = enabled && FGLower != null;
            EditFGHigher.Enabled = enabled && FGHigher != null;
            EditEntities.Enabled = enabled;
            importObjectsToolStripMenuItem.Enabled = enabled && StageConfig != null;
            entityManagerToolStripMenuItem.Enabled = enabled && StageConfig != null;
            importSoundsToolStripMenuItem.Enabled = enabled && StageConfig != null;
            layerManagerToolStripMenuItem.Enabled = enabled;
            editBackgroundColorsToolStripMenuItem.Enabled = enabled;
            preRenderSceneToolStripMenuItem.Enabled = enabled;

            editEntitiesOptionToolStrip.Enabled = enabled;

            if (enabled && EditFGLow.Checked) EditLayer = FGLow;
            else if (enabled && EditFGHigh.Checked) EditLayer = FGHigh;
            else if (enabled && EditFGHigher.Checked) EditLayer = FGHigher;
            else if (enabled && EditFGLower.Checked) EditLayer = FGLower;
            else if (enabled && _extraLayerEditButtons.Any(elb => elb.Checked))
            {
                var selectedExtraLayerButton = _extraLayerEditButtons.Single(elb => elb.Checked);
                var editorLayer = EditorScene.OtherLayers.Single(el => el.Name.Equals(selectedExtraLayerButton.Text));

                EditLayer = editorLayer;
            }
            /*
            else if (enabled && _extraLayerViewButtons.Any(elb => elb.Checked))
            {
                var selectedExtraLayerButton = _extraLayerViewButtons.Single(elb => elb.Checked);
                var editorLayer = EditorScene.OtherLayers.Single(el => el.Name.Equals(selectedExtraLayerButton.Text));

                EditLayer = editorLayer;
            }
            */
            else EditLayer = null;

            undoToolStripMenuItem.Enabled = enabled && undo.Count > 0;
            redoToolStripMenuItem.Enabled = enabled && redo.Count > 0;

            MagnetMode.Enabled = enabled && IsEntitiesEdit();
            MagnetMode.Checked = UseMagnetMode && IsEntitiesEdit();
            MagnetModeSplitButton.Enabled = enabled && IsEntitiesEdit();
            UseMagnetMode = IsEntitiesEdit() && MagnetMode.Checked;

            

            UndoButton.Enabled = enabled && undo.Count > 0;
            RedoButton.Enabled = enabled && redo.Count > 0;

            findAndReplaceToolStripMenuItem.Enabled = enabled && EditLayer != null;

            PointerButton.Enabled = enabled && IsTilesEdit();
            SelectToolButton.Enabled = enabled && IsTilesEdit();
            PlaceTilesButton.Enabled = enabled && IsTilesEdit();
            InteractionToolButton.Enabled = enabled;
            ChunksToolButton.Enabled = enabled && IsTilesEdit();

            ShowGridButton.Enabled = enabled && StageConfig != null;
            ShowCollisionAButton.Enabled = enabled && StageConfig != null;
            ShowCollisionBButton.Enabled = enabled && StageConfig != null;
            ShowTileIDButton.Enabled = enabled && StageConfig != null;
            GridSizeButton.Enabled = enabled && StageConfig != null;
            EncorePaletteButton.Enabled = enabled && encorePaletteExists;



            //Doing this too often seems to cause a lot of grief for the app, should be relocated and stored as a bool
            try
            {
                windowsClipboardState = Clipboard.ContainsData("ManiacTiles");
                windowsEntityClipboardState = Clipboard.ContainsData("ManiacEntities");
            }
            catch
            {
                windowsClipboardState = false;
                windowsEntityClipboardState = false;
            }



            if (enabled && (IsTilesEdit() || ((TilesClipboard != null || windowsClipboardState))))
                pasteToolStripMenuItem.Enabled = true;
            else
                pasteToolStripMenuItem.Enabled = false;

            if (enabled && (IsEntitiesEdit() || ((entitiesClipboard != null || windowsEntityClipboardState))))
                pasteToolStripMenuItem.Enabled = true;
            else
                pasteToolStripMenuItem.Enabled = false;


            if (IsTilesEdit() && !IsChunksEdit())
            {
                if (TilesToolbar == null)
                {
                    if (useEncoreColors)
                        TilesToolbar = new TilesToolbar(StageTiles, SceneFilepath, EncorePalette[0]);
                    else
                        TilesToolbar = new TilesToolbar(StageTiles, SceneFilepath, null);

                    TilesToolbar.TileDoubleClick = new Action<int>(x =>
                    {
                        EditorPlaceTile(new Point((int)(ShiftX / Zoom) + EditorLayer.TILE_SIZE - 1, (int)(ShiftY / Zoom) + EditorLayer.TILE_SIZE - 1), x);
                    });
                    TilesToolbar.TileOptionChanged = new Action<int, bool>((option, state) =>
                   {
                       EditLayer.SetPropertySelected(option + 12, state);

                   });
                    splitContainer1.Panel2.Controls.Clear();
                    splitContainer1.Panel2.Controls.Add(TilesToolbar);
                    splitContainer1.Panel2Collapsed = false;
                    TilesToolbar.Width = splitContainer1.Panel2.Width - 2;
                    TilesToolbar.Height = splitContainer1.Panel2.Height - 2;
                    Form1_Resize(null, null);
                }
                UpdateTilesOptions();
                TilesToolbar.ShowShortcuts = PlaceTilesButton.Checked;
            }
            else
            {
                TilesToolbar?.Dispose();
                TilesToolbar = null;
            }
            if (IsTilesEdit() && IsChunksEdit())
            {
                if (ChunksToolbar == null)
                {
                    if (useEncoreColors)
                        ChunksToolbar = new ChunksToolbar(StageTiles, SceneFilepath, EncorePalette[0]);
                    else
                        ChunksToolbar = new ChunksToolbar(StageTiles, SceneFilepath, null);

                    splitContainer1.Panel2.Controls.Clear();
                    splitContainer1.Panel2.Controls.Add(ChunksToolbar);
                    splitContainer1.Panel2Collapsed = false;
                    ChunksToolbar.Width = splitContainer1.Panel2.Width - 2;
                    ChunksToolbar.Height = splitContainer1.Panel2.Height - 2;
                    Form1_Resize(null, null);
                }
                //UpdateChunkOptions();
                //ChunksToolbar.ShowShortcuts = chunksToolStripButton.Checked;
            }
            else
            {
                ChunksToolbar?.Dispose();
                ChunksToolbar = null;
            }
            if (IsEntitiesEdit())
            {
                if (entitiesToolbar == null)
                {
                    entitiesToolbar = new EntitiesToolbar(EditorScene.Objects)
                    {
                        //entitiesToolbar = new EntitiesToolbar(ObjectList);
                        SelectedEntity = new Action<int>(x =>
                        {
                            entities.SelectSlot(x);
                            SetSelectOnlyButtonsState();
                        }),
                        AddAction = new Action<IAction>(x =>
                        {
                            undo.Push(x);
                            redo.Clear();
                            UpdateControls();
                        }),
                        Spawn = new Action<SceneObject>(x =>
                        {
                            entities.Add(x, new Position((short)(ShiftX / Zoom), (short)(ShiftY / Zoom)));
                            undo.Push(entities.LastAction);
                            redo.Clear();
                            UpdateControls();
                        })
                    };
                    splitContainer1.Panel2.Controls.Clear();
                    splitContainer1.Panel2.Controls.Add(entitiesToolbar);
                    splitContainer1.Panel2Collapsed = false;
                    entitiesToolbar.Width = splitContainer1.Panel2.Width - 2;
                    entitiesToolbar.Height = splitContainer1.Panel2.Height - 2;
                    Form1_Resize(null, null);
                }
                UpdateEntitiesToolbarList();
                entitiesToolbar.SelectedEntities = entities.SelectedEntities.Select(x => x.Entity).ToList();
            }
            else
            {
                entitiesToolbar?.Dispose();
                entitiesToolbar = null;
            }
            if (TilesToolbar == null && entitiesToolbar == null && ChunksToolbar == null)
            {
                splitContainer1.Panel2Collapsed = true;
                Form1_Resize(null, null);
            }

            SetSelectOnlyButtonsState(enabled);
        }

        private void UpdateControls(bool stageLoad = false)
        {
            if (mySettings.EntityFreeCam)
            {
                vScrollBar1.Enabled = false;
                hScrollBar1.Enabled = false;
            }
            else
            {
                vScrollBar1.Enabled = true;
                hScrollBar1.Enabled = true;
            }
            SetSceneOnlyButtonsState(EditorScene != null, stageLoad);
        }

        #endregion

        #region Refresh UI

        private void UpdateEntitiesToolbarList()
        {
            entitiesToolbar.Entities = entities.Entities.Select(x => x.Entity).ToList();
        }

        private void UpdateTilesOptions()
        {
            if (IsTilesEdit() && !IsChunksEdit())
            {
                List<ushort> values = EditLayer.GetSelectedValues();

                if (values.Count > 0)
                {
                    for (int i = 0; i < 4; ++i)
                    {
                        bool set = ((values[0] & (1 << (i + 12))) != 0);
                        bool unk = false;
                        foreach (ushort value in values)
                        {
                            if (set != ((value & (1 << (i + 12))) != 0))
                            {
                                unk = true;
                                break;
                            }
                        }
                        TilesToolbar.SetTileOptionState(i, unk ? TilesToolbar.TileOptionState.Indeterminate : set ? TilesToolbar.TileOptionState.Checked : TilesToolbar.TileOptionState.Unchcked);
                    }
                }
                else
                {
                    for (int i = 0; i < 4; ++i)
                        TilesToolbar.SetTileOptionState(i, TilesToolbar.TileOptionState.Disabled);
                }
            }
        }

        public void UpdateEditLayerActions()
        {
            if (EditLayer != null)
            {
                List<IAction> actions = EditLayer?.Actions;
                if (actions.Count > 0) redo.Clear();
                while (actions.Count > 0)
                {
                    bool create_new = false;
                    if (undo.Count == 0 || !(undo.Peek() is ActionsGroup))
                    {
                        create_new = true;
                    }
                    else
                    {
                        create_new = (undo.Peek() as ActionsGroup).IsClosed;
                    }
                    if (create_new)
                    {
                        undo.Push(new ActionsGroup());
                    }
                    (undo.Peek() as ActionsGroup).AddAction(actions[0]);
                    actions.RemoveAt(0);
                }

                //UpdateControls();
                //Potential Issue Causer
            }
        }

        private void UpdateStatusPanel(object sender, EventArgs e)
        {
            //
            // Tooltip Bar Info 
            //
                if (mySettings.pixelCountMode == false)
                {
                    positionLabel.Text = "X: " + (int)(lastX / Zoom) + " Y: " + (int)(lastY / Zoom);
                }
                else
                {
                    positionLabel.Text = "X: " + (int)((lastX / Zoom) / 16) + " Y: " + (int)((lastY / Zoom) / 16);
                }


            _levelIDLabel.Text = "Level ID: " + myEditorState.Level_ID.ToString();
            seperator1.Visible = true;
            seperator2.Visible = true;
            seperator3.Visible = true;
            seperator4.Visible = true;
            seperator5.Visible = true;
            seperator6.Visible = true;
            seperator7.Visible = true;
            seperator8.Visible = true;

            if (mySettings.pixelCountMode == false)
            {
                selectedPositionLabel.Text = "Selected Tile Position: X: " + (int)SelectedTileX + ", Y: " + (int)SelectedTileY;
                selectedPositionLabel.ToolTipText = "The Position of the Selected Tile";
            }
            else
            {
                selectedPositionLabel.Text = "Selected Tile Pixel Position: " + "X: " + (int)SelectedTileX * 16 + ", Y: " + (int)SelectedTileY * 16;
                selectedPositionLabel.ToolTipText = "The Pixel Position of the Selected Tile";
            }
            if (mySettings.pixelCountMode == false)
            {
                selectionSizeLabel.Text = "Amount of Tiles in Selection: " + (SelectedTilesCount - DeselectTilesCount);
                selectionSizeLabel.ToolTipText = "The Size of the Selection";
            }
            else
            {
                selectionSizeLabel.Text = "Length of Pixels in Selection: " + (SelectedTilesCount - DeselectTilesCount) * 16;
                selectionSizeLabel.ToolTipText = "The Length of all the Tiles (by Pixels) in the Selection";
            }

            selectionBoxSizeLabel.Text = "Selection Box Size: X: " + (select_x2 - select_x1) + ", Y: " + (select_y2 - select_y1);

            if (mySettings.ScrollLockDirection == true)
            {
                scrollLockDirLabel.Text = "Scroll Lock Direction: X";
            }
            else
            {
                scrollLockDirLabel.Text = "Scroll Lock Direction: Y";
            }
            int ScreenMaxH;
            int ScreenMaxV;
            if (ZoomLevel == 0)
            {
                ScreenMaxH = hScrollBar1.Maximum - hScrollBar1.LargeChange;
                ScreenMaxV = vScrollBar1.Maximum - vScrollBar1.LargeChange;
            }
            else
            {
                ScreenMaxH = hScrollBar1.Maximum - hScrollBar1.LargeChange;
                ScreenMaxV = vScrollBar1.Maximum - vScrollBar1.LargeChange;
            }

            Process proc = Process.GetCurrentProcess();
            long memory = proc.PrivateMemorySize64;

            //hVScrollBarXYLabel.Text = "Scroll Bar Position Values: X: " + (ScreenMaxH - hScrollBar1.Value) + ", Y: " + (ScreenMaxV - vScrollBar1.Value);
            //hVScrollBarXYLabel.Text = "Memory Used: " + memory.ToString();
            hVScrollBarXYLabel.Text = "Zoom Value: " + Zoom.ToString();


            //
            // End of Tooltip Bar Info Section
            //
        }

        private void UpdateTooltips()
        {
            UpdateTooltipForStacks(UndoButton, undo);
            UpdateTooltipForStacks(RedoButton, redo);
        }

        private void UpdateTooltipForStacks(ToolStripButton tsb, Stack<IAction> actionStack)
        {
            if (actionStack?.Count > 0)
            {
                IAction action = actionStack.Peek();
                tsb.ToolTipText = string.Format(tsb.Text, action.Description + " ");
            }
            else
            {
                tsb.ToolTipText = string.Format(tsb.Text, string.Empty);
            }
        }

        public void ToggleEditorButtons(bool enabled)
        {
            Editor.Instance.menuStrip1.Enabled = enabled;
            Editor.Instance.toolStrip1.Enabled = enabled;
            Editor.Instance.toolStrip2.Enabled = enabled;
            Editor.Instance.toolStrip3.Enabled = enabled;
            Editor.Instance.toolStrip4.Enabled = enabled;
            Editor.Instance.panel3.Enabled = enabled;
            Editor.Instance.splitContainer1.Enabled = enabled;
        }

        #endregion

        #region Editor Entity/Tile Management
        public void EditorPlaceTile(Point position, int tile)
        {
            Dictionary<Point, ushort> tiles = new Dictionary<Point, ushort>
            {
                [new Point(0, 0)] = (ushort)tile
            };
            EditLayer.PasteFromClipboard(position, tiles);
            UpdateEditLayerActions();
        }

        public void EditorTileReplaceTest(int findValue, int replaceValue, int applyState, bool copyResults, bool perserveColllision)
        {
            if (IsTilesEdit())
            {
                EditLayer.Select(new Rectangle(0, 0, 32768, 32768), true, false);
                UpdateEditLayerActions();
                Dictionary<Point, ushort> copyData = EditLayer.CopyToClipboard(true);
                FindReplaceClipboard = copyData;

                List<ushort> listValue = new List<ushort> { };
                List<Point> listPoint = new List<Point> { };
                List<int> listReplaceValues = new List<int> { };
                foreach (var item in FindReplaceClipboard)
                {
                    listPoint.Add(item.Key);
                }
                foreach (var item in FindReplaceClipboard)
                {
                    listValue.Add(item.Value);
                }
                for (int i = 0; i < listValue.Count; i++)
                {
                    if ((listValue[i] & 0x3ff) == (ushort)(findValue & 0x3ff))
                        unchecked
                        {
                            if (perserveColllision)
                            {
                                ushort TileIndex = (ushort)(listValue[i] & 0x3ff);
                                int TileIndexInt = (int)(listValue[i] & 0x3ff);
                                bool flipX = ((listValue[i] >> 10) & 1) == 1;
                                bool flipY = ((listValue[i] >> 11) & 1) == 1;
                                bool SolidTopA = ((listValue[i] >> 12) & 1) == 1;
                                bool SolidLrbA = ((listValue[i] >> 13) & 1) == 1;
                                bool SolidTopB = ((listValue[i] >> 14) & 1) == 1;
                                bool SolidLrbB = ((listValue[i] >> 15) & 1) == 1;

                                listValue[i] = (ushort)replaceValue;
                                if (flipX)
                                    listValue[i] |= (1 << 10);
                                else
                                    listValue[i] &= (ushort)~(1 << 10);
                                if (flipY)
                                    listValue[i] |= (1 << 11);
                                else
                                    listValue[i] &= (ushort)~(1 << 11);
                                if (SolidTopA)
                                    listValue[i] |= (1 << 12);
                                else
                                    listValue[i] &= (ushort)~(1 << 12);
                                if (SolidLrbA)
                                    listValue[i] |= (1 << 13);
                                else
                                    listValue[i] &= (ushort)~(1 << 13);
                                if (SolidTopB)
                                    listValue[i] |= (1 << 14);
                                else
                                    listValue[i] &= (ushort)~(1 << 14);
                                if (SolidLrbB)
                                    listValue[i] |= (1 << 15);
                                else
                                    listValue[i] &= (ushort)~(1 << 15);
                            }
                            else
                            {
                                listValue[i] = (ushort)replaceValue;
                            }
                        }
                }
                FindReplaceClipboard.Clear();
                for (int i = 0; i < listPoint.Count; i++)
                {
                    FindReplaceClipboard.Add(listPoint[i], listValue[i]);
                }

                // if there's none, use the internal clipboard
                if (FindReplaceClipboard != null)
                {
                    EditLayer.PasteFromClipboard(new Point(0, 0), FindReplaceClipboard);
                    UpdateEditLayerActions();
                }
                UpdateEditLayerActions();
                FindReplaceClipboard.Clear();
                Deselect();
            }

        }

        public void EditorTileFindTest(int tile, int applyState, bool copyResults)
        {
            if (IsTilesEdit())
            {
                EditLayer.Select(new Rectangle(0, 0, 32768, 32768), true, false);
                UpdateEditLayerActions();
                Dictionary<Point, ushort> copyData = EditLayer.CopyToClipboard(true);
                FindReplaceClipboard = copyData;

                List<ushort> listValue = new List<ushort> { };
                List<Point> listPoint = new List<Point> { };
                List<Point> listLocations = new List<Point> { };

                foreach (var item in FindReplaceClipboard)
                {
                    listPoint.Add(item.Key);
                }
                foreach (var item in FindReplaceClipboard)
                {
                    listValue.Add(item.Value);
                }
                for (int i = 0; i < listValue.Count; i++)
                {
                    if ((listValue[i] & 0x3ff) == (ushort)(tile & 0x3ff))
                    {
                        listLocations.Add(listPoint[i]);
                    }
                }
                FindReplaceClipboard.Clear();
                if (listLocations != null || listLocations.Count != 0)
                {
                    var message = string.Join(Environment.NewLine, listLocations);
                    MessageBox.Show("Tiles found at: " + Environment.NewLine + message, "Results");
                    if (copyResults && message != null)
                    {
                        Clipboard.SetText(message);
                    }
                }
                else
                {
                    MessageBox.Show("Found Nothing", "Results");
                }
                FindReplaceClipboard.Clear();
                Deselect();


            }

        }

        public void EditorTileFind(int tile, int applyState, bool copyResults)
        {
            List<Point> Points = new List<Point>();
            
            for (int y = 0; y < EditLayer.Layer.Height; y++)
            {
                for (int x = 0; x < EditLayer.Layer.Width; x++)
                {
                    ushort TileIndex = (ushort)(EditLayer.Layer.Tiles[y][x] & 0x3ff); //What is our tile index?
                    if (TileIndex == tile) //do the tiles match?
                    {
                        Points.Add(new Point(x * 16, y * 16)); //Add the found tile to our list of points!
                        //Console.WriteLine(x * 16 + " " + y * 16);                       
                    }
                }
            }
        }

        public void EditorTileFindReplace(int FindTile, int ReplaceTile, int applyState, bool copyResults)
        {
            List<Point> Points = new List<Point>();

            for (int y = 0; y < EditLayer.Layer.Height; y++)
            {
                for (int x = 0; x < EditLayer.Layer.Width; x++)
                {
                    ushort TileIndex = (ushort)(EditLayer.Layer.Tiles[y][x] & 0x3ff); //What is our tile index?
                    if (TileIndex == FindTile) //do the tiles match?
                    {
                        Points.Add(new Point(x * 16, y * 16)); //Add the found tile to our list of points!

                        ushort Tile = (ushort)ReplaceTile; //Make a new Ushort using the new tileindex
                        
                        //Copy the collision and flip data, but I'm to lazy rn lol

                        //Tile = (ushort)SetBit(10, FlipX, Tile); //Set the flip X value
                        //Tile = (ushort)SetBit(11, FlipY, Tile); //Set the flip Y value
                        //Tile = (ushort)SetBit(12, CollisionAT, Tile); //Set the collision (Top, Path A) value
                        //Tile = (ushort)SetBit(13, CollisionALRB, Tile); //Set the collision (All But Top, Path A) value
                        //Tile = (ushort)SetBit(14, CollisionBT, Tile); //Set the collision (Top, Path B) value
                        //Tile = (ushort)SetBit(15, CollisionBLRB, Tile); //Set the collision (All But Top, Path B) value

                        //TEMPORARY (because I'm lazy)
                        Tile = (ushort)SetBit(10, false, Tile);
                        Tile = (ushort)SetBit(11, false, Tile);
                        Tile = (ushort)SetBit(12, false, Tile);
                        Tile = (ushort)SetBit(13, false, Tile);
                        Tile = (ushort)SetBit(14, false, Tile);
                        Tile = (ushort)SetBit(15, false, Tile);

                        EditLayer.Layer.Tiles[y][x] = Tile; //Set our new tile Value

                        //Console.WriteLine(x * 16 + " " + y * 16);
                    }
                }
            }
        }

        //Used to set individual Bits in an int
        public static int SetBit(int pos, bool Set, int Value) //Shitty Maybe, but idc, it works
        {

            // "Pos" is what bit we are changing
            // "Set" tells it to be either on or off
            // "Value" is the value you want as your source

            if (Set)
            {
                Value |= 1 << pos;
            }
            if (!Set)
            {
                Value &= ~(1 << pos);
            }
            return Value;
        }

        public void DeleteSelected()
        {
            EditLayer?.DeleteSelected();
            UpdateEditLayerActions();

            if (IsEntitiesEdit())
            {
                entities.DeleteSelected();
                UpdateLastEntityAction();
            }
        }

        public void UpdateLastEntityAction()
        {
            if (entities.LastAction != null)
            {
                redo.Clear();
                undo.Push(entities.LastAction);
                entities.LastAction = null;
                UpdateControls();
            }

        }

        public void FlipEntities(FlipDirection direction)
        {
            Dictionary<EditorEntity, Point> initalPos = new Dictionary<EditorEntity, Point> ();
            Dictionary<EditorEntity, Point> postPos = new Dictionary<EditorEntity, Point>();
            foreach (EditorEntity e in entities.selectedEntities)
            {
                initalPos.Add(e, new Point(e.PositionX, e.PositionY));
            }
            entities.Flip(direction);
            entitiesToolbar.UpdateCurrentEntityProperites();
            foreach (EditorEntity e in entities.selectedEntities)
            {
                postPos.Add(e, new Point(e.PositionX, e.PositionY));
            }
            IAction action = new ActionMultipleMoveEntities(initalPos, postPos);
            undo.Push(action);
            redo.Clear();

        }

        public void SelectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (IsTilesEdit())
            {
                EditLayer.Select(new Rectangle(0, 0, 32768, 32768), true, false);
                UpdateEditLayerActions();
            }
            else if (IsEntitiesEdit())
            {
                entities.Select(new Rectangle(0, 0, 32768, 32768), true, false);
            }
            SetSelectOnlyButtonsState();
            ClickedX = -1;
            ClickedY = -1;
        }
        #endregion

        #region Mouse Actions

        private void GraphicPanel_OnMouseMove(object sender, MouseEventArgs e)
        {
            
            if (mySettings.allowForSmoothSelection)
            {
                UpdateRender();
            }

            if (playerSelected)
            {
                EditorGame.MovePlayer(new Point(e.X, e.Y), Zoom, selectedPlayer);

            }

            if (checkpointSelected)
            {
                Point clicked_point = new Point((int)(e.X / Zoom), (int)(e.Y / Zoom));
                EditorGame.UpdateCheckpoint(clicked_point, true);
            }

            if (ClickedX != -1)
            {
                Point clicked_point = new Point((int)(ClickedX / Zoom), (int)(ClickedY / Zoom));
                // There was just a click now we can determine that this click is dragging

                if (IsTilesEdit() && !InteractionToolButton.Checked)
                {
                    
                    if ((EditLayer?.IsPointSelected(clicked_point)).Value)
                    {
                        // Start dragging the tiles
                        dragged = true;
                        startDragged = true;
                        EditLayer?.StartDrag();
            
                    }

                    else if (!SelectToolButton.Checked && !ShiftPressed() && !CtrlPressed() && (EditLayer?.HasTileAt(clicked_point)).Value)
                    {
                        // Start dragging the single selected tile
                        EditLayer?.Select(clicked_point);
                        dragged = true;
                        startDragged = true;
                        EditLayer?.StartDrag();
            
                    }

                    else
                    {
                        // Start drag selection
                        //EditLayer.Select(clicked_point, ShiftPressed || CtrlPressed, CtrlPressed);
                        if (!ShiftPressed() && !CtrlPressed())
                            Deselect();
                        UpdateControls();
                        UpdateEditLayerActions();
            
                        draggingSelection = true;
                        selectingX = ClickedX;
                        selectingY = ClickedY;
                    }
                }
                else if (IsEntitiesEdit())
                {
                    if (entities.GetEntityAt(clicked_point)?.Selected ?? false)
                    {
                        ClickedX = e.X;
                        ClickedY = e.Y;
                        // Start dragging the entity
                        dragged = true;
                        draggedX = 0;
                        draggedY = 0;
                        startDragged = true;
            
                    }
                    else
                    {
                        // Start drag selection
                        if (!ShiftPressed() && !CtrlPressed())
                            Deselect();
                        UpdateControls();
                        draggingSelection = true;
                        selectingX = ClickedX;
                        selectingY = ClickedY;
            
                    }
                }
                ClickedX = -1;
                ClickedY = -1;
    
            }
            if (scrolling)
            {
                if (wheelClicked)
                {
                    scrollingDragged = true;
        
                }

                int xMove = (hScrollBar1.Visible) ? e.X - ShiftX - scrollPosition.X : 0;
                int yMove = (vScrollBar1.Visible) ? e.Y - ShiftY - scrollPosition.Y : 0;

                if (Math.Abs(xMove) < 15) xMove = 0;
                if (Math.Abs(yMove) < 15) yMove = 0;

                if (xMove > 0)
                {
                    if (yMove > 0) Cursor = Cursors.PanSE;
                    else if (yMove < 0) Cursor = Cursors.PanNE;
                    else Cursor = Cursors.PanEast;
        
                }
                else if (xMove < 0)
                {
                    if (yMove > 0) Cursor = Cursors.PanSW;
                    else if (yMove < 0) Cursor = Cursors.PanNW;
                    else Cursor = Cursors.PanWest;
        
                }
                else
                {
        
                    if (yMove > 0) Cursor = Cursors.PanSouth;
                    else if (yMove < 0) Cursor = Cursors.PanNorth;
                    else
                    {
                        if (vScrollBar1.Visible && hScrollBar1.Visible) Cursor = Cursors.NoMove2D;
                        else if (vScrollBar1.Visible) Cursor = Cursors.NoMoveVert;
                        else if (hScrollBar1.Visible) Cursor = Cursors.NoMoveHoriz;
                    }
        
                }

                Point position = new Point(ShiftX, ShiftY); ;
                int x = xMove / 10 + position.X;
                int y = yMove / 10 + position.Y;

                CustomX += xMove / 10;
                CustomY += yMove / 10;

                if (x < 0) x = 0;
                if (y < 0) y = 0;
                if (x > hScrollBar1.Maximum - hScrollBar1.LargeChange) x = hScrollBar1.Maximum - hScrollBar1.LargeChange;
                if (y > vScrollBar1.Maximum - vScrollBar1.LargeChange) y = vScrollBar1.Maximum - vScrollBar1.LargeChange;
                
                
                if (x != position.X || y != position.Y)
                {

                    if (vScrollBar1.Visible)
                    {
                        vScrollBar1.Value = y;
                    }
                    if (hScrollBar1.Visible)
                    {
                        hScrollBar1.Value = x;
                    }
                    
                   OnMouseMoveEvent();
        
                }

                UpdateRender();

            }
            if (IsEditing())
            {
                if (IsTilesEdit() && PlaceTilesButton.Checked)
                {
                    Point p = new Point((int)(e.X / Zoom), (int)(e.Y / Zoom));
                    if (e.Button == MouseButtons.Left)
                    {
                        // Place tile
                        if (TilesToolbar.SelectedTile != -1)
                        {
                            if (EditLayer.GetTileAt(p) != TilesToolbar.SelectedTile)
                            {
                                EditorPlaceTile(p, TilesToolbar.SelectedTile);
                            }
                            else if (!EditLayer.IsPointSelected(p))
                            {
                                EditLayer.Select(p);
                            }
                        }
                    }
                    else if (e.Button == MouseButtons.Right)
                    {
                        // Remove tile
                        if (!EditLayer.IsPointSelected(p))
                        {
                            EditLayer.Select(p);
                        }
                        DeleteSelected();

                    }
                }
                if (draggingSelection || dragged)
                {
                    Point position = new Point(ShiftX, ShiftY); ;
                    int ScreenMaxX = position.X + splitContainer1.Panel1.Width - System.Windows.Forms.SystemInformation.VerticalScrollBarWidth;
                    int ScreenMaxY = position.Y + splitContainer1.Panel1.Height - System.Windows.Forms.SystemInformation.HorizontalScrollBarHeight;
                    int ScreenMinX = position.X;
                    int ScreenMinY = position.Y;

                    int x = position.X;
                    int y = position.Y;

                    if (e.X > ScreenMaxX)
                    {
                        x += (e.X - ScreenMaxX) / 10;
                    }
                    else if (e.X < ScreenMinX)
                    {
                        x += (e.X - ScreenMinX) / 10;
                    }
                    if (e.Y > ScreenMaxY)
                    {
                        y += (e.Y - ScreenMaxY) / 10;
                    }
                    else if (e.Y < ScreenMinY)
                    {
                        y += (e.Y - ScreenMinY) / 10;
                    }

                    if (x < 0) x = 0;
                    if (y < 0) y = 0;
                    if (x > hScrollBar1.Maximum - hScrollBar1.LargeChange) x = hScrollBar1.Maximum - hScrollBar1.LargeChange;
                    if (y > vScrollBar1.Maximum - vScrollBar1.LargeChange) y = vScrollBar1.Maximum - vScrollBar1.LargeChange;

                    if (x != position.X || y != position.Y)
                    {
                        if (vScrollBar1.Visible)
                        {
                            vScrollBar1.Value = y;
                        }
                        if (hScrollBar1.Visible)
                        {
                            hScrollBar1.Value = x;
                        }
                        OnMouseMoveEvent();
                        UpdateRender();



                    }

                }

                if (draggingSelection)
                {
                    if (selectingX != e.X && selectingY != e.Y)
                    {
                        select_x1 = (int)(selectingX / Zoom);
                        select_x2 = (int)(e.X / Zoom);
                        select_y1 = (int)(selectingY / Zoom);
                        select_y2 = (int)(e.Y / Zoom);
                        if (select_x1 > select_x2)
                        {
                            select_x1 = (int)(e.X / Zoom);
                            select_x2 = (int)(selectingX / Zoom);
                        }
                        if (select_y1 > select_y2)
                        {
                            select_y1 = (int)(e.Y / Zoom);
                            select_y2 = (int)(selectingY / Zoom);
                        }
                        EditLayer?.TempSelection(new Rectangle(select_x1, select_y1, select_x2 - select_x1, select_y2 - select_y1), CtrlPressed());
                        UpdateTilesOptions();

                        if (IsEntitiesEdit()) entities.TempSelection(new Rectangle(select_x1, select_y1, select_x2 - select_x1, select_y2 - select_y1), CtrlPressed());
                    }
                }
                else if (dragged)
                {
                    int oldGridX = (int)((lastX / Zoom) / magnetSize) * magnetSize;
                    int oldGridY = (int)((lastY / Zoom) / magnetSize) * magnetSize;
                    int newGridX = (int)((e.X / Zoom) / magnetSize) * magnetSize;
                    int newGridY = (int)((e.Y / Zoom) / magnetSize) * magnetSize;
                    Point oldPointGrid = new Point(0, 0);
                    Point newPointGrid = new Point(0, 0);
                    if (UseMagnetMode && IsEntitiesEdit())
                    {
                        if (useMagnetXAxis == true && useMagnetYAxis == true)
                        {
                            oldPointGrid = new Point(oldGridX, oldGridY);
                            newPointGrid = new Point(newGridX, newGridY);
                        }
                        if (useMagnetXAxis && !useMagnetYAxis)
                        {
                            oldPointGrid = new Point(oldGridX, (int)(lastY / Zoom));
                            newPointGrid = new Point(newGridX, (int)(e.Y / Zoom));
                        }
                        if (!useMagnetXAxis && useMagnetYAxis)
                        {
                            oldPointGrid = new Point((int)(lastX / Zoom), oldGridY);
                            newPointGrid = new Point((int)(e.X / Zoom), newGridY);
                        }
                        if (!useMagnetXAxis && !useMagnetYAxis)
                        {
                            oldPointGrid = new Point((int)(lastX / Zoom), (int)(lastY / Zoom));
                            newPointGrid = new Point((int)(e.X / Zoom), (int)(e.Y / Zoom));
                        }
                    }
                    Point oldPoint = new Point((int)(lastX / Zoom), (int)(lastY / Zoom));
                    Point newPoint = new Point((int)(e.X / Zoom), (int)(e.Y / Zoom));


                    EditLayer?.MoveSelected(oldPoint, newPoint, CtrlPressed());

                    UpdateEditLayerActions();
                    if (IsEntitiesEdit())
                    {
                        if (UseMagnetMode)
                        {
                            int x = entities.SelectedEntities[0].Entity.Position.X.High;
                            int y = entities.SelectedEntities[0].Entity.Position.Y.High;

                            if (x % magnetSize != 0 && useMagnetXAxis)
                            {
                                int offsetX = x % magnetSize;
                                oldPointGrid.X -= offsetX;
                            }
                            if (y % magnetSize != 0 && useMagnetYAxis)
                            {
                                int offsetY = y % magnetSize;
                                oldPointGrid.Y -= offsetY;
                            }
                        }


                        try
                        {

                            if (Editor.Instance.UseMagnetMode)
                            {
                                entities.MoveSelected(oldPointGrid, newPointGrid, CtrlPressed() && startDragged);
                            }
                            else
                            {
                                entities.MoveSelected(oldPoint, newPoint, CtrlPressed() && startDragged);
                            }

                        }
                        catch (EditorEntities.TooManyEntitiesException)
                        {
                            MessageBox.Show("Too many entities! (limit: 2048)");
                            dragged = false;
                            return;
                        }
                        if (Editor.Instance.UseMagnetMode)
                        {
                            draggedX += newPointGrid.X - oldPointGrid.X;
                            draggedY += newPointGrid.Y - oldPointGrid.Y;
                        }
                        else
                        {
                            draggedX += newPoint.X - oldPoint.X;
                            draggedY += newPoint.Y - oldPoint.Y;
                        }
                        if (CtrlPressed() && startDragged)
                        {
                            UpdateEntitiesToolbarList();
                            SetSelectOnlyButtonsState();
                        }
                        entitiesToolbar.UpdateCurrentEntityProperites();
                    }
                    startDragged = false;
                }
            }
            

            lastX = e.X;
            lastY = e.Y;

            
        }

        private void GraphicPanel_OnMouseDown(object sender, MouseEventArgs e)
        {
            GraphicPanel.Focus();
            if (e.Button == MouseButtons.Left)
            {
                if (IsEditing() && !dragged)
                {
                    if (IsTilesEdit() && !InteractionToolButton.Checked)
                    {
                        if (PlaceTilesButton.Checked)
                        {
                            // Place tile
                            if (TilesToolbar.SelectedTile != -1)
                            {
                                EditorPlaceTile(new Point((int)(e.X / Zoom), (int)(e.Y / Zoom)), TilesToolbar.SelectedTile);
                            }
                        }
                        else
                        {
                            ClickedX = e.X;
                            ClickedY = e.Y;
                        }
                    }
                    else if (IsEntitiesEdit())
                    {
                        Point clicked_point = new Point((int)(e.X / Zoom), (int)(e.Y / Zoom));
                        if (entities.GetEntityAt(clicked_point)?.Selected ?? false)
                        {
                            // We will have to check if this dragging or clicking
                            ClickedX = e.X;
                            ClickedY = e.Y;
                        }
                        else if (!ShiftPressed() && !CtrlPressed() && entities.GetEntityAt(clicked_point) != null)
                        {
                            entities.Select(clicked_point);
                            SetSelectOnlyButtonsState();
                            // Start dragging the single selected entity
                            dragged = true;
                            draggedX = 0;
                            draggedY = 0;
                            startDragged = true;
                        }
                        else
                        {
                            ClickedX = e.X;
                            ClickedY = e.Y;
                        }
                    }
                }

                if (playerSelected)
                {
                    playerSelected = false;
                    selectedPlayer = 0;
                }
                if (checkpointSelected)
                {
                    checkpointSelected = false;
                }

                if (scrolling)
                {
                    scrolling = false;
                    Cursor = Cursors.Default;
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                if (IsTilesEdit() && PlaceTilesButton.Checked)
                {
                    // Remove tile
                    Point p = new Point((int)(e.X / Zoom), (int)(e.Y / Zoom));
                    if (!EditLayer.IsPointSelected(p))
                    {
                        EditLayer.Select(p);
                    }
                    DeleteSelected();
                }
            }
            else if (e.Button == MouseButtons.Middle)
            {
                wheelClicked = true;
                scrolling = true;
                scrollingDragged = false;
                scrollPosition = new Point(e.X - ShiftX, e.Y - ShiftY);
                if (vScrollBar1.Visible && hScrollBar1.Visible)
                {
                    Cursor = Cursors.NoMove2D;
                }
                else if (vScrollBar1.Visible)
                {
                    Cursor = Cursors.NoMoveVert;
                }
                else if (hScrollBar1.Visible)
                {
                    Cursor = Cursors.NoMoveHoriz;
                }
                else
                {
                    scrolling = false;
                }
            }
        }

        private void GraphicPanel_OnMouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (IsEditing())
                {
                    //MagnetDisable();
                    //This isn't what the new magnet mode is all about
                    if (draggingSelection)
                    {
                        if (selectingX != e.X && selectingY != e.Y)
                        {

                            int x1 = (int)(selectingX / Zoom), x2 = (int)(e.X / Zoom);
                            int y1 = (int)(selectingY / Zoom), y2 = (int)(e.Y / Zoom);
                            if (x1 > x2)
                            {
                                x1 = (int)(e.X / Zoom);
                                x2 = (int)(selectingX / Zoom);
                            }
                            if (y1 > y2)
                            {
                                y1 = (int)(e.Y / Zoom);
                                y2 = (int)(selectingY / Zoom);
                            }
                            EditLayer?.Select(new Rectangle(x1, y1, x2 - x1, y2 - y1), ShiftPressed() || CtrlPressed(), CtrlPressed());

                            if (IsEntitiesEdit()) entities.Select(new Rectangle(x1, y1, x2 - x1, y2 - y1), ShiftPressed() || CtrlPressed(), CtrlPressed());
                            SetSelectOnlyButtonsState();
                            UpdateEditLayerActions();
                        }
                        draggingSelection = false;
                        EditLayer?.EndTempSelection();
                        if (IsEntitiesEdit()) entities.EndTempSelection();
                    }
                    else
                    {
                        if (ClickedX != -1)
                        {
                            // So it was just click
                            Point clicked_point = new Point((int)(ClickedX / Zoom), (int)(ClickedY / Zoom));
                            if (IsTilesEdit())
                            {
                                EditLayer.Select(clicked_point, ShiftPressed() || CtrlPressed(), CtrlPressed());
                                UpdateEditLayerActions();
                            }
                            else if (IsEntitiesEdit())
                            {
                                entities.Select(clicked_point, ShiftPressed() || CtrlPressed(), CtrlPressed());
                            }                        

                            SetSelectOnlyButtonsState();
                            ClickedX = -1;
                            ClickedY = -1;
                        }
                        if (dragged && (draggedX != 0 || draggedY != 0))
                        {
                            if (IsEntitiesEdit())
                            {
                                IAction action = new ActionMoveEntities(entities.SelectedEntities.ToList(), new Point(draggedX, draggedY));
                                if (entities.LastAction != null)
                                {
                                    // If it is move & duplicate, merge them together
                                    var taction = new ActionsGroup();
                                    taction.AddAction(entities.LastAction);
                                    entities.LastAction = null;
                                    taction.AddAction(action);
                                    taction.Close();
                                    action = taction;
                                }
                                undo.Push(action);
                                redo.Clear();
                                UpdateControls();
                            }
                        }
                        dragged = false;
                    }
                }
            }
            else if (e.Button == MouseButtons.Middle)
            {
                wheelClicked = false;
                if (scrollingDragged)
                {
                    scrolling = false;
                    Cursor = Cursors.Default;
                }
            }
            UpdateControls();
        }

        private void GraphicPanel_MouseWheel(object sender, MouseEventArgs e)
        {
            GraphicPanel.Focus();
            if (CtrlPressed())
            {
                int maxZoom;
                int minZoom;
                if (mySettings.ReduceZoom)
                {
                    maxZoom = 5;
                    minZoom = -2;
                }
                else
                {
                    maxZoom = 5;
                    minZoom = -5;
                }
                int change = e.Delta / 120;
                ZoomLevel += change;
                if (ZoomLevel > maxZoom) ZoomLevel = maxZoom;
                if (ZoomLevel < minZoom) ZoomLevel = minZoom;

                SetZoomLevel(ZoomLevel, new Point(e.X - ShiftX, e.Y - ShiftY));
            }
            else
            {
                if (vScrollBar1.Visible || hScrollBar1.Visible)
                {
                    if (scrollDirection == "Y" && !mySettings.scrollLock)
                    {
                        if (vScrollBar1.Visible)
                        {
                            int y = vScrollBar1.Value - e.Delta;
                            if (y < 0) y = 0;
                            if (y > vScrollBar1.Maximum - vScrollBar1.LargeChange) y = vScrollBar1.Maximum - vScrollBar1.LargeChange;
                            vScrollBar1.Value = y;
                        }
                        else
                        {
                            int x = hScrollBar1.Value - e.Delta * 2;
                            if (x < 0) x = 0;
                            if (x > hScrollBar1.Maximum - hScrollBar1.LargeChange) x = hScrollBar1.Maximum - hScrollBar1.LargeChange;
                            hScrollBar1.Value = x;
                        }
                    }
                    else if (scrollDirection == "X" && !mySettings.scrollLock)
                    {
                        if (hScrollBar1.Visible)
                        {
                            int x = hScrollBar1.Value - e.Delta * 2;
                            if (x < 0) x = 0;
                            if (x > hScrollBar1.Maximum - hScrollBar1.LargeChange) x = hScrollBar1.Maximum - hScrollBar1.LargeChange;
                            hScrollBar1.Value = x;
                        }
                        else
                        {
                            int y = vScrollBar1.Value - e.Delta;
                            if (y < 0) y = 0;
                            if (y > vScrollBar1.Maximum - vScrollBar1.LargeChange) y = vScrollBar1.Maximum - vScrollBar1.LargeChange;
                            vScrollBar1.Value = y;
                        }
                    }
                    else if (scrollDirection == "Locked" || mySettings.scrollLock == true)
                    {
                        if (mySettings.ScrollLockDirection == false)
                        {
                            if (vScrollBar1.Visible)
                            {
                                int y = vScrollBar1.Value - e.Delta * 2;
                                if (y < 0) y = 0;
                                if (y > vScrollBar1.Maximum - vScrollBar1.LargeChange) y = vScrollBar1.Maximum - vScrollBar1.LargeChange;
                                if (y <= -1) y = 0;
                                vScrollBar1.Value = y;
                            }
                            else
                            {
                                int x = vScrollBar1.Value - e.Delta * 2;
                                if (x < 0) x = 0;
                                if (x > vScrollBar1.Maximum - vScrollBar1.LargeChange) x = vScrollBar1.Maximum - vScrollBar1.LargeChange;
                                if (x <= -1) x = 0;
                                vScrollBar1.Value = x;
                            }
                        }
                        else
                        {
                            if (hScrollBar1.Visible)
                            {
                                int x = hScrollBar1.Value - e.Delta * 2;
                                if (x < 0) x = 0;
                                if (x > hScrollBar1.Maximum - hScrollBar1.LargeChange) x = hScrollBar1.Maximum - hScrollBar1.LargeChange;
                                if (x <= -1) x = 0;
                                hScrollBar1.Value = x;
                            }
                            else
                            {
                                int y = vScrollBar1.Value - e.Delta;
                                if (y < 0) y = 0;
                                if (y > vScrollBar1.Maximum - vScrollBar1.LargeChange) y = vScrollBar1.Maximum - vScrollBar1.LargeChange;
                                if (y <= -1) y = 0;
                                vScrollBar1.Value = y;
                            }
                        }

                    }
                }
                if (mySettings.EntityFreeCam)
                {
                    if (mySettings.ScrollLockDirection) CustomX -= e.Delta;
                    else CustomY -= e.Delta;

                }

            }
        }
        #endregion

        #region GameConfig/Data Folders

        public string GetDataDirectory()
        {
            using (var folderBrowserDialog = new FolderSelectDialog())
            {
                folderBrowserDialog.Title = "Select Data Folder";

                if (!folderBrowserDialog.ShowDialog())
                    return null;

                return folderBrowserDialog.FileName;
            }
        }

        private ToolStripMenuItem CreateDataDirectoryMenuLink(string target)
        {
            ToolStripMenuItem newItem = new ToolStripMenuItem(target)
            {
                Tag = target
            };
            newItem.Click += RecentDataDirectoryClicked;
            return newItem;
        }

        public bool SetGameConfig()
        {
            try
            {
                GameConfig = new GameConfig(Path.Combine(DataDirectory, "Game", "GameConfig.bin"));
                return true;
            }
            catch
            {
                // Allow the User to be able to have a Maniac Editor Dedicated GameConfig, see if the user has made one
                try
                {
                    GameConfig = new GameConfig(Path.Combine(DataDirectory, "Game", "GameConfig_ME.bin"));
                    return true;
                }
                catch
                {
                    MessageBox.Show("Something is wrong with this GameConfig that we can't support! If for some reason it does work for you in Sonic Mania, you can create another GameConfig.bin called GameConfig_ME.bin and the editor should load that instead (assuming it's a clean GameConfig or one that works) allowing you to still be able to use the data folder, however, this is experimental so be careful when doing that.", "GameConfig Error!");
                    return false;
                }


            }

        }
        public bool IsDataDirectoryValid(string directoryToCheck)
        {
            return File.Exists(Path.Combine(directoryToCheck, "Game", "GameConfig.bin"));
        }

        private void RecentDataDirectoryClicked(object sender, EventArgs e, String dataDirectory)
        {
            var dataDirectories = mySettings.DataDirectories;
            mySettings.GamePath = GamePath;
            if (IsDataDirectoryValid(dataDirectory))
            {
                ResetDataDirectoryToAndResetScene(dataDirectory);
            }
            else
            {
                MessageBox.Show($"The specified Data Directory {dataDirectory} is not valid.",
                                "Invalid Data Directory!",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                dataDirectories.Remove(dataDirectory);
                RefreshDataDirectories(dataDirectories);

            }
            mySettings.Save();
        }

        private void ResetDataDirectoryToAndResetScene(string newDataDirectory)
        {
            Editor.Instance.SceneChangeWarning(null, null);
            if (AllowSceneChange == true || IsSceneLoaded() == false || mySettings.DisableSaveWarnings == true)
            {
                    AllowSceneChange = false;
                    UnloadScene();
                    UseDefaultPrefrences();
                    DataDirectory = newDataDirectory;
                    AddRecentDataFolder(newDataDirectory);
                    bool goodGameConfig = SetGameConfig();
                    if (goodGameConfig == true)
                    {
                        OpenScene(mySettings.forceBrowse);
                    }

            }


        }

        private void RecentDataDirectoryClicked(object sender, EventArgs e)
        {
            var menuItem = sender as ToolStripMenuItem;
            string dataDirectory = menuItem.Tag.ToString();
            var dataDirectories = mySettings.DataDirectories;
            mySettings.GamePath = GamePath;
            if (IsDataDirectoryValid(dataDirectory))
            {
                ResetDataDirectoryToAndResetScene(dataDirectory);
            }
            else
            {
                MessageBox.Show($"The specified Data Directory {dataDirectory} is not valid.",
                                "Invalid Data Directory!",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                dataDirectories.Remove(dataDirectory);
                RefreshDataDirectories(dataDirectories);

            }
            mySettings.Save();
        }

        /// <summary>
        /// Refreshes the Data directories displayed in the recent list under the File menu.
        /// </summary>
        /// <param name="settings">The settings file containing the </param>
        public void RefreshDataDirectories(StringCollection recentDataDirectories)
        {
            recentDataDirectoriesToolStripMenuItem.Visible = false;
            CleanUpRecentList();

            var startRecentItems = fileToolStripMenuItem.DropDownItems.IndexOf(recentDataDirectoriesToolStripMenuItem);
            var startRecentItemsButton = RecentDataDirectories.DropDownItems.IndexOf(noRecentDataDirectoriesToolStripMenuItem);

            foreach (var dataDirectory in recentDataDirectories)
            {
                _recentDataItems.Add(CreateDataDirectoryMenuLink(dataDirectory));
                _recentDataItems_Button.Add(CreateDataDirectoryMenuLink(dataDirectory));

            }


            foreach (var menuItem in _recentDataItems.Reverse())
            {
                fileToolStripMenuItem.DropDownItems.Insert(startRecentItems, menuItem);
            }

            foreach (var menuItem in _recentDataItems_Button.Reverse())
            {
                RecentDataDirectories.DropDownItems.Insert(startRecentItemsButton, menuItem);
            }


        }

        private void UpdateDataFolderLabel(object sender, EventArgs e)
        {
            string modFolderTag = "Mod Directory: {0} [Mod-Loaded]";
            string dataFolderTag_Normal = "Data Directory: {0}";
            string dataFolderTag_ModLoaded = "Data Directory: {0} [Mod-Loaded]";
            if (showingDataDirectory && ModDataDirectory != "")
            {
                _baseDataDirectoryLabel.Tag = modFolderTag;
                UpdateDataFolderLabel(ModDataDirectory);
                showingDataDirectory = false;
            }
            else
            {
                _baseDataDirectoryLabel.Tag = (ModDataDirectory != "" ? dataFolderTag_ModLoaded : dataFolderTag_Normal);
                UpdateDataFolderLabel();
                showingDataDirectory = true;
            }
            
        }


        private void UpdateDataFolderLabel(string dataDirectory = null)
        {          
            if (dataDirectory != null) _baseDataDirectoryLabel.Text = string.Format(_baseDataDirectoryLabel.Tag.ToString(), dataDirectory);
            else _baseDataDirectoryLabel.Text = string.Format(_baseDataDirectoryLabel.Tag.ToString(), DataDirectory);
        }

        /// <summary>
        /// Removes any recent Data directories from the File menu.
        /// </summary>
        private void CleanUpRecentList()
        {
            foreach (var menuItem in _recentDataItems)
            {
                menuItem.Click -= RecentDataDirectoryClicked;
                fileToolStripMenuItem.DropDownItems.Remove(menuItem);
            }
            foreach (var menuItem in _recentDataItems_Button)
            {
                menuItem.Click -= RecentDataDirectoryClicked;
                RecentDataDirectories.DropDownItems.Remove(menuItem);
            }
            _recentDataItems.Clear();
            _recentDataItems_Button.Clear();
        }

        public void AddRecentDataFolder(string dataDirectory)
        {
            try
            {
                var mySettings = Properties.Settings.Default;
                var dataDirectories = mySettings.DataDirectories;

                if (dataDirectories == null)
                {
                    dataDirectories = new StringCollection();
                    mySettings.DataDirectories = dataDirectories;
                }

                if (dataDirectories.Contains(dataDirectory))
                {
                    dataDirectories.Remove(dataDirectory);
                }

                if (dataDirectories.Count >= 10)
                {
                    for (int i = 9; i < dataDirectories.Count; i++)
                    {
                        dataDirectories.RemoveAt(i);
                    }
                }

                dataDirectories.Insert(0, dataDirectory);

                mySettings.Save();

                RefreshDataDirectories(dataDirectories);

                UpdateDataFolderLabel(dataDirectory);


            }
            catch (Exception ex)
            {
                Debug.Write("Failed to add data folder to recent list: " + ex);
            }
        }

        #endregion

        #region Zooming/Resizing Related Methods

        public void SetZoomLevel(int zoom_level, Point zoom_point)
        {
            double old_zoom = Zoom;

            ZoomLevel = zoom_level;

            switch (ZoomLevel)
            {
                case 5: Zoom = 4; break;
                case 4: Zoom = 3; break;
                case 3: Zoom = 2; break;
                case 2: Zoom = 3 / 2.0; break;
                case 1: Zoom = 5 / 4.0; break;
                case 0: Zoom = 1; break;
                case -1: Zoom = 2 / 3.0; break;
                case -2: Zoom = 1 / 2.0; break;
                case -3: Zoom = 1 / 3.0; break;
                case -4: Zoom = 1 / 4.0; break;
                case -5: Zoom = 1 / 8.0; break;
            }

            zooming = true;

            int oldShiftX = ShiftX;
            int oldShiftY = ShiftY;

            if (EditorScene != null)
                SetViewSize((int)(SceneWidth * Zoom), (int)(SceneHeight * Zoom));

            
            if (hScrollBar1.Visible)
            {
                ShiftX = (int)((zoom_point.X + oldShiftX) / old_zoom * Zoom - zoom_point.X);
                ShiftX = Math.Min(hScrollBar1.Maximum - hScrollBar1.LargeChange, Math.Max(0, ShiftX));
                hScrollBar1.Value = ShiftX;
            }
            if (vScrollBar1.Visible)
            {
                ShiftY = (int)((zoom_point.Y + oldShiftY) / old_zoom * Zoom - zoom_point.Y);
                ShiftY = Math.Min(vScrollBar1.Maximum - vScrollBar1.LargeChange, Math.Max(0, ShiftY));
                vScrollBar1.Value = ShiftY;
            }
            

            zooming = false;
            //if (mySettings.AllowMoreRenderUpdates)
            //{
            //    UpdateRender();
            //}

            UpdateControls();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (splitContainer1.Panel2.Controls.Count == 1)
            {
                splitContainer1.Panel2.Controls[0].Height = splitContainer1.Panel2.Height - 2;
                splitContainer1.Panel2.Controls[0].Width = splitContainer1.Panel2.Width - 2;
            }

            // TODO: It hides right now few pixels at the edge

            bool nvscrollbar = false;
            bool nhscrollbar = false;

            if (hScrollBar1.Maximum > viewPanel.Width - 2) nhscrollbar = true;
            if (vScrollBar1.Maximum > viewPanel.Height - 2) nvscrollbar = true;
            if (hScrollBar1.Maximum > viewPanel.Width - (nvscrollbar ? vScrollBar1.Width : 0)) hScrollBar1.Visible = true;
            if (vScrollBar1.Maximum > viewPanel.Height - (nhscrollbar ? hScrollBar1.Height : 0)) vScrollBar1.Visible = true;

            vScrollBar1.Visible = nvscrollbar;
            hScrollBar1.Visible = nhscrollbar;

            if (vScrollBar1.Visible)
            {
                // Docking isn't enough because we want that it will be high/wider when only one of the scroll bars is visible
                //vScrollBar1.Location = new Point(splitContainer1.SplitterDistance - 19, 0);
                vScrollBar1.Height = viewPanel.Height - (hScrollBar1.Visible ? hScrollBar1.Height : 0);
                vScrollBar1.LargeChange = vScrollBar1.Height;
                ScreenHeight = vScrollBar1.Height;
                hScrollBar1.Value = Math.Max(0, Math.Min(hScrollBar1.Value, hScrollBar1.Maximum - hScrollBar1.LargeChange));
            }
            else
            {
                ScreenHeight = GraphicPanel.Height;
                ShiftY = 0;
                vScrollBar1.Value = 0;
            }
            if (hScrollBar1.Visible)
            {
                //hScrollBar1.Location = new Point(0, splitContainer1.Height - 18);
                hScrollBar1.Width = viewPanel.Width - (vScrollBar1.Visible ? vScrollBar1.Width : 0);
                hScrollBar1.LargeChange = hScrollBar1.Width;
                ScreenWidth = hScrollBar1.Width;
                vScrollBar1.Value = Math.Max(0, Math.Min(vScrollBar1.Value, vScrollBar1.Maximum - vScrollBar1.LargeChange));
            }
            else
            {
                ScreenWidth = GraphicPanel.Width;
                ShiftX = 0;
                hScrollBar1.Value = 0;
            }

            if (hScrollBar1.Visible && vScrollBar1.Visible)
            {
                panel3.Visible = true;
                //panel3.Location = new Point(hScrollBar1.Width, vScrollBar1.Height);
            }
            else panel3.Visible = false;

            while (ScreenWidth > GraphicPanel.Width)
                ResizeGraphicPanel(GraphicPanel.Width * 2, GraphicPanel.Height);
            while (ScreenHeight > GraphicPanel.Height)
                ResizeGraphicPanel(GraphicPanel.Width, GraphicPanel.Height * 2);
                
        }

        private void SetViewSize(int width = 0, int height = 0)
        {
            if (mySettings.EntityFreeCam)
            {
                width = 10000000;
                height = 10000000;
            }

            if (!mySettings.EntityFreeCam)
            {
                vScrollBar1.Maximum = height;
                hScrollBar1.Maximum = width;
            }

            GraphicPanel.DrawWidth = Math.Min(width, GraphicPanel.Width);
            GraphicPanel.DrawHeight = Math.Min(height, GraphicPanel.Height);

            Form1_Resize(null, null);

            if (!mySettings.EntityFreeCam)
            {
                hScrollBar1.Value = Math.Max(0, Math.Min(hScrollBar1.Value, hScrollBar1.Maximum - hScrollBar1.LargeChange));
                vScrollBar1.Value = Math.Max(0, Math.Min(vScrollBar1.Value, vScrollBar1.Maximum - vScrollBar1.LargeChange));
            }
        }

        private void ResetViewSize()
        {
            SetViewSize((int)(SceneWidth * Zoom), (int)(SceneHeight * Zoom));
        }

        private void ResizeGraphicPanel(int width = 0, int height = 0)
        {
            if (mySettings.EntityFreeCam)
            {
                width = 10000000;
                height = 10000000;
            }


            GraphicPanel.Width = width;
            GraphicPanel.Height = height;

            GraphicPanel.ResetDevice();

            GraphicPanel.DrawWidth = Math.Min(hScrollBar1.Maximum, GraphicPanel.Width);
            GraphicPanel.DrawHeight = Math.Min(vScrollBar1.Maximum, GraphicPanel.Height);
        }

        #endregion

        #region Extra Layer Related Methods

        private void SetupLayerButtons()
        {
            TearDownExtraLayerButtons();
            //EDIT BUTTONS
            foreach (EditorLayer el in EditorScene.OtherLayers)
            {
                ToolStripButton tsb = new ToolStripButton(el.Name);
                toolStrip1.Items.Add(tsb);
                tsb.ForeColor = Color.LawnGreen;
                tsb.CheckOnClick = true;
                tsb.Click += AdHocLayerEdit;

                _extraLayerEditButtons.Add(tsb);
            }

            //EDIT BUTTONS SEPERATOR
            ToolStripSeparator tss = new ToolStripSeparator();
            toolStrip1.Items.Add(tss);
            _extraLayerSeperators.Add(tss);

            //VIEW BUTTONS
            foreach (EditorLayer el in EditorScene.OtherLayers)
            {
                ToolStripButton tsb = new ToolStripButton(el.Name);
                //toolStrip1.Items.Add(tsb);
                toolStrip1.Items.Insert(toolStrip1.Items.IndexOf(extraViewLayersSeperator), tsb);
                tsb.ForeColor = Color.FromArgb(0x33AD35);
                tsb.CheckOnClick = true;

                _extraLayerViewButtons.Add(tsb);
            }

            UpdateDualButtonsControlsForLayer(FGLow, ShowFGLow, EditFGLow);
            UpdateDualButtonsControlsForLayer(FGHigh, ShowFGHigh, EditFGHigh);
            UpdateDualButtonsControlsForLayer(FGLower, ShowFGLower, EditFGLower);
            UpdateDualButtonsControlsForLayer(FGHigher, ShowFGHigher, EditFGHigher);
        }

        private void TearDownExtraLayerButtons()
        {
            foreach (var elb in _extraLayerEditButtons)
            {
                elb.Click -= AdHocLayerEdit;
                toolStrip1.Items.Remove(elb);
            }
            _extraLayerEditButtons.Clear();

            foreach (var elb in _extraLayerViewButtons)
            {
                toolStrip1.Items.Remove(elb);
            }
            _extraLayerViewButtons.Clear();


            foreach (var els in _extraLayerSeperators)
            {
                toolStrip1.Items.Remove(els);
            }
            _extraLayerSeperators.Clear();

        }

        /// <summary>
        /// Given a scene layer, configure the given visibiltiy and edit buttons which will control that layer.
        /// </summary>
        /// <param name="layer">The layer of the scene from which to extract a name.</param>
        /// <param name="visibilityButton">The button which controls the visibility of the layer.</param>
        /// <param name="editButton">The button which controls editing the layer.</param>
        private void UpdateDualButtonsControlsForLayer(EditorLayer layer, ToolStripButton visibilityButton, ToolStripButton editButton)
        {
            bool layerValid = layer != null;
            visibilityButton.Checked = layerValid;
            if (layerValid)
            {
                string name = layer.Name;
                visibilityButton.Text = name;
                editButton.Text = name;
            }
        }

        private void AdHocLayerEdit(object sender, EventArgs e)
        {
            ToolStripButton tsb = sender as ToolStripButton;
            Deselect(false);
            if (tsb.Checked)
            {
                if (!mySettings.KeepLayersVisible)
                {
                    ShowFGLow.Checked = false;
                    ShowFGHigh.Checked = false;
                    ShowFGLower.Checked = false;
                    ShowFGHigher.Checked = false;
                }
                EditFGLow.Checked = false;
                EditFGHigh.Checked = false;
                EditFGLower.Checked = false;
                EditFGHigher.Checked = false;
                EditEntities.Checked = false;

                foreach (var elb in _extraLayerEditButtons)
                {
                    if (elb != tsb)
                    {
                        elb.Checked = false;
                    }
                }
            }

            UpdateControls();
        }
        #endregion

        #region Scene Loading / Unloading + Repair
        private void RepairScene()
        {
            string Result = null;
            OpenFileDialog open = new OpenFileDialog() { };
            open.Filter = "Scene File|*.bin";
            if (open.ShowDialog() != DialogResult.Cancel)
            {
                Result = open.FileName;
            }

            if (Result == null)
                return;

            UnloadScene();
            UseDefaultPrefrences();

            ObjectIDHealer healer = new ObjectIDHealer();
            ShowConsoleWindow();
            healer.startHealing(open.FileName);
            Application.DoEvents();
            HideConsoleWindow();

        }
        private bool EditorLoad()
        {
            if (DataDirectory == null)
            {
                // Old Code to be Removed Soon
                /*do
                {
                    MessageBox.Show("Please select the \"Data\" folder", "Message");
                    string newDataDirectory = GetDataDirectory();

                    // allow user to quit gracefully
                    if (string.IsNullOrWhiteSpace(newDataDirectory)) return false;
                    if (IsDataDirectoryValid(newDataDirectory))
                    {
                        DataDirectory = newDataDirectory;
                    }
                }
                while (null == DataDirectory);

                SetGameConfig();
                AddRecentDataFolder(DataDirectory);*/
                return false;
            }
            // Clears all the Textures
            EditorEntity_ini.ReleaseResources();
            return true;
        }
        void UnloadScene()
        {
            EditorScene?.Dispose();
            EditorScene = null;
            SceneFilename = null;
            SceneFilepath = null;
            StageConfig = null;
            StageConfigFileName = null;
            _levelIDLabel.Text = "Level ID: NULL";
            myEditorState.Level_ID = -1;
            encorePaletteExists = false;
            EncorePalette = null;
            EncoreSetupType = 0;
            playerObjectPosition = new List<SceneEntity> { };
            INILayerNameHigher = "";
            INILayerNameLower = "";
            SettingsReader.CleanPrefrences();
            userDefinedEntityRenderSwaps = new Dictionary<string, string>();
            userDefinedSpritePaths = new List<string>();

            t = new System.Windows.Forms.Timer
            {
                Interval = 10
            };
            t.Tick += new EventHandler(UpdateStatusPanel);
            t.Start();

            SelectedScene = null;
            SelectedZone = null;
            EncorePaletteButton.Checked = false;

            if (StageTiles != null) StageTiles.Dispose();
            StageTiles = null;

            TearDownExtraLayerButtons();

            Background = null;

            // If copying between scenes is allowed...
            if (mySettings.ForceCopyUnlock)
            {
                // ...but not for entities...
                if (mySettings.ProhibitEntityUseOnExternalClipboard)
                    // Clear local entities clipboard
                    entitiesClipboard = null;
                else if (entitiesClipboard != null)
                    // Prepare entities for external copy
                    foreach (EditorEntity entity in entitiesClipboard)
                        entity.PrepareForExternalCopy();
            }

            // If copying between scenes is NOT allowed...
            else
            {
                // Clear local clipboards
                TilesClipboard = null;
                entitiesClipboard = null;
            }

            entities = null;

            Zoom = 1;
            ZoomLevel = 0;

            undo.Clear();
            redo.Clear();

            EditFGLow.Checked = false;
            EditFGHigh.Checked = false;
            EditFGLower.Checked = false;
            EditFGHigher.Checked = false;
            EditEntities.Checked = false;

            SetViewSize();

            UpdateControls();

            // clear memory a little more aggressively 
            EditorEntity_ini.ReleaseResources();
            GC.Collect();

            CollisionLayerA.Clear();
            CollisionLayerB.Clear();
            TilesConfig = null;
        }

        private void OpenSceneForceFully()
        {
            DataDirectory = mySettings.DevForceRestartData;
            string Result = mySettings.DevForceRestartScene;
            int LevelID = mySettings.DeveForceRestartLevelID;
            bool isEncore = mySettings.DevForceRestartEncore;
            int x = mySettings.DevForceRestartX;
            int y = mySettings.DevForeRestartY;
            TempWarpCoords = new Point(x, y);
            ForceWarp = true;
            OpenScene(false, Result, LevelID, isEncore);


        }
        private void OpenSceneForceFully(string dataDir, string scenePath, string modPath, int levelID, bool isEncoreMode, int X, int Y)
        {
            DataDirectory = dataDir;
            string Result = scenePath;
            int LevelID = levelID;
            bool isEncore = isEncoreMode;
            TempWarpCoords = new Point(X, Y);
            ForceWarp = true;
            OpenScene(false, Result, LevelID, isEncore, (modPath != "" ? true : false), modPath);
        }

        private void OpenSceneForceFully(string dataDir)
        {
            DataDirectory = dataDir;
            OpenScene();
        }

        private void OpenScene(bool manual = false, string Result = null, int LevelID = -1, bool isEncore = false, bool modLoaded = false, string modDir = "")
        {
            SceneSelect select;
            ModDataDirectory = modDir; 
            string ResultPath = null;
            if (Result == null)
            {
                if (manual == false)
                {

                    if (!EditorLoad())
                    {
                        select = new SceneSelect();
                    }
                    else
                    {
                        select = new SceneSelect(GameConfig);
                    }
                    select.ShowDialog();
                    Result = select.Result;
                    LevelID = select.LevelID;
                    isEncore = select.isEncore;
                    ResultPath = Path.GetDirectoryName(Result);
                    modLoaded = (select.isModLoadedwithGameConfig);

                }
                else
                {
                    OpenFileDialog open = new OpenFileDialog
                    {
                        Filter = "Scene File|*.bin"
                    };
                    if (open.ShowDialog() != DialogResult.Cancel)
                    {
                        Result = open.FileName;
                    }


                }
            }


            if (Result == null)
            {
                return;
            }

            string _DataDirectory = (modLoaded == true ? Editor.ModDataDirectory : Editor.DataDirectory);
            

            ResultPath = Path.GetDirectoryName(Result);
            UnloadScene();
            UseDefaultPrefrences();
            bool goodGameConfig = SetGameConfig();
            if (goodGameConfig == false)
            {
                return;
            }

            if (isEncore)
            {
                EncorePaletteButton.Checked = true;
                useEncoreColors = true;
            }
            try
            {
                if (File.Exists(Result))
                {
                    OpenScenefromBrowse(Result, _DataDirectory, LevelID);
                }
                else
                {
                    OpenScenefromSceneSelect(Result, _DataDirectory, LevelID, modLoaded);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Load failed. Error: " + ex.ToString() + " " + Result);
                return;
            }

            UpdateDataFolderLabel(null, null);

            SetupLayerButtons();

            Background = new EditorBackground();

            entities = new EditorEntities(EditorScene);

            SetViewSize(SceneWidth, SceneHeight);

            UpdateControls(true);

        }

        private void OpenScenefromSceneSelect(string Result, string _DataDirectory, int LevelID, bool modLoaded)
        {       
                int searchType = 1;
                SelectedZone = Result.Replace(Path.GetFileName(Result), "");
                SelectedScene = Path.GetFileName(Result);
                SceneFilename = Path.Combine(_DataDirectory, "Stages", SelectedZone, SelectedScene);
                SceneFilepath = Path.Combine(_DataDirectory, "Stages", SelectedZone);               
                SelectedZone = SelectedZone.Replace("\\", "");
                myEditorState.Level_ID = LevelID;
                EditorScene = new EditorScene(SceneFilename, GraphicPanel);
                //Encore Palette + Stage Tiles Initaliazation
                EncorePalette = EditorScene.getEncorePalette(SelectedZone, _DataDirectory, SelectedScene, Result, searchType);
                EncoreSetupType = EditorScene.GetEncoreSetupType(SelectedZone, _DataDirectory, SelectedScene, Result);
                if (EncorePalette[0] != "")
                {
                    encorePaletteExists = true;
                }
                //Encore Palette + Stage Tiles
                if (useEncoreColors == true && EncorePalette[0] != "")
                {
                    StageTiles = new StageTiles(Path.Combine(_DataDirectory, "Stages", SelectedZone), EncorePalette[0]);
                }
                else
                {
                    StageTiles = new StageTiles(Path.Combine(_DataDirectory, "Stages", SelectedZone));
                }
                //These cause issues, but not clearing them means when new stages are loaded Collision Mask 0 will be index 1024... (I think)
                CollisionLayerA.Clear();
                CollisionLayerB.Clear();
                if (StageTiles != null && File.Exists(Path.Combine(SceneFilepath, "TileConfig.bin")))
                {

                TilesConfig = new TilesConfig(Path.Combine(SceneFilepath, "TileConfig.bin"));
                    for (int i = 0; i < 1024; i++)
                    {
                        CollisionLayerA.Add(TilesConfig.CollisionPath1[i].DrawCMask(Color.FromArgb(0, 0, 0, 0), CollisionAllSolid));
                        CollisionLayerB.Add(TilesConfig.CollisionPath2[i].DrawCMask(Color.FromArgb(0, 0, 0, 0), CollisionAllSolid));
                    }
                }
                // Object Rescue Mode
                if (mySettings.DisableEntityReading == true)
                {
                    RSDKv5.Scene.readTilesOnly = true;
                }
                else
                {
                    RSDKv5.Scene.readTilesOnly = false;
                }
                StageConfigFileName = Path.Combine(Path.GetDirectoryName(SceneFilename), "StageConfig.bin");
                if (File.Exists(StageConfigFileName))
                {
                    StageConfig = new StageConfig(StageConfigFileName);
                }
                ObjectList.Clear();
                for (int i = 0; i < GameConfig.ObjectsNames.Count; i++)
                {
                    ObjectList.Add(GameConfig.ObjectsNames[i]);
                }
                for (int i = 0; i < StageConfig.ObjectsNames.Count; i++)
                {
                    ObjectList.Add(StageConfig.ObjectsNames[i]);
                }
                ScenePath = Result;
                UpdateDiscord("Editing " + Result);

                if (File.Exists(SceneFilepath + "\\maniac.ini"))
                {
                    bool allowToRead = false;
                    using (Stream stream = SettingsReader.GetSceneIniResource(SceneFilepath + "\\maniac.ini"))
                    {
                        if (stream != null)
                        {
                            SettingsReader.GetSceneINISettings(stream);
                            allowToRead = true;
                        }
                        else
                        {
                            Debug.Print("Something went wrong");
                            allowToRead = false;
                        }
                    }
                    if (allowToRead)
                    {
                        try
                        {
                            SetINIDefaultPrefrences();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Failed to Inturpret INI File. Error: " + ex.ToString() + " " + Result);
                            SettingsReader.CleanPrefrences();
                        }

                    }


                }

            
        }

        private void OpenScenefromBrowse(string Result, string _DataDirectory, int LevelID)
        {
            int searchType = 0;
            // Selected file
            // Don't forget to populate these Members
            string directoryPath = Path.GetDirectoryName(Result);
            SelectedZone = new DirectoryInfo(directoryPath).Name;
            SelectedScene = Path.GetFileName(Result);
            SceneFilename = Result;
            SceneFilepath = Path.Combine(directoryPath);
            searchType = 0;
            SelectedZone = SelectedZone.Replace("\\", "");
            myEditorState.Level_ID = LevelID;
            EditorScene = new EditorScene(SceneFilename, GraphicPanel);
            //Encore Palette + Stage Tiles Initaliazation
            EncorePalette = EditorScene.getEncorePalette(SelectedZone, _DataDirectory, SelectedScene, Result, searchType);
            EncoreSetupType = EditorScene.GetEncoreSetupType(SelectedZone, _DataDirectory, SelectedScene, Result);
            if (EncorePalette[0] != "")
            {
                encorePaletteExists = true;
            }
            //Encore Palette + Stage Tiles
            //string directoryPath = Path.GetDirectoryName(Result);
            if (useEncoreColors == true && EncorePalette[0] != "")
            {
                StageTiles = new StageTiles(directoryPath, EncorePalette[0]);
            }
            else
            {
                StageTiles = new StageTiles(directoryPath);
            }
            //These cause issues, but not clearing them means when new stages are loaded Collision Mask 0 will be index 1024... (I think)
            CollisionLayerA.Clear();
            CollisionLayerB.Clear();

            if (StageTiles != null && File.Exists(Path.Combine(SceneFilepath, "TileConfig.bin")))
            {

                TilesConfig = new TilesConfig(Path.Combine(SceneFilepath, "TileConfig.bin"));
                for (int i = 0; i < 1024; i++)
                {
                    CollisionLayerA.Add(TilesConfig.CollisionPath1[i].DrawCMask(Color.FromArgb(0, 0, 0, 0), CollisionAllSolid));
                    CollisionLayerB.Add(TilesConfig.CollisionPath2[i].DrawCMask(Color.FromArgb(0, 0, 0, 0), CollisionAllSolid));
                }
            }

            // Object Rescue Mode
            if (mySettings.DisableEntityReading == true) RSDKv5.Scene.readTilesOnly = true;
            else RSDKv5.Scene.readTilesOnly = false;

            StageConfigFileName = Path.Combine(Path.GetDirectoryName(SceneFilename), "StageConfig.bin");
            if (File.Exists(StageConfigFileName)) StageConfig = new StageConfig(StageConfigFileName);
            ObjectList.Clear();

            for (int i = 0; i < GameConfig.ObjectsNames.Count; i++)
            {
                ObjectList.Add(GameConfig.ObjectsNames[i]);
            }
            for (int i = 0; i < StageConfig.ObjectsNames.Count; i++)
            {
                ObjectList.Add(StageConfig.ObjectsNames[i]);
            }
            ScenePath = Result;
            UpdateDiscord("Editing " + Result);

            if (File.Exists(SceneFilepath + "\\maniac.ini"))
            {
                bool allowToRead = false;
                using (Stream stream = SettingsReader.GetSceneIniResource(SceneFilepath + "\\maniac.ini"))
                {
                    if (stream != null)
                    {
                        SettingsReader.GetSceneINISettings(stream);
                        allowToRead = true;
                    }
                    else
                    {
                        Debug.Print("Something went wrong");
                        allowToRead = false;
                    }
                }
                if (allowToRead)
                {
                    try
                    {
                        SetINIDefaultPrefrences();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Failed to Inturpret INI File. Error: " + ex.ToString() + " " + Result);
                        SettingsReader.CleanPrefrences();
                    }

                }


            }
        }

        #endregion

        #region File Tab Buttons
        public void Open_Click(object sender, EventArgs e)
        {
            Editor.Instance.SceneChangeWarning(sender, e);
            if (AllowSceneChange == true || IsSceneLoaded() == false || mySettings.DisableSaveWarnings == true)
            {
                AllowSceneChange = false;
                OpenScene(mySettings.forceBrowse);

            }
            else
            {
                return;
            }

        }

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Open_Click(sender, e);
        }

        public void OpenDataDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SceneChangeWarning(sender, e);
            if (AllowSceneChange == true || IsSceneLoaded() == false)
            {
                AllowSceneChange = false;
                string newDataDirectory = GetDataDirectory();
                if (null == newDataDirectory) return;
                if (newDataDirectory.Equals(DataDirectory)) return;

                if (IsDataDirectoryValid(newDataDirectory))
                    ResetDataDirectoryToAndResetScene(newDataDirectory);
                else
                    MessageBox.Show($@"{newDataDirectory} is not
a valid Data Directory.",
                                    "Invalid Data Directory!",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
            }
            else
            {
                return;
            }

        }

        public void Save_Click(object sender, EventArgs e)
        {
            if (EditorScene == null) return;

            if (IsTilesEdit())
            {
                // Apply changes
                Deselect();
            }

            try
            {
                EditorScene.Save(SceneFilename);
            }
            catch (Exception ex)
            {
                ShowError($@"Failed to save the scene to file '{SceneFilename}'
Error: {ex.Message}");
            }

            try
            {
                StageConfig?.Write(StageConfigFileName);
            }
            catch (Exception ex)
            {
                ShowError($@"Failed to save the StageConfig to file '{StageConfigFileName}'
Error: {ex.Message}");
            }
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void SaveAspngToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (EditorScene == null) return;

            SaveFileDialog save = new SaveFileDialog
            {
                Filter = ".png File|*.png",
                DefaultExt = "png"
            };
            if (save.ShowDialog() != DialogResult.Cancel)
            {
                using (Bitmap bitmap = new Bitmap(SceneWidth, SceneHeight))
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    // not all scenes have both a Low and a High foreground
                    // only attempt to render the ones we actually have
                    FGLower?.Draw(g);
                    FGLow?.Draw(g);
                    FGHigh?.Draw(g);
                    FGHigher?.Draw(g);
                    entities?.Draw(g);
                    bitmap.Save(save.FileName);
                }
            }
        }

        private void ExportEachLayerAspngToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (EditorScene?.Layers == null || !EditorScene.Layers.Any()) return;

                var dialog = new FolderSelectDialog()
                {
                    Title = "Select folder to save each exported layer image to"
                };

                if (!dialog.ShowDialog()) return;

                int fileCount = 0;

                foreach (var editorLayer in EditorScene.AllLayers)
                {
                    string fileName = Path.Combine(dialog.FileName, editorLayer.Name + ".png");

                    if (!CanWriteFile(fileName))
                    {
                        ShowError($"Layer export aborted. {fileCount} images saved.");
                        return;
                    }

                    using (var bitmap = new Bitmap(editorLayer.Width * EditorLayer.TILE_SIZE, editorLayer.Height * EditorLayer.TILE_SIZE))
                    using (var g = Graphics.FromImage(bitmap))
                    {
                        editorLayer.Draw(g);
                        bitmap.Save(fileName, ImageFormat.Png);
                        ++fileCount;
                    }
                }

                MessageBox.Show($"Layer export succeeded. {fileCount} images saved.", "Success!",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                ShowError("An error occurred: " + ex.Message);
            }
        }

        public void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (EditorScene == null) return;

            if (IsTilesEdit())
            {
                // Apply changes
                Deselect();
            }

            SaveFileDialog save = new SaveFileDialog
            {
                Filter = "Scene File|*.bin",
                DefaultExt = "bin",
                InitialDirectory = Path.GetDirectoryName(SceneFilename),
                RestoreDirectory = false,
                FileName = Path.GetFileName(SceneFilename)
            };
            if (save.ShowDialog() != DialogResult.Cancel)
            {
                EditorScene.Write(save.FileName);
            }
        }

        private void BackupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BackupTool(null, null);
        }

        private void BackupRecoverButton_Click(object sender, EventArgs e)
        {
            string Result = null, ResultOriginal = null, ResultOld = null;
            OpenFileDialog open = new OpenFileDialog
            {
                Filter = "Backup Scene|*.bin.bak|Old Scene|*.bin.old|Crash Backup Scene|*.bin.crash.bak"
            };
            if (open.ShowDialog() != DialogResult.Cancel)
            {
                Result = open.FileName;
                ResultOriginal = Result.Split('.')[0] + ".bin";
                ResultOld = ResultOriginal + ".old";
                int i = 1;
                while ((File.Exists(ResultOld)))
                {
                    ResultOld = ResultOriginal.Substring(0, ResultOriginal.Length - 4) + "." + i + ".bin.old";
                    i++;
                }



            }

            if (Result == null)
                return;

            UnloadScene();
            UseDefaultPrefrences();
            File.Replace(Result, ResultOriginal, ResultOld);

        }

        private void RemoveObjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {

                using (var ObjectRemover = new ObjectManager(EditorScene.Objects, StageConfig))
                {
                    if (ObjectRemover.ShowDialog() != DialogResult.OK)
                        return; // nothing to do

                    // user clicked Import, get to it!
                    UpdateControls();
                    entitiesToolbar?.RefreshObjects(EditorScene.Objects);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to import Objects. " + ex.Message);
            }
        }

        private void UnloadSceneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UnloadScene();
        }

        #region Backup SubMenu
        private void StageConfigToolStripMenuItem_Click(object sender, EventArgs e)
        {
            backupType = 4;
            BackupToolStripMenuItem_Click(null, null);
            backupType = 0;
        }

        private void NormalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            backupType = 1;
            BackupToolStripMenuItem_Click(null, null);
            backupType = 0;
        }
        #endregion

        #endregion

        #region Edit Tab Buttons
        public void FlipHorizontalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditLayer?.FlipPropertySelected(FlipDirection.Horizontal);
            UpdateEditLayerActions();
        }

        public void FlipHorizontalIndividualToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditLayer?.FlipPropertySelected(FlipDirection.Horizontal, true);
            UpdateEditLayerActions();
        }

        private void DeleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteSelected();
        }

        public void CopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (IsTilesEdit())
                CopyTilesToClipboard();


            else if (IsEntitiesEdit())
                CopyEntitiesToClipboard();


            UpdateControls();
        }

        public void DuplicateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (IsTilesEdit())
            {
                EditLayer.PasteFromClipboard(new Point(16, 16), EditLayer.CopyToClipboard(true));
                UpdateEditLayerActions();
            }
            else if (IsEntitiesEdit())
            {
                try
                {
                    entities.PasteFromClipboard(new Point(16, 16), entities.CopyToClipboard(true));
                    UpdateLastEntityAction();
                }
                catch (EditorEntities.TooManyEntitiesException)
                {
                    MessageBox.Show("Too many entities! (limit: 2048)");
                    return;
                }
                UpdateEntitiesToolbarList();
                SetSelectOnlyButtonsState();
            }
        }

        private void UndoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditorUndo();
        }

        private void RedoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditorRedo();
        }

        public void CutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (IsTilesEdit())
            {
                CopyTilesToClipboard();
                DeleteSelected();
                UpdateControls();
                UpdateEditLayerActions();
            }
            else if (IsEntitiesEdit())
            {
                if (entitiesToolbar.ContainsFocus.Equals(false))
                {
                    CopyEntitiesToClipboard();
                    DeleteSelected();
                    UpdateControls();
                }
            }
        }

        public void PasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (IsTilesEdit())
            {
                // check if there are tiles on the Windows clipboard; if so, use those
                if (mySettings.EnableWindowsClipboard && Clipboard.ContainsData("ManiacTiles"))
                {
                    EditLayer.PasteFromClipboard(new Point((int)(lastX / Zoom) + EditorLayer.TILE_SIZE - 1, (int)(lastY / Zoom) + EditorLayer.TILE_SIZE - 1), (Dictionary<Point, ushort>)Clipboard.GetDataObject().GetData("ManiacTiles"));
                    UpdateEditLayerActions();
                }

                // if there's none, use the internal clipboard
                else if (TilesClipboard != null)
                {
                    EditLayer.PasteFromClipboard(new Point((int)(lastX / Zoom) + EditorLayer.TILE_SIZE - 1, (int)(lastY / Zoom) + EditorLayer.TILE_SIZE - 1), TilesClipboard);
                    UpdateEditLayerActions();
                }

            }
            else if (IsEntitiesEdit())
            {
                if (entitiesToolbar.ContainsFocus.Equals(false))
                {
                    try
                    {
                        // check if there are entities on the Windows clipboard; if so, use those
                        if (mySettings.EnableWindowsClipboard && Clipboard.ContainsData("ManiacEntities"))
                        {
                            entities.PasteFromClipboard(new Point((int)(lastX / Zoom), (int)(lastY / Zoom)), (List<EditorEntity>)Clipboard.GetDataObject().GetData("ManiacEntities"));
                            UpdateLastEntityAction();
                        }

                        // if there's none, use the internal clipboard
                        else if (entitiesClipboard != null)
                        {
                            entities.PasteFromClipboard(new Point((int)(lastX / Zoom), (int)(lastY / Zoom)), entitiesClipboard);
                            UpdateLastEntityAction();
                        }
                    }
                    catch (EditorEntities.TooManyEntitiesException)
                    {
                        MessageBox.Show("Too many entities! (limit: 2048)");
                        return;
                    }
                    UpdateEntitiesToolbarList();
                    SetSelectOnlyButtonsState();
                }
            }
        }

        public void FlipVerticalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditLayer?.FlipPropertySelected(FlipDirection.Veritcal);
            UpdateEditLayerActions();
        }

        public void FlipVerticalIndividualToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditLayer?.FlipPropertySelected(FlipDirection.Veritcal, true);
            UpdateEditLayerActions();
            UpdateEditLayerActions();
        }

        #endregion

        #region View Tab Buttons

        private void ShowEntitiesAboveAllOtherLayersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (showEntitiesAboveAllOtherLayersToolStripMenuItem.Checked)
            {
                entityVisibilityType = 1;
            }
            else
            {
                entityVisibilityType = 0;
            }

        }

        private void ChangeEncorePaleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetEncorePalete(sender);
        }

        private void SetEncorePalete(object sender = null, string path = "")
        {
            if (sender != null)
            {
                ToolStripMenuItem clickedItem = sender as ToolStripMenuItem;
                bool useModFolder = (clickedItem == fromModDirectoryToolStripMenuItem) && ModDataDirectory != "" && Directory.Exists(Path.Combine(ModDataDirectory, "Palettes"));
                string StartDir = (useModFolder ? ModDataDirectory : DataDirectory);
                try
                {
                    using (var fd = new OpenFileDialog())
                    {
                        fd.Filter = "Color Palette File|*.act";
                        fd.DefaultExt = ".act";
                        fd.Title = "Select an Encore Color Palette";
                        fd.InitialDirectory = Path.Combine(StartDir, "Palettes");
                        if (fd.ShowDialog() == DialogResult.OK)
                        {
                            EncorePalette = EditorScene.getEncorePalette("", "", "", "", -1, fd.FileName);
                            EncoreSetupType = 0;
                            if (File.Exists(EncorePalette[0]))
                            {
                                encorePaletteExists = true;
                                EncorePaletteButton.Checked = true;
                                useEncoreColors = true;
                                ReloadSpecificTextures(null, null);
                            }

                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unable to set Encore Colors. " + ex.Message);
                }
            }
            else if (path != "")
            {
                EncorePalette = EditorScene.getEncorePalette("", "", "", "", -1, path);
                EncoreSetupType = 0;
                if (File.Exists(EncorePalette[0]))
                {
                    encorePaletteExists = true;
                    EncorePaletteButton.Checked = true;
                    useEncoreColors = true;
                    ReloadSpecificTextures(null, null);
                }
                else
                {
                    MessageBox.Show("Unable to set Encore Colors. The Specified Path does not exist: " + Environment.NewLine + path);
                }
            }

        }

        private void MoveExtraLayersToFrontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (moveExtraLayersToFrontToolStripMenuItem.Checked)
            {
                extraLayersMoveToFront = true;
            }
            else
            {
                extraLayersMoveToFront = false;
            }
        }

        private void ToolStripTextBox1_TextChanged(object sender, EventArgs e)
        {
            entitiesTextFilter = toolStripTextBox1.Text;
            EditorEntities.FilterRefreshNeeded = true;
        }

        private void ShowEntitySelectionBoxesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (myEditorState.ShowEntitySelectionBoxes)
            {
                showEntitySelectionBoxesToolStripMenuItem.Checked = false;
                myEditorState.ShowEntitySelectionBoxes = false;
            }
            else
            {
                showEntitySelectionBoxesToolStripMenuItem.Checked = true;
                myEditorState.ShowEntitySelectionBoxes = true;
            }
        }

        private void ShowWaterLevelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (showWaterLevelToolStripMenuItem.Checked)
            {
                showWaterLevel = true;
            }
            else
            {
                showWaterLevel = false;
            }
        }

        private void WaterLevelAlwaysShowItem_Click(object sender, EventArgs e)
        {
            if (waterLevelAlwaysShowItem.Checked)
            {
                alwaysShowWaterLevel = true;
            }
            else
            {
                alwaysShowWaterLevel = false;
            }
        }

        private void SizeWithBoundsWhenNotSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sizeWithBoundsWhenNotSelectedToolStripMenuItem.Checked)
            {
                sizeWaterLevelwithBounds = true;
            }
            else
            {
                sizeWaterLevelwithBounds = false;
            }
        }

        private void ToggleEncoreManiaObjectVisibilityToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToggleEncoreManiaEntitiesToolStripMenuItem_Click(sender, e);
        }

        private void ShowParallaxSpritesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (showParallaxSpritesToolStripMenuItem.Checked)
            {
                myEditorState.ShowParallaxSprites = true;
            }
            else
            {
                myEditorState.ShowParallaxSprites = false;
            }
        }

        private void XToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mySettings.ScrollLockDirection)
            {
                mySettings.ScrollLockDirection = false;
                xToolStripMenuItem.Checked = false;
                yToolStripMenuItem.Checked = true;
                scrollLockDirLabel.Text = "Scroll Lock Direction: Y";
            }
            else
            {
                mySettings.ScrollLockDirection = true;
                xToolStripMenuItem.Checked = true;
                yToolStripMenuItem.Checked = false;
                scrollLockDirLabel.Text = "Scroll Lock Direction: X";
            }
        }

        private void YToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mySettings.ScrollLockDirection)
            {
                mySettings.ScrollLockDirection = false;
                xToolStripMenuItem.Checked = false;
                yToolStripMenuItem.Checked = true;
            }
            else
            {
                mySettings.ScrollLockDirection = true;
                xToolStripMenuItem.Checked = true;
                yToolStripMenuItem.Checked = false;
            }
        }

        private void ShowEntityPathToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (showEntityPathArrowsToolstripItem.Checked)
            {
                showEntityPathArrows = true;
            }
            else
            {
                showEntityPathArrows = false;
            }
        }

        #endregion

        #region Scene Tab Buttons
        public void ImportObjectsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Editor.Instance.importingObjects = true;
            try
            {
                Scene sourceScene = GetSceneSelection();
                if (sourceScene == null) return;

                using (var objectImporter = new ObjectImporter(sourceScene.Objects, EditorScene.Objects, StageConfig))
                {
                    if (objectImporter.ShowDialog() != DialogResult.OK)
                        return; // nothing to do

                    // user clicked Import, get to it!
                    UpdateControls();
                    entitiesToolbar?.RefreshObjects(EditorScene.Objects);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to import Objects. " + ex.Message);
            }
            Editor.Instance.importingObjects = false;
        }

        private void ImportSoundsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                StageConfig sourceStageConfig = null;
                using (var fd = new OpenFileDialog())
                {
                    fd.Filter = "Stage Config File|*.bin";
                    fd.DefaultExt = ".bin";
                    fd.Title = "Select Stage Config File";
                    fd.InitialDirectory = Path.Combine(DataDirectory, "Stages");
                    if (fd.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            sourceStageConfig = new StageConfig(fd.FileName);
                        }
                        catch
                        {
                            MessageBox.Show("Ethier this isn't a stage config, or this stage config is ethier corrupted or unreadable in Maniac.");
                            return;
                        }

                    }
                }
                if (null == sourceStageConfig) return;

                using (var soundImporter = new SoundImporter(sourceStageConfig, StageConfig))
                {
                    if (soundImporter.ShowDialog() != DialogResult.OK)
                        return; // nothing to do

                    // changing the sound list doesn't require us to do anything either
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to import sounds. " + ex.Message);
            }
        }

        private void LayerManagerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Deselect(true);

            using (var lm = new LayerManager(EditorScene))
            {
                lm.ShowDialog();
            }
            controlWindowOpen = true;

            SetupLayerButtons();
            ResetViewSize();
            UpdateControls();
        }

        private void PrimaryColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorPickerDialog colorSelect = new ColorPickerDialog
            {
                ForeColor = Color.Black,
                BackColor = Color.White,
                Color = Color.FromArgb(EditorScene.EditorMetadata.BackgroundColor1.R, EditorScene.EditorMetadata.BackgroundColor1.G, EditorScene.EditorMetadata.BackgroundColor1.B)
            };
            DialogResult result = colorSelect.ShowDialog();
            if (result == DialogResult.OK)
            {
                {
                    RSDKv5.Color returnColor = new RSDKv5.Color
                    {
                        R = colorSelect.Color.R,
                        A = colorSelect.Color.A,
                        B = colorSelect.Color.B,
                        G = colorSelect.Color.G
                    };
                    EditorScene.EditorMetadata.BackgroundColor1 = returnColor;
                }
              
            }
        }

        private void SecondaryColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorPickerDialog colorSelect = new ColorPickerDialog
            {
                ForeColor = Color.Black,
                BackColor = Color.White,
                Color = Color.FromArgb(EditorScene.EditorMetadata.BackgroundColor2.R, EditorScene.EditorMetadata.BackgroundColor2.G, EditorScene.EditorMetadata.BackgroundColor2.B)
            };
            DialogResult result = colorSelect.ShowDialog();
            if (result == DialogResult.OK)
            {
                {
                    RSDKv5.Color returnColor = new RSDKv5.Color
                    {
                        R = colorSelect.Color.R,
                        A = colorSelect.Color.A,
                        B = colorSelect.Color.B,
                        G = colorSelect.Color.G
                    };
                    EditorScene.EditorMetadata.BackgroundColor2 = returnColor;
                }

            }
        }

        #endregion

        #region Tools Tab Buttons

        private void OptimizeEntitySlotIDsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (EditorScene != null)
            {
                entities.OptimizeSlotIDs();
            }
        }

        private void MakeForDataFolderOnlyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string dataDir = DataDirectory;
            CreateShortcut(dataDir);
        }

        private void WithCurrentCoordinatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string dataDir = DataDirectory;
            string scenePath = ScenePath;
            string modPath = ModDataDirectory;
            int rX = (short)(ShiftX / Zoom);
            int rY = (short)(ShiftY / Zoom);
            bool isEncoreSet = Editor.Instance.useEncoreColors;
            int levelSlotNum = Editor.Instance.myEditorState.Level_ID;
            CreateShortcut(dataDir, scenePath, modPath, rX, rY, isEncoreSet, levelSlotNum);
        }

        private void WithoutCurrentCoordinatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string dataDir = DataDirectory;
            string scenePath = ScenePath;
            string modPath = ModDataDirectory;
            int rX = 0;
            int rY = 0;
            bool isEncoreSet = Editor.Instance.useEncoreColors;
            int levelSlotNum = Editor.Instance.myEditorState.Level_ID;
            CreateShortcut(dataDir, scenePath, modPath, rX, rY, isEncoreSet, levelSlotNum);
        }

        private void SoundLooperToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SoundLooper form = new SoundLooper();
            form.ShowDialog();
        }

        #endregion

        #region Apps Tab Buttons

        private void TileManiacToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mainform.IsDisposed) mainform = new TileManiac.Mainform();
            mainform.Show();
            if (Editor.Instance.TilesConfig != null && Editor.Instance.StageTiles != null)
            {
                if (!mainform.Visible || Editor.Instance.mainform.tcf == null)
                {
                    mainform.LoadTileConfigViaIntergration(TilesConfig, SceneFilepath);
                }
                else
                {
                    mainform.Activate();
                }

            }

        }
        private void InsanicManiacToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Sanic2Maniac sanic = new Sanic2Maniac();
            sanic.Show();
        }
        private void RSDKAnnimationEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            String aniProcessName = Path.GetFileNameWithoutExtension(mySettings.RunAniEdPath);
            IntPtr hWnd = FindWindow(aniProcessName, null); // this gives you the handle of the window you need.
            Process processes = Process.GetProcessesByName(aniProcessName).FirstOrDefault();
            if (processes != null)
            {
                // check if the window is hidden / minimized
                if (processes.MainWindowHandle == IntPtr.Zero)
                {
                    // the window is hidden so try to restore it before setting focus.
                    ShowWindow(processes.Handle, ShowWindowEnum.Restore);
                }

                // set user the focus to the window
                SetForegroundWindow(processes.MainWindowHandle);
            }
            else
            {

                // Ask where RSDK Annimation Editor is located when not set
                if (string.IsNullOrEmpty(mySettings.RunAniEdPath))
                {
                    var ofd = new OpenFileDialog
                    {
                        Title = "Select RSDK Animation Editor.exe",
                        Filter = "Windows PE Executable|*.exe"
                    };
                    if (ofd.ShowDialog() == DialogResult.OK)
                        mySettings.RunAniEdPath = ofd.FileName;
                }
                else
                {
                    if (!File.Exists(mySettings.RunGamePath))
                    {
                        mySettings.RunAniEdPath = "";
                        return;
                    }
                }

                ProcessStartInfo psi;
                psi = new ProcessStartInfo(mySettings.RunAniEdPath);
                Process.Start(psi);
            }
        }

        private void CToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void ColorPaletteEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            String palleteProcessName = Path.GetFileNameWithoutExtension(mySettings.RunPalleteEditorPath);
            IntPtr hWnd = FindWindow(palleteProcessName, null); // this gives you the handle of the window you need.
            Process processes = Process.GetProcessesByName(palleteProcessName).FirstOrDefault();
            if (processes != null)
            {
                // check if the window is hidden / minimized
                if (processes.MainWindowHandle == IntPtr.Zero)
                {
                    // the window is hidden so try to restore it before setting focus.
                    ShowWindow(processes.Handle, ShowWindowEnum.Restore);
                }

                // set user the focus to the window
                SetForegroundWindow(processes.MainWindowHandle);
            }
            else
            {
                // Ask where Color Palette Program is located when not set
                if (string.IsNullOrEmpty(mySettings.RunPalleteEditorPath))
                {
                    var ofd = new OpenFileDialog
                    {
                        Title = "Select Color Palette Program (.exe)",
                        Filter = "Windows PE Executable|*.exe"
                    };
                    if (ofd.ShowDialog() == DialogResult.OK)
                        mySettings.RunPalleteEditorPath = ofd.FileName;
                }
                else
                {
                    if (!File.Exists(mySettings.RunPalleteEditorPath))
                    {
                        mySettings.RunPalleteEditorPath = "";
                        return;
                    }
                }

                if (File.Exists(mySettings.RunPalleteEditorPath))
                    Process.Start(mySettings.RunPalleteEditorPath);
            }
        }

        private void DuplicateObjectIDHealerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("WARNING: Once you do this the editor will restart immediately, make sure your progress is closed and saved!", "WARNING", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            if (result == DialogResult.OK)
            {
                RepairScene();
            }
        }

        #endregion

        #region Folders Tab Buttons
        private void OpenSceneFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SceneFilename != null && SceneFilename != "" && File.Exists(SceneFilename))
            {
                string SceneFilename_mod = SceneFilename.Replace('/', '\\');
                Process.Start("explorer.exe", "/select, " + SceneFilename_mod);
            }
            else
            {
                MessageBox.Show("Scene File does not exist or simply isn't loaded!", "ERROR");
            }

        }

        private void OpenDataDirectoryFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string DataDirectory_mod = DataDirectory.Replace('/', '\\');          
            if (DataDirectory_mod != null && DataDirectory_mod != "" && Directory.Exists(DataDirectory_mod))
            {
                Process.Start("explorer.exe", "/select, " + DataDirectory_mod);
            }
            else
            {
                MessageBox.Show("Data Directory does not exist or simply isn't loaded!", "ERROR");
            }

        }

        private void OpenSonicManiaFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mySettings.RunGamePath != null && mySettings.RunGamePath != "" && File.Exists(mySettings.RunGamePath))
            {
                string GameFolder = mySettings.RunGamePath;
                string GameFolder_mod = GameFolder.Replace('/', '\\');
                Process.Start("explorer.exe", "/select, " + GameFolder_mod);
            }
            else
            {
                MessageBox.Show("Game Folder does not exist or isn't set!", "ERROR");
            }

        }

        private void OpenModDataDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ModDataDirectory != "")
            {
                string ModDataDir = ModDataDirectory.Replace('/', '\\');
                Process.Start("explorer.exe", "/select, " + ModDataDir);
            }
            else
            {
                MessageBox.Show("Mod Data Directory Not Loaded!", "ERROR");
            }


        }
        private void OpenASavedPlaceToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            if (Settings.mySettings.SavedPlaces != null && Settings.mySettings.SavedPlaces.Count > 0)
            {
                openASavedPlaceToolStripMenuItem.DropDownItems.RemoveAt(0);
                foreach (string savedPlace in Settings.mySettings.SavedPlaces)
                {
                    openASavedPlaceToolStripMenuItem.DropDownItems.Add(savedPlace, null, OpenASavedPlaceTrigger);
                }
            }

        }

        private void OpenASavedPlaceTrigger(object sender, EventArgs e)
        {
            ToolStripDropDownItem item = sender as ToolStripDropDownItem;
            string savedPlaceDir = item.Text.Replace('/', '\\');
            Process.Start("explorer.exe", "/select, " + savedPlaceDir);
        }

        private void OpenASavedPlaceToolStripMenuItem_DropDownClosed(object sender, EventArgs e)
        {
            openASavedPlaceToolStripMenuItem.DropDownItems.Clear();
            openASavedPlaceToolStripMenuItem.DropDownItems.Add("No Saved Places");
            openASavedPlaceToolStripMenuItem.DropDownItems[0].Enabled = false;
        }

        #endregion

        #region Other Tab Buttons
        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var aboutBox = new AboutBox())
            {
                aboutBox.ShowDialog();
            }
        }

        private void OptionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var optionBox = new OptionBox())
            {
                optionBox.ShowDialog();
            }
        }

        private void ControlsToolStripMenuItem_Click(object sender, EventArgs e)
        {

            using (var ControlBox = new ControlBox())
            {
                ControlBox.ShowDialog();
            }
        }
        #endregion

        #region Main Toolstrip Buttons
        private void NewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //New_Click(sender, e);
        }

        private void SToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Save_Click(sender, e);
        }

        private void MagnetMode_Click(object sender, EventArgs e)
        {
            if (UseMagnetMode)
            {
                UseMagnetMode = false;
                MagnetMode.Checked = false;
            }
            else
            {
                UseMagnetMode = true;
                MagnetMode.Checked = true;
            }
        }

        private void UndoButton_Click(object sender, EventArgs e)
        {
            EditorUndo();
        }

        private void RedoButton_Click(object sender, EventArgs e)
        {
            EditorRedo();
        }

        private void ZoomInButton_Click(object sender, EventArgs e)
        {
            ZoomLevel += 1;
            if (ZoomLevel >= 5) ZoomLevel = 5;
            if (ZoomLevel <= -5) ZoomLevel = -5;

            SetZoomLevel(ZoomLevel, new Point(0, 0));
        }

        private void ZoomOutButton_Click(object sender, EventArgs e)
        {
            ZoomLevel -= 1;
            if (ZoomLevel >= 5) ZoomLevel = 5;
            if (ZoomLevel <= -5) ZoomLevel = -5;

            SetZoomLevel(ZoomLevel, new Point(0, 0));
        }

        private void SelectTool_Click(object sender, EventArgs e)
        {
            SelectToolButton.Checked = !SelectToolButton.Checked;
            PointerButton.Checked = false;
            PlaceTilesButton.Checked = false;
            InteractionToolButton.Checked = false;
            ChunksToolButton.Checked = false;
            UpdateControls();
        }

        private void PointerButton_Click(object sender, EventArgs e)
        {
            PointerButton.Checked = !PointerButton.Checked;
            SelectToolButton.Checked = false;
            PlaceTilesButton.Checked = false;
            InteractionToolButton.Checked = false;
            ChunksToolButton.Checked = false;
            UpdateControls();
        }

        private void PlaceTilesButton_Click(object sender, EventArgs e)
        {
            PlaceTilesButton.Checked = !PlaceTilesButton.Checked;
            SelectToolButton.Checked = false;
            PointerButton.Checked = false;
            InteractionToolButton.Checked = false;
            ChunksToolButton.Checked = false;
            UpdateControls();
        }

        private void InteractionToolButton_Click(object sender, EventArgs e)
        {
            InteractionToolButton.Checked = !InteractionToolButton.Checked;
            PlaceTilesButton.Checked = false;
            SelectToolButton.Checked = false;
            PointerButton.Checked = false;
            ChunksToolButton.Checked = false;
            UpdateControls();
        }

        private void ChunkToolButton_Click(object sender, EventArgs e)
        {
            ChunksToolButton.Checked = !ChunksToolButton.Checked;
            InteractionToolButton.Checked = false;
            PlaceTilesButton.Checked = false;
            SelectToolButton.Checked = false;
            PointerButton.Checked = false;
            UpdateControls();
        }

        private void ReloadToolStripButton_Click(object sender, EventArgs e)
        {
            try
            {
                // release all our resources, and force a reload of the tiles
                // Entities should take care of themselves
                DisposeTextures();
                EditorEntity_ini.ReleaseResources();

                //Reload for Encore Palletes, otherwise reload the image normally
                if (useEncoreColors == true)
                {
                    StageTiles?.Image.Reload(EncorePalette[0]);
                    TilesToolbar?.Reload(EncorePalette[0]);
                }
                else
                {
                    StageTiles?.Image.Reload();
                    TilesToolbar?.Reload();
                }

                TilesConfig = new TilesConfig(Path.Combine(SceneFilepath, "TileConfig.bin"));

                CollisionLayerA.Clear();
                CollisionLayerB.Clear();

                for (int i = 0; i < 1024; i++)
                {
                    CollisionLayerA.Add(TilesConfig.CollisionPath1[i].DrawCMask(Color.FromArgb(0, 0, 0, 0), CollisionAllSolid));
                    CollisionLayerB.Add(TilesConfig.CollisionPath2[i].DrawCMask(Color.FromArgb(0, 0, 0, 0), CollisionAllSolid));
                }



            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void RunScene_Click(object sender, EventArgs e)
        {
            IntPtr hWnd = FindWindow("SonicMania", null); // this gives you the handle of the window you need.
            Process processes = Process.GetProcessesByName("SonicMania").FirstOrDefault();
            if (processes != null)
            {
                // check if the window is hidden / minimized
                if (processes.MainWindowHandle == IntPtr.Zero)
                {
                    // the window is hidden so try to restore it before setting focus.
                    ShowWindow(processes.Handle, ShowWindowEnum.Restore);
                }

                // set user the focus to the window
                SetForegroundWindow(processes.MainWindowHandle);
                if (!GameRunning)
                {
                    RunSequence(sender, e, true);
                }
            }
            else
            {
                if (!GameRunning)
                {
                    if (mySettings.RunModLoaderPath != null && mySettings.modConfigs?.Count > 0)
                    {
                        string ConfigPath = mySettings.RunGamePath;
                        var dropDownItem = selectConfigToolStripMenuItem.DropDownItems[0];
                        ConfigPath = ConfigPath.Replace('/', '\\');
                        ConfigPath = ConfigPath.Replace("SonicMania.exe", "//mods//ManiaModLoader.ini");
                        foreach (ToolStripMenuItem item in selectConfigToolStripMenuItem.DropDownItems)
                        {
                            if (item.Checked)
                            {
                                dropDownItem = item;
                            }
                        }
                        File.WriteAllText(ConfigPath, dropDownItem.Tag.ToString());
                    }
                    RunSequence(sender, e);
                }

            }
        }

        private void ShowTileIDButton_Click(object sender, EventArgs e)
        {
            if (ShowTileIDButton.Checked == false)
            {
                ShowTileIDButton.Checked = true;
                ReloadSpecificTextures(sender, e);
                showTileID = true;
            }
            else
            {
                ShowTileIDButton.Checked = false;
                ReloadSpecificTextures(sender, e);
                showTileID = false;
            }
        }

        private void ShowGridButton_Click(object sender, EventArgs e)
        {
            if (ShowGridButton.Checked == false)
            {
                ShowGridButton.Checked = true;
                showGrid = true;
                GridCheckStateCheck();

            }
            else
            {
                ShowGridButton.Checked = false;
                showGrid = false;
            }
        }

        private void ShowCollisionAButton_Click(object sender, EventArgs e)
        {
            if (ShowCollisionAButton.Checked == false)
            {
                ShowCollisionAButton.Checked = true;
                showCollisionA = true;
                ShowCollisionBButton.Checked = false;
                showCollisionB = false;
                ReloadSpecificTextures(sender, e);
            }
            else
            {
                ShowCollisionAButton.Checked = false;
                showCollisionA = false;
                ShowCollisionBButton.Checked = false;
                showCollisionB = false;
                ReloadSpecificTextures(sender, e);
            }
        }

        private void ShowCollisionBButton_Click(object sender, EventArgs e)
        {
            if (ShowCollisionBButton.Checked == false)
            {
                ShowCollisionBButton.Checked = true;
                showCollisionB = true;
                ShowCollisionAButton.Checked = false;
                showCollisionA = false;
                ReloadSpecificTextures(sender, e);
            }
            else
            {
                ShowCollisionBButton.Checked = false;
                showCollisionB = false;
                ShowCollisionAButton.Checked = false;
                showCollisionA = false;
                ReloadSpecificTextures(sender, e);
            }
        }

        private void OpenDataDirectoryMenuButton(object sender, EventArgs e)
        {
            if (_recentDataItems != null)
            {
                string dataDirectory = _recentDataItems[1].Tag.ToString();
                if (dataDirectory != null || dataDirectory != "")
                {
                    RecentDataDirectoryClicked(sender, e, dataDirectory);
                }
            }
        }

        private void ShowFlippedTileHelper_Click(object sender, EventArgs e)
        {
            if (FlipAssistButton.Checked == false)
            {
                FlipAssistButton.Checked = true;
                ReloadSpecificTextures(sender, e);
                showFlippedTileHelper = true;

            }
            else
            {
                FlipAssistButton.Checked = false;
                ReloadSpecificTextures(sender, e);
                showFlippedTileHelper = false;
            }
        }

        private void ResetDeviceButton_Click(object sender, EventArgs e)
        {
            GraphicPanel.AttemptRecovery(null);
        }

        private void EnableEncorePalette_Click(object sender, EventArgs e)
        {
            DisposeTextures();
            if (useEncoreColors == true)
            {
                EncorePaletteButton.Checked = false;
                useEncoreColors = false;
                StageTiles?.Image.Reload();
                TilesToolbar?.Reload();
            }
            else
            {
                EncorePaletteButton.Checked = true;
                useEncoreColors = true;
                StageTiles?.Image.Reload(EncorePalette[0]);
                TilesToolbar?.Reload(EncorePalette[0]);
            }
            EditorEntity_ini.ReleaseResources();
        }


        #endregion

        #region GraphicsPanel + Program + Splitcontainer

        public void OnResetDevice(object sender, DeviceEventArgs e)
        {
            Device device = e.Device;
        }

        public void CheckDeviceState(object sender, PowerModeChangedEventArgs e)
        {
            switch(e.Mode)
            {
                case PowerModes.Suspend:
                    SetDeviceSleepState(false);
                    break;
                case PowerModes.Resume:
                    SetDeviceSleepState(true);
                    break;                
            }
        }


            private void GraphicPanel_OnRender(object sender, DeviceEventArgs e)
            {
            // hmm, if I call refresh when I update the values, for some reason it will stop to render until I stop calling refrsh
            // So I will refresh it here

            if (entitiesToolbar?.NeedRefresh ?? false) entitiesToolbar.PropertiesRefresh();
            if (EditorScene != null)
            {
                if (!IsTilesEdit())
                    Background.Draw(GraphicPanel);
                if (IsTilesEdit())
                {
                    if (mySettings.ShowEditLayerBackground == true)
                    {
                        Background.DrawEdit(GraphicPanel);
                    }
                }

                // Future Implementation

                /*
                List<int> layerDrawingOrder = new List<int> { };
                var allLayers = EditorScene.AllLayers;
                foreach (var layer in allLayers)
                {
                    layerDrawingOrder.Add(layer.Layer.UnknownByte2);
                }
                layerDrawingOrder.Sort();
                for (int i = 0; i < layerDrawingOrder.Count; i++)
                {
                    DrawLayers(layerDrawingOrder[i]);
                }


                DrawLayers();
                */

                if (EditorScene.OtherLayers.Contains(EditLayer)) EditLayer.Draw(GraphicPanel);

                if (!extraLayersMoveToFront)
                {
                    foreach (var elb in _extraLayerViewButtons)
                    {
                        if (elb.Checked)
                        {
                            var _extraViewLayer = EditorScene.OtherLayers.Single(el => el.Name.Equals(elb.Text));
                            _extraViewLayer.Draw(GraphicPanel);
                        }
                    }
                }

                if (ShowFGLower.Checked || EditFGLower.Checked)
                    FGLower.Draw(GraphicPanel);

                if (ShowFGLow.Checked || EditFGLow.Checked)
                    FGLow.Draw(GraphicPanel);

                if (mySettings.PrioritizedObjectRendering && !EditEntities.Checked && entityVisibilityType == 0 && ShowEntities.Checked)
                {
                    entities.DrawPriority(GraphicPanel, 0);
                    entities.DrawPriority(GraphicPanel, 1);
                }

                if (!mySettings.PrioritizedObjectRendering && !EditEntities.Checked && ShowEntities.Checked && entityVisibilityType == 0) entities.Draw(GraphicPanel);

                if (ShowFGHigh.Checked || EditFGHigh.Checked)
                    FGHigh.Draw(GraphicPanel);

                if (mySettings.PrioritizedObjectRendering && !EditEntities.Checked && entityVisibilityType == 0 && ShowEntities.Checked)
                {
                    entities.DrawPriority(GraphicPanel, 2);
                    entities.DrawPriority(GraphicPanel, 3);
                }

                if (ShowFGHigher.Checked || EditFGHigher.Checked)
                    FGHigher.Draw(GraphicPanel);

                if (extraLayersMoveToFront) {
                    foreach (var elb in _extraLayerViewButtons)
                    {
                        if (elb.Checked)
                        {
                            var _extraViewLayer = EditorScene.OtherLayers.Single(el => el.Name.Equals(elb.Text));
                            _extraViewLayer.Draw(GraphicPanel);
                        }
                    }
                }

                if (EditEntities.Checked || (entityVisibilityType == 1 && ShowEntities.Checked)) entities.Draw(GraphicPanel);


            }
            if (draggingSelection)
            {
                int x1 = (int)(selectingX / Zoom), x2 = (int)(lastX / Zoom);
                int y1 = (int)(selectingY / Zoom), y2 = (int)(lastY / Zoom);
                if (x1 != x2 && y1 != y2)
                {
                    if (x1 > x2)
                    {
                        x1 = (int)(lastX / Zoom);
                        x2 = (int)(selectingX / Zoom);
                    }
                    if (y1 > y2)
                    {
                        y1 = (int)(lastY / Zoom);
                        y2 = (int)(selectingY / Zoom);
                    }

                    if (mySettings.UseFasterSelectionRendering == false)
                    {
                        GraphicPanel.DrawRectangle(x1, y1, x2, y2, Color.FromArgb(100, Color.Purple));
                    }
                    GraphicPanel.DrawLine(x1, y1, x2, y1, Color.Purple);
                    GraphicPanel.DrawLine(x1, y1, x1, y2, Color.Purple);
                    GraphicPanel.DrawLine(x2, y2, x2, y1, Color.Purple);
                    GraphicPanel.DrawLine(x2, y2, x1, y2, Color.Purple);
                }
            }
            if (!draggingSelection)
            {
                select_x1 = 0;
                select_x2 = 0;
                select_y1 = 0;
                select_y2 = 0;
            }
            bool deviceLost = GraphicPanel.getDeviceLostState();
            if (scrolling)
            {
                if (vScrollBar1.Visible && hScrollBar1.Visible && !deviceLost) GraphicPanel.Draw2DCursor(scrollPosition.X, scrollPosition.Y);
                else if (vScrollBar1.Visible && !deviceLost) GraphicPanel.DrawVertCursor(scrollPosition.X, scrollPosition.Y);
                else if (hScrollBar1.Visible && !deviceLost) GraphicPanel.DrawHorizCursor(scrollPosition.X, scrollPosition.Y);
            }
            if (showGrid && EditorScene != null)
                Background.DrawGrid(GraphicPanel);

            if (Editor.GameRunning)
            {
                EditorGame.DrawGameElements(GraphicPanel);

                if (playerSelected)
                {
                    EditorGame.MovePlayer(new Point(lastX, lastY), Zoom, selectedPlayer);
                }
                if (checkpointSelected)
                {
                    Point clicked_point = new Point((int)(lastX / Zoom), (int)(lastY / Zoom));
                    EditorGame.UpdateCheckpoint(clicked_point);
                }
            }
            

            Process proc = Process.GetCurrentProcess();
            long memory = proc.PrivateMemorySize64;

            if (!Environment.Is64BitProcess && memory >= 1500000000)
            {
                ReloadToolStripButton_Click(null, null);
            }

            if (ForceWarp)
            {
                SetZoomLevel(mySettings.DevForceRestartZoomLevel, new Point(0, 0));
                GoToPosition(TempWarpCoords.X, TempWarpCoords.Y, false, true);
            }
        }

        public void DrawLayers(int drawOrder = 0)
        {
            var _extraViewLayer = EditorScene.LayerByDrawingOrder.FirstOrDefault(el => el.Layer.UnknownByte2.Equals(drawOrder));
            _extraViewLayer.Draw(GraphicPanel);
        }

        public void Form1_Load(object sender, EventArgs e)
        {
            GraphicPanel.Init(this);
        }

        public void Run()
        {
            Show();
            Focus();
            GraphicPanel.Run();
        }

        private void GraphicPanel_DragEnter(object sender, DragEventArgs e)
        {
            if (!mySettings.DisableDraging)
            {
                if (e.Data.GetDataPresent(typeof(Int32)) && IsTilesEdit())
                {
                    Point rel = GraphicPanel.PointToScreen(Point.Empty);
                    e.Effect = DragDropEffects.Move;
                    //(ushort)((Int32)e.Data.GetData(e.Data.GetFormats()[0])
                    EditLayer?.StartDragOver(new Point((int)(((e.X - rel.X) + ShiftX) / Zoom), (int)(((e.Y - rel.Y) + ShiftY) / Zoom)), (ushort)TilesToolbar.SelectedTile);
                    UpdateEditLayerActions();
                }
                else
                {
                    e.Effect = DragDropEffects.None;
                }
            }
        }

        private void GraphicPanel_DragOver(object sender, DragEventArgs e)
        {
            if (!mySettings.DisableDraging)
            {
                if (e.Data.GetDataPresent(typeof(Int32)) && IsTilesEdit())
                {
                    Point rel = GraphicPanel.PointToScreen(Point.Empty);
                    EditLayer?.DragOver(new Point((int)(((e.X - rel.X) + ShiftX) / Zoom), (int)(((e.Y - rel.Y) + ShiftY) / Zoom)), (ushort)TilesToolbar.SelectedTile);
                    UpdateRender();

                }
            }
        }

        private void GraphicPanel_DragLeave(object sender, EventArgs e)
        {
            if (!mySettings.DisableDraging)
            {
                EditLayer?.EndDragOver(true);
                UpdateRender();
            }
        }

        private void GraphicPanel_DragDrop(object sender, DragEventArgs e)
        {
            if (!mySettings.DisableDraging)
            {
                EditLayer?.EndDragOver(false);
            }
        }

        public void GraphicPanel_OnKeyDown(object sender, KeyEventArgs e)
        {
            EditorControls.GraphicPanel_OnKeyDown(sender, e);
        }

        public void GraphicPanel_OnKeyUp(object sender, KeyEventArgs e)
        {
            EditorControls.GraphicPanel_OnKeyUp(sender, e);
        }

        private void MapEditor_Activated(object sender, EventArgs e)
        {
            GraphicPanel.Focus();
            if (mainform.hasModified)
            {
                ReloadToolStripButton_Click(sender, e);
            }
            
        }

        private void MapEditor_KeyDown(object sender, KeyEventArgs e)
        {
            if (!GraphicPanel.Focused && e.Control)
            {
                EditorControls.GraphicPanel_OnKeyDown(sender, e);
            }
        }

        private void Editor_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                GameRunning = false;
                var mySettings = Properties.Settings.Default;
                mySettings.IsMaximized = WindowState == FormWindowState.Maximized;
                mySettings.Save();
            }
            catch (Exception ex)
            {
                Debug.Write("Failed to write settings: " + ex);
            }
        }

        private void MapEditor_KeyUp(object sender, KeyEventArgs e)
        {
            if (!GraphicPanel.Focused && e.Control)
            {
                EditorControls.GraphicPanel_OnKeyUp(sender, e);
            }
        }

        private void SplitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
        {
            Form1_Resize(null, null);
        }

        private void GraphicPanel_MouseClick(object sender, MouseEventArgs e)
        {
            GraphicPanel.Focus();
            if (e.Button == MouseButtons.Right && IsTilesEdit() && InteractionToolButton.Checked)
            {
                Point clicked_point_tile = new Point((int)(e.X / Zoom), (int)(e.Y / Zoom));
                int tile = (ushort)(EditLayer?.GetTileAt(clicked_point_tile) & 0x3ff);
                SelectedTileID = tile;
                editTile0WithTileManiacToolStripMenuItem.Enabled = (tile <= 1023);
                moveThePlayerToHereToolStripMenuItem.Enabled = GameRunning;
                setPlayerRespawnToHereToolStripMenuItem.Enabled = GameRunning;

                editTile0WithTileManiacToolStripMenuItem.Text = String.Format("Edit Tile {0} in Tile Maniac", tile);
                contextMenuStrip1.Show(MousePosition.X, MousePosition.Y);
            }
            else if (e.Button == MouseButtons.Right && InteractionToolButton.Checked)
            {
                Point clicked_point_tile = new Point((int)(e.X / Zoom), (int)(e.Y / Zoom));
                string tile = "NULL";
                editTile0WithTileManiacToolStripMenuItem.Enabled = false;
                moveThePlayerToHereToolStripMenuItem.Enabled = GameRunning;
                setPlayerRespawnToHereToolStripMenuItem.Enabled = GameRunning;
                moveThisPlayerToolStripMenuItem.Enabled = GameRunning;
                moveCheckpointToolStripMenuItem.Enabled = GameRunning;

                editTile0WithTileManiacToolStripMenuItem.Text = String.Format("Edit Tile {0} in Tile Maniac", tile);
                contextMenuStrip1.Show(MousePosition.X, MousePosition.Y);
            }

            //Stuff that Doesn't work yet that I'm not ready to ship
            setPlayerRespawnToHereToolStripMenuItem.Enabled = false;
            removeCheckpointToolStripMenuItem.Enabled = false;
            assetResetToolStripMenuItem.Enabled = false;
            restartSceneToolStripMenuItem.Enabled = false;
            moveCheckpointToolStripMenuItem.Enabled = false;
        }

        private void ViewPanel_Click(object sender, EventArgs e)
        {

        }

        #endregion

        #region Normal Layer Button Methods
        private void LayerShowButton_Click(ToolStripButton button, string desc)
        {
            if (button.Checked)
            {
                button.Checked = false;
                button.ToolTipText = "Show " + desc;
            }
            else
            {
                button.Checked = true;
                button.ToolTipText = "Hide " + desc;
            }
        }

        private void ShowFGLow_Click(object sender, EventArgs e)
        {
            LayerShowButton_Click(ShowFGLow, "Layer FG Low");
        }

        private void ShowFGHigh_Click(object sender, EventArgs e)
        {
            LayerShowButton_Click(ShowFGHigh, "Layer FG High");
        }

        private void ShowFGHigher_Click(object sender, EventArgs e)
        {
            LayerShowButton_Click(ShowFGHigher, "Layer FG Higher");
        }

        private void ShowFGLower_Click(object sender, EventArgs e)
        {
            LayerShowButton_Click(ShowFGLower, "Layer FG Lower");
        }

        private void ShowEntities_Click(object sender, EventArgs e)
        {
            LayerShowButton_Click(ShowEntities, "Entities");
        }

        private void ShowAnimations_Click(object sender, EventArgs e)
        {
            LayerShowButton_Click(ShowAnimations, "Animations");
        }

        private void LayerEditButton_Click(ToolStripButton button)
        {
            Deselect(false);
            if (button.Checked)
            {
                button.Checked = false;
            }
            else
            {
                EditFGLow.Checked = false;
                EditFGHigh.Checked = false;
                EditFGLower.Checked = false;
                EditFGHigher.Checked = false;
                EditEntities.Checked = false;
                button.Checked = true;
            }

            foreach (var elb in _extraLayerEditButtons)
            {
                elb.Checked = false;
            }
            UpdateControls();
        }

        private void EditFGLow_Click(object sender, EventArgs e)
        {
            LayerEditButton_Click(EditFGLow);
        }

        private void EditFGHigh_Click(object sender, EventArgs e)
        {
            LayerEditButton_Click(EditFGHigh);
        }

        private void EditFGLower_Click(object sender, EventArgs e)
        {
            LayerEditButton_Click(EditFGLower);
        }

        private void EditFGHigher_Click(object sender, EventArgs e)
        {
            LayerEditButton_Click(EditFGHigher);
        }

        private void EditEntities_Click(object sender, EventArgs e)
        {
            LayerEditButton_Click(EditEntities);
        }
        #endregion

        #region Editor Functions Copy/Paste/Delete/etc.
        /// <summary>
        /// Deselects all tiles and entities
        /// </summary>
        /// <param name="updateControls">Whether to update associated on-screen controls</param>
        public void Deselect(bool updateControls = true)
        {
            if (IsEditing())
            {
                EditLayer?.Deselect();

                if (IsEntitiesEdit()) entities.Deselect();
                SetSelectOnlyButtonsState(false);
                if (updateControls)
                    UpdateEditLayerActions();
            }
        }

        public void EditorUndo()
        {
            if (undo.Count > 0)
            {
                if (IsTilesEdit())
                {
                    // Deselect to apply the changes
                    Deselect();
                }
                else if (IsEntitiesEdit())
                {
                    if (undo.Peek() is ActionAddDeleteEntities)
                    {
                        // deselect only if delete/create
                        Deselect();
                    }
                }
                IAction act = undo.Pop();
                act.Undo();
                redo.Push(act.Redo());
                if (IsEntitiesEdit() && IsSelected())
                {
                    // We need to update the properties of the selected entity
                    entitiesToolbar.UpdateCurrentEntityProperites();
                }
            }
            UpdateControls();
        }

        public void EditorRedo()
        {
            if (redo.Count > 0)
            {
                IAction act = redo.Pop();
                act.Undo();
                undo.Push(act.Redo());
                if (IsEntitiesEdit() && IsSelected())
                {
                    // We need to update the properties of the selected entity
                    entitiesToolbar.UpdateCurrentEntityProperites();
                }
            }
            UpdateControls();
        }

        private void CopyTilesToClipboard(bool doNotUseWindowsClipboard = false)
        {
            Dictionary<Point, ushort> copyData = EditLayer.CopyToClipboard();

            // Make a DataObject for the copied data and send it to the Windows clipboard for cross-instance copying
            if (mySettings.EnableWindowsClipboard && !doNotUseWindowsClipboard)
                Clipboard.SetDataObject(new DataObject("ManiacTiles", copyData), true);

            // Also copy to Maniac's clipboard in case it gets overwritten elsewhere
            TilesClipboard = copyData;

        }

        private void CopyEntitiesToClipboard()
        {
            if (entitiesToolbar.ContainsFocus.Equals(false))
            {
                // Windows Clipboard mode
                if (mySettings.EnableWindowsClipboard && !mySettings.ProhibitEntityUseOnExternalClipboard)
                {
                    // Clone the entities and stow them here
                    List<EditorEntity> copyData = entities.CopyToClipboard();

                    // Prepare each Entity for the copy to release unnecessary data
                    foreach (EditorEntity entity in copyData)
                        entity.PrepareForExternalCopy();

                    // Make a DataObject for the data and send it to the Windows clipboard for cross-instance copying
                    Clipboard.SetDataObject(new DataObject("ManiacEntities", copyData), true);
                }

                // Local Clipboard mode
                {
                    // Clone the entities and stow them here
                    List<EditorEntity> copyData = entities.CopyToClipboard();

                    // Send to Maniac's clipboard
                    entitiesClipboard = copyData;
                }
            }
        }

        public void MoveEntityOrTiles(object sender, KeyEventArgs e)
        {
            int x = 0, y = 0;
            if (MagnetMode.Checked == false)
            {
                UseMagnetMode = false;
            }
            if (nudgeFasterButton.Checked == false)
            {
                mySettings.EnableFasterNudge = false;
                nudgeFasterButton.Checked = false;
            }
            if (UseMagnetMode)
            {
                switch (e.KeyData)
                {
                    case Keys.Up: y = (useMagnetYAxis ? -magnetSize : -1); break;
                    case Keys.Down: y = (useMagnetYAxis ? magnetSize : 1); break;
                    case Keys.Left: x = (useMagnetXAxis ? -magnetSize : -1); break;
                    case Keys.Right: x = (useMagnetXAxis ? magnetSize : 1); break;
                }
            }
            if (mySettings.EnableFasterNudge)
            {
                if (UseMagnetMode)
                {
                    switch (e.KeyData)
                    {
                        case Keys.Up: y = (useMagnetYAxis ? -magnetSize * mySettings.FasterNudgeValue : -1 - mySettings.FasterNudgeValue); break;
                        case Keys.Down: y = (useMagnetYAxis ? magnetSize * mySettings.FasterNudgeValue : 1 + mySettings.FasterNudgeValue); break;
                        case Keys.Left: x = (useMagnetXAxis ? -magnetSize * mySettings.FasterNudgeValue : -1 - mySettings.FasterNudgeValue); break;
                        case Keys.Right: x = (useMagnetXAxis ? magnetSize * mySettings.FasterNudgeValue : 1 + mySettings.FasterNudgeValue); break;
                    }
                }
                else
                {
                    switch (e.KeyData)
                    {
                        case Keys.Up: y = -1 - mySettings.FasterNudgeValue; break;
                        case Keys.Down: y = 1 + mySettings.FasterNudgeValue; break;
                        case Keys.Left: x = -1 - mySettings.FasterNudgeValue; break;
                        case Keys.Right: x = 1 + mySettings.FasterNudgeValue; break;
                    }
                }

            }
            if (UseMagnetMode == false && mySettings.EnableFasterNudge == false)
            {
                switch (e.KeyData)
                {
                    case Keys.Up: y = -1; break;
                    case Keys.Down: y = 1; break;
                    case Keys.Left: x = -1; break;
                    case Keys.Right: x = 1; break;
                }

            }
            EditLayer?.MoveSelectedQuonta(new Point(x, y));

            UpdateEditLayerActions();

            if (IsEntitiesEdit())
            {
                if (UseMagnetMode)
                {
                    int xE = entities.SelectedEntities[0].Entity.Position.X.High;
                    int yE = entities.SelectedEntities[0].Entity.Position.Y.High;

                    if (xE % magnetSize != 0 && useMagnetXAxis)
                    {
                        int offsetX = x % magnetSize;
                        x -= offsetX;
                    }
                    if (yE % magnetSize != 0 && useMagnetYAxis)
                    {
                        int offsetY = y % magnetSize;
                        y -= offsetY;
                    }
                }


                entities.MoveSelected(new Point(0, 0), new Point(x, y), false);
                entitiesToolbar.UpdateCurrentEntityProperites();

                // Try to merge with last move
                if (undo.Count > 0 && undo.Peek() is ActionMoveEntities && (undo.Peek() as ActionMoveEntities).UpdateFromKey(entities.SelectedEntities, new Point(x, y))) { }
                else
                {
                    undo.Push(new ActionMoveEntities(entities.SelectedEntities.ToList(), new Point(x, y), true));
                    redo.Clear();
                    UpdateControls();
                }
            }
        }

        public void MoveCameraFreely(object sender, KeyEventArgs e)
        {
            hScrollBar1.MaximumSize = new Size(100000000, 100000000);
            vScrollBar1.MaximumSize = new Size(100000000, 100000000);
            if (CtrlPressed() && ShiftPressed())
            {
                switch (e.KeyData)
                {
                    case Keys.Up: CustomY -= 5; break;
                    case Keys.Down: CustomY += 5; break;
                    case Keys.Left: CustomX -= 5; break;
                    case Keys.Right: CustomX += 5; break;
                }
            }

        }

        #endregion

        #region Asset Reloading
        private void ReloadSpecificTextures(object sender, EventArgs e)
        {
            try
            {
                // release all our resources, and force a reload of the tiles
                // Entities should take care of themselves
                DisposeTextures();

                if (Editor.Instance.useEncoreColors)
                {
                    StageTiles?.Image.Reload(EncorePalette[0]);
                }
                else
                {
                    StageTiles?.Image.Reload();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void DisposeTextures()
        {
            // Make sure to dispose the textures of the extra layers too
            StageTiles?.DisposeTextures();
            if (FGHigh != null) FGHigh.DisposeTextures();
            if (FGLow != null) FGLow.DisposeTextures();
            if (FGHigher != null) FGHigher.DisposeTextures();
            if (FGLower != null) FGLower.DisposeTextures();

            foreach (var el in EditorScene.OtherLayers)
            {
                el.DisposeTextures();
            }
        }

        public void RefreshCollisionColours(bool RefreshMasks = false)
        {
            switch (collisionPreset)
            {
                case 2:
                    CollisionAllSolid = Properties.Settings.Default.CollisionSAColour;
                    CollisionTopOnlySolid = Properties.Settings.Default.CollisionTOColour;
                    CollisionLRDSolid = Properties.Settings.Default.CollisionLRDColour;
                    break;
                case 1:
                    CollisionAllSolid = Color.Black;
                    CollisionTopOnlySolid = Color.Yellow;
                    CollisionLRDSolid = Color.Red;
                    break;
                case 0:
                    CollisionAllSolid = Color.White;
                    CollisionTopOnlySolid = Color.Yellow;
                    CollisionLRDSolid = Color.Red;
                    break;
            }




            if (RefreshMasks)
            {
                CollisionLayerA.Clear();
                CollisionLayerB.Clear();

                for (int i = 0; i < 1024; i++)
                {
                    CollisionLayerA.Add(StageTiles.Config.CollisionPath1[i].DrawCMask(Color.FromArgb(0, 0, 0, 0), CollisionAllSolid));
                    CollisionLayerB.Add(StageTiles.Config.CollisionPath2[i].DrawCMask(Color.FromArgb(0, 0, 0, 0), CollisionAllSolid));
                }
            }
        }
        #endregion

        #region Run Mania Methods

        // TODO: Perfect Scene Autobooting
        private void RunSequence(object sender, EventArgs e, bool attachMode = false)
        {
            // Ask where Sonic Mania is located when not set
            string path = "steam://run/584400";
            bool ready = false;
            if (mySettings.UsePrePlusOffsets)
            {
                if (string.IsNullOrEmpty(mySettings.RunGamePath))
                {
                    var ofd = new OpenFileDialog
                    {
                        Title = "Select SonicMania.exe",
                        Filter = "Windows PE Executable|*.exe"
                    };
                    if (ofd.ShowDialog() == DialogResult.OK)
                        mySettings.RunGamePath = ofd.FileName;
                }
                else
                {
                    if (!File.Exists(mySettings.RunGamePath))
                    {
                        mySettings.RunGamePath = "";
                        return;
                    }
                }
                path = mySettings.RunGamePath;
            }
            ProcessStartInfo psi;

            if (mySettings.RunGameInsteadOfScene)
            {
                psi = new ProcessStartInfo(path);
            }
            else
            {
                if (mySettings.UsePrePlusOffsets == true)
                {
                    psi = new ProcessStartInfo(path, $"stage={SelectedZone};scene={SelectedScene[5]};");
                }
                else
                {
                    psi = new ProcessStartInfo(path);
                }

            }
            if (path != "" || attachMode)
            {
                string maniaDir = Path.GetDirectoryName(path);
                // Check if the mod loader is installed
                if (File.Exists(Path.Combine(maniaDir, "d3d9.dll")))
                    psi.WorkingDirectory = maniaDir;
                else
                    psi.WorkingDirectory = Path.GetDirectoryName(DataDirectory);
                Process p;
                if (!attachMode)
                {
                     p = Process.Start(psi);
                }
                else
                {
                    var mania = Process.GetProcessesByName("SonicMania.exe");
                    p = mania.FirstOrDefault();
                }
                GameRunning = true;

                int CurrentScene_ptr = 0x00E48758;          // &CurrentScene
                int GameState_ptr = 0x00E48776;             // &GameState
                int IsGameRunning_ptr = 0x0065D1C8;
                int Player1_ControllerID_ptr = 0x0085EB44;  // &Player1.ControllerID
                int Player2_ControllerID_ptr = 0x0085EF9C;  // &Player2.ControllerID
                if (mySettings.UsePrePlusOffsets)
                {
                    CurrentScene_ptr = 0x00CCF6F8;
                    // TODO: Get Pre Plus GameState address
                    IsGameRunning_ptr = 0x00628094;
                    Player1_ControllerID_ptr = 0x00A4C860;
                }

                if (mySettings.UsePrePlusOffsets)
                {
                    UpdateControls();
                    UseCheatCodes(p);
                    ready = true;
                }
                else
                {

                    // For Mania Plus, The best way to boot the game is by using the steam command.
                    // After Calling the Steam command, We need to wait until Steam responds and Starts the game.
                    // Once the game process starts up, We quickly attach to it and apply all the needed patches

                    // Wait for Steam to complete startup
                    new Thread(() =>
                    {
                        Process[] Procs;
                        while ((Procs = Process.GetProcessesByName("SonicMania")).Length == 0)
                            Thread.Sleep(1);
                        Invoke(new Action(() =>
                        {
                            p = Procs[0];
                            // Attach and Apply Cheats
                            UseCheatCodes(p);
                            UpdateControls();
                            ready = true;


                            // Wait until there is a Running Scene.
                            while (GameMemory.ReadByte(GameState_ptr) != 0x01)
                                Thread.Sleep(1);

                            // Swap the Scene
                            if (myEditorState.Level_ID != -1)
                            {
                                GameMemory.WriteByte(CurrentScene_ptr, (byte)myEditorState.Level_ID);
                                // Restart the Scene
                                GameMemory.WriteByte(GameState_ptr, 0);
                            }



                        }));
                    }).Start();
                }


                new Thread(() =>
                {
                    while (!ready)
                        Thread.Sleep(10);
                    /* Level != Main Menu*/
                    while (GameMemory.ReadByte(CurrentScene_ptr) != 0x02 || Properties.Settings.Default.DisableRunSceneMenuQuit == true)
                    {
                        // Check if the user closed the game
                        if (p.HasExited || !GameRunning)
                        {
                            GameRunning = false;
                            if (Visible)
                            {
                                Invoke(new Action(() => UpdateControls()));
                            }
                            return;
                        }
                        UseCheatCodes(p);
                        // Makes sure the process is attached and patches are applied
                        // Set Player 1 Controller Set to 1 (If we set it to AnyController (0x00) we can't use Debug Mode In-Game)
                        if (GameMemory.ReadByte(Player1_ControllerID_ptr) != 0x00 && Properties.Settings.Default.DisableRunSceneAutoInput == false)
                        {
                            GameMemory.WriteByte(Player1_ControllerID_ptr, 0x00); //setting this to 0x00 causes the inability to use debug mode
                            GameMemory.WriteByte(Player2_ControllerID_ptr, 0xFF);
                        }
                        Thread.Sleep(300);
                    }
                    // User is on the Main Menu
                    // Close the game
                    GameMemory.WriteByte(IsGameRunning_ptr, 0);
                    GameRunning = false;
                    Invoke(new Action(() => UpdateControls()));
                }).Start();
            }        
    }

        public void UseCheatCodes(Process p)
        {
            if (mySettings.UsePrePlusOffsets)
            {
                // Patches
                GameMemory.Attach(p);
            }
            else
            {
                GameMemory.Attach(p);

                // Mania Plus Patches
                GameMemory.WriteByte(0x00E48768, 0x01); // Enable Debug
                GameMemory.WriteByte(0x006F1806, 0x01); // Allow DevMenu
                GameMemory.WriteByte(0x005FDD00, 0xEB); // Disable Background Pausing
            }
        }

        #endregion

        #region Lower Right Status Bar Buttons

        private void PixelModeButton_Click(object sender, EventArgs e)
        {
            if (pixelModeButton.Checked == false || pixelModeToolStripMenuItem.Checked == false)
            {
                pixelModeButton.Checked = true;
                pixelModeToolStripMenuItem.Checked = true;
                mySettings.pixelCountMode = true;

            }
            else
            {
                pixelModeButton.Checked = false;
                pixelModeToolStripMenuItem.Checked = false;
                mySettings.pixelCountMode = false;
            }

        }

        public void ScrollLockButton_Click(object sender, EventArgs e)
        {
            if (mySettings.scrollLock == false)
            {
                scrollLockButton.Checked = true;
                mySettings.scrollLock = true;
                statusNAToolStripMenuItem.Checked = true;
                scrollDirection = "Locked";
            }
            else
            {
                if (mySettings.ScrollLockY == true)
                {
                    scrollLockButton.Checked = false;
                    statusNAToolStripMenuItem.Checked = false;
                    mySettings.scrollLock = false;
                    scrollDirection = "Y";
                }
                else
                {
                    scrollLockButton.Checked = false;
                    statusNAToolStripMenuItem.Checked = false;
                    mySettings.scrollLock = false;
                    scrollDirection = "X";
                }

            }

        }

        public void NudgeFasterButton_Click(object sender, EventArgs e)
        {
            if (nudgeFasterButton.Checked == false)
            {
                nudgeFasterButton.Checked = true;
                nudgeSelectionFasterToolStripMenuItem.Checked = true;
                mySettings.EnableFasterNudge = true;
            }
            else
            {
                nudgeFasterButton.Checked = false;
                nudgeSelectionFasterToolStripMenuItem.Checked = false;
                mySettings.EnableFasterNudge = false;
            }
        }

        #endregion

        #region Scrollbar Methods

        private void VScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            ShiftY = e.NewValue;
            UpdateRender();
        }

        private void HScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            ShiftX = e.NewValue;
            UpdateRender();
        }

        private void VScrollBar1_ValueChanged(object sender, EventArgs e)
        {                    
            ShiftY = (sender as VScrollBar).Value;
            if (!(zooming || draggingSelection || dragged || scrolling)) UpdateRender();
            if (draggingSelection)
            {
                OnMouseMoveEvent();
            }
            
        }

        private void HScrollBar1_ValueChanged(object sender, EventArgs e)
        {         
            ShiftX = hScrollBar1.Value;
            if (!(zooming || draggingSelection || dragged || scrolling)) UpdateRender();
            if (draggingSelection)
            {
                OnMouseMoveEvent();
            }
            
        }

        private void VScrollBar1_Entered(object sender, EventArgs e)
        {
            if (mySettings.scrollLock == false)
            {
                scrollDirection = "Y";
            }
            else
            {
                scrollDirection = "Locked";
            }

        }

        private void HScrollBar1_Entered(object sender, EventArgs e)
        {
            if (mySettings.scrollLock == false)
            {
                scrollDirection = "X";
            }
            else
            {
                scrollDirection = "Locked";
            }
        }

        #endregion

        #region Backup Tool Methods

        public void BackupScene()
        {
            backupType = 1;
            BackupToolStripMenuItem_Click(null, null);
            backupType = 0;
        }
        public void BackupSceneBeforeCrash()
        {
            backupType = 2;
            BackupToolStripMenuItem_Click(null, null);
            backupType = 0;
        }
        public void AutoBackupScene()
        {
            backupType = 3;
            BackupToolStripMenuItem_Click(null, null);
            backupType = 0;
        }
        public void BackupTool(object sender, EventArgs e)
        {
            //Backup Types:
            // 1: Manual Backups - Made by the user, and infinite amount
            // 2: Emergency Backups - Made by the editor, right before a crash or something progress losing, and only is made
            // 3: Automatic Backups - Made by the editor by user choice (toggle in options) automatically every so often
            // 4: Stage Config Backup - For Object Manager
            if (EditorScene == null) return;

            if (IsTilesEdit())
            {
                // Apply changes
                Deselect();
            }

            try
            {
                if (backupType == 1)
                {
                    String SceneFilenameBak = SceneFilename + ".bak";
                    String SceneFilenameBakReserve = SceneFilenameBak;
                    SceneFilenameBak += ".bin";
                    int i = 1;
                    while ((File.Exists(SceneFilenameBak)))
                    {
                        SceneFilenameBak = SceneFilenameBakReserve + i + ".bin";
                        i++;
                    }
                    EditorScene.Save(SceneFilenameBak);
                }
                if (backupType == 2)
                {
                    String SceneFilenameBak = SceneFilename + ".crash.bak.bin";
                    String SceneFilenameBakReserve = SceneFilenameBak;
                    EditorScene.Save(SceneFilenameBak);
                }
                if (backupType >= 3 && backupType != 4)
                {
                    String SceneFilenameBak = SceneFilename + ".idk.bak";
                    String SceneFilenameBakReserve = SceneFilenameBak;
                    SceneFilenameBak += ".bin";
                    int i = 1;
                    while ((File.Exists(SceneFilenameBak)))
                    {
                        SceneFilenameBak = SceneFilenameBakReserve + i + ".bin";
                        i++;
                    }
                    EditorScene.Save(SceneFilenameBak);
                }

            }
            catch (Exception ex)
            {
                ShowError($@"Failed to backup the scene to file '{SceneFilename}'
Error: {ex.Message}");
            }

            try
            {
                if (backupType == 4)
                {
                    String StageConfigFileName = SceneFilepath + "\\StageConfig" + ".bak";
                    String StageConfigFileNameReserve = StageConfigFileName;
                    StageConfigFileName += ".bin";
                    int i = 1;
                    while ((File.Exists(StageConfigFileName)))
                    {
                        StageConfigFileName = StageConfigFileNameReserve + i + ".bin";
                        i++;
                    }
                    StageConfig?.Write(StageConfigFileName);
                }
            }
            catch (Exception ex)
            {
                ShowError($@"Failed to backup the StageConfig to file '{StageConfigFileName}'
Error: {ex.Message}");
            }
        }

        #endregion

        #region Run Scene Button Methods/Buttons
        private void OpenDataDirectoryButton_DropDownOpened(object sender, EventArgs e)
        {
            RecentDataDirectories.AutoToolTip = false;
        }

        private void OpenDataDirectoryButton_DropDownClosed(object sender, EventArgs e)
        {
            RecentDataDirectories.AutoToolTip = true;
        }

        private void OpenModManagerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            String modProcessName = Path.GetFileNameWithoutExtension(mySettings.RunModLoaderPath);
            IntPtr hWnd = FindWindow(modProcessName, null); // this gives you the handle of the window you need.
            Process processes = Process.GetProcessesByName(modProcessName).FirstOrDefault();
            if (processes != null)
            {
                // check if the window is hidden / minimized
                if (processes.MainWindowHandle == IntPtr.Zero)
                {
                    // the window is hidden so try to restore it before setting focus.
                    ShowWindow(processes.Handle, ShowWindowEnum.Restore);
                }

                // set user the focus to the window
                SetForegroundWindow(processes.MainWindowHandle);
            }
            else
            {
                // Ask where the Mania Mod Manager is located when not set
                if (string.IsNullOrEmpty(mySettings.RunModLoaderPath))
                {
                    var ofd = new OpenFileDialog
                    {
                        Title = "Select Mania Mod Manager.exe",
                        Filter = "Windows PE Executable|*.exe"
                    };
                    if (ofd.ShowDialog() == DialogResult.OK)
                        mySettings.RunModLoaderPath = ofd.FileName;
                }
                else
                {
                    if (!File.Exists(mySettings.RunGamePath))
                    {
                        mySettings.RunModLoaderPath = "";
                        return;
                    }
                }

                if (File.Exists(mySettings.RunModLoaderPath))
                    Process.Start(mySettings.RunModLoaderPath);
            }
        }

        #endregion

        #region Show Grid Button Methods/Buttons
        private void GridCheckStateCheck()
        {
            if (x16ToolStripMenuItem.Checked == true)
            {
                Background.GRID_TILE_SIZE = 16;
            }
            if (x128ToolStripMenuItem.Checked == true)
            {
                Background.GRID_TILE_SIZE = 128;
            }
            if (x256ToolStripMenuItem.Checked == true)
            {
                Background.GRID_TILE_SIZE = 256;
            }
            if (customToolStripMenuItem.Checked == true)
            {
                Background.GRID_TILE_SIZE = mySettings.CustomGridSizeValue;
            }
        }
        private void X16ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Background.GRID_TILE_SIZE = 16;
            ResetGridOptions();
            x16ToolStripMenuItem.Checked = true;
        }

        private void X128ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Background.GRID_TILE_SIZE = 128;
            ResetGridOptions();
            x128ToolStripMenuItem.Checked = true;
        }

        private void ResetGridOptions()
        {
            x16ToolStripMenuItem.Checked = false;
            x128ToolStripMenuItem.Checked = false;
            x256ToolStripMenuItem.Checked = false;
            customToolStripMenuItem.Checked = false;
        }

        private void X256ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Background.GRID_TILE_SIZE = 256;
            ResetGridOptions();
            x256ToolStripMenuItem.Checked = true;
        }

        private void CustomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Background.GRID_TILE_SIZE = mySettings.CustomGridSizeValue;
            ResetGridOptions();
            customToolStripMenuItem.Checked = true;
        }
        #endregion

        #region Lower Right Status Bar Quick Options Button
        public void MoreSettingsButton_ButtonClick(object sender, EventArgs e)
        {
            switch (myEditorState.lastQuickButtonState)
            {
                case 1:
                    SwapScrollLockDirectionToolStripMenuItem_Click(sender, e);
                    break;
                case 2:
                    EditEntitesTransparencyToolStripMenuItem_Click(sender, e);
                    break;
                case 3:
                    ToggleEncoreManiaEntitiesToolStripMenuItem_Click(sender, e);
                    break;
                default:
                    SwapScrollLockDirectionToolStripMenuItem_Click(sender, e);
                    break;


            }
        }

        public void SwapScrollLockDirectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            myEditorState.lastQuickButtonState = 1;
            XToolStripMenuItem_Click(sender, e);
        }

        public void EditEntitesTransparencyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sender != transparentLayersForEditingEntitiesToolStripMenuItem)
            {
                myEditorState.lastQuickButtonState = 2;
            }
            if (myEditorState.editEntitiesTransparency == false)
            {
                myEditorState.editEntitiesTransparency = true;
                transparentLayersForEditingEntitiesToolStripMenuItem.Checked = true;
                editEntitesTransparencyToolStripMenuItem.Checked = true;
            }
            else
            {
                myEditorState.editEntitiesTransparency = false;
                transparentLayersForEditingEntitiesToolStripMenuItem.Checked = false;
                editEntitesTransparencyToolStripMenuItem.Checked = false;
            }
        }

        public void ToggleEncoreManiaEntitiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            myEditorState.lastQuickButtonState = 3;
            if (mySettings.showEncoreEntities == true && mySettings.showManiaEntities == true)
            {
                mySettings.showManiaEntities = true;
                mySettings.showEncoreEntities = false;
            }
            if (mySettings.showEncoreEntities == true && mySettings.showManiaEntities == false)
            {
                mySettings.showManiaEntities = true;
                mySettings.showEncoreEntities = false;
            }
            else
            {
                mySettings.showManiaEntities = false;
                mySettings.showEncoreEntities = true;
            }

        }
        #endregion

        #region Magnet Mode Methods/Buttons

        private void X8ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            magnetSize = 8;
            ResetMagnetModeOptions();
            x8ToolStripMenuItem.Checked = true;
        }

        private void X16ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            magnetSize = 16;
            ResetMagnetModeOptions();
            x16ToolStripMenuItem1.Checked = true;
        }

        private void X32ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            magnetSize = 32;
            ResetMagnetModeOptions();
            x32ToolStripMenuItem.Checked = true;
        }

        private void X64ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            magnetSize = 64;
            ResetMagnetModeOptions();
            x64ToolStripMenuItem.Checked = true;
        }

        private void ResetMagnetModeOptions()
        {
            x16ToolStripMenuItem1.Checked = false;
            x8ToolStripMenuItem.Checked = false;
            x32ToolStripMenuItem.Checked = false;
            x64ToolStripMenuItem.Checked = false;
        }

        private void EnableXAxisToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (enableXAxisToolStripMenuItem.Checked)
            {
                enableXAxisToolStripMenuItem.Checked = false;
                useMagnetXAxis = false;
            }
            else
            {
                enableXAxisToolStripMenuItem.Checked = true;
                useMagnetXAxis = true;
            }
        }

        private void EnableYAxisToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (enableYAxisToolStripMenuItem.Checked)
            {
                enableYAxisToolStripMenuItem.Checked = false;
                useMagnetYAxis = false;
            }
            else
            {
                enableYAxisToolStripMenuItem.Checked = true;
                useMagnetYAxis = true;
            }
        }

        #endregion

        #region Developer Stuff

        public void GoToToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GoToPositionBox form = new GoToPositionBox();
            DialogResult Result = form.ShowDialog();
            if (Result == DialogResult.OK)
            {
                int x = form.goTo_X;
                int y = form.goTo_Y;
                if (form.tilesMode)
                {
                    x *= 16;
                    y *= 16;
                }
                GoToPosition(x, y);
            }

        }

        public void PreLoadSceneButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this, "It is cautioned that you save now, as there is NO WAY TO END THIS PROCESS ONCE IT STARTS and you may be forced to force the program to quit! Are you sure you want to continue?", "WARNING", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
            {
                PreloadSceneButton.Enabled = false;
                isPreRending = true;
                PreLoadBox preLoadForm = new PreLoadBox
                {
                    TopLevel = false
                };
                GraphicPanel.Controls.Add(preLoadForm);
                preLoadForm.Show();
                ToggleEditorButtons(false);

                int ScrollAmount = 100;
                if (mySettings.preRenderTURBOMode)
                {
                    ScrollAmount = 500;
                }

                hScrollBar1.Value = 0;
                vScrollBar1.Value = 0;
                int ScreenMaxH = hScrollBar1.Maximum - hScrollBar1.LargeChange;
                int ScreenMaxV = vScrollBar1.Maximum - vScrollBar1.LargeChange;

                for (int y = 0; y < ScreenMaxV;)
                {
                    for (int x = 0; x < ScreenMaxH;)
                    {
                        hScrollBar1.Value = x;
                        int x_test = x + ScrollAmount;
                        if (x_test >= ScreenMaxH)
                        {
                            x = x + x_test - ScreenMaxH;
                        }
                        else
                        {
                            x = x + ScrollAmount;
                        }
                        Application.DoEvents();
                        //preLoadForm.SetProgressBarStatus(ScreenMaxH - x, ScreenMaxV - y);
                        // Enable when the previous TODO above is Fixed

                    }
                    vScrollBar1.Value = y;
                    int y_test = y + ScrollAmount;
                    if (y_test >= ScreenMaxV)
                    {
                        y = y + y_test - ScreenMaxV;
                    }
                    else
                    {
                        y = y + ScrollAmount;
                    }
                    Application.DoEvents();
                }
                hScrollBar1.Value = 0;
                vScrollBar1.Value = 0;

                // get the form reference back and close it
                isPreRending = false;
                preLoadForm.Close();
                ToggleEditorButtons(true);

                // Play a sound to tell the user we are finished
                System.IO.Stream str = Properties.Resources.ScoreTotal;
                System.Media.SoundPlayer snd = new System.Media.SoundPlayer(str);
                snd.Play();
                PreloadSceneButton.Enabled = true;
            }
        }

        private void DeveloperTerminalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeveloperTerminal devTerm = new DeveloperTerminal();
            devTerm.Show();

        }

        private void MD5GeneratorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MD5HashGen hashmap = new MD5HashGen();
            hashmap.Show();
        }

        private void PlayerSpawnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            selectPlayerObject_GoTo = -1;
            if (playerObjectPosition.Count == 0) return;

            if (playerObjectPosition.Count == 1)
            {
                //Set Zoom Level to Position so we can Move to that location
                int xPos = (int)(playerObjectPosition[0].Position.X.High);
                int yPos = (int)(playerObjectPosition[0].Position.Y.High);
                GoToPosition(xPos, yPos);
            }
            else
            {
                GoToPlayerBox goToPlayerBox = new GoToPlayerBox();
                DialogResult result = goToPlayerBox.ShowDialog();
                if (selectPlayerObject_GoTo != -1)
                {
                    int objectIndex = selectPlayerObject_GoTo;
                    int xPos = (int)((int)playerObjectPosition[objectIndex].Position.X.High);
                    int yPos = (int)((int)playerObjectPosition[objectIndex].Position.Y.High);
                    GoToPosition(xPos, yPos);
                }
            }
        }

        private void FindToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            FindandReplaceTool form = new FindandReplaceTool();
            DialogResult result = form.ShowDialog();
            if (result == DialogResult.OK)
            {
                while (form.GetReadyState() == false)
                {

                }
                int applyState = form.GetApplyState();
                bool copyResults = form.CopyResultsOrNot();
                bool replaceMode = form.IsReplaceMode();
                int find = form.GetFindValue();
                int replace = form.GetReplaceValue();
                bool perserveColllision = form.PerservingCollision();

                if (replaceMode)
                {
                    EditorTileFindReplace(find, replace, applyState, copyResults);//, perserveColllision
                }
                else
                {
                    EditorTileFind(find, applyState, copyResults);
                }

            }

        }

        private void FindUnusedTiles(object sender, EventArgs e)
        {
            ToggleEditorButtons(false);
            List<int> UnusedTiles = new List<int> { };

            for (int i = 0; i < 1024; i++)
            {
                Editor.Instance.TilesToolbar.selectedTileLabel.Text = "Selected Tile: " + i;
                bool Unusued = IsTileUnused(i);
                while (cooldownDone != true)
                {
                    Application.DoEvents();
                }
                cooldownDone = false;
                if (Unusued)
                {
                    UnusedTiles.Add(i);
                }
                Application.DoEvents();
            }
            if (UnusedTiles.Count != 0)
            {
                var message = string.Join(Environment.NewLine, UnusedTiles);
                MessageBox.Show("Tiles not used are: " + Environment.NewLine + message, "Results");
            }
            else
            {
                MessageBox.Show("Found Nothing", "Results");
            }
            ToggleEditorButtons(true);

        }

        private void ToolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void ConsoleWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            consoleWindowToolStripMenuItem.Checked = !consoleWindowToolStripMenuItem.Checked;

            if (consoleWindowToolStripMenuItem.Checked)
            {
                ShowConsoleWindow();
            }
            else if (!consoleWindowToolStripMenuItem.Checked)
            {
                HideConsoleWindow();
            }
        }

        private void SaveForForceOpenOnStartupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mySettings.DevForceRestartData = DataDirectory;
            mySettings.DevForceRestartScene = ScenePath;
            mySettings.DevForceRestartX = (short)(ShiftX / Zoom);
            mySettings.DevForeRestartY = (short)(ShiftY / Zoom);
            mySettings.DevForceRestartZoomLevel = ZoomLevel;
            mySettings.DevForceRestartEncore = Editor.Instance.useEncoreColors;
            mySettings.DeveForceRestartLevelID = Editor.Instance.myEditorState.Level_ID;
        }

        private void WikiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://docs.google.com/document/d/1NBvcqzvOzqeTVzgAYBR0ttAc5vLoFaQ4yh_cdf-7ceQ/edit?usp=sharing");
        }

        //TO-MOVE
        private void EditEntitiesOptionToolStrip_DropDownOpening(object sender, EventArgs e)
        {

        }





        #endregion

        #region Collision Toolstrip Menu Item Entries

        private void DefaultToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (defaultToolStripMenuItem.Checked)
            {
                invertedToolStripMenuItem.Checked = false;
                customToolStripMenuItem.Checked = false;
                collisionPreset = 0;
                ReloadSpecificTextures(sender, e);
                RefreshCollisionColours(true);
            }
            else
            {
                defaultToolStripMenuItem.Checked = true;
                invertedToolStripMenuItem.Checked = false;
                customToolStripMenuItem.Checked = false;
                collisionPreset = 0;
                ReloadSpecificTextures(sender, e);
                RefreshCollisionColours(true);
            }
        }

        private void InvertedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (invertedToolStripMenuItem.Checked)
            {
                defaultToolStripMenuItem.Checked = false;
                customToolStripMenuItem.Checked = false;
                collisionPreset = 1;
                ReloadSpecificTextures(sender, e);
                RefreshCollisionColours(true);
            }
            else
            {
                defaultToolStripMenuItem.Checked = true;
                invertedToolStripMenuItem.Checked = false;
                customToolStripMenuItem.Checked = false;
                collisionPreset = 0;
                ReloadSpecificTextures(sender, e);
                RefreshCollisionColours(true);
            }
        }

        private void CustomToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (customToolStripMenuItem1.Checked)
            {
                defaultToolStripMenuItem.Checked = false;
                invertedToolStripMenuItem.Checked = false;
                collisionPreset = 2;
                ReloadSpecificTextures(sender, e);
                RefreshCollisionColours(true);
            }
            else
            {
                defaultToolStripMenuItem.Checked = true;
                invertedToolStripMenuItem.Checked = false;
                customToolStripMenuItem.Checked = false;
                collisionPreset = 0;
                ReloadSpecificTextures(sender, e);
                RefreshCollisionColours(true);
            }
        }

        #endregion

        #region Annimations Button Toolstrip Items

        private void MovingPlatformsObjectsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (movingPlatformsObjectsToolStripMenuItem.Checked == false)
            {
                movingPlatformsObjectsToolStripMenuItem.Checked = true;
                myEditorState.movingPlatformsChecked = true;
            }
            else
            {
                movingPlatformsObjectsToolStripMenuItem.Checked = false;
                myEditorState.movingPlatformsChecked = false;
            }

        }

        private void SpriteFramesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (spriteFramesToolStripMenuItem.Checked == false)
            {
                spriteFramesToolStripMenuItem.Checked = true;
                myEditorState.annimationsChecked = true;
            }
            else
            {
                spriteFramesToolStripMenuItem.Checked = false;
                myEditorState.annimationsChecked = false;
            }
        }

        #endregion

        #region GetScreen + Get Zoom

        public Rectangle GetScreen()
        {   
            if (mySettings.EntityFreeCam) return new Rectangle(CustomX, CustomY, viewPanel.Width, viewPanel.Height);
            else return new Rectangle(ShiftX, ShiftY, viewPanel.Width, viewPanel.Height);
        }

        public double GetZoom()
        {
            return Zoom;
        }



        #endregion

        #region Theming Stuff

        public void UseDarkTheme(bool state = false)
        {
            if (state)
            {
                SystemColorsUtility systemColors = new SystemColorsUtility();
                systemColors.SetColor(KnownColor.Window, darkTheme1);
                systemColors.SetColor(KnownColor.Highlight, Color.Blue);
                systemColors.SetColor(KnownColor.WindowFrame, darkTheme2);
                systemColors.SetColor(KnownColor.GradientActiveCaption, darkTheme1);
                systemColors.SetColor(KnownColor.GradientInactiveCaption, darkTheme1);
                systemColors.SetColor(KnownColor.ControlText, darkTheme3);
                systemColors.SetColor(KnownColor.WindowText, darkTheme3);
                systemColors.SetColor(KnownColor.GrayText, Color.Gray);
                systemColors.SetColor(KnownColor.InfoText, darkTheme2);
                systemColors.SetColor(KnownColor.MenuText, darkTheme3);
                systemColors.SetColor(KnownColor.Control, darkTheme1);
                systemColors.SetColor(KnownColor.ButtonHighlight, darkTheme3);
                systemColors.SetColor(KnownColor.ButtonShadow, darkTheme2);
                systemColors.SetColor(KnownColor.ButtonFace, darkTheme1);
                systemColors.SetColor(KnownColor.Desktop, darkTheme1);
                systemColors.SetColor(KnownColor.ControlLightLight, darkTheme2);
                systemColors.SetColor(KnownColor.ControlLight, darkTheme1);
                systemColors.SetColor(KnownColor.ControlDark, darkTheme3);
                systemColors.SetColor(KnownColor.ControlDarkDark, darkTheme3);
                systemColors.SetColor(KnownColor.ActiveBorder, darkTheme1);
                systemColors.SetColor(KnownColor.ActiveCaption, darkTheme1);
                systemColors.SetColor(KnownColor.ActiveCaptionText, darkTheme3);
                systemColors.SetColor(KnownColor.InactiveBorder, darkTheme2);
                systemColors.SetColor(KnownColor.MenuBar, darkTheme1);
            }
            else
            {
                SystemColorsUtility systemColors = new SystemColorsUtility();
                systemColors.SetColor(KnownColor.Window, SystemColors.Window);
                systemColors.SetColor(KnownColor.Highlight, SystemColors.Highlight);
                systemColors.SetColor(KnownColor.WindowFrame, SystemColors.WindowFrame);
                systemColors.SetColor(KnownColor.GradientActiveCaption, SystemColors.GradientActiveCaption);
                systemColors.SetColor(KnownColor.GradientInactiveCaption, SystemColors.GradientInactiveCaption);
                systemColors.SetColor(KnownColor.ControlText, SystemColors.ControlText);
                systemColors.SetColor(KnownColor.WindowText, SystemColors.WindowText);
                systemColors.SetColor(KnownColor.GrayText, SystemColors.GrayText);
                systemColors.SetColor(KnownColor.InfoText, SystemColors.InfoText);
                systemColors.SetColor(KnownColor.MenuText, SystemColors.MenuText);
                systemColors.SetColor(KnownColor.Control, SystemColors.Control);
                systemColors.SetColor(KnownColor.ButtonHighlight, SystemColors.ButtonHighlight);
                systemColors.SetColor(KnownColor.ButtonShadow, SystemColors.ButtonShadow);
                systemColors.SetColor(KnownColor.ButtonFace, SystemColors.ButtonFace);
                systemColors.SetColor(KnownColor.Desktop, SystemColors.Desktop);
                systemColors.SetColor(KnownColor.ControlLightLight, SystemColors.ControlLightLight);
                systemColors.SetColor(KnownColor.ControlLight, SystemColors.ControlLight);
                systemColors.SetColor(KnownColor.ControlDark, SystemColors.ControlDark);
                systemColors.SetColor(KnownColor.ControlDarkDark, SystemColors.ControlDarkDark);
                systemColors.SetColor(KnownColor.ActiveBorder, SystemColors.ActiveBorder);
                systemColors.SetColor(KnownColor.ActiveCaption, SystemColors.ActiveCaption);
                systemColors.SetColor(KnownColor.ActiveCaptionText, SystemColors.ActiveCaptionText);
                systemColors.SetColor(KnownColor.InactiveBorder, SystemColors.InactiveBorder);
                systemColors.SetColor(KnownColor.MenuBar, SystemColors.MenuBar);
            }

        }

        public void SetButtonColors(object sender, Color OverallColor)
        {
            //Set the Overall Color for the Black Editor Buttons
            ToolStripButton button;
            ToolStripSplitButton splitButton;
            if (sender is ToolStripButton)
            {
                button = sender as ToolStripButton;
                Bitmap pic = new Bitmap(button.Image);
                for (int y = 0; (y <= (pic.Height - 1)); y++)
                {
                    for (int x = 0; (x <= (pic.Width - 1)); x++)
                    {
                        Color inv = pic.GetPixel(x, y);
                        inv = Color.FromArgb(inv.A, OverallColor.R, OverallColor.G, OverallColor.B);
                        pic.SetPixel(x, y, inv);
                    }
                }
                button.Image = pic;
            }
            else if (sender is ToolStripSplitButton)
            {
                splitButton = sender as ToolStripSplitButton;
                Bitmap pic = new Bitmap(splitButton.Image);
                for (int y = 0; (y <= (pic.Height - 1)); y++)
                {
                    for (int x = 0; (x <= (pic.Width - 1)); x++)
                    {
                        Color inv = pic.GetPixel(x, y);
                        inv = Color.FromArgb(inv.A, OverallColor.R, OverallColor.G, OverallColor.B);
                        pic.SetPixel(x, y, inv);
                    }
                }
                splitButton.Image = pic;
            }
        }

        public void SetupButtonColors()
        {
            SetButtonColors(New, MainThemeColor());
            SetButtonColors(Open, MainThemeColor(Color.FromArgb(0xFFE793), Color.FromArgb(0xFAD962)));
            SetButtonColors(RecentDataDirectories, MainThemeColor(Color.FromArgb(0xFFE793), Color.FromArgb(0xFAD962)));
            SetButtonColors(Save, Color.RoyalBlue);
            SetButtonColors(ZoomInButton, Color.SlateBlue);
            SetButtonColors(ZoomOutButton, Color.SlateBlue);
            SetButtonColors(RunSceneButton, MainThemeColor(Color.LimeGreen));
            SetButtonColors(FreezeDeviceButton, Color.Red);
            SetButtonColors(UndoButton, Color.RoyalBlue);
            SetButtonColors(RedoButton, Color.RoyalBlue);
            SetButtonColors(ReloadButton, Color.RoyalBlue);
            SetButtonColors(PointerButton, MainThemeColor());
            SetButtonColors(SelectToolButton, Color.MediumPurple);
            SetButtonColors(PlaceTilesButton, Color.Green);
            SetButtonColors(InteractionToolButton, Color.Gold);
            SetButtonColors(MagnetMode, Color.Red);
            SetButtonColors(ChunksToolButton, Color.SandyBrown);
            SetButtonColors(ShowTileIDButton, MainThemeColor());
            SetButtonColors(ShowGridButton, MainThemeColor(Color.Gray));
            SetButtonColors(ShowCollisionAButton, Color.DeepSkyBlue);
            SetButtonColors(ShowCollisionBButton, Color.DeepSkyBlue);
            SetButtonColors(FlipAssistButton, MainThemeColor());
            SetButtonColors(MoreSettingsButton, MainThemeColor());
            SetButtonColors(PreloadSceneButton, MainThemeColor());
            SetButtonColors(MultiSelectButton, MainThemeColor());
            if (mySettings.NightMode) MoreSettingsButton.ForeColor = Color.White;

        }

        public Color MainThemeColor(Color? CDC = null, Color? CWC = null)
        {
            Color NightColor;
            Color NormalColor;
            if (CDC != null) NightColor = CDC.Value;
            else NightColor = Color.White;

            if (CWC != null) NormalColor = CWC.Value;
            else NormalColor = Color.Black;

            return (mySettings.NightMode ? NightColor : NormalColor);
        }

        #endregion

        #region Game Manipulation Stuff

        private void SetPlayerRespawnToHereToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Point clicked_point = new Point((int)(lastX / Zoom), (int)(lastY / Zoom));
            if (Editor.GameRunning)
            {
                EditorGame.UpdateCheckpoint(clicked_point);
            }
        }

        private void MoveThisPlayerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Point clicked_point = new Point((int)(lastX / Zoom), (int)(lastY / Zoom));
            if (EditorGame.GetPlayerAt(clicked_point) != -1)
            {
                playerSelected = true;
                selectedPlayer = EditorGame.GetPlayerAt(clicked_point);
            }
        }

        private void Player1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            playerSelected = true;
            selectedPlayer = 0;
        }

        private void Player2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            playerSelected = true;
            selectedPlayer = 1;
        }

        private void Player3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            playerSelected = true;
            selectedPlayer = 2;
        }

        private void Player4ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            playerSelected = true;
            selectedPlayer = 3;
        }

        private void MoveCheckpointToolStripMenuItem_Click(object sender, EventArgs e)
        {
            checkpointSelected = true;
        }

        private void RemoveCheckpointToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditorGame.UpdateCheckpoint(new Point(0, 0), false);
        }

        private void AssetResetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int CurrentScene = 0x00E48758;
            int GameState = 0x00E48776;
            int timer = 0;
            IntPtr processHandle = Editor.GameMemory.ProcessHandle;

            EditorGame.UpdateCheckpoint(new Point((int)(lastX / Zoom), (int)(lastY / Zoom)));

            byte[] oldScene = EditorGame.ReadMemory(CurrentScene, 1, (int)processHandle);
            GameMemory.WriteByte(CurrentScene, 2);
            GameMemory.WriteByte(GameState, 0);

            while (timer <= 100000)
            {
                timer++;
            }
            GameMemory.WriteByte(CurrentScene, oldScene[0]);
            GameMemory.WriteByte(GameState, 0);
        }

        private void RestartSceneToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        #endregion

        #region Miscellaneous

        private void CreateShortcut(string dataDir, string scenePath = "", string modPath = "", int X = 0, int Y = 0, bool isEncoreMode = false, int LevelSlotNum = -1)
        {
            object shDesktop = (object)"Desktop";
            WshShell shell = new WshShell();
            string shortcutAddress = "";
            if (scenePath != "")
            {
                shortcutAddress = (string)shell.SpecialFolders.Item(ref shDesktop) + @"\" + "Scene Link" + " - Maniac.lnk";
            }
            else
            {
                shortcutAddress = (string)shell.SpecialFolders.Item(ref shDesktop) + @"\" + "Data Folder Link" + " - Maniac.lnk";
            }
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutAddress);

            string targetAddress = "\"" + Environment.CurrentDirectory + @"\ManiacEditor.exe" + "\"";
            string launchArguments = "";
            if (scenePath != "")
            {
                launchArguments = (dataDir != "" ? "DataDir=" + "\"" + dataDir + "\" " : "") + (scenePath != "" ? "ScenePath=" + "\"" + scenePath + "\" " : "") + (modPath != "" ? "ModPath=" + "\"" + modPath + "\" " : "") + (LevelSlotNum != -1 ? "LevelID=" + LevelSlotNum.ToString() + " " : "") + (isEncoreMode == true ? "EncoreMode=TRUE " : "") + (X != 0 ? "X=" + X.ToString() + " " : "") + (Y != 0 ? "Y=" + Y.ToString() + " " : "");
            }
            else
            {
                launchArguments = (dataDir != "" ? "DataDir=" + "\"" + dataDir + "\" " : "");
            }

            shortcut.TargetPath = targetAddress;
            shortcut.Arguments = launchArguments;
            shortcut.WorkingDirectory = Environment.CurrentDirectory;
            shortcut.Save();
        }

        private void MoveThePlayerToHereToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Editor.GameRunning)
            {         
                int ObjectAddress = 0x85E9A0;
                Editor.GameMemory.WriteInt16(ObjectAddress + 2, (short)(lastX / Zoom));
                Editor.GameMemory.WriteInt16(ObjectAddress + 6, (short)(lastY / Zoom));
            }
        }

        private void LangToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach(ToolStripMenuItem item in menuLanguageToolStripMenuItem.DropDownItems) item.Checked = false;
            ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
            CurrentLanguage = menuItem.Tag.ToString();
            menuItem.Checked = true;
        }

        private void TrackPlayerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            if (item != null)
            {
                if (!item.Checked)
                {
                    item.Checked = true;
                    int.TryParse(item.Tag.ToString(), out int player);
                    PlayerBeingTracked = player;
                }
                else
                {
                    item.Checked = false;
                    PlayerBeingTracked = -1;
                }
                

            }
        }

        private void SeeStatsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var StatsBox = new StatusBox())
            {
                StatsBox.ShowDialog();
            }
        }

        private void EditTileWithTileManiacToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Editor.Instance.mainform.IsDisposed) Editor.Instance.mainform = new TileManiac.Mainform();
            if (!Editor.Instance.mainform.Visible)
            {
                Editor.Instance.mainform.Show();
            }
            if (Editor.Instance.TilesConfig != null && Editor.Instance.StageTiles != null)
            {
                if (!Editor.Instance.mainform.Visible || Editor.Instance.mainform.tcf == null)
                {
                    Editor.Instance.mainform.LoadTileConfigViaIntergration(Editor.Instance.TilesConfig, Editor.Instance.SceneFilepath, SelectedTileID);
                }
                else
                {
                    Editor.Instance.mainform.SetCollisionIndex(SelectedTileID);
                    Editor.Instance.mainform.Activate();
                }

            }
        }

        private void EnableAllButtonsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < mainToolStrip.Items.Count; i++)
            {
                mainToolStrip.Items[i].Enabled = true;
            }
            
        }

        private void ShowError(string message, string title = "Error!")
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
            /*using (var customMsgBox = new CustomMsgBox(message, title, 1, 1))
            {
                customMsgBox.ShowDialog();
            }*/
        }

        private void ResetDeviceButton_Click_1(object sender, EventArgs e)
        {
            if (FreezeDeviceButton.Checked)
            {
                GraphicPanel.bRender = false;
            }
            else
            {
                ReloadToolStripButton_Click(null, null);
                GraphicPanel.bRender = true;
            }
        }

        private void SetDeviceSleepState(bool state)
        {
            GraphicPanel.bRender = state;
            if (state == true)
            {
                ReloadToolStripButton_Click(null, null);
            }
        }

        private bool CanWriteFile(string fullFilePath)
        {
            if (!File.Exists(fullFilePath)) return true;

            if (File.GetAttributes(fullFilePath).HasFlag(FileAttributes.ReadOnly))
            {
                ShowError($"The file '{fullFilePath}' is Read Only.", "File is Read Only.");
                return false;
            }

            var result = MessageBox.Show($"The file '{fullFilePath}' already exists. Overwrite?", "Overwrite?",
                                         MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes) return true;

            return false;
        }

        public Scene GetSceneSelection()
        {
            string selectedScene;
            using (SceneSelect select = new SceneSelect(GameConfig))
            {
                select.ShowDialog();

                if (select.Result == null)
                    return null;

                selectedScene = select.Result;
            }
            if (!File.Exists(selectedScene))
            {
                string[] splitted = selectedScene.Split('\\');

                string part1 = splitted[0];
                string part2 = splitted[1];

                selectedScene = Path.Combine(DataDirectory, "Stages", part1, part2);
            }
            return new Scene(selectedScene);
        }

        private void SceneChangeWarning(object sender, EventArgs e)
        {
            if (IsSceneLoaded() == true && mySettings.DisableSaveWarnings == false)
            {
                DialogResult exitBoxResult;
                using (var exitBox = new ExitWarningBox())
                {
                    exitBox.ShowDialog();
                    exitBoxResult = exitBox.DialogResult;
                }
                if (exitBoxResult == DialogResult.Yes)
                {
                    Save_Click(sender, e);
                    AllowSceneChange = true;
                }
                else if (exitBoxResult == DialogResult.No)
                {
                    AllowSceneChange = true;
                }
                else
                {
                    AllowSceneChange = false;
                }
            }
            else
            {
                AllowSceneChange = true;
            }

        }

        public void GoToPosition(int x, int y, bool CenterCoords = true, bool ShortcutClear = false)
        {
            if (CenterCoords)
            {
                Rectangle r = GraphicPanel.GetScreen();
                int x2 = (int)(r.Width * Zoom);
                int y2 = (int)(r.Height * Zoom);

                int ResultX = (int)(x * Zoom) - x2 / 2;
                int ResultY = (int)(y * Zoom) - y2 / 2;

                if ((ResultX <= 0)) ResultX = 0;
                if ((ResultY <= 0)) ResultY = 0;

                ShiftX = ResultX;
                ShiftY = ResultY;
            }
            else
            {
                int ResultX = (int)(x * Zoom);
                int ResultY = (int)(y * Zoom);

                if ((ResultX <= 0)) ResultX = 0;
                if ((ResultY <= 0)) ResultY = 0;

                ShiftX = ResultX;
                ShiftY = ResultY;
            }


            if (ShortcutClear)
            {
                ForceWarp = false;
                TempWarpCoords = new Point(0, 0);
            }

        }

        public void UpdateRender()
        {
            if (!mySettings.RellyOnRenderLoopForUpdatingOnly && GraphicPanel.bRender)
            {
                GraphicPanel.Render();
            }
        }

        public void OnMouseMoveEvent()
        {
            if (!mySettings.RellyOnRenderLoopForUpdatingOnly && GraphicPanel.bRender)
            {
                GraphicPanel.OnMouseMoveEventCreate();
            }
        }


        public class SystemColorsUtility
        {
            public SystemColorsUtility()
            {
                // force init color table
                byte unused = SystemColors.Window.R;

                var colorTableField = typeof(Color).Assembly.GetType("System.Drawing.KnownColorTable")
                    .GetField("colorTable", BindingFlags.Static | BindingFlags.NonPublic);

                _colorTable = (int[])colorTableField.GetValue(null);
            }

            public void SetColor(KnownColor knownColor, Color value)
            {
                _colorTable[(int)knownColor] = value.ToArgb();
            }

            private readonly int[] _colorTable;
        }



        #endregion

    }
}