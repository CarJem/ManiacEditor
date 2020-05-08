using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class Letterboard : EntityRenderer
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

            int letterID = (int)e.attributesMap["letterID"].ValueUInt8;
            bool controller = e.attributesMap["controller"].ValueBool;

            if (letterID == 0 || controller == true)
            {
                var Animation = LoadAnimation("Letterboard", d, 0, 0);
                DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);
            }
            else
            {
                var Animation = LoadAnimation("Letterboard", d, 1, letterID - 1);
                DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);
            }
        }

        public override string GetObjectName()
        {
            return "Letterboard";
        }
    }
}
