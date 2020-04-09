using IWshRuntimeLibrary;
using ManiacEditor.Actions;
using ManiacEditor.Entity_Renders;
using ManiacEditor.Controls;
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
using ManiacEditor.Controls.Global;
using ManiacEditor.Enums;
using ManiacEditor.EventHandlers;
using ManiacEditor.Extensions;
using System.Windows.Forms.Integration;


namespace ManiacEditor.Controls.Editor
{
    public partial class MainEditor : Window
    {
        #region Definitions

        public static ManiacEditor.Controls.Editor.MainEditor Instance { get; set; }

        #region Editor Collections
        public List<string> ObjectList = new List<string>(); //All Gameconfig + Stageconfig Object names (Unused)
        public List<string> userDefinedSpritePaths = new List<string>();
        public Dictionary<string, string> userDefinedEntityRenderSwaps = new Dictionary<string, string>();
        public System.ComponentModel.BindingList<TextBlock> SplineSelectedObjectSpawnList = new System.ComponentModel.BindingList<TextBlock>();
        #endregion

        #region Clipboards
        public Tuple<Dictionary<Point, ushort>, Dictionary<Point, ushort>> TilesClipboard;
        public Dictionary<Point, ushort> FindReplaceClipboard;
        public Dictionary<Point, ushort> TilesClipboardEditable;
        public List<Classes.Scene.EditorEntity> ObjectsClipboard;
        #endregion

        #region Collision Colours
        public Color CollisionAllSolid { get; set; } = Color.White;
        public Color CollisionTopOnlySolid { get; set; } = Color.Yellow;
        public Color CollisionLRDSolid { get; set; } = Color.Red;
        #endregion

        #region Internal/Public/Vital Classes
        internal Classes.Scene.EditorBackground EditBackground;
        public Classes.Scene.EditorChunks Chunks;
        public Methods.Layers.TileFindReplace FindAndReplace;
        public ManiacEditor.Controls.TileManiac.CollisionEditor TileManiacInstance = new ManiacEditor.Controls.TileManiac.CollisionEditor();
        #endregion

        #region Controls
        public ManiacEditor.Controls.Editor.Toolbars.TilesToolbar TilesToolbar = null;
        public ManiacEditor.Controls.Editor.Toolbars.EntitiesToolbar EntitiesToolbar = null;
        public ManiacEditor.Controls.Editor.Elements.StartScreen StartScreen;
        #endregion

        #endregion

        #region Init
        public MainEditor()
        {
            ManiacEditor.Methods.ProgramBase.Log.InfoFormat("Setting Up the Map Editor...");

            Methods.Internal.Theming.UpdateInstance(this);
            Methods.Internal.Settings.UpdateInstance(this);
            Methods.Editor.Solution.UpdateInstance(this);
            Controls.Global.Controls.RetroEDTileList.UpdateInstance(this);

            Instance = this;
            InitializeComponent();
            Methods.Internal.Theming.SetTheme();

            ElementHost.EnableModelessKeyboardInterop(this);

            System.Windows.Application.Current.MainWindow = this;

            InitilizeEditor();

            if (ManiacEditor.Properties.Settings.MyDevSettings.UseAutoForcefulStartup)
            {
                Methods.Editor.SolutionLoader.OpenSceneForceFully();
            }
        }
        public void InitilizeEditor()
        {
            this.Activated += new System.EventHandler(this.Editor_Activated);

            EditorToolbar.ExtraLayerEditViewButtons = new Dictionary<Controls.Global.EditLayerToggleButton, Controls.Global.EditLayerToggleButton>();
            EditorToolbar.ExtraLayerSeperators = new List<Separator>();



            //Old Classes
            FindAndReplace = new Methods.Layers.TileFindReplace(this);

            //Controls
            StartScreen = new ManiacEditor.Controls.Editor.Elements.StartScreen(this);

            //Classes
            Classes.Prefrences.RecentsRefrenceState.UpdateInstance(this);
            Methods.Editor.SolutionState.UpdateInstance(this);
            Controls.Editor.Elements.Toolbar.UpdateInstance(this);
            Methods.Entities.EntityDrawing.UpdateInstance(this);
            Methods.Entities.SplineSpawning.UpdateInstance(this);
            Methods.Editor.SolutionPaths.UpdateInstance(this);
            Classes.Prefrences.SceneCurrentSettings.UpdateInstance(this);
            Methods.Internal.UserInterface.UpdateInstance(this);
            Methods.Internal.Controls.UpdateInstance(this);
            ManiacEditor.Methods.Editor.SolutionLoader.UpdateInstance(this);
            Classes.Prefrences.SceneHistoryStorage.Initilize(this);
            ManiacEditor.Classes.Prefrences.DataStateHistoryStorage.Initilize(this);
            Methods.ProgramLauncher.UpdateInstance(this);
            Methods.Runtime.GameHandler.UpdateInstance(this);
            ViewPanel.UpdateInstance(this);
            ManiacEditor.Methods.Editor.EditorActions.UpdateInstance(this);
            ViewPanel.InfoHUD.UpdateInstance(this);
            MenuBar.UpdateInstance(this);
            ViewPanel.SplitContainer.UpdateInstance(this);
            Classes.Rendering.GraphicsTileDrawing.UpdateInstance(this);

            EditorStatusBar.UpdateFilterButtonApperance();

            this.Title = String.Format("Maniac Editor - Generations Edition {0}", Methods.ProgramBase.GetCasualVersion());

            Extensions.ExternalExtensions.AllocConsole();
            Extensions.ExternalExtensions.HideConsoleWindow();
            RefreshCollisionColours();
            ViewPanel.SharpPanel.UpdateGraphicsPanelControls();
            Methods.Internal.UserInterface.UpdateControls();
            Methods.Internal.Settings.TryLoadSettings();

            Methods.Internal.UserInterface.Misc.UpdateStartScreen(true, true);

            ViewPanel.SharpPanel.InitalizeGraphicsPanel();
        }

        #endregion

        #region Editor Events
        private void Editor_Activated(object sender, EventArgs e)
        {
            ViewPanel.SharpPanel.GraphicPanel.Focus();
            if (TileManiacInstance.HasConfigBeenModified)
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
                var mySettings = Properties.Settings.MySettings;
                ManiacEditor.Properties.Settings.MySettings.IsMaximized = WindowState == System.Windows.WindowState.Maximized;
                Properties.Settings.SaveAllSettings();
            }
            catch (Exception ex)
            {
                Debug.Write("Failed to write settings: " + ex);
            }

            ViewPanel.SharpPanel.Dispose();
            //editorView = null;
            ViewPanel.SharpPanel.Host.Child.Dispose();
            //host = null;



        }
        private void Editor_KeyDown(object sender, KeyEventArgs e)
        {
            var e2 = KeyEventExts.ToWinforms(e);
            if (e2 != null && ViewPanel.IsFocused)
            {
                Methods.Internal.Controls.GraphicPanel_OnKeyDown(sender, e2);
            }

        }
        private void Editor_KeyUp(object sender, KeyEventArgs e)
        {
            var e2 = KeyEventExts.ToWinforms(e);
            if (e2 != null && ViewPanel.IsFocused)
            {
                Methods.Internal.Controls.GraphicPanel_OnKeyUp(sender, e2);
            }

        }
        private void Editor_Loaded(object sender, RoutedEventArgs e)
        {

        }
        private void Editor_Resize(object sender, SizeChangedEventArgs e)
        {

        }
        public void Editor_Run()
        {
            Show();
            Focus();
            ViewPanel.SharpPanel.GraphicPanel.Show();
            ViewPanel.SharpPanel.GraphicPanel.Run();

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

                if (Methods.Editor.SolutionState.UseEncoreColors)
                {
                    if (Methods.Editor.Solution.CurrentTiles != null) Methods.Editor.Solution.CurrentTiles?.Image.Reload(Methods.Editor.SolutionPaths.EncorePalette[0]);
                }
                else
                {
                    if (Methods.Editor.Solution.CurrentTiles != null) Methods.Editor.Solution.CurrentTiles?.Image.Reload();
                }

            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }
        public void DisposeTextures()
        {
            if (Methods.Editor.Solution.CurrentScene != null)
            {
                // Make sure to dispose the textures of the extra layers too
                if (Methods.Editor.Solution.CurrentTiles != null) Methods.Editor.Solution.CurrentTiles?.Dispose();
                if (Methods.Editor.Solution.FGHigh != null) Methods.Editor.Solution.FGHigh.DisposeTextures();
                if (Methods.Editor.Solution.FGLow != null) Methods.Editor.Solution.FGLow.DisposeTextures();
                if (Methods.Editor.Solution.FGHigher != null) Methods.Editor.Solution.FGHigher.DisposeTextures();
                if (Methods.Editor.Solution.FGLower != null) Methods.Editor.Solution.FGLower.DisposeTextures();

                foreach (var el in Methods.Editor.Solution.CurrentScene.OtherLayers)
                {
                    el.DisposeTextures();
                }
            }
        }
        public void RefreshCollisionColours(bool RefreshMasks = false)
        {
            try
            {
                if (Methods.Editor.Solution.CurrentScene != null && Methods.Editor.Solution.CurrentTiles != null)
                {
                    switch (Methods.Editor.SolutionState.CollisionPreset)
                    {
                        case 2:
                            CollisionAllSolid = Methods.Editor.SolutionState.CollisionSAColour;
                            CollisionTopOnlySolid = Methods.Editor.SolutionState.CollisionTOColour;
                            CollisionLRDSolid = Methods.Editor.SolutionState.CollisionLRDColour;
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

        #region Mod Config List Stuff
        public MenuItem CreateModConfigMenuItem(int i)
        {
            MenuItem newItem = new MenuItem()
            {
                Header = ManiacEditor.Properties.Settings.MySettings.ModLoaderConfigsNames[i],
                Tag = ManiacEditor.Properties.Settings.MySettings.ModLoaderConfigs[i]
            };
            newItem.Click += ModConfigItemClicked;
            if (newItem.Tag.ToString() == ManiacEditor.Properties.Settings.MySettings.LastModConfig) newItem.IsChecked = true;
            return newItem;
        }
        private void ModConfigItemClicked(object sender, RoutedEventArgs e)
        {
            var modConfig_CheckedItem = (sender as MenuItem);
            SelectConfigToolStripMenuItem_Click(modConfig_CheckedItem);
            ManiacEditor.Properties.Settings.MySettings.LastModConfig = modConfig_CheckedItem.Tag.ToString();
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
    }
}
