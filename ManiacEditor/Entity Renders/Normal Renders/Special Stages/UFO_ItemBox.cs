using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class UFO_ItemBox : EntityRenderer
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

            int id = (int)e.attributesMap["type"].ValueEnum;
            if (id > 2)
            {
                e.attributesMap["type"].ValueEnum = 2;
            }


            var Animation = LoadAnimation("SpecialUFO/Items.bin", d, 0, id);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);
        }

        public override string GetObjectName()
        {
            return "UFO_ItemBox";
        }
    }
}
