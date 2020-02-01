using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class Jellygnite : EntityRenderer
    {

        public override void Draw(GraphicsHandler d, SceneEntity entity, Classes.Editor.Scene.Sets.EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            int direction = (int)entity.attributesMap["direction"].ValueUInt8;
            bool fliph = false;
            bool flipv = false;


            if (direction == 1)
            {
                fliph = true;
            }
            if (direction == 2)
            {
                flipv = true;
            }
            if (direction == 3)
            {
                flipv = true;
                fliph = true;
            }

            var editorAnim = Interfaces.Base.MapEditor.Instance.EntityDrawing.LoadAnimation2("Jellygnite", d.DevicePanel, 0, 0, fliph, flipv, false);
            var editorAnimFront = Interfaces.Base.MapEditor.Instance.EntityDrawing.LoadAnimation2("Jellygnite", d.DevicePanel, 3, 0, fliph, flipv, false);
            var editorAnimBack = Interfaces.Base.MapEditor.Instance.EntityDrawing.LoadAnimation2("Jellygnite", d.DevicePanel, 5, 0, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnimFront != null && editorAnimFront.Frames.Count != 0 && editorAnimBack != null && editorAnimBack.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[0];
                var frameFront = editorAnimFront.Frames[0];
                var frameBack = editorAnimBack.Frames[0];

                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame),
                    x + frame.Frame.PivotX,
                    y + frame.Frame.PivotY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);

                for (int i = 0; i < 4; i++)
                {
                    d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frameFront),
                        x + frameFront.Frame.PivotX + 12,
                        y + frameFront.Frame.PivotY + 6 + 6 * i,
                        frameFront.Frame.Width, frameFront.Frame.Height, false, Transparency);
                    d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frameFront),
                        x + frameFront.Frame.PivotX - 12,
                        y + frameFront.Frame.PivotY + 6 + 6 * i,
                        frameFront.Frame.Width, frameFront.Frame.Height, false, Transparency);
                    d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frameBack),
                        x + frameBack.Frame.PivotX,
                        y + frameBack.Frame.PivotY + 6 + 6 * i,
                        frameBack.Frame.Width, frameBack.Frame.Height, false, Transparency);
                }
             }
        }

        public override string GetObjectName()
        {
            return "Jellygnite";
        }
    }
}
