using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Windows.Media;
using RSDKv5;

namespace ManiacEditor.Classes.Scene
{
	public class EditorChunks
	{
		#region Definitions
		private Controls.Editor.MainEditor Instance { get; set; }
		private TexturedStamps StageStamps { get; set; }
		private Stamps EditorStamps { get; set; }

		#endregion

		#region Classes

		public class TexturedStamps : RSDKv5.Stamps
		{
			public class TexturedTileChunk : RSDKv5.Stamps.TileChunk
			{
				#region Init

				public static TexturedTileChunk ConvertFromNormal(RSDKv5.Stamps.TileChunk a)
				{
					TexturedTileChunk b = new TexturedTileChunk();
					b.ChunkSize = a.ChunkSize;
					b.KeyString = a.KeyString;
					b.TileMapA = a.TileMapA;
					b.TileMapB = a.TileMapB;
					return b;
				}

				public TexturedTileChunk(ushort Size = 8) : base(Size)
				{

				}

				public TexturedTileChunk(Dictionary<Point, ushort> points) : base(points)
				{

				}

				public TexturedTileChunk(ushort[][] mapA, ushort[][] mapB, ushort size) : base(mapA, mapB, size)
				{

				}

				public TexturedTileChunk(Dictionary<Point, ushort> pointsA, Dictionary<Point, ushort> pointsB) : base(pointsA, pointsB)
				{

				}

				public TexturedTileChunk(Reader reader) : base(reader)
				{

				}

				public TexturedTileChunk(Reader reader, int OldFormat = 0) : base(reader, OldFormat)
				{

				}
				#endregion

				private ImageSource GetTexture(bool isPathAB = false)
				{
					try
					{
						if (isPathAB && _MultiImage != null && !NeedsRefresh) return _MultiImage;
						else if (!isPathAB && _Image != null && !NeedsRefresh) return _Image;

						Rectangle rect = new Rectangle(0, 0, 8, 8);

						Bitmap bmp = new Bitmap(128, 128, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

						Graphics g = Graphics.FromImage(bmp);
						for (int ty = rect.Y; ty < rect.Y + rect.Height; ty++)
						{
							for (int tx = rect.X; tx < rect.X + rect.Width; ++tx)
							{
								if (this.TileMapA[tx][ty] != 0xffff)
								{
									Methods.Drawing.CommonDrawing.DrawTile(g, this.TileMapA[tx][ty], tx - rect.X, ty - rect.Y, true, false);
								}
								if (this.TileMapB[tx][ty] != 0xffff)
								{
									Methods.Drawing.CommonDrawing.DrawTile(g, this.TileMapB[tx][ty], tx - rect.X, ty - rect.Y, true, isPathAB);
								}
							}
						}
						g.Flush();
						g.Dispose();

						var source = Extensions.ImageExtensions.ImageSourceFromBitmap(bmp);

						if (isPathAB) _MultiImage = source;
						else _Image = source;

						return source;
					}
					catch (Exception ex)
					{
						throw ex;
					}

				}
				public bool NeedsRefresh { get; set; } = false;
				private ImageSource _Image { get; set; }
				private ImageSource _MultiImage { get; set; }
				public ImageSource Image
				{
					get
					{
						return GetTexture(true);
					}
				}
				public ImageSource MultiImage
				{
					get
					{
						return GetTexture();
					}
				}
				public void Dispose()
				{
					if (Image != null)
					{
						//_Image.Dispose();
						_Image = null;
					}
					if (MultiImage != null)
					{
						//_MultiImage.Dispose();
						_MultiImage = null;
					}
				}
			}
			private List<TexturedTileChunk> _TexturedStampList;
			public List<TexturedTileChunk> TexturedStampList
			{
				get
				{
					return _TexturedStampList;
				}
				set
				{
					_TexturedStampList = value;
					ConvertToNormalStamps(ref _TexturedStampList, ref StampList);
				}
			}
			public void DisposeTextures()
			{
				foreach (var stamp in this.TexturedStampList)
				{
					stamp.Dispose();
				}
				this.TexturedStampList.Clear();
			}
			public TexturedStamps() : base()
			{
				ConvertFromNormalStamps(ref _TexturedStampList, ref StampList);
			}
			public TexturedStamps(RSDKv5.Reader reader) : base(reader)
			{
				ConvertFromNormalStamps(ref _TexturedStampList, ref StampList);
			}
			public TexturedStamps(string filepath) : base(filepath)
			{
				ConvertFromNormalStamps(ref _TexturedStampList, ref StampList);
			}

			public static void ConvertFromNormalStamps(ref List<TexturedTileChunk> Textured, ref List<TileChunk> Normal)
			{
				Textured = Normal.ConvertAll<TexturedTileChunk>(x => ConvertToTexturedStamp(x));
			}
			public static void ConvertToNormalStamps(ref List<TexturedTileChunk> Textured, ref List<TileChunk> Normal)
			{
				Normal = Textured.ConvertAll<TileChunk>(x => ConvertToNormalStamp(x));
			}
			public List<TileChunk> GetNormalStamps()
			{
				return TexturedStampList.ConvertAll<TileChunk>(x => ConvertToNormalStamp(x));
			}
			public static RSDKv5.Stamps.TileChunk ConvertToNormalStamp(TexturedTileChunk chunk)
			{
				return chunk;
			}
			public static TexturedTileChunk ConvertToTexturedStamp(RSDKv5.Stamps.TileChunk chunk)
			{
				return TexturedTileChunk.ConvertFromNormal(chunk);
			}
		}

		#endregion

		#region Init
		public EditorChunks(Controls.Editor.MainEditor _Instance, TexturedStamps _StageStamps)
		{
			Instance = _Instance;
			StageStamps = _StageStamps;
			EditorStamps = new Stamps();
			AddBlankMap();
		}
		#endregion

		#region Load/Save

		public void Save(bool SaveAsMode = false, string SaveAsFilePath = "")
		{
			try
			{
				if (StageStamps != null)
				{
					StageStamps.StampList = StageStamps.GetNormalStamps();
					if (StageStamps?.loadstate == RSDKv5.Stamps.LoadState.Upgrade)
					{
						string message = "This Editor Chunk File needs to be updated to a newer version of the format. This will happen almost instantly, however you will be unable to use your chunks in a previous version of maniac on this is done. Would you like to continue?" + Environment.NewLine + "(Click Yes to Save, Click No to Continue without Saving Your Chunks)";
						string tile = "Chunk File Format Upgrade Required";
						System.Windows.MessageBoxResult result = System.Windows.MessageBox.Show(message, tile, System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Warning);
						if (result != System.Windows.MessageBoxResult.Yes) return;
					}
					if (SaveAsMode)
					{
						StageStamps?.Write(SaveAsFilePath);
						Methods.Solution.SolutionPaths.Stamps_Source = new Methods.Solution.SolutionPaths.FileSource(-3, SaveAsFilePath);
					}
					else
					{
						if (Methods.Solution.SolutionState.Main.DataDirectoryReadOnlyMode && Methods.Solution.SolutionPaths.Stamps_Source.SourceID == -1) return;
						else StageStamps?.Write(ManiacEditor.Methods.Solution.SolutionPaths.Stamps_Source.SourcePath);
					}
				}
			}
			catch (Exception ex)
			{
				Methods.Internal.Common.ShowError($@"Failed to save StageStamps to file '{ManiacEditor.Methods.Solution.SolutionPaths.Stamps_Source.SourcePath}' Error: {ex.Message}");
			}
		}

		#endregion

		#region Chunk Management
		public List<ImageSource> GetChunkCurrentImages()
		{
			try
			{
				List<ImageSource> ImageCollection = new List<ImageSource>();
				bool isMultiLayer = Methods.Solution.CurrentSolution.EditLayerB != null;
				foreach (var chunk in StageStamps.TexturedStampList)
				{
					var Image = (isMultiLayer ? chunk.MultiImage : chunk.Image);
					ImageCollection.Add(Image);
				}
				return ImageCollection;
			}
			catch (Exception ex)
			{
				throw ex;
			}

		}
		public Stamps.TileChunk GetStamp(int ChunkIndex)
		{
			return StageStamps.StampList[ChunkIndex];
		}
		public void AddChunk(TexturedStamps.TexturedTileChunk Stamp)
		{
			StageStamps.TexturedStampList.Add(Stamp);
		}
		public void RemoveChunk(int ChunkIndex)
		{
			StageStamps.TexturedStampList[ChunkIndex].Dispose();
			StageStamps.TexturedStampList.RemoveAt(ChunkIndex);
		}
		public void DuplicateChunk(int ChunkIndex)
		{
			StageStamps.TexturedStampList.Add(StageStamps.TexturedStampList[ChunkIndex]);
		}
		public void AutoGenerateChunks(Classes.Scene.EditorLayer LayerA)
		{
			Methods.Internal.UserInterface.ShowWaitingBox();

			System.Threading.Thread thread = new System.Threading.Thread(() =>
			{
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
						var newChunk = new TexturedStamps.TexturedTileChunk(ChunkMapA, ChunkMapB, 8);
						var duplicate = StageStamps.StampList.Exists(x => x.TileMapCodeA == newChunk.TileMapCodeA && x.TileMapCodeB == newChunk.TileMapCodeB);
						if (duplicate == false) AddChunk(newChunk);
					}
				}

				Methods.Internal.UserInterface.CloseWaitingBox();
			})
			{ IsBackground = true };
			thread.Start();



		}
		public void AutoGenerateChunks(Classes.Scene.EditorLayer LayerA, Classes.Scene.EditorLayer LayerB)
		{
			Methods.Internal.UserInterface.ShowWaitingBox();

			System.Threading.Thread thread = new System.Threading.Thread(() =>
			{
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
						var newChunk = new TexturedStamps.TexturedTileChunk(ChunkMapA, ChunkMapB, 8);
						var duplicate = StageStamps.StampList.Exists(x => x.TileMapCodeA == newChunk.TileMapCodeA && x.TileMapCodeB == newChunk.TileMapCodeB);
						if (duplicate == false) AddChunk(newChunk);
					}
				}

				Methods.Internal.UserInterface.CloseWaitingBox();
			})
			{ IsBackground = true };
			thread.Start();

		}

		#endregion

		#region Chunk Validation
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
		#endregion

		#region Clipboard
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


			StageStamps.StampList.Add(new TexturedStamps.TexturedTileChunk(convertedPoints));
		}
		public void ConvertClipboardtoMultiLayerChunk(Methods.Solution.SolutionClipboard.MultiTilesClipboardEntry data)
		{
			var points = data.GetData();
			var pointsA = points[0];
			var pointsB = points[1];


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


			StageStamps.StampList.Add(new TexturedStamps.TexturedTileChunk(convertedPointsAFinal, convertedPointsBFinal));
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

			EditLayerA?.PasteClipboardData(TileCoord, new Methods.Solution.SolutionClipboard.TilesClipboardEntry(ConvertedChunkA));
			EditLayerB?.PasteClipboardData(TileCoord, new Methods.Solution.SolutionClipboard.TilesClipboardEntry(ConvertedChunkB));
			Actions.UndoRedoModel.UpdateEditLayersActions();
			EditLayerA?.DeselectAll();
			EditLayerB?.DeselectAll();
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
		#endregion

		#region Test Methods
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
					ChunkData[x, y] = 0xffff;
				}
			}

			return ChunkData;
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
			StageStamps.StampList.Add(new TexturedStamps.TexturedTileChunk(TestChunkDic));
			TestChunkDic.Clear();

			for (int cx = 0; cx < 8; cx++)
			{
				for (int cy = 0; cy < 8; cy++)
				{
					TestChunkDic.Add(new Point(cx, cy), 0xffff);
				}
			}
			StageStamps.StampList.Add(new TexturedStamps.TexturedTileChunk(TestChunkDic));
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
        #endregion

        #region Dispose

		public void Dispose()
		{
			StageStamps.DisposeTextures();
		}

        #endregion





    }
}
