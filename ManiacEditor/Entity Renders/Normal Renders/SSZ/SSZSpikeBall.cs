using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class SSZSpikeBall : EntityRenderer
    {

        public override void Draw(GraphicsHandler d, SceneEntity entity, Classes.Editor.Scene.Sets.EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
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
                    d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame),
                       x + frame.Frame.PivotX,
                       y + frame.Frame.PivotY,
                       frame.Frame.Width, frame.Frame.Height, false, Transparency);
                }
                else
                {
                    d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frameSpike),
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
