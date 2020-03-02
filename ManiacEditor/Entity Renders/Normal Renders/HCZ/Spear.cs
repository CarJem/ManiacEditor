using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class Spear : EntityRenderer
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
            bool fliph = false; 
            bool flipv = false;
            int orientation = (int)entity.attributesMap["orientation"].ValueUInt8;
            int animID = 0;
            switch (orientation)
            {
                case 1:
                    animID = 1;
                    break;
                case 2:
                    flipv = true;
                    break;
                case 3:
                    fliph = true;
                    animID = 1;
                    break;
            }

            var editorAnim = Controls.Editor.MainEditor.Instance.EntityDrawing.LoadAnimation2("Spear", d.DevicePanel, animID, 0, fliph, flipv, false);
            var editorAnimSpear = Controls.Editor.MainEditor.Instance.EntityDrawing.LoadAnimation2("Spear", d.DevicePanel, animID, 1, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnimSpear != null && editorAnimSpear.Frames.Count != 0 && animID >= 0)
            {
                var frame = editorAnim.Frames[0];
                var frameSpear = editorAnimSpear.Frames[0];

                d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frameSpear),
                    x + (fliph ? -frameSpear.Frame.PivotX - frameSpear.Frame.Width : frameSpear.Frame.PivotX),
                    y + (flipv ? -frameSpear.Frame.PivotY - frameSpear.Frame.Height : frameSpear.Frame.PivotY),
                    frameSpear.Frame.Width, frameSpear.Frame.Height, false, Transparency);
                d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frame),
                    x + (fliph ? -frame.Frame.PivotX - frame.Frame.Width : frame.Frame.PivotX),
                    y + (flipv ? -frame.Frame.PivotY - frame.Frame.Height : frame.Frame.PivotY),
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);

            }
        }

        public override string GetObjectName()
        {
            return "Spear";
        }
    }
}
