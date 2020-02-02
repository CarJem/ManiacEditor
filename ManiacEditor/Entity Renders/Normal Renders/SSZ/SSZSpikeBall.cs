using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class SSZSpikeBall : EntityRenderer
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
            int direction = (int)entity.attributesMap["direction"].ValueUInt8;
            int type = (int)entity.attributesMap["type"].ValueUInt8;
            bool fliph = false;
            bool flipv = false;
            int animID = 0;
            switch (direction)
            {
                case 0:
                    animID = 0;
                    break;
                case 1:
                    animID = 1;
                    break;
                case 2:
                    animID = 2;
                    break;
                case 3:
                    animID = 3;
                    break;
            } 
            var editorAnim = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("SpikeBall", d.DevicePanel, 0, animID, fliph, flipv, false);
            var editorAnimSpikeBall = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("SpikeBall", d.DevicePanel, 1, 0, fliph, flipv, false);
            if (editorAnim != null && editorAnimSpikeBall != null && editorAnim.Frames.Count != 0 && editorAnimSpikeBall.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[0];
                var frameSpike = editorAnimSpikeBall.Frames[0];

                if (type == 0)
                {
                    d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(frame),
                       x + frame.Frame.PivotX,
                       y + frame.Frame.PivotY,
                       frame.Frame.Width, frame.Frame.Height, false, Transparency);
                }
                else
                {
                    d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(frameSpike),
                       x + frameSpike.Frame.PivotX,
                       y + frameSpike.Frame.PivotY,
                       frameSpike.Frame.Width, frameSpike.Frame.Height, false, Transparency);
                }


            }
        }

        public override string GetObjectName()
        {
            return "SSZSpikeBall";
        }
    }
}
