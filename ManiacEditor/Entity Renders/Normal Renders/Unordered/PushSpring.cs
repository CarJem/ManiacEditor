using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class PushSpring : EntityRenderer
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

            int type = (int)entity.attributesMap["type"].ValueUInt8;
            int direction = (int)entity.attributesMap["direction"].ValueUInt8;
            int animID = 0;

            switch (type)
            {
                case 0:
                    break;
                case 1:
                    animID = 1;
                    break;
                default:
                    break;
            }

            switch (direction)
            {
                case 0:
                    break;
                case 1:
                    fliph = true;
                    break;
                case 2:
                    flipv = true;
                    break;
                case 3:
                    fliph = true;
                    flipv = true;
                    break;
            }

            var editorAnim = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("PushSpring", d.DevicePanel, animID, 0, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[0];

                d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(frame),
                    x + (fliph ? -frame.Frame.PivotX - frame.Frame.Width : frame.Frame.PivotX),
                    y + (flipv ? -frame.Frame.PivotY - frame.Frame.Height : frame.Frame.PivotY),
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);

            }
        }

        public override string GetObjectName()
        {
            return "PushSpring";
        }
    }
}
