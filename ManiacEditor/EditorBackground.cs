using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using RSDKv5Color = RSDKv5.Color;
using IronPython.Modules;
using System.Net.Http.Headers;

namespace ManiacEditor
{
    class EditorBackground : IDrawable
    {
        const int BOX_SIZE = 8;
        const int TILE_BOX_SIZE = 1;
        public int GRID_TILE_SIZE = 16;

        //Let it snow
        int i = 0;
        snowflake[] snowflakes = new snowflake[256];

        static int DivideRoundUp(int number, int by)
        {
            return (number + by - 1) / by;
        }

        public void Draw(Graphics g)
        {
            
        }

        public void Draw(DevicePanel d)
        {
            Rectangle screen = d.GetScreen();

            RSDKv5Color rcolor1 = Editor.Instance.EditorScene.EditorMetadata.BackgroundColor1;
            RSDKv5Color rcolor2 = Editor.Instance.EditorScene.EditorMetadata.BackgroundColor2;

            Color color1 = Color.FromArgb(rcolor1.A, rcolor1.R, rcolor1.G, rcolor1.B);
            Color color2 = Color.FromArgb(rcolor2.A, rcolor2.R, rcolor2.G, rcolor2.B);

            int start_x = screen.X / (BOX_SIZE * EditorLayer.TILE_SIZE);
            int end_x = Math.Min(DivideRoundUp(screen.X + screen.Width, BOX_SIZE * EditorLayer.TILE_SIZE), Editor.Instance.SceneWidth);
            int start_y = screen.Y / (BOX_SIZE * EditorLayer.TILE_SIZE);
            int end_y = Math.Min(DivideRoundUp(screen.Y + screen.Height, BOX_SIZE * EditorLayer.TILE_SIZE), Editor.Instance.Height);

            // Draw with first color everything
            d.DrawRectangle(screen.X, screen.Y, screen.X + screen.Width, screen.Y + screen.Height, color1);

            if (color2.A != 0) {
                for (int y = start_y; y < end_y; ++y)
                {
                    for (int x = start_x; x < end_x; ++x)
                    {
                        if ((x + y) % 2 == 1) d.DrawRectangle(x * BOX_SIZE * EditorLayer.TILE_SIZE, y * BOX_SIZE * EditorLayer.TILE_SIZE, (x + 1) * BOX_SIZE * EditorLayer.TILE_SIZE, (y + 1) * BOX_SIZE * EditorLayer.TILE_SIZE, color2);
                    }
                }
            }
        }

        public void DrawEdit(DevicePanel d)
        {
            Rectangle screen = d.GetScreen();

            RSDKv5Color rcolor1 = Editor.Instance.EditorScene.EditorMetadata.BackgroundColor1;
            RSDKv5Color rcolor2 = Editor.Instance.EditorScene.EditorMetadata.BackgroundColor2;

            Color color1 = Color.FromArgb(30, rcolor1.R, rcolor1.G, rcolor1.B);
            Color color2 = Color.FromArgb(30, rcolor2.R, rcolor2.G, rcolor2.B);

            int start_x = screen.X / (BOX_SIZE * EditorLayer.TILE_SIZE);
            int end_x = Math.Min(DivideRoundUp(screen.X + screen.Width, BOX_SIZE * EditorLayer.TILE_SIZE), Editor.Instance.SceneWidth);
            int start_y = screen.Y / (BOX_SIZE * EditorLayer.TILE_SIZE);
            int end_y = Math.Min(DivideRoundUp(screen.Y + screen.Height, BOX_SIZE * EditorLayer.TILE_SIZE), Editor.Instance.Height);

            // Draw with first color everything
            d.DrawRectangle(screen.X, screen.Y, screen.X + screen.Width, screen.Y + screen.Height, color1);

            if (color2.A != 0)
            {
                for (int y = start_y; y < end_y; ++y)
                {
                    for (int x = start_x; x < end_x; ++x)
                    {
                        if ((x + y) % 2 == 1) d.DrawRectangle(x * BOX_SIZE * EditorLayer.TILE_SIZE, y * BOX_SIZE * EditorLayer.TILE_SIZE, (x + 1) * BOX_SIZE * EditorLayer.TILE_SIZE, (y + 1) * BOX_SIZE * EditorLayer.TILE_SIZE, color2);
                    }
                }
            }
        }

        public void DrawGrid(DevicePanel d)
        {
            Rectangle screen = d.GetScreen();

            int start_x = screen.X / (TILE_BOX_SIZE * GRID_TILE_SIZE);
            int end_x = Math.Min(DivideRoundUp(screen.X + screen.Width, TILE_BOX_SIZE * GRID_TILE_SIZE), Editor.Instance.SceneWidth);
            int start_y = screen.Y / (TILE_BOX_SIZE * GRID_TILE_SIZE);
            int end_y = Math.Min(DivideRoundUp(screen.Y + screen.Height, TILE_BOX_SIZE * GRID_TILE_SIZE), Editor.Instance.Height);


                for (int y = start_y; y < end_y; ++y)
                {
                    for (int x = start_x; x < end_x; ++x)
                    {
                            d.DrawLine(x * GRID_TILE_SIZE, y * GRID_TILE_SIZE, x * GRID_TILE_SIZE + GRID_TILE_SIZE, y * GRID_TILE_SIZE, Properties.Settings.Default.GridColorDefault);
                            d.DrawLine(x * GRID_TILE_SIZE, y * GRID_TILE_SIZE, x * GRID_TILE_SIZE, y * GRID_TILE_SIZE + GRID_TILE_SIZE, Properties.Settings.Default.GridColorDefault);
                            d.DrawLine(x * GRID_TILE_SIZE + GRID_TILE_SIZE, y * GRID_TILE_SIZE + GRID_TILE_SIZE, x * GRID_TILE_SIZE + GRID_TILE_SIZE, y * GRID_TILE_SIZE, Properties.Settings.Default.GridColorDefault);
                            d.DrawLine(x * GRID_TILE_SIZE + GRID_TILE_SIZE, y * GRID_TILE_SIZE + GRID_TILE_SIZE, x * GRID_TILE_SIZE, y * GRID_TILE_SIZE + GRID_TILE_SIZE, Properties.Settings.Default.GridColorDefault);
                    }
                }
        }

        public void DrawSnow(DevicePanel d)
        {
            Rectangle screen = d.GetScreen();

            int start_x = screen.X / (BOX_SIZE * EditorLayer.TILE_SIZE);
            int end_x = Math.Min(DivideRoundUp(screen.X + screen.Width, BOX_SIZE * EditorLayer.TILE_SIZE), Editor.Instance.SceneWidth);
            int start_y = screen.Y / (BOX_SIZE * EditorLayer.TILE_SIZE);
            int end_y = Math.Min(DivideRoundUp(screen.Y + screen.Height, BOX_SIZE * EditorLayer.TILE_SIZE), Editor.Instance.Height);

            LetItSnow();
            foreach (snowflake flake in snowflakes)
            {
                if (flake != null)
                {
                    d.DrawSnowLines(flake.Location.X, flake.Location.Y, flake.Location.X, flake.Location.Y, flake.Color);
                }
            }

        }

        public void DrawNight(DevicePanel d)
        {
            Rectangle screen = d.GetScreen();

            int start_x = screen.X / (TILE_BOX_SIZE * GRID_TILE_SIZE);
            int end_x = Math.Min(DivideRoundUp(screen.X + screen.Width, TILE_BOX_SIZE * GRID_TILE_SIZE), Editor.Instance.SceneWidth);
            int start_y = screen.Y / (TILE_BOX_SIZE * GRID_TILE_SIZE);
            int end_y = Math.Min(DivideRoundUp(screen.Y + screen.Height, TILE_BOX_SIZE * GRID_TILE_SIZE), Editor.Instance.Height);


            d.DrawRectangle(screen.X, screen.Y, screen.X + screen.Width, screen.Y + screen.Height, Color.FromArgb(100, 0, 0, 0));
        }

        public void LetItSnow()
        {
            if (i >= 256)
            {
                return;
            }
            snowflakes[i] = new snowflake();
            i++;
        }

        class snowflake : snowParticle
        {
            public snowflake()
            {
                create();
                move();
            }

            Random r = new Random();

            private void create()
            {
                this.Location = new Point(r.Next(-Editor.Instance.GraphicPanel.DrawWidth, Editor.Instance.GraphicPanel.DrawWidth), r.Next(-Editor.Instance.GraphicPanel.DrawHeight, Editor.Instance.GraphicPanel.DrawHeight));

                this.MinimumSize = new Size(3, 3);
                this.Size = new Size(5, 5);
                this.Color = Color.White;

            }

            public void move()
            {
                this.Location.Offset(0, 3);
                System.Windows.Forms.Timer t = new System.Windows.Forms.Timer();
                t.Interval = 5;
                t.Tick += new EventHandler(t_Tick);
                t.Start();
            }

            void t_Tick(object sender, EventArgs e)
            {
                this.Location.Offset(0, 3);
                if (this.Location.X > Editor.Instance.GraphicPanel.DrawWidth || this.Location.Y > Editor.Instance.GraphicPanel.DrawHeight)
                    this.Location = new Point(r.Next(-Editor.Instance.GraphicPanel.DrawWidth, Editor.Instance.GraphicPanel.DrawWidth), r.Next(-Editor.Instance.GraphicPanel.DrawHeight, Editor.Instance.GraphicPanel.DrawHeight));

            }
        }

        class snowParticle
        {
            public Point Location;
            public Size MinimumSize;
            public Size Size;
            public Color Color;
        }

    }
}
