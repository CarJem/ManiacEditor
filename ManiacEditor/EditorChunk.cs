using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using RSDKv5;
using SharpDX.Direct3D9;
using ManiacEditor.Actions;
using ManiacEditor.Enums;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using IronPython.Modules;

namespace ManiacEditor
{
    public class EditorChunk
    {

        const int TILES_CHUNK_SIZE = 16;

        public const int TILE_SIZE = 16;

        public DevicePanel GraphicsPanel;

        public Editor EditorInstance;


        Texture[][] TileChunksTextures;

        Texture[] ChunksTextures;


        /// <summary>
        /// Collection of rules and mappings representing the horizontal scrolling info
        /// and other rules affecting lines of pixels in this layer
        /// </summary>

        static int DivideRoundUp(int number, int by)
        {
            return (number + by - 1) / by;
        }

        public class PointsMap
        {
            HashSet<Point>[][] PointsChunks;
            HashSet<Point> OutOfBoundsPoints = new HashSet<Point>();
            public int Count = 0;

            public PointsMap(int width, int height)
            {
                PointsChunks = new HashSet<Point>[DivideRoundUp(height, TILES_CHUNK_SIZE)][];
                for (int i = 0; i < PointsChunks.Length; ++i)
                {
                    PointsChunks[i] = new HashSet<Point>[DivideRoundUp(width, TILES_CHUNK_SIZE)];
                    for (int j = 0; j < PointsChunks[i].Length; ++j)
                        PointsChunks[i][j] = new HashSet<Point>();
                }
            }

            public void Add(Point point)
            {

                HashSet<Point> h;
                if (point.Y < 0 || point.X < 0 || point.Y / TILES_CHUNK_SIZE >= PointsChunks.Length || point.X / TILES_CHUNK_SIZE >= PointsChunks[0].Length)
                    h = OutOfBoundsPoints;
                else
                    h = PointsChunks[point.Y / TILES_CHUNK_SIZE][point.X / TILES_CHUNK_SIZE];
                Count -= h.Count;
                h.Add(point);
                Count += h.Count;
            }

            public void Remove(Point point)
            {
                HashSet<Point> h;
                if (point.Y < 0 || point.X < 0 || point.Y / TILES_CHUNK_SIZE >= PointsChunks.Length || point.X / TILES_CHUNK_SIZE >= PointsChunks[0].Length)
                    h = OutOfBoundsPoints;
                else
                    h = PointsChunks[point.Y / TILES_CHUNK_SIZE][point.X / TILES_CHUNK_SIZE];
                Count -= h.Count;
                h.Remove(point);
                Count += h.Count;
            }

            public bool Contains(Point point)
            {
                if (point.Y < 0 || point.X < 0 || point.Y / TILES_CHUNK_SIZE >= PointsChunks.Length || point.X / TILES_CHUNK_SIZE >= PointsChunks[0].Length)
                    return OutOfBoundsPoints.Contains(point);
                else
                    return PointsChunks[point.Y / TILES_CHUNK_SIZE][point.X / TILES_CHUNK_SIZE].Contains(point);
            }

            public bool IsChunkUsed(int x, int y)
            {
                return PointsChunks[y][x].Count > 0;
            }

            public void Clear()
            {
                for (int i = 0; i < PointsChunks.Length; ++i)
                    for (int j = 0; j < PointsChunks[i].Length; ++j)
                        PointsChunks[i][j].Clear();
                OutOfBoundsPoints.Clear();
                Count = 0;
            }

            public HashSet<Point> GetChunkPoint(int x, int y)
            {
                return PointsChunks[y][x];
            }

            public List<Point> PopAll()
            {
                List<Point> points = GetAll();
                Clear();
                return points;
            }

            public List<Point> GetAll()
            {
                List<Point> points = new List<Point>(Count);
                for (int i = 0; i < PointsChunks.Length; ++i)
                    for (int j = 0; j < PointsChunks[i].Length; ++j)
                        points.AddRange(PointsChunks[i][j]);
                points.AddRange(OutOfBoundsPoints);
                return points;
            }

            public void AddPoints(List<Point> points)
            {
                points.ForEach(point => Add(point));
            }


        }

        public EditorChunk(Editor instance)
        {
            EditorInstance = instance;

            ChunksTextures = new Texture[1024];

            TileChunksTextures = new Texture[DivideRoundUp(1024, TILES_CHUNK_SIZE)][];
            for (int i = 0; i < TileChunksTextures.Length; ++i)
                TileChunksTextures[i] = new Texture[DivideRoundUp(1024, TILES_CHUNK_SIZE)];


        }

        private void InvalidateChunk(int x, int y)
        {
            TileChunksTextures[y][x]?.Dispose();
            TileChunksTextures[y][x] = null;
        }



        private Rectangle GetChunkArea(int x, int y)
        {
            return new Rectangle(x, y, 8, 8);
        }

        public void DrawTile(DevicePanel d, int tile, int x, int y, bool selected, int Transperncy)
        {
            //ushort TileIndex = (ushort)(tile & 0x3ff);
            int TileIndex = tile;
            bool flipX = false;
            bool flipY = false;

            d.DrawBitmap(EditorInstance.StageTiles.Image.GetTexture(d._device, new Rectangle(0, (tile & 0x3ff) * TILE_SIZE, TILE_SIZE, TILE_SIZE), flipX, flipY),
            x * TILE_SIZE, y * TILE_SIZE, TILE_SIZE, TILE_SIZE, selected, Transperncy);

            if (EditorInstance.showFlippedTileHelper == true)
            {
                d.DrawBitmap(EditorInstance.StageTiles.EditorImage.GetTexture(d._device, new Rectangle(0, 3 * TILE_SIZE, TILE_SIZE, TILE_SIZE), false, false),
                x * TILE_SIZE, y * TILE_SIZE, TILE_SIZE, TILE_SIZE, selected, Transperncy);
            }
            if (EditorInstance.showTileID == true)
            {
                d.DrawBitmap(EditorInstance.StageTiles.IDImage.GetTexture(d._device, new Rectangle(0, (tile & 0x3ff) * TILE_SIZE, TILE_SIZE, TILE_SIZE), false, false),
                x * TILE_SIZE, y * TILE_SIZE, TILE_SIZE, TILE_SIZE, selected, Transperncy);
            }
            if (selected)
            {
                d.DrawLine(x * TILE_SIZE, y * TILE_SIZE, x * TILE_SIZE + TILE_SIZE, y * TILE_SIZE, System.Drawing.Color.Brown);
                d.DrawLine(x * TILE_SIZE, y * TILE_SIZE, x * TILE_SIZE, y * TILE_SIZE + TILE_SIZE, System.Drawing.Color.Brown);
                d.DrawLine(x * TILE_SIZE + TILE_SIZE, y * TILE_SIZE + TILE_SIZE, x * TILE_SIZE + TILE_SIZE, y * TILE_SIZE, System.Drawing.Color.Brown);
                d.DrawLine(x * TILE_SIZE + TILE_SIZE, y * TILE_SIZE + TILE_SIZE, x * TILE_SIZE, y * TILE_SIZE + TILE_SIZE, System.Drawing.Color.Brown);
            }
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
            if (EditorInstance.showCollisionA == true)
            {
                if (SolidLrbA || SolidTopA)
                {
                    //Get a bitmap of the collision
                    Bitmap cm = EditorInstance.CollisionLayerA[TileIndexInt].Clone(new Rectangle(0, 0, 16, 16), System.Drawing.Imaging.PixelFormat.DontCare);

                    if (SolidTopA && !SolidLrbA)
                    {
                        for (int ix = 0; ix < cm.Width; ix++)
                        {
                            for (int iy = 0; iy < cm.Height; iy++)
                            {
                                //System.Drawing.Color gotColor = cm.GetPixel(ix, iy);
                                //if (gotColor == EditorInstance.CollisionAllSolid)
                                //{
                                cm.SetPixel(ix, iy, EditorInstance.CollisionTopOnlySolid);
                                //}
                            }
                        }
                    }//Change Colour if Solidity = Top

                    if (SolidLrbA && !SolidTopA)
                    {
                        for (int ix = 0; ix < cm.Width; ix++)
                        {
                            for (int iy = 0; iy < cm.Height; iy++)
                            {
                                //System.Drawing.Color gotColor = cm.GetPixel(ix, iy);
                                //if (gotColor == EditorInstance.CollisionAllSolid)
                                //{
                                cm.SetPixel(ix, iy, EditorInstance.CollisionLRDSolid);
                                //}
                            }
                        }
                    } //Change Colour if Solidity = All But Top

                    if (flipX) { cm.RotateFlip(RotateFlipType.RotateNoneFlipX); }

                    if (flipY) { cm.RotateFlip(RotateFlipType.RotateNoneFlipY); }

                    g.DrawImage(cm, new Rectangle(x * TILE_SIZE, y * TILE_SIZE, TILE_SIZE, TILE_SIZE));
                }
            }
            if (EditorInstance.showCollisionB == true)
            {
                if (SolidLrbB || SolidTopB)
                {
                    //Get a bitmap of the collision
                    Bitmap cm = EditorInstance.CollisionLayerB[TileIndex].Clone(new Rectangle(0, 0, 16, 16), System.Drawing.Imaging.PixelFormat.DontCare);

                    if (SolidTopB && !SolidLrbB)
                    {
                        for (int ix = 0; ix < cm.Width; ix++)
                        {
                            for (int iy = 0; iy < cm.Height; iy++)
                            {
                                System.Drawing.Color gotColor = cm.GetPixel(ix, iy);
                                if (gotColor == EditorInstance.CollisionAllSolid)
                                {
                                    cm.SetPixel(ix, iy, EditorInstance.CollisionTopOnlySolid);
                                }
                            }
                        }
                    }//Change Colour if Solidity = Top

                    if (SolidLrbB && !SolidTopB)
                    {
                        for (int ix = 0; ix < cm.Width; ix++)
                        {
                            for (int iy = 0; iy < cm.Height; iy++)
                            {
                                System.Drawing.Color gotColor = cm.GetPixel(ix, iy);
                                if (gotColor == EditorInstance.CollisionAllSolid)
                                {
                                    cm.SetPixel(ix, iy, EditorInstance.CollisionLRDSolid);
                                }
                            }
                        }
                    } //Change Colour if Solidity = All But Top

                    if (flipX) { cm.RotateFlip(RotateFlipType.RotateNoneFlipX); }

                    if (flipY) { cm.RotateFlip(RotateFlipType.RotateNoneFlipY); }

                    g.DrawImage(cm, new Rectangle(x * TILE_SIZE, y * TILE_SIZE, TILE_SIZE, TILE_SIZE));
                }
            }

            if (EditorInstance.showFlippedTileHelper == true)
            {
                g.DrawImage(EditorInstance.StageTiles.EditorImage.GetBitmap(new Rectangle(0, 3 * TILE_SIZE, TILE_SIZE, TILE_SIZE), false, false),
                            new Rectangle(x * TILE_SIZE, y * TILE_SIZE, TILE_SIZE, TILE_SIZE));
            }
            if (EditorInstance.showTileID == true)
            {
                g.DrawImage(EditorInstance.StageTiles.IDImage.GetBitmap(new Rectangle(0, TileIndex * TILE_SIZE, TILE_SIZE, TILE_SIZE), false, false),
                            new Rectangle(x * TILE_SIZE, y * TILE_SIZE, TILE_SIZE, TILE_SIZE));
            }


        }

        public void DrawTile(Graphics g, ushort tile)
        {
            g.DrawImage(EditorInstance.StageTiles.Image.GetBitmap(new Rectangle(0, 2 * TILE_SIZE, TILE_SIZE, TILE_SIZE), false, false),
                new Rectangle(0, 0, TILE_SIZE, TILE_SIZE));
        }


        public Texture GetChunkTexture(DevicePanel d, int x, int y, int tile = 0)
        {
            if (this.ChunksTextures[x] != null) return this.ChunksTextures[x];

            Rectangle rect = GetChunkArea(x, y);

            Bitmap bmp2 = new Bitmap(rect.Width * TILE_SIZE, rect.Height * TILE_SIZE, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            var squareSize = (bmp2.Width > bmp2.Height ? bmp2.Width : bmp2.Height);
            int factor = 32;
            int newSize = (int)Math.Round((squareSize / (double)factor), MidpointRounding.AwayFromZero) * factor;
            if (newSize == 0) newSize = factor;
            while (newSize < squareSize) newSize += factor;

            Bitmap bmp = new Bitmap(newSize, newSize, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            using (bmp)
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    for (int ty = rect.Y; ty < rect.Y + rect.Height; ++ty)
                    {
                        for (int tx = rect.X; tx < rect.X + rect.Width; ++tx)
                        {
                            //DrawTile(g, 2);
                        }
                    }
                }
                this.ChunksTextures[x] = TextureCreator.FromBitmap(d._device, bmp);
            }

            return this.ChunksTextures[x];
        }

        public void DrawChunk(DevicePanel d, int x, int y, int Transperncy, int tile = 0)
        {
            Rectangle rect = GetChunkArea(x, y);

            for (int ty = rect.Y; ty < rect.Y + rect.Height; ++ty)
            {
                for (int tx = rect.X; tx < rect.X + rect.Width; ++tx)
                {
                    Point p = new Point(tx, ty);
                    // We will draw those later
                    DrawTile(d, tile, tx, ty, false, Transperncy);
                }
            }
        }


        public void Dispose()
        {
            foreach (Texture[] textures in TileChunksTextures)
                foreach (Texture texture in textures)
                    if (texture != null)
                        texture.Dispose();
            TileChunksTextures = null;
        }

        public void DisposeTextures()
        {
            foreach (Texture[] textures in TileChunksTextures)
            {
                for (int i = 0; i < textures.Length; ++i)
                {
                    if (textures[i] != null)
                    {
                        textures[i].Dispose();
                        textures[i] = null;
                    }
                }
            }
        }

        public class TileChunk
        {
            Tile[][] TileMap;
            const int CHUNK_SIZE = 8;

            public TileChunk(Dictionary<Point, ushort> points)
            {
                TileMap = new Tile[CHUNK_SIZE][];
                for (int x = 0; x < CHUNK_SIZE; x++)
                {
                    for (int y = 0; y < CHUNK_SIZE; y++)
                    {
                        TileMap[x][y] = new Tile();
                    }
                }
            }

            public class Tile
            {
                ushort tile;
                ushort TileIndex;
                bool flipX;
                bool flipY;
                bool SolidTopA;
                bool SolidLrbA;
                bool SolidTopB;
                bool SolidLrbB;

                public Tile(ushort _tile = 1)
                {
                    tile = _tile;
                    TileIndex = (ushort)(tile & 0x3ff);
                    flipX = ((tile >> 10) & 1) == 1;
                    flipY = ((tile >> 11) & 1) == 1;
                    SolidTopA = ((tile >> 12) & 1) == 1;
                    SolidLrbA = ((tile >> 13) & 1) == 1;
                    SolidTopB = ((tile >> 14) & 1) == 1;
                    SolidLrbB = ((tile >> 15) & 1) == 1;
                }

            }

        }
    }
}
