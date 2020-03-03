using RSDKv5;
using System.Linq;

namespace ManiacEditor.Entity_Renders
{
    public class Spring : EntityRenderer
    {

        public override void Draw(Structures.EntityRenderProp Properties)
        {
            DevicePanel d = Properties.Graphics;
            SceneEntity entity = Properties.EditorObject.Entity; 
            Classes.Scene.Sets.EditorEntity e = Properties.EditorObject;
            int x = Properties.DrawX;
            int y = Properties.DrawY;
            int Transparency = Properties.Transparency;
            int animID = (int)entity.attributesMap["type"].ValueEnum;
            var flipFlag = entity.attributesMap["flipFlag"].ValueEnum;
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
            if (Animation != null && Animation.Animation != null)
            {
                if (Animation.Animation.Animations.Count != 0 && Animation.Animation.Animations.Count >= 1 && Animation.Animation.Animations[0].Frames != null && Animation.Animation.Animations[0].Frames.Count >= 1)
                {
                    var frame = Animation.Animation.Animations[animID].Frames[0];

                    d.DrawBitmap(Animation.Spritesheets.ElementAt(frame.SpriteSheet).Value, x + frame.PivotX, y + frame.PivotY, frame.X, frame.Y, frame.Width, frame.Height, false, Transparency);
                }
            }
        }

        public override string GetObjectName()
        {
            return "Spring";
        }
    }
}
