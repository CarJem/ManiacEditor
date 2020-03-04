using System;
using SFML.Graphics;
using SFML.Window;
using SFML.System;

namespace ManiacEditor.Controls.Editor.Elements.View
{
    /// <summary>
    /// Interaction logic for RenderWindow.xaml
    /// </summary>
    public partial class RenderPanel : System.Windows.Controls.UserControl
    {
        #region New Rendering Code
        private RenderWindow _RenderWindow;

        public RenderPanel()
        {
            InitializeComponent();

            //need to use this to prevent base.OnPaint and base.OnPaintBackground from erasing contents
            var mysurf = new MyDrawingSurface();
            this.FormHost.Child = mysurf;
            SetDoubleBuffered(mysurf); //same results whether or not i do this.

            var context = new ContextSettings { DepthBits = 24 };
            this._RenderWindow = new RenderWindow(mysurf.Handle);
            this._RenderWindow.MouseButtonPressed += _RenderWindow_MouseButtonPressed;

            var timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 1000 / 60);
            timer.Tick += Render;
            timer.Start();
        }

        public static void SetDoubleBuffered(System.Windows.Forms.Control c)
        {
            System.Reflection.PropertyInfo aProp = typeof(System.Windows.Forms.Control).GetProperty("DoubleBuffered", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            aProp.SetValue(c, true, null);
        }

        void _RenderWindow_MouseButtonPressed(object sender, MouseButtonEventArgs e)
        {
            //e.X and e.Y are actually what they should be. top-left of the visible area is 0,0
        }

        void Render(object sender, EventArgs e)
        {
            this._RenderWindow.DispatchEvents();

            Random rand = new Random();
            var clearColor = new Color((byte)rand.Next(255), (byte)rand.Next(255), (byte)rand.Next(255));
            this._RenderWindow.Clear(clearColor); //this works, it flashes all sorts of colors

            //trying several types of things to draw in the hopes that one will work. so far no luck.
            var rect = new RectangleShape();
            rect.Position = new Vector2f(-10, -10);
            rect.Size = new Vector2f(100, 200);
            rect.FillColor = Color.Blue;

            Vertex[] line = new Vertex[]
                    {
                        new Vertex(new Vector2f(0,0)) {Color = Color.White},
                        new Vertex(new Vector2f(50,100)) {Color = Color.Red}
                    };

            //tried drawing before clearing, after calling Display(), none worked.
            this._RenderWindow.Draw(line, PrimitiveType.Lines);
            this._RenderWindow.Draw(rect);

            this._RenderWindow.Display();
        }

        public class MyDrawingSurface : System.Windows.Forms.Control
        {
            protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
            {
                //base.OnPaint(e);
            }

            protected override void OnPaintBackground(System.Windows.Forms.PaintEventArgs pevent)
            {
                //base.OnPaintBackground(pevent);
            }
        }


        #region Drawing

        public void DrawBitmap(SFML.Graphics.Texture image, int x, int y, int width, int height, bool selected, int transparency)
        {

        }

        public void DrawBitmap(SFML.Graphics.Texture image, int x, int y, int rect_x, int rect_y, int width, int height, bool selected, int transparency, bool fliph = false, bool flipv = false, int rotation = 0)
        {

        }

        public void DrawLine(int X1, int Y1, int X2, int Y2, Color color = new Color(), bool useZoomOffseting = false)
        {

        }

        public void DrawLinePaperRoller(int X1, int Y1, int X2, int Y2, Color color, Color color2, Color color3, Color color4)
        {

        }

        public void DrawArrow(int x1, int y1, int x2, int y2, Color color)
        {
            int x3, y3, x4, y4;

            double angle = Math.Atan2(y2 - y1, x2 - x1) + Math.PI;

            x3 = (int)(x2 + 10 * Math.Cos(angle - Math.PI / 8));
            y3 = (int)(y2 + 10 * Math.Sin(angle - Math.PI / 8));
            x4 = (int)(x2 + 10 * Math.Cos(angle + Math.PI / 8));
            y4 = (int)(y2 + 10 * Math.Sin(angle + Math.PI / 8));

            DrawLine(x2, y2, x1, y1, color);
            DrawLine(x2, y2, x3, y3, color);
            DrawLine(x2, y2, x4, y4, color);
        }

        public void DrawBézierSplineCubic(int x1, int y1, int x2, int y2, int x3, int y3, int x4, int y4, Color color)
        {

        }

        public void DrawBézierSplineQuadratic(int x1, int y1, int x2, int y2, int x3, int y3, Color color)
        {

        }

        #endregion

        #endregion
    }
}