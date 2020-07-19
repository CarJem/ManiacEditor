using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Linq;

namespace ManiacEditor.Classes.Scene
{
    public class EditorTiles : IDisposable
    {
        #region Definitions

        #region GIF Variables
        public Classes.Rendering.GIF BaseImage { get; set; }
        public Classes.Rendering.GIF IDImage { get; set; }
        public Classes.Rendering.GIF InternalImage { get; set; }
        public Classes.Rendering.GIF CollisionMaskA { get; private set; }
        public Classes.Rendering.GIF CollisionMaskB { get; private set; }
        public Classes.Rendering.GIF CollectiveImage { get; set; }
        #endregion

        #region Tile Config
        public RSDKv5.Tileconfig TileConfig
        {
            get
            {
                return _TileConfig;
            }
            set
            {
                _TileConfig = value;
                UpdateConfigCollision();
            }
        }
        private RSDKv5.Tileconfig _TileConfig { get; set; }
        #endregion

        #endregion

        #region Init
        public EditorTiles()
        {
            BaseImage = new Classes.Rendering.GIF(Path.Combine(Environment.CurrentDirectory, "16x16Tiles_ID.gif"));
            TileConfig = new RSDKv5.Tileconfig();
            CreateDefaultTiles();
        }
        public EditorTiles(string StageDirectory, string PaletteDataPath = null)
		{
			BaseImage = new Classes.Rendering.GIF(Path.Combine(StageDirectory, "16x16Tiles.gif"), PaletteDataPath);
            CreateDefaultTiles();
        }


        private void CreateDefaultTiles()
        {
            IDImage = new Classes.Rendering.GIF(Environment.CurrentDirectory + "\\Resources\\Tile Overlays\\" + "16x16Tiles_ID.gif");
            InternalImage = new Classes.Rendering.GIF(Environment.CurrentDirectory + "\\Resources\\Tile Overlays\\" + "16x16Tiles_Edit.gif");
            CollectiveImage = new Classes.Rendering.GIF(CreateCollectiveImage());
        }
        #endregion

        #region Collision

        public void UpdateConfigCollision()
        {
            try
            {
                if (CollisionMaskA != null) CollisionMaskA.Dispose();
                if (CollisionMaskB != null) CollisionMaskB.Dispose();
                if (CollectiveImage != null) CollectiveImage.Dispose();

                CollisionMaskA = new Classes.Rendering.GIF(DrawCollisionMaskA());
                CollisionMaskB = new Classes.Rendering.GIF(DrawCollisionMaskB());
                CollectiveImage = new Classes.Rendering.GIF(CreateCollectiveImage());
            }
            catch (Exception ex)
            {
                throw new Events.TileConfigException("Unable to load Tileconfig.bin!" + Environment.NewLine + "Full Exception Details: " + ex.Message);
            }

        }
        private Bitmap DrawCollisionMaskA(bool Merged = false)
        {
            Bitmap bitmap = (Merged ? new Bitmap(BaseImage.ToBitmap()) : new Bitmap(16, 16384));
            using (Graphics gfx = Graphics.FromImage(bitmap))
            {
                for (int i = 0; i < 1024; i++)
                {
                    gfx.DrawImage(TileConfig.CollisionPath1[i].DrawCMask(Color.FromArgb(0, 0, 0, 0), Color.White), new Rectangle(0, (16 * i), 16, 16));
                }
            }
            return bitmap;
        }
        private Bitmap DrawCollisionMaskB(bool Merged = false)
        {
            Bitmap bitmap = (Merged ? new Bitmap(BaseImage.ToBitmap()) : new Bitmap(16, 16384));
            using (Graphics gfx = Graphics.FromImage(bitmap))
            {
                for (int i = 0; i < 1024; i++)
                {
                    gfx.DrawImage(TileConfig.CollisionPath2[i].DrawCMask(Color.FromArgb(0, 0, 0, 0), Color.White), new Rectangle(0, (16 * i), 16, 16));
                }
            }

            return bitmap;
        }

        #endregion

        #region Collective Image

        public Bitmap CreateCollectiveImage()
        {
            int bitmap_width = 16 * 5;
            int bitmap_height = 16384;
            Bitmap bitmap = new Bitmap(bitmap_width, bitmap_height);
            using (Graphics gfx = Graphics.FromImage(bitmap))
            {
                if (BaseImage != null) gfx.DrawImage(BaseImage.ToBitmap(), new Point(0, 0));
                if (IDImage != null) gfx.DrawImage(IDImage.ToBitmap(), new Point(16, 0));
                if (CollisionMaskA != null) gfx.DrawImage(CollisionMaskA.ToBitmap(), new Point(32, 0));
                if (CollisionMaskB != null) gfx.DrawImage(CollisionMaskB.ToBitmap(), new Point(48, 0));
                if (InternalImage != null) gfx.DrawImage(InternalImage.ToBitmap(), new Point(64, 0));
            }
            return bitmap;
        }

        #endregion

        #region Import/Export
        private static void DrawIndexedTile(ref Classes.Rendering.GIF InputImage, ref Bitmap ExportBitmap, int TileIndex, int x, int y)
        {
            var tileBitmap = InputImage.GetBitmapIndexed(GetSection(TileIndex, InputImage.Width));
            var bitmapData = tileBitmap.LockBits(new Rectangle(0, 0, 16, 16), ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
            byte[][] CurrentTileData = GetTileColors(bitmapData);

            for (int h = 0; h < 16; h++)
            {
                for (int w = 0; w < 16; w++)
                {
                    int indexColor = CurrentTileData[h][w];
                    SetPixel(ExportBitmap, x + w, y + h, indexColor);
                }
            }
            tileBitmap.UnlockBits(bitmapData);




            byte[][] GetTileColors(BitmapData _bitmapData)
            {
                byte[][] output = new byte[16][];
                for (int h = 0; h < 16; h++)
                {
                    output[h] = new byte[16];
                    for (int w = 0; w < 16; w++)
                    {
                        output[h][w] = GetIndexedPixel(w, h, _bitmapData);
                    }
                }
                return output;
            }

            Rectangle GetSection(int index, int base_width)
            {             
                int _columns = base_width / 16;

                int _posX = (index % _columns) * 16;
                int _posY = (index / _columns) * 16;

                return new Rectangle(_posX, _posY, 16, 16);
            }

            unsafe Byte GetIndexedPixel(int _x, int _y, BitmapData bmd)
            {
                byte* p = (byte*)bmd.Scan0.ToPointer();
                int offset = _y * bmd.Stride + _x;
                return p[offset];
            }

            void SetPixel(Bitmap bmp, int _x, int _y, int paletteEntry)
            {
                BitmapData data = bmp.LockBits(new Rectangle(new System.Drawing.Point(_x, _y), new System.Drawing.Size(1, 1)), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
                byte b = Marshal.ReadByte(data.Scan0);
                Marshal.WriteByte(data.Scan0, (byte)(b & 0xf | (paletteEntry)));
                bmp.UnlockBits(data);
            }
        }
        private static void DrawTile(Graphics Graphics, ref Classes.Rendering.GIF InputImage, int TileIndex, int x, int y)
        {
            var tileBitmap = InputImage.GetBitmap(GetSection(TileIndex, InputImage.Width));
            Graphics.DrawImage(tileBitmap, x, y);


            Rectangle GetSection(int index, int base_width)
            {
                int _columns = base_width / 16;

                int _posX = (index % _columns) * 16;
                int _posY = (index / _columns) * 16;

                return new Rectangle(_posX, _posY, 16, 16);
            }
        }
        public static void ExportIndexed(Classes.Rendering.GIF InputImage, string filename, int tilesPerRow)
        {
            int lastIndex = -1;
            try
            {
                int columns = InputImage.Width / 16;
                int rows = InputImage.Height / 16;


                int maxTiles = columns * rows;
                int tileSize = 16;

                int imageWidth = tileSize * tilesPerRow;
                int imageHeight = (maxTiles / tilesPerRow) * tileSize;

                Bitmap bitmap = new Bitmap(imageWidth, imageHeight, System.Drawing.Imaging.PixelFormat.Format8bppIndexed)
                {
                    Palette = InputImage.GetPalette()
                };

                int row = 0;
                for (int tileIndex = 0; tileIndex < maxTiles - 1;)
                {
                    for (int col = 0; col < tilesPerRow;)
                    {
                        if (tileIndex > maxTiles - 1) break;
                        else
                        {
                            DrawIndexedTile(ref InputImage, ref bitmap, tileIndex, col * 16, row * 16);
                            tileIndex++;
                            lastIndex = tileIndex;
                        }
                        col++;
                    }
                    row++;
                }

                bitmap.Save(filename);
                bitmap.Dispose();
            }
            catch (Exception ex) 
            {
                System.Windows.MessageBox.Show("[" + lastIndex + "]" + ex.Message);
            }

        }
        public static void ImportIndexed(string filename, string new_filename)
        {
            Classes.Rendering.GIF importedSheet = new Rendering.GIF(filename);
            ExportIndexed(importedSheet, new_filename, 1);
            importedSheet.Dispose();
            importedSheet = null;
        }
        public static void Export(Classes.Rendering.GIF InputImage, string filename, int tilesPerRow)
        {
            int lastIndex = -1;
            try
            {
                int columns = InputImage.Width / 16;
                int rows = InputImage.Height / 16;

                int maxTiles = columns * rows;
                int tileSize = 16;

                int imageWidth = tileSize * tilesPerRow;
                int imageHeight = (maxTiles / tilesPerRow) * tileSize;

                Bitmap bitmap = new Bitmap(imageWidth, imageHeight);
                bitmap.MakeTransparent();

                int row = 0;
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    for (int tileIndex = 0; tileIndex < maxTiles - 1;)
                    {
                        for (int col = 0; col < tilesPerRow;)
                        {
                            if (tileIndex > maxTiles - 1) break;
                            else
                            {
                                DrawTile(g, ref InputImage, tileIndex, col * tileSize, row * tileSize);
                                tileIndex++;
                                lastIndex = tileIndex;
                            }
                            col++;
                        }
                        row++;
                    }
                }
                bitmap.Save(filename);
                bitmap.Dispose();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("[" + lastIndex + "]" + ex.Message);
            }
        }
        public static Classes.Rendering.GIF Import(string filename)
        {
            Classes.Rendering.GIF InputImage = new Classes.Rendering.GIF(filename);
            int lastIndex = -1;
            try
            {
                int tilesPerRow = 1;

                int columns = InputImage.Width / 16;
                int rows = InputImage.Height / 16;

                int maxTiles = columns * rows;
                int tileSize = 16;

                int imageWidth = tileSize * tilesPerRow;
                int imageHeight = (maxTiles / tilesPerRow) * tileSize;

                Bitmap bitmap = new Bitmap(imageWidth, imageHeight);

                int row = 0;
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    for (int tileIndex = 0; tileIndex < maxTiles - 1;)
                    {
                        for (int col = 0; col < tilesPerRow;)
                        {
                            if (tileIndex > maxTiles - 1) break;
                            else
                            {
                                DrawTile(g, ref InputImage, tileIndex, col * tileSize, row * tileSize);
                                tileIndex++;
                                lastIndex = tileIndex;
                            }
                            col++;
                        }
                        row++;
                    }
                }
                return new Rendering.GIF(bitmap);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("[" + lastIndex + "]" + ex.Message);
                return null;
            }
        }


        #endregion

        #region Other
        public void Write(string filename)
        {
            Bitmap write = BaseImage.GetBitmap(new Rectangle(0,0,16,16384));
            write.Save(filename);
                
        }
        public void Reload(string PaletteDataPath = null)
        {
            if (BaseImage != null) BaseImage.Reload(PaletteDataPath);
            if (IDImage != null) IDImage.Reload();
            if (InternalImage != null) InternalImage.Reload();
            if (CollisionMaskA != null) CollisionMaskA.Reload(DrawCollisionMaskA());
            if (CollisionMaskB != null) CollisionMaskB.Reload(DrawCollisionMaskB());
            if (CollectiveImage != null) CollectiveImage.Reload(CreateCollectiveImage());
        }
        public void Dispose()
        {
            if (BaseImage != null) BaseImage.Dispose();
            if (IDImage != null) IDImage.Dispose();
            if (CollisionMaskA != null) CollisionMaskA.Dispose();
            if (CollisionMaskB != null) CollisionMaskB.Dispose();
            if (InternalImage != null) InternalImage.Dispose();
            if (CollectiveImage != null) CollectiveImage.Dispose();
        }
        #endregion
    }
}
