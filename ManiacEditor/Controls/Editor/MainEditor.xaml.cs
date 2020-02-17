using IWshRuntimeLibrary;
using ManiacEditor.Actions;
using ManiacEditor.Entity_Renders;
using ManiacEditor.Controls;
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
using ManiacEditor.Controls.Editor.Controls;
using ManiacEditor.Enums;
using ManiacEditor.Event_Handlers;
using ManiacEditor.Extensions;


namespace ManiacEditor.Controls.Editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainEditor : Window
    {
        #region Definitions

        public static ManiacEditor.Controls.Editor.MainEditor Instance;

        #region Misc
        public System.Timers.Timer Timer = new System.Timers.Timer();
        #endregion

        #region Extra Layer Buttons
        public IDictionary<EditLayerToggleButton, EditLayerToggleButton> ExtraLayerEditViewButtons;
        public IList<Separator> ExtraLayerSeperators; //Used for Adding Extra Seperators along side Extra Edit/View Layer Buttons
        #endregion

        #region Editor Collections
        public List<string> ObjectList = new List<string>(); //All Gameconfig + Stageconfig Object names (Unused)
        public IList<Tuple<MenuItem, MenuItem>> RecentSceneItems;
        public IList<Tuple<MenuItem, MenuItem>> RecentDataSourceItems;
        public List<string> userDefinedSpritePaths = new List<string>();
        public Dictionary<string, string> userDefinedEntityRenderSwaps = new Dictionary<string, string>();
        public System.ComponentModel.BindingList<TextBlock> SplineSelectedObjectSpawnList = new System.ComponentModel.BindingList<TextBlock>();
        #endregion

        #region Undo/Redo Stacks
        public Stack<IAction> UndoStack { get; set; } = new Stack<IAction>(); //Undo Actions Stack
        public Stack<IAction> RedoStack { get; set; } = new Stack<IAction>(); //Redo Actions Stack
        #endregion

        #region Clipboards
        public Tuple<Dictionary<Point, ushort>, Dictionary<Point, ushort>> TilesClipboard;
        public Dictionary<Point, ushort> FindReplaceClipboard;
        public Dictionary<Point, ushort> TilesClipboardEditable;
        public List<Classes.Editor.Scene.Sets.EditorEntity> entitiesClipboard;
        #endregion

        #region Collision Colours
        public Color CollisionAllSolid = Color.White;
        public Color CollisionTopOnlySolid = Color.Yellow;
        public Color CollisionLRDSolid = Color.Red;
        #endregion

        #region Internal/Public/Vital Classes
        internal Classes.Editor.Scene.EditorBackground EditBackground;
        public Methods.Entities.EntityDrawing EntityDrawing;
        public Classes.Editor.SolutionState StateModel;
        public Classes.Editor.Scene.EditorChunks Chunks;
        public Methods.Layers.TileFindReplace FindAndReplace;
        public Methods.Runtime.ProcessMemory GameMemory = new Methods.Runtime.ProcessMemory(); //Allows us to write hex codes like cheats, etc.
        public ManiacEditor.Controls.TileManiac.CollisionEditor TileManiacInstance = new ManiacEditor.Controls.TileManiac.CollisionEditor();
        #endregion

        #region Controls
        public ManiacEditor.Controls.Editor.Toolbars.TilesToolbar.TilesToolbar TilesToolbar = null;
        public ManiacEditor.Controls.Editor.Toolbars.EntitiesToolbar.EntitiesToolbar EntitiesToolbar = null;
        public ManiacEditor.Controls.Editor.Elements.StartScreen StartScreen;
        #endregion



        #endregion

        #region Editor Initalizing Methods
        public MainEditor(string dataDir = "", string scenePath = "", string modPath = "", int levelID = 0, bool ShortcutLaunch = false, int shortcutLaunchMode = 0, bool isEncoreMode = false, int X = 0, int Y = 0, double _ZoomedLevel = 0.0, int MegaManiacInstanceID = -1)
        {

            ManiacEditor.Methods.ProgramBase.Log.InfoFormat("Setting Up the Map Editor...");

            Methods.Internal.Theming.UpdateInstance(this);
            Methods.Internal.Settings.UpdateInstance(this);

            Methods.Internal.Theming.UseDarkTheme_WPF(ManiacEditor.Methods.Settings.MySettings.NightMode);
            Instance = this;
            InitializeComponent();

            Timer.Interval = 1;
            Timer.Elapsed += Timer_Elapsed;
            Timer.Start();



            System.Windows.Application.Current.MainWindow = this;

            InitilizeEditor();
            /*
            try
            {

            }
            catch (Exception ex)
            {
                Debug.Print("Couldn't Initialize Editor!" + ex.ToString());
                throw ex;
            }*/

            if (ManiacEditor.Methods.Settings.MyDevSettings.UseAutoForcefulStartup) OpenSceneForceFully();
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (Classes.Editor.Solution.CurrentScene != null)
            {
                foreach (var layer in Classes.Editor.Solution.CurrentScene.AllLayers)
                {
                    layer.UpdateLayerScrollIndex();
                }
            }
        }

        public void InitilizeEditor()
        {
            this.Activated += new System.EventHandler(this.Editor_Activated);

            ExtraLayerEditViewButtons = new Dictionary<EditLayerToggleButton, EditLayerToggleButton>();
            ExtraLayerSeperators = new List<Separator>();

            RecentSceneItems = new List<Tuple<MenuItem, MenuItem>>();
            RecentDataSourceItems = new List<Tuple<MenuItem, MenuItem>>();

            //Old Classes
            EntityDrawing = new Methods.Entities.EntityDrawing(this);
            StateModel = new Classes.Editor.SolutionState(this);
            FindAndReplace = new Methods.Layers.TileFindReplace(this);

            //Controls
            StartScreen = new ManiacEditor.Controls.Editor.Elements.StartScreen(this);

            //Classes
            Classes.Editor.SolutionPaths.UpdateInstance(this);
            Methods.Prefrences.SceneCurrentSettings.UpdateInstance(this);
            Methods.Internal.UserInterface.UpdateInstance(this);
            Methods.Internal.Controls.UpdateInstance(this);
            ManiacEditor.Classes.Editor.SolutionLoader.UpdateInstance(this);
            Methods.Prefrences.SceneHistoryStorage.Initilize(this);
            ManiacEditor.Methods.Prefrences.DataStateHistoryStorage.Initilize(this);
            Methods.ProgramLauncher.UpdateInstance(this);
            Methods.Prefrences.DataPackStorage.Initilize(this);
            Methods.Runtime.GameHandler.UpdateInstance(this);
            ViewPanel.UpdateInstance(this);
            ManiacEditor.Classes.Editor.EditorActions.UpdateInstance(this);
            ViewPanel.InfoHUD.UpdateInstance(this);
            MenuBar.UpdateInstance(this);
            ViewPanel.SplitContainer.UpdateInstance(this);

            EditorStatusBar.UpdateFilterButtonApperance(true);

            this.Title = String.Format("Maniac Editor - Generations Edition {0}", Methods.ProgramBase.GetCasualVersion());

            Extensions.ExternalExtensions.AllocConsole();
            Extensions.ExternalExtensions.HideConsoleWindow();
            RefreshCollisionColours();
            ViewPanel.SharpPanel.ResizeGraphicsPanel();
            Methods.Internal.UserInterface.UpdateControls();
            Methods.Internal.Settings.TryLoadSettings();


            UpdateStartScreen(true, true);
        }

        #endregion

        #region Open Scene Methods
        public void OpenSceneForceFully()
        {
            var item = ManiacEditor.Methods.Prefrences.SceneHistoryStorage.Collection.List.FirstOrDefault();
            if (item != null)
            {
                Classes.Editor.Solution.UnloadScene();
                ManiacEditor.Classes.Editor.SolutionPaths.CurrentSceneData = item.SceneState;
                ManiacEditor.Classes.Editor.SolutionLoader.OpenSceneFromSaveState(item);
            }
        }

        #endregion

        #region Editor Events
        private void Editor_Activated(object sender, EventArgs e)
        {
            ViewPanel.SharpPanel.GraphicPanel.Focus();
            if (TileManiacInstance.hasModified)
            {
                Methods.Internal.UserInterface.ReloadSpritesAndTextures();
            }

        }
        private void Editor_FormClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Methods.ProgramLauncher.ManiaPalConnector != null) Methods.ProgramLauncher.ManiaPalConnector.Kill();

            try
            {
                Methods.Runtime.GameHandler.GameRunning = false;
                var mySettings = Methods.Settings.MySettings;
                ManiacEditor.Methods.Settings.MySettings.IsMaximized = WindowState == System.Windows.WindowState.Maximized;
                Methods.Settings.SaveAllSettings();
            }
            catch (Exception ex)
            {
                Debug.Write("Failed to write settings: " + ex);
            }

            if (ManiaHost._process != null)
            {
                ManiaHost.ForceKillSonicMania();
            }

            ViewPanel.SharpPanel.Dispose();
            //editorView = null;
            ViewPanel.SharpPanel.Host.Child.Dispose();
            //host = null;



        }
        private void Editor_KeyDown(object sender, KeyEventArgs e)
        {
            var e2 = KeyEventExts.ToWinforms(e);
            if (e2 != null)
            {
                Methods.Internal.Controls.GraphicPanel_OnKeyDown(sender, e2);
            }

        }
        private void Editor_KeyUp(object sender, KeyEventArgs e)
        {
            var e2 = KeyEventExts.ToWinforms(e);
            if (e2 != null)
            {
                Methods.Internal.Controls.GraphicPanel_OnKeyUp(sender, e2);
            }

        }
        public void Editor_Resize(object sender, RoutedEventArgs e) { ViewPanel.SharpPanel.ResizeGraphicsPanel(); }
        private void Editor_Loaded(object sender, RoutedEventArgs e)
        {
            ViewPanel.SharpPanel.InitalizeGraphicsPanel();
        }
        #endregion

        #region Misc Events
        public void DrawLayers(int drawOrder = 0)
        {

            // Future Implementation

            /*
            List<int> layerDrawingOrder = new List<int> { };
            var allLayers = Classes.Edit.Scene.EditorSolution.Scene.AllLayers;
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

            var _extraViewLayer = Classes.Editor.Solution.CurrentScene.LayerByDrawingOrder.FirstOrDefault(el => el.Layer.DrawingOrder.Equals(drawOrder));
            _extraViewLayer.Draw(ViewPanel.SharpPanel.GraphicPanel);
        }
        public void Run()
        {
            Show();
            Focus();
            ViewPanel.SharpPanel.GraphicPanel.Show();
            ViewPanel.SharpPanel.GraphicPanel.Run();

        }
        public void UpdateStartScreen(bool visible, bool firstLoad = false)
        {
            if (firstLoad)
            {
                ViewPanel.OverlayPanel.Children.Add(StartScreen);
                StartScreen.SelectScreen.ReloadRecentsTree();
                ViewPanel.SharpPanel.Visibility = Visibility.Hidden;
                Instance.ViewPanel.SplitContainer.UpdateToolbars(false, false);
                RefreshRecentScenes();
                RefreshDataSources();
            }
            if (visible)
            {
                StartScreen.Visibility = Visibility.Visible;
                StartScreen.SelectScreen.ReloadRecentsTree();
                ViewPanel.SharpPanel.Visibility = Visibility.Hidden;
                Instance.ViewPanel.SplitContainer.UpdateToolbars(false, false);
                RefreshRecentScenes();
                RefreshDataSources();
            }
            else
            {
                StartScreen.Visibility = Visibility.Hidden;
                StartScreen.SelectScreen.ReloadRecentsTree();
                ViewPanel.SharpPanel.Visibility = Visibility.Visible;
                Instance.ViewPanel.SplitContainer.UpdateToolbars(false, false);
            }

        }
        #endregion

        #region Asset Reloading (TO-MOVE)
        public void ReloadSpecificTextures(object sender, RoutedEventArgs e)
        {
            try
            {
                // release all our resources, and force a reload of the tiles
                // Classes.Edit.Scene.EditorSolution.Entities should take care of themselves
                DisposeTextures();

                if (Classes.Editor.SolutionState.UseEncoreColors)
                {
                    if (Classes.Editor.Solution.CurrentTiles != null) Classes.Editor.Solution.CurrentTiles?.Image.Reload(Classes.Editor.SolutionPaths.EncorePalette[0]);
                }
                else
                {
                    if (Classes.Editor.Solution.CurrentTiles != null) Classes.Editor.Solution.CurrentTiles?.Image.Reload();
                }

            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }
        public void DisposeTextures()
        {
            if (Classes.Editor.Solution.CurrentScene != null)
            {
                // Make sure to dispose the textures of the extra layers too
                if (Classes.Editor.Solution.CurrentTiles != null) Classes.Editor.Solution.CurrentTiles?.DisposeTextures();
                if (Classes.Editor.Solution.FGHigh != null) Classes.Editor.Solution.FGHigh.DisposeTextures();
                if (Classes.Editor.Solution.FGLow != null) Classes.Editor.Solution.FGLow.DisposeTextures();
                if (Classes.Editor.Solution.FGHigher != null) Classes.Editor.Solution.FGHigher.DisposeTextures();
                if (Classes.Editor.Solution.FGLower != null) Classes.Editor.Solution.FGLower.DisposeTextures();

                foreach (var el in Classes.Editor.Solution.CurrentScene.OtherLayers)
                {
                    el.DisposeTextures();
                }
            }
        }
        public void RefreshCollisionColours(bool RefreshMasks = false)
        {
            try
            {
                if (Classes.Editor.Solution.CurrentScene != null && Classes.Editor.Solution.CurrentTiles != null)
                {
                    switch (Classes.Editor.SolutionState.CollisionPreset)
                    {
                        case 2:
                            CollisionAllSolid = Classes.Editor.SolutionState.CollisionSAColour;
                            CollisionTopOnlySolid = Classes.Editor.SolutionState.CollisionTOColour;
                            CollisionLRDSolid = Classes.Editor.SolutionState.CollisionLRDColour;
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
            catch
            {

            }


        }
        #endregion

        #region Layer Toolbar Events
        public void SetupLayerButtons()
        {
            TearDownExtraLayerButtons();
            IList<EditLayerToggleButton> _extraLayerEditButtons = new List<EditLayerToggleButton>(); //Used for Extra Layer Edit Buttons
            IList<EditLayerToggleButton> _extraLayerViewButtons = new List<EditLayerToggleButton>(); //Used for Extra Layer View Buttons

            //EDIT BUTTONS
            foreach (Classes.Editor.Scene.Sets.EditorLayer el in Classes.Editor.Solution.CurrentScene.OtherLayers)
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
            foreach (Classes.Editor.Scene.Sets.EditorLayer el in Classes.Editor.Solution.CurrentScene.OtherLayers)
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

            UpdateDualButtonsControlsForLayer(Classes.Editor.Solution.FGLow, EditorToolbar.ShowFGLow, EditorToolbar.EditFGLow);
            UpdateDualButtonsControlsForLayer(Classes.Editor.Solution.FGHigh, EditorToolbar.ShowFGHigh, EditorToolbar.EditFGHigh);
            UpdateDualButtonsControlsForLayer(Classes.Editor.Solution.FGLower, EditorToolbar.ShowFGLower, EditorToolbar.EditFGLower);
            UpdateDualButtonsControlsForLayer(Classes.Editor.Solution.FGHigher, EditorToolbar.ShowFGHigher, EditorToolbar.EditFGHigher);
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
        private void UpdateDualButtonsControlsForLayer(Classes.Editor.Scene.Sets.EditorLayer layer, ToggleButton visibilityButton, EditLayerToggleButton editButton)
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
            if (ClickType == MouseButton.Left && !Classes.Editor.SolutionState.MultiLayerEditMode) Normal();
            else if (ClickType == MouseButton.Left && Classes.Editor.SolutionState.MultiLayerEditMode) LayerA();
            else if (ClickType == MouseButton.Right && Classes.Editor.SolutionState.MultiLayerEditMode) LayerB();

            void Normal()
            {
                EditLayerToggleButton tsb = sender as EditLayerToggleButton;
                Classes.Editor.EditorActions.Deselect(false);
                Classes.Editor.EditorActions.Deselect(false);
                if (tsb.IsCheckedN.Value)
                {
                    if (!ManiacEditor.Methods.Settings.MySettings.KeepLayersVisible)
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
                Classes.Editor.EditorActions.Deselect(false);
                if (tsb.IsCheckedA.Value)
                {
                    if (!ManiacEditor.Methods.Settings.MySettings.KeepLayersVisible)
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
                Classes.Editor.EditorActions.Deselect(false);
                if (tsb.IsCheckedB.Value)
                {
                    if (!ManiacEditor.Methods.Settings.MySettings.KeepLayersVisible)
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

            Methods.Internal.UserInterface.UpdateControls();
        }
        #endregion

        #region Mod Config List Stuff
        public MenuItem CreateModConfigMenuItem(int i)
        {
            MenuItem newItem = new MenuItem()
            {
                Header = ManiacEditor.Methods.Settings.MySettings.ModLoaderConfigsNames[i],
                Tag = ManiacEditor.Methods.Settings.MySettings.ModLoaderConfigs[i]
            };
            newItem.Click += ModConfigItemClicked;
            if (newItem.Tag.ToString() == ManiacEditor.Methods.Settings.MySettings.LastModConfig) newItem.IsChecked = true;
            return newItem;
        }
        private void ModConfigItemClicked(object sender, RoutedEventArgs e)
        {
            var modConfig_CheckedItem = (sender as MenuItem);
            SelectConfigToolStripMenuItem_Click(modConfig_CheckedItem);
            ManiacEditor.Methods.Settings.MySettings.LastModConfig = modConfig_CheckedItem.Tag.ToString();
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
        public void UpdateDataFolderLabel(object sender, RoutedEventArgs e)
        {
            string dataFolderTag_Normal = "Data Directory: {0}";

            EditorStatusBar._baseDataDirectoryLabel.Tag = dataFolderTag_Normal;
            UpdateDataFolderLabel();
            Classes.Editor.SolutionState.ShowingDataDirectory = true;
        }
        private void UpdateDataFolderLabel(string dataDirectory = null)
        {
            if (dataDirectory != null) EditorStatusBar._baseDataDirectoryLabel.Content = string.Format(EditorStatusBar._baseDataDirectoryLabel.Tag.ToString(), dataDirectory);
            else EditorStatusBar._baseDataDirectoryLabel.Content = string.Format(EditorStatusBar._baseDataDirectoryLabel.Tag.ToString(), ManiacEditor.Classes.Editor.SolutionPaths.CurrentSceneData.DataDirectory);
        }
        public void AddRecentDataFolder(string dataDirectory)
        {
            try
            {
                var mySettings = Methods.Settings.MySettings;
                var dataDirectories = ManiacEditor.Methods.Settings.MySettings.DataDirectories;

                if (dataDirectories == null)
                {
                    dataDirectories = new StringCollection();
                    ManiacEditor.Methods.Settings.MySettings.DataDirectories = dataDirectories;
                }

                if (dataDirectories.Contains(dataDirectory)) dataDirectories.Remove(dataDirectory);

                if (dataDirectories.Count >= 10)
                {
                    for (int i = 9; i < dataDirectories.Count; i++) dataDirectories.RemoveAt(i);
                }

                dataDirectories.Insert(0, dataDirectory);

                ManiacEditor.Methods.Options.GeneralSettings.Save();

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
            if (Methods.Prefrences.SceneHistoryStorage.Collection.List.Count > 0)
            {

                MenuBar.NoRecentScenesItem.Visibility = Visibility.Collapsed;
                StartScreen.NoRecentsLabel1.Visibility = Visibility.Collapsed;
                CleanUpRecentScenesList();

                foreach (var RecentItem in Methods.Prefrences.SceneHistoryStorage.Collection.List)
                {
                    RecentSceneItems.Add(new Tuple<MenuItem, MenuItem>(CreateRecentScenesMenuLink(RecentItem.EntryName), CreateRecentScenesMenuLink(RecentItem.EntryName, true)));
                }

                foreach (var menuItem in RecentSceneItems.Reverse())
                {
                    MenuBar.RecentScenes.Items.Insert(0, menuItem.Item1);
                    StartScreen.RecentScenesList.Children.Insert(0, menuItem.Item2);
                }
            }
            else
            {
                MenuBar.NoRecentScenesItem.Visibility = Visibility.Visible;
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
            if (ManiacEditor.Classes.Editor.SolutionLoader.AllowSceneUnloading() != true) return;
            Classes.Editor.Solution.UnloadScene();
            var menuItem = sender as MenuItem;
            string entryName = menuItem.Tag.ToString();
            var item = Methods.Prefrences.SceneHistoryStorage.Collection.List.Where(x => x.EntryName == entryName).FirstOrDefault();
            ManiacEditor.Classes.Editor.SolutionLoader.OpenSceneFromSaveState(item);
            Methods.Prefrences.SceneHistoryStorage.AddRecentFile(item);
        }
        private void CleanUpRecentScenesList()
        {
            foreach (var menuItem in RecentSceneItems)
            {
                menuItem.Item1.Click -= RecentSceneEntryClicked;
                menuItem.Item2.Click -= RecentSceneEntryClicked;
                MenuBar.RecentScenes.Items.Remove(menuItem.Item1);
                StartScreen.RecentScenesList.Children.Remove(menuItem.Item2);
            }
            RecentSceneItems.Clear();
        }

        #endregion

        #region Recent Data Sources Methods

        public void RefreshDataSources()
        {
            if (ManiacEditor.Methods.Prefrences.DataStateHistoryStorage.Collection.List.Count > 0)
            {

                MenuBar.NoRecentDataSources.Visibility = Visibility.Collapsed;
                StartScreen.NoRecentsLabel2.Visibility = Visibility.Collapsed;

                CleanUpDataSourcesList();

                foreach (var RecentItem in ManiacEditor.Methods.Prefrences.DataStateHistoryStorage.Collection.List)
                {
                    RecentDataSourceItems.Add(new Tuple<MenuItem, MenuItem>(CreateRecentDataSourceMenuLink(RecentItem.EntryName), CreateRecentDataSourceMenuLink(RecentItem.EntryName, true)));
                }

                foreach (var menuItem in RecentDataSourceItems.Reverse())
                {
                    MenuBar.RecentDataSources.Items.Insert(0, menuItem.Item1);
                    StartScreen.RecentDataContextList.Children.Insert(0, menuItem.Item2);
                }
            }
            else
            {
                MenuBar.NoRecentDataSources.Visibility = Visibility.Visible;
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
            if (ManiacEditor.Classes.Editor.SolutionLoader.AllowSceneUnloading() != true) return;
            Classes.Editor.Solution.UnloadScene();
            var menuItem = sender as MenuItem;
            string entryName = menuItem.Tag.ToString();
            var item = ManiacEditor.Methods.Prefrences.DataStateHistoryStorage.Collection.List.Where(x => x.EntryName == entryName).FirstOrDefault();
            ManiacEditor.Classes.Editor.SolutionLoader.OpenSceneSelectFromPreviousDataPackSetup(item);
            ManiacEditor.Methods.Prefrences.DataStateHistoryStorage.AddRecentFile(item);
        }
        private void CleanUpDataSourcesList()
        {
            foreach (var menuItem in RecentDataSourceItems)
            {
                menuItem.Item1.Click -= RecentDataSourceEntryClicked;
                menuItem.Item2.Click -= RecentDataSourceEntryClicked;
                MenuBar.RecentDataSources.Items.Remove(menuItem.Item1);
                StartScreen.RecentDataContextList.Children.Remove(menuItem.Item2);
            }
            RecentDataSourceItems.Clear();

        }


        #endregion
    }
}
