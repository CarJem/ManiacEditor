using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class DoorTrigger : EntityRenderer
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

            int orientation = e.attributesMap["orientation"].ValueUInt8;

            int AnimID_2 = 1;
            int frameID = 0;
            int frameID_2 = 0;
            int offsetX = 0;
            int offsetY = 0;
            switch (orientation)
            {
                case 0:
                    frameID = 0;
                    frameID_2 = 0;
                    AnimID_2 = 1;
                    offsetX = 0;
                    offsetY = 0;
                    break;
                case 1:
                    frameID = 0;
                    frameID_2 = 0;
                    AnimID_2 = 1;
                    fliph = true;
                    offsetX = -23;
                    offsetY = 0;
                    break;
                case 2:
                    frameID = 1;
                    AnimID_2 = 2;
                    offsetX = 0;
                    offsetY = 0;
                    break;
                case 3:
                    frameID = 1;
                    AnimID_2 = 2;
                    flipv = true;
                    offsetX = 0;
                    offsetY = 0;
                    break;
            }

            var Animation = LoadAnimation("DoorTrigger", d, 0, frameID);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x + offsetX, y + offsetY, Transparency, fliph, flipv);

            Animation = LoadAnimation("DoorTrigger", d, AnimID_2, frameID_2);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x + offsetX, y + offsetY, Transparency, fliph, flipv);
        }

        public override string GetObjectName()
        {
            return "DoorTrigger";
        }
    }
}
