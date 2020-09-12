using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class Sol : EntityRenderer
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

            int direction = (int)e.attributesMap["direction"].ValueUInt8;
            bool fireOrbs = e.attributesMap["fireOrbs"].ValueBool;

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
            }

            var Animation = LoadAnimation("OOZ/Sol.bin", d, 0, 0);

            if (!fireOrbs)
            {
                Animation = LoadAnimation("OOZ/Sol.bin", d, 1, 0);
                DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x + 16, y, Transparency - 100, fliph, flipv);

                Animation = LoadAnimation("OOZ/Sol.bin", d, 1, 0);
                DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x - 30, y, Transparency - 100, fliph, flipv);
            }
            else
            {
                Animation = LoadAnimation("OOZ/Sol.bin", d, 1, 0);
                DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x + 16, y, Transparency, fliph, flipv);

                Animation = LoadAnimation("OOZ/Sol.bin", d, 1, 0);
                DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x - 30, y, Transparency, fliph, flipv);
            }


            Animation = LoadAnimation("OOZ/Sol.bin", d, 0, 0);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);
        }

        public override string GetObjectName()
        {
            return "Sol";
        }
    }
}
