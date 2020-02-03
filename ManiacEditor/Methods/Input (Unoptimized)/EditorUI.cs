using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using RSDKv5;
using ManiacEditor.Actions;
using System.Windows.Controls;

namespace ManiacEditor
{
    public class EditorUI
    {
        public EditorUI()
        {

        }



        #region Enable And Disable Editor Buttons
        public void SetSceneOnlyButtonsState(bool enabled, bool stageLoad = false)
        {
            Controls.Base.MainEditor.Instance.EditorToolbar.SetSceneOnlyButtonsState(enabled, stageLoad);
            Controls.Base.MainEditor.Instance.EditorMenuBar.SetSceneOnlyButtonsState(enabled, stageLoad);
            Controls.Base.MainEditor.Instance.EditorToolbar.UpdateGameRunningButton(enabled);

            SetEditButtonsState(enabled);
            UpdateTooltips();

            if (stageLoad)
            {
                Controls.Base.MainEditor.Instance.ZoomModel.SetViewSize((int)(Classes.Editor.Solution.SceneWidth * Classes.Editor.SolutionState.Zoom), (int)(Classes.Editor.Solution.SceneHeight * Classes.Editor.SolutionState.Zoom));
            }

            Methods.Internal.Theming.UpdateButtonColors();

        }

        public void SetParallaxAnimationOnlyButtonsState(bool enabled = true)
        {
            Controls.Base.MainEditor.Instance.EditorToolbar.Open.IsEnabled = !enabled;
            Controls.Base.MainEditor.Instance.EditorToolbar.ShowAnimations.IsEnabled = enabled || Classes.Editor.Solution.CurrentScene != null;
            Controls.Base.MainEditor.Instance.EditorToolbar.animationsSplitButton_Dropdown.IsEnabled = enabled || Classes.Editor.Solution.CurrentScene != null;
            Controls.Base.MainEditor.Instance.EditorMenuBar.MenuBar.IsEnabled = !enabled;
            Controls.Base.MainEditor.Instance.EditorStatusBar.StatusBar1.IsEnabled = !enabled; 
            Controls.Base.MainEditor.Instance.EditorTabControl.IsEnabled = !enabled;
            Controls.Base.MainEditor.Instance.EditorToolbar.New.IsEnabled = !enabled;
            Controls.Base.MainEditor.Instance.EditorToolbar.Open.IsEnabled = !enabled;

            if (enabled)
            {
                Controls.Base.MainEditor.Instance.EditorToolbar.ShowFGHigh.IsEnabled = Classes.Editor.Solution.FGHigh != null;
                Controls.Base.MainEditor.Instance.EditorToolbar.ShowFGLow.IsEnabled = Classes.Editor.Solution.FGLow != null;
                Controls.Base.MainEditor.Instance.EditorToolbar.ShowFGHigher.IsEnabled = Classes.Editor.Solution.FGHigher != null;
                Controls.Base.MainEditor.Instance.EditorToolbar.ShowFGLower.IsEnabled = Classes.Editor.Solution.FGLower != null;
                Controls.Base.MainEditor.Instance.EditorToolbar.ShowEntities.IsEnabled = true;
                Controls.Base.MainEditor.Instance.LeftToolbarToolbox.SelectedIndex = -1;
                UpdateToolbars(false, false, false);
                SetEditButtonsState(false);
            }
            foreach (var elb in Controls.Base.MainEditor.Instance.ExtraLayerEditViewButtons)
            {
                elb.Value.IsEnabled = !enabled;
            }
            Controls.Base.MainEditor.Instance.LeftToolbarToolbox.IsEnabled = !enabled;
        }
        public void SetSelectOnlyButtonsState(bool enabled = true)
        {
            Controls.Base.MainEditor.Instance.EditorMenuBar.SetPasteButtonsState(true);
            Controls.Base.MainEditor.Instance.EditorMenuBar.SetSelectOnlyButtonsState(enabled);
            enabled &= ManiacEditor.Classes.Editor.SolutionState.IsSelected();

            if (ManiacEditor.Classes.Editor.SolutionState.IsEntitiesEdit() && Controls.Base.MainEditor.Instance.EntitiesToolbar != null)
            {
                Controls.Base.MainEditor.Instance.EntitiesToolbar.SelectedEntities = Classes.Editor.Solution.Entities.SelectedEntities.Select(x => x.Entity).ToList();
            }
        }


        private void SetEditButtonsState(bool enabled)
        {
            Controls.Base.MainEditor.Instance.EditorToolbar.SetEditButtonsState(enabled);
            if (ManiacEditor.Classes.Editor.SolutionState.IsTilesEdit())
            {
                if (Controls.Base.MainEditor.Instance.TilesToolbar == null)
                {
                    if (Classes.Editor.SolutionState.UseEncoreColors)
                        Controls.Base.MainEditor.Instance.TilesToolbar = new Controls.Base.Toolbars.TilesToolbar.TilesToolbar(Classes.Editor.Solution.CurrentTiles, Controls.Base.MainEditor.Instance.Paths.StageTiles_Source, Controls.Base.MainEditor.Instance.EncorePalette[0], Controls.Base.MainEditor.Instance);
                    else
                        Controls.Base.MainEditor.Instance.TilesToolbar = new Controls.Base.Toolbars.TilesToolbar.TilesToolbar(Classes.Editor.Solution.CurrentTiles, Controls.Base.MainEditor.Instance.Paths.StageTiles_Source, null, Controls.Base.MainEditor.Instance);


                    Controls.Base.MainEditor.Instance.TilesToolbar.TileDoubleClick = new Action<int>(x =>
                    {
                        Controls.Base.MainEditor.Instance.EditorPlaceTile(new System.Drawing.Point((int)(Classes.Editor.SolutionState.ViewPositionX/ Classes.Editor.SolutionState.Zoom) + Classes.Editor.Constants.TILE_SIZE - 1, (int)(Classes.Editor.SolutionState.ViewPositionY / Classes.Editor.SolutionState.Zoom) + Classes.Editor.Constants.TILE_SIZE - 1), x, Classes.Editor.Solution.EditLayerA);
                    });
                    Controls.Base.MainEditor.Instance.TilesToolbar.TileOptionChanged = new Action<int, bool>((option, state) =>
                    {
                        Classes.Editor.Solution.EditLayerA?.SetPropertySelected(option + 12, state);
                        Classes.Editor.Solution.EditLayerB?.SetPropertySelected(option + 12, state);

                    });
                    Controls.Base.MainEditor.Instance.ToolBarPanelRight.Children.Clear();
                    Controls.Base.MainEditor.Instance.ToolBarPanelRight.Children.Add(Controls.Base.MainEditor.Instance.TilesToolbar);
                    UpdateToolbars(true, true);
                    Controls.Base.MainEditor.Instance.Editor_Resize(null, null);
                    Controls.Base.MainEditor.Instance.Focus();
                }
                if (ManiacEditor.Classes.Editor.SolutionState.IsChunksEdit()) Controls.Base.MainEditor.Instance.TilesToolbar.TabControl.TabIndex = 1;
                else Controls.Base.MainEditor.Instance.TilesToolbar.TabControl.TabIndex = 0;
                Controls.Base.MainEditor.Instance.UI.UpdateTilesOptions();
                Controls.Base.MainEditor.Instance.TilesToolbar.ShowShortcuts = Controls.Base.MainEditor.Instance.EditorToolbar.DrawToolButton.IsChecked.Value;
            }
            else
            {
                if (Controls.Base.MainEditor.Instance.TilesToolbar != null)
                {
                    Controls.Base.MainEditor.Instance.TilesToolbar.Dispose();
                    Controls.Base.MainEditor.Instance.TilesToolbar = null;
                    Controls.Base.MainEditor.Instance.Focus();
                }
            }
            if (ManiacEditor.Classes.Editor.SolutionState.IsEntitiesEdit())
            {
                if (Controls.Base.MainEditor.Instance.EntitiesToolbar == null)
                {
                    Controls.Base.MainEditor.Instance.EntitiesToolbar = new Controls.Base.Toolbars.EntitiesToolbar.EntitiesToolbar(Classes.Editor.Solution.CurrentScene.Objects, Controls.Base.MainEditor.Instance)
                    {
                        SelectedEntity = new Action<int>(x =>
                        {
                            Classes.Editor.Solution.Entities.SelectSlot(x);
                            SetSelectOnlyButtonsState();
                        }),
                        AddAction = new Action<ManiacEditor.Actions.IAction>(x =>
                        {
                            Controls.Base.MainEditor.Instance.UndoStack.Push(x);
                            Controls.Base.MainEditor.Instance.RedoStack.Clear();
                            UpdateControls();
                        }),
                        Spawn = new Action<SceneObject>(x =>
                        {
                            Classes.Editor.Solution.Entities.Add(x, GetEntitySpawnPoint());
                            Controls.Base.MainEditor.Instance.UndoStack.Push(Classes.Editor.Solution.Entities.LastAction);
                            Controls.Base.MainEditor.Instance.RedoStack.Clear();
                            UpdateControls();
                        })
                    };
                    Controls.Base.MainEditor.Instance.ToolBarPanelRight.Children.Clear();
                    Controls.Base.MainEditor.Instance.ToolBarPanelRight.Children.Add(Controls.Base.MainEditor.Instance.EntitiesToolbar);
                    UpdateToolbars(true, true);
                    Controls.Base.MainEditor.Instance.Editor_Resize(null, null);
                }
                Controls.Base.MainEditor.Instance.UI.UpdateEntitiesToolbarList();
                Controls.Base.MainEditor.Instance.EntitiesToolbar.SelectedEntities = Classes.Editor.Solution.Entities.SelectedEntities.Select(x => x.Entity).ToList();
            }
            else
            {
                if (Controls.Base.MainEditor.Instance.EntitiesToolbar != null)
                {
                    Controls.Base.MainEditor.Instance.EntitiesToolbar.Dispose();
                    Controls.Base.MainEditor.Instance.EntitiesToolbar = null;
                }
                if (Classes.Editor.Solution.Entities != null && Classes.Editor.Solution.Entities.SelectedEntities != null)
                {
                    if (Classes.Editor.Solution.Entities.SelectedEntities.Count != 0 && Classes.Editor.Solution.Entities.TemporarySelection.Count != 0)
                    {
                        Classes.Editor.Solution.Entities.EndTempSelection();
                        Classes.Editor.Solution.Entities.Deselect();
                    }
                }


            }
            if (Controls.Base.MainEditor.Instance.TilesToolbar == null && Controls.Base.MainEditor.Instance.EntitiesToolbar == null && (Controls.Base.MainEditor.Instance.ToolBarPanelRight.Children.Count != 0))
            {
                Controls.Base.MainEditor.Instance.ToolBarPanelRight.Children.Clear();
                UpdateToolbars(true, false);
                Controls.Base.MainEditor.Instance.Editor_Resize(null, null);
            }

            SetSelectOnlyButtonsState(enabled);

            Position GetEntitySpawnPoint()
            {
                if (Controls.Base.MainEditor.Instance.EditorToolbar.DrawToolButton.IsChecked.Value)
                {
                    short x = (short)(Classes.Editor.SolutionState.LastX / Classes.Editor.SolutionState.Zoom);
                    short y = (short)(Classes.Editor.SolutionState.LastY / Classes.Editor.SolutionState.Zoom);
                    if (Classes.Editor.SolutionState.UseMagnetMode)
                    {
                        short alignedX = (short)(Classes.Editor.SolutionState.MagnetSize * (x / Classes.Editor.SolutionState.MagnetSize));
                        short alignedY = (short)(Classes.Editor.SolutionState.MagnetSize * (y / Classes.Editor.SolutionState.MagnetSize));
                        return new Position(alignedX, alignedY);
                    }
                    else
                    {
                        return new Position(x, y);
                    }

                }
                else
                {
                    return new Position((short)(Classes.Editor.SolutionState.ViewPositionX/ Classes.Editor.SolutionState.Zoom), (short)(Classes.Editor.SolutionState.ViewPositionY / Classes.Editor.SolutionState.Zoom));
                }

            }
        }


        #endregion

        #region Updating Elements Methods
        public void ToggleEditorButtons(bool enabled, bool isParallaxAnimation = false)
        {
            Controls.Base.MainEditor.Instance.EditorMenuBar.MenuBar.IsEnabled = enabled;
            Controls.Base.MainEditor.Instance.EditorToolbar.LayerToolbar.IsEnabled = enabled;
            Controls.Base.MainEditor.Instance.EditorToolbar.MainToolbarButtons.IsEnabled = enabled;
            Controls.Base.MainEditor.Instance.UI.SetSceneOnlyButtonsState((enabled ? true : Classes.Editor.Solution.CurrentScene != null));
            Controls.Base.MainEditor.Instance.EditorToolbar.LayerToolbar.IsEnabled = enabled;
            Controls.Base.MainEditor.Instance.EditorStatusBar.StatusBar1.IsEnabled = enabled;
            Controls.Base.MainEditor.Instance.EditorStatusBar.StatusBar2.IsEnabled = enabled;
            if (Controls.Base.MainEditor.Instance.TilesToolbar != null) Controls.Base.MainEditor.Instance.TilesToolbar.IsEnabled = enabled;
            if (Controls.Base.MainEditor.Instance.EntitiesToolbar != null) Controls.Base.MainEditor.Instance.EntitiesToolbar.IsEnabled = enabled;
            if (isParallaxAnimation)
            {
                Controls.Base.MainEditor.Instance.EditorToolbar.LayerToolbar.IsEnabled = true;
                foreach (var pair in Controls.Base.MainEditor.Instance.ExtraLayerEditViewButtons)
                {
                    pair.Key.IsEnabled = false;
                    pair.Value.IsEnabled = true;
                }
                Controls.Base.MainEditor.Instance.EditorToolbar.EditFGHigh.IsEnabled = false;
                Controls.Base.MainEditor.Instance.EditorToolbar.EditFGHigher.IsEnabled = false;
                Controls.Base.MainEditor.Instance.EditorToolbar.EditFGLow.IsEnabled = false;
                Controls.Base.MainEditor.Instance.EditorToolbar.EditFGLower.IsEnabled = false;

            }
        }
        public void UpdateTilesOptions()
        {
            if (ManiacEditor.Classes.Editor.SolutionState.IsTilesEdit() && !ManiacEditor.Classes.Editor.SolutionState.IsChunksEdit())
            {
                if (Controls.Base.MainEditor.Instance.TilesToolbar != null)
                {
                    List<ushort> values = Classes.Editor.Solution.EditLayerA?.GetSelectedValues();
                    List<ushort> valuesB = Classes.Editor.Solution.EditLayerB?.GetSelectedValues();
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
                            Controls.Base.MainEditor.Instance.TilesToolbar.SetTileOptionState(i, unk ? Controls.Base.Toolbars.TilesToolbar.TilesToolbar.TileOptionState.Indeterminate : set ? Controls.Base.Toolbars.TilesToolbar.TilesToolbar.TileOptionState.Checked : Controls.Base.Toolbars.TilesToolbar.TilesToolbar.TileOptionState.Unchcked);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < 4; ++i)
                            Controls.Base.MainEditor.Instance.TilesToolbar.SetTileOptionState(i, Controls.Base.Toolbars.TilesToolbar.TilesToolbar.TileOptionState.Disabled);
                    }
                }

            }
        }
        public void UpdateEntitiesToolbarList()
        {
            Controls.Base.MainEditor.Instance.EntitiesToolbar.Entities = Classes.Editor.Solution.Entities.Entities.Select(x => x.Entity).ToList();
        }
        public void UpdateEditLayerActions()
        {
            if (Classes.Editor.Solution.EditLayerA != null)
            {
                List<IAction> actions = Classes.Editor.Solution.EditLayerA?.Actions;
                if (actions.Count > 0) Controls.Base.MainEditor.Instance.RedoStack.Clear();
                while (actions.Count > 0)
                {
                    bool create_new = false;
                    if (Controls.Base.MainEditor.Instance.UndoStack.Count == 0 || !(Controls.Base.MainEditor.Instance.UndoStack.Peek() is ActionsGroup))
                    {
                        create_new = true;
                    }
                    else
                    {
                        create_new = (Controls.Base.MainEditor.Instance.UndoStack.Peek() as ActionsGroup).IsClosed;
                    }
                    if (create_new)
                    {
                        Controls.Base.MainEditor.Instance.UndoStack.Push(new ActionsGroup());
                    }
                    (Controls.Base.MainEditor.Instance.UndoStack.Peek() as ActionsGroup).AddAction(actions[0]);
                    actions.RemoveAt(0);
                }
            }
            if (Classes.Editor.Solution.EditLayerB != null)
            {
                List<IAction> actions = Classes.Editor.Solution.EditLayerB?.Actions;
                if (actions.Count > 0) Controls.Base.MainEditor.Instance.RedoStack.Clear();
                while (actions.Count > 0)
                {
                    bool create_new = false;
                    if (Controls.Base.MainEditor.Instance.UndoStack.Count == 0 || !(Controls.Base.MainEditor.Instance.UndoStack.Peek() is ActionsGroup))
                    {
                        create_new = true;
                    }
                    else
                    {
                        create_new = (Controls.Base.MainEditor.Instance.UndoStack.Peek() as ActionsGroup).IsClosed;
                    }
                    if (create_new)
                    {
                        Controls.Base.MainEditor.Instance.UndoStack.Push(new ActionsGroup());
                    }
                    (Controls.Base.MainEditor.Instance.UndoStack.Peek() as ActionsGroup).AddAction(actions[0]);
                    actions.RemoveAt(0);
                }
            }
        }
        public void UpdateToolbars(bool rightToolbar = true, bool visible = false, bool fullCollapse = false)
        {
            if (rightToolbar)
            {
                if (visible)
                {
                    Controls.Base.MainEditor.Instance.ToolbarRight.Width = new GridLength(300);
                    Controls.Base.MainEditor.Instance.ToolbarRight.MinWidth = 300;
                    Controls.Base.MainEditor.Instance.ToolbarRight.MaxWidth = Controls.Base.MainEditor.Instance.ViewPanelForm.ActualWidth / 3;
                    Controls.Base.MainEditor.Instance.SplitterRight.Width = new GridLength(6);
                    Controls.Base.MainEditor.Instance.SplitterRight.MinWidth = 6;
                }
                else
                {
                    Controls.Base.MainEditor.Instance.ToolbarRight.Width = new GridLength(0);
                    Controls.Base.MainEditor.Instance.ToolbarRight.MinWidth = 0;
                    Controls.Base.MainEditor.Instance.ToolbarRight.MaxWidth = 0;
                    Controls.Base.MainEditor.Instance.SplitterRight.Width = new GridLength(0);
                    Controls.Base.MainEditor.Instance.SplitterRight.MinWidth = 0;
                }
            }

            else
            {
                if (visible)
                {
                    Controls.Base.MainEditor.Instance.ToolbarLeft.Width = new GridLength(200);
                    Controls.Base.MainEditor.Instance.ToolbarLeft.MinWidth = 200;
                    Controls.Base.MainEditor.Instance.ToolbarLeft.MaxWidth = Controls.Base.MainEditor.Instance.ViewPanelForm.ActualWidth / 3;
                    Controls.Base.MainEditor.Instance.SplitterLeft.Width = new GridLength(3);
                    Controls.Base.MainEditor.Instance.SplitterLeft.MinWidth = 3;
                    Controls.Base.MainEditor.Instance.LeftToolbarToolbox.Visibility = Visibility.Visible;
                }
                else
                {
                    if (!fullCollapse)
                    {
                        Controls.Base.MainEditor.Instance.ToolbarLeft.Width = new GridLength(10);
                        Controls.Base.MainEditor.Instance.ToolbarLeft.MinWidth = 10;
                        Controls.Base.MainEditor.Instance.ToolbarLeft.MaxWidth = 10;
                        Controls.Base.MainEditor.Instance.SplitterLeft.Width = new GridLength(0);
                        Controls.Base.MainEditor.Instance.SplitterLeft.MinWidth = 0;
                        Controls.Base.MainEditor.Instance.LeftToolbarToolbox.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        Controls.Base.MainEditor.Instance.ToolbarLeft.Width = new GridLength(0);
                        Controls.Base.MainEditor.Instance.ToolbarLeft.MinWidth = 0;
                        Controls.Base.MainEditor.Instance.ToolbarLeft.MaxWidth = 0;
                        Controls.Base.MainEditor.Instance.SplitterLeft.Width = new GridLength(0);
                        Controls.Base.MainEditor.Instance.SplitterLeft.MinWidth = 0;
                        Controls.Base.MainEditor.Instance.LeftToolbarToolbox.Visibility = Visibility.Hidden;
                    }

                }
            }

        }
        public void UpdateWaitingScreen(bool show)
        {
            if (show)
            {
                Controls.Base.MainEditor.Instance.ViewPanelForm.Visibility = Visibility.Hidden;
                Controls.Base.MainEditor.Instance.WaitingPanel.Visibility = Visibility.Visible;
            }
            else
            {
                Controls.Base.MainEditor.Instance.ViewPanelForm.Visibility = Visibility.Visible;
                Controls.Base.MainEditor.Instance.WaitingPanel.Visibility = Visibility.Collapsed;
            }

        }

        public void UpdateSplineSpawnObjectsList(List<RSDKv5.SceneObject> sceneObjects)
        {
            Classes.Editor.SolutionState.AllowSplineOptionsUpdate = false;
            sceneObjects.Sort((x, y) => x.Name.ToString().CompareTo(y.Name.ToString()));
            var bindingSceneObjectsList = new System.ComponentModel.BindingList<RSDKv5.SceneObject>(sceneObjects);


            Controls.Base.MainEditor.Instance.SplineSelectedObjectSpawnList.Clear();
            foreach (var _object in bindingSceneObjectsList)
            {
                TextBlock item = new TextBlock()
                {
                    Tag = _object,
                    Text = _object.Name.Name
                };
                Controls.Base.MainEditor.Instance.SplineSelectedObjectSpawnList.Add(item);
            }

            if (Controls.Base.MainEditor.Instance.SplineSelectedObjectSpawnList != null && Controls.Base.MainEditor.Instance.SplineSelectedObjectSpawnList.Count > 1)
            {
                Controls.Base.MainEditor.Instance.EditorToolbar.SelectedSplineRender.ItemsSource = Controls.Base.MainEditor.Instance.SplineSelectedObjectSpawnList;
                Controls.Base.MainEditor.Instance.EditorToolbar.SelectedSplineRender.SelectedItem = Controls.Base.MainEditor.Instance.EditorToolbar.SelectedSplineRender.Items[0];
                var SelectedItem = Controls.Base.MainEditor.Instance.EditorToolbar.SelectedSplineRender.SelectedItem as TextBlock;
                if (SelectedItem == null) return;              
                SelectedItem.Foreground = (System.Windows.Media.SolidColorBrush)Controls.Base.MainEditor.Instance.FindResource("NormalText");
                Classes.Editor.SolutionState.AllowSplineOptionsUpdate = true;

            }
        }

        public void UpdateSplineSettings(int splineID)
        {
            if (!Classes.Editor.SolutionState.SplineOptionsGroup.ContainsKey(splineID)) Classes.Editor.SolutionState.SplineOptionsGroup.Add(splineID, new Classes.Editor.SolutionState.SplineOptions());
            Controls.Base.MainEditor.Instance.EditorToolbar.SplineLineMode.IsChecked = Classes.Editor.SolutionState.SplineOptionsGroup[splineID].SplineLineMode;
            Controls.Base.MainEditor.Instance.EditorToolbar.SplineOvalMode.IsChecked = Classes.Editor.SolutionState.SplineOptionsGroup[splineID].SplineOvalMode;
            Controls.Base.MainEditor.Instance.EditorToolbar.SplineShowLineCheckbox.IsChecked = Classes.Editor.SolutionState.SplineOptionsGroup[splineID].SplineToolShowLines;
            Controls.Base.MainEditor.Instance.EditorToolbar.SplineShowObjectsCheckbox.IsChecked = Classes.Editor.SolutionState.SplineOptionsGroup[splineID].SplineToolShowObject;
            Controls.Base.MainEditor.Instance.EditorToolbar.SplineShowPointsCheckbox.IsChecked = Classes.Editor.SolutionState.SplineOptionsGroup[splineID].SplineToolShowPoints;
            Controls.Base.MainEditor.Instance.EditorToolbar.SplinePointSeperationNUD.Value = Classes.Editor.SolutionState.SplineOptionsGroup[splineID].SplineSize;
            Controls.Base.MainEditor.Instance.EditorToolbar.SplinePointSeperationSlider.Value = Classes.Editor.SolutionState.SplineOptionsGroup[splineID].SplineSize;

            if (Classes.Editor.SolutionState.SplineOptionsGroup[splineID].SplineObjectRenderingTemplate != null)
                Controls.Base.MainEditor.Instance.EditorToolbar.SplineRenderObjectName.Content = Classes.Editor.SolutionState.SplineOptionsGroup[splineID].SplineObjectRenderingTemplate.Entity.Object.Name.Name;
            else
                Controls.Base.MainEditor.Instance.EditorToolbar.SplineRenderObjectName.Content = "None";
        }

        public void UpdateSplineToolbox()
        {
            //Editor.Instance.SplineInfoLabel1.Text = string.Format("Number of Spline Objects: {0}", Editor.Instance.UIModes.SplineTotalNumberOfObjects);
            //Editor.Instance.SplineInfoLabel2.Text = string.Format("Point Frequency: {0}", Editor.Instance.UIModes.SplineSize);
            //Editor.Instance.SplineInfoLabel3.Text = string.Format("Total Number of Rendered Points: {0}", Editor.Instance.UIModes.SplineCurrentPointsDrawn);
        }

        public void UpdateCustomColors()
        {
            Controls.Base.MainEditor.Instance.EditorToolbar.CSAC.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(Classes.Editor.SolutionState.CollisionSAColour.A, Classes.Editor.SolutionState.CollisionSAColour.R, Classes.Editor.SolutionState.CollisionSAColour.G, Classes.Editor.SolutionState.CollisionSAColour.B));
            Controls.Base.MainEditor.Instance.EditorToolbar.SSTOC.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(Classes.Editor.SolutionState.CollisionTOColour.A, Classes.Editor.SolutionState.CollisionTOColour.R, Classes.Editor.SolutionState.CollisionTOColour.G, Classes.Editor.SolutionState.CollisionTOColour.B));
            Controls.Base.MainEditor.Instance.EditorToolbar.CSLRDC.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(Classes.Editor.SolutionState.CollisionLRDColour.A, Classes.Editor.SolutionState.CollisionLRDColour.R, Classes.Editor.SolutionState.CollisionLRDColour.G, Classes.Editor.SolutionState.CollisionLRDColour.B));
            Controls.Base.MainEditor.Instance.EditorToolbar.WLC.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(Classes.Editor.SolutionState.waterColor.A, Classes.Editor.SolutionState.waterColor.R, Classes.Editor.SolutionState.waterColor.G, Classes.Editor.SolutionState.waterColor.B));
            Controls.Base.MainEditor.Instance.EditorToolbar.GDC.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(Classes.Editor.SolutionState.GridColor.A, Classes.Editor.SolutionState.GridColor.R, Classes.Editor.SolutionState.GridColor.G, Classes.Editor.SolutionState.GridColor.B));
        }

        public void UpdateControls(bool stageLoad = false)
        {
            if (Core.Settings.MySettings.EntityFreeCam)
            {
                Controls.Base.MainEditor.Instance.DeviceModel.vScrollBar1.IsEnabled = false;
                Controls.Base.MainEditor.Instance.DeviceModel.hScrollBar1.IsEnabled = false;
            }
            else
            {
                Controls.Base.MainEditor.Instance.DeviceModel.vScrollBar1.IsEnabled = true;
                Controls.Base.MainEditor.Instance.DeviceModel.hScrollBar1.IsEnabled = true;
            }

            bool parallaxAnimationInProgress = Classes.Editor.SolutionState.AllowAnimations && Classes.Editor.SolutionState.ParallaxAnimationChecked;

            Controls.Base.MainEditor.Instance.EditorToolbar.UpdateGameRunningButton(Classes.Editor.Solution.CurrentScene != null);
            Methods.Internal.Theming.UpdateThemeForItemsWaiting();
            Controls.Base.MainEditor.Instance.EditorStatusBar.UpdateFilterButtonApperance(false);
            Controls.Base.MainEditor.Instance.EditorStatusBar.UpdateStatusPanel();
            SetSceneOnlyButtonsState(Classes.Editor.Solution.CurrentScene != null && !parallaxAnimationInProgress, stageLoad);
            SetParallaxAnimationOnlyButtonsState(parallaxAnimationInProgress);
            UpdateSplineToolbox();
            Controls.Base.MainEditor.Instance.EditorToolbar.CustomGridLabel.Text = string.Format(Controls.Base.MainEditor.Instance.EditorToolbar.CustomGridLabel.Tag.ToString(), Properties.Defaults.Default.CustomGridSizeValue);

        }

        private void UpdateTooltips()
        {
            UpdateTooltipForStacks(Controls.Base.MainEditor.Instance.EditorToolbar.UndoButton, Controls.Base.MainEditor.Instance.UndoStack);
            UpdateTooltipForStacks(Controls.Base.MainEditor.Instance.EditorToolbar.RedoButton, Controls.Base.MainEditor.Instance.RedoStack);
            UpdateTextBlockForStacks(Controls.Base.MainEditor.Instance.EditorMenuBar.UndoMenuItemInfo, Controls.Base.MainEditor.Instance.UndoStack);
            UpdateTextBlockForStacks(Controls.Base.MainEditor.Instance.EditorMenuBar.RedoMenuItemInfo, Controls.Base.MainEditor.Instance.RedoStack);
            if (Controls.Base.MainEditor.Instance.EditorControls != null)
            {
                if (Controls.Base.MainEditor.Instance.IsVisible)
                {
                    Controls.Base.MainEditor.Instance.EditorMenuBar.UpdateMenuItems();
                    Controls.Base.MainEditor.Instance.EditorControls.UpdateTooltips();
                }

            }

        }
        private void UpdateTextBlockForStacks(TextBlock tsb, Stack<IAction> actionStack)
        {
            if (actionStack?.Count > 0)
            {
                IAction action = actionStack.Peek();
                tsb.Visibility = Visibility.Visible;
                tsb.Text = string.Format("({0})", action.Description);
            }
            else
            {
                tsb.Visibility = Visibility.Collapsed;
                tsb.Text = string.Empty;
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

        #endregion

        #region Graphical Events
        public void ReloadSpritesAndTextures()
        {
            try
            {
                // release all our resources, and force a reload of the tiles
                // Entities should take care of themselves
                Controls.Base.MainEditor.Instance.DisposeTextures();
                Controls.Base.MainEditor.Instance.EntityDrawing.ReleaseResources();
                //EditorEntity_ini.rendersWithErrors.Clear();

                //Reload for Encore Palletes, otherwise reload the image normally
                if (Classes.Editor.SolutionState.UseEncoreColors == true)
                {
                    Classes.Editor.Solution.CurrentTiles?.Image.Reload(Controls.Base.MainEditor.Instance.EncorePalette[0]);
                    Controls.Base.MainEditor.Instance.TilesToolbar?.Reload(Controls.Base.MainEditor.Instance.EncorePalette[0]);
                }
                else
                {
                    Classes.Editor.Solution.CurrentTiles?.Image.Reload();
                    Controls.Base.MainEditor.Instance.TilesToolbar?.Reload();
                }

                Classes.Editor.Solution.TileConfig = new Tileconfig(Controls.Base.MainEditor.Instance.Paths.TileConfig_Source);



            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        #endregion
    }
}
