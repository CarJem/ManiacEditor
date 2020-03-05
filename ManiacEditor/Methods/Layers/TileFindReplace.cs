using System;
using System.Collections.Generic;
using Point = System.Drawing.Point;
using System.Drawing;
using System.Windows.Forms;

namespace ManiacEditor.Methods.Layers
{
    public class TileFindReplace
    {
        public Controls.Editor.MainEditor Editor;

        public TileFindReplace(Controls.Editor.MainEditor instance)
        {
            Editor = instance;
        }

        #region Find Unused Tiles
        public void FindUnusedTiles()
        {
            Methods.Internal.UserInterface.UpdateWaitingScreen(true);
            Methods.Internal.UserInterface.ToggleEditorButtons(false);
            List<int> UnusedTiles = new List<int> { };


            System.Threading.Thread thread = new System.Threading.Thread(() =>
            {
                for (int i = 0; i < 1024; i++)
                {
                    if (Editor.TilesToolbar != null) Editor.TilesToolbar.SelectedTileLabel.Content = "Selected Tile: " + i;
                    bool Unusued = IsTileUnused(i);
                    if (Unusued) UnusedTiles.Add(i);
                }
                Editor.Dispatcher.Invoke(new Action(() => ShowUnusedTiles(UnusedTiles)));
            })
            { IsBackground = true };
            thread.Start();

        }
        public bool IsTileUnused(int tile)
        {
            IEnumerable<Classes.Scene.EditorLayer> AllLayers = Methods.Editor.Solution.CurrentScene.AllLayers;
            bool unused = true;

            foreach (var editorLayer in Methods.Editor.Solution.CurrentScene.AllLayers)
            {
                for (int x = 0; x < editorLayer.Layer.Tiles.Length; x++)
                {
                    for (int y = 0; y < editorLayer.Layer.Tiles[x].Length; y++)
                    {
                        ushort currentTile = editorLayer.Layer.Tiles[x][y];
                        int tileIndex = (currentTile & 0x3ff);
                        if (tileIndex == tile) unused = false;

                    }
                }
            }
            return unused;
        }
        public void ShowUnusedTiles(List<int> UnusedTiles)
        {
            if (UnusedTiles.Count != 0)
            {
                var message = "";
                int overflowMax = 32;
                int currentRowPos = 0;
                for (int i = 0; i < UnusedTiles.Count; i++)
                {
                    if (i != UnusedTiles.Count - 1)
                    {
                        if (currentRowPos > overflowMax)
                        {
                            currentRowPos = 0;
                            message += Environment.NewLine;
                            message += string.Format("{0},", UnusedTiles[i]);
                            currentRowPos++;
                        }
                        else
                        {
                            message += string.Format("{0},", UnusedTiles[i]);
                            currentRowPos++;
                        }

                    }
                    else
                    {
                        message += string.Format("{0}", UnusedTiles[i]);
                    }
                }
                //System.Windows.MessageBoxResult result = System.Windows.MessageBox.ShowYesNo("Tiles not used are: " + Environment.NewLine + message, "Results", "Copy to Clipboard", "OK", System.Windows.MessageBoxImage.Information);
                System.Windows.MessageBoxResult result = System.Windows.MessageBox.Show("Tiles not used are: " + Environment.NewLine + message, "Results", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Information);
                if (result == System.Windows.MessageBoxResult.Yes)
                {
                    System.Windows.Forms.Clipboard.SetText(message);
                }
            }
            else
            {
                System.Windows.MessageBox.Show("Found Nothing", "Results", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            }
            Methods.Internal.UserInterface.UpdateWaitingScreen(false);
            Methods.Internal.UserInterface.ToggleEditorButtons(true);
        }
        #endregion

        #region Unsorted
        public void EditorTileReplaceTest(int findValue, int replaceValue, int applyState, bool copyResults, bool perserveColllision)
        {
            if (ManiacEditor.Methods.Editor.SolutionState.IsTilesEdit())
            {
                Methods.Editor.Solution.EditLayerA.Select(new Rectangle(0, 0, 32768, 32768), true, false);
                Methods.Internal.UserInterface.UpdateEditLayerActions();
                Dictionary<Point, ushort> copyData = Methods.Editor.Solution.EditLayerA.CopyToClipboard(true);
                Editor.FindReplaceClipboard = copyData;

                List<ushort> listValue = new List<ushort> { };
                List<Point> listPoint = new List<Point> { };
                List<int> listReplaceValues = new List<int> { };
                foreach (var item in Editor.FindReplaceClipboard)
                {
                    listPoint.Add(item.Key);
                }
                foreach (var item in Editor.FindReplaceClipboard)
                {
                    listValue.Add(item.Value);
                }
                for (int i = 0; i < listValue.Count; i++)
                {
                    if ((listValue[i] & 0x3ff) == (ushort)(findValue & 0x3ff))
                        unchecked
                        {
                            if (perserveColllision)
                            {
                                ushort TileIndex = (ushort)(listValue[i] & 0x3ff);
                                int TileIndexInt = (int)(listValue[i] & 0x3ff);
                                bool flipX = ((listValue[i] >> 10) & 1) == 1;
                                bool flipY = ((listValue[i] >> 11) & 1) == 1;
                                bool SolidTopA = ((listValue[i] >> 12) & 1) == 1;
                                bool SolidLrbA = ((listValue[i] >> 13) & 1) == 1;
                                bool SolidTopB = ((listValue[i] >> 14) & 1) == 1;
                                bool SolidLrbB = ((listValue[i] >> 15) & 1) == 1;

                                listValue[i] = (ushort)replaceValue;
                                if (flipX)
                                    listValue[i] |= (1 << 10);
                                else
                                    listValue[i] &= (ushort)~(1 << 10);
                                if (flipY)
                                    listValue[i] |= (1 << 11);
                                else
                                    listValue[i] &= (ushort)~(1 << 11);
                                if (SolidTopA)
                                    listValue[i] |= (1 << 12);
                                else
                                    listValue[i] &= (ushort)~(1 << 12);
                                if (SolidLrbA)
                                    listValue[i] |= (1 << 13);
                                else
                                    listValue[i] &= (ushort)~(1 << 13);
                                if (SolidTopB)
                                    listValue[i] |= (1 << 14);
                                else
                                    listValue[i] &= (ushort)~(1 << 14);
                                if (SolidLrbB)
                                    listValue[i] |= (1 << 15);
                                else
                                    listValue[i] &= (ushort)~(1 << 15);
                            }
                            else
                            {
                                listValue[i] = (ushort)replaceValue;
                            }
                        }
                }
                Editor.FindReplaceClipboard.Clear();
                for (int i = 0; i < listPoint.Count; i++)
                {
                    Editor.FindReplaceClipboard.Add(listPoint[i], listValue[i]);
                }

                // if there's none, use the internal clipboard
                if (Editor.FindReplaceClipboard != null)
                {
                    Methods.Editor.Solution.EditLayerA.PasteFromClipboard(new Point(0, 0), Editor.FindReplaceClipboard);
                    Methods.Internal.UserInterface.UpdateEditLayerActions();
                }
                Methods.Internal.UserInterface.UpdateEditLayerActions();
                Editor.FindReplaceClipboard.Clear();
                Methods.Editor.EditorActions.Deselect();
            }

        }
        public void EditorTileFindTest(int tile, int applyState, bool copyResults)
        {
            if (ManiacEditor.Methods.Editor.SolutionState.IsTilesEdit())
            {
                Methods.Editor.Solution.EditLayerA.Select(new Rectangle(0, 0, 32768, 32768), true, false);
                Methods.Internal.UserInterface.UpdateEditLayerActions();
                Dictionary<Point, ushort> copyData = Methods.Editor.Solution.EditLayerA.CopyToClipboard(true);
                Editor.FindReplaceClipboard = copyData;

                List<ushort> listValue = new List<ushort> { };
                List<Point> listPoint = new List<Point> { };
                List<Point> listLocations = new List<Point> { };

                foreach (var item in Editor.FindReplaceClipboard)
                {
                    listPoint.Add(item.Key);
                }
                foreach (var item in Editor.FindReplaceClipboard)
                {
                    listValue.Add(item.Value);
                }
                for (int i = 0; i < listValue.Count; i++)
                {
                    if ((listValue[i] & 0x3ff) == (ushort)(tile & 0x3ff))
                    {
                        listLocations.Add(listPoint[i]);
                    }
                }
                Editor.FindReplaceClipboard.Clear();
                if (listLocations != null || listLocations.Count != 0)
                {
                    var message = string.Join(Environment.NewLine, listLocations);
                    System.Windows.MessageBox.Show("Tiles found at: " + Environment.NewLine + message, "Results");
                    if (copyResults && message != null)
                    {
                        Clipboard.SetText(message);
                    }
                }
                else
                {
                    System.Windows.MessageBox.Show("Found Nothing", "Results");
                }
                Editor.FindReplaceClipboard.Clear();
                Methods.Editor.EditorActions.Deselect();


            }

        }
        public void EditorTileFind(int tile, int applyState, bool copyResults)
        {
            List<Point> Points = new List<Point>();

            for (int y = 0; y < Methods.Editor.Solution.EditLayerA.Layer.Height; y++)
            {
                for (int x = 0; x < Methods.Editor.Solution.EditLayerA.Layer.Width; x++)
                {
                    ushort TileIndex = (ushort)(Methods.Editor.Solution.EditLayerA.Layer.Tiles[y][x] & 0x3ff); //What is our tile index?
                    if (TileIndex == tile) //do the tiles match?
                    {
                        Points.Add(new Point(x * 16, y * 16)); //Add the found tile to our list of points!
                                                               //Console.WriteLine(x * 16 + " " + y * 16);                       
                    }
                }
            }

            Methods.Editor.Solution.EditLayerA.Deselect();
            foreach (var point in Points)
            {
                Methods.Editor.Solution.EditLayerA.Select(point, true);
            }
            Editor.EditorStatusBar.UpdateStatusPanel();


        }
        public void EditorTileFindReplace(int FindTile, int ReplaceTile, int applyState, bool copyResults)
        {
            List<Point> Points = new List<Point>();

            for (int y = 0; y < Methods.Editor.Solution.EditLayerA.Layer.Height; y++)
            {
                for (int x = 0; x < Methods.Editor.Solution.EditLayerA.Layer.Width; x++)
                {
                    ushort TileIndex = (ushort)(Methods.Editor.Solution.EditLayerA.Layer.Tiles[y][x] & 0x3ff); //What is our tile index?
                    if (TileIndex == FindTile) //do the tiles match?
                    {
                        Points.Add(new Point(x * 16, y * 16)); //Add the found tile to our list of points!

                        ushort Tile = (ushort)ReplaceTile; //Make a new Ushort using the new tileindex

                        //Copy the collision and flip data, but I'm to lazy rn lol

                        //Tile = (ushort)SetBit(10, FlipX, Tile); //Set the flip X value
                        //Tile = (ushort)SetBit(11, FlipY, Tile); //Set the flip Y value
                        //Tile = (ushort)SetBit(12, CollisionAT, Tile); //Set the collision (Top, Path A) value
                        //Tile = (ushort)SetBit(13, CollisionALRB, Tile); //Set the collision (All But Top, Path A) value
                        //Tile = (ushort)SetBit(14, CollisionBT, Tile); //Set the collision (Top, Path B) value
                        //Tile = (ushort)SetBit(15, CollisionBLRB, Tile); //Set the collision (All But Top, Path B) value

                        //TEMPORARY (because I'm lazy)
                        Tile = (ushort)SetBit(10, false, Tile);
                        Tile = (ushort)SetBit(11, false, Tile);
                        Tile = (ushort)SetBit(12, false, Tile);
                        Tile = (ushort)SetBit(13, false, Tile);
                        Tile = (ushort)SetBit(14, false, Tile);
                        Tile = (ushort)SetBit(15, false, Tile);

                        Methods.Editor.Solution.EditLayerA.Layer.Tiles[y][x] = Tile; //Set our new tile Value

                        //Console.WriteLine(x * 16 + " " + y * 16);
                    }
                }
            }
        }
        //Used to set individual Bits in an int
        public static int SetBit(int pos, bool Set, int Value) //Shitty Maybe, but idc, it works
        {

            // "Pos" is what bit we are changing
            // "Set" tells it to be either on or off
            // "Value" is the value you want as your source

            if (Set)
            {
                Value |= 1 << pos;
            }
            if (!Set)
            {
                Value &= ~(1 << pos);
            }
            return Value;
        }

        #endregion
    }
}
