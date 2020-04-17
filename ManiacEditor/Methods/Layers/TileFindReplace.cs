using System;
using System.Collections.Generic;
using Point = System.Drawing.Point;
using System.Drawing;
using System.Windows.Forms;

namespace ManiacEditor.Methods.Layers
{
    public static class TileFindReplace
    {
        private static ManiacEditor.Controls.Editor.MainEditor Instance;
        public static void UpdateInstance(ManiacEditor.Controls.Editor.MainEditor instance)
        {
            Instance = instance;
        }

        #region Find Unused Tiles
        public static void FindUnusedTiles()
        {
            Methods.Internal.UserInterface.LockUserInterface = true;
            Methods.Internal.UserInterface.ShowWaitScreen = true;
            Methods.Internal.UserInterface.UpdateControls();
            List<int> UnusedTiles = new List<int> { };


            System.Threading.Thread thread = new System.Threading.Thread(() =>
            {
                for (int i = 0; i < 1024; i++)
                {
                    if (Instance.TilesToolbar != null) Instance.TilesToolbar.SelectedTileLabel.Text = "Selected Tile: " + i;
                    bool Unusued = IsTileUnused(i);
                    if (Unusued) UnusedTiles.Add(i);
                }
                Instance.Dispatcher.Invoke(new Action(() => ShowUnusedTiles(UnusedTiles)));
            })
            { IsBackground = true };
            thread.Start();

        }
        public static bool IsTileUnused(int tile)
        {
            IEnumerable<Classes.Scene.EditorLayer> AllLayers = Methods.Solution.CurrentSolution.CurrentScene.AllLayers;
            bool unused = true;

            foreach (var editorLayer in Methods.Solution.CurrentSolution.CurrentScene.AllLayers)
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
        public static void ShowUnusedTiles(List<int> UnusedTiles)
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
            Methods.Internal.UserInterface.LockUserInterface = false;
            Methods.Internal.UserInterface.ShowWaitScreen = false;
            Methods.Internal.UserInterface.UpdateControls();
        }
        #endregion

        #region Offset Tile Indexes

        public static void OffsetEditLayerIndexes(int Amount)
        {
            if (Methods.Solution.CurrentSolution.EditLayerA != null)
            {
               for (int x = 0; x < Methods.Solution.CurrentSolution.EditLayerA.Tiles.Length; x++)
               {
                    for (int y = 0; y < Methods.Solution.CurrentSolution.EditLayerA.Tiles[x].Length; y++)
                    {
                        RSDKv5.Tile currentTile = new RSDKv5.Tile(Methods.Solution.CurrentSolution.EditLayerA.Tiles[x][y]);
                        int currentIndex = currentTile.Index;
                        if (currentIndex + Amount > 1024)
                        {
                            currentTile.Index = (ushort)(1024);
                        }
                        else if (currentIndex + Amount < 0)
                        {
                            currentTile.Index = (ushort)(0);
                        }
                        else
                        {
                            currentTile.Index = (ushort)(currentTile.Index + Amount);
                        }


                        Methods.Solution.CurrentSolution.EditLayerA.Tiles[x][y] = currentTile.RawData;
                    }
                }
            }
        }

        #endregion

        #region Unsorted
        public static void EditorTileFind(int tile, int applyState, bool copyResults)
        {
            List<Point> Points = new List<Point>();

            for (int y = 0; y < Methods.Solution.CurrentSolution.EditLayerA.Layer.Height; y++)
            {
                for (int x = 0; x < Methods.Solution.CurrentSolution.EditLayerA.Layer.Width; x++)
                {
                    ushort TileIndex = (ushort)(Methods.Solution.CurrentSolution.EditLayerA.Layer.Tiles[y][x] & 0x3ff); //What is our tile index?
                    if (TileIndex == tile) //do the tiles match?
                    {
                        Points.Add(new Point(x * 16, y * 16)); //Add the found tile to our list of points!
                                                               //Console.WriteLine(x * 16 + " " + y * 16);                       
                    }
                }
            }

            Methods.Solution.CurrentSolution.EditLayerA.DeselectAll();
            foreach (var point in Points)
            {
                Methods.Solution.CurrentSolution.EditLayerA.Select(point, true);
            }
            Instance.EditorStatusBar.UpdateStatusPanel();


        }
        public static void EditorTileFindReplace(int FindTile, int ReplaceTile, int applyState, bool copyResults)
        {
            List<Point> Points = new List<Point>();

            for (int y = 0; y < Methods.Solution.CurrentSolution.EditLayerA.Layer.Height; y++)
            {
                for (int x = 0; x < Methods.Solution.CurrentSolution.EditLayerA.Layer.Width; x++)
                {
                    ushort TileIndex = (ushort)(Methods.Solution.CurrentSolution.EditLayerA.Layer.Tiles[y][x] & 0x3ff); //What is our tile index?
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

                        Methods.Solution.CurrentSolution.EditLayerA.Layer.Tiles[y][x] = Tile; //Set our new tile Value

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
