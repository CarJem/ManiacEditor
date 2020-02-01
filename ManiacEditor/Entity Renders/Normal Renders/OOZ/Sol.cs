using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class Sol : EntityRenderer
    {

        public override void Draw(GraphicsHandler d, SceneEntity entity, Classes.Editor.Scene.Sets.EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            bool fliph = false;
            bool flipv = false;
            int direction = (int)entity.attributesMap["direction"].ValueUInt8;
            bool fireOrbs = entity.attributesMap["fireOrbs"].ValueBool;

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
            }

            var editorAnim = Interfaces.Base.MapEditor.Instance.EntityDrawing.LoadAnimation2("Sol", d.DevicePanel, 0, 0, fliph, flipv, false);
            var editorAnim2 = Interfaces.Base.MapEditor.Instance.EntityDrawing.LoadAnimation2("Sol", d.DevicePanel, 1, -1, false, false, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnim2 != null && editorAnim2.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[0];
                var frame2 = editorAnim2.Frames[Animation.index];

                Animation.ProcessAnimation(frame2.Entry.SpeedMultiplyer, frame2.Entry.Frames.Count, frame2.Frame.Delay);

                if (!fireOrbs)
                {
                    d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame2),
                        x + frame2.Frame.PivotX + 16,
                        y + frame2.Frame.PivotY,
                        frame2.Frame.Width, frame2.Frame.Height, false, Transparency);
                    d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame2),
                        x - frame2.Frame.PivotX - 30,
                        y + frame2.Frame.PivotY,
                        frame2.Frame.Width, frame2.Frame.Height, false, Transparency);
                }
                else
                {
                    d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame2),
                        x + frame2.Frame.PivotX + 16,
                        y + frame2.Frame.PivotY,
                        frame2.Frame.Width, frame2.Frame.Height, false, Transparency-100);
                    d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame2),
                        x - frame2.Frame.PivotX - 30,
                        y + frame2.Frame.PivotY,
                        frame2.Frame.Width, frame2.Frame.Height, false, Transparency-100);
                }


                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame),
                    x + frame.Frame.PivotX,
                    y + frame.Frame.PivotY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);

            }
        }

        public override string GetObjectName()
        {
            return "Sol";
        }
    }
}
