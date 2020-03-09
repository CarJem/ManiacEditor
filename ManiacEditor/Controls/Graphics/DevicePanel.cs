using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SharpDX;
using SharpDX.Direct3D9;
using SharpDX.Windows;
using SFML.Graphics;
using SFML.Window;
using Font = SharpDX.Direct3D9.Font;
using Rectangle = System.Drawing.Rectangle;
using Color = System.Drawing.Color;
using Point = System.Drawing.Point;
using Bitmap = System.Drawing.Bitmap;
using ManiacEditor.EventHandlers;
using System.Diagnostics;
using Texture = SFML.Graphics.Texture;


namespace ManiacEditor
{
    public partial class DevicePanel : UserControl
    {

        #region Members

        #region SFML Variables

        // On this event we can start to render our scene
        public event RenderEventHandler OnRender;

        // Now we know that the device is created
        public event CreateDeviceEventHandler OnCreateDevice;

        public SFML.Graphics.RenderWindow RenderWindow { get; set; }

        public IDrawArea _parent = null;
        #endregion

        #region FPS Variables
        public double FPS { get; set; } = 0.0;
        System.Diagnostics.Stopwatch FPS_Clock { get; set; }
        double TotalTime_FPS { get; set; }
        long FrameCount_FPS { get; set; }

        #endregion

        #region Properties

        /// <summary>
        /// Extend this list of properties if you like
        /// </summary>
        private Color _deviceBackColor = Color.Black;

        public Color DeviceBackColor
        {
            get { return _deviceBackColor; }
            set { _deviceBackColor = value; }
        }


        #endregion

        #region Other Variables
        public bool MouseMoved { get; set; } = false;
        public MouseEventArgs LastEvent { get; set; }
        public bool AllowLoopToRender { get; set; } = true;
        #endregion

        #endregion

        #region Constructor

        public DevicePanel(Controls.Editor.MainEditor instance = null)
        {
            InitializeComponent();
        }

        #endregion

        #region Init

        private void CreateRenderWindow()
        {
            if (this.RenderWindow != null)
            {
                this.RenderWindow.SetActive(false);
                this.RenderWindow.Dispose();
            }

            var context = new ContextSettings {  };
            this.RenderWindow = new RenderWindow(this.Handle, context);

            RenderWindow.SetFramerateLimit(120);

            this.RenderWindow.SetActive(true);
        }

        /// <summary>
        /// Init SFML Render
        /// </summary>
        /// <param name="parent">parent of the DevicePanel</param>
        public void Init(IDrawArea parent)
        {
            _parent = parent;
            CreateRenderWindow();
            InitDeviceResources();

            if (OnCreateDevice != null)
            {
                OnCreateDevice(this, new DeviceEventArgs(RenderWindow));
            }

        }

        public void Run()
        {
            RenderLoop.Run(this, () =>
            {
                // Another option is not use RenderLoop at all and call Render when needed, and call here every tick for animations
                if (AllowLoopToRender) Render();
                if (MouseMoved)
                {
                    OnMouseMove(LastEvent);
                    MouseMoved = false;
                }
                Application.DoEvents();
            });
        }

        public void InitDeviceResources()
        {
            FPS_Clock = Stopwatch.StartNew();
        }

        #endregion

        #region Reset (Unused)

        /// <summary>
        /// Attempt to recover the device if it is lost.
        /// </summary>
        protected void AttemptRecovery()
        {

        }

        public void ResetDevice()
        {

        }

        #endregion

        #region Rendering

        /// <summary>
        /// The New Rendering-method
        /// </summary>
        /// 
        public void Render()
        {
            RenderWindow.DispatchEvents();
            RenderWindow.Clear(new SFML.Graphics.Color(DeviceBackColor.R, DeviceBackColor.G, DeviceBackColor.B));
            RenderWindow.GetView().Zoom((float)_parent.GetZoom());

            // Render of Scene Here
            if (OnRender != null) OnRender(this, new DeviceEventArgs(RenderWindow));

            RenderWindow.Display();
            UpdateFPS();
        }
        private void UpdateFPS()
        {
            FrameCount_FPS++;
            var timeElapsed = (double)FPS_Clock.ElapsedTicks / Stopwatch.Frequency; ;
            TotalTime_FPS += timeElapsed;
            if (TotalTime_FPS >= 1.0f)
            {
                FPS = (double)FrameCount_FPS / TotalTime_FPS;
                FrameCount_FPS = 0;
                TotalTime_FPS = 0.0;
            }

            FPS_Clock.Restart();

        }

        #endregion

        #region Event/Overrides
        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            base.OnPaint(e);
        }

        protected override void OnPaintBackground(System.Windows.Forms.PaintEventArgs e)
        {
            base.OnPaintBackground(e);
        }
        protected override bool IsInputKey(Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Up: return true;
                case Keys.Down: return true;
                case Keys.Right: return true;
                case Keys.Left: return true;
                case Keys.PageUp: return true;
                case Keys.PageDown: return true;
            }
            return base.IsInputKey(keyData);
        }
        protected override Point ScrollToControl(Control activeControl)
        {
            return this.AutoScrollPosition;
        }
        protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            if (_parent != null)
            {
                LastEvent = e;
                var screen = _parent.GetScreen();
                base.OnMouseMove(new MouseEventArgs(e.Button, e.Clicks, e.X + screen.X, e.Y + screen.Y, e.Delta));
            }
        }
        protected override void OnMouseWheel(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseWheel(new MouseEventArgs(e.Button, e.Clicks, e.X + GetParentScreen().X, e.Y + GetParentScreen().Y, e.Delta));
        }
        protected override void OnMouseClick(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseClick(new MouseEventArgs(e.Button, e.Clicks, e.X + GetParentScreen().X, e.Y + GetParentScreen().Y, e.Delta));
        }
        protected override void OnMouseDoubleClick(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseDoubleClick(new MouseEventArgs(e.Button, e.Clicks, e.X + GetParentScreen().X, e.Y + GetParentScreen().Y, e.Delta));
        }
        protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseDown(new MouseEventArgs(e.Button, e.Clicks, e.X + GetParentScreen().X, e.Y + GetParentScreen().Y, e.Delta));
        }
        protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseUp(new MouseEventArgs(e.Button, e.Clicks, e.X + GetParentScreen().X, e.Y + GetParentScreen().Y, e.Delta));
        }
        public void OnMouseMoveEventCreate()
        {
            System.Windows.Forms.Cursor.Position = new Point(System.Windows.Forms.Cursor.Position.X, System.Windows.Forms.Cursor.Position.Y);
        }

        #endregion

        #region Get Screen
        public Rectangle GetParentScreen()
        {
            if (_parent == null) return new Rectangle(0, 0, 10, 10);
            else return _parent.GetScreen();
        }
        public Rectangle GetScreen()
        {
            if (_parent == null) return new Rectangle(0, 0, 10, 10);
            Rectangle screen = GetParentScreen();
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
            if (_parent == null) return false;
            Rectangle screen = GetParentScreen();
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

        #endregion

        #region Draw Methods
        public void DrawTexture(SFML.Graphics.Texture image, int x, int y, int width, int height, bool selected, int transparency)
        {
            DrawTexture(image, x, y, 0, 0, width, height, selected, transparency);
        }
        public void DrawTexture(SFML.Graphics.Texture image, int x, int y, int rect_x, int rect_y, int width, int height, bool selected, int transparency, bool fliph = false, bool flipv = false, int rotation = 0)
        {
            Rectangle screen = GetParentScreen();
            double zoom = _parent.GetZoom();

            Rectangle boundBox = new Rectangle(rect_x, rect_y, width, height);

            int real_x = x - (int)(screen.X / zoom);
            int real_y = y - (int)(screen.Y / zoom);

            SFML.System.Vector2f position = new SFML.System.Vector2f(real_x, real_y);
            SFML.System.Vector2f size = new SFML.System.Vector2f(width, height);

            Vector3 center = new Vector3(new float[] { 0, 0, 0 });
            if (fliph || flipv)
            {
                if (fliph) position.X = -position.X;
                if (flipv) position.Y = -position.Y;

                if (fliph) center.X = -boundBox.Width;
                if (flipv) center.Y = -boundBox.Height;

                float negatedZoom = -(float)zoom;
                float normalZoom = (float)zoom;
            }


            SFML.Graphics.RectangleShape rect = new SFML.Graphics.RectangleShape();
            rect.Position = position;
            rect.Size = size;
            rect.Texture = image;
            rect.TextureRect = new IntRect(rect_x, rect_y, width, height);
            RenderWindow.Draw(rect);
        }
        public void DrawLine(int X1, int Y1, int X2, int Y2, Color color = new Color(), bool useZoomOffseting = false)
        {
            Rectangle screen = GetParentScreen();
            double zoom = _parent.GetZoom();

            int real_x1 = X1 - (int)(screen.X / zoom);
            int real_x2 = X2 - (int)(screen.X / zoom);
            int real_y1 = Y1 - (int)(screen.Y / zoom);
            int real_y2 = Y2 - (int)(screen.Y / zoom);


            SFML.Graphics.VertexArray line = new SFML.Graphics.VertexArray();
            line.PrimitiveType = SFML.Graphics.PrimitiveType.Lines;
            line.Append(new Vertex(new SFML.System.Vector2f(real_x1, real_y1), new SFML.Graphics.Color(color.R, color.G, color.B, color.A)));
            line.Append(new Vertex(new SFML.System.Vector2f(real_x2, real_y2), new SFML.Graphics.Color(color.R, color.G, color.B, color.A)));
            RenderWindow.Draw(line);
        }
        public void DrawLinePaperRoller(int X1, int Y1, int X2, int Y2, Color color, Color color2, Color color3, Color color4)
        {
            Rectangle screen = GetParentScreen();
            double zoom = _parent.GetZoom();
            int width = Math.Abs(X2 - X1);
            int height = Math.Abs(Y2 - Y1);
            int x = Math.Min(X1, X2);
            int y = Math.Min(Y1, Y2);
            int pixel_width = Math.Max((int)zoom, 1);

            if (!IsObjectOnScreen(x, y, width, height)) return;


            /*
            if (width == 0 || height == 0)
            {
                if (width == 0) width = pixel_width;
                else width = (int)(width * zoom);
                if (height == 0) height = pixel_width;
                else height = (int)(height * zoom);
                DrawTexture(tx, new Rectangle(0, 0, width, height), new Vector3(0, 0, 0), new Vector3((int)((x - (int)(screen.X / zoom)) * zoom), (int)((y - (int)(screen.Y / zoom)) * zoom), 0), color);
            }
            else
            {*/
            DrawLinePBPDoted(X1, Y1, X2, Y2, color, color2, color3, color4);
            //}
        }
        public void DrawArrow(int x0, int y0, int x1, int y1, Color color)
        {
            int x2, y2, x3, y3;

            double angle = Math.Atan2(y1 - y0, x1 - x0) + Math.PI;

            x2 = (int)(x1 + 10 * Math.Cos(angle - Math.PI / 8));
            y2 = (int)(y1 + 10 * Math.Sin(angle - Math.PI / 8));
            x3 = (int)(x1 + 10 * Math.Cos(angle + Math.PI / 8));
            y3 = (int)(y1 + 10 * Math.Sin(angle + Math.PI / 8));

            DrawLine(x1, y1, x0, y0, color);
            DrawLine(x1, y1, x2, y2, color);
            DrawLine(x1, y1, x3, y3, color);
        }
        public void DrawBézierSplineCubic(int x1, int y1, int x2, int y2, int x3, int y3, int x4, int y4, Color color)
        {
            for (double i = 0; i < 1; i += 0.01)
            {
                // The Green Lines
                int xa = getPt(x1, x2, i);
                int ya = getPt(y1, y2, i);
                int xb = getPt(x2, x3, i);
                int yb = getPt(y2, y3, i);
                int xc = getPt(x3, x4, i);
                int yc = getPt(y3, y4, i);

                // The Blue Line
                int xm = getPt(xa, xb, i);
                int ym = getPt(ya, yb, i);
                int xn = getPt(xb, xc, i);
                int yn = getPt(yb, yc, i);

                // The Black Dot
                int x = getPt(xm, xn, i);
                int y = getPt(ym, yn, i);

                DrawLinePBP(x, y, x, y, color);
            }

            int getPt(int n1, int n2, double perc)
            {
                int diff = n2 - n1;

                return (int)(n1 + (diff * perc));
            }
        }
        public void DrawBézierSplineQuadratic(int x1, int y1, int x2, int y2, int x3, int y3, Color color)
        {
            for (double i = 0; i < 1; i += 0.01)
            {
                // The Green Line
                int xa = getPt(x1, x2, i);
                int ya = getPt(y1, y2, i);
                int xb = getPt(x2, x3, i);
                int yb = getPt(y2, y3, i);

                // The Black Dot
                int x = getPt(xa, xb, i);
                int y = getPt(ya, yb, i);

                DrawLinePBP(x, y, x, y, color);
            }


            int getPt(int n1, int n2, double perc)
            {
                int diff = n2 - n1;

                return (int)(n1 + (diff * perc));
            }
        }
        public void DrawLinePBP(int x0, int y0, int x1, int y1, Color color)
        {
            /*
            Rectangle screen = GetParentScreen();
            double zoom = _parent.GetZoom();
            int dx, dy, inx, iny, e;
            int pixel_width = (int)Math.Ceiling(zoom);

            dx = x1 - x0;
            dy = y1 - y0;
            inx = dx > 0 ? 1 : -1;
            iny = dy > 0 ? 1 : -1;

            dx = Math.Abs(dx);
            dy = Math.Abs(dy);

            if (dx >= dy)
            {
                dy <<= 1;
                e = dy - dx;
                dx <<= 1;
                while (x0 != x1)
                {
                    DrawTexture(tx, new Rectangle(0, 0, pixel_width, pixel_width), new Vector3(0, 0, 0), new Vector3((int)((x0 - (int)(screen.X / zoom)) * zoom), (int)((y0 - (int)(screen.Y / zoom)) * zoom), 0), color);
                    if (e >= 0)
                    {
                        y0 += iny;
                        e -= dx;
                    }
                    e += dy; x0 += inx;
                }
            }
            else
            {
                dx <<= 1;
                e = dx - dy;
                dy <<= 1;
                while (y0 != y1)
                {
                    DrawTexture(tx, new Rectangle(0, 0, pixel_width, pixel_width), new Vector3(0, 0, 0), new Vector3((int)((x0 - (int)(screen.X / zoom)) * zoom), (int)((y0 - (int)(screen.Y / zoom)) * zoom), 0), color);
                    if (e >= 0)
                    {
                        x0 += inx;
                        e -= dy;
                    }
                    e += dx; y0 += iny;
                }
            }
            DrawTexture(tx, new Rectangle(0, 0, pixel_width, pixel_width), new Vector3(0, 0, 0), new Vector3((int)((x0 - (int)(screen.X / zoom)) * zoom), (int)((y0 - (int)(screen.Y / zoom)) * zoom), 0), color);
            */
        }
        public void DrawLinePBPDoted(int x0, int y0, int x1, int y1, Color color, Color color2, Color color3, Color color4)
        {
            /*
            Rectangle screen = GetParentScreen();
            double zoom = _parent.GetZoom();
            int dx, dy, inx, iny, e;
            int pixel_width = (int)(Math.Ceiling(zoom + 0.3));

            dx = x1 - x0;
            dy = y1 - y0;
            inx = dx > 0 ? 1 : -1;
            iny = dy > 0 ? 1 : -1;

            dx = Math.Abs(dx);
            dy = Math.Abs(dy);

            Color currentColor = color;
            int iterations = 0;

            if (dx >= dy)
            {
                dy <<= 1;
                e = dy - dx;
                dx <<= 1;
                while (x0 != x1)
                {
                    if (iterations >= 5)
                    {
                        if (currentColor == color4) currentColor = color;
                        else if (currentColor == color3) currentColor = color4;
                        else if (currentColor == color2) currentColor = color3;
                        else if (currentColor == color) currentColor = color2;

                        iterations = 0;
                    }


                    DrawTexture(tx, new Rectangle(0, 0, pixel_width, pixel_width), new Vector3(0, 0, 0), new Vector3((int)((x0 - (int)(screen.X / zoom)) * zoom), (int)((y0 - (int)(screen.Y / zoom)) * zoom), 0), currentColor);
                    if (e >= 0)
                    {
                        y0 += iny;
                        e -= dx;
                    }
                    e += dy; x0 += inx; iterations++;

                }
            }
            else
            {
                dx <<= 1;
                e = dx - dy;
                dy <<= 1;
                while (y0 != y1)
                {
                    if (iterations >= 5)
                    {
                        if (currentColor == color4) currentColor = color;
                        else if (currentColor == color3) currentColor = color4;
                        else if (currentColor == color2) currentColor = color3;
                        else if (currentColor == color) currentColor = color2;

                        iterations = 0;
                    }

                    DrawTexture(tx, new Rectangle(0, 0, pixel_width, pixel_width), new Vector3(0, 0, 0), new Vector3((int)((x0 - (int)(screen.X / zoom)) * zoom), (int)((y0 - (int)(screen.Y / zoom)) * zoom), 0), currentColor);
                    if (e >= 0)
                    {
                        x0 += inx;
                        e -= dy;
                    }
                    e += dx; y0 += iny; iterations++;
                }
            }
            DrawTexture(tx, new Rectangle(0, 0, pixel_width, pixel_width), new Vector3(0, 0, 0), new Vector3((int)((x0 - (int)(screen.X / zoom)) * zoom), (int)((y0 - (int)(screen.Y / zoom)) * zoom), 0), color);
            */
        }
        public void DrawRectangle(int x1, int y1, int x2, int y2, Color color)
        {
            Rectangle screen = GetParentScreen();
            double zoom = _parent.GetZoom();

            int real_x1 = x1 - (int)(screen.X / zoom);
            int real_x2 = x2 - (int)(screen.X / zoom);
            int real_y1 = y1 - (int)(screen.Y / zoom);
            int real_y2 = y2 - (int)(screen.Y / zoom);

            SFML.System.Vector2f position = new SFML.System.Vector2f(real_x1, real_y1);
            SFML.System.Vector2f size = new SFML.System.Vector2f(real_x2 - real_x1, real_y2 - real_y1);

            SFML.Graphics.RectangleShape rect = new SFML.Graphics.RectangleShape();
            rect.FillColor = new SFML.Graphics.Color(color.R, color.G, color.B, color.A);
            rect.Position = position;
            rect.Size = size;
            RenderWindow.Draw(rect);
        }
        public void DrawQuad(int x1, int y1, int x2, int y2, Color color)
        {
            /*
            if (!IsObjectOnScreen(x1, y1, x2 - x1, y2 - y1)) return;
            Rectangle screen = GetParentScreen();
            double zoom = _parent.GetZoom();

            DrawTexture(tx, new Rectangle(0, 0, x2 - x1, y2 - y1), new Vector3(0, 0, 0), new Vector3(x1 - (int)(screen.X / zoom), y1 - (int)(screen.Y / zoom), 0), color);
            */
        }
        public void DrawText(string text, int x, int y, int width, Color color, bool bold)
        {
            /*
           Rectangle screen = GetParentScreen();
           double zoom = _parent.GetZoom();
           if (width >= 30)
           {
               ((bold) ? fontBold : font).DrawText(sprite, text, new SharpDX.Rectangle(x - (int)(screen.X / zoom), y - (int)(screen.Y / zoom), width, 1000), FontDrawFlags.WordBreak, new SharpDX.Color(color.R, color.G, color.B, color.A));
           }
           else
           {
               ((bold) ? fontBold : font).DrawText(sprite, text, x - (int)(screen.X / zoom), y - (int)(screen.Y / zoom), new SharpDX.Color(color.R, color.G, color.B, color.A));
           }
           */
        }
        public void DrawTextSmall(string text, int x, int y, int width, Color color, bool bold)
        {
            /*
            Rectangle screen = GetParentScreen();
            double zoom = _parent.GetZoom();

            sprite.Transform = Matrix.Scaling((float)zoom / 4, (float)zoom / 4, 1f);
            if (width >= 10)
            {
                ((bold) ? fontBold : font).DrawText(sprite, text, new SharpDX.Rectangle((x - (int)(screen.X / zoom)) * 4, (y - (int)(screen.Y / zoom)) * 4, width * 4, 1000), FontDrawFlags.WordBreak, new SharpDX.Color(color.R, color.G, color.B, color.A));
            }
            else
            {
                ((bold) ? fontBold : font).DrawText(sprite, text, (x - (int)(screen.X / zoom)) * 4, (y - (int)(screen.Y / zoom)) * 4, new SharpDX.Color(color.R, color.G, color.B, color.A));
            }
            sprite.Transform = Matrix.Scaling((float)zoom, (float)zoom, 1f);
            */
        }
        public void Draw2DCursor(int x, int y)
        {


        }
        public void DrawHorizCursor(int x, int y)
        {

        }
        public void DrawVertCursor(int x, int y)
        {

        }

        #endregion

        #region Disposing

        public void DisposeDeviceResources()
        {

        }
        public new void Dispose()
        {
            DisposeDeviceResources();
            if (_parent != null) _parent.DisposeTextures();
            if (RenderWindow != null) RenderWindow.Dispose();
            base.Dispose();
        }

        #endregion


    }
}
