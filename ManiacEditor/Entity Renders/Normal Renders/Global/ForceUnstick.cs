using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class ForceUnstick : EntityRenderer
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
            var width = (int)(entity.attributesMap["width"].ValueUInt8);
            var height = (int)(entity.attributesMap["height"].ValueUInt8);
            bool breakClimb = entity.attributesMap["breakClimb"].ValueBool;
            int type;
            if (breakClimb)
                type = 9;
            else
                type = 6;
            var editorAnim = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("ItemBox", d.DevicePanel, 2, type, false, false, false);
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
                        d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(frame),
                            x + (wEven ? frame.Frame.PivotX : -frame.Frame.Width) + (-width / 2 + xx) * frame.Frame.Width,
                            y + (hEven ? frame.Frame.PivotY : -frame.Frame.Height) + (-height / 2 + yy) * frame.Frame.Height,
                            frame.Frame.Width, frame.Frame.Height, false, Transparency);
                    }
                }
            }
        }

        public override bool isObjectOnScreen(Classes.Core.Draw.GraphicsHandler d, SceneEntity entity, Classes.Core.Scene.Sets.EditorEntity e, int x, int y, int Transparency)
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
