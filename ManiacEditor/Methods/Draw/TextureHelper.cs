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
            MemoryStream stm = new MemoryStream();
            bitmap.Save(stm, System.Drawing.Imaging.ImageFormat.Png);
            return new Texture(stm);
        }
    }
}
