using System.Drawing;
using SFML.System;
using SFML.Graphics;

namespace ManiacEditor
{
    public interface IDrawAreaSFML
    {
        Rectangle GetScreen();
        float GetZoom();
        SFML.System.Vector2i GetPosition();
    }

    public interface IDrawArea
    {
        void DisposeTextures();

        Rectangle GetScreen();
        double GetZoom();
    }
}
