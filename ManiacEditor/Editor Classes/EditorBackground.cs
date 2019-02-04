using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using RSDKv5Color = RSDKv5.Color;
using IronPython.Modules;
using System.Net.Http.Headers;
using OpenTK.Platform.Windows;
using OpenTK.Graphics.OpenGL;
using PrimitiveType = OpenTK.Graphics.OpenGL.PrimitiveType;

namespace ManiacEditor
{
    class EditorBackground : IDrawable
    {
        const int BOX_SIZE = 8;
        const int TILE_BOX_SIZE = 1;
        public int GRID_TILE_SIZE = 16;
        public Editor EditorInstance;

		int width;
		int height;

		public EditorBackground(Editor instance)
        {
            EditorInstance = instance;
        }

		public EditorBackground(Editor instance, int width, int height)
		{
			this.width = width;
			this.height = height;
		}


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

            RSDKv5Color rcolor1 = EditorInstance.EditorScene.EditorMetadata.BackgroundColor1;
            RSDKv5Color rcolor2 = EditorInstance.EditorScene.EditorMetadata.BackgroundColor2;

            Color color1 = Color.FromArgb(rcolor1.A, rcolor1.R, rcolor1.G, rcolor1.B);
            Color color2 = Color.FromArgb(rcolor2.A, rcolor2.R, rcolor2.G, rcolor2.B);

            int start_x = screen.X / (BOX_SIZE * EditorLayer.TILE_SIZE);
            int end_x = Math.Min(DivideRoundUp(screen.X + screen.Width, BOX_SIZE * EditorLayer.TILE_SIZE), EditorInstance.SceneWidth);
            int start_y = screen.Y / (BOX_SIZE * EditorLayer.TILE_SIZE);
            int end_y = Math.Min(DivideRoundUp(screen.Y + screen.Height, BOX_SIZE * EditorLayer.TILE_SIZE), EditorInstance.SceneHeight);

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

            RSDKv5Color rcolor1 = EditorInstance.EditorScene.EditorMetadata.BackgroundColor1;
            RSDKv5Color rcolor2 = EditorInstance.EditorScene.EditorMetadata.BackgroundColor2;

            Color color1 = Color.FromArgb(30, rcolor1.R, rcolor1.G, rcolor1.B);
            Color color2 = Color.FromArgb(30, rcolor2.R, rcolor2.G, rcolor2.B);

            int start_x = screen.X / (BOX_SIZE * EditorLayer.TILE_SIZE);
            int end_x = Math.Min(DivideRoundUp(screen.X + screen.Width, BOX_SIZE * EditorLayer.TILE_SIZE), EditorInstance.SceneWidth);
            int start_y = screen.Y / (BOX_SIZE * EditorLayer.TILE_SIZE);
            int end_y = Math.Min(DivideRoundUp(screen.Y + screen.Height, BOX_SIZE * EditorLayer.TILE_SIZE), EditorInstance.SceneHeight);

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

			Color GridColor = Color.FromArgb((int)EditorInstance.gridOpacitySlider.Value, Properties.Settings.Default.GridColorDefault.R, Properties.Settings.Default.GridColorDefault.B, Properties.Settings.Default.GridColorDefault.G);

            int start_x = screen.X / (TILE_BOX_SIZE * GRID_TILE_SIZE);
            int end_x = Math.Min(DivideRoundUp(screen.X + screen.Width, TILE_BOX_SIZE * GRID_TILE_SIZE), EditorInstance.SceneWidth);
            int start_y = screen.Y / (TILE_BOX_SIZE * GRID_TILE_SIZE);
            int end_y = Math.Min(DivideRoundUp(screen.Y + screen.Height, TILE_BOX_SIZE * GRID_TILE_SIZE), EditorInstance.SceneHeight);


                for (int y = start_y; y < end_y; ++y)
                {
                    for (int x = start_x; x < end_x; ++x)
                    {
                            d.DrawLine(x * GRID_TILE_SIZE, y * GRID_TILE_SIZE, x * GRID_TILE_SIZE + GRID_TILE_SIZE, y * GRID_TILE_SIZE, GridColor);
                            d.DrawLine(x * GRID_TILE_SIZE, y * GRID_TILE_SIZE, x * GRID_TILE_SIZE, y * GRID_TILE_SIZE + GRID_TILE_SIZE, GridColor);
                            d.DrawLine(x * GRID_TILE_SIZE + GRID_TILE_SIZE, y * GRID_TILE_SIZE + GRID_TILE_SIZE, x * GRID_TILE_SIZE + GRID_TILE_SIZE, y * GRID_TILE_SIZE, GridColor);
                            d.DrawLine(x * GRID_TILE_SIZE + GRID_TILE_SIZE, y * GRID_TILE_SIZE + GRID_TILE_SIZE, x * GRID_TILE_SIZE, y * GRID_TILE_SIZE + GRID_TILE_SIZE, GridColor);
                    }
                }
        }

        public void DrawNight(DevicePanel d)
        {
            Rectangle screen = d.GetScreen();

            int start_x = screen.X / (TILE_BOX_SIZE * GRID_TILE_SIZE);
            int end_x = Math.Min(DivideRoundUp(screen.X + screen.Width, TILE_BOX_SIZE * GRID_TILE_SIZE), EditorInstance.SceneWidth);
            int start_y = screen.Y / (TILE_BOX_SIZE * GRID_TILE_SIZE);
            int end_y = Math.Min(DivideRoundUp(screen.Y + screen.Height, TILE_BOX_SIZE * GRID_TILE_SIZE), EditorInstance.SceneHeight);


            d.DrawRectangle(screen.X, screen.Y, screen.X + screen.Width, screen.Y + screen.Height, Color.FromArgb(100, 0, 0, 0));
        }


    }
}
