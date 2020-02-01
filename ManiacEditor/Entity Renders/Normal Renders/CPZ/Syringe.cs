using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class Syringe : EntityRenderer
    {

        public override void Draw(GraphicsHandler d, SceneEntity entity, Classes.Editor.Scene.Sets.EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            int type = (int)entity.attributesMap["type"].ValueEnum;
            System.Drawing.Color colour;
            switch (type)
            {
                case 0:
                    colour = System.Drawing.Color.FromArgb(140, 0, 8, 192);
                    break;
                case 1:
                    colour = System.Drawing.Color.FromArgb(140, 8, 184, 0);
                    break;
                case 2:
                    colour = System.Drawing.Color.FromArgb(140, 56, 168, 240);
                    break;
                default:
                    colour = System.Drawing.Color.FromArgb(140, 56, 168, 240);
                    break;
            }
            bool fliph = false;
            bool flipv = false;
            int animID = 0;
            var editorAnim = Editor.Instance.EntityDrawing.LoadAnimation2("Syringe", d.DevicePanel, animID, -1, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && animID >= 0)
            {
                var frame = editorAnim.Frames[0];
                var frame2 = editorAnim.Frames[1];

                //ProcessAnimation(frame.Entry.SpeedMultiplyer, frame.Entry.Frames.Count, frame.Frame.Delay);
                d.DrawRectangle(x - 14, y + 80, x + 14, y, colour);
                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame),
                    x + frame.Frame.PivotX - (fliph ? (frame.Frame.Width - editorAnim.Frames[0].Frame.Width) : 0),
                    y + frame.Frame.PivotY + (flipv ? (frame.Frame.Height - editorAnim.Frames[0].Frame.Height) : 0),
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame2),
                    x + frame2.Frame.PivotX - (fliph ? (frame2.Frame.Width - editorAnim.Frames[0].Frame.Width) : 0),
                    y + frame2.Frame.PivotY + (flipv ? (frame2.Frame.Height - editorAnim.Frames[0].Frame.Height) : 0),
                    frame2.Frame.Width, frame2.Frame.Height, false, Transparency);

            }
        }

        public override string GetObjectName()
        {
            return "Syringe";
        }
    }
}
