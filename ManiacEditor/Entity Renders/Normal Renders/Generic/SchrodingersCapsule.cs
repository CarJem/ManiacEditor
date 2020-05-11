﻿using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class SchrodingersCapsule : EntityRenderer
    {

        public override void Draw(Structures.EntityRenderProp properties)
        {
            Methods.Draw.GraphicsHandler d = properties.Graphics;
            SceneEntity entity = properties.Object; 
            Classes.Scene.Sets.EditorEntity e = properties.EditorObject;
            int x = properties.X;
            int y = properties.Y;
            int Transparency = properties.Transparency;
            int index = properties.Index;
            int previousChildCount = properties.PreviousChildCount;
            int platformAngle = properties.PlatformAngle;
            Methods.Entities.EntityAnimator Animation = properties.Animations;
            bool selected  = properties.isSelected;
            bool fliph = false;
            bool flipv = false;
            var editorAnim = Controls.Editor.MainEditor.Instance.EntityDrawing.LoadAnimation2("SchrodingersCapsule", d.DevicePanel, 0, 0, fliph, flipv, false);
            var editorAnimInside = Controls.Editor.MainEditor.Instance.EntityDrawing.LoadAnimation2("SchrodingersCapsule", d.DevicePanel, 0, 2, fliph, flipv, false);
            var editorAnimExclamation = Controls.Editor.MainEditor.Instance.EntityDrawing.LoadAnimation2("SchrodingersCapsule", d.DevicePanel, 0, 3, fliph, flipv, false);
            var editorAnimButton = Controls.Editor.MainEditor.Instance.EntityDrawing.LoadAnimation2("SchrodingersCapsule", d.DevicePanel, 1, -1, fliph, flipv, false);

            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnimButton != null && editorAnimButton.Frames.Count != 0 && editorAnimInside != null && editorAnimInside.Frames.Count != 0 && editorAnimExclamation != null && editorAnimExclamation.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[0];
                var insideFrame = editorAnimInside.Frames[0];
                var exclamationFrame = editorAnimExclamation.Frames[0];
                var buttonFrame = editorAnimButton.Frames[0];


                d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(buttonFrame),
                    x + buttonFrame.Frame.PivotX - (fliph ? (buttonFrame.Frame.Width - editorAnimButton.Frames[0].Frame.Width) : 0),
                    y + exclamationFrame.Frame.PivotY + (flipv ? (buttonFrame.Frame.Height - editorAnimButton.Frames[0].Frame.Height) : 0),
                    buttonFrame.Frame.Width, buttonFrame.Frame.Height, false, Transparency);
                d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(exclamationFrame),
                    x + exclamationFrame.Frame.PivotX - (fliph ? (exclamationFrame.Frame.Width - editorAnimExclamation.Frames[3].Frame.Width) : 0),
                    y + exclamationFrame.Frame.PivotY + (flipv ? (exclamationFrame.Frame.Height - editorAnimExclamation.Frames[3].Frame.Height) : 0),
                    exclamationFrame.Frame.Width, exclamationFrame.Frame.Height, false, Transparency);
                d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(insideFrame),
                    x + insideFrame.Frame.PivotX - (fliph ? (insideFrame.Frame.Width - editorAnimInside.Frames[2].Frame.Width) : 0),
                    y + insideFrame.Frame.PivotY + (flipv ? (insideFrame.Frame.Height - editorAnimInside.Frames[2].Frame.Height) : 0),
                    insideFrame.Frame.Width, insideFrame.Frame.Height, false, Transparency);



                var mightyAnim = Controls.Editor.MainEditor.Instance.EntityDrawing.LoadAnimation2("SchrodingersCapsule", d.DevicePanel, 3, -1, fliph, flipv, false);
                var rayAnim = Controls.Editor.MainEditor.Instance.EntityDrawing.LoadAnimation2("SchrodingersCapsule", d.DevicePanel, 4, -1, fliph, flipv, false);

                if (mightyAnim != null && mightyAnim.Frames.Count != 0 && rayAnim != null && rayAnim.Frames.Count != 0)
                {
                    var mightyFrame = mightyAnim.Frames[Animation.index2 < 6 ? Animation.index2 : 0];
                    var rayFrame = rayAnim.Frames[Animation.index];

                    Animation.ProcessAnimation(rayFrame.Entry.SpeedMultiplyer, rayFrame.Entry.Frames.Count, rayFrame.Frame.Delay);
                    //Animation.ProcessAnimation2(mightyFrame.Entry.SpeedMultiplyer, mightyFrame.Entry.Frames.Count, mightyFrame.Frame.Delay);

                    d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(mightyFrame),
                        x + mightyFrame.Frame.PivotX - (fliph ? (mightyFrame.Frame.Width - mightyAnim.Frames[0].Frame.Width) : 0) + 15,
                        y + mightyFrame.Frame.PivotY + (flipv ? (mightyFrame.Frame.Height - mightyAnim.Frames[0].Frame.Height) : 0),
                        mightyFrame.Frame.Width, mightyFrame.Frame.Height, false, Transparency);
                    d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(rayFrame),
                        x + rayFrame.Frame.PivotX - (fliph ? (rayFrame.Frame.Width - rayAnim.Frames[0].Frame.Width) : 0) - 15,
                        y + rayFrame.Frame.PivotY + (flipv ? (rayFrame.Frame.Height - rayAnim.Frames[0].Frame.Height) : 0),
                        rayFrame.Frame.Width, rayFrame.Frame.Height, false, Transparency);
                }

                var editorAnimGlass = Controls.Editor.MainEditor.Instance.EntityDrawing.LoadAnimation2("SchrodingersCapsule", d.DevicePanel, 2, -1, fliph, flipv, false);

                if (editorAnimGlass != null && editorAnimGlass.Frames.Count != 0)
                {
                    var glassFrame = editorAnimGlass.Frames[Animation.index2];

                    Animation.ProcessAnimation3(glassFrame.Entry.SpeedMultiplyer, glassFrame.Entry.Frames.Count, glassFrame.Frame.Delay);

                    d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(glassFrame),
                        x + glassFrame.Frame.PivotX - (fliph ? (glassFrame.Frame.Width - editorAnimGlass.Frames[0].Frame.Width) : 0),
                        y + glassFrame.Frame.PivotY + (flipv ? (glassFrame.Frame.Height - editorAnimGlass.Frames[0].Frame.Height) : 0),
                        glassFrame.Frame.Width, glassFrame.Frame.Height, false, 50);
                }





                d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frame),
                    x + frame.Frame.PivotX - (fliph ? (frame.Frame.Width - editorAnim.Frames[0].Frame.Width) : 0),
                    y + frame.Frame.PivotY + (flipv ? (frame.Frame.Height - editorAnim.Frames[0].Frame.Height) : 0),
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
                d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frame),
                    x + frame.Frame.PivotX - (fliph ? (frame.Frame.Width - editorAnim.Frames[0].Frame.Width) : 0),
                    y + frame.Frame.PivotY + (flipv ? (frame.Frame.Height - editorAnim.Frames[0].Frame.Height) : 0),
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }



        }

        public override string GetObjectName()
        {
            return "SchrodingersCapsule";
        }
    }
}