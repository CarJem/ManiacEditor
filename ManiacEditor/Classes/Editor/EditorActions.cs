using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Input;
using ManiacEditor.Controls.Editor.Controls;
using ManiacEditor.Enums;
using ManiacEditor.Event_Handlers;
using ManiacEditor.Extensions;
using ManiacEditor.Actions;
using Microsoft.Scripting.Utils;
using Microsoft.Win32;
using RSDKv5;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using IWshRuntimeLibrary;
using Clipboard = System.Windows.Clipboard;
using Color = System.Drawing.Color;
using DataObject = System.Windows.DataObject;
using File = System.IO.File;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MenuItem = System.Windows.Controls.MenuItem;
using Path = System.IO.Path;
using Point = System.Drawing.Point;
using Rectangle = System.Drawing.Rectangle;

namespace ManiacEditor.Classes.Editor
{
    public static class EditorActions
    {
        private static Controls.Editor.MainEditor Instance { get; set; }
        public static void UpdateInstance(Controls.Editor.MainEditor mainEditor)
        {
            Instance = mainEditor;
        }

        #region Common Editor Functions

        public static void ZoomIn()
        {
            Classes.Editor.SolutionState.ZoomLevel += 1;
            if (Classes.Editor.SolutionState.ZoomLevel >= 5) Classes.Editor.SolutionState.ZoomLevel = 5;
            if (Classes.Editor.SolutionState.ZoomLevel <= -5) Classes.Editor.SolutionState.ZoomLevel = -5;

            Instance.ViewPanel.SharpPanel.UpdateZoomLevel(Classes.Editor.SolutionState.ZoomLevel, new Point(0, 0));
        }
        public static void ZoomOut()
        {
            Classes.Editor.SolutionState.ZoomLevel -= 1;
            if (Classes.Editor.SolutionState.ZoomLevel >= 5) Classes.Editor.SolutionState.ZoomLevel = 5;
            if (Classes.Editor.SolutionState.ZoomLevel <= -5) Classes.Editor.SolutionState.ZoomLevel = -5;

            Instance.ViewPanel.SharpPanel.UpdateZoomLevel(Classes.Editor.SolutionState.ZoomLevel, new Point(0, 0));
        }
        public static void PasteToChunks()
        {
            if (Instance.TilesClipboard != null)
            {
                Instance.Chunks.ConvertClipboardtoMultiLayerChunk(Instance.TilesClipboard.Item1, Instance.TilesClipboard.Item2);
                Instance.TilesToolbar?.ChunksReload();
            }

        }
        public static void SelectAll()
        {
            if (ManiacEditor.Classes.Editor.SolutionState.IsTilesEdit() && !ManiacEditor.Classes.Editor.SolutionState.IsChunksEdit())
            {
                if (Classes.Editor.Solution.EditLayerA != null) Classes.Editor.Solution.EditLayerA?.SelectAll();
                if (Classes.Editor.Solution.EditLayerB != null) Classes.Editor.Solution.EditLayerB?.SelectAll();
                Methods.Internal.UserInterface.UpdateEditLayerActions();
            }
            else if (ManiacEditor.Classes.Editor.SolutionState.IsEntitiesEdit())
            {
                Classes.Editor.Solution.Entities.SelectAll();
            }
            Methods.Internal.UserInterface.SetSelectOnlyButtonsState();
            Classes.Editor.SolutionState.RegionX1 = -1;
            Classes.Editor.SolutionState.RegionY1 = -1;
        }
        public static void FlipHorizontal()
        {
            Classes.Editor.Solution.EditLayerA?.FlipPropertySelected(FlipDirection.Horizontal);
            Classes.Editor.Solution.EditLayerB?.FlipPropertySelected(FlipDirection.Horizontal);
            Methods.Internal.UserInterface.UpdateEditLayerActions();
        }
        public static void FlipHorizontalIndividual()
        {
            Classes.Editor.Solution.EditLayerA?.FlipPropertySelected(FlipDirection.Horizontal, true);
            Classes.Editor.Solution.EditLayerB?.FlipPropertySelected(FlipDirection.Horizontal, true);
            Methods.Internal.UserInterface.UpdateEditLayerActions();
        }
        public static void Delete()
        {
            Classes.Editor.EditorActions.DeleteSelected();
        }
        public static void Copy()
        {
            if (ManiacEditor.Classes.Editor.SolutionState.IsTilesEdit())
                Classes.Editor.EditorActions.CopyTilesToClipboard();


            else if (ManiacEditor.Classes.Editor.SolutionState.IsEntitiesEdit())
                Classes.Editor.EditorActions.CopyEntitiesToClipboard();


            Methods.Internal.UserInterface.UpdateControls();
        }
        public static void Duplicate()
        {
            if (ManiacEditor.Classes.Editor.SolutionState.IsTilesEdit())
            {
                Classes.Editor.Solution.EditLayerA?.PasteFromClipboard(new Point(16, 16), Classes.Editor.Solution.EditLayerA?.CopyToClipboard(true));
                Classes.Editor.Solution.EditLayerB?.PasteFromClipboard(new Point(16, 16), Classes.Editor.Solution.EditLayerB?.CopyToClipboard(true));
                Methods.Internal.UserInterface.UpdateEditLayerActions();
            }
            else if (ManiacEditor.Classes.Editor.SolutionState.IsEntitiesEdit())
            {
                try
                {
                    Classes.Editor.Solution.Entities.PasteFromClipboard(new Point(16, 16), Classes.Editor.Solution.Entities.CopyToClipboard(true));
                    Classes.Editor.EditorActions.UpdateLastEntityAction();
                }
                catch (Classes.Editor.Scene.EditorEntities.TooManyEntitiesException)
                {
                    System.Windows.MessageBox.Show("Too many entities! (limit: 2048)");
                    return;
                }
                Methods.Internal.UserInterface.SetSelectOnlyButtonsState();
                Methods.Internal.UserInterface.UpdateEntitiesToolbarList();

            }
        }
        public static void Cut()
        {
            if (ManiacEditor.Classes.Editor.SolutionState.IsTilesEdit())
            {
                Classes.Editor.EditorActions.CopyTilesToClipboard();
                Classes.Editor.EditorActions.DeleteSelected();
                Methods.Internal.UserInterface.UpdateControls();
                Methods.Internal.UserInterface.UpdateEditLayerActions();
            }
            else if (ManiacEditor.Classes.Editor.SolutionState.IsEntitiesEdit())
            {
                if (Instance.EntitiesToolbar.IsFocused.Equals(false))
                {
                    Classes.Editor.EditorActions.CopyEntitiesToClipboard();
                    Classes.Editor.EditorActions.DeleteSelected();
                    Methods.Internal.UserInterface.UpdateControls();
                    Methods.Internal.UserInterface.UpdateEntitiesToolbarList();
                }
            }
        }
        public static void Paste()
        {
            if (ManiacEditor.Classes.Editor.SolutionState.IsTilesEdit())
            {
                // check if there are tiles on the Windows clipboard; if so, use those
                if (System.Windows.Clipboard.ContainsData("ManiacTiles"))
                {
                    var pasteData = (Tuple<Dictionary<Point, ushort>, Dictionary<Point, ushort>>)System.Windows.Clipboard.GetDataObject().GetData("ManiacTiles");
                    Point pastePoint = GetPastePoint();
                    if (Classes.Editor.Solution.EditLayerA != null) Classes.Editor.Solution.EditLayerA.PasteFromClipboard(pastePoint, pasteData.Item1);
                    if (Classes.Editor.Solution.EditLayerB != null) Classes.Editor.Solution.EditLayerB.PasteFromClipboard(pastePoint, pasteData.Item2);

                    Methods.Internal.UserInterface.UpdateEditLayerActions();
                }

                // if there's none, use the internal clipboard
                else if (Instance.TilesClipboard != null)
                {
                    Point pastePoint = GetPastePoint();
                    if (Classes.Editor.Solution.EditLayerA != null) Classes.Editor.Solution.EditLayerA.PasteFromClipboard(pastePoint, Instance.TilesClipboard.Item1);
                    if (Classes.Editor.Solution.EditLayerB != null) Classes.Editor.Solution.EditLayerB.PasteFromClipboard(pastePoint, Instance.TilesClipboard.Item2);
                    Methods.Internal.UserInterface.UpdateEditLayerActions();
                }

            }
            else if (ManiacEditor.Classes.Editor.SolutionState.IsEntitiesEdit())
            {
                Classes.Editor.EditorActions.PasteEntitiesToClipboard();
            }

            Point GetPastePoint()
            {
                if (ManiacEditor.Classes.Editor.SolutionState.IsChunksEdit())
                {

                    Point p = new Point((int)(Classes.Editor.SolutionState.LastX / Classes.Editor.SolutionState.Zoom), (int)(Classes.Editor.SolutionState.LastY / Classes.Editor.SolutionState.Zoom));
                    return Classes.Editor.Scene.Sets.EditorLayer.GetChunkCoordinatesTopEdge(p.X, p.Y);
                }
                else
                {
                    return new Point((int)(Classes.Editor.SolutionState.LastX / Classes.Editor.SolutionState.Zoom) + Classes.Editor.Constants.TILE_SIZE - 1, (int)(Classes.Editor.SolutionState.LastY / Classes.Editor.SolutionState.Zoom) + Classes.Editor.Constants.TILE_SIZE - 1);

                }
            }
        }
        public static void FlipVertical()
        {
            Classes.Editor.Solution.EditLayerA?.FlipPropertySelected(FlipDirection.Veritcal);
            Classes.Editor.Solution.EditLayerB?.FlipPropertySelected(FlipDirection.Veritcal);
            Methods.Internal.UserInterface.UpdateEditLayerActions();
        }
        public static void FlipVerticalIndividual()
        {
            Classes.Editor.Solution.EditLayerA?.FlipPropertySelected(FlipDirection.Veritcal, true);
            Classes.Editor.Solution.EditLayerB?.FlipPropertySelected(FlipDirection.Veritcal, true);
            Methods.Internal.UserInterface.UpdateEditLayerActions();
        }
        public static void EditorPlaceTile(Point position, int tile, Classes.Editor.Scene.Sets.EditorLayer layer, bool isDrawing = false)
        {
            if (isDrawing)
            {
                double offset = (Classes.Editor.SolutionState.DrawBrushSize / 2) * Classes.Editor.Constants.TILE_SIZE;
                Point finalPosition = new Point((int)(position.X - offset), (int)(position.Y - offset));
                Dictionary<Point, ushort> tiles = new Dictionary<Point, ushort>();
                for (int x = 0; x < Classes.Editor.SolutionState.DrawBrushSize; x++)
                {
                    for (int y = 0; y < Classes.Editor.SolutionState.DrawBrushSize; y++)
                    {
                        if (!tiles.ContainsKey(new Point(x, y))) tiles.Add(new Point(x, y), (ushort)tile);
                    }
                }
                layer.DrawAsBrush(finalPosition, tiles);
            }
            else
            {
                Dictionary<Point, ushort> tiles = new Dictionary<Point, ushort>
                {
                    [new Point(0, 0)] = (ushort)tile
                };
                layer.PasteFromClipboard(position, tiles);
            }
        }
        public static void DeleteSelected()
        {
            Classes.Editor.Solution.EditLayerA?.DeleteSelected();
            Classes.Editor.Solution.EditLayerB?.DeleteSelected();
            Methods.Internal.UserInterface.UpdateEditLayerActions();

            if (ManiacEditor.Classes.Editor.SolutionState.IsEntitiesEdit())
            {
                Classes.Editor.Solution.Entities.DeleteSelected();
                UpdateLastEntityAction();
            }
        }
        public static void UpdateLastEntityAction()
        {
            if (Classes.Editor.Solution.Entities.LastAction != null || Classes.Editor.Solution.Entities.LastActionInternal != null) Instance.RedoStack.Clear();
            if (Classes.Editor.Solution.Entities.LastAction != null)
            {
                Instance.UndoStack.Push(Classes.Editor.Solution.Entities.LastAction);
                Classes.Editor.Solution.Entities.LastAction = null;
            }
            if (Classes.Editor.Solution.Entities.LastActionInternal != null)
            {
                Instance.UndoStack.Push(Classes.Editor.Solution.Entities.LastActionInternal);
                Classes.Editor.Solution.Entities.LastActionInternal = null;
            }
            if (Classes.Editor.Solution.Entities.LastAction != null || Classes.Editor.Solution.Entities.LastActionInternal != null) Methods.Internal.UserInterface.UpdateControls();

        }
        public static void FlipEntities(FlipDirection direction)
        {
            Dictionary<Classes.Editor.Scene.Sets.EditorEntity, Point> initalPos = new Dictionary<Classes.Editor.Scene.Sets.EditorEntity, Point>();
            Dictionary<Classes.Editor.Scene.Sets.EditorEntity, Point> postPos = new Dictionary<Classes.Editor.Scene.Sets.EditorEntity, Point>();
            foreach (Classes.Editor.Scene.Sets.EditorEntity e in Classes.Editor.Solution.Entities.SelectedEntities)
            {
                initalPos.Add(e, new Point(e.PositionX, e.PositionY));
            }
            Classes.Editor.Solution.Entities.Flip(direction);
            Instance.EntitiesToolbar.UpdateCurrentEntityProperites();
            foreach (Classes.Editor.Scene.Sets.EditorEntity e in Classes.Editor.Solution.Entities.SelectedEntities)
            {
                postPos.Add(e, new Point(e.PositionX, e.PositionY));
            }
            IAction action = new ActionMultipleMoveEntities(initalPos, postPos);
            Instance.UndoStack.Push(action);
            Instance.RedoStack.Clear();

        }
        public static void Deselect(bool updateControls = true)
        {
            if (ManiacEditor.Classes.Editor.SolutionState.IsEditing())
            {
                Classes.Editor.Solution.EditLayerA?.Deselect();
                Classes.Editor.Solution.EditLayerB?.Deselect();

                if (ManiacEditor.Classes.Editor.SolutionState.IsEntitiesEdit()) Classes.Editor.Solution.Entities.Deselect();
                Methods.Internal.UserInterface.SetSelectOnlyButtonsState(false);
                if (updateControls)
                    Methods.Internal.UserInterface.UpdateEditLayerActions();
            }
        }
        public static void EditorUndo()
        {
            if (Instance.UndoStack.Count > 0)
            {
                if (ManiacEditor.Classes.Editor.SolutionState.IsTilesEdit())
                {
                    // Deselect to apply the changes
                    Deselect();
                }
                else if (ManiacEditor.Classes.Editor.SolutionState.IsEntitiesEdit())
                {
                    if (Instance.UndoStack.Peek() is ActionAddDeleteEntities)
                    {
                        // deselect only if delete/create
                        Deselect();
                    }
                }
                IAction act = Instance.UndoStack.Pop();
                act.Undo();
                Instance.RedoStack.Push(act.Redo());
                if (ManiacEditor.Classes.Editor.SolutionState.IsEntitiesEdit() && ManiacEditor.Classes.Editor.SolutionState.IsSelected())
                {
                    // We need to update the properties of the selected entity
                    Instance.EntitiesToolbar.UpdateCurrentEntityProperites();
                }
            }
            Instance.ViewPanel.SharpPanel.GraphicPanel.Render();
            Methods.Internal.UserInterface.UpdateControls();
        }
        public static void EditorRedo()
        {
            if (Instance.RedoStack.Count > 0)
            {
                IAction act = Instance.RedoStack.Pop();
                act.Undo();
                Instance.UndoStack.Push(act.Redo());
                if (ManiacEditor.Classes.Editor.SolutionState.IsEntitiesEdit() && ManiacEditor.Classes.Editor.SolutionState.IsSelected())
                {
                    // We need to update the properties of the selected entity
                    Instance.EntitiesToolbar.UpdateCurrentEntityProperites();
                }
            }
            Instance.ViewPanel.SharpPanel.GraphicPanel.Render();
            Methods.Internal.UserInterface.UpdateControls();
        }
        public static void CopyTilesToClipboard(bool doNotUseWindowsClipboard = false)
        {
            bool hasMultipleValidLayers = Classes.Editor.Solution.EditLayerA != null && Classes.Editor.Solution.EditLayerB != null;
            if (!hasMultipleValidLayers)
            {
                Dictionary<Point, ushort> copyDataA = Classes.Editor.Solution.EditLayerA?.CopyToClipboard();
                Dictionary<Point, ushort> copyDataB = Classes.Editor.Solution.EditLayerB?.CopyToClipboard();
                Tuple<Dictionary<Point, ushort>, Dictionary<Point, ushort>> copyData = new Tuple<Dictionary<Point, ushort>, Dictionary<Point, ushort>>(copyDataA, copyDataB);

                // Make a DataObject for the copied data and send it to the Windows clipboard for cross-instance copying
                if (!doNotUseWindowsClipboard)
                    Clipboard.SetDataObject(new DataObject("ManiacTiles", copyData), true);

                // Also copy to Maniac's clipboard in case it gets overwritten elsewhere
                Instance.TilesClipboard = copyData;
            }
            else if (hasMultipleValidLayers && Classes.Editor.SolutionState.MultiLayerEditMode)
            {
                Tuple<Dictionary<Point, ushort>, Dictionary<Point, ushort>> copyData = Classes.Editor.Scene.Sets.EditorLayer.CopyMultiSelectionToClipboard(Classes.Editor.Solution.EditLayerA, Classes.Editor.Solution.EditLayerB);

                // Make a DataObject for the copied data and send it to the Windows clipboard for cross-instance copying
                if (!doNotUseWindowsClipboard)
                    Clipboard.SetDataObject(new DataObject("ManiacTiles", copyData), true);

                // Also copy to Maniac's clipboard in case it gets overwritten elsewhere
                Instance.TilesClipboard = copyData;
            }


        }
        public static void CopyEntitiesToClipboard()
        {
            if (Instance.EntitiesToolbar.IsFocused == false)
            {
                List<Classes.Editor.Scene.Sets.EditorEntity> copyData = Classes.Editor.Solution.Entities.CopyToClipboard();

                /*
                // Prepare each Entity for the copy to release unnecessary data
                foreach (Classes.Edit.Scene.Sets.EditorEntity entity in copyData)
                   entity.PrepareForExternalCopy();

                CloseClipboard();

                // Make a DataObject for the data and send it to the Windows clipboard for cross-instance copying
                Clipboard.SetDataObject(new DataObject("ManiacEntities", copyData));*/

                // Send to Maniac's clipboard
                Instance.entitiesClipboard = copyData;
            }
        }
        public static void PasteEntitiesToClipboard()
        {
            if (Instance.EntitiesToolbar.IsFocused.Equals(false))
            {
                try
                {

                    // check if there are Classes.Edit.Scene.EditorSolution.Entities on the Windows clipboard; if so, use those
                    if (System.Windows.Clipboard.ContainsData("ManiacEntities"))
                    {
                        Classes.Editor.Solution.Entities.PasteFromClipboard(new Point((int)(Classes.Editor.SolutionState.LastX / Classes.Editor.SolutionState.Zoom), (int)(Classes.Editor.SolutionState.LastY / Classes.Editor.SolutionState.Zoom)), (List<Classes.Editor.Scene.Sets.EditorEntity>)System.Windows.Clipboard.GetDataObject().GetData("ManiacEntities"));
                        UpdateLastEntityAction();
                    }

                    // if there's none, use the internal clipboard
                    else if (Instance.entitiesClipboard != null)
                    {
                        Classes.Editor.Solution.Entities.PasteFromClipboard(new Point((int)(Classes.Editor.SolutionState.LastX / Classes.Editor.SolutionState.Zoom), (int)(Classes.Editor.SolutionState.LastY / Classes.Editor.SolutionState.Zoom)), Instance.entitiesClipboard);
                        UpdateLastEntityAction();
                    }
                }
                catch (Classes.Editor.Scene.EditorEntities.TooManyEntitiesException)
                {
                    System.Windows.MessageBox.Show("Too many Classes.Edit.Scene.EditorSolution.Entities! (limit: 2048)");
                    return;
                }
                Methods.Internal.UserInterface.UpdateEntitiesToolbarList();
                Methods.Internal.UserInterface.SetSelectOnlyButtonsState();
            }
        }
        public static void MoveEntityOrTiles(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            int x = 0, y = 0;
            int modifier = (ManiacEditor.Classes.Editor.SolutionState.IsChunksEdit() ? 8 : 1);
            if (Classes.Editor.SolutionState.UseMagnetMode)
            {
                switch (e.KeyData)
                {
                    case Keys.Up: y = (Classes.Editor.SolutionState.UseMagnetYAxis ? -Classes.Editor.SolutionState.MagnetSize : -1); break;
                    case Keys.Down: y = (Classes.Editor.SolutionState.UseMagnetYAxis ? Classes.Editor.SolutionState.MagnetSize : 1); break;
                    case Keys.Left: x = (Classes.Editor.SolutionState.UseMagnetXAxis ? -Classes.Editor.SolutionState.MagnetSize : -1); break;
                    case Keys.Right: x = (Classes.Editor.SolutionState.UseMagnetXAxis ? Classes.Editor.SolutionState.MagnetSize : 1); break;
                }
            }
            if (Classes.Editor.SolutionState.EnableFasterNudge)
            {
                if (Classes.Editor.SolutionState.UseMagnetMode)
                {
                    switch (e.KeyData)
                    {
                        case Keys.Up: y = (Classes.Editor.SolutionState.UseMagnetYAxis ? -Classes.Editor.SolutionState.MagnetSize * Classes.Editor.SolutionState.FasterNudgeAmount : -1 - Classes.Editor.SolutionState.FasterNudgeAmount); break;
                        case Keys.Down: y = (Classes.Editor.SolutionState.UseMagnetYAxis ? Classes.Editor.SolutionState.MagnetSize * Classes.Editor.SolutionState.FasterNudgeAmount : 1 + Classes.Editor.SolutionState.FasterNudgeAmount); break;
                        case Keys.Left: x = (Classes.Editor.SolutionState.UseMagnetXAxis ? -Classes.Editor.SolutionState.MagnetSize * Classes.Editor.SolutionState.FasterNudgeAmount : -1 - Classes.Editor.SolutionState.FasterNudgeAmount); break;
                        case Keys.Right: x = (Classes.Editor.SolutionState.UseMagnetXAxis ? Classes.Editor.SolutionState.MagnetSize * Classes.Editor.SolutionState.FasterNudgeAmount : 1 + Classes.Editor.SolutionState.FasterNudgeAmount); break;
                    }
                }
                else
                {
                    if (ManiacEditor.Classes.Editor.SolutionState.IsChunksEdit())
                    {
                        switch (e.KeyData)
                        {
                            case Keys.Up: y = -1 * modifier; break;
                            case Keys.Down: y = 1 * modifier; break;
                            case Keys.Left: x = -1 * modifier; break;
                            case Keys.Right: x = 1 * modifier; break;
                        }
                    }
                    else
                    {
                        switch (e.KeyData)
                        {
                            case Keys.Up: y = (-1 - Classes.Editor.SolutionState.FasterNudgeAmount) * modifier; break;
                            case Keys.Down: y = (1 + Classes.Editor.SolutionState.FasterNudgeAmount) * modifier; break;
                            case Keys.Left: x = (-1 - Classes.Editor.SolutionState.FasterNudgeAmount) * modifier; break;
                            case Keys.Right: x = (1 + Classes.Editor.SolutionState.FasterNudgeAmount) * modifier; break;
                        }
                    }

                }

            }
            if (Classes.Editor.SolutionState.UseMagnetMode == false && Classes.Editor.SolutionState.EnableFasterNudge == false)
            {
                switch (e.KeyData)
                {
                    case Keys.Up: y = -1 * modifier; break;
                    case Keys.Down: y = 1 * modifier; break;
                    case Keys.Left: x = -1 * modifier; break;
                    case Keys.Right: x = 1 * modifier; break;
                }

            }
            Classes.Editor.Solution.EditLayerA?.MoveSelectedQuonta(new Point(x, y));
            Classes.Editor.Solution.EditLayerB?.MoveSelectedQuonta(new Point(x, y));

            Methods.Internal.UserInterface.UpdateEditLayerActions();

            if (ManiacEditor.Classes.Editor.SolutionState.IsEntitiesEdit())
            {
                if (Classes.Editor.SolutionState.UseMagnetMode)
                {
                    int xE = Classes.Editor.Solution.Entities.SelectedEntities[0].Entity.Position.X.High;
                    int yE = Classes.Editor.Solution.Entities.SelectedEntities[0].Entity.Position.Y.High;

                    if (xE % Classes.Editor.SolutionState.MagnetSize != 0 && Classes.Editor.SolutionState.UseMagnetXAxis)
                    {
                        int offsetX = x % Classes.Editor.SolutionState.MagnetSize;
                        x -= offsetX;
                    }
                    if (yE % Classes.Editor.SolutionState.MagnetSize != 0 && Classes.Editor.SolutionState.UseMagnetYAxis)
                    {
                        int offsetY = y % Classes.Editor.SolutionState.MagnetSize;
                        y -= offsetY;
                    }
                }


                Classes.Editor.Solution.Entities.MoveSelected(new Point(0, 0), new Point(x, y), false);
                Instance.EntitiesToolbar.UpdateCurrentEntityProperites();

                // Try to merge with last move
                List<Classes.Editor.Scene.Sets.EditorEntity> SelectedList = Classes.Editor.Solution.Entities.SelectedEntities.ToList();
                List<Classes.Editor.Scene.Sets.EditorEntity> SelectedInternalList = Classes.Editor.Solution.Entities.SelectedInternalEntities.ToList();
                bool selectedActionsState = Instance.UndoStack.Count > 0 && Instance.UndoStack.Peek() is ActionMoveEntities && (Instance.UndoStack.Peek() as ActionMoveEntities).UpdateFromKey(SelectedList, new Point(x, y));
                bool selectedInternalActionsState = Instance.UndoStack.Count > 0 && Instance.UndoStack.Peek() is ActionMoveEntities && (Instance.UndoStack.Peek() as ActionMoveEntities).UpdateFromKey(SelectedInternalList, new Point(x, y));

                if (selectedActionsState || selectedInternalActionsState) { }
                else
                {
                    if (SelectedList.Count != 0) Instance.UndoStack.Push(new ActionMoveEntities(SelectedList, new Point(x, y), true));
                    if (SelectedInternalList.Count != 0) Instance.UndoStack.Push(new ActionMoveEntities(SelectedInternalList, new Point(x, y), true));

                    Instance.RedoStack.Clear();
                    Methods.Internal.UserInterface.UpdateControls();
                }
            }
        }
        public static void CreateShortcut(string dataDir, string scenePath = "", string modPath = "", int X = 0, int Y = 0, bool isEncoreMode = false, int LevelSlotNum = -1, double ZoomedLevel = 0.0)
        {
            object shDesktop = (object)"Desktop";
            WshShell shell = new WshShell();
            string shortcutAddress = "";
            if (scenePath != "")
            {
                shortcutAddress = (string)shell.SpecialFolders.Item(ref shDesktop) + @"\" + "Scene Link" + " - Maniac.lnk";
            }
            else
            {
                shortcutAddress = (string)shell.SpecialFolders.Item(ref shDesktop) + @"\" + "Data Folder Link" + " - Maniac.lnk";
            }
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutAddress);

            string targetAddress = "\"" + Environment.CurrentDirectory + @"\ManiacEditor.exe" + "\"";
            string launchArguments = "";
            if (scenePath != "")
            {
                launchArguments = (dataDir != "" ? "DataDir=" + "\"" + dataDir + "\" " : "") + (scenePath != "" ? "ScenePath=" + "\"" + scenePath + "\" " : "") + (modPath != "" ? "ModPath=" + "\"" + modPath + "\" " : "") + (LevelSlotNum != -1 ? "LevelID=" + LevelSlotNum.ToString() + " " : "") + (isEncoreMode == true ? "EncoreMode=TRUE " : "") + (X != 0 ? "X=" + X.ToString() + " " : "") + (Y != 0 ? "Y=" + Y.ToString() + " " : "") + (ZoomedLevel != 0 ? "ZoomedLevel=" + ZoomedLevel.ToString() + " " : "");
            }
            else
            {
                launchArguments = (dataDir != "" ? "DataDir=" + "\"" + dataDir + "\" " : "");
            }

            shortcut.TargetPath = targetAddress;
            shortcut.Arguments = launchArguments;
            shortcut.WorkingDirectory = Environment.CurrentDirectory;
            shortcut.Save();
        }
        public static void GoToPosition(int x, int y, bool CenterCoords = true, bool ShortcutClear = false)
        {
            if (CenterCoords)
            {
                Rectangle r = Instance.ViewPanel.SharpPanel.GraphicPanel.GetScreen();
                int x2 = (int)(r.Width * Classes.Editor.SolutionState.Zoom);
                int y2 = (int)(r.Height * Classes.Editor.SolutionState.Zoom);

                int ResultX = (int)(x * Classes.Editor.SolutionState.Zoom) - x2 / 2;
                int ResultY = (int)(y * Classes.Editor.SolutionState.Zoom) - y2 / 2;

                if ((ResultX <= 0)) ResultX = 0;
                if ((ResultY <= 0)) ResultY = 0;


                Classes.Editor.SolutionState.SetViewPositionX(ResultX);
                Classes.Editor.SolutionState.SetViewPositionY(ResultY);
            }
            else
            {
                int ResultX = (int)(x * Classes.Editor.SolutionState.Zoom);
                int ResultY = (int)(y * Classes.Editor.SolutionState.Zoom);

                if ((ResultX <= 0)) ResultX = 0;
                if ((ResultY <= 0)) ResultY = 0;

                Classes.Editor.SolutionState.SetViewPositionX(ResultX);
                Classes.Editor.SolutionState.SetViewPositionY(ResultY);
            }
        }

        #endregion

        #region Specific Editor Functions

        public static void ChangeLevelID(object sender, RoutedEventArgs e)
        {
            string inputValue = GenerationsLib.WPF.TextPrompt2.ShowDialog("Change Level ID", "This is only temporary and will reset when you reload the scene.", Classes.Editor.SolutionState.LevelID.ToString());
            int.TryParse(inputValue.ToString(), out int output);
            Classes.Editor.SolutionState.LevelID = output;
            Controls.Editor.MainEditor.Instance.EditorStatusBar._levelIDLabel.Content = "Level ID: " + Classes.Editor.SolutionState.LevelID.ToString();
        }
        public static void MakeShortcutForDataFolderOnly(object sender, RoutedEventArgs e)
        {
            string dataDir = Instance.DataDirectory;
            Classes.Editor.EditorActions.CreateShortcut(dataDir);
        }
        public static void MakeShortcutWithCurrentCoordinatesToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            string dataDir = Instance.DataDirectory;
            string scenePath = ManiacEditor.Classes.Editor.Solution.Paths.GetScenePath();
            int rX = (short)(Classes.Editor.SolutionState.ViewPositionX);
            int rY = (short)(Classes.Editor.SolutionState.ViewPositionY);
            double _ZoomLevel = Classes.Editor.SolutionState.ZoomLevel;
            bool isEncoreSet = Classes.Editor.SolutionState.UseEncoreColors;
            int levelSlotNum = Classes.Editor.SolutionState.LevelID;
            Classes.Editor.EditorActions.CreateShortcut(dataDir, scenePath, "", rX, rY, isEncoreSet, levelSlotNum, _ZoomLevel);
        }
        public static void MakeShortcutWithoutCurrentCoordinatesToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            string dataDir = Instance.DataDirectory;
            string scenePath = ManiacEditor.Classes.Editor.Solution.Paths.GetScenePath();
            int rX = 0;
            int rY = 0;
            bool isEncoreSet = Classes.Editor.SolutionState.UseEncoreColors;
            int levelSlotNum = Classes.Editor.SolutionState.LevelID;
            Classes.Editor.EditorActions.CreateShortcut(dataDir, scenePath, "", rX, rY, isEncoreSet, levelSlotNum);
        }
        public static void GoToPosition(object sender, RoutedEventArgs e)
        {
            ManiacEditor.Controls.Utility.GoToPositionBox form = new ManiacEditor.Controls.Utility.GoToPositionBox(Instance);
            form.Owner = Instance as Window;
            form.ShowDialog();
        }
        public static void SoundLooperToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {

        }
        public static void MD5GeneratorToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ManiacEditor.Controls.Utility.Editor.Dev.MD5HashGen hashmap = new ManiacEditor.Controls.Utility.Editor.Dev.MD5HashGen(Instance);
            hashmap.Show();
        }
        public static void FindAndReplaceTool(object sender, RoutedEventArgs e)
        {
            ManiacEditor.Controls.Utility.FindandReplaceTool form = new ManiacEditor.Controls.Utility.FindandReplaceTool();
            form.ShowDialog();
            if (form.DialogResult == true)
            {
                while (form.GetReadyState() == false)
                {

                }
                int applyState = form.GetApplyState();
                bool copyResults = form.CopyResultsOrNot();
                bool replaceMode = form.IsReplaceMode();
                int find = form.GetFindValue();
                int replace = form.GetReplaceValue();
                bool perserveColllision = form.PerservingCollision();

                if (replaceMode)
                {
                    Controls.Editor.MainEditor.Instance.FindAndReplace.EditorTileFindReplace(find, replace, applyState, copyResults);//, perserveColllision
                }
                else
                {
                    Controls.Editor.MainEditor.Instance.FindAndReplace.EditorTileFind(find, applyState, copyResults);
                }

            }

        }
        public static void ConsoleWindowToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (!Classes.Editor.SolutionState.IsConsoleWindowOpen)
            {
                Classes.Editor.SolutionState.IsConsoleWindowOpen = true;
                Extensions.ExternalExtensions.ShowConsoleWindow();
            }
            else
            {
                Classes.Editor.SolutionState.IsConsoleWindowOpen = false;
                Extensions.ExternalExtensions.HideConsoleWindow();
            }
        }
        public static void SaveForForceOpenOnStartupToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Methods.Settings.MyDevSettings.DevForceRestartData = Instance.DataDirectory;
            Methods.Settings.MyDevSettings.DevForceRestartScene = ManiacEditor.Classes.Editor.Solution.Paths.SceneFilePath;
            Methods.Settings.MyDevSettings.DevForceRestartX = (short)(Classes.Editor.SolutionState.ViewPositionX / Classes.Editor.SolutionState.Zoom);
            Methods.Settings.MyDevSettings.DevForceRestartY = (short)(Classes.Editor.SolutionState.ViewPositionY / Classes.Editor.SolutionState.Zoom);
            Methods.Settings.MyDevSettings.DevForceRestartZoomLevel = Classes.Editor.SolutionState.ZoomLevel;
            Methods.Settings.MyDevSettings.DevForceRestartIsEncore = ManiacEditor.Classes.Editor.Solution.Paths.isEncoreMode;
            Methods.Settings.MyDevSettings.DevForceRestartID = Classes.Editor.SolutionState.LevelID;
            Methods.Settings.MyDevSettings.DevForceRestartCurrentName = ManiacEditor.Classes.Editor.Solution.Paths.CurrentName;
            Methods.Settings.MyDevSettings.DevForceRestartCurrentZone = ManiacEditor.Classes.Editor.Solution.Paths.CurrentZone;
            Methods.Settings.MyDevSettings.DevForceRestartSceneID = ManiacEditor.Classes.Editor.Solution.Paths.CurrentSceneID;
            Methods.Settings.MyDevSettings.DevForceRestartIsBrowsed = ManiacEditor.Classes.Editor.Solution.Paths.Browsed;
            Methods.Settings.MyDevSettings.DevForceRestartResourcePacks = new System.Collections.Specialized.StringCollection();
            Methods.Settings.MyDevSettings.DevForceRestartResourcePacks.AddRange(Instance.ResourcePackList.ToArray());
        }
        public static void EnableAllButtonsToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            object[] MTB = Instance.EditorToolbar.MainToolbarButtons.Items.Cast<object>().ToArray();
            object[] LT = Instance.EditorToolbar.LayerToolbar.Items.Cast<object>().ToArray();
            ManiacEditor.Extensions.Extensions.EnableButtonList(MTB);
            ManiacEditor.Extensions.Extensions.EnableButtonList(LT);
        }

        public static void SwapEncoreManiaEntityVisibility()
        {
            if (Methods.Settings.MyDefaults.ShowEncoreEntities == true && Methods.Settings.MyDefaults.ShowManiaEntities == true)
            {
                Methods.Settings.MyDefaults.ShowManiaEntities = true;
                Methods.Settings.MyDefaults.ShowEncoreEntities = false;
            }
            if (Methods.Settings.MyDefaults.ShowEncoreEntities == true && Methods.Settings.MyDefaults.ShowManiaEntities == false)
            {
                Methods.Settings.MyDefaults.ShowManiaEntities = true;
                Methods.Settings.MyDefaults.ShowEncoreEntities = false;
            }
            else
            {
                Methods.Settings.MyDefaults.ShowManiaEntities = false;
                Methods.Settings.MyDefaults.ShowEncoreEntities = true;
            }

        }

        public static void SetScrollLockDirection()
        {
            if (Classes.Editor.SolutionState.ScrollDirection == Axis.X)
            {
                Classes.Editor.SolutionState.ScrollDirection = Axis.Y;
                Instance.EditorStatusBar.UpdateStatusPanel();
                Instance.MenuBar.xToolStripMenuItem.IsChecked = false;
                Instance.MenuBar.yToolStripMenuItem.IsChecked = true;
            }
            else
            {
                Classes.Editor.SolutionState.ScrollDirection = Axis.X;
                Instance.EditorStatusBar.UpdateStatusPanel();
                Instance.MenuBar.xToolStripMenuItem.IsChecked = true;
                Instance.MenuBar.yToolStripMenuItem.IsChecked = false;
            }
        }

        public static void SetManiaMenuInputType(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.MenuItem menuItem = sender as System.Windows.Controls.MenuItem;
            if (menuItem != null)
            {
                if (menuItem.Tag != null)
                {
                    string tag = menuItem.Tag.ToString();
                    var allItems = Instance.MenuBar.menuButtonsToolStripMenuItem.Items.Cast<System.Windows.Controls.MenuItem>().ToArray();
                    foreach (System.Windows.Controls.MenuItem item in allItems)
                    {
                        if (item.Tag == null || item.Tag.ToString() != menuItem.Tag.ToString()) item.IsChecked = false;
                        else if (item.Tag.ToString() == menuItem.Tag.ToString()) item.IsChecked = true;
                        var allSubItems = Instance.MenuBar.menuButtonsToolStripMenuItem.Items.Cast<System.Windows.Controls.MenuItem>().ToArray();
                        foreach (System.Windows.Controls.MenuItem subItem in allSubItems)
                        {
                            if (subItem.Tag == null || subItem.Tag.ToString() != menuItem.Tag.ToString()) subItem.IsChecked = false;
                            else if (subItem.Tag.ToString() == menuItem.Tag.ToString()) subItem.IsChecked = true;
                        }
                    }
                    switch (tag)
                    {
                        case "Xbox":
                            Classes.Editor.SolutionState.CurrentControllerButtons = 2;
                            break;
                        case "Switch":
                            Classes.Editor.SolutionState.CurrentControllerButtons = 4;
                            break;
                        case "PS4":
                            Classes.Editor.SolutionState.CurrentControllerButtons = 3;
                            break;
                        case "Saturn Black":
                            Classes.Editor.SolutionState.CurrentControllerButtons = 5;
                            break;
                        case "Saturn White":
                            Classes.Editor.SolutionState.CurrentControllerButtons = 6;
                            break;
                        case "Switch Joy L":
                            Classes.Editor.SolutionState.CurrentControllerButtons = 7;
                            break;
                        case "Switch Joy R":
                            Classes.Editor.SolutionState.CurrentControllerButtons = 8;
                            break;
                        case "PC EN/JP":
                            Classes.Editor.SolutionState.CurrentControllerButtons = 1;
                            break;
                        case "PC FR":
                            Classes.Editor.SolutionState.CurrentControllerButtons = 9;
                            break;
                        case "PC IT":
                            Classes.Editor.SolutionState.CurrentControllerButtons = 10;
                            break;
                        case "PC GE":
                            Classes.Editor.SolutionState.CurrentControllerButtons = 11;
                            break;
                        case "PC SP":
                            Classes.Editor.SolutionState.CurrentControllerButtons = 12;
                            break;
                    }
                    menuItem.IsChecked = true;
                }

            }

        }
        public static void SetManiaMenuInputType(string tag)
        {
            switch (tag)
            {
                case "Xbox":
                    Classes.Editor.SolutionState.CurrentControllerButtons = 2;
                    break;
                case "Switch":
                    Classes.Editor.SolutionState.CurrentControllerButtons = 4;
                    break;
                case "PS4":
                    Classes.Editor.SolutionState.CurrentControllerButtons = 3;
                    break;
                case "Saturn Black":
                    Classes.Editor.SolutionState.CurrentControllerButtons = 5;
                    break;
                case "Saturn White":
                    Classes.Editor.SolutionState.CurrentControllerButtons = 6;
                    break;
                case "Switch Joy L":
                    Classes.Editor.SolutionState.CurrentControllerButtons = 7;
                    break;
                case "Switch Joy R":
                    Classes.Editor.SolutionState.CurrentControllerButtons = 8;
                    break;
                case "PC EN/JP":
                    Classes.Editor.SolutionState.CurrentControllerButtons = 1;
                    break;
                case "PC FR":
                    Classes.Editor.SolutionState.CurrentControllerButtons = 9;
                    break;
                case "PC IT":
                    Classes.Editor.SolutionState.CurrentControllerButtons = 10;
                    break;
                case "PC GE":
                    Classes.Editor.SolutionState.CurrentControllerButtons = 11;
                    break;
                case "PC SP":
                    Classes.Editor.SolutionState.CurrentControllerButtons = 12;
                    break;
            }
        }
        public static void SetEncorePallete(object sender = null, string path = "")
        {
            if (sender != null)
            {
                var clickedItem = sender as System.Windows.Controls.MenuItem;
                string StartDir = Instance.DataDirectory;
                try
                {
                    using (var fd = new System.Windows.Forms.OpenFileDialog())
                    {
                        fd.Filter = "Color Palette File|*.act";
                        fd.DefaultExt = ".act";
                        fd.Title = "Select an Encore Color Palette";
                        fd.InitialDirectory = Path.Combine(StartDir, "Palettes");
                        if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            Instance.EncorePalette = Classes.Editor.Solution.CurrentScene.GetEncorePalette("", "", "", "", -1, fd.FileName);
                            Classes.Editor.SolutionState.EncoreSetupType = 0;
                            if (File.Exists(Instance.EncorePalette[0]))
                            {
                                Classes.Editor.SolutionState.EncorePaletteExists = true;
                                Classes.Editor.SolutionState.UseEncoreColors = true;
                            }

                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("Unable to set Encore Colors. " + ex.Message);
                }
            }
            else if (path != "")
            {
                Instance.EncorePalette = Classes.Editor.Solution.CurrentScene.GetEncorePalette("", "", "", "", -1, path);
                Classes.Editor.SolutionState.EncoreSetupType = 0;
                if (File.Exists(Instance.EncorePalette[0]))
                {
                    Classes.Editor.SolutionState.EncorePaletteExists = true;
                    Classes.Editor.SolutionState.UseEncoreColors = true;
                }
                else
                {
                    System.Windows.MessageBox.Show("Unable to set Encore Colors. The Specified Path does not exist: " + Environment.NewLine + path);
                }
            }

        }
        private static bool LockEntityFilterTextChanged { get; set; } = false;
        public static void UpdateEntityFilterFromTextBox(object sender, TextChangedEventArgs e)
        {
            if (sender is System.Windows.Controls.TextBox && LockEntityFilterTextChanged == false)
            {
                LockEntityFilterTextChanged = true;
                System.Windows.Controls.TextBox theSender = sender as System.Windows.Controls.TextBox;
                Classes.Editor.SolutionState.entitiesTextFilter = theSender.Text;
                Instance.MenuBar.toolStripTextBox1.Text = Classes.Editor.SolutionState.entitiesTextFilter;
                //Editor.toolStripTextBox2.Text = Editor.entitiesTextFilter;
                Classes.Editor.Solution.Entities.FilterRefreshNeeded = true;
                LockEntityFilterTextChanged = false;
            }


        }
        public static void ManiaMenuLanguageChanged(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.MenuItem menuItem = sender as System.Windows.Controls.MenuItem;
            Classes.Editor.SolutionState.CurrentLanguage = menuItem.Tag.ToString();
            var allLangItems = Instance.MenuBar.menuLanguageToolStripMenuItem.Items.Cast<System.Windows.Controls.MenuItem>().ToArray();
            foreach (var item in allLangItems)
            {
                if (item.Tag.ToString() != menuItem.Tag.ToString()) item.IsChecked = false;
                else if (item.Tag.ToString() == menuItem.Tag.ToString()) item.IsChecked = true;
            }


        }

        #endregion
    }
}
