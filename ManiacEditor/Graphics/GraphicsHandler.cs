using System;
using System.Drawing;

namespace ManiacEditor
{
    public class GraphicsHandler
    {
        public DevicePanel DevicePanel;
        public System.Drawing.Graphics SystemGraphics;

        public class GraphicsInfo
        {
            public EditorEntityDrawing.EditorAnimation.EditorFrame EntityFrame;
            public SharpDX.Direct3D9.Texture Texture;


            public RenderType ObjectType;
            public enum RenderType
            {
                EditorFrame,
                Texture

            }

            public GraphicsInfo(EditorEntityDrawing.EditorAnimation.EditorFrame frame)
            {
                EntityFrame = frame;
                ObjectType = RenderType.EditorFrame;
            }

            public GraphicsInfo(SharpDX.Direct3D9.Texture _texture)
            {
                Texture = _texture;
                ObjectType = RenderType.Texture;

            }
        }

        public GraphicsType GraphicsMode;
        public enum GraphicsType
        {
            SharpDX,
            System

        }
        public GraphicsHandler(DevicePanel d)
        {
            GraphicsMode = GraphicsType.SharpDX;
            DevicePanel = d;
        }

        public GraphicsHandler(System.Drawing.Graphics g)
        {
            GraphicsMode = GraphicsType.System;
            SystemGraphics = g;
        }


        public void DrawBitmap(GraphicsInfo info, int x, int y, int width, int height, bool selected, int transparency, System.Drawing.Color? CustomColor = null)
        {
            if (GraphicsMode == GraphicsType.SharpDX)
            {
                if (info.ObjectType == GraphicsInfo.RenderType.Texture) DevicePanel.DrawBitmap(info.Texture, x, y, width, height, selected, transparency, CustomColor);
                else if (info.ObjectType == GraphicsInfo.RenderType.EditorFrame) DevicePanel.DrawBitmap(info.EntityFrame.Texture, x, y, width, height, selected, transparency, CustomColor);
            }
            else if (GraphicsMode == GraphicsType.System)
            {
                if (info.ObjectType == GraphicsInfo.RenderType.EditorFrame)
                {
                    if (info.EntityFrame._Bitmap != null) SystemGraphics.DrawImage(info.EntityFrame._Bitmap, x, y);
                }
            }
        }

        public void DrawBitmap(SharpDX.Direct3D9.Texture image, int x, int y, int width, int height, bool selected, int transparency, System.Drawing.Color? CustomColor = null)
        {
            if (GraphicsMode == GraphicsType.SharpDX)
            {
                DevicePanel.DrawBitmap(image, x, y, width, height, selected, transparency, CustomColor);
            }
        }

        public void DrawHUDRectangle(int x1, int y1, int x2, int y2, System.Drawing.Color color)
        {
            if (GraphicsMode == GraphicsType.SharpDX)
            {
                DevicePanel.DrawHUDRectangle( x1,  y1,  x2,  y2, color);
            }
        }

        public void DrawHUDBitmap(SharpDX.Direct3D9.Texture image, int x, int y, int width, int height, bool selected, int transparency)
        {
            if (GraphicsMode == GraphicsType.SharpDX)
            {
                DevicePanel.DrawHUDBitmap(image, x, y, width, height, selected, transparency);
            }
        }

        public void DrawLine(int X1, int Y1, int X2, int Y2, System.Drawing.Color color = new System.Drawing.Color(), bool useZoomOffseting = false)
        {
            if (GraphicsMode == GraphicsType.SharpDX)
            {
                DevicePanel.DrawLine(X1, Y1, X2, Y2, color, useZoomOffseting);
            }
            else if (GraphicsMode == GraphicsType.System)
            {
                SystemGraphics.DrawLine(new Pen(color), X1, Y1, X2, Y2);
            }
        }

        public void DrawRectangle(int x1, int y1, int x2, int y2, Color color)
        {
            if (GraphicsMode == GraphicsType.SharpDX)
            {
                DevicePanel.DrawRectangle(x1, y1, x2, y2, color);
            }
            else if (GraphicsMode == GraphicsType.System)
            {
                SystemGraphics.DrawRectangle(new Pen(color), x1, y2, x2, y2);
            }
        }

        public void DrawQuad(int x1, int y1, int x2, int y2, Color color)
        {
            if (GraphicsMode == GraphicsType.SharpDX)
            {
                DevicePanel.DrawQuad(x1, y1, x2, y2, color);
            }
        }

        public void DrawText(string text, int x, int y, int width, Color color, bool bold)
        {
            if (GraphicsMode == GraphicsType.SharpDX)
            {
                DevicePanel.DrawText(text, x, y, width, color, bold);
            }
            else if (GraphicsMode == GraphicsType.System)
            {
                Brush brush = new SolidBrush(color);
                FontFamily family = new FontFamily(System.Drawing.Text.GenericFontFamilies.SansSerif);
                Font font = new Font(family, 18, (bold ? FontStyle.Bold : FontStyle.Regular));
                SystemGraphics.DrawString(text, font, brush, x, y);
            }
        }
        public void DrawTextSmall(string text, int x, int y, int width, Color color, bool bold)
        {
            if (GraphicsMode == GraphicsType.SharpDX)
            {
               DevicePanel.DrawTextSmall(text, x, y, width, color, bold);
            }
        }
        public void Draw2DCursor(int x, int y)
        {
            if (GraphicsMode == GraphicsType.SharpDX)
            {
                Draw2DCursor(x, y);
            }
        }
        public void DrawHorizCursor(int x, int y)
        {
            if (GraphicsMode == GraphicsType.SharpDX)
            {
                DrawHorizCursor(x, y);
            }
        }

        public void DrawVertCursor(int x, int y)
        {
            if (GraphicsMode == GraphicsType.SharpDX)
            {
                DrawVertCursor(x, y);
            }
        }

        public bool IsObjectOnScreen(int x, int y, int width, int height)
        {
            if (GraphicsMode == GraphicsType.SharpDX)
            {
                return DevicePanel.IsObjectOnScreen(x, y, width, height);
            }
            else if (GraphicsMode == GraphicsType.System)
            {
                return true;
            }
            else return false;
        }

        public void DrawLinePaperRoller(int X1, int Y1, int X2, int Y2, Color color, Color color2, Color color3, Color color4)
        {
            if (GraphicsMode == GraphicsType.SharpDX)
            {
                DevicePanel.DrawLinePaperRoller(X1, Y1, X2, Y2, color, color2, color3, color4);
            }
            else if (GraphicsMode == GraphicsType.System)
            {
                //TODO: Improve
                Pen pen = new Pen(color);
                SystemGraphics.DrawLine(pen, X1, Y1, X2, Y2);
            }
        }

        public void DrawArrow(int x0, int y0, int x1, int y1, Color color)
        {
            if (GraphicsMode == GraphicsType.SharpDX)
            {
                DevicePanel.DrawArrow(x0, y0, x1, y1, color);
            }
            else if (GraphicsMode == GraphicsType.System)
            {
                int x2, y2, x3, y3;

                double angle = Math.Atan2(y1 - y0, x1 - x0) + Math.PI;

                x2 = (int)(x1 + 10 * Math.Cos(angle - Math.PI / 8));
                y2 = (int)(y1 + 10 * Math.Sin(angle - Math.PI / 8));
                x3 = (int)(x1 + 10 * Math.Cos(angle + Math.PI / 8));
                y3 = (int)(y1 + 10 * Math.Sin(angle + Math.PI / 8));

                SystemGraphics.DrawLine(new Pen(color), x1, y1, x0, y0);
                SystemGraphics.DrawLine(new Pen(color), x1, y1, x2, y2);
                SystemGraphics.DrawLine(new Pen(color), x1, y1, x3, y3);
            }
        }


    }
}
