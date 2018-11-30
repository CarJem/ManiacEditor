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

        //Editor Status States (Like are we pre-loading a scene)
        public bool importingObjects = false; //Determines if we are importing objects so we can disable all the other Scene Select Options
        public static bool isPreRending = false; //Determines if we are Preloading a Scene
        bool AllowSceneChange = false; // For the Save Warning Dialog
        bool encorePaletteExists = false; // Determines if an Encore Pallete Exists
        bool forceResize = false; //For Opening a Scene Forcefully
        int forceResizeGoToX = 0; //For Opening a Scene Forcefully and then going to the specified X
        int forceResizeGoToY = 0; //For Opening a Scene Forcefullyand then going to the specified Y
        //bool onRenderActive = false;


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

        //Editor Paths
        public static string DataDirectory; //Used to get the current Data Directory
        public string SelectedZone; //Used to get the Selected zone
        string SelectedScene; //Used to get the Scene zone
        public static string[] EncorePalette = new string[6]; //Used to store the location of the encore palletes
        string SceneFilename = null; //Used for fetching the scene's file name
        string SceneFilepath = null; //Used for fetching the folder that contains the scene file
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
        public List<string> entityRenderingObjects = EditorEntity_ini.getSpecialRenderList(1); //Used to get the Render List for Objects
        public List<string> renderOnScreenExlusions = EditorEntity_ini.getSpecialRenderList(0); //Used to get the Always Render List for Objects
        public IList<ToolStripMenuItem> _recentDataItems; //Used to get items for the Data Directory Toolstrip Area
        private IList<ToolStripMenuItem> _recentDataItems_Button; //Used to get items for the Data Directory Button Toolstrip
        public IList<SceneEntity> playerObjectPosition = new List<SceneEntity> { }; //Used to store the scenes current playerObjectPositions

        //Used for Get Common Layers
        internal EditorLayer FGHigher => EditorScene?.HighDetails;
        internal EditorLayer FGHigh => EditorScene?.ForegroundHigh;
        internal EditorLayer FGLow => EditorScene?.ForegroundLow;
        internal EditorLayer FGLower => EditorScene?.LowDetails;
        internal EditorLayer ScratchLayer => EditorScene?.Scratch;

        internal IEnumerable<EditorLayer> AllLayers => EditorScene?.AllLayers;
        //Used to Get the Maximum Layer Height and Width
        internal int SceneWidth => EditorScene.Layers.Max(sl => sl.Width) * 16;
        internal int SceneHeight => EditorScene.Layers.Max(sl => sl.Height) * 16;

        //Used for "Run Scene"
        public static ProcessMemory GameMemory = new ProcessMemory(); //Allows us to write hex codes like cheats, etc.
        public static bool GameRunning = false; //Tells us if the game is running
        public static string GamePath = ""; //Tells us where the game is located

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
        private EntitiesToolbar entitiesToolbar = null;
        public EditorUpdater Updater = new EditorUpdater();

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


        public Editor()
        {
            Instance = this;
            useDarkTheme(mySettings.NightMode);
            InitializeComponent();
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
            if (!Updater.GetVersion().Contains("DEV") && mySettings.checkForUpdatesAuto)
            {
                Updater.CheckforUpdates();
            }
            this.splitContainer1.Panel2MinSize = 254;

            GraphicPanel.Width = SystemInformation.PrimaryMonitorSize.Width;
            GraphicPanel.Height = SystemInformation.PrimaryMonitorSize.Height;

            _extraLayerEditButtons = new List<ToolStripButton>();
            _extraLayerViewButtons = new List<ToolStripButton>();
            _extraLayerSeperators = new List<ToolStripSeparator>();
            _recentDataItems = new List<ToolStripMenuItem>();
            _recentDataItems_Button = new List<ToolStripMenuItem>();
            EditorControls = new EditorControls();

            SetViewSize();


            UpdateControls();

            TryLoadSettings();

            if (mySettings.UseForcefulStartup)
            {
                OpenSceneForceFully();
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
            Dictionary<String,String> ListedPrefrences = SettingsReader.ReturnPrefrences();
            if (ListedPrefrences.ContainsKey("LevelID"))
            {
                string value;
                ListedPrefrences.TryGetValue("LevelID", out value);
                int resultingInt;
                Int32.TryParse(value, out resultingInt);
                if (resultingInt >= -1)
                {
                    myEditorState.Level_ID = resultingInt;
                }

            }
            if (ListedPrefrences.ContainsKey("FGLower"))
            {
                string value;
                ListedPrefrences.TryGetValue("FGLower", out value);
                INILayerNameLower = value;
            }
            if (ListedPrefrences.ContainsKey("FGHigher"))
            {
                string value;
                ListedPrefrences.TryGetValue("FGHigher", out value);
                INILayerNameHigher = value;
            }
            if (ListedPrefrences.ContainsKey("WaterColor"))
            {
                string value;
                ListedPrefrences.TryGetValue("WaterColor", out value);
                Debug.Print(value);
                Color color = System.Drawing.ColorTranslator.FromHtml(value);
                
                if (ListedPrefrences.ContainsKey("WaterColorAlpha"))
                {
                    string value2;
                    ListedPrefrences.TryGetValue("WaterColorAlpha", out value2);
                    int alpha;
                    Int32.TryParse(value2, out alpha);
                    color = Color.FromArgb(alpha, color.R, color.G, color.B);
                }
                waterColor = color;
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
            return newItem;
        }

        private void ModConfigItemClicked(object sender, EventArgs e)
        {
            var modConfig_CheckedItem = (sender as ToolStripMenuItem);
            selectConfigToolStripMenuItem_Click(modConfig_CheckedItem);
        }

        public void editConfigsToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void selectConfigToolStripMenuItem_Click(ToolStripMenuItem modConfig_CheckedItem)
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
            goToToolStripMenuItem2.Enabled = enabled;

            ShowFGHigh.Enabled = enabled && FGHigh != null;
            ShowFGLow.Enabled = enabled && FGLow != null;
            ShowFGHigher.Enabled = enabled && FGHigher != null;
            ShowFGLower.Enabled = enabled && FGLower != null;
            ShowEntities.Enabled = enabled;
            ShowAnimations.Enabled = enabled;
            animationsSplitButton.Enabled = enabled;
            ReloadToolStripButton.Enabled = enabled;
            preLoadSceneButton.Enabled = enabled;

            Save.Enabled = enabled;

            if (mySettings.ReduceZoom)
            {
                zoomInButton.Enabled = enabled && ZoomLevel < 5;
                zoomOutButton.Enabled = enabled && ZoomLevel > -2;
            }
            else
            {
                zoomInButton.Enabled = enabled && ZoomLevel < 5;
                zoomOutButton.Enabled = enabled && ZoomLevel > -5;
            }



            runSceneButton.Enabled = enabled && !GameRunning;

            SetEditButtonsState(enabled);
            UpdateTooltips();

            if (mySettings.preRenderSceneOption == 3 && enabled && stageLoad)
            {
                preLoadSceneButton_Click(null, null);
            }
            else if (mySettings.preRenderSceneOption == 2 && enabled && stageLoad)
            {
                DialogResult result = MessageBox.Show("Do you wish to Pre-Render this scene?", "Requesting to Pre-Render the Scene", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (result == DialogResult.Yes)
                {
                    preLoadSceneButton_Click(null, null);
                }
            }
            else if (mySettings.preRenderSceneOption == 1 && Properties.EditorState.Default.preRenderSceneSelectCheckbox && enabled && stageLoad)
            {
                preLoadSceneButton_Click(null, null);
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

            openDataDirectoryFolderToolStripMenuItem.Enabled = enabled;
            openSonicManiaFolderToolStripMenuItem.Enabled = enabled;
            openSceneFolderToolStripMenuItem.Enabled = enabled;

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

            

            undoButton.Enabled = enabled && undo.Count > 0;
            redoButton.Enabled = enabled && redo.Count > 0;

            findAndReplaceToolStripMenuItem.Enabled = enabled && EditLayer != null;

            pointerButton.Enabled = enabled && IsTilesEdit();
            selectTool.Enabled = enabled && IsTilesEdit();
            placeTilesButton.Enabled = enabled && IsTilesEdit();

            showGridButton.Enabled = enabled && StageConfig != null;
            showCollisionAButton.Enabled = enabled && StageConfig != null;
            showCollisionBButton.Enabled = enabled && StageConfig != null;
            showTileIDButton.Enabled = enabled && StageConfig != null;
            gridSizeButton.Enabled = enabled && StageConfig != null;
            enableEncorePalette.Enabled = enabled && encorePaletteExists;

            playerSpawnToolStripMenuItem.Enabled = enabled;
            goToToolStripMenuItem2.Enabled = enabled;



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


            if (IsTilesEdit())
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
                TilesToolbar.ShowShortcuts = placeTilesButton.Checked;
            }
            else
            {
                TilesToolbar?.Dispose();
                TilesToolbar = null;
            }
            if (IsEntitiesEdit())
            {
                if (entitiesToolbar == null)
                {
                    entitiesToolbar = new EntitiesToolbar(EditorScene.Objects);
                    //entitiesToolbar = new EntitiesToolbar(ObjectList);
                    entitiesToolbar.SelectedEntity = new Action<int>(x =>
                    {
                        entities.SelectSlot(x);
                        SetSelectOnlyButtonsState();
                    });
                    entitiesToolbar.AddAction = new Action<IAction>(x =>
                    {
                        undo.Push(x);
                        redo.Clear();
                        UpdateControls();
                    });
                    entitiesToolbar.Spawn = new Action<SceneObject>(x =>
                    {
                        entities.Add(x, new Position((short)(ShiftX / Zoom), (short)(ShiftY / Zoom)));
                        undo.Push(entities.LastAction);
                        redo.Clear();
                        UpdateControls();
                    });
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
            if (TilesToolbar == null && entitiesToolbar == null)
            {
                splitContainer1.Panel2Collapsed = true;
                Form1_Resize(null, null);
            }

            SetSelectOnlyButtonsState(enabled);
        }

        private void UpdateControls(bool stageLoad = false)
        {
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
            if (IsTilesEdit())
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
            hVScrollBarXYLabel.Text = "Memory Used: " + memory.ToString();


            //
            // End of Tooltip Bar Info Section
            //
        }

        private void UpdateTooltips()
        {
            UpdateTooltipForStacks(undoButton, undo);
            UpdateTooltipForStacks(redoButton, redo);
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

        public void toggleEditorButtons(bool enabled)
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
            Dictionary<Point, ushort> tiles = new Dictionary<Point, ushort>();
            tiles[new Point(0, 0)] = (ushort)tile;
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


                            //Debug.Print(listValue[i].ToString());
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
            entities.Flip(direction);
            entitiesToolbar.UpdateCurrentEntityProperites();
        }

        public void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
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


            if (ClickedX != -1)
            {
                Point clicked_point = new Point((int)(ClickedX / Zoom), (int)(ClickedY / Zoom));
                // There was just a click now we can determine that this click is dragging

                if (IsTilesEdit())
                {
                    
                    if ((EditLayer?.IsPointSelected(clicked_point)).Value)
                    {
                        // Start dragging the tiles
                        dragged = true;
                        startDragged = true;
                        EditLayer?.StartDrag();
            
                    }

                    else if (!selectTool.Checked && !ShiftPressed() && !CtrlPressed() && (EditLayer?.HasTileAt(clicked_point)).Value)
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
                if (IsTilesEdit() && placeTilesButton.Checked)
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
                    if (IsTilesEdit())
                    {
                        if (placeTilesButton.Checked)
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

                if (scrolling)
                {
                    scrolling = false;
                    Cursor = Cursors.Default;
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                if (IsTilesEdit() && placeTilesButton.Checked)
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
            MessageBox.Show("Something is wrong with this GameConfig that we can't support! If for some reason it does work for you in Sonic Mania, you can create another GameConfig.bin called GameConfig_ME.bin and the editor should load that instead (assuming it's a clean GameConfig or one that works) allowing you to still be able to use the data folder, however, this is experimental so be careful when doing that.", "GameConfig Error!");
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
                    if (mySettings.forceBrowse == true)
                        OpenScene(true);
                    else
                        OpenScene();
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
            var startRecentItemsButton = toolStripSplitButton1.DropDownItems.IndexOf(noRecentDataDirectoriesToolStripMenuItem);

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
                toolStripSplitButton1.DropDownItems.Insert(startRecentItemsButton, menuItem);
            }


        }

        private void UpdateDataFolderLabel(string dataDirectory = null)
        {
            if (dataDirectory != null)
            {
                _baseDataDirectoryLabel.Text = string.Format(_baseDataDirectoryLabel.Tag.ToString(),
                                                 dataDirectory);
            }
            else
            {
                _baseDataDirectoryLabel.Text = string.Format(_baseDataDirectoryLabel.Tag.ToString(),
                                                 DataDirectory);
            }

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
                toolStripSplitButton1.DropDownItems.Remove(menuItem);
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
            vScrollBar1.Maximum = height;
            hScrollBar1.Maximum = width;

            GraphicPanel.DrawWidth = Math.Min(width, GraphicPanel.Width);
            GraphicPanel.DrawHeight = Math.Min(height, GraphicPanel.Height);

            Form1_Resize(null, null);

            hScrollBar1.Value = Math.Max(0, Math.Min(hScrollBar1.Value, hScrollBar1.Maximum - hScrollBar1.LargeChange));
            vScrollBar1.Value = Math.Max(0, Math.Min(vScrollBar1.Value, vScrollBar1.Maximum - vScrollBar1.LargeChange));
        }

        private void ResetViewSize()
        {
            SetViewSize((int)(SceneWidth * Zoom), (int)(SceneHeight * Zoom));
        }

        private void ResizeGraphicPanel(int width = 0, int height = 0)
        {
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
                tsb.ForeColor = Color.ForestGreen;
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
                tsb.ForeColor = Color.DarkGreen;
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
            OpenFileDialog open = new OpenFileDialog();
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
        private bool load()
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

            t = new System.Windows.Forms.Timer();
            t.Interval = 10;
            t.Tick += new EventHandler(UpdateStatusPanel);
            t.Start();

            SelectedScene = null;
            SelectedZone = null;
            enableEncorePalette.Checked = false;

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
        }

        private void OpenSceneForceFully()
        {

            DataDirectory = mySettings.DevForceRestartData;
            string Result = mySettings.DevForceRestartScene;
            int LevelID = mySettings.DeveForceRestartLevelID;
            bool isEncore = mySettings.DevForceRestartEncore;
            forceResize = true;
            int x = mySettings.DevForceRestartX;
            int y = mySettings.DevForeRestartY;
            forceResizeGoToX = mySettings.DevForceRestartX;
            forceResizeGoToY = mySettings.DevForeRestartY;
            OpenScene(false, Result, LevelID, isEncore, true);


        }

        private void OpenScene(bool manual = false, string Result = null, int LevelID = -1, bool isEncore = false, bool shortcut = false)
        {
            SceneSelect select;
            string ResultPath = null;
            if (Result == null)
            {
                if (manual == false)
                {

                    if (!load())
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

                }
                else
                {
                    OpenFileDialog open = new OpenFileDialog();
                    open.Filter = "Scene File|*.bin";
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
            //Debug.Print(Result);

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
                enableEncorePalette.Checked = true;
                useEncoreColors = true;
            }

            try
            {
                int searchType = 0;
                if (File.Exists(Result))
                {
                    // Selected file
                    // Don't forget to populate these Members
                    string directoryPath = Path.GetDirectoryName(Result);
                    SelectedZone = new DirectoryInfo(directoryPath).Name;
                    SelectedScene = Path.GetFileName(Result);
                    SceneFilename = Result;
                    SceneFilepath = ResultPath;
                    searchType = 0;
                }
                else
                {
                    SelectedZone = Result.Replace(Path.GetFileName(Result), "");
                    SelectedScene = Path.GetFileName(Result);
                    SceneFilename = Path.Combine(DataDirectory, "Stages", SelectedZone, SelectedScene);
                    SceneFilepath = Path.Combine(DataDirectory, "Stages", SelectedZone);
                    searchType = 1;
                }
                SelectedZone = SelectedZone.Replace("\\", "");

                myEditorState.Level_ID = LevelID;
                EditorScene = new EditorScene(SceneFilename);


                //Encore Palette + Stage Tiles Initaliazation
                EncorePalette = EditorScene.getEncorePalette(SelectedZone, DataDirectory, SelectedScene, Result, searchType);
                EncoreSetupType = EditorScene.GetEncoreSetupType(SelectedZone, DataDirectory, SelectedScene, Result);
                if (EncorePalette[0] != "")
                {
                    encorePaletteExists = true;
                }

                //Encore Palette + Stage Tiles
                if (File.Exists(Result))
                {
                    string directoryPath = Path.GetDirectoryName(Result);
                    if (useEncoreColors == true && EncorePalette[0] != "")
                    {
                        StageTiles = new StageTiles(directoryPath, EncorePalette[0]);
                    }
                    else
                    {
                        StageTiles = new StageTiles(directoryPath);
                    }
                }
                else
                {
                    if (useEncoreColors == true && EncorePalette[0] != "")
                    {
                        StageTiles = new StageTiles(Path.Combine(DataDirectory, "Stages", SelectedZone), EncorePalette[0]);
                    }
                    else
                    {
                        StageTiles = new StageTiles(Path.Combine(DataDirectory, "Stages", SelectedZone));
                    }
                }


                //These cause issues, but not clearing them means when new stages are loaded Collision Mask 0 will be index 1024... (I think)
                CollisionLayerA.Clear();
                CollisionLayerB.Clear();

                if (StageTiles != null && File.Exists(Path.Combine(SceneFilepath, "TileConfig.bin")))
                {
                    for (int i = 0; i < 1024; i++)
                    {
                        CollisionLayerA.Add(StageTiles.Config.CollisionPath1[i].DrawCMask(Color.FromArgb(0, 0, 0, 0), CollisionAllSolid));
                        CollisionLayerB.Add(StageTiles.Config.CollisionPath2[i].DrawCMask(Color.FromArgb(0, 0, 0, 0), CollisionAllSolid));
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
            catch (Exception ex)
            {
                MessageBox.Show("Load failed. Error: " + ex.ToString() + " " + Result);
                return;
            }

            UpdateDataFolderLabel();

            SetupLayerButtons();

            Background = new EditorBackground();

            entities = new EditorEntities(EditorScene);

            SetViewSize(SceneWidth, SceneHeight);

            UpdateControls(true);

            if (shortcut)
            {
                UpdateControls();
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
                if (mySettings.forceBrowse == true)
                    OpenScene(true);
                else
                    OpenScene();

            }
            else
            {
                return;
            }

        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Open_Click(sender, e);
        }

        public void openDataDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void saveAspngToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (EditorScene == null) return;

            SaveFileDialog save = new SaveFileDialog();
            save.Filter = ".png File|*.png";
            save.DefaultExt = "png";
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

        private void exportEachLayerAspngToolStripMenuItem_Click(object sender, EventArgs e)
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

        public void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (EditorScene == null) return;

            if (IsTilesEdit())
            {
                // Apply changes
                Deselect();
            }

            SaveFileDialog save = new SaveFileDialog();
            save.Filter = "Scene File|*.bin";
            save.DefaultExt = "bin";
            save.InitialDirectory = Path.GetDirectoryName(SceneFilename);
            save.RestoreDirectory = false;
            save.FileName = Path.GetFileName(SceneFilename);
            if (save.ShowDialog() != DialogResult.Cancel)
            {
                EditorScene.Write(save.FileName);
            }
        }

        private void backupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            backupTool(null, null);
        }

        private void backupRecoverButton_Click(object sender, EventArgs e)
        {
            string Result = null, ResultOriginal = null, ResultOld = null;
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Backup Scene|*.bin.bak|Old Scene|*.bin.old|Crash Backup Scene|*.bin.crash.bak";
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

        private void removeObjectToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void unloadSceneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UnloadScene();
        }

        #region Backup SubMenu
        private void stageConfigToolStripMenuItem_Click(object sender, EventArgs e)
        {
            backupType = 4;
            backupToolStripMenuItem_Click(null, null);
            backupType = 0;
        }

        private void normalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            backupType = 1;
            backupToolStripMenuItem_Click(null, null);
            backupType = 0;
        }
        #endregion

        #endregion

        #region Edit Tab Buttons
        public void flipHorizontalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditLayer?.FlipPropertySelected(FlipDirection.Horizontal);
            UpdateEditLayerActions();
        }

        public void flipHorizontalIndividualToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditLayer?.FlipPropertySelected(FlipDirection.Horizontal, true);
            UpdateEditLayerActions();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteSelected();
        }

        public void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (IsTilesEdit())
                CopyTilesToClipboard();


            else if (IsEntitiesEdit())
                CopyEntitiesToClipboard();


            UpdateControls();
        }

        public void duplicateToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditorUndo();
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditorRedo();
        }

        public void cutToolStripMenuItem_Click(object sender, EventArgs e)
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

        public void pasteToolStripMenuItem_Click(object sender, EventArgs e)
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

        public void flipVerticalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditLayer?.FlipPropertySelected(FlipDirection.Veritcal);
            UpdateEditLayerActions();
        }

        public void flipVerticalIndividualToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditLayer?.FlipPropertySelected(FlipDirection.Veritcal, true);
            UpdateEditLayerActions();
        }

        #endregion

        #region View Tab Buttons

        private void showEntitiesAboveAllOtherLayersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            uncheckOtherEntityVisibilityStates();
            entityVisibilityType = 2;
            showEntitiesAboveAllOtherLayersToolStripMenuItem.Checked = true;
        }

        private void onTopOnlyWhileEditingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            uncheckOtherEntityVisibilityStates();
            entityVisibilityType = 1;
            onTopOnlyWhileEditingToolStripMenuItem.Checked = true;
        }

        private void constantEntityRenderItem_Click(object sender, EventArgs e)
        {
            uncheckOtherEntityVisibilityStates();
            entityVisibilityType = 0;
            constantEntityRenderItem.Checked = true;
        }

        private void uncheckOtherEntityVisibilityStates()
        {
            showEntitiesAboveAllOtherLayersToolStripMenuItem.Checked = false;
            onTopOnlyWhileEditingToolStripMenuItem.Checked = false;
            constantEntityRenderItem.Checked = false;
        }

        private void moveExtraLayersToFrontToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void toolStripTextBox1_TextChanged(object sender, EventArgs e)
        {
            entitiesTextFilter = toolStripTextBox1.Text;
            EditorEntities.FilterRefreshNeeded = true;
        }

        private void showEntitySelectionBoxesToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void showWaterLevelToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void waterLevelAlwaysShowItem_Click(object sender, EventArgs e)
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

        private void sizeWithBoundsWhenNotSelectedToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void toggleEncoreManiaObjectVisibilityToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toggleEncoreManiaEntitiesToolStripMenuItem_Click(sender, e);
        }

        private void showParallaxSpritesToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void xToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void yToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void showEntityPathToolStripMenuItem_Click(object sender, EventArgs e)
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
        public void importObjectsToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void importSoundsToolStripMenuItem_Click(object sender, EventArgs e)
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
                        sourceStageConfig = new StageConfig(fd.FileName);
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

        private void layerManagerToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void primaryColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorPickerDialog colorSelect = new ColorPickerDialog();
            colorSelect.ForeColor = Color.Black;
            colorSelect.BackColor = Color.White;
            colorSelect.Color = Color.FromArgb(EditorScene.EditorMetadata.BackgroundColor1.R, EditorScene.EditorMetadata.BackgroundColor1.G, EditorScene.EditorMetadata.BackgroundColor1.B);
            DialogResult result = colorSelect.ShowDialog();
            if (result == DialogResult.OK)
            {
                {
                    RSDKv5.Color returnColor = new RSDKv5.Color();
                    returnColor.R = colorSelect.Color.R;
                    returnColor.A = colorSelect.Color.A;
                    returnColor.B = colorSelect.Color.B;
                    returnColor.G = colorSelect.Color.G;
                    EditorScene.EditorMetadata.BackgroundColor1 = returnColor;
                }
              
            }
        }

        private void secondaryColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorPickerDialog colorSelect = new ColorPickerDialog();
            colorSelect.ForeColor = Color.Black;
            colorSelect.BackColor = Color.White;
            colorSelect.Color = Color.FromArgb(EditorScene.EditorMetadata.BackgroundColor2.R, EditorScene.EditorMetadata.BackgroundColor2.G, EditorScene.EditorMetadata.BackgroundColor2.B);
            DialogResult result = colorSelect.ShowDialog();
            if (result == DialogResult.OK)
            {
                {
                    RSDKv5.Color returnColor = new RSDKv5.Color();
                    returnColor.R = colorSelect.Color.R;
                    returnColor.A = colorSelect.Color.A;
                    returnColor.B = colorSelect.Color.B;
                    returnColor.G = colorSelect.Color.G;
                    EditorScene.EditorMetadata.BackgroundColor2 = returnColor;
                }

            }
        }

        #endregion

        #region Apps Tab Buttons
        private void rSDKAnnimationEditorToolStripMenuItem_Click(object sender, EventArgs e)
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
                    var ofd = new OpenFileDialog();
                    ofd.Title = "Select RSDK Animation Editor.exe";
                    ofd.Filter = "Windows PE Executable|*.exe";
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

        private void cToolStripMenuItem_Click(object sender, EventArgs e)
        {
            String collisionProcessName = Path.GetFileNameWithoutExtension(mySettings.RunTileManiacPath);
            IntPtr hWnd = FindWindow(collisionProcessName, null); // this gives you the handle of the window you need.
            Process processes = Process.GetProcessesByName(collisionProcessName).FirstOrDefault();
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
                // Ask where Tile Maniac is located when not set
                if (string.IsNullOrEmpty(mySettings.RunTileManiacPath))
                {
                    var ofd = new OpenFileDialog();
                    ofd.Title = "Select TileManiac.exe";
                    ofd.Filter = "Windows PE Executable|*.exe";
                    if (ofd.ShowDialog() == DialogResult.OK)
                        mySettings.RunTileManiacPath = ofd.FileName;
                }
                else
                {
                    if (!File.Exists(mySettings.RunTileManiacPath))
                    {
                        mySettings.RunTileManiacPath = "";
                        return;
                    }
                }

                ProcessStartInfo psi;
                psi = new ProcessStartInfo(mySettings.RunTileManiacPath);
                Process.Start(psi);
            }

        }

        private void colorPaletteEditorToolStripMenuItem_Click(object sender, EventArgs e)
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
                    var ofd = new OpenFileDialog();
                    ofd.Title = "Select Color Palette Program (.exe)";
                    ofd.Filter = "Windows PE Executable|*.exe";
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

        private void duplicateObjectIDHealerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("WARNING: Once you do this the editor will restart immediately, make sure your progress is closed and saved!", "WARNING", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            if (result == DialogResult.OK)
            {
                RepairScene();
            }
        }

        #endregion

        #region Folders Tab Buttons
        private void openSceneFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string SceneFilename_mod = SceneFilename.Replace('/', '\\');
            Process.Start("explorer.exe", "/select, " + SceneFilename_mod);
            //MessageBox.Show(SceneFilename_mod);
        }

        private void openDataDirectoryFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string DataDirectory_mod = DataDirectory.Replace('/', '\\');
            Process.Start("explorer.exe", "/select, " + DataDirectory_mod);
            //MessageBox.Show(DataDirectory_mod);
        }

        private void openSonicManiaFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {

            string GameFolder = mySettings.RunGamePath;
            string GameFolder_mod = GameFolder.Replace('/', '\\');
            Process.Start("explorer.exe", "/select, " + GameFolder_mod);
            //MessageBox.Show(GameFolder_mod);
        }
        #endregion

        #region Other Tab Buttons
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var aboutBox = new AboutBox())
            {
                aboutBox.ShowDialog();
            }
        }

        private void optionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var optionBox = new OptionBox())
            {
                optionBox.ShowDialog();
            }
        }

        private void controlsToolStripMenuItem_Click(object sender, EventArgs e)
        {

            using (var ControlBox = new ControlBox())
            {
                ControlBox.ShowDialog();
            }
        }
        #endregion

        #region Main Toolstrip Buttons
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //New_Click(sender, e);
        }

        private void sToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void undoButton_Click(object sender, EventArgs e)
        {
            EditorUndo();
        }

        private void redoButton_Click(object sender, EventArgs e)
        {
            EditorRedo();
        }

        private void zoomInButton_Click(object sender, EventArgs e)
        {
            ZoomLevel += 1;
            if (ZoomLevel >= 5) ZoomLevel = 5;
            if (ZoomLevel <= -5) ZoomLevel = -5;

            SetZoomLevel(ZoomLevel, new Point(0, 0));
        }

        private void zoomOutButton_Click(object sender, EventArgs e)
        {
            ZoomLevel -= 1;
            if (ZoomLevel >= 5) ZoomLevel = 5;
            if (ZoomLevel <= -5) ZoomLevel = -5;

            SetZoomLevel(ZoomLevel, new Point(0, 0));
        }

        private void selectTool_Click(object sender, EventArgs e)
        {
            selectTool.Checked = !selectTool.Checked;
            pointerButton.Checked = false;
            placeTilesButton.Checked = false;
            UpdateControls();
        }

        private void pointerButton_Click(object sender, EventArgs e)
        {
            pointerButton.Checked = !pointerButton.Checked;
            selectTool.Checked = false;
            placeTilesButton.Checked = false;
            UpdateControls();
        }

        private void placeTilesButton_Click(object sender, EventArgs e)
        {
            placeTilesButton.Checked = !placeTilesButton.Checked;
            selectTool.Checked = false;
            pointerButton.Checked = false;
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

                CollisionLayerA.Clear();
                CollisionLayerB.Clear();

                for (int i = 0; i < 1024; i++)
                {
                    CollisionLayerA.Add(StageTiles.Config.CollisionPath1[i].DrawCMask(Color.FromArgb(0, 0, 0, 0), CollisionAllSolid));
                    CollisionLayerB.Add(StageTiles.Config.CollisionPath2[i].DrawCMask(Color.FromArgb(0, 0, 0, 0), CollisionAllSolid));
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
            }
            else
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

        private void showTileIDButton_Click(object sender, EventArgs e)
        {
            if (showTileIDButton.Checked == false)
            {
                showTileIDButton.Checked = true;
                ReloadSpecificTextures(sender, e);
                showTileID = true;
            }
            else
            {
                showTileIDButton.Checked = false;
                ReloadSpecificTextures(sender, e);
                showTileID = false;
            }
        }

        private void showGridButton_Click(object sender, EventArgs e)
        {
            if (showGridButton.Checked == false)
            {
                showGridButton.Checked = true;
                showGrid = true;
                gridCheckStateCheck();

            }
            else
            {
                showGridButton.Checked = false;
                showGrid = false;
            }
        }

        private void ShowCollisionAButton_Click(object sender, EventArgs e)
        {
            if (showCollisionAButton.Checked == false)
            {
                showCollisionAButton.Checked = true;
                showCollisionA = true;
                showCollisionBButton.Checked = false;
                showCollisionB = false;
                ReloadSpecificTextures(sender, e);
            }
            else
            {
                showCollisionAButton.Checked = false;
                showCollisionA = false;
                showCollisionBButton.Checked = false;
                showCollisionB = false;
                ReloadSpecificTextures(sender, e);
            }
        }

        private void showCollisionBButton_Click(object sender, EventArgs e)
        {
            if (showCollisionBButton.Checked == false)
            {
                showCollisionBButton.Checked = true;
                showCollisionB = true;
                showCollisionAButton.Checked = false;
                showCollisionA = false;
                ReloadSpecificTextures(sender, e);
            }
            else
            {
                showCollisionBButton.Checked = false;
                showCollisionB = false;
                showCollisionAButton.Checked = false;
                showCollisionA = false;
                ReloadSpecificTextures(sender, e);
            }
        }

        private void resetDeviceButton_Click(object sender, EventArgs e)
        {
            GraphicPanel.AttemptRecovery(null);
        }

        private void openDataDirectoryMenuButton(object sender, EventArgs e)
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

        private void enableEncorePalette_Click(object sender, EventArgs e)
        {
            DisposeTextures();
            if (useEncoreColors == true)
            {
                enableEncorePalette.Checked = false;
                useEncoreColors = false;
                StageTiles?.Image.Reload();
                TilesToolbar?.Reload();
            }
            else
            {
                enableEncorePalette.Checked = true;
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

        private void GraphicPanel_OnRender(object sender, DeviceEventArgs e)
        {
            // hmm, if I call refresh when I update the values, for some reason it will stop to render until I stop calling refrsh
            // So I will refresh it here

            //onRenderActive = true;
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

                if (mySettings.PrioritizedObjectRendering && !EditEntities.Checked)
                {
                    entities.DrawPriority(GraphicPanel, 0);
                    entities.DrawPriority(GraphicPanel, 1);
                }

                if (!mySettings.PrioritizedObjectRendering && !EditEntities.Checked) entities.Draw(GraphicPanel);

                if (ShowFGHigh.Checked || EditFGHigh.Checked)
                    FGHigh.Draw(GraphicPanel);

                if (mySettings.PrioritizedObjectRendering && !EditEntities.Checked)
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

                if (EditEntities.Checked) entities.Draw(GraphicPanel);


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
            if (showGrid)
                Background.DrawGrid(GraphicPanel);



            //if (EditorScene != null)
            //{
            //    Background.DrawSnow(GraphicPanel);
            //}


            //onRenderActive = false;
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

        private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
        {
            Form1_Resize(null, null);
        }

        private void GraphicPanel_MouseClick(object sender, MouseEventArgs e)
        {
            GraphicPanel.Focus();
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
        private void RunSequence(object sender, EventArgs e)
        {
            // Ask where Sonic Mania is located when not set
            string path = "steam://run/584400";
            bool ready = false;
            if (mySettings.UsePrePlusOffsets)
            {
                if (string.IsNullOrEmpty(mySettings.RunGamePath))
                {
                    var ofd = new OpenFileDialog();
                    ofd.Title = "Select SonicMania.exe";
                    ofd.Filter = "Windows PE Executable|*.exe";
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
            if (path != "")
            {
                string maniaDir = Path.GetDirectoryName(path);
                // Check if the mod loader is installed
                if (File.Exists(Path.Combine(maniaDir, "d3d9.dll")))
                    psi.WorkingDirectory = maniaDir;
                else
                    psi.WorkingDirectory = Path.GetDirectoryName(DataDirectory);
                var p = Process.Start(psi);
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

        private void pixelModeButton_Click(object sender, EventArgs e)
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

        public void scrollLockButton_Click(object sender, EventArgs e)
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

        public void nudgeFasterButton_Click(object sender, EventArgs e)
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

        private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            ShiftY = e.NewValue;
            UpdateRender();
        }

        private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            ShiftX = e.NewValue;
            UpdateRender();
        }

        private void vScrollBar1_ValueChanged(object sender, EventArgs e)
        {
            if (forceResize)
            {
                forceResize = false;
                SetViewSize(SceneWidth, SceneHeight);
                GoToPosition(forceResizeGoToX, forceResizeGoToY);
                //SetZoomLevel(mySettings.DevForceRestartZoomLevel, new Point(forceResizeGoToX, forceResizeGoToY));
            }
            ShiftY = (sender as VScrollBar).Value;
            if (!(zooming || draggingSelection || dragged || scrolling)) UpdateRender();
            if (draggingSelection)
            {
                OnMouseMoveEvent();
            }
        }

        private void hScrollBar1_ValueChanged(object sender, EventArgs e)
        {
            if (forceResize)
            {
                forceResize = false;
                SetViewSize(SceneWidth, SceneHeight);
                GoToPosition(forceResizeGoToX, forceResizeGoToY);
                SetZoomLevel(mySettings.DevForceRestartZoomLevel, new Point(forceResizeGoToX, forceResizeGoToY));
            }
            ShiftX = hScrollBar1.Value;
            if (!(zooming || draggingSelection || dragged || scrolling)) UpdateRender();
            if (draggingSelection)
            {
                OnMouseMoveEvent();
            }
        }

        private void vScrollBar1_Entered(object sender, EventArgs e)
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

        private void hScrollBar1_Entered(object sender, EventArgs e)
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

        public void backupScene()
        {
            backupType = 1;
            backupToolStripMenuItem_Click(null, null);
            backupType = 0;
        }
        public void backupSceneBeforeCrash()
        {
            backupType = 2;
            backupToolStripMenuItem_Click(null, null);
            backupType = 0;
        }
        public void autoBackupScene()
        {
            backupType = 3;
            backupToolStripMenuItem_Click(null, null);
            backupType = 0;
        }
        public void backupTool(object sender, EventArgs e)
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
                    int i = 1;
                    while ((File.Exists(SceneFilenameBak)))
                    {
                        SceneFilenameBak = SceneFilename.Substring(0, SceneFilename.Length - 4) + "." + i + ".bin.bak";
                        i++;
                    }
                    EditorScene.Save(SceneFilenameBak);
                }
                if (backupType == 2)
                {
                    String SceneFilenameBak = SceneFilename + ".crash.bak";
                    EditorScene.Save(SceneFilenameBak);
                }
                if (backupType >= 3 && backupType != 4)
                {
                    String SceneFilenameBak = SceneFilename + ".idk.bak";
                    int i = 1;
                    while ((File.Exists(SceneFilenameBak)))
                    {
                        SceneFilenameBak = SceneFilename.Substring(0, SceneFilename.Length - 4) + "." + i + ".bin.bak";
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
                    String StageConfigFileName = SceneFilepath + "StageConfig.bin" + ".bak";
                    Debug.Print(StageConfigFileName);
                    int i = 1;
                    while ((File.Exists(StageConfigFileName)))
                    {
                        StageConfigFileName = "StageConfig" + "." + i + ".bin.bak";
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
        private void openDataDirectoryButton_DropDownOpened(object sender, EventArgs e)
        {
            toolStripSplitButton1.AutoToolTip = false;
        }

        private void openDataDirectoryButton_DropDownClosed(object sender, EventArgs e)
        {
            toolStripSplitButton1.AutoToolTip = true;
        }

        private void openModManagerToolStripMenuItem_Click(object sender, EventArgs e)
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
                    var ofd = new OpenFileDialog();
                    ofd.Title = "Select Mania Mod Manager.exe";
                    ofd.Filter = "Windows PE Executable|*.exe";
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
        private void gridCheckStateCheck()
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
        private void x16ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Background.GRID_TILE_SIZE = 16;
            resetGridOptions();
            x16ToolStripMenuItem.Checked = true;
        }

        private void x128ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Background.GRID_TILE_SIZE = 128;
            resetGridOptions();
            x128ToolStripMenuItem.Checked = true;
        }

        private void resetGridOptions()
        {
            x16ToolStripMenuItem.Checked = false;
            x128ToolStripMenuItem.Checked = false;
            x256ToolStripMenuItem.Checked = false;
            customToolStripMenuItem.Checked = false;
        }

        private void x256ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Background.GRID_TILE_SIZE = 256;
            resetGridOptions();
            x128ToolStripMenuItem.Checked = true;
        }

        private void customToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Background.GRID_TILE_SIZE = mySettings.CustomGridSizeValue;
            resetGridOptions();
            customToolStripMenuItem.Checked = true;
        }
        #endregion

        #region Lower Right Status Bar Quick Options Button
        public void moreSettingsButton_ButtonClick(object sender, EventArgs e)
        {
            switch (myEditorState.lastQuickButtonState)
            {
                case 1:
                    swapScrollLockDirectionToolStripMenuItem_Click(sender, e);
                    break;
                case 2:
                    editEntitesTransparencyToolStripMenuItem_Click(sender, e);
                    break;
                case 3:
                    toggleEncoreManiaEntitiesToolStripMenuItem_Click(sender, e);
                    break;
                default:
                    swapScrollLockDirectionToolStripMenuItem_Click(sender, e);
                    break;


            }
        }

        public void swapScrollLockDirectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            myEditorState.lastQuickButtonState = 1;
            xToolStripMenuItem_Click(sender, e);
        }

        public void editEntitesTransparencyToolStripMenuItem_Click(object sender, EventArgs e)
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

        public void toggleEncoreManiaEntitiesToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void x8ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            magnetSize = 8;
            ResetMagnetModeOptions();
            x8ToolStripMenuItem.Checked = true;
        }

        private void x16ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            magnetSize = 16;
            ResetMagnetModeOptions();
            x16ToolStripMenuItem1.Checked = true;
        }

        private void x32ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            magnetSize = 32;
            ResetMagnetModeOptions();
            x32ToolStripMenuItem.Checked = true;
        }

        private void x64ToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void enableXAxisToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void enableYAxisToolStripMenuItem_Click(object sender, EventArgs e)
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

        public void goToToolStripMenuItem_Click(object sender, EventArgs e)
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

        public void preLoadSceneButton_Click(object sender, EventArgs e)
        {
            preLoadSceneButton.Enabled = false;
            isPreRending = true;
            PreLoadBox preLoadForm = new PreLoadBox();
            preLoadForm.TopLevel = false;
            GraphicPanel.Controls.Add(preLoadForm);
            preLoadForm.Show();
            toggleEditorButtons(false);

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
                    //preLoadForm.SetProgressBarStatus(progressValueX, progressValueY);
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
                //preLoadForm.SetProgressBarStatus(progressValueX, progressValueY);
            }
            hScrollBar1.Value = 0;
            vScrollBar1.Value = 0;

            // get the form reference back and close it
            isPreRending = false;
            preLoadForm.Close();
            toggleEditorButtons(true);

            // Play a sound to tell the user we are finished
            System.IO.Stream str = Properties.Resources.ScoreTotal;
            System.Media.SoundPlayer snd = new System.Media.SoundPlayer(str);
            snd.Play();
            preLoadSceneButton.Enabled = true;
        }

        private void developerTerminalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeveloperTerminal devTerm = new DeveloperTerminal();
            devTerm.Show();

        }

        private void mD5GeneratorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MD5HashGen hashmap = new MD5HashGen();
            hashmap.Show();
        }

        private void playerSpawnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            selectPlayerObject_GoTo = -1;
            if (playerObjectPosition.Count == 0) return;

            if (playerObjectPosition.Count == 1)
            {
                //Set Zoom Level to Position so we can Move to that location
                SetZoomLevel(0, new Point(0, 0));
                int xPos = (int)(playerObjectPosition[0].Position.X.High / Zoom);
                int yPos = (int)(playerObjectPosition[0].Position.Y.High / Zoom);
                hScrollBar1.Value = xPos;
                vScrollBar1.Value = yPos;
            }
            else
            {
                GoToPlayerBox goToPlayerBox = new GoToPlayerBox();
                DialogResult result = goToPlayerBox.ShowDialog();
                if (selectPlayerObject_GoTo != -1)
                {
                    int objectIndex = selectPlayerObject_GoTo;
                    SetZoomLevel(0, new Point(0, 0));
                    int xPos = (int)((int)playerObjectPosition[objectIndex].Position.X.High / Zoom);
                    int yPos = (int)((int)playerObjectPosition[objectIndex].Position.Y.High / Zoom);
                    hScrollBar1.Value = xPos;
                    vScrollBar1.Value = yPos;
                }
            }
        }

        private void findToolStripMenuItem1_Click(object sender, EventArgs e)
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

        private void findUnusedTiles(object sender, EventArgs e)
        {
            toggleEditorButtons(false);
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
            toggleEditorButtons(true);

        }

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void consoleWindowToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void saveForForceOpenOnStartupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mySettings.DevForceRestartData = DataDirectory;
            mySettings.DevForceRestartScene = ScenePath;
            mySettings.DevForceRestartX = (short)(ShiftX / Zoom);
            mySettings.DevForeRestartY = (short)(ShiftY / Zoom);
            mySettings.DevForceRestartZoomLevel = ZoomLevel;
            mySettings.DevForceRestartEncore = Editor.Instance.useEncoreColors;
            mySettings.DeveForceRestartLevelID = Editor.Instance.myEditorState.Level_ID;
        }

        private void wikiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://docs.google.com/document/d/1NBvcqzvOzqeTVzgAYBR0ttAc5vLoFaQ4yh_cdf-7ceQ/edit?usp=sharing");
        }

        //TO-MOVE
        private void editEntitiesOptionToolStrip_DropDownOpening(object sender, EventArgs e)
        {

        }





        #endregion

        #region Collision Toolstrip Menu Item Entries

        private void defaultToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void invertedToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void customToolStripMenuItem1_Click(object sender, EventArgs e)
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

        private void movingPlatformsObjectsToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void spriteFramesToolStripMenuItem_Click(object sender, EventArgs e)
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
            return new Rectangle(ShiftX, ShiftY, viewPanel.Width, viewPanel.Height);
        }

        public double GetZoom()
        {
            return Zoom;
        }

        #endregion

        #region Miscellaneous

        private void ShowError(string message, string title = "Error!")
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
            /*using (var customMsgBox = new CustomMsgBox(message, title, 1, 1))
            {
                customMsgBox.ShowDialog();
            }*/
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
            Debug.Print(selectedScene);
            if (!File.Exists(selectedScene))
            {
                string[] splitted = selectedScene.Split('\\');

                string part1 = splitted[0];
                string part2 = splitted[1];

                selectedScene = Path.Combine(DataDirectory, "Stages", part1, part2);
                Debug.Print(selectedScene);
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

        public void GoToPosition(int x, int y)
        {
            //Set Zoom Level to Position so we can Move to that location
            SetZoomLevel(0, new Point(0, 0));
            int xPos = (int)(x / Zoom);
            int yPos = (int)(y / Zoom);
            hScrollBar1.Value = xPos;
            vScrollBar1.Value = yPos;
        }

        public void UpdateRender()
        {
            if (!mySettings.RellyOnRenderLoopForUpdatingOnly)
            {
                GraphicPanel.Render();
            }
        }

        public void OnMouseMoveEvent()
        {
            if (!mySettings.RellyOnRenderLoopForUpdatingOnly)
            {
                GraphicPanel.OnMouseMoveEventCreate();
            }
        }

        public void useDarkTheme(bool state = false)
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

            private int[] _colorTable;
        }



        #endregion
    }
}
