using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class ForceUnstick : EntityRenderer
    {

        public override void Draw(GraphicsHandler d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            var width = (int)(entity.attributesMap["width"].ValueUInt8);
            var height = (int)(entity.attributesMap["height"].ValueUInt8);
            bool breakClimb = entity.attributesMap["breakClimb"].ValueBool;
            int type;
            if (breakClimb)
                type = 9;
            else
                type = 6;
            var editorAnim = Editor.Instance.EntityDrawing.LoadAnimation2("ItemBox", d.DevicePanel, 2, type, false, false, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[Animation.index];
                Animation.ProcessAnimation(frame.Entry.SpeedMultiplyer, frame.Entry.Frames.Count, frame.Frame.Delay);
                bool wEven = width % 2 == 0;
                bool hEven = height % 2 == 0;
                for (int xx = 0; xx <= width; ++xx)
                {
                    for (int yy = 0; yy <= height; ++yy)
                    {
                        d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame),
                            x + (wEven ? frame.Frame.PivotX : -frame.Frame.Width) + (-width / 2 + xx) * frame.Frame.Width,
                            y + (hEven ? frame.Frame.PivotY : -frame.Frame.Height) + (-height / 2 + yy) * frame.Frame.Height,
                            frame.Frame.Width, frame.Frame.Height, false, Transparency);
                    }
                }
            }
        }

        public override bool isObjectOnScreen(GraphicsHandler d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency)
        {
            var width = (int)(entity.attributesMap["width"].ValueUInt8);
            var height = (int)(entity.attributesMap["height"].ValueUInt8);
            int widthPixels = width * 16;
            int heightPixels = height * 16;
            return d.IsObjectOnScreen(x - 8 - widthPixels / 2, y - 8 - heightPixels / 2, widthPixels + 8, heightPixels + 8);
        }

        public override string GetObjectName()
        {
            return "ForceUnstick";
        }
    }
}
