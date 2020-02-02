using RSDKv5;
using SystemColors = System.Drawing.Color;

namespace ManiacEditor.Entity_Renders
{
    public class Outro_Intro_Object : EntityRenderer
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
            bool fliph = false;
            bool flipv = false;
            var editorAnim = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("EditorIcons2", d.DevicePanel, 0, 3, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[Animation.index];

                d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(frame),
                    x + frame.Frame.PivotX - (fliph ? (frame.Frame.Width - editorAnim.Frames[0].Frame.Width) : 0),
                    y + frame.Frame.PivotY + (flipv ? (frame.Frame.Height - editorAnim.Frames[0].Frame.Height) : 0),
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }

            if (entity.Object.Name.Name == "LRZ1Intro")
            {
                var editorAnim3 = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("IntroSub", d.DevicePanel, 0, 0, fliph, flipv, false);
                if (editorAnim3 != null && editorAnim3.Frames.Count != 0)
                {
                    var frame = editorAnim3.Frames[Animation.index];

                    d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(frame),
                        x + frame.Frame.PivotX - (fliph ? (frame.Frame.Width - editorAnim.Frames[0].Frame.Width) : 0),
                        y + frame.Frame.PivotY + (flipv ? (frame.Frame.Height - editorAnim.Frames[0].Frame.Height) : 0),
                        frame.Frame.Width, frame.Frame.Height, false, Transparency);
                }
            }


            int widthPixels = AttributeHandler.AttributesMapPositionHighX("size", entity) * 2;
            var heightPixels = AttributeHandler.AttributesMapPositionHighY("size", entity) * 2;
            var width = (int)widthPixels / 16;
            var height = (int)heightPixels / 16;

            var editorAnim2 = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("EditorAssets", d.DevicePanel, 0, 1, false, false, false);

            if (width != 0 && height != 0)
            {
                int x1 = x + widthPixels / -2;
                int x2 = x + widthPixels / 2 - 1;
                int y1 = y + heightPixels / -2;
                int y2 = y + heightPixels / 2 - 1;


                d.DrawLine(x1, y1, x1, y2, SystemColors.White);
                d.DrawLine(x1, y1, x2, y1, SystemColors.White);
                d.DrawLine(x2, y2, x1, y2, SystemColors.White);
                d.DrawLine(x2, y2, x2, y1, SystemColors.White);

                // draw corners
                for (int i = 0; i < 4; i++)
                {
                    bool right = (i & 1) > 0;
                    bool bottom = (i & 2) > 0;

                    editorAnim2 = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("EditorAssets", d.DevicePanel, 0, 1, right, bottom, false);
                    if (editorAnim2 != null && editorAnim2.Frames.Count != 0)
                    {
                        var frame = editorAnim2.Frames[Animation.index];
                        Animation.ProcessAnimation(frame.Entry.SpeedMultiplyer, frame.Entry.Frames.Count, frame.Frame.Delay);
                        d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(frame),
                            (x + widthPixels / (right ? 2 : -2)) - (right ? frame.Frame.Width : 0),
                            (y + heightPixels / (bottom ? 2 : -2) - (bottom ? frame.Frame.Height : 0)),
                            frame.Frame.Width, frame.Frame.Height, false, Transparency);

                    }
                }
            }
        }

        public override string GetObjectName()
        {
            return "Outro_Intro_Object";
        }
    }
}
