using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class TVPole : EntityRenderer
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

            var value = e.attributesMap["length"].ValueUInt16 + 1;

            bool wEven = false;
            for (int xx = 0; xx <= value; ++xx)
            {
                int pos_x = x + (wEven ? -4 : -8) + (-value / 2 + xx) * 8;
                int pos_y = y + -4;
                if (xx == 0)
                {
                    var Animation = LoadAnimation("TVPole", d, 1, 0);
                    DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, pos_x, pos_y, Transparency, false, false);
                }
                else if (xx == value)
                {
                    var Animation = LoadAnimation("TVPole", d, 1, 2);
                    DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, pos_x, pos_y, Transparency, false, false);
                }
                else
                {
                    var Animation = LoadAnimation("TVPole", d, 1, 1);
                    DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, pos_x, pos_y, Transparency, false, false);

                }

            }
        }

        public override string GetObjectName()
        {
            return "TVPole";
        }
    }
}
