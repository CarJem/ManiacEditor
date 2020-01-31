using IWshRuntimeLibrary;
using ManiacEditor.Actions;
using ManiacEditor.Entity_Renders;
using ManiacEditor.Interfaces;
using Microsoft.Scripting.Utils;
using Microsoft.Win32;
using RSDKv5;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using Clipboard = System.Windows.Clipboard;
using Color = System.Drawing.Color;
using DataObject = System.Windows.DataObject;
using File = System.IO.File;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MenuItem = System.Windows.Controls.MenuItem;
using Path = System.IO.Path;
using Point = System.Drawing.Point;
using Rectangle = System.Drawing.Rectangle;


namespace ManiacEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Editor : Window
	{
        #region Classical Regions
        #region Definitions
        public static Editor Instance;

		//Editor Paths
		public string DataDirectory; //Used to get the current Data Directory
		public string MasterDataDirectory = Environment.CurrentDirectory + "\\Data"; //Used as a way of allowing mods to not have to lug all the files in their folder just to load in Maniac.
		public IList<string> ResourcePackList { get; set; } = new List<string>();
        public string LoadedDataPack = "";
		public string[] EncorePalette = new string[6]; //Used to store the location of the encore palletes

		// Extra Layer Buttons
		public IDictionary<EditLayerToggleButton, EditLayerToggleButton> ExtraLayerEditViewButtons;
		public IList<Separator> ExtraLayerSeperators; //Used for Adding Extra Seperators along side Extra Edit/View Layer Buttons

		// Editor Collections
		public List<string> ObjectList = new List<string>(); //All Gameconfig + Stageconfig Object names (Unused)
        public IList<Tuple<MenuItem, MenuItem>> RecentSceneItems;
        public IList<Tuple<MenuItem, MenuItem>> RecentDataSourceItems;
		public List<string> userDefinedSpritePaths = new List<string>();
		public Dictionary<string, string> userDefinedEntityRenderSwaps = new Dictionary<string, string>();
        public System.ComponentModel.BindingList<TextBlock> SplineSelectedObjectSpawnList = new System.ComponentModel.BindingList<TextBlock>();
        public System.Timers.Timer Timer = new System.Timers.Timer();

        //Undo + Redo
        public Stack<IAction> UndoStack { get; set; } = new Stack<IAction>(); //Undo Actions Stack
        public Stack<IAction> RedoStack = new Stack<IAction>(); //Redo Actions Stack

        //Editor Layers
        internal EditorSolution.EditorLayer FGHigher => EditorSolution.CurrentScene?.HighDetails;
		internal EditorSolution.EditorLayer FGHigh => EditorSolution.CurrentScene?.ForegroundHigh;
		internal EditorSolution.EditorLayer FGLow => EditorSolution.CurrentScene?.ForegroundLow;
		internal EditorSolution.EditorLayer FGLower => EditorSolution.CurrentScene?.LowDetails;
		internal EditorSolution.EditorLayer ScratchLayer => EditorSolution.CurrentScene?.Scratch;
        public EditorSolution.EditorLayer EditLayerA { get; set; }
        public EditorSolution.EditorLayer EditLayerB { get; set; }
        //Scene Width + Height (For Drawing)
        internal int SceneWidth => (EditorSolution.CurrentScene != null ? EditorSolution.CurrentScene.Layers.Max(sl => sl.Width) * 16 : 0);
		internal int SceneHeight => (EditorSolution.CurrentScene != null ? EditorSolution.CurrentScene.Layers.Max(sl => sl.Height) * 16 : 0);



		//Clipboards
		public Tuple<Dictionary<Point, ushort>, Dictionary<Point, ushort>> TilesClipboard;
		public Dictionary<Point, ushort> FindReplaceClipboard;
		public Dictionary<Point, ushort> TilesClipboardEditable;
		public List<EditorEntity> entitiesClipboard;

        //Collision Colours
        public Color CollisionAllSolid = Color.White;
        public Color CollisionTopOnlySolid = Color.Yellow;
        public Color CollisionLRDSolid = Color.Red;

        //Internal/Public/Vital Classes

        public EditorControl EditorControls;

		internal EditorBackground BackgroundDX;
		public TilesToolbar TilesToolbar = null;
		public EntitiesToolbar EntitiesToolbar = null;
		public EditorEntityDrawing EntityDrawing;
		public EditorUpdater Updater;
		public EditorInGame InGame;
		public StartScreen StartScreen;
		public EditorStateModel StateModel;
		public EditorChunk Chunks;
		public EditorFormsModel FormsModel;
		public EditorUIEvents UIEvents;
		public EditorPath Paths;
		public EditorFileHandler FileHandler;
		public EditorDataPacks DataPacks;
		public EditorFindReplace FindAndReplace;
        public EditorZoomModel ZoomModel;
        public EditorTheming Theming;
        public EditorSettings Settings;
        public EditorManiacINI ManiacINI;
        public EditorUI UI;
        public UserStateModel Options;
        public EditorRecentSceneSourcesList RecentsList;
        public EditorRecentDataSourcesList RecentDataSourcesList;
        public ProcessMemory GameMemory = new ProcessMemory(); //Allows us to write hex codes like cheats, etc.
        public System.Windows.Forms.Integration.WindowsFormsHost FormsHost;
        public MainWindow TileManiacInstance = new MainWindow();
        public EditorDefaults Defaulter;

		// Stuff Used for Command Line Tool to Fix Duplicate Object ID's
		#region DLL Import Stuff
		[System.Runtime.InteropServices.DllImport("user32.dll")]
		[return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
		private static extern bool ShowWindow(IntPtr hWnd, ShowWindowEnum flags);

		[DllImport("USER32.DLL")]
		public static extern bool SetForegroundWindow(IntPtr hWnd);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern IntPtr CloseClipboard();

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
            Theming = new EditorTheming(this);
            Settings = new EditorSettings(this);
            Options = new UserStateModel();

            Theming.UseDarkTheme_WPF(ManiacEditor.Settings.MySettings.NightMode);
            Instance = this;
            InitializeComponent();

            Timer.Interval = 1;
            Timer.Elapsed += Timer_Elapsed;
            Timer.Start();



            System.Windows.Application.Current.MainWindow = this;

            try
            {
                InitilizeEditor();
            }
            catch (Exception ex)
            {
                Debug.Print("Couldn't Initialize Editor!" + ex.ToString());
                throw ex;
            }

			if (ManiacEditor.Settings.MyDevSettings.DevAutoStart) OpenSceneForceFully();

			if (ShortcutLaunch)
			{
				try
				{
					if (dataDir != "" && scenePath != "") OpenSceneForceFully(dataDir, scenePath, modPath, levelID, isEncoreMode, X, Y);

					else if (dataDir != "") OpenSceneForceFully(dataDir);
				}
				catch
				{
					Debug.Print("Couldn't Force Open Maniac Editor with the Specified Arguments!");
				}
			}
		}

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (EditorSolution.CurrentScene != null)
            {
                foreach (var layer in EditorSolution.CurrentScene.AllLayers)
                {
                    layer.UpdateLayerScrollIndex();
                }
            }
        }

        public void InitilizeEditor()
		{
			FormsModel = new EditorFormsModel(this);

			this.FormsModel.vScrollBar1.Scroll += new System.Windows.Controls.Primitives.ScrollEventHandler(this.VScrollBar1_Scroll);
			this.FormsModel.vScrollBar1.ValueChanged += new RoutedPropertyChangedEventHandler<double>(this.VScrollBar1_ValueChanged);
			this.FormsModel.vScrollBar1.MouseEnter += new System.Windows.Input.MouseEventHandler(this.VScrollBar1_Entered);
			this.FormsModel.hScrollBar1.Scroll += new System.Windows.Controls.Primitives.ScrollEventHandler(this.HScrollBar1_Scroll);
			this.FormsModel.hScrollBar1.ValueChanged += new RoutedPropertyChangedEventHandler<double>(this.HScrollBar1_ValueChanged);
			this.FormsModel.hScrollBar1.MouseEnter += new System.Windows.Input.MouseEventHandler(this.HScrollBar1_Entered);
			this.Activated += new System.EventHandler(this.Editor_Activated);
			this.FormsModel.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Editor_KeyDown);
			this.FormsModel.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Editor_KeyUp);

			this.FormsModel.GraphicPanel.OnRender += new ManiacEditor.RenderEventHandler(this.GraphicPanel_OnRender);
			this.FormsModel.GraphicPanel.OnCreateDevice += new ManiacEditor.CreateDeviceEventHandler(this.OnResetDevice);
			this.FormsModel.GraphicPanel.DragDrop += new System.Windows.Forms.DragEventHandler(this.GraphicPanel_DragDrop);
			this.FormsModel.GraphicPanel.DragEnter += new System.Windows.Forms.DragEventHandler(this.GraphicPanel_DragEnter);
			this.FormsModel.GraphicPanel.DragOver += new System.Windows.Forms.DragEventHandler(this.GraphicPanel_DragOver);
			this.FormsModel.GraphicPanel.DragLeave += new System.EventHandler(this.GraphicPanel_DragLeave);
			this.FormsModel.GraphicPanel.KeyDown += new System.Windows.Forms.KeyEventHandler(this.GraphicPanel_OnKeyDown);
			this.FormsModel.GraphicPanel.KeyUp += new System.Windows.Forms.KeyEventHandler(this.GraphicPanel_OnKeyUp);
			this.FormsModel.GraphicPanel.MouseClick += new System.Windows.Forms.MouseEventHandler(this.GraphicPanel_MouseClick);
			this.FormsModel.GraphicPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.GraphicPanel_OnMouseDown);
			this.FormsModel.GraphicPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.GraphicPanel_OnMouseMove);
			this.FormsModel.GraphicPanel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.GraphicPanel_OnMouseUp);
			this.FormsModel.GraphicPanel.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.GraphicPanel_MouseWheel);

			ExtraLayerEditViewButtons = new Dictionary<EditLayerToggleButton, EditLayerToggleButton>();
			ExtraLayerSeperators = new List<Separator>();

            RecentSceneItems = new List<Tuple<MenuItem, MenuItem>>();
            RecentDataSourceItems = new List<Tuple<MenuItem, MenuItem>>();
            Options.MenuChar = Options.MenuCharS.ToCharArray();
			Options.MenuChar_Small = Options.MenuCharS_Small.ToCharArray();
			Options.LevelSelectChar = Options.LevelSelectCharS.ToCharArray();
			InGame = new EditorInGame(this);
			EntityDrawing = new EditorEntityDrawing(this);
            StateModel = new EditorStateModel(this);
            EditorControls = new EditorControl();
			StartScreen = new StartScreen(this);
			Updater = new EditorUpdater();
			UIEvents = new EditorUIEvents(this);
			Paths = new EditorPath(this);
			FileHandler = new EditorFileHandler(this);
			DataPacks = new EditorDataPacks(this);
			FindAndReplace = new EditorFindReplace(this);
            ZoomModel = new EditorZoomModel(this);
            ManiacINI = new EditorManiacINI(this);
            EditorLaunch.UpdateInstance(this);
            UI = new EditorUI();
            RecentsList = new EditorRecentSceneSourcesList(this);
            RecentDataSourcesList = new EditorRecentDataSourcesList(this);
            Defaulter = new EditorDefaults();

            EditorStatusBar.UpdateFilterButtonApperance(true);



            this.Title = String.Format("Maniac Editor - Generations Edition {0}", Updater.GetVersion());
			FormsModel.GraphicPanel.Width = SystemInformation.PrimaryMonitorSize.Width;
			FormsModel.GraphicPanel.Height = SystemInformation.PrimaryMonitorSize.Height;

			ViewPanelContextMenu.Foreground = (SolidColorBrush)FindResource("NormalText");
			ViewPanelContextMenu.Background = (SolidColorBrush)FindResource("NormalBackground");




			AllocConsole();
			HideConsoleWindow();
			RefreshCollisionColours();
            ZoomModel.SetViewSize();
            UI.UpdateControls();
			Settings.TryLoadSettings();

            
			UpdateStartScreen(true, true);
		}

        public void UpdateScrollBars()
        {
            this.FormsModel.vScrollBar1.Scroll += new System.Windows.Controls.Primitives.ScrollEventHandler(this.VScrollBar1_Scroll);
            this.FormsModel.vScrollBar1.ValueChanged += new RoutedPropertyChangedEventHandler<double>(this.VScrollBar1_ValueChanged);
            this.FormsModel.vScrollBar1.MouseEnter += new System.Windows.Input.MouseEventHandler(this.VScrollBar1_Entered);
            this.FormsModel.hScrollBar1.Scroll += new System.Windows.Controls.Primitives.ScrollEventHandler(this.HScrollBar1_Scroll);
            this.FormsModel.hScrollBar1.ValueChanged += new RoutedPropertyChangedEventHandler<double>(this.HScrollBar1_ValueChanged);
            this.FormsModel.hScrollBar1.MouseEnter += new System.Windows.Input.MouseEventHandler(this.HScrollBar1_Entered);
        }

		#endregion
		#region Boolean States
		public bool IsEditing()
		{
			return IsTilesEdit() || IsEntitiesEdit() || IsChunksEdit();
		}
		public bool IsSceneLoaded()
		{
			if (EditorSolution.CurrentScene != null)
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
			return EditorToolbar.ChunksToolButton.IsChecked.Value && EditLayerA != null;
		}
        public bool IsEntitiesEdit()
        {
            return EditorToolbar.EditEntities.IsCheckedN.Value || EditorToolbar.EditEntities.IsCheckedA.Value || EditorToolbar.EditEntities.IsCheckedB.Value;
		}
		public bool IsSelected(bool dualModeSelect = false)
		{
			if (IsTilesEdit())
			{

				bool SelectedA = EditLayerA?.SelectedTiles.Count > 0 || EditLayerA?.TempSelectionTiles.Count > 0;
				bool SelectedB = EditLayerB?.SelectedTiles.Count > 0 || EditLayerB?.TempSelectionTiles.Count > 0;
                if (dualModeSelect) return SelectedA && SelectedB;
                else return SelectedA || SelectedB;
			}
			else if (IsEntitiesEdit())
			{
				return EditorSolution.Entities.IsSelected();
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

			var result = System.Windows.MessageBox.Show($"The file '{fullFilePath}' already exists. Overwrite?", "Overwrite?",
										 MessageBoxButton.YesNo, MessageBoxImage.Warning);

			if (result == MessageBoxResult.Yes) return true;

			return false;
		}
        #endregion
		#region Common Editor Functions
		public void EditorPlaceTile(Point position, int tile, EditorSolution.EditorLayer layer, bool isDrawing = false)
		{
            if (isDrawing)
            {
                double offset = (Options.DrawBrushSize / 2) * EditorConstants.TILE_SIZE;
                Point finalPosition = new Point((int)(position.X - offset), (int)(position.Y - offset));
                Dictionary<Point, ushort> tiles = new Dictionary<Point, ushort>();
                for (int x = 0; x < Options.DrawBrushSize; x++)
                {
                    for (int y = 0; y < Options.DrawBrushSize; y++)
                    {
                        if (!tiles.ContainsKey(new Point(x, y))) tiles.Add(new Point(x, y), (ushort)tile);
                    }
                }
                layer.DrawAsBrush(finalPosition, tiles);
            }
            else
            {
                Dictionary<Point, ushort> tiles = new Dictionary<Point, ushort>
                {
                    [new Point(0, 0)] = (ushort)tile
                };
                layer.PasteFromClipboard(position, tiles);
            }
		}
		public void DeleteSelected()
		{
			EditLayerA?.DeleteSelected();
			EditLayerB?.DeleteSelected();
			UI.UpdateEditLayerActions();

			if (IsEntitiesEdit())
			{
				EditorSolution.Entities.DeleteSelected();
				UpdateLastEntityAction();
			}
		}
        public void UpdateLastEntityAction()
        {
            if (EditorSolution.Entities.LastAction != null || EditorSolution.Entities.LastActionInternal != null) RedoStack.Clear();
            if (EditorSolution.Entities.LastAction != null)
			{
				UndoStack.Push(EditorSolution.Entities.LastAction);
				EditorSolution.Entities.LastAction = null;
			}
            if (EditorSolution.Entities.LastActionInternal != null)
            {
                UndoStack.Push(EditorSolution.Entities.LastActionInternal);
                EditorSolution.Entities.LastActionInternal = null;
            }
            if (EditorSolution.Entities.LastAction != null || EditorSolution.Entities.LastActionInternal != null) UI.UpdateControls();

        }
		public void FlipEntities(FlipDirection direction)
		{
			Dictionary<EditorEntity, Point> initalPos = new Dictionary<EditorEntity, Point>();
			Dictionary<EditorEntity, Point> postPos = new Dictionary<EditorEntity, Point>();
			foreach (EditorEntity e in EditorSolution.Entities.SelectedEntities)
			{
				initalPos.Add(e, new Point(e.PositionX, e.PositionY));
			}
			EditorSolution.Entities.Flip(direction);
			EntitiesToolbar.UpdateCurrentEntityProperites();
			foreach (EditorEntity e in EditorSolution.Entities.SelectedEntities)
			{
				postPos.Add(e, new Point(e.PositionX, e.PositionY));
			}
			IAction action = new ActionMultipleMoveEntities(initalPos, postPos);
			UndoStack.Push(action);
			RedoStack.Clear();

		}
        /// <summary>
        /// Deselects all tiles and EditorSolution.Entities
        /// </summary>
        /// <param name="updateControls">Whether to update associated on-screen controls</param>
        public void Deselect(bool updateControls = true)
        {
            if (IsEditing())
            {
                EditLayerA?.Deselect();
                EditLayerB?.Deselect();

                if (IsEntitiesEdit()) EditorSolution.Entities.Deselect();
                UI.SetSelectOnlyButtonsState(false);
                if (updateControls)
                    UI.UpdateEditLayerActions();
            }
        }
        public void EditorUndo()
        {
            if (UndoStack.Count > 0)
            {
                if (IsTilesEdit())
                {
                    // Deselect to apply the changes
                    Deselect();
                }
                else if (IsEntitiesEdit())
                {
                    if (UndoStack.Peek() is ActionAddDeleteEntities)
                    {
                        // deselect only if delete/create
                        Deselect();
                    }
                }
                IAction act = UndoStack.Pop();
                act.Undo();
                RedoStack.Push(act.Redo());
                if (IsEntitiesEdit() && IsSelected())
                {
                    // We need to update the properties of the selected entity
                    EntitiesToolbar.UpdateCurrentEntityProperites();
                }
            }
            FormsModel.GraphicPanel.Render();
            UI.UpdateControls();
        }
        public void EditorRedo()
        {
            if (RedoStack.Count > 0)
            {
                IAction act = RedoStack.Pop();
                act.Undo();
                UndoStack.Push(act.Redo());
                if (IsEntitiesEdit() && IsSelected())
                {
                    // We need to update the properties of the selected entity
                    EntitiesToolbar.UpdateCurrentEntityProperites();
                }
            }
            FormsModel.GraphicPanel.Render();
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
                if (!doNotUseWindowsClipboard)
                    Clipboard.SetDataObject(new DataObject("ManiacTiles", copyData), true);

                // Also copy to Maniac's clipboard in case it gets overwritten elsewhere
                TilesClipboard = copyData;
            }
            else if (hasMultipleValidLayers && Options.MultiLayerEditMode)
            {
                Tuple<Dictionary<Point, ushort>, Dictionary<Point, ushort>> copyData = EditorSolution.EditorLayer.CopyMultiSelectionToClipboard(EditLayerA, EditLayerB);

                // Make a DataObject for the copied data and send it to the Windows clipboard for cross-instance copying
                if (!doNotUseWindowsClipboard)
                    Clipboard.SetDataObject(new DataObject("ManiacTiles", copyData), true);

                // Also copy to Maniac's clipboard in case it gets overwritten elsewhere
                TilesClipboard = copyData;
            }


        }
        public void CopyEntitiesToClipboard()
        {
            if (EntitiesToolbar.IsFocused == false)
            {
                List<EditorEntity> copyData = EditorSolution.Entities.CopyToClipboard();

                /*
                // Prepare each Entity for the copy to release unnecessary data
                foreach (EditorEntity entity in copyData)
                   entity.PrepareForExternalCopy();

                CloseClipboard();

                // Make a DataObject for the data and send it to the Windows clipboard for cross-instance copying
                Clipboard.SetDataObject(new DataObject("ManiacEntities", copyData));*/

                // Send to Maniac's clipboard
                entitiesClipboard = copyData;
            }
        }

        public void PasteEntitiesToClipboard()
        {
            if (EntitiesToolbar.IsFocused.Equals(false))
            {
                try
                {

                    // check if there are EditorSolution.Entities on the Windows clipboard; if so, use those
                    if (System.Windows.Clipboard.ContainsData("ManiacEntities"))
                    {
                        EditorSolution.Entities.PasteFromClipboard(new Point((int)(EditorStateModel.LastX / EditorStateModel.Zoom), (int)(EditorStateModel.LastY / EditorStateModel.Zoom)), (List<EditorEntity>)System.Windows.Clipboard.GetDataObject().GetData("ManiacEntities"));
                        UpdateLastEntityAction();
                    }

                    // if there's none, use the internal clipboard
                    else if (entitiesClipboard != null)
                    {
                        EditorSolution.Entities.PasteFromClipboard(new Point((int)(EditorStateModel.LastX / EditorStateModel.Zoom), (int)(EditorStateModel.LastY / EditorStateModel.Zoom)), entitiesClipboard);
                        UpdateLastEntityAction();
                    }
                }
                catch (EditorEntities.TooManyEntitiesException)
                {
                    System.Windows.MessageBox.Show("Too many EditorSolution.Entities! (limit: 2048)");
                    return;
                }
                UI.UpdateEntitiesToolbarList();
                UI.SetSelectOnlyButtonsState();
            }
        }

        public void MoveEntityOrTiles(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            int x = 0, y = 0;
            int modifier = (IsChunksEdit() ? 8 : 1);
            if (Options.UseMagnetMode)
            {
                switch (e.KeyData)
                {
                    case Keys.Up: y = (Options.UseMagnetYAxis ? -Options.MagnetSize : -1); break;
                    case Keys.Down: y = (Options.UseMagnetYAxis ? Options.MagnetSize : 1); break;
                    case Keys.Left: x = (Options.UseMagnetXAxis ? -Options.MagnetSize : -1); break;
                    case Keys.Right: x = (Options.UseMagnetXAxis ? Options.MagnetSize : 1); break;
                }
            }
            if (Options.EnableFasterNudge)
            {
                if (Options.UseMagnetMode)
                {
                    switch (e.KeyData)
                    {
                        case Keys.Up: y = (Options.UseMagnetYAxis ? -Options.MagnetSize * Options.FasterNudgeAmount : -1 - Options.FasterNudgeAmount); break;
                        case Keys.Down: y = (Options.UseMagnetYAxis ? Options.MagnetSize * Options.FasterNudgeAmount : 1 + Options.FasterNudgeAmount); break;
                        case Keys.Left: x = (Options.UseMagnetXAxis ? -Options.MagnetSize * Options.FasterNudgeAmount : -1 - Options.FasterNudgeAmount); break;
                        case Keys.Right: x = (Options.UseMagnetXAxis ? Options.MagnetSize * Options.FasterNudgeAmount : 1 + Options.FasterNudgeAmount); break;
                    }
                }
                else
                {
                    if (IsChunksEdit())
                    {
                        switch (e.KeyData)
                        {
                            case Keys.Up: y = -1 * modifier; break;
                            case Keys.Down: y = 1 * modifier; break;
                            case Keys.Left: x = -1 * modifier; break;
                            case Keys.Right: x = 1 * modifier; break;
                        }
                    }
                    else
                    {
                        switch (e.KeyData)
                        {
                            case Keys.Up: y = (-1 - Options.FasterNudgeAmount) * modifier; break;
                            case Keys.Down: y = (1 + Options.FasterNudgeAmount) * modifier; break;
                            case Keys.Left: x = (-1 - Options.FasterNudgeAmount) * modifier; break;
                            case Keys.Right: x = (1 + Options.FasterNudgeAmount) * modifier; break;
                        }
                    }

                }

            }
            if (Options.UseMagnetMode == false && Options.EnableFasterNudge == false)
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

            UI.UpdateEditLayerActions();

            if (IsEntitiesEdit())
            {
                if (Options.UseMagnetMode)
                {
                    int xE = EditorSolution.Entities.SelectedEntities[0].Entity.Position.X.High;
                    int yE = EditorSolution.Entities.SelectedEntities[0].Entity.Position.Y.High;

                    if (xE % Options.MagnetSize != 0 && Options.UseMagnetXAxis)
                    {
                        int offsetX = x % Options.MagnetSize;
                        x -= offsetX;
                    }
                    if (yE % Options.MagnetSize != 0 && Options.UseMagnetYAxis)
                    {
                        int offsetY = y % Options.MagnetSize;
                        y -= offsetY;
                    }
                }


                EditorSolution.Entities.MoveSelected(new Point(0, 0), new Point(x, y), false);
                EntitiesToolbar.UpdateCurrentEntityProperites();

                // Try to merge with last move
                List<EditorEntity> SelectedList = EditorSolution.Entities.SelectedEntities.ToList();
                List<EditorEntity> SelectedInternalList = EditorSolution.Entities.SelectedInternalEntities.ToList();
                bool selectedActionsState = UndoStack.Count > 0 && UndoStack.Peek() is ActionMoveEntities && (UndoStack.Peek() as ActionMoveEntities).UpdateFromKey(SelectedList, new Point(x, y));
                bool selectedInternalActionsState = UndoStack.Count > 0 && UndoStack.Peek() is ActionMoveEntities && (UndoStack.Peek() as ActionMoveEntities).UpdateFromKey(SelectedInternalList, new Point(x, y));

                if (selectedActionsState || selectedInternalActionsState) { }
                else
                {
                    if (SelectedList.Count != 0) UndoStack.Push(new ActionMoveEntities(SelectedList, new Point(x, y), true));
                    if (SelectedInternalList.Count != 0) UndoStack.Push(new ActionMoveEntities(SelectedInternalList, new Point(x, y), true));

                    RedoStack.Clear();
                    UI.UpdateControls();
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
            System.Windows.MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
        }
        public void GoToPosition(int x, int y, bool CenterCoords = true, bool ShortcutClear = false)
        {
            if (CenterCoords)
            {
                Rectangle r = FormsModel.GraphicPanel.GetScreen();
                int x2 = (int)(r.Width * EditorStateModel.Zoom);
                int y2 = (int)(r.Height * EditorStateModel.Zoom);

                int ResultX = (int)(x * EditorStateModel.Zoom) - x2 / 2;
                int ResultY = (int)(y * EditorStateModel.Zoom) - y2 / 2;

                if ((ResultX <= 0)) ResultX = 0;
                if ((ResultY <= 0)) ResultY = 0;


                EditorStateModel.ViewPositionX = ResultX;
                EditorStateModel.ViewPositionY = ResultY;
            }
            else
            {
                int ResultX = (int)(x * EditorStateModel.Zoom);
                int ResultY = (int)(y * EditorStateModel.Zoom);

                if ((ResultX <= 0)) ResultX = 0;
                if ((ResultY <= 0)) ResultY = 0;

                EditorStateModel.ViewPositionX = ResultX;
                EditorStateModel.ViewPositionY = ResultY;
            }
        }

        #endregion
        #region GameConfig/Data Folders
        public string GetDataDirectory()
		{
			using (var folderBrowserDialog = new GenerationsLib.Core.FolderSelectDialog())
			{
				folderBrowserDialog.Title = "Select Data Folder";

				if (!folderBrowserDialog.ShowDialog())
					return null;

				return folderBrowserDialog.FileName;
			}
		}
		public bool SetGameConfig() { return Paths.SetGameConfig(); }
		public bool IsDataDirectoryValid(string directoryToCheck) { return Paths.IsDataDirectoryValid(directoryToCheck); }
        #endregion
        #region Open Scene Methods
        public void OpenSceneForceFully()
		{
			string dataDirectory = ManiacEditor.Settings.MyDevSettings.DevForceRestartData;
			if (dataDirectory != null) DataDirectory = dataDirectory;
			string Result = ManiacEditor.Settings.MyDevSettings.DevForceRestartScene;
			int LevelID = ManiacEditor.Settings.MyDevSettings.DevForceRestartID;
			bool isEncore = ManiacEditor.Settings.MyDevSettings.DevForceRestartIsEncore;
			string CurrentZone = ManiacEditor.Settings.MyDevSettings.DevForceRestartCurrentZone;
			string CurrentName = ManiacEditor.Settings.MyDevSettings.DevForceRestartCurrentName;
			string CurrentSceneID = ManiacEditor.Settings.MyDevSettings.DevForceRestartSceneID;
			bool Browsed = ManiacEditor.Settings.MyDevSettings.DevForceRestartIsBrowsed;
            IList<string> DevResourcePacks = new List<string>();
            if (ManiacEditor.Settings.MyDevSettings.DevForceRestartResourcePacks != null) DevResourcePacks = ManiacEditor.Settings.MyDevSettings.DevForceRestartResourcePacks.Cast<string>().ToList();
            int x = ManiacEditor.Settings.MyDevSettings.DevForceRestartX;
			int y = ManiacEditor.Settings.MyDevSettings.DevForceRestartY;

			FileHandler.OpenSceneFromSaveState(dataDirectory, Result, LevelID, isEncore, CurrentZone, CurrentZone, CurrentSceneID, Browsed, DevResourcePacks);

            GoToPosition(x, y, true);

        }
		private void OpenSceneForceFully(string dataDir, string scenePath, string modPath, int levelID, bool isEncoreMode, int X, int Y, double _ZoomScale = 0.0, string SceneID = "", string Zone = "", string Name = "")
		{
            System.Windows.MessageBox.Show("These Kind of Shortcuts are Broken for now! SORRY!");

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
			FileHandler.OpenSceneSelectWithPrefrences(DataDirectory);
		}
		public void OpenScene(bool manual = false, string Result = null, int LevelID = -1, bool isEncore = false, bool modLoaded = false, string modDir = "")
		{
			FileHandler.OpenSceneUsingSceneSelect();
		}
        #endregion
        #region Main Events
        #region Editor Events
        private void Editor_Activated(object sender, EventArgs e)
        {
            FormsModel.GraphicPanel.Focus();
            if (TileManiacInstance.hasModified)
            {
                ReloadToolStripButton_Click(sender, null);
            }

        }
        private void Editor_FormClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (EditorLaunch.ManiaPalConnector != null) EditorLaunch.ManiaPalConnector.Kill();

            try
            {
                InGame.GameRunning = false;
                var mySettings = Properties.Settings.Default;
                ManiacEditor.Settings.MySettings.IsMaximized = WindowState == System.Windows.WindowState.Maximized;
                EditorConstants.SaveAllSettings();
            }
            catch (Exception ex)
            {
                Debug.Write("Failed to write settings: " + ex);
            }

            if (ManiaHost._process != null)
            {
                ManiaHost.ForceKillSonicMania();
            }

            FormsModel.Dispose();
            //editorView = null;
            FormsHost.Child.Dispose();
            //host = null;



        }
        private void Editor_KeyDown(object sender, KeyEventArgs e)
        {
            var e2 = KeyEventExts.ToWinforms(e);
            if (e2 != null)
            {
                EditorControls.GraphicPanel_OnKeyDown(sender, e2);
            }

        }
        private void Editor_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (!FormsModel.GraphicPanel.Focused)
            {
                EditorControls.GraphicPanel_OnKeyDown(sender, e);
            }
        }
        private void Editor_KeyUp(object sender, KeyEventArgs e)
        {
            var e2 = KeyEventExts.ToWinforms(e);
            if (e2 != null)
            {
                EditorControls.GraphicPanel_OnKeyUp(sender, e2);
            }

        }
        private void Editor_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (!FormsModel.GraphicPanel.Focused)
            {
                EditorControls.GraphicPanel_OnKeyUp(sender, e);
            }
        }
        public void Editor_Resize(object sender, RoutedEventArgs e) { ZoomModel.Resize(sender, e); }
        private void Editor_Loaded(object sender, RoutedEventArgs e)
        {
            // Create the interop host control.
            FormsHost = new System.Windows.Forms.Integration.WindowsFormsHost();

            // Create the MaskedTextBox control.

            // Assign the MaskedTextBox control as the host control's child.
            FormsHost.Child = FormsModel;

            FormsHost.Foreground = (SolidColorBrush)FindResource("NormalText");

            // Add the interop host control to the Grid
            // control's collection of child controls.
            this.ViewPanelForm.Children.Add(FormsHost);

            FormsModel.GraphicPanel.Init(FormsModel);
        }
        #endregion
        #region Graphics Panel Events
        private void GraphicPanel_DragEnter(object sender, System.Windows.Forms.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(Int32)) && IsTilesEdit())
            {
                Point rel = FormsModel.GraphicPanel.PointToScreen(Point.Empty);
                e.Effect = System.Windows.Forms.DragDropEffects.Move;
                EditLayerA?.StartDragOver(new Point((int)(((e.X - rel.X) + EditorStateModel.ViewPositionX) / EditorStateModel.Zoom), (int)(((e.Y - rel.Y) + EditorStateModel.ViewPositionY) / EditorStateModel.Zoom)), (ushort)TilesToolbar.SelectedTile);
                UI.UpdateEditLayerActions();
            }
            else
            {
                e.Effect = System.Windows.Forms.DragDropEffects.None;
            }
        }
        private void GraphicPanel_DragOver(object sender, System.Windows.Forms.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(Int32)) && IsTilesEdit())
            {
                Point rel = FormsModel.GraphicPanel.PointToScreen(Point.Empty);
                EditLayerA?.DragOver(new Point((int)(((e.X - rel.X) + EditorStateModel.ViewPositionX) / EditorStateModel.Zoom), (int)(((e.Y - rel.Y) + EditorStateModel.ViewPositionY) / EditorStateModel.Zoom)), (ushort)TilesToolbar.SelectedTile);
                FormsModel.GraphicPanel.Render();

            }
        }
        private void GraphicPanel_DragLeave(object sender, EventArgs e)
        {
            EditLayerA?.EndDragOver(true);
            FormsModel.GraphicPanel.Render();
        }
        private void GraphicPanel_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
        {
            EditLayerA?.EndDragOver(false);
        }
        public void GraphicPanel_OnKeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            EditorControls.GraphicPanel_OnKeyDown(sender, e);
        }
        public void GraphicPanel_OnKeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            EditorControls.GraphicPanel_OnKeyUp(sender, e);
        }
        #endregion
        #region Mouse Actions Event Handlers
        private void GraphicPanel_OnMouseMove(object sender, System.Windows.Forms.MouseEventArgs e) { EditorControls.MouseMove(sender, e); }
        private void GraphicPanel_OnMouseDown(object sender, System.Windows.Forms.MouseEventArgs e) { EditorControls.MouseDown(sender, e); }
        private void GraphicPanel_OnMouseUp(object sender, System.Windows.Forms.MouseEventArgs e) { EditorControls.MouseUp(sender, e); }
        private void GraphicPanel_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e) { EditorControls.MouseWheel(sender, e); }
        private void GraphicPanel_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e) { EditorControls.MouseClick(sender, e); }
        #endregion
        #region Splitter Events
        private void Spliter_DragDelta(object sender, DragDeltaEventArgs e) { ZoomModel.Resize(sender, e); }
        private void Spliter_SizeChanged(object sender, SizeChangedEventArgs e) { ZoomModel.SetZoomLevel(EditorStateModel.ZoomLevel, new System.Drawing.Point(EditorStateModel.ViewPositionX, EditorStateModel.ViewPositionY), 0.0, false); }
        #endregion
        #region Scrollbar Events
        private void VScrollBar1_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e) { ZoomModel.VScrollBar1_Scroll(sender, e); }
        private void HScrollBar1_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e) { ZoomModel.HScrollBar1_Scroll(sender, e); }
        private void VScrollBar1_ValueChanged(object sender, RoutedEventArgs e) { ZoomModel.VScrollBar1_ValueChanged(sender, e); }
        private void HScrollBar1_ValueChanged(object sender, RoutedEventArgs e) { ZoomModel.HScrollBar1_ValueChanged(sender, e); }
        private void VScrollBar1_Entered(object sender, EventArgs e) { ZoomModel.VScrollBar1_Entered(sender, e); }
        private void HScrollBar1_Entered(object sender, EventArgs e) { ZoomModel.HScrollBar1_Entered(sender, e); }
        #endregion

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

			bool showEntities = EditorToolbar.ShowEntities.IsChecked.Value && !EditorToolbar.EditEntities.IsCheckedAll;
			bool showEntitiesEditing = EditorToolbar.EditEntities.IsCheckedAll;

			bool PriorityMode = Options.PrioritizedEntityViewing;
            bool AboveAllMode = Options.EntitiesVisibileAboveAllLayers;


			if (EntitiesToolbar?.NeedRefresh ?? false) EntitiesToolbar.PropertiesRefresh();
            if (EditorSolution.CurrentScene != null)
            {
                DrawBackground();

                //if (UIModes.DebugStatsVisibleOnPanel && EditorSolution.CurrentScene != null) DrawDebugHUD();

                if (EditorSolution.CurrentScene.OtherLayers.Contains(EditLayerA)) EditLayerA.Draw(FormsModel.GraphicPanel);

                if (!Options.ExtraLayersMoveToFront) DrawExtraLayers();

                DrawLayer(EditorToolbar.ShowFGLower.IsChecked.Value, EditorToolbar.EditFGLower.IsCheckedAll, FGLower);

                DrawLayer(EditorToolbar.ShowFGLow.IsChecked.Value, EditorToolbar.EditFGLow.IsCheckedAll, FGLow);


                if (showEntities && !AboveAllMode)
                    if (PriorityMode) EntitiesDraw(2);
                    else EntitiesDraw(0);

                DrawLayer(EditorToolbar.ShowFGHigh.IsChecked.Value, EditorToolbar.EditFGHigh.IsCheckedAll, FGHigh);

                if (showEntities && PriorityMode && !AboveAllMode) EntitiesDraw(3);

                DrawLayer(EditorToolbar.ShowFGHigher.IsChecked.Value, EditorToolbar.EditFGHigher.IsCheckedAll, FGHigher);

                if (Options.ExtraLayersMoveToFront) DrawExtraLayers();

                if (showEntitiesEditing || AboveAllMode)
                    if (PriorityMode) EntitiesDraw(1);
                    else EntitiesDraw(0);

                if (EditorSolution.CurrentScene != null) EditorSolution.Entities.DrawInternalObjects(FormsModel.GraphicPanel);

                if (Options.EntitySelectionBoxesAlwaysPrioritized && (showEntities || showEntitiesEditing)) EditorSolution.Entities.DrawSelectionBoxes(FormsModel.GraphicPanel);

            }

            if (EditorStateModel.DraggingSelection) DrawSelectionBox();
            else DrawSelectionBox(true);

            if (EditorStateModel.isTileDrawing && Options.DrawBrushSize != 1) DrawBrushBox();

            if (Options.ShowGrid && EditorSolution.CurrentScene != null) BackgroundDX.DrawGrid(FormsModel.GraphicPanel);


            if (InGame.GameRunning) DrawGameElements();

            if (EditorStateModel.Scrolling) DrawScroller();

            void DrawBackground()
            {
                if (!IsTilesEdit()) BackgroundDX.Draw(FormsModel.GraphicPanel);
                if (IsTilesEdit()) if (ManiacEditor.Settings.MyPerformance.ShowEditLayerBackground == true) BackgroundDX.DrawEdit(FormsModel.GraphicPanel);
            }

            void DrawScroller()
            {
                if (FormsModel.vScrollBar1.IsVisible && FormsModel.hScrollBar1.IsVisible) FormsModel.GraphicPanel.Draw2DCursor(EditorStateModel.ScrollPosition.X, EditorStateModel.ScrollPosition.Y);
                else if (FormsModel.vScrollBar1.IsVisible) FormsModel.GraphicPanel.DrawVertCursor(EditorStateModel.ScrollPosition.X, EditorStateModel.ScrollPosition.Y);
                else if (FormsModel.hScrollBar1.IsVisible) FormsModel.GraphicPanel.DrawHorizCursor(EditorStateModel.ScrollPosition.X, EditorStateModel.ScrollPosition.Y);
            }

            void DrawExtraLayers()
            {
                foreach (var elb in ExtraLayerEditViewButtons)
                {
                    if (elb.Value.IsCheckedAll || elb.Key.IsCheckedAll)
                    {
                        var _extraViewLayer = EditorSolution.CurrentScene.OtherLayers.Single(el => el.Name.Equals(elb.Key.Text));
                        _extraViewLayer.Draw(FormsModel.GraphicPanel);
                    }
                }
            }

            void EntitiesDraw(int mode)
            {
                switch (mode)
                {
                    case 0:
                        EditorSolution.Entities.Draw(FormsModel.GraphicPanel);
                        break;
                    case 1:
                        EditorSolution.Entities.DrawPriority(FormsModel.GraphicPanel, -1);
                        EditorSolution.Entities.DrawPriority(FormsModel.GraphicPanel, 0);
                        EditorSolution.Entities.DrawPriority(FormsModel.GraphicPanel, 1);
                        EditorSolution.Entities.DrawPriority(FormsModel.GraphicPanel, 2);
                        EditorSolution.Entities.DrawPriority(FormsModel.GraphicPanel, 3);
                        break;
                    case 2:
                        EditorSolution.Entities.DrawPriority(FormsModel.GraphicPanel, -1);
                        EditorSolution.Entities.DrawPriority(FormsModel.GraphicPanel, 0);
                        EditorSolution.Entities.DrawPriority(FormsModel.GraphicPanel, 1);
                        break;
                    case 3:
                        EditorSolution.Entities.DrawPriority(FormsModel.GraphicPanel, 2);
                        EditorSolution.Entities.DrawPriority(FormsModel.GraphicPanel, 3);
                        break;
                }
            }

            /*
            void DrawDebugHUD()
            {
                Point point = new Point((short)(15), (short)(15));

                DebugTextHUD.DrawEditorHUDText(this, FormsModel.GraphicPanel, point.X, point.Y, StateModel.GetDataFolder(), true, 255, 15);
                DebugTextHUD.DrawEditorHUDText(this, FormsModel.GraphicPanel, point.X, point.Y + 12 * 1, StateModel.GetMasterDataFolder(), true, 255, 22);
                DebugTextHUD.DrawEditorHUDText(this, FormsModel.GraphicPanel, point.X, point.Y + 12 * 2, StateModel.GetScenePath(), true, 255, 11);
                DebugTextHUD.DrawEditorHUDText(this, FormsModel.GraphicPanel, point.X, point.Y + 12 * 3, StateModel.GetSceneFilePath(), true, 255, 12);
                DebugTextHUD.DrawEditorHUDText(this, FormsModel.GraphicPanel, point.X, point.Y + 12 * 4, StateModel.GetZoom(), true, 255, 11);
                DebugTextHUD.DrawEditorHUDText(this, FormsModel.GraphicPanel, point.X, point.Y + 12 * 5, StateModel.GetSetupObject(), true, 255, 13);
                DebugTextHUD.DrawEditorHUDText(this, FormsModel.GraphicPanel, point.X, point.Y + 12 * 6, StateModel.GetSelectedZone(), true, 255, 14);

                DebugTextHUD.DrawEditorHUDText(this, FormsModel.GraphicPanel, point.X, point.Y + 12 * 8, "Use " + EditorControls.KeyBindPraser("StatusBoxToggle") + " to Toggle this Information", true, 255, EditorControls.KeyBindPraser("StatusBoxToggle").Length, 4);
            }*/

            void DrawSelectionBox(bool resetSelection = false)
            {
                if (!resetSelection)
                {
                    int bound_x1 = (int)(EditorStateModel.RegionX2 / EditorStateModel.Zoom); int bound_x2 = (int)(EditorStateModel.LastX / EditorStateModel.Zoom);
                    int bound_y1 = (int)(EditorStateModel.RegionY2 / EditorStateModel.Zoom); int bound_y2 = (int)(EditorStateModel.LastY / EditorStateModel.Zoom);
                    if (bound_x1 != bound_x2 && bound_y1 != bound_y2)
                    {
                        if (bound_x1 > bound_x2)
                        {
                            bound_x1 = (int)(EditorStateModel.LastX / EditorStateModel.Zoom);
                            bound_x2 = (int)(EditorStateModel.RegionX2 / EditorStateModel.Zoom);
                        }
                        if (bound_y1 > bound_y2)
                        {
                            bound_y1 = (int)(EditorStateModel.LastY / EditorStateModel.Zoom);
                            bound_y2 = (int)(EditorStateModel.RegionY2 / EditorStateModel.Zoom);
                        }
                        if (IsChunksEdit())
                        {
                            bound_x1 = EditorSolution.EditorLayer.GetChunkCoordinatesTopEdge(bound_x1, bound_y1).X;
                            bound_y1 = EditorSolution.EditorLayer.GetChunkCoordinatesTopEdge(bound_x1, bound_y1).Y;
                            bound_x2 = EditorSolution.EditorLayer.GetChunkCoordinatesBottomEdge(bound_x2, bound_y2).X;
                            bound_y2 = EditorSolution.EditorLayer.GetChunkCoordinatesBottomEdge(bound_x2, bound_y2).Y;
                        }


                    }

                    FormsModel.GraphicPanel.DrawRectangle(bound_x1, bound_y1, bound_x2, bound_y2, Color.FromArgb(100, Color.Purple));
                    FormsModel.GraphicPanel.DrawLine(bound_x1, bound_y1, bound_x2, bound_y1, Color.Purple);
                    FormsModel.GraphicPanel.DrawLine(bound_x1, bound_y1, bound_x1, bound_y2, Color.Purple);
                    FormsModel.GraphicPanel.DrawLine(bound_x2, bound_y2, bound_x2, bound_y1, Color.Purple);
                    FormsModel.GraphicPanel.DrawLine(bound_x2, bound_y2, bound_x1, bound_y2, Color.Purple);
                }
                else
                {
                    EditorStateModel.select_x1 = 0; EditorStateModel.select_x2 = 0; EditorStateModel.select_y1 = 0; EditorStateModel.select_y2 = 0;
                }
            }

            void DrawBrushBox()
            {

                int offset = (Options.DrawBrushSize / 2) * EditorConstants.TILE_SIZE;
                int x1 = (int)(EditorStateModel.LastX / EditorStateModel.Zoom) - offset;
                int x2 = (int)(EditorStateModel.LastX / EditorStateModel.Zoom) + offset;
                int y1 = (int)(EditorStateModel.LastY / EditorStateModel.Zoom) - offset;
                int y2 = (int)(EditorStateModel.LastY / EditorStateModel.Zoom) + offset;


                int bound_x1 = (int)(x1); int bound_x2 = (int)(x2);
                int bound_y1 = (int)(y1); int bound_y2 = (int)(y2);
                if (bound_x1 != bound_x2 && bound_y1 != bound_y2)
                {
                    if (bound_x1 > bound_x2)
                    {
                        bound_x1 = (int)(x2);
                        bound_x2 = (int)(x1);
                    }
                    if (bound_y1 > bound_y2)
                    {
                        bound_y1 = (int)(y2);
                        bound_y2 = (int)(y1);
                    }
                }

                FormsModel.GraphicPanel.DrawRectangle(bound_x1, bound_y1, bound_x2, bound_y2, Color.FromArgb(100, Color.Purple));
                FormsModel.GraphicPanel.DrawLine(bound_x1, bound_y1, bound_x2, bound_y1, Color.Purple);
                FormsModel.GraphicPanel.DrawLine(bound_x1, bound_y1, bound_x1, bound_y2, Color.Purple);
                FormsModel.GraphicPanel.DrawLine(bound_x2, bound_y2, bound_x2, bound_y1, Color.Purple);
                FormsModel.GraphicPanel.DrawLine(bound_x2, bound_y2, bound_x1, bound_y2, Color.Purple);
            }

            void DrawLayer(bool ShowLayer, bool EditLayer, EditorSolution.EditorLayer layer)
            {
                if (ShowLayer || EditLayer) layer.Draw(FormsModel.GraphicPanel);
            }

            void DrawGameElements()
            {
                InGame.DrawGameElements(FormsModel.GraphicPanel);

                if (InGame.PlayerSelected) InGame.MovePlayer(new Point(EditorStateModel.LastX, EditorStateModel.LastY), EditorStateModel.Zoom, InGame.SelectedPlayer);
                if (InGame.CheckpointSelected)
                {
                    Point clicked_point = new Point((int)(EditorStateModel.LastX / EditorStateModel.Zoom), (int)(EditorStateModel.LastY / EditorStateModel.Zoom));
                    InGame.UpdateCheckpoint(clicked_point);
                }
            }
		}

        public void DrawLayers(int drawOrder = 0)
		{

            // Future Implementation

            /*
            List<int> layerDrawingOrder = new List<int> { };
            var allLayers = EditorSolution.Scene.AllLayers;
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

            var _extraViewLayer = EditorSolution.CurrentScene.LayerByDrawingOrder.FirstOrDefault(el => el.Layer.DrawingOrder.Equals(drawOrder));
			_extraViewLayer.Draw(FormsModel.GraphicPanel);
		}
		public void Run()
		{
			Show();
			Focus();
			FormsModel.Show();
			FormsModel.GraphicPanel.Run();

		}
        public void UpdateStartScreen(bool visible, bool firstLoad = false)
        {
            if (firstLoad)
            {
                Thread thread = new Thread(() => {
                    Updater.CheckforUpdates(true, true);
                });
                thread.Start();
                this.OverlayPanel.Children.Add(StartScreen);
                StartScreen.SelectScreen.ReloadRecentsTree();
                this.ViewPanelForm.Visibility = Visibility.Hidden;
                UI.UpdateToolbars(false, false, true);
                RefreshRecentScenes();
                RefreshDataSources();


            }
            if (visible)
            {
                StartScreen.Visibility = Visibility.Visible;
                StartScreen.SelectScreen.ReloadRecentsTree();
                this.ViewPanelForm.Visibility = Visibility.Hidden;
                UI.UpdateToolbars(false, false, true);
                RefreshRecentScenes();
                RefreshDataSources();
            }
            else
            {
                StartScreen.Visibility = Visibility.Hidden;
                StartScreen.SelectScreen.ReloadRecentsTree();
                this.ViewPanelForm.Visibility = Visibility.Visible;
                UI.UpdateToolbars(false, false, false);
            }

        }

        #endregion
        #region Asset Reloading
        public void ReloadSpecificTextures(object sender, RoutedEventArgs e)
		{
			try
			{
				// release all our resources, and force a reload of the tiles
				// EditorSolution.Entities should take care of themselves
				DisposeTextures();

				if (Options.UseEncoreColors)
				{
                    if (EditorSolution.CurrentTiles != null) EditorSolution.CurrentTiles.StageTiles?.Image.Reload(EncorePalette[0]);
				}
				else
				{
                    if (EditorSolution.CurrentTiles != null) EditorSolution.CurrentTiles.StageTiles?.Image.Reload();
				}

			}
			catch (Exception ex)
			{
				System.Windows.MessageBox.Show(ex.Message);
			}
		}
		public void DisposeTextures()
		{
            if (EditorSolution.CurrentScene != null)
            {
                // Make sure to dispose the textures of the extra layers too
                if (EditorSolution.CurrentTiles != null) EditorSolution.CurrentTiles?.DisposeTextures();
                if (FGHigh != null) FGHigh.DisposeTextures();
                if (FGLow != null) FGLow.DisposeTextures();
                if (FGHigher != null) FGHigher.DisposeTextures();
                if (FGLower != null) FGLower.DisposeTextures();

                foreach (var el in EditorSolution.CurrentScene.OtherLayers)
                {
                    el.DisposeTextures();
                }
            }
		}
		public void RefreshCollisionColours(bool RefreshMasks = false)
		{
			if (EditorSolution.CurrentScene != null && EditorSolution.CurrentTiles.StageTiles != null)
			{
                switch (Options.CollisionPreset)
                {
                    case 2:
                        CollisionAllSolid = Options.CollisionSAColour;
						CollisionTopOnlySolid = Options.CollisionTOColour;
						CollisionLRDSolid = Options.CollisionLRDColour;
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
                    //UI.ReloadSpritesAndTextures();
                }
			}

		}
		#endregion
		#region Get + Set Methods
		public Rectangle GetScreen()
		{
			if (ManiacEditor.Settings.MySettings.EntityFreeCam) return new Rectangle(EditorStateModel.CustomX, EditorStateModel.CustomY, FormsModel.mainPanel.Width, FormsModel.mainPanel.Height);
			else return new Rectangle(EditorStateModel.ViewPositionX, EditorStateModel.ViewPositionY, FormsModel.mainPanel.Width, FormsModel.mainPanel.Height);
		}
		public double GetZoom()
		{
			return EditorStateModel.Zoom;
		}
		private void SetDeviceSleepState(bool state)
		{
			FormsModel.GraphicPanel.bRender = state;
			if (state == true)
			{
				ReloadToolStripButton_Click(null, null);
			}
		}
		public Scene GetSceneSelection()
		{
			string selectedScene;

			ManiacEditor.Interfaces.SceneSelectWindow select = new ManiacEditor.Interfaces.SceneSelectWindow(EditorSolution.GameConfig, this);
			select.Owner = Window.GetWindow(this);
			select.ShowDialog();
			if (select.SceneSelect.SelectedSceneResult == null)
				return null;
			selectedScene = select.SceneSelect.SelectedSceneResult;

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
        #region Action Events (MenuItems, Clicks, etc.)
        #region File Events
        public void OpenSceneEvent(object sender, RoutedEventArgs e) { FileHandler.OpenScene(); }
        public void OpenDataDirectoryEvent(object sender, RoutedEventArgs e) { FileHandler.OpenDataDirectory(); }
        public void SaveSceneEvent(object sender, RoutedEventArgs e) { FileHandler.Save(); }
        public void SaveSceneAsEvent(object sender, RoutedEventArgs e) { FileHandler.SaveAs(); }
        #endregion
        #region Edit Events
        public void PasteToChunksEvent(object sender, RoutedEventArgs e) { UIEvents.PasteToChunks(); }
        public void SelectAllEvent(object sender, RoutedEventArgs e) { UIEvents.SelectAll(); }
        public void CutEvent(object sender, RoutedEventArgs e) { UIEvents.Cut(); }
        public void CopyEvent(object sender, RoutedEventArgs e) { UIEvents.Copy(); }
        public void PasteEvent(object sender, RoutedEventArgs e) { UIEvents.Paste(); }
        public void DuplicateEvent(object sender, RoutedEventArgs e) { UIEvents.Duplicate(); }
        private void DeleteEvent(object sender, RoutedEventArgs e) { UIEvents.Delete(); }
        public void FlipVerticalEvent(object sender, RoutedEventArgs e) { UIEvents.FlipVertical(); }
        public void FlipHorizontalEvent(object sender, RoutedEventArgs e) { UIEvents.FlipHorizontal(); }
        public void FlipVerticalIndividualEvent(object sender, RoutedEventArgs e) { UIEvents.FlipVerticalIndividual(); }
        public void FlipHorizontalIndividualEvent(object sender, RoutedEventArgs e) { UIEvents.FlipHorizontalIndividual(); }
        #endregion
		public void ReloadToolStripButton_Click(object sender, RoutedEventArgs e) { UI.ReloadSpritesAndTextures(); }
		public void ToggleSlotIDEvent(object sender, RoutedEventArgs e) { Options.ShowTileID ^= true; }
        public void ToggleFasterNudgeEvent(object sender, RoutedEventArgs e) { Options.EnableFasterNudge ^= true; }
        public void ShowCollisionAEvent(object sender, RoutedEventArgs e) { Options.ShowCollisionA ^= true; }
        public void ShowCollisionBEvent(object sender, RoutedEventArgs e) { Options.ShowCollisionB ^= true; }
        public void ToggleDebugHUDEvent(object sender, RoutedEventArgs e) { Options.DebugStatsVisibleOnPanel ^= true; }
        public void MenuButtonChangedEvent(string tag) { UIEvents.SetMenuButtonType(tag); }

        #region Grid Events
        public void ToggleGridEvent(object sender, RoutedEventArgs e) { Options.ShowGrid ^= true; }
        #endregion

        #region Apps
        private void TileManiacEditTileEvent(object sender, RoutedEventArgs e) { EditorLaunch.TileManiacIntergration(); }
        #endregion

        #region Settings and Other Menu Events
        public void AboutScreenEvent(object sender, RoutedEventArgs e) { EditorLaunch.AboutScreen(); }
        public void ImportObjectsToolStripMenuItem_Click(Window window = null) { EditorLaunch.ImportObjectsToolStripMenuItem_Click(window); }
        public void ImportObjectsWithMegaList(Window window = null) { EditorLaunch.ImportObjectsWithMegaList(window); }
        public void ImportSoundsEvent(Window window = null) { EditorLaunch.ImportSounds(window); }
        public void OptionsMenuEvent(object sender, RoutedEventArgs e) { EditorLaunch.OptionsMenu(); }
        #endregion

        #region Game Running Events
        private void MoveThePlayerToHere(object sender, RoutedEventArgs e) { InGame.MoveThePlayerToHere(); }
        private void SetPlayerRespawnToHere(object sender, RoutedEventArgs e) { InGame.SetPlayerRespawnToHere(); }
        private void MoveCheckpoint(object sender, RoutedEventArgs e) { InGame.CheckpointSelected = true; }
        private void RemoveCheckpoint(object sender, RoutedEventArgs e) { InGame.UpdateCheckpoint(new Point(0, 0), false); }
        private void AssetReset(object sender, RoutedEventArgs e) { InGame.AssetReset(); }
        private void RestartScene(object sender, RoutedEventArgs e) { InGame.RestartScene(); }
        #endregion

        #endregion
        #region Layer Toolbar Events
		public void SetupLayerButtons()
		{
			TearDownExtraLayerButtons();
			IList<EditLayerToggleButton> _extraLayerEditButtons = new List<EditLayerToggleButton>(); //Used for Extra Layer Edit Buttons
			IList<EditLayerToggleButton> _extraLayerViewButtons = new List<EditLayerToggleButton>(); //Used for Extra Layer View Buttons

			//EDIT BUTTONS
			foreach (EditorSolution.EditorLayer el in EditorSolution.CurrentScene.OtherLayers)
			{
				EditLayerToggleButton tsb = new EditLayerToggleButton()
				{
					Text = el.Name,
                    LayerName = "Edit" + el.Name
				};
                EditorToolbar.LayerToolbar.Items.Add(tsb);
                tsb.DualSelect = true;
                tsb.TextForeground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(Color.LawnGreen.A, Color.LawnGreen.R, Color.LawnGreen.G, Color.LawnGreen.B));
				tsb.RightClick += AdHocLayerEdit_RightClick;
                tsb.IsLayerOptionsEnabled = true;

                tsb.Click += AdHocLayerEdit_Click;

				_extraLayerEditButtons.Add(tsb);
			}

			//EDIT BUTTONS SEPERATOR
			Separator tss = new Separator();
            EditorToolbar.LayerToolbar.Items.Add(tss);
			ExtraLayerSeperators.Add(tss);

			//VIEW BUTTONS
			foreach (EditorSolution.EditorLayer el in EditorSolution.CurrentScene.OtherLayers)
			{
				EditLayerToggleButton tsb = new EditLayerToggleButton()
				{
					Text = el.Name,
                    LayerName = "Show" + el.Name.Replace(" ", "")
				};
                EditorToolbar.LayerToolbar.Items.Insert(EditorToolbar.LayerToolbar.Items.IndexOf(EditorToolbar.extraViewLayersSeperator), tsb);
				tsb.TextForeground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, Color.FromArgb(0x33AD35).R, Color.FromArgb(0x33AD35).G, Color.FromArgb(0x33AD35).B));
                tsb.IsLayerOptionsEnabled = true;


                _extraLayerViewButtons.Add(tsb);
			}

			//EDIT + VIEW BUTTONS LIST
			for (int i = 0; i < _extraLayerViewButtons.Count; i++)
			{
				ExtraLayerEditViewButtons.Add(_extraLayerViewButtons[i], _extraLayerEditButtons[i]);
			}

            UpdateDualButtonsControlsForLayer(FGLow, EditorToolbar.ShowFGLow, EditorToolbar.EditFGLow);
			UpdateDualButtonsControlsForLayer(FGHigh, EditorToolbar.ShowFGHigh, EditorToolbar.EditFGHigh);
			UpdateDualButtonsControlsForLayer(FGLower, EditorToolbar.ShowFGLower, EditorToolbar.EditFGLower);
			UpdateDualButtonsControlsForLayer(FGHigher, EditorToolbar.ShowFGHigher, EditorToolbar.EditFGHigher);
		}
		public void TearDownExtraLayerButtons()
		{
			foreach (var elb in ExtraLayerEditViewButtons)
			{
                EditorToolbar.LayerToolbar.Items.Remove(elb.Key);
                elb.Value.Click -= AdHocLayerEdit_Click;
				elb.Value.RightClick -= AdHocLayerEdit_RightClick;
                EditorToolbar.LayerToolbar.Items.Remove(elb.Value);
			}
			ExtraLayerEditViewButtons.Clear();


			foreach (var els in ExtraLayerSeperators)
			{
                EditorToolbar.LayerToolbar.Items.Remove(els);
			}
			ExtraLayerSeperators.Clear();

		}
		/// <summary>
		/// Given a scene layer, configure the given visibiltiy and edit buttons which will control that layer.
		/// </summary>
		/// <param name="layer">The layer of the scene from which to extract a name.</param>
		/// <param name="visibilityButton">The button which controls the visibility of the layer.</param>
		/// <param name="editButton">The button which controls editing the layer.</param>
		private void UpdateDualButtonsControlsForLayer(EditorSolution.EditorLayer layer, ToggleButton visibilityButton, EditLayerToggleButton editButton)
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
			if (ClickType == MouseButton.Left && !Options.MultiLayerEditMode) Normal();
			else if (ClickType == MouseButton.Left && Options.MultiLayerEditMode) LayerA();
			else if (ClickType == MouseButton.Right && Options.MultiLayerEditMode) LayerB();

			void Normal()
			{
				EditLayerToggleButton tsb = sender as EditLayerToggleButton;
				Deselect(false);
				if (tsb.IsCheckedN.Value)
				{
					if (!ManiacEditor.Settings.MySettings.KeepLayersVisible)
					{
                        EditorToolbar.ShowFGLow.IsChecked = false;
                        EditorToolbar.ShowFGHigh.IsChecked = false;
                        EditorToolbar.ShowFGLower.IsChecked = false;
                        EditorToolbar.ShowFGHigher.IsChecked = false;
					}
                    EditorToolbar.EditFGLow.ClearCheckedItems(3);
                    EditorToolbar.EditFGHigh.ClearCheckedItems(3);
                    EditorToolbar.EditFGLower.ClearCheckedItems(3);
                    EditorToolbar.EditFGHigher.ClearCheckedItems(3);
                    EditorToolbar.EditEntities.ClearCheckedItems(3);

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
					if (!ManiacEditor.Settings.MySettings.KeepLayersVisible)
					{
                        EditorToolbar.ShowFGLow.IsChecked = false;
                        EditorToolbar.ShowFGHigh.IsChecked = false;
                        EditorToolbar.ShowFGLower.IsChecked = false;
                        EditorToolbar.ShowFGHigher.IsChecked = false;
					}
                    EditorToolbar.EditFGLow.ClearCheckedItems(1);
                    EditorToolbar.EditFGHigh.ClearCheckedItems(1);
                    EditorToolbar.EditFGLower.ClearCheckedItems(1);
                    EditorToolbar.EditFGHigher.ClearCheckedItems(1);
                    EditorToolbar.EditEntities.ClearCheckedItems(1);

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
					if (!ManiacEditor.Settings.MySettings.KeepLayersVisible)
					{
                        EditorToolbar.ShowFGLow.IsChecked = false;
                        EditorToolbar.ShowFGHigh.IsChecked = false;
                        EditorToolbar.ShowFGLower.IsChecked = false;
                        EditorToolbar.ShowFGHigher.IsChecked = false;
					}
                    EditorToolbar.EditFGLow.ClearCheckedItems(2);
                    EditorToolbar.EditFGHigh.ClearCheckedItems(2);
                    EditorToolbar.EditFGLower.ClearCheckedItems(2);
                    EditorToolbar.EditFGHigher.ClearCheckedItems(2);
                    EditorToolbar.EditEntities.ClearCheckedItems(2);

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
        #region Mod Config List Stuff
        public MenuItem CreateModConfigMenuItem(int i)
        {
            MenuItem newItem = new MenuItem()
            {
                Header = ManiacEditor.Settings.MySettings.ModLoaderConfigsNames[i],
                Tag = ManiacEditor.Settings.MySettings.ModLoaderConfigs[i]
            };
            newItem.Click += ModConfigItemClicked;
            if (newItem.Tag.ToString() == ManiacEditor.Settings.MySettings.LastModConfig) newItem.IsChecked = true;
            return newItem;
        }
        private void ModConfigItemClicked(object sender, RoutedEventArgs e)
        {
            var modConfig_CheckedItem = (sender as MenuItem);
            SelectConfigToolStripMenuItem_Click(modConfig_CheckedItem);
            ManiacEditor.Settings.MySettings.LastModConfig = modConfig_CheckedItem.Tag.ToString();
        }
        private void SelectConfigToolStripMenuItem_Click(MenuItem modConfig_CheckedItem)
        {
            var allItems = EditorToolbar.selectConfigToolStripMenuItem.Items.Cast<System.Windows.Controls.MenuItem>().ToArray();
            foreach (var item in allItems)
            {
                item.IsChecked = false;
            }
            modConfig_CheckedItem.IsChecked = true;

        }
        #endregion
        #region Recent Data Folder Methods
        public void ResetDataDirectoryToAndResetScene(string newDataDirectory, bool forceBrowse = false, bool forceSceneSelect = false)
        {
            if (FileHandler.AllowSceneUnloading() != true) return;
            EditorSolution.UnloadScene();
            Settings.UseDefaultPrefrences();
            DataDirectory = newDataDirectory;
            AddRecentDataFolder(newDataDirectory);
            bool goodGameConfig = SetGameConfig();
            if (goodGameConfig == true)
            {
                if (forceBrowse) OpenScene(true);
                else if (forceSceneSelect) OpenScene(false);
                else OpenScene();

            }


        }
        public void UpdateDataFolderLabel(object sender, RoutedEventArgs e)
        {
            string dataFolderTag_Normal = "Data Directory: {0}";

            EditorStatusBar._baseDataDirectoryLabel.Tag = dataFolderTag_Normal;
            UpdateDataFolderLabel();
            Options.ShowingDataDirectory = true;
        }
        private void UpdateDataFolderLabel(string dataDirectory = null)
        {
            if (dataDirectory != null) EditorStatusBar._baseDataDirectoryLabel.Content = string.Format(EditorStatusBar._baseDataDirectoryLabel.Tag.ToString(), dataDirectory);
            else EditorStatusBar._baseDataDirectoryLabel.Content = string.Format(EditorStatusBar._baseDataDirectoryLabel.Tag.ToString(), DataDirectory);
        }
        public void AddRecentDataFolder(string dataDirectory)
        {
            try
            {
                var mySettings = Properties.Settings.Default;
                var dataDirectories = ManiacEditor.Settings.MySettings.DataDirectories;

                if (dataDirectories == null)
                {
                    dataDirectories = new StringCollection();
                    ManiacEditor.Settings.MySettings.DataDirectories = dataDirectories;
                }

                if (dataDirectories.Contains(dataDirectory)) dataDirectories.Remove(dataDirectory);

                if (dataDirectories.Count >= 10)
                {
                    for (int i = 9; i < dataDirectories.Count; i++) dataDirectories.RemoveAt(i);
                }

                dataDirectories.Insert(0, dataDirectory);

                ManiacEditor.Settings.MySettings.Save();

                UpdateDataFolderLabel(dataDirectory);


            }
            catch (Exception ex)
            {
                Debug.Write("Failed to add data folder to recent list: " + ex);
            }
        }
        #endregion
        #region Recent Scenes Methods

        public void RefreshRecentScenes()
        {
            if (RecentsList.Collection.List.Count > 0)
            {

                EditorMenuBar.NoRecentScenesItem.Visibility = Visibility.Collapsed;
                StartScreen.NoRecentsLabel1.Visibility = Visibility.Collapsed;
                CleanUpRecentScenesList();

                foreach (var RecentItem in RecentsList.Collection.List)
                {
                    RecentSceneItems.Add(new Tuple<MenuItem, MenuItem>(CreateRecentScenesMenuLink(RecentItem.EntryName), CreateRecentScenesMenuLink(RecentItem.EntryName, true)));
                }

                foreach (var menuItem in RecentSceneItems.Reverse())
                {
                    EditorMenuBar.RecentScenes.Items.Insert(0, menuItem.Item1);
                    StartScreen.RecentScenesList.Children.Insert(0, menuItem.Item2);
                }
            }
            else
            {
                EditorMenuBar.NoRecentScenesItem.Visibility = Visibility.Visible;
                StartScreen.NoRecentsLabel1.Visibility = Visibility.Visible;
            }
        }

        private MenuItem CreateRecentScenesMenuLink(string target, bool startScreenEntry = false)
        {

            MenuItem newItem = new MenuItem();
            TextBlock label = new TextBlock();

            label.Text = target.Replace("/n/n", Environment.NewLine);
            newItem.Tag = target;
            newItem.Header = label;
            newItem.Click += RecentSceneEntryClicked;
            return newItem;
        }
        public void RecentSceneEntryClicked(object sender, RoutedEventArgs e)
        {
            if (FileHandler.AllowSceneUnloading() != true) return;
            EditorSolution.UnloadScene();
            var menuItem = sender as MenuItem;
            string entryName = menuItem.Tag.ToString();
            var item = RecentsList.Collection.List.Where(x => x.EntryName == entryName).FirstOrDefault();
            DataDirectory = item.DataDirectory;
            FileHandler.OpenSceneFromSaveState(item.DataDirectory, item.Result, item.LevelID, item.isEncore, item.CurrentName, item.CurrentZone, item.CurrentSceneID, item.Browsed, item.ResourcePacks);
            RecentsList.AddRecentFile(item);
        }
        private void CleanUpRecentScenesList()
        {
            foreach (var menuItem in RecentSceneItems)
            {
                menuItem.Item1.Click -= RecentSceneEntryClicked;
                menuItem.Item2.Click -= RecentSceneEntryClicked;
                EditorMenuBar.RecentScenes.Items.Remove(menuItem.Item1);
                StartScreen.RecentScenesList.Children.Remove(menuItem.Item2);
            }
            RecentSceneItems.Clear();
        }

        #endregion
        #region Recent Data Sources Methods


        public void RefreshDataSources()
        {
            if (RecentDataSourcesList.Collection.List.Count > 0)
            {

                EditorMenuBar.NoRecentDataSources.Visibility = Visibility.Collapsed;
                StartScreen.NoRecentsLabel2.Visibility = Visibility.Collapsed;

                CleanUpDataSourcesList();

                foreach (var RecentItem in RecentDataSourcesList.Collection.List)
                {
                    RecentDataSourceItems.Add(new Tuple<MenuItem, MenuItem>(CreateRecentDataSourceMenuLink(RecentItem.EntryName), CreateRecentDataSourceMenuLink(RecentItem.EntryName, true)));
                }

                foreach (var menuItem in RecentDataSourceItems.Reverse())
                {
                    EditorMenuBar.RecentDataSources.Items.Insert(0, menuItem.Item1);
                    StartScreen.RecentDataContextList.Children.Insert(0, menuItem.Item2);
                }
            }
            else
            {
                EditorMenuBar.NoRecentDataSources.Visibility = Visibility.Visible;
                StartScreen.NoRecentsLabel2.Visibility = Visibility.Visible;
            }
        }

        private MenuItem CreateRecentDataSourceMenuLink(string target, bool wrapText = false)
        {
            MenuItem newItem = new MenuItem();
            TextBlock label = new TextBlock();
            label.Text = target.Replace("/n/n", Environment.NewLine);
            if (wrapText) label.TextWrapping = TextWrapping.Wrap;
            newItem.Header = label;
            newItem.Tag = target;
            newItem.Click += RecentDataSourceEntryClicked;
            return newItem;
        }
        public void RecentDataSourceEntryClicked(object sender, RoutedEventArgs e)
        {
            if (FileHandler.AllowSceneUnloading() != true) return;
            EditorSolution.UnloadScene();
            var menuItem = sender as MenuItem;
            string entryName = menuItem.Tag.ToString();
            var item = RecentDataSourcesList.Collection.List.Where(x => x.EntryName == entryName).FirstOrDefault();
            DataDirectory = item.DataDirectory;
            FileHandler.OpenSceneSelectFromPreviousConfiguration(item);
            RecentDataSourcesList.AddRecentFile(item);
        }
        private void CleanUpDataSourcesList()
        {
            foreach (var menuItem in RecentDataSourceItems)
            {
                menuItem.Item1.Click -= RecentDataSourceEntryClicked;
                menuItem.Item2.Click -= RecentDataSourceEntryClicked;
                EditorMenuBar.RecentDataSources.Items.Remove(menuItem.Item1);
                StartScreen.RecentDataContextList.Children.Remove(menuItem.Item2);
            }
            RecentDataSourceItems.Clear();

        }


        #endregion

        private void LeftToolbarToolbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (UI != null && ZoomModel != null && StartScreen.Visibility != Visibility.Visible)
            {
                if (LeftToolbarToolbox.SelectedIndex == 0)
                {
                    UI.UpdateToolbars(false, false);
                }
                else
                {
                    UI.UpdateToolbars(false, true);
                }
                Editor.Instance.Editor_Resize(null, null);
            }

        }


        #endregion

        #region Reworked Regions



        #endregion


    }
}
