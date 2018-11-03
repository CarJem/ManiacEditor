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
    public class SchrodingersCapsule : EntityRenderer
    {

        public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency)
        {
            bool fliph = false;
            bool flipv = false;
            var editorAnim = e.LoadAnimation2("SchrodingersCapsule", d, 0, 0, fliph, flipv, false);
            var editorAnimInside = e.LoadAnimation2("SchrodingersCapsule", d, 0, 2, fliph, flipv, false);
            var editorAnimExclamation = e.LoadAnimation2("SchrodingersCapsule", d, 0, 3, fliph, flipv, false);
            var editorAnimButton = e.LoadAnimation2("SchrodingersCapsule", d, 1, -1, fliph, flipv, false);

            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnimButton != null && editorAnimButton.Frames.Count != 0 && editorAnimInside != null && editorAnimInside.Frames.Count != 0 && editorAnimExclamation != null && editorAnimExclamation.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[0];
                var insideFrame = editorAnimInside.Frames[0];
                var exclamationFrame = editorAnimExclamation.Frames[0];
                var buttonFrame = editorAnimButton.Frames[0];


                d.DrawBitmap(buttonFrame.Texture,
                    x + buttonFrame.Frame.CenterX - (fliph ? (buttonFrame.Frame.Width - editorAnimButton.Frames[0].Frame.Width) : 0),
                    y + exclamationFrame.Frame.CenterY + (flipv ? (buttonFrame.Frame.Height - editorAnimButton.Frames[0].Frame.Height) : 0),
                    buttonFrame.Frame.Width, buttonFrame.Frame.Height, false, Transparency);
                d.DrawBitmap(exclamationFrame.Texture,
                    x + exclamationFrame.Frame.CenterX - (fliph ? (exclamationFrame.Frame.Width - editorAnimExclamation.Frames[3].Frame.Width) : 0),
                    y + exclamationFrame.Frame.CenterY + (flipv ? (exclamationFrame.Frame.Height - editorAnimExclamation.Frames[3].Frame.Height) : 0),
                    exclamationFrame.Frame.Width, exclamationFrame.Frame.Height, false, Transparency);
                d.DrawBitmap(insideFrame.Texture,
                    x + insideFrame.Frame.CenterX - (fliph ? (insideFrame.Frame.Width - editorAnimInside.Frames[2].Frame.Width) : 0),
                    y + insideFrame.Frame.CenterY + (flipv ? (insideFrame.Frame.Height - editorAnimInside.Frames[2].Frame.Height) : 0),
                    insideFrame.Frame.Width, insideFrame.Frame.Height, false, Transparency);



                var mightyAnim = e.LoadAnimation2("SchrodingersCapsule", d, 3, -1, fliph, flipv, false);
                var rayAnim = e.LoadAnimation2("SchrodingersCapsule", d, 4, -1, fliph, flipv, false);

                if (mightyAnim != null && mightyAnim.Frames.Count != 0 && rayAnim != null && rayAnim.Frames.Count != 0)
                {
                    var mightyFrame = mightyAnim.Frames[e.EditorAnimations.index2 < 6 ? e.EditorAnimations.index2 : 0];
                    var rayFrame = rayAnim.Frames[e.index];

                    e.ProcessAnimation(rayFrame.Entry.FrameSpeed, rayFrame.Entry.Frames.Count, rayFrame.Frame.Duration);
                    //e.EditorAnimations.ProcessAnimation2(mightyFrame.Entry.FrameSpeed, mightyFrame.Entry.Frames.Count, mightyFrame.Frame.Duration);

                    d.DrawBitmap(mightyFrame.Texture,
                        x + mightyFrame.Frame.CenterX - (fliph ? (mightyFrame.Frame.Width - mightyAnim.Frames[0].Frame.Width) : 0) + 15,
                        y + mightyFrame.Frame.CenterY + (flipv ? (mightyFrame.Frame.Height - mightyAnim.Frames[0].Frame.Height) : 0),
                        mightyFrame.Frame.Width, mightyFrame.Frame.Height, false, Transparency);
                    d.DrawBitmap(rayFrame.Texture,
                        x + rayFrame.Frame.CenterX - (fliph ? (rayFrame.Frame.Width - rayAnim.Frames[0].Frame.Width) : 0) - 15,
                        y + rayFrame.Frame.CenterY + (flipv ? (rayFrame.Frame.Height - rayAnim.Frames[0].Frame.Height) : 0),
                        rayFrame.Frame.Width, rayFrame.Frame.Height, false, Transparency);
                }

                var editorAnimGlass = e.LoadAnimation2("SchrodingersCapsule", d, 2, -1, fliph, flipv, false);

                if (editorAnimGlass != null && editorAnimGlass.Frames.Count != 0)
                {
                    var glassFrame = editorAnimGlass.Frames[e.EditorAnimations.index2];

                    e.EditorAnimations.ProcessAnimation3(glassFrame.Entry.FrameSpeed, glassFrame.Entry.Frames.Count, glassFrame.Frame.Duration);

                    d.DrawBitmap(glassFrame.Texture,
                        x + glassFrame.Frame.CenterX - (fliph ? (glassFrame.Frame.Width - editorAnimGlass.Frames[0].Frame.Width) : 0),
                        y + glassFrame.Frame.CenterY + (flipv ? (glassFrame.Frame.Height - editorAnimGlass.Frames[0].Frame.Height) : 0),
                        glassFrame.Frame.Width, glassFrame.Frame.Height, false, 50);
                }





                d.DrawBitmap(frame.Texture,
                    x + frame.Frame.CenterX - (fliph ? (frame.Frame.Width - editorAnim.Frames[0].Frame.Width) : 0),
                    y + frame.Frame.CenterY + (flipv ? (frame.Frame.Height - editorAnim.Frames[0].Frame.Height) : 0),
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
                d.DrawBitmap(frame.Texture,
                    x + frame.Frame.CenterX - (fliph ? (frame.Frame.Width - editorAnim.Frames[0].Frame.Width) : 0),
                    y + frame.Frame.CenterY + (flipv ? (frame.Frame.Height - editorAnim.Frames[0].Frame.Height) : 0),
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }



        }

        public override string GetObjectName()
        {
            return "SchrodingersCapsule";
        }
    }
}
