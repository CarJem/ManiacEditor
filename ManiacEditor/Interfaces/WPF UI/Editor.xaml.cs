using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using ManiacEditor.Actions;
using RSDKv5;
using SharpDX.Direct3D9;
using Color = System.Drawing.Color;
using System.Reflection;
using ManiacEditor.Interfaces;
using Cyotek.Windows.Forms;
using Microsoft.Scripting.Utils;
using Microsoft.Win32;
using ManiacEditor.Entity_Renders;
using IWshRuntimeLibrary;
using System.Drawing;
using File = System.IO.File;
using SaveFileDialog = System.Windows.Forms.SaveFileDialog;
using OpenFileDialog = System.Windows.Forms.OpenFileDialog;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Xceed.Wpf.Toolkit;
using Point = System.Drawing.Point;
using DragEventArgs = System.Windows.DragEventArgs;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using Path = System.IO.Path;
using System.Windows.Controls.Primitives;
using Rectangle = System.Drawing.Rectangle;
using SystemColors = System.Drawing.SystemColors;
using MenuItem = System.Windows.Controls.MenuItem;
using Control = System.Windows.Forms.Control;
using Clipboard = System.Windows.Clipboard;
using DataObject = System.Windows.DataObject;
using Button = System.Windows.Controls.Button;
using Cursors = System.Windows.Input.Cursors;
using RSDKrU;
using MessageBox = RSDKrU.MessageBox;


namespace ManiacEditor
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class Editor : Window
	{
		#region Definitions
		//Editor Editing States
		public bool dragged;
		public bool startDragged;
		public int lastX, lastY, draggedX, draggedY;
		public int ShiftX = 0, ShiftY = 0, ScreenWidth, ScreenHeight;
		public int CustomX = 0, CustomY = 0;
		public int select_x1, select_x2, select_y1, select_y2;
		public int ClickedX = -1, ClickedY = -1;
		public bool draggingSelection; //Determines if we are dragging a selection
		public int selectingX, selectingY;
		public bool zooming; //Detects if we are zooming
		public double Zoom = 1; //Double Value for Zoom Levels
		public int ZoomLevel = 0; //Interger Value for Zoom Levels
		public int SelectedTilesCount; //Used to get the Amount of Selected Tiles in a Selection
		public int DeselectTilesCount; //Used in combination with SelectedTilesCount to get the definitive amount of Selected Tiles
		public int SelectedTileX = 0; //Used to get a single Selected Tile's X
		public int SelectedTileY = 0; //Used to get a single Selected Tile's Y
		public bool scrolling = false; //Determines if the User is Scrolling
		public bool scrollingDragged = false, wheelClicked = false; //Dermines if the mouse wheel was clicked or is the user is drag-scrolling.
		public Point scrollPosition; //For Getting the Scroll Position

		//Editor Toggles
		public bool showTileID; // Show Tile ID Mode Status
		public bool showGrid; // Show Grid Mode Status
		public bool useEncoreColors = false; // Show Encore Color Status
		public bool showCollisionA; //Show Collision Path A Status
		public bool showCollisionB; //Show Collision Path B Status
		public int backupType = 0; //Determines What Kind of Backup to Make
		public bool UseMagnetMode = false; // Determines the state of Magnet Mode
		public bool useMagnetXAxis = true; //Determines if the Magnet should use the X Axis
		public bool useMagnetYAxis = true; //Determines if the Magnet should use the Y Axis
		public bool showEntityPathArrows = true; //Determines if we want to see Object Arrow Paths
		public bool showWaterLevel = false; //Determines if the Water Object should show it's Water Level.
		public bool alwaysShowWaterLevel = false; //Determines if the Water Level Should be Shown at all times regardless of the object being selected
		public bool sizeWaterLevelwithBounds = false; //Determines if the water level width should match those of the object's bounds
		public bool extraLayersMoveToFront = false; //Determines if we should render the extra layers in front of everything on behind everything
		public bool showFlippedTileHelper = false; //Determines if we should Show Flip Assist or Not
		public bool showingDataDirectory = false; //Determines who's turn it is when swaping the label's entry to display ethier the Data Directory and Mod Folder.
		public bool showParallaxSprites = false; //Determines if we should show the parallax sprites
		public bool applyEditEntitiesTransparency = false; //Determines if the other layers should be semi-transparent when editing entities.
		public bool showEntitySelectionBoxes = false; //Determines if we should show the entity selection boxes.
		public bool EnablePixelCountMode = false; //Count the selection in pixels per tile or not
		public bool isConsoleWindowOpen = false; //Show the Console Window or not
		public bool rightClicktoSwapSlotID = false; //Swap Entity Slot ID's with Right Click


		//Scroll Lock States
		public int ScrollDirection = 1;
		public bool ScrollLocked = true;



		//Editor Status States (Like are we pre-loading a scene)
		public bool importingObjects = false; //Determines if we are importing objects so we can disable all the other Scene Select Options
		public bool isPreRending = false; //Determines if we are Preloading a Scene
		public bool encorePaletteExists = false; // Determines if an Encore Pallete Exists
		public int SelectedTileID = -1; //For Tile Maniac Intergration via Right Click in Editor View Panel
		public string CurrentLanguage = "EN"; //Current Selected Language
		public Point TempWarpCoords = new Point(0, 0); //Temporary Warp Position for Shortcuts and Force Open
		public bool ForceWarp = false; //For Shortcuts and Force Open.
		public bool ShortcutHasZoom = false; //For Shortcuts and Force Open.
		public int PlayerBeingTracked = -1;
		public int CurrentControllerButtons = 2; //For Setting the Menu Control Button Images.
		public bool isExportingImage = false; //For Setting the right options when exporting entitites.
		public int LevelID = -1; //Self Explanatory
		public int LastQuickButtonState = 0; //Gets the Last Quick Button State, so we can tell what action was used last
		public bool MovingPlatformsChecked = true; //Self Explanatory
		public bool AnnimationsChecked = true; //Self Explanatory
		public bool PreRenderSceneSelectCheckbox = false; //Self Explanatory
		public bool RemoveStageConfigEntriesAllowed = false; //Self Explanatory
		public bool AddStageConfigEntriesAllowed = false; //Self Explanatory
		public int InstanceID = 0; //Mega Maniac Instance ID
		public bool CloseMegaManiacTab = false; //Tells Mega Maniac to Remove the Tab
		public bool KickStartMegaManiacRenderLoop = false; //Used to start the render loop when starting the editor for Mega Maniac
		public bool KickStartMegaManiacRenderLoopFinished = false; //Used to end the process of starting the render loop when starting the editor for Mega Maniac
		public bool DebugStatsVisibleOnPanel = false;
		public bool UseLargeDebugStats = false;
		public bool collisionOpacityChanged = false;
		public static bool UpdateUpdaterMessage = false;
		public bool CopyAir = false;
		public bool showMouseTooltip = false;
		public bool MultiLayerEditMode = false;
		public string MultiLayerA = "";
		public string MultiLayerB = "";


		//Editor Variable States (Like Scroll Lock is in the X Direction)
		public int magnetSize = 16; //Determines the Magnets Size
		public int EncoreSetupType; //Used to determine what kind of encore setup the stage uses
		public string ToolbarSelectedTile; //Used to display the selected tile in the tiles toolbar
		internal bool controlWindowOpen; //Used somewhere in the Layer Manager (Unkown)
		public int selectPlayerObject_GoTo = 0; //Used to determine which player object to go to
		public bool cooldownDone = false; // For waiting on functions
		public Color waterColor = new Color(); // The color used for the Water Entity
		public string INILayerNameLower = ""; //Reserved String for INI Default Layer Prefrences
		public string INILayerNameHigher = ""; //Reserved String for INI Default Layer Prefrences
		public string entitiesTextFilter = ""; //Used to hide objects that don't match the discription
		public int entityVisibilityType = 0; // Used to determine how to display entities
		string LevelSelectCharS = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ*+,-./: \'\"";
		string MenuCharS = "ABCDEFGHIJKLMNOPQRSTUVWXYZ ?!.";
		string MenuCharS_Small = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789 ?.:'\"!-,&¡<>¿"; //49 out of 121
		public char[] MenuChar;
		public char[] LevelSelectChar;
		public char[] MenuChar_Small;
		public double ShortcutZoomValue = 0.0;

		//Editor Paths
		public string DataDirectory; //Used to get the current Data Directory
		public string MasterDataDirectory = Environment.CurrentDirectory + "\\Data"; //Used as a way of allowing mods to not have to lug all the files in their folder just to load in Maniac.
		public IList<string> ResourcePackList = new List<string>();
		//public string SelectedZone; //Used to get the Selected zone
		//public string SelectedScene; //Used to get the Scene zone
		public string[] EncorePalette = new string[6]; //Used to store the location of the encore palletes
													   //public string SceneFilename = null; //Used for fetching the scene's file name
													   //public string SceneFilepath = null; //Used for fetching the folder that contains the scene file
													   //public string StageConfigFileName = null; //Used for fetch the scene's stage config file name

		// Extra Layer Buttons
		public IDictionary<EditLayerToggleButton, EditLayerToggleButton> ExtraLayerEditViewButtons;
		private IList<Separator> _extraLayerSeperators; //Used for Adding Extra Seperators along side Extra Edit/View Layer Buttons

		// Editor Collections
		public List<string> ObjectList = new List<string>(); //All Gameconfig + Stageconfig Object names (Unused)
		public List<Bitmap> CollisionLayerA = new List<Bitmap>(); //Collection of Collision Type A for the Loaded Scene
		public List<Bitmap> CollisionLayerB = new List<Bitmap>(); //Collection of Collision Type B for the Loaded Scene
		public Stack<IAction> undo = new Stack<IAction>(); //Undo Actions Stack
		public Stack<IAction> redo = new Stack<IAction>(); //Redo Actions Stack
		public List<string> entityRenderingObjects = EditorEntity_ini.GetSpecialRenderList(1); //Used to get the Render List for Objects
		public List<string> renderOnScreenExlusions = EditorEntity_ini.GetSpecialRenderList(0); //Used to get the Always Render List for Objects
		public IList<MenuItem> _recentDataItems; //Used to get items for the Data Directory Toolstrip Area
		private IList<MenuItem> _recentDataItems_Button; //Used to get items for the Data Directory Button Toolstrip
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
		internal int SceneWidth => (EditorScene != null ? EditorScene.Layers.Max(sl => sl.Width) * 16 : 0);
		internal int SceneHeight => (EditorScene != null ? EditorScene.Layers.Max(sl => sl.Height) * 16 : 0);

		//Used for "Run Scene"
		public ProcessMemory GameMemory = new ProcessMemory(); //Allows us to write hex codes like cheats, etc.
		public bool GameRunning = false; //Tells us if the game is running
		public string GamePath = ""; //Tells us where the game is located
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
		public Tuple<Dictionary<Point, ushort>, Dictionary<Point, ushort>> TilesClipboard;
		public Dictionary<Point, ushort> FindReplaceClipboard;
		public Dictionary<Point, ushort> TilesClipboardEditable;
		public List<EditorEntity> entitiesClipboard;

		//Collision Colours
		public Color CollisionAllSolid = Color.FromArgb(255, 255, 255, 255);
		public Color CollisionTopOnlySolid = Color.FromArgb(255, 255, 255, 255);
		public Color CollisionLRDSolid = Color.FromArgb(255, 255, 255, 0);
		public int collisionPreset = 0; //For Collision Presets

		//Internal/Public/Vital Classes
		public StageTiles StageTiles;
		public EditorScene EditorScene;
		public StageConfig StageConfig;
		public GameConfig GameConfig;
		public EditorControls EditorControls;
		public EditorEntities entities;
		//public int InstanceID = 0;
		//public static Editor Instance; //Used the access this class easier
		//public Editor ThisInstance;
		internal EditorBackground EditorBackground;
		public EditorLayer EditLayer { get => EditLayerA; set => EditLayerA = value; }
		public EditorLayer EditLayerA { get; set; }
		public EditorLayer EditLayerB { get; set; }
		public TilesToolbar TilesToolbar = null;
		public EntitiesToolbar2 entitiesToolbar = null;
		public EditorEntity_ini EditorEntity_ini;
		public EditorUpdater Updater;
		public TileConfig TilesConfig;
		public EditorInGame EditorGame;
		public StartScreen StartScreen;
		public StatusBox statusBox;
		public UIText DebugTextHUD = new UIText();
		public EditorChunk EditorChunk;
		public System.Windows.Forms.Integration.WindowsFormsHost host;
		public EditorView editorView;
		public EditorDiscordRP Discord;
		public EditorInteractions Interactions;
		public EditorPath EditorPath;
		public EditorSceneLoading EditorSceneLoading;
		public EditorDirectories EditorDirectories;
		public EditorFindReplace FindAndReplace;

		//Tile Maniac + ManiaPal Instance
		public TileManiacWPF.MainWindow mainform = new TileManiacWPF.MainWindow();


		//Editor Misc. Variables
		System.Windows.Forms.Timer StatusPanelTickTimer;

		//Dark Theme
		public static Color darkTheme0 = Color.FromArgb(255, 40, 40, 40);
		public static Color darkTheme1 = Color.FromArgb(255, 50, 50, 50);
		public static Color darkTheme2 = Color.FromArgb(255, 70, 70, 70);
		public static Color darkTheme3 = Color.White;
		public static Color darkTheme4 = Color.FromArgb(255, 49, 162, 247);
		public static Color darkTheme5 = Color.FromArgb(255, 80, 80, 80);

		//Shorthanding Setting Files
		public Properties.Settings mySettings = Properties.Settings.Default;
		public Properties.KeyBinds myKeyBinds = Properties.KeyBinds.Default;


		//GL Variables
		public const double LAYER_DEPTH = 0.1;

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

		#region Editor Initalizing Methods
		public Editor(string dataDir = "", string scenePath = "", string modPath = "", int levelID = 0, bool ShortcutLaunch = false, int shortcutLaunchMode = 0, bool isEncoreMode = false, int X = 0, int Y = 0, double _ZoomedLevel = 0.0, int MegaManiacInstanceID = -1)
		{
			SystemEvents.PowerModeChanged += CheckDeviceState;
			InstanceID = MegaManiacInstanceID;

			UseDarkTheme_WPF(mySettings.NightMode);
			InitializeComponent();
			InitilizeEditor();

			try
			{
				Discord.InitDiscord();
			}
			catch (Exception ex)
			{
				Debug.Print("Discord RP couldn't start! Exception Error:" + ex.ToString());
			}

			if (mySettings.UseAutoForcefulStartup && mySettings.UseForcefulStartup) OpenSceneForceFully();

			if (ShortcutLaunch)
			{
				try
				{
					if (dataDir != "" && scenePath != "") OpenSceneForceFully(dataDir, scenePath, modPath, levelID, isEncoreMode, X, Y);

					else if (dataDir != "") OpenSceneForceFully(dataDir);
				}
				catch
				{
					Debug.Print("Couldn't Force Open Maniac Editor with the Specificied Arguments!");
				}
			}
		}

		public void InitilizeEditor()
		{
			editorView = new EditorView(this);

			this.editorView.splitContainer1.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.SplitContainer1_SplitterMoved);
			this.editorView.vScrollBar1.Scroll += new System.Windows.Controls.Primitives.ScrollEventHandler(this.VScrollBar1_Scroll);
			this.editorView.vScrollBar1.ValueChanged += new RoutedPropertyChangedEventHandler<double>(this.VScrollBar1_ValueChanged);
			this.editorView.vScrollBar1.MouseEnter += new System.Windows.Input.MouseEventHandler(this.VScrollBar1_Entered);
			this.editorView.hScrollBar1.Scroll += new System.Windows.Controls.Primitives.ScrollEventHandler(this.HScrollBar1_Scroll);
			this.editorView.hScrollBar1.ValueChanged += new RoutedPropertyChangedEventHandler<double>(this.HScrollBar1_ValueChanged);
			this.editorView.hScrollBar1.MouseEnter += new System.Windows.Input.MouseEventHandler(this.HScrollBar1_Entered);
			this.Activated += new System.EventHandler(this.MapEditor_Activated);
			this.editorView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MapEditor_KeyDown);
			this.editorView.KeyUp += new System.Windows.Forms.KeyEventHandler(this.MapEditor_KeyUp);

			this.editorView.GraphicPanel.OnRender += new ManiacEditor.RenderEventHandler(this.GraphicPanel_OnRender);
			this.editorView.GraphicPanel.OnCreateDevice += new ManiacEditor.CreateDeviceEventHandler(this.OnResetDevice);
			this.editorView.GraphicPanel.DragDrop += new System.Windows.Forms.DragEventHandler(this.GraphicPanel_DragDrop);
			this.editorView.GraphicPanel.DragEnter += new System.Windows.Forms.DragEventHandler(this.GraphicPanel_DragEnter);
			this.editorView.GraphicPanel.DragOver += new System.Windows.Forms.DragEventHandler(this.GraphicPanel_DragOver);
			this.editorView.GraphicPanel.DragLeave += new System.EventHandler(this.GraphicPanel_DragLeave);
			this.editorView.GraphicPanel.KeyDown += new System.Windows.Forms.KeyEventHandler(this.GraphicPanel_OnKeyDown);
			this.editorView.GraphicPanel.KeyUp += new System.Windows.Forms.KeyEventHandler(this.GraphicPanel_OnKeyUp);
			this.editorView.GraphicPanel.MouseClick += new System.Windows.Forms.MouseEventHandler(this.GraphicPanel_MouseClick);
			this.editorView.GraphicPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.GraphicPanel_OnMouseDown);
			this.editorView.GraphicPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.GraphicPanel_OnMouseMove);
			this.editorView.GraphicPanel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.GraphicPanel_OnMouseUp);
			this.editorView.GraphicPanel.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.GraphicPanel_MouseWheel);

			ExtraLayerEditViewButtons = new Dictionary<EditLayerToggleButton, EditLayerToggleButton>();
			_extraLayerSeperators = new List<Separator>();

			_recentDataItems = new List<MenuItem>();
			_recentDataItems_Button = new List<MenuItem>();
			MenuChar = MenuCharS.ToCharArray();
			MenuChar_Small = MenuCharS_Small.ToCharArray();
			LevelSelectChar = LevelSelectCharS.ToCharArray();
			EditorGame = new EditorInGame(this);
			EditorEntity_ini = new EditorEntity_ini(this);
			statusBox = new StatusBox(this);
			EditorControls = new EditorControls(this);
			StartScreen = new StartScreen(this);
			Discord = new EditorDiscordRP(this);
			Updater = new EditorUpdater();
			Interactions = new EditorInteractions(this);
			EditorPath = new EditorPath(this);
			EditorSceneLoading = new EditorSceneLoading(this);
			EditorDirectories = new EditorDirectories(this);
			FindAndReplace = new EditorFindReplace(this);

			this.Title = String.Format("Maniac Editor - Generations Edition {0}", Updater.GetVersion());
			editorView.GraphicPanel.Width = SystemInformation.PrimaryMonitorSize.Width;
			editorView.GraphicPanel.Height = SystemInformation.PrimaryMonitorSize.Height;

			ViewPanelContextMenu.Foreground = (SolidColorBrush)FindResource("NormalText");
			ViewPanelContextMenu.Background = (SolidColorBrush)FindResource("NormalBackground");


			EditFGLower.Click += EditFGLower_Click;
			EditFGLow.Click += EditFGLow_Click;
			EditFGHigh.Click += EditFGHigh_Click;
			EditFGHigher.Click += EditFGHigher_Click;

			EditFGLower.RightClick += EditFGLower_RightClick;
			EditFGLow.RightClick += EditFGLow_RightClick;
			EditFGHigh.RightClick += EditFGHigh_RightClick;
			EditFGHigher.RightClick += EditFGHigher_RightClick;

			AllocConsole();
			HideConsoleWindow();
			RefreshCollisionColours();
			SetViewSize();
			UpdateControls();
			TryLoadSettings();
			UpdateStartScreen(true, true);

			StatusPanelTickTimer = new System.Windows.Forms.Timer
			{
				Interval = 10
			};
			StatusPanelTickTimer.Tick += new EventHandler(UpdateStatusPanel);
			StatusPanelTickTimer.Start();
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

				WindowState = mySettings.IsMaximized ? System.Windows.WindowState.Maximized : WindowState;
				GamePath = mySettings.GamePath;


				RefreshDataDirectories(mySettings.DataDirectories);


				if (mySettings.modConfigs?.Count > 0)
				{
					selectConfigToolStripMenuItem.Items.Clear();
					for (int i = 0; i < mySettings.modConfigs.Count; i++)
					{
						selectConfigToolStripMenuItem.Items.Add(CreateModConfigMenuItem(i));

					}
				}

				ApplyDefaults();





			}
			catch (Exception ex)
			{
				Debug.Write("Failed to load settings: " + ex);
			}
		}
		private void ApplyDefaults()
		{
			// These Prefrences are applied on Editor Load
			editEntitesTransparencyToolStripMenuItem.IsChecked = mySettings.EditEntitiesTransparencyDefault;
			transparentLayersForEditingEntitiesToolStripMenuItem.IsChecked = mySettings.EditEntitiesTransparencyDefault;
			applyEditEntitiesTransparency = mySettings.EditEntitiesTransparencyDefault;

			ScrollLocked = mySettings.ScrollLockEnabledDefault;
			statusNAToolStripMenuItem.IsChecked = mySettings.ScrollLockEnabledDefault;
			scrollLockButton.IsChecked = mySettings.ScrollLockEnabledDefault;

			ScrollDirection = mySettings.ScrollLockXYDefault;

			xToolStripMenuItem.IsChecked = ScrollDirection == (int)ScrollDir.X;
			yToolStripMenuItem.IsChecked = ScrollDirection == (int)ScrollDir.Y;

			pixelModeButton.IsChecked = mySettings.EnablePixelModeDefault;
			pixelModeToolStripMenuItem.IsChecked = mySettings.EnablePixelModeDefault;
			EnablePixelCountMode = mySettings.EnablePixelModeDefault;

			showEntityPathArrowsToolstripItem.IsChecked = mySettings.ShowEntityArrowPathsDefault;
			showEntityPathArrows = mySettings.ShowEntityArrowPathsDefault;

			showWaterLevelToolStripMenuItem.IsChecked = mySettings.showWaterLevelDefault;
			showWaterLevel = mySettings.showWaterLevelDefault;
			alwaysShowWaterLevel = mySettings.AlwaysShowWaterLevelDefault;
			sizeWaterLevelwithBounds = mySettings.SizeWaterLevelWithBoundsDefault;
			waterLevelAlwaysShowItem.IsChecked = mySettings.AlwaysShowWaterLevelDefault;
			sizeWithBoundsWhenNotSelectedToolStripMenuItem.IsChecked = mySettings.SizeWaterLevelWithBoundsDefault;

			showParallaxSpritesToolStripMenuItem.IsChecked = mySettings.ShowFullParallaxEntityRenderDefault;
			showParallaxSprites = mySettings.ShowFullParallaxEntityRenderDefault;
			prioritizedViewingToolStripMenuItem.IsChecked = mySettings.PrioritizedObjectRendering;

			showEntitySelectionBoxes = mySettings.ShowEntitySelectionBoxesDefault;
			showEntitySelectionBoxesToolStripMenuItem.IsChecked = mySettings.ShowEntitySelectionBoxesDefault;

			showStatsToolStripMenuItem.IsChecked = mySettings.ShowStatsViewerDefault;
			useLargeTextToolStripMenuItem.IsChecked = mySettings.StatsViewerLargeTextDefault;

			DebugStatsVisibleOnPanel = mySettings.ShowStatsViewerDefault;
			UseLargeDebugStats = mySettings.StatsViewerLargeTextDefault;



			var allLangItems = menuLanguageToolStripMenuItem.Items.Cast<System.Windows.Controls.MenuItem>().ToArray();
			foreach (var item in allLangItems)
				if (item != null)
				{
					if (item.Tag.ToString() == mySettings.LangDefault)
					{
						item.IsChecked = true;
						CurrentLanguage = item.Tag.ToString();
					}
				}


			bool endSearch = false;
			var allButtonItems = menuButtonsToolStripMenuItem.Items.Cast<System.Windows.Controls.MenuItem>().ToArray();
			foreach (var item in allButtonItems)
			{
				if (item.Tag != null)
				{
					if (item.Tag.ToString() == mySettings.ButtonLayoutDefault && !endSearch)
					{
						item.IsChecked = true;
						SetMenuButtons(item.Tag.ToString());
						endSearch = true;
					}
					var allSubButtonItems = item.Items.Cast<System.Windows.Controls.MenuItem>().ToArray();
					foreach (var subItem in allSubButtonItems)
					{
						if (subItem.Tag != null)
						{
							if (subItem.Tag.ToString() == mySettings.ButtonLayoutDefault && !endSearch)
							{
								subItem.IsChecked = true;
								SetMenuButtons(subItem.Tag.ToString());
								endSearch = true;
							}
						}
					}
				}

			}


		}
		public void UseDefaultPrefrences()
		{
			//These Prefrences are applied on Stage Load

			//Default Layer Visibility Preferences
			if (!mySettings.FGLowerDefault) ShowFGLower.IsChecked = false;
			else ShowFGLower.IsChecked = true;
			if (!mySettings.FGLowDefault) ShowFGLow.IsChecked = false;
			else ShowFGLow.IsChecked = true;
			if (!mySettings.FGHighDefault) ShowFGHigh.IsChecked = false;
			else ShowFGHigh.IsChecked = true;
			if (!mySettings.FGHigherDefault) ShowFGHigher.IsChecked = false;
			else ShowFGHigher.IsChecked = true;
			if (!mySettings.EntitiesDefault) ShowEntities.IsChecked = false;
			else ShowEntities.IsChecked = true;
			if (!mySettings.AnimationsDefault) ShowAnimations.IsChecked = false;
			else ShowAnimations.IsChecked = true;

			//Default Enabled Annimation Preferences
			movingPlatformsObjectsToolStripMenuItem.IsChecked = mySettings.MovingPlatformsDefault;
			MovingPlatformsChecked = mySettings.MovingPlatformsDefault;

			spriteFramesToolStripMenuItem.IsChecked = mySettings.AnimatedSpritesDefault;
			AnnimationsChecked = mySettings.AnimatedSpritesDefault;

			waterColor = mySettings.WaterColorDefault;




			//Default Grid Preferences
			if (!mySettings.x16Default) x16ToolStripMenuItem.IsChecked = false;
			else x16ToolStripMenuItem.IsChecked = true;
			if (!mySettings.x128Default) x128ToolStripMenuItem.IsChecked = false;
			else x128ToolStripMenuItem.IsChecked = true;
			if (!mySettings.x256Default) x256ToolStripMenuItem.IsChecked = false;
			else x256ToolStripMenuItem.IsChecked = true;
			if (!mySettings.CustomGridDefault) customToolStripMenuItem.IsChecked = false;
			else customToolStripMenuItem.IsChecked = true;

			//Collision Color Presets
			defaultToolStripMenuItem.IsChecked = mySettings.CollisionColorsDefault == 0;
			invertedToolStripMenuItem.IsChecked = mySettings.CollisionColorsDefault == 1;
			customToolStripMenuItem1.IsChecked = mySettings.CollisionColorsDefault == 2;
			collisionPreset = mySettings.CollisionColorsDefault;
			RefreshCollisionColours();

			if (mySettings.ScrollLockXYDefault.Equals(ScrollDir.X))
			{
				ScrollDirection = (int)ScrollDir.X;
				UpdateStatusPanel(null, null);

			}
			else
			{
				ScrollDirection = (int)ScrollDir.Y;
				UpdateStatusPanel(null, null);
			}

		}
		#endregion

		#region Boolean States
		public bool IsEditing()
		{
			return IsTilesEdit() || IsEntitiesEdit() || IsChunksEdit();
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
			return EditLayerA != null;
		}
		public bool IsChunksEdit()
		{
			return ChunksToolButton.IsChecked.Value && EditLayerA != null;
		}
		public bool IsEntitiesEdit()
		{
			return EditEntities.IsCheckedN.Value;
		}
		public bool IsSelected()
		{
			if (IsTilesEdit())
			{
				bool SelectedA = EditLayerA?.SelectedTiles.Count > 0 || EditLayerA?.TempSelectionTiles.Count > 0;
				bool SelectedB = EditLayerB?.SelectedTiles.Count > 0 || EditLayerB?.TempSelectionTiles.Count > 0;
				return SelectedA || SelectedB;
			}
			else if (IsEntitiesEdit())
			{
				return entities.IsSelected();
			}
			return false;
		}
		public bool CtrlPressed()
		{
			return System.Windows.Forms.Control.ModifierKeys.HasFlag(System.Windows.Forms.Keys.Control);
		}
		public bool ShiftPressed()
		{
			return System.Windows.Forms.Control.ModifierKeys.HasFlag(System.Windows.Forms.Keys.Shift);
		}

		public bool CanWriteFile(string fullFilePath)
		{
			if (!File.Exists(fullFilePath)) return true;

			if (File.GetAttributes(fullFilePath).HasFlag(FileAttributes.ReadOnly))
			{
				ShowError($"The file '{fullFilePath}' is Read Only.", "File is Read Only.");
				return false;
			}

			var result = RSDKrU.MessageBox.Show($"The file '{fullFilePath}' already exists. Overwrite?", "Overwrite?",
										 MessageBoxButton.YesNo, MessageBoxImage.Warning);

			if (result == MessageBoxResult.Yes) return true;

			return false;
		}
		public bool AllowSceneUnloading()
		{
			bool AllowSceneChange = false;
			if (IsSceneLoaded() == false)
			{
				AllowSceneChange = true;
				return AllowSceneChange;
			}
			else if (IsSceneLoaded() == true && mySettings.DisableSaveWarnings == false)
			{
				var exitBox = new UnloadingSceneWarning();
				exitBox.Owner = GetWindow(this);
				exitBox.ShowDialog();
				var exitBoxResult = exitBox.WindowResult;
				if (exitBoxResult == UnloadingSceneWarning.WindowDialogResult.Yes)
				{
					Save_Click(null, null);
					AllowSceneChange = true;
				}
				else if (exitBoxResult == UnloadingSceneWarning.WindowDialogResult.No)
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
			return AllowSceneChange;


		}
		#endregion

		#region Enable And Disable Editor Buttons
		private void SetSceneOnlyButtonsState(bool enabled, bool stageLoad = false)
		{
			saveToolStripMenuItem.IsEnabled = enabled;
			saveAsToolStripMenuItem.IsEnabled = enabled;
			backupToolStripMenuItem.IsEnabled = enabled;
			unloadSceneToolStripMenuItem.IsEnabled = enabled;
			goToToolStripMenuItem1.IsEnabled = enabled;
			specificPlaceToolStripMenuItem.IsEnabled = enabled;
			playerSpawnToolStripMenuItem.IsEnabled = enabled;
			findUnusedTilesToolStripMenuItem.IsEnabled = enabled;

			ShowFGHigh.IsEnabled = enabled && FGHigh != null;
			ShowFGLow.IsEnabled = enabled && FGLow != null;
			ShowFGHigher.IsEnabled = enabled && FGHigher != null;
			ShowFGLower.IsEnabled = enabled && FGLower != null;
			ShowEntities.IsEnabled = enabled;
			ShowAnimations.IsEnabled = enabled;
			animationsSplitButton_Dropdown.IsEnabled = enabled;
			ReloadButton.IsEnabled = enabled;
			newShortcutToolStripMenuItem.IsEnabled = Directory.Exists(DataDirectory);
			withoutCurrentCoordinatesToolStripMenuItem.IsEnabled = EditorScene != null;
			withCurrentCoordinatesToolStripMenuItem.IsEnabled = EditorScene != null;
			changeEncorePaleteToolStripMenuItem.IsEnabled = enabled;

			MultiLayerHint.Visibility = (MultiLayerEditMode ? Visibility.Visible : Visibility.Collapsed);
			LayerHint.Visibility = (MultiLayerEditMode ? Visibility.Collapsed : Visibility.Visible);


			Save.IsEnabled = enabled;

			if (mySettings.ReduceZoom)
			{
				ZoomInButton.IsEnabled = enabled && ZoomLevel < 5;
				ZoomOutButton.IsEnabled = enabled && ZoomLevel > -2;
			}
			else
			{
				ZoomInButton.IsEnabled = enabled && ZoomLevel < 5;
				ZoomOutButton.IsEnabled = enabled && ZoomLevel > -5;
			}



			RunSceneButton.IsEnabled = enabled;
			RunSceneDropDown.IsEnabled = enabled;

			if (GameRunning) SetButtonColors(RunSceneButton, Color.Blue);
			else SetButtonColors(RunSceneButton, Color.Green);

			SetEditButtonsState(enabled);
			UpdateTooltips();

			if (mySettings.preRenderSceneOption == 3 && enabled && stageLoad)
			{
				PreLoadSceneButton_Click(null, null);
			}
			else if (mySettings.preRenderSceneOption == 2 && enabled && stageLoad)
			{
				MessageBoxResult result = RSDKrU.MessageBox.Show("Do you wish to Pre-Render this scene?", "Requesting to Pre-Render the Scene", MessageBoxButton.YesNo, MessageBoxImage.Information);
				if (result == MessageBoxResult.Yes)
				{
					PreLoadSceneButton_Click(null, null);
				}
			}
			else if (mySettings.preRenderSceneOption == 1 && PreRenderSceneSelectCheckbox && enabled && stageLoad)
			{
				PreLoadSceneButton_Click(null, null);
			}

			if (stageLoad)
			{
				SetViewSize((int)(SceneWidth * Zoom), (int)(SceneHeight * Zoom));
			}

			UpdateButtonColors();
		}
		public void SetSelectOnlyButtonsState(bool enabled = true)
		{
			enabled &= IsSelected();
			deleteToolStripMenuItem.IsEnabled = enabled;
			copyToolStripMenuItem.IsEnabled = enabled;
			cutToolStripMenuItem.IsEnabled = enabled;
			duplicateToolStripMenuItem.IsEnabled = enabled;

			flipHorizontalToolStripMenuItem.IsEnabled = enabled && IsTilesEdit();
			flipVerticalToolStripMenuItem.IsEnabled = enabled && IsTilesEdit();
			flipHorizontalIndvidualToolStripMenuItem.IsEnabled = enabled && IsTilesEdit();
			flipVerticalIndvidualToolStripMenuItem.IsEnabled = enabled && IsTilesEdit();

			selectAllToolStripMenuItem.IsEnabled = (IsTilesEdit() && !IsChunksEdit()) || IsEntitiesEdit();

			if (IsEntitiesEdit() && entitiesToolbar != null)
			{
				entitiesToolbar.SelectedEntities = entities.SelectedEntities.Select(x => x.Entity).ToList();
			}
		}
		private void SetLayerEditButtonsState(bool enabled)
		{
			if (!MultiLayerEditMode)
			{
				if (enabled && EditFGLow.IsCheckedN.Value) EditLayer = FGLow;
				else if (enabled && EditFGHigh.IsCheckedN.Value) EditLayer = FGHigh;
				else if (enabled && EditFGHigher.IsCheckedN.Value) EditLayer = FGHigher;
				else if (enabled && EditFGLower.IsCheckedN.Value) EditLayer = FGLower;
				else if (enabled && ExtraLayerEditViewButtons.Any(elb => elb.Value.IsCheckedN.Value))
				{
					var selectedExtraLayerButton = ExtraLayerEditViewButtons.Single(elb => elb.Value.IsCheckedN.Value);
					var editorLayer = EditorScene.OtherLayers.Single(el => el.Name.Equals(selectedExtraLayerButton.Value.Text));

					EditLayer = editorLayer;
				}
				else EditLayer = null;
			}
			else
			{
				SetEditLayerA();
				SetEditLayerB();
			}

			if (TilesToolbar != null)
			{
				TilesToolbar.ChunksReload();
			}

			void SetEditLayerA()
			{
				if (enabled && EditFGLow.IsCheckedA.Value) EditLayerA = FGLow;
				else if (enabled && EditFGHigh.IsCheckedA.Value) EditLayerA = FGHigh;
				else if (enabled && EditFGHigher.IsCheckedA.Value) EditLayerA = FGHigher;
				else if (enabled && EditFGLower.IsCheckedA.Value) EditLayerA = FGLower;
				else if (enabled && ExtraLayerEditViewButtons.Any(elb => elb.Value.IsCheckedA.Value))
				{
					var selectedExtraLayerButton = ExtraLayerEditViewButtons.Single(elb => elb.Value.IsCheckedA.Value);
					var editorLayer = EditorScene.OtherLayers.Single(el => el.Name.Equals(selectedExtraLayerButton.Value.Text));

					EditLayerA = editorLayer;
				}
				else EditLayerA = null;
			}
			void SetEditLayerB()
			{
				if (enabled && EditFGLow.IsCheckedB.Value) EditLayerB = FGLow;
				else if (enabled && EditFGHigh.IsCheckedB.Value) EditLayerB = FGHigh;
				else if (enabled && EditFGHigher.IsCheckedB.Value) EditLayerB = FGHigher;
				else if (enabled && EditFGLower.IsCheckedB.Value) EditLayerB = FGLower;
				else if (enabled && ExtraLayerEditViewButtons.Any(elb => elb.Value.IsCheckedB.Value))
				{
					var selectedExtraLayerButton = ExtraLayerEditViewButtons.Single(elb => elb.Value.IsCheckedB.Value);
					var editorLayer = EditorScene.OtherLayers.Single(el => el.Name.Equals(selectedExtraLayerButton.Value.Text));

					EditLayerB = editorLayer;
				}
				else EditLayerB = null;
			}

		}
		private void SetEditButtonsState(bool enabled)
		{
			bool windowsClipboardState;
			bool windowsEntityClipboardState;
			EditFGLow.IsEnabled = enabled && FGLow != null;
			EditFGHigh.IsEnabled = enabled && FGHigh != null;
			EditFGLower.IsEnabled = enabled && FGLower != null;
			EditFGHigher.IsEnabled = enabled && FGHigher != null;
			EditEntities.IsEnabled = enabled;
			entityManagerToolStripMenuItem.IsEnabled = enabled && StageConfig != null;
			importSoundsToolStripMenuItem.IsEnabled = enabled && StageConfig != null;
			layerManagerToolStripMenuItem.IsEnabled = enabled;
			editBackgroundColorsToolStripMenuItem.IsEnabled = enabled;
			preRenderSceneToolStripMenuItem.IsEnabled = enabled;

			editEntitiesOptionToolStrip.IsEnabled = enabled;

			SetLayerEditButtonsState(enabled);

			undoToolStripMenuItem.IsEnabled = enabled && undo.Count > 0;
			redoToolStripMenuItem.IsEnabled = enabled && redo.Count > 0;

			MagnetMode.IsEnabled = enabled && IsEntitiesEdit();
			MagnetMode.IsChecked = UseMagnetMode && IsEntitiesEdit();
			MagnetModeSplitButton.IsEnabled = enabled && IsEntitiesEdit();
			UseMagnetMode = IsEntitiesEdit() && MagnetMode.IsChecked.Value;



			UndoButton.IsEnabled = enabled && undo.Count > 0;
			RedoButton.IsEnabled = enabled && redo.Count > 0;

			findAndReplaceToolStripMenuItem.IsEnabled = enabled && EditLayer != null;

			PointerButton.IsEnabled = enabled && IsTilesEdit();
			SelectToolButton.IsEnabled = enabled && IsTilesEdit();
			PlaceTilesButton.IsEnabled = enabled && IsTilesEdit();
			InteractionToolButton.IsEnabled = enabled;
			ChunksToolButton.IsEnabled = enabled && IsTilesEdit();

			PointerButton.IsChecked = (bool)PointerButton.IsChecked || (!(bool)PointerButton.IsChecked && !(bool)SelectToolButton.IsChecked && !(bool)PlaceTilesButton.IsChecked);
			PlaceTilesButton.IsChecked = PlaceTilesButton.IsChecked;
			InteractionToolButton.IsChecked = InteractionToolButton.IsChecked;
			ChunksToolButton.IsChecked = (bool)ChunksToolButton.IsChecked && !IsEntitiesEdit();
			if (TilesToolbar != null)
			{
				if (ChunksToolButton.IsChecked.Value)
				{
					TilesToolbar.TabControl.SelectedIndex = 1;
				}
				else
				{
					TilesToolbar.TabControl.SelectedIndex = 0;
				}
			}



			ShowGridButton.IsEnabled = enabled && StageConfig != null;
			ShowCollisionAButton.IsEnabled = enabled && TilesConfig != null;
			ShowCollisionBButton.IsEnabled = enabled && TilesConfig != null;
			ShowTileIDButton.IsEnabled = enabled && StageConfig != null;
			GridSizeButton.IsEnabled = enabled && StageConfig != null;
			EncorePaletteButton.IsEnabled = enabled && encorePaletteExists;
			FlipAssistButton.IsEnabled = enabled;



			//Doing this too often seems to cause a lot of grief for the app, should be relocated and stored as a bool
			try
			{
				if (IsTilesEdit()) windowsClipboardState = Clipboard.ContainsData("ManiacTiles");
				else windowsClipboardState = false;
				if (IsEntitiesEdit()) windowsEntityClipboardState = Clipboard.ContainsData("ManiacEntities");
				else windowsEntityClipboardState = false;
			}
			catch
			{
				windowsClipboardState = false;
				windowsEntityClipboardState = false;
			}



			if (enabled && (IsTilesEdit() || ((TilesClipboard != null || windowsClipboardState))))
				pasteToolStripMenuItem.IsEnabled = true;
			else
				pasteToolStripMenuItem.IsEnabled = false;

			if (enabled && (IsEntitiesEdit() || ((entitiesClipboard != null || windowsEntityClipboardState))))
				pasteToolStripMenuItem.IsEnabled = true;
			else
				pasteToolStripMenuItem.IsEnabled = false;


			if (IsTilesEdit())
			{
				if (TilesToolbar == null)
				{
					if (useEncoreColors)
						TilesToolbar = new TilesToolbar(StageTiles, EditorPath.StageTiles_Source, EncorePalette[0], this);
					else
						TilesToolbar = new TilesToolbar(StageTiles, EditorPath.StageTiles_Source, null, this);


					TilesToolbar.TileDoubleClick = new Action<int>(x =>
					{
						EditorPlaceTile(new Point((int)(ShiftX / Zoom) + EditorLayer.TILE_SIZE - 1, (int)(ShiftY / Zoom) + EditorLayer.TILE_SIZE - 1), x, EditLayerA);
					});
					TilesToolbar.TileOptionChanged = new Action<int, bool>((option, state) =>
					{
						EditLayerA?.SetPropertySelected(option + 12, state);
						EditLayerB?.SetPropertySelected(option + 12, state);

					});
					ToolBarPanelRight.Children.Clear();
					ToolBarPanelRight.Children.Add(TilesToolbar);
					UpdateToolbars(true, true);
					Form1_Resize(null, null);
					this.Focus();
				}
				if (IsChunksEdit()) TilesToolbar.TabControl.TabIndex = 1;
				else TilesToolbar.TabControl.TabIndex = 0;
				UpdateTilesOptions();
				TilesToolbar.ShowShortcuts = PlaceTilesButton.IsChecked.Value;
			}
			else
			{
				if (TilesToolbar != null)
				{
					TilesToolbar.Dispose();
					TilesToolbar = null;
					this.Focus();
				}
			}
			if (IsEntitiesEdit())
			{
				if (entitiesToolbar == null)
				{
					entitiesToolbar = new EntitiesToolbar2(EditorScene.Objects, this)
					{
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
					ToolBarPanelRight.Children.Clear();
					ToolBarPanelRight.Children.Add(entitiesToolbar);
					UpdateToolbars(true, true);
					Form1_Resize(null, null);
				}
				UpdateEntitiesToolbarList();
				entitiesToolbar.SelectedEntities = entities.SelectedEntities.Select(x => x.Entity).ToList();
			}
			else
			{
				if (entitiesToolbar != null) entitiesToolbar.Dispose();
				entitiesToolbar = null;
			}
			if (TilesToolbar == null && entitiesToolbar == null)
			{
				ToolBarPanelRight.Children.Clear();
				UpdateToolbars(true, false);
				Form1_Resize(null, null);
			}

			SetSelectOnlyButtonsState(enabled);
		}
		public void UpdateControls(bool stageLoad = false)
		{
			if (mySettings.EntityFreeCam)
			{
				editorView.vScrollBar1.IsEnabled = false;
				editorView.hScrollBar1.IsEnabled = false;
			}
			else
			{
				editorView.vScrollBar1.IsEnabled = true;
				editorView.hScrollBar1.IsEnabled = true;
			}
			SetSceneOnlyButtonsState(EditorScene != null, stageLoad);
		}
		private void UpdateToolbars(bool rightToolbar = true, bool visible = false)
		{
			if (rightToolbar)
			{
				if (visible)
				{
					ToolbarRight.Width = new GridLength(300);
					ToolbarRight.MinWidth = 300;
					ToolbarRight.MaxWidth = ViewPanelForm.ActualWidth / 3;
					SplitterRight.Width = new GridLength(6);
					SplitterRight.MinWidth = 6;
				}
				else
				{
					ToolbarRight.Width = new GridLength(0);
					ToolbarRight.MinWidth = 0;
					ToolbarRight.MaxWidth = 0;
					SplitterRight.Width = new GridLength(0);
					SplitterRight.MinWidth = 0;
				}
			}

			else
			{
				if (visible)
				{
					if (ToolbarLeft.Width.Value == 0)
					{
						ToolbarLeft.Width = new GridLength(300);
						ToolbarLeft.MinWidth = 300;
						ToolbarLeft.MaxWidth = ViewPanelForm.ActualWidth / 3;
						SplitterLeft.Width = new GridLength(6);
						SplitterLeft.MinWidth = 6;
					}
				}
				else
				{
					ToolbarLeft.Width = new GridLength(0);
					ToolbarLeft.MinWidth = 0;
					ToolbarLeft.MaxWidth = 0;
					SplitterLeft.Width = new GridLength(0);
					SplitterLeft.MinWidth = 0;
				}
			}

		}

		public void UpdateWaitingScreen(bool show)
		{
			if (show)
			{
				ViewPanelForm.Visibility = Visibility.Hidden;
				WaitingPanel.Visibility = Visibility.Visible;
			}
			else
			{
				ViewPanelForm.Visibility = Visibility.Visible;
				WaitingPanel.Visibility = Visibility.Collapsed;
			}

		}
		#endregion

		#region Refresh UI

		public void UpdateEntitiesToolbarList()
		{
			entitiesToolbar.Entities = entities.Entities.Select(x => x.Entity).ToList();
		}

		private void DropDownMenuUpdater(object sender, EventArgs e)
		{

			if (this.selectConfigToolStripMenuItem != null)
				if (this.selectConfigToolStripMenuItem.IsFocused)
					selectConfigToolStripMenuItem.IsSubmenuOpen = true;

			if (this.trackThePlayerToolStripMenuItem != null)
				if (this.trackThePlayerToolStripMenuItem.IsFocused)
					this.trackThePlayerToolStripMenuItem.IsSubmenuOpen = true;


		}

		public void UpdateTilesOptions()
		{
			if (IsTilesEdit() && !IsChunksEdit())
			{
				if (TilesToolbar != null)
				{
					List<ushort> values = EditLayerA?.GetSelectedValues();
					List<ushort> valuesB = EditLayerB?.GetSelectedValues();
					if (valuesB != null) values.AddRange(valuesB);

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
		}

		public void UpdateEditLayerActions()
		{
			if (EditLayerA != null)
			{
				List<IAction> actions = EditLayerA?.Actions;
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
			}
			if (EditLayerB != null)
			{
				List<IAction> actions = EditLayerB?.Actions;
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
			}
		}

		public void UpdateStatusPanel(object sender, EventArgs e)
		{
			//
			// Tooltip Bar Info 
			//

			_levelIDLabel.Content = "Level ID: " + LevelID.ToString();
			seperator1.Visibility = Visibility.Visible;
			seperator2.Visibility = Visibility.Visible;
			seperator3.Visibility = Visibility.Visible;
			seperator4.Visibility = Visibility.Visible;
			seperator5.Visibility = Visibility.Visible;
			seperator6.Visibility = Visibility.Visible;
			seperator7.Visibility = Visibility.Visible;
			//seperator8.Visibility = Visibility.Visible;
			//seperator9.Visibility = Visibility.Visible;

			if (EnablePixelCountMode == false)
			{
				selectedPositionLabel.Content = "Selected Tile Position: X: " + (int)SelectedTileX + ", Y: " + (int)SelectedTileY;
				selectedPositionLabel.ToolTip = "The Position of the Selected Tile";
			}
			else
			{
				selectedPositionLabel.Content = "Selected Tile Pixel Position: " + "X: " + (int)SelectedTileX * 16 + ", Y: " + (int)SelectedTileY * 16;
				selectedPositionLabel.ToolTip = "The Pixel Position of the Selected Tile";
			}
			if (EnablePixelCountMode == false)
			{
				selectionSizeLabel.Content = "Amount of Tiles in Selection: " + (SelectedTilesCount - DeselectTilesCount);
				selectionSizeLabel.ToolTip = "The Size of the Selection";
			}
			else
			{
				selectionSizeLabel.Content = "Length of Pixels in Selection: " + (SelectedTilesCount - DeselectTilesCount) * 16;
				selectionSizeLabel.ToolTip = "The Length of all the Tiles (by Pixels) in the Selection";
			}

			selectionBoxSizeLabel.Content = "Selection Box Size: X: " + (select_x2 - select_x1) + ", Y: " + (select_y2 - select_y1);

			scrollLockDirLabel.Content = "Scroll Direction: " + (ScrollDirection == (int)ScrollDir.X ? "X" : "Y") + (ScrollLocked ? " (Locked)" : "");


			hVScrollBarXYLabel.Content = "Zoom Value: " + Zoom.ToString();

			if (UpdateUpdaterMessage)
			{
				if (StartScreen != null) StartScreen.UpdateStatusLabel(Updater.condition, Updater);
				UpdateUpdaterMessage = false;
			}

			//
			// End of Tooltip Bar Info Section
			//
		}

		private void UpdateTooltips()
		{
			UpdateTooltipForStacks(UndoButton, undo);
			UpdateTooltipForStacks(RedoButton, redo);
			if (EditorControls != null)
			{
				if (this.IsVisible)
				{
					EditorControls.UpdateMenuItems();
					EditorControls.UpdateTooltips();
				}

			}

		}

		private void UpdateTooltipForStacks(Button tsb, Stack<IAction> actionStack)
		{

			if (actionStack?.Count > 0)
			{
				IAction action = actionStack.Peek();
				System.Windows.Controls.ToolTip tooltip = new System.Windows.Controls.ToolTip { Content = string.Format(tsb.Tag.ToString(), action.Description + " ") };
				tsb.ToolTip = tooltip;
			}
			else
			{
				System.Windows.Controls.ToolTip tooltip = new System.Windows.Controls.ToolTip { Content = string.Format(tsb.Tag.ToString(), string.Empty) };
				tsb.ToolTip = tooltip;
			}
		}

		public void ToggleEditorButtons(bool enabled)
		{
			MenuBar.IsEnabled = enabled;
			LayerToolbar.IsEnabled = enabled;
			MainToolbarButtons.IsEnabled = enabled;
			SetSceneOnlyButtonsState((enabled ? true : EditorScene != null));
			LayerToolbar.IsEnabled = enabled;
			StatusBar1.IsEnabled = enabled;
			StatusBar2.IsEnabled = enabled;
			if (TilesToolbar != null) TilesToolbar.IsEnabled = enabled;
			if (entitiesToolbar != null) entitiesToolbar.IsEnabled = enabled;
		}

		#endregion

		#region Editor Entity/Tile Management
		public void EditorPlaceTile(Point position, int tile, EditorLayer layer)
		{
			Dictionary<Point, ushort> tiles = new Dictionary<Point, ushort>
			{
				[new Point(0, 0)] = (ushort)tile
			};
			layer.PasteFromClipboard(position, tiles);
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
					RSDKrU.MessageBox.Show("Tiles found at: " + Environment.NewLine + message, "Results");
					if (copyResults && message != null)
					{
						Clipboard.SetText(message);
					}
				}
				else
				{
					RSDKrU.MessageBox.Show("Found Nothing", "Results");
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
			EditLayerA?.DeleteSelected();
			EditLayerB?.DeleteSelected();
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
			Dictionary<EditorEntity, Point> initalPos = new Dictionary<EditorEntity, Point>();
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

		private MenuItem CreateDataDirectoryMenuLink(string target)
		{
			MenuItem newItem = new MenuItem();
			newItem.Header = target;
			newItem.Tag = target;
			newItem.Click += RecentDataDirectoryClicked;
			return newItem;
		}

		public bool SetGameConfig() { return EditorPath.SetGameConfig(); }
		public bool IsDataDirectoryValid(string directoryToCheck) { return EditorPath.IsDataDirectoryValid(directoryToCheck); }

		public void RecentDataDirectoryClicked(object sender, RoutedEventArgs e, String dataDirectory)
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
								MessageBoxButton.OK,
								MessageBoxImage.Error);
				dataDirectories.Remove(dataDirectory);
				RefreshDataDirectories(dataDirectories);

			}
			mySettings.Save();
		}

		public void ResetDataDirectoryToAndResetScene(string newDataDirectory, bool forceBrowse = false, bool forceSceneSelect = false)
		{
			if (AllowSceneUnloading() != true) return;
			UnloadScene();
			UseDefaultPrefrences();
			DataDirectory = newDataDirectory;
			AddRecentDataFolder(newDataDirectory);
			bool goodGameConfig = SetGameConfig();
			if (goodGameConfig == true)
			{
				if (forceBrowse) OpenScene(true);
				else if (forceSceneSelect) OpenScene(false);
				else OpenScene(mySettings.forceBrowse);

			}


		}

		public void RecentDataDirectoryClicked(object sender, RoutedEventArgs e)
		{
			var menuItem = sender as MenuItem;
			string dataDirectory = menuItem.Tag.ToString();
			var dataDirectories = mySettings.DataDirectories;
			mySettings.GamePath = GamePath;
			if (IsDataDirectoryValid(dataDirectory))
			{
				ResetDataDirectoryToAndResetScene(dataDirectory);
			}
			else
			{
				RSDKrU.MessageBox.Show($"The specified Data Directory {dataDirectory} is not valid.",
								"Invalid Data Directory!",
								MessageBoxButton.OK,
								MessageBoxImage.Error);
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
			if (mySettings.DataDirectories?.Count > 0)
			{
				recentDataDirectoriesToolStripMenuItem.Visibility = Visibility.Collapsed;
				noRecentDataDirectoriesToolStripMenuItem.Visibility = Visibility.Collapsed;
				CleanUpRecentList();

				var startRecentItems = fileToolStripMenuItem.Items.IndexOf(recentDataDirectoriesToolStripMenuItem);
				var startRecentItemsButton = RecentDataDirectories_DropDown.Items.IndexOf(noRecentDataDirectoriesToolStripMenuItem);

				foreach (var dataDirectory in recentDataDirectories)
				{
					_recentDataItems.Add(CreateDataDirectoryMenuLink(dataDirectory));
					_recentDataItems_Button.Add(CreateDataDirectoryMenuLink(dataDirectory));

				}


				foreach (var menuItem in _recentDataItems.Reverse())
				{
					fileToolStripMenuItem.Items.Insert(startRecentItems, menuItem);
				}

				foreach (var menuItem in _recentDataItems_Button.Reverse())
				{
					RecentDataDirectories_DropDown.Items.Insert(startRecentItemsButton, menuItem);
				}
			}
			else
			{
				recentDataDirectoriesToolStripMenuItem.Visibility = Visibility.Visible;
				noRecentDataDirectoriesToolStripMenuItem.Visibility = Visibility.Visible;
			}



		}

		public void UpdateDataFolderLabel(object sender, RoutedEventArgs e)
		{
			string dataFolderTag_Normal = "Data Directory: {0}";

			_baseDataDirectoryLabel.Tag = dataFolderTag_Normal;
			UpdateDataFolderLabel();
			showingDataDirectory = true;
		}

		private void UpdateDataFolderLabel(string dataDirectory = null)
		{
			if (dataDirectory != null) _baseDataDirectoryLabel.Content = string.Format(_baseDataDirectoryLabel.Tag.ToString(), dataDirectory);
			else _baseDataDirectoryLabel.Content = string.Format(_baseDataDirectoryLabel.Tag.ToString(), DataDirectory);
		}

		/// <summary>
		/// Removes any recent Data directories from the File menu.
		/// </summary>
		private void CleanUpRecentList()
		{
			foreach (var menuItem in _recentDataItems)
			{
				menuItem.Click -= RecentDataDirectoryClicked;
				fileToolStripMenuItem.Items.Remove(menuItem);
			}
			foreach (var menuItem in _recentDataItems_Button)
			{
				menuItem.Click -= RecentDataDirectoryClicked;
				RecentDataDirectories_DropDown.Items.Remove(menuItem);
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

		public void SetZoomLevel(int zoom_level, Point zoom_point, double zoom_level_d = 0.0)
		{
			double old_zoom = Zoom;



			if (zoom_level_d == 0.0)
			{
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
			}
			else
			{
				ZoomLevel = (int)zoom_level_d;
				Zoom = zoom_level_d;
			}


			zooming = true;

			int oldShiftX = ShiftX;
			int oldShiftY = ShiftY;

			if (EditorScene != null)
				SetViewSize((int)(SceneWidth * Zoom), (int)(SceneHeight * Zoom));


			if (editorView.hScrollBar1.IsVisible)
			{
				ShiftX = (int)((zoom_point.X + oldShiftX) / old_zoom * Zoom - zoom_point.X);
				ShiftX = (int)Math.Min((editorView.hScrollBar1.Maximum), Math.Max(0, ShiftX));
				editorView.hScrollBar1.Value = ShiftX;
			}
			if (editorView.vScrollBar1.IsVisible)
			{
				ShiftY = (int)((zoom_point.Y + oldShiftY) / old_zoom * Zoom - zoom_point.Y);
				ShiftY = (int)Math.Min((editorView.vScrollBar1.Maximum), Math.Max(0, ShiftY));
				editorView.vScrollBar1.Value = ShiftY;
			}


			zooming = false;

			UpdateControls();
		}

		public void Form1_Resize(object sender, RoutedEventArgs e)
		{
			// TODO: It hides right now few pixels at the edge

			Visibility nvscrollbar = Visibility.Visible;
			Visibility nhscrollbar = Visibility.Visible;

			if (editorView.hScrollBar1.Maximum == 0) nhscrollbar = Visibility.Hidden;
			if (editorView.vScrollBar1.Maximum == 0) nvscrollbar = Visibility.Hidden;

			editorView.vScrollBar1.Visibility = nvscrollbar;
			editorView.vScrollBar1Host.Child.Visibility = nvscrollbar;
			editorView.hScrollBar1Host.Child.Visibility = nhscrollbar;
			editorView.hScrollBar1.Visibility = nhscrollbar;

			if (editorView.vScrollBar1.IsVisible)
			{
				editorView.vScrollBar1.ViewportSize = SceneHeight;
				editorView.vScrollBar1.LargeChange = editorView.vScrollBar1Host.Height;
				editorView.vScrollBar1.SmallChange = editorView.vScrollBar1Host.Height / 4;
				ScreenHeight = (int)editorView.vScrollBar1Host.Height;
				editorView.hScrollBar1.Value = Math.Max(0, Math.Min(editorView.hScrollBar1.Value, editorView.hScrollBar1.Maximum));
			}
			else
			{
				ScreenHeight = editorView.GraphicPanel.Height;
				ShiftY = 0;
				editorView.vScrollBar1.Value = 0;
			}
			if (editorView.hScrollBar1.IsVisible)
			{
				editorView.hScrollBar1.ViewportSize = SceneWidth;
				editorView.hScrollBar1.LargeChange = editorView.hScrollBar1Host.Width;
				editorView.hScrollBar1.SmallChange = editorView.hScrollBar1Host.Width / 4;
				ScreenWidth = (int)editorView.hScrollBar1Host.Width;
				editorView.vScrollBar1.Value = Math.Max(0, Math.Min(editorView.vScrollBar1.Value, editorView.vScrollBar1.Maximum));
			}
			else
			{
				ScreenWidth = editorView.GraphicPanel.Width;
				ShiftX = 0;
				editorView.hScrollBar1.Value = 0;
			}



			while (ScreenWidth > editorView.GraphicPanel.Width)
				ResizeGraphicPanel(editorView.GraphicPanel.Width * 2, editorView.GraphicPanel.Height);
			while (ScreenHeight > editorView.GraphicPanel.Height)
				ResizeGraphicPanel(editorView.GraphicPanel.Width, editorView.GraphicPanel.Height * 2);


		}

		public void SetViewSize(int width = 0, int height = 0)
		{

			if (mySettings.EntityFreeCam)
			{
				width = 10000000;
				height = 10000000;
			}
			else if (isExportingImage)
			{
				width = SceneWidth;
				height = SceneHeight;
			}

            if (!isExportingImage && !mySettings.EntityFreeCam)
            {
                editorView.vScrollBar1.Maximum = height - editorView.vScrollBar1.LargeChange;
                editorView.hScrollBar1.Maximum = width - editorView.hScrollBar1.LargeChange;
            }

			editorView.GraphicPanel.DrawWidth = Math.Min((int)width, editorView.GraphicPanel.Width);
			editorView.GraphicPanel.DrawHeight = Math.Min((int)height, editorView.GraphicPanel.Height);

			Form1_Resize(null, null);

            if (!isExportingImage && !mySettings.EntityFreeCam)
            {
                editorView.hScrollBar1.Value = Math.Max(0, Math.Min(editorView.hScrollBar1.Value, editorView.hScrollBar1.Maximum));
                editorView.vScrollBar1.Value = Math.Max(0, Math.Min(editorView.vScrollBar1.Value, editorView.vScrollBar1.Maximum));
            }

        }

		public void ResetViewSize()
		{
			SetViewSize((int)(SceneWidth * Zoom), (int)(SceneHeight * Zoom));
		}

		private void ResizeGraphicPanel(int width = 0, int height = 0)
		{
			if (mySettings.EntityFreeCam)
			{
				width = SceneWidth;
				height = SceneHeight;
			}

			editorView.GraphicPanel.Width = width;
			editorView.GraphicPanel.Height = height;

			editorView.GraphicPanel.ResetDevice();

			editorView.GraphicPanel.DrawWidth = Math.Min((int)editorView.hScrollBar1.Maximum, editorView.GraphicPanel.Width);
			editorView.GraphicPanel.DrawHeight = Math.Min((int)editorView.vScrollBar1.Maximum, editorView.GraphicPanel.Height);

		}

		#endregion

		#region Scene Loading / Unloading + Repair
		public void RepairScene()
		{
			string Result = null;
			OpenFileDialog open = new OpenFileDialog() { };
			open.Filter = "Scene File|*.bin";
			if (open.ShowDialog() != System.Windows.Forms.DialogResult.Cancel)
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
			HideConsoleWindow();

		}
		private bool EditorLoad()
		{
			if (DataDirectory == null)
			{
				return false;
			}
			EditorEntity_ini.ReleaseResources();
			return true;
		}
		public void UnloadScene()
		{
			EditorScene?.Dispose();
			EditorScene = null;
			StageConfig = null;
			_levelIDLabel.Content = "Level ID: NULL";
			LevelID = -1;
			encorePaletteExists = false;
			EncoreSetupType = 0;
			playerObjectPosition = new List<SceneEntity> { };
			INILayerNameHigher = "";
			INILayerNameLower = "";
			EditorSettings.CleanPrefrences();
			userDefinedEntityRenderSwaps = new Dictionary<string, string>();
			userDefinedSpritePaths = new List<string>();
			EncorePaletteButton.IsChecked = false;
			EditorPath.UnloadScene();

			if (StageTiles != null) StageTiles.Dispose();
			StageTiles = null;

			TearDownExtraLayerButtons();

			Background = null;

			EditorChunk = null;

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

			EditFGLow.ClearCheckedItems();
			EditFGHigh.ClearCheckedItems();
			EditFGLower.ClearCheckedItems();
			EditFGHigher.ClearCheckedItems();
			EditEntities.ClearCheckedItems();

			SetViewSize();

			UpdateControls();

			// clear memory a little more aggressively 
			EditorEntity_ini.ReleaseResources();
			GC.Collect();

			CollisionLayerA.Clear();
			CollisionLayerB.Clear();
			TilesConfig = null;

			MenuChar = MenuCharS.ToCharArray();
			MenuChar_Small = MenuCharS_Small.ToCharArray();
			LevelSelectChar = LevelSelectCharS.ToCharArray();

			ResourcePackList.Clear();

			UpdateStartScreen(true);
		}

		public void OpenSceneForceFully()
		{

			string dataDirectory = mySettings.DevForceRestartData;
			DataDirectory = dataDirectory;
			string Result = mySettings.DevForceRestartScene;
			int LevelID = mySettings.DeveForceRestartLevelID;
			bool isEncore = mySettings.DevForceRestartEncore;
			string CurrentZone = mySettings.DevForceRestartCurrentZone;
			string CurrentName = mySettings.DevForceRestartCurrentName;
			string CurrentSceneID = mySettings.DevForceRestartCurrentSceneID;
			bool Browsed = mySettings.DevForceRestartBrowsed;

			int x = mySettings.DevForceRestartX;
			int y = mySettings.DevForeRestartY;
			TempWarpCoords = new Point(x, y);
			ForceWarp = true;

			EditorSceneLoading.OpenSceneForcefully(dataDirectory, Result, LevelID, isEncore, CurrentZone, CurrentZone, CurrentSceneID, Browsed);
		}
		private void OpenSceneForceFully(string dataDir, string scenePath, string modPath, int levelID, bool isEncoreMode, int X, int Y, double _ZoomScale = 0.0, string SceneID = "", string Zone = "", string Name = "")
		{
			MessageBox.Show("These Kind of Shortcuts are Broken for now! SORRY!");

			/*
			string dataDirectory = dataDir;
			DataDirectory = dataDirectory;
			string Result = scenePath;
			int LevelID = levelID;
			bool isEncore = isEncoreMode;
			string CurrentZone = Zone;
			string CurrentName = Name;
			string CurrentSceneID = SceneID;
			bool Browsed = false;

			if (_ZoomScale != 0.0)
			{
				ShortcutZoomValue = _ZoomScale;
				ShortcutHasZoom = true;
			}
			TempWarpCoords = new Point(X, Y);
			ForceWarp = true;

			if (CurrentZone == "" || CurrentName == "" || CurrentSceneID == "")
			{
				MessageBox.Show("Shortcuts are Broken for now! SORRY!");
				return;
			}
			else
			{
				EditorSceneLoading.OpenSceneForcefully(dataDirectory, Result, LevelID, isEncore, CurrentZone, CurrentZone, CurrentSceneID, Browsed);
			}*/
		}

		private void OpenSceneForceFully(string dataDir)
		{
			DataDirectory = dataDir;
			EditorSceneLoading.OpenSceneForcefullyUsingSceneSelect(DataDirectory);
		}

		public void OpenScene(bool manual = false, string Result = null, int LevelID = -1, bool isEncore = false, bool modLoaded = false, string modDir = "")
		{
			EditorSceneLoading.OpenSceneUsingSceneSelect();
		}
		#endregion

		#region GraphicsPanel + Program + Splitcontainer
		public void OnResetDevice(object sender, DeviceEventArgs e)
		{
			Device device = e.Device;
		}
		public void CheckDeviceState(object sender, PowerModeChangedEventArgs e)
		{
			switch (e.Mode)
			{
				case PowerModes.Suspend:
					SetDeviceSleepState(false);
					break;
				case PowerModes.Resume:
					SetDeviceSleepState(true);
					break;
			}
		}
		private void Editor_Resize(object sender, SizeChangedEventArgs e)
		{
			Form1_Resize(this, null);
		}
		private void ParentGrid_Loaded(object sender, RoutedEventArgs e)
		{
			// Create the interop host control.
			host = new System.Windows.Forms.Integration.WindowsFormsHost();

			// Create the MaskedTextBox control.

			// Assign the MaskedTextBox control as the host control's child.
			host.Child = editorView;

			host.Foreground = (SolidColorBrush)FindResource("NormalText");

			// Add the interop host control to the Grid
			// control's collection of child controls.
			this.ViewPanelForm.Children.Add(host);

			editorView.GraphicPanel.Init(editorView);
		}
		private void GraphicPanel_OnRender(object sender, DeviceEventArgs e)
		{
			// hmm, if I call refresh when I update the values, for some reason it will stop to render until I stop calling refrsh
			// So I will refresh it here

			bool showEntities = ShowEntities.IsChecked.Value && !EditEntities.IsCheckedAll;
			bool showEntitiesEditing = EditEntities.IsCheckedAll;

			bool PriorityMode = mySettings.PrioritizedObjectRendering;
			bool AboveAllMode = entityVisibilityType == 1;


			if (entitiesToolbar?.NeedRefresh ?? false) entitiesToolbar.PropertiesRefresh();
			if (EditorScene != null)
			{
				if (!isExportingImage)
				{
					if (!IsTilesEdit())
						EditorBackground.Draw(editorView.GraphicPanel);
					if (IsTilesEdit())
					{
						if (mySettings.ShowEditLayerBackground == true)
						{
							EditorBackground.DrawEdit(editorView.GraphicPanel);
						}
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


				if (DebugStatsVisibleOnPanel && EditorScene != null)
				{
					Point point = new Point((short)(15), (short)(15));

					DebugTextHUD.DrawEditorHUDText(this, editorView.GraphicPanel, point.X, point.Y, statusBox.GetDataFolder(), true, 255, 15);
					DebugTextHUD.DrawEditorHUDText(this, editorView.GraphicPanel, point.X, point.Y + 12 * 1, statusBox.GetMasterDataFolder(), true, 255, 22);
					DebugTextHUD.DrawEditorHUDText(this, editorView.GraphicPanel, point.X, point.Y + 12 * 2, statusBox.GetScenePath(), true, 255, 11);
					DebugTextHUD.DrawEditorHUDText(this, editorView.GraphicPanel, point.X, point.Y + 12 * 3, statusBox.GetSceneFilePath(), true, 255, 12);
					DebugTextHUD.DrawEditorHUDText(this, editorView.GraphicPanel, point.X, point.Y + 12 * 4, statusBox.GetZoom(), true, 255, 11);
					DebugTextHUD.DrawEditorHUDText(this, editorView.GraphicPanel, point.X, point.Y + 12 * 5, statusBox.GetSetupObject(), true, 255, 13);
					DebugTextHUD.DrawEditorHUDText(this, editorView.GraphicPanel, point.X, point.Y + 12 * 6, statusBox.GetSelectedZone(), true, 255, 14);

					DebugTextHUD.DrawEditorHUDText(this, editorView.GraphicPanel, point.X, point.Y + 12 * 8, "Use " + EditorControls.KeyBindPraser("StatusBoxToggle") + " to Toggle this Information", true, 255, EditorControls.KeyBindPraser("StatusBoxToggle").Length, 4);
				}



				if (EditorScene.OtherLayers.Contains(EditLayer)) EditLayer.Draw(editorView.GraphicPanel);

				if (!extraLayersMoveToFront)
				{

					foreach (var elb in ExtraLayerEditViewButtons)
					{
						if (elb.Key.IsCheckedAll || elb.Value.IsCheckedAll)
						{
							var _extraViewLayer = EditorScene.OtherLayers.Single(el => el.Name.Equals(elb.Key.Text));
							_extraViewLayer.Draw(editorView.GraphicPanel);
						}
					}
				}

				if (ShowFGLower.IsChecked.Value || EditFGLower.IsCheckedAll) FGLower.Draw(editorView.GraphicPanel);
				if (ShowFGLow.IsChecked.Value || EditFGLow.IsCheckedAll) FGLow.Draw(editorView.GraphicPanel);


				if (showEntities && !AboveAllMode)
				{
					if (PriorityMode)
					{
						entities.DrawPriority(editorView.GraphicPanel, -1);
						entities.DrawPriority(editorView.GraphicPanel, 0);
						entities.DrawPriority(editorView.GraphicPanel, 1);
					}
					else
					{
						entities.Draw(editorView.GraphicPanel);
					}
				}

				if (ShowFGHigh.IsChecked.Value || EditFGHigh.IsCheckedAll)
					FGHigh.Draw(editorView.GraphicPanel);

				
				if (showEntities && PriorityMode && !AboveAllMode)
				{
					entities.DrawPriority(editorView.GraphicPanel, 2);
					entities.DrawPriority(editorView.GraphicPanel, 3);
				}

				if (ShowFGHigher.IsChecked.Value || EditFGHigher.IsCheckedAll)
					FGHigher.Draw(editorView.GraphicPanel);

				if (extraLayersMoveToFront)
				{
					foreach (var elb in ExtraLayerEditViewButtons)
					{
						if (elb.Value.IsCheckedAll || elb.Key.IsCheckedAll)
						{
							var _extraViewLayer = EditorScene.OtherLayers.Single(el => el.Name.Equals(elb.Key.Text));
							_extraViewLayer.Draw(editorView.GraphicPanel);
						}
					}
				}

				if (showEntitiesEditing || AboveAllMode)
				{
					if (PriorityMode)
					{
						entities.DrawPriority(editorView.GraphicPanel, -1);
						entities.DrawPriority(editorView.GraphicPanel, 0);
						entities.DrawPriority(editorView.GraphicPanel, 1);
						entities.DrawPriority(editorView.GraphicPanel, 2);
						entities.DrawPriority(editorView.GraphicPanel, 3);
					}
					else
					{
						entities.Draw(editorView.GraphicPanel);
					}
				}

			}

			if (draggingSelection)
			{
				int bound_x1 = (int)(selectingX / Zoom); int bound_x2 = (int)(lastX / Zoom);
				int bound_y1 = (int)(selectingY / Zoom); int bound_y2 = (int)(lastY / Zoom);
				if (bound_x1 != bound_x2 && bound_y1 != bound_y2)
				{
					if (bound_x1 > bound_x2)
					{
						bound_x1 = (int)(lastX / Zoom);
						bound_x2 = (int)(selectingX / Zoom);
					}
					if (bound_y1 > bound_y2)
					{
						bound_y1 = (int)(lastY / Zoom);
						bound_y2 = (int)(selectingY / Zoom);
					}


				}

				editorView.GraphicPanel.DrawRectangle(bound_x1, bound_y1, bound_x2, bound_y2, Color.FromArgb(100, Color.Purple));
				editorView.GraphicPanel.DrawLine(bound_x1, bound_y1, bound_x2, bound_y1, Color.Purple);
				editorView.GraphicPanel.DrawLine(bound_x1, bound_y1, bound_x1, bound_y2, Color.Purple);
				editorView.GraphicPanel.DrawLine(bound_x2, bound_y2, bound_x2, bound_y1, Color.Purple);
				editorView.GraphicPanel.DrawLine(bound_x2, bound_y2, bound_x1, bound_y2, Color.Purple);
			}
			else
			{
				select_x1 = 0; select_x2 = 0; select_y1 = 0; select_y2 = 0;
			}

			if (showGrid && EditorScene != null)
				EditorBackground.DrawGrid(editorView.GraphicPanel);

			if (GameRunning)
			{
				EditorGame.DrawGameElements(editorView.GraphicPanel);

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

			if (scrolling)
			{
				if (editorView.vScrollBar1.IsVisible && editorView.hScrollBar1.IsVisible) editorView.GraphicPanel.Draw2DCursor(scrollPosition.X, scrollPosition.Y);
				else if (editorView.vScrollBar1.IsVisible) editorView.GraphicPanel.DrawVertCursor(scrollPosition.X, scrollPosition.Y);
				else if (editorView.hScrollBar1.IsVisible) editorView.GraphicPanel.DrawHorizCursor(scrollPosition.X, scrollPosition.Y);
			}
			if (ForceWarp)
			{
				if (ShortcutHasZoom) SetZoomLevel(0, TempWarpCoords, ShortcutZoomValue);
				else SetZoomLevel(mySettings.DevForceRestartZoomLevel, TempWarpCoords);
				GoToPosition(TempWarpCoords.X, TempWarpCoords.Y, false, true);
				SetViewSize((int)(SceneWidth * Zoom), (int)(SceneHeight * Zoom));
			}
		}
		public void DrawLayers(int drawOrder = 0)
		{
			var _extraViewLayer = EditorScene.LayerByDrawingOrder.FirstOrDefault(el => el.Layer.DrawingOrder.Equals(drawOrder));
			_extraViewLayer.Draw(editorView.GraphicPanel);
		}
		public void Run()
		{
			Show();
			Focus();
			editorView.Show();
			editorView.GraphicPanel.Run();

		}
		private void GraphicPanel_DragEnter(object sender, System.Windows.Forms.DragEventArgs e)
		{
			if (!mySettings.DisableDraging)
			{
				if (e.Data.GetDataPresent(typeof(Int32)) && IsTilesEdit())
				{
					Point rel = editorView.GraphicPanel.PointToScreen(Point.Empty);
					e.Effect = System.Windows.Forms.DragDropEffects.Move;
					EditLayer?.StartDragOver(new Point((int)(((e.X - rel.X) + ShiftX) / Zoom), (int)(((e.Y - rel.Y) + ShiftY) / Zoom)), (ushort)TilesToolbar.SelectedTile);
					UpdateEditLayerActions();
				}
				else
				{
					e.Effect = System.Windows.Forms.DragDropEffects.None;
				}
			}
		}
		private void GraphicPanel_DragOver(object sender, System.Windows.Forms.DragEventArgs e)
		{
			if (!mySettings.DisableDraging)
			{
				if (e.Data.GetDataPresent(typeof(Int32)) && IsTilesEdit())
				{
					Point rel = editorView.GraphicPanel.PointToScreen(Point.Empty);
					EditLayer?.DragOver(new Point((int)(((e.X - rel.X) + ShiftX) / Zoom), (int)(((e.Y - rel.Y) + ShiftY) / Zoom)), (ushort)TilesToolbar.SelectedTile);
					editorView.GraphicPanel.Render();

				}
			}
		}
		private void GraphicPanel_DragLeave(object sender, EventArgs e)
		{
			if (!mySettings.DisableDraging)
			{
				EditLayer?.EndDragOver(true);
				editorView.GraphicPanel.Render();
			}
		}
		private void GraphicPanel_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
		{
			if (!mySettings.DisableDraging)
			{
				EditLayer?.EndDragOver(false);
			}
		}
		public void GraphicPanel_OnKeyDown(object    sender, System.Windows.Forms.KeyEventArgs e)
		{
			EditorControls.GraphicPanel_OnKeyDown(sender, e);
		}
		private void EditorViewWPF_KeyDown(object sender, KeyEventArgs e)
		{
			var e2 = KeyEventExts.ToWinforms(e);
			if (e2 != null)
			{
				EditorControls.GraphicPanel_OnKeyDown(sender, e2);
			}

		}
		public void GraphicPanel_OnKeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			EditorControls.GraphicPanel_OnKeyUp(sender, e);
		}
		private void EditorViewWPF_KeyUp(object sender, KeyEventArgs e)
		{
			var e2 = KeyEventExts.ToWinforms(e);
			if (e2 != null)
			{
				EditorControls.GraphicPanel_OnKeyUp(sender, e2);
			}

		}
		private void MapEditor_Activated(object sender, EventArgs e)
		{
			editorView.GraphicPanel.Focus();
			if (mainform.hasModified)
			{
				ReloadToolStripButton_Click(sender, null);
			}

		}
		private void MapEditor_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (!editorView.GraphicPanel.Focused)
			{
				EditorControls.GraphicPanel_OnKeyDown(sender, e);
			}
		}
		private void Editor_FormClosing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (Interactions.ManiaPalConnector != null) Interactions.ManiaPalConnector.Kill();

			try
			{
				GameRunning = false;
				var mySettings = Properties.Settings.Default;
				mySettings.IsMaximized = WindowState == System.Windows.WindowState.Maximized;
				mySettings.Save();
			}
			catch (Exception ex)
			{
				Debug.Write("Failed to write settings: " + ex);
			}

			editorView.Dispose();
			//editorView = null;
			host.Child.Dispose();
			//host = null;



		}
		private void MapEditor_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (!editorView.GraphicPanel.Focused)
			{
				EditorControls.GraphicPanel_OnKeyUp(sender, e);
			}
		}
		private void SplitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
		{
			Form1_Resize(null, null);
		}
		public void UpdateStartScreen(bool visible, bool firstLoad = false)
		{
			if (firstLoad)
			{
				Thread thread = new Thread(() => {
					Updater.CheckforUpdates(true, true);
					Editor.UpdateUpdaterMessage = true;
				});
				thread.Start();
				this.OverlayPanel.Children.Add(StartScreen);
				StartScreen.SelectScreen.ReloadQuickPanel();
				this.ViewPanelForm.Visibility = Visibility.Hidden;


			}
			if (visible)
			{
				StartScreen.Visibility = Visibility.Visible;
				StartScreen.SelectScreen.ReloadQuickPanel();
				this.ViewPanelForm.Visibility = Visibility.Hidden;
			}
			else
			{
				StartScreen.Visibility = Visibility.Hidden;
				StartScreen.SelectScreen.ReloadQuickPanel();
				this.ViewPanelForm.Visibility = Visibility.Visible;
			}

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
				EditLayerA?.Deselect();
				EditLayerB?.Deselect();

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

		public void CopyTilesToClipboard(bool doNotUseWindowsClipboard = false)
		{
			bool hasMultipleValidLayers = EditLayerA != null && EditLayerB != null;
			if (!hasMultipleValidLayers)
			{
				Dictionary<Point, ushort> copyDataA = EditLayerA?.CopyToClipboard();
				Dictionary<Point, ushort> copyDataB = EditLayerB?.CopyToClipboard();
				Tuple<Dictionary<Point, ushort>, Dictionary<Point, ushort>> copyData = new Tuple<Dictionary<Point, ushort>, Dictionary<Point, ushort>>(copyDataA, copyDataB);

				// Make a DataObject for the copied data and send it to the Windows clipboard for cross-instance copying
				if (mySettings.EnableWindowsClipboard && !doNotUseWindowsClipboard)
					Clipboard.SetDataObject(new DataObject("ManiacTiles", copyData), true);

				// Also copy to Maniac's clipboard in case it gets overwritten elsewhere
				TilesClipboard = copyData;
			}
			else if (hasMultipleValidLayers && MultiLayerEditMode)
			{
				Tuple<Dictionary<Point, ushort>, Dictionary<Point, ushort>> copyData = EditorLayer.CopyMultiSelectionToClipboard(EditLayerA, EditLayerB);

				// Make a DataObject for the copied data and send it to the Windows clipboard for cross-instance copying
				if (mySettings.EnableWindowsClipboard && !doNotUseWindowsClipboard)
					Clipboard.SetDataObject(new DataObject("ManiacTiles", copyData), true);

				// Also copy to Maniac's clipboard in case it gets overwritten elsewhere
				TilesClipboard = copyData;
			}


		}

		public void CopyEntitiesToClipboard()
		{
			if (entitiesToolbar.IsFocused == false)
			{
				// Windows Clipboard mode (WPF Current Breaks this Apparently)
				/*
				if (mySettings.EnableWindowsClipboard && !mySettings.ProhibitEntityUseOnExternalClipboard)
				{
					// Clone the entities and stow them here
					List<EditorEntity> copyData = entities.CopyToClipboard();

					// Prepare each Entity for the copy to release unnecessary data
					foreach (EditorEntity entity in copyData)
						entity.PrepareForExternalCopy();

					// Make a DataObject for the data and send it to the Windows clipboard for cross-instance copying
					Clipboard.SetDataObject(new DataObject("ManiacEntities", copyData), true);
				}*/

				// Local Clipboard mode
				{
					// Clone the entities and stow them here
					List<EditorEntity> copyData = entities.CopyToClipboard();

					// Send to Maniac's clipboard
					entitiesClipboard = copyData;
				}
			}
		}

		public void MoveEntityOrTiles(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			int x = 0, y = 0;
			int modifier = (IsChunksEdit() ? 8 : 1);
			if (MagnetMode.IsChecked == false)
			{
				UseMagnetMode = false;
			}
			if (nudgeFasterButton.IsChecked == false)
			{
				mySettings.EnableFasterNudge = false;
				nudgeFasterButton.IsChecked = false;
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
						case Keys.Up: y = (-1 - mySettings.FasterNudgeValue) * modifier; break;
						case Keys.Down: y = (1 + mySettings.FasterNudgeValue) * modifier; break;
						case Keys.Left: x = (-1 - mySettings.FasterNudgeValue) * modifier; break;
						case Keys.Right: x = (1 + mySettings.FasterNudgeValue) * modifier; break;
					}
				}

			}
			if (UseMagnetMode == false && mySettings.EnableFasterNudge == false)
			{
				switch (e.KeyData)
				{
					case Keys.Up: y = -1 * modifier; break;
					case Keys.Down: y = 1 * modifier; break;
					case Keys.Left: x = -1 * modifier; break;
					case Keys.Right: x = 1 * modifier; break;
				}

			}
			EditLayerA?.MoveSelectedQuonta(new Point(x, y));
			EditLayerB?.MoveSelectedQuonta(new Point(x, y));

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
			if (CtrlPressed() && ShiftPressed())
			{
				switch (e.Key)
				{
					case Key.Up: CustomY -= 5; break;
					case Key.Down: CustomY += 5; break;
					case Key.Left: CustomX -= 5; break;
					case Key.Right: CustomX += 5; break;
				}
			}

		}

		public void CreateShortcut(string dataDir, string scenePath = "", string modPath = "", int X = 0, int Y = 0, bool isEncoreMode = false, int LevelSlotNum = -1, double ZoomedLevel = 0.0)
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
				launchArguments = (dataDir != "" ? "DataDir=" + "\"" + dataDir + "\" " : "") + (scenePath != "" ? "ScenePath=" + "\"" + scenePath + "\" " : "") + (modPath != "" ? "ModPath=" + "\"" + modPath + "\" " : "") + (LevelSlotNum != -1 ? "LevelID=" + LevelSlotNum.ToString() + " " : "") + (isEncoreMode == true ? "EncoreMode=TRUE " : "") + (X != 0 ? "X=" + X.ToString() + " " : "") + (Y != 0 ? "Y=" + Y.ToString() + " " : "") + (ZoomedLevel != 0 ? "ZoomedLevel=" + ZoomedLevel.ToString() + " " : "");
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
		public void ShowError(string message, string title = "Error!")
		{
			MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
		}
		public void GoToPosition(int x, int y, bool CenterCoords = true, bool ShortcutClear = false)
		{
			if (CenterCoords)
			{
				Rectangle r = editorView.GraphicPanel.GetScreen();
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
				ShortcutHasZoom = false;
				ShortcutZoomValue = 0.0;
			}

		}

		#endregion

		#region Asset Reloading
		public void ReloadSpecificTextures(object sender, RoutedEventArgs e)
		{
			try
			{
				// release all our resources, and force a reload of the tiles
				// Entities should take care of themselves
				DisposeTextures();

				if (useEncoreColors)
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
				RSDKrU.MessageBox.Show(ex.Message);
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
			if (EditorScene != null && CollisionLayerA != null && CollisionLayerB != null)
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

		}
		#endregion

		#region Scrollbar Methods

		private void VScrollBar1_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
		{
			ShiftY = (int)e.NewValue;
            //editorView.GraphicPanel.Render();
        }

		private void HScrollBar1_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
		{
			ShiftX = (int)e.NewValue;
            //editorView.GraphicPanel.Render();
        }

		private void VScrollBar1_ValueChanged(object sender, RoutedEventArgs e)
		{
			ShiftY = (int)editorView.vScrollBar1.Value;
			if (!(zooming || draggingSelection || dragged || scrolling)) editorView.GraphicPanel.Render();
			if (draggingSelection)
			{
				editorView.GraphicPanel.OnMouseMoveEventCreate();
			}

		}

		private void HScrollBar1_ValueChanged(object sender, RoutedEventArgs e)
		{
			ShiftX = (int)editorView.hScrollBar1.Value;
			if (!(zooming || draggingSelection || dragged || scrolling)) editorView.GraphicPanel.Render();
			if (draggingSelection)
			{
				editorView.GraphicPanel.OnMouseMoveEventCreate();
			}

		}

		private void VScrollBar1_Entered(object sender, EventArgs e)
		{
			if (!ScrollLocked)
			{
				ScrollDirection = (int)ScrollDir.Y;
			}
		}

		private void HScrollBar1_Entered(object sender, EventArgs e)
		{
			if (!ScrollLocked)
			{
				ScrollDirection = (int)ScrollDir.X;
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
		public void BackupTool(object sender, RoutedEventArgs e)
		{

		}

		#endregion

		#region Get + Set Methods
		public Rectangle GetScreen()
		{
			if (mySettings.EntityFreeCam && !isExportingImage) return new Rectangle(CustomX, CustomY, editorView.mainPanel.Width, editorView.mainPanel.Height);
			else if (isExportingImage) return new Rectangle(0, 0, SceneWidth, SceneHeight);
			else return new Rectangle(ShiftX, ShiftY, editorView.mainPanel.Width, editorView.mainPanel.Height);
		}
		public double GetZoom()
		{
			if (isExportingImage) return 1;
			else return Zoom;
		}
		private void SetDeviceSleepState(bool state)
		{
			editorView.GraphicPanel.bRender = state;
			if (state == true)
			{
				ReloadToolStripButton_Click(null, null);
			}
		}
		public Scene GetSceneSelection()
		{
			string selectedScene;

			ManiacEditor.Interfaces.SceneSelectWindow select = new ManiacEditor.Interfaces.SceneSelectWindow(GameConfig, this);
			select.Owner = Window.GetWindow(this);
			select.ShowDialog();
			if (select.SceneSelect.Result == null)
				return null;
			selectedScene = select.SceneSelect.Result;

			if (!File.Exists(selectedScene))
			{
				string[] splitted = selectedScene.Split('\\');

				string part1 = splitted[0];
				string part2 = splitted[1];

				selectedScene = Path.Combine(DataDirectory, "Stages", part1, part2);
			}
			return new Scene(selectedScene);
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
				systemColors.SetColor(KnownColor.InfoText, darkTheme3);
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
		public void UseDarkTheme_WPF(bool state = false)
		{
			if (state)
			{
				App.ChangeSkin(Skin.Dark);
				UseDarkTheme(true);
			}
			else
			{
				App.ChangeSkin(Skin.Light);
				UseDarkTheme(false);
			}
		}
		public Control UseExternalDarkTheme(Control control)
		{
			foreach (Control c in control.Controls)
			{
				if (c is Cyotek.Windows.Forms.ColorEditor)
				{
					foreach (Control c2 in c.Controls)
					{
						if (c2 is System.Windows.Forms.NumericUpDown)
						{
							c2.ForeColor = Color.Black;
							c2.BackColor = Color.White;
						}
						if (c2 is System.Windows.Forms.ComboBox)
						{
							c2.ForeColor = Color.Black;
							c2.BackColor = Color.White;
						}
					}
				}

				if (c is System.Windows.Forms.Button)
				{
					c.ForeColor = Color.Black;
				}
				if (c is NumericUpDown)
				{
					c.ForeColor = Color.Black;
					c.BackColor = Color.White;
				}
				if (c is System.Windows.Forms.ComboBox)
				{
					c.ForeColor = Color.Black;
					c.BackColor = Color.White;
				}
				if (c is System.Windows.Forms.TextBox)
				{
					c.ForeColor = Color.Black;
					c.BackColor = Color.White;
				}
			}
			return control;
		}
		public void SetButtonColors(object sender, Color OverallColor)
		{
			if (sender is ToggleButton)
			{

				var item = (sender as ToggleButton);
				if (item == null) return;
				if (item.Content == null) return;
				var objContent = (sender as ToggleButton).Content;
				if (objContent == null) return;
				if (objContent is System.Windows.Shapes.Rectangle)
				{
					System.Windows.Shapes.Rectangle content = objContent as System.Windows.Shapes.Rectangle;
					Color DisabledOpacity = Color.FromArgb(128, 0, 0, 0);
					System.Windows.Media.Color ConvertedColor = System.Windows.Media.Color.FromArgb((item.IsEnabled ? OverallColor.A : DisabledOpacity.A), OverallColor.R, OverallColor.G, OverallColor.B);
					content.Fill = new SolidColorBrush(ConvertedColor);

				}


			}

			if (sender is Button)
			{

				var item = (sender as Button);
				if (item == null) return;
				if (item.Content == null) return;
				var objContent = (sender as Button).Content;
				if (objContent == null) return;
				if (objContent is System.Windows.Shapes.Rectangle)
				{
					System.Windows.Shapes.Rectangle content = objContent as System.Windows.Shapes.Rectangle;
					Color DisabledOpacity = Color.FromArgb(128, 0, 0, 0);
					System.Windows.Media.Color ConvertedColor = System.Windows.Media.Color.FromArgb((item.IsEnabled ? OverallColor.A : DisabledOpacity.A), OverallColor.R, OverallColor.G, OverallColor.B);
					content.Fill = new SolidColorBrush(ConvertedColor);

				}


			}

			if (sender is MenuItem)
			{

				var item = (sender as MenuItem);
				if (item == null) return;
				if (item.Header == null) return;
				var objContent = (sender as MenuItem).Header;
				if (objContent == null) return;
				if (objContent is System.Windows.Shapes.Rectangle)
				{
					System.Windows.Shapes.Rectangle content = objContent as System.Windows.Shapes.Rectangle;
					Color DisabledOpacity = Color.FromArgb(128, 0, 0, 0);
					System.Windows.Media.Color ConvertedColor = System.Windows.Media.Color.FromArgb((item.IsEnabled ? OverallColor.A : DisabledOpacity.A), OverallColor.R, OverallColor.G, OverallColor.B);
					content.Fill = new SolidColorBrush(ConvertedColor);

				}


			}

			if (sender is SplitButton)
			{
				var item = (sender as SplitButton);
				if (item == null) return;
				if (item.Content == null) return;
				var objContent = (sender as SplitButton).Content;
				if (objContent == null) return;
				if (objContent is System.Windows.Shapes.Rectangle)
				{
					System.Windows.Shapes.Rectangle content = objContent as System.Windows.Shapes.Rectangle;
					Color DisabledOpacity = Color.FromArgb(128, 0, 0, 0);
					System.Windows.Media.Color ConvertedColor = System.Windows.Media.Color.FromArgb((item.IsEnabled ? OverallColor.A : DisabledOpacity.A), OverallColor.R, OverallColor.G, OverallColor.B);
					content.Fill = new SolidColorBrush(ConvertedColor);

				}

			}
		}
		public void UpdateButtonColors()
		{
			SetButtonColors(New, MainThemeColor());
			SetButtonColors(Open, MainThemeColor(Color.FromArgb(255, 231, 147), Color.FromArgb(250, 217, 98)));
			SetButtonColors(RecentDataDirectories, MainThemeColor(Color.FromArgb(255, 231, 147), Color.FromArgb(250, 217, 98)));
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
			SetButtonColors(RunSceneButton, Color.Green);
			SetButtonColors(MagnetModeSplitButton, MainThemeColor());
			SetButtonColors(GridSizeButton, MainThemeColor());
			SetButtonColors(RunSceneDropDown, MainThemeColor());
			SetButtonColors(RecentDataDirectories_DropDown, MainThemeColor());
			SetButtonColors(MagnetModeSplitDropDown, MainThemeColor());
			SetButtonColors(GridSizeButton, MainThemeColor());
			SetButtonColors(animationsSplitButton_Dropdown, MainThemeColor());
			SetButtonColors(MoreSettingsButton, MainThemeColor());

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

		#region Miscellaneous

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

		#region Orginized Tab Event Handlers

		#region File Tab Buttons

		private void New_Click(object sender, RoutedEventArgs e) { Interactions.New_Click(sender, e); }
		public void Open_Click(object sender, RoutedEventArgs e) { Interactions.Open_Click(sender, e); }
		private void OpenToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Interactions.OpenToolStripMenuItem_Click(sender, e); }
		public void OpenDataDirectoryToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Interactions.OpenDataDirectoryToolStripMenuItem_Click(sender, e); }
		public void Save_Click(object sender, RoutedEventArgs e) { Interactions.Save_Click(sender, e); }
		private void ExitToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Interactions.ExitToolStripMenuItem_Click(sender, e); }
		private void SaveAspngToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Interactions.SaveAspngToolStripMenuItem_Click(sender, e); }
		private void ExportEachLayerAspngToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Interactions.ExportEachLayerAspngToolStripMenuItem_Click(sender, e); }
		public void SaveAsToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Interactions.SaveAsToolStripMenuItem_Click(sender, e); }
		private void BackupToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Interactions.BackupToolStripMenuItem_Click(sender, e); }
		private void BackupRecoverButton_Click(object sender, RoutedEventArgs e) { Interactions.BackupRecoverButton_Click(sender, e); }
		private void ObjectManagerToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Interactions.ObjectManagerToolStripMenuItem_Click(sender, e); }
		public void UnloadSceneToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Interactions.UnloadSceneToolStripMenuItem_Click(sender, e); }
		private void StageConfigToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Interactions.StageConfigToolStripMenuItem_Click(sender, e); }
		private void NormalToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Interactions.NormalToolStripMenuItem_Click(sender, e); }
		#endregion

		#region Edit Tab Buttons
		public void chunkToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Interactions.chunkToolStripMenuItem_Click(sender, e); }
		public void SelectAllToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Interactions.SelectAllToolStripMenuItem_Click(sender, e); }
		public void FlipHorizontalToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Interactions.FlipHorizontalToolStripMenuItem_Click(sender, e); }
		public void FlipHorizontalIndividualToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Interactions.FlipHorizontalIndividualToolStripMenuItem_Click(sender, e); }
		private void DeleteToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Interactions.DeleteToolStripMenuItem_Click(sender, e); }
		public void CopyToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Interactions.CopyToolStripMenuItem_Click(sender, e); }
		public void DuplicateToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Interactions.DuplicateToolStripMenuItem_Click(sender, e); }
		private void UndoToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Interactions.UndoToolStripMenuItem_Click(sender, e); }
		private void RedoToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Interactions.RedoToolStripMenuItem_Click(sender, e); }
		public void CutToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Interactions.CutToolStripMenuItem_Click(sender, e); }
		public void PasteToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Interactions.PasteToolStripMenuItem_Click(sender, e); }
		public void FlipVerticalToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Interactions.FlipVerticalToolStripMenuItem_Click(sender, e); }
		public void FlipVerticalIndividualToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Interactions.FlipVerticalIndividualToolStripMenuItem_Click(sender, e); }
		#endregion

		#region View Tab Buttons
		public void statsToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Interactions.statsToolStripMenuItem_Click(sender, e); }
		private void PointerTooltipToggleToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Interactions.PointerTooltipToggleToolStripMenuItem_Click(sender, e); }
		private void ResetZoomLevelToolstripMenuItem_Click(object sender, RoutedEventArgs e) { Interactions.ResetZoomLevelToolstripMenuItem_Click(sender, e); }
		private void useLargeTextToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Interactions.useLargeTextToolStripMenuItem_Click(sender, e); }
		private void SetMenuButtons(object sender, RoutedEventArgs e) { Interactions.SetMenuButtons(sender, e); }
		private void SetMenuButtons(string tag) { Interactions.SetMenuButtons(tag); }
		private void ShowEntitiesAboveAllOtherLayersToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Interactions.ShowEntitiesAboveAllOtherLayersToolStripMenuItem_Click(sender, e); }
		private void prioritizedViewingToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Interactions.prioritizedViewingToolStripMenuItem_Click(sender, e); }
		private void ChangeEncorePaleteToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Interactions.ChangeEncorePaleteToolStripMenuItem_Click(sender, e); }
		public void SetEncorePallete(object sender = null, string path = "") { Interactions.SetEncorePallete(sender, path); }
		private void MoveExtraLayersToFrontToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Interactions.MoveExtraLayersToFrontToolStripMenuItem_Click(sender, e); }
		private void ToolStripTextBox1_TextChanged(object sender, RoutedEventArgs e) { Interactions.ToolStripTextBox1_TextChanged(sender, e); }
		private void ShowEntitySelectionBoxesToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Interactions.ShowEntitySelectionBoxesToolStripMenuItem_Click(sender, e); }
		private void ShowWaterLevelToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Interactions.ShowWaterLevelToolStripMenuItem_Click(sender, e); }
		private void WaterLevelAlwaysShowItem_Click(object sender, RoutedEventArgs e) { Interactions.WaterLevelAlwaysShowItem_Click(sender, e); }
		private void SizeWithBoundsWhenNotSelectedToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Interactions.SizeWithBoundsWhenNotSelectedToolStripMenuItem_Click(sender, e); }
		private void ToggleEncoreManiaObjectVisibilityToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Interactions.ToggleEncoreManiaObjectVisibilityToolStripMenuItem_Click(sender, e); }
		private void ShowParallaxSpritesToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Interactions.ShowParallaxSpritesToolStripMenuItem_Click(sender, e); }
		private void XToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Interactions.XToolStripMenuItem_Click(sender, e); }
		private void YToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Interactions.YToolStripMenuItem_Click(sender, e); }
		private void ShowEntityPathToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Interactions.ShowEntityPathToolStripMenuItem_Click(sender, e); }
		private void LangToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Interactions.LangToolStripMenuItem_Click(sender, e); }

		#region Collision Options
		private void DefaultToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Interactions.DefaultToolStripMenuItem_Click(sender, e); }
		private void InvertedToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Interactions.InvertedToolStripMenuItem_Click(sender, e); }
		private void CustomToolStripMenuItem1_Click(object sender, RoutedEventArgs e) { Interactions.CustomToolStripMenuItem1_Click(sender, e); }
		private void CollisionOpacitySlider_DragCompleted(object sender, DragCompletedEventArgs e) { Interactions.CollisionOpacitySlider_DragCompleted(sender, e); }
		private void CollisionOpacitySlider_LostFocus(object sender, RoutedEventArgs e) { Interactions.CollisionOpacitySlider_LostFocus(sender, e); }
		private void CollisionOpacitySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) { Interactions?.CollisionOpacitySlider_ValueChanged(sender, e); }
		#endregion

		#endregion

		#region Scene Tab Buttons
		public void ImportObjectsToolStripMenuItem_Click(object sender, RoutedEventArgs e, Window window = null) { Interactions.ImportObjectsToolStripMenuItem_Click(sender, e); }
		public void ImportSoundsToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Interactions.ImportSoundsToolStripMenuItem_Click(sender, e); }
		public void ImportSoundsToolStripMenuItem_Click(object sender, RoutedEventArgs e, Window window = null) { Interactions.ImportSoundsToolStripMenuItem_Click(sender, e, window); }
		private void LayerManagerToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Interactions.LayerManagerToolStripMenuItem_Click(sender, e); }
		private void PrimaryColorToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Interactions.PrimaryColorToolStripMenuItem_Click(sender, e); }
		private void SecondaryColorToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Interactions.SecondaryColorToolStripMenuItem_Click(sender, e); }
		#endregion

		#region Tools Tab Buttons
		private void OptimizeEntitySlotIDsToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Interactions.OptimizeEntitySlotIDsToolStripMenuItem_Click(sender, e); }
		private void RightClicktoSwapSlotIDs_Click(object sender, RoutedEventArgs e) { Interactions.RightClicktoSwapSlotIDs_Click(sender, e); }
		private void CopyAirToggle_Click(object sender, RoutedEventArgs e) { Interactions.CopyAirToggle_Click(sender, e); }
		private void changeLevelIDToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Interactions.changeLevelIDToolStripMenuItem_Click(sender, e); }
		private void MultiLayerSelectionToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Interactions.MultiLayerSelectionToolStripMenuItem_Click(sender, e); }
		private void MakeForDataFolderOnlyToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Interactions.MakeForDataFolderOnlyToolStripMenuItem_Click(sender, e); }
		private void WithCurrentCoordinatesToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Interactions.WithCurrentCoordinatesToolStripMenuItem_Click(sender, e); }
		private void WithoutCurrentCoordinatesToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Interactions.WithoutCurrentCoordinatesToolStripMenuItem_Click(sender, e); }
		private void SoundLooperToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Interactions.SoundLooperToolStripMenuItem_Click(sender, e); }
		private void FindUnusedTiles(object sender, RoutedEventArgs e) { Interactions.FindUnusedTiles(sender, e); }

		#region Developer Stuff
		public void GoToToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Interactions.GoToToolStripMenuItem_Click(sender, e); }
		public void PreLoadSceneButton_Click(object sender, RoutedEventArgs e) { Interactions.PreLoadSceneButton_Click(sender, e); }
		private void DeveloperTerminalToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Interactions.DeveloperTerminalToolStripMenuItem_Click(sender, e); }
		private void MD5GeneratorToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Interactions.MD5GeneratorToolStripMenuItem_Click(sender, e); }
		private void PlayerSpawnToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Interactions.PlayerSpawnToolStripMenuItem_Click(sender, e); }
		private void FindToolStripMenuItem1_Click(object sender, RoutedEventArgs e) { Interactions.FindToolStripMenuItem1_Click(sender, e); }
		private void ConsoleWindowToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Interactions.ConsoleWindowToolStripMenuItem_Click(sender, e); }
		private void SaveForForceOpenOnStartupToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Interactions.SaveForForceOpenOnStartupToolStripMenuItem_Click(sender, e); }
		private void LeftToolbarToggleDev_Click(object sender, RoutedEventArgs e) { UpdateToolbars(false, true); } 
		private void RightToolbarToggleDev_Click(object sender, RoutedEventArgs e) { UpdateToolbars(true, true); }
		private void EnableAllButtonsToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Interactions.EnableAllButtonsToolStripMenuItem_Click(sender, e); }
		private void NextGenSceneSelectTest_Click(object sender, RoutedEventArgs e) { EditorSceneLoading.OpenSceneUsingSceneSelect(); }
		#endregion

		#endregion

		#region Apps Tab Buttons
		private void TileManiacToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Interactions.TileManiacToolStripMenuItem_Click(sender, e); }
		private void InsanicManiacToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Interactions.InsanicManiacToolStripMenuItem_Click(sender, e); }
		private void RSDKAnnimationEditorToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Interactions.RSDKAnnimationEditorToolStripMenuItem_Click(sender, e); }
		private void ColorPaletteEditorToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Interactions.ColorPaletteEditorToolStripMenuItem_Click(sender, e); }
		private void ManiaPalMenuItem_SubmenuOpened(object sender, RoutedEventArgs e) { Interactions.ManiaPalMenuItem_SubmenuOpened(sender, e); }
		private void DuplicateObjectIDHealerToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Interactions.DuplicateObjectIDHealerToolStripMenuItem_Click(sender, e); }
		#endregion

		#region Folders Tab Buttons
		private void OpenSceneFolderToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Interactions.OpenSceneFolderToolStripMenuItem_Click(sender, e); }
		private void OpenDataDirectoryFolderToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Interactions.OpenDataDirectoryFolderToolStripMenuItem_Click(sender, e); }
		private void OpenSonicManiaFolderToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Interactions.OpenSonicManiaFolderToolStripMenuItem_Click(sender, e); }
		private void OpenModDataDirectoryToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Interactions.OpenModDataDirectoryToolStripMenuItem_Click(sender, e); }
		private void OpenASavedPlaceToolStripMenuItem_DropDownOpening(object sender, RoutedEventArgs e) { Interactions.OpenASavedPlaceToolStripMenuItem_DropDownOpening(sender, e); }
		private void OpenASavedPlaceToolStripMenuItem_DropDownClosed(object sender, RoutedEventArgs e) { Interactions.OpenASavedPlaceToolStripMenuItem_DropDownClosed(sender, e); }
		#endregion

		#region Other Tab Buttons
		public void AboutToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Interactions.AboutToolStripMenuItem_Click(sender, e); }
		private void WikiToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Interactions.WikiToolStripMenuItem_Click(sender, e); }
		public void OptionToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Interactions.OptionToolStripMenuItem_Click(sender, e); }
		private void ControlsToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Interactions.ControlsToolStripMenuItem_Click(sender, e); }
		#endregion

		#endregion

		#region Main Toolstrip Item's Event Handlers
		private void NewToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Interactions.NewToolStripMenuItem_Click(sender, e); }
		private void SToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Interactions.SToolStripMenuItem_Click(sender, e); }
		private void MagnetMode_Click(object sender, RoutedEventArgs e) { Interactions.MagnetMode_Click(sender, e); }
		private void UndoButton_Click(object sender, RoutedEventArgs e) { Interactions.UndoButton_Click(sender, e); }
		private void RedoButton_Click(object sender, RoutedEventArgs e) { Interactions.RedoButton_Click(sender, e); }
		private void ZoomInButton_Click(object sender, RoutedEventArgs e) { Interactions.ZoomInButton_Click(sender, e); }
		private void ZoomOutButton_Click(object sender, RoutedEventArgs e) { Interactions.ZoomOutButton_Click(sender, e); }
		private void SelectTool_Click(object sender, RoutedEventArgs e) { Interactions.SelectTool_Click(sender, e); }
		private void PointerButton_Click(object sender, RoutedEventArgs e) { Interactions.PointerButton_Click(sender, e); }
		private void PlaceTilesButton_Click(object sender, RoutedEventArgs e) { Interactions.PlaceTilesButton_Click(sender, e); }
		private void InteractionToolButton_Click(object sender, RoutedEventArgs e) { Interactions.InteractionToolButton_Click(sender, e); }
		private void ChunkToolButton_Click(object sender, RoutedEventArgs e) { Interactions.ChunkToolButton_Click(sender, e); }
		public void ReloadToolStripButton_Click(object sender, RoutedEventArgs e) { Interactions.ReloadToolStripButton_Click(sender, e); }
		public void ShowTileIDButton_Click(object sender, RoutedEventArgs e) { Interactions.ShowTileIDButton_Click(sender, e); }

		#region Magnet Mode Methods/Buttons

		private void X8ToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			magnetSize = 8;
			ResetMagnetModeOptions();
			x8ToolStripMenuItem.IsChecked = true;
		}

		private void X16ToolStripMenuItem1_Click(object sender, RoutedEventArgs e)
		{
			magnetSize = 16;
			ResetMagnetModeOptions();
			x16ToolStripMenuItem1.IsChecked = true;
		}

		private void X32ToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			magnetSize = 32;
			ResetMagnetModeOptions();
			x32ToolStripMenuItem.IsChecked = true;
		}

		private void X64ToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			magnetSize = 64;
			ResetMagnetModeOptions();
			x64ToolStripMenuItem.IsChecked = true;
		}

		private void ResetMagnetModeOptions()
		{
			x16ToolStripMenuItem1.IsChecked = false;
			x8ToolStripMenuItem.IsChecked = false;
			x32ToolStripMenuItem.IsChecked = false;
			x64ToolStripMenuItem.IsChecked = false;
		}

		private void EnableXAxisToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (useMagnetXAxis)
			{
				enableXAxisToolStripMenuItem.IsChecked = false;
				useMagnetXAxis = false;
			}
			else
			{
				enableXAxisToolStripMenuItem.IsChecked = true;
				useMagnetXAxis = true;
			}
		}

		private void EnableYAxisToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (useMagnetYAxis)
			{
				enableYAxisToolStripMenuItem.IsChecked = false;
				useMagnetYAxis = false;
			}
			else
			{
				enableYAxisToolStripMenuItem.IsChecked = true;
				useMagnetYAxis = true;
			}
		}

		#endregion

		#region Run Scene Events
		private void RunScene_Click(object sender, RoutedEventArgs e) { Interactions.RunScene_Click(sender, e); }
		private void OpenModManagerToolStripMenuItem_Click(object sender, RoutedEventArgs e)
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
					if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
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
		private void TrackPlayerToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			MenuItem item = sender as MenuItem;
			if (item != null)
			{

				if (!item.IsChecked)
				{
					UncheckAllPlayers();
					item.IsChecked = true;
					int.TryParse(item.Tag.ToString(), out int player);
					PlayerBeingTracked = player;
				}
				else
				{
					item.IsChecked = false;
					PlayerBeingTracked = -1;
				}


			}
		}
		private void UncheckAllPlayers()
		{
			trackP1ToolStripMenuItem.IsChecked = false;
			trackP2ToolStripMenuItem.IsChecked = false;
			trackP3ToolStripMenuItem.IsChecked = false;
			trackP4ToolStripMenuItem.IsChecked = false;
		}
		private void RunSceneButton_DropDownOpening(object sender, RoutedEventArgs e) { Interactions.RunSceneButton_DropDownOpening(sender, e); }

		#region Mod Config List Stuff
		private MenuItem CreateModConfigMenuItem(int i)
		{
			MenuItem newItem = new MenuItem()
			{
				Header = mySettings.modConfigsNames[i],
				Tag = mySettings.modConfigs[i]
			};
			newItem.Click += ModConfigItemClicked;
			if (newItem.Tag.ToString() == mySettings.LastModConfig) newItem.IsChecked = true;
			return newItem;
		}

		private void ModConfigItemClicked(object sender, RoutedEventArgs e)
		{
			var modConfig_CheckedItem = (sender as MenuItem);
			SelectConfigToolStripMenuItem_Click(modConfig_CheckedItem);
			mySettings.LastModConfig = modConfig_CheckedItem.Tag.ToString();
		}

		public void EditConfigsToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			Interfaces.WPF_UI.ConfigManager configManager = new Interfaces.WPF_UI.ConfigManager();
			configManager.Owner = GetWindow(this);
			configManager.ShowDialog();

			// TODO: Fix NullReferenceException on mySettings.modConfigs
			selectConfigToolStripMenuItem.Items.Clear();
			for (int i = 0; i < mySettings.modConfigs.Count; i++)
			{
				selectConfigToolStripMenuItem.Items.Add(CreateModConfigMenuItem(i));
			}
		}

		private void SelectConfigToolStripMenuItem_Click(MenuItem modConfig_CheckedItem)
		{
			var allItems = selectConfigToolStripMenuItem.Items.Cast<System.Windows.Controls.MenuItem>().ToArray();
			foreach (var item in allItems)
			{
				item.IsChecked = false;
			}
			modConfig_CheckedItem.IsChecked = true;

		}

		#endregion

		#endregion

		#region Grid Options
		public void ShowGridButton_Click(object sender, RoutedEventArgs e) { Interactions.ShowGridButton_Click(sender, e); }
		public void GridCheckStateCheck() { Interactions.GridCheckStateCheck(); }
		private void X16ToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Interactions.X16ToolStripMenuItem_Click(sender, e); }
		private void X128ToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Interactions.X128ToolStripMenuItem_Click(sender, e); }
		private void ResetGridOptions() { Interactions.ResetGridOptions(); }
		private void X256ToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Interactions.X256ToolStripMenuItem_Click(sender, e); }
		private void CustomToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Interactions.CustomToolStripMenuItem_Click(sender, e); }
		#endregion

		public void ShowCollisionAButton_Click(object sender, RoutedEventArgs e) { Interactions.ShowCollisionAButton_Click(sender, e); }
		public void ShowCollisionBButton_Click(object sender, RoutedEventArgs e) { Interactions.ShowCollisionBButton_Click(sender, e); }
		private void OpenDataDirectoryMenuButton(object sender, RoutedEventArgs e) { Interactions.OpenDataDirectoryMenuButton(sender, e); }
		private void ResetDeviceButton_Click_1(object sender, RoutedEventArgs e) { Interactions.ResetDeviceButton_Click_1(sender, e); }
		private void ShowFlippedTileHelper_Click(object sender, RoutedEventArgs e) { Interactions.ShowFlippedTileHelper_Click(sender, e); }
		public void EnableEncorePalette_Click(object sender, RoutedEventArgs e) { Interactions.EnableEncorePalette_Click(); }


		#endregion

		#region Layer Toolbar Items
		private void LayerShowButton_Click(ToggleButton button, string desc)
		{
			if (button.IsChecked.Value)
			{
				button.IsChecked = false;
				button.ToolTip = "Show " + desc;
			}
			else
			{
				button.IsChecked = true;
				button.ToolTip = "Hide " + desc;
			}
		}

		private void ShowFGLow_Click(object sender, RoutedEventArgs e)
		{
			ToggleButton toggle = sender as ToggleButton;
			toggle.IsChecked = !toggle.IsChecked.Value;
			LayerShowButton_Click(ShowFGLow, "Layer FG Low");
		}

		private void ShowFGHigh_Click(object sender, RoutedEventArgs e)
		{
			ToggleButton toggle = sender as ToggleButton;
			toggle.IsChecked = !toggle.IsChecked.Value;
			LayerShowButton_Click(ShowFGHigh, "Layer FG High");
		}

		private void ShowFGHigher_Click(object sender, RoutedEventArgs e)
		{
			ToggleButton toggle = sender as ToggleButton;
			toggle.IsChecked = !toggle.IsChecked.Value;
			LayerShowButton_Click(ShowFGHigher, "Layer FG Higher");
		}

		private void ShowFGLower_Click(object sender, RoutedEventArgs e)
		{
			ToggleButton toggle = sender as ToggleButton;
			toggle.IsChecked = !toggle.IsChecked.Value;
			LayerShowButton_Click(ShowFGLower, "Layer FG Lower");
		}

		private void ShowEntities_Click(object sender, RoutedEventArgs e)
		{
			ToggleButton toggle = sender as ToggleButton;
			toggle.IsChecked = !toggle.IsChecked.Value;
			LayerShowButton_Click(ShowEntities, "Entities");
		}

		private void ShowAnimations_Click(object sender, RoutedEventArgs e)
		{
			ToggleButton toggle = sender as ToggleButton;
			toggle.IsChecked = !toggle.IsChecked.Value;
			LayerShowButton_Click(ShowAnimations, "Animations");
		}

		#region Animations DropDown
		private void MovingPlatformsObjectsToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (MovingPlatformsChecked == false)
			{
				movingPlatformsObjectsToolStripMenuItem.IsChecked = true;
				MovingPlatformsChecked = true;
			}
			else
			{
				movingPlatformsObjectsToolStripMenuItem.IsChecked = false;
				MovingPlatformsChecked = false;
			}

		}

		private void SpriteFramesToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (AnnimationsChecked == false)
			{
				spriteFramesToolStripMenuItem.IsChecked = true;
				AnnimationsChecked = true;
			}
			else
			{
				spriteFramesToolStripMenuItem.IsChecked = false;
				AnnimationsChecked = false;
			}
		}

		#endregion

		private void LayerEditButton_Click(EditLayerToggleButton button, MouseButton ClickType)
		{
			if (MultiLayerEditMode)
			{
				if (ClickType == MouseButton.Left) LayerA();
				else if (ClickType == MouseButton.Right) LayerB();
			}
			else
			{
				if (ClickType == MouseButton.Left) Normal();
			}
			UpdateControls();



			void Normal()
			{
				Deselect(false);
				if (!button.IsCheckedN.Value)
				{
					button.IsCheckedN = false;
				}
				else
				{
					EditFGLow.IsCheckedN = false;
					EditFGHigh.IsCheckedN = false;
					EditFGLower.IsCheckedN = false;
					EditFGHigher.IsCheckedN = false;
					EditEntities.IsCheckedN = false;
					button.IsCheckedN = true;
				}

				foreach (var elb in ExtraLayerEditViewButtons.Values)
				{
					elb.IsCheckedN = false;
				}



			}
			void LayerA()
			{
				Deselect(false);
				if (!button.IsCheckedA.Value)
				{
					button.IsCheckedA = false;
				}
				else
				{
					EditFGLow.IsCheckedA = false;
					EditFGHigh.IsCheckedA = false;
					EditFGLower.IsCheckedA = false;
					EditFGHigher.IsCheckedA = false;
					EditEntities.IsCheckedA = false;
					button.IsCheckedA = true;
				}

				foreach (var elb in ExtraLayerEditViewButtons.Values)
				{
					elb.IsCheckedA = false;
				}
			}
			void LayerB()
			{
				Deselect(false);
				if (!button.IsCheckedB.Value)
				{
					button.IsCheckedB = false;
				}
				else
				{
					EditFGLow.IsCheckedB = false;
					EditFGHigh.IsCheckedB = false;
					EditFGLower.IsCheckedB = false;
					EditFGHigher.IsCheckedB = false;
					EditEntities.IsCheckedB = false;
					button.IsCheckedB = true;
				}

				foreach (var elb in ExtraLayerEditViewButtons.Values)
				{
					elb.IsCheckedB = false;
				}
			}
		}

		private void EditFGLow_Click(object sender, RoutedEventArgs e)
		{
			EditLayerToggleButton toggle = sender as EditLayerToggleButton;
			LayerEditButton_Click(EditFGLow, MouseButton.Left);
		}

		private void EditFGLow_RightClick(object sender, RoutedEventArgs e)
		{
			EditLayerToggleButton toggle = sender as EditLayerToggleButton;
			LayerEditButton_Click(EditFGLow, MouseButton.Right);
		}

		private void EditFGHigh_Click(object sender, RoutedEventArgs e)
		{
			EditLayerToggleButton toggle = sender as EditLayerToggleButton;
			LayerEditButton_Click(EditFGHigh, MouseButton.Left);
		}

		private void EditFGHigh_RightClick(object sender, RoutedEventArgs e)
		{
			EditLayerToggleButton toggle = sender as EditLayerToggleButton;
			LayerEditButton_Click(EditFGHigh, MouseButton.Right);
		}

		private void EditFGLower_Click(object sender, RoutedEventArgs e)
		{
			EditLayerToggleButton toggle = sender as EditLayerToggleButton;
			LayerEditButton_Click(EditFGLower, MouseButton.Left);
		}

		private void EditFGLower_RightClick(object sender, RoutedEventArgs e)
		{
			EditLayerToggleButton toggle = sender as EditLayerToggleButton;
			LayerEditButton_Click(EditFGLower, MouseButton.Right);
		}

		private void EditFGHigher_Click(object sender, RoutedEventArgs e)
		{
			EditLayerToggleButton toggle = sender as EditLayerToggleButton;
			LayerEditButton_Click(EditFGHigher, MouseButton.Left);
		}

		private void EditFGHigher_RightClick(object sender, RoutedEventArgs e)
		{
			EditLayerToggleButton toggle = sender as EditLayerToggleButton;
			LayerEditButton_Click(EditFGHigher, MouseButton.Right);
		}

		private void EditEntities_Click(object sender, RoutedEventArgs e)
		{
			EditLayerToggleButton toggle = sender as EditLayerToggleButton;
			LayerEditButton_Click(EditEntities, MouseButton.Left);
		}

		public void SetupLayerButtons()
		{
			TearDownExtraLayerButtons();
			IList<EditLayerToggleButton> _extraLayerEditButtons = new List<EditLayerToggleButton>(); //Used for Extra Layer Edit Buttons
			IList<EditLayerToggleButton> _extraLayerViewButtons = new List<EditLayerToggleButton>(); //Used for Extra Layer View Buttons

			//EDIT BUTTONS
			foreach (EditorLayer el in EditorScene.OtherLayers)
			{
				EditLayerToggleButton tsb = new EditLayerToggleButton()
				{
					Text = el.Name,
					Name = "Edit" + el.Name.Replace(" ", "")
				};
				LayerToolbar.Items.Add(tsb);
				tsb.TextForeground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(Color.LawnGreen.A, Color.LawnGreen.R, Color.LawnGreen.G, Color.LawnGreen.B));
				tsb.RightClick += AdHocLayerEdit_RightClick;
				tsb.Click += AdHocLayerEdit_Click;

				_extraLayerEditButtons.Add(tsb);
			}

			//EDIT BUTTONS SEPERATOR
			Separator tss = new Separator();
			LayerToolbar.Items.Add(tss);
			_extraLayerSeperators.Add(tss);

			//VIEW BUTTONS
			foreach (EditorLayer el in EditorScene.OtherLayers)
			{
				EditLayerToggleButton tsb = new EditLayerToggleButton()
				{
					Text = el.Name,
					Name = "Show" + el.Name.Replace(" ", "")
				};
				//toolStrip1.Items.Add(tsb);
				LayerToolbar.Items.Insert(LayerToolbar.Items.IndexOf(extraViewLayersSeperator), tsb);
				tsb.TextForeground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, Color.FromArgb(0x33AD35).R, Color.FromArgb(0x33AD35).G, Color.FromArgb(0x33AD35).B));

				_extraLayerViewButtons.Add(tsb);
			}

			//EDIT + VIEW BUTTONS LIST
			for (int i = 0; i < _extraLayerViewButtons.Count; i++)
			{
				ExtraLayerEditViewButtons.Add(_extraLayerViewButtons[i], _extraLayerEditButtons[i]);
			}

			UpdateDualButtonsControlsForLayer(FGLow, ShowFGLow, EditFGLow);
			UpdateDualButtonsControlsForLayer(FGHigh, ShowFGHigh, EditFGHigh);
			UpdateDualButtonsControlsForLayer(FGLower, ShowFGLower, EditFGLower);
			UpdateDualButtonsControlsForLayer(FGHigher, ShowFGHigher, EditFGHigher);
		}

		public void TearDownExtraLayerButtons()
		{
			foreach (var elb in ExtraLayerEditViewButtons)
			{
				LayerToolbar.Items.Remove(elb.Key);
				elb.Value.RightClick -= AdHocLayerEdit_RightClick;
				LayerToolbar.Items.Remove(elb.Value);
			}
			ExtraLayerEditViewButtons.Clear();


			foreach (var els in _extraLayerSeperators)
			{
				LayerToolbar.Items.Remove(els);
			}
			_extraLayerSeperators.Clear();

		}

		/// <summary>
		/// Given a scene layer, configure the given visibiltiy and edit buttons which will control that layer.
		/// </summary>
		/// <param name="layer">The layer of the scene from which to extract a name.</param>
		/// <param name="visibilityButton">The button which controls the visibility of the layer.</param>
		/// <param name="editButton">The button which controls editing the layer.</param>
		private void UpdateDualButtonsControlsForLayer(EditorLayer layer, ToggleButton visibilityButton, EditLayerToggleButton editButton)
		{
			bool layerValid = layer != null;
			visibilityButton.IsChecked = layerValid;
			if (layerValid)
			{
				string name = layer.Name;
				visibilityButton.Content = name;
				editButton.Text = name.ToString();
			}
		}

		private void AdHocLayerEdit_RightClick(object sender, RoutedEventArgs e)
		{
			AdHocLayerEdit(sender, MouseButton.Right);
		}

		private void AdHocLayerEdit_Click(object sender, RoutedEventArgs e)
		{
			AdHocLayerEdit(sender, MouseButton.Left);
		}

		private void AdHocLayerEdit(object sender, MouseButton ClickType)
		{
			if (ClickType == MouseButton.Left && !MultiLayerEditMode) Normal();
			else if (ClickType == MouseButton.Left && MultiLayerEditMode) LayerA();
			else if (ClickType == MouseButton.Right && MultiLayerEditMode) LayerB();

			void Normal()
			{
				EditLayerToggleButton tsb = sender as EditLayerToggleButton;
				Deselect(false);
				if (tsb.IsCheckedN.Value)
				{
					if (!mySettings.KeepLayersVisible)
					{
						ShowFGLow.IsChecked = false;
						ShowFGHigh.IsChecked = false;
						ShowFGLower.IsChecked = false;
						ShowFGHigher.IsChecked = false;
					}
					EditFGLow.ClearCheckedItems(3);
					EditFGHigh.ClearCheckedItems(3);
					EditFGLower.ClearCheckedItems(3);
					EditFGHigher.ClearCheckedItems(3);
					EditEntities.ClearCheckedItems(3);

					foreach (var elb in ExtraLayerEditViewButtons)
					{
						if (elb.Value != tsb)
						{
							elb.Value.IsCheckedN = false;
						}
					}
				}
			}
			void LayerA()
			{
				EditLayerToggleButton tsb = sender as EditLayerToggleButton;
				Deselect(false);
				if (tsb.IsCheckedA.Value)
				{
					if (!mySettings.KeepLayersVisible)
					{
						ShowFGLow.IsChecked = false;
						ShowFGHigh.IsChecked = false;
						ShowFGLower.IsChecked = false;
						ShowFGHigher.IsChecked = false;
					}
					EditFGLow.ClearCheckedItems(1);
					EditFGHigh.ClearCheckedItems(1);
					EditFGLower.ClearCheckedItems(1);
					EditFGHigher.ClearCheckedItems(1);
					EditEntities.ClearCheckedItems(1);

					foreach (var elb in ExtraLayerEditViewButtons)
					{
						if (elb.Value != tsb)
						{
							elb.Value.IsCheckedA = false;
						}
					}
				}
			}
			void LayerB()
			{
				EditLayerToggleButton tsb = sender as EditLayerToggleButton;
				Deselect(false);
				if (tsb.IsCheckedB.Value)
				{
					if (!mySettings.KeepLayersVisible)
					{
						ShowFGLow.IsChecked = false;
						ShowFGHigh.IsChecked = false;
						ShowFGLower.IsChecked = false;
						ShowFGHigher.IsChecked = false;
					}
					EditFGLow.ClearCheckedItems(2);
					EditFGHigh.ClearCheckedItems(2);
					EditFGLower.ClearCheckedItems(2);
					EditFGHigher.ClearCheckedItems(2);
					EditEntities.ClearCheckedItems(2);

					foreach (var elb in ExtraLayerEditViewButtons)
					{
						if (elb.Value != tsb)
						{
							elb.Value.IsCheckedB = false;
						}
					}
				}
			}

			UpdateControls();
		}
		#endregion

		#region Mouse Actions Event Handlers
		private void GraphicPanel_OnMouseMove(object sender, System.Windows.Forms.MouseEventArgs e) { EditorControls.MouseMove(sender, e); }
		private void GraphicPanel_OnMouseDown(object sender, System.Windows.Forms.MouseEventArgs e) { EditorControls.MouseDown(sender, e); }
		private void GraphicPanel_OnMouseUp(object sender, System.Windows.Forms.MouseEventArgs e) { EditorControls.MouseUp(sender, e); }
		private void GraphicPanel_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e) { EditorControls.MouseWheel(sender, e); }
		private void GraphicPanel_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e) { EditorControls.MouseClick(sender, e); }
		#endregion

		#region Status Bar Event Handlers
		private void PixelModeButton_Click(object sender, RoutedEventArgs e) { Interactions.PixelModeButton_Click(sender, e); }
		public void TooltipButton_Click(object sender, RoutedEventArgs e) { Interactions.TooltipButton_Click(sender, e); }
		public void ScrollLockButton_Click(object sender, RoutedEventArgs e) { Interactions.ScrollLockButton_Click(sender, e); }
		public void NudgeFasterButton_Click(object sender, RoutedEventArgs e) { Interactions.NudgeFasterButton_Click(sender, e); }

		#region Quick Button Event Handlers
		public void MoreSettingsButton_ButtonClick(object sender, RoutedEventArgs e) { Interactions.MoreSettingsButton_ButtonClick(sender, e); }
		public void SwapScrollLockDirectionToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Interactions.SwapScrollLockDirectionToolStripMenuItem_Click(sender, e); }
		public void EditEntitesTransparencyToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Interactions.EditEntitesTransparencyToolStripMenuItem_Click(sender, e); }
		public void ToggleEncoreManiaEntitiesToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Interactions.ToggleEncoreManiaEntitiesToolStripMenuItem_Click(sender, e); }
		#endregion

		#endregion

		#region Interaction Tool Items
		private void MoveThePlayerToHereToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (GameRunning)
			{
				int ObjectAddress = 0x85E9A0;
				GameMemory.WriteInt16(ObjectAddress + 2, (short)(lastX / Zoom));
				GameMemory.WriteInt16(ObjectAddress + 6, (short)(lastY / Zoom));
			}
		}

		private void EditTileWithTileManiacToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (mainform == null || mainform.IsClosed) mainform = new TileManiacWPF.MainWindow();
			if (mainform.Visibility != Visibility.Visible)
			{
				mainform.Show();
			}
			mainform.SetIntergrationNightMode(Properties.Settings.Default.NightMode);
			if (TilesConfig != null && StageTiles != null)
			{
				if (mainform.Visibility != Visibility.Visible || mainform.tcf == null)
				{
					mainform.LoadTileConfigViaIntergration(TilesConfig, EditorPath.SceneFile_Directory, SelectedTileID);
				}
				else
				{
					mainform.SetCollisionIndex(SelectedTileID);
					mainform.Activate();
				}

			}
		}

		private void SetPlayerRespawnToHereToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			Point clicked_point = new Point((int)(lastX / Zoom), (int)(lastY / Zoom));
			if (GameRunning)
			{
				EditorGame.UpdateCheckpoint(clicked_point);
			}
		}

		private void MoveCheckpointToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			checkpointSelected = true;
		}

		private void RemoveCheckpointToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			EditorGame.UpdateCheckpoint(new Point(0, 0), false);
		}

		private void AssetResetToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			EditorGame.AssetReset();
		}

		private void RestartSceneToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			EditorGame.RestartScene();
		}
		#endregion

	}
}
