﻿using System;
using System.Drawing;
using RSDKv5Color = RSDKv5.Color;

namespace ManiacEditor.Classes.Editor.Scene
{
    public class EditorBackground : IDrawable
    {

        public ManiacEditor.Controls.Base.MainEditor EditorInstance;

		int width;
		int height;

		public EditorBackground(ManiacEditor.Controls.Base.MainEditor instance)
        {
            EditorInstance = instance;
        }

		public EditorBackground(ManiacEditor.Controls.Base.MainEditor instance, int width, int height)
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

            RSDKv5Color rcolor1 = Classes.Editor.Solution.CurrentScene.EditorMetadata.BackgroundColor1;
            RSDKv5Color rcolor2 = Classes.Editor.Solution.CurrentScene.EditorMetadata.BackgroundColor2;

            Color color1 = Color.FromArgb(rcolor1.A, rcolor1.R, rcolor1.G, rcolor1.B);
            Color color2 = Color.FromArgb(rcolor2.A, rcolor2.R, rcolor2.G, rcolor2.B);

            int start_x = screen.X / (Classes.Editor.Constants.BOX_SIZE * Classes.Editor.Constants.TILE_SIZE);
            int end_x = Math.Min(DivideRoundUp(screen.X + screen.Width, Classes.Editor.Constants.BOX_SIZE * Classes.Editor.Constants.TILE_SIZE), Classes.Editor.Solution.SceneWidth);
            int start_y = screen.Y / (Classes.Editor.Constants.BOX_SIZE * Classes.Editor.Constants.TILE_SIZE);
            int end_y = Math.Min(DivideRoundUp(screen.Y + screen.Height, Classes.Editor.Constants.BOX_SIZE * Classes.Editor.Constants.TILE_SIZE), Classes.Editor.Solution.SceneHeight);

            // Draw with first color everything
            d.DrawRectangle(screen.X, screen.Y, screen.X + screen.Width, screen.Y + screen.Height, color1);

            if (color2.A != 0) {
                for (int y = start_y; y < end_y; ++y)
                {
                    for (int x = start_x; x < end_x; ++x)
                    {
                        if ((x + y) % 2 == 1) d.DrawRectangle(x * Classes.Editor.Constants.BOX_SIZE * Classes.Editor.Constants.TILE_SIZE, y * Classes.Editor.Constants.BOX_SIZE * Classes.Editor.Constants.TILE_SIZE, (x + 1) * Classes.Editor.Constants.BOX_SIZE * Classes.Editor.Constants.TILE_SIZE, (y + 1) * Classes.Editor.Constants.BOX_SIZE * Classes.Editor.Constants.TILE_SIZE, color2);
                    }
                }
            }
        }

		public void DrawEdit(DevicePanel d)
        {
            Rectangle screen = d.GetScreen();

            RSDKv5Color rcolor1 = Classes.Editor.Solution.CurrentScene.EditorMetadata.BackgroundColor1;
            RSDKv5Color rcolor2 = Classes.Editor.Solution.CurrentScene.EditorMetadata.BackgroundColor2;

            Color color1 = Color.FromArgb(30, rcolor1.R, rcolor1.G, rcolor1.B);
            Color color2 = Color.FromArgb(30, rcolor2.R, rcolor2.G, rcolor2.B);

            int start_x = screen.X / (Classes.Editor.Constants.BOX_SIZE * Classes.Editor.Constants.TILE_SIZE);
            int end_x = Math.Min(DivideRoundUp(screen.X + screen.Width, Classes.Editor.Constants.BOX_SIZE * Classes.Editor.Constants.TILE_SIZE), Classes.Editor.Solution.SceneWidth);
            int start_y = screen.Y / (Classes.Editor.Constants.BOX_SIZE * Classes.Editor.Constants.TILE_SIZE);
            int end_y = Math.Min(DivideRoundUp(screen.Y + screen.Height, Classes.Editor.Constants.BOX_SIZE * Classes.Editor.Constants.TILE_SIZE), Classes.Editor.Solution.SceneHeight);

            // Draw with first color everything
            d.DrawRectangle(screen.X, screen.Y, screen.X + screen.Width, screen.Y + screen.Height, color1);

            if (color2.A != 0)
            {
                for (int y = start_y; y < end_y; ++y)
                {
                    for (int x = start_x; x < end_x; ++x)
                    {
                        if ((x + y) % 2 == 1) d.DrawRectangle(x * Classes.Editor.Constants.BOX_SIZE * Classes.Editor.Constants.TILE_SIZE, y * Classes.Editor.Constants.BOX_SIZE * Classes.Editor.Constants.TILE_SIZE, (x + 1) * Classes.Editor.Constants.BOX_SIZE * Classes.Editor.Constants.TILE_SIZE, (y + 1) * Classes.Editor.Constants.BOX_SIZE * Classes.Editor.Constants.TILE_SIZE, color2);
                    }
                }
            }
        }

        public void DrawGrid(DevicePanel d)
        {
            int GridSize = (EditorInstance != null ? Classes.Editor.SolutionState.GridSize : 0);
            Rectangle screen = d.GetScreen();

			Color GridColor = Color.FromArgb((int)EditorInstance.EditorToolbar.gridOpacitySlider.Value, Classes.Editor.SolutionState.GridColor.R, Classes.Editor.SolutionState.GridColor.B, Classes.Editor.SolutionState.GridColor.G);

            int start_x = screen.X / (Classes.Editor.Constants.TILE_BOX_SIZE * GridSize);
            int end_x = Math.Min(DivideRoundUp(screen.X + screen.Width, Classes.Editor.Constants.TILE_BOX_SIZE * GridSize), Classes.Editor.Solution.SceneWidth);
            int start_y = screen.Y / (Classes.Editor.Constants.TILE_BOX_SIZE * GridSize);
            int end_y = Math.Min(DivideRoundUp(screen.Y + screen.Height, Classes.Editor.Constants.TILE_BOX_SIZE * GridSize), Classes.Editor.Solution.SceneHeight);


                for (int y = start_y; y < end_y; ++y)
                {
                    for (int x = start_x; x < end_x; ++x)
                    {
                            d.DrawLine(x * GridSize, y * GridSize, x * GridSize + GridSize, y * GridSize, GridColor);
                            d.DrawLine(x * GridSize, y * GridSize, x * GridSize, y * GridSize + GridSize, GridColor);
                            d.DrawLine(x * GridSize + GridSize, y * GridSize + GridSize, x * GridSize + GridSize, y * GridSize, GridColor);
                            d.DrawLine(x * GridSize + GridSize, y * GridSize + GridSize, x * GridSize, y * GridSize + GridSize, GridColor);
                    }
                }
        }


	}
}