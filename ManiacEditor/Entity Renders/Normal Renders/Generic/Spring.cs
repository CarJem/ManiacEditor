using RSDKv5;
using System.Linq;

namespace ManiacEditor.Entity_Renders
{
    public class Spring : EntityRenderer
    {

        public override void Draw(Structures.EntityRenderProp Properties)
        {
            DevicePanel d = Properties.Graphics;
             
            Classes.Scene.EditorEntity e = Properties.EditorObject;
            int x = Properties.DrawX;
            int y = Properties.DrawY;
            int Transparency = Properties.Transparency;
            int animID = (int)e.attributesMap["type"].ValueEnum;
            var flipFlag = e.attributesMap["flipFlag"].ValueEnum;
            bool fliph = false;
            bool flipv = false;

            // Handle springs being flipped in both planes
            // Down
            if ((flipFlag & 0x02) == 0x02)
                flipv = true;
            // Left
            if ((flipFlag & 0x01) == 0x01)
                fliph = true;

            var Animation = Methods.Entities.EntityDrawing.LoadAnimation(d, "Springs");
            if (EntityRenderer.IsValidated(Animation, new System.Tuple<int, int>(animID, 0)))
            {
                var frame = Animation.Animation.Animations[animID].Frames[0];
                d.DrawTexture(Animation.Spritesheets.ElementAt(frame.SpriteSheet).Value, x + frame.PivotX, y + frame.PivotY, frame.X, frame.Y, frame.Width, frame.Height, false, Transparency, fliph, flipv);
            }
        }
        public override bool isObjectOnScreen(DevicePanel d, Classes.Scene.EditorEntity e, int x, int y, int Transparency)
        {
            return d.IsObjectOnScreen(x, y, 20, 20);
        }
        public override string GetObjectName()
        {
            return "Spring";
        }
    }
}
