using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class SeeSaw : EntityRenderer
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

            if (Methods.Solution.CurrentSolution.Entities.SetupObject != "MMZSetup")
            {
                int side = (int)e.attributesMap["side"].ValueUInt8;
                switch (side)
                {
                    case 0:
                        fliph = false;
                        break;
                    case 1:
                        fliph = true;
                        break;
                    default:
                        fliph = false;
                        break;
                }


                var Animation = LoadAnimation("MSZ/SeeSaw.bin", d, 2, 0);
                DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x - (fliph ? -35 : 35), y - 15, Transparency, fliph, flipv);

                Animation = LoadAnimation("MSZ/SeeSaw.bin", d, 0, 0);
                DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);

                Animation = LoadAnimation("MSZ/SeeSaw.bin", d, 1, 0);
                DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);


            }

        }

        public override string GetObjectName()
        {
            return "SeeSaw";
        }
    }
}
