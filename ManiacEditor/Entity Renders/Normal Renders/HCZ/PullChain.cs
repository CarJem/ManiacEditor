using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class PullChain : EntityRenderer
    {

        public override void Draw(Structures.EntityRenderProp properties)
        {
            Classes.Editor.Draw.GraphicsHandler d = properties.Graphics;
            SceneEntity entity = properties.Object; 
            Classes.Editor.Scene.Sets.EditorEntity e = properties.EditorObject;
            int x = properties.X;
            int y = properties.Y;
            int Transparency = properties.Transparency;
            int index = properties.Index;
            int previousChildCount = properties.PreviousChildCount;
            int platformAngle = properties.PlatformAngle;
            Methods.Entities.EntityAnimator Animation = properties.Animations;
            bool selected  = properties.isSelected;
            int type = (int)entity.attributesMap["type"].ValueEnum;
            bool decorMode = entity.attributesMap["decorMode"].ValueBool;
            int length = (int)entity.attributesMap["length"].ValueUInt32;
            int frameID = 0;
            if (decorMode == true)
            {
                frameID = 1;
            }
            bool fliph = false;
            bool flipv = false;
            var editorAnim = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("PullChain", d.DevicePanel, 0, frameID, fliph, flipv, false);
            var editorAnimChain = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("PullChain", d.DevicePanel, 1, frameID, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnimChain != null && editorAnimChain.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[Animation.index];
                var frameChain = editorAnimChain.Frames[0];

                Animation.ProcessAnimation(frame.Entry.SpeedMultiplyer, frame.Entry.Frames.Count, frame.Frame.Delay);

                d.DrawBitmap(new Classes.Editor.Draw.GraphicsHandler.GraphicsInfo(frame),
                    x + frame.Frame.PivotX - (fliph ? (frame.Frame.Width - editorAnim.Frames[0].Frame.Width) : 0),
                    y + frame.Frame.PivotY + (flipv ? (frame.Frame.Height - editorAnim.Frames[0].Frame.Height) : 0),
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
                if (length != 0)
                {
                    for (int i = 0; i < length; i++)
                    {
                        d.DrawBitmap(new Classes.Editor.Draw.GraphicsHandler.GraphicsInfo(frameChain),
                        x + frameChain.Frame.PivotX,
                        y + frameChain.Frame.PivotY - frameChain.Frame.Height * i,
                        frameChain.Frame.Width, frameChain.Frame.Height, false, Transparency);
                    }
                }
            }
        }

        public override string GetObjectName()
        {
            return "PullChain";
        }
    }
}
