using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class PrintBlock : EntityRenderer
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
            int letter = (int)entity.attributesMap["letter"].ValueUInt8;
            int duration = (int)entity.attributesMap["duration"].ValueUInt16;
            bool fliph = false;
            bool flipv = false;
            int frameID = 0;
            if (letter >= 11)
            {
                entity.attributesMap["letter"].ValueUInt8 = 11;
            }
            if (duration != 0)
            {
                frameID = 4;
            }

           var editorAnim = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("PrintBlock", d.DevicePanel, letter, frameID, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[Animation.index];

                d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(frame),
                    x + frame.Frame.PivotX - (fliph ? (frame.Frame.Width - editorAnim.Frames[0].Frame.Width) : 0),
                    y + frame.Frame.PivotY + (flipv ? (frame.Frame.Height - editorAnim.Frames[0].Frame.Height) : 0),
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }
        }

        public override string GetObjectName()
        {
            return "PrintBlock";
        }
    }
}
