using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class Spring : EntityRenderer
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
            int animID = (int)entity.attributesMap["type"].ValueEnum;
            var flipFlag = entity.attributesMap["flipFlag"].ValueEnum;
            bool fliph = false;
            bool flipv = false;

            // Handle springs being flipped in both planes
            // Down
            if ((flipFlag & 0x02) == 0x02)
                flipv = true;
            // Left
            if ((flipFlag & 0x01) == 0x01)
                fliph = true;

            var editorAnim = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("Springs", d.DevicePanel, animID % 6, -1, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && animID >= 0)
            {
                var frame = editorAnim.Frames[Animation.index];

                Animation.ProcessAnimation(frame.Entry.SpeedMultiplyer, frame.Entry.Frames.Count, frame.Frame.Delay);

                d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(frame),
                    x + frame.Frame.PivotX - (fliph ? (frame.Frame.Width - editorAnim.Frames[0].Frame.Width) : 0),
                    y + frame.Frame.PivotY + (flipv ? (frame.Frame.Height - editorAnim.Frames[0].Frame.Height) : 0),
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }
        }

        public override string GetObjectName()
        {
            return "Spring";
        }
    }
}
