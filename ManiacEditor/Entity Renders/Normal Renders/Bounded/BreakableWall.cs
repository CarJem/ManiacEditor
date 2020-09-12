using RSDKv5;
using SystemColors = System.Drawing.Color;
using System.Linq;

namespace ManiacEditor.Entity_Renders
{
    public class BreakableWall : EntityRenderer
    {

        public override void Draw(Structures.EntityRenderProp Properties)
        {
            DevicePanel d = Properties.Graphics;
            Classes.Scene.EditorEntity e = Properties.EditorObject;
            
            int x = Properties.DrawX;
            int y = Properties.DrawY;
            int Transparency = Properties.Transparency;

            var type = e.attributesMap["type"].ValueUInt8;
            var width = (int)(e.attributesMap["size"].ValueVector2.X.High) - 1;
			var height = (int)(e.attributesMap["size"].ValueVector2.Y.High) - 1;


            var Animation = Methods.Drawing.ObjectDrawing.LoadAnimation(d, "EditorAssets");

            if (width != -1 && height != -1)
            {
                bool wEven = width % 2 == 0;
                bool hEven = height % 2 == 0;

                int x1 = (x + (wEven ? -8 : -16) + (-width / 2 + width) * 16) + 15;
                int x2 = (x + (wEven ? -8 : -16) + (-width / 2) * 16);
                int y1 = (y + (hEven ? -8 : -16) + (-height / 2 + height) * 16) + 15;
                int y2 = (y + (hEven ? -8 : -16) + (-height / 2) * 16);


                d.DrawRectangle(x1, y1, x2, y2, SystemColors.Transparent, SystemColors.White, 2);


                // draw corners
                for (int i = 0; i < 4; i++)
                {
                    bool right = (i & 1) > 0;
                    bool bottom = (i & 2) > 0;

                    Animation = Methods.Drawing.ObjectDrawing.LoadAnimation(d, "EditorAssets");
                    if (EntityRenderer.IsValidated(Animation, new System.Tuple<int, int>(0, 1)))
                    {
                        var frame = Animation.Animation.Animations[0].Frames[1];
                        d.DrawBitmap(Animation.Spritesheets.ElementAt(frame.SpriteSheet).Value,
                            (x + (wEven ? frame.PivotX : -frame.Width) + (-width / 2 + (right ? width : 0)) * frame.Width),
                            (y + (hEven ? frame.PivotY : -frame.Height) + (-height / 2 + (bottom ? height : 0)) * frame.Height),
                            frame.X, frame.Y, frame.Width, frame.Height, false, Transparency, right, bottom);

                    }
                }
            }

            bool knux = e.attributesMap["onlyKnux"].ValueBool;
            bool mighty = e.attributesMap.ContainsKey("onlyMighty") && e.attributesMap["onlyMighty"].ValueBool;

            // draw Knuckles icon
            if (knux)
            {
                Animation = Methods.Drawing.ObjectDrawing.LoadAnimation(d, "Global/HUD.bin");
                if (EntityRenderer.IsValidated(Animation, new System.Tuple<int, int>(2, 2)))
                {
                    var frame = Animation.Animation.Animations[2].Frames[2];
                    d.DrawBitmap(Animation.Spritesheets.ElementAt(frame.SpriteSheet).Value, x - frame.Width / (mighty ? 1 : 2), y - frame.Height / 2, frame.X, frame.Y, frame.Width, frame.Height, false, Transparency);
                }
            }

            // draw Mighty icon
            if (mighty)
            {
                Animation = Methods.Drawing.ObjectDrawing.LoadAnimation(d, "Global/HUD.bin");
                if (EntityRenderer.IsValidated(Animation, new System.Tuple<int, int>(2, 3)))
                {
                    var frame = Animation.Animation.Animations[2].Frames[3];
                    d.DrawBitmap(Animation.Spritesheets.ElementAt(frame.SpriteSheet).Value, x - (knux ? 0 : frame.Width / 2), y - frame.Height / 2, frame.X, frame.Y, frame.Width, frame.Height, false, Transparency);
                }
            }
        }

        public override bool isObjectOnScreen(DevicePanel d, Classes.Scene.EditorEntity entity, int x, int y, int Transparency)
        {
            var widthPixels = (int)(entity.attributesMap["size"].ValueVector2.X.High * 2 - 1) * 16;
            var heightPixels = (int)(entity.attributesMap["size"].ValueVector2.Y.High * 2 - 1) * 16;
            return d.IsObjectOnScreen(x - widthPixels / 2, y - heightPixels / 2, widthPixels + 15, heightPixels + 15);
        }

        public override string GetObjectName()
        {
            return "BreakableWall";
        }
    }
}
