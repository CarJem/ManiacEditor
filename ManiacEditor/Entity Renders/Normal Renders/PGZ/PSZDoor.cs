using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class PSZDoor : EntityRenderer
    {

        public override void Draw(Classes.Core.Draw.GraphicsHandler d, SceneEntity entity, Classes.Core.Scene.Sets.EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            int orientation = entity.attributesMap["orientation"].ValueUInt8;
            bool open = entity.attributesMap["open"].ValueBool;
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
            var editorAnim = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("PSZDoor", d.DevicePanel, 0, frameID, fliph, flipv, false);
            var cogPart2 = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("PSZDoor", d.DevicePanel, 0, 2, fliph, flipv, false);
            var cogPart1 = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("PSZDoor", d.DevicePanel, 1, 0, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && frameID >= 0 && cogPart1 != null && cogPart1.Frames.Count != 0 && cogPart2 != null && cogPart2.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[Animation.index];
                var cog1 = cogPart1.Frames[Animation.index];
                var cog2 = cogPart2.Frames[Animation.index];

                int COG_SPACE = 18;

                int cogSpaceH = 56;
                int cogSpaceW = -COG_SPACE;
                int cogSpaceH_2 = 56;
                int cogSpaceW_2 = -COG_SPACE;
                int doorAdjX = frame.Frame.Width;
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
                        doorAdjY = frame.Frame.Height;
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
                        doorAdjY = -frame.Frame.Height;
                        break;
                    case 2:
                        // Upper
                        cogSpaceW = -56;
                        cogSpaceH = COG_SPACE;
                        // Lower
                        cogSpaceW_2 = -56;
                        cogSpaceH_2 = -COG_SPACE;


                        //Door
                        doorAdjX = -frame.Frame.Width;
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
                        doorAdjX = frame.Frame.Width;
                        doorAdjY = 0;
                        break;
                }

                d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(frame),
                    x + frame.Frame.PivotX + (open ? doorAdjX : 0),
                    y + frame.Frame.PivotY + (open ? doorAdjY : 0),
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);

                //Upper Cog
                d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(cog1),
                    x + cog1.Frame.PivotX + cogSpaceW,
                    y + cog1.Frame.PivotY + cogSpaceH,
                    cog1.Frame.Width, cog1.Frame.Height, false, Transparency);

                d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(cog2),
                    x + cog2.Frame.PivotX + cogSpaceW,
                    y + cog2.Frame.PivotY + cogSpaceH,
                    cog2.Frame.Width, cog2.Frame.Height, false, Transparency);

                //Lower Cog
                d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(cog1),
                    x + cog1.Frame.PivotX + cogSpaceW_2,
                    y + cog1.Frame.PivotY + cogSpaceH_2,
                    cog1.Frame.Width, cog1.Frame.Height, false, Transparency);

                d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(cog2),
                    x + cog2.Frame.PivotX + cogSpaceW_2,
                    y + cog2.Frame.PivotY + cogSpaceH_2,
                    cog2.Frame.Width, cog2.Frame.Height, false, Transparency);
            }
        }

        public override string GetObjectName()
        {
            return "PSZDoor";
        }
    }
}
