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
        public EditorUI()
        {

        }



        #region Enable And Disable Editor Buttons
        public void SetSceneOnlyButtonsState(bool enabled, bool stageLoad = false)
        {
            Editor.Instance.saveToolStripMenuItem.IsEnabled = enabled;
            Editor.Instance.saveAsToolStripMenuItem.IsEnabled = enabled;
            Editor.Instance.backupToolStripMenuItem.IsEnabled = enabled;
            Editor.Instance.unloadSceneToolStripMenuItem.IsEnabled = enabled;
            Editor.Instance.goToToolStripMenuItem1.IsEnabled = enabled;
            Editor.Instance.specificPlaceToolStripMenuItem.IsEnabled = enabled;
            Editor.Instance.playerSpawnToolStripMenuItem.IsEnabled = enabled;
            Editor.Instance.findUnusedTilesToolStripMenuItem.IsEnabled = enabled;
            Editor.Instance.maniacinieditorToolStripMenuItem.IsEnabled = enabled;

            Editor.Instance.ShowFGHigh.IsEnabled = enabled && Editor.Instance.FGHigh != null;
            Editor.Instance.ShowFGLow.IsEnabled = enabled && Editor.Instance.FGLow != null;
            Editor.Instance.ShowFGHigher.IsEnabled = enabled && Editor.Instance.FGHigher != null;
            Editor.Instance.ShowFGLower.IsEnabled = enabled && Editor.Instance.FGLower != null;
            Editor.Instance.ShowEntities.IsEnabled = enabled;

            Editor.Instance.ReloadButton.IsEnabled = enabled;
            Editor.Instance.newShortcutToolStripMenuItem.IsEnabled = System.IO.Directory.Exists(Editor.Instance.DataDirectory);
            Editor.Instance.withoutCurrentCoordinatesToolStripMenuItem.IsEnabled = Editor.Instance.EditorScene != null;
            Editor.Instance.withCurrentCoordinatesToolStripMenuItem.IsEnabled = Editor.Instance.EditorScene != null;
            Editor.Instance.changeEncorePaleteToolStripMenuItem.IsEnabled = enabled;
            Editor.Instance.entityEditOptionsHost.IsEnabled = enabled;
            Editor.Instance.EntitiesViewOptionsHost.IsEnabled = enabled;
            Editor.Instance.layerEditOptionsHost.IsEnabled = enabled;
            Editor.Instance.LayerViewOptionsHost.IsEnabled = enabled;
            Editor.Instance.GridSizeButton.IsEnabled = enabled && Editor.Instance.EditorScene != null;


            Editor.Instance.Save.IsEnabled = enabled;

            if (Settings.MyPerformance.ReduceZoom)
            {
                Editor.Instance.ZoomInButton.IsEnabled = enabled && Editor.Instance.StateModel.ZoomLevel < 5;
                Editor.Instance.ZoomOutButton.IsEnabled = enabled && Editor.Instance.StateModel.ZoomLevel > -2;
            }
            else
            {
                Editor.Instance.ZoomInButton.IsEnabled = enabled && Editor.Instance.StateModel.ZoomLevel < 5;
                Editor.Instance.ZoomOutButton.IsEnabled = enabled && Editor.Instance.StateModel.ZoomLevel > -5;
            }


            UpdateGameRunningButton(enabled);

            SetEditButtonsState(enabled);
            UpdateTooltips();

            if (stageLoad)
            {
                Editor.Instance.ZoomModel.SetViewSize((int)(Editor.Instance.SceneWidth * Editor.Instance.StateModel.Zoom), (int)(Editor.Instance.SceneHeight * Editor.Instance.StateModel.Zoom));
            }

            Editor.Instance.Theming.UpdateButtonColors();

        }

        public void SetParallaxAnimationOnlyButtonsState(bool enabled = true)
        {
            Editor.Instance.Open.IsEnabled = !enabled;
            Editor.Instance.ShowAnimations.IsEnabled = enabled || Editor.Instance.EditorScene != null;
            Editor.Instance.animationsSplitButton_Dropdown.IsEnabled = enabled || Editor.Instance.EditorScene != null;
            Editor.Instance.MenuBar.IsEnabled = !enabled;
            Editor.Instance.StatusBar1.IsEnabled = !enabled; 
            Editor.Instance.EditorTabControl.IsEnabled = !enabled;
            Editor.Instance.New.IsEnabled = !enabled;
            Editor.Instance.Open.IsEnabled = !enabled;

            if (enabled)
            {
                Editor.Instance.ShowFGHigh.IsEnabled = Editor.Instance.FGHigh != null;
                Editor.Instance.ShowFGLow.IsEnabled = Editor.Instance.FGLow != null;
                Editor.Instance.ShowFGHigher.IsEnabled = Editor.Instance.FGHigher != null;
                Editor.Instance.ShowFGLower.IsEnabled = Editor.Instance.FGLower != null;
                Editor.Instance.ShowEntities.IsEnabled = true;
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
            SetPasteButtonsState(true);
            enabled &= Editor.Instance.IsSelected();
            Editor.Instance.deleteToolStripMenuItem.IsEnabled = enabled;
            Editor.Instance.copyToolStripMenuItem.IsEnabled = enabled;
            Editor.Instance.cutToolStripMenuItem.IsEnabled = enabled;
            Editor.Instance.duplicateToolStripMenuItem.IsEnabled = enabled;


            Editor.Instance.flipHorizontalToolStripMenuItem.IsEnabled = enabled && CanFlip(0);
            Editor.Instance.flipVerticalToolStripMenuItem.IsEnabled = enabled && CanFlip(0);
            Editor.Instance.flipHorizontalIndvidualToolStripMenuItem.IsEnabled = enabled && CanFlip(1);
            Editor.Instance.flipVerticalIndvidualToolStripMenuItem.IsEnabled = enabled && CanFlip(1);

            Editor.Instance.selectAllToolStripMenuItem.IsEnabled = (Editor.Instance.IsTilesEdit() && !Editor.Instance.IsChunksEdit()) || Editor.Instance.IsEntitiesEdit();

            if (Editor.Instance.IsEntitiesEdit() && Editor.Instance.EntitiesToolbar != null)
            {
                Editor.Instance.EntitiesToolbar.SelectedEntities = Editor.Instance.Entities.SelectedEntities.Select(x => x.Entity).ToList();
            }

            bool CanFlip(int option)
            {
                switch (option)
                {
                    case 0:
                        if (Editor.Instance.IsEntitiesEdit() && Editor.Instance.IsSelected()) return true;
                        else if (Editor.Instance.IsTilesEdit()) return true;
                        break;
                    case 1:
                        return Editor.Instance.IsTilesEdit();
                }
                return false;
            }
        }

        public void SetPasteButtonsState(bool enabled)
        {
            bool windowsClipboardState;
            bool windowsEntityClipboardState;
            //Doing this too often seems to cause a lot of grief for the app, should be relocated and stored as a bool
            try
            {
                if (Editor.Instance.IsTilesEdit()) windowsClipboardState = Clipboard.ContainsData("ManiacTiles");
                else windowsClipboardState = false;
                if (Editor.Instance.IsEntitiesEdit()) windowsEntityClipboardState = Clipboard.ContainsData("ManiacEntities");
                else windowsEntityClipboardState = false;
            }
            catch
            {
                windowsClipboardState = false;
                windowsEntityClipboardState = false;
            }


            if (Editor.Instance.IsTilesEdit())
            {
                if (enabled && HasCopyDataTiles()) SetPasteEnabledButtons(true);
                else SetPasteEnabledButtons(false);
            }
            else if (Editor.Instance.IsEntitiesEdit())
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
                Editor.Instance.pasteToolStripMenuItem.IsEnabled = pasteEnabled;
                Editor.Instance.pasteToToolStripMenuItem.IsEnabled = pasteEnabled;
                Editor.Instance.pasteTochunkToolStripMenuItem.IsEnabled = pasteEnabled && Editor.Instance.IsTilesEdit();
            }

            bool HasCopyDataTiles() { return Editor.Instance.TilesClipboard != null || windowsClipboardState == true; }
            bool HasCopyDataEntities() { return Editor.Instance.entitiesClipboard != null || windowsEntityClipboardState == true; }
        }
        private void SetLayerEditButtonsState(bool enabled)
        {
            if (!Editor.Instance.UIModes.MultiLayerEditMode)
            {
                if (enabled && Editor.Instance.EditFGLow.IsCheckedN.Value) Editor.Instance.EditLayerA = Editor.Instance.FGLow;
                else if (enabled && Editor.Instance.EditFGHigh.IsCheckedN.Value) Editor.Instance.EditLayerA = Editor.Instance.FGHigh;
                else if (enabled && Editor.Instance.EditFGHigher.IsCheckedN.Value) Editor.Instance.EditLayerA = Editor.Instance.FGHigher;
                else if (enabled && Editor.Instance.EditFGLower.IsCheckedN.Value) Editor.Instance.EditLayerA = Editor.Instance.FGLower;
                else if (enabled && Editor.Instance.ExtraLayerEditViewButtons.Any(elb => elb.Value.IsCheckedN.Value))
                {
                    var selectedExtraLayerButton = Editor.Instance.ExtraLayerEditViewButtons.Single(elb => elb.Value.IsCheckedN.Value);
                    var editorLayer = Editor.Instance.EditorScene.OtherLayers.Single(el => el.Name.Equals(selectedExtraLayerButton.Value.Text));

                    Editor.Instance.EditLayerA = editorLayer;
                }
                else Editor.Instance.EditLayerA = null;
            }
            else
            {
                SetEditLayerA();
                SetEditLayerB();
            }

            void SetEditLayerA()
            {
                if (enabled && Editor.Instance.EditFGLow.IsCheckedA.Value) Editor.Instance.EditLayerA = Editor.Instance.FGLow;
                else if (enabled && Editor.Instance.EditFGHigh.IsCheckedA.Value) Editor.Instance.EditLayerA = Editor.Instance.FGHigh;
                else if (enabled && Editor.Instance.EditFGHigher.IsCheckedA.Value) Editor.Instance.EditLayerA = Editor.Instance.FGHigher;
                else if (enabled && Editor.Instance.EditFGLower.IsCheckedA.Value) Editor.Instance.EditLayerA = Editor.Instance.FGLower;
                else if (enabled && Editor.Instance.ExtraLayerEditViewButtons.Any(elb => elb.Value.IsCheckedA.Value))
                {
                    var selectedExtraLayerButton = Editor.Instance.ExtraLayerEditViewButtons.Single(elb => elb.Value.IsCheckedA.Value);
                    var editorLayer = Editor.Instance.EditorScene.OtherLayers.Single(el => el.Name.Equals(selectedExtraLayerButton.Value.Text));

                    Editor.Instance.EditLayerA = editorLayer;
                }
                else Editor.Instance.EditLayerA = null;
            }
            void SetEditLayerB()
            {
                if (enabled && Editor.Instance.EditFGLow.IsCheckedB.Value) Editor.Instance.EditLayerB = Editor.Instance.FGLow;
                else if (enabled && Editor.Instance.EditFGHigh.IsCheckedB.Value) Editor.Instance.EditLayerB = Editor.Instance.FGHigh;
                else if (enabled && Editor.Instance.EditFGHigher.IsCheckedB.Value) Editor.Instance.EditLayerB = Editor.Instance.FGHigher;
                else if (enabled && Editor.Instance.EditFGLower.IsCheckedB.Value) Editor.Instance.EditLayerB = Editor.Instance.FGLower;
                else if (enabled && Editor.Instance.ExtraLayerEditViewButtons.Any(elb => elb.Value.IsCheckedB.Value))
                {
                    var selectedExtraLayerButton = Editor.Instance.ExtraLayerEditViewButtons.Single(elb => elb.Value.IsCheckedB.Value);
                    var editorLayer = Editor.Instance.EditorScene.OtherLayers.Single(el => el.Name.Equals(selectedExtraLayerButton.Value.Text));

                    Editor.Instance.EditLayerB = editorLayer;
                }
                else Editor.Instance.EditLayerB = null;
            }

        }
        private void SetEditButtonsState(bool enabled)
        {

            Editor.Instance.EditFGLow.IsEnabled = enabled && Editor.Instance.FGLow != null;
            Editor.Instance.EditFGHigh.IsEnabled = enabled && Editor.Instance.FGHigh != null;
            Editor.Instance.EditFGLower.IsEnabled = enabled && Editor.Instance.FGLower != null;
            Editor.Instance.EditFGHigher.IsEnabled = enabled && Editor.Instance.FGHigher != null;
            Editor.Instance.EditEntities.IsEnabled = enabled;

            Editor.Instance.EditFGLow.IsCheckedA = enabled && Editor.Instance.EditFGLow.IsCheckedA.Value;
            Editor.Instance.EditFGHigh.IsCheckedA = enabled && Editor.Instance.EditFGHigh.IsCheckedA.Value;
            Editor.Instance.EditFGLower.IsCheckedA = enabled && Editor.Instance.EditFGLower.IsCheckedA.Value;
            Editor.Instance.EditFGHigher.IsCheckedA = enabled && Editor.Instance.EditFGHigher.IsCheckedA.Value;

            Editor.Instance.EditFGLow.IsCheckedB = enabled && Editor.Instance.EditFGLow.IsCheckedB.Value;
            Editor.Instance.EditFGHigh.IsCheckedB = enabled && Editor.Instance.EditFGHigh.IsCheckedB.Value;
            Editor.Instance.EditFGLower.IsCheckedB = enabled && Editor.Instance.EditFGLower.IsCheckedB.Value;
            Editor.Instance.EditFGHigher.IsCheckedB = enabled && Editor.Instance.EditFGHigher.IsCheckedB.Value;

            foreach (var layerButtons in Editor.Instance.ExtraLayerEditViewButtons)
            {
                layerButtons.Value.IsCheckedA = layerButtons.Value.IsCheckedA.Value && enabled;
                layerButtons.Value.IsCheckedB = layerButtons.Value.IsCheckedB.Value && enabled;
            }

            Editor.Instance.EditEntities.IsCheckedN = enabled && Editor.Instance.EditEntities.IsCheckedN.Value;

            Editor.Instance.entityManagerToolStripMenuItem.IsEnabled = enabled && Editor.Instance.StageConfig != null;
            Editor.Instance.importSoundsToolStripMenuItem.IsEnabled = enabled && Editor.Instance.StageConfig != null;
            Editor.Instance.layerManagerToolStripMenuItem.IsEnabled = enabled;
            Editor.Instance.editBackgroundColorsToolStripMenuItem.IsEnabled = enabled;

            SetLayerEditButtonsState(enabled);

            Editor.Instance.undoToolStripMenuItem.IsEnabled = enabled && Editor.Instance.UndoStack.Count > 0;
            Editor.Instance.redoToolStripMenuItem.IsEnabled = enabled && Editor.Instance.RedoStack.Count > 0;

            Editor.Instance.MagnetMode.IsEnabled = enabled && Editor.Instance.IsEntitiesEdit();
            Editor.Instance.MagnetMode.IsChecked = Editor.Instance.UIModes.UseMagnetMode && Editor.Instance.IsEntitiesEdit();
            Editor.Instance.MagnetModeSplitButton.IsEnabled = enabled && Editor.Instance.IsEntitiesEdit();
            Editor.Instance.UIModes.UseMagnetMode = Editor.Instance.IsEntitiesEdit() && Editor.Instance.MagnetMode.IsChecked.Value;



            Editor.Instance.UndoButton.IsEnabled = enabled && Editor.Instance.UndoStack.Count > 0;
            Editor.Instance.RedoButton.IsEnabled = enabled && Editor.Instance.RedoStack.Count > 0;

            Editor.Instance.findAndReplaceToolStripMenuItem.IsEnabled = enabled && Editor.Instance.EditLayerA != null;

            Editor.Instance.PointerToolButton.IsEnabled = enabled;
            Editor.Instance.SelectToolButton.IsEnabled = enabled && Editor.Instance.IsTilesEdit();
            Editor.Instance.DrawToolButton.IsEnabled = enabled && Editor.Instance.IsTilesEdit() || Editor.Instance.IsEntitiesEdit();
            Editor.Instance.InteractionToolButton.IsEnabled = enabled;
            Editor.Instance.ChunksToolButton.IsEnabled = enabled && Editor.Instance.IsTilesEdit();
            Editor.Instance.SplineToolButton.IsEnabled = enabled && Editor.Instance.IsEntitiesEdit();
            Editor.Instance.SplineToolButton.IsChecked = Editor.Instance.SplineToolButton.IsChecked.Value && Editor.Instance.IsEntitiesEdit();

            bool isAnyOtherToolChecked()
            {
                bool isPointer = (bool)Editor.Instance.PointerToolButton.IsChecked.Value;
                bool isSelect = (bool)Editor.Instance.SelectToolButton.IsChecked.Value;
                bool isDraw = (bool)Editor.Instance.DrawToolButton.IsChecked.Value;
                bool isSpline = (bool)Editor.Instance.SplineToolButton.IsChecked.Value;

                if (Editor.Instance.IsEntitiesEdit())
                {
                    if (isDraw || isSpline)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    if (isDraw || isSelect)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }


            Editor.Instance.PointerToolButton.IsChecked = isAnyOtherToolChecked();
            Editor.Instance.ChunksToolButton.IsChecked = (bool)Editor.Instance.ChunksToolButton.IsChecked && !Editor.Instance.IsEntitiesEdit();
            if (Editor.Instance.TilesToolbar != null)
            {
                if (Editor.Instance.ChunksToolButton.IsChecked.Value)
                {
                    Editor.Instance.TilesToolbar.TabControl.SelectedIndex = 1;
                }
                else
                {
                    Editor.Instance.TilesToolbar.TabControl.SelectedIndex = 0;
                }
            }

            if (Editor.Instance.Entities != null && Editor.Instance.Entities.SelectedEntities != null && Editor.Instance.Entities.SelectedEntities.Count > 1)
            {
                Editor.Instance.SortSelectedSlotIDs.IsEnabled = true;
                Editor.Instance.SortSelectedSlotIDsOptimized.IsEnabled = true;
                Editor.Instance.SortSelectedSlotIDsOrdered.IsEnabled = true;
            }
            else
            {
                Editor.Instance.SortSelectedSlotIDs.IsEnabled = false;
                Editor.Instance.SortSelectedSlotIDsOptimized.IsEnabled = false;
                Editor.Instance.SortSelectedSlotIDsOrdered.IsEnabled = false;
            }

            Editor.Instance.ShowGridButton.IsEnabled = enabled && Editor.Instance.StageConfig != null;
            Editor.Instance.ShowCollisionAButton.IsEnabled = enabled && Editor.Instance.TileConfig != null;
            Editor.Instance.ShowCollisionBButton.IsEnabled = enabled && Editor.Instance.TileConfig != null;
            Editor.Instance.ShowTileIDButton.IsEnabled = enabled && Editor.Instance.StageConfig != null;
            Editor.Instance.GridSizeButton.IsEnabled = enabled && Editor.Instance.StageConfig != null;
            Editor.Instance.EncorePaletteButton.IsEnabled = enabled && Editor.Instance.UIModes.EncorePaletteExists;
            Editor.Instance.FlipAssistButton.IsEnabled = enabled;

            if (Editor.Instance.IsTilesEdit())
            {
                if (Editor.Instance.TilesToolbar == null)
                {
                    if (Editor.Instance.UIModes.UseEncoreColors)
                        Editor.Instance.TilesToolbar = new TilesToolbar(Editor.Instance.EditorTiles.StageTiles, Editor.Instance.Paths.StageTiles_Source, Editor.Instance.EncorePalette[0], Editor.Instance);
                    else
                        Editor.Instance.TilesToolbar = new TilesToolbar(Editor.Instance.EditorTiles.StageTiles, Editor.Instance.Paths.StageTiles_Source, null, Editor.Instance);


                    Editor.Instance.TilesToolbar.TileDoubleClick = new Action<int>(x =>
                    {
                        Editor.Instance.EditorPlaceTile(new System.Drawing.Point((int)(Editor.Instance.StateModel.ShiftX / Editor.Instance.StateModel.Zoom) + EditorConstants.TILE_SIZE - 1, (int)(Editor.Instance.StateModel.ShiftY / Editor.Instance.StateModel.Zoom) + EditorConstants.TILE_SIZE - 1), x, Editor.Instance.EditLayerA);
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
                Editor.Instance.TilesToolbar.ShowShortcuts = Editor.Instance.DrawToolButton.IsChecked.Value;
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
                    Editor.Instance.EntitiesToolbar = new EntitiesToolbar(Editor.Instance.EditorScene.Objects, Editor.Instance)
                    {
                        SelectedEntity = new Action<int>(x =>
                        {
                            Editor.Instance.Entities.SelectSlot(x);
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
                            Editor.Instance.Entities.Add(x, GetEntitySpawnPoint());
                            Editor.Instance.UndoStack.Push(Editor.Instance.Entities.LastAction);
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
                Editor.Instance.EntitiesToolbar.SelectedEntities = Editor.Instance.Entities.SelectedEntities.Select(x => x.Entity).ToList();
            }
            else
            {
                if (Editor.Instance.EntitiesToolbar != null)
                {
                    Editor.Instance.EntitiesToolbar.Dispose();
                    Editor.Instance.EntitiesToolbar = null;
                }
                if (Editor.Instance.Entities != null && Editor.Instance.Entities.SelectedEntities != null)
                {
                    if (Editor.Instance.Entities.SelectedEntities.Count != 0 && Editor.Instance.Entities.TemporarySelection.Count != 0)
                    {
                        Editor.Instance.Entities.EndTempSelection();
                        Editor.Instance.Entities.Deselect();
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
                if (Editor.Instance.DrawToolButton.IsChecked.Value)
                {
                    short x = (short)(Editor.Instance.StateModel.lastX / Editor.Instance.StateModel.Zoom);
                    short y = (short)(Editor.Instance.StateModel.lastY / Editor.Instance.StateModel.Zoom);
                    if (Editor.Instance.UIModes.UseMagnetMode)
                    {
                        short alignedX = (short)(Editor.Instance.UIModes.MagnetSize * (x / Editor.Instance.UIModes.MagnetSize));
                        short alignedY = (short)(Editor.Instance.UIModes.MagnetSize * (y / Editor.Instance.UIModes.MagnetSize));
                        return new Position(alignedX, alignedY);
                    }
                    else
                    {
                        return new Position(x, y);
                    }

                }
                else
                {
                    return new Position((short)(Editor.Instance.StateModel.ShiftX / Editor.Instance.StateModel.Zoom), (short)(Editor.Instance.StateModel.ShiftY / Editor.Instance.StateModel.Zoom));
                }

            }
        }


        #endregion

        #region Updating Elements Methods
        public void ToggleEditorButtons(bool enabled, bool isParallaxAnimation = false)
        {
            Editor.Instance.MenuBar.IsEnabled = enabled;
            Editor.Instance.LayerToolbar.IsEnabled = enabled;
            Editor.Instance.MainToolbarButtons.IsEnabled = enabled;
            Editor.Instance.UI.SetSceneOnlyButtonsState((enabled ? true : Editor.Instance.EditorScene != null));
            Editor.Instance.LayerToolbar.IsEnabled = enabled;
            Editor.Instance.StatusBar1.IsEnabled = enabled;
            Editor.Instance.StatusBar2.IsEnabled = enabled;
            if (Editor.Instance.TilesToolbar != null) Editor.Instance.TilesToolbar.IsEnabled = enabled;
            if (Editor.Instance.EntitiesToolbar != null) Editor.Instance.EntitiesToolbar.IsEnabled = enabled;
            if (isParallaxAnimation)
            {
                Editor.Instance.LayerToolbar.IsEnabled = true;
                foreach (var pair in Editor.Instance.ExtraLayerEditViewButtons)
                {
                    pair.Key.IsEnabled = false;
                    pair.Value.IsEnabled = true;
                }
                Editor.Instance.EditFGHigh.IsEnabled = false;
                Editor.Instance.EditFGHigher.IsEnabled = false;
                Editor.Instance.EditFGLow.IsEnabled = false;
                Editor.Instance.EditFGLower.IsEnabled = false;

            }
        }
        public void UpdateStatusPanel()
        {
            //
            // Tooltip Bar Info 
            //

            Editor.Instance._levelIDLabel.Content = "Level ID: " + Editor.Instance.UIModes.LevelID.ToString();
            Editor.Instance.seperator1.Visibility = Visibility.Visible;
            Editor.Instance.seperator2.Visibility = Visibility.Visible;
            Editor.Instance.seperator3.Visibility = Visibility.Visible;
            Editor.Instance.seperator4.Visibility = Visibility.Visible;
            Editor.Instance.seperator5.Visibility = Visibility.Visible;
            Editor.Instance.seperator6.Visibility = Visibility.Visible;
            Editor.Instance.seperator7.Visibility = Visibility.Visible;
            //seperator8.Visibility = Visibility.Visible;
            //seperator9.Visibility = Visibility.Visible;

            if (Editor.Instance.UIModes.EnablePixelCountMode == false)
            {
                Editor.Instance.selectedPositionLabel.Content = "Selected Tile Position: X: " + (int)Editor.Instance.StateModel.SelectedTileX + ", Y: " + (int)Editor.Instance.StateModel.SelectedTileY;
                Editor.Instance.selectedPositionLabel.ToolTip = "The Position of the Selected Tile";
            }
            else
            {
                Editor.Instance.selectedPositionLabel.Content = "Selected Tile Pixel Position: " + "X: " + (int)Editor.Instance.StateModel.SelectedTileX * 16 + ", Y: " + (int)Editor.Instance.StateModel.SelectedTileY * 16;
                Editor.Instance.selectedPositionLabel.ToolTip = "The Pixel Position of the Selected Tile";
            }
            if (Editor.Instance.UIModes.EnablePixelCountMode == false)
            {
                Editor.Instance.selectionSizeLabel.Content = "Amount of Tiles in Selection: " + (Editor.Instance.StateModel.SelectedTilesCount - Editor.Instance.StateModel.DeselectTilesCount);
                Editor.Instance.selectionSizeLabel.ToolTip = "The Size of the Selection";
            }
            else
            {
                Editor.Instance.selectionSizeLabel.Content = "Length of Pixels in Selection: " + (Editor.Instance.StateModel.SelectedTilesCount - Editor.Instance.StateModel.DeselectTilesCount) * 16;
                Editor.Instance.selectionSizeLabel.ToolTip = "The Length of all the Tiles (by Pixels) in the Selection";
            }

            Editor.Instance.selectionBoxSizeLabel.Content = "Selection Box Size: X: " + (Editor.Instance.StateModel.select_x2 - Editor.Instance.StateModel.select_x1) + ", Y: " + (Editor.Instance.StateModel.select_y2 - Editor.Instance.StateModel.select_y1);

            Editor.Instance.scrollLockDirLabel.Content = "Scroll Direction: " + (Editor.Instance.UIModes.ScrollDirection == (int)ScrollDir.X ? "X" : "Y") + (Editor.Instance.UIModes.ScrollLocked ? " (Locked)" : "");


            Editor.Instance.hVScrollBarXYLabel.Content = "Zoom Value: " + Editor.Instance.StateModel.Zoom.ToString();

            if (EditorUIModes.UpdateUpdaterMessage)
            {
                if (Editor.Instance.StartScreen != null) Editor.Instance.StartScreen.UpdateStatusLabel(Editor.Instance.Updater.condition, Editor.Instance.Updater);
                EditorUIModes.UpdateUpdaterMessage = false;
            }

            //
            // End of Tooltip Bar Info Section
            //
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
            Editor.Instance.EntitiesToolbar.Entities = Editor.Instance.Entities.Entities.Select(x => x.Entity).ToList();
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
            Editor.Instance.UIModes.AllowSplineOptionsUpdate = false;
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
                Editor.Instance.SelectedSplineRender.ItemsSource = Editor.Instance.SplineSelectedObjectSpawnList;
                Editor.Instance.SelectedSplineRender.SelectedItem = Editor.Instance.SelectedSplineRender.Items[0];
                var SelectedItem = Editor.Instance.SelectedSplineRender.SelectedItem as TextBlock;
                if (SelectedItem == null) return;              
                SelectedItem.Foreground = (System.Windows.Media.SolidColorBrush)Editor.Instance.FindResource("NormalText");
                Editor.Instance.UIModes.AllowSplineOptionsUpdate = true;

            }
        }

        public void UpdateSplineSettings(int splineID)
        {
            if (!Editor.Instance.UIModes.SplineOptionsGroup.ContainsKey(splineID)) Editor.Instance.UIModes.SplineOptionsGroup.Add(splineID, new EditorUIModes.SplineOptions());
            Editor.Instance.SplineLineMode.IsChecked = Editor.Instance.UIModes.SplineOptionsGroup[splineID].SplineLineMode;
            Editor.Instance.SplineOvalMode.IsChecked = Editor.Instance.UIModes.SplineOptionsGroup[splineID].SplineOvalMode;
            Editor.Instance.SplineShowLineCheckbox.IsChecked = Editor.Instance.UIModes.SplineOptionsGroup[splineID].SplineToolShowLines;
            Editor.Instance.SplineShowObjectsCheckbox.IsChecked = Editor.Instance.UIModes.SplineOptionsGroup[splineID].SplineToolShowObject;
            Editor.Instance.SplineShowPointsCheckbox.IsChecked = Editor.Instance.UIModes.SplineOptionsGroup[splineID].SplineToolShowPoints;
            Editor.Instance.SplinePointSeperationNUD.Value = Editor.Instance.UIModes.SplineOptionsGroup[splineID].SplineSize;
            Editor.Instance.SplinePointSeperationSlider.Value = Editor.Instance.UIModes.SplineOptionsGroup[splineID].SplineSize;

            if (Editor.Instance.UIModes.SplineOptionsGroup[splineID].SplineObjectRenderingTemplate != null)
                Editor.Instance.SplineRenderObjectName.Content = Editor.Instance.UIModes.SplineOptionsGroup[splineID].SplineObjectRenderingTemplate.Entity.Object.Name.Name;
            else
                Editor.Instance.SplineRenderObjectName.Content = "None";
        }

        public void UpdateSplineToolbox()
        {
            //Editor.Instance.SplineInfoLabel1.Text = string.Format("Number of Spline Objects: {0}", Editor.Instance.UIModes.SplineTotalNumberOfObjects);
            //Editor.Instance.SplineInfoLabel2.Text = string.Format("Point Frequency: {0}", Editor.Instance.UIModes.SplineSize);
            //Editor.Instance.SplineInfoLabel3.Text = string.Format("Total Number of Rendered Points: {0}", Editor.Instance.UIModes.SplineCurrentPointsDrawn);
        }

        public void UpdateFilterButtonApperance(bool startup)
        {
            if (startup)
            {
                Editor.Instance.maniaFilterCheck.Foreground = Editor.Instance.Theming.GetColorBrush(2);
                Editor.Instance.encoreFilterCheck.Foreground = Editor.Instance.Theming.GetColorBrush(4);
                Editor.Instance.otherFilterCheck.Foreground = Editor.Instance.Theming.GetColorBrush(0);
                Editor.Instance.bothFilterCheck.Foreground = Editor.Instance.Theming.GetColorBrush(1);
                Editor.Instance.pinballFilterCheck.Foreground = Editor.Instance.Theming.GetColorBrush(255);
            }
            if (Properties.Settings.Default.UseBitOperators)
            {
                Editor.Instance.maniaFilterCheck.Content = "Mania (0b0010)";
                Editor.Instance.encoreFilterCheck.Content = "Encore (0b0100)";
                Editor.Instance.otherFilterCheck.Content = "Other (0b0000)";
                Editor.Instance.bothFilterCheck.Content = "Both (0b0001)";
                Editor.Instance.pinballFilterCheck.Content = "All (0b11111111)";
            }
            else
            {
                Editor.Instance.maniaFilterCheck.Content = "Mania (2)";
                Editor.Instance.encoreFilterCheck.Content = "Encore (4)";
                Editor.Instance.otherFilterCheck.Content = "Other (0)";
                Editor.Instance.bothFilterCheck.Content = "Both (1 & 5)";
                Editor.Instance.pinballFilterCheck.Content = "All (255)";
            }
        }

        public void UpdateCustomColors()
        {
            Editor.Instance.CSAC.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(Editor.Instance.UIModes.CollisionSAColour.A, Editor.Instance.UIModes.CollisionSAColour.R, Editor.Instance.UIModes.CollisionSAColour.G, Editor.Instance.UIModes.CollisionSAColour.B));
            Editor.Instance.SSTOC.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(Editor.Instance.UIModes.CollisionTOColour.A, Editor.Instance.UIModes.CollisionTOColour.R, Editor.Instance.UIModes.CollisionTOColour.G, Editor.Instance.UIModes.CollisionTOColour.B));
            Editor.Instance.CSLRDC.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(Editor.Instance.UIModes.CollisionLRDColour.A, Editor.Instance.UIModes.CollisionLRDColour.R, Editor.Instance.UIModes.CollisionLRDColour.G, Editor.Instance.UIModes.CollisionLRDColour.B));
            Editor.Instance.WLC.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(Editor.Instance.UIModes.waterColor.A, Editor.Instance.UIModes.waterColor.R, Editor.Instance.UIModes.waterColor.G, Editor.Instance.UIModes.waterColor.B));
            Editor.Instance.GDC.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(Editor.Instance.UIModes.GridColor.A, Editor.Instance.UIModes.GridColor.R, Editor.Instance.UIModes.GridColor.G, Editor.Instance.UIModes.GridColor.B));
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

            bool parallaxAnimationInProgress = Editor.Instance.UIModes.AnimationsEnabled && Editor.Instance.UIModes.ParallaxAnimationChecked;

            UpdateGameRunningButton(Editor.Instance.EditorScene != null);
            Editor.Instance.Theming.UpdateThemeForItemsWaiting();
            UpdateFilterButtonApperance(false);
            UpdateStatusPanel();
            SetSceneOnlyButtonsState(Editor.Instance.EditorScene != null && !parallaxAnimationInProgress, stageLoad);
            SetParallaxAnimationOnlyButtonsState(parallaxAnimationInProgress);
            UpdateSplineToolbox();
            Editor.Instance.CustomGridLabel.Text = string.Format(Editor.Instance.CustomGridLabel.Tag.ToString(), Properties.Defaults.Default.CustomGridSizeValue);

        }
        public void UpdateGameRunningButton(bool enabled = true)
        {
            
            Editor.Instance.RunSceneButton.IsEnabled = enabled;
            Editor.Instance.RunSceneDropDown.IsEnabled = enabled && Editor.Instance.RunSceneButton.IsEnabled;

            if (Editor.Instance.InGame.GameRunning || System.Diagnostics.Process.GetProcessesByName("SonicMania").FirstOrDefault() != null)
            {
                if (Editor.Instance.InGame.GameRunning) Editor.Instance.RunSceneIcon.Fill = System.Windows.Media.Brushes.Blue;
                else Editor.Instance.RunSceneIcon.Fill = System.Windows.Media.Brushes.Green;
            }
            else
            {
                Editor.Instance.RunSceneIcon.Fill = System.Windows.Media.Brushes.Gray;
            }
        }
        private void UpdateTooltips()
        {
            UpdateTooltipForStacks(Editor.Instance.UndoButton, Editor.Instance.UndoStack);
            UpdateTooltipForStacks(Editor.Instance.RedoButton, Editor.Instance.RedoStack);
            UpdateTextBlockForStacks(Editor.Instance.UndoMenuItemInfo, Editor.Instance.UndoStack);
            UpdateTextBlockForStacks(Editor.Instance.RedoMenuItemInfo, Editor.Instance.RedoStack);
            if (Editor.Instance.UIControl != null)
            {
                if (Editor.Instance.IsVisible)
                {
                    Editor.Instance.UIControl.UpdateMenuItems();
                    Editor.Instance.UIControl.UpdateTooltips();
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
                if (Editor.Instance.UIModes.UseEncoreColors == true)
                {
                    Editor.Instance.EditorTiles.StageTiles?.Image.Reload(Editor.Instance.EncorePalette[0]);
                    Editor.Instance.TilesToolbar?.Reload(Editor.Instance.EncorePalette[0]);
                }
                else
                {
                    Editor.Instance.EditorTiles.StageTiles?.Image.Reload();
                    Editor.Instance.TilesToolbar?.Reload();
                }

                Editor.Instance.TileConfig = new TileConfig(Editor.Instance.Paths.TileConfig_Source);



            }
            catch (Exception ex)
            {
                RSDKrU.MessageBox.Show(ex.Message);
            }
        }

        #endregion
    }
}
