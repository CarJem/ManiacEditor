using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using RSDKv5;
using ManiacEditor.Actions;
using System.Windows.Controls;
using EditClasses.Solution;

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
            Editor.Instance.EditorToolbar.SetSceneOnlyButtonsState(enabled, stageLoad);
            Editor.Instance.EditorToolbar.UpdateGameRunningButton(enabled);

            SetEditButtonsState(enabled);
            UpdateTooltips();

            if (stageLoad)
            {
                Editor.Instance.ZoomModel.SetViewSize((int)(Editor.Instance.SceneWidth * EditClasses.EditorState.Zoom), (int)(Editor.Instance.SceneHeight * EditClasses.EditorState.Zoom));
            }

            Editor.Instance.Theming.UpdateButtonColors();

        }

        public void SetParallaxAnimationOnlyButtonsState(bool enabled = true)
        {
            Editor.Instance.EditorToolbar.Open.IsEnabled = !enabled;
            Editor.Instance.EditorToolbar.ShowAnimations.IsEnabled = enabled || CurrentSolution.CurrentScene != null;
            Editor.Instance.EditorToolbar.animationsSplitButton_Dropdown.IsEnabled = enabled || CurrentSolution.CurrentScene != null;
            Editor.Instance.EditorMenuBar.MenuBar.IsEnabled = !enabled;
            Editor.Instance.EditorStatusBar.StatusBar1.IsEnabled = !enabled; 
            Editor.Instance.EditorTabControl.IsEnabled = !enabled;
            Editor.Instance.EditorToolbar.New.IsEnabled = !enabled;
            Editor.Instance.EditorToolbar.Open.IsEnabled = !enabled;

            if (enabled)
            {
                Editor.Instance.EditorToolbar.ShowFGHigh.IsEnabled = Editor.Instance.FGHigh != null;
                Editor.Instance.EditorToolbar.ShowFGLow.IsEnabled = Editor.Instance.FGLow != null;
                Editor.Instance.EditorToolbar.ShowFGHigher.IsEnabled = Editor.Instance.FGHigher != null;
                Editor.Instance.EditorToolbar.ShowFGLower.IsEnabled = Editor.Instance.FGLower != null;
                Editor.Instance.EditorToolbar.ShowEntities.IsEnabled = true;
                Editor.Instance.LeftToolbarToolbox.SelectedIndex = -1;
                UpdateToolbars(false, false, false);
                SetEditButtonsState(false);
            }
            foreach (var elb in Editor.Instance.ExtraLayerEditViewButtons)
            {
                elb.Value.IsEnabled = !enabled;
            }
            Editor.Instance.LeftToolbarToolbox.IsEnabled = !enabled;
        }
        public void SetSelectOnlyButtonsState(bool enabled = true)
        {
            Editor.Instance.EditorMenuBar.SetPasteButtonsState(true);
            Editor.Instance.EditorMenuBar.SetSelectOnlyButtonsState(enabled);
            enabled &= Editor.Instance.IsSelected();

            if (Editor.Instance.IsEntitiesEdit() && Editor.Instance.EntitiesToolbar != null)
            {
                Editor.Instance.EntitiesToolbar.SelectedEntities = CurrentSolution.Entities.SelectedEntities.Select(x => x.Entity).ToList();
            }
        }


        private void SetEditButtonsState(bool enabled)
        {
            Editor.Instance.EditorToolbar.SetEditButtonsState(enabled);
            if (Editor.Instance.IsTilesEdit())
            {
                if (Editor.Instance.TilesToolbar == null)
                {
                    if (EditClasses.EditorState.UseEncoreColors)
                        Editor.Instance.TilesToolbar = new TilesToolbar(CurrentSolution.CurrentTiles.StageTiles, Editor.Instance.Paths.StageTiles_Source, Editor.Instance.EncorePalette[0], Editor.Instance);
                    else
                        Editor.Instance.TilesToolbar = new TilesToolbar(CurrentSolution.CurrentTiles.StageTiles, Editor.Instance.Paths.StageTiles_Source, null, Editor.Instance);


                    Editor.Instance.TilesToolbar.TileDoubleClick = new Action<int>(x =>
                    {
                        Editor.Instance.EditorPlaceTile(new System.Drawing.Point((int)(EditClasses.EditorState.ViewPositionX/ EditClasses.EditorState.Zoom) + EditorConstants.TILE_SIZE - 1, (int)(EditClasses.EditorState.ViewPositionY / EditClasses.EditorState.Zoom) + EditorConstants.TILE_SIZE - 1), x, Editor.Instance.EditLayerA);
                    });
                    Editor.Instance.TilesToolbar.TileOptionChanged = new Action<int, bool>((option, state) =>
                    {
                        Editor.Instance.EditLayerA?.SetPropertySelected(option + 12, state);
                        Editor.Instance.EditLayerB?.SetPropertySelected(option + 12, state);

                    });
                    Editor.Instance.ToolBarPanelRight.Children.Clear();
                    Editor.Instance.ToolBarPanelRight.Children.Add(Editor.Instance.TilesToolbar);
                    UpdateToolbars(true, true);
                    Editor.Instance.Editor_Resize(null, null);
                    Editor.Instance.Focus();
                }
                if (Editor.Instance.IsChunksEdit()) Editor.Instance.TilesToolbar.TabControl.TabIndex = 1;
                else Editor.Instance.TilesToolbar.TabControl.TabIndex = 0;
                Editor.Instance.UI.UpdateTilesOptions();
                Editor.Instance.TilesToolbar.ShowShortcuts = Editor.Instance.EditorToolbar.DrawToolButton.IsChecked.Value;
            }
            else
            {
                if (Editor.Instance.TilesToolbar != null)
                {
                    Editor.Instance.TilesToolbar.Dispose();
                    Editor.Instance.TilesToolbar = null;
                    Editor.Instance.Focus();
                }
            }
            if (Editor.Instance.IsEntitiesEdit())
            {
                if (Editor.Instance.EntitiesToolbar == null)
                {
                    Editor.Instance.EntitiesToolbar = new EntitiesToolbar(CurrentSolution.CurrentScene.Objects, Editor.Instance)
                    {
                        SelectedEntity = new Action<int>(x =>
                        {
                            CurrentSolution.Entities.SelectSlot(x);
                            SetSelectOnlyButtonsState();
                        }),
                        AddAction = new Action<ManiacEditor.Actions.IAction>(x =>
                        {
                            Editor.Instance.UndoStack.Push(x);
                            Editor.Instance.RedoStack.Clear();
                            UpdateControls();
                        }),
                        Spawn = new Action<SceneObject>(x =>
                        {
                            CurrentSolution.Entities.Add(x, GetEntitySpawnPoint());
                            Editor.Instance.UndoStack.Push(CurrentSolution.Entities.LastAction);
                            Editor.Instance.RedoStack.Clear();
                            UpdateControls();
                        })
                    };
                    Editor.Instance.ToolBarPanelRight.Children.Clear();
                    Editor.Instance.ToolBarPanelRight.Children.Add(Editor.Instance.EntitiesToolbar);
                    UpdateToolbars(true, true);
                    Editor.Instance.Editor_Resize(null, null);
                }
                Editor.Instance.UI.UpdateEntitiesToolbarList();
                Editor.Instance.EntitiesToolbar.SelectedEntities = CurrentSolution.Entities.SelectedEntities.Select(x => x.Entity).ToList();
            }
            else
            {
                if (Editor.Instance.EntitiesToolbar != null)
                {
                    Editor.Instance.EntitiesToolbar.Dispose();
                    Editor.Instance.EntitiesToolbar = null;
                }
                if (CurrentSolution.Entities != null && CurrentSolution.Entities.SelectedEntities != null)
                {
                    if (CurrentSolution.Entities.SelectedEntities.Count != 0 && CurrentSolution.Entities.TemporarySelection.Count != 0)
                    {
                        CurrentSolution.Entities.EndTempSelection();
                        CurrentSolution.Entities.Deselect();
                    }
                }


            }
            if (Editor.Instance.TilesToolbar == null && Editor.Instance.EntitiesToolbar == null && (Editor.Instance.ToolBarPanelRight.Children.Count != 0))
            {
                Editor.Instance.ToolBarPanelRight.Children.Clear();
                UpdateToolbars(true, false);
                Editor.Instance.Editor_Resize(null, null);
            }
            SetSelectOnlyButtonsState(enabled);
            Position GetEntitySpawnPoint()
            {
                if (Editor.Instance.EditorToolbar.DrawToolButton.IsChecked.Value)
                {
                    short x = (short)(EditClasses.EditorState.LastX / EditClasses.EditorState.Zoom);
                    short y = (short)(EditClasses.EditorState.LastY / EditClasses.EditorState.Zoom);
                    if (EditClasses.EditorState.UseMagnetMode)
                    {
                        short alignedX = (short)(EditClasses.EditorState.MagnetSize * (x / EditClasses.EditorState.MagnetSize));
                        short alignedY = (short)(EditClasses.EditorState.MagnetSize * (y / EditClasses.EditorState.MagnetSize));
                        return new Position(alignedX, alignedY);
                    }
                    else
                    {
                        return new Position(x, y);
                    }

                }
                else
                {
                    return new Position((short)(EditClasses.EditorState.ViewPositionX/ EditClasses.EditorState.Zoom), (short)(EditClasses.EditorState.ViewPositionY / EditClasses.EditorState.Zoom));
                }

            }
        }


        #endregion

        #region Updating Elements Methods
        public void ToggleEditorButtons(bool enabled, bool isParallaxAnimation = false)
        {
            Editor.Instance.EditorMenuBar.MenuBar.IsEnabled = enabled;
            Editor.Instance.EditorToolbar.LayerToolbar.IsEnabled = enabled;
            Editor.Instance.EditorToolbar.MainToolbarButtons.IsEnabled = enabled;
            Editor.Instance.UI.SetSceneOnlyButtonsState((enabled ? true : CurrentSolution.CurrentScene != null));
            Editor.Instance.EditorToolbar.LayerToolbar.IsEnabled = enabled;
            Editor.Instance.EditorStatusBar.StatusBar1.IsEnabled = enabled;
            Editor.Instance.EditorStatusBar.StatusBar2.IsEnabled = enabled;
            if (Editor.Instance.TilesToolbar != null) Editor.Instance.TilesToolbar.IsEnabled = enabled;
            if (Editor.Instance.EntitiesToolbar != null) Editor.Instance.EntitiesToolbar.IsEnabled = enabled;
            if (isParallaxAnimation)
            {
                Editor.Instance.EditorToolbar.LayerToolbar.IsEnabled = true;
                foreach (var pair in Editor.Instance.ExtraLayerEditViewButtons)
                {
                    pair.Key.IsEnabled = false;
                    pair.Value.IsEnabled = true;
                }
                Editor.Instance.EditorToolbar.EditFGHigh.IsEnabled = false;
                Editor.Instance.EditorToolbar.EditFGHigher.IsEnabled = false;
                Editor.Instance.EditorToolbar.EditFGLow.IsEnabled = false;
                Editor.Instance.EditorToolbar.EditFGLower.IsEnabled = false;

            }
        }
        public void UpdateTilesOptions()
        {
            if (Editor.Instance.IsTilesEdit() && !Editor.Instance.IsChunksEdit())
            {
                if (Editor.Instance.TilesToolbar != null)
                {
                    List<ushort> values = Editor.Instance.EditLayerA?.GetSelectedValues();
                    List<ushort> valuesB = Editor.Instance.EditLayerB?.GetSelectedValues();
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
                            Editor.Instance.TilesToolbar.SetTileOptionState(i, unk ? TilesToolbar.TileOptionState.Indeterminate : set ? TilesToolbar.TileOptionState.Checked : TilesToolbar.TileOptionState.Unchcked);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < 4; ++i)
                            Editor.Instance.TilesToolbar.SetTileOptionState(i, TilesToolbar.TileOptionState.Disabled);
                    }
                }

            }
        }
        public void UpdateEntitiesToolbarList()
        {
            Editor.Instance.EntitiesToolbar.Entities = CurrentSolution.Entities.Entities.Select(x => x.Entity).ToList();
        }
        public void UpdateEditLayerActions()
        {
            if (Editor.Instance.EditLayerA != null)
            {
                List<IAction> actions = Editor.Instance.EditLayerA?.Actions;
                if (actions.Count > 0) Editor.Instance.RedoStack.Clear();
                while (actions.Count > 0)
                {
                    bool create_new = false;
                    if (Editor.Instance.UndoStack.Count == 0 || !(Editor.Instance.UndoStack.Peek() is ActionsGroup))
                    {
                        create_new = true;
                    }
                    else
                    {
                        create_new = (Editor.Instance.UndoStack.Peek() as ActionsGroup).IsClosed;
                    }
                    if (create_new)
                    {
                        Editor.Instance.UndoStack.Push(new ActionsGroup());
                    }
                    (Editor.Instance.UndoStack.Peek() as ActionsGroup).AddAction(actions[0]);
                    actions.RemoveAt(0);
                }
            }
            if (Editor.Instance.EditLayerB != null)
            {
                List<IAction> actions = Editor.Instance.EditLayerB?.Actions;
                if (actions.Count > 0) Editor.Instance.RedoStack.Clear();
                while (actions.Count > 0)
                {
                    bool create_new = false;
                    if (Editor.Instance.UndoStack.Count == 0 || !(Editor.Instance.UndoStack.Peek() is ActionsGroup))
                    {
                        create_new = true;
                    }
                    else
                    {
                        create_new = (Editor.Instance.UndoStack.Peek() as ActionsGroup).IsClosed;
                    }
                    if (create_new)
                    {
                        Editor.Instance.UndoStack.Push(new ActionsGroup());
                    }
                    (Editor.Instance.UndoStack.Peek() as ActionsGroup).AddAction(actions[0]);
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
                    Editor.Instance.ToolbarRight.Width = new GridLength(300);
                    Editor.Instance.ToolbarRight.MinWidth = 300;
                    Editor.Instance.ToolbarRight.MaxWidth = Editor.Instance.ViewPanelForm.ActualWidth / 3;
                    Editor.Instance.SplitterRight.Width = new GridLength(6);
                    Editor.Instance.SplitterRight.MinWidth = 6;
                }
                else
                {
                    Editor.Instance.ToolbarRight.Width = new GridLength(0);
                    Editor.Instance.ToolbarRight.MinWidth = 0;
                    Editor.Instance.ToolbarRight.MaxWidth = 0;
                    Editor.Instance.SplitterRight.Width = new GridLength(0);
                    Editor.Instance.SplitterRight.MinWidth = 0;
                }
            }

            else
            {
                if (visible)
                {
                    Editor.Instance.ToolbarLeft.Width = new GridLength(200);
                    Editor.Instance.ToolbarLeft.MinWidth = 200;
                    Editor.Instance.ToolbarLeft.MaxWidth = Editor.Instance.ViewPanelForm.ActualWidth / 3;
                    Editor.Instance.SplitterLeft.Width = new GridLength(3);
                    Editor.Instance.SplitterLeft.MinWidth = 3;
                    Editor.Instance.LeftToolbarToolbox.Visibility = Visibility.Visible;
                }
                else
                {
                    if (!fullCollapse)
                    {
                        Editor.Instance.ToolbarLeft.Width = new GridLength(10);
                        Editor.Instance.ToolbarLeft.MinWidth = 10;
                        Editor.Instance.ToolbarLeft.MaxWidth = 10;
                        Editor.Instance.SplitterLeft.Width = new GridLength(0);
                        Editor.Instance.SplitterLeft.MinWidth = 0;
                        Editor.Instance.LeftToolbarToolbox.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        Editor.Instance.ToolbarLeft.Width = new GridLength(0);
                        Editor.Instance.ToolbarLeft.MinWidth = 0;
                        Editor.Instance.ToolbarLeft.MaxWidth = 0;
                        Editor.Instance.SplitterLeft.Width = new GridLength(0);
                        Editor.Instance.SplitterLeft.MinWidth = 0;
                        Editor.Instance.LeftToolbarToolbox.Visibility = Visibility.Hidden;
                    }

                }
            }

        }
        public void UpdateWaitingScreen(bool show)
        {
            if (show)
            {
                Editor.Instance.ViewPanelForm.Visibility = Visibility.Hidden;
                Editor.Instance.WaitingPanel.Visibility = Visibility.Visible;
            }
            else
            {
                Editor.Instance.ViewPanelForm.Visibility = Visibility.Visible;
                Editor.Instance.WaitingPanel.Visibility = Visibility.Collapsed;
            }

        }

        public void UpdateSplineSpawnObjectsList(List<RSDKv5.SceneObject> sceneObjects)
        {
            EditClasses.EditorState.AllowSplineOptionsUpdate = false;
            sceneObjects.Sort((x, y) => x.Name.ToString().CompareTo(y.Name.ToString()));
            var bindingSceneObjectsList = new System.ComponentModel.BindingList<RSDKv5.SceneObject>(sceneObjects);


            Editor.Instance.SplineSelectedObjectSpawnList.Clear();
            foreach (var _object in bindingSceneObjectsList)
            {
                TextBlock item = new TextBlock()
                {
                    Tag = _object,
                    Text = _object.Name.Name
                };
                Editor.Instance.SplineSelectedObjectSpawnList.Add(item);
            }

            if (Editor.Instance.SplineSelectedObjectSpawnList != null && Editor.Instance.SplineSelectedObjectSpawnList.Count > 1)
            {
                Editor.Instance.EditorToolbar.SelectedSplineRender.ItemsSource = Editor.Instance.SplineSelectedObjectSpawnList;
                Editor.Instance.EditorToolbar.SelectedSplineRender.SelectedItem = Editor.Instance.EditorToolbar.SelectedSplineRender.Items[0];
                var SelectedItem = Editor.Instance.EditorToolbar.SelectedSplineRender.SelectedItem as TextBlock;
                if (SelectedItem == null) return;              
                SelectedItem.Foreground = (System.Windows.Media.SolidColorBrush)Editor.Instance.FindResource("NormalText");
                EditClasses.EditorState.AllowSplineOptionsUpdate = true;

            }
        }

        public void UpdateSplineSettings(int splineID)
        {
            if (!EditClasses.EditorState.SplineOptionsGroup.ContainsKey(splineID)) EditClasses.EditorState.SplineOptionsGroup.Add(splineID, new EditClasses.EditorState.SplineOptions());
            Editor.Instance.EditorToolbar.SplineLineMode.IsChecked = EditClasses.EditorState.SplineOptionsGroup[splineID].SplineLineMode;
            Editor.Instance.EditorToolbar.SplineOvalMode.IsChecked = EditClasses.EditorState.SplineOptionsGroup[splineID].SplineOvalMode;
            Editor.Instance.EditorToolbar.SplineShowLineCheckbox.IsChecked = EditClasses.EditorState.SplineOptionsGroup[splineID].SplineToolShowLines;
            Editor.Instance.EditorToolbar.SplineShowObjectsCheckbox.IsChecked = EditClasses.EditorState.SplineOptionsGroup[splineID].SplineToolShowObject;
            Editor.Instance.EditorToolbar.SplineShowPointsCheckbox.IsChecked = EditClasses.EditorState.SplineOptionsGroup[splineID].SplineToolShowPoints;
            Editor.Instance.EditorToolbar.SplinePointSeperationNUD.Value = EditClasses.EditorState.SplineOptionsGroup[splineID].SplineSize;
            Editor.Instance.EditorToolbar.SplinePointSeperationSlider.Value = EditClasses.EditorState.SplineOptionsGroup[splineID].SplineSize;

            if (EditClasses.EditorState.SplineOptionsGroup[splineID].SplineObjectRenderingTemplate != null)
                Editor.Instance.EditorToolbar.SplineRenderObjectName.Content = EditClasses.EditorState.SplineOptionsGroup[splineID].SplineObjectRenderingTemplate.Entity.Object.Name.Name;
            else
                Editor.Instance.EditorToolbar.SplineRenderObjectName.Content = "None";
        }

        public void UpdateSplineToolbox()
        {
            //Editor.Instance.SplineInfoLabel1.Text = string.Format("Number of Spline Objects: {0}", Editor.Instance.UIModes.SplineTotalNumberOfObjects);
            //Editor.Instance.SplineInfoLabel2.Text = string.Format("Point Frequency: {0}", Editor.Instance.UIModes.SplineSize);
            //Editor.Instance.SplineInfoLabel3.Text = string.Format("Total Number of Rendered Points: {0}", Editor.Instance.UIModes.SplineCurrentPointsDrawn);
        }

        public void UpdateCustomColors()
        {
            Editor.Instance.EditorToolbar.CSAC.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(EditClasses.EditorState.CollisionSAColour.A, EditClasses.EditorState.CollisionSAColour.R, EditClasses.EditorState.CollisionSAColour.G, EditClasses.EditorState.CollisionSAColour.B));
            Editor.Instance.EditorToolbar.SSTOC.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(EditClasses.EditorState.CollisionTOColour.A, EditClasses.EditorState.CollisionTOColour.R, EditClasses.EditorState.CollisionTOColour.G, EditClasses.EditorState.CollisionTOColour.B));
            Editor.Instance.EditorToolbar.CSLRDC.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(EditClasses.EditorState.CollisionLRDColour.A, EditClasses.EditorState.CollisionLRDColour.R, EditClasses.EditorState.CollisionLRDColour.G, EditClasses.EditorState.CollisionLRDColour.B));
            Editor.Instance.EditorToolbar.WLC.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(EditClasses.EditorState.waterColor.A, EditClasses.EditorState.waterColor.R, EditClasses.EditorState.waterColor.G, EditClasses.EditorState.waterColor.B));
            Editor.Instance.EditorToolbar.GDC.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(EditClasses.EditorState.GridColor.A, EditClasses.EditorState.GridColor.R, EditClasses.EditorState.GridColor.G, EditClasses.EditorState.GridColor.B));
        }

        public void UpdateControls(bool stageLoad = false)
        {
            if (Settings.MySettings.EntityFreeCam)
            {
                Editor.Instance.FormsModel.vScrollBar1.IsEnabled = false;
                Editor.Instance.FormsModel.hScrollBar1.IsEnabled = false;
            }
            else
            {
                Editor.Instance.FormsModel.vScrollBar1.IsEnabled = true;
                Editor.Instance.FormsModel.hScrollBar1.IsEnabled = true;
            }

            bool parallaxAnimationInProgress = EditClasses.EditorState.AllowAnimations && EditClasses.EditorState.ParallaxAnimationChecked;

            Editor.Instance.EditorToolbar.UpdateGameRunningButton(CurrentSolution.CurrentScene != null);
            Editor.Instance.Theming.UpdateThemeForItemsWaiting();
            Editor.Instance.EditorStatusBar.UpdateFilterButtonApperance(false);
            Editor.Instance.EditorStatusBar.UpdateStatusPanel();
            SetSceneOnlyButtonsState(CurrentSolution.CurrentScene != null && !parallaxAnimationInProgress, stageLoad);
            SetParallaxAnimationOnlyButtonsState(parallaxAnimationInProgress);
            UpdateSplineToolbox();
            Editor.Instance.EditorToolbar.CustomGridLabel.Text = string.Format(Editor.Instance.EditorToolbar.CustomGridLabel.Tag.ToString(), Properties.Defaults.Default.CustomGridSizeValue);

        }

        private void UpdateTooltips()
        {
            UpdateTooltipForStacks(Editor.Instance.EditorToolbar.UndoButton, Editor.Instance.UndoStack);
            UpdateTooltipForStacks(Editor.Instance.EditorToolbar.RedoButton, Editor.Instance.RedoStack);
            UpdateTextBlockForStacks(Editor.Instance.EditorMenuBar.UndoMenuItemInfo, Editor.Instance.UndoStack);
            UpdateTextBlockForStacks(Editor.Instance.EditorMenuBar.RedoMenuItemInfo, Editor.Instance.RedoStack);
            if (Editor.Instance.EditorControls != null)
            {
                if (Editor.Instance.IsVisible)
                {
                    Editor.Instance.EditorMenuBar.UpdateMenuItems();
                    Editor.Instance.EditorControls.UpdateTooltips();
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
                Editor.Instance.DisposeTextures();
                Editor.Instance.EntityDrawing.ReleaseResources();
                //EditorEntity_ini.rendersWithErrors.Clear();

                //Reload for Encore Palletes, otherwise reload the image normally
                if (EditClasses.EditorState.UseEncoreColors == true)
                {
                    CurrentSolution.CurrentTiles.StageTiles?.Image.Reload(Editor.Instance.EncorePalette[0]);
                    Editor.Instance.TilesToolbar?.Reload(Editor.Instance.EncorePalette[0]);
                }
                else
                {
                    CurrentSolution.CurrentTiles.StageTiles?.Image.Reload();
                    Editor.Instance.TilesToolbar?.Reload();
                }

                CurrentSolution.TileConfig = new Tileconfig(Editor.Instance.Paths.TileConfig_Source);



            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        #endregion
    }
}
