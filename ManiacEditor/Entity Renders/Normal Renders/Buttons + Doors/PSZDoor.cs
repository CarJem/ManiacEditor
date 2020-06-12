using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class PSZDoor : EntityRenderer
    {

        public override void Draw(Structures.EntityRenderProp Properties)
        {
            DevicePanel d = Properties.Graphics;
            Classes.Scene.EditorEntity e = Properties.EditorObject;
            int x = Properties.DrawX;
            int y = Properties.DrawY;
            int Transparency = Properties.Transparency;

            int orientation = e.attributesMap["orientation"].ValueUInt8;
            bool open = e.attributesMap["open"].ValueBool;
            bool fliph = false;
            bool flipv = false;
            int frameID = 0;
            switch (orientation)
            {
                case 0:
                    frameID = 0;
                    break;
                case 1:
                    frameID = 0;
                    fliph = true;
                    break;
                case 2:
                    frameID = 1;

                    break;
                case 3:
                    frameID = 1;
                    flipv = true;
                    break;
            }


            var Animation = LoadAnimation("PSZDoor", d, 0, frameID);


                int COG_SPACE = 18;

                int cogSpaceH = 56;
                int cogSpaceW = -COG_SPACE;
                int cogSpaceH_2 = 56;
                int cogSpaceW_2 = -COG_SPACE;
                int doorAdjX = Animation.RequestedFrame.Width;
                int doorAdjY = 0;

                switch (orientation)
                {
                    case 1:
                        // Upper
                        cogSpaceH = 56;
                        cogSpaceW = -COG_SPACE;
                        // Lower
                        cogSpaceH_2 = 56;
                        cogSpaceW_2 = COG_SPACE;


                        //Door
                        doorAdjX = 0;
                        doorAdjY = Animation.RequestedFrame.Height;
                        break;
                    case 0:
                        // Upper
                        cogSpaceH = -56;
                        cogSpaceW = -COG_SPACE;
                        // Lower
                        cogSpaceH_2 = -56;
                        cogSpaceW_2 = COG_SPACE;


                        //Door
                        doorAdjX = 0;
                        doorAdjY = -Animation.RequestedFrame.Height;
                        break;
                    case 2:
                        // Upper
                        cogSpaceW = -56;
                        cogSpaceH = COG_SPACE;
                        // Lower
                        cogSpaceW_2 = -56;
                        cogSpaceH_2 = -COG_SPACE;


                        //Door
                        doorAdjX = -Animation.RequestedFrame.Width;
                        doorAdjY = 0;
                        break;
                    case 3:
                        // Upper
                        cogSpaceW = 56;
                        cogSpaceH = COG_SPACE;
                        // Lower
                        cogSpaceW_2 = 56;
                        cogSpaceH_2 = -COG_SPACE;


                        //Door
                        doorAdjX = Animation.RequestedFrame.Width;
                        doorAdjY = 0;
                        break;
                }

            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x + (open ? doorAdjX : 0), y + (open ? doorAdjY : 0), Transparency, fliph, flipv);

            var cogPart1 = LoadAnimation("PSZDoor", d, 1, 0);
            DrawTexturePivotNormal(d, cogPart1, cogPart1.RequestedAnimID, cogPart1.RequestedFrameID, x + cogSpaceW, y + cogSpaceH, Transparency, fliph, flipv);
            DrawTexturePivotNormal(d, cogPart1, cogPart1.RequestedAnimID, cogPart1.RequestedFrameID, x + cogSpaceW_2, y + cogSpaceH_2, Transparency, fliph, flipv);

            var cogPart2 = LoadAnimation("PSZDoor", d, 0, 2);
            DrawTexturePivotNormal(d, cogPart2, cogPart2.RequestedAnimID, cogPart2.RequestedFrameID, x + cogSpaceW, y + cogSpaceH, Transparency, fliph, flipv);
            DrawTexturePivotNormal(d, cogPart2, cogPart2.RequestedAnimID, cogPart2.RequestedFrameID, x + cogSpaceW_2, y + cogSpaceH_2, Transparency, fliph, flipv);
        }

        public override string GetObjectName()
        {
            return "PSZDoor";
        }
    }
}
