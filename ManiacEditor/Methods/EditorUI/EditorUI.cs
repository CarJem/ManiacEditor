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
            Interfaces.Base.MainEditor.Instance.EditorToolbar.ShowFGHigh.IsEnabled = enabled && Classes.Editor.Solution.FGHigh != null;
            Interfaces.Base.MainEditor.Instance.EditorToolbar.ShowFGLow.IsEnabled = enabled && Classes.Editor.Solution.FGLow != null;
            Interfaces.Base.MainEditor.Instance.EditorToolbar.ShowFGHigher.IsEnabled = enabled && Classes.Editor.Solution.FGHigher != null;
            Interfaces.Base.MainEditor.Instance.EditorToolbar.ShowFGLower.IsEnabled = enabled && Classes.Editor.Solution.FGLower != null;
            Interfaces.Base.MainEditor.Instance.EditorToolbar.ShowEntities.IsEnabled = enabled;

            Interfaces.Base.MainEditor.Instance.EditorToolbar.ReloadButton.IsEnabled = enabled;

            Interfaces.Base.MainEditor.Instance.EditorMenuBar.SetSceneOnlyButtonsState(enabled, stageLoad);

            Interfaces.Base.MainEditor.Instance.EditorToolbar.Save.IsEnabled = enabled;

            if (Settings.MyPerformance.ReduceZoom)
            {
                Interfaces.Base.MainEditor.Instance.EditorToolbar.ZoomInButton.IsEnabled = enabled && Classes.Editor.SolutionState.ZoomLevel < 5;
                Interfaces.Base.MainEditor.Instance.EditorToolbar.ZoomOutButton.IsEnabled = enabled && Classes.Editor.SolutionState.ZoomLevel > -2;
            }
            else
            {
                Interfaces.Base.MainEditor.Instance.EditorToolbar.ZoomInButton.IsEnabled = enabled && Classes.Editor.SolutionState.ZoomLevel < 5;
                Interfaces.Base.MainEditor.Instance.EditorToolbar.ZoomOutButton.IsEnabled = enabled && Classes.Editor.SolutionState.ZoomLevel > -5;
            }


            UpdateGameRunningButton(enabled);

            SetEditButtonsState(enabled);
            UpdateTooltips();

            if (stageLoad)
            {
                Interfaces.Base.MainEditor.Instance.ZoomModel.SetViewSize((int)(Classes.Editor.Solution.SceneWidth * Classes.Editor.SolutionState.Zoom), (int)(Classes.Editor.Solution.SceneHeight * Classes.Editor.SolutionState.Zoom));
            }

            Interfaces.Base.MainEditor.Instance.Theming.UpdateButtonColors();

        }

        public void SetParallaxAnimationOnlyButtonsState(bool enabled = true)
        {
            Interfaces.Base.MainEditor.Instance.EditorToolbar.Open.IsEnabled = !enabled;
            Interfaces.Base.MainEditor.Instance.EditorToolbar.ShowAnimations.IsEnabled = enabled || Classes.Editor.Solution.CurrentScene != null;
            Interfaces.Base.MainEditor.Instance.EditorToolbar.animationsSplitButton_Dropdown.IsEnabled = enabled || Classes.Editor.Solution.CurrentScene != null;
            Interfaces.Base.MainEditor.Instance.EditorMenuBar.MenuBar.IsEnabled = !enabled;
            Interfaces.Base.MainEditor.Instance.EditorStatusBar.StatusBar1.IsEnabled = !enabled; 
            Interfaces.Base.MainEditor.Instance.EditorTabControl.IsEnabled = !enabled;
            Interfaces.Base.MainEditor.Instance.EditorToolbar.New.IsEnabled = !enabled;
            Interfaces.Base.MainEditor.Instance.EditorToolbar.Open.IsEnabled = !enabled;

            if (enabled)
            {
                Interfaces.Base.MainEditor.Instance.EditorToolbar.ShowFGHigh.IsEnabled = Classes.Editor.Solution.FGHigh != null;
                Interfaces.Base.MainEditor.Instance.EditorToolbar.ShowFGLow.IsEnabled = Classes.Editor.Solution.FGLow != null;
                Interfaces.Base.MainEditor.Instance.EditorToolbar.ShowFGHigher.IsEnabled = Classes.Editor.Solution.FGHigher != null;
                Interfaces.Base.MainEditor.Instance.EditorToolbar.ShowFGLower.IsEnabled = Classes.Editor.Solution.FGLower != null;
                Interfaces.Base.MainEditor.Instance.EditorToolbar.ShowEntities.IsEnabled = true;
                Interfaces.Base.MainEditor.Instance.LeftToolbarToolbox.SelectedIndex = -1;
                UpdateToolbars(false, false, false);
                SetEditButtonsState(false);
            }
            foreach (var elb in Interfaces.Base.MainEditor.Instance.ExtraLayerEditViewButtons)
            {
                elb.Value.IsEnabled = !enabled;
            }
            Interfaces.Base.MainEditor.Instance.LeftToolbarToolbox.IsEnabled = !enabled;
        }
        public void SetSelectOnlyButtonsState(bool enabled = true)
        {
            Interfaces.Base.MainEditor.Instance.EditorMenuBar.SetPasteButtonsState(true);
            Interfaces.Base.MainEditor.Instance.EditorMenuBar.SetSelectOnlyButtonsState(enabled);
            enabled &= Interfaces.Base.MainEditor.Instance.IsSelected();

            if (Interfaces.Base.MainEditor.Instance.IsEntitiesEdit() && Interfaces.Base.MainEditor.Instance.EntitiesToolbar != null)
            {
                Interfaces.Base.MainEditor.Instance.EntitiesToolbar.SelectedEntities = Classes.Editor.Solution.Entities.SelectedEntities.Select(x => x.Entity).ToList();
            }
        }

        private void SetLayerEditButtonsState(bool enabled)
        {
            if (!Classes.Editor.SolutionState.MultiLayerEditMode)
            {
                if (enabled && Interfaces.Base.MainEditor.Instance.EditorToolbar.EditFGLow.IsCheckedN.Value) Classes.Editor.Solution.EditLayerA = Classes.Editor.Solution.FGLow;
                else if (enabled && Interfaces.Base.MainEditor.Instance.EditorToolbar.EditFGHigh.IsCheckedN.Value) Classes.Editor.Solution.EditLayerA = Classes.Editor.Solution.FGHigh;
                else if (enabled && Interfaces.Base.MainEditor.Instance.EditorToolbar.EditFGHigher.IsCheckedN.Value) Classes.Editor.Solution.EditLayerA = Classes.Editor.Solution.FGHigher;
                else if (enabled && Interfaces.Base.MainEditor.Instance.EditorToolbar.EditFGLower.IsCheckedN.Value) Classes.Editor.Solution.EditLayerA = Classes.Editor.Solution.FGLower;
                else if (enabled && Interfaces.Base.MainEditor.Instance.ExtraLayerEditViewButtons.Any(elb => elb.Value.IsCheckedN.Value))
                {
                    var selectedExtraLayerButton = Interfaces.Base.MainEditor.Instance.ExtraLayerEditViewButtons.Single(elb => elb.Value.IsCheckedN.Value);
                    var editorLayer = Classes.Editor.Solution.CurrentScene.OtherLayers.Single(el => el.Name.Equals(selectedExtraLayerButton.Value.Text));

                    Classes.Editor.Solution.EditLayerA = editorLayer;
                }
                else Classes.Editor.Solution.EditLayerA = null;
            }
            else
            {
                SetEditLayerA();
                SetEditLayerB();
            }

            void SetEditLayerA()
            {
                if (enabled && Interfaces.Base.MainEditor.Instance.EditorToolbar.EditFGLow.IsCheckedA.Value) Classes.Editor.Solution.EditLayerA = Classes.Editor.Solution.FGLow;
                else if (enabled && Interfaces.Base.MainEditor.Instance.EditorToolbar.EditFGHigh.IsCheckedA.Value) Classes.Editor.Solution.EditLayerA = Classes.Editor.Solution.FGHigh;
                else if (enabled && Interfaces.Base.MainEditor.Instance.EditorToolbar.EditFGHigher.IsCheckedA.Value) Classes.Editor.Solution.EditLayerA = Classes.Editor.Solution.FGHigher;
                else if (enabled && Interfaces.Base.MainEditor.Instance.EditorToolbar.EditFGLower.IsCheckedA.Value) Classes.Editor.Solution.EditLayerA = Classes.Editor.Solution.FGLower;
                else if (enabled && Interfaces.Base.MainEditor.Instance.ExtraLayerEditViewButtons.Any(elb => elb.Value.IsCheckedA.Value))
                {
                    var selectedExtraLayerButton = Interfaces.Base.MainEditor.Instance.ExtraLayerEditViewButtons.Single(elb => elb.Value.IsCheckedA.Value);
                    var editorLayer = Classes.Editor.Solution.CurrentScene.OtherLayers.Single(el => el.Name.Equals(selectedExtraLayerButton.Value.Text));

                    Classes.Editor.Solution.EditLayerA = editorLayer;
                }
                else Classes.Editor.Solution.EditLayerA = null;
            }
            void SetEditLayerB()
            {
                if (enabled && Interfaces.Base.MainEditor.Instance.EditorToolbar.EditFGLow.IsCheckedB.Value) Classes.Editor.Solution.EditLayerB = Classes.Editor.Solution.FGLow;
                else if (enabled && Interfaces.Base.MainEditor.Instance.EditorToolbar.EditFGHigh.IsCheckedB.Value) Classes.Editor.Solution.EditLayerB = Classes.Editor.Solution.FGHigh;
                else if (enabled && Interfaces.Base.MainEditor.Instance.EditorToolbar.EditFGHigher.IsCheckedB.Value) Classes.Editor.Solution.EditLayerB = Classes.Editor.Solution.FGHigher;
                else if (enabled && Interfaces.Base.MainEditor.Instance.EditorToolbar.EditFGLower.IsCheckedB.Value) Classes.Editor.Solution.EditLayerB = Classes.Editor.Solution.FGLower;
                else if (enabled && Interfaces.Base.MainEditor.Instance.ExtraLayerEditViewButtons.Any(elb => elb.Value.IsCheckedB.Value))
                {
                    var selectedExtraLayerButton = Interfaces.Base.MainEditor.Instance.ExtraLayerEditViewButtons.Single(elb => elb.Value.IsCheckedB.Value);
                    var editorLayer = Classes.Editor.Solution.CurrentScene.OtherLayers.Single(el => el.Name.Equals(selectedExtraLayerButton.Value.Text));

                    Classes.Editor.Solution.EditLayerB = editorLayer;
                }
                else Classes.Editor.Solution.EditLayerB = null;
            }

        }
        private void SetEditButtonsState(bool enabled)
        {

            Interfaces.Base.MainEditor.Instance.EditorToolbar.EditFGLow.IsEnabled = enabled && Classes.Editor.Solution.FGLow != null;
            Interfaces.Base.MainEditor.Instance.EditorToolbar.EditFGHigh.IsEnabled = enabled && Classes.Editor.Solution.FGHigh != null;
            Interfaces.Base.MainEditor.Instance.EditorToolbar.EditFGLower.IsEnabled = enabled && Classes.Editor.Solution.FGLower != null;
            Interfaces.Base.MainEditor.Instance.EditorToolbar.EditFGHigher.IsEnabled = enabled && Classes.Editor.Solution.FGHigher != null;
            Interfaces.Base.MainEditor.Instance.EditorToolbar.EditEntities.IsEnabled = enabled;

            Interfaces.Base.MainEditor.Instance.EditorToolbar.EditFGLow.IsCheckedA = enabled && Interfaces.Base.MainEditor.Instance.EditorToolbar.EditFGLow.IsCheckedA.Value;
            Interfaces.Base.MainEditor.Instance.EditorToolbar.EditFGHigh.IsCheckedA = enabled && Interfaces.Base.MainEditor.Instance.EditorToolbar.EditFGHigh.IsCheckedA.Value;
            Interfaces.Base.MainEditor.Instance.EditorToolbar.EditFGLower.IsCheckedA = enabled && Interfaces.Base.MainEditor.Instance.EditorToolbar.EditFGLower.IsCheckedA.Value;
            Interfaces.Base.MainEditor.Instance.EditorToolbar.EditFGHigher.IsCheckedA = enabled && Interfaces.Base.MainEditor.Instance.EditorToolbar.EditFGHigher.IsCheckedA.Value;

            Interfaces.Base.MainEditor.Instance.EditorToolbar.EditFGLow.IsCheckedB = enabled && Interfaces.Base.MainEditor.Instance.EditorToolbar.EditFGLow.IsCheckedB.Value;
            Interfaces.Base.MainEditor.Instance.EditorToolbar.EditFGHigh.IsCheckedB = enabled && Interfaces.Base.MainEditor.Instance.EditorToolbar.EditFGHigh.IsCheckedB.Value;
            Interfaces.Base.MainEditor.Instance.EditorToolbar.EditFGLower.IsCheckedB = enabled && Interfaces.Base.MainEditor.Instance.EditorToolbar.EditFGLower.IsCheckedB.Value;
            Interfaces.Base.MainEditor.Instance.EditorToolbar.EditFGHigher.IsCheckedB = enabled && Interfaces.Base.MainEditor.Instance.EditorToolbar.EditFGHigher.IsCheckedB.Value;

            foreach (var layerButtons in Interfaces.Base.MainEditor.Instance.ExtraLayerEditViewButtons)
            {
                layerButtons.Value.IsCheckedA = layerButtons.Value.IsCheckedA.Value && enabled;
                layerButtons.Value.IsCheckedB = layerButtons.Value.IsCheckedB.Value && enabled;
            }

            Interfaces.Base.MainEditor.Instance.EditorToolbar.EditEntities.IsCheckedN = enabled && Interfaces.Base.MainEditor.Instance.EditorToolbar.EditEntities.IsCheckedN.Value;

            Interfaces.Base.MainEditor.Instance.EditorMenuBar.SetEditButtonsState(enabled);

            SetLayerEditButtonsState(enabled);

            Interfaces.Base.MainEditor.Instance.EditorToolbar.MagnetMode.IsEnabled = enabled && Interfaces.Base.MainEditor.Instance.IsEntitiesEdit();
            Interfaces.Base.MainEditor.Instance.EditorToolbar.MagnetMode.IsChecked = Classes.Editor.SolutionState.UseMagnetMode && Interfaces.Base.MainEditor.Instance.IsEntitiesEdit();
            Interfaces.Base.MainEditor.Instance.EditorToolbar.MagnetModeSplitButton.IsEnabled = enabled && Interfaces.Base.MainEditor.Instance.IsEntitiesEdit();
            Classes.Editor.SolutionState.UseMagnetMode = Interfaces.Base.MainEditor.Instance.IsEntitiesEdit() && Interfaces.Base.MainEditor.Instance.EditorToolbar.MagnetMode.IsChecked.Value;



            Interfaces.Base.MainEditor.Instance.EditorToolbar.UndoButton.IsEnabled = enabled && Interfaces.Base.MainEditor.Instance.UndoStack.Count > 0;
            Interfaces.Base.MainEditor.Instance.EditorToolbar.RedoButton.IsEnabled = enabled && Interfaces.Base.MainEditor.Instance.RedoStack.Count > 0;



            Interfaces.Base.MainEditor.Instance.EditorToolbar.PointerToolButton.IsEnabled = enabled;
            Interfaces.Base.MainEditor.Instance.EditorToolbar.SelectToolButton.IsEnabled = enabled && Interfaces.Base.MainEditor.Instance.IsTilesEdit();
            Interfaces.Base.MainEditor.Instance.EditorToolbar.DrawToolButton.IsEnabled = enabled && Interfaces.Base.MainEditor.Instance.IsTilesEdit() || Interfaces.Base.MainEditor.Instance.IsEntitiesEdit();
            Interfaces.Base.MainEditor.Instance.EditorToolbar.InteractionToolButton.IsEnabled = enabled;
            Interfaces.Base.MainEditor.Instance.EditorToolbar.ChunksToolButton.IsEnabled = enabled && Interfaces.Base.MainEditor.Instance.IsTilesEdit();
            Interfaces.Base.MainEditor.Instance.EditorToolbar.SplineToolButton.IsEnabled = enabled && Interfaces.Base.MainEditor.Instance.IsEntitiesEdit();
            Interfaces.Base.MainEditor.Instance.EditorToolbar.SplineToolButton.IsChecked = Interfaces.Base.MainEditor.Instance.EditorToolbar.SplineToolButton.IsChecked.Value && Interfaces.Base.MainEditor.Instance.IsEntitiesEdit();

            bool isAnyOtherToolChecked()
            {
                bool isPointer = (bool)Interfaces.Base.MainEditor.Instance.EditorToolbar.PointerToolButton.IsChecked.Value;
                bool isSelect = (bool)Interfaces.Base.MainEditor.Instance.EditorToolbar.SelectToolButton.IsChecked.Value;
                bool isDraw = (bool)Interfaces.Base.MainEditor.Instance.EditorToolbar.DrawToolButton.IsChecked.Value;
                bool isSpline = (bool)Interfaces.Base.MainEditor.Instance.EditorToolbar.SplineToolButton.IsChecked.Value;

                if (Interfaces.Base.MainEditor.Instance.IsEntitiesEdit())
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


            Interfaces.Base.MainEditor.Instance.EditorToolbar.PointerToolButton.IsChecked = isAnyOtherToolChecked();
            Interfaces.Base.MainEditor.Instance.EditorToolbar.ChunksToolButton.IsChecked = (bool)Interfaces.Base.MainEditor.Instance.EditorToolbar.ChunksToolButton.IsChecked && !Interfaces.Base.MainEditor.Instance.IsEntitiesEdit();
            if (Interfaces.Base.MainEditor.Instance.TilesToolbar != null)
            {
                if (Interfaces.Base.MainEditor.Instance.EditorToolbar.ChunksToolButton.IsChecked.Value)
                {
                    Interfaces.Base.MainEditor.Instance.TilesToolbar.TabControl.SelectedIndex = 1;
                }
                else
                {
                    Interfaces.Base.MainEditor.Instance.TilesToolbar.TabControl.SelectedIndex = 0;
                }
            }

            Interfaces.Base.MainEditor.Instance.EditorToolbar.ShowGridButton.IsEnabled = enabled && Classes.Editor.Solution.StageConfig != null;
            Interfaces.Base.MainEditor.Instance.EditorToolbar.ShowCollisionAButton.IsEnabled = enabled && Classes.Editor.Solution.TileConfig != null;
            Interfaces.Base.MainEditor.Instance.EditorToolbar.ShowCollisionBButton.IsEnabled = enabled && Classes.Editor.Solution.TileConfig != null;
            Interfaces.Base.MainEditor.Instance.EditorToolbar.ShowTileIDButton.IsEnabled = enabled && Classes.Editor.Solution.StageConfig != null;
            Interfaces.Base.MainEditor.Instance.EditorToolbar.EncorePaletteButton.IsEnabled = enabled && Classes.Editor.SolutionState.EncorePaletteExists;
            Interfaces.Base.MainEditor.Instance.EditorToolbar.FlipAssistButton.IsEnabled = enabled;

            if (Interfaces.Base.MainEditor.Instance.IsTilesEdit())
            {
                if (Interfaces.Base.MainEditor.Instance.TilesToolbar == null)
                {
                    if (Classes.Editor.SolutionState.UseEncoreColors)
                        Interfaces.Base.MainEditor.Instance.TilesToolbar = new TilesToolbar(Classes.Editor.Solution.CurrentTiles.StageTiles, Interfaces.Base.MainEditor.Instance.Paths.StageTiles_Source, Interfaces.Base.MainEditor.Instance.EncorePalette[0], Interfaces.Base.MainEditor.Instance);
                    else
                        Interfaces.Base.MainEditor.Instance.TilesToolbar = new TilesToolbar(Classes.Editor.Solution.CurrentTiles.StageTiles, Interfaces.Base.MainEditor.Instance.Paths.StageTiles_Source, null, Interfaces.Base.MainEditor.Instance);


                    Interfaces.Base.MainEditor.Instance.TilesToolbar.TileDoubleClick = new Action<int>(x =>
                    {
                        Interfaces.Base.MainEditor.Instance.EditorPlaceTile(new System.Drawing.Point((int)(Classes.Editor.SolutionState.ViewPositionX/ Classes.Editor.SolutionState.Zoom) + Classes.Editor.Constants.TILE_SIZE - 1, (int)(Classes.Editor.SolutionState.ViewPositionY / Classes.Editor.SolutionState.Zoom) + Classes.Editor.Constants.TILE_SIZE - 1), x, Classes.Editor.Solution.EditLayerA);
                    });
                    Interfaces.Base.MainEditor.Instance.TilesToolbar.TileOptionChanged = new Action<int, bool>((option, state) =>
                    {
                        Classes.Editor.Solution.EditLayerA?.SetPropertySelected(option + 12, state);
                        Classes.Editor.Solution.EditLayerB?.SetPropertySelected(option + 12, state);

                    });
                    Interfaces.Base.MainEditor.Instance.ToolBarPanelRight.Children.Clear();
                    Interfaces.Base.MainEditor.Instance.ToolBarPanelRight.Children.Add(Interfaces.Base.MainEditor.Instance.TilesToolbar);
                    UpdateToolbars(true, true);
                    Interfaces.Base.MainEditor.Instance.Editor_Resize(null, null);
                    Interfaces.Base.MainEditor.Instance.Focus();
                }
                if (Interfaces.Base.MainEditor.Instance.IsChunksEdit()) Interfaces.Base.MainEditor.Instance.TilesToolbar.TabControl.TabIndex = 1;
                else Interfaces.Base.MainEditor.Instance.TilesToolbar.TabControl.TabIndex = 0;
                Interfaces.Base.MainEditor.Instance.UI.UpdateTilesOptions();
                Interfaces.Base.MainEditor.Instance.TilesToolbar.ShowShortcuts = Interfaces.Base.MainEditor.Instance.EditorToolbar.DrawToolButton.IsChecked.Value;
            }
            else
            {
                if (Interfaces.Base.MainEditor.Instance.TilesToolbar != null)
                {
                    Interfaces.Base.MainEditor.Instance.TilesToolbar.Dispose();
                    Interfaces.Base.MainEditor.Instance.TilesToolbar = null;
                    Interfaces.Base.MainEditor.Instance.Focus();
                }
            }
            if (Interfaces.Base.MainEditor.Instance.IsEntitiesEdit())
            {
                if (Interfaces.Base.MainEditor.Instance.EntitiesToolbar == null)
                {
                    Interfaces.Base.MainEditor.Instance.EntitiesToolbar = new EntitiesToolbar(Classes.Editor.Solution.CurrentScene.Objects, Interfaces.Base.MainEditor.Instance)
                    {
                        SelectedEntity = new Action<int>(x =>
                        {
                            Classes.Editor.Solution.Entities.SelectSlot(x);
                            SetSelectOnlyButtonsState();
                        }),
                        AddAction = new Action<ManiacEditor.Actions.IAction>(x =>
                        {
                            Interfaces.Base.MainEditor.Instance.UndoStack.Push(x);
                            Interfaces.Base.MainEditor.Instance.RedoStack.Clear();
                            UpdateControls();
                        }),
                        Spawn = new Action<SceneObject>(x =>
                        {
                            Classes.Editor.Solution.Entities.Add(x, GetEntitySpawnPoint());
                            Interfaces.Base.MainEditor.Instance.UndoStack.Push(Classes.Editor.Solution.Entities.LastAction);
                            Interfaces.Base.MainEditor.Instance.RedoStack.Clear();
                            UpdateControls();
                        })
                    };
                    Interfaces.Base.MainEditor.Instance.ToolBarPanelRight.Children.Clear();
                    Interfaces.Base.MainEditor.Instance.ToolBarPanelRight.Children.Add(Interfaces.Base.MainEditor.Instance.EntitiesToolbar);
                    UpdateToolbars(true, true);
                    Interfaces.Base.MainEditor.Instance.Editor_Resize(null, null);
                }
                Interfaces.Base.MainEditor.Instance.UI.UpdateEntitiesToolbarList();
                Interfaces.Base.MainEditor.Instance.EntitiesToolbar.SelectedEntities = Classes.Editor.Solution.Entities.SelectedEntities.Select(x => x.Entity).ToList();
            }
            else
            {
                if (Interfaces.Base.MainEditor.Instance.EntitiesToolbar != null)
                {
                    Interfaces.Base.MainEditor.Instance.EntitiesToolbar.Dispose();
                    Interfaces.Base.MainEditor.Instance.EntitiesToolbar = null;
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
            if (Interfaces.Base.MainEditor.Instance.TilesToolbar == null && Interfaces.Base.MainEditor.Instance.EntitiesToolbar == null && (Interfaces.Base.MainEditor.Instance.ToolBarPanelRight.Children.Count != 0))
            {
                Interfaces.Base.MainEditor.Instance.ToolBarPanelRight.Children.Clear();
                UpdateToolbars(true, false);
                Interfaces.Base.MainEditor.Instance.Editor_Resize(null, null);
            }

            SetSelectOnlyButtonsState(enabled);

            Position GetEntitySpawnPoint()
            {
                if (Interfaces.Base.MainEditor.Instance.EditorToolbar.DrawToolButton.IsChecked.Value)
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
            Interfaces.Base.MainEditor.Instance.EditorMenuBar.MenuBar.IsEnabled = enabled;
            Interfaces.Base.MainEditor.Instance.EditorToolbar.LayerToolbar.IsEnabled = enabled;
            Interfaces.Base.MainEditor.Instance.EditorToolbar.MainToolbarButtons.IsEnabled = enabled;
            Interfaces.Base.MainEditor.Instance.UI.SetSceneOnlyButtonsState((enabled ? true : Classes.Editor.Solution.CurrentScene != null));
            Interfaces.Base.MainEditor.Instance.EditorToolbar.LayerToolbar.IsEnabled = enabled;
            Interfaces.Base.MainEditor.Instance.EditorStatusBar.StatusBar1.IsEnabled = enabled;
            Interfaces.Base.MainEditor.Instance.EditorStatusBar.StatusBar2.IsEnabled = enabled;
            if (Interfaces.Base.MainEditor.Instance.TilesToolbar != null) Interfaces.Base.MainEditor.Instance.TilesToolbar.IsEnabled = enabled;
            if (Interfaces.Base.MainEditor.Instance.EntitiesToolbar != null) Interfaces.Base.MainEditor.Instance.EntitiesToolbar.IsEnabled = enabled;
            if (isParallaxAnimation)
            {
                Interfaces.Base.MainEditor.Instance.EditorToolbar.LayerToolbar.IsEnabled = true;
                foreach (var pair in Interfaces.Base.MainEditor.Instance.ExtraLayerEditViewButtons)
                {
                    pair.Key.IsEnabled = false;
                    pair.Value.IsEnabled = true;
                }
                Interfaces.Base.MainEditor.Instance.EditorToolbar.EditFGHigh.IsEnabled = false;
                Interfaces.Base.MainEditor.Instance.EditorToolbar.EditFGHigher.IsEnabled = false;
                Interfaces.Base.MainEditor.Instance.EditorToolbar.EditFGLow.IsEnabled = false;
                Interfaces.Base.MainEditor.Instance.EditorToolbar.EditFGLower.IsEnabled = false;

            }
        }
        public void UpdateTilesOptions()
        {
            if (Interfaces.Base.MainEditor.Instance.IsTilesEdit() && !Interfaces.Base.MainEditor.Instance.IsChunksEdit())
            {
                if (Interfaces.Base.MainEditor.Instance.TilesToolbar != null)
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
                            Interfaces.Base.MainEditor.Instance.TilesToolbar.SetTileOptionState(i, unk ? TilesToolbar.TileOptionState.Indeterminate : set ? TilesToolbar.TileOptionState.Checked : TilesToolbar.TileOptionState.Unchcked);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < 4; ++i)
                            Interfaces.Base.MainEditor.Instance.TilesToolbar.SetTileOptionState(i, TilesToolbar.TileOptionState.Disabled);
                    }
                }

            }
        }
        public void UpdateEntitiesToolbarList()
        {
            Interfaces.Base.MainEditor.Instance.EntitiesToolbar.Entities = Classes.Editor.Solution.Entities.Entities.Select(x => x.Entity).ToList();
        }
        public void UpdateEditLayerActions()
        {
            if (Classes.Editor.Solution.EditLayerA != null)
            {
                List<IAction> actions = Classes.Editor.Solution.EditLayerA?.Actions;
                if (actions.Count > 0) Interfaces.Base.MainEditor.Instance.RedoStack.Clear();
                while (actions.Count > 0)
                {
                    bool create_new = false;
                    if (Interfaces.Base.MainEditor.Instance.UndoStack.Count == 0 || !(Interfaces.Base.MainEditor.Instance.UndoStack.Peek() is ActionsGroup))
                    {
                        create_new = true;
                    }
                    else
                    {
                        create_new = (Interfaces.Base.MainEditor.Instance.UndoStack.Peek() as ActionsGroup).IsClosed;
                    }
                    if (create_new)
                    {
                        Interfaces.Base.MainEditor.Instance.UndoStack.Push(new ActionsGroup());
                    }
                    (Interfaces.Base.MainEditor.Instance.UndoStack.Peek() as ActionsGroup).AddAction(actions[0]);
                    actions.RemoveAt(0);
                }
            }
            if (Classes.Editor.Solution.EditLayerB != null)
            {
                List<IAction> actions = Classes.Editor.Solution.EditLayerB?.Actions;
                if (actions.Count > 0) Interfaces.Base.MainEditor.Instance.RedoStack.Clear();
                while (actions.Count > 0)
                {
                    bool create_new = false;
                    if (Interfaces.Base.MainEditor.Instance.UndoStack.Count == 0 || !(Interfaces.Base.MainEditor.Instance.UndoStack.Peek() is ActionsGroup))
                    {
                        create_new = true;
                    }
                    else
                    {
                        create_new = (Interfaces.Base.MainEditor.Instance.UndoStack.Peek() as ActionsGroup).IsClosed;
                    }
                    if (create_new)
                    {
                        Interfaces.Base.MainEditor.Instance.UndoStack.Push(new ActionsGroup());
                    }
                    (Interfaces.Base.MainEditor.Instance.UndoStack.Peek() as ActionsGroup).AddAction(actions[0]);
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
                    Interfaces.Base.MainEditor.Instance.ToolbarRight.Width = new GridLength(300);
                    Interfaces.Base.MainEditor.Instance.ToolbarRight.MinWidth = 300;
                    Interfaces.Base.MainEditor.Instance.ToolbarRight.MaxWidth = Interfaces.Base.MainEditor.Instance.ViewPanelForm.ActualWidth / 3;
                    Interfaces.Base.MainEditor.Instance.SplitterRight.Width = new GridLength(6);
                    Interfaces.Base.MainEditor.Instance.SplitterRight.MinWidth = 6;
                }
                else
                {
                    Interfaces.Base.MainEditor.Instance.ToolbarRight.Width = new GridLength(0);
                    Interfaces.Base.MainEditor.Instance.ToolbarRight.MinWidth = 0;
                    Interfaces.Base.MainEditor.Instance.ToolbarRight.MaxWidth = 0;
                    Interfaces.Base.MainEditor.Instance.SplitterRight.Width = new GridLength(0);
                    Interfaces.Base.MainEditor.Instance.SplitterRight.MinWidth = 0;
                }
            }

            else
            {
                if (visible)
                {
                    Interfaces.Base.MainEditor.Instance.ToolbarLeft.Width = new GridLength(200);
                    Interfaces.Base.MainEditor.Instance.ToolbarLeft.MinWidth = 200;
                    Interfaces.Base.MainEditor.Instance.ToolbarLeft.MaxWidth = Interfaces.Base.MainEditor.Instance.ViewPanelForm.ActualWidth / 3;
                    Interfaces.Base.MainEditor.Instance.SplitterLeft.Width = new GridLength(3);
                    Interfaces.Base.MainEditor.Instance.SplitterLeft.MinWidth = 3;
                    Interfaces.Base.MainEditor.Instance.LeftToolbarToolbox.Visibility = Visibility.Visible;
                }
                else
                {
                    if (!fullCollapse)
                    {
                        Interfaces.Base.MainEditor.Instance.ToolbarLeft.Width = new GridLength(10);
                        Interfaces.Base.MainEditor.Instance.ToolbarLeft.MinWidth = 10;
                        Interfaces.Base.MainEditor.Instance.ToolbarLeft.MaxWidth = 10;
                        Interfaces.Base.MainEditor.Instance.SplitterLeft.Width = new GridLength(0);
                        Interfaces.Base.MainEditor.Instance.SplitterLeft.MinWidth = 0;
                        Interfaces.Base.MainEditor.Instance.LeftToolbarToolbox.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        Interfaces.Base.MainEditor.Instance.ToolbarLeft.Width = new GridLength(0);
                        Interfaces.Base.MainEditor.Instance.ToolbarLeft.MinWidth = 0;
                        Interfaces.Base.MainEditor.Instance.ToolbarLeft.MaxWidth = 0;
                        Interfaces.Base.MainEditor.Instance.SplitterLeft.Width = new GridLength(0);
                        Interfaces.Base.MainEditor.Instance.SplitterLeft.MinWidth = 0;
                        Interfaces.Base.MainEditor.Instance.LeftToolbarToolbox.Visibility = Visibility.Hidden;
                    }

                }
            }

        }
        public void UpdateWaitingScreen(bool show)
        {
            if (show)
            {
                Interfaces.Base.MainEditor.Instance.ViewPanelForm.Visibility = Visibility.Hidden;
                Interfaces.Base.MainEditor.Instance.WaitingPanel.Visibility = Visibility.Visible;
            }
            else
            {
                Interfaces.Base.MainEditor.Instance.ViewPanelForm.Visibility = Visibility.Visible;
                Interfaces.Base.MainEditor.Instance.WaitingPanel.Visibility = Visibility.Collapsed;
            }

        }

        public void UpdateSplineSpawnObjectsList(List<RSDKv5.SceneObject> sceneObjects)
        {
            Classes.Editor.SolutionState.AllowSplineOptionsUpdate = false;
            sceneObjects.Sort((x, y) => x.Name.ToString().CompareTo(y.Name.ToString()));
            var bindingSceneObjectsList = new System.ComponentModel.BindingList<RSDKv5.SceneObject>(sceneObjects);


            Interfaces.Base.MainEditor.Instance.SplineSelectedObjectSpawnList.Clear();
            foreach (var _object in bindingSceneObjectsList)
            {
                TextBlock item = new TextBlock()
                {
                    Tag = _object,
                    Text = _object.Name.Name
                };
                Interfaces.Base.MainEditor.Instance.SplineSelectedObjectSpawnList.Add(item);
            }

            if (Interfaces.Base.MainEditor.Instance.SplineSelectedObjectSpawnList != null && Interfaces.Base.MainEditor.Instance.SplineSelectedObjectSpawnList.Count > 1)
            {
                Interfaces.Base.MainEditor.Instance.EditorToolbar.SelectedSplineRender.ItemsSource = Interfaces.Base.MainEditor.Instance.SplineSelectedObjectSpawnList;
                Interfaces.Base.MainEditor.Instance.EditorToolbar.SelectedSplineRender.SelectedItem = Interfaces.Base.MainEditor.Instance.EditorToolbar.SelectedSplineRender.Items[0];
                var SelectedItem = Interfaces.Base.MainEditor.Instance.EditorToolbar.SelectedSplineRender.SelectedItem as TextBlock;
                if (SelectedItem == null) return;              
                SelectedItem.Foreground = (System.Windows.Media.SolidColorBrush)Interfaces.Base.MainEditor.Instance.FindResource("NormalText");
                Classes.Editor.SolutionState.AllowSplineOptionsUpdate = true;

            }
        }

        public void UpdateSplineSettings(int splineID)
        {
            if (!Classes.Editor.SolutionState.SplineOptionsGroup.ContainsKey(splineID)) Classes.Editor.SolutionState.SplineOptionsGroup.Add(splineID, new Classes.Editor.SolutionState.SplineOptions());
            Interfaces.Base.MainEditor.Instance.EditorToolbar.SplineLineMode.IsChecked = Classes.Editor.SolutionState.SplineOptionsGroup[splineID].SplineLineMode;
            Interfaces.Base.MainEditor.Instance.EditorToolbar.SplineOvalMode.IsChecked = Classes.Editor.SolutionState.SplineOptionsGroup[splineID].SplineOvalMode;
            Interfaces.Base.MainEditor.Instance.EditorToolbar.SplineShowLineCheckbox.IsChecked = Classes.Editor.SolutionState.SplineOptionsGroup[splineID].SplineToolShowLines;
            Interfaces.Base.MainEditor.Instance.EditorToolbar.SplineShowObjectsCheckbox.IsChecked = Classes.Editor.SolutionState.SplineOptionsGroup[splineID].SplineToolShowObject;
            Interfaces.Base.MainEditor.Instance.EditorToolbar.SplineShowPointsCheckbox.IsChecked = Classes.Editor.SolutionState.SplineOptionsGroup[splineID].SplineToolShowPoints;
            Interfaces.Base.MainEditor.Instance.EditorToolbar.SplinePointSeperationNUD.Value = Classes.Editor.SolutionState.SplineOptionsGroup[splineID].SplineSize;
            Interfaces.Base.MainEditor.Instance.EditorToolbar.SplinePointSeperationSlider.Value = Classes.Editor.SolutionState.SplineOptionsGroup[splineID].SplineSize;

            if (Classes.Editor.SolutionState.SplineOptionsGroup[splineID].SplineObjectRenderingTemplate != null)
                Interfaces.Base.MainEditor.Instance.EditorToolbar.SplineRenderObjectName.Content = Classes.Editor.SolutionState.SplineOptionsGroup[splineID].SplineObjectRenderingTemplate.Entity.Object.Name.Name;
            else
                Interfaces.Base.MainEditor.Instance.EditorToolbar.SplineRenderObjectName.Content = "None";
        }

        public void UpdateSplineToolbox()
        {
            //Editor.Instance.SplineInfoLabel1.Text = string.Format("Number of Spline Objects: {0}", Editor.Instance.UIModes.SplineTotalNumberOfObjects);
            //Editor.Instance.SplineInfoLabel2.Text = string.Format("Point Frequency: {0}", Editor.Instance.UIModes.SplineSize);
            //Editor.Instance.SplineInfoLabel3.Text = string.Format("Total Number of Rendered Points: {0}", Editor.Instance.UIModes.SplineCurrentPointsDrawn);
        }

        public void UpdateCustomColors()
        {
            Interfaces.Base.MainEditor.Instance.EditorToolbar.CSAC.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(Classes.Editor.SolutionState.CollisionSAColour.A, Classes.Editor.SolutionState.CollisionSAColour.R, Classes.Editor.SolutionState.CollisionSAColour.G, Classes.Editor.SolutionState.CollisionSAColour.B));
            Interfaces.Base.MainEditor.Instance.EditorToolbar.SSTOC.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(Classes.Editor.SolutionState.CollisionTOColour.A, Classes.Editor.SolutionState.CollisionTOColour.R, Classes.Editor.SolutionState.CollisionTOColour.G, Classes.Editor.SolutionState.CollisionTOColour.B));
            Interfaces.Base.MainEditor.Instance.EditorToolbar.CSLRDC.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(Classes.Editor.SolutionState.CollisionLRDColour.A, Classes.Editor.SolutionState.CollisionLRDColour.R, Classes.Editor.SolutionState.CollisionLRDColour.G, Classes.Editor.SolutionState.CollisionLRDColour.B));
            Interfaces.Base.MainEditor.Instance.EditorToolbar.WLC.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(Classes.Editor.SolutionState.waterColor.A, Classes.Editor.SolutionState.waterColor.R, Classes.Editor.SolutionState.waterColor.G, Classes.Editor.SolutionState.waterColor.B));
            Interfaces.Base.MainEditor.Instance.EditorToolbar.GDC.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(Classes.Editor.SolutionState.GridColor.A, Classes.Editor.SolutionState.GridColor.R, Classes.Editor.SolutionState.GridColor.G, Classes.Editor.SolutionState.GridColor.B));
        }

        public void UpdateControls(bool stageLoad = false)
        {
            if (Settings.MySettings.EntityFreeCam)
            {
                Interfaces.Base.MainEditor.Instance.FormsModel.vScrollBar1.IsEnabled = false;
                Interfaces.Base.MainEditor.Instance.FormsModel.hScrollBar1.IsEnabled = false;
            }
            else
            {
                Interfaces.Base.MainEditor.Instance.FormsModel.vScrollBar1.IsEnabled = true;
                Interfaces.Base.MainEditor.Instance.FormsModel.hScrollBar1.IsEnabled = true;
            }

            bool parallaxAnimationInProgress = Classes.Editor.SolutionState.AllowAnimations && Classes.Editor.SolutionState.ParallaxAnimationChecked;

            UpdateGameRunningButton(Classes.Editor.Solution.CurrentScene != null);
            Interfaces.Base.MainEditor.Instance.Theming.UpdateThemeForItemsWaiting();
            Interfaces.Base.MainEditor.Instance.EditorStatusBar.UpdateFilterButtonApperance(false);
            Interfaces.Base.MainEditor.Instance.EditorStatusBar.UpdateStatusPanel();
            SetSceneOnlyButtonsState(Classes.Editor.Solution.CurrentScene != null && !parallaxAnimationInProgress, stageLoad);
            SetParallaxAnimationOnlyButtonsState(parallaxAnimationInProgress);
            UpdateSplineToolbox();
            Interfaces.Base.MainEditor.Instance.EditorToolbar.CustomGridLabel.Text = string.Format(Interfaces.Base.MainEditor.Instance.EditorToolbar.CustomGridLabel.Tag.ToString(), Properties.Defaults.Default.CustomGridSizeValue);

        }
        public void UpdateGameRunningButton(bool enabled = true)
        {
            
            Interfaces.Base.MainEditor.Instance.EditorToolbar.RunSceneButton.IsEnabled = enabled;
            Interfaces.Base.MainEditor.Instance.EditorToolbar.RunSceneDropDown.IsEnabled = enabled && Interfaces.Base.MainEditor.Instance.EditorToolbar.RunSceneButton.IsEnabled;

            if (Interfaces.Base.MainEditor.Instance.InGame.GameRunning || System.Diagnostics.Process.GetProcessesByName("SonicMania").FirstOrDefault() != null)
            {
                if (Interfaces.Base.MainEditor.Instance.InGame.GameRunning) Interfaces.Base.MainEditor.Instance.EditorToolbar.RunSceneIcon.Fill = System.Windows.Media.Brushes.Blue;
                else Interfaces.Base.MainEditor.Instance.EditorToolbar.RunSceneIcon.Fill = System.Windows.Media.Brushes.Green;
            }
            else
            {
                Interfaces.Base.MainEditor.Instance.EditorToolbar.RunSceneIcon.Fill = System.Windows.Media.Brushes.Gray;
            }
        }
        private void UpdateTooltips()
        {
            UpdateTooltipForStacks(Interfaces.Base.MainEditor.Instance.EditorToolbar.UndoButton, Interfaces.Base.MainEditor.Instance.UndoStack);
            UpdateTooltipForStacks(Interfaces.Base.MainEditor.Instance.EditorToolbar.RedoButton, Interfaces.Base.MainEditor.Instance.RedoStack);
            UpdateTextBlockForStacks(Interfaces.Base.MainEditor.Instance.EditorMenuBar.UndoMenuItemInfo, Interfaces.Base.MainEditor.Instance.UndoStack);
            UpdateTextBlockForStacks(Interfaces.Base.MainEditor.Instance.EditorMenuBar.RedoMenuItemInfo, Interfaces.Base.MainEditor.Instance.RedoStack);
            if (Interfaces.Base.MainEditor.Instance.EditorControls != null)
            {
                if (Interfaces.Base.MainEditor.Instance.IsVisible)
                {
                    Interfaces.Base.MainEditor.Instance.EditorMenuBar.UpdateMenuItems();
                    Interfaces.Base.MainEditor.Instance.EditorControls.UpdateTooltips();
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
                Interfaces.Base.MainEditor.Instance.DisposeTextures();
                Interfaces.Base.MainEditor.Instance.EntityDrawing.ReleaseResources();
                //EditorEntity_ini.rendersWithErrors.Clear();

                //Reload for Encore Palletes, otherwise reload the image normally
                if (Classes.Editor.SolutionState.UseEncoreColors == true)
                {
                    Classes.Editor.Solution.CurrentTiles.StageTiles?.Image.Reload(Interfaces.Base.MainEditor.Instance.EncorePalette[0]);
                    Interfaces.Base.MainEditor.Instance.TilesToolbar?.Reload(Interfaces.Base.MainEditor.Instance.EncorePalette[0]);
                }
                else
                {
                    Classes.Editor.Solution.CurrentTiles.StageTiles?.Image.Reload();
                    Interfaces.Base.MainEditor.Instance.TilesToolbar?.Reload();
                }

                Classes.Editor.Solution.TileConfig = new Tileconfig(Interfaces.Base.MainEditor.Instance.Paths.TileConfig_Source);



            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        #endregion
    }
}
