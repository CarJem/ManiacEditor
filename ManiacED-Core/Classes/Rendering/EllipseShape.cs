using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;

namespace ManiacEditor.Classes.Rendering
{
    public class EllipseShape : Shape
    {
        private Vector2f Radius { get; set; }


        public Vector2f GetRadius()
        {
            return Radius;
        }

        public void SetRadius(Vector2f _Radius)
        {
            Radius = _Radius;
            Update();
        }


        const float PI = 3.141592654f;

        public override uint GetPointCount()
        {
            return 30; // fixed, but could be an attribute of the class if needed
        }

        public override Vector2f GetPoint(uint index)
        {
            float angle = index * 2 * PI / GetPointCount() - PI / 2;
            float x = (float)Math.Cos(angle) * Radius.X;
            float y = (float)Math.Sin(angle) * Radius.Y;

            return new Vector2f(Radius.X + x, Radius.Y + y);
        }
    }
}
