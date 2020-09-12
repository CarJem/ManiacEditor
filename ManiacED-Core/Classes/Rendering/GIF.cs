using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using ManiacEditor.Extensions;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;
using SFML.Graphics;
using RSDKv5;
using SystemColor = System.Drawing.Color;
using ImageSource = System.Windows.Media.ImageSource;
using GenerationsLib.Core;

namespace ManiacEditor.Classes.Rendering
{
    public class GIF : IDisposable
    {
        public static SFML.Graphics.Texture FromBitmap(Bitmap bitmap)
        {
            MemoryStream stm = new MemoryStream();
            bitmap.Save(stm, System.Drawing.Imaging.ImageFormat.Png);
            return new SFML.Graphics.Texture(stm);
        }

        #region Cache Collections

        public struct CacheKey
        {
            public Rectangle Rectangle { get; set; }
            public bool FlipX { get; set; }
            public bool FlipY { get; set; }

            public CacheKey(Rectangle _Rectangle, bool _FlipX, bool _FlipY)
            {
                this.Rectangle = _Rectangle;
                this.FlipX = _FlipX;
                this.FlipY = _FlipY;
            }
        }


        Dictionary<CacheKey, Bitmap> StandardCache { get; set; } = new Dictionary<CacheKey, Bitmap>();
        Dictionary<CacheKey, Bitmap> TransparentCache { get; set; } = new Dictionary<CacheKey, Bitmap>();
        Dictionary<CacheKey, BitmapSource> StandardSourceCache { get; set; } = new Dictionary<CacheKey, BitmapSource>();
        Dictionary<CacheKey, BitmapSource> TransparentSourceCache { get; set; } = new Dictionary<CacheKey, BitmapSource>();
        Dictionary<CacheKey, Texture> TextureCache { get; set; } = new Dictionary<CacheKey, Texture>();
        #endregion

        #region Definitions

        private bool IsRefreshable { get; set; } = false;

        private Bitmap StandardBitmap { get; set; }
        private Bitmap IndexedBitmap { get; set; }
        private Bitmap TransparentBitmap;
        private ColorPalette OriginalPalette;

        private Texture TextureBitmap;
        private float SemiOpacity = (float)0.5;
        public string Filename { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        #endregion

        #region Init
        public GIF(string _Filename, string PaletteDataPath = null)
        {
            this.IsRefreshable = true;
            this.Filename = _Filename;
            RefreshGIF(PaletteDataPath);
        }
        public GIF(Bitmap _Bitmap)
        {
            this.IsRefreshable = false;
            CreateMemoryOnlyGIF(_Bitmap);
        }
        #endregion

        #region Creation
        private void CreateTextureImage()
        {
            this.TextureBitmap = FromBitmap(StandardBitmap);
        }
        private void CreateTransparentImage()
        {
            TransparentBitmap = StandardBitmap.Clone(new Rectangle(0, 0, StandardBitmap.Width, StandardBitmap.Height), PixelFormat.Format32bppArgb);

            if (TransparentBitmap.Palette != null && TransparentBitmap.Palette.Entries.Length > 0) TransparentBitmap.MakeTransparent(TransparentBitmap.Palette.Entries[0]);
            else TransparentBitmap.MakeTransparent(SystemColor.FromArgb(0xff00ff));

            TransparentBitmap = SetImageOpacity(TransparentBitmap, SemiOpacity);
        }
        private void CreateStandardImage(string PaletteDataPath)
        {
            StandardBitmap = SetPaletteColors(BitmapExtensions.LoadBitmap(Filename), PaletteDataPath);
            if (StandardBitmap.Palette != null && StandardBitmap.Palette.Entries.Length > 0) StandardBitmap.MakeTransparent(StandardBitmap.Palette.Entries[0]);
            else StandardBitmap.MakeTransparent(SystemColor.FromArgb(0xff00ff));
        }

        private void CreateIndexedImage()
        {
            IndexedBitmap = BitmapExtensions.LoadBitmap(Filename);
            OriginalPalette = IndexedBitmap.Palette;
            Width = IndexedBitmap.Width;
            Height = IndexedBitmap.Height;
        }

        #endregion

        #region Manipulation
        public Bitmap SetImageOpacity(Bitmap image, float opacity)
        {
            Bitmap bmp = new Bitmap(image.Width, image.Height);
            using (Graphics gfx = Graphics.FromImage(bmp))
            {
                ColorMatrix matrix = new ColorMatrix();
                matrix.Matrix33 = opacity;
                ImageAttributes attributes = new ImageAttributes();
                attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                gfx.DrawImage(image, new Rectangle(0, 0, bmp.Width, bmp.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attributes);
            }
            return bmp;
        }
        private Bitmap CropImage(Bitmap source, Rectangle section)
        {
            try
            {
                Bitmap bmp = new Bitmap(section.Width, section.Height);
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.DrawImage(source, 0, 0, section, GraphicsUnit.Pixel);
                }
                return bmp;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        #endregion

        #region Palette
        private Bitmap SetPaletteColors(Bitmap _Bitmap, string PaletteDataPath)
        {
            Bitmap ModifiedStandardBitmap = _Bitmap.Clone(new Rectangle(0, 0, _Bitmap.Width, _Bitmap.Height), PixelFormat.Format8bppIndexed);

            //Encore Palettes (WIP Potentially Improvable)
            RSDKv5.Color[] PaletteColors = new RSDKv5.Color[256];
            if (PaletteDataPath != null && File.Exists(PaletteDataPath))
            {
                using (var stream = File.OpenRead(PaletteDataPath))
                {
                    for (int y = 0; y < 255; ++y)
                    {
                        PaletteColors[y].R = (byte)stream.ReadByte();
                        PaletteColors[y].G = (byte)stream.ReadByte();
                        PaletteColors[y].B = (byte)stream.ReadByte();
                    }
                }

                ColorPalette pal = ModifiedStandardBitmap.Palette;
                for (int y = 0; y < 255; ++y) pal.Entries[y] = SystemColor.FromArgb(PaletteColors[y].R, PaletteColors[y].G, PaletteColors[y].B);
                ModifiedStandardBitmap.Palette = pal;

                return ModifiedStandardBitmap;
            }
            else return _Bitmap;

        }

        #endregion

        #region Retrival
        public Bitmap ToBitmap()
        {
            return StandardBitmap;
        }
        public Classes.Rendering.GIF Clone()
        {
            return new Classes.Rendering.GIF(Filename);
        }
        public SFML.Graphics.Texture GetTexture()
        {
            return TextureBitmap;
        }

        public SFML.Graphics.Texture GetTexture(Rectangle section, bool flipX = false, bool flipY = false)
        {
            Texture texture;
            if (TextureCache.TryGetValue(new CacheKey(section, flipX, flipY), out texture)) return texture;
            else
            {
                Bitmap bmp;
                bmp = CropImage(StandardBitmap, section);
                if (flipX) bmp.RotateFlip(RotateFlipType.RotateNoneFlipX);
                if (flipY) bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
                texture = FromBitmap(bmp);

                TextureCache.Add(new CacheKey(section, flipX, flipY), texture);
                return texture;
            }
        }



        #endregion

        #region Get Bitmap
        public PixelFormat GetPixelFormat()
        {
            return PixelFormat.Format8bppIndexed;
        }
        public ColorPalette GetPalette()
        {
            return OriginalPalette;
        }
        public Bitmap GetBitmapIndexed(Rectangle section)
        {
            Bitmap bmp = new Bitmap(section.Width, section.Height);
            bmp = IndexedBitmap.Clone(section, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
            return bmp;
        }
        public Bitmap GetBitmap(Rectangle section, bool flipX = false, bool flipY = false, bool SemiTransparent = false)
        {
            if (SemiTransparent) return GetTransparentBitmap(section, flipX, flipY);
            else return GetNormalBitmap(section, flipX, flipY);
        }
        public Bitmap GetNormalBitmap(Rectangle section, bool flipX = false, bool flipY = false)
        {
            Bitmap bmp;
            if (StandardCache.TryGetValue(new CacheKey(section, flipX, flipY), out bmp)) return bmp;
            else
            {
                bmp = CropImage(StandardBitmap, section);
                if (flipX) bmp.RotateFlip(RotateFlipType.RotateNoneFlipX);
                if (flipY) bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);

                StandardCache.Add(new CacheKey(section, flipX, flipY), bmp);
                return bmp;
            }
        }
        public Bitmap GetTransparentBitmap(Rectangle section, bool flipX = false, bool flipY = false)
        {
            Bitmap bmp;
            if (TransparentCache.TryGetValue(new CacheKey(section, flipX, flipY), out bmp)) return bmp;
            else
            {
                bmp = CropImage(TransparentBitmap, section);
                if (flipX) bmp.RotateFlip(RotateFlipType.RotateNoneFlipX);
                if (flipY) bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);

                TransparentCache.Add(new CacheKey(section, flipX, flipY), bmp);
                return bmp;
            }
        }

        #endregion

        #region Get ImageSource

        public BitmapSource GetImageSource(Rectangle section, bool flipX = false, bool flipY = false, bool SemiTransparent = false)
        {
            if (SemiTransparent) return GetTransparentImageSource(section, false, false);
            else return GetNormalImageSource(section, false, false);
        }
        public BitmapSource GetNormalImageSource(Rectangle section, bool flipX = false, bool flipY = false)
        {
            BitmapSource source;
            Bitmap bmp;
            if (StandardSourceCache.TryGetValue(new CacheKey(section, flipX, flipY), out source)) return source;
            else
            {
                bmp = CropImage(StandardBitmap, section);
                if (flipX) bmp.RotateFlip(RotateFlipType.RotateNoneFlipX);
                if (flipY) bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);


                source = Extensions.ImageExtensions.ToBitmapSource(bmp);
                StandardSourceCache.Add(new CacheKey(section, flipX, flipY), source);
                return source;
            }
        }
        public BitmapSource GetTransparentImageSource(Rectangle section, bool flipX = false, bool flipY = false)
        {
            BitmapSource source;
            Bitmap bmp;
            if (TransparentSourceCache.TryGetValue(new CacheKey(section, flipX, flipY), out source)) return source;
            else
            {
                bmp = CropImage(TransparentBitmap, section);
                if (flipX) bmp.RotateFlip(RotateFlipType.RotateNoneFlipX);
                if (flipY) bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);

                source = Extensions.ImageExtensions.ToBitmapSource(bmp);
                TransparentSourceCache.Add(new CacheKey(section, flipX, flipY), source);
                return source;
            }
        }


        #endregion

        #region Refresh
        private void CreateMemoryOnlyGIF(Bitmap _Bitmap)
        {
            this.StandardBitmap = new Bitmap(_Bitmap);
            this.TransparentBitmap = this.StandardBitmap.Clone(new Rectangle(0, 0, StandardBitmap.Width, StandardBitmap.Height), PixelFormat.Format32bppArgb);
            this.TransparentBitmap = SetImageOpacity(TransparentBitmap, SemiOpacity);
            this.TextureBitmap = FromBitmap(StandardBitmap);

            Width = StandardBitmap.Width;
            Height = StandardBitmap.Height;
        }
        public void RefreshGIF(string PaletteDataPath = null)
        {
            if (!File.Exists(Filename)) throw new FileNotFoundException("The GIF file was not found.", Filename);

            CreateStandardImage(PaletteDataPath);
            CreateTransparentImage();
            CreateTextureImage();
            CreateIndexedImage();
        }
        public void Reload(string PaletteDataPath = null)
        {
            if (!IsRefreshable) return;
            Dispose();
            RefreshGIF(PaletteDataPath);
        }

        public void Reload(Bitmap _Bitmap)
        {
            if (!IsRefreshable)
            {
                Dispose();
                CreateMemoryOnlyGIF(_Bitmap);
            }

        }

        #endregion

        #region Disposal
        public void Dispose()
        {
            if (!IsRefreshable) return;

            if (StandardBitmap != null) StandardBitmap.Dispose();
            if (TransparentBitmap != null) TransparentBitmap.Dispose();
            if (TextureBitmap != null) TextureBitmap.Dispose();

            DisposeCache();
            DisposeOpaqueCache();

            DisposeSourceCache();
            DisposeOpaqueSourceCache();

            DisposeTextureCache();
        }

        private void DisposeCache()
        {
            if (null == StandardCache) return;
            foreach (Bitmap b in StandardCache.Values) b?.Dispose();
                StandardCache.Clear();
        }

        private void DisposeOpaqueCache()
        {
            if (null == TransparentCache) return;
            foreach (Bitmap b in TransparentCache.Values) b?.Dispose();
            TransparentCache.Clear();
        }

        private void DisposeSourceCache()
        {
            if (null == StandardSourceCache) return;
            StandardSourceCache.Clear();
        }

        private void DisposeOpaqueSourceCache()
        {
            if (null == TransparentSourceCache) return;
            TransparentSourceCache.Clear();
        }

        private void DisposeTextureCache()
        {
            if (null == TextureCache) return;
            foreach (var texture in TextureCache.Values) texture?.Dispose();
            TextureCache.Clear();
        }
        #endregion


    }



}