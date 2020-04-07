using System.Drawing;
using SFML.System;
using SFML.Graphics;

namespace ManiacEditor
{
    public interface IDrawArea
    {
        void DisposeTextures();       
        Rectangle GetScreen();
        float GetZoom();
        SFML.System.Vector2i GetPosition();
    }
}
