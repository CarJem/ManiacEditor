using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class PrintBlock : EntityRenderer
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

            int letter = (int)e.attributesMap["letter"].ValueUInt8;
            int duration = (int)e.attributesMap["duration"].ValueUInt16;

            int frameID = 0;
            if (letter >= 11)
            {
                letter = 11;
            }
            if (duration != 0)
            {
                frameID = 4;
            }


            var Animation = LoadAnimation("PrintBlock", d, letter, frameID);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);
        }

        public override string GetObjectName()
        {
            return "PrintBlock";
        }
    }
}
