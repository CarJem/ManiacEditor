using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class Armadiloid : EntityRenderer
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
            bool fliph = false;
            bool flipv = false;
            var editorAnim = Controls.Editor.MainEditor.Instance.EntityDrawing.LoadAnimation2("Armadiloid", d.DevicePanel, 0, 0, fliph, flipv, false);
            var editorAnimHead = Controls.Editor.MainEditor.Instance.EntityDrawing.LoadAnimation2("Armadiloid", d.DevicePanel, 1, 0, fliph, flipv, false);
            var editorAnimBoost = Controls.Editor.MainEditor.Instance.EntityDrawing.LoadAnimation2("Armadiloid", d.DevicePanel, 3, -1, fliph, flipv, false);
            var editorAnimRider = Controls.Editor.MainEditor.Instance.EntityDrawing.LoadAnimation2("Armadiloid", d.DevicePanel, 4, -1, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnimHead != null && editorAnimHead.Frames.Count != 0 && editorAnimBoost != null && editorAnimBoost.Frames.Count != 0 && editorAnimRider != null && editorAnimRider.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[0];
                var frameHead = editorAnimHead.Frames[0];
                var frameBoost = editorAnimBoost.Frames[Animation.index];
                var frameRider = editorAnimRider.Frames[Animation.index];

                if (type == 0)
                {
                    Animation.ProcessAnimation(frameBoost.Entry.SpeedMultiplyer, frameBoost.Entry.Frames.Count, frameBoost.Frame.Delay);

                    d.DrawBitmap(new Classes.Editor.Draw.GraphicsHandler.GraphicsInfo(frame),
                        x + frame.Frame.PivotX - (fliph ? (frame.Frame.Width - editorAnim.Frames[0].Frame.Width) : 0),
                        y + frame.Frame.PivotY + (flipv ? (frame.Frame.Height - editorAnim.Frames[0].Frame.Height) : 0),
                        frame.Frame.Width, frame.Frame.Height, false, Transparency);

                    d.DrawBitmap(new Classes.Editor.Draw.GraphicsHandler.GraphicsInfo(frameHead),
                        x + frameHead.Frame.PivotX - (fliph ? (frameHead.Frame.Width - editorAnimHead.Frames[0].Frame.Width) : 0),
                        y + frameHead.Frame.PivotY + (flipv ? (frameHead.Frame.Height - editorAnimHead.Frames[0].Frame.Height) : 0),
                        frameHead.Frame.Width, frameHead.Frame.Height, false, Transparency);

                    d.DrawBitmap(new Classes.Editor.Draw.GraphicsHandler.GraphicsInfo(frameBoost),
                        x + frameBoost.Frame.PivotX - (fliph ? (frameBoost.Frame.Width - editorAnimBoost.Frames[0].Frame.Width) : 0),
                        y + frameBoost.Frame.PivotY + (flipv ? (frameBoost.Frame.Height - editorAnimBoost.Frames[0].Frame.Height) : 0),
                        frameBoost.Frame.Width, frameBoost.Frame.Height, false, Transparency);
                }
                else if (type == 1)
                {
                    Animation.ProcessAnimation(frameRider.Entry.SpeedMultiplyer, frameRider.Entry.Frames.Count, frameRider.Frame.Delay);

                    d.DrawBitmap(new Classes.Editor.Draw.GraphicsHandler.GraphicsInfo(frameRider),
                        x + frameRider.Frame.PivotX - (fliph ? (frameRider.Frame.Width - editorAnimRider.Frames[0].Frame.Width) : 0),
                        y + frameRider.Frame.PivotY + (flipv ? (frameRider.Frame.Height - editorAnimRider.Frames[0].Frame.Height) : 0),
                        frameRider.Frame.Width, frameRider.Frame.Height, false, Transparency);
                }

            }
        }

        public override string GetObjectName()
        {
            return "Armadiloid";
        }
    }
}
