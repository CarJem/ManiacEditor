using System.Drawing;

namespace ManiacEditor
{
    public interface IDrawable
    {
        void Draw(Graphics g);

        void Draw(DevicePanel d);

        //void DrawAnimation(DevicePanel d);
    }
}
