using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using RSDKv5;
using ManiacEditor.Actions;
using System.Windows.Controls;
using ManiacEditor.Controls.Editor;
using ManiacEditor.Controls.Editor_Toolbars;
using ManiacEditor.Controls.TileManiac;
using GenerationsLib.WPF;

namespace ManiacEditor.Methods.Internal
{
    public static class UserInterface
    {
        #region Definitions
        private static MainEditor Instance;

        public enum UpdateType : int
        {
            MouseMoved = 0,
            MouseClick = 1,
            MouseHeld = 2,
            UndoRedoButtons = 3
        }

        private static bool RefreshInProgress { get; set; } = false;

        private static bool _LockUserInterface = false;
        public static bool LockUserInterface
        {
            get
            {
                return _LockUserInterface;
            }
            set
            {
                _LockUserInterface = value;
                SetLockUIState();
            }
        }
        public static bool ShowWaitScreen { get; set; } = false;

        public static void UpdateInstance(MainEditor _instance)
        {
            Instance = _instance;
        }
        private static bool IsSceneLoaded()
        {
            return Methods.Solution.CurrentSolution.CurrentScene != null;
        }
        private static bool IsStartScreenVisible()
        {
            return Instance.StartScreen.Visibility == Visibility.Visible;
        }
        #endregion

        #region Main UI Refresh
        public static void UpdateControls()
        {
            RefreshInProgress = true;
            if (Instance != null)
            {
                bool isSceneLoaded = IsSceneLoaded();
                SetGlobalControlsState(isSceneLoaded);
                SetSceneOnlyButtonsState(isSceneLoaded);
                SetEditButtonsState(isSceneLoaded);
                SetSelectOnlyButtonsState(isSceneLoaded);
                UpdateStatusState(isSceneLoaded);
            }
            RefreshInProgress = false;
        }
        public static void UpdateControls(UpdateType updateType)
        {
            RefreshInProgress = true;
            if (Instance != null)
            {
                bool isSceneLoaded = IsSceneLoaded();
                switch (updateType)
                {
                    case UpdateType.MouseMoved:
                        Instance.EditorStatusBar.UpdateStatusPanel();
                        break;
                    case UpdateType.MouseClick:
                        SetSceneOnlyButtonsState(isSceneLoaded);
                        SetEditButtonsState(isSceneLoaded);
                        SetSelectOnlyButtonsState(isSceneLoaded);
                        UpdateStatusState(isSceneLoaded);
                        break;
                    case UpdateType.MouseHeld:
                        SetSceneOnlyButtonsState(isSceneLoaded);
                        SetEditButtonsState(isSceneLoaded);
                        SetSelectOnlyButtonsState(isSceneLoaded);
                        UpdateStatusState(isSceneLoaded);
                        break;
                    case UpdateType.UndoRedoButtons:
                        Tooltips.UpdateTooltips();
                        Instance.MenuBar.UpdateUndoRedoButtons(isSceneLoaded);
                        Instance.EditorToolbar.UpdateUndoRedoButtons(isSceneLoaded);
                        Instance.ViewPanel.SharpPanel.GraphicPanel.Render();
                        break;
                    default:
                        UpdateControls();
                        break;
                }
            }
            RefreshInProgress = false;
        }
        public static void SetLockUIState()
        {
            Instance.MenuBar.MenuBar.IsEnabled = !LockUserInterface;
            Instance.EditorToolbar.LayerToolbar.IsEnabled = !LockUserInterface;
            Instance.EditorToolbar.MainToolbarButtons.IsEnabled = !LockUserInterface;
            Instance.EditorStatusBar.StatusBar1.IsEnabled = !LockUserInterface;
            Instance.EditorStatusBar.StatusBar2.IsEnabled = !LockUserInterface;
        }
        public static void SetGlobalControlsState(bool enabled)
        {
            if (LockUserInterface) SetLockUIState();

            Instance.ViewPanel.SharpPanel.UpdateGraphicsPanelControls();

            Misc.UpdateWaitingScreen(ShowWaitScreen);
            Misc.UpdateCameraUnlockControls();
            UpdateStylesState(enabled);

            Instance.ViewPanel.InfoHUD.UpdatePopupSize();
            Instance.EditorToolbar.UpdateGameRunningButton(IsSceneLoaded());
        }
        public static void SetSceneOnlyButtonsState(bool enabled)
        {
            SplineControls.UpdateSplineToolbox();
            Methods.Draw.ObjectDrawing.RequestEntityVisiblityRefresh(true);
            Instance.MenuBar.SetSceneOnlyButtonsState(enabled);
            Instance.EditorToolbar.SetSceneOnlyButtonsState(enabled);
            Instance.EditorStatusBar.SetSceneOnlyButtonsState(enabled);
            EditorToolbars.UpdateEditorToolbars();
        }
        public static void SetSelectOnlyButtonsState(bool enabled = true)
        {
            bool isSelected = ManiacEditor.Methods.Solution.SolutionState.Main.IsSelected();
            Instance.MenuBar.SetPasteButtonsState(enabled);
            Instance.MenuBar.SetSelectOnlyButtonsState(enabled && isSelected);
        }
        private static void SetEditButtonsState(bool enabled)
        {
            Instance.EditorToolbar.SetEditButtonsState(enabled);
            Instance.MenuBar.SetEditButtonsState(enabled);
        }
        public static void UpdateStatusState(bool enabled)
        {
            /*
            Instance.EditorToolbar.CSAC.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(Methods.Editor.SolutionState.Main.CollisionSAColour.A, Methods.Editor.SolutionState.Main.CollisionSAColour.R, Methods.Editor.SolutionState.Main.CollisionSAColour.G, Methods.Editor.SolutionState.Main.CollisionSAColour.B));
            Instance.EditorToolbar.SSTOC.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(Methods.Editor.SolutionState.Main.CollisionTOColour.A, Methods.Editor.SolutionState.Main.CollisionTOColour.R, Methods.Editor.SolutionState.Main.CollisionTOColour.G, Methods.Editor.SolutionState.Main.CollisionTOColour.B));
            Instance.EditorToolbar.CSLRDC.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(Methods.Editor.SolutionState.Main.CollisionLRDColour.A, Methods.Editor.SolutionState.Main.CollisionLRDColour.R, Methods.Editor.SolutionState.Main.CollisionLRDColour.G, Methods.Editor.SolutionState.Main.CollisionLRDColour.B));
            Instance.EditorToolbar.WLC.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(Methods.Editor.SolutionState.Main.waterColor.A, Methods.Editor.SolutionState.Main.waterColor.R, Methods.Editor.SolutionState.Main.waterColor.G, Methods.Editor.SolutionState.Main.waterColor.B));
            Instance.EditorToolbar.GDC.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(Methods.Editor.SolutionState.Main.GridColor.A, Methods.Editor.SolutionState.Main.GridColor.R, Methods.Editor.SolutionState.Main.GridColor.G, Methods.Editor.SolutionState.Main.GridColor.B));*/

            Instance.EditorStatusBar.UpdateStatusPanel();
            Instance.EditorToolbar.CustomGridSizeLabel.Text = string.Format(Instance.EditorToolbar.CustomGridSizeLabel.Tag.ToString(), Properties.Settings.MyDefaults.CustomGridSizeValue);
        }
        public static void UpdateStylesState(bool enabled)
        {
            Methods.Internal.Theming.UpdateThemeForItemsWaiting();
            Instance.EditorStatusBar.UpdateFilterButtonApperance();
            Methods.Internal.Theming.UpdateButtonColors();
            Tooltips.UpdateTooltips();
        }
        #endregion

        #region Misc

        public static void TileManiac_UpdateMenuItems()
        {
            if (CollisionEditor.Instance != null)
            {
                CollisionEditor.Instance.InvokeIfRequired(() =>
                {
                    CollisionEditor.Instance.newInstanceMenuItem.InputGestureText = "Ctrl + N";
                    CollisionEditor.Instance.openMenuItem.InputGestureText = "Ctrl + O";
                    CollisionEditor.Instance.saveMenuItem.InputGestureText = "Ctrl + S";
                    CollisionEditor.Instance.saveAsMenuItem.InputGestureText = "Ctrl + Alt + S";

                    CollisionEditor.Instance.copyMenuItem.InputGestureText = "Ctrl + C";
                    CollisionEditor.Instance.copyToOtherPathMenuItem.InputGestureText = "Ctrl + Alt + V";
                    CollisionEditor.Instance.pasteMenuItem.InputGestureText = "Ctrl + V";
                    CollisionEditor.Instance.mirrorPathsToolStripMenuItem1.InputGestureText = "";

                    CollisionEditor.Instance.showPathBToolStripMenuItem.InputGestureText = "Ctrl + B";
                    CollisionEditor.Instance.showGridToolStripMenuItem.InputGestureText = "Ctrl + G";

                }, System.Windows.Threading.DispatcherPriority.Background);

            }
        }
        public static void ReloadSpritesAndTextures()
        {
            try
            {
                // release all our resources, and force a reload of the tiles
                // Entities should take care of themselves
                Methods.Draw.ObjectDrawing.ReleaseResources();
                Methods.Draw.ObjectDrawing.RefreshRenderLists();

                //Reload for Encore Palletes, otherwise reload the image normally
                if (Methods.Solution.SolutionState.Main.UseEncoreColors == true) Methods.Solution.CurrentSolution.CurrentTiles?.Reload(ManiacEditor.Methods.Solution.SolutionPaths.EncorePalette[0]);
                else Methods.Solution.CurrentSolution.CurrentTiles?.Reload();

                Instance.Chunks?.Dispose();
                if (Methods.Solution.CurrentSolution.TileConfig != null) Methods.Solution.CurrentSolution.TileConfig = new Tileconfig(ManiacEditor.Methods.Solution.SolutionPaths.TileConfig_Source.ToString());
                Methods.Solution.CurrentSolution.CurrentScene?.Reload();
                Instance.TilesToolbar?.Reload();

            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        #endregion

        #region Subsections
        public static class Status
        {
            public static void UpdateDataFolderLabel(string dataDirectory = null)
            {
                string dataFolderTag_Normal = "Data Directory: {0}";
                Instance.EditorStatusBar._baseDataDirectoryLabel.Tag = dataFolderTag_Normal;

                if (dataDirectory != null) Instance.EditorStatusBar._baseDataDirectoryLabel.Content = string.Format(Instance.EditorStatusBar._baseDataDirectoryLabel.Tag.ToString(), dataDirectory);
                else Instance.EditorStatusBar._baseDataDirectoryLabel.Content = string.Format(Instance.EditorStatusBar._baseDataDirectoryLabel.Tag.ToString(), ManiacEditor.Methods.Solution.SolutionPaths.CurrentSceneData.DataDirectory);
            }
        }
        public static class Misc
        {
            public static void UpdateStartScreen(bool visible, bool firstLoad = false)
            {
                if (firstLoad)
                {
                    if (Instance.StartScreen.SelectScreen != null) Instance.StartScreen.SelectScreen.UpdateRecentsTree();
                    Instance.EditorTabControl.SelectedIndex = 1;
                    Instance.MainPageTab.Visibility = Visibility.Collapsed;
                    Instance.StartPageTab.Visibility = Visibility.Visible;
                    Instance.ViewPanel.SplitContainer.UpdateToolbars(false, false);
                    Classes.Prefrences.RecentsRefrenceState.RefreshRecentScenes();
                    Classes.Prefrences.RecentsRefrenceState.RefreshDataSources();
                }
                if (visible)
                {
                    if (Instance.StartScreen.SelectScreen != null) Instance.StartScreen.SelectScreen.UpdateRecentsTree();
                    Instance.EditorTabControl.SelectedIndex = 1;
                    Instance.MainPageTab.Visibility = Visibility.Collapsed;
                    Instance.StartPageTab.Visibility = Visibility.Visible;
                    Instance.ViewPanel.SplitContainer.UpdateToolbars(false, false);
                    Classes.Prefrences.RecentsRefrenceState.RefreshRecentScenes();
                    Classes.Prefrences.RecentsRefrenceState.RefreshDataSources();
                }
                else
                {
                    if (Instance.StartScreen.SelectScreen != null) Instance.StartScreen.SelectScreen.UpdateRecentsTree();
                    Instance.EditorTabControl.SelectedIndex = 0;
                    Instance.MainPageTab.Visibility = Visibility.Visible;
                    Instance.StartPageTab.Visibility = Visibility.Collapsed;
                    Instance.ViewPanel.SplitContainer.UpdateToolbars(false, false);
                }

            }

            public static void UpdateWaitingScreen(bool show)
            {
                if (show)
                {
                    Instance.ViewPanel.SharpPanel.Visibility = Visibility.Hidden;
                    Instance.ViewPanel.WaitingPanel.Source.Visibility = Visibility.Visible;
                }
                else
                {
                    if (!IsStartScreenVisible()) Instance.ViewPanel.SharpPanel.Visibility = Visibility.Visible;
                    Instance.ViewPanel.WaitingPanel.Source.Visibility = Visibility.Collapsed;
                }
            }
            public static void UpdateCameraUnlockControls()
            {
                if (Methods.Solution.SolutionState.Main.UnlockCamera)
                {
                    Instance.ViewPanel.SharpPanel.vScrollBar1.IsEnabled = false;
                    Instance.ViewPanel.SharpPanel.hScrollBar1.IsEnabled = false;
                }
                else
                {
                    Instance.ViewPanel.SharpPanel.vScrollBar1.IsEnabled = true;
                    Instance.ViewPanel.SharpPanel.hScrollBar1.IsEnabled = true;
                }
            }
        }
        public static class Tooltips
        {
            public static void UpdateTooltips()
            {
                UpdateTooltipForStacks(Instance.EditorToolbar.UndoButton, Actions.UndoRedoModel.UndoStack);
                UpdateTooltipForStacks(Instance.EditorToolbar.RedoButton, Actions.UndoRedoModel.RedoStack);
                UpdateTextBlockForStacks(Instance.MenuBar.UndoMenuItemInfo, Actions.UndoRedoModel.UndoStack);
                UpdateTextBlockForStacks(Instance.MenuBar.RedoMenuItemInfo, Actions.UndoRedoModel.RedoStack);
                if (Instance.IsVisible)
                {
                    Instance.MenuBar.UpdateMenuItems();
                    Instance.EditorStatusBar.UpdateTooltips();
                    Instance.EditorToolbar.UpdateTooltips();
                }

            }

            private static void UpdateTextBlockForStacks(TextBlock tsb, Stack<IAction> actionStack)
            {
                if (actionStack?.Count > 0)
                {
                    IAction action = actionStack.Peek();
                    if (action != null)
                    {
                        tsb.Visibility = Visibility.Visible;
                        tsb.Text = string.Format("({0})", action.Description);
                    }
                    else
                    {
                        tsb.Visibility = Visibility.Collapsed;
                        tsb.Text = string.Empty;
                    }
                }
                else
                {
                    tsb.Visibility = Visibility.Collapsed;
                    tsb.Text = string.Empty;
                }
            }
            private static void UpdateTooltipForStacks(Button tsb, Stack<IAction> actionStack)
            {
                if (actionStack?.Count > 0)
                {
                    IAction action = actionStack.Peek();
                    if (action != null)
                    {
                        System.Windows.Controls.ToolTip tooltip = new System.Windows.Controls.ToolTip { Content = string.Format(tsb.Tag.ToString(), action.Description + " ") };
                        tsb.ToolTip = tooltip;
                    }
                    else
                    {
                        System.Windows.Controls.ToolTip tooltip = new System.Windows.Controls.ToolTip { Content = string.Format(tsb.Tag.ToString(), string.Empty) };
                        tsb.ToolTip = tooltip;
                    }

                }
                else
                {
                    System.Windows.Controls.ToolTip tooltip = new System.Windows.Controls.ToolTip { Content = string.Format(tsb.Tag.ToString(), string.Empty) };
                    tsb.ToolTip = tooltip;
                }
            }
        }
        public static class SplineControls
        {
            public static void UpdateSplineSpawnObjectsList(List<RSDKv5.SceneObject> sceneObjects)
            {
                Instance.Dispatcher.BeginInvoke(new Action(() =>
                {
                    Methods.Solution.SolutionState.Main.AllowSplineOptionsUpdate = false;
                    sceneObjects.Sort((x, y) => x.Name.ToString().CompareTo(y.Name.ToString()));
                    var bindingSceneObjectsList = new System.ComponentModel.BindingList<RSDKv5.SceneObject>(sceneObjects);


                    Instance.SplineSelectedObjectSpawnList.Clear();
                    foreach (var _object in bindingSceneObjectsList)
                    {
                        TextBlock item = new TextBlock()
                        {
                            Tag = _object,
                            Text = _object.Name.Name
                        };
                        Instance.SplineSelectedObjectSpawnList.Add(item);
                    }

                    if (Instance.SplineSelectedObjectSpawnList != null && Instance.SplineSelectedObjectSpawnList.Count > 1)
                    {
                        Instance.EditorToolbar.SelectedSplineRender.ItemsSource = Instance.SplineSelectedObjectSpawnList;
                        Instance.EditorToolbar.SelectedSplineRender.SelectedItem = Instance.EditorToolbar.SelectedSplineRender.Items[0];
                        var SelectedItem = Instance.EditorToolbar.SelectedSplineRender.SelectedItem as TextBlock;
                        if (SelectedItem == null) return;
                        SelectedItem.Foreground = (System.Windows.Media.SolidColorBrush)Instance.FindResource("NormalText");
                        Methods.Solution.SolutionState.Main.AllowSplineOptionsUpdate = true;

                    }
                }));

            }
            public static void UpdateSplineSettings(int splineID)
            {
                Instance.Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (!Methods.Solution.SolutionState.Main.SplineOptionsGroup.ContainsKey(splineID)) Methods.Solution.SolutionState.Main.SplineOptionsGroup.Add(splineID, new Methods.Solution.SolutionState.StateModel.SplineOptions(Methods.Solution.SolutionState.Main));
                    Instance.EditorToolbar.SplineLineMode.IsChecked = Methods.Solution.SolutionState.Main.SplineOptionsGroup[splineID].SplineLineMode;
                    Instance.EditorToolbar.SplineOvalMode.IsChecked = Methods.Solution.SolutionState.Main.SplineOptionsGroup[splineID].SplineOvalMode;
                    Instance.EditorToolbar.SplineShowLineCheckbox.IsChecked = Methods.Solution.SolutionState.Main.SplineOptionsGroup[splineID].SplineToolShowLines;
                    Instance.EditorToolbar.SplineShowObjectsCheckbox.IsChecked = Methods.Solution.SolutionState.Main.SplineOptionsGroup[splineID].SplineToolShowObject;
                    Instance.EditorToolbar.SplineShowPointsCheckbox.IsChecked = Methods.Solution.SolutionState.Main.SplineOptionsGroup[splineID].SplineToolShowPoints;
                    Instance.EditorToolbar.SplinePointSeperationNUD.Value = Methods.Solution.SolutionState.Main.SplineOptionsGroup[splineID].SplineSize;
                    Instance.EditorToolbar.SplinePointSeperationSlider.Value = Methods.Solution.SolutionState.Main.SplineOptionsGroup[splineID].SplineSize;

                    if (Methods.Solution.SolutionState.Main.SplineOptionsGroup[splineID].SplineObjectRenderingTemplate != null)
                        Instance.EditorToolbar.SplineRenderObjectName.Content = Methods.Solution.SolutionState.Main.SplineOptionsGroup[splineID].SplineObjectRenderingTemplate.Object.Name.Name;
                    else
                        Instance.EditorToolbar.SplineRenderObjectName.Content = "None";
                }));


            }
            public static void UpdateSplineToolbox()
            {
                Instance.Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (Instance != null && Instance.EditorToolbar != null)
                    {
                        int splineID = Instance.EditorToolbar.SplineGroupID.Value.Value;
                        Instance.EditorToolbar.SplineInfoLabel1.Header = string.Empty;


                        if (Methods.Solution.SolutionState.Main.SplineOptionsGroup.Count - 1 <= splineID)
                        {
                            Instance.EditorToolbar.SplineInfoLabel2.Header = string.Format("Total Number of Spline Objects: {0}", 0);
                            Instance.EditorToolbar.SplineInfoLabel3.Header = string.Format("Total Number of Rendered Points: {0}", 0);
                            Instance.EditorToolbar.RenderSelectedSpline.IsEnabled = false;
                        }
                        else
                        {
                            Instance.EditorToolbar.SplineInfoLabel2.Header = string.Format("Total Number of Spline Objects: {0}", Methods.Solution.SolutionState.Main.SplineOptionsGroup[splineID].SplineTotalNumberOfObjects);
                            Instance.EditorToolbar.SplineInfoLabel3.Header = string.Format("Total Number of Rendered Points: {0}", Methods.Solution.SolutionState.Main.SplineOptionsGroup[splineID].SplineNumberOfObjectsRendered);

                            if (Methods.Solution.SolutionState.Main.SplineOptionsGroup[Methods.Solution.SolutionState.Main.SelectedSplineID].SplineObjectRenderingTemplate != null && Methods.Solution.SolutionState.Main.SplineOptionsGroup[Methods.Solution.SolutionState.Main.SelectedSplineID].SplineTotalNumberOfObjects >= 2)
                            {
                                Instance.EditorToolbar.RenderSelectedSpline.IsEnabled = true;
                            }
                            else
                            {
                                Instance.EditorToolbar.RenderSelectedSpline.IsEnabled = false;
                            }
                        }

                    }
                }));
            }
        }
        public static class EditorToolbars
        {

            #region Init (Tiles)

            private static void DisposeTilesToolbar()
            {
                /*
                Instance.TilesToolbar.Dispose();
                Instance.TilesToolbar = null;
                Instance.Focus();
                */
            }
            private static void CreateTilesToolbar()
            {
                bool isEncore = Methods.Solution.SolutionState.Main.UseEncoreColors;
                var editorTiles = Methods.Solution.CurrentSolution.CurrentTiles;
                string tileSource = ManiacEditor.Methods.Solution.SolutionPaths.StageTiles_Source.ToString();
                string palette = (isEncore ? ManiacEditor.Methods.Solution.SolutionPaths.EncorePalette[0] : null);

                Instance.TilesToolbar = new ManiacEditor.Controls.Editor_Toolbars.TilesToolbar(Instance);
                Instance.TilesToolbar.TileDoubleClick = new Action<ushort>(x => { TilesToolbar_TileDoubleClick(x); });
                Instance.TilesToolbar.MultiTileDoubleClick = new Action<Tuple<List<ushort>, int[]>>(x => { TilesToolbar_MultiTileDoubleClick(x); });
                Instance.TilesToolbar.TileOptionChanged = new Action<int, bool>((option, state) => { TilesToolbar_TileOptionChanged(option, state); });

                AttachTilesToolbar();

                Instance.Focus();
            }

            private static void AttachTilesToolbar()
            {
                if (!Instance.ViewPanel.ToolBarPanelRight.Children.Contains(Instance.TilesToolbar))
                {
                    Instance.ViewPanel.ToolBarPanelRight.Children.Clear();
                    Instance.ViewPanel.ToolBarPanelRight.Children.Add(Instance.TilesToolbar);
                    Instance.ViewPanel.SplitContainer.UpdateToolbars(true, true);
                }
            }

            private static void TilesToolbar_TileDoubleClick(ushort tile)
            {
                Methods.Solution.CurrentSolution.EditLayerA.PlaceTile(new System.Drawing.Point((int)(Methods.Solution.SolutionState.Main.ViewPositionX) + Methods.Solution.SolutionConstants.TILE_SIZE - 1, (int)(Methods.Solution.SolutionState.Main.ViewPositionY) + Methods.Solution.SolutionConstants.TILE_SIZE - 1), tile);
            }
            private static void TilesToolbar_MultiTileDoubleClick(Tuple<List<ushort>, int[]> tiles)
            {
                Methods.Solution.CurrentSolution.EditLayerA.PlaceTiles(new System.Drawing.Point((int)(Methods.Solution.SolutionState.Main.ViewPositionX) + Methods.Solution.SolutionConstants.TILE_SIZE - 1, (int)(Methods.Solution.SolutionState.Main.ViewPositionY) + Methods.Solution.SolutionConstants.TILE_SIZE - 1), tiles.Item1, tiles.Item2[0], tiles.Item2[1]);

            }
            private static void TilesToolbar_TileOptionChanged(int option, bool state)
            {
                Methods.Solution.SolutionMultiLayer.SetPropertySelected(option + 10, state);
            }
            private static void UpdateTilesOptions()
            {
                List<ushort> values = Methods.Solution.SolutionMultiLayer.GetSelectedValues();

                if (values.Count > 0)
                {
                    for (int i = 0; i < 6; ++i)
                    {
                        bool set = ((values[0] & (1 << (i + 10))) != 0);
                        bool unk = false;
                        foreach (ushort value in values)
                        {
                            if (set != ((value & (1 << (i + 10))) != 0))
                            {
                                unk = true;
                                break;
                            }
                        }
                        Instance.TilesToolbar.SetTileOptionState(i, unk ? ManiacEditor.Controls.Editor_Toolbars.TilesToolbar.TileOptionState.Indeterminate : set ? ManiacEditor.Controls.Editor_Toolbars.TilesToolbar.TileOptionState.Checked : ManiacEditor.Controls.Editor_Toolbars.TilesToolbar.TileOptionState.Unchcked);
                    }
                }
                else
                {
                    for (int i = 0; i < 6; ++i)
                        Instance.TilesToolbar.SetTileOptionState(i, ManiacEditor.Controls.Editor_Toolbars.TilesToolbar.TileOptionState.Disabled);
                }
            }

            private static void RefreshTilesToolbar()
            {
                if (Instance.TilesToolbar != null)
                {
                    Instance.TilesToolbar.UpdateChunksListIfNeeded();
                    Instance.TilesToolbar.UpdateModeSpecifics();
                    UpdateTilesOptions();
                    Instance.TilesToolbar.ShowShortcuts = Methods.Solution.SolutionState.Main.IsDrawMode();
                }
            }

            #endregion

            #region Init (Entities)

            private static void DisposeEntitiesToolbar()
            {
                /*
                Instance.EntitiesToolbar.Dispose();
                Instance.EntitiesToolbar = null;
                */
            }
            private static void CreateEntitiesToolbar()
            {
                Instance.EntitiesToolbar = new ManiacEditor.Controls.Editor_Toolbars.EntitiesToolbar(Methods.Solution.CurrentSolution.CurrentScene.Entities.SceneObjects, Instance)
                {
                    SelectedEntity = new Action<int>(x => { EntitiesToolbar_EntitySelected(x); }),
                    AddAction = new Action<ManiacEditor.Actions.IAction>(x => { EntitiesToolbar_ActionAdded(x); }),
                    Spawn = new Action<SceneObject>(x => { EntitiesToolbar_ObjectSpawned(x); })
                };
                AttachEntitiesToolbar();
            }


            private static void AttachEntitiesToolbar()
            {
                if (!Instance.ViewPanel.ToolBarPanelRight.Children.Contains(Instance.EntitiesToolbar))
                {
                    Instance.ViewPanel.ToolBarPanelRight.Children.Clear();
                    Instance.ViewPanel.ToolBarPanelRight.Children.Add(Instance.EntitiesToolbar);
                    Instance.ViewPanel.SplitContainer.UpdateToolbars(true, true);
                }
            }

            private static void EntitiesToolbar_ObjectSpawned(SceneObject sceneObject)
            {
                Methods.Solution.CurrentSolution.Entities.Spawn(sceneObject, GetEntitySpawnPoint());
                Actions.UndoRedoModel.UndoStack.Push(Methods.Solution.CurrentSolution.Entities.LastAction);
                Actions.UndoRedoModel.RedoStack.Clear();
                UpdateControls();

                Position GetEntitySpawnPoint()
                {
                    if (Methods.Solution.SolutionState.Main.IsDrawMode())
                    {
                        short x = (short)(Methods.Solution.SolutionState.Main.LastX);
                        short y = (short)(Methods.Solution.SolutionState.Main.LastY);
                        if (Methods.Solution.SolutionState.Main.UseMagnetMode)
                        {
                            short alignedX = (short)(Methods.Solution.SolutionState.Main.MagnetSize * (x / Methods.Solution.SolutionState.Main.MagnetSize));
                            short alignedY = (short)(Methods.Solution.SolutionState.Main.MagnetSize * (y / Methods.Solution.SolutionState.Main.MagnetSize));
                            return new Position(alignedX, alignedY);
                        }
                        else
                        {
                            return new Position(x, y);
                        }

                    }
                    else
                    {
                        return new Position((short)(Methods.Solution.SolutionState.Main.ViewPositionX), (short)(Methods.Solution.SolutionState.Main.ViewPositionY));
                    }

                }
            }
            private static void EntitiesToolbar_ActionAdded(ManiacEditor.Actions.IAction action)
            {
                Actions.UndoRedoModel.UndoStack.Push(action);
                Actions.UndoRedoModel.RedoStack.Clear();
                UpdateControls();
            }
            private static void EntitiesToolbar_EntitySelected(int x)
            {
                Methods.Solution.CurrentSolution.Entities.SelectSlot(x);
                UpdateControls();
            }

            public static void ChangeSplineSelectedID(int value)
            {
                Instance.EditorToolbar.SelectedSplineIDChangedEvent(value);
            }

            private static void RefreshEntitiesToolbar()
            {
                if (Instance.EntitiesToolbar != null)
                {
                    Instance.EntitiesToolbar.SelectedEntities = Methods.Solution.CurrentSolution.Entities.SelectedEntities;
                }
            }

            #endregion

            public static void ValidateEditorToolbars()
            {
                UpdateEditorToolbars();
                /*
                bool missingToolbar1 = ManiacEditor.Methods.Solution.SolutionState.Main.IsTilesEdit() && Instance.TilesToolbar == null;
                bool missingToolbar2 = ManiacEditor.Methods.Solution.SolutionState.Main.IsEntitiesEdit() && Instance.EntitiesToolbar == null;
                bool misplacedToolbar1 = !ManiacEditor.Methods.Solution.SolutionState.Main.IsTilesEdit() && Instance.TilesToolbar != null;
                bool misplacedToolbar2 = !ManiacEditor.Methods.Solution.SolutionState.Main.IsEntitiesEdit() && Instance.EntitiesToolbar != null;

                if (missingToolbar1 || missingToolbar2 || misplacedToolbar1 || misplacedToolbar2) UpdateEditorToolbars();
                */
            }

            public static void UpdateEditorToolbars()
            {
                if (ManiacEditor.Methods.Solution.SolutionState.Main.IsTilesEdit())
                {
                    if (Instance.TilesToolbar == null) CreateTilesToolbar();
                    AttachTilesToolbar();
                    RefreshTilesToolbar();
                }
                else if (Instance.TilesToolbar != null) DisposeTilesToolbar();

                if (ManiacEditor.Methods.Solution.SolutionState.Main.IsEntitiesEdit())
                {
                    if (Instance.EntitiesToolbar == null) CreateEntitiesToolbar();
                    AttachEntitiesToolbar();
                    RefreshEntitiesToolbar();
                }
                else
                {
                    if (Instance.EntitiesToolbar != null) DisposeEntitiesToolbar();
                    if (Methods.Solution.CurrentSolution.Entities != null && Methods.Solution.CurrentSolution.Entities.SelectedEntities != null)
                    {
                        if (Methods.Solution.CurrentSolution.Entities.SelectedEntities.Count != 0 && Methods.Solution.CurrentSolution.Entities.TemporarySelection.Count != 0)
                        {
                            Methods.Solution.CurrentSolution.Entities.EndTempSelection();
                            Methods.Solution.CurrentSolution.Entities.ClearSelection();
                        }
                    }


                }
                if (!ManiacEditor.Methods.Solution.SolutionState.Main.IsTilesEdit() && !ManiacEditor.Methods.Solution.SolutionState.Main.IsEntitiesEdit() && (Instance.ViewPanel.ToolBarPanelRight.Children.Count != 0))
                {
                    Instance.ViewPanel.ToolBarPanelRight.Children.Clear();
                    Instance.ViewPanel.SplitContainer.UpdateToolbars(true, false);
                }


            }
        }
        #endregion
    }


}
