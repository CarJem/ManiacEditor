using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkiaSharp;
using System.Drawing;

namespace ManiacEditor.Controls.Base.Render
{
    public class DevicePanel2 : SkiaSharp.Views.WPF.SKElement, IDrawable
    {
        public bool mouseMoved = false;

        public int DrawWidth;
        public int DrawHeight;

        public int ScreenPosWidth;
        public int ScreenPosHeight;

        SKBitmap tx;
        Bitmap txb;
        SKBitmap hvcursor;
        Bitmap hvcursorb;
        SKBitmap vcursor;
        Bitmap vcursorb;
        SKBitmap hcursor;
        Bitmap hcursorb;

        public IDrawArea _parent = null;

        public Rectangle GetScreen()
        {
            Rectangle screen = _parent.GetScreen();
            double zoom = _parent.GetZoom();
            if (zoom == 1.0)
                return screen;
            else
                return new Rectangle((int)Math.Floor(screen.X / zoom),
                    (int)Math.Floor(screen.Y / zoom),
                    (int)Math.Ceiling(screen.Width / zoom),
                    (int)Math.Ceiling(screen.Height / zoom));
        }

        public bool IsObjectOnScreen(int x, int y, int width, int height)
        {
            Rectangle screen = _parent.GetScreen();
            double zoom = _parent.GetZoom();
            if (zoom == 1.0)
                return !(x > screen.X + screen.Width
                || x + width < screen.X
                || y > screen.Y + screen.Height
                || y + height < screen.Y);
            else
                return !(x * zoom > screen.X + screen.Width
                || (x + width) * zoom < screen.X
                || y * zoom > screen.Y + screen.Height
                || (y + height) * zoom < screen.Y);
        }

        public void Render(SkiaSharp.Views.Desktop.SKPaintSurfaceEventArgs e)
        {




        }
    }
}
