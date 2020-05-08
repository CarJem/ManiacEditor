using RSDKv5;
using System.Linq;

namespace ManiacEditor.Entity_Renders
{
    public class LRZRockPile : EntityRenderer
    {

        public override void Draw(Structures.EntityRenderProp Properties)
        {
            DevicePanel d = Properties.Graphics;

            Classes.Scene.EditorEntity e = Properties.EditorObject;
            int x = Properties.DrawX;
            int y = Properties.DrawY;
            int Transparency = Properties.Transparency;

            bool fliph = false;
            bool flipv = false;

            int type = (int)e.attributesMap["type"].ValueUInt8;
            if (type > 1) type = 1;

            var Animation = Methods.Draw.ObjectDrawing.LoadAnimation(d, "LRZRockPile", type, 0);
            if (EntityRenderer.IsValidated(Animation, new System.Tuple<int, int>(type, 0)))
            {
                var Frame = Animation.Animation.Animations[type].Frames[0];
                d.DrawTexture(Animation.Spritesheets.ElementAt(Frame.SpriteSheet).Value, x + Frame.PivotX, y + Frame.PivotY, Frame.X, Frame.Y, Frame.Width, Frame.Height, false, Transparency);
            }

            bool knux = e.attributesMap["onlyKnux"].ValueBool;
            bool mighty = e.attributesMap.ContainsKey("onlyMighty") && e.attributesMap["onlyMighty"].ValueBool;

            // draw Knuckles icon
            if (knux)
            {
                Animation = Methods.Draw.ObjectDrawing.LoadAnimation(d, "HUD");
                if (EntityRenderer.IsValidated(Animation, new System.Tuple<int, int>(2, 2)))
                {
                    var frame = Animation.Animation.Animations[2].Frames[2];
                    d.DrawTexture(Animation.Spritesheets.ElementAt(frame.SpriteSheet).Value, x - frame.Width / (mighty ? 1 : 2), y - frame.Height / 2, frame.X, frame.Y, frame.Width, frame.Height, false, Transparency);
                }
            }

            // draw Mighty icon
            if (mighty)
            {
                Animation = Methods.Draw.ObjectDrawing.LoadAnimation(d, "HUD");
                if (EntityRenderer.IsValidated(Animation, new System.Tuple<int, int>(2, 3)))
                {
                    var frame = Animation.Animation.Animations[2].Frames[3];
                    d.DrawTexture(Animation.Spritesheets.ElementAt(frame.SpriteSheet).Value, x - (knux ? 0 : frame.Width / 2), y - frame.Height / 2, frame.X, frame.Y, frame.Width, frame.Height, false, Transparency);
                }
            }
        }

        public override string GetObjectName()
        {
            return "LRZRockPile";
        }
    }
}
