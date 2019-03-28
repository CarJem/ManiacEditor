using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using RSDKv5;
using ManiacEditor.Actions;
using System.Windows.Controls;

namespace ManiacEditor
{
    public class EditorUI
    {
        private Editor Editor;
        public EditorUI(Editor instance)
        {
            Editor = instance;
        }



        #region Enable And Disable Editor Buttons
        public void SetSceneOnlyButtonsState(bool enabled, bool stageLoad = false)
        {
            Editor.saveToolStripMenuItem.IsEnabled = enabled;
            Editor.saveAsToolStripMenuItem.IsEnabled = enabled;
            Editor.backupToolStripMenuItem.IsEnabled = enabled;
            Editor.unloadSceneToolStripMenuItem.IsEnabled = enabled;
            Editor.goToToolStripMenuItem1.IsEnabled = enabled;
            Editor.specificPlaceToolStripMenuItem.IsEnabled = enabled;
            Editor.playerSpawnToolStripMenuItem.IsEnabled = enabled;
            Editor.findUnusedTilesToolStripMenuItem.IsEnabled = enabled;
            Editor.maniacinieditorToolStripMenuItem.IsEnabled = enabled;

            Editor.ShowFGHigh.IsEnabled = enabled && Editor.FGHigh != null;
            Editor.ShowFGLow.IsEnabled = enabled && Editor.FGLow != null;
            Editor.ShowFGHigher.IsEnabled = enabled && Editor.FGHigher != null;
            Editor.ShowFGLower.IsEnabled = enabled && Editor.FGLower != null;
            Editor.ShowEntities.IsEnabled = enabled;
            Editor.ShowAnimations.IsEnabled = enabled;
            Editor.animationsSplitButton_Dropdown.IsEnabled = enabled;
            Editor.ReloadButton.IsEnabled = enabled;
            Editor.newShortcutToolStripMenuItem.IsEnabled = System.IO.Directory.Exists(Editor.DataDirectory);
            Editor.withoutCurrentCoordinatesToolStripMenuItem.IsEnabled = Editor.EditorScene != null;
            Editor.withCurrentCoordinatesToolStripMenuItem.IsEnabled = Editor.EditorScene != null;
            Editor.changeEncorePaleteToolStripMenuItem.IsEnabled = enabled;
            Editor.entityEditOptionsHost.IsEnabled = enabled;
            Editor.EntitiesViewOptionsHost.IsEnabled = enabled;
            Editor.layerEditOptionsHost.IsEnabled = enabled;
            Editor.LayerViewOptionsHost.IsEnabled = enabled;


            Editor.Save.IsEnabled = enabled;

            if (Settings.MyPerformance.ReduceZoom)
            {
                Editor.ZoomInButton.IsEnabled = enabled && Editor.StateModel.ZoomLevel < 5;
                Editor.ZoomOutButton.IsEnabled = enabled && Editor.StateModel.ZoomLevel > -2;
            }
            else
            {
                Editor.ZoomInButton.IsEnabled = enabled && Editor.StateModel.ZoomLevel < 5;
                Editor.ZoomOutButton.IsEnabled = enabled && Editor.StateModel.ZoomLevel > -5;
            }


            UpdateGameRunningButton(enabled);

            SetEditButtonsState(enabled);
            UpdateTooltips();

            if (stageLoad)
            {
                Editor.ZoomModel.SetViewSize((int)(Editor.SceneWidth * Editor.StateModel.Zoom), (int)(Editor.SceneHeight * Editor.StateModel.Zoom));
            }

            Editor.Theming.UpdateButtonColors();

        }
        public void SetSelectOnlyButtonsState(bool enabled = true)
        {
            SetPasteButtonsState(true);
            enabled &= Editor.IsSelected();
            Editor.deleteToolStripMenuItem.IsEnabled = enabled;
            Editor.copyToolStripMenuItem.IsEnabled = enabled;
            Editor.cutToolStripMenuItem.IsEnabled = enabled;
            Editor.duplicateToolStripMenuItem.IsEnabled = enabled;


            Editor.flipHorizontalToolStripMenuItem.IsEnabled = enabled && Editor.IsTilesEdit();
            Editor.flipVerticalToolStripMenuItem.IsEnabled = enabled && Editor.IsTilesEdit();
            Editor.flipHorizontalIndvidualToolStripMenuItem.IsEnabled = enabled && Editor.IsTilesEdit();
            Editor.flipVerticalIndvidualToolStripMenuItem.IsEnabled = enabled && Editor.IsTilesEdit();

            Editor.selectAllToolStripMenuItem.IsEnabled = (Editor.IsTilesEdit() && !Editor.IsChunksEdit()) || Editor.IsEntitiesEdit();

            if (Editor.IsEntitiesEdit() && Editor.EntitiesToolbar != null)
            {
                Editor.EntitiesToolbar.SelectedEntities = Editor.Entities.SelectedEntities.Select(x => x.Entity).ToList();
            }
        }

        public void SetPasteButtonsState(bool enabled)
        {
            bool windowsClipboardState;
            bool windowsEntityClipboardState;
            //Doing this too often seems to cause a lot of grief for the app, should be relocated and stored as a bool
            try
            {
                if (Editor.IsTilesEdit()) windowsClipboardState = Clipboard.ContainsData("ManiacTiles");
                else windowsClipboardState = false;
                if (Editor.IsEntitiesEdit()) windowsEntityClipboardState = Clipboard.ContainsData("ManiacEntities");
                else windowsEntityClipboardState = false;
            }
            catch
            {
                windowsClipboardState = false;
                windowsEntityClipboardState = false;
            }


            if (Editor.IsTilesEdit())
            {
                if (enabled && HasCopyDataTiles()) SetPasteEnabledButtons(true);
                else SetPasteEnabledButtons(false);
            }
            else if (Editor.IsEntitiesEdit())
            {
                if (enabled && HasCopyDataEntities()) SetPasteEnabledButtons(true);
                else SetPasteEnabledButtons(false);
            }
            else
            {
                SetPasteEnabledButtons(false);
            }

            void SetPasteEnabledButtons(bool pasteEnabled)
            {
                Editor.pasteToolStripMenuItem.IsEnabled = pasteEnabled;
                Editor.pasteToToolStripMenuItem.IsEnabled = pasteEnabled;
                Editor.pasteTochunkToolStripMenuItem.IsEnabled = pasteEnabled && Editor.IsTilesEdit();
            }

            bool HasCopyDataTiles() { return Editor.TilesClipboard != null || windowsClipboardState == true; }
            bool HasCopyDataEntities() { return Editor.entitiesClipboard != null || windowsEntityClipboardState == true; }
        }
        private void SetLayerEditButtonsState(bool enabled)
        {
            if (!Editor.UIModes.MultiLayerEditMode)
            {
                if (enabled && Editor.EditFGLow.IsCheckedN.Value) Editor.EditLayerA = Editor.FGLow;
                else if (enabled && Editor.EditFGHigh.IsCheckedN.Value) Editor.EditLayerA = Editor.FGHigh;
                else if (enabled && Editor.EditFGHigher.IsCheckedN.Value) Editor.EditLayerA = Editor.FGHigher;
                else if (enabled && Editor.EditFGLower.IsCheckedN.Value) Editor.EditLayerA = Editor.FGLower;
                else if (enabled && Editor.ExtraLayerEditViewButtons.Any(elb => elb.Value.IsCheckedN.Value))
                {
                    var selectedExtraLayerButton = Editor.ExtraLayerEditViewButtons.Single(elb => elb.Value.IsCheckedN.Value);
                    var editorLayer = Editor.EditorScene.OtherLayers.Single(el => el.Name.Equals(selectedExtraLayerButton.Value.Text));

                    Editor.EditLayerA = editorLayer;
                }
                else Editor.EditLayerA = null;
            }
            else
            {
                SetEditLayerA();
                SetEditLayerB();
            }

            void SetEditLayerA()
            {
                if (enabled && Editor.EditFGLow.IsCheckedA.Value) Editor.EditLayerA = Editor.FGLow;
                else if (enabled && Editor.EditFGHigh.IsCheckedA.Value) Editor.EditLayerA = Editor.FGHigh;
                else if (enabled && Editor.EditFGHigher.IsCheckedA.Value) Editor.EditLayerA = Editor.FGHigher;
                else if (enabled && Editor.EditFGLower.IsCheckedA.Value) Editor.EditLayerA = Editor.FGLower;
                else if (enabled && Editor.ExtraLayerEditViewButtons.Any(elb => elb.Value.IsCheckedA.Value))
                {
                    var selectedExtraLayerButton = Editor.ExtraLayerEditViewButtons.Single(elb => elb.Value.IsCheckedA.Value);
                    var editorLayer = Editor.EditorScene.OtherLayers.Single(el => el.Name.Equals(selectedExtraLayerButton.Value.Text));

                    Editor.EditLayerA = editorLayer;
                }
                else Editor.EditLayerA = null;
            }
            void SetEditLayerB()
            {
                if (enabled && Editor.EditFGLow.IsCheckedB.Value) Editor.EditLayerB = Editor.FGLow;
                else if (enabled && Editor.EditFGHigh.IsCheckedB.Value) Editor.EditLayerB = Editor.FGHigh;
                else if (enabled && Editor.EditFGHigher.IsCheckedB.Value) Editor.EditLayerB = Editor.FGHigher;
                else if (enabled && Editor.EditFGLower.IsCheckedB.Value) Editor.EditLayerB = Editor.FGLower;
                else if (enabled && Editor.ExtraLayerEditViewButtons.Any(elb => elb.Value.IsCheckedB.Value))
                {
                    var selectedExtraLayerButton = Editor.ExtraLayerEditViewButtons.Single(elb => elb.Value.IsCheckedB.Value);
                    var editorLayer = Editor.EditorScene.OtherLayers.Single(el => el.Name.Equals(selectedExtraLayerButton.Value.Text));

                    Editor.EditLayerB = editorLayer;
                }
                else Editor.EditLayerB = null;
            }

        }
        private void SetEditButtonsState(bool enabled)
        {

            Editor.EditFGLow.IsEnabled = enabled && Editor.FGLow != null;
            Editor.EditFGHigh.IsEnabled = enabled && Editor.FGHigh != null;
            Editor.EditFGLower.IsEnabled = enabled && Editor.FGLower != null;
            Editor.EditFGHigher.IsEnabled = enabled && Editor.FGHigher != null;
            Editor.EditEntities.IsEnabled = enabled;
            Editor.entityManagerToolStripMenuItem.IsEnabled = enabled && Editor.StageConfig != null;
            Editor.importSoundsToolStripMenuItem.IsEnabled = enabled && Editor.StageConfig != null;
            Editor.layerManagerToolStripMenuItem.IsEnabled = enabled;
            Editor.editBackgroundColorsToolStripMenuItem.IsEnabled = enabled;

            SetLayerEditButtonsState(enabled);

            Editor.undoToolStripMenuItem.IsEnabled = enabled && Editor.UndoStack.Count > 0;
            Editor.redoToolStripMenuItem.IsEnabled = enabled && Editor.RedoStack.Count > 0;

            Editor.MagnetMode.IsEnabled = enabled && Editor.IsEntitiesEdit();
            Editor.MagnetMode.IsChecked = Editor.UIModes.UseMagnetMode && Editor.IsEntitiesEdit();
            Editor.MagnetModeSplitButton.IsEnabled = enabled && Editor.IsEntitiesEdit();
            Editor.UIModes.UseMagnetMode = Editor.IsEntitiesEdit() && Editor.MagnetMode.IsChecked.Value;



            Editor.UndoButton.IsEnabled = enabled && Editor.UndoStack.Count > 0;
            Editor.RedoButton.IsEnabled = enabled && Editor.RedoStack.Count > 0;

            Editor.findAndReplaceToolStripMenuItem.IsEnabled = enabled && Editor.EditLayerA != null;

            Editor.PointerToolButton.IsEnabled = enabled && Editor.IsTilesEdit();
            Editor.SelectToolButton.IsEnabled = enabled && Editor.IsTilesEdit();
            Editor.PlaceTilesButton.IsEnabled = enabled && Editor.IsTilesEdit();
            Editor.InteractionToolButton.IsEnabled = enabled;
            Editor.ChunksToolButton.IsEnabled = enabled && Editor.IsTilesEdit();

            Editor.PointerToolButton.IsChecked = (bool)Editor.PointerToolButton.IsChecked || (!(bool)Editor.PointerToolButton.IsChecked && !(bool)Editor.SelectToolButton.IsChecked && !(bool)Editor.PlaceTilesButton.IsChecked);
            Editor.PlaceTilesButton.IsChecked = Editor.PlaceTilesButton.IsChecked;
            Editor.InteractionToolButton.IsChecked = Editor.InteractionToolButton.IsChecked;
            Editor.ChunksToolButton.IsChecked = (bool)Editor.ChunksToolButton.IsChecked && !Editor.IsEntitiesEdit();
            if (Editor.TilesToolbar != null)
            {
                if (Editor.ChunksToolButton.IsChecked.Value)
                {
                    Editor.TilesToolbar.TabControl.SelectedIndex = 1;
                }
                else
                {
                    Editor.TilesToolbar.TabControl.SelectedIndex = 0;
                }
            }



            Editor.ShowGridButton.IsEnabled = enabled && Editor.StageConfig != null;
            Editor.ShowCollisionAButton.IsEnabled = enabled && Editor.TileConfig != null;
            Editor.ShowCollisionBButton.IsEnabled = enabled && Editor.TileConfig != null;
            Editor.ShowTileIDButton.IsEnabled = enabled && Editor.StageConfig != null;
            Editor.GridSizeButton.IsEnabled = enabled && Editor.StageConfig != null;
            Editor.EncorePaletteButton.IsEnabled = enabled && Editor.UIModes.EncorePaletteExists;
            Editor.FlipAssistButton.IsEnabled = enabled;

            if (Editor.IsTilesEdit())
            {
                if (Editor.TilesToolbar == null)
                {
                    if (Editor.UIModes.UseEncoreColors)
                        Editor.TilesToolbar = new TilesToolbar(Editor.EditorTiles.StageTiles, Editor.Paths.StageTiles_Source, Editor.EncorePalette[0], Editor);
                    else
                        Editor.TilesToolbar = new TilesToolbar(Editor.EditorTiles.StageTiles, Editor.Paths.StageTiles_Source, null, Editor);


                    Editor.TilesToolbar.TileDoubleClick = new Action<int>(x =>
                    {
                        Editor.EditorPlaceTile(new System.Drawing.Point((int)(Editor.StateModel.ShiftX / Editor.StateModel.Zoom) + EditorConstants.TILE_SIZE - 1, (int)(Editor.StateModel.ShiftY / Editor.StateModel.Zoom) + EditorConstants.TILE_SIZE - 1), x, Editor.EditLayerA);
                    });
                    Editor.TilesToolbar.TileOptionChanged = new Action<int, bool>((option, state) =>
                    {
                        Editor.EditLayerA?.SetPropertySelected(option + 12, state);
                        Editor.EditLayerB?.SetPropertySelected(option + 12, state);

                    });
                    Editor.ToolBarPanelRight.Children.Clear();
                    Editor.ToolBarPanelRight.Children.Add(Editor.TilesToolbar);
                    UpdateToolbars(true, true);
                    Editor.Editor_Resize(null, null);
                    Editor.Focus();
                }
                if (Editor.IsChunksEdit()) Editor.TilesToolbar.TabControl.TabIndex = 1;
                else Editor.TilesToolbar.TabControl.TabIndex = 0;
                Editor.UI.UpdateTilesOptions();
                Editor.TilesToolbar.ShowShortcuts = Editor.PlaceTilesButton.IsChecked.Value;
            }
            else
            {
                if (Editor.TilesToolbar != null)
                {
                    Editor.TilesToolbar.Dispose();
                    Editor.TilesToolbar = null;
                    Editor.Focus();
                }
            }
            if (Editor.IsEntitiesEdit())
            {
                if (Editor.EntitiesToolbar == null)
                {
                    Editor.EntitiesToolbar = new EntitiesToolbar(Editor.EditorScene.Objects, Editor)
                    {
                        SelectedEntity = new Action<int>(x =>
                        {
                            Editor.Entities.SelectSlot(x);
                            SetSelectOnlyButtonsState();
                        }),
                        AddAction = new Action<ManiacEditor.Actions.IAction>(x =>
                        {
                            Editor.UndoStack.Push(x);
                            Editor.RedoStack.Clear();
                            UpdateControls();
                        }),
                        Spawn = new Action<SceneObject>(x =>
                        {
                            Editor.Entities.Add(x, new Position((short)(Editor.StateModel.ShiftX / Editor.StateModel.Zoom), (short)(Editor.StateModel.ShiftY / Editor.StateModel.Zoom)));
                            Editor.UndoStack.Push(Editor.Entities.LastAction);
                            Editor.RedoStack.Clear();
                            UpdateControls();
                        })
                    };
                    Editor.ToolBarPanelRight.Children.Clear();
                    Editor.ToolBarPanelRight.Children.Add(Editor.EntitiesToolbar);
                    UpdateToolbars(true, true);
                    Editor.Editor_Resize(null, null);
                }
                Editor.UI.UpdateEntitiesToolbarList();
                Editor.EntitiesToolbar.SelectedEntities = Editor.Entities.SelectedEntities.Select(x => x.Entity).ToList();
            }
            else
            {
                if (Editor.EntitiesToolbar != null)
                {
                    Editor.EntitiesToolbar.Dispose();
                    Editor.EntitiesToolbar = null;
                }
                if (Editor.Entities != null && Editor.Entities.SelectedEntities != null)
                {
                    if (Editor.Entities.SelectedEntities.Count != 0 && Editor.Entities.tempSelection.Count != 0)
                    {
                        Editor.Entities.EndTempSelection();
                        Editor.Entities.Deselect();
                    }
                }


            }
            if (Editor.TilesToolbar == null && Editor.EntitiesToolbar == null && (Editor.ToolBarPanelRight.Children.Count != 0))
            {
                Editor.ToolBarPanelRight.Children.Clear();
                UpdateToolbars(true, false);
                Editor.Editor_Resize(null, null);
            }

            SetSelectOnlyButtonsState(enabled);
        }


        #endregion

        #region Updating Elements Methods
        public void ToggleEditorButtons(bool enabled)
        {
            Editor.MenuBar.IsEnabled = enabled;
            Editor.LayerToolbar.IsEnabled = enabled;
            Editor.MainToolbarButtons.IsEnabled = enabled;
            Editor.UI.SetSceneOnlyButtonsState((enabled ? true : Editor.EditorScene != null));
            Editor.LayerToolbar.IsEnabled = enabled;
            Editor.StatusBar1.IsEnabled = enabled;
            Editor.StatusBar2.IsEnabled = enabled;
            if (Editor.TilesToolbar != null) Editor.TilesToolbar.IsEnabled = enabled;
            if (Editor.EntitiesToolbar != null) Editor.EntitiesToolbar.IsEnabled = enabled;
        }
        public void UpdateStatusPanel()
        {
            //
            // Tooltip Bar Info 
            //

            Editor._levelIDLabel.Content = "Level ID: " + Editor.UIModes.LevelID.ToString();
            Editor.seperator1.Visibility = Visibility.Visible;
            Editor.seperator2.Visibility = Visibility.Visible;
            Editor.seperator3.Visibility = Visibility.Visible;
            Editor.seperator4.Visibility = Visibility.Visible;
            Editor.seperator5.Visibility = Visibility.Visible;
            Editor.seperator6.Visibility = Visibility.Visible;
            Editor.seperator7.Visibility = Visibility.Visible;
            //seperator8.Visibility = Visibility.Visible;
            //seperator9.Visibility = Visibility.Visible;

            if (Editor.UIModes.EnablePixelCountMode == false)
            {
                Editor.selectedPositionLabel.Content = "Selected Tile Position: X: " + (int)Editor.StateModel.SelectedTileX + ", Y: " + (int)Editor.StateModel.SelectedTileY;
                Editor.selectedPositionLabel.ToolTip = "The Position of the Selected Tile";
            }
            else
            {
                Editor.selectedPositionLabel.Content = "Selected Tile Pixel Position: " + "X: " + (int)Editor.StateModel.SelectedTileX * 16 + ", Y: " + (int)Editor.StateModel.SelectedTileY * 16;
                Editor.selectedPositionLabel.ToolTip = "The Pixel Position of the Selected Tile";
            }
            if (Editor.UIModes.EnablePixelCountMode == false)
            {
                Editor.selectionSizeLabel.Content = "Amount of Tiles in Selection: " + (Editor.StateModel.SelectedTilesCount - Editor.StateModel.DeselectTilesCount);
                Editor.selectionSizeLabel.ToolTip = "The Size of the Selection";
            }
            else
            {
                Editor.selectionSizeLabel.Content = "Length of Pixels in Selection: " + (Editor.StateModel.SelectedTilesCount - Editor.StateModel.DeselectTilesCount) * 16;
                Editor.selectionSizeLabel.ToolTip = "The Length of all the Tiles (by Pixels) in the Selection";
            }

            Editor.selectionBoxSizeLabel.Content = "Selection Box Size: X: " + (Editor.StateModel.select_x2 - Editor.StateModel.select_x1) + ", Y: " + (Editor.StateModel.select_y2 - Editor.StateModel.select_y1);

            Editor.scrollLockDirLabel.Content = "Scroll Direction: " + (Editor.UIModes.ScrollDirection == (int)ScrollDir.X ? "X" : "Y") + (Editor.UIModes.ScrollLocked ? " (Locked)" : "");


            Editor.hVScrollBarXYLabel.Content = "Zoom Value: " + Editor.StateModel.Zoom.ToString();

            if (EditorUIModes.UpdateUpdaterMessage)
            {
                if (Editor.StartScreen != null) Editor.StartScreen.UpdateStatusLabel(Editor.Updater.condition, Editor.Updater);
                EditorUIModes.UpdateUpdaterMessage = false;
            }

            //
            // End of Tooltip Bar Info Section
            //
        }
        public void UpdateTilesOptions()
        {
            if (Editor.IsTilesEdit() && !Editor.IsChunksEdit())
            {
                if (Editor.TilesToolbar != null)
                {
                    List<ushort> values = Editor.EditLayerA?.GetSelectedValues();
                    List<ushort> valuesB = Editor.EditLayerB?.GetSelectedValues();
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
                            Editor.TilesToolbar.SetTileOptionState(i, unk ? TilesToolbar.TileOptionState.Indeterminate : set ? TilesToolbar.TileOptionState.Checked : TilesToolbar.TileOptionState.Unchcked);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < 4; ++i)
                            Editor.TilesToolbar.SetTileOptionState(i, TilesToolbar.TileOptionState.Disabled);
                    }
                }

            }
        }
        public void UpdateEntitiesToolbarList()
        {
            Editor.EntitiesToolbar.Entities = Editor.Entities.Entities.Select(x => x.Entity).ToList();
        }
        public void UpdateEditLayerActions()
        {
            bool pushActions = false;
            if (Editor.EditLayerA != null)
            {
                List<IAction> actions = Editor.EditLayerA?.Actions;
                if (actions.Count > 0) Editor.RedoStack.Clear();
                while (actions.Count > 0)
                {
                    bool create_new = false;
                    if (Editor.UndoStack.Count == 0 || !(Editor.UndoStack.Peek() is ActionsGroup))
                    {
                        create_new = true;
                    }
                    else
                    {
                        create_new = (Editor.UndoStack.Peek() as ActionsGroup).IsClosed;
                    }
                    if (create_new)
                    {
                        Editor.UndoStack.Push(new ActionsGroup());
                    }
                    (Editor.UndoStack.Peek() as ActionsGroup).AddAction(actions[0]);
                    actions.RemoveAt(0);
                }
            }
            if (Editor.EditLayerB != null)
            {
                List<IAction> actions = Editor.EditLayerB?.Actions;
                if (actions.Count > 0) Editor.RedoStack.Clear();
                while (actions.Count > 0)
                {
                    bool create_new = false;
                    if (Editor.UndoStack.Count == 0 || !(Editor.UndoStack.Peek() is ActionsGroup))
                    {
                        create_new = true;
                    }
                    else
                    {
                        create_new = (Editor.UndoStack.Peek() as ActionsGroup).IsClosed;
                    }
                    if (create_new)
                    {
                        Editor.UndoStack.Push(new ActionsGroup());
                    }
                    (Editor.UndoStack.Peek() as ActionsGroup).AddAction(actions[0]);
                    actions.RemoveAt(0);
                }
            }
        }
        public void UpdateToolbars(bool rightToolbar = true, bool visible = false)
        {
            if (rightToolbar)
            {
                if (visible)
                {
                    Editor.ToolbarRight.Width = new GridLength(300);
                    Editor.ToolbarRight.MinWidth = 300;
                    Editor.ToolbarRight.MaxWidth = Editor.ViewPanelForm.ActualWidth / 3;
                    Editor.SplitterRight.Width = new GridLength(6);
                    Editor.SplitterRight.MinWidth = 6;
                }
                else
                {
                    Editor.ToolbarRight.Width = new GridLength(0);
                    Editor.ToolbarRight.MinWidth = 0;
                    Editor.ToolbarRight.MaxWidth = 0;
                    Editor.SplitterRight.Width = new GridLength(0);
                    Editor.SplitterRight.MinWidth = 0;
                }
            }

            else
            {
                if (visible)
                {
                    if (Editor.ToolbarLeft.Width.Value == 0)
                    {
                        Editor.ToolbarLeft.Width = new GridLength(300);
                        Editor.ToolbarLeft.MinWidth = 300;
                        Editor.ToolbarLeft.MaxWidth = Editor.ViewPanelForm.ActualWidth / 3;
                        Editor.SplitterLeft.Width = new GridLength(6);
                        Editor.SplitterLeft.MinWidth = 6;
                    }
                }
                else
                {
                    Editor.ToolbarLeft.Width = new GridLength(0);
                    Editor.ToolbarLeft.MinWidth = 0;
                    Editor.ToolbarLeft.MaxWidth = 0;
                    Editor.SplitterLeft.Width = new GridLength(0);
                    Editor.SplitterLeft.MinWidth = 0;
                }
            }

        }
        public void UpdateWaitingScreen(bool show)
        {
            if (show)
            {
                Editor.ViewPanelForm.Visibility = Visibility.Hidden;
                Editor.WaitingPanel.Visibility = Visibility.Visible;
            }
            else
            {
                Editor.ViewPanelForm.Visibility = Visibility.Visible;
                Editor.WaitingPanel.Visibility = Visibility.Collapsed;
            }

        }
        public void UpdateControls(bool stageLoad = false)
        {
            if (Settings.MySettings.EntityFreeCam)
            {
                Editor.FormsModel.vScrollBar1.IsEnabled = false;
                Editor.FormsModel.hScrollBar1.IsEnabled = false;
            }
            else
            {
                Editor.FormsModel.vScrollBar1.IsEnabled = true;
                Editor.FormsModel.hScrollBar1.IsEnabled = true;
            }

            UpdateStatusPanel();
            SetSceneOnlyButtonsState(Editor.EditorScene != null, stageLoad);
        }
        public void UpdateGameRunningButton(bool enabled = true)
        {
            Editor.RunSceneButton.IsEnabled = enabled;
            Editor.RunSceneDropDown.IsEnabled = enabled;

            if (Editor.InGame.GameRunning || System.Diagnostics.Process.GetProcessesByName("SonicMania").FirstOrDefault() != null)
            {
                if (Editor.InGame.GameRunning) Editor.Theming.SetButtonColors(Editor.RunSceneButton, System.Drawing.Color.Blue);
                else Editor.Theming.SetButtonColors(Editor.RunSceneButton, System.Drawing.Color.Green);
            }
            else
            {
                Editor.Theming.SetButtonColors(Editor.RunSceneButton, System.Drawing.Color.Gray);
            }
        }
        private void UpdateTooltips()
        {
            UpdateTooltipForStacks(Editor.UndoButton, Editor.UndoStack);
            UpdateTooltipForStacks(Editor.RedoButton, Editor.RedoStack);
            UpdateTextBlockForStacks(Editor.UndoMenuItemInfo, Editor.UndoStack);
            UpdateTextBlockForStacks(Editor.RedoMenuItemInfo, Editor.RedoStack);
            if (Editor.UIControl != null)
            {
                if (Editor.IsVisible)
                {
                    Editor.UIControl.UpdateMenuItems();
                    Editor.UIControl.UpdateTooltips();
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
                Editor.DisposeTextures();
                Editor.EntityDrawing.ReleaseResources();
                //EditorEntity_ini.rendersWithErrors.Clear();

                //Reload for Encore Palletes, otherwise reload the image normally
                if (Editor.UIModes.UseEncoreColors == true)
                {
                    Editor.EditorTiles.StageTiles?.Image.Reload(Editor.EncorePalette[0]);
                    Editor.TilesToolbar?.Reload(Editor.EncorePalette[0]);
                }
                else
                {
                    Editor.EditorTiles.StageTiles?.Image.Reload();
                    Editor.TilesToolbar?.Reload();
                }

                Editor.TileConfig = new TileConfig(Editor.Paths.TileConfig_Source);

                Editor.CollisionLayerA.Clear();
                Editor.CollisionLayerB.Clear();

                for (int i = 0; i < 1024; i++)
                {
                    Editor.CollisionLayerA.Add(Editor.TileConfig.CollisionPath1[i].DrawCMask(System.Drawing.Color.FromArgb(0, 0, 0, 0), Editor.CollisionAllSolid));
                    Editor.CollisionLayerB.Add(Editor.TileConfig.CollisionPath2[i].DrawCMask(System.Drawing.Color.FromArgb(0, 0, 0, 0), Editor.CollisionAllSolid));
                }



            }
            catch (Exception ex)
            {
                RSDKrU.MessageBox.Show(ex.Message);
            }
        }

        #endregion
    }
}
