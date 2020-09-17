using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ManiacEditor
{
    public interface IDrawAreaSFML
    {
        Rectangle GetScreen();
        float GetZoom();
        SFML.System.Vector2i GetPosition();
    }
}
