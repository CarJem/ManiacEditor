using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class Bridge : EntityRenderer
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
            var value = entity.attributesMap["length"].ValueUInt8;
            var editorAnim = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("Bridge", d.DevicePanel, 0, 0, false, false, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[0];
                bool wEven = value % 2 == 0;
                for (int xx = 0; xx <= value; ++xx)
                {
                    d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(frame),
                        x + (wEven ? frame.Frame.PivotX : -frame.Frame.Width) + (-value / 2 + xx) * frame.Frame.Width,
                        y + frame.Frame.PivotY,
                        frame.Frame.Width, frame.Frame.Height, false, Transparency);
                }
            }
        }

        public override bool isObjectOnScreen(Classes.Core.Draw.GraphicsHandler d, SceneEntity entity, Classes.Core.Scene.Sets.EditorEntity e, int x, int y, int Transparency)
        {
            var length = entity.attributesMap["length"].ValueUInt8;
            int widthPixels = length * 16;
            int heightPixels = 16;
            return d.IsObjectOnScreen(x - widthPixels / 2, y - heightPixels / 2, widthPixels + 15, heightPixels);
        }

        public override string GetObjectName()
        {
            return "Bridge";
        }
    }
}
