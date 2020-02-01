using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class HangConveyor : EntityRenderer
    {

        public override void Draw(GraphicsHandler d, SceneEntity entity, Classes.Editor.Scene.Sets.EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            bool fliph = false;
            int direction = (int)entity.attributesMap["direction"].ValueUInt8;
            int length = (int)entity.attributesMap["length"].ValueUInt32*16;
            if (direction == 1)
            {
                fliph = true;
            }
            var editorAnim = Interfaces.Base.MapEditor.Instance.EntityDrawing.LoadAnimation2("HangConveyor", d.DevicePanel, 0, -1, fliph, false, false);
            var editorAnimEnd = Interfaces.Base.MapEditor.Instance.EntityDrawing.LoadAnimation2("HangConveyor", d.DevicePanel, 1, -1, !fliph, false, false);
            var editorAnimMid = Interfaces.Base.MapEditor.Instance.EntityDrawing.LoadAnimation2("HangConveyor", d.DevicePanel, 2, -1, fliph, false, false);
            var editorAnimMid2 = Interfaces.Base.MapEditor.Instance.EntityDrawing.LoadAnimation2("HangConveyor", d.DevicePanel, 2, -1, !fliph, false, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnimEnd != null && editorAnimEnd.Frames.Count != 0 && editorAnimMid != null && editorAnimMid.Frames.Count != 0 && editorAnimMid2 != null && editorAnimMid2.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[Animation.index];
                var frameEnd = editorAnimEnd.Frames[Animation.index];
                var frameMid = editorAnimMid.Frames[Animation.index];
                var frameMid2 = editorAnimMid2.Frames[Animation.index];

                Animation.ProcessAnimation(frame.Entry.SpeedMultiplyer, frame.Entry.Frames.Count, frame.Frame.Delay);

                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame),
                    x + frame.Frame.PivotX + (direction == 1 ? length / 2 : -(length / 2)),
                    y + frame.Frame.PivotY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);

                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frameEnd),
                    x + frameEnd.Frame.PivotX - (direction == 1 ? length / 2 : -(length / 2)),
                    y + frameEnd.Frame.PivotY,
                    frameEnd.Frame.Width, frameEnd.Frame.Height, false, Transparency);

                int start_x = x + frameEnd.Frame.PivotX - length / 2 + frameEnd.Frame.Width - 6;
                int start_x2 = x + frameEnd.Frame.PivotX - length / 2 + frameEnd.Frame.Width - 10;
                int length2 = (length / 16 ) - 1;
                for (int i = 0; i < length2; i++)
                {
                    d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frameMid),
                        start_x + frameMid.Frame.PivotX + 16*i,
                        y - 21 + frameMid.Frame.PivotY,
                        frameMid.Frame.Width, frameMid.Frame.Height, false, Transparency);
                }

                for (int i = 0; i < length2; i++)
                {
                    d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frameMid2),
                        start_x2 + frameMid2.Frame.PivotX + 16 * i,
                        y + 21 + frameMid2.Frame.PivotY,
                        frameMid2.Frame.Width, frameMid2.Frame.Height, false, Transparency);
                }
            }
        }

        public override string GetObjectName()
        {
            return "HangConveyor";
        }
    }
}
