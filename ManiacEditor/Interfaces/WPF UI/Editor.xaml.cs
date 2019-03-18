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


		//Scroll Lock States
		public int ScrollDirection = 1;
		public bool ScrollLocked = true;


		//Editor Status States (Like are we pre-loading a scene)
		public bool importingObjects = false; //Determines if we are importing objects so we can disable all the other Scene Select Options
		public bool isPreRending = false; //Determines if we are Preloading a Scene
		public bool EncorePaletteExists = false; // Determines if an Encore Pallete Exists
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
		public bool RemoveStageConfigEntriesAllowed = true; //Self Explanatory
		public bool AddStageConfigEntriesAllowed = true; //Self Explanatory
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
        public string LoadedDataPack = "";
		public string[] EncorePalette = new string[6]; //Used to store the location of the encore palletes

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

        public EditorTiles EditorTiles;
		public EditorScene EditorScene;
		public StageConfig StageConfig;
		public GameConfig GameConfig;
		public EditorUIControl UIControl;
		public EditorEntities entities;
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
		public EditorState EditorState;
		public UIText DebugTextHUD = new UIText();
		public EditorChunk EditorChunk;
		public System.Windows.Forms.Integration.WindowsFormsHost host;
		public EditorGraphicsModel GraphicsModel;
		public EditorDiscordRP Discord;
		public EditorUIEvents UIEvents;
		public EditorPath EditorPath;
		public EditorSceneLoading EditorSceneLoading;
		public EditorDirectories EditorDirectories;
		public EditorFindReplace FindAndReplace;
        public EditorViewModel EditorView;
        public EditorTheming EditorTheming;
        public EditorSettings EditorSettings;
        public EditorManiacINI ManiacINI;
        public EditorUI UI;
        public EditorUIModes UIModes;

        //Tile Maniac + ManiaPal Instance
        public TileManiac.MainWindow mainform = new TileManiac.MainWindow();

		//Editor Misc. Variables
		System.Windows.Forms.Timer StatusPanelTickTimer;

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
            EditorTheming = new EditorTheming(this);
            EditorSettings = new EditorSettings(this);

            EditorTheming.UseDarkTheme_WPF(Settings.mySettings.NightMode);
			InitializeComponent();

            System.Windows.Application.Current.MainWindow = this;

            try
            {
                InitilizeEditor();
            }
            catch (Exception ex)
            {
                Debug.Print("Couldn't Initilize Editor!" + ex.ToString());
                throw ex;
            }
            try
            {
                Discord.InitDiscord();
            }
            catch (Exception ex)
            {
                Debug.Print("Discord RP couldn't start! Exception Error:" + ex.ToString());
            }

			if (Settings.mySettings.UseAutoForcefulStartup && Settings.mySettings.UseForcefulStartup) OpenSceneForceFully();

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
			GraphicsModel = new EditorGraphicsModel(this);

			//this.editorView.splitContainer1.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.SplitContainer1_SplitterMoved);
			this.GraphicsModel.vScrollBar1.Scroll += new System.Windows.Controls.Primitives.ScrollEventHandler(this.VScrollBar1_Scroll);
			this.GraphicsModel.vScrollBar1.ValueChanged += new RoutedPropertyChangedEventHandler<double>(this.VScrollBar1_ValueChanged);
			this.GraphicsModel.vScrollBar1.MouseEnter += new System.Windows.Input.MouseEventHandler(this.VScrollBar1_Entered);
			this.GraphicsModel.hScrollBar1.Scroll += new System.Windows.Controls.Primitives.ScrollEventHandler(this.HScrollBar1_Scroll);
			this.GraphicsModel.hScrollBar1.ValueChanged += new RoutedPropertyChangedEventHandler<double>(this.HScrollBar1_ValueChanged);
			this.GraphicsModel.hScrollBar1.MouseEnter += new System.Windows.Input.MouseEventHandler(this.HScrollBar1_Entered);
			this.Activated += new System.EventHandler(this.MapEditor_Activated);
			this.GraphicsModel.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MapEditor_KeyDown);
			this.GraphicsModel.KeyUp += new System.Windows.Forms.KeyEventHandler(this.MapEditor_KeyUp);

			this.GraphicsModel.GraphicPanel.OnRender += new ManiacEditor.RenderEventHandler(this.GraphicPanel_OnRender);
			this.GraphicsModel.GraphicPanel.OnCreateDevice += new ManiacEditor.CreateDeviceEventHandler(this.OnResetDevice);
			this.GraphicsModel.GraphicPanel.DragDrop += new System.Windows.Forms.DragEventHandler(this.GraphicPanel_DragDrop);
			this.GraphicsModel.GraphicPanel.DragEnter += new System.Windows.Forms.DragEventHandler(this.GraphicPanel_DragEnter);
			this.GraphicsModel.GraphicPanel.DragOver += new System.Windows.Forms.DragEventHandler(this.GraphicPanel_DragOver);
			this.GraphicsModel.GraphicPanel.DragLeave += new System.EventHandler(this.GraphicPanel_DragLeave);
			this.GraphicsModel.GraphicPanel.KeyDown += new System.Windows.Forms.KeyEventHandler(this.GraphicPanel_OnKeyDown);
			this.GraphicsModel.GraphicPanel.KeyUp += new System.Windows.Forms.KeyEventHandler(this.GraphicPanel_OnKeyUp);
			this.GraphicsModel.GraphicPanel.MouseClick += new System.Windows.Forms.MouseEventHandler(this.GraphicPanel_MouseClick);
			this.GraphicsModel.GraphicPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.GraphicPanel_OnMouseDown);
			this.GraphicsModel.GraphicPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.GraphicPanel_OnMouseMove);
			this.GraphicsModel.GraphicPanel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.GraphicPanel_OnMouseUp);
			this.GraphicsModel.GraphicPanel.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.GraphicPanel_MouseWheel);

			ExtraLayerEditViewButtons = new Dictionary<EditLayerToggleButton, EditLayerToggleButton>();
			_extraLayerSeperators = new List<Separator>();

			_recentDataItems = new List<MenuItem>();
			_recentDataItems_Button = new List<MenuItem>();
			MenuChar = MenuCharS.ToCharArray();
			MenuChar_Small = MenuCharS_Small.ToCharArray();
			LevelSelectChar = LevelSelectCharS.ToCharArray();
			EditorGame = new EditorInGame(this);
			EditorEntity_ini = new EditorEntity_ini(this);
            EditorState = new EditorState(this);
			UIControl = new EditorUIControl(this);
			StartScreen = new StartScreen(this);
			Discord = new EditorDiscordRP(this);
			Updater = new EditorUpdater();
			UIEvents = new EditorUIEvents(this);
			EditorPath = new EditorPath(this);
			EditorSceneLoading = new EditorSceneLoading(this);
			EditorDirectories = new EditorDirectories(this);
			FindAndReplace = new EditorFindReplace(this);
            EditorView = new EditorViewModel(this);
            ManiacINI = new EditorManiacINI(this);
            UI = new EditorUI(this);
            UIModes = new EditorUIModes(this);


            this.Title = String.Format("Maniac Editor - Generations Edition {0}", Updater.GetVersion());
			GraphicsModel.GraphicPanel.Width = SystemInformation.PrimaryMonitorSize.Width;
			GraphicsModel.GraphicPanel.Height = SystemInformation.PrimaryMonitorSize.Height;

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
            UI.UpdateControls();
			EditorSettings.TryLoadSettings();
			UpdateStartScreen(true, true);

			StatusPanelTickTimer = new System.Windows.Forms.Timer
			{
				Interval = 10
			};
			StatusPanelTickTimer.Tick += new EventHandler(UpdateStatusPanel);
			StatusPanelTickTimer.Start();
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
			else if (IsSceneLoaded() == true && Settings.mySettings.DisableSaveWarnings == false)
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

        #region Refresh UI

        public void UpdateEntitiesToolbarList()
		{
			entitiesToolbar.Entities = entities.Entities.Select(x => x.Entity).ToList();
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

			if (UIModes.EnablePixelCountMode == false)
			{
				selectedPositionLabel.Content = "Selected Tile Position: X: " + (int)EditorState.SelectedTileX + ", Y: " + (int)EditorState.SelectedTileY;
				selectedPositionLabel.ToolTip = "The Position of the Selected Tile";
			}
			else
			{
				selectedPositionLabel.Content = "Selected Tile Pixel Position: " + "X: " + (int)EditorState.SelectedTileX * 16 + ", Y: " + (int)EditorState.SelectedTileY * 16;
				selectedPositionLabel.ToolTip = "The Pixel Position of the Selected Tile";
			}
			if (UIModes.EnablePixelCountMode == false)
			{
				selectionSizeLabel.Content = "Amount of Tiles in Selection: " + (EditorState.SelectedTilesCount - EditorState.DeselectTilesCount);
				selectionSizeLabel.ToolTip = "The Size of the Selection";
			}
			else
			{
				selectionSizeLabel.Content = "Length of Pixels in Selection: " + (EditorState.SelectedTilesCount - EditorState.DeselectTilesCount) * 16;
				selectionSizeLabel.ToolTip = "The Length of all the Tiles (by Pixels) in the Selection";
			}

			selectionBoxSizeLabel.Content = "Selection Box Size: X: " + (EditorState.select_x2 - EditorState.select_x1) + ", Y: " + (EditorState.select_y2 - EditorState.select_y1);

			scrollLockDirLabel.Content = "Scroll Direction: " + (ScrollDirection == (int)ScrollDir.X ? "X" : "Y") + (ScrollLocked ? " (Locked)" : "");


			hVScrollBarXYLabel.Content = "Zoom Value: " + EditorState.Zoom.ToString();

			if (UpdateUpdaterMessage)
			{
				if (StartScreen != null) StartScreen.UpdateStatusLabel(Updater.condition, Updater);
				UpdateUpdaterMessage = false;
			}

			//
			// End of Tooltip Bar Info Section
			//
		}
		public void ToggleEditorButtons(bool enabled)
		{
			MenuBar.IsEnabled = enabled;
			LayerToolbar.IsEnabled = enabled;
			MainToolbarButtons.IsEnabled = enabled;
            UI.SetSceneOnlyButtonsState((enabled ? true : EditorScene != null));
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
                UI.UpdateControls();
			}

		}

		public void FlipEntities(FlipDirection direction)
		{
			Dictionary<EditorEntity, Point> initalPos = new Dictionary<EditorEntity, Point>();
			Dictionary<EditorEntity, Point> postPos = new Dictionary<EditorEntity, Point>();
			foreach (EditorEntity e in entities.SelectedEntities)
			{
				initalPos.Add(e, new Point(e.PositionX, e.PositionY));
			}
			entities.Flip(direction);
			entitiesToolbar.UpdateCurrentEntityProperites();
			foreach (EditorEntity e in entities.SelectedEntities)
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
			var dataDirectories = Settings.mySettings.DataDirectories;
			Settings.mySettings.GamePath = GamePath;
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
			Settings.mySettings.Save();
		}

		public void ResetDataDirectoryToAndResetScene(string newDataDirectory, bool forceBrowse = false, bool forceSceneSelect = false)
		{
			if (AllowSceneUnloading() != true) return;
			UnloadScene();
            EditorSettings.UseDefaultPrefrences();
			DataDirectory = newDataDirectory;
			AddRecentDataFolder(newDataDirectory);
			bool goodGameConfig = SetGameConfig();
			if (goodGameConfig == true)
			{
				if (forceBrowse) OpenScene(true);
				else if (forceSceneSelect) OpenScene(false);
				else OpenScene(Settings.mySettings.forceBrowse);

			}


		}

		public void RecentDataDirectoryClicked(object sender, RoutedEventArgs e)
		{
			var menuItem = sender as MenuItem;
			string dataDirectory = menuItem.Tag.ToString();
			var dataDirectories = Settings.mySettings.DataDirectories;
			Settings.mySettings.GamePath = GamePath;
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
			Settings.mySettings.Save();
		}

		/// <summary>
		/// Refreshes the Data directories displayed in the recent list under the File menu.
		/// </summary>
		/// <param name="settings">The settings file containing the </param>
		public void RefreshDataDirectories(StringCollection recentDataDirectories)
		{
			if (Settings.mySettings.DataDirectories?.Count > 0)
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
			UIModes.ShowingDataDirectory = true;
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
				var dataDirectories = Settings.mySettings.DataDirectories;

				if (dataDirectories == null)
				{
					dataDirectories = new StringCollection();
					Settings.mySettings.DataDirectories = dataDirectories;
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

				Settings.mySettings.Save();

				RefreshDataDirectories(dataDirectories);

				UpdateDataFolderLabel(dataDirectory);


			}
			catch (Exception ex)
			{
				Debug.Write("Failed to add data folder to recent list: " + ex);
			}
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
            EditorSettings.UseDefaultPrefrences();

			ObjectIDHealer healer = new ObjectIDHealer();
			ShowConsoleWindow();
			healer.startHealing(open.FileName);
			HideConsoleWindow();

		}
		public void UnloadScene()
		{
			EditorScene?.Dispose();
			EditorScene = null;
			StageConfig = null;
			_levelIDLabel.Content = "Level ID: NULL";
			LevelID = -1;
			EncorePaletteExists = false;
			EncoreSetupType = 0;
			playerObjectPosition = new List<SceneEntity> { };
			INILayerNameHigher = "";
			INILayerNameLower = "";
			ManiacINI.ClearSettings();
			userDefinedEntityRenderSwaps = new Dictionary<string, string>();
			userDefinedSpritePaths = new List<string>();
			EncorePaletteButton.IsChecked = false;
			EditorPath.UnloadScene();

			if (EditorTiles != null) EditorTiles.Dispose();
            EditorTiles = null;

			TearDownExtraLayerButtons();

			Background = null;

			EditorChunk = null;

			// If copying between scenes is allowed...
			if (Settings.mySettings.ForceCopyUnlock)
			{
				// ...but not for entities...
				if (Settings.mySettings.ProhibitEntityUseOnExternalClipboard)
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

			EditorState.Zoom = 1;
			EditorState.ZoomLevel = 0;

			undo.Clear();
			redo.Clear();

			EditFGLow.ClearCheckedItems();
			EditFGHigh.ClearCheckedItems();
			EditFGLower.ClearCheckedItems();
			EditFGHigher.ClearCheckedItems();
			EditEntities.ClearCheckedItems();

			SetViewSize();

            UI.UpdateControls();

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

			string dataDirectory = Settings.mySettings.DevForceRestartData;
			DataDirectory = dataDirectory;
			string Result = Settings.mySettings.DevForceRestartScene;
			int LevelID = Settings.mySettings.DeveForceRestartLevelID;
			bool isEncore = Settings.mySettings.DevForceRestartEncore;
			string CurrentZone = Settings.mySettings.DevForceRestartCurrentZone;
			string CurrentName = Settings.mySettings.DevForceRestartCurrentName;
			string CurrentSceneID = Settings.mySettings.DevForceRestartCurrentSceneID;
			bool Browsed = Settings.mySettings.DevForceRestartBrowsed;

			int x = Settings.mySettings.DevForceRestartX;
			int y = Settings.mySettings.DevForeRestartY;
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
			host.Child = GraphicsModel;

			host.Foreground = (SolidColorBrush)FindResource("NormalText");

			// Add the interop host control to the Grid
			// control's collection of child controls.
			this.ViewPanelForm.Children.Add(host);

			GraphicsModel.GraphicPanel.Init(GraphicsModel);
		}
		private void GraphicPanel_OnRender(object sender, DeviceEventArgs e)
		{
			// hmm, if I call refresh when I update the values, for some reason it will stop to render until I stop calling refrsh
			// So I will refresh it here

			bool showEntities = ShowEntities.IsChecked.Value && !EditEntities.IsCheckedAll;
			bool showEntitiesEditing = EditEntities.IsCheckedAll;

			bool PriorityMode = Settings.mySettings.PrioritizedObjectRendering;
			bool AboveAllMode = entityVisibilityType == 1;


			if (entitiesToolbar?.NeedRefresh ?? false) entitiesToolbar.PropertiesRefresh();
            if (EditorScene != null)
            {
                if (!isExportingImage)
                {
                    if (!IsTilesEdit())
                        EditorBackground.Draw(GraphicsModel.GraphicPanel);
                    if (IsTilesEdit())
                    {
                        if (Settings.mySettings.ShowEditLayerBackground == true)
                        {
                            EditorBackground.DrawEdit(GraphicsModel.GraphicPanel);
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

                    DebugTextHUD.DrawEditorHUDText(this, GraphicsModel.GraphicPanel, point.X, point.Y, EditorState.GetDataFolder(), true, 255, 15);
                    DebugTextHUD.DrawEditorHUDText(this, GraphicsModel.GraphicPanel, point.X, point.Y + 12 * 1, EditorState.GetMasterDataFolder(), true, 255, 22);
                    DebugTextHUD.DrawEditorHUDText(this, GraphicsModel.GraphicPanel, point.X, point.Y + 12 * 2, EditorState.GetScenePath(), true, 255, 11);
                    DebugTextHUD.DrawEditorHUDText(this, GraphicsModel.GraphicPanel, point.X, point.Y + 12 * 3, EditorState.GetSceneFilePath(), true, 255, 12);
                    DebugTextHUD.DrawEditorHUDText(this, GraphicsModel.GraphicPanel, point.X, point.Y + 12 * 4, EditorState.GetZoom(), true, 255, 11);
                    DebugTextHUD.DrawEditorHUDText(this, GraphicsModel.GraphicPanel, point.X, point.Y + 12 * 5, EditorState.GetSetupObject(), true, 255, 13);
                    DebugTextHUD.DrawEditorHUDText(this, GraphicsModel.GraphicPanel, point.X, point.Y + 12 * 6, EditorState.GetSelectedZone(), true, 255, 14);

                    DebugTextHUD.DrawEditorHUDText(this, GraphicsModel.GraphicPanel, point.X, point.Y + 12 * 8, "Use " + UIControl.KeyBindPraser("StatusBoxToggle") + " to Toggle this Information", true, 255, UIControl.KeyBindPraser("StatusBoxToggle").Length, 4);
                }



                if (EditorScene.OtherLayers.Contains(EditLayer)) EditLayer.Draw(GraphicsModel.GraphicPanel);

                if (!UIModes.ExtraLayersMoveToFront)
                {

                    foreach (var elb in ExtraLayerEditViewButtons)
                    {
                        if (elb.Key.IsCheckedAll || elb.Value.IsCheckedAll)
                        {
                            var _extraViewLayer = EditorScene.OtherLayers.Single(el => el.Name.Equals(elb.Key.Text));
                            _extraViewLayer.Draw(GraphicsModel.GraphicPanel);
                        }
                    }
                }

                if (ShowFGLower.IsChecked.Value || EditFGLower.IsCheckedAll) FGLower.Draw(GraphicsModel.GraphicPanel);
                if (ShowFGLow.IsChecked.Value || EditFGLow.IsCheckedAll) FGLow.Draw(GraphicsModel.GraphicPanel);


                if (showEntities && !AboveAllMode)
                {
                    if (PriorityMode)
                    {
                        entities.DrawPriority(GraphicsModel.GraphicPanel, -1);
                        entities.DrawPriority(GraphicsModel.GraphicPanel, 0);
                        entities.DrawPriority(GraphicsModel.GraphicPanel, 1);
                    }
                    else
                    {
                        entities.Draw(GraphicsModel.GraphicPanel);
                    }
                }

                if (ShowFGHigh.IsChecked.Value || EditFGHigh.IsCheckedAll)
                    FGHigh.Draw(GraphicsModel.GraphicPanel);


                if (showEntities && PriorityMode && !AboveAllMode)
                {
                    entities.DrawPriority(GraphicsModel.GraphicPanel, 2);
                    entities.DrawPriority(GraphicsModel.GraphicPanel, 3);
                }

                if (ShowFGHigher.IsChecked.Value || EditFGHigher.IsCheckedAll)
                    FGHigher.Draw(GraphicsModel.GraphicPanel);

                if (UIModes.ExtraLayersMoveToFront)
                {
                    foreach (var elb in ExtraLayerEditViewButtons)
                    {
                        if (elb.Value.IsCheckedAll || elb.Key.IsCheckedAll)
                        {
                            var _extraViewLayer = EditorScene.OtherLayers.Single(el => el.Name.Equals(elb.Key.Text));
                            _extraViewLayer.Draw(GraphicsModel.GraphicPanel);
                        }
                    }
                }

                if (showEntitiesEditing || AboveAllMode)
                {
                    if (PriorityMode)
                    {
                        entities.DrawPriority(GraphicsModel.GraphicPanel, -1);
                        entities.DrawPriority(GraphicsModel.GraphicPanel, 0);
                        entities.DrawPriority(GraphicsModel.GraphicPanel, 1);
                        entities.DrawPriority(GraphicsModel.GraphicPanel, 2);
                        entities.DrawPriority(GraphicsModel.GraphicPanel, 3);
                    }
                    else
                    {
                        entities.Draw(GraphicsModel.GraphicPanel);
                    }
                }

                if (UIModes.EntitySelectionBoxesAlwaysPrioritized && (showEntities || showEntitiesEditing))
                {
                    entities.DrawSelectionBoxes(GraphicsModel.GraphicPanel);
                }

			}

			if (EditorState.draggingSelection)
			{
				int bound_x1 = (int)(EditorState.selectingX / EditorState.Zoom); int bound_x2 = (int)(EditorState.lastX / EditorState.Zoom);
				int bound_y1 = (int)(EditorState.selectingY / EditorState.Zoom); int bound_y2 = (int)(EditorState.lastY / EditorState.Zoom);
				if (bound_x1 != bound_x2 && bound_y1 != bound_y2)
				{
					if (bound_x1 > bound_x2)
					{
						bound_x1 = (int)(EditorState.lastX / EditorState.Zoom);
						bound_x2 = (int)(EditorState.selectingX / EditorState.Zoom);
					}
					if (bound_y1 > bound_y2)
					{
						bound_y1 = (int)(EditorState.lastY / EditorState.Zoom);
						bound_y2 = (int)(EditorState.selectingY / EditorState.Zoom);
					}


				}

				GraphicsModel.GraphicPanel.DrawRectangle(bound_x1, bound_y1, bound_x2, bound_y2, Color.FromArgb(100, Color.Purple));
				GraphicsModel.GraphicPanel.DrawLine(bound_x1, bound_y1, bound_x2, bound_y1, Color.Purple);
				GraphicsModel.GraphicPanel.DrawLine(bound_x1, bound_y1, bound_x1, bound_y2, Color.Purple);
				GraphicsModel.GraphicPanel.DrawLine(bound_x2, bound_y2, bound_x2, bound_y1, Color.Purple);
				GraphicsModel.GraphicPanel.DrawLine(bound_x2, bound_y2, bound_x1, bound_y2, Color.Purple);
			}
			else
			{
				EditorState.select_x1 = 0; EditorState.select_x2 = 0; EditorState.select_y1 = 0; EditorState.select_y2 = 0;
			}

			if (UIModes.ShowGrid && EditorScene != null)
				EditorBackground.DrawGrid(GraphicsModel.GraphicPanel);

			if (GameRunning)
			{
				EditorGame.DrawGameElements(GraphicsModel.GraphicPanel);

				if (playerSelected)
				{
					EditorGame.MovePlayer(new Point(EditorState.lastX, EditorState.lastY), EditorState.Zoom, selectedPlayer);
				}
				if (checkpointSelected)
				{
					Point clicked_point = new Point((int)(EditorState.lastX / EditorState.Zoom), (int)(EditorState.lastY / EditorState.Zoom));
					EditorGame.UpdateCheckpoint(clicked_point);
				}
			}

			if (EditorState.scrolling)
			{
				if (GraphicsModel.vScrollBar1.IsVisible && GraphicsModel.hScrollBar1.IsVisible) GraphicsModel.GraphicPanel.Draw2DCursor(EditorState.scrollPosition.X, EditorState.scrollPosition.Y);
				else if (GraphicsModel.vScrollBar1.IsVisible) GraphicsModel.GraphicPanel.DrawVertCursor(EditorState.scrollPosition.X, EditorState.scrollPosition.Y);
				else if (GraphicsModel.hScrollBar1.IsVisible) GraphicsModel.GraphicPanel.DrawHorizCursor(EditorState.scrollPosition.X, EditorState.scrollPosition.Y);
			}
			if (ForceWarp)
			{
				if (ShortcutHasZoom) SetZoomLevel(0, TempWarpCoords, ShortcutZoomValue);
				else SetZoomLevel(Settings.mySettings.DevForceRestartZoomLevel, TempWarpCoords);
				GoToPosition(TempWarpCoords.X, TempWarpCoords.Y, false, true);
				SetViewSize((int)(SceneWidth * EditorState.Zoom), (int)(SceneHeight * EditorState.Zoom));

			}
		}
		public void DrawLayers(int drawOrder = 0)
		{
			var _extraViewLayer = EditorScene.LayerByDrawingOrder.FirstOrDefault(el => el.Layer.DrawingOrder.Equals(drawOrder));
			_extraViewLayer.Draw(GraphicsModel.GraphicPanel);
		}
		public void Run()
		{
			Show();
			Focus();
			GraphicsModel.Show();
			GraphicsModel.GraphicPanel.Run();

		}
		private void GraphicPanel_DragEnter(object sender, System.Windows.Forms.DragEventArgs e)
		{
			if (!Settings.mySettings.DisableDraging)
			{
				if (e.Data.GetDataPresent(typeof(Int32)) && IsTilesEdit())
				{
					Point rel = GraphicsModel.GraphicPanel.PointToScreen(Point.Empty);
					e.Effect = System.Windows.Forms.DragDropEffects.Move;
					EditLayer?.StartDragOver(new Point((int)(((e.X - rel.X) + EditorState.ShiftX) / EditorState.Zoom), (int)(((e.Y - rel.Y) + EditorState.ShiftY) / EditorState.Zoom)), (ushort)TilesToolbar.SelectedTile);
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
			if (!Settings.mySettings.DisableDraging)
			{
				if (e.Data.GetDataPresent(typeof(Int32)) && IsTilesEdit())
				{
					Point rel = GraphicsModel.GraphicPanel.PointToScreen(Point.Empty);
					EditLayer?.DragOver(new Point((int)(((e.X - rel.X) + EditorState.ShiftX) / EditorState.Zoom), (int)(((e.Y - rel.Y) + EditorState.ShiftY) / EditorState.Zoom)), (ushort)TilesToolbar.SelectedTile);
                    GraphicsModel.GraphicPanel.Render();

				}
			}
		}
		private void GraphicPanel_DragLeave(object sender, EventArgs e)
		{
			if (!Settings.mySettings.DisableDraging)
			{
				EditLayer?.EndDragOver(true);
			    GraphicsModel.GraphicPanel.Render();
			}
		}
		private void GraphicPanel_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
		{
			if (!Settings.mySettings.DisableDraging)
			{
				EditLayer?.EndDragOver(false);
			}
		}
		public void GraphicPanel_OnKeyDown(object    sender, System.Windows.Forms.KeyEventArgs e)
		{
			UIControl.GraphicPanel_OnKeyDown(sender, e);
		}
		private void EditorViewWPF_KeyDown(object sender, KeyEventArgs e)
		{
			var e2 = KeyEventExts.ToWinforms(e);
			if (e2 != null)
			{
				UIControl.GraphicPanel_OnKeyDown(sender, e2);
			}

		}
		public void GraphicPanel_OnKeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			UIControl.GraphicPanel_OnKeyUp(sender, e);
		}
		private void EditorViewWPF_KeyUp(object sender, KeyEventArgs e)
		{
			var e2 = KeyEventExts.ToWinforms(e);
			if (e2 != null)
			{
				UIControl.GraphicPanel_OnKeyUp(sender, e2);
			}

		}
		private void MapEditor_Activated(object sender, EventArgs e)
		{
			GraphicsModel.GraphicPanel.Focus();
			if (mainform.hasModified)
			{
				ReloadToolStripButton_Click(sender, null);
			}

		}
		private void MapEditor_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (!GraphicsModel.GraphicPanel.Focused)
			{
				UIControl.GraphicPanel_OnKeyDown(sender, e);
			}
		}
		private void Editor_FormClosing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (UIEvents.ManiaPalConnector != null) UIEvents.ManiaPalConnector.Kill();

			try
			{
				GameRunning = false;
				var mySettings = Properties.Settings.Default;
				Settings.mySettings.IsMaximized = WindowState == System.Windows.WindowState.Maximized;
				Settings.mySettings.Save();
			}
			catch (Exception ex)
			{
				Debug.Write("Failed to write settings: " + ex);
			}

			GraphicsModel.Dispose();
			//editorView = null;
			host.Child.Dispose();
			//host = null;



		}
		private void MapEditor_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (!GraphicsModel.GraphicPanel.Focused)
			{
				UIControl.GraphicPanel_OnKeyUp(sender, e);
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
                UI.SetSelectOnlyButtonsState(false);
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
            UI.UpdateControls();
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
            UI.UpdateControls();
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
				if (Settings.mySettings.EnableWindowsClipboard && !doNotUseWindowsClipboard)
					Clipboard.SetDataObject(new DataObject("ManiacTiles", copyData), true);

				// Also copy to Maniac's clipboard in case it gets overwritten elsewhere
				TilesClipboard = copyData;
			}
			else if (hasMultipleValidLayers && MultiLayerEditMode)
			{
				Tuple<Dictionary<Point, ushort>, Dictionary<Point, ushort>> copyData = EditorLayer.CopyMultiSelectionToClipboard(EditLayerA, EditLayerB);

				// Make a DataObject for the copied data and send it to the Windows clipboard for cross-instance copying
				if (Settings.mySettings.EnableWindowsClipboard && !doNotUseWindowsClipboard)
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
				if (Settings.mySettings.EnableWindowsClipboard && !Settings.mySettings.ProhibitEntityUseOnExternalClipboard)
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
				UIModes.UseMagnetMode = false;
			}
			if (nudgeFasterButton.IsChecked == false)
			{
				Settings.mySettings.EnableFasterNudge = false;
				nudgeFasterButton.IsChecked = false;
			}
			if (UIModes.UseMagnetMode)
			{
				switch (e.KeyData)
				{
					case Keys.Up: y = (UIModes.UseMagnetYAxis ? -magnetSize : -1); break;
					case Keys.Down: y = (UIModes.UseMagnetYAxis ? magnetSize : 1); break;
					case Keys.Left: x = (UIModes.UseMagnetXAxis ? -magnetSize : -1); break;
					case Keys.Right: x = (UIModes.UseMagnetXAxis ? magnetSize : 1); break;
				}
			}
			if (Settings.mySettings.EnableFasterNudge)
			{
				if (UIModes.UseMagnetMode)
				{
					switch (e.KeyData)
					{
						case Keys.Up: y = (UIModes.UseMagnetYAxis ? -magnetSize * Settings.mySettings.FasterNudgeValue : -1 - Settings.mySettings.FasterNudgeValue); break;
						case Keys.Down: y = (UIModes.UseMagnetYAxis ? magnetSize * Settings.mySettings.FasterNudgeValue : 1 + Settings.mySettings.FasterNudgeValue); break;
						case Keys.Left: x = (UIModes.UseMagnetXAxis ? -magnetSize * Settings.mySettings.FasterNudgeValue : -1 - Settings.mySettings.FasterNudgeValue); break;
						case Keys.Right: x = (UIModes.UseMagnetXAxis ? magnetSize * Settings.mySettings.FasterNudgeValue : 1 + Settings.mySettings.FasterNudgeValue); break;
					}
				}
				else
				{
					switch (e.KeyData)
					{
						case Keys.Up: y = (-1 - Settings.mySettings.FasterNudgeValue) * modifier; break;
						case Keys.Down: y = (1 + Settings.mySettings.FasterNudgeValue) * modifier; break;
						case Keys.Left: x = (-1 - Settings.mySettings.FasterNudgeValue) * modifier; break;
						case Keys.Right: x = (1 + Settings.mySettings.FasterNudgeValue) * modifier; break;
					}
				}

			}
			if (UIModes.UseMagnetMode == false && Settings.mySettings.EnableFasterNudge == false)
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
				if (UIModes.UseMagnetMode)
				{
					int xE = entities.SelectedEntities[0].Entity.Position.X.High;
					int yE = entities.SelectedEntities[0].Entity.Position.Y.High;

					if (xE % magnetSize != 0 && UIModes.UseMagnetXAxis)
					{
						int offsetX = x % magnetSize;
						x -= offsetX;
					}
					if (yE % magnetSize != 0 && UIModes.UseMagnetYAxis)
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
                    UI.UpdateControls();
				}
			}
		}

		public void MoveCameraFreely(object sender, KeyEventArgs e)
		{
			if (CtrlPressed() && ShiftPressed())
			{
				switch (e.Key)
				{
					case Key.Up: EditorState.CustomY -= 5; break;
					case Key.Down: EditorState.CustomY += 5; break;
					case Key.Left: EditorState.CustomX -= 5; break;
					case Key.Right: EditorState.CustomX += 5; break;
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
				Rectangle r = GraphicsModel.GraphicPanel.GetScreen();
				int x2 = (int)(r.Width * EditorState.Zoom);
				int y2 = (int)(r.Height * EditorState.Zoom);

				int ResultX = (int)(x * EditorState.Zoom) - x2 / 2;
				int ResultY = (int)(y * EditorState.Zoom) - y2 / 2;

				if ((ResultX <= 0)) ResultX = 0;
				if ((ResultY <= 0)) ResultY = 0;


                EditorState.ShiftX = ResultX;
				EditorState.ShiftY = ResultY;
			}
			else
			{
				int ResultX = (int)(x * EditorState.Zoom);
				int ResultY = (int)(y * EditorState.Zoom);

				if ((ResultX <= 0)) ResultX = 0;
				if ((ResultY <= 0)) ResultY = 0;

                EditorState.ShiftX = ResultX;
                EditorState.ShiftY = ResultY;
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

				if (UIModes.UseEncoreColors)
				{
                    EditorTiles.StageTiles?.Image.Reload(EncorePalette[0]);
				}
				else
				{
                    EditorTiles.StageTiles?.Image.Reload();
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
            EditorTiles?.DisposeTextures();
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
						CollisionLayerA.Add(EditorTiles.StageTiles.Config.CollisionPath1[i].DrawCMask(Color.FromArgb(0, 0, 0, 0), CollisionAllSolid));
						CollisionLayerB.Add(EditorTiles.StageTiles.Config.CollisionPath2[i].DrawCMask(Color.FromArgb(0, 0, 0, 0), CollisionAllSolid));
					}
				}
			}

		}
		#endregion

		#region Backup Tool Methods

		public void BackupScene()
		{
			UIModes.BackupType = 1;
			BackupToolStripMenuItem_Click(null, null);
			UIModes.BackupType = 0;
		}
		public void BackupSceneBeforeCrash()
		{
			UIModes.BackupType = 2;
			BackupToolStripMenuItem_Click(null, null);
			UIModes.BackupType = 0;
		}
		public void AutoBackupScene()
		{
			UIModes.BackupType = 3;
			BackupToolStripMenuItem_Click(null, null);
			UIModes.BackupType = 0;
		}
		public void BackupTool(object sender, RoutedEventArgs e)
		{

		}

		#endregion

		#region Get + Set Methods
		public Rectangle GetScreen()
		{
			if (Settings.mySettings.EntityFreeCam && !isExportingImage) return new Rectangle(EditorState.CustomX, EditorState.CustomY, GraphicsModel.mainPanel.Width, GraphicsModel.mainPanel.Height);
			else if (isExportingImage) return new Rectangle(0, 0, SceneWidth, SceneHeight);
			else return new Rectangle(EditorState.ShiftX, EditorState.ShiftY, GraphicsModel.mainPanel.Width, GraphicsModel.mainPanel.Height);
		}
		public double GetZoom()
		{
			return EditorState.Zoom;
		}
		private void SetDeviceSleepState(bool state)
		{
			GraphicsModel.GraphicPanel.bRender = state;
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

		#region Orginized Tab Event Handlers

		#region File Tab Buttons

		private void New_Click(object sender, RoutedEventArgs e) { UIEvents.New_Click(sender, e); }
		public void Open_Click(object sender, RoutedEventArgs e) { UIEvents.Open_Click(sender, e); }
		private void OpenToolStripMenuItem_Click(object sender, RoutedEventArgs e) { UIEvents.OpenToolStripMenuItem_Click(sender, e); }
		public void OpenDataDirectoryToolStripMenuItem_Click(object sender, RoutedEventArgs e) { UIEvents.OpenDataDirectoryToolStripMenuItem_Click(sender, e); }
		public void Save_Click(object sender, RoutedEventArgs e) { UIEvents.Save_Click(sender, e); }
		private void ExitToolStripMenuItem_Click(object sender, RoutedEventArgs e) { UIEvents.ExitToolStripMenuItem_Click(sender, e); }
		private void SaveAspngToolStripMenuItem_Click(object sender, RoutedEventArgs e) { UIEvents.SaveAspngToolStripMenuItem_Click(sender, e); }
		private void ExportEachLayerAspngToolStripMenuItem_Click(object sender, RoutedEventArgs e) { UIEvents.ExportEachLayerAspngToolStripMenuItem_Click(sender, e); }
		public void SaveAsToolStripMenuItem_Click(object sender, RoutedEventArgs e) { UIEvents.SaveAsToolStripMenuItem_Click(sender, e); }
		private void BackupToolStripMenuItem_Click(object sender, RoutedEventArgs e) { UIEvents.BackupToolStripMenuItem_Click(sender, e); }
		private void BackupRecoverButton_Click(object sender, RoutedEventArgs e) { UIEvents.BackupRecoverButton_Click(sender, e); }
		private void ObjectManagerToolStripMenuItem_Click(object sender, RoutedEventArgs e) { UIEvents.ObjectManagerToolStripMenuItem_Click(sender, e); }
		public void UnloadSceneToolStripMenuItem_Click(object sender, RoutedEventArgs e) { UIEvents.UnloadSceneToolStripMenuItem_Click(sender, e); }
		private void StageConfigToolStripMenuItem_Click(object sender, RoutedEventArgs e) { UIEvents.StageConfigToolStripMenuItem_Click(sender, e); }
		private void NormalToolStripMenuItem_Click(object sender, RoutedEventArgs e) { UIEvents.NormalToolStripMenuItem_Click(sender, e); }
		#endregion

		#region Edit Tab Buttons
		public void chunkToolStripMenuItem_Click(object sender, RoutedEventArgs e) { UIEvents.chunkToolStripMenuItem_Click(sender, e); }
		public void SelectAllToolStripMenuItem_Click(object sender, RoutedEventArgs e) { UIEvents.SelectAllToolStripMenuItem_Click(sender, e); }
		public void FlipHorizontalToolStripMenuItem_Click(object sender, RoutedEventArgs e) { UIEvents.FlipHorizontalToolStripMenuItem_Click(sender, e); }
		public void FlipHorizontalIndividualToolStripMenuItem_Click(object sender, RoutedEventArgs e) { UIEvents.FlipHorizontalIndividualToolStripMenuItem_Click(sender, e); }
		private void DeleteToolStripMenuItem_Click(object sender, RoutedEventArgs e) { UIEvents.DeleteToolStripMenuItem_Click(sender, e); }
		public void CopyToolStripMenuItem_Click(object sender, RoutedEventArgs e) { UIEvents.CopyToolStripMenuItem_Click(sender, e); }
		public void DuplicateToolStripMenuItem_Click(object sender, RoutedEventArgs e) { UIEvents.DuplicateToolStripMenuItem_Click(sender, e); }
		private void UndoToolStripMenuItem_Click(object sender, RoutedEventArgs e) { UIEvents.UndoToolStripMenuItem_Click(sender, e); }
		private void RedoToolStripMenuItem_Click(object sender, RoutedEventArgs e) { UIEvents.RedoToolStripMenuItem_Click(sender, e); }
		public void CutToolStripMenuItem_Click(object sender, RoutedEventArgs e) { UIEvents.CutToolStripMenuItem_Click(sender, e); }
		public void PasteToolStripMenuItem_Click(object sender, RoutedEventArgs e) { UIEvents.PasteToolStripMenuItem_Click(sender, e); }
		public void FlipVerticalToolStripMenuItem_Click(object sender, RoutedEventArgs e) { UIEvents.FlipVerticalToolStripMenuItem_Click(sender, e); }
		public void FlipVerticalIndividualToolStripMenuItem_Click(object sender, RoutedEventArgs e) { UIEvents.FlipVerticalIndividualToolStripMenuItem_Click(sender, e); }
		#endregion

		#region View Tab Buttons
		public void statsToolStripMenuItem_Click(object sender, RoutedEventArgs e) { UIEvents.statsToolStripMenuItem_Click(sender, e); }
		private void PointerTooltipToggleToolStripMenuItem_Click(object sender, RoutedEventArgs e) { UIEvents.PointerTooltipToggleToolStripMenuItem_Click(sender, e); }
		private void ResetZoomLevelToolstripMenuItem_Click(object sender, RoutedEventArgs e) { UIEvents.ResetZoomLevelToolstripMenuItem_Click(sender, e); }
		private void useLargeTextToolStripMenuItem_Click(object sender, RoutedEventArgs e) { UIEvents.useLargeTextToolStripMenuItem_Click(sender, e); }
		public void SetMenuButtons(object sender, RoutedEventArgs e) { UIEvents.SetMenuButtons(sender, e); }
        public void SetMenuButtons(string tag) { UIEvents.SetMenuButtons(tag); }
		private void ShowEntitiesAboveAllOtherLayersToolStripMenuItem_Click(object sender, RoutedEventArgs e) { UIEvents.ShowEntitiesAboveAllOtherLayersToolStripMenuItem_Click(sender, e); }
        private void SelectionBoxesAlwaysPrioritized_Click(object sender, RoutedEventArgs e) { UIEvents.SelectionBoxesAlwaysPrioritized_Click(sender, e); }
        private void prioritizedViewingToolStripMenuItem_Click(object sender, RoutedEventArgs e) { UIEvents.prioritizedViewingToolStripMenuItem_Click(sender, e); }
		private void ChangeEncorePaleteToolStripMenuItem_Click(object sender, RoutedEventArgs e) { UIEvents.ChangeEncorePaleteToolStripMenuItem_Click(sender, e); }
		public void SetEncorePallete(object sender = null, string path = "") { UIEvents.SetEncorePallete(sender, path); }
		private void MoveExtraLayersToFrontToolStripMenuItem_Click(object sender, RoutedEventArgs e) { UIEvents.MoveExtraLayersToFrontToolStripMenuItem_Click(sender, e); }
		private void ToolStripTextBox1_TextChanged(object sender, RoutedEventArgs e) { UIEvents.ToolStripTextBox1_TextChanged(sender, e); }
		private void ShowEntitySelectionBoxesToolStripMenuItem_Click(object sender, RoutedEventArgs e) { UIEvents.ShowEntitySelectionBoxesToolStripMenuItem_Click(sender, e); }
		private void ShowWaterLevelToolStripMenuItem_Click(object sender, RoutedEventArgs e) { UIEvents.ShowWaterLevelToolStripMenuItem_Click(sender, e); }
		private void WaterLevelAlwaysShowItem_Click(object sender, RoutedEventArgs e) { UIEvents.WaterLevelAlwaysShowItem_Click(sender, e); }
		private void SizeWithBoundsWhenNotSelectedToolStripMenuItem_Click(object sender, RoutedEventArgs e) { UIEvents.SizeWithBoundsWhenNotSelectedToolStripMenuItem_Click(sender, e); }
		private void ToggleEncoreManiaObjectVisibilityToolStripMenuItem_Click(object sender, RoutedEventArgs e) { UIEvents.ToggleEncoreManiaObjectVisibilityToolStripMenuItem_Click(sender, e); }
		private void ShowParallaxSpritesToolStripMenuItem_Click(object sender, RoutedEventArgs e) { UIEvents.ShowParallaxSpritesToolStripMenuItem_Click(sender, e); }
		private void XToolStripMenuItem_Click(object sender, RoutedEventArgs e) { UIEvents.XToolStripMenuItem_Click(sender, e); }
		private void YToolStripMenuItem_Click(object sender, RoutedEventArgs e) { UIEvents.YToolStripMenuItem_Click(sender, e); }
		private void ShowEntityPathToolStripMenuItem_Click(object sender, RoutedEventArgs e) { UIEvents.ShowEntityPathToolStripMenuItem_Click(sender, e); }
		private void LangToolStripMenuItem_Click(object sender, RoutedEventArgs e) { UIEvents.LangToolStripMenuItem_Click(sender, e); }

		#region Collision Options
		private void DefaultToolStripMenuItem_Click(object sender, RoutedEventArgs e) { UIEvents.DefaultToolStripMenuItem_Click(sender, e); }
		private void InvertedToolStripMenuItem_Click(object sender, RoutedEventArgs e) { UIEvents.InvertedToolStripMenuItem_Click(sender, e); }
		private void CustomToolStripMenuItem1_Click(object sender, RoutedEventArgs e) { UIEvents.CustomToolStripMenuItem1_Click(sender, e); }
		private void CollisionOpacitySlider_DragCompleted(object sender, DragCompletedEventArgs e) { UIEvents.CollisionOpacitySlider_DragCompleted(sender, e); }
		private void CollisionOpacitySlider_LostFocus(object sender, RoutedEventArgs e) { UIEvents.CollisionOpacitySlider_LostFocus(sender, e); }
		private void CollisionOpacitySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) { UIEvents?.CollisionOpacitySlider_ValueChanged(sender, e); }
		#endregion

		#endregion

		#region Scene Tab Buttons
		public void ImportObjectsToolStripMenuItem_Click(object sender, RoutedEventArgs e, Window window = null) { UIEvents.ImportObjectsToolStripMenuItem_Click(sender, e); }
		public void ImportSoundsToolStripMenuItem_Click(object sender, RoutedEventArgs e) { UIEvents.ImportSoundsToolStripMenuItem_Click(sender, e); }
		public void ImportSoundsToolStripMenuItem_Click(object sender, RoutedEventArgs e, Window window = null) { UIEvents.ImportSoundsToolStripMenuItem_Click(sender, e, window); }
		private void LayerManagerToolStripMenuItem_Click(object sender, RoutedEventArgs e) { UIEvents.LayerManagerToolStripMenuItem_Click(sender, e); }

        private void ManiacinieditorToolStripMenuItem_Click(object sender, RoutedEventArgs e) { UIEvents.ManiacinieditorToolStripMenuItem_Click(sender, e); }
        private void PrimaryColorToolStripMenuItem_Click(object sender, RoutedEventArgs e) { UIEvents.PrimaryColorToolStripMenuItem_Click(sender, e); }
		private void SecondaryColorToolStripMenuItem_Click(object sender, RoutedEventArgs e) { UIEvents.SecondaryColorToolStripMenuItem_Click(sender, e); }
		#endregion

		#region Tools Tab Buttons
		private void OptimizeEntitySlotIDsToolStripMenuItem_Click(object sender, RoutedEventArgs e) { UIEvents.OptimizeEntitySlotIDsToolStripMenuItem_Click(sender, e); }
		private void RightClicktoSwapSlotIDs_Click(object sender, RoutedEventArgs e) { UIEvents.RightClicktoSwapSlotIDs_Click(sender, e); }
		private void CopyAirToggle_Click(object sender, RoutedEventArgs e) { UIEvents.CopyAirToggle_Click(sender, e); }
		private void changeLevelIDToolStripMenuItem_Click(object sender, RoutedEventArgs e) { UIEvents.changeLevelIDToolStripMenuItem_Click(sender, e); }
		private void MultiLayerSelectionToolStripMenuItem_Click(object sender, RoutedEventArgs e) { UIEvents.MultiLayerSelectionToolStripMenuItem_Click(sender, e); }
		private void MakeForDataFolderOnlyToolStripMenuItem_Click(object sender, RoutedEventArgs e) { UIEvents.MakeForDataFolderOnlyToolStripMenuItem_Click(sender, e); }
		private void WithCurrentCoordinatesToolStripMenuItem_Click(object sender, RoutedEventArgs e) { UIEvents.WithCurrentCoordinatesToolStripMenuItem_Click(sender, e); }
		private void WithoutCurrentCoordinatesToolStripMenuItem_Click(object sender, RoutedEventArgs e) { UIEvents.WithoutCurrentCoordinatesToolStripMenuItem_Click(sender, e); }
		private void SoundLooperToolStripMenuItem_Click(object sender, RoutedEventArgs e) { UIEvents.SoundLooperToolStripMenuItem_Click(sender, e); }
		private void FindUnusedTiles(object sender, RoutedEventArgs e) { UIEvents.FindUnusedTiles(sender, e); }

		#region Developer Stuff
		public void GoToToolStripMenuItem_Click(object sender, RoutedEventArgs e) { UIEvents.GoToToolStripMenuItem_Click(sender, e); }
		public void PreLoadSceneButton_Click(object sender, RoutedEventArgs e) { UIEvents.PreLoadSceneButton_Click(sender, e); }
		private void DeveloperTerminalToolStripMenuItem_Click(object sender, RoutedEventArgs e) { UIEvents.DeveloperTerminalToolStripMenuItem_Click(sender, e); }
		private void MD5GeneratorToolStripMenuItem_Click(object sender, RoutedEventArgs e) { UIEvents.MD5GeneratorToolStripMenuItem_Click(sender, e); }
		private void PlayerSpawnToolStripMenuItem_Click(object sender, RoutedEventArgs e) { UIEvents.PlayerSpawnToolStripMenuItem_Click(sender, e); }
		private void FindToolStripMenuItem1_Click(object sender, RoutedEventArgs e) { UIEvents.FindToolStripMenuItem1_Click(sender, e); }
		private void ConsoleWindowToolStripMenuItem_Click(object sender, RoutedEventArgs e) { UIEvents.ConsoleWindowToolStripMenuItem_Click(sender, e); }
		private void SaveForForceOpenOnStartupToolStripMenuItem_Click(object sender, RoutedEventArgs e) { UIEvents.SaveForForceOpenOnStartupToolStripMenuItem_Click(sender, e); }
		private void LeftToolbarToggleDev_Click(object sender, RoutedEventArgs e) { UI.UpdateToolbars(false, true); } 
		private void RightToolbarToggleDev_Click(object sender, RoutedEventArgs e) { UI.UpdateToolbars(true, true); }
		private void EnableAllButtonsToolStripMenuItem_Click(object sender, RoutedEventArgs e) { UIEvents.EnableAllButtonsToolStripMenuItem_Click(sender, e); }
		private void NextGenSceneSelectTest_Click(object sender, RoutedEventArgs e) { EditorSceneLoading.OpenSceneUsingSceneSelect(); }
		#endregion

		#endregion

		#region Apps Tab Buttons
		private void TileManiacToolStripMenuItem_Click(object sender, RoutedEventArgs e) { UIEvents.TileManiacToolStripMenuItem_Click(sender, e); }
		private void InsanicManiacToolStripMenuItem_Click(object sender, RoutedEventArgs e) { UIEvents.InsanicManiacToolStripMenuItem_Click(sender, e); }
		private void RSDKAnnimationEditorToolStripMenuItem_Click(object sender, RoutedEventArgs e) { UIEvents.RSDKAnnimationEditorToolStripMenuItem_Click(sender, e); }
        private void RenderListManagerToolstripMenuItem_Click(object sender, RoutedEventArgs e) { UIEvents.RenderListManagerToolstripMenuItem_Click(sender, e); }
        private void ColorPaletteEditorToolStripMenuItem_Click(object sender, RoutedEventArgs e) { UIEvents.ColorPaletteEditorToolStripMenuItem_Click(sender, e); }
		private void ManiaPalMenuItem_SubmenuOpened(object sender, RoutedEventArgs e) { UIEvents.ManiaPalMenuItem_SubmenuOpened(sender, e); }
		private void DuplicateObjectIDHealerToolStripMenuItem_Click(object sender, RoutedEventArgs e) { UIEvents.DuplicateObjectIDHealerToolStripMenuItem_Click(sender, e); }
		#endregion

		#region Folders Tab Buttons
		private void OpenSceneFolderToolStripMenuItem_Click(object sender, RoutedEventArgs e) { UIEvents.OpenSceneFolderToolStripMenuItem_Click(sender, e); }
		private void OpenDataDirectoryFolderToolStripMenuItem_Click(object sender, RoutedEventArgs e) { UIEvents.OpenDataDirectoryFolderToolStripMenuItem_Click(sender, e); }
		private void OpenSonicManiaFolderToolStripMenuItem_Click(object sender, RoutedEventArgs e) { UIEvents.OpenSonicManiaFolderToolStripMenuItem_Click(sender, e); }
		private void OpenModDataDirectoryToolStripMenuItem_Click(object sender, RoutedEventArgs e) { UIEvents.OpenModDataDirectoryToolStripMenuItem_Click(sender, e); }
		private void OpenASavedPlaceToolStripMenuItem_DropDownOpening(object sender, RoutedEventArgs e) { UIEvents.OpenASavedPlaceToolStripMenuItem_DropDownOpening(sender, e); }
		private void OpenASavedPlaceToolStripMenuItem_DropDownClosed(object sender, RoutedEventArgs e) { UIEvents.OpenASavedPlaceToolStripMenuItem_DropDownClosed(sender, e); }
		#endregion

		#region Other Tab Buttons
		public void AboutToolStripMenuItem_Click(object sender, RoutedEventArgs e) { UIEvents.AboutToolStripMenuItem_Click(sender, e); }
		private void WikiToolStripMenuItem_Click(object sender, RoutedEventArgs e) { UIEvents.WikiToolStripMenuItem_Click(sender, e); }
		public void OptionToolStripMenuItem_Click(object sender, RoutedEventArgs e) { UIEvents.OptionToolStripMenuItem_Click(sender, e); }
		private void ControlsToolStripMenuItem_Click(object sender, RoutedEventArgs e) { UIEvents.ControlsToolStripMenuItem_Click(sender, e); }
		#endregion

		#endregion

		#region Main Toolstrip Item's Event Handlers
		private void NewToolStripMenuItem_Click(object sender, RoutedEventArgs e) { UIEvents.NewToolStripMenuItem_Click(sender, e); }
		private void SToolStripMenuItem_Click(object sender, RoutedEventArgs e) { UIEvents.SToolStripMenuItem_Click(sender, e); }
		private void MagnetMode_Click(object sender, RoutedEventArgs e) { UIEvents.MagnetMode_Click(sender, e); }
		private void UndoButton_Click(object sender, RoutedEventArgs e) { UIEvents.UndoButton_Click(sender, e); }
		private void RedoButton_Click(object sender, RoutedEventArgs e) { UIEvents.RedoButton_Click(sender, e); }
		private void ZoomInButton_Click(object sender, RoutedEventArgs e) { UIEvents.ZoomInButton_Click(sender, e); }
		private void ZoomOutButton_Click(object sender, RoutedEventArgs e) { UIEvents.ZoomOutButton_Click(sender, e); }
		private void SelectTool_Click(object sender, RoutedEventArgs e) { UIEvents.SelectTool_Click(sender, e); }
		private void PointerButton_Click(object sender, RoutedEventArgs e) { UIEvents.PointerButton_Click(sender, e); }
		private void PlaceTilesButton_Click(object sender, RoutedEventArgs e) { UIEvents.PlaceTilesButton_Click(sender, e); }
		private void InteractionToolButton_Click(object sender, RoutedEventArgs e) { UIEvents.InteractionToolButton_Click(sender, e); }
		private void ChunkToolButton_Click(object sender, RoutedEventArgs e) { UIEvents.ChunkToolButton_Click(sender, e); }
		public void ReloadToolStripButton_Click(object sender, RoutedEventArgs e) { UIEvents.ReloadToolStripButton_Click(sender, e); }
		public void ShowTileIDButton_Click(object sender, RoutedEventArgs e) { UIModes.ShowTileID = ShowTileIDButton.IsChecked.Value; }

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
			if (UIModes.UseMagnetXAxis)
			{
				enableXAxisToolStripMenuItem.IsChecked = false;
				UIModes.UseMagnetXAxis = false;
			}
			else
			{
				enableXAxisToolStripMenuItem.IsChecked = true;
				UIModes.UseMagnetXAxis = true;
			}
		}

		private void EnableYAxisToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (UIModes.UseMagnetYAxis)
			{
				enableYAxisToolStripMenuItem.IsChecked = false;
				UIModes.UseMagnetYAxis = false;
			}
			else
			{
				enableYAxisToolStripMenuItem.IsChecked = true;
				UIModes.UseMagnetYAxis = true;
			}
		}

		#endregion

		#region Run Scene Events
		private void RunScene_Click(object sender, RoutedEventArgs e) { UIEvents.RunScene_Click(sender, e); }
		private void OpenModManagerToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			String modProcessName = Path.GetFileNameWithoutExtension(Settings.mySettings.RunModLoaderPath);
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
				if (string.IsNullOrEmpty(Settings.mySettings.RunModLoaderPath))
				{
					var ofd = new OpenFileDialog
					{
						Title = "Select Mania Mod Manager.exe",
						Filter = "Windows PE Executable|*.exe"
					};
					if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
						Settings.mySettings.RunModLoaderPath = ofd.FileName;
				}
				else
				{
					if (!File.Exists(Settings.mySettings.RunGamePath))
					{
						Settings.mySettings.RunModLoaderPath = "";
						return;
					}
				}

				if (File.Exists(Settings.mySettings.RunModLoaderPath))
					Process.Start(Settings.mySettings.RunModLoaderPath);
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
		private void RunSceneButton_DropDownOpening(object sender, RoutedEventArgs e) { UIEvents.RunSceneButton_DropDownOpening(sender, e); }

		#region Mod Config List Stuff
		public MenuItem CreateModConfigMenuItem(int i)
		{
			MenuItem newItem = new MenuItem()
			{
				Header = Settings.mySettings.modConfigsNames[i],
				Tag = Settings.mySettings.modConfigs[i]
			};
			newItem.Click += ModConfigItemClicked;
			if (newItem.Tag.ToString() == Settings.mySettings.LastModConfig) newItem.IsChecked = true;
			return newItem;
		}

		private void ModConfigItemClicked(object sender, RoutedEventArgs e)
		{
			var modConfig_CheckedItem = (sender as MenuItem);
			SelectConfigToolStripMenuItem_Click(modConfig_CheckedItem);
			Settings.mySettings.LastModConfig = modConfig_CheckedItem.Tag.ToString();
		}

		public void EditConfigsToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			Interfaces.WPF_UI.ConfigManager configManager = new Interfaces.WPF_UI.ConfigManager();
			configManager.Owner = GetWindow(this);
			configManager.ShowDialog();

			// TODO: Fix NullReferenceException on Settings.mySettings.modConfigs
			selectConfigToolStripMenuItem.Items.Clear();
			for (int i = 0; i < Settings.mySettings.modConfigs.Count; i++)
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

        private void GameOptionsMenuItem_Click(object sender, RoutedEventArgs e) { UIEvents.GameOptionsMenuItem_Click(sender, e); }

        #endregion

        #region Grid Options
        public void ShowGridButton_Click(object sender, RoutedEventArgs e) { UIModes.ShowGrid = ShowGridButton.IsChecked.Value; }
		private void X16ToolStripMenuItem_Click(object sender, RoutedEventArgs e) { UIEvents.X16ToolStripMenuItem_Click(sender, e); }
		private void X128ToolStripMenuItem_Click(object sender, RoutedEventArgs e) { UIEvents.X128ToolStripMenuItem_Click(sender, e); }
		private void X256ToolStripMenuItem_Click(object sender, RoutedEventArgs e) { UIEvents.X256ToolStripMenuItem_Click(sender, e); }
		private void CustomToolStripMenuItem_Click(object sender, RoutedEventArgs e) { UIEvents.CustomToolStripMenuItem_Click(sender, e); }
        #endregion

        #region Main Toolbar Buttons
        public void ShowCollisionAButton_Click(object sender, RoutedEventArgs e) { UIModes.ShowCollisionA = ShowCollisionAButton.IsChecked.Value; }
		public void ShowCollisionBButton_Click(object sender, RoutedEventArgs e) { UIModes.ShowCollisionB = ShowCollisionAButton.IsChecked.Value; }
		private void OpenDataDirectoryMenuButton(object sender, RoutedEventArgs e) { UIEvents.OpenDataDirectoryMenuButton(sender, e); }
		private void ResetDeviceButton_Click_1(object sender, RoutedEventArgs e) { UIEvents.ResetDeviceButton_Click_1(sender, e); }
		private void ShowFlippedTileHelper_Click(object sender, RoutedEventArgs e) { UIEvents.ShowFlippedTileHelper_Click(sender, e); }
		public void EnableEncorePalette_Click(object sender, RoutedEventArgs e) { UIModes.UseEncoreColors = EncorePaletteButton.IsChecked.Value; }
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
            UI.UpdateControls();



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
					if (!Settings.mySettings.KeepLayersVisible)
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
					if (!Settings.mySettings.KeepLayersVisible)
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
					if (!Settings.mySettings.KeepLayersVisible)
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

            UI.UpdateControls();
		}
		#endregion

		#region Mouse Actions Event Handlers
		private void GraphicPanel_OnMouseMove(object sender, System.Windows.Forms.MouseEventArgs e) { UIControl.MouseMove(sender, e); }
		private void GraphicPanel_OnMouseDown(object sender, System.Windows.Forms.MouseEventArgs e) { UIControl.MouseDown(sender, e); }
		private void GraphicPanel_OnMouseUp(object sender, System.Windows.Forms.MouseEventArgs e) { UIControl.MouseUp(sender, e); }
		private void GraphicPanel_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e) { UIControl.MouseWheel(sender, e); }
		private void GraphicPanel_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e) { UIControl.MouseClick(sender, e); }
		#endregion

		#region Status Bar Event Handlers
		private void PixelModeButton_Click(object sender, RoutedEventArgs e) { UIEvents.PixelModeButton_Click(sender, e); }
		public void TooltipButton_Click(object sender, RoutedEventArgs e) { UIEvents.TooltipButton_Click(sender, e); }

        public void ScrollLockButton_Click(object sender, RoutedEventArgs e) { UIEvents.ScrollLockButton_Click(sender, e); }
		public void NudgeFasterButton_Click(object sender, RoutedEventArgs e) { UIEvents.NudgeFasterButton_Click(sender, e); }

		#region Quick Button Event Handlers
		public void MoreSettingsButton_ButtonClick(object sender, RoutedEventArgs e) { UIEvents.MoreSettingsButton_ButtonClick(sender, e); }
		public void SwapScrollLockDirectionToolStripMenuItem_Click(object sender, RoutedEventArgs e) { UIEvents.SwapScrollLockDirectionToolStripMenuItem_Click(sender, e); }
		public void EditEntitesTransparencyToolStripMenuItem_Click(object sender, RoutedEventArgs e) { UIEvents.EditEntitesTransparencyToolStripMenuItem_Click(sender, e); }
		public void ToggleEncoreManiaEntitiesToolStripMenuItem_Click(object sender, RoutedEventArgs e) { UIEvents.ToggleEncoreManiaEntitiesToolStripMenuItem_Click(sender, e); }
		#endregion

		#endregion

		#region Interaction Tool Items
		private void MoveThePlayerToHereToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (GameRunning)
			{
				int ObjectAddress = 0x85E9A0;
				GameMemory.WriteInt16(ObjectAddress + 2, (short)(EditorState.lastX / EditorState.Zoom));
				GameMemory.WriteInt16(ObjectAddress + 6, (short)(EditorState.lastY / EditorState.Zoom));
			}
		}

		private void EditTileWithTileManiacToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (mainform == null || mainform.IsClosed) mainform = new TileManiac.MainWindow();
			if (mainform.Visibility != Visibility.Visible)
			{
				mainform.Show();
			}
			mainform.SetIntergrationNightMode(Properties.Settings.Default.NightMode);
			if (TilesConfig != null && EditorTiles.StageTiles != null)
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
			Point clicked_point = new Point((int)(EditorState.lastX / EditorState.Zoom), (int)(EditorState.lastY / EditorState.Zoom));
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

        #region Temporary Region

        private void Spliter_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Form1_Resize(null, null);
        }

        private void Spliter_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetZoomLevel(EditorState.ZoomLevel, new System.Drawing.Point(EditorState.ShiftX, EditorState.ShiftY), 0.0, false);
        }

        public void SetViewSize(int width = 0, int height = 0, bool resizeForm = true) { EditorView.SetViewSize(width, height, resizeForm); }

        public void Form1_Resize(object sender, RoutedEventArgs e) { EditorView.Form1_Resize(sender, e); }

        #region Scrollbar Methods

        private void VScrollBar1_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e) { EditorView.VScrollBar1_Scroll(sender, e); }

        private void HScrollBar1_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e) { EditorView.HScrollBar1_Scroll(sender, e); }


        private void VScrollBar1_ValueChanged(object sender, RoutedEventArgs e) { EditorView.VScrollBar1_ValueChanged(sender, e); }

        private void HScrollBar1_ValueChanged(object sender, RoutedEventArgs e) { EditorView.HScrollBar1_ValueChanged(sender, e); }


        private void VScrollBar1_Entered(object sender, EventArgs e) { EditorView.VScrollBar1_Entered(sender, e); }

        private void HScrollBar1_Entered(object sender, EventArgs e) { EditorView.HScrollBar1_Entered(sender, e); }

        #region Zooming/Resizing Related Methods

        public void SetZoomLevel(int zoom_level, Point zoom_point, double zoom_level_d = 0.0, bool updateControls = true) { EditorView.SetZoomLevel(zoom_level, zoom_point, zoom_level_d, updateControls); }

        public void ResetViewSize() { EditorView.ResetViewSize(); }
        public void ResizeGraphicPanel(int width = 0, int height = 0) { EditorView.ResizeGraphicPanel(width, height); }

        #endregion

        #endregion

        #endregion
    }
}
