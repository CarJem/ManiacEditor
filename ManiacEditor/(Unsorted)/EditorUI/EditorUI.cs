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
            Controls.Base.MainEditor.Instance.EditorToolbar.ShowFGHigh.IsEnabled = enabled && Classes.Core.Solution.FGHigh != null;
            Controls.Base.MainEditor.Instance.EditorToolbar.ShowFGLow.IsEnabled = enabled && Classes.Core.Solution.FGLow != null;
            Controls.Base.MainEditor.Instance.EditorToolbar.ShowFGHigher.IsEnabled = enabled && Classes.Core.Solution.FGHigher != null;
            Controls.Base.MainEditor.Instance.EditorToolbar.ShowFGLower.IsEnabled = enabled && Classes.Core.Solution.FGLower != null;
            Controls.Base.MainEditor.Instance.EditorToolbar.ShowEntities.IsEnabled = enabled;

            Controls.Base.MainEditor.Instance.EditorToolbar.ReloadButton.IsEnabled = enabled;

            Controls.Base.MainEditor.Instance.EditorMenuBar.SetSceneOnlyButtonsState(enabled, stageLoad);

            Controls.Base.MainEditor.Instance.EditorToolbar.Save.IsEnabled = enabled;

            if (Core.Settings.MyPerformance.ReduceZoom)
            {
                Controls.Base.MainEditor.Instance.EditorToolbar.ZoomInButton.IsEnabled = enabled && Classes.Core.SolutionState.ZoomLevel < 5;
                Controls.Base.MainEditor.Instance.EditorToolbar.ZoomOutButton.IsEnabled = enabled && Classes.Core.SolutionState.ZoomLevel > -2;
            }
            else
            {
                Controls.Base.MainEditor.Instance.EditorToolbar.ZoomInButton.IsEnabled = enabled && Classes.Core.SolutionState.ZoomLevel < 5;
                Controls.Base.MainEditor.Instance.EditorToolbar.ZoomOutButton.IsEnabled = enabled && Classes.Core.SolutionState.ZoomLevel > -5;
            }


            UpdateGameRunningButton(enabled);

            SetEditButtonsState(enabled);
            UpdateTooltips();

            if (stageLoad)
            {
                Controls.Base.MainEditor.Instance.ZoomModel.SetViewSize((int)(Classes.Core.Solution.SceneWidth * Classes.Core.SolutionState.Zoom), (int)(Classes.Core.Solution.SceneHeight * Classes.Core.SolutionState.Zoom));
            }

            Controls.Base.MainEditor.Instance.Theming.UpdateButtonColors();

        }

        public void SetParallaxAnimationOnlyButtonsState(bool enabled = true)
        {
            Controls.Base.MainEditor.Instance.EditorToolbar.Open.IsEnabled = !enabled;
            Controls.Base.MainEditor.Instance.EditorToolbar.ShowAnimations.IsEnabled = enabled || Classes.Core.Solution.CurrentScene != null;
            Controls.Base.MainEditor.Instance.EditorToolbar.animationsSplitButton_Dropdown.IsEnabled = enabled || Classes.Core.Solution.CurrentScene != null;
            Controls.Base.MainEditor.Instance.EditorMenuBar.MenuBar.IsEnabled = !enabled;
            Controls.Base.MainEditor.Instance.EditorStatusBar.StatusBar1.IsEnabled = !enabled; 
            Controls.Base.MainEditor.Instance.EditorTabControl.IsEnabled = !enabled;
            Controls.Base.MainEditor.Instance.EditorToolbar.New.IsEnabled = !enabled;
            Controls.Base.MainEditor.Instance.EditorToolbar.Open.IsEnabled = !enabled;

            if (enabled)
            {
                Controls.Base.MainEditor.Instance.EditorToolbar.ShowFGHigh.IsEnabled = Classes.Core.Solution.FGHigh != null;
                Controls.Base.MainEditor.Instance.EditorToolbar.ShowFGLow.IsEnabled = Classes.Core.Solution.FGLow != null;
                Controls.Base.MainEditor.Instance.EditorToolbar.ShowFGHigher.IsEnabled = Classes.Core.Solution.FGHigher != null;
                Controls.Base.MainEditor.Instance.EditorToolbar.ShowFGLower.IsEnabled = Classes.Core.Solution.FGLower != null;
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
            enabled &= Controls.Base.MainEditor.Instance.IsSelected();

            if (Controls.Base.MainEditor.Instance.IsEntitiesEdit() && Controls.Base.MainEditor.Instance.EntitiesToolbar != null)
            {
                Controls.Base.MainEditor.Instance.EntitiesToolbar.SelectedEntities = Classes.Core.Solution.Entities.SelectedEntities.Select(x => x.Entity).ToList();
            }
        }

        private void SetLayerEditButtonsState(bool enabled)
        {
            if (!Classes.Core.SolutionState.MultiLayerEditMode)
            {
                if (enabled && Controls.Base.MainEditor.Instance.EditorToolbar.EditFGLow.IsCheckedN.Value) Classes.Core.Solution.EditLayerA = Classes.Core.Solution.FGLow;
                else if (enabled && Controls.Base.MainEditor.Instance.EditorToolbar.EditFGHigh.IsCheckedN.Value) Classes.Core.Solution.EditLayerA = Classes.Core.Solution.FGHigh;
                else if (enabled && Controls.Base.MainEditor.Instance.EditorToolbar.EditFGHigher.IsCheckedN.Value) Classes.Core.Solution.EditLayerA = Classes.Core.Solution.FGHigher;
                else if (enabled && Controls.Base.MainEditor.Instance.EditorToolbar.EditFGLower.IsCheckedN.Value) Classes.Core.Solution.EditLayerA = Classes.Core.Solution.FGLower;
                else if (enabled && Controls.Base.MainEditor.Instance.ExtraLayerEditViewButtons.Any(elb => elb.Value.IsCheckedN.Value))
                {
                    var selectedExtraLayerButton = Controls.Base.MainEditor.Instance.ExtraLayerEditViewButtons.Single(elb => elb.Value.IsCheckedN.Value);
                    var editorLayer = Classes.Core.Solution.CurrentScene.OtherLayers.Single(el => el.Name.Equals(selectedExtraLayerButton.Value.Text));

                    Classes.Core.Solution.EditLayerA = editorLayer;
                }
                else Classes.Core.Solution.EditLayerA = null;
            }
            else
            {
                SetEditLayerA();
                SetEditLayerB();
            }

            void SetEditLayerA()
            {
                if (enabled && Controls.Base.MainEditor.Instance.EditorToolbar.EditFGLow.IsCheckedA.Value) Classes.Core.Solution.EditLayerA = Classes.Core.Solution.FGLow;
                else if (enabled && Controls.Base.MainEditor.Instance.EditorToolbar.EditFGHigh.IsCheckedA.Value) Classes.Core.Solution.EditLayerA = Classes.Core.Solution.FGHigh;
                else if (enabled && Controls.Base.MainEditor.Instance.EditorToolbar.EditFGHigher.IsCheckedA.Value) Classes.Core.Solution.EditLayerA = Classes.Core.Solution.FGHigher;
                else if (enabled && Controls.Base.MainEditor.Instance.EditorToolbar.EditFGLower.IsCheckedA.Value) Classes.Core.Solution.EditLayerA = Classes.Core.Solution.FGLower;
                else if (enabled && Controls.Base.MainEditor.Instance.ExtraLayerEditViewButtons.Any(elb => elb.Value.IsCheckedA.Value))
                {
                    var selectedExtraLayerButton = Controls.Base.MainEditor.Instance.ExtraLayerEditViewButtons.Single(elb => elb.Value.IsCheckedA.Value);
                    var editorLayer = Classes.Core.Solution.CurrentScene.OtherLayers.Single(el => el.Name.Equals(selectedExtraLayerButton.Value.Text));

                    Classes.Core.Solution.EditLayerA = editorLayer;
                }
                else Classes.Core.Solution.EditLayerA = null;
            }
            void SetEditLayerB()
            {
                if (enabled && Controls.Base.MainEditor.Instance.EditorToolbar.EditFGLow.IsCheckedB.Value) Classes.Core.Solution.EditLayerB = Classes.Core.Solution.FGLow;
                else if (enabled && Controls.Base.MainEditor.Instance.EditorToolbar.EditFGHigh.IsCheckedB.Value) Classes.Core.Solution.EditLayerB = Classes.Core.Solution.FGHigh;
                else if (enabled && Controls.Base.MainEditor.Instance.EditorToolbar.EditFGHigher.IsCheckedB.Value) Classes.Core.Solution.EditLayerB = Classes.Core.Solution.FGHigher;
                else if (enabled && Controls.Base.MainEditor.Instance.EditorToolbar.EditFGLower.IsCheckedB.Value) Classes.Core.Solution.EditLayerB = Classes.Core.Solution.FGLower;
                else if (enabled && Controls.Base.MainEditor.Instance.ExtraLayerEditViewButtons.Any(elb => elb.Value.IsCheckedB.Value))
                {
                    var selectedExtraLayerButton = Controls.Base.MainEditor.Instance.ExtraLayerEditViewButtons.Single(elb => elb.Value.IsCheckedB.Value);
                    var editorLayer = Classes.Core.Solution.CurrentScene.OtherLayers.Single(el => el.Name.Equals(selectedExtraLayerButton.Value.Text));

                    Classes.Core.Solution.EditLayerB = editorLayer;
                }
                else Classes.Core.Solution.EditLayerB = null;
            }

        }
        private void SetEditButtonsState(bool enabled)
        {

            Controls.Base.MainEditor.Instance.EditorToolbar.EditFGLow.IsEnabled = enabled && Classes.Core.Solution.FGLow != null;
            Controls.Base.MainEditor.Instance.EditorToolbar.EditFGHigh.IsEnabled = enabled && Classes.Core.Solution.FGHigh != null;
            Controls.Base.MainEditor.Instance.EditorToolbar.EditFGLower.IsEnabled = enabled && Classes.Core.Solution.FGLower != null;
            Controls.Base.MainEditor.Instance.EditorToolbar.EditFGHigher.IsEnabled = enabled && Classes.Core.Solution.FGHigher != null;
            Controls.Base.MainEditor.Instance.EditorToolbar.EditEntities.IsEnabled = enabled;

            Controls.Base.MainEditor.Instance.EditorToolbar.EditFGLow.IsCheckedA = enabled && Controls.Base.MainEditor.Instance.EditorToolbar.EditFGLow.IsCheckedA.Value;
            Controls.Base.MainEditor.Instance.EditorToolbar.EditFGHigh.IsCheckedA = enabled && Controls.Base.MainEditor.Instance.EditorToolbar.EditFGHigh.IsCheckedA.Value;
            Controls.Base.MainEditor.Instance.EditorToolbar.EditFGLower.IsCheckedA = enabled && Controls.Base.MainEditor.Instance.EditorToolbar.EditFGLower.IsCheckedA.Value;
            Controls.Base.MainEditor.Instance.EditorToolbar.EditFGHigher.IsCheckedA = enabled && Controls.Base.MainEditor.Instance.EditorToolbar.EditFGHigher.IsCheckedA.Value;

            Controls.Base.MainEditor.Instance.EditorToolbar.EditFGLow.IsCheckedB = enabled && Controls.Base.MainEditor.Instance.EditorToolbar.EditFGLow.IsCheckedB.Value;
            Controls.Base.MainEditor.Instance.EditorToolbar.EditFGHigh.IsCheckedB = enabled && Controls.Base.MainEditor.Instance.EditorToolbar.EditFGHigh.IsCheckedB.Value;
            Controls.Base.MainEditor.Instance.EditorToolbar.EditFGLower.IsCheckedB = enabled && Controls.Base.MainEditor.Instance.EditorToolbar.EditFGLower.IsCheckedB.Value;
            Controls.Base.MainEditor.Instance.EditorToolbar.EditFGHigher.IsCheckedB = enabled && Controls.Base.MainEditor.Instance.EditorToolbar.EditFGHigher.IsCheckedB.Value;

            foreach (var layerButtons in Controls.Base.MainEditor.Instance.ExtraLayerEditViewButtons)
            {
                layerButtons.Value.IsCheckedA = layerButtons.Value.IsCheckedA.Value && enabled;
                layerButtons.Value.IsCheckedB = layerButtons.Value.IsCheckedB.Value && enabled;
            }

            Controls.Base.MainEditor.Instance.EditorToolbar.EditEntities.IsCheckedN = enabled && Controls.Base.MainEditor.Instance.EditorToolbar.EditEntities.IsCheckedN.Value;

            Controls.Base.MainEditor.Instance.EditorMenuBar.SetEditButtonsState(enabled);

            SetLayerEditButtonsState(enabled);

            Controls.Base.MainEditor.Instance.EditorToolbar.MagnetMode.IsEnabled = enabled && Controls.Base.MainEditor.Instance.IsEntitiesEdit();
            Controls.Base.MainEditor.Instance.EditorToolbar.MagnetMode.IsChecked = Classes.Core.SolutionState.UseMagnetMode && Controls.Base.MainEditor.Instance.IsEntitiesEdit();
            Controls.Base.MainEditor.Instance.EditorToolbar.MagnetModeSplitButton.IsEnabled = enabled && Controls.Base.MainEditor.Instance.IsEntitiesEdit();
            Classes.Core.SolutionState.UseMagnetMode = Controls.Base.MainEditor.Instance.IsEntitiesEdit() && Controls.Base.MainEditor.Instance.EditorToolbar.MagnetMode.IsChecked.Value;



            Controls.Base.MainEditor.Instance.EditorToolbar.UndoButton.IsEnabled = enabled && Controls.Base.MainEditor.Instance.UndoStack.Count > 0;
            Controls.Base.MainEditor.Instance.EditorToolbar.RedoButton.IsEnabled = enabled && Controls.Base.MainEditor.Instance.RedoStack.Count > 0;



            Controls.Base.MainEditor.Instance.EditorToolbar.PointerToolButton.IsEnabled = enabled;
            Controls.Base.MainEditor.Instance.EditorToolbar.SelectToolButton.IsEnabled = enabled && Controls.Base.MainEditor.Instance.IsTilesEdit();
            Controls.Base.MainEditor.Instance.EditorToolbar.DrawToolButton.IsEnabled = enabled && Controls.Base.MainEditor.Instance.IsTilesEdit() || Controls.Base.MainEditor.Instance.IsEntitiesEdit();
            Controls.Base.MainEditor.Instance.EditorToolbar.InteractionToolButton.IsEnabled = enabled;
            Controls.Base.MainEditor.Instance.EditorToolbar.ChunksToolButton.IsEnabled = enabled && Controls.Base.MainEditor.Instance.IsTilesEdit();
            Controls.Base.MainEditor.Instance.EditorToolbar.SplineToolButton.IsEnabled = enabled && Controls.Base.MainEditor.Instance.IsEntitiesEdit();
            Controls.Base.MainEditor.Instance.EditorToolbar.SplineToolButton.IsChecked = Controls.Base.MainEditor.Instance.EditorToolbar.SplineToolButton.IsChecked.Value && Controls.Base.MainEditor.Instance.IsEntitiesEdit();

            bool isAnyOtherToolChecked()
            {
                bool isPointer = (bool)Controls.Base.MainEditor.Instance.EditorToolbar.PointerToolButton.IsChecked.Value;
                bool isSelect = (bool)Controls.Base.MainEditor.Instance.EditorToolbar.SelectToolButton.IsChecked.Value;
                bool isDraw = (bool)Controls.Base.MainEditor.Instance.EditorToolbar.DrawToolButton.IsChecked.Value;
                bool isSpline = (bool)Controls.Base.MainEditor.Instance.EditorToolbar.SplineToolButton.IsChecked.Value;

                if (Controls.Base.MainEditor.Instance.IsEntitiesEdit())
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


            Controls.Base.MainEditor.Instance.EditorToolbar.PointerToolButton.IsChecked = isAnyOtherToolChecked();
            Controls.Base.MainEditor.Instance.EditorToolbar.ChunksToolButton.IsChecked = (bool)Controls.Base.MainEditor.Instance.EditorToolbar.ChunksToolButton.IsChecked && !Controls.Base.MainEditor.Instance.IsEntitiesEdit();
            if (Controls.Base.MainEditor.Instance.TilesToolbar != null)
            {
                if (Controls.Base.MainEditor.Instance.EditorToolbar.ChunksToolButton.IsChecked.Value)
                {
                    Controls.Base.MainEditor.Instance.TilesToolbar.TabControl.SelectedIndex = 1;
                }
                else
                {
                    Controls.Base.MainEditor.Instance.TilesToolbar.TabControl.SelectedIndex = 0;
                }
            }

            Controls.Base.MainEditor.Instance.EditorToolbar.ShowGridButton.IsEnabled = enabled && Classes.Core.Solution.StageConfig != null;
            Controls.Base.MainEditor.Instance.EditorToolbar.ShowCollisionAButton.IsEnabled = enabled && Classes.Core.Solution.TileConfig != null;
            Controls.Base.MainEditor.Instance.EditorToolbar.ShowCollisionBButton.IsEnabled = enabled && Classes.Core.Solution.TileConfig != null;
            Controls.Base.MainEditor.Instance.EditorToolbar.ShowTileIDButton.IsEnabled = enabled && Classes.Core.Solution.StageConfig != null;
            Controls.Base.MainEditor.Instance.EditorToolbar.EncorePaletteButton.IsEnabled = enabled && Classes.Core.SolutionState.EncorePaletteExists;
            Controls.Base.MainEditor.Instance.EditorToolbar.FlipAssistButton.IsEnabled = enabled;

            if (Controls.Base.MainEditor.Instance.IsTilesEdit())
            {
                if (Controls.Base.MainEditor.Instance.TilesToolbar == null)
                {
                    if (Classes.Core.SolutionState.UseEncoreColors)
                        Controls.Base.MainEditor.Instance.TilesToolbar = new Controls.Base.Toolbars.TilesToolbar.TilesToolbar(Classes.Core.Solution.CurrentTiles.StageTiles, Controls.Base.MainEditor.Instance.Paths.StageTiles_Source, Controls.Base.MainEditor.Instance.EncorePalette[0], Controls.Base.MainEditor.Instance);
                    else
                        Controls.Base.MainEditor.Instance.TilesToolbar = new Controls.Base.Toolbars.TilesToolbar.TilesToolbar(Classes.Core.Solution.CurrentTiles.StageTiles, Controls.Base.MainEditor.Instance.Paths.StageTiles_Source, null, Controls.Base.MainEditor.Instance);


                    Controls.Base.MainEditor.Instance.TilesToolbar.TileDoubleClick = new Action<int>(x =>
                    {
                        Controls.Base.MainEditor.Instance.EditorPlaceTile(new System.Drawing.Point((int)(Classes.Core.SolutionState.ViewPositionX/ Classes.Core.SolutionState.Zoom) + Classes.Core.Constants.TILE_SIZE - 1, (int)(Classes.Core.SolutionState.ViewPositionY / Classes.Core.SolutionState.Zoom) + Classes.Core.Constants.TILE_SIZE - 1), x, Classes.Core.Solution.EditLayerA);
                    });
                    Controls.Base.MainEditor.Instance.TilesToolbar.TileOptionChanged = new Action<int, bool>((option, state) =>
                    {
                        Classes.Core.Solution.EditLayerA?.SetPropertySelected(option + 12, state);
                        Classes.Core.Solution.EditLayerB?.SetPropertySelected(option + 12, state);

                    });
                    Controls.Base.MainEditor.Instance.ToolBarPanelRight.Children.Clear();
                    Controls.Base.MainEditor.Instance.ToolBarPanelRight.Children.Add(Controls.Base.MainEditor.Instance.TilesToolbar);
                    UpdateToolbars(true, true);
                    Controls.Base.MainEditor.Instance.Editor_Resize(null, null);
                    Controls.Base.MainEditor.Instance.Focus();
                }
                if (Controls.Base.MainEditor.Instance.IsChunksEdit()) Controls.Base.MainEditor.Instance.TilesToolbar.TabControl.TabIndex = 1;
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
            if (Controls.Base.MainEditor.Instance.IsEntitiesEdit())
            {
                if (Controls.Base.MainEditor.Instance.EntitiesToolbar == null)
                {
                    Controls.Base.MainEditor.Instance.EntitiesToolbar = new Controls.Base.Toolbars.EntitiesToolbar.EntitiesToolbar(Classes.Core.Solution.CurrentScene.Objects, Controls.Base.MainEditor.Instance)
                    {
                        SelectedEntity = new Action<int>(x =>
                        {
                            Classes.Core.Solution.Entities.SelectSlot(x);
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
                            Classes.Core.Solution.Entities.Add(x, GetEntitySpawnPoint());
                            Controls.Base.MainEditor.Instance.UndoStack.Push(Classes.Core.Solution.Entities.LastAction);
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
                Controls.Base.MainEditor.Instance.EntitiesToolbar.SelectedEntities = Classes.Core.Solution.Entities.SelectedEntities.Select(x => x.Entity).ToList();
            }
            else
            {
                if (Controls.Base.MainEditor.Instance.EntitiesToolbar != null)
                {
                    Controls.Base.MainEditor.Instance.EntitiesToolbar.Dispose();
                    Controls.Base.MainEditor.Instance.EntitiesToolbar = null;
                }
                if (Classes.Core.Solution.Entities != null && Classes.Core.Solution.Entities.SelectedEntities != null)
                {
                    if (Classes.Core.Solution.Entities.SelectedEntities.Count != 0 && Classes.Core.Solution.Entities.TemporarySelection.Count != 0)
                    {
                        Classes.Core.Solution.Entities.EndTempSelection();
                        Classes.Core.Solution.Entities.Deselect();
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
                    short x = (short)(Classes.Core.SolutionState.LastX / Classes.Core.SolutionState.Zoom);
                    short y = (short)(Classes.Core.SolutionState.LastY / Classes.Core.SolutionState.Zoom);
                    if (Classes.Core.SolutionState.UseMagnetMode)
                    {
                        short alignedX = (short)(Classes.Core.SolutionState.MagnetSize * (x / Classes.Core.SolutionState.MagnetSize));
                        short alignedY = (short)(Classes.Core.SolutionState.MagnetSize * (y / Classes.Core.SolutionState.MagnetSize));
                        return new Position(alignedX, alignedY);
                    }
                    else
                    {
                        return new Position(x, y);
                    }

                }
                else
                {
                    return new Position((short)(Classes.Core.SolutionState.ViewPositionX/ Classes.Core.SolutionState.Zoom), (short)(Classes.Core.SolutionState.ViewPositionY / Classes.Core.SolutionState.Zoom));
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
            Controls.Base.MainEditor.Instance.UI.SetSceneOnlyButtonsState((enabled ? true : Classes.Core.Solution.CurrentScene != null));
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
            if (Controls.Base.MainEditor.Instance.IsTilesEdit() && !Controls.Base.MainEditor.Instance.IsChunksEdit())
            {
                if (Controls.Base.MainEditor.Instance.TilesToolbar != null)
                {
                    List<ushort> values = Classes.Core.Solution.EditLayerA?.GetSelectedValues();
                    List<ushort> valuesB = Classes.Core.Solution.EditLayerB?.GetSelectedValues();
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
            Controls.Base.MainEditor.Instance.EntitiesToolbar.Entities = Classes.Core.Solution.Entities.Entities.Select(x => x.Entity).ToList();
        }
        public void UpdateEditLayerActions()
        {
            if (Classes.Core.Solution.EditLayerA != null)
            {
                List<IAction> actions = Classes.Core.Solution.EditLayerA?.Actions;
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
            if (Classes.Core.Solution.EditLayerB != null)
            {
                List<IAction> actions = Classes.Core.Solution.EditLayerB?.Actions;
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
            Classes.Core.SolutionState.AllowSplineOptionsUpdate = false;
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
                Classes.Core.SolutionState.AllowSplineOptionsUpdate = true;

            }
        }

        public void UpdateSplineSettings(int splineID)
        {
            if (!Classes.Core.SolutionState.SplineOptionsGroup.ContainsKey(splineID)) Classes.Core.SolutionState.SplineOptionsGroup.Add(splineID, new Classes.Core.SolutionState.SplineOptions());
            Controls.Base.MainEditor.Instance.EditorToolbar.SplineLineMode.IsChecked = Classes.Core.SolutionState.SplineOptionsGroup[splineID].SplineLineMode;
            Controls.Base.MainEditor.Instance.EditorToolbar.SplineOvalMode.IsChecked = Classes.Core.SolutionState.SplineOptionsGroup[splineID].SplineOvalMode;
            Controls.Base.MainEditor.Instance.EditorToolbar.SplineShowLineCheckbox.IsChecked = Classes.Core.SolutionState.SplineOptionsGroup[splineID].SplineToolShowLines;
            Controls.Base.MainEditor.Instance.EditorToolbar.SplineShowObjectsCheckbox.IsChecked = Classes.Core.SolutionState.SplineOptionsGroup[splineID].SplineToolShowObject;
            Controls.Base.MainEditor.Instance.EditorToolbar.SplineShowPointsCheckbox.IsChecked = Classes.Core.SolutionState.SplineOptionsGroup[splineID].SplineToolShowPoints;
            Controls.Base.MainEditor.Instance.EditorToolbar.SplinePointSeperationNUD.Value = Classes.Core.SolutionState.SplineOptionsGroup[splineID].SplineSize;
            Controls.Base.MainEditor.Instance.EditorToolbar.SplinePointSeperationSlider.Value = Classes.Core.SolutionState.SplineOptionsGroup[splineID].SplineSize;

            if (Classes.Core.SolutionState.SplineOptionsGroup[splineID].SplineObjectRenderingTemplate != null)
                Controls.Base.MainEditor.Instance.EditorToolbar.SplineRenderObjectName.Content = Classes.Core.SolutionState.SplineOptionsGroup[splineID].SplineObjectRenderingTemplate.Entity.Object.Name.Name;
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
            Controls.Base.MainEditor.Instance.EditorToolbar.CSAC.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(Classes.Core.SolutionState.CollisionSAColour.A, Classes.Core.SolutionState.CollisionSAColour.R, Classes.Core.SolutionState.CollisionSAColour.G, Classes.Core.SolutionState.CollisionSAColour.B));
            Controls.Base.MainEditor.Instance.EditorToolbar.SSTOC.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(Classes.Core.SolutionState.CollisionTOColour.A, Classes.Core.SolutionState.CollisionTOColour.R, Classes.Core.SolutionState.CollisionTOColour.G, Classes.Core.SolutionState.CollisionTOColour.B));
            Controls.Base.MainEditor.Instance.EditorToolbar.CSLRDC.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(Classes.Core.SolutionState.CollisionLRDColour.A, Classes.Core.SolutionState.CollisionLRDColour.R, Classes.Core.SolutionState.CollisionLRDColour.G, Classes.Core.SolutionState.CollisionLRDColour.B));
            Controls.Base.MainEditor.Instance.EditorToolbar.WLC.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(Classes.Core.SolutionState.waterColor.A, Classes.Core.SolutionState.waterColor.R, Classes.Core.SolutionState.waterColor.G, Classes.Core.SolutionState.waterColor.B));
            Controls.Base.MainEditor.Instance.EditorToolbar.GDC.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(Classes.Core.SolutionState.GridColor.A, Classes.Core.SolutionState.GridColor.R, Classes.Core.SolutionState.GridColor.G, Classes.Core.SolutionState.GridColor.B));
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

            bool parallaxAnimationInProgress = Classes.Core.SolutionState.AllowAnimations && Classes.Core.SolutionState.ParallaxAnimationChecked;

            UpdateGameRunningButton(Classes.Core.Solution.CurrentScene != null);
            Controls.Base.MainEditor.Instance.Theming.UpdateThemeForItemsWaiting();
            Controls.Base.MainEditor.Instance.EditorStatusBar.UpdateFilterButtonApperance(false);
            Controls.Base.MainEditor.Instance.EditorStatusBar.UpdateStatusPanel();
            SetSceneOnlyButtonsState(Classes.Core.Solution.CurrentScene != null && !parallaxAnimationInProgress, stageLoad);
            SetParallaxAnimationOnlyButtonsState(parallaxAnimationInProgress);
            UpdateSplineToolbox();
            Controls.Base.MainEditor.Instance.EditorToolbar.CustomGridLabel.Text = string.Format(Controls.Base.MainEditor.Instance.EditorToolbar.CustomGridLabel.Tag.ToString(), Properties.Defaults.Default.CustomGridSizeValue);

        }
        public void UpdateGameRunningButton(bool enabled = true)
        {
            
            Controls.Base.MainEditor.Instance.EditorToolbar.RunSceneButton.IsEnabled = enabled;
            Controls.Base.MainEditor.Instance.EditorToolbar.RunSceneDropDown.IsEnabled = enabled && Controls.Base.MainEditor.Instance.EditorToolbar.RunSceneButton.IsEnabled;

            if (Controls.Base.MainEditor.Instance.InGame.GameRunning || System.Diagnostics.Process.GetProcessesByName("SonicMania").FirstOrDefault() != null)
            {
                if (Controls.Base.MainEditor.Instance.InGame.GameRunning) Controls.Base.MainEditor.Instance.EditorToolbar.RunSceneIcon.Fill = System.Windows.Media.Brushes.Blue;
                else Controls.Base.MainEditor.Instance.EditorToolbar.RunSceneIcon.Fill = System.Windows.Media.Brushes.Green;
            }
            else
            {
                Controls.Base.MainEditor.Instance.EditorToolbar.RunSceneIcon.Fill = System.Windows.Media.Brushes.Gray;
            }
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
                if (Classes.Core.SolutionState.UseEncoreColors == true)
                {
                    Classes.Core.Solution.CurrentTiles.StageTiles?.Image.Reload(Controls.Base.MainEditor.Instance.EncorePalette[0]);
                    Controls.Base.MainEditor.Instance.TilesToolbar?.Reload(Controls.Base.MainEditor.Instance.EncorePalette[0]);
                }
                else
                {
                    Classes.Core.Solution.CurrentTiles.StageTiles?.Image.Reload();
                    Controls.Base.MainEditor.Instance.TilesToolbar?.Reload();
                }

                Classes.Core.Solution.TileConfig = new Tileconfig(Controls.Base.MainEditor.Instance.Paths.TileConfig_Source);



            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        #endregion
    }
}
