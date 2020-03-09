using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using SystemColor = System.Drawing.Color;
using SharpDX.Direct3D9;
using RSDKv5;
using ManiacEditor.Extensions;

namespace ManiacEditor.Methods.Draw
{
    public class GIF : IDisposable
    {
        Bitmap _bitmap;
        Bitmap _bitmap_selected;
        string _bitmapFilename;
        SFML.Graphics.Texture _bitmap_texture;

        Dictionary<Tuple<Rectangle, bool, bool>, Bitmap> _bitmapCache { get; set; } = new Dictionary<Tuple<Rectangle, bool, bool>, Bitmap>();
        Dictionary<Tuple<Rectangle, bool, bool>, Bitmap> _bitmap_selected_Cache { get; set; } = new Dictionary<Tuple<Rectangle, bool, bool>, Bitmap>();
        Dictionary<Tuple<Rectangle, bool, bool>, SFML.Graphics.Texture> _texturesCache { get; set; } = new Dictionary<Tuple<Rectangle, bool, bool>, SFML.Graphics.Texture>();

        public GIF(string filename, string encoreColors = null)
        {

            if (!File.Exists(filename))
            {
                throw new FileNotFoundException("The GIF file was not found.", filename);
            }
            _bitmap = new Bitmap(filename);
            _bitmap_selected = _bitmap.Clone(new Rectangle(0, 0, _bitmap.Width, _bitmap.Height), PixelFormat.Format32bppArgb);

            if (encoreColors != null)
            {
                LoadEncoreColors(encoreColors);
            }


            if (_bitmap.Palette != null && _bitmap.Palette.Entries.Length > 0) _bitmap.MakeTransparent(_bitmap.Palette.Entries[0]);
            else _bitmap.MakeTransparent(SystemColor.FromArgb(0xff00ff));

            if (_bitmap_selected.Palette != null && _bitmap_selected.Palette.Entries.Length > 0) _bitmap_selected.MakeTransparent(_bitmap_selected.Palette.Entries[0]);
            else _bitmap_selected.MakeTransparent(SystemColor.FromArgb(0xff00ff));

            ColorImage(ref _bitmap_selected);

            // stash the filename too, so we can reload later
            _bitmapFilename = filename;
        }

        public GIF(Bitmap bitmap)
        {
            this._bitmap = new Bitmap(bitmap);
            this._bitmap_selected = this._bitmap.Clone(new Rectangle(0, 0, _bitmap.Width, _bitmap.Height), PixelFormat.Format32bppArgb);
            ColorImage(ref this._bitmap_selected);
            this._bitmap_texture = TextureHelper.FromBitmap(_bitmap);
        }

        public GIF(Image image)
        {
            this._bitmap = new Bitmap(image);
        }

        private void ColorImage(ref Bitmap pImage)
        {

            System.Drawing.Color tintColor = System.Drawing.Color.FromArgb(255, 0, 0);

            for (int x = 0; x < pImage.Width; x++)
            {
                for (int y = 0; y < pImage.Height; y++)
                {

                    //Calculate the new color
                    var oldColor = pImage.GetPixel(x, y);
                    var newColor = oldColor.Blend(tintColor, 0.7);

                    System.Drawing.Color newColorA = System.Drawing.Color.FromArgb(oldColor.A, newColor.R, newColor.G, newColor.B);
                    pImage.SetPixel(x, y, newColorA);

                }
            }
        }



        private void LoadEncoreColors(string encoreColors = null)
        {
            Bitmap _bitmapEditMemory;
            _bitmapEditMemory = _bitmap.Clone(new Rectangle(0, 0, _bitmap.Width, _bitmap.Height), PixelFormat.Format8bppIndexed);
            //Debug.Print(_bitmapEditMemory.Palette.Entries.Length.ToString() + "(1)");

            //Encore Palettes (WIP Potentially Improvable)
            RSDKv5.Color[] readableColors = new RSDKv5.Color[256];
            bool loadSpecialColors = false;
            if (encoreColors != null && File.Exists(encoreColors))
            {
                using (var stream = File.OpenRead(encoreColors))
                {
                    for (int y = 0; y < 255; ++y)
                    {
                        readableColors[y].R = (byte)stream.ReadByte();
                        readableColors[y].G = (byte)stream.ReadByte();
                        readableColors[y].B = (byte)stream.ReadByte();
                    }
                }
                loadSpecialColors = true;
            }

            if (loadSpecialColors == true)
            {
                ColorPalette pal = _bitmapEditMemory.Palette;
                //Debug.Print(_bitmapEditMemory.Palette.Entries.Length.ToString() + "(2)");
                for (int y = 0; y < 255; ++y)
                {
                    //if (readableColors[y].R != 255 && readableColors[y].G != 0 && readableColors[y].B != 255)
                    //{
                    pal.Entries[y] = SystemColor.FromArgb(readableColors[y].R, readableColors[y].G, readableColors[y].B);
                    //}
                }
                _bitmapEditMemory.Palette = pal;
            }
            _bitmap = _bitmapEditMemory;
            _bitmap_selected = _bitmapEditMemory.Clone(new Rectangle(0, 0, _bitmap.Width, _bitmap.Height), PixelFormat.Format32bppArgb);

            ColorImage(ref _bitmap_selected);
        }

        private Bitmap CropImage(Bitmap source, Rectangle section)
        {
            // An empty bitmap which will hold the cropped image
            Bitmap bmp = new Bitmap(section.Width, section.Height);

            using (Graphics g = Graphics.FromImage(bmp))
            {
                // Draw the given area (section) of the source image
                // at location 0,0 on the empty bitmap (bmp)
                g.DrawImage(source, 0, 0, section, GraphicsUnit.Pixel);
            }

            return bmp;
        }

        public SFML.Graphics.Texture GetTexture()
        {
            return _bitmap_texture;
        }

        public Bitmap GetBitmap(Rectangle section, bool flipX = false, bool flipY = false, bool isSelected = false)
        {
            Bitmap bmp;
            if (isSelected)
            {
                if (_bitmap_selected_Cache.TryGetValue(new Tuple<Rectangle, bool, bool>(section, flipX, flipY), out bmp)) return bmp;
                GetSelectedBitmap();
            }
            else
            {
                if (_bitmapCache.TryGetValue(new Tuple<Rectangle, bool, bool>(section, flipX, flipY), out bmp)) return bmp;
                GetNormalBitmap();
            }
            return bmp;

            void GetSelectedBitmap()
            {
                bmp = CropImage(_bitmap_selected, section);
                if (flipX)
                {
                    bmp.RotateFlip(RotateFlipType.RotateNoneFlipX);
                }
                if (flipY)
                {
                    bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
                }

                _bitmap_selected_Cache[new Tuple<Rectangle, bool, bool>(section, flipX, flipY)] = bmp;
            }

            void GetNormalBitmap()
            {
                bmp = CropImage(_bitmap, section);
                if (flipX)
                {
                    bmp.RotateFlip(RotateFlipType.RotateNoneFlipX);
                }
                if (flipY)
                {
                    bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
                }

                _bitmapCache[new Tuple<Rectangle, bool, bool>(section, flipX, flipY)] = bmp;
            }
        }

        public Bitmap ToBitmap()
        {
            return _bitmap;
        }

        public void Dispose()
        {
            ReleaseResources();
        }

        public void DisposeTextures()
        {
            if (null == _texturesCache) return;
            foreach (var texture in _texturesCache.Values)
                texture?.Dispose();
            _texturesCache.Clear();
        }

        public void Reload(string encoreColors = null)
        {
            if (!File.Exists(_bitmapFilename))
            {
                throw new FileNotFoundException(string.Format("Could not find the file {0}", _bitmapFilename),
                                                _bitmapFilename);
            }
            ReleaseResources();
            _bitmap = new Bitmap(_bitmapFilename);
            _bitmap_selected = _bitmap.Clone(new Rectangle(0, 0, _bitmap.Width, _bitmap.Height), PixelFormat.Format32bppArgb);

            ColorImage(ref _bitmap_selected);

            if (encoreColors != null)
            {
                LoadEncoreColors(encoreColors);
            }

            if (_bitmap.Palette != null && _bitmap.Palette.Entries.Length > 0)
            {
                _bitmap.MakeTransparent(_bitmap.Palette.Entries[0]);
            }
            else
            {
                _bitmap.MakeTransparent(SystemColor.FromArgb(0xff00ff));
            }

        }

        private void ReleaseResources()
        {
            _bitmap.Dispose();
            _bitmap_selected.Dispose();
            DisposeTextures();
            foreach (Bitmap b in _bitmapCache.Values)
                b?.Dispose();
            foreach (Bitmap b in _bitmap_selected_Cache.Values)
                b?.Dispose();
            _bitmapCache.Clear();
            _bitmap_selected_Cache.Clear();
        }

        public Methods.Draw.GIF Clone()
        {
            return new Methods.Draw.GIF(_bitmapFilename);
        }
    }



}