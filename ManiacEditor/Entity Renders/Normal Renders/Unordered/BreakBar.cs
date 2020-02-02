using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class BreakBar : EntityRenderer
    {

        public override void Draw(Structures.EntityRenderProp properties)
        {
            Classes.Editor.Draw.GraphicsHandler d = properties.Graphics;
            SceneEntity entity = properties.Object; 
            Classes.Editor.Scene.Sets.EditorEntity e = properties.EditorObject;
            int x = properties.X;
            int y = properties.Y;
            int Transparency = properties.Transparency;
            int index = properties.Index;
            int previousChildCount = properties.PreviousChildCount;
            int platformAngle = properties.PlatformAngle;
            Methods.Entities.EntityAnimator Animation = properties.Animations;
            bool selected  = properties.isSelected;
            var length = (short)(entity.attributesMap["length"].ValueUInt16);
            var orientation = entity.attributesMap["orientation"].ValueUInt8;
            if (orientation >= 2)
            {
                orientation = 0;
            }
            var editorAnim = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("BreakBar", d.DevicePanel, orientation, -1, false, false, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0)
            {
                var frameTop = editorAnim.Frames[0];
                var frameBase = editorAnim.Frames[1];
                var frameBottom = editorAnim.Frames[2];
                for (int i = -length / 2; i <= length / 2; ++i)
                {
                    if (orientation == 1)
                    {

                    }
                    d.DrawBitmap(new Classes.Editor.Draw.GraphicsHandler.GraphicsInfo(frameBase), x + frameBase.Frame.PivotX + (orientation == 1 ? i * frameBase.Frame.Width : 0),
                        y + frameBase.Frame.PivotY + (orientation == 0 ? i * frameBase.Frame.Height  : 0),
                        frameBase.Frame.Width, frameBase.Frame.Height, false, Transparency);
                }
                d.DrawBitmap(new Classes.Editor.Draw.GraphicsHandler.GraphicsInfo(frameTop), x + frameTop.Frame.PivotX - (orientation == 1 ? (length / 2 + 1) * frameBase.Frame.Width : 0) + (orientation == 1 ? 4 : 0),
                    y + frameTop.Frame.PivotY - (orientation == 0 ? (length / 2 + 1) * frameBase.Frame.Height : 0) + (orientation == 0 ? 4 : 0),
                    frameTop.Frame.Width, frameTop.Frame.Height, false, Transparency);
                d.DrawBitmap(new Classes.Editor.Draw.GraphicsHandler.GraphicsInfo(frameBottom), x + frameBottom.Frame.PivotX + (orientation == 1 ? (length / 2 + 1) * frameBase.Frame.Width : 0) - (orientation == 1 ? 4 : 0),
                    y + frameBottom.Frame.PivotY + (orientation == 0 ? (length / 2 + 1) * frameBase.Frame.Height : 0) - (orientation == 0 ? 4 : 0),
                    frameBottom.Frame.Width, frameBottom.Frame.Height, false, Transparency);

            }
        }

        public override string GetObjectName()
        {
            return "BreakBar";
        }
    }
}
