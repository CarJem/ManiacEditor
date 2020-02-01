using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class Spear : EntityRenderer
    {

        public override void Draw(GraphicsHandler d, SceneEntity entity, Classes.Editor.Scene.Sets.EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
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

            var editorAnim = Interfaces.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("Spear", d.DevicePanel, animID, 0, fliph, flipv, false);
            var editorAnimSpear = Interfaces.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("Spear", d.DevicePanel, animID, 1, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnimSpear != null && editorAnimSpear.Frames.Count != 0 && animID >= 0)
            {
                var frame = editorAnim.Frames[0];
                var frameSpear = editorAnimSpear.Frames[0];

                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frameSpear),
                    x + (fliph ? -frameSpear.Frame.PivotX - frameSpear.Frame.Width : frameSpear.Frame.PivotX),
                    y + (flipv ? -frameSpear.Frame.PivotY - frameSpear.Frame.Height : frameSpear.Frame.PivotY),
                    frameSpear.Frame.Width, frameSpear.Frame.Height, false, Transparency);
                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame),
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
