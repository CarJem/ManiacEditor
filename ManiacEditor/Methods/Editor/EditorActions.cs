using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Input;
using ManiacEditor.Controls.Global;
using ManiacEditor.Enums;
using ManiacEditor.EventHandlers;
using ManiacEditor.Extensions;
using ManiacEditor.Actions;
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

namespace ManiacEditor.Methods.Editor
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
            Methods.Editor.SolutionState.ZoomLevel += 1;
            if (Methods.Editor.SolutionState.ZoomLevel >= 5) Methods.Editor.SolutionState.ZoomLevel = 5;
            if (Methods.Editor.SolutionState.ZoomLevel <= -5) Methods.Editor.SolutionState.ZoomLevel = -5;

            Instance.ViewPanel.SharpPanel.UpdateZoomLevel(Methods.Editor.SolutionState.ZoomLevel, new Point(0, 0));
        }
        public static void ZoomOut()
        {
            Methods.Editor.SolutionState.ZoomLevel -= 1;
            if (Methods.Editor.SolutionState.ZoomLevel >= 5) Methods.Editor.SolutionState.ZoomLevel = 5;
            if (Methods.Editor.SolutionState.ZoomLevel <= -5) Methods.Editor.SolutionState.ZoomLevel = -5;

            Instance.ViewPanel.SharpPanel.UpdateZoomLevel(Methods.Editor.SolutionState.ZoomLevel, new Point(0, 0));
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
            if (ManiacEditor.Methods.Editor.SolutionState.IsTilesEdit() && !ManiacEditor.Methods.Editor.SolutionState.IsChunksEdit())
            {
                if (Methods.Editor.Solution.EditLayerA != null) Methods.Editor.Solution.EditLayerA?.SelectAll();
                if (Methods.Editor.Solution.EditLayerB != null) Methods.Editor.Solution.EditLayerB?.SelectAll();
                Actions.UndoRedoModel.UpdateEditLayerActions();
            }
            else if (ManiacEditor.Methods.Editor.SolutionState.IsEntitiesEdit())
            {
                Methods.Editor.Solution.Entities.SelectAll();
            }
            Methods.Internal.UserInterface.UpdateControls();
            Methods.Editor.SolutionState.RegionX1 = -1;
            Methods.Editor.SolutionState.RegionY1 = -1;
        }
        public static void FlipHorizontal()
        {
            Methods.Editor.Solution.EditLayerA?.FlipPropertySelected(FlipDirection.Horizontal);
            Methods.Editor.Solution.EditLayerB?.FlipPropertySelected(FlipDirection.Horizontal);
            Actions.UndoRedoModel.UpdateEditLayerActions();
        }
        public static void FlipHorizontalIndividual()
        {
            Methods.Editor.Solution.EditLayerA?.FlipPropertySelected(FlipDirection.Horizontal, true);
            Methods.Editor.Solution.EditLayerB?.FlipPropertySelected(FlipDirection.Horizontal, true);
            Actions.UndoRedoModel.UpdateEditLayerActions();
        }
        public static void Delete()
        {
            Methods.Editor.EditorActions.DeleteSelected();
        }
        public static void Copy()
        {
            if (ManiacEditor.Methods.Editor.SolutionState.IsTilesEdit())
                Methods.Editor.EditorActions.CopyTilesToClipboard();


            else if (ManiacEditor.Methods.Editor.SolutionState.IsEntitiesEdit())
                Methods.Editor.EditorActions.CopyEntitiesToClipboard();


            Methods.Internal.UserInterface.UpdateControls();
        }
        public static void Duplicate()
        {
            if (ManiacEditor.Methods.Editor.SolutionState.IsTilesEdit())
            {
                Methods.Editor.Solution.EditLayerA?.PasteFromClipboard(new Point(16, 16), Methods.Editor.Solution.EditLayerA?.CopyToClipboard(true));
                Methods.Editor.Solution.EditLayerB?.PasteFromClipboard(new Point(16, 16), Methods.Editor.Solution.EditLayerB?.CopyToClipboard(true));
                Actions.UndoRedoModel.UpdateEditLayerActions();
            }
            else if (ManiacEditor.Methods.Editor.SolutionState.IsEntitiesEdit())
            {
                try
                {
                    Methods.Editor.Solution.Entities.PasteFromClipboard(new Point(16, 16), Methods.Editor.Solution.Entities.CopyToClipboard(true));
                    Actions.UndoRedoModel.UpdateLastEntityAction();
                }
                catch (Classes.Scene.EditorEntities.TooManyEntitiesException)
                {
                    System.Windows.MessageBox.Show("Too many entities! (limit: 2048)");
                    return;
                }
                Methods.Internal.UserInterface.UpdateControls();

            }
        }
        public static void Cut()
        {
            if (ManiacEditor.Methods.Editor.SolutionState.IsTilesEdit())
            {
                Methods.Editor.EditorActions.CopyTilesToClipboard();
                Methods.Editor.EditorActions.DeleteSelected();
                Methods.Internal.UserInterface.UpdateControls();
                Actions.UndoRedoModel.UpdateEditLayerActions();
            }
            else if (ManiacEditor.Methods.Editor.SolutionState.IsEntitiesEdit())
            {
                if (Instance.EntitiesToolbar.IsFocused.Equals(false))
                {
                    Methods.Editor.EditorActions.CopyEntitiesToClipboard();
                    Methods.Editor.EditorActions.DeleteSelected();
                    Methods.Internal.UserInterface.UpdateControls();
                }
            }
        }
        public static void Paste()
        {
            try
            {
                if (ManiacEditor.Methods.Editor.SolutionState.IsTilesEdit())
                {
                    // check if there are tiles on the Windows clipboard; if so, use those
                    if (System.Windows.Clipboard.ContainsData("ManiacTiles"))
                    {
                        var pasteData = (Tuple<Dictionary<Point, ushort>, Dictionary<Point, ushort>>)System.Windows.Clipboard.GetDataObject().GetData("ManiacTiles");
                        Point pastePoint = GetPastePoint();
                        if (Methods.Editor.Solution.EditLayerA != null) Methods.Editor.Solution.EditLayerA.PasteFromClipboard(pastePoint, pasteData.Item1);
                        if (Methods.Editor.Solution.EditLayerB != null) Methods.Editor.Solution.EditLayerB.PasteFromClipboard(pastePoint, pasteData.Item2);

                        Actions.UndoRedoModel.UpdateEditLayerActions();
                    }

                    // if there's none, use the internal clipboard
                    else if (Instance.TilesClipboard != null)
                    {
                        Point pastePoint = GetPastePoint();
                        if (Methods.Editor.Solution.EditLayerA != null) Methods.Editor.Solution.EditLayerA.PasteFromClipboard(pastePoint, Instance.TilesClipboard.Item1);
                        if (Methods.Editor.Solution.EditLayerB != null) Methods.Editor.Solution.EditLayerB.PasteFromClipboard(pastePoint, Instance.TilesClipboard.Item2);
                        Actions.UndoRedoModel.UpdateEditLayerActions();
                    }

                }
                else if (ManiacEditor.Methods.Editor.SolutionState.IsEntitiesEdit())
                {
                    Methods.Editor.EditorActions.PasteEntitiesToClipboard();
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("There was a problem with pasting the content provided: " + Environment.NewLine + ex.Message);
            }


            Point GetPastePoint()
            {
                if (ManiacEditor.Methods.Editor.SolutionState.IsChunksEdit())
                {

                    Point p = new Point((int)(Methods.Editor.SolutionState.LastX / Methods.Editor.SolutionState.Zoom), (int)(Methods.Editor.SolutionState.LastY / Methods.Editor.SolutionState.Zoom));
                    return Classes.Scene.EditorLayer.GetChunkCoordinatesTopEdge(p.X, p.Y);
                }
                else
                {
                    return new Point((int)(Methods.Editor.SolutionState.LastX / Methods.Editor.SolutionState.Zoom) + Methods.Editor.EditorConstants.TILE_SIZE - 1, (int)(Methods.Editor.SolutionState.LastY / Methods.Editor.SolutionState.Zoom) + Methods.Editor.EditorConstants.TILE_SIZE - 1);

                }
            }
        }
        public static void FlipVertical()
        {
            Methods.Editor.Solution.EditLayerA?.FlipPropertySelected(FlipDirection.Veritcal);
            Methods.Editor.Solution.EditLayerB?.FlipPropertySelected(FlipDirection.Veritcal);
            Actions.UndoRedoModel.UpdateEditLayerActions();
        }
        public static void FlipVerticalIndividual()
        {
            Methods.Editor.Solution.EditLayerA?.FlipPropertySelected(FlipDirection.Veritcal, true);
            Methods.Editor.Solution.EditLayerB?.FlipPropertySelected(FlipDirection.Veritcal, true);
            Actions.UndoRedoModel.UpdateEditLayerActions();
        }
        public static void EditorPlaceTile(Point position, ushort tile, Classes.Scene.EditorLayer layer, bool isDrawing = false)
        {
            if (isDrawing)
            {
                double offset = (Methods.Editor.SolutionState.DrawBrushSize / 2) * Methods.Editor.EditorConstants.TILE_SIZE;
                Point finalPosition = new Point((int)(position.X - offset), (int)(position.Y - offset));
                Dictionary<Point, ushort> tiles = new Dictionary<Point, ushort>();
                for (int x = 0; x < Methods.Editor.SolutionState.DrawBrushSize; x++)
                {
                    for (int y = 0; y < Methods.Editor.SolutionState.DrawBrushSize; y++)
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
            Methods.Editor.Solution.EditLayerA?.DeleteSelected();
            Methods.Editor.Solution.EditLayerB?.DeleteSelected();
            Actions.UndoRedoModel.UpdateEditLayerActions();

            if (ManiacEditor.Methods.Editor.SolutionState.IsEntitiesEdit())
            {
                Methods.Editor.Solution.Entities.DeleteSelected();
                Actions.UndoRedoModel.UpdateLastEntityAction();
            }
        }



        public static void FlipEntities(FlipDirection direction)
        {
            Dictionary<Classes.Scene.EditorEntity, Point> initalPos = new Dictionary<Classes.Scene.EditorEntity, Point>();
            Dictionary<Classes.Scene.EditorEntity, Point> postPos = new Dictionary<Classes.Scene.EditorEntity, Point>();
            foreach (Classes.Scene.EditorEntity e in Methods.Editor.Solution.Entities.SelectedEntities)
            {
                initalPos.Add(e, new Point(e.PositionX, e.PositionY));
            }
            Methods.Editor.Solution.Entities.Flip(direction);
            Instance.EntitiesToolbar.UpdateSelectedProperties();
            foreach (Classes.Scene.EditorEntity e in Methods.Editor.Solution.Entities.SelectedEntities)
            {
                postPos.Add(e, new Point(e.PositionX, e.PositionY));
            }
            IAction action = new ActionMultipleMoveEntities(initalPos, postPos);
            Actions.UndoRedoModel.UndoStack.Push(action);
            Actions.UndoRedoModel.RedoStack.Clear();

        }
        public static void Deselect(bool updateControls = true)
        {
            if (ManiacEditor.Methods.Editor.SolutionState.IsEditing())
            {
                Methods.Editor.Solution.EditLayerA?.Deselect();
                Methods.Editor.Solution.EditLayerB?.Deselect();

                if (ManiacEditor.Methods.Editor.SolutionState.IsEntitiesEdit()) Methods.Editor.Solution.Entities.Deselect();
                Methods.Internal.UserInterface.UpdateControls();
                if (updateControls) Actions.UndoRedoModel.UpdateEditLayerActions();
            }
        }
        public static void EditorUndo()
        {
            if (Actions.UndoRedoModel.UndoStack.Count > 0)
            {
                if (ManiacEditor.Methods.Editor.SolutionState.IsTilesEdit())
                {
                    // Deselect to apply the changes
                    Deselect();
                }
                else if (ManiacEditor.Methods.Editor.SolutionState.IsEntitiesEdit())
                {
                    if (Actions.UndoRedoModel.UndoStack.Peek() is ActionAddDeleteEntities)
                    {
                        // deselect only if delete/create
                        Deselect();
                    }
                }
                IAction act = Actions.UndoRedoModel.UndoStack.Pop();
                if (act != null)
                {
                    act.Undo();
                    Actions.UndoRedoModel.RedoStack.Push(act.Redo());
                    if (ManiacEditor.Methods.Editor.SolutionState.IsEntitiesEdit() && ManiacEditor.Methods.Editor.SolutionState.IsSelected())
                    {
                        // We need to update the properties of the selected entity
                        Instance.EntitiesToolbar.UpdateSelectedProperties();
                    }
                }
            }
            Instance.ViewPanel.SharpPanel.GraphicPanel.Render();
            Methods.Internal.UserInterface.UpdateControls();
        }
        public static void EditorRedo()
        {
            if (Actions.UndoRedoModel.RedoStack.Count > 0)
            {
                IAction act = Actions.UndoRedoModel.RedoStack.Pop();
                act.Undo();
                Actions.UndoRedoModel.UndoStack.Push(act.Redo());
                if (ManiacEditor.Methods.Editor.SolutionState.IsEntitiesEdit() && ManiacEditor.Methods.Editor.SolutionState.IsSelected())
                {
                    // We need to update the properties of the selected entity
                    Instance.EntitiesToolbar.UpdateSelectedProperties();
                }
            }
            Instance.ViewPanel.SharpPanel.GraphicPanel.Render();
            Methods.Internal.UserInterface.UpdateControls();
        }
        public static void CopyTilesToClipboard(bool doNotUseWindowsClipboard = false)
        {
            bool hasMultipleValidLayers = Methods.Editor.Solution.EditLayerA != null && Methods.Editor.Solution.EditLayerB != null;
            if (!hasMultipleValidLayers)
            {
                Dictionary<Point, ushort> copyDataA = Methods.Editor.Solution.EditLayerA?.CopyToClipboard();
                Dictionary<Point, ushort> copyDataB = Methods.Editor.Solution.EditLayerB?.CopyToClipboard();
                Tuple<Dictionary<Point, ushort>, Dictionary<Point, ushort>> copyData = new Tuple<Dictionary<Point, ushort>, Dictionary<Point, ushort>>(copyDataA, copyDataB);

                // Make a DataObject for the copied data and send it to the Windows clipboard for cross-instance copying
                if (!doNotUseWindowsClipboard)
                    Clipboard.SetDataObject(new DataObject("ManiacTiles", copyData), true);

                // Also copy to Maniac's clipboard in case it gets overwritten elsewhere
                Instance.TilesClipboard = copyData;
            }
            else if (hasMultipleValidLayers && Methods.Editor.SolutionState.MultiLayerEditMode)
            {
                Tuple<Dictionary<Point, ushort>, Dictionary<Point, ushort>> copyData = Classes.Scene.EditorLayer.CopyMultiSelectionToClipboard(Methods.Editor.Solution.EditLayerA, Methods.Editor.Solution.EditLayerB);

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
                List<Classes.Scene.EditorEntity> copyData = Methods.Editor.Solution.Entities.CopyToClipboard();

                /*
                // Prepare each Entity for the copy to release unnecessary data
                foreach (Classes.Edit.Scene.Sets.EditorEntity entity in copyData)
                   entity.PrepareForExternalCopy();

                CloseClipboard();

                // Make a DataObject for the data and send it to the Windows clipboard for cross-instance copying
                Clipboard.SetDataObject(new DataObject("ManiacEntities", copyData));*/

                // Send to Maniac's clipboard
                Instance.ObjectsClipboard = copyData;
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
                        Methods.Editor.Solution.Entities.PasteFromClipboard(new Point((int)(Methods.Editor.SolutionState.LastX / Methods.Editor.SolutionState.Zoom), (int)(Methods.Editor.SolutionState.LastY / Methods.Editor.SolutionState.Zoom)), (List<Classes.Scene.EditorEntity>)System.Windows.Clipboard.GetDataObject().GetData("ManiacEntities"));
                        Actions.UndoRedoModel.UpdateLastEntityAction();
                    }

                    // if there's none, use the internal clipboard
                    else if (Instance.ObjectsClipboard != null)
                    {
                        Methods.Editor.Solution.Entities.PasteFromClipboard(new Point((int)(Methods.Editor.SolutionState.LastX / Methods.Editor.SolutionState.Zoom), (int)(Methods.Editor.SolutionState.LastY / Methods.Editor.SolutionState.Zoom)), Instance.ObjectsClipboard);
                        Actions.UndoRedoModel.UpdateLastEntityAction();
                    }
                }
                catch (Classes.Scene.EditorEntities.TooManyEntitiesException)
                {
                    System.Windows.MessageBox.Show("Too many Classes.Edit.Scene.EditorSolution.Entities! (limit: 2048)");
                    return;
                }
                Methods.Internal.UserInterface.UpdateControls();
            }
        }
        public static void MoveEntityOrTiles(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            int x = 0, y = 0;
            int modifier = (ManiacEditor.Methods.Editor.SolutionState.IsChunksEdit() ? 8 : 1);
            if (Methods.Editor.SolutionState.UseMagnetMode)
            {
                switch (e.KeyData)
                {
                    case Keys.Up: y = (Methods.Editor.SolutionState.UseMagnetYAxis ? -Methods.Editor.SolutionState.MagnetSize : -1); break;
                    case Keys.Down: y = (Methods.Editor.SolutionState.UseMagnetYAxis ? Methods.Editor.SolutionState.MagnetSize : 1); break;
                    case Keys.Left: x = (Methods.Editor.SolutionState.UseMagnetXAxis ? -Methods.Editor.SolutionState.MagnetSize : -1); break;
                    case Keys.Right: x = (Methods.Editor.SolutionState.UseMagnetXAxis ? Methods.Editor.SolutionState.MagnetSize : 1); break;
                }
            }
            if (Methods.Editor.SolutionState.EnableFasterNudge)
            {
                if (Methods.Editor.SolutionState.UseMagnetMode)
                {
                    switch (e.KeyData)
                    {
                        case Keys.Up: y = (Methods.Editor.SolutionState.UseMagnetYAxis ? -Methods.Editor.SolutionState.MagnetSize * Methods.Editor.SolutionState.FasterNudgeAmount : -1 - Methods.Editor.SolutionState.FasterNudgeAmount); break;
                        case Keys.Down: y = (Methods.Editor.SolutionState.UseMagnetYAxis ? Methods.Editor.SolutionState.MagnetSize * Methods.Editor.SolutionState.FasterNudgeAmount : 1 + Methods.Editor.SolutionState.FasterNudgeAmount); break;
                        case Keys.Left: x = (Methods.Editor.SolutionState.UseMagnetXAxis ? -Methods.Editor.SolutionState.MagnetSize * Methods.Editor.SolutionState.FasterNudgeAmount : -1 - Methods.Editor.SolutionState.FasterNudgeAmount); break;
                        case Keys.Right: x = (Methods.Editor.SolutionState.UseMagnetXAxis ? Methods.Editor.SolutionState.MagnetSize * Methods.Editor.SolutionState.FasterNudgeAmount : 1 + Methods.Editor.SolutionState.FasterNudgeAmount); break;
                    }
                }
                else
                {
                    if (ManiacEditor.Methods.Editor.SolutionState.IsChunksEdit())
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
                            case Keys.Up: y = (-1 - Methods.Editor.SolutionState.FasterNudgeAmount) * modifier; break;
                            case Keys.Down: y = (1 + Methods.Editor.SolutionState.FasterNudgeAmount) * modifier; break;
                            case Keys.Left: x = (-1 - Methods.Editor.SolutionState.FasterNudgeAmount) * modifier; break;
                            case Keys.Right: x = (1 + Methods.Editor.SolutionState.FasterNudgeAmount) * modifier; break;
                        }
                    }

                }

            }
            if (Methods.Editor.SolutionState.UseMagnetMode == false && Methods.Editor.SolutionState.EnableFasterNudge == false)
            {
                switch (e.KeyData)
                {
                    case Keys.Up: y = -1 * modifier; break;
                    case Keys.Down: y = 1 * modifier; break;
                    case Keys.Left: x = -1 * modifier; break;
                    case Keys.Right: x = 1 * modifier; break;
                }

            }
            Methods.Editor.Solution.EditLayerA?.MoveSelectedQuonta(new Point(x, y));
            Methods.Editor.Solution.EditLayerB?.MoveSelectedQuonta(new Point(x, y));

            Actions.UndoRedoModel.UpdateEditLayerActions();

            if (ManiacEditor.Methods.Editor.SolutionState.IsEntitiesEdit())
            {
                if (Methods.Editor.SolutionState.UseMagnetMode)
                {
                    int xE = Methods.Editor.Solution.Entities.SelectedEntities[0].Position.X.High;
                    int yE = Methods.Editor.Solution.Entities.SelectedEntities[0].Position.Y.High;

                    if (xE % Methods.Editor.SolutionState.MagnetSize != 0 && Methods.Editor.SolutionState.UseMagnetXAxis)
                    {
                        int offsetX = x % Methods.Editor.SolutionState.MagnetSize;
                        x -= offsetX;
                    }
                    if (yE % Methods.Editor.SolutionState.MagnetSize != 0 && Methods.Editor.SolutionState.UseMagnetYAxis)
                    {
                        int offsetY = y % Methods.Editor.SolutionState.MagnetSize;
                        y -= offsetY;
                    }
                }


                Methods.Editor.Solution.Entities.MoveSelected(new Point(0, 0), new Point(x, y), false);
                Instance.EntitiesToolbar.UpdateSelectedProperties();

                // Try to merge with last move
                List<Classes.Scene.EditorEntity> SelectedList = Methods.Editor.Solution.Entities.SelectedEntities.ToList();
                List<Classes.Scene.EditorEntity> SelectedInternalList = Methods.Editor.Solution.Entities.SelectedInternalEntities.ToList();
                bool selectedActionsState = Actions.UndoRedoModel.UndoStack.Count > 0 && Actions.UndoRedoModel.UndoStack.Peek() is ActionMoveEntities && (Actions.UndoRedoModel.UndoStack.Peek() as ActionMoveEntities).UpdateFromKey(SelectedList, new Point(x, y));
                bool selectedInternalActionsState = Actions.UndoRedoModel.UndoStack.Count > 0 && Actions.UndoRedoModel.UndoStack.Peek() is ActionMoveEntities && (Actions.UndoRedoModel.UndoStack.Peek() as ActionMoveEntities).UpdateFromKey(SelectedInternalList, new Point(x, y));

                if (selectedActionsState || selectedInternalActionsState) { }
                else
                {
                    if (SelectedList.Count != 0) Actions.UndoRedoModel.UndoStack.Push(new ActionMoveEntities(SelectedList, new Point(x, y), true));
                    if (SelectedInternalList.Count != 0) Actions.UndoRedoModel.UndoStack.Push(new ActionMoveEntities(SelectedInternalList, new Point(x, y), true));

                    Actions.UndoRedoModel.RedoStack.Clear();
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
        public static void GoToPosition(int x, int y, bool CenterCoords = true)
        {
            if (Methods.Editor.SolutionState.UnlockCamera) CenterCoords = true;

            /*
            if (CenterCoords)
            {
                Rectangle r = Instance.ViewPanel.SharpPanel.GraphicPanel.GetScreen();
                int x2 = (int)(r.Width * Methods.Editor.SolutionState.Zoom);
                int y2 = (int)(r.Height * Methods.Editor.SolutionState.Zoom);

                int ResultX = (int)(x * Methods.Editor.SolutionState.Zoom) - x2 / 2;
                int ResultY = (int)(y * Methods.Editor.SolutionState.Zoom) - y2 / 2;

                if ((ResultX <= 0 && !Methods.Editor.SolutionState.UnlockCamera)) ResultX = 0;
                if ((ResultY <= 0 && !Methods.Editor.SolutionState.UnlockCamera)) ResultY = 0;


                Methods.Editor.SolutionState.SetViewPositionX(ResultX);
                Methods.Editor.SolutionState.SetViewPositionY(ResultY);
            }
            else
            {*/
                int ResultX = (int)(x * Methods.Editor.SolutionState.Zoom);
                int ResultY = (int)(y * Methods.Editor.SolutionState.Zoom);

                if ((ResultX <= 0)) ResultX = 0;
                if ((ResultY <= 0)) ResultY = 0;

                Methods.Editor.SolutionState.SetViewPositionX(ResultX);
                Methods.Editor.SolutionState.SetViewPositionY(ResultY);
            //}
        }

        #endregion

        #region Specific Editor Functions

        public static void ChangeLevelID(object sender, RoutedEventArgs e)
        {
            string inputValue = GenerationsLib.WPF.TextPrompt2.ShowDialog("Change Level ID", "This is only temporary and will reset when you reload the scene.", Methods.Editor.SolutionState.LevelID.ToString());
            int.TryParse(inputValue.ToString(), out int output);
            Methods.Editor.SolutionState.LevelID = output;
            Instance.EditorStatusBar._levelIDLabel.Content = "Level ID: " + Methods.Editor.SolutionState.LevelID.ToString();
        }
        public static void MakeShortcutForDataFolderOnly(object sender, RoutedEventArgs e)
        {
            string dataDir = ManiacEditor.Methods.Editor.SolutionPaths.CurrentSceneData.MasterDataDirectory;
            Methods.Editor.EditorActions.CreateShortcut(dataDir);
        }
        public static void MakeShortcutWithCurrentCoordinatesToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            string dataDir = ManiacEditor.Methods.Editor.SolutionPaths.CurrentSceneData.MasterDataDirectory;
            string scenePath = ManiacEditor.Methods.Editor.SolutionPaths.GetScenePath();
            int rX = (short)(Methods.Editor.SolutionState.ViewPositionX);
            int rY = (short)(Methods.Editor.SolutionState.ViewPositionY);
            double _ZoomLevel = Methods.Editor.SolutionState.ZoomLevel;
            bool isEncoreSet = Methods.Editor.SolutionState.UseEncoreColors;
            int levelSlotNum = Methods.Editor.SolutionState.LevelID;
            Methods.Editor.EditorActions.CreateShortcut(dataDir, scenePath, "", rX, rY, isEncoreSet, levelSlotNum, _ZoomLevel);
        }
        public static void MakeShortcutWithoutCurrentCoordinatesToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            string dataDir = ManiacEditor.Methods.Editor.SolutionPaths.CurrentSceneData.MasterDataDirectory;
            string scenePath = ManiacEditor.Methods.Editor.SolutionPaths.GetScenePath();
            int rX = 0;
            int rY = 0;
            bool isEncoreSet = Methods.Editor.SolutionState.UseEncoreColors;
            int levelSlotNum = Methods.Editor.SolutionState.LevelID;
            Methods.Editor.EditorActions.CreateShortcut(dataDir, scenePath, "", rX, rY, isEncoreSet, levelSlotNum);
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
            ManiacEditor.Controls.Utility.Editors.Dev.MD5HashGen hashmap = new ManiacEditor.Controls.Utility.Editors.Dev.MD5HashGen(Instance);
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
                    Instance.FindAndReplace.EditorTileFindReplace(find, replace, applyState, copyResults);//, perserveColllision
                }
                else
                {
                    Instance.FindAndReplace.EditorTileFind(find, applyState, copyResults);
                }

            }

        }
        public static void ConsoleWindowToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (!Methods.Editor.SolutionState.IsConsoleWindowOpen)
            {
                Methods.Editor.SolutionState.IsConsoleWindowOpen = true;
                Extensions.ExternalExtensions.ShowConsoleWindow();
            }
            else
            {
                Methods.Editor.SolutionState.IsConsoleWindowOpen = false;
                Extensions.ExternalExtensions.HideConsoleWindow();
            }
        }
        public static void SaveForForceOpenOnStartupToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.MyDevSettings.DevForceRestartData = ManiacEditor.Methods.Editor.SolutionPaths.CurrentSceneData.MasterDataDirectory;
            Properties.Settings.MyDevSettings.DevForceRestartScene = ManiacEditor.Methods.Editor.SolutionPaths.CurrentSceneData.FilePath;
            Properties.Settings.MyDevSettings.DevForceRestartX = (short)(Methods.Editor.SolutionState.ViewPositionX / Methods.Editor.SolutionState.Zoom);
            Properties.Settings.MyDevSettings.DevForceRestartY = (short)(Methods.Editor.SolutionState.ViewPositionY / Methods.Editor.SolutionState.Zoom);
            Properties.Settings.MyDevSettings.DevForceRestartZoomLevel = Methods.Editor.SolutionState.ZoomLevel;
            Properties.Settings.MyDevSettings.DevForceRestartIsEncore = ManiacEditor.Methods.Editor.SolutionPaths.CurrentSceneData.IsEncoreMode;
            Properties.Settings.MyDevSettings.DevForceRestartID = Methods.Editor.SolutionState.LevelID;
            Properties.Settings.MyDevSettings.DevForceRestartCurrentName = ManiacEditor.Methods.Editor.SolutionPaths.CurrentSceneData.Name;
            Properties.Settings.MyDevSettings.DevForceRestartCurrentZone = ManiacEditor.Methods.Editor.SolutionPaths.CurrentSceneData.Zone;
            Properties.Settings.MyDevSettings.DevForceRestartSceneID = ManiacEditor.Methods.Editor.SolutionPaths.CurrentSceneData.SceneID;
            Properties.Settings.MyDevSettings.DevForceRestartIsBrowsed = ManiacEditor.Methods.Editor.SolutionPaths.CurrentSceneData.IsFullPath;
            Properties.Settings.MyDevSettings.DevForceRestartResourcePacks = new System.Collections.Specialized.StringCollection();
            Properties.Settings.MyDevSettings.DevForceRestartResourcePacks.AddRange(ManiacEditor.Methods.Editor.SolutionPaths.CurrentSceneData.ExtraDataDirectories.ToArray());
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
            if (Properties.Settings.MyDefaults.ShowEncoreEntities == true && Properties.Settings.MyDefaults.ShowManiaEntities == true)
            {
                Properties.Settings.MyDefaults.ShowManiaEntities = true;
                Properties.Settings.MyDefaults.ShowEncoreEntities = false;
            }
            if (Properties.Settings.MyDefaults.ShowEncoreEntities == true && Properties.Settings.MyDefaults.ShowManiaEntities == false)
            {
                Properties.Settings.MyDefaults.ShowManiaEntities = true;
                Properties.Settings.MyDefaults.ShowEncoreEntities = false;
            }
            else
            {
                Properties.Settings.MyDefaults.ShowManiaEntities = false;
                Properties.Settings.MyDefaults.ShowEncoreEntities = true;
            }

        }

        public static void SetScrollLockDirection()
        {
            if (Methods.Editor.SolutionState.ScrollDirection == Axis.X)
            {
                Methods.Editor.SolutionState.ScrollDirection = Axis.Y;
                Instance.EditorStatusBar.UpdateStatusPanel();
                Instance.MenuBar.xToolStripMenuItem.IsChecked = false;
                Instance.MenuBar.yToolStripMenuItem.IsChecked = true;
            }
            else
            {
                Methods.Editor.SolutionState.ScrollDirection = Axis.X;
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
                            Methods.Editor.SolutionState.CurrentControllerButtons = 2;
                            break;
                        case "Switch":
                            Methods.Editor.SolutionState.CurrentControllerButtons = 4;
                            break;
                        case "PS4":
                            Methods.Editor.SolutionState.CurrentControllerButtons = 3;
                            break;
                        case "Saturn Black":
                            Methods.Editor.SolutionState.CurrentControllerButtons = 5;
                            break;
                        case "Saturn White":
                            Methods.Editor.SolutionState.CurrentControllerButtons = 6;
                            break;
                        case "Switch Joy L":
                            Methods.Editor.SolutionState.CurrentControllerButtons = 7;
                            break;
                        case "Switch Joy R":
                            Methods.Editor.SolutionState.CurrentControllerButtons = 8;
                            break;
                        case "PC EN/JP":
                            Methods.Editor.SolutionState.CurrentControllerButtons = 1;
                            break;
                        case "PC FR":
                            Methods.Editor.SolutionState.CurrentControllerButtons = 9;
                            break;
                        case "PC IT":
                            Methods.Editor.SolutionState.CurrentControllerButtons = 10;
                            break;
                        case "PC GE":
                            Methods.Editor.SolutionState.CurrentControllerButtons = 11;
                            break;
                        case "PC SP":
                            Methods.Editor.SolutionState.CurrentControllerButtons = 12;
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
                    Methods.Editor.SolutionState.CurrentControllerButtons = 2;
                    break;
                case "Switch":
                    Methods.Editor.SolutionState.CurrentControllerButtons = 4;
                    break;
                case "PS4":
                    Methods.Editor.SolutionState.CurrentControllerButtons = 3;
                    break;
                case "Saturn Black":
                    Methods.Editor.SolutionState.CurrentControllerButtons = 5;
                    break;
                case "Saturn White":
                    Methods.Editor.SolutionState.CurrentControllerButtons = 6;
                    break;
                case "Switch Joy L":
                    Methods.Editor.SolutionState.CurrentControllerButtons = 7;
                    break;
                case "Switch Joy R":
                    Methods.Editor.SolutionState.CurrentControllerButtons = 8;
                    break;
                case "PC EN/JP":
                    Methods.Editor.SolutionState.CurrentControllerButtons = 1;
                    break;
                case "PC FR":
                    Methods.Editor.SolutionState.CurrentControllerButtons = 9;
                    break;
                case "PC IT":
                    Methods.Editor.SolutionState.CurrentControllerButtons = 10;
                    break;
                case "PC GE":
                    Methods.Editor.SolutionState.CurrentControllerButtons = 11;
                    break;
                case "PC SP":
                    Methods.Editor.SolutionState.CurrentControllerButtons = 12;
                    break;
            }
        }
        public static void SetEncorePallete(object sender = null, string path = "")
        {
            if (sender != null)
            {
                var clickedItem = sender as System.Windows.Controls.MenuItem;
                string StartDir = ManiacEditor.Methods.Editor.SolutionPaths.CurrentSceneData.MasterDataDirectory;
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
                            ManiacEditor.Methods.Editor.SolutionPaths.EncorePalette = Methods.Editor.Solution.CurrentScene.GetEncorePalette("", "", "", "", -1, fd.FileName);
                            Methods.Editor.SolutionState.EncoreSetupType = 0;
                            if (File.Exists(ManiacEditor.Methods.Editor.SolutionPaths.EncorePalette[0]))
                            {
                                Methods.Editor.SolutionState.EncorePaletteExists = true;
                                Methods.Editor.SolutionState.UseEncoreColors = true;
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
                ManiacEditor.Methods.Editor.SolutionPaths.EncorePalette = Methods.Editor.Solution.CurrentScene.GetEncorePalette("", "", "", "", -1, path);
                Methods.Editor.SolutionState.EncoreSetupType = 0;
                if (File.Exists(ManiacEditor.Methods.Editor.SolutionPaths.EncorePalette[0]))
                {
                    Methods.Editor.SolutionState.EncorePaletteExists = true;
                    Methods.Editor.SolutionState.UseEncoreColors = true;
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
            if (sender is System.Windows.Controls.TextBox && LockEntityFilterTextChanged == false && Methods.Editor.Solution.Entities != null)
            {
                LockEntityFilterTextChanged = true;
                System.Windows.Controls.TextBox theSender = sender as System.Windows.Controls.TextBox;
                Methods.Editor.SolutionState.ObjectFilter = theSender.Text;
                Instance.MenuBar.EntityFilterTextbox.Text = Methods.Editor.SolutionState.ObjectFilter;
                Classes.Scene.EditorEntities.ObjectRefreshNeeded = true;
                LockEntityFilterTextChanged = false;
            }
            else
            {
                LockEntityFilterTextChanged = true;
                Methods.Editor.SolutionState.ObjectFilter = string.Empty;
                Instance.MenuBar.EntityFilterTextbox.Text = string.Empty;
                LockEntityFilterTextChanged = false;
            }


        }
        public static void ManiaMenuLanguageChanged(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.MenuItem menuItem = sender as System.Windows.Controls.MenuItem;
            Methods.Editor.SolutionState.CurrentLanguage = menuItem.Tag.ToString();
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
