using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class MegaOctus : EntityRenderer
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
            var editorAnim = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("MegaOctus", d.DevicePanel, 0, 0, false, false, false);
            var editorAnimEye = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("MegaOctus", d.DevicePanel, 0, 1, false, false, false);
            var editorAnimPupil = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("MegaOctus", d.DevicePanel, 0, 2, false, false, false);
            var editorAnimHeadJoint = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("MegaOctus", d.DevicePanel, 0, 6, false, false, false);
            var editorAnimNose = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("MegaOctus", d.DevicePanel, 1, 0, false, false, false);
            var editorAnimBolt = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("MegaOctus", d.DevicePanel, 2, 0, false, false, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnimEye != null && editorAnimEye.Frames.Count != 0 && editorAnimPupil != null && editorAnimPupil.Frames.Count != 0 && editorAnimHeadJoint != null && editorAnimHeadJoint.Frames.Count != 0 && editorAnimNose != null && editorAnimNose.Frames.Count != 0 && editorAnimBolt != null && editorAnimBolt.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[0];
                var frameEye = editorAnimEye.Frames[0];
                var framePupil = editorAnimPupil.Frames[0];
                var frameNose = editorAnimNose.Frames[0];
                var frameJoint = editorAnimHeadJoint.Frames[0];
                var frameBolt = editorAnimBolt.Frames[0];

                d.DrawBitmap(new Classes.Editor.Draw.GraphicsHandler.GraphicsInfo(frame),
                    x + frame.Frame.PivotX,
                    y + frame.Frame.PivotY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
                d.DrawBitmap(new Classes.Editor.Draw.GraphicsHandler.GraphicsInfo(frameBolt),
                    x + frameBolt.Frame.PivotX,
                    y + frameBolt.Frame.PivotY,
                    frameBolt.Frame.Width, frameBolt.Frame.Height, false, Transparency);

                d.DrawBitmap(new Classes.Editor.Draw.GraphicsHandler.GraphicsInfo(frameEye),
                    x + frameEye.Frame.PivotX + 35,
                    y + frameEye.Frame.PivotY,
                    frameEye.Frame.Width, frameEye.Frame.Height, false, Transparency);
                d.DrawBitmap(new Classes.Editor.Draw.GraphicsHandler.GraphicsInfo(framePupil),
                    x + framePupil.Frame.PivotX + 40,
                    y + framePupil.Frame.PivotY,
                    framePupil.Frame.Width, framePupil.Frame.Height, false, Transparency);


                d.DrawBitmap(new Classes.Editor.Draw.GraphicsHandler.GraphicsInfo(frameEye),
                    x + frameEye.Frame.PivotX + 20,
                    y + frameEye.Frame.PivotY,
                    frameEye.Frame.Width, frameEye.Frame.Height, false, Transparency);
                d.DrawBitmap(new Classes.Editor.Draw.GraphicsHandler.GraphicsInfo(framePupil),
                    x + framePupil.Frame.PivotX + 20,
                    y + framePupil.Frame.PivotY,
                    framePupil.Frame.Width, framePupil.Frame.Height, false, Transparency);




                d.DrawBitmap(new Classes.Editor.Draw.GraphicsHandler.GraphicsInfo(frameNose),
                    x + frameNose.Frame.PivotX,
                    y + frameNose.Frame.PivotY,
                    frameNose.Frame.Width, frameNose.Frame.Height, false, Transparency);
                d.DrawBitmap(new Classes.Editor.Draw.GraphicsHandler.GraphicsInfo(frameJoint),
                    x + frameJoint.Frame.PivotX,
                    y + frameJoint.Frame.PivotY,
                    frameJoint.Frame.Width, frameJoint.Frame.Height, false, Transparency);
            }
        }

        public override string GetObjectName()
        {
            return "MegaOctus";
        }
    }
}
