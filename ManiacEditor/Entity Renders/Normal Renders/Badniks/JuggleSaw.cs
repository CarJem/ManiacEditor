using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class JuggleSaw : EntityRenderer
    {

        public override void Draw(Structures.EntityRenderProp properties)
        {
            Methods.Draw.GraphicsHandler d = properties.Graphics;
            SceneEntity entity = properties.Object; 
            Classes.Scene.Sets.EditorEntity e = properties.EditorObject;
            int x = properties.X;
            int y = properties.Y;
            int Transparency = properties.Transparency;
            int index = properties.Index;
            int previousChildCount = properties.PreviousChildCount;
            int platformAngle = properties.PlatformAngle;
            Methods.Entities.EntityAnimator Animation = properties.Animations;
            bool selected  = properties.isSelected;
            int direction = (int)entity.attributesMap["direction"].ValueUInt8;
            bool hasSaw = entity.attributesMap["hasSaw"].ValueBool;
            bool fliph = false;
            bool flipv = false;
            int animID;
            if (direction == 2)
            {
                if (hasSaw)
                {
                    animID = 4;
                }
                else
                {
                    animID = 3;
                }

            }
            else if (direction == 3)
            {
                fliph = true;
                if (hasSaw)
                {
                    animID = 4;
                }
                else
                {
                    animID = 3;
                }

            }
            else
            {
                if (hasSaw)
                {
                    animID = 1;
                }
                else
                {
                    animID = 0;
                }
            }
            if (direction == 1)
            {
                flipv = true;
            }
            var editorAnim = Controls.Editor.MainEditor.Instance.EntityDrawing.LoadAnimation2("JuggleSaw", d.DevicePanel, animID, -1, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && animID >= 0)
            {
                var frame = editorAnim.Frames[Animation.index];

                Animation.ProcessAnimation(frame.Entry.SpeedMultiplyer, frame.Entry.Frames.Count, frame.Frame.Delay);

                d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frame),
                    x + frame.Frame.PivotX - (fliph ? (frame.Frame.Width - editorAnim.Frames[0].Frame.Width + (hasSaw ? (38) : 16)) : 0),
                    y + frame.Frame.PivotY + (flipv ? (frame.Frame.Height - editorAnim.Frames[0].Frame.Height + (hasSaw ? (37) : 15)) : 0),
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }
        }

        public override string GetObjectName()
        {
            return "JuggleSaw";
        }
    }
}
