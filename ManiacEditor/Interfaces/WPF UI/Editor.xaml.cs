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
using ManiacEditor.Enums;
using RSDKv5;
using SharpDX.Direct3D9;
using Color = System.Drawing.Color;
using System.Reflection;
using ManiacEditor.Interfaces;
using Cyotek.Windows.Forms;
using Microsoft.Scripting.Utils;
using TileManiacWPF;
using Microsoft.Win32;
using ManiacEditor.Entity_Renders;
using IWshRuntimeLibrary;
using System.Drawing;
using File = System.IO.File;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Net.Sockets;
using System.Windows.Forms.VisualStyles;
using SaveFileDialog = System.Windows.Forms.SaveFileDialog;
using OpenFileDialog = System.Windows.Forms.OpenFileDialog;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit;
using ManiacEditor;
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
using Application = System.Windows.Application;
using Button = System.Windows.Controls.Button;
using Cursors = System.Windows.Input.Cursors;


namespace ManiacEditor
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class Editor : Window
	{
		#region Definitions
		//Editor Editing States
		bool dragged;
		bool startDragged;
		public int lastX, lastY, draggedX, draggedY;
		public int ShiftX = 0, ShiftY = 0, ScreenWidth, ScreenHeight;
		public int CustomX = 0, CustomY = 0;
		public int select_x1, select_x2, select_y1, select_y2;
		int ClickedX = -1, ClickedY = -1;
		bool draggingSelection; //Determines if we are dragging a selection
		int selectingX, selectingY;
		bool zooming; //Detects if we are zooming
		public double Zoom = 1; //Double Value for Zoom Levels
		int ZoomLevel = 0; //Interger Value for Zoom Levels
		public int SelectedTilesCount; //Used to get the Amount of Selected Tiles in a Selection
		public int DeselectTilesCount; //Used in combination with SelectedTilesCount to get the definitive amount of Selected Tiles
		public int SelectedTileX = 0; //Used to get a single Selected Tile's X
		public int SelectedTileY = 0; //Used to get a single Selected Tile's Y
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
		public bool showParallaxSprites = false; //Determines if we should show the parallax sprites
		public bool applyEditEntitiesTransparency = false; //Determines if the other layers should be semi-transparent when editing entities.
		public bool showEntitySelectionBoxes = false; //Determines if we should show the entity selection boxes.
		public bool EnablePixelCountMode = false; //Count the selection in pixels per tile or not
		public bool isConsoleWindowOpen = false; //Show the Console Window or not

		//Editor Status States (Like are we pre-loading a scene)
		public bool importingObjects = false; //Determines if we are importing objects so we can disable all the other Scene Select Options
		public bool isPreRending = false; //Determines if we are Preloading a Scene
		bool encorePaletteExists = false; // Determines if an Encore Pallete Exists
		int SelectedTileID = -1; //For Tile Maniac Intergration via Right Click in Editor View Panel
		public string CurrentLanguage = "EN"; //Current Selected Language
		Point TempWarpCoords = new Point(0, 0); //Temporary Warp Position for Shortcuts and Force Open
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


		//Editor Variable States (Like Scroll Lock is in the X Direction)
		string scrollDirection = "X"; //Determines Scroll Lock Direction
		int magnetSize = 16; //Determines the Magnets Size
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
		public string ModDataDirectory = ""; //Used as a way of allowing mods to not have to lug all the files in their folder just to load in Maniac.
		public string SelectedZone; //Used to get the Selected zone
		string SelectedScene; //Used to get the Scene zone
		public string[] EncorePalette = new string[6]; //Used to store the location of the encore palletes
		string SceneFilename = null; //Used for fetching the scene's file name
		public string SceneFilepath = null; //Used for fetching the folder that contains the scene file
		string StageConfigFileName = null; //Used for fetch the scene's stage config file name

		// Extra Layer Buttons
		private IList<ToggleButton> _extraLayerEditButtons; //Used for Extra Layer Edit Buttons
		private IList<ToggleButton> _extraLayerViewButtons; //Used for Extra Layer View Buttons
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
		internal int SceneWidth => EditorScene.Layers.Max(sl => sl.Width) * 16;
		internal int SceneHeight => EditorScene.Layers.Max(sl => sl.Height) * 16;

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
		public Dictionary<Point, ushort> TilesClipboard;
		public Dictionary<Point, ushort> FindReplaceClipboard;
		public Dictionary<Point, ushort> TilesClipboardEditable;
		private List<EditorEntity> entitiesClipboard;

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
		public ManiacEditor.Interfaces.DeveloperTerminal DevController;
		public EditorEntities entities;
		//public int InstanceID = 0;
		//public static Editor Instance; //Used the access this class easier
		//public Editor ThisInstance;
		internal EditorBackground EditorBackground;
		public EditorLayer EditLayer;
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

		//Tile Maniac + ManiaPal Instance
		public TileManiacWPF.MainWindow mainform = new TileManiacWPF.MainWindow();
		public static ManiaPal.App app;
		public static ManiaPal.MainWindow ManiaPalInstance;

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

			_extraLayerEditButtons = new List<ToggleButton>();
			_extraLayerViewButtons = new List<ToggleButton>();
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
			DevController = new ManiacEditor.Interfaces.DeveloperTerminal(this);

			this.Title = String.Format("Maniac Editor - Generations Edition {0}", Updater.GetVersion());
			editorView.GraphicPanel.Width = SystemInformation.PrimaryMonitorSize.Width;
			editorView.GraphicPanel.Height = SystemInformation.PrimaryMonitorSize.Height;

			ViewPanelContextMenu.Foreground = (SolidColorBrush)FindResource("NormalText");
			ViewPanelContextMenu.Background = (SolidColorBrush)FindResource("NormalBackground");

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

		#region Discord Rich Presence


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

			mySettings.scrollLock = mySettings.ScrollLockDefault;
			statusNAToolStripMenuItem.IsChecked = mySettings.ScrollLockDefault;
			scrollLockButton.IsChecked = mySettings.ScrollLockDefault;


			xToolStripMenuItem.IsChecked = mySettings.ScrollLockDirection;
			yToolStripMenuItem.IsChecked = !mySettings.ScrollLockDirection;

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
		void UseDefaultPrefrences()
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

		}

		#endregion

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
			return EditLayer != null;
		}

		public bool IsChunksEdit()
		{
			return ChunksToolButton.IsChecked.Value && EditLayer != null;
		}

		public bool IsEntitiesEdit()
		{
			return EditEntities.IsChecked.Value;
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
			return System.Windows.Forms.Control.ModifierKeys.HasFlag(System.Windows.Forms.Keys.Control);
		}

		private bool ShiftPressed()
		{
			return System.Windows.Forms.Control.ModifierKeys.HasFlag(System.Windows.Forms.Keys.Alt);
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
			saveToolStripMenuItem.IsEnabled = enabled;
			saveAsToolStripMenuItem.IsEnabled = enabled;
			backupToolStripMenuItem.IsEnabled = enabled;
			unloadSceneToolStripMenuItem.IsEnabled = enabled;
			goToToolStripMenuItem1.IsEnabled = enabled;
			specificPlaceToolStripMenuItem.IsEnabled = enabled;
			playerSpawnToolStripMenuItem.IsEnabled = enabled;

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
				MessageBoxResult result = System.Windows.MessageBox.Show("Do you wish to Pre-Render this scene?", "Requesting to Pre-Render the Scene", MessageBoxButton.YesNo, MessageBoxImage.Information);
				if (result == MessageBoxResult.Yes)
				{
					PreLoadSceneButton_Click(null, null);
				}
			}
			else if (mySettings.preRenderSceneOption == 1 && PreRenderSceneSelectCheckbox && enabled && stageLoad)
			{
				PreLoadSceneButton_Click(null, null);
			}

			UpdateButtonColors();
		}

		private void SetSelectOnlyButtonsState(bool enabled = true)
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

			selectAllToolStripMenuItem.IsEnabled = IsTilesEdit() || IsEntitiesEdit();

			if (IsEntitiesEdit())
			{
				entitiesToolbar.SelectedEntities = entities.SelectedEntities.Select(x => x.Entity).ToList();
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

			if (enabled && EditFGLow.IsChecked.Value) EditLayer = FGLow;
			else if (enabled && EditFGHigh.IsChecked.Value) EditLayer = FGHigh;
			else if (enabled && EditFGHigher.IsChecked.Value) EditLayer = FGHigher;
			else if (enabled && EditFGLower.IsChecked.Value) EditLayer = FGLower;
			else if (enabled && _extraLayerEditButtons.Any(elb => elb.IsChecked.Value))
			{
				var selectedExtraLayerButton = _extraLayerEditButtons.Single(elb => elb.IsChecked.Value);
				var editorLayer = EditorScene.OtherLayers.Single(el => el.Name.Equals(selectedExtraLayerButton.Content));

				EditLayer = editorLayer;
			}
			else EditLayer = null;

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
			SelectToolButton.IsEnabled = enabled && IsTilesEdit() && !IsChunksEdit();
			PlaceTilesButton.IsEnabled = enabled && IsTilesEdit();
			InteractionToolButton.IsEnabled = enabled;
			ChunksToolButton.IsEnabled = enabled && IsTilesEdit();

			PointerButton.IsChecked = (bool)PointerButton.IsChecked || (!(bool)PointerButton.IsChecked && !(bool)SelectToolButton.IsChecked && !(bool)PlaceTilesButton.IsChecked);
			if ((bool)SelectToolButton.IsChecked && IsChunksEdit())
			{
				SelectToolButton.IsChecked = false;
				PlaceTilesButton.IsChecked = true;
			}
			PlaceTilesButton.IsChecked = PlaceTilesButton.IsChecked;
			InteractionToolButton.IsChecked = InteractionToolButton.IsChecked;
			ChunksToolButton.IsChecked = (bool)ChunksToolButton.IsChecked && !IsEntitiesEdit();





			ShowGridButton.IsEnabled = enabled && StageConfig != null;
			ShowCollisionAButton.IsEnabled = enabled && StageConfig != null;
			ShowCollisionBButton.IsEnabled = enabled && StageConfig != null;
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
						TilesToolbar = new TilesToolbar(StageTiles, SceneFilepath, EncorePalette[0], this);
					else
						TilesToolbar = new TilesToolbar(StageTiles, SceneFilepath, null, this);

					
					TilesToolbar.TileDoubleClick = new Action<int>(x =>
					{
						EditorPlaceTile(new Point((int)(ShiftX / Zoom) + EditorLayer.TILE_SIZE - 1, (int)(ShiftY / Zoom) + EditorLayer.TILE_SIZE - 1), x);
					});
					TilesToolbar.TileOptionChanged = new Action<int, bool>((option, state) =>
					{
						EditLayer.SetPropertySelected(option + 12, state);

					});
					ToolBarPanelRight.Children.Clear();
					ToolBarPanelRight.Children.Add(TilesToolbar);
					UpdateToolbars(true, true);
					Form1_Resize(null, null);
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

		private void UpdateControls(bool stageLoad = false)
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

		#endregion

		#region Refresh UI



		private void UpdateEntitiesToolbarList()
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

		private void UpdateTilesOptions()
		{
			if (IsTilesEdit() && !IsChunksEdit())
			{
				if (TilesToolbar != null)
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
			}
		}

		private void UpdateStatusPanel(object sender, EventArgs e)
		{
			//
			// Tooltip Bar Info 
			//
			if (EnablePixelCountMode == false)
			{
				positionLabel.Content = "X: " + (int)(lastX / Zoom) + " Y: " + (int)(lastY / Zoom);
			}
			else
			{
				positionLabel.Content = "X: " + (int)((lastX / Zoom) / 16) + " Y: " + (int)((lastY / Zoom) / 16);
			}


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

			if (mySettings.ScrollLockDirection == true)
			{
				scrollLockDirLabel.Content = "Scroll Lock Direction: X";
			}
			else
			{
				scrollLockDirLabel.Content = "Scroll Lock Direction: Y";
			}


			hVScrollBarXYLabel.Content = "Zoom Value: " + Zoom.ToString();
			//hScrollBarValueLabel.Content = "Horz. ScrollBar Value: " + editorView.hScrollBar1.Value + " Horz. ScrollBar Max: " + editorView.hScrollBar1.Maximum;
			//vScrollBarValueLabel.Content = "Vert. ScrollBar Value: " + editorView.vScrollBar1.Value + " Vert. ScrollBar Max: " + editorView.vScrollBar1.Maximum;

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
			//toolStrip2.IsEnabled = enabled;
			//toolStrip3.IsEnabled = enabled;
			editorView.splitContainer1.Enabled = enabled;
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
					System.Windows.MessageBox.Show("Tiles found at: " + Environment.NewLine + message, "Results");
					if (copyResults && message != null)
					{
						Clipboard.SetText(message);
					}
				}
				else
				{
					System.Windows.MessageBox.Show("Found Nothing", "Results");
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

		#region Mouse Actions

		private void GraphicPanel_OnMouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (InstanceID != -1 && !KickStartMegaManiacRenderLoopFinished)
			{
				KickStartMegaManiacRenderLoop = true;
			}

			if (mySettings.allowForSmoothSelection)
			{
				editorView.GraphicPanel.Render();
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

				if (IsTilesEdit() && !InteractionToolButton.IsChecked.Value)
				{

					if ((EditLayer?.IsPointSelected(clicked_point)).Value)
					{
						// Start dragging the tiles
						dragged = true;
						startDragged = true;
						EditLayer?.StartDrag();

					}

					else if (!SelectToolButton.IsChecked.Value && !ShiftPressed() && !CtrlPressed() && (EditLayer?.HasTileAt(clicked_point)).Value)
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

				int xMove = (editorView.hScrollBar1.IsVisible) ? e.X - ShiftX - scrollPosition.X : 0;
				int yMove = (editorView.vScrollBar1.IsVisible) ? e.Y - ShiftY - scrollPosition.Y : 0;

				if (Math.Abs(xMove) < 15) xMove = 0;
				if (Math.Abs(yMove) < 15) yMove = 0;

				if (xMove > 0)
				{
					if (yMove > 0) Cursor = Cursors.ScrollSE;
					else if (yMove < 0) Cursor = Cursors.ScrollNE;
					else Cursor = Cursors.ScrollE;

				}
				else if (xMove < 0)
				{
					if (yMove > 0) Cursor = Cursors.ScrollSW;
					else if (yMove < 0) Cursor = Cursors.ScrollNW;
					else Cursor = Cursors.ScrollW;

				}
				else
				{

					if (yMove > 0) Cursor = Cursors.ScrollS;
					else if (yMove < 0) Cursor = Cursors.ScrollN;
					else
					{
						if (editorView.vScrollBar1.IsVisible && editorView.hScrollBar1.IsVisible) Cursor = Cursors.ScrollAll;
						else if (editorView.vScrollBar1.IsVisible) Cursor = Cursors.ScrollNS;
						else if (editorView.hScrollBar1.IsVisible) Cursor = Cursors.ScrollWE;
					}

				}

				Point position = new Point(ShiftX, ShiftY); ;
				int x = xMove / 10 + position.X;
				int y = yMove / 10 + position.Y;

				CustomX += xMove / 10;
				CustomY += yMove / 10;

				if (x < 0) x = 0;
				if (y < 0) y = 0;
				if (x > editorView.hScrollBar1.Maximum) x = (int)editorView.hScrollBar1.Maximum;
				if (y > editorView.vScrollBar1.Maximum) y = (int)editorView.vScrollBar1.Maximum;


				if (x != position.X || y != position.Y)
				{

					if (editorView.vScrollBar1.IsVisible)
					{
						editorView.vScrollBar1.Value = y;
					}
					if (editorView.hScrollBar1.IsVisible)
					{
						editorView.hScrollBar1.Value = x;
					}

					editorView.GraphicPanel.OnMouseMoveEventCreate();

				}

				editorView.GraphicPanel.Render();

			}
			if (IsEditing())
			{
				if (IsTilesEdit() && !IsChunksEdit() && PlaceTilesButton.IsChecked.Value)
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
				else if (IsChunksEdit() && EditLayer != null)
				{
					Point p = new Point((int)(e.X / Zoom), (int)(e.Y / Zoom));
					Point pC = EditLayer.GetChunkCoordinates(p.X, p.Y);

					if (e.Button == MouseButtons.Left)
					{
						int selectedIndex = TilesToolbar.retroEDTileList1.SelectedIndex;
						// Place Stamp
						if (selectedIndex != -1)
						{
							if (!EditorChunk.DoesChunkMatch(pC, EditorChunk.StageStamps.StampList[selectedIndex], EditLayer))
							{
								EditorChunk.PasteStamp(pC, selectedIndex, EditLayer);
							}

						}
					}

					else if (e.Button == MouseButtons.Right)
					{
						if (!EditorChunk.IsChunkEmpty(pC, EditLayer))
						{
							// Remove Stamp Sized Area
							EditorChunk.PasteStamp(pC, 0, EditLayer, true);
						}
					}
				}



				if (draggingSelection || dragged)
				{
					Point position = new Point(ShiftX, ShiftY); ;
					int ScreenMaxX = position.X + editorView.splitContainer1.Panel1.Width - (int)editorView.vScrollBar1Host.Width;
					int ScreenMaxY = position.Y + editorView.splitContainer1.Panel1.Height - (int)editorView.hScrollBar1Host.Height;
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
					if (x > editorView.hScrollBar1.Maximum) x = (int)editorView.hScrollBar1.Maximum;
					if (y > editorView.vScrollBar1.Maximum) y = (int)editorView.vScrollBar1.Maximum;

					if (x != position.X || y != position.Y)
					{
						if (editorView.vScrollBar1.IsVisible)
						{
							editorView.vScrollBar1.Value = y;
						}
						if (editorView.hScrollBar1.IsVisible)
						{
							editorView.hScrollBar1.Value = x;
						}
						editorView.GraphicPanel.OnMouseMoveEventCreate();
						editorView.GraphicPanel.Render();



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

							if (UseMagnetMode)
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
							System.Windows.MessageBox.Show("Too many entities! (limit: 2048)");
							dragged = false;
							return;
						}
						if (UseMagnetMode)
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

		private void GraphicPanel_OnMouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			editorView.GraphicPanel.Focus();
			if (e.Button == MouseButtons.Left)
			{
				if (IsEditing() && !dragged)
				{
					if (IsTilesEdit() && !InteractionToolButton.IsChecked.Value && !IsChunksEdit())
					{
						if (PlaceTilesButton.IsChecked.Value)
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
					if (IsChunksEdit() && IsSceneLoaded())
					{
						Point p = new Point((int)(e.X / Zoom), (int)(e.Y / Zoom));
						Point pC = EditLayer.GetChunkCoordinates(p.X, p.Y);


						int selectedIndex = TilesToolbar.retroEDTileList1.SelectedIndex;
						// Place Stamp
						if (selectedIndex != -1)
						{
							if (!EditorChunk.DoesChunkMatch(pC, EditorChunk.StageStamps.StampList[selectedIndex], EditLayer))
							{
								EditorChunk.PasteStamp(pC, selectedIndex, EditLayer);
							}

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
					Cursor = Cursors.Arrow;
				}
			}
			else if (e.Button == MouseButtons.Right)
			{
				if (IsTilesEdit() && PlaceTilesButton.IsChecked.Value && !IsChunksEdit())
				{
					// Remove tile
					Point p = new Point((int)(e.X / Zoom), (int)(e.Y / Zoom));
					if (!EditLayer.IsPointSelected(p))
					{
						EditLayer.Select(p);
					}
					DeleteSelected();
				}
				else if (IsChunksEdit())
				{
					Point p = new Point((int)(e.X / Zoom), (int)(e.Y / Zoom));
					Point pC = EditLayer.GetChunkCoordinates(p.X, p.Y);

					if (!EditorChunk.IsChunkEmpty(pC, EditLayer))
					{
						// Remove Stamp Sized Area
						EditorChunk.PasteStamp(pC, 0, EditLayer, true);
					}


				}
			}
			else if (e.Button == MouseButtons.Middle || Mouse.MiddleButton == MouseButtonState.Pressed)
			{
				wheelClicked = true;
				scrolling = true;
				scrollingDragged = false;
				scrollPosition = new Point(e.X - ShiftX, e.Y - ShiftY);
				if (editorView.vScrollBar1.IsVisible && editorView.hScrollBar1.IsVisible)
				{
					//Cursor = System.Windows.Forms.Cursors.NoMove2D;
					Cursor = Cursors.ScrollAll;

				}
				else if (editorView.vScrollBar1.IsVisible)
				{
					//Cursor = System.Windows.Forms.Cursors.NoMoveHoriz;
					Cursor = Cursors.ScrollWE;
				}
				else if (editorView.hScrollBar1.IsVisible)
				{
					//Cursor = System.Windows.Forms.Cursors.NoMoveVert;
					Cursor = Cursors.ScrollNS;
				}
				else
				{
					scrolling = false;
				}
			}
		}

		private void GraphicPanel_OnMouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
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
			else if (e.Button == MouseButtons.Middle || Mouse.MiddleButton == MouseButtonState.Pressed)
			{
				wheelClicked = false;
				if (scrollingDragged)
				{
					scrolling = false;
					Cursor = Cursors.Arrow;
				}
			}
			UpdateControls();
		}

		private void GraphicPanel_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			editorView.GraphicPanel.Focus();
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
				if (editorView.vScrollBar1.IsVisible || editorView.hScrollBar1.IsVisible)
				{
					if (scrollDirection == "Y" && !mySettings.scrollLock)
					{
						if (editorView.vScrollBar1.IsVisible)
						{
							int y = (int)editorView.vScrollBar1.Value - e.Delta;
							if (y < 0) y = 0;
							if (y > editorView.vScrollBar1.Maximum) y = (int)editorView.vScrollBar1.Maximum;
							editorView.vScrollBar1.Value = y;
						}
						else
						{
							int x = (int)editorView.hScrollBar1.Value - e.Delta * 2;
							if (x < 0) x = 0;
							if (x > editorView.hScrollBar1.Maximum) x = (int)editorView.hScrollBar1.Maximum;
							editorView.hScrollBar1.Value = x;
						}
					}
					else if (scrollDirection == "X" && !mySettings.scrollLock)
					{
						if (editorView.hScrollBar1.IsVisible)
						{
							int x = (int)editorView.hScrollBar1.Value - e.Delta * 2;
							if (x < 0) x = 0;
							if (x > editorView.hScrollBar1.Maximum) x = (int)editorView.hScrollBar1.Maximum;
							editorView.hScrollBar1.Value = x;
						}
						else
						{
							int y = (int)editorView.vScrollBar1.Value - e.Delta;
							if (y < 0) y = 0;
							if (y > editorView.vScrollBar1.Maximum) y = (int)editorView.vScrollBar1.Maximum;
							editorView.vScrollBar1.Value = y;
						}
					}
					else if (scrollDirection == "Locked" || mySettings.scrollLock == true)
					{
						if (mySettings.ScrollLockDirection == false)
						{
							if (editorView.vScrollBar1.IsVisible)
							{
								int y = (int)editorView.vScrollBar1.Value - e.Delta * 2;
								if (y < 0) y = 0;
								if (y > editorView.vScrollBar1.Maximum) y = (int)editorView.vScrollBar1.Maximum;
								if (y <= -1) y = 0;
								editorView.vScrollBar1.Value = y;
							}
							else
							{
								int x = (int)editorView.vScrollBar1.Value - e.Delta * 2;
								if (x < 0) x = 0;
								if (x > editorView.vScrollBar1.Maximum) x = (int)editorView.vScrollBar1.Maximum;
								if (x <= -1) x = 0;
								editorView.vScrollBar1.Value = x;
							}
						}
						else
						{
							if (editorView.hScrollBar1.IsVisible)
							{
								int x = (int)editorView.hScrollBar1.Value - e.Delta * 2;
								if (x < 0) x = 0;
								if (x > editorView.hScrollBar1.Maximum) x = (int)editorView.hScrollBar1.Maximum;
								if (x <= -1) x = 0;
								editorView.hScrollBar1.Value = x;
							}
							else
							{
								int y = (int)editorView.vScrollBar1.Value - e.Delta;
								if (y < 0) y = 0;
								if (y > editorView.vScrollBar1.Maximum) y = (int)editorView.vScrollBar1.Maximum;
								if (y <= -1) y = 0;
								editorView.vScrollBar1.Value = y;
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

		private MenuItem CreateDataDirectoryMenuLink(string target)
		{
			MenuItem newItem = new MenuItem();
			newItem.Header = target;
			newItem.Tag = target;
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
					System.Windows.MessageBox.Show("Something is wrong with this GameConfig that we can't support! If for some reason it does work for you in Sonic Mania, you can create another GameConfig.bin called GameConfig_ME.bin and the editor should load that instead (assuming it's a clean GameConfig or one that works) allowing you to still be able to use the data folder, however, this is experimental so be careful when doing that.", "GameConfig Error!");
					return false;
				}


			}

		}
		public bool IsDataDirectoryValid(string directoryToCheck)
		{
			return File.Exists(Path.Combine(directoryToCheck, "Game", "GameConfig.bin"));
		}

		private void RecentDataDirectoryClicked(object sender, RoutedEventArgs e, String dataDirectory)
		{
			var dataDirectories = mySettings.DataDirectories;
			mySettings.GamePath = GamePath;
			if (IsDataDirectoryValid(dataDirectory))
			{
				ResetDataDirectoryToAndResetScene(dataDirectory);
			}
			else
			{
				System.Windows.MessageBox.Show($"The specified Data Directory {dataDirectory} is not valid.",
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

		private void RecentDataDirectoryClicked(object sender, RoutedEventArgs e)
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
				System.Windows.MessageBox.Show($"The specified Data Directory {dataDirectory} is not valid.",
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

		private void UpdateDataFolderLabel(object sender, RoutedEventArgs e)
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
				ShiftX = (int)Math.Min(editorView.hScrollBar1.Maximum, Math.Max(0, ShiftX));
				editorView.hScrollBar1.Value = ShiftX;
			}
			if (editorView.vScrollBar1.IsVisible)
			{
				ShiftY = (int)((zoom_point.Y + oldShiftY) / old_zoom * Zoom - zoom_point.Y);
				ShiftY = (int)Math.Min(editorView.vScrollBar1.Maximum, Math.Max(0, ShiftY));
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
				ResizeGraphicPanel(editorView.GraphicPanel.Width, editorView.GraphicPanel.Height);
			while (ScreenHeight > editorView.GraphicPanel.Height)
				ResizeGraphicPanel(editorView.GraphicPanel.Width, editorView.GraphicPanel.Height);


		}

		private void SetViewSize(int width = 0, int height = 0)
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


			if (!mySettings.EntityFreeCam || !isExportingImage)
			{
				editorView.vScrollBar1.Maximum = height - (int)editorView.vScrollBar1.LargeChange;
				editorView.hScrollBar1.Maximum = width - (int)editorView.hScrollBar1.LargeChange;
			}

			editorView.GraphicPanel.DrawWidth = Math.Min(width, editorView.GraphicPanel.Width);
			editorView.GraphicPanel.DrawHeight = Math.Min(height, editorView.GraphicPanel.Height);

			Form1_Resize(editorView, null);


			if (!mySettings.EntityFreeCam || !isExportingImage)
			{
				editorView.hScrollBar1.Value = Math.Max(0, Math.Min(editorView.hScrollBar1.Value, editorView.hScrollBar1.Maximum));
				editorView.vScrollBar1.Value = Math.Max(0, Math.Min(editorView.vScrollBar1.Value, editorView.vScrollBar1.Maximum));
			}
		}

		private void ResetViewSize()
		{
			SetViewSize((int)(SceneWidth * Zoom), (int)(SceneHeight * Zoom));
		}

		private void ResizeGraphicPanel(int width = 0, int height = 0)
		{
			/*
            if (mySettings.EntityFreeCam)
            {
                width = SceneWidth;
                height = SceneHeight;
            }

            
            editorView.GraphicPanel.Width = width;
            editorView.GraphicPanel.Height = height;

            editorView.GraphicPanel.ResetDevice();

            editorView.GraphicPanel.DrawWidth = (int)Math.Min(editorView.hScrollBar1.Maximum, editorView.GraphicPanel.Width);
            editorView.GraphicPanel.DrawHeight = (int)Math.Min(editorView.vScrollBar1.Maximum, editorView.GraphicPanel.Height);
            */
		}

		#endregion

		#region Extra Layer Related Methods

		private void SetupLayerButtons()
		{
			TearDownExtraLayerButtons();
			//EDIT BUTTONS
			foreach (EditorLayer el in EditorScene.OtherLayers)
			{
				ToggleButton tsb = new ToggleButton()
				{
					Content = el.Name
				};
				LayerToolbar.Items.Add(tsb);
				tsb.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(Color.LawnGreen.A, Color.LawnGreen.R, Color.LawnGreen.G, Color.LawnGreen.B));
				tsb.IsChecked = false;
				tsb.Style = (Style)FindResource("Flat");
				tsb.Click += AdHocLayerEdit;

				_extraLayerEditButtons.Add(tsb);
			}

			//EDIT BUTTONS SEPERATOR
			Separator tss = new Separator();
			LayerToolbar.Items.Add(tss);
			_extraLayerSeperators.Add(tss);

			//VIEW BUTTONS
			foreach (EditorLayer el in EditorScene.OtherLayers)
			{
				ToggleButton tsb = new ToggleButton()
				{
					Content = el.Name
				};
				//toolStrip1.Items.Add(tsb);
				LayerToolbar.Items.Insert(LayerToolbar.Items.IndexOf(extraViewLayersSeperator), tsb);
				tsb.Style = (Style)FindResource("Flat");
				tsb.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, Color.FromArgb(0x33AD35).R, Color.FromArgb(0x33AD35).G, Color.FromArgb(0x33AD35).B));
				tsb.IsChecked = false;

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
				LayerToolbar.Items.Remove(elb);
			}
			_extraLayerEditButtons.Clear();

			foreach (var elb in _extraLayerViewButtons)
			{
				LayerToolbar.Items.Remove(elb);
			}
			_extraLayerViewButtons.Clear();


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
		private void UpdateDualButtonsControlsForLayer(EditorLayer layer, ToggleButton visibilityButton, ToggleButton editButton)
		{
			bool layerValid = layer != null;
			visibilityButton.IsChecked = layerValid;
			if (layerValid)
			{
				string name = layer.Name;
				//visibilityButton.Name = name;
				editButton.Content = name.ToString();
			}
		}

		private void AdHocLayerEdit(object sender, RoutedEventArgs e)
		{
			ToggleButton tsb = sender as ToggleButton;
			Deselect(false);
			if (tsb.IsChecked.Value)
			{
				if (!mySettings.KeepLayersVisible)
				{
					ShowFGLow.IsChecked = false;
					ShowFGHigh.IsChecked = false;
					ShowFGLower.IsChecked = false;
					ShowFGHigher.IsChecked = false;
				}
				EditFGLow.IsChecked = false;
				EditFGHigh.IsChecked = false;
				EditFGLower.IsChecked = false;
				EditFGHigher.IsChecked = false;
				EditEntities.IsChecked = false;

				foreach (var elb in _extraLayerEditButtons)
				{
					if (elb != tsb)
					{
						elb.IsChecked = false;
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
		void UnloadScene()
		{
			EditorScene?.Dispose();
			EditorScene = null;
			SceneFilename = null;
			SceneFilepath = null;
			StageConfig = null;
			StageConfigFileName = null;
			_levelIDLabel.Content = "Level ID: NULL";
			LevelID = -1;
			encorePaletteExists = false;
			EncorePalette = null;
			EncoreSetupType = 0;
			playerObjectPosition = new List<SceneEntity> { };
			INILayerNameHigher = "";
			INILayerNameLower = "";
			EditorSettings.CleanPrefrences();
			userDefinedEntityRenderSwaps = new Dictionary<string, string>();
			userDefinedSpritePaths = new List<string>();

			SelectedScene = null;
			SelectedZone = null;
			EncorePaletteButton.IsChecked = false;

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

			EditFGLow.IsChecked = false;
			EditFGHigh.IsChecked = false;
			EditFGLower.IsChecked = false;
			EditFGHigher.IsChecked = false;
			EditEntities.IsChecked = false;

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

			UpdateStartScreen(true);
		}

		public void OpenSceneForceFully()
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
		private void OpenSceneForceFully(string dataDir, string scenePath, string modPath, int levelID, bool isEncoreMode, int X, int Y, double _ZoomScale = 0.0)
		{
			DataDirectory = dataDir;
			string Result = scenePath;
			int LevelID = levelID;
			bool isEncore = isEncoreMode;
			if (_ZoomScale != 0.0)
			{
				ShortcutZoomValue = _ZoomScale;
				ShortcutHasZoom = true;
			}
			TempWarpCoords = new Point(X, Y);
			ForceWarp = true;
			OpenScene(false, Result, LevelID, isEncore, (modPath != "" ? true : false), modPath);
		}

		private void OpenSceneForceFully(string dataDir)
		{
			DataDirectory = dataDir;
			OpenScene();
		}

		public void OpenScene(bool manual = false, string Result = null, int LevelID = -1, bool isEncore = false, bool modLoaded = false, string modDir = "")
		{
			ManiacEditor.Interfaces.SceneSelectWindow select;
			ModDataDirectory = modDir;
			string ResultPath = null;
			if (Result == null)
			{
				if (manual == false)
				{

					if (!EditorLoad())
					{
						select = new ManiacEditor.Interfaces.SceneSelectWindow(null, this);
						select.Owner = GetWindow(this);
						select.ShowDialog();
					}
					else
					{
						select = new ManiacEditor.Interfaces.SceneSelectWindow(GameConfig, this);
						select.Owner = GetWindow(this);
						select.ShowDialog();
					}
					Result = select.SceneSelect.Result;
					LevelID = select.SceneSelect.LevelID;
					isEncore = select.SceneSelect.isEncore;
					ResultPath = Path.GetDirectoryName(Result);
					modLoaded = (select.SceneSelect.isModLoadedwithGameConfig);

				}
				else
				{
					OpenFileDialog open = new OpenFileDialog
					{
						Filter = "Scene File|*.bin"
					};
					if (open.ShowDialog() != System.Windows.Forms.DialogResult.Cancel)
					{
						Result = open.FileName;
					}


				}
			}


			if (Result == null)
			{
				return;
			}

			string _DataDirectory = (modLoaded == true ? ModDataDirectory : DataDirectory);


			try
			{
				ResultPath = Path.GetDirectoryName(Result);
			}
			catch
			{
				ResultPath = Result;
			}


			UnloadScene();
			UseDefaultPrefrences();
			bool goodGameConfig = SetGameConfig();
			if (goodGameConfig == false)
			{
				return;
			}

			if (isEncore)
			{
				EncorePaletteButton.IsChecked = true;
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
				System.Windows.MessageBox.Show("Load failed. Error: " + ex.ToString() + " " + Result);
				return;
			}

			UpdateStartScreen(false);

			UpdateDataFolderLabel(null, null);

			SetupLayerButtons();

			Stamps StageStamps = new Stamps();

			if (File.Exists(SceneFilepath + "\\ManiacStamps.bin"))
			{
				StageStamps = new Stamps(SceneFilepath + "\\ManiacStamps.bin");
			}

			EditorChunk = new EditorChunk(this, StageTiles, StageStamps);

			EditorBackground = new EditorBackground(this);

			entities = new EditorEntities(EditorScene, this);

			SetViewSize(SceneWidth, SceneHeight);

			UpdateControls(true);

		}

		private void OpenScenefromSceneSelect(string Result, string _DataDirectory, int _LevelID, bool modLoaded)
		{
			int searchType = 1;
			SelectedZone = Result.Replace(Path.GetFileName(Result), "");
			SelectedScene = Path.GetFileName(Result);
			SceneFilename = Path.Combine(_DataDirectory, "Stages", SelectedZone, SelectedScene);
			SceneFilepath = Path.Combine(_DataDirectory, "Stages", SelectedZone);
			SelectedZone = SelectedZone.Replace("\\", "");
			LevelID = _LevelID;
			EditorScene = new EditorScene(SceneFilename, editorView.GraphicPanel, this);
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

				TilesConfig = new TileConfig(Path.Combine(SceneFilepath, "TileConfig.bin"));
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
			Discord.ScenePath = Result;
			Discord.UpdateDiscord("Editing " + Result);


			if (File.Exists(SceneFilepath + "\\maniac.ini"))
			{
				bool allowToRead = false;
				using (Stream stream = EditorSettings.GetSceneIniResource(SceneFilepath + "\\maniac.ini"))
				{
					if (stream != null)
					{
						EditorSettings.GetSceneINISettings(stream);
						allowToRead = true;
					}
					else
					{
						Debug.Print("Something went wrong trying to read the Maniac.INI File");
						allowToRead = false;
					}
				}
				if (allowToRead)
				{
					try
					{
						EditorSettings.SetINIDefaultPrefrences(this);
					}
					catch (Exception ex)
					{
						System.Windows.MessageBox.Show("Failed to Inturpret INI File. Error: " + ex.ToString() + " " + Result);
						EditorSettings.CleanPrefrences();
					}

				}


			}


		}

		private void OpenScenefromBrowse(string Result, string _DataDirectory, int _LevelID)
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
			LevelID = _LevelID;
			EditorScene = new EditorScene(SceneFilename, editorView.GraphicPanel, this);
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

				TilesConfig = new TileConfig(Path.Combine(SceneFilepath, "TileConfig.bin"));
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
			Discord.ScenePath = Result;
			Discord.UpdateDiscord("Editing " + Result);

			if (File.Exists(SceneFilepath + "\\maniac.ini"))
			{
				bool allowToRead = false;
				using (Stream stream = EditorSettings.GetSceneIniResource(SceneFilepath + "\\maniac.ini"))
				{
					if (stream != null)
					{
						EditorSettings.GetSceneINISettings(stream);
						allowToRead = true;
					}
					else
					{
						Debug.Print("Something went wrong trying to read the Maniac.INI File");
						allowToRead = false;
					}
				}
				if (allowToRead)
				{
					try
					{
						EditorSettings.SetINIDefaultPrefrences(this);
					}
					catch (Exception ex)
					{
						System.Windows.MessageBox.Show("Failed to Inturpret INI File. Error: " + ex.ToString() + " " + Result);
						EditorSettings.CleanPrefrences();
					}

				}


			}
		}

		#endregion

		#region File Tab Buttons
		public void Open_Click(object sender, RoutedEventArgs e)
		{
			if (AllowSceneUnloading() != true) return;
			UnloadScene();

			OpenScene(mySettings.forceBrowse);

		}

		private void OpenToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			Open_Click(sender, e);
		}

		public void OpenDataDirectoryToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
				if (AllowSceneUnloading() != true) return;

				string newDataDirectory = GetDataDirectory();
				if (null == newDataDirectory) return;
				if (newDataDirectory.Equals(DataDirectory)) return;

				if (IsDataDirectoryValid(newDataDirectory))
					ResetDataDirectoryToAndResetScene(newDataDirectory);
				else
					System.Windows.MessageBox.Show($@"{newDataDirectory} is not
a valid Data Directory.",
									"Invalid Data Directory!",
									MessageBoxButton.OK,
									MessageBoxImage.Error);

		}

		public void Save_Click(object sender, RoutedEventArgs e)
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

			try
			{
				EditorChunk.StageStamps?.Write(SceneFilepath + "//ManiacStamps.bin");
			}
			catch (Exception ex)
			{
				ShowError($@"Failed to save StageStamps to file '{SceneFilepath + "ManiacStamps.bin"}'
Error: {ex.Message}");
			}
		}

		private void ExitToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (InstanceID == -1)
			{
				Close();
			}
			else
			{
				CloseMegaManiacTab = true;
			}
		}

		private void SaveAspngToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (EditorScene == null) return;

			SaveFileDialog save = new SaveFileDialog
			{
				Filter = ".png File|*.png",
				DefaultExt = "png"
			};
			if (save.ShowDialog() != System.Windows.Forms.DialogResult.Cancel)
			{
				isExportingImage = true;
				using (Bitmap bitmap = new Bitmap(SceneWidth, SceneHeight))
				using (Graphics g = Graphics.FromImage(bitmap))
				{
					// not all scenes have both a Low and a High foreground
					// only attempt to render the ones we actually have
					FGLower?.Draw(g);
					FGLow?.Draw(g);
					FGHigh?.Draw(g);
					FGHigher?.Draw(g);
					//entities?.GraphicsDraw(editorView.GraphicPanel, g);

					bitmap.Save(save.FileName);
				}

			}
		}

		private void ExportEachLayerAspngToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				if (EditorScene?._editorLayers == null || !EditorScene._editorLayers.Any()) return;

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
						bitmap.Save(fileName, System.Drawing.Imaging.ImageFormat.Png);
						++fileCount;
					}
				}

				System.Windows.MessageBox.Show($"Layer export succeeded. {fileCount} images saved.", "Success!",
								MessageBoxButton.OK, MessageBoxImage.Information);
			}
			catch (Exception ex)
			{
				ShowError("An error occurred: " + ex.Message);
			}
		}

		public void SaveAsToolStripMenuItem_Click(object sender, RoutedEventArgs e)
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
			if (save.ShowDialog() != System.Windows.Forms.DialogResult.Cancel)
			{
				EditorScene.Write(save.FileName);
			}
		}

		private void BackupToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			BackupTool(null, null);
		}

		private void BackupRecoverButton_Click(object sender, RoutedEventArgs e)
		{
			string Result = null, ResultOriginal = null, ResultOld = null;
			OpenFileDialog open = new OpenFileDialog
			{
				Filter = "Backup Scene|*.bin.bak|Old Scene|*.bin.old|Crash Backup Scene|*.bin.crash.bak"
			};
			if (open.ShowDialog() != System.Windows.Forms.DialogResult.Cancel)
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

		private void ObjectManagerToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			try
			{

				var objectManager = new ManiacEditor.Interfaces.ObjectManager(EditorScene.Objects, StageConfig, this);
				objectManager.Owner = Window.GetWindow(this);
				objectManager.ShowDialog();

				if (objectManager.DialogResult.Value != true)
					return; // nothing to do

				// user clicked Import, get to it!
				UpdateControls();
				entitiesToolbar?.RefreshSpawningObjects(EditorScene.Objects);
			}
			catch (Exception ex)
			{
				System.Windows.MessageBox.Show("Unable to import Objects. " + ex.Message);
			}
		}

		public void UnloadSceneToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			UnloadScene();
		}

		#region Backup SubMenu
		private void StageConfigToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			backupType = 4;
			BackupToolStripMenuItem_Click(null, null);
			backupType = 0;
		}

		private void NormalToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			backupType = 1;
			BackupToolStripMenuItem_Click(null, null);
			backupType = 0;
		}
		#endregion

		#endregion

		#region Edit Tab Buttons

		private void chunkToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (TilesClipboard != null)
			{
				EditorChunk.ConvertClipboardtoChunk(TilesClipboard);
				TilesToolbar?.ChunksReload();
			}

		}

		public void SelectAllToolStripMenuItem_Click(object sender, RoutedEventArgs e)
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

		public void FlipHorizontalToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			EditLayer?.FlipPropertySelected(FlipDirection.Horizontal);
			UpdateEditLayerActions();
		}

		public void FlipHorizontalIndividualToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			EditLayer?.FlipPropertySelected(FlipDirection.Horizontal, true);
			UpdateEditLayerActions();
		}

		private void DeleteToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			DeleteSelected();
		}

		public void CopyToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (IsTilesEdit())
				CopyTilesToClipboard();


			else if (IsEntitiesEdit())
				CopyEntitiesToClipboard();


			UpdateControls();
		}

		public void DuplicateToolStripMenuItem_Click(object sender, RoutedEventArgs e)
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
					System.Windows.MessageBox.Show("Too many entities! (limit: 2048)");
					return;
				}
				UpdateEntitiesToolbarList();
				SetSelectOnlyButtonsState();
				UpdateEntitiesToolbarList();

			}
		}

		private void UndoToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			EditorUndo();
		}

		private void RedoToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			EditorRedo();
		}

		public void CutToolStripMenuItem_Click(object sender, RoutedEventArgs e)
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
				if (entitiesToolbar.IsFocused.Equals(false))
				{
					CopyEntitiesToClipboard();
					DeleteSelected();
					UpdateControls();
					UpdateEntitiesToolbarList();
				}
			}
		}

		public void PasteToolStripMenuItem_Click(object sender, RoutedEventArgs e)
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
				if (entitiesToolbar.IsFocused.Equals(false))
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
						System.Windows.MessageBox.Show("Too many entities! (limit: 2048)");
						return;
					}
					UpdateEntitiesToolbarList();
					SetSelectOnlyButtonsState();
				}
			}
		}

		public void FlipVerticalToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			EditLayer?.FlipPropertySelected(FlipDirection.Veritcal);
			UpdateEditLayerActions();
		}

		public void FlipVerticalIndividualToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			EditLayer?.FlipPropertySelected(FlipDirection.Veritcal, true);
			UpdateEditLayerActions();
			UpdateEditLayerActions();
		}

		#endregion

		#region View Tab Buttons

		private void statsToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			DebugStatsVisibleOnPanel = !DebugStatsVisibleOnPanel;
			showStatsToolStripMenuItem.IsChecked = DebugStatsVisibleOnPanel;
		}

		private void ResetZoomLevelToolstripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			SetZoomLevel(0, new Point(0, 0));
		}

		private void useLargeTextToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			UseLargeDebugStats = !UseLargeDebugStats;
			useLargeTextToolStripMenuItem.IsChecked = DebugStatsVisibleOnPanel;
		}

		private void SetMenuButtons(object sender, RoutedEventArgs e)
		{
			MenuItem menuItem = sender as MenuItem;
			if (menuItem != null)
			{
				if (menuItem.Tag != null)
				{
					var allItems = menuButtonsToolStripMenuItem.Items.Cast<System.Windows.Controls.MenuItem>().ToArray();
					foreach (MenuItem item in allItems)
					{
						item.IsChecked = false;
						var allSubItems = menuButtonsToolStripMenuItem.Items.Cast<System.Windows.Controls.MenuItem>().ToArray();
						foreach (MenuItem subItem in allSubItems)
						{
							subItem.IsChecked = false;
						}
					}
					string tag = menuItem.Tag.ToString();
					switch (tag)
					{
						case "Xbox":
							CurrentControllerButtons = 2;
							break;
						case "Switch":
							CurrentControllerButtons = 4;
							break;
						case "PS4":
							CurrentControllerButtons = 3;
							break;
						case "Saturn Black":
							CurrentControllerButtons = 5;
							break;
						case "Saturn White":
							CurrentControllerButtons = 6;
							break;
						case "Switch Joy L":
							CurrentControllerButtons = 7;
							break;
						case "Switch Joy R":
							CurrentControllerButtons = 8;
							break;
						case "PC EN/JP":
							CurrentControllerButtons = 1;
							break;
						case "PC FR":
							CurrentControllerButtons = 9;
							break;
						case "PC IT":
							CurrentControllerButtons = 10;
							break;
						case "PC GE":
							CurrentControllerButtons = 11;
							break;
						case "PC SP":
							CurrentControllerButtons = 12;
							break;
					}
					menuItem.IsChecked = true;
				}

			}

		}

		private void SetMenuButtons(string tag)
		{
			switch (tag)
			{
				case "Xbox":
					CurrentControllerButtons = 2;
					break;
				case "Switch":
					CurrentControllerButtons = 4;
					break;
				case "PS4":
					CurrentControllerButtons = 3;
					break;
				case "Saturn Black":
					CurrentControllerButtons = 5;
					break;
				case "Saturn White":
					CurrentControllerButtons = 6;
					break;
				case "Switch Joy L":
					CurrentControllerButtons = 7;
					break;
				case "Switch Joy R":
					CurrentControllerButtons = 8;
					break;
				case "PC EN/JP":
					CurrentControllerButtons = 1;
					break;
				case "PC FR":
					CurrentControllerButtons = 9;
					break;
				case "PC IT":
					CurrentControllerButtons = 10;
					break;
				case "PC GE":
					CurrentControllerButtons = 11;
					break;
				case "PC SP":
					CurrentControllerButtons = 12;
					break;
			}
		}

		private void ShowEntitiesAboveAllOtherLayersToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (entityVisibilityType == 0)
			{
				showEntitiesAboveAllOtherLayersToolStripMenuItem.IsChecked = true;
				entityVisibilityType = 1;
			}
			else
			{
				showEntitiesAboveAllOtherLayersToolStripMenuItem.IsChecked = false;
				entityVisibilityType = 0;
			}

		}

		private void prioritizedViewingToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (!mySettings.PrioritizedObjectRendering)
			{
				mySettings.PrioritizedObjectRendering = true;
				prioritizedViewingToolStripMenuItem.IsChecked = true;
			}
			else
			{
				mySettings.PrioritizedObjectRendering = false;
				prioritizedViewingToolStripMenuItem.IsChecked = false;
			}

		}


		private void ChangeEncorePaleteToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			SetEncorePallete(sender);
		}

		public void SetEncorePallete(object sender = null, string path = "")
		{
			if (sender != null)
			{
				var clickedItem = sender as System.Windows.Controls.MenuItem;
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
						if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
						{
							EncorePalette = EditorScene.getEncorePalette("", "", "", "", -1, fd.FileName);
							EncoreSetupType = 0;
							if (File.Exists(EncorePalette[0]))
							{
								encorePaletteExists = true;
								EncorePaletteButton.IsChecked = true;
								useEncoreColors = true;
								ReloadSpecificTextures(null, null);
							}

						}
					}
				}
				catch (Exception ex)
				{
					System.Windows.MessageBox.Show("Unable to set Encore Colors. " + ex.Message);
				}
			}
			else if (path != "")
			{
				EncorePalette = EditorScene.getEncorePalette("", "", "", "", -1, path);
				EncoreSetupType = 0;
				if (File.Exists(EncorePalette[0]))
				{
					encorePaletteExists = true;
					EncorePaletteButton.IsChecked = true;
					useEncoreColors = true;
					ReloadSpecificTextures(null, null);
				}
				else
				{
					System.Windows.MessageBox.Show("Unable to set Encore Colors. The Specified Path does not exist: " + Environment.NewLine + path);
				}
			}

		}

		private void MoveExtraLayersToFrontToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (!extraLayersMoveToFront)
			{
				moveExtraLayersToFrontToolStripMenuItem.IsChecked = true;
				extraLayersMoveToFront = true;
			}
			else
			{
				moveExtraLayersToFrontToolStripMenuItem.IsChecked = false;
				extraLayersMoveToFront = false;
			}
		}

		private void ToolStripTextBox1_TextChanged(object sender, RoutedEventArgs e)
		{
			entitiesTextFilter = toolStripTextBox1.Text;
			entities.FilterRefreshNeeded = true;
		}

		private void ShowEntitySelectionBoxesToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (showEntitySelectionBoxes)
			{
				showEntitySelectionBoxesToolStripMenuItem.IsChecked = false;
				showEntitySelectionBoxes = false;
			}
			else
			{
				showEntitySelectionBoxesToolStripMenuItem.IsChecked = true;
				showEntitySelectionBoxes = true;
			}
		}

		private void ShowWaterLevelToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (!showWaterLevel)
			{
				showWaterLevelToolStripMenuItem.IsChecked = true;
				showWaterLevel = true;
			}
			else
			{
				showWaterLevelToolStripMenuItem.IsChecked = false;
				showWaterLevel = false;
			}
		}

		private void WaterLevelAlwaysShowItem_Click(object sender, RoutedEventArgs e)
		{

			if (!alwaysShowWaterLevel)
			{
				waterLevelAlwaysShowItem.IsChecked = true;
				alwaysShowWaterLevel = true;
			}
			else
			{
				waterLevelAlwaysShowItem.IsChecked = false;
				alwaysShowWaterLevel = false;
			}
		}

		private void SizeWithBoundsWhenNotSelectedToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (!sizeWaterLevelwithBounds)
			{
				sizeWithBoundsWhenNotSelectedToolStripMenuItem.IsChecked = false;
				sizeWaterLevelwithBounds = true;
			}
			else
			{
				sizeWithBoundsWhenNotSelectedToolStripMenuItem.IsChecked = false;
				sizeWaterLevelwithBounds = false;
			}
		}

		private void ToggleEncoreManiaObjectVisibilityToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			ToggleEncoreManiaEntitiesToolStripMenuItem_Click(sender, e);
		}

		private void ShowParallaxSpritesToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (showParallaxSprites)
			{
				showEntityPathArrowsToolstripItem.IsChecked = false;
				showParallaxSprites = false;
			}
			else
			{
				showEntityPathArrowsToolstripItem.IsChecked = false;
				showParallaxSprites = true;
			}
		}

		private void XToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (mySettings.ScrollLockDirection)
			{
				mySettings.ScrollLockDirection = false;
				xToolStripMenuItem.IsChecked = false;
				yToolStripMenuItem.IsChecked = true;
				scrollLockDirLabel.Content = "Scroll Lock Direction: Y";
			}
			else
			{
				mySettings.ScrollLockDirection = true;
				xToolStripMenuItem.IsChecked = true;
				yToolStripMenuItem.IsChecked = false;
				scrollLockDirLabel.Content = "Scroll Lock Direction: X";
			}
		}

		private void YToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (mySettings.ScrollLockDirection)
			{
				mySettings.ScrollLockDirection = false;
				xToolStripMenuItem.IsChecked = false;
				yToolStripMenuItem.IsChecked = true;
			}
			else
			{
				mySettings.ScrollLockDirection = true;
				xToolStripMenuItem.IsChecked = true;
				yToolStripMenuItem.IsChecked = false;
			}
		}

		private void ShowEntityPathToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (!showEntityPathArrows)
			{
				showEntityPathArrowsToolstripItem.IsChecked = true;
				showEntityPathArrows = true;
			}
			else
			{
				showEntityPathArrowsToolstripItem.IsChecked = false;
				showEntityPathArrows = false;
			}
		}

		#endregion

		#region Scene Tab Buttons
		public void ImportObjectsToolStripMenuItem_Click(object sender, RoutedEventArgs e, Window window = null)
		{
			importingObjects = true;
			try
			{
				Scene sourceScene = GetSceneSelection();
				if (sourceScene == null) return;
				var objectImporter = new ManiacEditor.Interfaces.ObjectImporter(sourceScene.Objects, EditorScene.Objects, StageConfig, this);
				if (window != null) objectImporter.Owner = window;
				objectImporter.ShowDialog();

				if (objectImporter.DialogResult != true)
					return; // nothing to do

				// user clicked Import, get to it!
				UpdateControls();
				entitiesToolbar?.RefreshSpawningObjects(EditorScene.Objects);

			}
			catch (Exception ex)
			{
				System.Windows.MessageBox.Show("Unable to import Objects. " + ex.Message);
			}
			importingObjects = false;
		}

		public void ImportSoundsToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			ImportSoundsToolStripMenuItem_Click(sender, e, GetWindow(this));
		}
		public void ImportSoundsToolStripMenuItem_Click(object sender, RoutedEventArgs e, Window window = null)
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
					if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
					{
						try
						{
							sourceStageConfig = new StageConfig(fd.FileName);
						}
						catch
						{
							System.Windows.MessageBox.Show("Ethier this isn't a stage config, or this stage config is ethier corrupted or unreadable in Maniac.");
							return;
						}

					}
				}
				if (null == sourceStageConfig) return;

				var soundImporter = new ManiacEditor.Interfaces.SoundImporter(sourceStageConfig, StageConfig);
				soundImporter.ShowDialog();

				if (soundImporter.DialogResult != true)
					return; // nothing to do


				// changing the sound list doesn't require us to do anything either

			}
			catch (Exception ex)
			{
				System.Windows.MessageBox.Show("Unable to import sounds. " + ex.Message);
			}
		}

		private void LayerManagerToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			Deselect(true);

			var lm = new LayerManager(EditorScene);
			lm.Owner = Window.GetWindow(this);
			lm.ShowDialog();

			SetupLayerButtons();
			ResetViewSize();
			UpdateControls();
		}

		private void PrimaryColorToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			ColorPickerDialog colorSelect = new ColorPickerDialog
			{
				Color = Color.FromArgb(EditorScene.EditorMetadata.BackgroundColor1.R, EditorScene.EditorMetadata.BackgroundColor1.G, EditorScene.EditorMetadata.BackgroundColor1.B)
			};
			UseExternalDarkTheme(colorSelect);
			System.Windows.Forms.DialogResult result = colorSelect.ShowDialog();
			if (result == System.Windows.Forms.DialogResult.OK)
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

		private void SecondaryColorToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			ColorPickerDialog colorSelect = new ColorPickerDialog
			{
				Color = Color.FromArgb(EditorScene.EditorMetadata.BackgroundColor2.R, EditorScene.EditorMetadata.BackgroundColor2.G, EditorScene.EditorMetadata.BackgroundColor2.B)
			};
			UseExternalDarkTheme(colorSelect);
			System.Windows.Forms.DialogResult result = colorSelect.ShowDialog();
			if (result == System.Windows.Forms.DialogResult.OK)
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

		private void OptimizeEntitySlotIDsToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (EditorScene != null)
			{
				entities.OptimizeSlotIDs();
			}
		}

		private void changeLevelIDToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			string inputValue = TextPrompt.ShowDialog("Change Level ID", "This is only temporary and will reset when you reload the scene.", LevelID.ToString());
			int.TryParse(inputValue.ToString(), out int output);
			LevelID = output;
			_levelIDLabel.Content = "Level ID: " + LevelID.ToString();
		}

		private void MakeForDataFolderOnlyToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			string dataDir = DataDirectory;
			CreateShortcut(dataDir);
		}

		private void WithCurrentCoordinatesToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			string dataDir = DataDirectory;
			string scenePath = Discord.ScenePath;
			string modPath = ModDataDirectory;
			int rX = (short)(ShiftX);
			int rY = (short)(ShiftY);
			double _ZoomLevel = ZoomLevel;
			bool isEncoreSet = useEncoreColors;
			int levelSlotNum = LevelID;
			CreateShortcut(dataDir, scenePath, modPath, rX, rY, isEncoreSet, levelSlotNum, _ZoomLevel);
		}

		private void WithoutCurrentCoordinatesToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			string dataDir = DataDirectory;
			string scenePath = Discord.ScenePath;
			string modPath = ModDataDirectory;
			int rX = 0;
			int rY = 0;
			bool isEncoreSet = useEncoreColors;
			int levelSlotNum = LevelID;
			CreateShortcut(dataDir, scenePath, modPath, rX, rY, isEncoreSet, levelSlotNum);
		}

		private void SoundLooperToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			SoundLooper form = new SoundLooper();
			form.ShowDialog();
		}

		#endregion

		#region Apps Tab Buttons

		private void TileManiacToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (mainform == null || mainform.IsClosed) mainform = new TileManiacWPF.MainWindow();
			mainform.Show();
			if (TilesConfig != null && StageTiles != null)
			{
				if (mainform.Visibility != Visibility.Visible || mainform.tcf == null)
				{
					mainform.LoadTileConfigViaIntergration(TilesConfig, SceneFilepath);
				}
				else
				{
					mainform.Activate();
				}

			}

		}

		private void InsanicManiacToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			//Sanic2Maniac sanic = new Sanic2Maniac(null, this);
			//sanic.Show();
		}
		private void RSDKAnnimationEditorToolStripMenuItem_Click(object sender, RoutedEventArgs e)
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
						Filter = "Windows Executable|*.exe"
					};
					if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
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

		private void ColorPaletteEditorToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			MenuItem button = sender as MenuItem;

			if (ManiaPal.MainWindow.Instance == null)
			{
				ManiaPalInstance = new ManiaPal.MainWindow();
				ManiaPalInstance.Show();

				while (ManiaPal.MainWindow.Instance == null)
					Thread.Sleep(100);
				var MP = ManiaPal.MainWindow.Instance;
				if (button != null && button == maniaPalGameConfigToolStripMenuItem && GameConfig != null)
				{
					if (GameConfig.FilePath != null) MP.LoadFile(GameConfig.FilePath);
				}
				else if (StageConfig != null)
				{
					if (StageConfig.FilePath != null) MP.LoadFile(StageConfig.FilePath);
				}

				MP.RefreshPalette(MP.CurrentPaletteSet);
				MP.Activate();
			}
			else
			{
				var MP = ManiaPal.MainWindow.Instance;
				MP.Visibility = System.Windows.Visibility.Visible;
				MP.Activate();


				if (button != null && button == maniaPalStageConfigToolStripMenuItem && StageConfig != null)
				{
					if (StageConfig.FilePath != null) MP.LoadFile(StageConfig.FilePath);
					MP.RefreshPalette(MP.CurrentPaletteSet);
				}

				if (button != null && button == maniaPalGameConfigToolStripMenuItem && GameConfig != null)
				{
					if (GameConfig.FilePath != null) MP.LoadFile(GameConfig.FilePath);
					MP.RefreshPalette(MP.CurrentPaletteSet);
				}

			}


		}

		private void colorPaletteEditorToolStripMenuItem_DropDownOpened(object sender, RoutedEventArgs e)
		{
			maniaPalHint.Header = "HINT: The Button that houses this dropdown" + Environment.NewLine + "will focus ManiaPal if it is opened already" + Environment.NewLine + "(without reloading the currently loaded colors)";
			if (!maniaPalGameConfigToolStripMenuItem.Header.ToString().Contains(Environment.NewLine))
			{
				maniaPalGameConfigToolStripMenuItem.Header += Environment.NewLine + Environment.NewLine;
				maniaPalStageConfigToolStripMenuItem.Header += Environment.NewLine + Environment.NewLine;
			}

		}


		private void DuplicateObjectIDHealerToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			MessageBoxResult result = System.Windows.MessageBox.Show("WARNING: Once you do this the editor will restart immediately, make sure your progress is closed and saved!", "WARNING", MessageBoxButton.OKCancel, MessageBoxImage.Information);
			if (result == MessageBoxResult.OK)
			{
				RepairScene();
			}
		}

		#endregion

		#region Folders Tab Buttons
		private void OpenSceneFolderToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (SceneFilename != null && SceneFilename != "" && File.Exists(SceneFilename))
			{
				string SceneFilename_mod = SceneFilename.Replace('/', '\\');
				Process.Start("explorer.exe", "/select, " + SceneFilename_mod);
			}
			else
			{
				System.Windows.MessageBox.Show("Scene File does not exist or simply isn't loaded!", "ERROR");
			}

		}

		private void OpenDataDirectoryFolderToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			string DataDirectory_mod = DataDirectory.Replace('/', '\\');
			if (DataDirectory_mod != null && DataDirectory_mod != "" && Directory.Exists(DataDirectory_mod))
			{
				Process.Start("explorer.exe", "/select, " + DataDirectory_mod);
			}
			else
			{
				System.Windows.MessageBox.Show("Data Directory does not exist or simply isn't loaded!", "ERROR");
			}

		}

		private void OpenSonicManiaFolderToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (mySettings.RunGamePath != null && mySettings.RunGamePath != "" && File.Exists(mySettings.RunGamePath))
			{
				string GameFolder = mySettings.RunGamePath;
				string GameFolder_mod = GameFolder.Replace('/', '\\');
				Process.Start("explorer.exe", "/select, " + GameFolder_mod);
			}
			else
			{
				System.Windows.MessageBox.Show("Game Folder does not exist or isn't set!", "ERROR");
			}

		}

		private void OpenModDataDirectoryToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (ModDataDirectory != "")
			{
				string ModDataDir = ModDataDirectory.Replace('/', '\\');
				Process.Start("explorer.exe", "/select, " + ModDataDir);
			}
			else
			{
				System.Windows.MessageBox.Show("Mod Data Directory Not Loaded!", "ERROR");
			}


		}
		private void OpenASavedPlaceToolStripMenuItem_DropDownOpening(object sender, RoutedEventArgs e)
		{
			if (Settings.mySettings.SavedPlaces != null && Settings.mySettings.SavedPlaces.Count > 0)
			{
				openASavedPlaceToolStripMenuItem.Items.Clear();
				var allItems = openASavedPlaceToolStripMenuItem.Items.Cast<System.Windows.Controls.MenuItem>().ToArray();
				foreach (string savedPlace in Settings.mySettings.SavedPlaces)
				{
					var savedPlaceItem = new MenuItem()
					{
						Header = savedPlace,
						Tag = savedPlace
					};
					savedPlaceItem.Click += OpenASavedPlaceTrigger;
					openASavedPlaceToolStripMenuItem.Items.Add(savedPlaceItem);
				}
			}

		}

		private void OpenASavedPlaceTrigger(object sender, RoutedEventArgs e)
		{
			MenuItem item = sender as MenuItem;
			string savedPlaceDir = item.Header.ToString().Replace('/', '\\');
			Process.Start("explorer.exe", "/select, " + savedPlaceDir);
		}

		private void OpenASavedPlaceToolStripMenuItem_DropDownClosed(object sender, RoutedEventArgs e)
		{
			openASavedPlaceToolStripMenuItem.Items.Clear();
			openASavedPlaceToolStripMenuItem.Items.Add("No Saved Places");
			//openASavedPlaceToolStripMenuItem.DropDownItems[0].IsEnabled = false;
		}

		#endregion

		#region Other Tab Buttons
		public void AboutToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			var aboutBox = new ManiacEditor.Interfaces.AboutWindow();
			aboutBox.Owner = Window.GetWindow(this);
			aboutBox.ShowDialog();
		}

		private void WikiToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			System.Diagnostics.Process.Start("https://docs.google.com/document/d/1NBvcqzvOzqeTVzgAYBR0ttAc5vLoFaQ4yh_cdf-7ceQ/edit?usp=sharing");
		}

		public void OptionToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			/*
            using (var optionBox = new OptionBox(this))
            {
                optionBox.ShowDialog();
            }
			*/
			var optionMenu = new ManiacEditor.Interfaces.OptionsMenu(this);
			optionMenu.Owner = Window.GetWindow(this);
			optionMenu.ShowDialog();
		}

		private void ControlsToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			var optionMenu = new ManiacEditor.Interfaces.OptionsMenu(this);
			optionMenu.Owner = Window.GetWindow(this);
			optionMenu.MainTabControl.SelectedIndex = 2;
			optionMenu.ShowDialog();
		}
		#endregion

		#region Main Toolstrip Buttons
		private void NewToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			New_Click(sender, e);
		}

		private void SToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			Save_Click(sender, e);
		}

		private void MagnetMode_Click(object sender, RoutedEventArgs e)
		{
			if (UseMagnetMode)
			{
				UseMagnetMode = false;
				MagnetMode.IsChecked = false;
			}
			else
			{
				UseMagnetMode = true;
				MagnetMode.IsChecked = true;
			}
		}

		private void UndoButton_Click(object sender, RoutedEventArgs e)
		{
			EditorUndo();
		}

		private void RedoButton_Click(object sender, RoutedEventArgs e)
		{
			EditorRedo();
		}

		private void ZoomInButton_Click(object sender, RoutedEventArgs e)
		{
			ZoomLevel += 1;
			if (ZoomLevel >= 5) ZoomLevel = 5;
			if (ZoomLevel <= -5) ZoomLevel = -5;

			SetZoomLevel(ZoomLevel, new Point(0, 0));
		}

		private void ZoomOutButton_Click(object sender, RoutedEventArgs e)
		{
			ZoomLevel -= 1;
			if (ZoomLevel >= 5) ZoomLevel = 5;
			if (ZoomLevel <= -5) ZoomLevel = -5;

			SetZoomLevel(ZoomLevel, new Point(0, 0));
		}

		private void SelectTool_Click(object sender, RoutedEventArgs e)
		{
			//SelectToolButton.IsChecked = !SelectToolButton.IsChecked;
			PointerButton.IsChecked = false;
			PlaceTilesButton.IsChecked = false;
			InteractionToolButton.IsChecked = false;
			UpdateControls();
		}

		private void PointerButton_Click(object sender, RoutedEventArgs e)
		{
			//PointerButton.IsChecked = !PointerButton.IsChecked;
			SelectToolButton.IsChecked = false;
			PlaceTilesButton.IsChecked = false;
			InteractionToolButton.IsChecked = false;
			UpdateControls();
		}

		private void PlaceTilesButton_Click(object sender, RoutedEventArgs e)
		{
			//PlaceTilesButton.IsChecked = !PlaceTilesButton.IsChecked;
			SelectToolButton.IsChecked = false;
			PointerButton.IsChecked = false;
			InteractionToolButton.IsChecked = false;
			UpdateControls();
		}

		private void InteractionToolButton_Click(object sender, RoutedEventArgs e)
		{
			//InteractionToolButton.IsChecked = !InteractionToolButton.IsChecked;
			PlaceTilesButton.IsChecked = false;
			SelectToolButton.IsChecked = false;
			PointerButton.IsChecked = false;
			UpdateControls();
		}

		private void ChunkToolButton_Click(object sender, RoutedEventArgs e)
		{
			//ChunksToolButton.IsChecked = !ChunksToolButton.IsChecked;
			UpdateControls();
		}

		public void ReloadToolStripButton_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				// release all our resources, and force a reload of the tiles
				// Entities should take care of themselves
				DisposeTextures();
				EditorEntity_ini.ReleaseResources();
				//EditorEntity_ini.rendersWithErrors.Clear();

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

				TilesConfig = new TileConfig(Path.Combine(SceneFilepath, "TileConfig.bin"));

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
				System.Windows.MessageBox.Show(ex.Message);
			}
		}

		private void RunScene_Click(object sender, RoutedEventArgs e)
		{
			IntPtr hWnd = FindWindow("SonicMania", null); // this gives you the handle of the window you need.
			Process processes = Process.GetProcessesByName("SonicMania").FirstOrDefault();
			if (sender == RunSceneButton && GameRunning)
			{
				RunSceneDropDown.IsSubmenuOpen = true;
				return;
			}
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
					Dispatcher.Invoke(new Action(() => RunSequence(sender, e, true)));
				}
			}
			else
			{
				if (!GameRunning)
				{
					if (mySettings.RunModLoaderPath != null && mySettings.modConfigs?.Count > 0)
					{
						string ConfigPath = mySettings.RunGamePath;
						MenuItem dropDownItem = selectConfigToolStripMenuItem.Items[0] as MenuItem;
						ConfigPath = ConfigPath.Replace('/', '\\');
						ConfigPath = ConfigPath.Replace("SonicMania.exe", "//mods//ManiaModLoader.ini");
						var allItems = selectConfigToolStripMenuItem.Items.Cast<System.Windows.Controls.MenuItem>().ToArray();
						foreach (MenuItem item in allItems)
						{
							if (item.IsChecked)
							{
								dropDownItem = item;
							}
						}
						File.WriteAllText(ConfigPath, dropDownItem.Tag.ToString());
					}
					Dispatcher.Invoke(new Action(() => RunSequence(sender, e)));
				}

			}
		}

		private void RunSceneButton_DropDownOpening(object sender, RoutedEventArgs e)
		{
			trackThePlayerToolStripMenuItem.IsEnabled = GameRunning;
			assetResetToolStripMenuItem1.IsEnabled = GameRunning;
			moveThePlayerToHereToolStripMenuItem.IsEnabled = GameRunning;
			restartSceneToolStripMenuItem1.IsEnabled = GameRunning;
			selectConfigToolStripMenuItem.IsEnabled = !GameRunning;
		}

		public void ShowTileIDButton_Click(object sender, RoutedEventArgs e)
		{
			if (showTileID == false)
			{
				ShowTileIDButton.IsChecked = true;
				ReloadSpecificTextures(sender, e);
				showTileID = true;
			}
			else
			{
				ShowTileIDButton.IsChecked = false;
				ReloadSpecificTextures(sender, e);
				showTileID = false;
			}
		}

		public void ShowGridButton_Click(object sender, RoutedEventArgs e)
		{
			if (showGrid == false)
			{
				ShowGridButton.IsChecked = true;
				showGrid = true;
				GridCheckStateCheck();

			}
			else
			{
				ShowGridButton.IsChecked = false;
				showGrid = false;
			}
		}

		public void ShowCollisionAButton_Click(object sender, RoutedEventArgs e)
		{
			if (showCollisionA == false)
			{
				ShowCollisionAButton.IsChecked = true;
				showCollisionA = true;
				ShowCollisionBButton.IsChecked = false;
				showCollisionB = false;
				ReloadSpecificTextures(sender, e);
			}
			else
			{
				ShowCollisionAButton.IsChecked = false;
				showCollisionA = false;
				ShowCollisionBButton.IsChecked = false;
				showCollisionB = false;
				ReloadSpecificTextures(sender, e);
			}
		}

		public void ShowCollisionBButton_Click(object sender, RoutedEventArgs e)
		{
			if (showCollisionB == false)
			{
				ShowCollisionBButton.IsChecked = true;
				showCollisionB = true;
				ShowCollisionAButton.IsChecked = false;
				showCollisionA = false;
				ReloadSpecificTextures(sender, e);
			}
			else
			{
				ShowCollisionBButton.IsChecked = false;
				showCollisionB = false;
				ShowCollisionAButton.IsChecked = false;
				showCollisionA = false;
				ReloadSpecificTextures(sender, e);
			}
		}

		private void OpenDataDirectoryMenuButton(object sender, RoutedEventArgs e)
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
		private void ResetDeviceButton_Click_1(object sender, RoutedEventArgs e)
		{
			if (editorView.GraphicPanel.bRender)
			{
				editorView.GraphicPanel.bRender = false;
			}
			else
			{
				ReloadToolStripButton_Click(null, null);
				editorView.GraphicPanel.bRender = true;
			}
		}

		private void ShowFlippedTileHelper_Click(object sender, RoutedEventArgs e)
		{
			if (showFlippedTileHelper == false)
			{
				ReloadSpecificTextures(sender, e);
				showFlippedTileHelper = true;

			}
			else
			{
				ReloadSpecificTextures(sender, e);
				showFlippedTileHelper = false;
			}
		}

		private void ResetDeviceButton_Click(object sender, RoutedEventArgs e)
		{
			editorView.GraphicPanel.AttemptRecovery(null);
		}

		private void EnableEncorePalette_Click(object sender, RoutedEventArgs e)
		{
			DisposeTextures();
			if (useEncoreColors == true)
			{
				EncorePaletteButton.IsChecked = false;
				useEncoreColors = false;
				StageTiles?.Image.Reload();
				TilesToolbar?.Reload();
			}
			else
			{
				EncorePaletteButton.IsChecked = true;
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


		private void GraphicPanel_OnRender(object sender, DeviceEventArgs e)
		{
			// hmm, if I call refresh when I update the values, for some reason it will stop to render until I stop calling refrsh
			// So I will refresh it here

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


				if (DebugStatsVisibleOnPanel && EditorScene != null && DebugTextHUD != null && EditorEntity_ini != null)
				{
					Point point = new Point((short)(15), (short)(15));

					DebugTextHUD.DrawEditorHUDText(this, editorView.GraphicPanel, point.X, point.Y, statusBox.GetDataFolder(), true, 255, 15);
					DebugTextHUD.DrawEditorHUDText(this, editorView.GraphicPanel, point.X, point.Y + 12 * 1, statusBox.GetModDataFolder(), true, 255, 19);
					DebugTextHUD.DrawEditorHUDText(this, editorView.GraphicPanel, point.X, point.Y + 12 * 2, statusBox.GetMasterDataFolder(), true, 255, 22);
					DebugTextHUD.DrawEditorHUDText(this, editorView.GraphicPanel, point.X, point.Y + 12 * 3, statusBox.GetScenePath(), true, 255, 11);
					DebugTextHUD.DrawEditorHUDText(this, editorView.GraphicPanel, point.X, point.Y + 12 * 4, statusBox.GetSceneFilePath(), true, 255, 14);
					DebugTextHUD.DrawEditorHUDText(this, editorView.GraphicPanel, point.X, point.Y + 12 * 5, statusBox.GetZoom(), true, 255, 11);
					DebugTextHUD.DrawEditorHUDText(this, editorView.GraphicPanel, point.X, point.Y + 12 * 6, statusBox.GetSetupObject(), true, 255, 13);
					DebugTextHUD.DrawEditorHUDText(this, editorView.GraphicPanel, point.X, point.Y + 12 * 7, statusBox.GetSelectedZone(), true, 255, 14);
					//DebugTextHUD.DrawEditorHUDText(this, editorView.GraphicPanel, point.X, point.Y + 12 * 8, statusBox.GetMemoryUsage(), true, 255, 13);
					//DebugTextHUD.DrawEditorHUDText(this, editorView.GraphicPanel, point.X, point.Y + 12 * 9, statusBox.GetPhysicalMemoryUsage(), true, 255, 22);                   

					//DebugTextHUD.DrawEditorHUDText(this, GraphicPanel, point.X, point.Y + 12 * 11, "Use F1 and F2 to Swap Information", true, 255);
				}



				if (EditorScene.OtherLayers.Contains(EditLayer)) EditLayer.Draw(editorView.GraphicPanel);

				if (!extraLayersMoveToFront)
				{
					foreach (var elb in _extraLayerViewButtons)
					{
						if (elb.IsChecked.Value)
						{
							var _extraViewLayer = EditorScene.OtherLayers.Single(el => el.Name.Equals(elb.Content));
							_extraViewLayer.Draw(editorView.GraphicPanel);
						}
					}
				}

				if (ShowFGLower.IsChecked.Value || EditFGLower.IsChecked.Value)
					FGLower.Draw(editorView.GraphicPanel);

				if (ShowFGLow.IsChecked.Value || EditFGLow.IsChecked.Value)
					FGLow.Draw(editorView.GraphicPanel);

				if (mySettings.PrioritizedObjectRendering && !EditEntities.IsChecked.Value && ShowEntities.IsChecked.Value && entityVisibilityType != 1 && entities != null)
				{
					entities.DrawPriority(editorView.GraphicPanel, -1);
					entities.DrawPriority(editorView.GraphicPanel, 0);
					entities.DrawPriority(editorView.GraphicPanel, 1);
				}

				if (!mySettings.PrioritizedObjectRendering && !EditEntities.IsChecked.Value && ShowEntities.IsChecked.Value && entityVisibilityType == 0 && entities != null) entities.Draw(editorView.GraphicPanel);

				if (ShowFGHigh.IsChecked.Value || EditFGHigh.IsChecked.Value)
					FGHigh.Draw(editorView.GraphicPanel);

				if (mySettings.PrioritizedObjectRendering && !EditEntities.IsChecked.Value && ShowEntities.IsChecked.Value && entityVisibilityType != 1 && entities != null)
				{
					entities.DrawPriority(editorView.GraphicPanel, 2);
					entities.DrawPriority(editorView.GraphicPanel, 3);
				}

				if (ShowFGHigher.IsChecked.Value || EditFGHigher.IsChecked.Value)
					FGHigher.Draw(editorView.GraphicPanel);

				if (extraLayersMoveToFront)
				{
					foreach (var elb in _extraLayerViewButtons)
					{
						if (elb.IsChecked.Value)
						{
							var _extraViewLayer = EditorScene.OtherLayers.Single(el => el.Name.Equals(elb.Content));
							_extraViewLayer.Draw(editorView.GraphicPanel);
						}
					}
				}

				if (EditEntities.IsChecked.Value || (entityVisibilityType == 1 && ShowEntities.IsChecked.Value)) entities.Draw(editorView.GraphicPanel);


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

					editorView.GraphicPanel.DrawRectangle(x1, y1, x2, y2, Color.FromArgb(100, Color.Purple));
					editorView.GraphicPanel.DrawLine(x1, y1, x2, y1, Color.Purple);
					editorView.GraphicPanel.DrawLine(x1, y1, x1, y2, Color.Purple);
					editorView.GraphicPanel.DrawLine(x2, y2, x2, y1, Color.Purple);
					editorView.GraphicPanel.DrawLine(x2, y2, x1, y2, Color.Purple);
				}
			}
			if (!draggingSelection)
			{
				select_x1 = 0;
				select_x2 = 0;
				select_y1 = 0;
				select_y2 = 0;
			}
			bool deviceLost = editorView.GraphicPanel.getDeviceLostState();
			if (scrolling)
			{
				if (editorView.vScrollBar1.IsVisible && editorView.hScrollBar1.IsVisible && !deviceLost) editorView.GraphicPanel.Draw2DCursor(scrollPosition.X, scrollPosition.Y);
				else if (editorView.vScrollBar1.IsVisible && !deviceLost) editorView.GraphicPanel.DrawVertCursor(scrollPosition.X, scrollPosition.Y);
				else if (editorView.hScrollBar1.IsVisible && !deviceLost) editorView.GraphicPanel.DrawHorizCursor(scrollPosition.X, scrollPosition.Y);
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


			Process proc = Process.GetCurrentProcess();
			long memory = proc.PrivateMemorySize64;

			if (!Environment.Is64BitProcess && memory >= 1500000000)
			{
				ReloadToolStripButton_Click(null, null);
			}

			if (ForceWarp)
			{
				if (ShortcutHasZoom) SetZoomLevel(0, TempWarpCoords, ShortcutZoomValue);
				else SetZoomLevel(mySettings.DevForceRestartZoomLevel, TempWarpCoords);
				GoToPosition(TempWarpCoords.X, TempWarpCoords.Y, false, true);
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

		public void GraphicPanel_OnKeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			EditorControls.GraphicPanel_OnKeyDown(sender, e);
		}

		private void EditorViewWPF_KeyDown(object sender, KeyEventArgs e)
		{
			EditorControls.GraphicPanel_OnKeyDown(sender, KeyEventExts.ToWinforms(e));
		}

		public void GraphicPanel_OnKeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			EditorControls.GraphicPanel_OnKeyUp(sender, e);
		}

		private void EditorViewWPF_KeyUp(object sender, KeyEventArgs e)
		{
			EditorControls.GraphicPanel_OnKeyUp(sender, KeyEventExts.ToWinforms(e));
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

			if (ManiaPalInstance != null)
			{
				var MP = ManiaPal.MainWindow.Instance;
				MP.Close();
			}


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
			editorView = null;
			host.Child.Dispose();
			host = null;



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

		private void GraphicPanel_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			editorView.GraphicPanel.Focus();
			if (e.Button == MouseButtons.Right && IsTilesEdit() && InteractionToolButton.IsChecked.Value)
			{
				Point clicked_point_tile = new Point((int)(e.X / Zoom), (int)(e.Y / Zoom));
				int tile = (ushort)(EditLayer?.GetTileAt(clicked_point_tile) & 0x3ff);
				SelectedTileID = tile;
				editTile0WithTileManiacToolStripMenuItem.IsEnabled = (tile < 1023);
				moveThePlayerToHereToolStripMenuItem.IsEnabled = GameRunning;
				setPlayerRespawnToHereToolStripMenuItem.IsEnabled = GameRunning;
				removeCheckpointToolStripMenuItem.IsEnabled = GameRunning && EditorGame.CheckpointEnabled;
				assetResetToolStripMenuItem.IsEnabled = GameRunning;
				restartSceneToolStripMenuItem.IsEnabled = GameRunning;
				moveCheckpointToolStripMenuItem.IsEnabled = GameRunning && EditorGame.CheckpointEnabled;


				editTile0WithTileManiacToolStripMenuItem.Header = String.Format("Edit Tile {0} in Tile Maniac", tile);
				ViewPanelContextMenu.Placement = PlacementMode.Mouse;
				ViewPanelContextMenu.IsOpen = true;
			}
			else if (e.Button == MouseButtons.Right && InteractionToolButton.IsChecked.Value)
			{
				Point clicked_point_tile = new Point((int)(e.X / Zoom), (int)(e.Y / Zoom));
				string tile = "NULL";
				editTile0WithTileManiacToolStripMenuItem.IsEnabled = false;
				moveThePlayerToHereToolStripMenuItem.IsEnabled = GameRunning;
				setPlayerRespawnToHereToolStripMenuItem.IsEnabled = GameRunning;
				moveThisPlayerToolStripMenuItem.IsEnabled = GameRunning;
				moveCheckpointToolStripMenuItem.IsEnabled = GameRunning;

				setPlayerRespawnToHereToolStripMenuItem.IsEnabled = GameRunning;
				removeCheckpointToolStripMenuItem.IsEnabled = GameRunning;
				assetResetToolStripMenuItem.IsEnabled = GameRunning;
				restartSceneToolStripMenuItem.IsEnabled = GameRunning;
				moveCheckpointToolStripMenuItem.IsEnabled = GameRunning;

				editTile0WithTileManiacToolStripMenuItem.Header = String.Format("Edit Tile {0} in Tile Maniac", tile);
				ViewPanelContextMenu.Placement = PlacementMode.Mouse;
				ViewPanelContextMenu.IsOpen = true;

			}

		}

		#endregion

		#region Normal Layer Button Methods
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

		private void LayerEditButton_Click(ToggleButton button)
		{
			Deselect(false);
			if (button.IsChecked.Value)
			{
				button.IsChecked = false;
			}
			else
			{
				EditFGLow.IsChecked = false;
				EditFGHigh.IsChecked = false;
				EditFGLower.IsChecked = false;
				EditFGHigher.IsChecked = false;
				EditEntities.IsChecked = false;
				button.IsChecked = true;
			}

			foreach (var elb in _extraLayerEditButtons)
			{
				elb.IsChecked = false;
			}
			UpdateControls();
		}

		private void EditFGLow_Click(object sender, RoutedEventArgs e)
		{
			ToggleButton toggle = sender as ToggleButton;
			toggle.IsChecked = !toggle.IsChecked.Value;
			LayerEditButton_Click(EditFGLow);
		}

		private void EditFGHigh_Click(object sender, RoutedEventArgs e)
		{
			ToggleButton toggle = sender as ToggleButton;
			toggle.IsChecked = !toggle.IsChecked.Value;
			LayerEditButton_Click(EditFGHigh);
		}

		private void EditFGLower_Click(object sender, RoutedEventArgs e)
		{
			ToggleButton toggle = sender as ToggleButton;
			toggle.IsChecked = !toggle.IsChecked.Value;
			LayerEditButton_Click(EditFGLower);
		}

		private void EditFGHigher_Click(object sender, RoutedEventArgs e)
		{
			ToggleButton toggle = sender as ToggleButton;
			toggle.IsChecked = !toggle.IsChecked.Value;
			LayerEditButton_Click(EditFGHigher);
		}

		private void EditEntities_Click(object sender, RoutedEventArgs e)
		{
			ToggleButton toggle = sender as ToggleButton;
			toggle.IsChecked = !toggle.IsChecked.Value;
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
			//editorView.hScrollBar1.Size = new System.Drawing.Size(100000000, 100000000);
			//editorView.vScrollBar1.Size = new System.Drawing.Size(100000000, 100000000);
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

		#endregion

		#region Asset Reloading
		private void ReloadSpecificTextures(object sender, RoutedEventArgs e)
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
				System.Windows.MessageBox.Show(ex.Message);
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
					if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
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
				string modLoaderDLL = maniaDir + "//d3d9.dll";
				if (File.Exists(modLoaderDLL))
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
						Dispatcher.Invoke(new Action(() =>
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
							if (LevelID != -1)
							{
								GameMemory.WriteByte(CurrentScene_ptr, (byte)LevelID);
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
							if (IsVisible)
							{
								Dispatcher.Invoke(new Action(() => UpdateControls()));
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
					Dispatcher.Invoke(new Action(() => UpdateControls()));
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

		private void PixelModeButton_Click(object sender, RoutedEventArgs e)
		{
			if (EnablePixelCountMode == false)
			{
				pixelModeButton.IsChecked = true;
				pixelModeToolStripMenuItem.IsChecked = true;
				EnablePixelCountMode = true;

			}
			else
			{
				pixelModeButton.IsChecked = false;
				pixelModeToolStripMenuItem.IsChecked = false;
				EnablePixelCountMode = false;
			}

		}

		public void ScrollLockButton_Click(object sender, RoutedEventArgs e)
		{
			if (mySettings.scrollLock == false)
			{
				scrollLockButton.IsChecked = true;
				mySettings.scrollLock = true;
				statusNAToolStripMenuItem.IsChecked = true;
				scrollDirection = "Locked";
			}
			else
			{
				if (mySettings.ScrollLockY == true)
				{
					scrollLockButton.IsChecked = false;
					statusNAToolStripMenuItem.IsChecked = false;
					mySettings.scrollLock = false;
					scrollDirection = "Y";
				}
				else
				{
					scrollLockButton.IsChecked = false;
					statusNAToolStripMenuItem.IsChecked = false;
					mySettings.scrollLock = false;
					scrollDirection = "X";
				}

			}

		}


		public void NudgeFasterButton_Click(object sender, RoutedEventArgs e)
		{
			if (mySettings.EnableFasterNudge == false)
			{
				nudgeFasterButton.IsChecked = true;
				nudgeSelectionFasterToolStripMenuItem.IsChecked = true;
				mySettings.EnableFasterNudge = true;
			}
			else
			{
				nudgeFasterButton.IsChecked = false;
				nudgeSelectionFasterToolStripMenuItem.IsChecked = false;
				mySettings.EnableFasterNudge = false;
			}
		}

		#endregion

		#region Scrollbar Methods

		private void VScrollBar1_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
		{
			ShiftY = (int)e.NewValue;
			editorView.GraphicPanel.Render();
		}

		private void HScrollBar1_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
		{
			ShiftX = (int)e.NewValue;
			editorView.GraphicPanel.Render();
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
		public void BackupTool(object sender, RoutedEventArgs e)
		{

		}

		#endregion

		#region Run Scene Button Methods/Buttons
		private void OpenDataDirectoryButton_DropDownOpened(object sender, RoutedEventArgs e)
		{
			//RecentDataDirectories.AutoToolTip = false;
		}

		private void OpenDataDirectoryButton_DropDownClosed(object sender, RoutedEventArgs e)
		{
			//RecentDataDirectories.AutoToolTip = true;
		}

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

		#endregion

		#region Show Grid Button Methods/Buttons
		private void GridCheckStateCheck()
		{
			if (x16ToolStripMenuItem.IsChecked == true)
			{
				EditorBackground.GRID_TILE_SIZE = 16;
			}
			if (x128ToolStripMenuItem.IsChecked == true)
			{
				EditorBackground.GRID_TILE_SIZE = 128;
			}
			if (x256ToolStripMenuItem.IsChecked == true)
			{
				EditorBackground.GRID_TILE_SIZE = 256;
			}
			if (customToolStripMenuItem.IsChecked == true)
			{
				EditorBackground.GRID_TILE_SIZE = mySettings.CustomGridSizeValue;
			}
		}
		private void X16ToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			EditorBackground.GRID_TILE_SIZE = 16;
			ResetGridOptions();
			x16ToolStripMenuItem.IsChecked = true;
		}

		private void X128ToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			EditorBackground.GRID_TILE_SIZE = 128;
			ResetGridOptions();
			x128ToolStripMenuItem.IsChecked = true;
		}

		private void ResetGridOptions()
		{
			x16ToolStripMenuItem.IsChecked = false;
			x128ToolStripMenuItem.IsChecked = false;
			x256ToolStripMenuItem.IsChecked = false;
			customToolStripMenuItem.IsChecked = false;
		}

		private void X256ToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			EditorBackground.GRID_TILE_SIZE = 256;
			ResetGridOptions();
			x256ToolStripMenuItem.IsChecked = true;
		}

		private void CustomToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			EditorBackground.GRID_TILE_SIZE = mySettings.CustomGridSizeValue;
			ResetGridOptions();
			customToolStripMenuItem.IsChecked = true;
		}
		#endregion

		#region Lower Right Status Bar Quick Options Button
		public void MoreSettingsButton_ButtonClick(object sender, RoutedEventArgs e)
		{
			switch (LastQuickButtonState)
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

		public void SwapScrollLockDirectionToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			LastQuickButtonState = 1;
			XToolStripMenuItem_Click(sender, e);
		}

		public void EditEntitesTransparencyToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (sender != transparentLayersForEditingEntitiesToolStripMenuItem)
			{
				LastQuickButtonState = 2;
			}
			if (applyEditEntitiesTransparency == false)
			{
				applyEditEntitiesTransparency = true;
				transparentLayersForEditingEntitiesToolStripMenuItem.IsChecked = true;
				editEntitesTransparencyToolStripMenuItem.IsChecked = true;
			}
			else
			{
				applyEditEntitiesTransparency = false;
				transparentLayersForEditingEntitiesToolStripMenuItem.IsChecked = false;
				editEntitesTransparencyToolStripMenuItem.IsChecked = false;
			}
		}

		public void ToggleEncoreManiaEntitiesToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			LastQuickButtonState = 3;
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

		#region Developer Stuff

		public void GoToToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			GoToPositionBox form = new GoToPositionBox();
			if (form.ShowDialog().Value == true)
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

		public void PreLoadSceneButton_Click(object sender, RoutedEventArgs e)
		{
			//Disabled By Checking for Result OK
			entities.PreLoadDraw(editorView.GraphicPanel);
		}

		private void DeveloperTerminalToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			DevController.Owner = Window.GetWindow(this);
			DevController.Show();
		}

		private void MD5GeneratorToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			ManiacEditor.Interfaces.WPF_UI.Options___Dev.MD5HashGen hashmap = new ManiacEditor.Interfaces.WPF_UI.Options___Dev.MD5HashGen(this);
			hashmap.Show();
		}

		private void PlayerSpawnToolStripMenuItem_Click(object sender, RoutedEventArgs e)
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
				GoToPlayerBox goToPlayerBox = new GoToPlayerBox(this);
				goToPlayerBox.ShowDialog();
				if (selectPlayerObject_GoTo != -1)
				{
					int objectIndex = selectPlayerObject_GoTo;
					int xPos = (int)((int)playerObjectPosition[objectIndex].Position.X.High);
					int yPos = (int)((int)playerObjectPosition[objectIndex].Position.Y.High);
					GoToPosition(xPos, yPos);
				}
			}
		}

		private void FindToolStripMenuItem1_Click(object sender, RoutedEventArgs e)
		{
			ManiacEditor.Interfaces.WPF_UI.Editor_Tools.FindandReplaceTool form = new ManiacEditor.Interfaces.WPF_UI.Editor_Tools.FindandReplaceTool();
			form.ShowDialog();
			if (form.DialogResult == true)
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

		private void FindUnusedTiles(object sender, RoutedEventArgs e)
		{
			ToggleEditorButtons(false);
			List<int> UnusedTiles = new List<int> { };

			for (int i = 0; i < 1024; i++)
			{
				TilesToolbar.SelectedTileLabel.Content = "Selected Tile: " + i;
				bool Unusued = IsTileUnused(i);
				while (cooldownDone != true)
				{
					//Application.DoEvents();
				}
				cooldownDone = false;
				if (Unusued)
				{
					UnusedTiles.Add(i);
				}
				//Application.DoEvents();
			}
			if (UnusedTiles.Count != 0)
			{
				var message = string.Join(Environment.NewLine, UnusedTiles);
				System.Windows.MessageBox.Show("Tiles not used are: " + Environment.NewLine + message, "Results");
			}
			else
			{
				System.Windows.MessageBox.Show("Found Nothing", "Results");
			}
			ToggleEditorButtons(true);

		}

		private void ConsoleWindowToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (!isConsoleWindowOpen)
			{
				isConsoleWindowOpen = true;
				ShowConsoleWindow();
			}
			else
			{
				isConsoleWindowOpen = false;
				HideConsoleWindow();
			}
		}

		private void SaveForForceOpenOnStartupToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			mySettings.DevForceRestartData = DataDirectory;
			mySettings.DevForceRestartScene = Discord.ScenePath;
			mySettings.DevForceRestartX = (short)(ShiftX / Zoom);
			mySettings.DevForeRestartY = (short)(ShiftY / Zoom);
			mySettings.DevForceRestartZoomLevel = ZoomLevel;
			mySettings.DevForceRestartEncore = encorePaletteExists;
			mySettings.DeveForceRestartLevelID = LevelID;
		}

		//TO-MOVE
		private void EditEntitiesOptionToolStrip_DropDownOpening(object sender, RoutedEventArgs e)
		{

		}





		#endregion

		#region Collision Toolstrip Menu Item Entries

		private void DefaultToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (collisionPreset != 0)
			{
				invertedToolStripMenuItem.IsChecked = false;
				customToolStripMenuItem1.IsChecked = false;
				defaultToolStripMenuItem.IsChecked = true;
				collisionPreset = 0;
				ReloadSpecificTextures(sender, e);
				RefreshCollisionColours(true);
			}
			else
			{
				defaultToolStripMenuItem.IsChecked = true;
				invertedToolStripMenuItem.IsChecked = false;
				customToolStripMenuItem1.IsChecked = false;
				collisionPreset = 0;
				ReloadSpecificTextures(sender, e);
				RefreshCollisionColours(true);
			}
		}

		private void InvertedToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (collisionPreset != 1)
			{
				defaultToolStripMenuItem.IsChecked = false;
				customToolStripMenuItem1.IsChecked = false;
				invertedToolStripMenuItem.IsChecked = true;
				collisionPreset = 1;
				ReloadSpecificTextures(sender, e);
				RefreshCollisionColours(true);
			}
			else
			{
				defaultToolStripMenuItem.IsChecked = true;
				invertedToolStripMenuItem.IsChecked = false;
				customToolStripMenuItem1.IsChecked = false;
				collisionPreset = 0;
				ReloadSpecificTextures(sender, e);
				RefreshCollisionColours(true);
			}
		}

		private void CustomToolStripMenuItem1_Click(object sender, RoutedEventArgs e)
		{
			if (collisionPreset != 2)
			{
				defaultToolStripMenuItem.IsChecked = false;
				invertedToolStripMenuItem.IsChecked = false;
				customToolStripMenuItem1.IsChecked = true;
				collisionPreset = 2;
				ReloadSpecificTextures(sender, e);
				RefreshCollisionColours(true);
			}
			else
			{
				defaultToolStripMenuItem.IsChecked = true;
				invertedToolStripMenuItem.IsChecked = false;
				customToolStripMenuItem1.IsChecked = false;
				collisionPreset = 0;
				ReloadSpecificTextures(sender, e);
				RefreshCollisionColours(true);
			}
		}

		#endregion

		#region Annimations Button Toolstrip Items

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

		#region GetScreen + Get Zoom

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

		public Bitmap GetButtonImage(object sender)
		{
			Bitmap bmpOut = null;
			ToggleButton userControl = sender as ToggleButton;
			if (userControl == null) return null;

			var d = new DataObject(System.Windows.DataFormats.Bitmap, userControl.Content, true);
			bmpOut = d.GetData("System.Drawing.Bitmap") as System.Drawing.Bitmap;

			if (bmpOut == null) return null;

			return bmpOut;
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

		#region Game Manipulation Stuff

		private void SetPlayerRespawnToHereToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			Point clicked_point = new Point((int)(lastX / Zoom), (int)(lastY / Zoom));
			if (GameRunning)
			{
				EditorGame.UpdateCheckpoint(clicked_point);
			}
		}

		private void MoveThisPlayerToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			Point clicked_point = new Point((int)(lastX / Zoom), (int)(lastY / Zoom));
			if (EditorGame.GetPlayerAt(clicked_point) != -1)
			{
				playerSelected = true;
				selectedPlayer = EditorGame.GetPlayerAt(clicked_point);
			}
		}

		private void Player1ToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			playerSelected = true;
			selectedPlayer = 0;
		}

		private void Player2ToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			playerSelected = true;
			selectedPlayer = 1;
		}

		private void Player3ToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			playerSelected = true;
			selectedPlayer = 2;
		}

		private void Player4ToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			playerSelected = true;
			selectedPlayer = 3;
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

		#region Miscellaneous

		private void CreateShortcut(string dataDir, string scenePath = "", string modPath = "", int X = 0, int Y = 0, bool isEncoreMode = false, int LevelSlotNum = -1, double ZoomedLevel = 0.0)
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

		private void MoveThePlayerToHereToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (GameRunning)
			{
				int ObjectAddress = 0x85E9A0;
				GameMemory.WriteInt16(ObjectAddress + 2, (short)(lastX / Zoom));
				GameMemory.WriteInt16(ObjectAddress + 6, (short)(lastY / Zoom));
			}
		}

		private void LangToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			var allLangItems = menuLanguageToolStripMenuItem.Items.Cast<System.Windows.Controls.MenuItem>().ToArray();
			foreach (var item in allLangItems) item.IsChecked = false;
			MenuItem menuItem = sender as MenuItem;
			CurrentLanguage = menuItem.Tag.ToString();
			menuItem.IsChecked = true;
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


		private void New_Click(object sender, RoutedEventArgs e)
		{
			UnloadScene();
			NewSceneWindow makerDialog = new NewSceneWindow();
			makerDialog.Owner = GetWindow(this);
			if (makerDialog.ShowDialog() == true)
			{
				string directoryPath = Path.GetDirectoryName(makerDialog.SceneFolder);
				SelectedZone = (new DirectoryInfo(directoryPath).Name).Replace("\\", "");
				SelectedScene = Path.GetFileName(makerDialog.SceneFolder);
				SceneFilename = "Scene1.bin";
				SceneFilepath = Path.Combine(directoryPath) + "//Scene1.bin";

				EditorScene = new EditorScene(editorView.GraphicPanel, makerDialog.Scene_Width, makerDialog.Scene_Height, makerDialog.BG_Width, makerDialog.BG_Height, this);
				TilesConfig = new TileConfig();
				StageTiles = new StageTiles();
				StageConfig = new StageConfig();

				string ImagePath = directoryPath + "//16x16Tiles.gif";
				string TilesPath = directoryPath + "//TilesConfig.bin";
				string StagePath = directoryPath + "//StageConfig.bin";

				File.Create(SceneFilepath).Dispose();
				File.Create(ImagePath).Dispose();
				File.Create(TilesPath).Dispose();
				File.Create(StagePath).Dispose();

				//EditorScene.Write(SceneFilepath);
				TilesConfig.Write(TilesPath);
				//StageConfig.Write(StagePath);
				StageTiles.Write(ImagePath);


				UpdateDataFolderLabel(null, null);

				SetupLayerButtons();


				EditorBackground = new EditorBackground(this);

				entities = new EditorEntities(EditorScene, this);

				SetViewSize(SceneWidth, SceneHeight);

				UpdateControls(true);
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
					mainform.LoadTileConfigViaIntergration(TilesConfig, SceneFilepath, SelectedTileID);
				}
				else
				{
					mainform.SetCollisionIndex(SelectedTileID);
					mainform.Activate();
				}

			}
		}





		private void EnableAllButtonsToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			object[] MTB = MainToolbarButtons.Items.Cast<object>().ToArray();
			object[] LT = LayerToolbar.Items.Cast<object>().ToArray();
			ManiacEditor.Extensions.EnableButtonList(MTB);
			ManiacEditor.Extensions.EnableButtonList(LT);
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



		private void CollisionOpacitySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			collisionOpacityChanged = true;
		}

		private void LeftToolbarToggleDev_Click(object sender, RoutedEventArgs e)
		{
			UpdateToolbars(false, true);
		}

		private void RightToolbarToggleDev_Click(object sender, RoutedEventArgs e)
		{
			UpdateToolbars(true, true);
		}

		private void MoreSettingsButton_Opened(object sender, RoutedEventArgs e)
		{
			
		}



		private void CollisionColorsToolStripMenuItem_SubmenuClosed(object sender, RoutedEventArgs e)
		{
			if (collisionOpacityChanged)
			{
				collisionOpacityChanged = false;
				ReloadSpecificTextures(sender, e);
				RefreshCollisionColours(true);
			}
		}

		private void CollisionOpacitySlider_DragCompleted(object sender, DragCompletedEventArgs e)
		{
			if (collisionOpacityChanged)
			{
				collisionOpacityChanged = false;
				ReloadSpecificTextures(sender, e);
				RefreshCollisionColours(true);
			}
		}

		private void CollisionOpacitySlider_LostFocus(object sender, RoutedEventArgs e)
		{
			if (collisionOpacityChanged)
			{
				collisionOpacityChanged = false;
				ReloadSpecificTextures(sender, e);
				RefreshCollisionColours(true);
			}
		}



		private void ShowError(string message, string title = "Error!")
		{
			System.Windows.MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
			/*using (var customMsgBox = new CustomMsgBox(message, title, 1, 1))
            {
                customMsgBox.ShowDialog();
            }*/
		}



		public void FreezeDevice(bool state)
		{
			if (state)
			{
				editorView.GraphicPanel.bRender = false;
			}
			else
			{
				ReloadToolStripButton_Click(null, null);
				editorView.GraphicPanel.bRender = true;
			}
		}

		private void SetDeviceSleepState(bool state)
		{
			editorView.GraphicPanel.bRender = state;
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

			var result = System.Windows.MessageBox.Show($"The file '{fullFilePath}' already exists. Overwrite?", "Overwrite?",
										 MessageBoxButton.YesNo, MessageBoxImage.Warning);

			if (result == MessageBoxResult.Yes) return true;

			return false;
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

		private bool AllowSceneUnloading()
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


		private void ManiaPalMenuItem_SubmenuOpened(object sender, RoutedEventArgs e)
		{
			maniaPalHint.Header = "HINT: The Button that houses this dropdown" + Environment.NewLine + "will focus ManiaPal if it is opened already" + Environment.NewLine + "(without reloading the currently loaded colors)";
		}
	}
}
