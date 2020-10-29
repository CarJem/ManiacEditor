using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SharpDX;
using SharpDX.Direct3D9;
using SharpDX.Windows;
using SharpDX.Mathematics;
using SharpDX.Mathematics.Interop;
using Font = SharpDX.Direct3D9.Font;
using Rectangle = System.Drawing.Rectangle;
using Color = System.Drawing.Color;
using Point = System.Drawing.Point;
using Bitmap = System.Drawing.Bitmap;
using ManiacEditor.Events;
using System.Diagnostics;
using ManiacEditor.Classes.Rendering;
using System.Collections.Generic;

namespace ManiacEditor
{
    public partial class DevicePanel : UserControl
    {

        #region Structures

        public struct EllipseKey
        {
            public int radiusX;
            public int radiusY;
            public Color color;
            public int thickness;

            public EllipseKey(int _radiusX, int _radiusY, Color _color, int _thickness)
            {
                radiusX = _radiusX;
                radiusY = _radiusY;
                color = _color;
                thickness = _thickness;
            }
        }

        #endregion

        #region DX Members

        private Sprite sprite;
        private Sprite sprite2;
        private Classes.Rendering.TextureExt tx;
        private Classes.Rendering.TextureExt hvcursor;
        private Classes.Rendering.TextureExt vcursor;
        private Classes.Rendering.TextureExt hcursor;
        private Dictionary<EllipseKey, Classes.Rendering.TextureExt> tx_ellipses;

        public Device _device = null;
        private Direct3D direct3d = null;
        public IDrawArea _parent = null; // The Form to place the DevicePanel onto
        private PresentParameters presentParams;

        #endregion

        #region Event Members

        public event RenderEventHandler OnRender; // On this event we can start to render our scene
        public event CreateDeviceEventHandler OnCreateDevice; // This event tells us when the device is Ready
        public MouseEventArgs LastMouseEvent;

        #endregion

        #region Status Properties

        public bool AllowRender { get; set; } = true;
        public bool isRendering { get; private set; } = false;
        public bool isDeviceLost { get; private set; }
        public Color DeviceBackColor { get; set; } = Color.Black;

        #endregion

        #region Font Members

        private Font FontDX;
        private Font FontDX_Bold;
        public static FontDescription FontDescription = new FontDescription()
        {
            Height = 18,
            Italic = false,
            CharacterSet = FontCharacterSet.Ansi,
            FaceName = "Microsoft Sans Serif",
            MipLevels = 0,
            OutputPrecision = FontPrecision.TrueType,
            PitchAndFamily = FontPitchAndFamily.Default,
            Quality = FontQuality.Antialiased,
            Weight = FontWeight.Regular
        };
        public static FontDescription FontDescription_Bold = new FontDescription()
        {
            Height = 18,
            Italic = false,
            CharacterSet = FontCharacterSet.Ansi,
            FaceName = "Microsoft Sans Serif",
            MipLevels = 0,
            OutputPrecision = FontPrecision.TrueType,
            PitchAndFamily = FontPitchAndFamily.Default,
            Quality = FontQuality.Antialiased,
            Weight = FontWeight.Bold
        };

        #endregion

        #region FPS Members
        public double FPS { get; private set; } = 0.0;
        System.Diagnostics.Stopwatch FPS_Clock;
        double FPS_TotalTime;
        long FPS_FrameCount;

        #endregion

        #region Constructor

        public DevicePanel()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);
        }

        #endregion

        #region Init DX

        /// <summary>
        /// Init the DirectX-Stuff here
        /// </summary>
        /// <param name="parent">parent of the DevicePanel</param>
        /// 
        public void Init(IDrawArea parent)
        {
            try
            {
                _parent = parent;

                // Setup our D3D stuff
                presentParams = new PresentParameters();
                presentParams.Windowed = true;
                presentParams.DeviceWindowHandle = this.Handle;
                presentParams.BackBufferHeight = this.Height;
                presentParams.BackBufferWidth = this.Width;
                presentParams.PresentationInterval = PresentInterval.Immediate;
                presentParams.SwapEffect = SwapEffect.Discard;

                direct3d = new Direct3D();
                Capabilities caps = direct3d.Adapters.First().GetCaps(DeviceType.Hardware);

                CreateFlags createFlags;

                if ((caps.DeviceCaps & DeviceCaps.HWTransformAndLight) != 0)
                {
                    createFlags = CreateFlags.HardwareVertexProcessing;
                }
                else
                {
                    createFlags = CreateFlags.SoftwareVertexProcessing;
                }

                if ((caps.DeviceCaps & DeviceCaps.PureDevice) != 0 && createFlags ==
                    CreateFlags.HardwareVertexProcessing)
                {
                    createFlags |= CreateFlags.PureDevice;
                }

                _device = new Device(direct3d, 0, DeviceType.Hardware, this.Handle, createFlags, presentParams);
                _device.SetSamplerState(0, SamplerState.MinFilter, TextureFilter.None);
                _device.SetSamplerState(0, SamplerState.MagFilter, TextureFilter.None);
                _device.SetSamplerState(0, SamplerState.MipFilter, TextureFilter.None);
                _device.SetRenderState(RenderState.CullMode, false);
                _device.SetRenderState(RenderState.ZEnable, ZBufferType.DontUseZBuffer);

                if (OnCreateDevice != null)
                {
                    OnCreateDevice(this, new DeviceEventArgs(_device));
                }

                InitDeviceResources();
            }
            catch (SharpDXException ex)
            {
                throw new ArgumentException("Error initializing DirectX", ex);
            }
            catch (DllNotFoundException)
            {
                throw new Exception("Please install DirectX Redistributable June 2010");
            }
        }
        public void InitDeviceResources()
        {
            Bitmap txb = new Bitmap(1, 1);
            using (Graphics g = Graphics.FromImage(txb))
                g.Clear(Color.White);

            Bitmap hcursorb = new Bitmap(32, 32);
            using (Graphics g = Graphics.FromImage(hcursorb))
                Cursors.NoMoveHoriz.Draw(g, new Rectangle(0, 0, 32, 32));
            MakeGray(hcursorb);

            Bitmap vcursorb = new Bitmap(32, 32);
            using (Graphics g = Graphics.FromImage(vcursorb))
                Cursors.NoMoveVert.Draw(g, new Rectangle(0, 0, 32, 32));
            MakeGray(vcursorb);

            Bitmap hvcursorb = new Bitmap(32, 32);
            using (Graphics g = Graphics.FromImage(hvcursorb))
                Cursors.NoMove2D.Draw(g, new Rectangle(0, 0, 32, 32));
            MakeGray(hvcursorb);

            sprite = new Sprite(_device);
            sprite2 = new Sprite(_device);

            tx_ellipses = new Dictionary<EllipseKey, TextureExt>();

            tx = Methods.Drawing.TextureCreator.FromBitmap(_device, txb);
            hcursor = Methods.Drawing.TextureCreator.FromBitmap(_device, hcursorb);
            vcursor = Methods.Drawing.TextureCreator.FromBitmap(_device, vcursorb);
            hvcursor = Methods.Drawing.TextureCreator.FromBitmap(_device, hvcursorb);

            FontDX = new Font(_device, FontDescription);
            FontDX_Bold = new Font(_device, FontDescription_Bold);

            FPS_Clock = Stopwatch.StartNew();
        }

        #endregion

        #region Ellipse Retrival

        public Bitmap GetTextureReadyBitmap(Bitmap bmp2)
        {
            var squareSize = (bmp2.Width > bmp2.Height ? bmp2.Width : bmp2.Height);
            int factor = 32;
            int newSize = (int)Math.Round((squareSize / (double)factor), MidpointRounding.AwayFromZero) * factor;
            if (newSize == 0) newSize = factor;
            while (newSize < squareSize) newSize += factor;

            Bitmap bmp = new Bitmap(newSize, newSize);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.DrawImage(bmp2, 0, 0, new Rectangle(0, 0, bmp2.Width, bmp2.Height), GraphicsUnit.Pixel);
            }
            bmp2 = (Bitmap)bmp.Clone();
            bmp.Dispose();
            return bmp2;
        }
        public TextureExt GetEllipse(EllipseKey key)
        {
            if (tx_ellipses.ContainsKey(key)) return tx_ellipses[key];
            else
            {
                int size = 0;
                if (key.radiusX * 2 > key.radiusY * 2) size = key.radiusX * 2;
                else size = key.radiusY * 2;
                size += key.thickness;
                size += 1;
                Bitmap txb_ellipse = GetTextureReadyBitmap(new Bitmap(size, size));
                using (Graphics g = Graphics.FromImage(txb_ellipse))
                {
                    g.Clear(Color.Transparent);
                    Pen pen = new Pen(key.color);
                    pen.Width = key.thickness;

                    int x = 0;
                    int y = 0;
                    int width = key.radiusX * 2;
                    int height = key.radiusY * 2;

                    g.DrawEllipse(pen, new System.Drawing.RectangleF(x, y, width, height));
                }
                TextureExt tx_ellipse = Methods.Drawing.TextureCreator.FromBitmap(_device, txb_ellipse);
                tx_ellipses.Add(key, tx_ellipse);
                txb_ellipse.Dispose();
                return tx_ellipses[key];
            }
        }

        #endregion

        #region Details

        private double GetZoom()
        {
            if (_parent == null) return 1;
            return _parent.GetZoom();
        }

        private Point GetMousePoint(System.Windows.Forms.MouseEventArgs e)
        {
            var position = _parent.GetScreen();
            var zoom = GetZoom();
            return new Point((int)((e.X + position.X) / zoom), (int)((e.Y + position.Y) / zoom));
        }

        #endregion

        #region Rendering
        public void Run()
        {
            ManiacEditor.Classes.Rendering.RenderLoop.Run(this, () =>
            {
                if (!isRendering)
                {
                    Draw();
                }
            }, true);
        }
        
        public void Render()
        {
            /*
            if (!isRendering)
            {
                Draw();
            }
            */
        }

        public void Draw()
        {
            if (!AllowRender) return;

            if (isRendering) return;

            if (isDeviceLost || _device == null)
            {
                if (_device == null) return;
                else
                {
                    AttemptRecovery();
                    return;
                }
            }

            isRendering = true;
            try
            {
                Rectangle screen = _parent.GetScreen();
                double zoom = _parent.GetZoom();

                //Clear the backbuffer
                _device.Clear(ClearFlags.Target, new SharpDX.Color(DeviceBackColor.R, DeviceBackColor.B, DeviceBackColor.G, DeviceBackColor.A), 1.0f, 0);

                //Begin the scene
                _device.BeginScene();

                sprite.Transform = Matrix.Scaling((float)zoom, (float)zoom, 1f);

                sprite2.Begin(SpriteFlags.AlphaBlend);

                if (zoom > 1)
                {
                    // If zoomin, just do near-neighbor scaling
                    _device.SetSamplerState(0, SamplerState.MinFilter, TextureFilter.None);
                    _device.SetSamplerState(0, SamplerState.MagFilter, TextureFilter.None);
                    _device.SetSamplerState(0, SamplerState.MipFilter, TextureFilter.None);
                }
                else if (zoom < 1)
                {
                    // If zoomout, just do near-neighbor scaling
                    _device.SetSamplerState(0, SamplerState.MinFilter, TextureFilter.None);
                    _device.SetSamplerState(0, SamplerState.MagFilter, TextureFilter.None);
                    _device.SetSamplerState(0, SamplerState.MipFilter, TextureFilter.None);
                }

                sprite.Begin(SpriteFlags.AlphaBlend | SpriteFlags.DoNotModifyRenderState);

                // Render of scene here
                if (OnRender != null) OnRender(this, new DeviceEventArgs(_device));

                sprite.Transform = Matrix.Scaling(1f, 1f, 1f);

                sprite.End();
                sprite2.End();
                //End the scene
                _device.EndScene();
                _device.Present();

                FPS_FrameCount++;
                var timeElapsed = (double)FPS_Clock.ElapsedTicks / Stopwatch.Frequency;
                FPS_TotalTime += timeElapsed;
                if (FPS_TotalTime >= 1.0f)
                {
                    FPS = (double)FPS_FrameCount / FPS_TotalTime;
                    FPS_FrameCount = 0;
                    FPS_TotalTime = 0.0;
                }

                FPS_Clock.Restart();
            }
            catch (SharpDXException ex)
            {
                if (ex.ResultCode == ResultCode.DeviceLost)
                    isDeviceLost = true;
                else
                    throw ex;
            }
            isRendering = false;

        }

        #endregion

        #region Overrides
        protected override void OnPaint(PaintEventArgs e)
        {
            this.Render();
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
            Cursor.Position = new Point(Cursor.Position.X, Cursor.Position.Y);
        }

        #endregion

        #region Drawing
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
        public bool IsObjectOnScreenPoint(int x1, int y1, int x2, int y2)
        {
            int x = (x1 > x2 ? x2 : x1);
            int y = (y1 > y2 ? y2 : y1);
            int width = (x1 > x2 ? x1 - x2 : x2 - x1);
            int height = (y1 > y2 ? y1 - y2 : y2 - y1);

            return IsObjectOnScreen(x, y, width, height);
        }
        public bool IsObjectOnScreen(int x, int y, int width, int height)
        {
            Rectangle screen = _parent.GetScreen();
            double zoom = _parent.GetZoom();

            int zoom_x = (int)(x * zoom);
            int zoom_y = (int)(y * zoom);
            int zoom_width = (int)((x + width) * zoom);
            int zoom_height = (int)((y + height) * zoom);

            int MaxX = screen.X + screen.Width;
            int MinX = screen.X;
            int MaxY = screen.Y + screen.Height;
            int MinY = screen.Y;

            bool OutOfX = zoom_x > MaxX;
            bool NotOfX = zoom_width < MinX;
            bool OutOfY = zoom_y > MaxY;
            bool NotOfY = zoom_height < MinY;


            return !(OutOfX || NotOfX || OutOfY || NotOfY);
        }
        private void DrawTexture(Classes.Rendering.TextureExt image, Rectangle srcRect, Vector3 center, Vector3 position, Color color)
        {
            if (image == null) return;
            var dx_color = new SharpDX.Color(color.R, color.G, color.B, color.A);
            var dx_rec = new SharpDX.Rectangle(srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height);
            sprite.Draw(image, dx_color, dx_rec, center, position);
        }
        private void DrawTexture2(Classes.Rendering.TextureExt image, Rectangle srcRect, Vector3 center, Vector3 position, Color color)
        {
            if (image == null) return;
            var dx_color = new SharpDX.Color(color.R, color.G, color.B, color.A);
            var dx_rec = new SharpDX.Rectangle(srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height);
            sprite2.Draw(image, dx_color, dx_rec, center, position);
        }
        public void DrawBitmap(Classes.Rendering.TextureExt image, int x, int y, int width, int height, bool selected, int transparency)
        {
            DrawBitmap(image, x, y, 0, 0, width, height, selected, transparency);
        }
        public void DrawBitmap(Classes.Rendering.TextureExt image, int x, int y, int rect_x, int rect_y, int width, int height, bool selected, int Transparency, bool fliph = false, bool flipv = false, int rotation = 0, Color? color = null)
        {
            var LastTransform = sprite.Transform;

            Rectangle screen = _parent.GetScreen();
            double zoom = _parent.GetZoom();

            sprite.Transform = Matrix.Scaling((float)zoom, (float)zoom, 1f);

            Rectangle boundBox = new Rectangle(rect_x, rect_y, width, height);
            Vector3 position = new Vector3(x - (int)(screen.X / zoom), y - (int)(screen.Y / zoom), 0);
            Vector3 center = new Vector3(new float[] { 0, 0, 0 });

            if (fliph || flipv || rotation != 0)
            {
                float negatedZoom = -(float)zoom;
                float normalZoom = (float)zoom;
                if (rotation != 0)
                {

                    float center_x = (fliph ? (float)(width / 2) : (float)(width / 2));
                    float center_y = (flipv ? (float)(height / 2) : (float)(height / 2));
                    float position_x = (fliph ? position.X : position.X);
                    float position_y = (flipv ? position.Y : position.Y);

                    var scalingCenter = new Vector2(0, 0);
                    var scalingRotation = 1f;
                    var scaling = new Vector2(1f, 1f);
                    var rotationCenter = new Vector2(center_x, center_y);
                    var Rotation = (float)(rotation * (Math.PI / 180.0));
                    var translation = new Vector2(position_x, position_y);
                    sprite.Transform = Matrix.Transformation2D(scalingCenter, scalingRotation, scaling, rotationCenter, Rotation, translation);
                    position = Vector3.Zero;
                    center = Vector3.Zero;
                    //sprite.Transform *= Matrix.Scaling((fliph ? negatedZoom : normalZoom), (flipv ? negatedZoom : normalZoom), 1f);
                    sprite.Transform *= Matrix.Scaling(normalZoom, normalZoom, 1f);
                }
                else
                {
                    if (fliph) position.X = -position.X;
                    if (flipv) position.Y = -position.Y;

                    if (fliph) center.X = boundBox.Width;
                    if (flipv) center.Y = boundBox.Height;
                    sprite.Transform = Matrix.Scaling((fliph ? negatedZoom : normalZoom), (flipv ? negatedZoom : normalZoom), 1f);
                }
            }

            if (color != null) DrawTexture(image, boundBox, center, position, Color.FromArgb(Transparency, color.Value));
            else DrawTexture(image, boundBox, center, position, (selected) ? Color.BlueViolet : Color.FromArgb(Transparency, Color.White));

            sprite.Transform = LastTransform;
        }
        public void DrawEllipse(int x, int y, int radiusX, int radiusY, Color color, int thickness = 1)
        {
            if (radiusX <= -1) radiusX *= -1;
            if (radiusY <= -1) radiusY *= -1;

            int x1 = x - radiusX;
            int y1 = y - radiusY;
            int x2 = x + radiusX;
            int y2 = y + radiusY;

            if (!IsObjectOnScreen(x1, y1, x2 - x1, y2 - y1)) return;

            EllipseKey key = new EllipseKey(radiusX, radiusY, color, thickness);


            Rectangle screen = _parent.GetScreen();
            double zoom = _parent.GetZoom();

            TextureExt tx_ellipse = GetEllipse(key);

            DrawTexture(tx_ellipse, new Rectangle(0, 0, x2 - x1, y2 - y1), new Vector3(0, 0, 0), new Vector3(x1 - (int)(screen.X / zoom), y1 - (int)(screen.Y / zoom), 0), color);
        }
        public void DrawLine(int X1, int Y1, int X2, int Y2, Color color = new Color(), float thickness = 1, bool useZoomOffseting = false)
        {
            Rectangle screen = _parent.GetScreen();
            double zoom = _parent.GetZoom();
            int width = Math.Abs(X2 - X1);
            int height = Math.Abs(Y2 - Y1);
            int x = Math.Min(X1, X2);
            int y = Math.Min(Y1, Y2);
            int pixel_width = Math.Max((int)zoom, 1);

            if (!IsObjectOnScreen(x, y, width, height)) return;


            sprite.Transform = Matrix.Scaling(1f, 1f, 1f);
            if (width == 0 || height == 0)
            {
                int zoomOffset = (zoom % 1 == 0 ? 0 : 1);
                if (!useZoomOffseting) zoomOffset = 0;
                if (width == 0) width = pixel_width + zoomOffset;
                else width = (int)(width * zoom) + zoomOffset;
                if (height == 0) height = pixel_width + zoomOffset;
                else height = (int)(height * zoom) + zoomOffset;
                DrawTexture(tx, new Rectangle(0, 0, width, height), new Vector3(0, 0, 0), new Vector3((int)((x - (int)(screen.X / zoom)) * zoom), (int)((y - (int)(screen.Y / zoom)) * zoom), 0), color);
            }
            else
            {
                DrawLinePBP(X1, Y1, X2, Y2, color);
            }
            sprite.Transform = Matrix.Scaling((float)zoom, (float)zoom, 1f);
        }
        public void DrawLinePaperRoller(int X1, int Y1, int X2, int Y2, Color color, Color color2, Color color3, Color color4)
        {
            Rectangle screen = _parent.GetScreen();
            double zoom = _parent.GetZoom();
            int width = Math.Abs(X2 - X1);
            int height = Math.Abs(Y2 - Y1);
            int x = Math.Min(X1, X2);
            int y = Math.Min(Y1, Y2);
            int pixel_width = Math.Max((int)zoom, 1);

            if (!IsObjectOnScreen(x, y, width, height)) return;


            sprite.Transform = Matrix.Scaling(1f, 1f, 1f);
            if (width == 0 || height == 0)
            {
                if (width == 0) width = pixel_width;
                else width = (int)(width * zoom);
                if (height == 0) height = pixel_width;
                else height = (int)(height * zoom);
                DrawTexture(tx, new Rectangle(0, 0, width, height), new Vector3(0, 0, 0), new Vector3((int)((x - (int)(screen.X / zoom)) * zoom), (int)((y - (int)(screen.Y / zoom)) * zoom), 0), color);
            }
            else
            {
                DrawLinePBPDoted(X1, Y1, X2, Y2, color, color2, color3, color4);
            }
            sprite.Transform = Matrix.Scaling((float)zoom, (float)zoom, 1f);
        }
        public void DrawArrow(int x0, int y0, int x1, int y1, Color color, float thickness = 1)
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
            Rectangle screen = _parent.GetScreen();
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
        }
        public void DrawLinePBPDoted(int x0, int y0, int x1, int y1, Color color, Color color2, Color color3, Color color4)
        {
            Rectangle screen = _parent.GetScreen();
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
        }
        public void DrawRectangle(int x1, int y1, int x2, int y2, Color color, bool scaling = true)
        {
            DrawRectangle(x1, y1, x2, y2, color, System.Drawing.Color.Transparent, 0, scaling);
        }
        public void DrawRectangle(int x1, int y1, int x2, int y2, Color color, Color color2, int thickness, bool scaling = true)
        {
            if (!IsObjectOnScreenPoint(x1, y1, x2, y2)) return;
            Rectangle screen = _parent.GetScreen();
            double zoom = _parent.GetZoom();

            if (scaling) sprite.Transform = Matrix.Scaling((float)zoom, (float)zoom, 1f);

            if (color2 != System.Drawing.Color.Transparent)
            {
                DrawTexture(tx, new Rectangle(0, 0, x2 - x1, y2 - y1), new Vector3(0, 0, 0), new Vector3(x1 - (int)(screen.X / zoom), y1 - (int)(screen.Y / zoom), 0), color);

                DrawLine(x1, y1, x2, y1, color2);
                DrawLine(x1, y1, x1, y2, color2);
                DrawLine(x2, y2, x2, y1, color2);
                DrawLine(x2, y2, x1, y2, color2);
            }
            else
            {
                DrawTexture(tx, new Rectangle(0, 0, x2 - x1, y2 - y1), new Vector3(0, 0, 0), new Vector3(x1 - (int)(screen.X / zoom), y1 - (int)(screen.Y / zoom), 0), color);
            }

        }
        public void DrawQuad(int x1, int y1, int x2, int y2, Color color, Color color2, int thickness)
        {
            if (!IsObjectOnScreen(x1, y1, x2 - x1, y2 - y1)) return;
            Rectangle screen = _parent.GetScreen();
            double zoom = _parent.GetZoom();

            sprite.Transform = Matrix.Scaling((float)zoom, (float)zoom, 1f);

            DrawTexture(tx, new Rectangle(0, 0, x2 - x1, y2 - y1), new Vector3(0, 0, 0), new Vector3(x1 - (int)(screen.X / zoom), y1 - (int)(screen.Y / zoom), 0), color);
        }
        public void DrawText(string text, int x, int y, int width, Color color, bool bold)
        {
            Rectangle screen = _parent.GetScreen();
            double zoom = _parent.GetZoom();
            if (width >= 30)
            {
                ((bold) ? FontDX_Bold : FontDX).DrawText(sprite, text, new SharpDX.Rectangle(x - (int)(screen.X / zoom), y - (int)(screen.Y / zoom), width, 1000), FontDrawFlags.WordBreak, new SharpDX.Color(color.R, color.G, color.B, color.A));
            }
            else
            {
                ((bold) ? FontDX_Bold : FontDX).DrawText(sprite, text, x - (int)(screen.X / zoom), y - (int)(screen.Y / zoom), new SharpDX.Color(color.R, color.G, color.B, color.A));
            }
        }
        public void DrawTextSmall(string text, int x, int y, int width, Color color, bool bold)
        {
            Rectangle screen = _parent.GetScreen();
            double zoom = _parent.GetZoom();


            sprite.Transform = Matrix.Scaling((float)zoom / 4, (float)zoom / 4, 1f);
            if (width >= 10)
            {
                ((bold) ? FontDX_Bold : FontDX).DrawText(sprite, text, new SharpDX.Rectangle((x - (int)(screen.X / zoom)) * 4, (y - (int)(screen.Y / zoom)) * 4, width * 4, 1000), FontDrawFlags.WordBreak, new SharpDX.Color(color.R, color.G, color.B, color.A));
            }
            else
            {
                ((bold) ? FontDX_Bold : FontDX).DrawText(sprite, text, (x - (int)(screen.X / zoom)) * 4, (y - (int)(screen.Y / zoom)) * 4, new SharpDX.Color(color.R, color.G, color.B, color.A));
            }
            sprite.Transform = Matrix.Scaling((float)zoom, (float)zoom, 1f);
        }
        public void Draw2DCursor(int x, int y)
        {
            DrawTexture2(hvcursor, new Rectangle(Point.Empty, Cursors.NoMove2D.Size), new Vector3(0, 0, 0), new Vector3(x - 16, y - 15, 0), Color.White);

        }
        public void DrawHorizCursor(int x, int y)
        {
            DrawTexture2(hcursor, new Rectangle(Point.Empty, Cursors.NoMove2D.Size), new Vector3(0, 0, 0), new Vector3(x - 16, y - 15, 0), Color.White);
        }
        public void DrawVertCursor(int x, int y)
        {
            DrawTexture2(vcursor, new Rectangle(Point.Empty, Cursors.NoMove2D.Size), new Vector3(0, 0, 0), new Vector3(x - 16, y - 15, 0), Color.White);
            DrawTexture2(hcursor, new Rectangle(Point.Empty, Cursors.NoMove2D.Size), new Vector3(0, 0, 0), new Vector3(x - 16, y - 15, 0), Color.White);
        }

        #endregion

        #region Reset/Dispose

        /// <summary>
        /// Attempt to recover the device if it is lost.
        /// </summary>
        protected void AttemptRecovery()
        {
            if (_device == null) return;

            Result result = _device.TestCooperativeLevel();
            if (result == ResultCode.DeviceLost) return;
            if (result == ResultCode.DeviceNotReset)
            {
                try
                {
                    ResetDevice();
                }
                catch (SharpDXException ex)
                {
                    // If it's still lost or lost again, just do nothing
                    if (ex.ResultCode == ResultCode.DeviceLost) return;
                    else if (ex.ResultCode == ResultCode.OutOfVideoMemory) throw ex;
                    else throw ex;
                }
            }
        }
        public void ResetDevice()
        {
            if (_parent != null && _device != null)
            {
                DisposeDeviceResources();
                _parent.DisposeTextures();
                try
                {
                    _device.Reset(presentParams);
                    isDeviceLost = false;
                    InitDeviceResources();
                }
                catch
                {                 
                    _device.Dispose();
                    direct3d.Dispose();
                    _device = null;
                    direct3d = null;
                    Init(_parent);
                    isDeviceLost = false;
                    _parent.Reload();
                }
            }
        }
        public void DisposeDeviceResources()
        {
            if (tx != null)
            {
                tx.Dispose();
                tx = null;
            }
            if (hvcursor != null)
            {
                hvcursor.Dispose();
                hvcursor = null;
            }
            if (vcursor != null)
            {
                vcursor.Dispose();
                vcursor = null;
            }
            if (hcursor != null)
            {
                hcursor.Dispose();
                hcursor = null;
            }

            if (FontDX != null)
            {
                FontDX.Dispose();
                FontDX = null;
            }
            if (FontDX_Bold != null)
            {
                FontDX_Bold.Dispose();
                FontDX_Bold = null;
            }

            if (sprite != null)
            {
                sprite.Dispose();
                sprite = null;
            }
            if (sprite2 != null)
            {
                sprite2.Dispose();
                sprite2 = null;
            }
            if (tx_ellipses != null)
            {
                foreach (var tx_ellipse in tx_ellipses)
                {
                    tx_ellipses[tx_ellipse.Key].Dispose();
                }
                tx_ellipses.Clear();
                tx_ellipses = null;
            }

        }
        public void DisposePanel()
        {
            DisposeDeviceResources();
            _parent.DisposeTextures();
            _device.Dispose();
            direct3d.Dispose();
            base.Dispose();
        }

        #endregion

        #region Misc

        private void MakeGray(Bitmap image)
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

        #endregion





    }
}
