using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using RSDKv5;
using ManiacEditor.Actions;
using System.Windows.Controls;
using ManiacEditor.Controls.Editor;
using ManiacEditor.Controls.Editor.Toolbars;
using ManiacEditor.Controls.TileManiac;

namespace ManiacEditor.Methods.Internal
{
    public static class UserInterface
    {
        private static MainEditor Instance;
        public static void UpdateInstance(MainEditor _instance)
        {
            Instance = _instance;
            UpdateTooltips();
        }
        public static void SetSceneOnlyButtonsState(bool enabled, bool stageLoad = false)
        {
            Instance.EditorToolbar.SetSceneOnlyButtonsState(enabled, stageLoad);
            Instance.MenuBar.SetSceneOnlyButtonsState(enabled, stageLoad);
            Instance.EditorToolbar.UpdateGameRunningButton(enabled);

            SetEditButtonsState(enabled);
            UpdateTooltips();

            if (stageLoad)
            {
                Instance.ViewPanel.SharpPanel.ResizeGraphicsPanel();
            }

            Methods.Internal.Theming.UpdateButtonColors();

        }
        public static void SetParallaxAnimationOnlyButtonsState(bool enabled = true)
        {
            Instance.EditorToolbar.Open.IsEnabled = !enabled;
            Instance.EditorToolbar.ShowAnimations.IsEnabled = enabled || Methods.Editor.Solution.CurrentScene != null;
            Instance.EditorToolbar.animationsSplitButton_Dropdown.IsEnabled = enabled || Methods.Editor.Solution.CurrentScene != null;
            Instance.MenuBar.MenuBar.IsEnabled = !enabled;
            Instance.EditorStatusBar.StatusBar1.IsEnabled = !enabled;
            Instance.EditorTabControl.IsEnabled = !enabled;
            Instance.EditorToolbar.New.IsEnabled = !enabled;
            Instance.EditorToolbar.Open.IsEnabled = !enabled;

            if (enabled)
            {
                Instance.EditorToolbar.ShowFGHigh.IsEnabled = Methods.Editor.Solution.FGHigh != null;
                Instance.EditorToolbar.ShowFGLow.IsEnabled = Methods.Editor.Solution.FGLow != null;
                Instance.EditorToolbar.ShowFGHigher.IsEnabled = Methods.Editor.Solution.FGHigher != null;
                Instance.EditorToolbar.ShowFGLower.IsEnabled = Methods.Editor.Solution.FGLower != null;
                Instance.EditorToolbar.ShowEntities.IsEnabled = true;
                Instance.ViewPanel.SplitContainer.UpdateToolbars(false, false);
                SetEditButtonsState(false);
            }
            foreach (var elb in Instance.ExtraLayerEditViewButtons)
            {
                elb.Value.IsEnabled = !enabled;
            }
        }
        public static void SetSelectOnlyButtonsState(bool enabled = true)
        {
            Instance.MenuBar.SetPasteButtonsState(true);
            Instance.MenuBar.SetSelectOnlyButtonsState(enabled);
            enabled &= ManiacEditor.Methods.Editor.SolutionState.IsSelected();

            if (ManiacEditor.Methods.Editor.SolutionState.IsEntitiesEdit() && Instance.EntitiesToolbar != null)
            {
                Instance.EntitiesToolbar.SelectedEntities = Methods.Editor.Solution.Entities.SelectedEntities.Select(x => x.Entity).ToList();
            }
        }
        private static void SetEditButtonsState(bool enabled)
        {
            Instance.EditorToolbar.SetEditButtonsState(enabled);
            if (ManiacEditor.Methods.Editor.SolutionState.IsTilesEdit())
            {
                if (Instance.TilesToolbar == null)
                {
                    if (Methods.Editor.SolutionState.UseEncoreColors)
                        Instance.TilesToolbar = new ManiacEditor.Controls.Editor.Toolbars.TilesToolbar.TilesToolbar(Methods.Editor.Solution.CurrentTiles, ManiacEditor.Methods.Editor.SolutionPaths.StageTiles_Source.ToString(), ManiacEditor.Methods.Editor.SolutionPaths.EncorePalette[0], MainEditor.Instance);
                    else
                        Instance.TilesToolbar = new ManiacEditor.Controls.Editor.Toolbars.TilesToolbar.TilesToolbar(Methods.Editor.Solution.CurrentTiles, ManiacEditor.Methods.Editor.SolutionPaths.StageTiles_Source.ToString(), null, MainEditor.Instance);


                    Instance.TilesToolbar.TileDoubleClick = new Action<ushort>(x =>
                    {
                        Methods.Editor.EditorActions.EditorPlaceTile(new System.Drawing.Point((int)(Methods.Editor.SolutionState.ViewPositionX / Methods.Editor.SolutionState.Zoom) + Methods.Editor.EditorConstants.TILE_SIZE - 1, (int)(Methods.Editor.SolutionState.ViewPositionY / Methods.Editor.SolutionState.Zoom) + Methods.Editor.EditorConstants.TILE_SIZE - 1), x, Methods.Editor.Solution.EditLayerA);
                    });
                    Instance.TilesToolbar.TileOptionChanged = new Action<int, bool>((option, state) =>
                    {
                        Methods.Editor.Solution.EditLayerA?.SetPropertySelected(option + 12, state);
                        Methods.Editor.Solution.EditLayerB?.SetPropertySelected(option + 12, state);

                    });
                    Instance.ViewPanel.ToolBarPanelRight.Children.Clear();
                    Instance.ViewPanel.ToolBarPanelRight.Children.Add(Instance.TilesToolbar);
                    Instance.ViewPanel.SplitContainer.UpdateToolbars(true, true);
                    Instance.Editor_Resize(null, null);
                    Instance.Focus();
                }
                if (ManiacEditor.Methods.Editor.SolutionState.IsChunksEdit()) Instance.TilesToolbar.TabControl.TabIndex = 1;
                else Instance.TilesToolbar.TabControl.TabIndex = 0;
                UpdateTilesOptions();
                Instance.TilesToolbar.ShowShortcuts = Instance.EditorToolbar.DrawToolButton.IsChecked.Value;
            }
            else
            {
                if (Instance.TilesToolbar != null)
                {
                    Instance.TilesToolbar.Dispose();
                    Instance.TilesToolbar = null;
                    Instance.Editor_Resize(null, null);
                    
                    Instance.Focus();
                }
            }
            if (ManiacEditor.Methods.Editor.SolutionState.IsEntitiesEdit())
            {
                if (Instance.EntitiesToolbar == null)
                {
                    Instance.EntitiesToolbar = new ManiacEditor.Controls.Editor.Toolbars.EntitiesToolbar.EntitiesToolbar(Methods.Editor.Solution.CurrentScene.Objects, MainEditor.Instance)
                    {
                        SelectedEntity = new Action<int>(x =>
                        {
                            Methods.Editor.Solution.Entities.SelectSlot(x);
                            SetSelectOnlyButtonsState();
                        }),
                        AddAction = new Action<ManiacEditor.Actions.IAction>(x =>
                        {
                            Instance.UndoStack.Push(x);
                            Instance.RedoStack.Clear();
                            UpdateControls();
                        }),
                        Spawn = new Action<SceneObject>(x =>
                        {
                            Methods.Editor.Solution.Entities.Add(x, GetEntitySpawnPoint());
                            Instance.UndoStack.Push(Methods.Editor.Solution.Entities.LastAction);
                            Instance.RedoStack.Clear();
                            UpdateControls();
                        })
                    };
                    Instance.ViewPanel.ToolBarPanelRight.Children.Clear();
                    Instance.ViewPanel.ToolBarPanelRight.Children.Add(Instance.EntitiesToolbar);
                    Instance.ViewPanel.SplitContainer.UpdateToolbars(true, true);
                    Instance.Editor_Resize(null, null);
                }
                UpdateEntitiesToolbarList();
                Instance.EntitiesToolbar.SelectedEntities = Methods.Editor.Solution.Entities.SelectedEntities.Select(x => x.Entity).ToList();
            }
            else
            {
                if (Instance.EntitiesToolbar != null)
                {
                    Instance.EntitiesToolbar.Dispose();
                    Instance.EntitiesToolbar = null;
                }
                if (Methods.Editor.Solution.Entities != null && Methods.Editor.Solution.Entities.SelectedEntities != null)
                {
                    if (Methods.Editor.Solution.Entities.SelectedEntities.Count != 0 && Methods.Editor.Solution.Entities.TemporarySelection.Count != 0)
                    {
                        Methods.Editor.Solution.Entities.EndTempSelection();
                        Methods.Editor.Solution.Entities.Deselect();
                    }
                }


            }
            if (Instance.TilesToolbar == null && Instance.EntitiesToolbar == null && (Instance.ViewPanel.ToolBarPanelRight.Children.Count != 0))
            {
                Instance.ViewPanel.ToolBarPanelRight.Children.Clear();
                Instance.ViewPanel.SplitContainer.UpdateToolbars(true, false);
            }

            SetSelectOnlyButtonsState(enabled);

            Position GetEntitySpawnPoint()
            {
                if (Instance.EditorToolbar.DrawToolButton.IsChecked.Value)
                {
                    short x = (short)(Methods.Editor.SolutionState.LastX / Methods.Editor.SolutionState.Zoom);
                    short y = (short)(Methods.Editor.SolutionState.LastY / Methods.Editor.SolutionState.Zoom);
                    if (Methods.Editor.SolutionState.UseMagnetMode)
                    {
                        short alignedX = (short)(Methods.Editor.SolutionState.MagnetSize * (x / Methods.Editor.SolutionState.MagnetSize));
                        short alignedY = (short)(Methods.Editor.SolutionState.MagnetSize * (y / Methods.Editor.SolutionState.MagnetSize));
                        return new Position(alignedX, alignedY);
                    }
                    else
                    {
                        return new Position(x, y);
                    }

                }
                else
                {
                    return new Position((short)(Methods.Editor.SolutionState.ViewPositionX / Methods.Editor.SolutionState.Zoom), (short)(Methods.Editor.SolutionState.ViewPositionY / Methods.Editor.SolutionState.Zoom));
                }

            }
        }
        public static void ToggleEditorButtons(bool enabled, bool isParallaxAnimation = false)
        {
            Instance.MenuBar.MenuBar.IsEnabled = enabled;
            Instance.EditorToolbar.LayerToolbar.IsEnabled = enabled;
            Instance.EditorToolbar.MainToolbarButtons.IsEnabled = enabled;
            SetSceneOnlyButtonsState((enabled ? true : Methods.Editor.Solution.CurrentScene != null));
            Instance.EditorToolbar.LayerToolbar.IsEnabled = enabled;
            Instance.EditorStatusBar.StatusBar1.IsEnabled = enabled;
            Instance.EditorStatusBar.StatusBar2.IsEnabled = enabled;
            if (Instance.TilesToolbar != null) Instance.TilesToolbar.IsEnabled = enabled;
            if (Instance.EntitiesToolbar != null) Instance.EntitiesToolbar.IsEnabled = enabled;
            if (isParallaxAnimation)
            {
                Instance.EditorToolbar.LayerToolbar.IsEnabled = true;
                foreach (var pair in Instance.ExtraLayerEditViewButtons)
                {
                    pair.Key.IsEnabled = false;
                    pair.Value.IsEnabled = true;
                }
                Instance.EditorToolbar.EditFGHigh.IsEnabled = false;
                Instance.EditorToolbar.EditFGHigher.IsEnabled = false;
                Instance.EditorToolbar.EditFGLow.IsEnabled = false;
                Instance.EditorToolbar.EditFGLower.IsEnabled = false;

            }
            Instance.ViewPanel.InfoHUD.UpdatePopupSize();
        }
        public static void UpdateTilesOptions()
        {
            if (ManiacEditor.Methods.Editor.SolutionState.IsTilesEdit() && !ManiacEditor.Methods.Editor.SolutionState.IsChunksEdit())
            {
                if (Instance.TilesToolbar != null)
                {
                    List<ushort> values = Methods.Editor.Solution.EditLayerA?.GetSelectedValues();
                    List<ushort> valuesB = Methods.Editor.Solution.EditLayerB?.GetSelectedValues();
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
                            Instance.TilesToolbar.SetTileOptionState(i, unk ? ManiacEditor.Controls.Editor.Toolbars.TilesToolbar.TilesToolbar.TileOptionState.Indeterminate : set ? ManiacEditor.Controls.Editor.Toolbars.TilesToolbar.TilesToolbar.TileOptionState.Checked : ManiacEditor.Controls.Editor.Toolbars.TilesToolbar.TilesToolbar.TileOptionState.Unchcked);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < 4; ++i)
                            Instance.TilesToolbar.SetTileOptionState(i, ManiacEditor.Controls.Editor.Toolbars.TilesToolbar.TilesToolbar.TileOptionState.Disabled);
                    }
                }

            }
        }
        public static void UpdateEntitiesToolbarList()
        {
            if (Instance.EntitiesToolbar != null) Instance.EntitiesToolbar.Entities = Methods.Editor.Solution.Entities.Entities.Select(x => x.Entity).ToList();
        }
        public static void UpdateEditLayerActions()
        {
            if (Methods.Editor.Solution.EditLayerA != null)
            {
                List<IAction> actions = Methods.Editor.Solution.EditLayerA?.Actions;
                if (actions.Count > 0) Instance.RedoStack.Clear();
                while (actions.Count > 0)
                {
                    bool create_new = false;
                    if (Instance.UndoStack.Count == 0 || !(Instance.UndoStack.Peek() is ActionsGroup))
                    {
                        create_new = true;
                    }
                    else
                    {
                        create_new = (Instance.UndoStack.Peek() as ActionsGroup).IsClosed;
                    }
                    if (create_new)
                    {
                        Instance.UndoStack.Push(new ActionsGroup());
                    }
                    (Instance.UndoStack.Peek() as ActionsGroup).AddAction(actions[0]);
                    actions.RemoveAt(0);
                }
            }
            if (Methods.Editor.Solution.EditLayerB != null)
            {
                List<IAction> actions = Methods.Editor.Solution.EditLayerB?.Actions;
                if (actions.Count > 0) Instance.RedoStack.Clear();
                while (actions.Count > 0)
                {
                    bool create_new = false;
                    if (Instance.UndoStack.Count == 0 || !(Instance.UndoStack.Peek() is ActionsGroup))
                    {
                        create_new = true;
                    }
                    else
                    {
                        create_new = (Instance.UndoStack.Peek() as ActionsGroup).IsClosed;
                    }
                    if (create_new)
                    {
                        Instance.UndoStack.Push(new ActionsGroup());
                    }
                    (Instance.UndoStack.Peek() as ActionsGroup).AddAction(actions[0]);
                    actions.RemoveAt(0);
                }
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
                Instance.ViewPanel.SharpPanel.Visibility = Visibility.Visible;
                Instance.ViewPanel.WaitingPanel.Source.Visibility = Visibility.Collapsed;
            }

        }
        public static void UpdateSplineSpawnObjectsList(List<RSDKv5.SceneObject> sceneObjects)
        {
            Methods.Editor.SolutionState.AllowSplineOptionsUpdate = false;
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
                Methods.Editor.SolutionState.AllowSplineOptionsUpdate = true;

            }
        }
        public static void UpdateSplineSettings(int splineID)
        {
            if (!Methods.Editor.SolutionState.SplineOptionsGroup.ContainsKey(splineID)) Methods.Editor.SolutionState.SplineOptionsGroup.Add(splineID, new Methods.Editor.SolutionState.SplineOptions());
            Instance.EditorToolbar.SplineLineMode.IsChecked = Methods.Editor.SolutionState.SplineOptionsGroup[splineID].SplineLineMode;
            Instance.EditorToolbar.SplineOvalMode.IsChecked = Methods.Editor.SolutionState.SplineOptionsGroup[splineID].SplineOvalMode;
            Instance.EditorToolbar.SplineShowLineCheckbox.IsChecked = Methods.Editor.SolutionState.SplineOptionsGroup[splineID].SplineToolShowLines;
            Instance.EditorToolbar.SplineShowObjectsCheckbox.IsChecked = Methods.Editor.SolutionState.SplineOptionsGroup[splineID].SplineToolShowObject;
            Instance.EditorToolbar.SplineShowPointsCheckbox.IsChecked = Methods.Editor.SolutionState.SplineOptionsGroup[splineID].SplineToolShowPoints;
            Instance.EditorToolbar.SplinePointSeperationNUD.Value = Methods.Editor.SolutionState.SplineOptionsGroup[splineID].SplineSize;
            Instance.EditorToolbar.SplinePointSeperationSlider.Value = Methods.Editor.SolutionState.SplineOptionsGroup[splineID].SplineSize;

            if (Methods.Editor.SolutionState.SplineOptionsGroup[splineID].SplineObjectRenderingTemplate != null)
                Instance.EditorToolbar.SplineRenderObjectName.Content = Methods.Editor.SolutionState.SplineOptionsGroup[splineID].SplineObjectRenderingTemplate.Entity.Object.Name.Name;
            else
                Instance.EditorToolbar.SplineRenderObjectName.Content = "None";

        }
        public static void UpdateSplineToolbox()
        {
            if (Instance != null && Instance.EditorToolbar != null)
            {
                int splineID = Instance.EditorToolbar.SplineGroupID.Value.Value;
                //Instance.EditorToolbar.SplineInfoLabel1.Header = string.Format("Number of Spline Entities: {0}", Methods.Editor.SolutionState.SplineOptionsGroup[splineID].SplineSize);
                Instance.EditorToolbar.SplineInfoLabel1.Header = string.Empty;
                Instance.EditorToolbar.SplineInfoLabel2.Header = string.Format("TotalNumber of Spline Objects: {0}", Methods.Editor.SolutionState.SplineOptionsGroup[splineID].SplineTotalNumberOfObjects);
                Instance.EditorToolbar.SplineInfoLabel3.Header = string.Format("Total Number of Rendered Points: {0}", Methods.Editor.SolutionState.SplineOptionsGroup[splineID].SplineNumberOfObjectsRendered);

                if (Methods.Editor.SolutionState.SplineOptionsGroup[Methods.Editor.SolutionState.SelectedSplineID].SplineObjectRenderingTemplate != null && Methods.Editor.SolutionState.SplineOptionsGroup[Methods.Editor.SolutionState.SelectedSplineID].SplineTotalNumberOfObjects >= 2)
                {
                    Instance.EditorToolbar.RenderSelectedSpline.IsEnabled = true;
                }
                else
                {
                    Instance.EditorToolbar.RenderSelectedSpline.IsEnabled = false;
                }
            }
        }
        public static void UpdateCustomColors()
        {
            Instance.EditorToolbar.CSAC.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(Methods.Editor.SolutionState.CollisionSAColour.A, Methods.Editor.SolutionState.CollisionSAColour.R, Methods.Editor.SolutionState.CollisionSAColour.G, Methods.Editor.SolutionState.CollisionSAColour.B));
            Instance.EditorToolbar.SSTOC.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(Methods.Editor.SolutionState.CollisionTOColour.A, Methods.Editor.SolutionState.CollisionTOColour.R, Methods.Editor.SolutionState.CollisionTOColour.G, Methods.Editor.SolutionState.CollisionTOColour.B));
            Instance.EditorToolbar.CSLRDC.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(Methods.Editor.SolutionState.CollisionLRDColour.A, Methods.Editor.SolutionState.CollisionLRDColour.R, Methods.Editor.SolutionState.CollisionLRDColour.G, Methods.Editor.SolutionState.CollisionLRDColour.B));
            Instance.EditorToolbar.WLC.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(Methods.Editor.SolutionState.waterColor.A, Methods.Editor.SolutionState.waterColor.R, Methods.Editor.SolutionState.waterColor.G, Methods.Editor.SolutionState.waterColor.B));
            Instance.EditorToolbar.GDC.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(Methods.Editor.SolutionState.GridColor.A, Methods.Editor.SolutionState.GridColor.R, Methods.Editor.SolutionState.GridColor.G, Methods.Editor.SolutionState.GridColor.B));
        }
        public static void UpdateControls(bool stageLoad = false)
        {
            if (Instance != null)
            {
                if (Properties.Settings.MySettings.EntityFreeCam)
                {
                    Instance.ViewPanel.SharpPanel.vScrollBar1.IsEnabled = false;
                    Instance.ViewPanel.SharpPanel.hScrollBar1.IsEnabled = false;
                }
                else
                {
                    Instance.ViewPanel.SharpPanel.vScrollBar1.IsEnabled = true;
                    Instance.ViewPanel.SharpPanel.hScrollBar1.IsEnabled = true;
                }

                bool parallaxAnimationInProgress = Methods.Editor.SolutionState.AllowAnimations && Methods.Editor.SolutionState.ParallaxAnimationChecked;

                Instance.EditorToolbar.UpdateGameRunningButton(Methods.Editor.Solution.CurrentScene != null);
                Methods.Internal.Theming.UpdateThemeForItemsWaiting();
                Instance.EditorStatusBar.UpdateFilterButtonApperance(false);
                Instance.EditorStatusBar.UpdateStatusPanel();
                SetSceneOnlyButtonsState(Methods.Editor.Solution.CurrentScene != null && !parallaxAnimationInProgress, stageLoad);
                SetParallaxAnimationOnlyButtonsState(parallaxAnimationInProgress);
                UpdateSplineToolbox();
                Instance.EditorToolbar.CustomGridLabel.Text = string.Format(Instance.EditorToolbar.CustomGridLabel.Tag.ToString(), Properties.Settings.MyDefaults.CustomGridSizeValue);
                Instance.ViewPanel.InfoHUD.UpdatePopupSize();
            }
        }
        public static void UpdateTooltips()
        {
            UpdateTooltipForStacks(Instance.EditorToolbar.UndoButton, Instance.UndoStack);
            UpdateTooltipForStacks(Instance.EditorToolbar.RedoButton, Instance.RedoStack);
            UpdateTextBlockForStacks(Instance.MenuBar.UndoMenuItemInfo, Instance.UndoStack);
            UpdateTextBlockForStacks(Instance.MenuBar.RedoMenuItemInfo, Instance.RedoStack);
            if (Instance.IsVisible)
            {
                Instance.MenuBar.UpdateMenuItems();
                Instance.EditorStatusBar.UpdateTooltips();
                Instance.EditorToolbar.UpdateTooltips();
            }

        }
        public static void TileManiac_UpdateMenuItems()
        {
            if (CollisionEditor.Instance != null)
            {
                CollisionEditor.Instance.newInstanceMenuItem.InputGestureText = Extensions.KeyEventExts.KeyBindPraser("NewInstance");
                CollisionEditor.Instance.openMenuItem.InputGestureText = Extensions.KeyEventExts.KeyBindPraser("TileManiacOpen");
                CollisionEditor.Instance.saveMenuItem.InputGestureText = Extensions.KeyEventExts.KeyBindPraser("TileManiacSave");
                CollisionEditor.Instance.saveAsMenuItem.InputGestureText = Extensions.KeyEventExts.KeyBindPraser("TileManiacSaveAs");
                CollisionEditor.Instance.saveAsUncompressedMenuItem.InputGestureText = Extensions.KeyEventExts.KeyBindPraser("TileManiacSaveAsUncompressed");
                CollisionEditor.Instance.saveUncompressedMenuItem.InputGestureText = Extensions.KeyEventExts.KeyBindPraser("TileManiacSaveUncompressed");
                CollisionEditor.Instance.backupTilesConfigMenuItem.InputGestureText = Extensions.KeyEventExts.KeyBindPraser("TileManiacbackupConfig", false, true);
                CollisionEditor.Instance.backupTilesMenuItem.InputGestureText = Extensions.KeyEventExts.KeyBindPraser("TileManiacbackupImage", false, true);
                CollisionEditor.Instance.importMenuItem.InputGestureText = Extensions.KeyEventExts.KeyBindPraser("TileManiacImportFromOlderRSDK", false, true);
                CollisionEditor.Instance.OpenSingleColMaskMenuItem.InputGestureText = Extensions.KeyEventExts.KeyBindPraser("TileManiacOpenSingleColMask", false, true);
                CollisionEditor.Instance.exportCurrentMaskMenuItem.InputGestureText = Extensions.KeyEventExts.KeyBindPraser("TileManiacExportColMask", false, true);

                CollisionEditor.Instance.copyMenuItem.InputGestureText = Extensions.KeyEventExts.KeyBindPraser("TileManiacCopy");
                CollisionEditor.Instance.copyToOtherPathMenuItem.InputGestureText = Extensions.KeyEventExts.KeyBindPraser("TileManiacPastetoOther");
                CollisionEditor.Instance.pasteMenuItem.InputGestureText = Extensions.KeyEventExts.KeyBindPraser("TileManiacPaste");
                CollisionEditor.Instance.mirrorPathsToolStripMenuItem1.InputGestureText = Extensions.KeyEventExts.KeyBindPraser("TileManiacMirrorMode");
                CollisionEditor.Instance.restorePathAMenuItem.InputGestureText = Extensions.KeyEventExts.KeyBindPraser("TileManiacRestorePathA", false, true);
                CollisionEditor.Instance.restorePathBMenuItem.InputGestureText = Extensions.KeyEventExts.KeyBindPraser("TileManiacRestorePathB", false, true);
                CollisionEditor.Instance.restoreBothMenuItem.InputGestureText = Extensions.KeyEventExts.KeyBindPraser("TileManiacRestorePaths", false, true);

                CollisionEditor.Instance.showPathBToolStripMenuItem.InputGestureText = Extensions.KeyEventExts.KeyBindPraser("TileManiacShowPathB");
                CollisionEditor.Instance.showGridToolStripMenuItem.InputGestureText = Extensions.KeyEventExts.KeyBindPraser("TileManiacShowGrid");
                CollisionEditor.Instance.classicViewModeToolStripMenuItem.InputGestureText = Extensions.KeyEventExts.KeyBindPraser("TileManiacClassicMode", false, true);
                CollisionEditor.Instance.windowAlwaysOnTop.InputGestureText = Extensions.KeyEventExts.KeyBindPraser("TileManiacWindowAlwaysOnTop");


                CollisionEditor.Instance.splitFileMenuItem.InputGestureText = Extensions.KeyEventExts.KeyBindPraser("TileManiacSplitFile", false, true);
                CollisionEditor.Instance.flipTileHMenuItem.InputGestureText = Extensions.KeyEventExts.KeyBindPraser("TileManiacFlipTileH", false, true);
                CollisionEditor.Instance.flipTileVMenuItem.InputGestureText = Extensions.KeyEventExts.KeyBindPraser("TileManiacFlipTileV", false, true);

                CollisionEditor.Instance.openCollisionHomeFolderToolStripMenuItem.InputGestureText = Extensions.KeyEventExts.KeyBindPraser("TileManiacHomeFolderOpen", false, true);

                CollisionEditor.Instance.aboutMenuItem.InputGestureText = Extensions.KeyEventExts.KeyBindPraser("TileManiacAbout", false, true);
                CollisionEditor.Instance.settingsMenuItem.InputGestureText = Extensions.KeyEventExts.KeyBindPraser("TileManiacSettings", false, true);
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
        public static void ReloadSpritesAndTextures()
        {
            try
            {
                // release all our resources, and force a reload of the tiles
                // Entities should take care of themselves
                Instance.DisposeTextures();
                Instance.EntityDrawing.ReleaseResources();
                //EditorEntity_ini.rendersWithErrors.Clear();

                //Reload for Encore Palletes, otherwise reload the image normally
                if (Methods.Editor.SolutionState.UseEncoreColors == true)
                {
                    Methods.Editor.Solution.CurrentTiles?.Image.Reload(ManiacEditor.Methods.Editor.SolutionPaths.EncorePalette[0]);
                    Instance.TilesToolbar?.Reload(ManiacEditor.Methods.Editor.SolutionPaths.EncorePalette[0]);
                }
                else
                {
                    Methods.Editor.Solution.CurrentTiles?.Image.Reload();
                    Instance.TilesToolbar?.Reload();
                }

                if (Methods.Editor.Solution.TileConfig != null) Methods.Editor.Solution.TileConfig = new Tileconfig(ManiacEditor.Methods.Editor.SolutionPaths.TileConfig_Source.ToString());



            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }
    }
}
