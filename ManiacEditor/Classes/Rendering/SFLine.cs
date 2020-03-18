using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;

namespace ManiacEditor.Classes.Rendering
{
    public class SfLine : SFML.Graphics.Drawable
    {

        private Vertex[] vertices = new Vertex[4];
        float thickness;
        private Color color;

        public SfLine(Vector2f point1, Vector2f point2, Color color, float thickness)
        {
            this.thickness = thickness;
            this.color = color;

            Vector2f direction = point2 - point1;

            float unitDirX = (float)(direction.X / Math.Sqrt(direction.X * direction.X + direction.Y * direction.Y));
            float unitDirY = (float)(direction.Y / Math.Sqrt(direction.X * direction.X + direction.Y * direction.Y));

            Vector2f unitDirection = new Vector2f(unitDirX, unitDirY);
            Vector2f unitPerpendicular = new Vector2f(-unitDirection.Y, unitDirection.X);

            Vector2f offset = (thickness / 2f) * unitPerpendicular;

            vertices[0].Position = point1 + offset;
            vertices[1].Position = point2 + offset;
            vertices[2].Position = point2 - offset;
            vertices[3].Position = point1 - offset;

            for (int i = 0; i < 4; ++i)
                vertices[i].Color = color;
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            target.Draw(vertices, PrimitiveType.Lines, states);
        }
    }
}
