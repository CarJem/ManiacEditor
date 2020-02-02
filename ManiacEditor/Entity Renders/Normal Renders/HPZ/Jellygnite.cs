using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class Jellygnite : EntityRenderer
    {

        public override void Draw(Structures.EntityLoadOptions properties)
        {
            Classes.Core.Draw.GraphicsHandler d = properties.Graphics;
            SceneEntity entity = properties.Object; 
            Classes.Core.Scene.Sets.EditorEntity e = properties.EditorObject;
            int x = properties.X;
            int y = properties.Y;
            int Transparency = properties.Transparency;
            int index = properties.Index;
            int previousChildCount = properties.PreviousChildCount;
            int platformAngle = properties.PlatformAngle;
            EditorAnimations Animation = properties.Animations;
            bool selected  = properties.isSelected;
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

            var editorAnim = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("Jellygnite", d.DevicePanel, 0, 0, fliph, flipv, false);
            var editorAnimFront = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("Jellygnite", d.DevicePanel, 3, 0, fliph, flipv, false);
            var editorAnimBack = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("Jellygnite", d.DevicePanel, 5, 0, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnimFront != null && editorAnimFront.Frames.Count != 0 && editorAnimBack != null && editorAnimBack.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[0];
                var frameFront = editorAnimFront.Frames[0];
                var frameBack = editorAnimBack.Frames[0];

                d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(frame),
                    x + frame.Frame.PivotX,
                    y + frame.Frame.PivotY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);

                for (int i = 0; i < 4; i++)
                {
                    d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(frameFront),
                        x + frameFront.Frame.PivotX + 12,
                        y + frameFront.Frame.PivotY + 6 + 6 * i,
                        frameFront.Frame.Width, frameFront.Frame.Height, false, Transparency);
                    d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(frameFront),
                        x + frameFront.Frame.PivotX - 12,
                        y + frameFront.Frame.PivotY + 6 + 6 * i,
                        frameFront.Frame.Width, frameFront.Frame.Height, false, Transparency);
                    d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(frameBack),
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
