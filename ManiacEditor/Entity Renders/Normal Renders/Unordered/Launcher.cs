using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class Launcher : EntityRenderer
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
            int direction = (int)entity.attributesMap["direction"].ValueUInt8;
            bool fliph = false;
            bool flipv = false;
            if (direction == 1)
            {
                fliph = true;
            }
                var editorAnim = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("Platform", d.DevicePanel, 1, 4, fliph, flipv, false);
                if (editorAnim != null && editorAnim.Frames.Count != 0)
                {
                    var frame = editorAnim.Frames[0];

                    Animation.ProcessAnimation(frame.Entry.SpeedMultiplyer, frame.Entry.Frames.Count, frame.Frame.Delay);

                    d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(frame),
                        x + frame.Frame.PivotX,
                        y + frame.Frame.PivotY,
                        frame.Frame.Width, frame.Frame.Height, false, Transparency);
                }
        }

        public override string GetObjectName()
        {
            return "Launcher";
        }
    }
}
