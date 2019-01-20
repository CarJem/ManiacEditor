using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using RSDKv5;
using ManiacEditor.Actions;
using ManiacEditor.Enums;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using IronPython.Modules;
using SharpDX.Direct3D9;
using Microsoft.Scripting.Utils;

namespace ManiacEditor
{
	public class EditorChunk
	{

		const int x128_CHUNK_SIZE = 128;

		public const int TILE_SIZE = 16;

		public Editor EditorInstance;

		private StageTiles Tiles;

		public Stamps StageStamps;

		private Stamps EditorStamps;

		public IList<Bitmap> ChunkImages = new List<Bitmap>();

		public EditorChunk(Editor instance, StageTiles stageTiles, Stamps stageStamps)
		{
			EditorInstance = instance;
			StageStamps = stageStamps;
			EditorStamps = new Stamps();
			AddBlankMap();
			Tiles = instance.StageTiles;
		}


		public void DrawTile(Graphics g, ushort tile, int x, int y)
		{
			ushort TileIndex = (ushort)(tile & 0x3ff);
			int TileIndexInt = (int)TileIndex;
			bool flipX = ((tile >> 10) & 1) == 1;
			bool flipY = ((tile >> 11) & 1) == 1;
			bool SolidTopA = ((tile >> 12) & 1) == 1;
			bool SolidLrbA = ((tile >> 13) & 1) == 1;
			bool SolidTopB = ((tile >> 14) & 1) == 1;
			bool SolidLrbB = ((tile >> 15) & 1) == 1;

			g.DrawImage(EditorInstance.StageTiles.Image.GetBitmap(new Rectangle(0, TileIndex * TILE_SIZE, TILE_SIZE, TILE_SIZE), flipX, flipY),
				new Rectangle(x * TILE_SIZE, y * TILE_SIZE, TILE_SIZE, TILE_SIZE));

		}


		public Bitmap GetChunkTexture(int chunkIndex)
		{
			if (this.ChunkImages.ElementAtOrDefault(chunkIndex) != null) return this.ChunkImages[chunkIndex];

			Rectangle rect = new Rectangle(0, 0, 8, 8);

			Bitmap bmp = new Bitmap(128, 128, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

			Graphics g = Graphics.FromImage(bmp);
			for (int ty = rect.Y; ty < rect.Y + rect.Height; ty++)
			{
				for (int tx = rect.X; tx < rect.X + rect.Width; ++tx)
				{
					if (this.StageStamps.StampList[chunkIndex].TileMap[tx, ty] != 0xffff)
					{
						DrawTile(g, this.StageStamps.StampList[chunkIndex].TileMap[tx, ty], tx - rect.X, ty - rect.Y);
					}
				}
			}
			g.Flush();
			g.Dispose();
			this.ChunkImages.Insert(chunkIndex, bmp);
			return this.ChunkImages[chunkIndex];
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

		public void PasteStamp(Point ChunkCoord, int index, EditorLayer EditLayer, bool deleteMode = false)
		{
			Point TileCoord = new Point(ChunkCoord.X * 128, ChunkCoord.Y * 128);
			Dictionary<Point, ushort> ConvertedChunk = new Dictionary<Point, ushort>();
			if (deleteMode)
			{
				ConvertedChunk = ConvertChunktoClipboard(EditorStamps.StampList[0]);
			}
			else
			{
				ConvertedChunk = ConvertChunktoClipboard(StageStamps.StampList[index]);
			}

			EditLayer.PasteFromClipboard(TileCoord, ConvertedChunk);
			EditorInstance.UpdateEditLayerActions();
			EditLayer.Deselect();
		}

		public bool DoesChunkMatch(Point point, Stamps.TileChunk CompareChunk, EditorLayer EditLayer, int chunkSize = 8)
		{
			Point TileCoord = new Point(point.X * 128, point.Y * 128);
			for (int x = 0; x < chunkSize; x++)
			{
				for (int y = 0; y < chunkSize; y++)
				{
					if (CompareChunk.TileMap[x, y] == EditLayer.GetTileAt(new Point(TileCoord.X + x, TileCoord.Y + y))) continue;
					else return false;
				}
			}
			return true;

		}

		public bool DoesChunkMatch(Point point, ushort[,] CompareChunk, EditorLayer EditLayer, int chunkSize = 8)
		{
			Point TileCoord = new Point(point.X * 128, point.Y * 128);
			for (int x = 0; x < chunkSize; x++)
			{
				for (int y = 0; y < chunkSize; y++)
				{
					if (CompareChunk[x, y] == EditLayer.GetTileAt(new Point(TileCoord.X + x, TileCoord.Y + y))) continue;
					else return false;
				}
			}
			return true;

		}

		public Dictionary<Point, ushort> ConvertChunktoClipboard(Stamps.TileChunk Chunk)
		{
			ushort[,] ChunkData = Chunk.TileMap;
			Dictionary<Point, ushort> ClipboardData = new Dictionary<Point, ushort>();

			for (int x = 0; x < Chunk.ChunkSize; x++)
			{
				for (int y = 0; y < Chunk.ChunkSize; y++)
				{
					ClipboardData.Add(new Point(x, y), ChunkData[x, y]);
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
			foreach (var image in ChunkImages)
			{
				image.Dispose();
			}
			ChunkImages.Clear();
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
