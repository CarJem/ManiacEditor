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
            Editor.Instance.EditorToolbar.ShowFGHigh.IsEnabled = enabled && Classes.Editor.Solution.FGHigh != null;
            Editor.Instance.EditorToolbar.ShowFGLow.IsEnabled = enabled && Classes.Editor.Solution.FGLow != null;
            Editor.Instance.EditorToolbar.ShowFGHigher.IsEnabled = enabled && Classes.Editor.Solution.FGHigher != null;
            Editor.Instance.EditorToolbar.ShowFGLower.IsEnabled = enabled && Classes.Editor.Solution.FGLower != null;
            Editor.Instance.EditorToolbar.ShowEntities.IsEnabled = enabled;

            Editor.Instance.EditorToolbar.ReloadButton.IsEnabled = enabled;

            Editor.Instance.EditorMenuBar.SetSceneOnlyButtonsState(enabled, stageLoad);

            Editor.Instance.EditorToolbar.Save.IsEnabled = enabled;

            if (Settings.MyPerformance.ReduceZoom)
            {
                Editor.Instance.EditorToolbar.ZoomInButton.IsEnabled = enabled && Classes.Editor.SolutionState.ZoomLevel < 5;
                Editor.Instance.EditorToolbar.ZoomOutButton.IsEnabled = enabled && Classes.Editor.SolutionState.ZoomLevel > -2;
            }
            else
            {
                Editor.Instance.EditorToolbar.ZoomInButton.IsEnabled = enabled && Classes.Editor.SolutionState.ZoomLevel < 5;
                Editor.Instance.EditorToolbar.ZoomOutButton.IsEnabled = enabled && Classes.Editor.SolutionState.ZoomLevel > -5;
            }


            UpdateGameRunningButton(enabled);

            SetEditButtonsState(enabled);
            UpdateTooltips();

            if (stageLoad)
            {
                Editor.Instance.ZoomModel.SetViewSize((int)(Classes.Editor.Solution.SceneWidth * Classes.Editor.SolutionState.Zoom), (int)(Classes.Editor.Solution.SceneHeight * Classes.Editor.SolutionState.Zoom));
            }

            Editor.Instance.Theming.UpdateButtonColors();

        }

        public void SetParallaxAnimationOnlyButtonsState(bool enabled = true)
        {
            Editor.Instance.EditorToolbar.Open.IsEnabled = !enabled;
            Editor.Instance.EditorToolbar.ShowAnimations.IsEnabled = enabled || Classes.Editor.Solution.CurrentScene != null;
            Editor.Instance.EditorToolbar.animationsSplitButton_Dropdown.IsEnabled = enabled || Classes.Editor.Solution.CurrentScene != null;
            Editor.Instance.EditorMenuBar.MenuBar.IsEnabled = !enabled;
            Editor.Instance.EditorStatusBar.StatusBar1.IsEnabled = !enabled; 
            Editor.Instance.EditorTabControl.IsEnabled = !enabled;
            Editor.Instance.EditorToolbar.New.IsEnabled = !enabled;
            Editor.Instance.EditorToolbar.Open.IsEnabled = !enabled;

            if (enabled)
            {
                Editor.Instance.EditorToolbar.ShowFGHigh.IsEnabled = Classes.Editor.Solution.FGHigh != null;
                Editor.Instance.EditorToolbar.ShowFGLow.IsEnabled = Classes.Editor.Solution.FGLow != null;
                Editor.Instance.EditorToolbar.ShowFGHigher.IsEnabled = Classes.Editor.Solution.FGHigher != null;
                Editor.Instance.EditorToolbar.ShowFGLower.IsEnabled = Classes.Editor.Solution.FGLower != null;
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
                Editor.Instance.EntitiesToolbar.SelectedEntities = Classes.Editor.Solution.Entities.SelectedEntities.Select(x => x.Entity).ToList();
            }
        }

        private void SetLayerEditButtonsState(bool enabled)
        {
            if (!Classes.Editor.SolutionState.MultiLayerEditMode)
            {
                if (enabled && Editor.Instance.EditorToolbar.EditFGLow.IsCheckedN.Value) Classes.Editor.Solution.EditLayerA = Classes.Editor.Solution.FGLow;
                else if (enabled && Editor.Instance.EditorToolbar.EditFGHigh.IsCheckedN.Value) Classes.Editor.Solution.EditLayerA = Classes.Editor.Solution.FGHigh;
                else if (enabled && Editor.Instance.EditorToolbar.EditFGHigher.IsCheckedN.Value) Classes.Editor.Solution.EditLayerA = Classes.Editor.Solution.FGHigher;
                else if (enabled && Editor.Instance.EditorToolbar.EditFGLower.IsCheckedN.Value) Classes.Editor.Solution.EditLayerA = Classes.Editor.Solution.FGLower;
                else if (enabled && Editor.Instance.ExtraLayerEditViewButtons.Any(elb => elb.Value.IsCheckedN.Value))
                {
                    var selectedExtraLayerButton = Editor.Instance.ExtraLayerEditViewButtons.Single(elb => elb.Value.IsCheckedN.Value);
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
                if (enabled && Editor.Instance.EditorToolbar.EditFGLow.IsCheckedA.Value) Classes.Editor.Solution.EditLayerA = Classes.Editor.Solution.FGLow;
                else if (enabled && Editor.Instance.EditorToolbar.EditFGHigh.IsCheckedA.Value) Classes.Editor.Solution.EditLayerA = Classes.Editor.Solution.FGHigh;
                else if (enabled && Editor.Instance.EditorToolbar.EditFGHigher.IsCheckedA.Value) Classes.Editor.Solution.EditLayerA = Classes.Editor.Solution.FGHigher;
                else if (enabled && Editor.Instance.EditorToolbar.EditFGLower.IsCheckedA.Value) Classes.Editor.Solution.EditLayerA = Classes.Editor.Solution.FGLower;
                else if (enabled && Editor.Instance.ExtraLayerEditViewButtons.Any(elb => elb.Value.IsCheckedA.Value))
                {
                    var selectedExtraLayerButton = Editor.Instance.ExtraLayerEditViewButtons.Single(elb => elb.Value.IsCheckedA.Value);
                    var editorLayer = Classes.Editor.Solution.CurrentScene.OtherLayers.Single(el => el.Name.Equals(selectedExtraLayerButton.Value.Text));

                    Classes.Editor.Solution.EditLayerA = editorLayer;
                }
                else Classes.Editor.Solution.EditLayerA = null;
            }
            void SetEditLayerB()
            {
                if (enabled && Editor.Instance.EditorToolbar.EditFGLow.IsCheckedB.Value) Classes.Editor.Solution.EditLayerB = Classes.Editor.Solution.FGLow;
                else if (enabled && Editor.Instance.EditorToolbar.EditFGHigh.IsCheckedB.Value) Classes.Editor.Solution.EditLayerB = Classes.Editor.Solution.FGHigh;
                else if (enabled && Editor.Instance.EditorToolbar.EditFGHigher.IsCheckedB.Value) Classes.Editor.Solution.EditLayerB = Classes.Editor.Solution.FGHigher;
                else if (enabled && Editor.Instance.EditorToolbar.EditFGLower.IsCheckedB.Value) Classes.Editor.Solution.EditLayerB = Classes.Editor.Solution.FGLower;
                else if (enabled && Editor.Instance.ExtraLayerEditViewButtons.Any(elb => elb.Value.IsCheckedB.Value))
                {
                    var selectedExtraLayerButton = Editor.Instance.ExtraLayerEditViewButtons.Single(elb => elb.Value.IsCheckedB.Value);
                    var editorLayer = Classes.Editor.Solution.CurrentScene.OtherLayers.Single(el => el.Name.Equals(selectedExtraLayerButton.Value.Text));

                    Classes.Editor.Solution.EditLayerB = editorLayer;
                }
                else Classes.Editor.Solution.EditLayerB = null;
            }

        }
        private void SetEditButtonsState(bool enabled)
        {

            Editor.Instance.EditorToolbar.EditFGLow.IsEnabled = enabled && Classes.Editor.Solution.FGLow != null;
            Editor.Instance.EditorToolbar.EditFGHigh.IsEnabled = enabled && Classes.Editor.Solution.FGHigh != null;
            Editor.Instance.EditorToolbar.EditFGLower.IsEnabled = enabled && Classes.Editor.Solution.FGLower != null;
            Editor.Instance.EditorToolbar.EditFGHigher.IsEnabled = enabled && Classes.Editor.Solution.FGHigher != null;
            Editor.Instance.EditorToolbar.EditEntities.IsEnabled = enabled;

            Editor.Instance.EditorToolbar.EditFGLow.IsCheckedA = enabled && Editor.Instance.EditorToolbar.EditFGLow.IsCheckedA.Value;
            Editor.Instance.EditorToolbar.EditFGHigh.IsCheckedA = enabled && Editor.Instance.EditorToolbar.EditFGHigh.IsCheckedA.Value;
            Editor.Instance.EditorToolbar.EditFGLower.IsCheckedA = enabled && Editor.Instance.EditorToolbar.EditFGLower.IsCheckedA.Value;
            Editor.Instance.EditorToolbar.EditFGHigher.IsCheckedA = enabled && Editor.Instance.EditorToolbar.EditFGHigher.IsCheckedA.Value;

            Editor.Instance.EditorToolbar.EditFGLow.IsCheckedB = enabled && Editor.Instance.EditorToolbar.EditFGLow.IsCheckedB.Value;
            Editor.Instance.EditorToolbar.EditFGHigh.IsCheckedB = enabled && Editor.Instance.EditorToolbar.EditFGHigh.IsCheckedB.Value;
            Editor.Instance.EditorToolbar.EditFGLower.IsCheckedB = enabled && Editor.Instance.EditorToolbar.EditFGLower.IsCheckedB.Value;
            Editor.Instance.EditorToolbar.EditFGHigher.IsCheckedB = enabled && Editor.Instance.EditorToolbar.EditFGHigher.IsCheckedB.Value;

            foreach (var layerButtons in Editor.Instance.ExtraLayerEditViewButtons)
            {
                layerButtons.Value.IsCheckedA = layerButtons.Value.IsCheckedA.Value && enabled;
                layerButtons.Value.IsCheckedB = layerButtons.Value.IsCheckedB.Value && enabled;
            }

            Editor.Instance.EditorToolbar.EditEntities.IsCheckedN = enabled && Editor.Instance.EditorToolbar.EditEntities.IsCheckedN.Value;

            Editor.Instance.EditorMenuBar.SetEditButtonsState(enabled);

            SetLayerEditButtonsState(enabled);

            Editor.Instance.EditorToolbar.MagnetMode.IsEnabled = enabled && Editor.Instance.IsEntitiesEdit();
            Editor.Instance.EditorToolbar.MagnetMode.IsChecked = Classes.Editor.SolutionState.UseMagnetMode && Editor.Instance.IsEntitiesEdit();
            Editor.Instance.EditorToolbar.MagnetModeSplitButton.IsEnabled = enabled && Editor.Instance.IsEntitiesEdit();
            Classes.Editor.SolutionState.UseMagnetMode = Editor.Instance.IsEntitiesEdit() && Editor.Instance.EditorToolbar.MagnetMode.IsChecked.Value;



            Editor.Instance.EditorToolbar.UndoButton.IsEnabled = enabled && Editor.Instance.UndoStack.Count > 0;
            Editor.Instance.EditorToolbar.RedoButton.IsEnabled = enabled && Editor.Instance.RedoStack.Count > 0;



            Editor.Instance.EditorToolbar.PointerToolButton.IsEnabled = enabled;
            Editor.Instance.EditorToolbar.SelectToolButton.IsEnabled = enabled && Editor.Instance.IsTilesEdit();
            Editor.Instance.EditorToolbar.DrawToolButton.IsEnabled = enabled && Editor.Instance.IsTilesEdit() || Editor.Instance.IsEntitiesEdit();
            Editor.Instance.EditorToolbar.InteractionToolButton.IsEnabled = enabled;
            Editor.Instance.EditorToolbar.ChunksToolButton.IsEnabled = enabled && Editor.Instance.IsTilesEdit();
            Editor.Instance.EditorToolbar.SplineToolButton.IsEnabled = enabled && Editor.Instance.IsEntitiesEdit();
            Editor.Instance.EditorToolbar.SplineToolButton.IsChecked = Editor.Instance.EditorToolbar.SplineToolButton.IsChecked.Value && Editor.Instance.IsEntitiesEdit();

            bool isAnyOtherToolChecked()
            {
                bool isPointer = (bool)Editor.Instance.EditorToolbar.PointerToolButton.IsChecked.Value;
                bool isSelect = (bool)Editor.Instance.EditorToolbar.SelectToolButton.IsChecked.Value;
                bool isDraw = (bool)Editor.Instance.EditorToolbar.DrawToolButton.IsChecked.Value;
                bool isSpline = (bool)Editor.Instance.EditorToolbar.SplineToolButton.IsChecked.Value;

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


            Editor.Instance.EditorToolbar.PointerToolButton.IsChecked = isAnyOtherToolChecked();
            Editor.Instance.EditorToolbar.ChunksToolButton.IsChecked = (bool)Editor.Instance.EditorToolbar.ChunksToolButton.IsChecked && !Editor.Instance.IsEntitiesEdit();
            if (Editor.Instance.TilesToolbar != null)
            {
                if (Editor.Instance.EditorToolbar.ChunksToolButton.IsChecked.Value)
                {
                    Editor.Instance.TilesToolbar.TabControl.SelectedIndex = 1;
                }
                else
                {
                    Editor.Instance.TilesToolbar.TabControl.SelectedIndex = 0;
                }
            }

            Editor.Instance.EditorToolbar.ShowGridButton.IsEnabled = enabled && Classes.Editor.Solution.StageConfig != null;
            Editor.Instance.EditorToolbar.ShowCollisionAButton.IsEnabled = enabled && Classes.Editor.Solution.TileConfig != null;
            Editor.Instance.EditorToolbar.ShowCollisionBButton.IsEnabled = enabled && Classes.Editor.Solution.TileConfig != null;
            Editor.Instance.EditorToolbar.ShowTileIDButton.IsEnabled = enabled && Classes.Editor.Solution.StageConfig != null;
            Editor.Instance.EditorToolbar.EncorePaletteButton.IsEnabled = enabled && Classes.Editor.SolutionState.EncorePaletteExists;
            Editor.Instance.EditorToolbar.FlipAssistButton.IsEnabled = enabled;

            if (Editor.Instance.IsTilesEdit())
            {
                if (Editor.Instance.TilesToolbar == null)
                {
                    if (Classes.Editor.SolutionState.UseEncoreColors)
                        Editor.Instance.TilesToolbar = new TilesToolbar(Classes.Editor.Solution.CurrentTiles.StageTiles, Editor.Instance.Paths.StageTiles_Source, Editor.Instance.EncorePalette[0], Editor.Instance);
                    else
                        Editor.Instance.TilesToolbar = new TilesToolbar(Classes.Editor.Solution.CurrentTiles.StageTiles, Editor.Instance.Paths.StageTiles_Source, null, Editor.Instance);


                    Editor.Instance.TilesToolbar.TileDoubleClick = new Action<int>(x =>
                    {
                        Editor.Instance.EditorPlaceTile(new System.Drawing.Point((int)(Classes.Editor.SolutionState.ViewPositionX/ Classes.Editor.SolutionState.Zoom) + Classes.Editor.Constants.TILE_SIZE - 1, (int)(Classes.Editor.SolutionState.ViewPositionY / Classes.Editor.SolutionState.Zoom) + Classes.Editor.Constants.TILE_SIZE - 1), x, Classes.Editor.Solution.EditLayerA);
                    });
                    Editor.Instance.TilesToolbar.TileOptionChanged = new Action<int, bool>((option, state) =>
                    {
                        Classes.Editor.Solution.EditLayerA?.SetPropertySelected(option + 12, state);
                        Classes.Editor.Solution.EditLayerB?.SetPropertySelected(option + 12, state);

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
                    Editor.Instance.EntitiesToolbar = new EntitiesToolbar(Classes.Editor.Solution.CurrentScene.Objects, Editor.Instance)
                    {
                        SelectedEntity = new Action<int>(x =>
                        {
                            Classes.Editor.Solution.Entities.SelectSlot(x);
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
                            Classes.Editor.Solution.Entities.Add(x, GetEntitySpawnPoint());
                            Editor.Instance.UndoStack.Push(Classes.Editor.Solution.Entities.LastAction);
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
                Editor.Instance.EntitiesToolbar.SelectedEntities = Classes.Editor.Solution.Entities.SelectedEntities.Select(x => x.Entity).ToList();
            }
            else
            {
                if (Editor.Instance.EntitiesToolbar != null)
                {
                    Editor.Instance.EntitiesToolbar.Dispose();
                    Editor.Instance.EntitiesToolbar = null;
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
            Editor.Instance.EditorMenuBar.MenuBar.IsEnabled = enabled;
            Editor.Instance.EditorToolbar.LayerToolbar.IsEnabled = enabled;
            Editor.Instance.EditorToolbar.MainToolbarButtons.IsEnabled = enabled;
            Editor.Instance.UI.SetSceneOnlyButtonsState((enabled ? true : Classes.Editor.Solution.CurrentScene != null));
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
            Editor.Instance.EntitiesToolbar.Entities = Classes.Editor.Solution.Entities.Entities.Select(x => x.Entity).ToList();
        }
        public void UpdateEditLayerActions()
        {
            if (Classes.Editor.Solution.EditLayerA != null)
            {
                List<IAction> actions = Classes.Editor.Solution.EditLayerA?.Actions;
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
            if (Classes.Editor.Solution.EditLayerB != null)
            {
                List<IAction> actions = Classes.Editor.Solution.EditLayerB?.Actions;
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
            Classes.Editor.SolutionState.AllowSplineOptionsUpdate = false;
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
                Classes.Editor.SolutionState.AllowSplineOptionsUpdate = true;

            }
        }

        public void UpdateSplineSettings(int splineID)
        {
            if (!Classes.Editor.SolutionState.SplineOptionsGroup.ContainsKey(splineID)) Classes.Editor.SolutionState.SplineOptionsGroup.Add(splineID, new Classes.Editor.SolutionState.SplineOptions());
            Editor.Instance.EditorToolbar.SplineLineMode.IsChecked = Classes.Editor.SolutionState.SplineOptionsGroup[splineID].SplineLineMode;
            Editor.Instance.EditorToolbar.SplineOvalMode.IsChecked = Classes.Editor.SolutionState.SplineOptionsGroup[splineID].SplineOvalMode;
            Editor.Instance.EditorToolbar.SplineShowLineCheckbox.IsChecked = Classes.Editor.SolutionState.SplineOptionsGroup[splineID].SplineToolShowLines;
            Editor.Instance.EditorToolbar.SplineShowObjectsCheckbox.IsChecked = Classes.Editor.SolutionState.SplineOptionsGroup[splineID].SplineToolShowObject;
            Editor.Instance.EditorToolbar.SplineShowPointsCheckbox.IsChecked = Classes.Editor.SolutionState.SplineOptionsGroup[splineID].SplineToolShowPoints;
            Editor.Instance.EditorToolbar.SplinePointSeperationNUD.Value = Classes.Editor.SolutionState.SplineOptionsGroup[splineID].SplineSize;
            Editor.Instance.EditorToolbar.SplinePointSeperationSlider.Value = Classes.Editor.SolutionState.SplineOptionsGroup[splineID].SplineSize;

            if (Classes.Editor.SolutionState.SplineOptionsGroup[splineID].SplineObjectRenderingTemplate != null)
                Editor.Instance.EditorToolbar.SplineRenderObjectName.Content = Classes.Editor.SolutionState.SplineOptionsGroup[splineID].SplineObjectRenderingTemplate.Entity.Object.Name.Name;
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
            Editor.Instance.EditorToolbar.CSAC.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(Classes.Editor.SolutionState.CollisionSAColour.A, Classes.Editor.SolutionState.CollisionSAColour.R, Classes.Editor.SolutionState.CollisionSAColour.G, Classes.Editor.SolutionState.CollisionSAColour.B));
            Editor.Instance.EditorToolbar.SSTOC.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(Classes.Editor.SolutionState.CollisionTOColour.A, Classes.Editor.SolutionState.CollisionTOColour.R, Classes.Editor.SolutionState.CollisionTOColour.G, Classes.Editor.SolutionState.CollisionTOColour.B));
            Editor.Instance.EditorToolbar.CSLRDC.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(Classes.Editor.SolutionState.CollisionLRDColour.A, Classes.Editor.SolutionState.CollisionLRDColour.R, Classes.Editor.SolutionState.CollisionLRDColour.G, Classes.Editor.SolutionState.CollisionLRDColour.B));
            Editor.Instance.EditorToolbar.WLC.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(Classes.Editor.SolutionState.waterColor.A, Classes.Editor.SolutionState.waterColor.R, Classes.Editor.SolutionState.waterColor.G, Classes.Editor.SolutionState.waterColor.B));
            Editor.Instance.EditorToolbar.GDC.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(Classes.Editor.SolutionState.GridColor.A, Classes.Editor.SolutionState.GridColor.R, Classes.Editor.SolutionState.GridColor.G, Classes.Editor.SolutionState.GridColor.B));
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

            bool parallaxAnimationInProgress = Classes.Editor.SolutionState.AllowAnimations && Classes.Editor.SolutionState.ParallaxAnimationChecked;

            UpdateGameRunningButton(Classes.Editor.Solution.CurrentScene != null);
            Editor.Instance.Theming.UpdateThemeForItemsWaiting();
            Editor.Instance.EditorStatusBar.UpdateFilterButtonApperance(false);
            Editor.Instance.EditorStatusBar.UpdateStatusPanel();
            SetSceneOnlyButtonsState(Classes.Editor.Solution.CurrentScene != null && !parallaxAnimationInProgress, stageLoad);
            SetParallaxAnimationOnlyButtonsState(parallaxAnimationInProgress);
            UpdateSplineToolbox();
            Editor.Instance.EditorToolbar.CustomGridLabel.Text = string.Format(Editor.Instance.EditorToolbar.CustomGridLabel.Tag.ToString(), Properties.Defaults.Default.CustomGridSizeValue);

        }
        public void UpdateGameRunningButton(bool enabled = true)
        {
            
            Editor.Instance.EditorToolbar.RunSceneButton.IsEnabled = enabled;
            Editor.Instance.EditorToolbar.RunSceneDropDown.IsEnabled = enabled && Editor.Instance.EditorToolbar.RunSceneButton.IsEnabled;

            if (Editor.Instance.InGame.GameRunning || System.Diagnostics.Process.GetProcessesByName("SonicMania").FirstOrDefault() != null)
            {
                if (Editor.Instance.InGame.GameRunning) Editor.Instance.EditorToolbar.RunSceneIcon.Fill = System.Windows.Media.Brushes.Blue;
                else Editor.Instance.EditorToolbar.RunSceneIcon.Fill = System.Windows.Media.Brushes.Green;
            }
            else
            {
                Editor.Instance.EditorToolbar.RunSceneIcon.Fill = System.Windows.Media.Brushes.Gray;
            }
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
                if (Classes.Editor.SolutionState.UseEncoreColors == true)
                {
                    Classes.Editor.Solution.CurrentTiles.StageTiles?.Image.Reload(Editor.Instance.EncorePalette[0]);
                    Editor.Instance.TilesToolbar?.Reload(Editor.Instance.EncorePalette[0]);
                }
                else
                {
                    Classes.Editor.Solution.CurrentTiles.StageTiles?.Image.Reload();
                    Editor.Instance.TilesToolbar?.Reload();
                }

                Classes.Editor.Solution.TileConfig = new Tileconfig(Editor.Instance.Paths.TileConfig_Source);



            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        #endregion
    }
}
