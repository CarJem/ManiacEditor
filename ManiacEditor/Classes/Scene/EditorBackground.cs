using System;
using System.Drawing;
using RSDKv5Color = RSDKv5.Color;

namespace ManiacEditor.Classes.Scene
{
    public class EditorBackground : IDrawable
    {

        public ManiacEditor.Controls.Editor.MainEditor EditorInstance;

		int width;
		int height;

		public EditorBackground(ManiacEditor.Controls.Editor.MainEditor instance)
        {
            EditorInstance = instance;
        }

		public EditorBackground(ManiacEditor.Controls.Editor.MainEditor instance, int width, int height)
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
            Draw(d, false);
        }

        public void Draw(DevicePanel d, bool drawEdit)
        {
            Rectangle screen = d.GetScreen();

            RSDKv5Color rcolor1 = Methods.Editor.Solution.CurrentScene.EditorMetadata.BackgroundColor1;
            RSDKv5Color rcolor2 = Methods.Editor.Solution.CurrentScene.EditorMetadata.BackgroundColor2;


            int transparency1 = (drawEdit ? 30 : rcolor1.A);
            int transparency2 = (drawEdit ? 30 : rcolor2.A);

            Color color1 = Color.FromArgb(transparency1, rcolor1.R, rcolor1.G, rcolor1.B);
            Color color2 = Color.FromArgb(transparency2, rcolor2.R, rcolor2.G, rcolor2.B);

            int start_x;
            int end_x;
            int start_y;
            int end_y;

            if (!Methods.Editor.SolutionState.UnlockCamera)
            {
                start_x = screen.X / (Methods.Editor.EditorConstants.BOX_SIZE * Methods.Editor.EditorConstants.TILE_SIZE);
                end_x = Math.Min(DivideRoundUp(screen.X + screen.Width, Methods.Editor.EditorConstants.BOX_SIZE * Methods.Editor.EditorConstants.TILE_SIZE), Methods.Editor.Solution.SceneWidth);
                start_y = screen.Y / (Methods.Editor.EditorConstants.BOX_SIZE * Methods.Editor.EditorConstants.TILE_SIZE);
                end_y = Math.Min(DivideRoundUp(screen.Y + screen.Height, Methods.Editor.EditorConstants.BOX_SIZE * Methods.Editor.EditorConstants.TILE_SIZE), Methods.Editor.Solution.SceneHeight);
            }
            else
            {
                start_x = 0;
                end_x = Math.Min(DivideRoundUp(Methods.Editor.Solution.SceneWidth, Methods.Editor.EditorConstants.BOX_SIZE * Methods.Editor.EditorConstants.TILE_SIZE), (int)(Methods.Editor.Solution.SceneWidth));
                start_y = 0;
                end_y = Math.Min(DivideRoundUp(Methods.Editor.Solution.SceneHeight, Methods.Editor.EditorConstants.BOX_SIZE * Methods.Editor.EditorConstants.TILE_SIZE), (int)(Methods.Editor.Solution.SceneHeight));
            }
            

            // Draw with first color everything
            d.DrawRectangle(screen.X, screen.Y, screen.X + screen.Width, screen.Y + screen.Height, color1);

            if (color2.A != 0) {
                for (int y = start_y; y < end_y; ++y)
                {
                    for (int x = start_x; x < end_x; ++x)
                    {
                        if ((x + y) % 2 == 1) d.DrawRectangle(x * Methods.Editor.EditorConstants.BOX_SIZE * Methods.Editor.EditorConstants.TILE_SIZE, y * Methods.Editor.EditorConstants.BOX_SIZE * Methods.Editor.EditorConstants.TILE_SIZE, (x + 1) * Methods.Editor.EditorConstants.BOX_SIZE * Methods.Editor.EditorConstants.TILE_SIZE, (y + 1) * Methods.Editor.EditorConstants.BOX_SIZE * Methods.Editor.EditorConstants.TILE_SIZE, color2);
                    }
                }
            }
        }

        public void DrawGrid(DevicePanel d)
        {
            int GridSize = (EditorInstance != null ? Methods.Editor.SolutionState.GridSize : 16);
            Rectangle screen = d.GetScreen();

			Color GridColor = Color.FromArgb((int)EditorInstance.EditorToolbar.gridOpacitySlider.Value, Methods.Editor.SolutionState.GridColor.R, Methods.Editor.SolutionState.GridColor.B, Methods.Editor.SolutionState.GridColor.G);


            int start_x;
            int end_x;
            int start_y;
            int end_y;

            if (!Methods.Editor.SolutionState.UnlockCamera)
            {
                start_x = screen.X / (Methods.Editor.EditorConstants.TILE_BOX_SIZE * GridSize);
                end_x = Math.Min(DivideRoundUp(screen.X + screen.Width, Methods.Editor.EditorConstants.TILE_BOX_SIZE * GridSize), Methods.Editor.Solution.SceneWidth);
                start_y = screen.Y / (Methods.Editor.EditorConstants.TILE_BOX_SIZE * GridSize);
                end_y = Math.Min(DivideRoundUp(screen.Y + screen.Height, Methods.Editor.EditorConstants.TILE_BOX_SIZE * GridSize), Methods.Editor.Solution.SceneHeight);
            }
            else
            {
                start_x = screen.X / (Methods.Editor.EditorConstants.TILE_BOX_SIZE * GridSize);
                end_x = Math.Min(DivideRoundUp(screen.X + screen.Width, Methods.Editor.EditorConstants.TILE_BOX_SIZE * GridSize), screen.Width);
                start_y = screen.Y / (Methods.Editor.EditorConstants.TILE_BOX_SIZE * GridSize);
                end_y = Math.Min(DivideRoundUp(screen.Y + screen.Height, Methods.Editor.EditorConstants.TILE_BOX_SIZE * GridSize), screen.Height);
            }

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
