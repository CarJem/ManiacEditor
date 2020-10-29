using System.Drawing;
using SFML.System;
using SFML.Graphics;

namespace ManiacEditor
{

    public interface IDrawArea
    {
        void DisposeTextures();
        void Reload();
        Rectangle GetScreen();
        double GetZoom();
    }
}
