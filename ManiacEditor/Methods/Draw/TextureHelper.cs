using System.IO;
using System.Drawing;
using SFML.Graphics;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace ManiacEditor.Methods.Draw
{
    public class TextureHelper
    {
        
        public static SFML.Graphics.Texture FromBitmap(Bitmap bitmap)
        {
            return new Texture(ToSFMLImage(bitmap));
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
    }
}
