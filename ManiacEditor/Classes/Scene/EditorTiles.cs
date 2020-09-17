using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Linq;
using GenerationsLib.WPF;
using System.Windows.Documents;

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

        public static unsafe Byte GetIndexedPixel(int x, int y, BitmapData bmd)
        {
            byte* p = (byte*)bmd.Scan0.ToPointer();
            int offset = y * bmd.Stride + x;
            return p[offset];
        }
        private static void SetPixel(Bitmap bmp, int x, int y, int paletteEntry)
        {
            BitmapData data = bmp.LockBits(new Rectangle(new System.Drawing.Point(x, y), new System.Drawing.Size(1, 1)), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
            byte b = Marshal.ReadByte(data.Scan0);
            Marshal.WriteByte(data.Scan0, (byte)(b & 0xf | (paletteEntry)));
            bmp.UnlockBits(data);
        }
        public static Bitmap CropImage(Bitmap source, Rectangle section, bool indexed = false)
        {
            // An empty bitmap which will hold the cropped image


            Bitmap bmp = new Bitmap(section.Width, section.Height);
            if (indexed)
            {
                bmp = source.Clone(section, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
            }
            else
            {
                Graphics g = Graphics.FromImage(bmp);

                // Draw the given area (section) of the source image
                // at location 0,0 on the empty bitmap (bmp)
                g.DrawImage(source, 0, 0, section, GraphicsUnit.Pixel);
            }
            return bmp;
        }
        public static Bitmap MergeTiles(Bitmap[] IndexedTiles)
        {
            Bitmap mergedImg = new Bitmap(16, 16384, System.Drawing.Imaging.PixelFormat.Format8bppIndexed)
            {
                Palette = IndexedTiles[0].Palette
            };
            for (int i = 0; i < IndexedTiles.Count(); i++)
            {
                var bitmapData = IndexedTiles[i].LockBits(new Rectangle(0, 0, IndexedTiles[i].Width, IndexedTiles[i].Height), ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
                for (int h = 0; h < 16; h++)
                {
                    for (int w = 0; w < 16; w++)
                    {
                        int indexColor = GetIndexedPixel(w, h, bitmapData);
                        SetPixel(mergedImg, w, h + (16 * i), indexColor);
                    }
                }
                IndexedTiles[i].UnlockBits(bitmapData);
            }

            return mergedImg;
        }
        public static Bitmap[] SeperateTiles(Bitmap TileSet, bool indexedMode = false)
        {
            System.Collections.Generic.List<Bitmap> TileList = new System.Collections.Generic.List<Bitmap>();

            int tsize = TileSet.Height; //Height of the image in pixels
            for (int i = 0; i < (tsize / 16); i++) //We divide by 16 to get the "height" in blocks
            {
                Rectangle CropArea = new Rectangle(0, (i * 16), 16, 16); //we then get tile at Y: i * 16, 
                                                                         //we have to multiply i by 16 to get the "true Tile value" (1* 16 = 16, 2 * 16 = 32, etc.)
                if (!indexedMode)
                {
                    Bitmap CroppedImage = CropImage(TileSet, CropArea); // crop that image
                    TileList.Add(CroppedImage); // add it to the tile list
                }
                else
                {
                    Bitmap CroppedImageIndexed = CropImage(TileSet, CropArea, true); // crop that indexed image
                    TileList.Add(CroppedImageIndexed); // add it to the indexed tile list
                }
            }
            return TileList.ToArray();
        }
        public static void EditTileGraphics(int TileID)
        {         
            try
            {
                string OriginalPath = Methods.Solution.CurrentSolution.CurrentTiles.BaseImage.Filename;
                var Bitmap = Methods.Solution.CurrentSolution.CurrentTiles.BaseImage.GetBitmapIndexed(new System.Drawing.Rectangle(0, 16 * TileID, Methods.Solution.SolutionConstants.TILE_SIZE, Methods.Solution.SolutionConstants.TILE_SIZE));
                string Temp_Dir = Path.Combine(Methods.ProgramPaths.GetSettingsDirectory(), "TEMP");
                if (!System.IO.Directory.Exists(Temp_Dir)) Directory.CreateDirectory(Temp_Dir);
                string Temp_Path = Path.Combine(Temp_Dir, "TempTileEdit.gif");

                bool InitalSave = false;
                while (!InitalSave)
                {
                    try
                    {
                        Bitmap.Save(Temp_Path);
                        Bitmap.Dispose();
                        Bitmap = null;
                        InitalSave = true;
                    }
                    catch (Exception ex)
                    {
                        var dialogResult = System.Windows.MessageBox.Show(ex.ToString(), "", System.Windows.MessageBoxButton.OKCancel);
                        if (dialogResult != System.Windows.MessageBoxResult.OK) throw ex;
                    }
                }




                if (Temp_Path != null && Temp_Path != "" && File.Exists(Temp_Path)) Methods.ProgramLauncher.OpenImage(Temp_Path);
                else return;

                var result = System.Windows.MessageBox.Show("Click OK when you are done making your edits!", "", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                if (result == System.Windows.MessageBoxResult.OK)
                {
                    if (Temp_Path != null && Temp_Path != "" && File.Exists(Temp_Path))
                    {
                        Bitmap TileBitmap = GenerationsLib.Core.BitmapExtensions.LoadBitmap(Temp_Path);
                        Bitmap OriginalBitmap = GenerationsLib.Core.BitmapExtensions.LoadBitmap(OriginalPath);

                        var Tiles = SeperateTiles(OriginalBitmap, true);
                        Tiles[TileID] = TileBitmap;
                        var NewBitmap = MergeTiles(Tiles);


                        bool FinalSave = false;
                        while (!FinalSave)
                        {
                            try
                            {
                                NewBitmap.Save(OriginalPath);
                                NewBitmap.Dispose();
                                NewBitmap = null;
                                FinalSave = true;
                            }
                            catch (Exception ex)
                            {
                                var dialogResult = System.Windows.MessageBox.Show(ex.ToString(), "", System.Windows.MessageBoxButton.OKCancel);
                                if (dialogResult != System.Windows.MessageBoxResult.OK) throw ex;
                            }
                        }
                        Methods.Internal.UserInterface.ReloadSpritesAndTextures();

                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString());
                return;
            }
        }
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
