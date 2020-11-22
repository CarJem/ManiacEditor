using System;
using System.Drawing;
using RSDKv5Color = RSDKv5.Color;

namespace ManiacEditor.Classes.Scene
{
    public class EditorBackground : IDrawable
    {

        private const int TILE_BOX_SIZE = 1;
        private const int BOX_SIZE = 8;
        private const int TILE_SIZE = 16;

        private bool CameraUnlocked
        {
            get
            {
                return Methods.Solution.SolutionState.Main.UnlockCamera;
            }
        }
        private int GridSize
        {
            get
            {
                return (Methods.Solution.CurrentSolution.UI_Instance != null ? Methods.Solution.SolutionState.Main.GridSize : 16);
            }
        }
        private int SceneWidth
        {
            get
            {
                return Methods.Solution.CurrentSolution.SceneWidth;
            }
        }
        private int SceneHeight
        {
            get
            {
                return Methods.Solution.CurrentSolution.SceneHeight;
            }
        }
        private System.Drawing.Color GridColor
        {
            get
            {
                int A = (int)Methods.Solution.CurrentSolution.UI_Instance.EditorToolbar.gridOpacitySlider.Value;
                int R = Methods.Solution.SolutionState.Main.GridColor.R;
                int G = Methods.Solution.SolutionState.Main.GridColor.G;
                int B = Methods.Solution.SolutionState.Main.GridColor.B;

                return Color.FromArgb(A, R, G, B);
            }
        }
        private RSDKv5Color BackgroundColor1
        {
            get
            {
                return Methods.Solution.CurrentSolution.CurrentScene.EditorMetadata.BackgroundColor1;
            }
        }
        private RSDKv5Color BackgroundColor2
        {
            get
            {
                return Methods.Solution.CurrentSolution.CurrentScene.EditorMetadata.BackgroundColor2;
            }
        }

        public EditorBackground()
        {

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
        public void Draw(DevicePanel d, bool EditDraw)
        {
            Rectangle screen = d.GetScreen();

            int Transparency1 = (EditDraw ? 30 : BackgroundColor1.A);
            int Transparency2 = (EditDraw ? 30 : BackgroundColor2.A);

            Color Color1 = Color.FromArgb(Transparency1, BackgroundColor1.R, BackgroundColor1.G, BackgroundColor1.B);
            Color Color2 = Color.FromArgb(Transparency2, BackgroundColor2.R, BackgroundColor2.G, BackgroundColor2.B);

            int start_x;
            int end_x;
            int start_y;
            int end_y;

            if (!CameraUnlocked)
            {
                start_x = screen.X / (BOX_SIZE * TILE_SIZE);
                end_x = Math.Min(DivideRoundUp(screen.X + screen.Width, BOX_SIZE * TILE_SIZE), SceneWidth);
                start_y = screen.Y / (BOX_SIZE * TILE_SIZE);
                end_y = Math.Min(DivideRoundUp(screen.Y + screen.Height, BOX_SIZE * TILE_SIZE), SceneHeight);
            }
            else
            {
                start_x = 0;
                end_x = Math.Min(DivideRoundUp(SceneWidth, BOX_SIZE * TILE_SIZE), (int)(SceneWidth));
                start_y = 0;
                end_y = Math.Min(DivideRoundUp(SceneHeight, BOX_SIZE * TILE_SIZE), (int)(SceneHeight));
            }
            

            // Draw with first color everything
            d.DrawRectangle(screen.X, screen.Y, screen.X + screen.Width, screen.Y + screen.Height, Color1);

            if (Color2.A != 0) {
                for (int y = start_y; y < end_y; ++y)
                {
                    for (int x = start_x; x < end_x; ++x)
                    {
                        if ((x + y) % 2 == 1) d.DrawRectangle(x * BOX_SIZE * TILE_SIZE, y * BOX_SIZE * TILE_SIZE, (x + 1) * BOX_SIZE * TILE_SIZE, (y + 1) * BOX_SIZE * TILE_SIZE, Color2);
                    }
                }
            }
        }
        public void DrawGrid(DevicePanel d)
        {
            Rectangle screen = d.GetScreen();

            int start_x;
            int end_x;
            int start_y;
            int end_y;

            if (!CameraUnlocked)
            {
                start_x = screen.X / (TILE_BOX_SIZE * GridSize);
                end_x = Math.Min(DivideRoundUp(screen.X + screen.Width, TILE_BOX_SIZE * GridSize), SceneWidth);
                start_y = screen.Y / (TILE_BOX_SIZE * GridSize);
                end_y = Math.Min(DivideRoundUp(screen.Y + screen.Height, TILE_BOX_SIZE * GridSize), SceneHeight);
            }
            else
            {
                start_x = screen.X / (TILE_BOX_SIZE * GridSize);
                end_x = Math.Min(DivideRoundUp(screen.X + screen.Width, TILE_BOX_SIZE * GridSize), screen.Width);
                start_y = screen.Y / (TILE_BOX_SIZE * GridSize);
                end_y = Math.Min(DivideRoundUp(screen.Y + screen.Height, TILE_BOX_SIZE * GridSize), screen.Height);
            }

            for (int y = start_y; y < end_y; ++y)
            {
                for (int x = start_x; x < end_x; ++x)
                {
                    if (x >= 0 && y >= 0)
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
}