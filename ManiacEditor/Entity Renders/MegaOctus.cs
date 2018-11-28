using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ManiacEditor;
using Microsoft.Xna.Framework;
using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class MegaOctus : EntityRenderer
    {

        public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            var editorAnim = EditorEntity_ini.LoadAnimation2("MegaOctus", d, 0, 0, false, false, false);
            var editorAnimEye = EditorEntity_ini.LoadAnimation2("MegaOctus", d, 0, 1, false, false, false);
            var editorAnimPupil = EditorEntity_ini.LoadAnimation2("MegaOctus", d, 0, 2, false, false, false);
            var editorAnimHeadJoint = EditorEntity_ini.LoadAnimation2("MegaOctus", d, 0, 6, false, false, false);
            var editorAnimNose = EditorEntity_ini.LoadAnimation2("MegaOctus", d, 1, 0, false, false, false);
            var editorAnimBolt = EditorEntity_ini.LoadAnimation2("MegaOctus", d, 2, 0, false, false, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnimEye != null && editorAnimEye.Frames.Count != 0 && editorAnimPupil != null && editorAnimPupil.Frames.Count != 0 && editorAnimHeadJoint != null && editorAnimHeadJoint.Frames.Count != 0 && editorAnimNose != null && editorAnimNose.Frames.Count != 0 && editorAnimBolt != null && editorAnimBolt.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[0];
                var frameEye = editorAnimEye.Frames[0];
                var framePupil = editorAnimPupil.Frames[0];
                var frameNose = editorAnimNose.Frames[0];
                var frameJoint = editorAnimHeadJoint.Frames[0];
                var frameBolt = editorAnimBolt.Frames[0];

                d.DrawBitmap(frame.Texture,
                    x + frame.Frame.CenterX,
                    y + frame.Frame.CenterY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
                d.DrawBitmap(frameBolt.Texture,
                    x + frameBolt.Frame.CenterX,
                    y + frameBolt.Frame.CenterY,
                    frameBolt.Frame.Width, frameBolt.Frame.Height, false, Transparency);

                d.DrawBitmap(frameEye.Texture,
                    x + frameEye.Frame.CenterX + 35,
                    y + frameEye.Frame.CenterY,
                    frameEye.Frame.Width, frameEye.Frame.Height, false, Transparency);
                d.DrawBitmap(framePupil.Texture,
                    x + framePupil.Frame.CenterX + 40,
                    y + framePupil.Frame.CenterY,
                    framePupil.Frame.Width, framePupil.Frame.Height, false, Transparency);


                d.DrawBitmap(frameEye.Texture,
                    x + frameEye.Frame.CenterX + 20,
                    y + frameEye.Frame.CenterY,
                    frameEye.Frame.Width, frameEye.Frame.Height, false, Transparency);
                d.DrawBitmap(framePupil.Texture,
                    x + framePupil.Frame.CenterX + 20,
                    y + framePupil.Frame.CenterY,
                    framePupil.Frame.Width, framePupil.Frame.Height, false, Transparency);




                d.DrawBitmap(frameNose.Texture,
                    x + frameNose.Frame.CenterX,
                    y + frameNose.Frame.CenterY,
                    frameNose.Frame.Width, frameNose.Frame.Height, false, Transparency);
                d.DrawBitmap(frameJoint.Texture,
                    x + frameJoint.Frame.CenterX,
                    y + frameJoint.Frame.CenterY,
                    frameJoint.Frame.Width, frameJoint.Frame.Height, false, Transparency);
            }
        }

        public override string GetObjectName()
        {
            return "MegaOctus";
        }
    }
}
