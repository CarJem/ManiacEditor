using System.IO;
using System.Drawing;
using SFML.Graphics;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace ManiacEditor.Methods.Draw
{
    public class TextureHelper
    {        
        public static SFML.Graphics.Texture FromBitmap(Bitmap bitmap)
        {                    
            MemoryStream stm = new MemoryStream();
            bitmap.Save(stm, System.Drawing.Imaging.ImageFormat.Png);
            return new Texture(stm);            
        }
        public static SFML.Graphics.Texture FromBitmap2(Bitmap bitmap)
        {
            return new Texture(GetRGBValues(bitmap));
        }
        public static Texture FromBitmap3(Bitmap bitmap)
        {
            byte[] array = new byte[bitmap.Width * bitmap.Height * 4];
            int i = 0;
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    var px = bitmap.GetPixel(x, y);
                    array[i] = px.R;
                    array[i + 1] = px.G;
                    array[i + 2] = px.B;
                    array[i + 3] = px.A;
                    i += 4;
                }
            }
            var tx = new Texture((uint)bitmap.Width, (uint)bitmap.Height);
            tx.Update(array);
            return tx;
        }

        private static SFML.Graphics.Image ToSFMLImage(System.Drawing.Bitmap bmp)
        {
            SFML.Graphics.Color[,] sfmlcolorarray = new SFML.Graphics.Color[bmp.Height, bmp.Width];
            SFML.Graphics.Image newimage = null;
            for (int x = 0; x < bmp.Width; x++)
            {
                for (int y = 0; y < bmp.Height; y++)
                {
                    System.Drawing.Color csharpcolor = bmp.GetPixel(x, y);
                    sfmlcolorarray[y, x] = new SFML.Graphics.Color(csharpcolor.R, csharpcolor.G, csharpcolor.B, csharpcolor.A);
                }
            }
            newimage = new SFML.Graphics.Image(sfmlcolorarray);
            return newimage;
        }

        public static Texture FromBitmap4(Bitmap bitmap)
        {
            BitmapData bd = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            int bufferSize = bd.Height * bd.Stride;

            //create data buffer 
            byte[] bytes = new byte[bufferSize];

            // copy bitmap data into buffer
            Marshal.Copy(bd.Scan0, bytes, 0, bytes.Length);

            bitmap.UnlockBits(bd);
            return new Texture(bytes);
        }

        public static Texture FromBitmap5(Bitmap bitmap)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
                memoryStream.Seek(0, SeekOrigin.Begin);
                return new Texture(memoryStream);
            }
        }

        private static byte[] GetRGBValues(System.Drawing.Bitmap bmp)
        {
            // Lock the bitmap's bits. 
            System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height);
            System.Drawing.Imaging.BitmapData bmpData = bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly, bmp.PixelFormat);

            // Get the address of the first line.
            IntPtr ptr = bmpData.Scan0;

            // Declare an array to hold the bytes of the bitmap.
            int bytes = bmpData.Stride * bmp.Height;
            byte[] rgbValues = new byte[bytes];

            // Copy the RGB values into the array.
            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);
            bmp.UnlockBits(bmpData);

            return rgbValues;
        }


        private static byte[] ImageToByteArray(System.Drawing.Image resourceimage)
        {
            System.Drawing.ImageConverter imgcon = new System.Drawing.ImageConverter();
            return (byte[])imgcon.ConvertTo(resourceimage, typeof(byte[]));
        }

        private static byte[] MarshalImageToByteArray(System.Drawing.Bitmap bmp)
        {
            // Lock the bitmap's bits.  
            System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height);
            System.Drawing.Imaging.BitmapData bmpData = bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat);

            // Get the address of the first line.
            IntPtr ptr = bmpData.Scan0;

            // Declare an array to hold the bytes of the bitmap. 
            int bytes = Math.Abs(bmpData.Stride) * bmp.Height;
            byte[] rgbValues = new byte[bytes];

            // Copy the RGB values into the array.
            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);
            bmp.UnlockBits(bmpData);
            return rgbValues;
        }
    }
}
