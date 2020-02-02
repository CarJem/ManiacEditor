using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class Vultron : EntityRenderer
    {

        public override void Draw(Classes.Core.Draw.GraphicsHandler d, SceneEntity entity, Classes.Core.Scene.Sets.EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            int type = (int)entity.attributesMap["type"].ValueUInt8;
            int direction = (int)entity.attributesMap["direction"].ValueUInt8; 
            bool fliph = false;
            bool flipv = false;
            int frameID;
            switch (type)
            {
                case 0:
                    frameID = 0;
                    break;
                case 1:
                    frameID = 5;
                    break;
                default:
                    frameID = 0;
                    break;
            }
            switch(direction)
            {
                case 0:
                    fliph = false;
                    break;
                case 1:
                    fliph = true;
                    break;
                default:
                    fliph = false;
                    break;
            }
            var editorAnim = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("Vultron", d.DevicePanel, 0, frameID, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[0];

                d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(frame),
                    x + frame.Frame.PivotX,
                    y + frame.Frame.PivotY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }
        }

        public override string GetObjectName()
        {
            return "Vultron";
        }
    }
}
