using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class Technosqueek : EntityRenderer
    {

        public override void Draw(GraphicsHandler d, SceneEntity entity, Classes.Editor.Scene.Sets.EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            int type = (int)entity.attributesMap["type"].ValueUInt8;
            int direction = (int)entity.attributesMap["direction"].ValueUInt8;
            bool fliph = false;
            bool flipv = false;
            int animID = 0;
            if (type == 0)
            {
                animID = 0;
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
            }
            else if (type == 1)
            {
                animID = 3;
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
            }
            else if (type == 2)
            {
                animID = 0;
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

            }
            else if (type == 3)
            {
                animID = 3;
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

            }
            var editorAnim = Editor.Instance.EntityDrawing.LoadAnimation2("Technosqueek", d.DevicePanel, animID, -1, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && animID >= 0)
            {
                var frame = editorAnim.Frames[Animation.index];

                Animation.ProcessAnimation(frame.Entry.SpeedMultiplyer, frame.Entry.Frames.Count, frame.Frame.Delay);

                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame),
                    x + frame.Frame.PivotX - (fliph ? (frame.Frame.Width - editorAnim.Frames[0].Frame.Width - 2) : 0),
                    y + frame.Frame.PivotY + (flipv ? (frame.Frame.Height - editorAnim.Frames[0].Frame.Height + 2) : 0),
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }
        }

        public override string GetObjectName()
        {
            return "Technosqueek";
        }
    }
}
