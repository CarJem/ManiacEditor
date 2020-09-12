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
using ManiacEditor.Events;
using System.Diagnostics;
using Texture = SFML.Graphics.Texture;
using System.Collections.Generic;
using ManiacEditor.Extensions;
using ManiacEditor.Methods.Drawing;
using System.IO;

namespace ManiacEditor
{
    public partial class DevicePanelSFML : UserControl
    {
        public static SFML.Graphics.Texture FromBitmap(Bitmap bitmap)
        {
            MemoryStream stm = new MemoryStream();
            bitmap.Save(stm, System.Drawing.Imaging.ImageFormat.Png);
            return new SFML.Graphics.Texture(stm);
        }

        #region Members

        #region SFML Variables

        // On this event we can start to render our scene
        public event RenderEventHandlerSFML OnRender;

        // Now we know that the device is created
        public event CreateDeviceEventHandlerSFML OnCreateDevice;

        public SFML.Graphics.RenderWindow RenderWindow { get; set; }

        public IDrawAreaSFML _parent = null;


        public SFML.System.Vector2i LastPosition { get; set; } = new SFML.System.Vector2i(0, 0);
        public SFML.System.Vector2f ZoomOffset { get; private set; } = new SFML.System.Vector2f(0, 0);


        SFML.Graphics.Font RenderingFont;

        SFML.Graphics.Font RenderingFontBold;

        #endregion

        #region Built-In Textures

        Texture tx;
        Bitmap txb;
        Texture tcircle;
        Texture tecircle;
        Texture hvcursor;
        Bitmap hvcursorb;
        Texture vcursor;
        Bitmap vcursorb;
        Texture hcursor;
        Bitmap hcursorb;
        Bitmap tcircleb;
        Bitmap tecircleb;

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

        private bool _CanRender = true;
        private bool CanRender
        {
            get
            {
                return _CanRender;
            }
            set
            {
                _CanRender = true;
                if (VSyncAwaiting && value == true) Render(true);
            }
        }
        private bool VSync { get; set; } = false;
        private bool VSyncAwaiting { get; set; } = false;
        #endregion

        #endregion

        #region Constructor

        public DevicePanelSFML()
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

            var context = new ContextSettings { };
            this.RenderWindow = new RenderWindow(this.Handle, context);



            //RenderWindow.SetFramerateLimit(60);

            this.RenderWindow.SetActive(true);
        }

        /// <summary>
        /// Init SFML Render
        /// </summary>
        /// <param name="parent">parent of the DevicePanel</param>
        public void Init(IDrawAreaSFML parent)
        {
            _parent = parent;
            CreateRenderWindow();
            InitDeviceResources();

            if (OnCreateDevice != null)
            {
                OnCreateDevice(this, new DeviceEventArgsSFML(RenderWindow));
            }

        }

        public void Run()
        {
            ManiacEditor.Classes.Rendering.RenderLoop.Run(this, () =>
            {
                Render();
                if (MouseMoved)
                {
                    OnMouseMove(LastEvent);
                    MouseMoved = false;
                }
            }, true);

        }

        public void InitDeviceResources()
        {
            FPS_Clock = Stopwatch.StartNew();

            //RenderingFont = new SFML.Graphics.Font(System.IO.Path.Combine(Methods.ProgramPaths.FontsDirectory, "sonic2system.ttf"));
            //RenderingFontBold = new SFML.Graphics.Font(System.IO.Path.Combine(Methods.ProgramPaths.FontsDirectory, "sonic2system.ttf"));

            txb = new Bitmap(1, 1);
            using (Graphics g = Graphics.FromImage(txb))
                g.Clear(Color.White);

            tcircleb = new Bitmap(9, 9);
            using (Graphics g = Graphics.FromImage(tcircleb))
                g.FillEllipse(Brushes.White, new Rectangle(0, 0, 9, 9));

            tecircleb = new Bitmap(14, 14);
            using (Graphics g = Graphics.FromImage(tecircleb))
                g.DrawEllipse(new Pen(Brushes.White), new Rectangle(0, 0, 13, 13));

            hcursorb = new Bitmap(32, 32);
            using (Graphics g = Graphics.FromImage(hcursorb))
                Cursors.NoMoveHoriz.Draw(g, new Rectangle(0, 0, 32, 32));
            MakeGray(hcursorb);

            vcursorb = new Bitmap(32, 32);
            using (Graphics g = Graphics.FromImage(vcursorb))
                Cursors.NoMoveVert.Draw(g, new Rectangle(0, 0, 32, 32));
            MakeGray(vcursorb);

            hvcursorb = new Bitmap(32, 32);
            using (Graphics g = Graphics.FromImage(hvcursorb))
                Cursors.NoMove2D.Draw(g, new Rectangle(0, 0, 32, 32));
            MakeGray(hvcursorb);

            tx = FromBitmap(txb);
            tcircle = FromBitmap(tcircleb);
            tecircle = FromBitmap(tecircleb);
            hcursor = FromBitmap(hcursorb);
            vcursor = FromBitmap(vcursorb);
            hvcursor = FromBitmap(hvcursorb);

            void MakeGray(Bitmap image)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    for (int y = 0; y < image.Height; y++)
                    {
                        if (image.GetPixel(x, y).Name == "ffffffff")
                            image.SetPixel(x, y, Color.Transparent);
                        else if (image.GetPixel(x, y).Name == "ff000000")
                            image.SetPixel(x, y, Color.Gray);
                    }
                }
            }
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

        #region Pause Rendering
        public void ToggleRendering(bool value)
        {
            CanRender = value;
        }

        #endregion

        #region Rendering

        /// <summary>
        /// The New Rendering-method
        /// </summary>
        /// 
        public void Render(bool isAwaited = false)
        {
            if (VSync && (!CanRender && isAwaited == false))
            {
                VSyncAwaiting = true;
                return;
            }
            else if (isAwaited)
            {
                VSyncAwaiting = false;
            }

            if (RenderWindow != null)
            {
                CanRender = true;
                RenderWindow.DispatchEvents();
                RenderWindow.Clear(new SFML.Graphics.Color(DeviceBackColor.R, DeviceBackColor.G, DeviceBackColor.B));
                var windowSize = GetWindowSize();
                RenderWindow.Size = windowSize;

                var pos = GetPosition();
                if (LastPosition != pos)
                {
                    LastPosition = pos;
                    RenderWindow.SetView(GetCurrentView());
                }

                // Render of Scene Here
                if (OnRender != null) OnRender(this, new DeviceEventArgsSFML(RenderWindow));

                UpdateFPS();
                RenderWindow.Display();
                CanRender = false;
            }
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

        public Point GetRelativeMousePosition()
        {
            var pixelPos = SFML.Window.Mouse.GetPosition(RenderWindow);
            var zoom = GetZoom();
            var mousePos = RenderWindow.MapPixelToCoords(pixelPos, RenderWindow.GetView());
            return new Point((int)(mousePos.X / zoom), (int)(mousePos.Y / zoom));
        }

        private Point GetMousePoint(System.Windows.Forms.MouseEventArgs e)
        {
            var position = GetParentScreen();
            var zoom = GetZoom();
            return new Point((int)((e.X + position.X) / zoom), (int)((e.Y + position.Y) / zoom));
        }
        protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            if (_parent != null)
            {
                LastEvent = e;
                var screen = _parent.GetScreen();
                var point = GetMousePoint(e);
                base.OnMouseMove(new MouseEventArgs(e.Button, e.Clicks, point.X, point.Y, e.Delta));
            }
        }
        protected override void OnMouseWheel(System.Windows.Forms.MouseEventArgs e)
        {
            var Zoom = GetZoom();
            var point = GetMousePoint(e);
            base.OnMouseWheel(new MouseEventArgs(e.Button, e.Clicks, point.X, point.Y, e.Delta));
        }
        protected override void OnMouseClick(System.Windows.Forms.MouseEventArgs e)
        {
            var Zoom = GetZoom();
            var point = GetMousePoint(e);
            base.OnMouseClick(new MouseEventArgs(e.Button, e.Clicks, point.X, point.Y, e.Delta));
        }
        protected override void OnMouseDoubleClick(System.Windows.Forms.MouseEventArgs e)
        {
            var Zoom = GetZoom();
            var point = GetMousePoint(e);
            base.OnMouseDoubleClick(new MouseEventArgs(e.Button, e.Clicks, point.X, point.Y, e.Delta));
        }
        protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            var Zoom = GetZoom();
            var point = GetMousePoint(e);
            base.OnMouseDown(new MouseEventArgs(e.Button, e.Clicks, point.X, point.Y, e.Delta));
        }
        protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            var Zoom = GetZoom();
            var point = GetMousePoint(e);
            base.OnMouseUp(new MouseEventArgs(e.Button, e.Clicks, point.X, point.Y, e.Delta));
        }
        public void OnMouseMoveEventCreate()
        {
            System.Windows.Forms.Cursor.Position = new Point(System.Windows.Forms.Cursor.Position.X, System.Windows.Forms.Cursor.Position.Y);
        }

        #endregion

        #region Get Screen / Zoom
        public Rectangle GetParentScreen()
        {
            if (_parent == null) return new Rectangle(0, 0, 10, 10);
            else return _parent.GetScreen();
        }
        public Rectangle GetScreen()
        {
            if (_parent == null) return new Rectangle(0, 0, 10, 10);
            Rectangle screen = GetParentScreen();
            double zoom = GetZoom();
            if (zoom == 1.0)
                return screen;
            else
                return new Rectangle((int)Math.Floor(screen.X / zoom),
                    (int)Math.Floor(screen.Y / zoom),
                    (int)Math.Ceiling(screen.Width / zoom),
                    (int)Math.Ceiling(screen.Height / zoom));
        }

        private SFML.System.Vector2i GetPosition()
        {
            if (_parent == null) return new SFML.System.Vector2i(0, 0);
            return _parent.GetPosition();
        }

        private FloatRect GetRectScreen()
        {
            var position = GetPosition();
            var size = GetSize();
            return new FloatRect(position.X, position.Y, size.X, size.Y);
        }

        private float GetZoom()
        {
            if (_parent == null) return 1;
            return _parent.GetZoom();
        }

        public SFML.Graphics.View GetCurrentView()
        {
            var view = new SFML.Graphics.View(GetRectScreen());
            return view;
        }

        private SFML.System.Vector2u GetSize()
        {
            uint width = (uint)(this.Width);
            uint height = (uint)(this.Height);

            return new SFML.System.Vector2u(width, height);
        }

        private SFML.System.Vector2u GetWindowSize()
        {
            uint width = (uint)(this.Width);
            uint height = (uint)(this.Height);

            return new SFML.System.Vector2u(width, height);
        }

        public bool IsObjectOnScreen(int x, int y, int width, int height)
        {
            var screen = GetRectScreen();
            float zoom = GetZoom();


            bool x1 = x * zoom > screen.Left + screen.Width;
            bool x2 = (x + width) * zoom < screen.Left;
            bool y1 = y * zoom > screen.Top + screen.Height;
            bool y2 = (y + height) * zoom < screen.Top;

            return !(x1 || y1 || x2 || y2);
        }

        public bool ArePointsOnScreen(int x1, int y1, int x2, int y2)
        {
            int width, height;
            int top, left;
            if (x1 > x2)
            {
                width = x1 - x2;
                left = x2;
            }
            else
            {
                width = x2 - x1;
                left = x1;
            }

            if (y1 > y2)
            {
                height = y1 - y2;
                top = y2;
            }
            else
            {
                height = y2 - y1;
                top = y1;
            }

            return IsObjectOnScreen(left, top, width, height);
        }


        #endregion

        #region Draw Methods

        #region Basic
        public void DrawTexture(SFML.Graphics.Texture image, int x, int y, int width, int height, bool selected, int transparency)
        {
            DrawTexture(image, x, y, 0, 0, width, height, selected, transparency);
        }
        public void DrawTexture(SFML.Graphics.Texture image, int x, int y, int rect_x, int rect_y, int width, int height, bool selected, int Transparency, bool fliph = false, bool flipv = false, int rotation = 0, Color? color = null)
        {
            if (!IsObjectOnScreen(x, y, width, height) || image == null) return;

            var zoom = GetZoom();

            int real_x = (int)(x * zoom);
            int real_y = (int)(y * zoom);

            SFML.System.Vector2f size = new SFML.System.Vector2f(width * zoom, height * zoom);
            SFML.System.Vector2f center = new SFML.System.Vector2f(size.X / 2, size.Y / 2);
            SFML.System.Vector2f position = new SFML.System.Vector2f(real_x + center.X, real_y + center.Y);


            var textureRect = new IntRect(rect_x, rect_y, width, height);



            if (fliph || flipv)
            {
                if (fliph)
                {
                    var temp = textureRect.Left;
                    textureRect.Left = temp + textureRect.Width;
                    textureRect.Width = -textureRect.Width;
                }
                if (flipv)
                {
                    var temp = textureRect.Top;
                    textureRect.Top = temp + textureRect.Height;
                    textureRect.Height = -textureRect.Height;
                }
            }



            SFML.Graphics.RectangleShape rect = new SFML.Graphics.RectangleShape();
            rect.Position = position;
            rect.Origin = center;
            rect.Rotation = rotation;
            rect.Size = size;
            rect.Texture = image;
            rect.FillColor = GetTransparency(color, Transparency);
            rect.TextureRect = textureRect;
            RenderWindow.Draw(rect);
        }

        public void DrawTexture(SFML.Graphics.Texture image, int x, int y, int rect_x, int rect_y, int rect_width, int rect_height, int width, int height, bool selected, int Transparency, bool fliph = false, bool flipv = false, int rotation = 0, Color? color = null)
        {
            if (!IsObjectOnScreen(x, y, width, height) || image == null) return;

            var zoom = GetZoom();

            int real_x = (int)(x * zoom);
            int real_y = (int)(y * zoom);

            SFML.System.Vector2f size = new SFML.System.Vector2f(rect_width, rect_height);
            SFML.System.Vector2f center = new SFML.System.Vector2f(size.X / 2, size.Y / 2);
            SFML.System.Vector2f position = new SFML.System.Vector2f(real_x + center.X, real_y + center.Y);


            var textureRect = new IntRect(rect_x, rect_y, width, height);



            if (fliph || flipv)
            {
                if (fliph)
                {
                    var temp = textureRect.Left;
                    textureRect.Left = temp + textureRect.Width;
                    textureRect.Width = -textureRect.Width;
                }
                if (flipv)
                {
                    var temp = textureRect.Top;
                    textureRect.Top = temp + textureRect.Height;
                    textureRect.Height = -textureRect.Height;
                }
            }



            SFML.Graphics.RectangleShape rect = new SFML.Graphics.RectangleShape();
            rect.Position = position;
            rect.Origin = center;
            rect.Rotation = rotation;
            rect.Size = size;
            rect.Texture = image;
            rect.FillColor = GetTransparency(color, Transparency);
            rect.TextureRect = textureRect;
            RenderWindow.Draw(rect);
        }

        private SFML.Graphics.Color GetTransparency(Color? Color, int Transparency)
        {
            SFML.Graphics.Color OriginalColor;
            if (Color.HasValue) OriginalColor = new SFML.Graphics.Color(Color.Value.R, Color.Value.G, Color.Value.B, Color.Value.A);
            else OriginalColor = SFML.Graphics.Color.White;

            byte alpha = (byte)(Transparency & 0x000000FF);
            return new SFML.Graphics.Color(OriginalColor.R, OriginalColor.G, OriginalColor.B, alpha);
        }

        public void DrawRectangle(int x1, int y1, int x2, int y2, Color color)
        {
            DrawRectangle(x1, y1, x2, y2, color, System.Drawing.Color.Transparent, 0);

        }
        public void DrawRectangle(int x1, int y1, int x2, int y2, Color color, Color color2, int thickness)
        {
            if (!IsObjectOnScreen(x1, y1, x2 - x1, y2 - y1)) return;

            var zoom = GetZoom();

            int real_x1 = (int)(x1 * zoom);
            int real_x2 = (int)(x2 * zoom);
            int real_y1 = (int)(y1 * zoom);
            int real_y2 = (int)(y2 * zoom);

            SFML.System.Vector2f position = new SFML.System.Vector2f(real_x1, real_y1);
            SFML.System.Vector2f size = new SFML.System.Vector2f(real_x2 - real_x1, real_y2 - real_y1);

            SFML.Graphics.RectangleShape rect = new SFML.Graphics.RectangleShape();
            rect.OutlineThickness = thickness;
            rect.OutlineColor = new SFML.Graphics.Color(color2.R, color2.G, color2.B, color2.A);
            rect.FillColor = new SFML.Graphics.Color(color.R, color.G, color.B, color.A);
            rect.Position = position;
            rect.Size = size;
            RenderWindow.Draw(rect);

        }

        public void DrawQuad(int x, int y, int width, int height, Color color, Color color2, int thickness)
        {

            var zoom = GetZoom();

            int real_x = (int)(x * zoom);
            int real_y = (int)(y * zoom);
            int real_width = (int)(width * zoom);
            int real_height = (int)(height * zoom);

            SFML.System.Vector2f position = new SFML.System.Vector2f(real_x, real_y);
            SFML.System.Vector2f size = new SFML.System.Vector2f(real_width, real_height);

            float offset_x = 0;
            float offset_y = 0;

            SFML.Graphics.ConvexShape rect = new SFML.Graphics.ConvexShape(4);
            rect.OutlineThickness = thickness;
            rect.OutlineColor = new SFML.Graphics.Color(color2.R, color2.G, color2.B, color2.A);
            rect.FillColor = new SFML.Graphics.Color(color.R, color.G, color.B, color.A);
            rect.Position = position;
            rect.SetPoint(0, new SFML.System.Vector2f(offset_x, offset_y));
            rect.SetPoint(1, new SFML.System.Vector2f(offset_x + size.X, offset_y));
            rect.SetPoint(2, new SFML.System.Vector2f(offset_x + size.X - size.Y, offset_y + size.Y));
            rect.SetPoint(3, new SFML.System.Vector2f(offset_x - size.Y, offset_y + size.Y));
            RenderWindow.Draw(rect);

        }

        public void DrawEllipse(int x1, int y1, int radiusX, int radiusY, Color color, float thickness = 1)
        {
            var zoom = GetZoom();

            int real_x = (int)(x1 * zoom);
            int real_y = (int)(y1 * zoom);
            float real_radiusX = (int)(radiusX * zoom);
            float real_radiusY = (int)(radiusY * zoom);

            SFML.System.Vector2f position = new SFML.System.Vector2f(real_x - real_radiusX, real_y - real_radiusY);
            SFML.System.Vector2f radius = new SFML.System.Vector2f(real_radiusX, real_radiusY);

            Classes.Rendering.EllipseShape rect = new Classes.Rendering.EllipseShape();
            rect.OutlineThickness = thickness;
            rect.OutlineColor = new SFML.Graphics.Color(color.R, color.G, color.B, color.A);
            rect.FillColor = SFML.Graphics.Color.Transparent;
            rect.Position = position;
            rect.SetRadius(radius);
            RenderWindow.Draw(rect);

        }

        public void DrawSimpleLine(int x1, int y1, int x2, int y2, Color color = new Color())
        {
            if (!ArePointsOnScreen(x1, y1, x2, y2)) return;

            var zoom = GetZoom();

            int real_x1 = (int)(x1 * zoom);
            int real_x2 = (int)(x2 * zoom);
            int real_y1 = (int)(y1 * zoom);
            int real_y2 = (int)(y2 * zoom);


            SFML.Graphics.VertexArray line = new SFML.Graphics.VertexArray();
            line.PrimitiveType = SFML.Graphics.PrimitiveType.Lines;
            line.Append(new Vertex(new SFML.System.Vector2f(real_x1, real_y1), new SFML.Graphics.Color(color.R, color.G, color.B, color.A)));
            line.Append(new Vertex(new SFML.System.Vector2f(real_x2, real_y2), new SFML.Graphics.Color(color.R, color.G, color.B, color.A)));
            RenderWindow.Draw(line);

        }
        public void DrawLine(int x1, int y1, int x2, int y2, Color color = new Color(), float thickness = 1)
        {
            if (!ArePointsOnScreen(x1, y1, x2, y2)) return;

            var zoom = GetZoom();

            int real_x1 = (int)(x1 * zoom);
            int real_x2 = (int)(x2 * zoom);
            int real_y1 = (int)(y1 * zoom);
            int real_y2 = (int)(y2 * zoom);

            var point1 = new SFML.System.Vector2f(real_x1, real_y1);
            var point2 = new SFML.System.Vector2f(real_x2, real_y2);
            var sfmlColor = new SFML.Graphics.Color(color.R, color.G, color.B, color.A);

            Classes.Rendering.SelbaWardLine line = new Classes.Rendering.SelbaWardLine(point1, point2, sfmlColor, thickness);
            RenderWindow.Draw(line);

        }
        #endregion

        #region Extra
        public void DrawDashedLine(int X1, int Y1, int X2, int Y2, Color color, Color color2, Color color3, Color color4, float thickness = 1, int dashLength = 5)
        {
            Rectangle screen = GetParentScreen();

            var zoom = GetZoom();

            int width = Math.Abs(X2 - X1);
            int height = Math.Abs(Y2 - Y1);
            int x = Math.Min(X1, X2);
            int y = Math.Min(Y1, Y2);

            if (!IsObjectOnScreen(x, y, width, height)) return;

            Point first = new Point(X1, Y1);
            Point last = new Point(X2, Y2);

            Point currentA = first;
            int segment = 0;
            Color currentColor = color;
            int currentDashPoint = 0;

            List<Point> points = GetPoints(first, last, dashLength);
            points.Add(last);

            foreach (var currentB in points)
            {
                if (currentDashPoint == dashLength)
                {
                    if (segment == 0) currentColor = color;
                    else if (segment == 1) currentColor = color2;
                    else if (segment == 2) currentColor = color3;
                    else if (segment == 3) currentColor = color4;

                    DrawLine(currentA.X, currentA.Y, currentB.X, currentB.Y, currentColor, thickness);
                    segment++;
                    if (segment >= 3) segment = 0;
                    currentA = currentB;
                    currentDashPoint = 0;
                }
                else
                {
                    currentDashPoint++;
                }
            }
        }
        public List<Point> GetPoints(Point p1, Point p2, int dashLength)
        {
            List<Point> points = new System.Collections.Generic.List<Point>();

            int length = (int)Math.Sqrt(Math.Pow((p2.Y - p1.Y), 2) + Math.Pow((p2.X - p1.X), 2));
            int count = length / dashLength;

            for (int i = 1; i <= count; i++)
            {
                var point = new Point();
                point.X = (Math.Abs(p1.X - p2.X) / count) * i + p2.X;
                point.Y = (Math.Abs(p1.Y - p2.Y) / count) * i + p2.Y;
                points.Add(point);
            }

            return points;
        }
        public void DrawArrow(int x0, int y0, int x1, int y1, Color color, float thickness = 1)
        {
            int x2, y2, x3, y3;

            double angle = Math.Atan2(y1 - y0, x1 - x0) + Math.PI;

            x2 = (int)(x1 + 10 * Math.Cos(angle - Math.PI / 8));
            y2 = (int)(y1 + 10 * Math.Sin(angle - Math.PI / 8));
            x3 = (int)(x1 + 10 * Math.Cos(angle + Math.PI / 8));
            y3 = (int)(y1 + 10 * Math.Sin(angle + Math.PI / 8));

            DrawLine(x1, y1, x0, y0, color, thickness);
            DrawLine(x1, y1, x2, y2, color, thickness);
            DrawLine(x1, y1, x3, y3, color, thickness);
        }
        public void DrawText(string text, int x, int y, Color color, bool bold, int size = 4)
        {
            DrawText(text, x, y, color, bold, size, System.Drawing.Color.Transparent);
        }
        public void DrawText(string text, int x, int y, Color color, bool bold, int size, Color bordercolor)
        {
            SFML.Graphics.Text textObject = new Text();
            if (bold) textObject.Font = RenderingFontBold;
            else textObject.Font = RenderingFont;

            textObject.DisplayedString = text;
            float zoom = GetZoom();
            textObject.CharacterSize = (uint)(size * zoom);
            textObject.OutlineThickness = 1;
            textObject.OutlineColor = new SFML.Graphics.Color(bordercolor.R, bordercolor.G, bordercolor.B, bordercolor.A);
            textObject.Position = new SFML.System.Vector2f(x * zoom, y * zoom);
            textObject.FillColor = new SFML.Graphics.Color(color.R, color.G, color.B, color.A);
            RenderWindow.Draw(textObject);
        }

        #endregion

        #region Retro

        public void Draw2DCursor(int x, int y)
        {
            //hvcursor = new Texture(_device, hvcursorb, Usage.Dynamic, Pool.Default);

            DrawTexture(hvcursor, x - 15, y - 16, 0, 0, 32, 32, 32, 32, false, 255, false, false, 0, Color.White);
            //DrawTexture2(hvcursor, new Rectangle(Point.Empty, Cursors.NoMove2D.Size), new Vector3(0, 0, 0), new Vector3(x - 16, y - 15, 0), Color.White);
        }
        public void DrawHorizCursor(int x, int y)
        {
            //hcursor = new Texture(_device, hcursorb, Usage.Dynamic, Pool.Default);

            DrawTexture(hcursor, x - 15, y - 16, 0, 0, 32, 32, 32, 32, false, 255, false, false, 0, Color.White);
            //DrawTexture(hcursor, new Rectangle(Point.Empty, Cursors.NoMove2D.Size), new Vector3(0, 0, 0), new Vector3(x - 16, y - 15, 0), Color.White);
        }

        public void DrawVertCursor(int x, int y)
        {
            //vcursor = new Texture(_device, vcursorb, Usage.Dynamic, Pool.Default);

            DrawTexture(vcursor, x - 15, y - 16, 0, 0, 32, 32, 32, 32, false, 255, false, false, 0, Color.White);
            //DrawTexture(vcursor, new Rectangle(Point.Empty, Cursors.NoMove2D.Size), new Vector3(0, 0, 0), new Vector3(x - 16, y - 15, 0), Color.White);
        }

        public void DrawCircle(int x, int y, Color color)
        {
            if (!IsObjectOnScreen(x - 4, y - 4, 9, 9)) return;
            //tcircle = new Texture(_device, tcircleb, Usage.Dynamic, Pool.Default);

            Rectangle screen = _parent.GetScreen();
            double zoom = _parent.GetZoom();
            DrawTexture(tcircle, 0, 0, 0, 0, 9, 9, false, 255, false, false, 0, color);
        }

        public void DrawEmptyCircle(int x, int y, Color color)
        {
            if (!IsObjectOnScreen(x - 6, y - 6, 14, 14)) return;
            //tecircle = new Texture(_device, tecircleb, Usage.Dynamic, Pool.Default);

            Rectangle screen = _parent.GetScreen();
            double zoom = _parent.GetZoom();
            DrawTexture(tecircle, 0, 0, 0, 0, 9, 9, false, 255, false, false, 0, color);
        }

        #endregion

        #endregion

        #region Disposing
        public new void Dispose()
        {
            if (RenderWindow != null) RenderWindow.Dispose();
            base.Dispose();
        }

        #endregion


    }
}
