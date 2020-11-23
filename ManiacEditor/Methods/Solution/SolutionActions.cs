using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Input;
using ManiacEditor.Controls.Global;
using ManiacEditor.Enums;
using ManiacEditor.Events;
using ManiacEditor.Extensions;
using ManiacEditor.Actions;
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
using IWshRuntimeLibrary;
using Clipboard = System.Windows.Clipboard;
using Color = System.Drawing.Color;
using DataObject = System.Windows.DataObject;
using File = System.IO.File;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MenuItem = System.Windows.Controls.MenuItem;
using Path = System.IO.Path;
using Point = System.Drawing.Point;
using Rectangle = System.Drawing.Rectangle;

namespace ManiacEditor.Methods.Solution
{
    public static class SolutionActions
    {
        #region Init
        private static Controls.Editor.MainEditor Instance { get; set; }
        public static void UpdateInstance(Controls.Editor.MainEditor mainEditor)
        {
            Instance = mainEditor;
        }
        #endregion

        #region Edit Functions

        #region Flipping

        private static void FlipTiles(bool Individual, FlipDirection Direction)
        {
            Methods.Solution.CurrentSolution.EditLayerA?.FlipPropertySelected(Direction, Individual);
            Methods.Solution.CurrentSolution.EditLayerB?.FlipPropertySelected(Direction, Individual);
            Methods.Solution.CurrentSolution.EditLayerC?.FlipPropertySelected(Direction, Individual);
            Methods.Solution.CurrentSolution.EditLayerD?.FlipPropertySelected(Direction, Individual);
            ManiacEditor.Actions.UndoRedoModel.UpdateEditLayersActions();
        }

        public static void FlipHorizontal()
        {
            if (ManiacEditor.Methods.Solution.SolutionState.Main.IsTilesEdit()) FlipTiles(false, FlipDirection.Horizontal);
            else if (ManiacEditor.Methods.Solution.SolutionState.Main.IsEntitiesEdit()) ManiacEditor.Classes.Scene.EditorEntities.FlipEntities(FlipDirection.Horizontal);
        }
        public static void FlipVertical()
        {
            if (ManiacEditor.Methods.Solution.SolutionState.Main.IsTilesEdit()) FlipTiles(false, FlipDirection.Veritcal);
            else if (ManiacEditor.Methods.Solution.SolutionState.Main.IsEntitiesEdit()) ManiacEditor.Classes.Scene.EditorEntities.FlipEntities(FlipDirection.Veritcal);
        }
        public static void FlipHorizontalIndividual()
        {
            if (ManiacEditor.Methods.Solution.SolutionState.Main.IsTilesEdit()) FlipTiles(true, FlipDirection.Horizontal);
        }
        public static void FlipVerticalIndividual()
        {
            if (ManiacEditor.Methods.Solution.SolutionState.Main.IsTilesEdit()) FlipTiles(true, FlipDirection.Veritcal);
        }

        #endregion

        #region Selection/Clipboard
        public static void PasteToChunks()
        {
            if (Methods.Solution.SolutionClipboard.TilesClipboard != null)
            {
                Instance.Chunks.ConvertClipboardtoMultiLayerChunk(Methods.Solution.SolutionClipboard.TilesClipboard);
                Instance.TilesToolbar?.ChunksReload();
                Methods.Internal.UserInterface.UpdateControls();
            }

        }
        public static void SelectAll()
        {
            if (ManiacEditor.Methods.Solution.SolutionState.Main.IsTilesEdit() && !ManiacEditor.Methods.Solution.SolutionState.Main.IsChunksEdit())
            {
                if (Methods.Solution.CurrentSolution.EditLayerA != null) Methods.Solution.CurrentSolution.EditLayerA?.SelectEverything();
                if (Methods.Solution.CurrentSolution.EditLayerB != null) Methods.Solution.CurrentSolution.EditLayerB?.SelectEverything();
                if (Methods.Solution.CurrentSolution.EditLayerC != null) Methods.Solution.CurrentSolution.EditLayerC?.SelectEverything();
                if (Methods.Solution.CurrentSolution.EditLayerD != null) Methods.Solution.CurrentSolution.EditLayerD?.SelectEverything();
                ManiacEditor.Actions.UndoRedoModel.UpdateEditLayersActions();
            }
            else if (ManiacEditor.Methods.Solution.SolutionState.Main.IsEntitiesEdit()) Classes.Scene.EditorEntities.SelectAll();

            Methods.Internal.UserInterface.UpdateControls();
            Methods.Solution.SolutionState.Main.RegionX1 = -1;
            Methods.Solution.SolutionState.Main.RegionY1 = -1;
        }
        public static void Delete()
        {
            if (ManiacEditor.Methods.Solution.SolutionState.Main.IsTilesEdit())
            {
                Methods.Solution.CurrentSolution.EditLayerA?.DeleteSelected();
                Methods.Solution.CurrentSolution.EditLayerB?.DeleteSelected();
                Methods.Solution.CurrentSolution.EditLayerC?.DeleteSelected();
                Methods.Solution.CurrentSolution.EditLayerD?.DeleteSelected();
                ManiacEditor.Actions.UndoRedoModel.UpdateEditLayersActions();
            }
            if (ManiacEditor.Methods.Solution.SolutionState.Main.IsEntitiesEdit()) Classes.Scene.EditorEntities.Delete();

            Methods.Internal.UserInterface.UpdateControls();
        }
        public static void Copy()
        {
            if (ManiacEditor.Methods.Solution.SolutionState.Main.IsTilesEdit())
            {
                Methods.Solution.SolutionMultiLayer.Copy();
            }
            else if (ManiacEditor.Methods.Solution.SolutionState.Main.IsEntitiesEdit() && Instance.EntitiesToolbar.IsFocused.Equals(false)) Classes.Scene.EditorEntities.Copy();

            Methods.Internal.UserInterface.UpdateControls();
        }
        public static void Duplicate()
        {
            if (ManiacEditor.Methods.Solution.SolutionState.Main.IsTilesEdit())
            {
                var copyDataA = Methods.Solution.CurrentSolution.EditLayerA?.GetClipboardData();
                var copyDataB = Methods.Solution.CurrentSolution.EditLayerB?.GetClipboardData();
                var copyDataC = Methods.Solution.CurrentSolution.EditLayerC?.GetClipboardData();
                var copyDataD = Methods.Solution.CurrentSolution.EditLayerD?.GetClipboardData();
                Methods.Solution.CurrentSolution.EditLayerA?.PasteClipboardData(new Point(16, 16), copyDataA);
                Methods.Solution.CurrentSolution.EditLayerB?.PasteClipboardData(new Point(16, 16), copyDataB);
                Methods.Solution.CurrentSolution.EditLayerC?.PasteClipboardData(new Point(16, 16), copyDataC);
                Methods.Solution.CurrentSolution.EditLayerD?.PasteClipboardData(new Point(16, 16), copyDataD);
                ManiacEditor.Actions.UndoRedoModel.UpdateEditLayersActions();
            }
            else if (ManiacEditor.Methods.Solution.SolutionState.Main.IsEntitiesEdit()) Classes.Scene.EditorEntities.Duplicate();

            Methods.Internal.UserInterface.UpdateControls();
        }
        public static void Cut()
        {
            if (ManiacEditor.Methods.Solution.SolutionState.Main.IsTilesEdit())
            {
                Methods.Solution.SolutionMultiLayer.Copy();
                Methods.Solution.CurrentSolution.EditLayerA?.DeleteSelected();
                Methods.Solution.CurrentSolution.EditLayerB?.DeleteSelected();
                Methods.Solution.CurrentSolution.EditLayerC?.DeleteSelected();
                Methods.Solution.CurrentSolution.EditLayerD?.DeleteSelected();
                ManiacEditor.Actions.UndoRedoModel.UpdateEditLayersActions();
            }
            else if (ManiacEditor.Methods.Solution.SolutionState.Main.IsEntitiesEdit() && Instance.EntitiesToolbar.IsFocused.Equals(false)) Classes.Scene.EditorEntities.Cut();

            Methods.Internal.UserInterface.UpdateControls();
        }
        public static void Paste()
        {
            try
            {
                if (ManiacEditor.Methods.Solution.SolutionState.Main.IsTilesEdit())
                {
                    Methods.Solution.SolutionMultiLayer.Paste();
                }
                else if (ManiacEditor.Methods.Solution.SolutionState.Main.IsEntitiesEdit() && Instance.EntitiesToolbar.IsFocused.Equals(false)) Classes.Scene.EditorEntities.Paste();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("There was a problem with pasting the content provided: " + Environment.NewLine + ex.Message);
            }

            Instance.ViewPanel.SharpPanel.GraphicPanel.Render();
            Methods.Internal.UserInterface.UpdateControls();
        }
        public static void Deselect(bool UpdateControls = true)
        {
            if (ManiacEditor.Methods.Solution.SolutionState.Main.IsEditing)
            {
                if (SolutionState.Main.IsTilesEdit())
                {
                    Methods.Solution.CurrentSolution.EditLayerA?.DeselectAll();
                    Methods.Solution.CurrentSolution.EditLayerB?.DeselectAll();
                    Methods.Solution.CurrentSolution.EditLayerC?.DeselectAll();
                    Methods.Solution.CurrentSolution.EditLayerD?.DeselectAll();
                    ManiacEditor.Actions.UndoRedoModel.UpdateEditLayersActions();
                }
                if (SolutionState.Main.IsEntitiesEdit()) Classes.Scene.EditorEntities.Deselect(); 
                if (UpdateControls) Methods.Internal.UserInterface.UpdateControls();
            }
        }

        #endregion

        #region Misc

        public static void ZoomIn()
        {
            Methods.Solution.SolutionState.Main.ZoomLevel += 1;
            if (Methods.Solution.SolutionState.Main.ZoomLevel >= 5) Methods.Solution.SolutionState.Main.ZoomLevel = 5;
            if (Methods.Solution.SolutionState.Main.ZoomLevel <= -5) Methods.Solution.SolutionState.Main.ZoomLevel = -5;

            Instance.ViewPanel.SharpPanel.UpdateZoomLevel(Methods.Solution.SolutionState.Main.ZoomLevel, new Point(0, 0));
        }
        public static void ZoomOut()
        {
            Methods.Solution.SolutionState.Main.ZoomLevel -= 1;
            if (Methods.Solution.SolutionState.Main.ZoomLevel >= 5) Methods.Solution.SolutionState.Main.ZoomLevel = 5;
            if (Methods.Solution.SolutionState.Main.ZoomLevel <= -5) Methods.Solution.SolutionState.Main.ZoomLevel = -5;

            Instance.ViewPanel.SharpPanel.UpdateZoomLevel(Methods.Solution.SolutionState.Main.ZoomLevel, new Point(0, 0));
        }
        private static void MoveTiles(System.Windows.Forms.KeyEventArgs e)
        {
            int x = 0, y = 0;
            int modifier = (ManiacEditor.Methods.Solution.SolutionState.Main.IsChunksEdit() ? 8 : 1);
            if (Methods.Solution.SolutionState.Main.UseMagnetMode)
            {
                switch (e.KeyData)
                {
                    case Keys.Up: y = (Methods.Solution.SolutionState.Main.UseMagnetYAxis ? -Methods.Solution.SolutionState.Main.MagnetSize : -1); break;
                    case Keys.Down: y = (Methods.Solution.SolutionState.Main.UseMagnetYAxis ? Methods.Solution.SolutionState.Main.MagnetSize : 1); break;
                    case Keys.Left: x = (Methods.Solution.SolutionState.Main.UseMagnetXAxis ? -Methods.Solution.SolutionState.Main.MagnetSize : -1); break;
                    case Keys.Right: x = (Methods.Solution.SolutionState.Main.UseMagnetXAxis ? Methods.Solution.SolutionState.Main.MagnetSize : 1); break;
                }
            }
            if (Methods.Solution.SolutionState.Main.EnableFasterNudge)
            {
                if (Methods.Solution.SolutionState.Main.UseMagnetMode)
                {
                    switch (e.KeyData)
                    {
                        case Keys.Up: y = (Methods.Solution.SolutionState.Main.UseMagnetYAxis ? -Methods.Solution.SolutionState.Main.MagnetSize * Methods.Solution.SolutionState.Main.FasterNudgeAmount : -1 - Methods.Solution.SolutionState.Main.FasterNudgeAmount); break;
                        case Keys.Down: y = (Methods.Solution.SolutionState.Main.UseMagnetYAxis ? Methods.Solution.SolutionState.Main.MagnetSize * Methods.Solution.SolutionState.Main.FasterNudgeAmount : 1 + Methods.Solution.SolutionState.Main.FasterNudgeAmount); break;
                        case Keys.Left: x = (Methods.Solution.SolutionState.Main.UseMagnetXAxis ? -Methods.Solution.SolutionState.Main.MagnetSize * Methods.Solution.SolutionState.Main.FasterNudgeAmount : -1 - Methods.Solution.SolutionState.Main.FasterNudgeAmount); break;
                        case Keys.Right: x = (Methods.Solution.SolutionState.Main.UseMagnetXAxis ? Methods.Solution.SolutionState.Main.MagnetSize * Methods.Solution.SolutionState.Main.FasterNudgeAmount : 1 + Methods.Solution.SolutionState.Main.FasterNudgeAmount); break;
                    }
                }
                else
                {
                    if (ManiacEditor.Methods.Solution.SolutionState.Main.IsChunksEdit())
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
                            case Keys.Up: y = (-1 - Methods.Solution.SolutionState.Main.FasterNudgeAmount) * modifier; break;
                            case Keys.Down: y = (1 + Methods.Solution.SolutionState.Main.FasterNudgeAmount) * modifier; break;
                            case Keys.Left: x = (-1 - Methods.Solution.SolutionState.Main.FasterNudgeAmount) * modifier; break;
                            case Keys.Right: x = (1 + Methods.Solution.SolutionState.Main.FasterNudgeAmount) * modifier; break;
                        }
                    }

                }

            }
            if (Methods.Solution.SolutionState.Main.UseMagnetMode == false && Methods.Solution.SolutionState.Main.EnableFasterNudge == false)
            {
                switch (e.KeyData)
                {
                    case Keys.Up: y = -1 * modifier; break;
                    case Keys.Down: y = 1 * modifier; break;
                    case Keys.Left: x = -1 * modifier; break;
                    case Keys.Right: x = 1 * modifier; break;
                }

            }
            Methods.Solution.CurrentSolution.EditLayerA?.MoveSelectedQuonta(new Point(x, y));
            Methods.Solution.CurrentSolution.EditLayerB?.MoveSelectedQuonta(new Point(x, y));
            Methods.Solution.CurrentSolution.EditLayerC?.MoveSelectedQuonta(new Point(x, y));
            Methods.Solution.CurrentSolution.EditLayerD?.MoveSelectedQuonta(new Point(x, y));

            ManiacEditor.Actions.UndoRedoModel.UpdateEditLayersActions();
        }
        public static void Move(System.Windows.Forms.KeyEventArgs e)
        {
            if (ManiacEditor.Methods.Solution.SolutionState.Main.IsTilesEdit()) MoveTiles(e);
            else if (ManiacEditor.Methods.Solution.SolutionState.Main.IsEntitiesEdit()) Classes.Scene.EditorEntities.MoveEntities(e);

            if (Instance.EntitiesToolbar != null) Instance.EntitiesToolbar.UpdateSelectedProperties();
        }
        public static void GoToPosition(int x, int y, bool CenterCoords = true)
        {
            if (Methods.Solution.SolutionState.Main.UnlockCamera) CenterCoords = true;

            CenterCoords = false;


            if (CenterCoords)
            {
                double ScreenMaxX = (int)Instance.ViewPanel.SharpPanel.ActualWidth * Methods.Solution.SolutionState.Main.Zoom;
                double ScreenMaxY = (int)Instance.ViewPanel.SharpPanel.ActualHeight * Methods.Solution.SolutionState.Main.Zoom;
                int ResultX = (int)(x) - ((int)ScreenMaxX / 2);
                int ResultY = (int)(y) - ((int)ScreenMaxY / 2);

                if ((ResultX <= 0)) ResultX = 0;
                if ((ResultY <= 0)) ResultY = 0;

                Methods.Solution.SolutionState.Main.SetViewPositionX(ResultX);
                Methods.Solution.SolutionState.Main.SetViewPositionY(ResultY);
            }
            else
            {
                int ResultX = (int)(x);
                int ResultY = (int)(y);

                if ((ResultX <= 0)) ResultX = 0;
                if ((ResultY <= 0)) ResultY = 0;

                Methods.Solution.SolutionState.Main.SetViewPositionX(ResultX);
                Methods.Solution.SolutionState.Main.SetViewPositionY(ResultY);
            }
        }

        #endregion

        #endregion

        #region Specific Editor Functions

        public static void ChangeLevelID(object sender, RoutedEventArgs e)
        {
            string inputValue = GenerationsLib.WPF.TextPrompt2.ShowDialog("Change Level ID", "This is only temporary and will reset when you reload the scene.", Methods.Solution.CurrentSolution.LevelID.ToString());
            int.TryParse(inputValue.ToString(), out int output);
            Methods.Solution.CurrentSolution.LevelID = output;
            Instance.EditorStatusBar.LevelIdentifierLabel.Content = "Level ID: " + Methods.Solution.CurrentSolution.LevelID.ToString();
        }
        public static void GoToPosition(object sender, RoutedEventArgs e)
        {
            ManiacEditor.Controls.Toolbox.GoToPositionBox form = new ManiacEditor.Controls.Toolbox.GoToPositionBox(Instance);
            form.Owner = Instance as Window;
            form.ShowDialog();
        }
        public static void MD5GeneratorToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ManiacEditor.Controls.Misc.Dev.MD5HashGen hashmap = new ManiacEditor.Controls.Misc.Dev.MD5HashGen(Instance);
            hashmap.Show();
        }
        public static void OffsetTileIndexesTool()
        {
            ManiacEditor.Controls.Toolbox.OffsetTilesTool form = new ManiacEditor.Controls.Toolbox.OffsetTilesTool();
            form.ShowDialog();
            if (form.DialogResult == true)
            {
                Methods.Layers.TileFindReplace.OffsetEditLayerIndexes(form.IndexOffsetAmount);
            }
        }
        public static void FindAndReplaceTool(object sender, RoutedEventArgs e)
        {
            ManiacEditor.Controls.Toolbox.FindandReplaceTool form = new ManiacEditor.Controls.Toolbox.FindandReplaceTool();
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
                    Methods.Layers.TileFindReplace.EditorTileFindReplace(find, replace, applyState, copyResults);//, perserveColllision
                }
                else
                {
                    Methods.Layers.TileFindReplace.EditorTileFind(find, applyState, copyResults);
                }

            }

        }
        public static void ConsoleWindowToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ProgramLauncher.ManiacConsoleToggle();
        }
        public static void SaveForForceOpenOnStartupToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.MyDevSettings.DevForceRestartData = ManiacEditor.Methods.Solution.SolutionPaths.CurrentSceneData.MasterDataDirectory;
            Properties.Settings.MyDevSettings.DevForceRestartScene = ManiacEditor.Methods.Solution.SolutionPaths.CurrentSceneData.FilePath;
            Properties.Settings.MyDevSettings.DevForceRestartX = (short)(Methods.Solution.SolutionState.Main.ViewPositionX);
            Properties.Settings.MyDevSettings.DevForceRestartY = (short)(Methods.Solution.SolutionState.Main.ViewPositionY);
            Properties.Settings.MyDevSettings.DevForceRestartZoomLevel = Methods.Solution.SolutionState.Main.ZoomLevel;
            Properties.Settings.MyDevSettings.DevForceRestartIsEncore = ManiacEditor.Methods.Solution.SolutionPaths.CurrentSceneData.IsEncoreMode;
            Properties.Settings.MyDevSettings.DevForceRestartID = Methods.Solution.CurrentSolution.LevelID;
            Properties.Settings.MyDevSettings.DevForceRestartCurrentName = ManiacEditor.Methods.Solution.SolutionPaths.CurrentSceneData.Name;
            Properties.Settings.MyDevSettings.DevForceRestartCurrentZone = ManiacEditor.Methods.Solution.SolutionPaths.CurrentSceneData.Zone;
            Properties.Settings.MyDevSettings.DevForceRestartSceneID = ManiacEditor.Methods.Solution.SolutionPaths.CurrentSceneData.SceneID;
            Properties.Settings.MyDevSettings.DevForceRestartIsBrowsed = ManiacEditor.Methods.Solution.SolutionPaths.CurrentSceneData.IsFullPath;
            Properties.Settings.MyDevSettings.DevForceRestartResourcePacks = new System.Collections.Specialized.StringCollection();
            Properties.Settings.MyDevSettings.DevForceRestartResourcePacks.AddRange(ManiacEditor.Methods.Solution.SolutionPaths.CurrentSceneData.ExtraDataDirectories.ToArray());
        }
        public static void EnableAllButtonsToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            object[] MTB = Instance.EditorToolbar.MainToolbarButtons.Items.Cast<object>().ToArray();
            object[] LT = Instance.EditorToolbar.LayerToolbar.Items.Cast<object>().ToArray();
            ManiacEditor.Extensions.Extensions.EnableButtonList(MTB);
            ManiacEditor.Extensions.Extensions.EnableButtonList(LT);
        }
        public static void SwapEncoreManiaEntityVisibility()
        {
            if (Properties.Settings.MyDefaults.ShowEncoreEntities == true && Properties.Settings.MyDefaults.ShowManiaEntities == true)
            {
                Properties.Settings.MyDefaults.ShowManiaEntities = true;
                Properties.Settings.MyDefaults.ShowEncoreEntities = false;
            }
            if (Properties.Settings.MyDefaults.ShowEncoreEntities == true && Properties.Settings.MyDefaults.ShowManiaEntities == false)
            {
                Properties.Settings.MyDefaults.ShowManiaEntities = true;
                Properties.Settings.MyDefaults.ShowEncoreEntities = false;
            }
            else
            {
                Properties.Settings.MyDefaults.ShowManiaEntities = false;
                Properties.Settings.MyDefaults.ShowEncoreEntities = true;
            }

        }
        public static void SetScrollLockDirection()
        {
            if (Methods.Solution.SolutionState.Main.ScrollDirection == Axis.X)
            {
                Methods.Solution.SolutionState.Main.ScrollDirection = Axis.Y;
                Instance.EditorStatusBar.UpdateStatusPanel();
                Instance.MenuBar.xToolStripMenuItem.IsChecked = false;
                Instance.MenuBar.yToolStripMenuItem.IsChecked = true;
            }
            else
            {
                Methods.Solution.SolutionState.Main.ScrollDirection = Axis.X;
                Instance.EditorStatusBar.UpdateStatusPanel();
                Instance.MenuBar.xToolStripMenuItem.IsChecked = true;
                Instance.MenuBar.yToolStripMenuItem.IsChecked = false;
            }
        }
        public static void SetManiaMenuInputType(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.MenuItem menuItem = sender as System.Windows.Controls.MenuItem;
            if (menuItem != null)
            {
                if (menuItem.Tag != null)
                {
                    string tag = menuItem.Tag.ToString();
                    var allItems = Instance.MenuBar.menuButtonsToolStripMenuItem.Items.Cast<System.Windows.Controls.MenuItem>().ToArray();
                    foreach (System.Windows.Controls.MenuItem item in allItems)
                    {
                        if (item.Tag == null || item.Tag.ToString() != menuItem.Tag.ToString()) item.IsChecked = false;
                        else if (item.Tag.ToString() == menuItem.Tag.ToString()) item.IsChecked = true;
                        var allSubItems = Instance.MenuBar.menuButtonsToolStripMenuItem.Items.Cast<System.Windows.Controls.MenuItem>().ToArray();
                        foreach (System.Windows.Controls.MenuItem subItem in allSubItems)
                        {
                            if (subItem.Tag == null || subItem.Tag.ToString() != menuItem.Tag.ToString()) subItem.IsChecked = false;
                            else if (subItem.Tag.ToString() == menuItem.Tag.ToString()) subItem.IsChecked = true;
                        }
                    }
                    switch (tag)
                    {
                        case "Xbox":
                            Methods.Solution.SolutionState.Main.CurrentManiaUIControllerType = 2;
                            break;
                        case "Switch":
                            Methods.Solution.SolutionState.Main.CurrentManiaUIControllerType = 4;
                            break;
                        case "PS4":
                            Methods.Solution.SolutionState.Main.CurrentManiaUIControllerType = 3;
                            break;
                        case "Saturn Black":
                            Methods.Solution.SolutionState.Main.CurrentManiaUIControllerType = 5;
                            break;
                        case "Saturn White":
                            Methods.Solution.SolutionState.Main.CurrentManiaUIControllerType = 6;
                            break;
                        case "Switch Joy L":
                            Methods.Solution.SolutionState.Main.CurrentManiaUIControllerType = 7;
                            break;
                        case "Switch Joy R":
                            Methods.Solution.SolutionState.Main.CurrentManiaUIControllerType = 8;
                            break;
                        case "PC EN/JP":
                            Methods.Solution.SolutionState.Main.CurrentManiaUIControllerType = 1;
                            break;
                        case "PC FR":
                            Methods.Solution.SolutionState.Main.CurrentManiaUIControllerType = 9;
                            break;
                        case "PC IT":
                            Methods.Solution.SolutionState.Main.CurrentManiaUIControllerType = 10;
                            break;
                        case "PC GE":
                            Methods.Solution.SolutionState.Main.CurrentManiaUIControllerType = 11;
                            break;
                        case "PC SP":
                            Methods.Solution.SolutionState.Main.CurrentManiaUIControllerType = 12;
                            break;
                    }
                    menuItem.IsChecked = true;
                }

            }

        }
        public static void SetManiaMenuInputType(string tag)
        {
            switch (tag)
            {
                case "Xbox":
                    Methods.Solution.SolutionState.Main.CurrentManiaUIControllerType = 2;
                    break;
                case "Switch":
                    Methods.Solution.SolutionState.Main.CurrentManiaUIControllerType = 4;
                    break;
                case "PS4":
                    Methods.Solution.SolutionState.Main.CurrentManiaUIControllerType = 3;
                    break;
                case "Saturn Black":
                    Methods.Solution.SolutionState.Main.CurrentManiaUIControllerType = 5;
                    break;
                case "Saturn White":
                    Methods.Solution.SolutionState.Main.CurrentManiaUIControllerType = 6;
                    break;
                case "Switch Joy L":
                    Methods.Solution.SolutionState.Main.CurrentManiaUIControllerType = 7;
                    break;
                case "Switch Joy R":
                    Methods.Solution.SolutionState.Main.CurrentManiaUIControllerType = 8;
                    break;
                case "PC EN/JP":
                    Methods.Solution.SolutionState.Main.CurrentManiaUIControllerType = 1;
                    break;
                case "PC FR":
                    Methods.Solution.SolutionState.Main.CurrentManiaUIControllerType = 9;
                    break;
                case "PC IT":
                    Methods.Solution.SolutionState.Main.CurrentManiaUIControllerType = 10;
                    break;
                case "PC GE":
                    Methods.Solution.SolutionState.Main.CurrentManiaUIControllerType = 11;
                    break;
                case "PC SP":
                    Methods.Solution.SolutionState.Main.CurrentManiaUIControllerType = 12;
                    break;
            }
        }
        public static void SetEncorePallete(object sender = null, string path = "")
        {
            if (sender != null)
            {
                var clickedItem = sender as System.Windows.Controls.MenuItem;
                string StartDir = ManiacEditor.Methods.Solution.SolutionPaths.CurrentSceneData.MasterDataDirectory;
                try
                {
                    using (var fd = new System.Windows.Forms.OpenFileDialog())
                    {
                        fd.Filter = "Color Palette File|*.act";
                        fd.DefaultExt = ".act";
                        fd.Title = "Select an Encore Color Palette";
                        fd.InitialDirectory = Path.Combine(StartDir, "Palettes");
                        if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            ManiacEditor.Methods.Solution.SolutionPaths.EncorePalette = Methods.Solution.CurrentSolution.CurrentScene.GetEncorePalette("", "", "", "", -1, fd.FileName);
                            Methods.Solution.SolutionState.Main.EncoreSetupType = 0;
                            if (File.Exists(ManiacEditor.Methods.Solution.SolutionPaths.EncorePalette[0]))
                            {
                                Methods.Solution.SolutionState.Main.IsEncorePaletteLoaded = true;
                                Methods.Solution.SolutionState.Main.UseEncoreColors = true;
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
                ManiacEditor.Methods.Solution.SolutionPaths.EncorePalette = Methods.Solution.CurrentSolution.CurrentScene.GetEncorePalette("", "", "", "", -1, path);
                Methods.Solution.SolutionState.Main.EncoreSetupType = 0;
                if (File.Exists(ManiacEditor.Methods.Solution.SolutionPaths.EncorePalette[0]))
                {
                    Methods.Solution.SolutionState.Main.IsEncorePaletteLoaded = true;
                    Methods.Solution.SolutionState.Main.UseEncoreColors = true;
                }
                else
                {
                    System.Windows.MessageBox.Show("Unable to set Encore Colors. The Specified Path does not exist: " + Environment.NewLine + path);
                }
            }

        }
        private static bool LockEntityFilterTextChanged { get; set; } = false;
        public static void UpdateEntityFilterFromTextBox(object sender, TextChangedEventArgs e)
        {
            if (sender is System.Windows.Controls.TextBox && LockEntityFilterTextChanged == false && Methods.Solution.CurrentSolution.Entities != null)
            {
                LockEntityFilterTextChanged = true;
                System.Windows.Controls.TextBox theSender = sender as System.Windows.Controls.TextBox;
                Methods.Solution.SolutionState.Main.ObjectFilter = theSender.Text;
                Instance.MenuBar.EntityFilterTextbox.Text = Methods.Solution.SolutionState.Main.ObjectFilter;
                Classes.Scene.EditorEntities.ObjectRefreshNeeded = true;
                LockEntityFilterTextChanged = false;
            }
            else
            {
                LockEntityFilterTextChanged = true;
                Methods.Solution.SolutionState.Main.ObjectFilter = string.Empty;
                Instance.MenuBar.EntityFilterTextbox.Text = string.Empty;
                LockEntityFilterTextChanged = false;
            }


        }
        public static void ManiaMenuLanguageChanged(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.MenuItem menuItem = sender as System.Windows.Controls.MenuItem;
            Methods.Solution.SolutionState.Main.CurrentManiaUILanguage = menuItem.Tag.ToString();
            var allLangItems = Instance.MenuBar.menuLanguageToolStripMenuItem.Items.Cast<System.Windows.Controls.MenuItem>().ToArray();
            foreach (var item in allLangItems)
            {
                if (item.Tag.ToString() != menuItem.Tag.ToString()) item.IsChecked = false;
                else if (item.Tag.ToString() == menuItem.Tag.ToString()) item.IsChecked = true;
            }


        }

        #endregion
    }
}
