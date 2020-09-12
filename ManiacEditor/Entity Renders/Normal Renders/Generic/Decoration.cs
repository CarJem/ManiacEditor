using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class Decoration : EntityRenderer
    {

        public override void Draw(Structures.EntityRenderProp Properties)
        {

            DevicePanel d = Properties.Graphics;
            Classes.Scene.EditorEntity e = Properties.EditorObject;
            
            int x = Properties.DrawX;
            int y = Properties.DrawY;
            int Transparency = Properties.Transparency;

            bool flipv = false;
            bool fliph = false;
            var type = e.attributesMap["type"].ValueUInt8;
            var direction = e.attributesMap["direction"].ValueUInt8;
            var repeatSpacing = e.attributesMap["repeatSpacing"].ValueVector2;
            var repeatTimes = e.attributesMap["repeatTimes"].ValueVector2;
            var rotSpeed = e.attributesMap["rotSpeed"].ValueEnum;

            int offsetX = (int)repeatSpacing.X.High;
            int repeatX = (int)repeatTimes.X.High;
            int offsetY = (int)repeatSpacing.Y.High;
            int repeatY = (int)repeatTimes.Y.High;

            switch (direction)
            {               
                case 0:
                    break;
                case 1:
                    fliph = true;
                    break;
                case 2:
                    flipv = true;
                    break;
                case 3:
                    flipv = true;
                    fliph = true;
                    break;
            }



            var Animation = Methods.Drawing.ObjectDrawing.LoadAnimation(Properties.Graphics, GetSetupAnimation(), type, 0);

            for (int yy = 0; yy <= repeatY; yy++)
            {
                for (int xx = 0; xx <= repeatX; xx++)
                {
                    int current_x = (x + Animation.RequestedFrame.RelCenterX(fliph) + offsetX * xx) - (offsetX * repeatX / 2);
                    int current_y = (y + Animation.RequestedFrame.RelCenterY(flipv) + offsetY * yy) - (offsetY * repeatY / 2);
                    DrawTexture(d, Animation, type, 0, current_x, current_y, Transparency, fliph, flipv);
                }
            }
        }

        public override string GetSetupAnimation()
        {
            return GetSpriteAnimationPath("/Decoration.bin", "Decoration", new string[] { "GHZ", "CPZ", "SPZ1", "FBZ", "HCZ", "MSZ", "TMZ1", "AIZ" });
        }

        public override string GetObjectName()
        {
            return "Decoration";
        }
    }
}
