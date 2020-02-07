using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SharpDX;
using SharpDX.Direct3D9;
using SharpDX.Windows;
using Font = SharpDX.Direct3D9.Font;
using Rectangle = System.Drawing.Rectangle;
using Color = System.Drawing.Color;
using Point = System.Drawing.Point;
using Bitmap = System.Drawing.Bitmap;
using ManiacEditor.Event_Handlers;


/*
 * This file is a ported old rendering code
 * It will be replaced with OpenGL soon (Update: Or never if this pace keeps up - CarJem Generations)
 */


namespace ManiacEditor
{
    public partial class DevicePanel : UserControl
    {

        #region Members



        public bool mouseMoved = false;

        public int DrawWidth { get; set; }
        public int DrawHeight { get; set; }

        public int ScreenPosWidth;
        public int ScreenPosHeight;

        private Sprite sprite;
        private Sprite sprite2;
        Texture tx;
        Bitmap txb;
        Texture hvcursor;
        Bitmap hvcursorb;
        Texture vcursor;
        Bitmap vcursorb;
        Texture hcursor;
        Bitmap hcursorb;


        public bool bRender = true;

        // The DirectX device
        public DeviceEx _device = null;
        public bool deviceLost;
        private Direct3DEx direct3d = new Direct3DEx();
        private Font font;
        private Font fontBold;
        // The Form to place the DevicePanel onto
        public IDrawArea _parent = null;
        private PresentParameters presentParams;
        // On this event we can start to render our scene
        public event RenderEventHandler OnRender;

        // Now we know that the device is created
        public event CreateDeviceEventHandler OnCreateDevice;

        public MouseEventArgs lastEvent;


        public static FontDescription fontDescription = new FontDescription()
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

        public static FontDescription fontDescriptionBold = new FontDescription()
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

        #region Constructor

        public DevicePanel(Controls.Editor.MainEditor instance = null)
        {
            InitializeComponent();
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
                presentParams.SwapEffect = SwapEffect.Discard;

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


                _device = new DeviceEx(direct3d, 0, DeviceType.Hardware, this.Handle, createFlags, presentParams);
                _device.SetSamplerState(0, SamplerState.MinFilter, TextureFilter.None);
                _device.SetSamplerState(0, SamplerState.MagFilter, TextureFilter.None);
                _device.SetSamplerState(0, SamplerState.MipFilter, TextureFilter.None);
                //_device.SetRenderState(RenderState.ZWriteEnable, false);
                //_device.SetRenderState(RenderState.ZEnable, false);

                if (OnCreateDevice != null)
                {
                    OnCreateDevice(this, new DeviceEventArgs(_device));
                }



                txb = new Bitmap(1, 1);
                using (Graphics g = Graphics.FromImage(txb))
                    g.Clear(Color.White);
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



        public void Run()
        {
          
            RenderLoop.Run(this, () =>
            {
                // Another option is not use RenderLoop at all and call Render when needed, and call here every tick for animations
                if (bRender) Render();
                if (mouseMoved)
                {
                    OnMouseMove(lastEvent);
                    mouseMoved = false;
                }
                Application.DoEvents();
            });
        }

        public void InitDeviceResources()
        {
            sprite = new Sprite(_device);
            sprite2 = new Sprite(_device);

            tx = Classes.Editor.Draw.TextureCreator.FromBitmap(_device, txb);
            hcursor = Classes.Editor.Draw.TextureCreator.FromBitmap(_device, hcursorb);
            vcursor = Classes.Editor.Draw.TextureCreator.FromBitmap(_device, vcursorb);
            hvcursor = Classes.Editor.Draw.TextureCreator.FromBitmap(_device, hvcursorb);

            font = new Font(_device, fontDescription);
            fontBold = new Font(_device, fontDescriptionBold);
        }
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
                    else throw ex;
                }
            }
        }

        public void ResetDevice()
        {
            DisposeDeviceResources();
            _parent.DisposeTextures();
            _device.Reset(presentParams);
            deviceLost = false;
            InitDeviceResources();
        }

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

        #region Rendering

        /// <summary>
        /// Rendering-method
        /// </summary>
        /// 

        private static bool isRendering { get; set; } = false;

        public void Render()
        {
            if (deviceLost) AttemptRecovery();
            if (deviceLost) return;
            //if (isRendering) return;

            if (_device == null)
                return;

            isRendering = true;
            try
            {

                Rectangle screen = _parent.GetScreen();
                double zoom = _parent.GetZoom();

                //Clear the backbuffer
                _device.Clear(ClearFlags.Target, new SharpDX.Color(_deviceBackColor.R, _deviceBackColor.B, _deviceBackColor.G, _deviceBackColor.A), 1.0f, 0);

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

                sprite.Begin(SpriteFlags.AlphaBlend | SpriteFlags.DoNotModifyRenderState);

                // Render of scene here
                if (OnRender != null) OnRender(this, new DeviceEventArgs(_device));

                sprite.Transform = Matrix.Scaling(1f, 1f, 1f);

                sprite.End();
                sprite2.End();
                //End the scene
                _device.EndScene();
                _device.Present();

            }
            catch (SharpDXException ex)
            {
                if (ex.ResultCode == ResultCode.DeviceLost)
                    deviceLost = true;
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
            lastEvent = e;
            base.OnMouseMove(new MouseEventArgs(e.Button, e.Clicks, e.X + _parent.GetScreen().X, e.Y + _parent.GetScreen().Y, e.Delta));
        }

        protected override void OnMouseWheel(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseWheel(new MouseEventArgs(e.Button, e.Clicks, e.X + _parent.GetScreen().X, e.Y + _parent.GetScreen().Y, e.Delta));
        }

        protected override void OnMouseClick(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseClick(new MouseEventArgs(e.Button, e.Clicks, e.X + _parent.GetScreen().X, e.Y + _parent.GetScreen().Y, e.Delta));
        }

        protected override void OnMouseDoubleClick(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseDoubleClick(new MouseEventArgs(e.Button, e.Clicks, e.X + _parent.GetScreen().X, e.Y + _parent.GetScreen().Y, e.Delta));
        }

        protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseDown(new MouseEventArgs(e.Button, e.Clicks, e.X + _parent.GetScreen().X, e.Y + _parent.GetScreen().Y, e.Delta));
        }

        protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseUp(new MouseEventArgs(e.Button, e.Clicks, e.X + _parent.GetScreen().X, e.Y + _parent.GetScreen().Y, e.Delta));
        }

        #endregion

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

        private void DrawTexture(Texture image, Rectangle srcRect, Vector3 center, Vector3 position, Color color)
        {
            sprite.Draw(image, new SharpDX.Color(color.R, color.G, color.B, color.A), new SharpDX.Rectangle(srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height), center, position);
        }
        private void DrawTexture2(Texture image, Rectangle srcRect, Vector3 center, Vector3 position, Color color)
        {
            sprite2.Draw(image, new SharpDX.Color(color.R, color.G, color.B, color.A), new SharpDX.Rectangle(srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height), center, position);

        }

        public void DrawBitmap(Texture image, int x, int y, int width, int height, bool selected, int transparency, Color? CustomColor = null)
        {
            if (!IsObjectOnScreen(x, y, width, height)) return;

            Color CustomSelectedColor = Color.BlueViolet;
            if (CustomColor == null)
            {
                CustomColor = Color.White;
            }
            else
            {
                CustomSelectedColor = Extensions.Extensions.Blend(Color.Purple, CustomColor.Value, 100);
            }
            Rectangle screen = _parent.GetScreen();
            double zoom = _parent.GetZoom();
            DrawTexture(image, new Rectangle(0, 0, width, height), new Vector3(), new Vector3(x - (int)(screen.X / zoom), y - (int)(screen.Y / zoom), 0), (selected) ? CustomSelectedColor : Color.FromArgb(transparency, CustomColor.Value));
        }

        public void DrawLine(int X1, int Y1, int X2, int Y2, Color color = new Color(), bool useZoomOffseting = false)
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
            sprite.Transform = Matrix.Scaling((float)zoom, (float)zoom, 1f);
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
        }



        int getPt(int n1, int n2, double perc)
        {
            int diff = n2 - n1;

            return (int)(n1 + (diff * perc));
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

        void DrawLinePBPDoted(int x0, int y0, int x1, int y1, Color color, Color color2, Color color3, Color color4)
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

        public void DrawRectangle(int x1, int y1, int x2, int y2, Color color)
        {
            if (!IsObjectOnScreen(x1, y1, x2 - x1, y2 - y1)) return;
            Rectangle screen = _parent.GetScreen();
            double zoom = _parent.GetZoom();

            DrawTexture(tx, new Rectangle(0, 0, x2 - x1, y2 - y1), new Vector3(0, 0, 0), new Vector3(x1 - (int)(screen.X / zoom), y1 - (int)(screen.Y / zoom), 0), color);
        }

        public void DrawQuad(int x1, int y1, int x2, int y2, Color color)
        {
            if (!IsObjectOnScreen(x1, y1, x2 - x1, y2 - y1)) return;
            Rectangle screen = _parent.GetScreen();
            double zoom = _parent.GetZoom();

            DrawTexture(tx, new Rectangle(0, 0, x2 - x1, y2 - y1), new Vector3(0, 0, 0), new Vector3(x1 - (int)(screen.X / zoom), y1 - (int)(screen.Y / zoom), 0), color);
        }

        public void DrawText(string text, int x, int y, int width, Color color, bool bold)
        {
            Rectangle screen = _parent.GetScreen();
            double zoom = _parent.GetZoom();
            if (width >= 30)
            {
                ((bold) ? fontBold : font).DrawText(sprite, text, new SharpDX.Rectangle(x - (int)(screen.X / zoom), y - (int)(screen.Y / zoom), width, 1000), FontDrawFlags.WordBreak, new SharpDX.Color(color.R, color.G, color.B, color.A));
            }
            else
            {
                ((bold) ? fontBold : font).DrawText(sprite, text, x - (int)(screen.X / zoom), y - (int)(screen.Y / zoom), new SharpDX.Color(color.R, color.G, color.B, color.A));
            }
        }
        public void DrawTextSmall(string text, int x, int y, int width, Color color, bool bold)
        {
            Rectangle screen = _parent.GetScreen();
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
        }
        public void Draw2DCursor(int x, int y)
        {
            DrawTexture2(hvcursor, new Rectangle(Point.Empty, Cursors.NoMove2D.Size), new Vector3(0, 0, 0), new Vector3(x - 16, y - 15, 0), Color.White);

        }
        public void DrawHorizCursor(int x, int y)
        {
            //hcursor = new Texture(_device, hcursorb, Usage.Dynamic, Pool.Default);
            DrawTexture2(hcursor, new Rectangle(Point.Empty, Cursors.NoMove2D.Size), new Vector3(0, 0, 0), new Vector3(x - 16, y - 15, 0), Color.White);
        }

        public void DrawVertCursor(int x, int y)
        {
            //vcursor = new Texture(_device, vcursorb, Usage.Dynamic, Pool.Default);
            DrawTexture2(vcursor, new Rectangle(Point.Empty, Cursors.NoMove2D.Size), new Vector3(0, 0, 0), new Vector3(x - 16, y - 15, 0), Color.White);
            DrawTexture2(hcursor, new Rectangle(Point.Empty, Cursors.NoMove2D.Size), new Vector3(0, 0, 0), new Vector3(x - 16, y - 15, 0), Color.White);
        }
        public void OnMouseMoveEventCreate()
        {
            Cursor.Position = new Point(Cursor.Position.X, Cursor.Position.Y);
        }

        public void DisposeDeviceResources(int type = 0)
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

            if (font != null)
            {
                font.Dispose();
                font = null;
            }
            if (fontBold != null)
            {
                fontBold.Dispose();
                fontBold = null;
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

        }

        public new void Dispose()
        {
            DisposeDeviceResources();
            _parent.DisposeTextures();
            _device.Dispose();
            direct3d.Dispose();
            base.Dispose();
        }


    }
}
