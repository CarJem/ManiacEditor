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

        public IList<TileChunk> ChunkList = new List<TileChunk>();

        public IList<Bitmap> ChunkImages = new List<Bitmap>();

        public EditorChunk(Editor instance, StageTiles stageTiles)
        {
            EditorInstance = instance;
            Tiles = instance.StageTiles;

            AddTestMaps();


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
                    if (this.ChunkList[chunkIndex].TileMap[tx, ty].tile != 0xffff)
                    {
                        DrawTile(g, this.ChunkList[chunkIndex].TileMap[tx, ty].tile, tx - rect.X, ty - rect.Y);
                    }
                }
            }
            g.Flush();
            g.Dispose();
            this.ChunkImages.Insert(chunkIndex,bmp);
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


            ChunkList.Add(new TileChunk(convertedPoints));        
        }

        public void DisposeTextures()
        {
            foreach(var image in ChunkImages)
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
            ChunkList.Add(new TileChunk(TestChunkDic));
            TestChunkDic.Clear();

            for (int cx = 0; cx < 8; cx++)
            {
                for (int cy = 0; cy < 8; cy++)
                {
                    TestChunkDic.Add(new Point(cx, cy), 0x0001);
                }
            }
            ChunkList.Add(new TileChunk(TestChunkDic));
            TestChunkDic.Clear();
        }

        public class TileChunk
        {
            public Tile[,] TileMap;
            const int CHUNK_SIZE = 8;

            public TileChunk(Dictionary<Point, ushort> points)
            {
                TileMap = new Tile[CHUNK_SIZE, CHUNK_SIZE];
                for (int x = 0; x < CHUNK_SIZE; x++)
                {
                    for (int y = 0; y < CHUNK_SIZE; y++)
                    {
                        Point p = new Point(x, y);
                        if(points.ContainsKey(p))TileMap[x , y] = new Tile(points[p]);
                        else TileMap[x, y] = new Tile(0xffff);
                    }   
                }
            }

            public class Tile
            {
                public ushort tile;

                public Tile(ushort _tile)
                {
                    tile = _tile;
                }

            }

        }
    }
}
