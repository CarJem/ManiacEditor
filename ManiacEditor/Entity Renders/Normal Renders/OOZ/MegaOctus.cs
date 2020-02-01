using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class MegaOctus : EntityRenderer
    {

        public override void Draw(GraphicsHandler d, SceneEntity entity, Classes.Edit.Scene.Sets.EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            var editorAnim = Editor.Instance.EntityDrawing.LoadAnimation2("MegaOctus", d.DevicePanel, 0, 0, false, false, false);
            var editorAnimEye = Editor.Instance.EntityDrawing.LoadAnimation2("MegaOctus", d.DevicePanel, 0, 1, false, false, false);
            var editorAnimPupil = Editor.Instance.EntityDrawing.LoadAnimation2("MegaOctus", d.DevicePanel, 0, 2, false, false, false);
            var editorAnimHeadJoint = Editor.Instance.EntityDrawing.LoadAnimation2("MegaOctus", d.DevicePanel, 0, 6, false, false, false);
            var editorAnimNose = Editor.Instance.EntityDrawing.LoadAnimation2("MegaOctus", d.DevicePanel, 1, 0, false, false, false);
            var editorAnimBolt = Editor.Instance.EntityDrawing.LoadAnimation2("MegaOctus", d.DevicePanel, 2, 0, false, false, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnimEye != null && editorAnimEye.Frames.Count != 0 && editorAnimPupil != null && editorAnimPupil.Frames.Count != 0 && editorAnimHeadJoint != null && editorAnimHeadJoint.Frames.Count != 0 && editorAnimNose != null && editorAnimNose.Frames.Count != 0 && editorAnimBolt != null && editorAnimBolt.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[0];
                var frameEye = editorAnimEye.Frames[0];
                var framePupil = editorAnimPupil.Frames[0];
                var frameNose = editorAnimNose.Frames[0];
                var frameJoint = editorAnimHeadJoint.Frames[0];
                var frameBolt = editorAnimBolt.Frames[0];

                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame),
                    x + frame.Frame.PivotX,
                    y + frame.Frame.PivotY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frameBolt),
                    x + frameBolt.Frame.PivotX,
                    y + frameBolt.Frame.PivotY,
                    frameBolt.Frame.Width, frameBolt.Frame.Height, false, Transparency);

                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frameEye),
                    x + frameEye.Frame.PivotX + 35,
                    y + frameEye.Frame.PivotY,
                    frameEye.Frame.Width, frameEye.Frame.Height, false, Transparency);
                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(framePupil),
                    x + framePupil.Frame.PivotX + 40,
                    y + framePupil.Frame.PivotY,
                    framePupil.Frame.Width, framePupil.Frame.Height, false, Transparency);


                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frameEye),
                    x + frameEye.Frame.PivotX + 20,
                    y + frameEye.Frame.PivotY,
                    frameEye.Frame.Width, frameEye.Frame.Height, false, Transparency);
                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(framePupil),
                    x + framePupil.Frame.PivotX + 20,
                    y + framePupil.Frame.PivotY,
                    framePupil.Frame.Width, framePupil.Frame.Height, false, Transparency);




                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frameNose),
                    x + frameNose.Frame.PivotX,
                    y + frameNose.Frame.PivotY,
                    frameNose.Frame.Width, frameNose.Frame.Height, false, Transparency);
                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frameJoint),
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
