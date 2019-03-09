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
            if (Settings.mySettings.EntityFreeCam)
            {
                Editor.GraphicsModel.vScrollBar1.IsEnabled = false;
                Editor.GraphicsModel.hScrollBar1.IsEnabled = false;
            }
            else
            {
                Editor.GraphicsModel.vScrollBar1.IsEnabled = true;
                Editor.GraphicsModel.hScrollBar1.IsEnabled = true;
            }
            SetSceneOnlyButtonsState(Editor.EditorScene != null, stageLoad);
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

            Editor.MultiLayerHint.Visibility = (Editor.MultiLayerEditMode ? Visibility.Visible : Visibility.Collapsed);
            Editor.LayerHint.Visibility = (Editor.MultiLayerEditMode ? Visibility.Collapsed : Visibility.Visible);


            Editor.Save.IsEnabled = enabled;

            if (Settings.mySettings.ReduceZoom)
            {
                Editor.ZoomInButton.IsEnabled = enabled && Editor.EditorState.ZoomLevel < 5;
                Editor.ZoomOutButton.IsEnabled = enabled && Editor.EditorState.ZoomLevel > -2;
            }
            else
            {
                Editor.ZoomInButton.IsEnabled = enabled && Editor.EditorState.ZoomLevel < 5;
                Editor.ZoomOutButton.IsEnabled = enabled && Editor.EditorState.ZoomLevel > -5;
            }


            UpdateGameRunningButton(enabled);

            SetEditButtonsState(enabled);
            UpdateTooltips();

            if (Settings.mySettings.preRenderSceneOption == 3 && enabled && stageLoad)
            {
                Editor.PreLoadSceneButton_Click(null, null);
            }
            else if (Settings.mySettings.preRenderSceneOption == 2 && enabled && stageLoad)
            {
                MessageBoxResult result = RSDKrU.MessageBox.Show("Do you wish to Pre-Render this scene?", "Requesting to Pre-Render the Scene", MessageBoxButton.YesNo, MessageBoxImage.Information);
                if (result == MessageBoxResult.Yes)
                {
                    Editor.PreLoadSceneButton_Click(null, null);
                }
            }
            else if (Settings.mySettings.preRenderSceneOption == 1 && Editor.PreRenderSceneSelectCheckbox && enabled && stageLoad)
            {
                Editor.PreLoadSceneButton_Click(null, null);
            }

            if (stageLoad)
            {
                Editor.SetViewSize((int)(Editor.SceneWidth * Editor.EditorState.Zoom), (int)(Editor.SceneHeight * Editor.EditorState.Zoom));
            }

            Editor.EditorTheming.UpdateButtonColors();
        }

        public void UpdateGameRunningButton(bool enabled = true)
        {
            Editor.RunSceneButton.IsEnabled = enabled;
            Editor.RunSceneDropDown.IsEnabled = enabled;

            if (Editor.GameRunning || System.Diagnostics.Process.GetProcessesByName("SonicMania").FirstOrDefault() != null)
            {
                if (Editor.GameRunning) Editor.EditorTheming.SetButtonColors(Editor.RunSceneButton, System.Drawing.Color.Blue);
                else Editor.EditorTheming.SetButtonColors(Editor.RunSceneButton, System.Drawing.Color.Green);
            }
            else
            {
                Editor.EditorTheming.SetButtonColors(Editor.RunSceneButton, System.Drawing.Color.Gray);
            }
        }
        public void SetSelectOnlyButtonsState(bool enabled = true)
        {
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

            if (Editor.IsEntitiesEdit() && Editor.entitiesToolbar != null)
            {
                Editor.entitiesToolbar.SelectedEntities = Editor.entities.SelectedEntities.Select(x => x.Entity).ToList();
            }
        }
        private void SetLayerEditButtonsState(bool enabled)
        {
            if (!Editor.MultiLayerEditMode)
            {
                if (enabled && Editor.EditFGLow.IsCheckedN.Value) Editor.EditLayer = Editor.FGLow;
                else if (enabled && Editor.EditFGHigh.IsCheckedN.Value) Editor.EditLayer = Editor.FGHigh;
                else if (enabled && Editor.EditFGHigher.IsCheckedN.Value) Editor.EditLayer = Editor.FGHigher;
                else if (enabled && Editor.EditFGLower.IsCheckedN.Value) Editor.EditLayer = Editor.FGLower;
                else if (enabled && Editor.ExtraLayerEditViewButtons.Any(elb => elb.Value.IsCheckedN.Value))
                {
                    var selectedExtraLayerButton = Editor.ExtraLayerEditViewButtons.Single(elb => elb.Value.IsCheckedN.Value);
                    var editorLayer = Editor.EditorScene.OtherLayers.Single(el => el.Name.Equals(selectedExtraLayerButton.Value.Text));

                    Editor.EditLayer = editorLayer;
                }
                else Editor.EditLayer = null;
            }
            else
            {
                SetEditLayerA();
                SetEditLayerB();
            }

            if (Editor.TilesToolbar != null)
            {
                Editor.TilesToolbar.ChunksReload();
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
            bool windowsClipboardState;
            bool windowsEntityClipboardState;
            Editor.EditFGLow.IsEnabled = enabled && Editor.FGLow != null;
            Editor.EditFGHigh.IsEnabled = enabled && Editor.FGHigh != null;
            Editor.EditFGLower.IsEnabled = enabled && Editor.FGLower != null;
            Editor.EditFGHigher.IsEnabled = enabled && Editor.FGHigher != null;
            Editor.EditEntities.IsEnabled = enabled;
            Editor.entityManagerToolStripMenuItem.IsEnabled = enabled && Editor.StageConfig != null;
            Editor.importSoundsToolStripMenuItem.IsEnabled = enabled && Editor.StageConfig != null;
            Editor.layerManagerToolStripMenuItem.IsEnabled = enabled;
            Editor.editBackgroundColorsToolStripMenuItem.IsEnabled = enabled;
            Editor.preRenderSceneToolStripMenuItem.IsEnabled = enabled;

            Editor.editEntitiesOptionToolStrip.IsEnabled = enabled;

            SetLayerEditButtonsState(enabled);

            Editor.undoToolStripMenuItem.IsEnabled = enabled && Editor.undo.Count > 0;
            Editor.redoToolStripMenuItem.IsEnabled = enabled && Editor.redo.Count > 0;

            Editor.MagnetMode.IsEnabled = enabled && Editor.IsEntitiesEdit();
            Editor.MagnetMode.IsChecked = Editor.UITools.UseMagnetMode && Editor.IsEntitiesEdit();
            Editor.MagnetModeSplitButton.IsEnabled = enabled && Editor.IsEntitiesEdit();
            Editor.UITools.UseMagnetMode = Editor.IsEntitiesEdit() && Editor.MagnetMode.IsChecked.Value;



            Editor.UndoButton.IsEnabled = enabled && Editor.undo.Count > 0;
            Editor.RedoButton.IsEnabled = enabled && Editor.redo.Count > 0;

            Editor.findAndReplaceToolStripMenuItem.IsEnabled = enabled && Editor.EditLayer != null;

            Editor.PointerButton.IsEnabled = enabled && Editor.IsTilesEdit();
            Editor.SelectToolButton.IsEnabled = enabled && Editor.IsTilesEdit();
            Editor.PlaceTilesButton.IsEnabled = enabled && Editor.IsTilesEdit();
            Editor.InteractionToolButton.IsEnabled = enabled;
            Editor.ChunksToolButton.IsEnabled = enabled && Editor.IsTilesEdit();

            Editor.PointerButton.IsChecked = (bool)Editor.PointerButton.IsChecked || (!(bool)Editor.PointerButton.IsChecked && !(bool)Editor.SelectToolButton.IsChecked && !(bool)Editor.PlaceTilesButton.IsChecked);
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
            Editor.ShowCollisionAButton.IsEnabled = enabled && Editor.TilesConfig != null;
            Editor.ShowCollisionBButton.IsEnabled = enabled && Editor.TilesConfig != null;
            Editor.ShowTileIDButton.IsEnabled = enabled && Editor.StageConfig != null;
            Editor.GridSizeButton.IsEnabled = enabled && Editor.StageConfig != null;
            Editor.EncorePaletteButton.IsEnabled = enabled && Editor.EncorePaletteExists;
            Editor.FlipAssistButton.IsEnabled = enabled;



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



            if (enabled && (Editor.IsTilesEdit() || ((Editor.TilesClipboard != null || windowsClipboardState))))
                Editor.pasteToolStripMenuItem.IsEnabled = true;
            else
                Editor.pasteToolStripMenuItem.IsEnabled = false;

            if (enabled && (Editor.IsEntitiesEdit() || ((Editor.entitiesClipboard != null || windowsEntityClipboardState))))
                Editor.pasteToolStripMenuItem.IsEnabled = true;
            else
                Editor.pasteToolStripMenuItem.IsEnabled = false;


            if (Editor.IsTilesEdit())
            {
                if (Editor.TilesToolbar == null)
                {
                    if (Editor.UITools.UseEncoreColors)
                        Editor.TilesToolbar = new TilesToolbar(Editor.EditorTiles.StageTiles, Editor.EditorPath.StageTiles_Source, Editor.EncorePalette[0], Editor);
                    else
                        Editor.TilesToolbar = new TilesToolbar(Editor.EditorTiles.StageTiles, Editor.EditorPath.StageTiles_Source, null, Editor);


                    Editor.TilesToolbar.TileDoubleClick = new Action<int>(x =>
                    {
                        Editor.EditorPlaceTile(new System.Drawing.Point((int)(Editor.EditorState.ShiftX / Editor.EditorState.Zoom) + EditorConstants.TILE_SIZE - 1, (int)(Editor.EditorState.ShiftY / Editor.EditorState.Zoom) + EditorConstants.TILE_SIZE - 1), x, Editor.EditLayerA);
                    });
                    Editor.TilesToolbar.TileOptionChanged = new Action<int, bool>((option, state) =>
                    {
                        Editor.EditLayerA?.SetPropertySelected(option + 12, state);
                        Editor.EditLayerB?.SetPropertySelected(option + 12, state);

                    });
                    Editor.ToolBarPanelRight.Children.Clear();
                    Editor.ToolBarPanelRight.Children.Add(Editor.TilesToolbar);
                    UpdateToolbars(true, true);
                    Editor.Form1_Resize(null, null);
                    Editor.Focus();
                }
                if (Editor.IsChunksEdit()) Editor.TilesToolbar.TabControl.TabIndex = 1;
                else Editor.TilesToolbar.TabControl.TabIndex = 0;
                Editor.UpdateTilesOptions();
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
                if (Editor.entitiesToolbar == null)
                {
                    Editor.entitiesToolbar = new EntitiesToolbar2(Editor.EditorScene.Objects, Editor)
                    {
                        SelectedEntity = new Action<int>(x =>
                        {
                            Editor.entities.SelectSlot(x);
                            SetSelectOnlyButtonsState();
                        }),
                        AddAction = new Action<ManiacEditor.Actions.IAction>(x =>
                        {
                            Editor.undo.Push(x);
                            Editor.redo.Clear();
                            UpdateControls();
                        }),
                        Spawn = new Action<SceneObject>(x =>
                        {
                            Editor.entities.Add(x, new Position((short)(Editor.EditorState.ShiftX / Editor.EditorState.Zoom), (short)(Editor.EditorState.ShiftY / Editor.EditorState.Zoom)));
                            Editor.undo.Push(Editor.entities.LastAction);
                            Editor.redo.Clear();
                            UpdateControls();
                        })
                    };
                    Editor.ToolBarPanelRight.Children.Clear();
                    Editor.ToolBarPanelRight.Children.Add(Editor.entitiesToolbar);
                    UpdateToolbars(true, true);
                    Editor.Form1_Resize(null, null);
                }
                Editor.UpdateEntitiesToolbarList();
                Editor.entitiesToolbar.SelectedEntities = Editor.entities.SelectedEntities.Select(x => x.Entity).ToList();
            }
            else
            {
                if (Editor.entitiesToolbar != null)
                {
                    Editor.entitiesToolbar.Dispose();
                    Editor.entitiesToolbar = null;
                }
                if (Editor.entities != null && Editor.entities.SelectedEntities != null)
                {
                    if (Editor.entities.SelectedEntities.Count != 0 && Editor.entities.tempSelection.Count != 0)
                    {
                        Editor.entities.EndTempSelection();
                        Editor.entities.Deselect();
                    }
                }


            }
            if (Editor.TilesToolbar == null && Editor.entitiesToolbar == null && (Editor.ToolBarPanelRight.Children.Count != 0))
            {
                Editor.ToolBarPanelRight.Children.Clear();
                UpdateToolbars(true, false);
                Editor.Form1_Resize(null, null);
            }

            SetSelectOnlyButtonsState(enabled);
        }

        private void UpdateTooltips()
        {
            UpdateTooltipForStacks(Editor.UndoButton, Editor.undo);
            UpdateTooltipForStacks(Editor.RedoButton, Editor.redo);
            if (Editor.UIControl != null)
            {
                if (Editor.IsVisible)
                {
                    Editor.UIControl.UpdateMenuItems();
                    Editor.UIControl.UpdateTooltips();
                }

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
    }
}
