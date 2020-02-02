using RSDKv5;
using SystemColors = System.Drawing.Color;

namespace ManiacEditor.Entity_Renders
{
    public class Hatch : EntityRenderer
    {

        public override void Draw(Structures.EntityRenderProp properties)
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
            var widthPixels = (int)(entity.attributesMap["subOff2"].ValueVector2.X.High) * 16;
            var heightPixels = (int)(entity.attributesMap["subOff2"].ValueVector2.Y.High) * 16;
            var width = (int)widthPixels / 16;
            var height = (int)heightPixels / 16;
            int offsetX = (int)(entity.attributesMap["subOff1"].ValueVector2.X.High);
            int offsetY = (int)(entity.attributesMap["subOff1"].ValueVector2.Y.High);

            var editorAnim = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("EditorAssets", d.DevicePanel, 0, 1, false, false, false);

            var editorAnim2 = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("Hatch", d.DevicePanel, 0, 0, false, false, false);
            var editorAnim3 = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("Hatch", d.DevicePanel, 0, 1, false, false, false);
            if (editorAnim2 != null && editorAnim2.Frames.Count != 0 && editorAnim3 != null && editorAnim3.Frames.Count != 0)
            {
                var frame = editorAnim2.Frames[0];
                var frame2 = editorAnim3.Frames[0];

                Animation.ProcessAnimation(frame.Entry.SpeedMultiplyer, frame.Entry.Frames.Count, frame.Frame.Delay);

                d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(frame2),
                    x + frame2.Frame.PivotX,
                    y + frame2.Frame.PivotY,
                    frame2.Frame.Width, frame2.Frame.Height, false, Transparency);
                d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(frame),
                    x + frame.Frame.PivotX,
                    y + frame.Frame.PivotY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);

            }

            if (width != 0 && height != 0)
            {
                int x1 = x + offsetX + widthPixels / -2;
                int x2 = x + offsetX + widthPixels / 2 - 1;
                int y1 = y + offsetY + heightPixels / -2;
                int y2 = y + offsetY + heightPixels / 2 - 1;


                d.DrawLine(x1, y1, x1, y2, SystemColors.White);
                d.DrawLine(x1, y1, x2, y1, SystemColors.White);
                d.DrawLine(x2, y2, x1, y2, SystemColors.White);
                d.DrawLine(x2, y2, x2, y1, SystemColors.White);

                // draw corners
                for (int i = 0; i < 4; i++)
                {
                    bool right = (i & 1) > 0;
                    bool bottom = (i & 2) > 0;

                    editorAnim = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("EditorAssets", d.DevicePanel, 0, 1, right, bottom, false);
                    if (editorAnim != null && editorAnim.Frames.Count != 0)
                    {
                        var frame = editorAnim.Frames[Animation.index];
                        Animation.ProcessAnimation(frame.Entry.SpeedMultiplyer, frame.Entry.Frames.Count, frame.Frame.Delay);
                        d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(frame),
                            (x + offsetX + widthPixels / (right ? 2 : -2)) - (right ? frame.Frame.Width : 0),
                            (y + offsetY + heightPixels / (bottom ? 2 : -2) - (bottom ? frame.Frame.Height : 0)),
                            frame.Frame.Width, frame.Frame.Height, false, Transparency);

                    }
                }
            }
        }

        public override string GetObjectName()
        {
            return "Hatch";
        }
    }
}
