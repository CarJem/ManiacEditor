using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ManiacEditor.Methods.Solution
{
    public static class SolutionMultiLayer
    {
        public static void Copy()
        {
            var copyData = Classes.Scene.EditorLayer.GetMultiClipboardData();

            // Also copy to Maniac's clipboard in case it gets overwritten elsewhere
            Methods.Solution.SolutionClipboard.SetTileClipboard(copyData);
        }
        public static void Paste()
        {
            // check if there are tiles on the Windows clipboard; if so, use those
            if (System.Windows.Clipboard.ContainsData("ManiacTiles"))
            {
                var pasteData = (List<Dictionary<Point, ushort>>)System.Windows.Clipboard.GetDataObject().GetData("ManiacTiles");
                Point pastePoint = GetPastePoint();
                if (Methods.Solution.CurrentSolution.EditLayerA != null) Methods.Solution.CurrentSolution.EditLayerA.PasteClipboardData(pastePoint, new Methods.Solution.SolutionClipboard.TilesClipboardEntry(pasteData[0]));
                if (Methods.Solution.CurrentSolution.EditLayerB != null) Methods.Solution.CurrentSolution.EditLayerB.PasteClipboardData(pastePoint, new Methods.Solution.SolutionClipboard.TilesClipboardEntry(pasteData[1]));
                if (Methods.Solution.CurrentSolution.EditLayerC != null) Methods.Solution.CurrentSolution.EditLayerC.PasteClipboardData(pastePoint, new Methods.Solution.SolutionClipboard.TilesClipboardEntry(pasteData[2]));
                if (Methods.Solution.CurrentSolution.EditLayerD != null) Methods.Solution.CurrentSolution.EditLayerD.PasteClipboardData(pastePoint, new Methods.Solution.SolutionClipboard.TilesClipboardEntry(pasteData[3]));

                ManiacEditor.Actions.UndoRedoModel.UpdateEditLayersActions();
            }

            // if there's none, use the internal clipboard
            else if (Methods.Solution.SolutionClipboard.TilesClipboard != null)
            {
                Point pastePoint = GetPastePoint();
                if (Methods.Solution.CurrentSolution.EditLayerA != null) Methods.Solution.CurrentSolution.EditLayerA.PasteClipboardData(pastePoint, new Methods.Solution.SolutionClipboard.TilesClipboardEntry(Methods.Solution.SolutionClipboard.TilesClipboard.GetData()[0]));
                if (Methods.Solution.CurrentSolution.EditLayerB != null) Methods.Solution.CurrentSolution.EditLayerB.PasteClipboardData(pastePoint, new Methods.Solution.SolutionClipboard.TilesClipboardEntry(Methods.Solution.SolutionClipboard.TilesClipboard.GetData()[1]));
                if (Methods.Solution.CurrentSolution.EditLayerA != null) Methods.Solution.CurrentSolution.EditLayerA.PasteClipboardData(pastePoint, new Methods.Solution.SolutionClipboard.TilesClipboardEntry(Methods.Solution.SolutionClipboard.TilesClipboard.GetData()[2]));
                if (Methods.Solution.CurrentSolution.EditLayerB != null) Methods.Solution.CurrentSolution.EditLayerB.PasteClipboardData(pastePoint, new Methods.Solution.SolutionClipboard.TilesClipboardEntry(Methods.Solution.SolutionClipboard.TilesClipboard.GetData()[3]));
                ManiacEditor.Actions.UndoRedoModel.UpdateEditLayersActions();
            }


            Point GetPastePoint()
            {
                if (ManiacEditor.Methods.Solution.SolutionState.Main.IsChunksEdit())
                {

                    Point p = Methods.Solution.SolutionState.Main.GetLastXY();
                    return Classes.Scene.EditorLayer.GetChunkCoordinatesTopEdge(p.X, p.Y);
                }
                else
                {
                    return Methods.Solution.SolutionState.Main.GetLastXY();
                }
            }
        }
        public static bool IsSelected()
        {
            bool SelectedA = Methods.Solution.CurrentSolution.EditLayerA?.SelectedTiles.Count > 0 || Methods.Solution.CurrentSolution.EditLayerA?.TempSelectionTiles.Count > 0;
            bool SelectedB = Methods.Solution.CurrentSolution.EditLayerB?.SelectedTiles.Count > 0 || Methods.Solution.CurrentSolution.EditLayerB?.TempSelectionTiles.Count > 0;
            bool SelectedC = Methods.Solution.CurrentSolution.EditLayerC?.SelectedTiles.Count > 0 || Methods.Solution.CurrentSolution.EditLayerC?.TempSelectionTiles.Count > 0;
            bool SelectedD = Methods.Solution.CurrentSolution.EditLayerD?.SelectedTiles.Count > 0 || Methods.Solution.CurrentSolution.EditLayerD?.TempSelectionTiles.Count > 0;
            return SelectedA || SelectedB || SelectedC || SelectedD;
        }
        public static List<ushort> GetSelectedValues()
        {
            List<ushort> valuesAll = new List<ushort>();

            var valuesA = Methods.Solution.CurrentSolution.EditLayerA?.GetSelectedValues();
            var valuesB = Methods.Solution.CurrentSolution.EditLayerB?.GetSelectedValues();
            var valuesC = Methods.Solution.CurrentSolution.EditLayerC?.GetSelectedValues();
            var valuesD = Methods.Solution.CurrentSolution.EditLayerD?.GetSelectedValues();

            if (valuesA != null) valuesAll.AddRange(valuesA);
            if (valuesB != null) valuesAll.AddRange(valuesB);
            if (valuesC != null) valuesAll.AddRange(valuesC);
            if (valuesD != null) valuesAll.AddRange(valuesD);

            return valuesAll;
        }
        public static void SetPropertySelected(int bit, bool state)
        {
            Methods.Solution.CurrentSolution.EditLayerA?.SetPropertySelected(bit, state);
            Methods.Solution.CurrentSolution.EditLayerB?.SetPropertySelected(bit, state);
            Methods.Solution.CurrentSolution.EditLayerC?.SetPropertySelected(bit, state);
            Methods.Solution.CurrentSolution.EditLayerD?.SetPropertySelected(bit, state);
        }
        public static void RemoveChunk(Controls.Editor.MainEditor Instance, Point pC)
        {
            if (!Instance.Chunks.IsChunkEmpty(pC, Methods.Solution.CurrentSolution.EditLayerA, Methods.Solution.CurrentSolution.EditLayerB))
            {
                // Remove Stamp Sized Area
                Instance.Chunks.PasteStamp(pC, 0, Methods.Solution.CurrentSolution.EditLayerA, Methods.Solution.CurrentSolution.EditLayerB, true);
            }
        }
        public static void PlaceChunk(Controls.Editor.MainEditor Instance, Point pC)
        {
            int selectedIndex = Instance.TilesToolbar.ChunkList.SelectedIndex;
            // Place Stamp
            if (selectedIndex != -1)
            {
                if (!Instance.Chunks.DoesChunkMatch(pC, Instance.Chunks.GetStamp(selectedIndex), Methods.Solution.CurrentSolution.EditLayerA, Methods.Solution.CurrentSolution.EditLayerB))
                {
                    Instance.Chunks.PasteStamp(pC, selectedIndex, Methods.Solution.CurrentSolution.EditLayerA, Methods.Solution.CurrentSolution.EditLayerB);
                }
            }
        }
        public static void RemoveTile(Controls.Editor.MainEditor Instance, Point p)
        {
            // Remove tile
            if (Methods.Solution.SolutionState.Main.DrawBrushSize == 1)
            {
                Methods.Solution.CurrentSolution.EditLayerA?.EraseTiles(p);
                Methods.Solution.CurrentSolution.EditLayerB?.EraseTiles(p);
            }
            else
            {

                int lastX = p.X / Solution.SolutionConstants.TILE_SIZE * Solution.SolutionConstants.TILE_SIZE;
                int lastY = p.Y / Solution.SolutionConstants.TILE_SIZE * Solution.SolutionConstants.TILE_SIZE;


                int oddMod = ((Methods.Solution.SolutionState.Main.DrawBrushSize) % 2) * Solution.SolutionConstants.TILE_SIZE;
                int offset = ((Methods.Solution.SolutionState.Main.DrawBrushSize) / 2) * Solution.SolutionConstants.TILE_SIZE;
                int x1 = (int)(lastX) - offset;
                int x2 = (int)(lastX) + offset + oddMod;
                int y1 = (int)(lastY) - offset;
                int y2 = (int)(lastY) + offset + oddMod;

                int width = x2 - x1;
                int height = y2 - y1;

                Methods.Solution.CurrentSolution.EditLayerA?.EraseTiles(new Rectangle(x1, y1, width, height));
                Methods.Solution.CurrentSolution.EditLayerB?.EraseTiles(new Rectangle(x1, y1, width, height));
            }
        }
        public static void PlaceTile(Controls.Editor.MainEditor Instance, Point p)
        {
            if (Methods.Solution.SolutionState.Main.DrawBrushSize == 1)
            {
                if (Instance.TilesToolbar.SelectedTileIndex != -1)
                {
                    if (Methods.Solution.CurrentSolution.EditLayerA.GetTileAt(p) != Instance.TilesToolbar.SelectedTileIndex) Methods.Solution.CurrentSolution.EditLayerA.PlaceTile(p, Instance.TilesToolbar.SelectedTile);
                    else if (!Methods.Solution.CurrentSolution.EditLayerA.IsPointSelected(p)) Methods.Solution.CurrentSolution.EditLayerA.Select(p);
                }
            }
            else
            {
                if (Instance.TilesToolbar.SelectedTileIndex != -1) Methods.Solution.CurrentSolution.EditLayerA.PlaceTile(p, Instance.TilesToolbar.SelectedTile, true);
            }
        }
        public static ushort GetTileAt(Point point)
        {
            ushort tileA = (ushort)(Methods.Solution.CurrentSolution.EditLayerA?.GetTileAt(point) ?? 0xffff);
            ushort tileB = (ushort)(Methods.Solution.CurrentSolution.EditLayerB?.GetTileAt(point) ?? 0xffff);
            ushort tileC = (ushort)(Methods.Solution.CurrentSolution.EditLayerC?.GetTileAt(point) ?? 0xffff);
            ushort tileD = (ushort)(Methods.Solution.CurrentSolution.EditLayerD?.GetTileAt(point) ?? 0xffff);


            if (tileD != 0xffff) return tileD;
            else if (tileC != 0xffff) return tileC;
            else if (tileB != 0xffff) return tileB;
            else return tileA;
        }
        public static void StampPlace(Controls.Editor.MainEditor Instance, System.Windows.Forms.MouseEventArgs e)
        {
            Point p = new Point((int)(e.X), (int)(e.Y));
            Point pC = Classes.Scene.EditorLayer.GetChunkCoordinates(p.X, p.Y);
            int selectedIndex = Instance.TilesToolbar.ChunkList.SelectedIndex;
            // Place Stamp
            if (selectedIndex != -1)
            {
                if (!Instance.Chunks.DoesChunkMatch(pC, Instance.Chunks.GetStamp(selectedIndex), Methods.Solution.CurrentSolution.EditLayerA, Methods.Solution.CurrentSolution.EditLayerB))
                {
                    Instance.Chunks.PasteStamp(pC, selectedIndex, Methods.Solution.CurrentSolution.EditLayerA, Methods.Solution.CurrentSolution.EditLayerB);
                }

            }
        }
        public static void StampRemove(System.Windows.Forms.MouseEventArgs e)
        {
            Point p = new Point((int)(e.X), (int)(e.Y));
            Point chunk_point = Classes.Scene.EditorLayer.GetChunkCoordinatesTopEdge(p.X, p.Y);
            Rectangle clicked_chunk = new Rectangle(chunk_point.X, chunk_point.Y, 128, 128);

            // Remove Stamp Sized Area
            if (!Methods.Solution.CurrentSolution.EditLayerA.DoesChunkContainASelectedTile(p)) Methods.Solution.CurrentSolution.EditLayerA?.Select(clicked_chunk);
            if (Methods.Solution.CurrentSolution.EditLayerB != null && !Methods.Solution.CurrentSolution.EditLayerB.DoesChunkContainASelectedTile(p)) Methods.Solution.CurrentSolution.EditLayerB?.Select(clicked_chunk);
            ManiacEditor.Methods.Solution.SolutionActions.Delete();
        }
        public static void StartDraw()
        {
            Methods.Solution.CurrentSolution.EditLayerA?.StartDraw();
            Methods.Solution.CurrentSolution.EditLayerB?.StartDraw();
            Methods.Solution.CurrentSolution.EditLayerC?.StartDraw();
            Methods.Solution.CurrentSolution.EditLayerD?.StartDraw();
        }
        public static void EndDraw()
        {
            Methods.Solution.CurrentSolution.EditLayerA?.EndDraw();
            Methods.Solution.CurrentSolution.EditLayerB?.EndDraw();
            Methods.Solution.CurrentSolution.EditLayerC?.EndDraw();
            Methods.Solution.CurrentSolution.EditLayerD?.EndDraw();
        }
        public static void EndTempSelection()
        {
            Methods.Solution.CurrentSolution.EditLayerA?.EndTempSelection();
            Methods.Solution.CurrentSolution.EditLayerB?.EndTempSelection();
            Methods.Solution.CurrentSolution.EditLayerC?.EndTempSelection();
            Methods.Solution.CurrentSolution.EditLayerD?.EndTempSelection();
        }
        public static void Select(Rectangle area, bool addSelection = false, bool deselectIfSelected = false)
        {
            Methods.Solution.CurrentSolution.EditLayerA?.Select(area, addSelection, deselectIfSelected);
            Methods.Solution.CurrentSolution.EditLayerB?.Select(area, addSelection, deselectIfSelected);
            Methods.Solution.CurrentSolution.EditLayerC?.Select(area, addSelection, deselectIfSelected);
            Methods.Solution.CurrentSolution.EditLayerD?.Select(area, addSelection, deselectIfSelected);
        }
        public static void SelectChunk(Point point, bool addSelection = false, bool deselectIfSelected = false)
        {
            Methods.Solution.CurrentSolution.EditLayerA?.SelectChunk(point, addSelection, deselectIfSelected);
            Methods.Solution.CurrentSolution.EditLayerB?.SelectChunk(point, addSelection, deselectIfSelected);
            Methods.Solution.CurrentSolution.EditLayerC?.SelectChunk(point, addSelection, deselectIfSelected);
            Methods.Solution.CurrentSolution.EditLayerD?.SelectChunk(point, addSelection, deselectIfSelected);
        }
        public static void Select(Point point, bool addSelection = false, bool deselectIfSelected = false)
        {
            Methods.Solution.CurrentSolution.EditLayerA?.Select(point, addSelection, deselectIfSelected);
            Methods.Solution.CurrentSolution.EditLayerB?.Select(point, addSelection, deselectIfSelected);
            Methods.Solution.CurrentSolution.EditLayerC?.Select(point, addSelection, deselectIfSelected);
            Methods.Solution.CurrentSolution.EditLayerD?.Select(point, addSelection, deselectIfSelected);
        }
        public static void StartDrag()
        {
            Methods.Solution.CurrentSolution.EditLayerA?.StartDrag();
            Methods.Solution.CurrentSolution.EditLayerB?.StartDrag();
            Methods.Solution.CurrentSolution.EditLayerC?.StartDrag();
            Methods.Solution.CurrentSolution.EditLayerD?.StartDrag();
        }
        public static bool DoesChunkContainASelectedTile(Point point)
        {
            bool isA = Methods.Solution.CurrentSolution.EditLayerA?.DoesChunkContainASelectedTile(point) ?? false;
            bool isB = Methods.Solution.CurrentSolution.EditLayerB?.DoesChunkContainASelectedTile(point) ?? false;
            bool isC = Methods.Solution.CurrentSolution.EditLayerC?.DoesChunkContainASelectedTile(point) ?? false;
            bool isD = Methods.Solution.CurrentSolution.EditLayerD?.DoesChunkContainASelectedTile(point) ?? false;

            return (isA || isB || isC || isD);
        }
        public static bool IsPointSelected(Point point)
        {
            bool isA = Methods.Solution.CurrentSolution.EditLayerA?.IsPointSelected(point) ?? false;
            bool isB = Methods.Solution.CurrentSolution.EditLayerB?.IsPointSelected(point) ?? false;
            bool isC = Methods.Solution.CurrentSolution.EditLayerC?.IsPointSelected(point) ?? false;
            bool isD = Methods.Solution.CurrentSolution.EditLayerD?.IsPointSelected(point) ?? false;

            return (isA || isB || isC || isD);
        }
        public static bool HasTileAt(Point point)
        {
            bool isA = Methods.Solution.CurrentSolution.EditLayerA?.HasTileAt(point) ?? false;
            bool isB = Methods.Solution.CurrentSolution.EditLayerB?.HasTileAt(point) ?? false;
            bool isC = Methods.Solution.CurrentSolution.EditLayerC?.HasTileAt(point) ?? false;
            bool isD = Methods.Solution.CurrentSolution.EditLayerD?.HasTileAt(point) ?? false;

            return (isA || isB || isC || isD);
        }
        public static void MoveSelected(Point oldPos, Point newPos, bool duplicate)
        {
            Methods.Solution.CurrentSolution.EditLayerA?.MoveSelected(oldPos, newPos, duplicate);
            Methods.Solution.CurrentSolution.EditLayerB?.MoveSelected(oldPos, newPos, duplicate);
            Methods.Solution.CurrentSolution.EditLayerC?.MoveSelected(oldPos, newPos, duplicate);
            Methods.Solution.CurrentSolution.EditLayerD?.MoveSelected(oldPos, newPos, duplicate);
        }
        public static void TempSelection(Rectangle area, bool deselectIfSelected)
        {
            Methods.Solution.CurrentSolution.EditLayerA?.TempSelection(area, deselectIfSelected);
            Methods.Solution.CurrentSolution.EditLayerB?.TempSelection(area, deselectIfSelected);
            Methods.Solution.CurrentSolution.EditLayerC?.TempSelection(area, deselectIfSelected);
            Methods.Solution.CurrentSolution.EditLayerD?.TempSelection(area, deselectIfSelected);
        }
    }
}
