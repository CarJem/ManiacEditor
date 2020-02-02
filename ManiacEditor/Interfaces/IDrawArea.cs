using System.Drawing;

namespace ManiacEditor
{
    public interface IDrawArea
    {
        void DisposeTextures();

        Rectangle GetScreen();
        double GetZoom();
    }
}
