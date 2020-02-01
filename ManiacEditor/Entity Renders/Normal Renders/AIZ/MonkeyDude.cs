using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class MonkeyDude : EntityRenderer
    {

        public override void Draw(GraphicsHandler d, SceneEntity entity, Classes.Editor.Scene.Sets.EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            bool fliph = false;
            bool flipv = false;
            var editorAnim = Interfaces.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("MonkeyDude", d.DevicePanel, 0, -1, fliph, flipv, false);
            var editorAnimTail = Interfaces.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("MonkeyDude", d.DevicePanel, 1, -1, fliph, flipv, false);
            var editorAnimArm = Interfaces.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("MonkeyDude", d.DevicePanel, 2, -1, fliph, flipv, false);
            var editorAnimHand = Interfaces.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("MonkeyDude", d.DevicePanel, 3, -1, fliph, flipv, false);
            var editorAnimCoconut = Interfaces.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("MonkeyDude", d.DevicePanel, 4, -1, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnimTail != null && editorAnimTail.Frames.Count != 0 && editorAnimArm != null && editorAnimArm.Frames.Count != 0 && editorAnimCoconut != null && editorAnimCoconut.Frames.Count != 0 && editorAnimHand != null && editorAnimHand.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[Animation.index];
                var frameTail = editorAnimTail.Frames[Animation.index];
                var frameArm = editorAnimArm.Frames[Animation.index];
                var frameHand = editorAnimHand.Frames[Animation.index];
                var frameCoconut = editorAnimCoconut.Frames[Animation.index];
                int i;
                for (i = 0; i < 4; i++)
                {
                    d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frameArm),
    (x - 15) + frameArm.Frame.PivotX * i - 3,
    (y - 5) + frameArm.Frame.PivotY * i - 3,
    frameArm.Frame.Width, frameArm.Frame.Height, false, Transparency);
                }
                i++;
                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frameHand),
                    (x - 15) + frameHand.Frame.PivotX * i,
                    (y - 5) + frameHand.Frame.PivotY * i,
                    frameHand.Frame.Width, frameHand.Frame.Height, false, Transparency);
                i++;
                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frameCoconut),
                    (x - 15) + frameCoconut.Frame.PivotX * i,
                    (y - 5) + frameCoconut.Frame.PivotY * i,
                    frameCoconut.Frame.Width, frameCoconut.Frame.Height, false, Transparency);
                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame),
                    x + frame.Frame.PivotX - (fliph ? (frame.Frame.Width - editorAnim.Frames[0].Frame.Width) : 0),
                    y + frame.Frame.PivotY + (flipv ? (frame.Frame.Height - editorAnim.Frames[0].Frame.Height) : 0),
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frameTail),
                    x + frameTail.Frame.PivotX - (fliph ? (frameTail.Frame.Width - editorAnimTail.Frames[0].Frame.Width) : 0),
                    y + frameTail.Frame.PivotY + (flipv ? (frameTail.Frame.Height - editorAnimTail.Frames[0].Frame.Height) : 0),
                    frameTail.Frame.Width, frameTail.Frame.Height, false, Transparency);

            }
        }

        public override string GetObjectName()
        {
            return "MonkeyDude";
        }
    }
}
