using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using RSDKv5;

namespace ManiacEditor.Classes.Scene
{
    public class EditorChunks
	{

		public Controls.Editor.MainEditor EditorInstance;

		private Classes.Scene.EditorTiles Tiles;

		public Stamps StageStamps;

		private Stamps EditorStamps;

		public IList<Bitmap> ChunkImagesA = new List<Bitmap>();
		public IList<Bitmap> ChunkImagesAB = new List<Bitmap>();
		public EditorChunks(Controls.Editor.MainEditor instance, Classes.Scene.EditorTiles stageTiles, Stamps stageStamps)
		{
			EditorInstance = instance;
			StageStamps = stageStamps;
			EditorStamps = new Stamps();
			AddBlankMap();
			Tiles = Methods.Editor.Solution.CurrentTiles;
		}
		public void DrawTile(Graphics g, ushort tile, int x, int y, bool semitransparent = false)
		{
			ushort TileIndex = (ushort)(tile & 0x3ff);
			int TileIndexInt = (int)TileIndex;
			bool flipX = ((tile >> 10) & 1) == 1;
			bool flipY = ((tile >> 11) & 1) == 1;
			bool SolidTopA = ((tile >> 12) & 1) == 1;
			bool SolidLrbA = ((tile >> 13) & 1) == 1;
			bool SolidTopB = ((tile >> 14) & 1) == 1;
			bool SolidLrbB = ((tile >> 15) & 1) == 1;

            if (semitransparent)
            {
                g.DrawImage(Methods.Editor.Solution.CurrentTiles.ImageTransparent.GetBitmap(new Rectangle(0, TileIndex * Methods.Editor.EditorConstants.TILE_SIZE, Methods.Editor.EditorConstants.TILE_SIZE, Methods.Editor.EditorConstants.TILE_SIZE), flipX, flipY), new Rectangle(x * Methods.Editor.EditorConstants.TILE_SIZE, y * Methods.Editor.EditorConstants.TILE_SIZE, Methods.Editor.EditorConstants.TILE_SIZE, Methods.Editor.EditorConstants.TILE_SIZE));

            }
            else
            {
                g.DrawImage(Methods.Editor.Solution.CurrentTiles.Image.GetBitmap(new Rectangle(0, TileIndex * Methods.Editor.EditorConstants.TILE_SIZE, Methods.Editor.EditorConstants.TILE_SIZE, Methods.Editor.EditorConstants.TILE_SIZE), flipX, flipY), new Rectangle(x * Methods.Editor.EditorConstants.TILE_SIZE, y * Methods.Editor.EditorConstants.TILE_SIZE, Methods.Editor.EditorConstants.TILE_SIZE, Methods.Editor.EditorConstants.TILE_SIZE));

            }

        }
        public Bitmap GetChunkTexture(int chunkIndex)
		{
			if (Methods.Editor.Solution.EditLayerB != null)
			{
				return GetChunkTextureAB(chunkIndex);
			}
			else
			{
                return GetChunkTextureA(chunkIndex);
            }
		}

		public Bitmap GetChunkTextureA(int chunkIndex)
		{
			if (this.ChunkImagesA.ElementAtOrDefault(chunkIndex) != null) return this.ChunkImagesA[chunkIndex];

			Rectangle rect = new Rectangle(0, 0, 8, 8);

			Bitmap bmp = new Bitmap(128, 128, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

			Graphics g = Graphics.FromImage(bmp);
			for (int ty = rect.Y; ty < rect.Y + rect.Height; ty++)
			{
                for (int tx = rect.X; tx < rect.X + rect.Width; ++tx)
                {
                    if (this.StageStamps.StampList[chunkIndex].TileMapA[tx][ty] != 0xffff)
                    {
                        DrawTile(g, this.StageStamps.StampList[chunkIndex].TileMapA[tx][ty], tx - rect.X, ty - rect.Y);
                    }
                    if (this.StageStamps.StampList[chunkIndex].TileMapB[tx][ty] != 0xffff)
                    {
                        DrawTile(g, this.StageStamps.StampList[chunkIndex].TileMapB[tx][ty], tx - rect.X, ty - rect.Y, true);
                    }
                }
            }
			g.Flush();
			g.Dispose();
			this.ChunkImagesA.Insert(chunkIndex, bmp);
			return this.ChunkImagesA[chunkIndex];
		}

        public Bitmap GetChunkTextureAB(int chunkIndex)
        {
            if (this.ChunkImagesAB.ElementAtOrDefault(chunkIndex) != null) return this.ChunkImagesAB[chunkIndex];

            Rectangle rect = new Rectangle(0, 0, 8, 8);

            Bitmap bmp = new Bitmap(128, 128, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            Graphics g = Graphics.FromImage(bmp);
            for (int ty = rect.Y; ty < rect.Y + rect.Height; ty++)
            {
                for (int tx = rect.X; tx < rect.X + rect.Width; ++tx)
                {
                    if (this.StageStamps.StampList[chunkIndex].TileMapA[tx][ty] != 0xffff)
                    {
                        DrawTile(g, this.StageStamps.StampList[chunkIndex].TileMapA[tx][ty], tx - rect.X, ty - rect.Y);
                    }
                    if (this.StageStamps.StampList[chunkIndex].TileMapB[tx][ty] != 0xffff)
                    {
                        DrawTile(g, this.StageStamps.StampList[chunkIndex].TileMapB[tx][ty], tx - rect.X, ty - rect.Y);
                    }
                }
            }
            g.Flush();
            g.Dispose();
            this.ChunkImagesAB.Insert(chunkIndex, bmp);
            return this.ChunkImagesAB[chunkIndex];
        }

        public void ConvertClipboardtoChunk(Dictionary<Point, ushort> points)
		{
			int minimumX = points.Min(kvp => kvp.Key.X);
			int minimumY = points.Min(kvp => kvp.Key.Y);

			var keys = points.Select(kvp => kvp.Key).ToList();
			var values = points.Select(kvp => kvp.Value).ToList();

			for (int i = 0; i < keys.Count; i++)
			{
				int x = keys[i].X - minimumX;
				int y = keys[i].Y - minimumY;
				keys[i] = new Point(x, y);
			}

			var convertedPoints = keys.Zip(values, (k, v) => new { k, v }).ToDictionary(x => x.k, x => x.v);
			for (int y = 0; y < 8; y++)
			{
				for (int x = 0; x < 8; x++)
				{
					Point p = new Point(x, y);
					if (!convertedPoints.Keys.Contains(p)) convertedPoints.Add(p, 0xffff);
				}
			}
			convertedPoints = convertedPoints.OrderBy(x => x.Key.X).ThenBy(x => x.Key.Y).ToDictionary(x => x.Key, x => x.Value);


			StageStamps.StampList.Add(new Stamps.TileChunk(convertedPoints));
		}

        public void AutoGenerateChunks(Classes.Scene.EditorLayer LayerA)
        {
            Methods.Internal.UserInterface.UpdateWaitingScreen(true);
            Methods.Internal.UserInterface.ToggleEditorButtons(false);

            System.Threading.Thread thread = new System.Threading.Thread(() => {
                int width = LayerA.Width;
                int height = LayerA.Height;
                int ChunkWidth = width / 8;
                int ChunkHeight = height / 8;

                for (int i = 0; i < ChunkHeight; ++i)
                {
                    for (int j = 0; j < ChunkWidth; ++j)
                    {
                        int tileX = j * 8;
                        int tileY = i * 8;

                        int x1 = j * 8;
                        int x2 = x1 + 8;
                        int y1 = i * 8;
                        int y2 = y1 + 8;
                        ushort[][] ChunkMapA = new ushort[8][];
                        ushort[][] ChunkMapB = new ushort[8][];
                        for (int x = 0; x < 8; x++)
                        {
                            ChunkMapA[x] = new ushort[8];
                            ChunkMapB[x] = new ushort[8];
                            for (int y = 0; y < 8; y++)
                            {
                                ChunkMapA[x][y] = LayerA.GetTileAt(tileX + x, tileY + y);
                                ChunkMapB[x][y] = 0xffff;
                            }
                        }
                        var newChunk = new RSDKv5.Stamps.TileChunk(ChunkMapA, ChunkMapB, 8);
                        var duplicate = StageStamps.StampList.Exists(x => x.TileMapCodeA == newChunk.TileMapCodeA && x.TileMapCodeB == newChunk.TileMapCodeB);
                        if (duplicate == false) StageStamps.StampList.Add(newChunk);
                    }
                }
                EditorInstance.Dispatcher.Invoke(new Action(() => Methods.Internal.UserInterface.UpdateWaitingScreen(false)));
                EditorInstance.Dispatcher.Invoke(new Action(() => Methods.Internal.UserInterface.ToggleEditorButtons(true)));

            })
            { IsBackground = true };
            thread.Start();
        }

        public void AutoGenerateChunks(Classes.Scene.EditorLayer LayerA, Classes.Scene.EditorLayer LayerB)
        {
            Methods.Internal.UserInterface.UpdateWaitingScreen(true);
            Methods.Internal.UserInterface.ToggleEditorButtons(false);

            System.Threading.Thread thread = new System.Threading.Thread(() => {
                int width = (LayerA.Width > LayerB.Width ? LayerA.Width : LayerB.Width);
                int height = (LayerA.Height > LayerB.Height ? LayerA.Height : LayerB.Height);
                int ChunkWidth = width / 8;
                int ChunkHeight = height / 8;

                for (int i = 0; i < ChunkHeight; ++i)
                {
                    for (int j = 0; j < ChunkWidth; ++j)
                    {
                        int tileX = j * 8;
                        int tileY = i * 8;

                        int x1 = j * 8;
                        int x2 = x1 + 8;
                        int y1 = i * 8;
                        int y2 = y1 + 8;
                        ushort[][] ChunkMapA = new ushort[8][];
                        ushort[][] ChunkMapB = new ushort[8][];
                        for (int x = 0; x < 8; x++)
                        {
                            ChunkMapA[x] = new ushort[8];
                            ChunkMapB[x] = new ushort[8];
                            for (int y = 0; y < 8; y++)
                            {
                                ChunkMapA[x][y] = LayerA.GetTileAt(tileX + x, tileY + y);
                                ChunkMapB[x][y] = LayerB.GetTileAt(tileX + x, tileY + y);
                            }
                        }
                        var newChunk = new RSDKv5.Stamps.TileChunk(ChunkMapA, ChunkMapB, 8);
                        var duplicate = StageStamps.StampList.Exists(x => x.TileMapCodeA == newChunk.TileMapCodeA && x.TileMapCodeB == newChunk.TileMapCodeB);
                        if (duplicate == false) StageStamps.StampList.Add(newChunk);
                    }
                }
                EditorInstance.Dispatcher.Invoke(new Action(() => Methods.Internal.UserInterface.UpdateWaitingScreen(false)));
                EditorInstance.Dispatcher.Invoke(new Action(() => Methods.Internal.UserInterface.ToggleEditorButtons(true)));

            })
            { IsBackground = true };
            thread.Start();
        }

        public void ConvertClipboardtoMultiLayerChunk(Dictionary<Point, ushort> pointsA, Dictionary<Point, ushort> pointsB = null)
		{
			if (pointsB == null || pointsB.Count == 0)
			{
				ConvertClipboardtoChunk(pointsA);
				return;
			}
            else if (pointsA == null || pointsA.Count == 0)
            {
                ConvertClipboardtoChunk(pointsB);
                return;
            }

            int minimumX_A = pointsA.Min(kvp => kvp.Key.X);
			int minimumY_A = pointsA.Min(kvp => kvp.Key.Y);
			int minimumX_B = pointsB.Min(kvp => kvp.Key.X);
			int minimumY_B = pointsB.Min(kvp => kvp.Key.Y);

			int minimumX = Math.Min(minimumX_A, minimumX_B);
			int minimumY = Math.Min(minimumY_A, minimumY_B);

			var keys_A = pointsA.Select(kvp => kvp.Key).ToList();
			var values_A = pointsA.Select(kvp => kvp.Value).ToList();

			var keys_B = pointsB.Select(kvp => kvp.Key).ToList();
			var values_B = pointsB.Select(kvp => kvp.Value).ToList();

			for (int i = 0; i < keys_A.Count; i++)
			{
				int x = keys_A[i].X - minimumX;
				int y = keys_A[i].Y - minimumY;
				keys_A[i] = new Point(x, y);
			}

			for (int i = 0; i < keys_B.Count; i++)
			{
				int x = keys_B[i].X - minimumX;
				int y = keys_B[i].Y - minimumY;
				keys_B[i] = new Point(x, y);
			}

			var convertedPointsA = keys_A.Zip(values_A, (k, v) => new { k, v }).ToDictionary(x => x.k, x => x.v);
			var convertedPointsB = keys_B.Zip(values_B, (k, v) => new { k, v }).ToDictionary(x => x.k, x => x.v);
			for (int y = 0; y < 8; y++)
			{
				for (int x = 0; x < 8; x++)
				{
					Point p = new Point(x, y);
					if (!convertedPointsA.Keys.Contains(p)) convertedPointsA.Add(p, 0xffff);
					if (!convertedPointsB.Keys.Contains(p)) convertedPointsB.Add(p, 0xffff);
				}
			}

			var convertedPointsAFinal = convertedPointsA.OrderBy(x => x.Key.X).ThenBy(x => x.Key.Y).ToDictionary(x => x.Key, x => x.Value);
			var convertedPointsBFinal = convertedPointsB.OrderBy(x => x.Key.X).ThenBy(x => x.Key.Y).ToDictionary(x => x.Key, x => x.Value);


			StageStamps.StampList.Add(new Stamps.TileChunk(convertedPointsAFinal, convertedPointsBFinal));
		}

		public void PasteStamp(Point ChunkCoord, int index, Classes.Scene.EditorLayer EditLayerA, Classes.Scene.EditorLayer EditLayerB, bool deleteMode = false)
		{
			Point TileCoord = new Point(ChunkCoord.X * 128, ChunkCoord.Y * 128);
			Dictionary<Point, ushort> ConvertedChunkA = new Dictionary<Point, ushort>();
			Dictionary<Point, ushort> ConvertedChunkB = new Dictionary<Point, ushort>();
			if (deleteMode)
			{
				ConvertedChunkA = ConvertChunkSideAtoClipboard(EditorStamps.StampList[0]);
				ConvertedChunkB = ConvertChunkSideBtoClipboard(EditorStamps.StampList[0]);
			}
			else
			{
				ConvertedChunkA = ConvertChunkSideAtoClipboard(StageStamps.StampList[index]);
				ConvertedChunkB = ConvertChunkSideBtoClipboard(StageStamps.StampList[index]);
			}

			EditLayerA?.PasteFromClipboard(TileCoord, ConvertedChunkA);
			EditLayerB?.PasteFromClipboard(TileCoord, ConvertedChunkB);
			Methods.Internal.UserInterface.UpdateEditLayerActions();
			EditLayerA?.Deselect();
			EditLayerB?.Deselect();
		}

        public bool DoesChunkMatch(Point point, Stamps.TileChunk CompareChunk, Classes.Scene.EditorLayer EditLayerA, Classes.Scene.EditorLayer EditLayerB, int chunkSize = 8)
        {
            Point TileCoord = new Point(point.X * 128, point.Y * 128);
            for (int x = 0; x < chunkSize; x++)
            {
                for (int y = 0; y < chunkSize; y++)
                {
                    Point p = new Point((TileCoord.X / 16) + x, (TileCoord.Y / 16) + y);
                    if (CompareChunk.TileMapA[x][y] == EditLayerA.GetTileAt(p.X, p.Y))
                    {
                        if (EditLayerB != null)
                        {
                            if (CompareChunk.TileMapB[x][y] == EditLayerB.GetTileAt(p.X, p.Y)) continue;
                            else return false;
                        }
                        else continue;
                    }
                    else return false;

                }
            }
            return true;
        }

        public bool DoesChunkMatch(Point point, Stamps.TileChunk CompareChunk, Classes.Scene.EditorLayer EditLayer, int chunkSize = 8)
		{
			Point TileCoord = new Point(point.X * 128, point.Y * 128);
			for (int x = 0; x < chunkSize; x++)
			{
				for (int y = 0; y < chunkSize; y++)
				{
					Point p = new Point((TileCoord.X / 16) + x, (TileCoord.Y / 16) + y);
					if (CompareChunk.TileMapA[x][y] == EditLayer.GetTileAt(p.X, p.Y)) continue;
					else
					{
						return false;
					}

				}
			}
			return true;
		}

        public bool DoesChunkMatch(Stamps.TileChunk CompareChunk, Stamps.TileChunk CompareChunk2, int chunkSize = 8)
        {
            for (int x = 0; x < chunkSize; x++)
            {
                for (int y = 0; y < chunkSize; y++)
                {
                    if (CompareChunk.TileMapA[x][y] == CompareChunk.TileMapA[x][y])
                    {
                        if (CompareChunk.TileMapB[x][y] == CompareChunk.TileMapB[x][y]) continue;
                        else return false;
                    }
                    else return false;


                }
            }
            return true;
        }

        public bool IsChunkEmpty(Point point, Classes.Scene.EditorLayer EditLayer, int chunkSize = 8)
		{
			if (EditLayer == null) return true;
			Point TileCoord = new Point(point.X * 128, point.Y * 128);
			for (int x = 0; x < chunkSize; x++)
			{
				for (int y = 0; y < chunkSize; y++)
				{
					Point p = new Point((TileCoord.X / 16) + x, (TileCoord.Y / 16) + y);
					if (EditLayer.GetTileAt(p.X, p.Y) == 0xffff) continue;
					else return false;

				}
			}
			return true;
		}
        public bool IsChunkEmpty(Point point, Classes.Scene.EditorLayer EditLayerA, Classes.Scene.EditorLayer EditLayerB, int chunkSize = 8)
        {
            bool isEmptyA = IsChunkEmpty(point, EditLayerA, chunkSize);
            bool isEmptyB = IsChunkEmpty(point, EditLayerB, chunkSize);

            if (isEmptyA == false || isEmptyB == false) return false;
            else return true;
        }


        public Dictionary<Point, ushort> ConvertChunkSideAtoClipboard(Stamps.TileChunk Chunk)
		{
			ushort[][] ChunkData = Chunk.TileMapA;
			Dictionary<Point, ushort> ClipboardData = new Dictionary<Point, ushort>();

			for (int x = 0; x < Chunk.ChunkSize; x++)
			{
				for (int y = 0; y < Chunk.ChunkSize; y++)
				{
					ClipboardData.Add(new Point(x, y), ChunkData[x][y]);
				}
			}

			return ClipboardData;
		}

		public Dictionary<Point, ushort> ConvertChunkSideBtoClipboard(Stamps.TileChunk Chunk)
		{
			ushort[][] ChunkData = Chunk.TileMapB;
			Dictionary<Point, ushort> ClipboardData = new Dictionary<Point, ushort>();

			for (int x = 0; x < Chunk.ChunkSize; x++)
			{
				for (int y = 0; y < Chunk.ChunkSize; y++)
				{
					ClipboardData.Add(new Point(x, y), ChunkData[x][y]);
				}
			}

			return ClipboardData;
		}

		public Dictionary<Point, ushort> MakeaBlankChunk_Clip(int chunkSize = 8)
		{
			Dictionary<Point, ushort> ClipboardData = new Dictionary<Point, ushort>();

			for (int x = 0; x < chunkSize; x++)
			{
				for (int y = 0; y < chunkSize; y++)
				{
					ClipboardData.Add(new Point(x, y), (ushort)0xffff);
				}
			}

			return ClipboardData;
		}

		public ushort[,] MakeaBlankChunk(int chunkSize = 8)
		{
			ushort[,] ChunkData = new ushort[chunkSize, chunkSize];

			for (int x = 0; x < chunkSize; x++)
			{
				for (int y = 0; y < chunkSize; y++)
				{
					ChunkData[x,y] = 0xffff;
				}
			}

			return ChunkData;
		}

		public void DisposeTextures()
		{
			foreach (var image in ChunkImagesA)
			{
				image.Dispose();
			}
			foreach (var image in ChunkImagesAB)
			{
				image.Dispose();
			}
			ChunkImagesA.Clear();
			ChunkImagesAB.Clear();
		}

		public void AddTestMaps()
		{
			Dictionary<Point, ushort> TestChunkDic = new Dictionary<Point, ushort>();
			for (int cx = 0; cx < 8; cx++)
			{
				for (int cy = 0; cy < 8; cy++)
				{
					if (cy == 0) TestChunkDic.Add(new Point(cx, cy), 0xffff);
					else if (cy == 1) TestChunkDic.Add(new Point(cx, cy), 0x0003);
					else if (cy == 2) TestChunkDic.Add(new Point(cx, cy), 0x0013);
					else if (cy == 3) TestChunkDic.Add(new Point(cx, cy), 0x0023);
					else TestChunkDic.Add(new Point(cx, cy), 0x0001);
				}
			}
			StageStamps.StampList.Add(new Stamps.TileChunk(TestChunkDic));
			TestChunkDic.Clear();

			for (int cx = 0; cx < 8; cx++)
			{
				for (int cy = 0; cy < 8; cy++)
				{
					TestChunkDic.Add(new Point(cx, cy), 0xffff);
				}
			}
			StageStamps.StampList.Add(new Stamps.TileChunk(TestChunkDic));
			TestChunkDic.Clear();
		}

		public void AddBlankMap()
		{
			Dictionary<Point, ushort> TestChunkDic = new Dictionary<Point, ushort>();
			for (int cx = 0; cx < 8; cx++)
			{
				for (int cy = 0; cy < 8; cy++)
				{
					TestChunkDic.Add(new Point(cx, cy), 0xffff);
				}
			}
			EditorStamps.StampList.Add(new Stamps.TileChunk(TestChunkDic));
			TestChunkDic.Clear();
		}

	}
}
