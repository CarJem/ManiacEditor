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
    public class LaundroMobile : EntityRenderer
    {
        //Shorthanding Setting Files
        Properties.Settings mySettings = Properties.Settings.Default;
        Properties.KeyBinds myKeyBinds = Properties.KeyBinds.Default;
        public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            bool fliph = false;
            bool flipv = false;
            int type = (int)entity.attributesMap["type"].ValueUInt8;
            int blockID = 0;
            int randomNum = entity.SlotID % 3;
            switch (type)
            {
                case 4:
                    blockID = randomNum;
                    break;
                case 5:

                    blockID = randomNum+3;
                    break;
            }
            var editorAnim = e.EditorInstance.EditorEntity_ini.LoadAnimation2("LaundroMobile", d, 0, 0, fliph, flipv, false);
            var editorAnim2 = e.EditorInstance.EditorEntity_ini.LoadAnimation2("LaundroMobile", d, 9, blockID, fliph, flipv, false);
            var editorAnim3 = e.EditorInstance.EditorEntity_ini.LoadAnimation2("LaundroMobile", d, 1, -1, fliph, flipv, false);
            var editorAnim4 = e.EditorInstance.EditorEntity_ini.LoadAnimation2("LaundroMobile", d, 3, -1, fliph, flipv, false);
            var editorAnim5 = e.EditorInstance.EditorEntity_ini.LoadAnimation2("LaundroMobile", d, 8, -1, fliph, flipv, false);
            var editorAnim6 = e.EditorInstance.EditorEntity_ini.LoadAnimation2("LaundroMobile", d, 2, -1, fliph, flipv, false);
            var editorAnim7 = e.EditorInstance.EditorEntity_ini.LoadAnimation2("LaundroMobile", d, 0, 3, fliph, flipv, false);
            var editorAnim8 = e.EditorInstance.EditorEntity_ini.LoadAnimation2("LaundroMobile", d, 0, 2, fliph, flipv, false);
            var editorAnim9 = e.EditorInstance.EditorEntity_ini.LoadAnimation2("LaundroMobile", d, 0, 4, fliph, flipv, false);
            var editorAnimIcon = e.EditorInstance.EditorEntity_ini.LoadAnimation2("EditorIcons2", d, 0, 14, false, false, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnim2 != null && editorAnim2.Frames.Count != 0 && editorAnim3 != null && editorAnim3.Frames.Count != 0 && editorAnim4 != null && editorAnim4.Frames.Count != 0 && editorAnim5 != null && editorAnim5.Frames.Count != 0 && editorAnim6 != null && editorAnim6.Frames.Count != 0 && editorAnim7 != null && editorAnim7.Frames.Count != 0 && editorAnim8 != null && editorAnim8.Frames.Count != 0 && editorAnim9 != null && editorAnim9.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[0];
                var frameBlock = editorAnim2.Frames[0];
                var frameLaundry = editorAnim3.Frames[Animation.index];
                var framePropel = editorAnim4.Frames[Animation.index];
                var frameBomb = editorAnim5.Frames[Animation.index2];
                var frameLaundryCenter = editorAnim6.Frames[Animation.index];
                var frameLaundryCenterBottom = editorAnim7.Frames[0];
                var frameLaundryCenterTop = editorAnim8.Frames[0];
                var frameRockets = editorAnim9.Frames[0];


                if (type == 0) {
                    Animation.ProcessAnimation(framePropel.Entry.FrameSpeed, framePropel.Entry.Frames.Count, framePropel.Frame.Duration);
                    d.DrawBitmap(frameRockets.Texture,
                        x + frameRockets.Frame.CenterX - 4,
                        y + frameRockets.Frame.CenterY - 27,
                        frameRockets.Frame.Width, frameRockets.Frame.Height, false, Transparency);
                    d.DrawBitmap(frame.Texture,
                        x + frame.Frame.CenterX,
                        y + frame.Frame.CenterY,
                        frame.Frame.Width, frame.Frame.Height, false, Transparency);
                    d.DrawBitmap(framePropel.Texture,
                        x + framePropel.Frame.CenterX,
                        y + framePropel.Frame.CenterY,
                        framePropel.Frame.Width, framePropel.Frame.Height, false, Transparency);
                    d.DrawBitmap(frameRockets.Texture,
                        x + frameRockets.Frame.CenterX - 18,
                        y + frameRockets.Frame.CenterY + 20,
                        frameRockets.Frame.Width, frameRockets.Frame.Height, false, Transparency);
                    d.DrawBitmap(frameRockets.Texture,
                        x + frameRockets.Frame.CenterX - 18,
                        y + frameRockets.Frame.CenterY - 27,
                        frameRockets.Frame.Width, frameRockets.Frame.Height, false, Transparency);
                }
                    else if (type == 1) {
                    Animation.ProcessAnimation3(frameBomb.Entry.FrameSpeed, frameBomb.Entry.Frames.Count, frameBomb.Frame.Duration);
                    d.DrawBitmap(frameBomb.Texture,
                            x + frameBomb.Frame.CenterX,
                            y + frameBomb.Frame.CenterY,
                            frameBomb.Frame.Width, frameBomb.Frame.Height, false, Transparency);
                    }
                    else if (type == 2)
                    {
                    Animation.ProcessAnimation2(frameLaundry.Entry.FrameSpeed, frameLaundry.Entry.Frames.Count, frameLaundry.Frame.Duration);
                    d.DrawBitmap(frameLaundry.Texture,
                        x + frameLaundry.Frame.CenterX,
                        y + frameLaundry.Frame.CenterY + 16,
                        frameLaundry.Frame.Width, frameLaundry.Frame.Height, false, Transparency);
                    d.DrawBitmap(frameLaundry.Texture,
                        x + frameLaundry.Frame.CenterX,
                        y + frameLaundry.Frame.CenterY + frameLaundry.Frame.CenterY + 16,
                        frameLaundry.Frame.Width, frameLaundry.Frame.Height, false, Transparency);
                    d.DrawBitmap(frameLaundry.Texture,
                        x + frameLaundry.Frame.CenterX,
                        y + frameLaundry.Frame.CenterY + -(frameLaundry.Frame.CenterY * 2) + 16,
                        frameLaundry.Frame.Width, frameLaundry.Frame.Height, false, Transparency);
                    d.DrawBitmap(frameLaundry.Texture,
                        x + frameLaundry.Frame.CenterX,
                        y - frameLaundry.Frame.CenterY + -(frameLaundry.Frame.CenterY * 2) + 16,
                        frameLaundry.Frame.Width, frameLaundry.Frame.Height, false, Transparency);

                    d.DrawBitmap(frameLaundryCenterBottom.Texture,
                        x + frameLaundryCenterBottom.Frame.CenterX,
                        y + frameLaundryCenterBottom.Frame.CenterY + frameLaundryCenterBottom.Frame.CenterY * 3 + 16,
                        frameLaundryCenterBottom.Frame.Width, frameLaundryCenterBottom.Frame.Height, false, Transparency);

                    d.DrawBitmap(frameLaundryCenterTop.Texture,
                        x + frameLaundryCenterTop.Frame.CenterX,
                        y + frameLaundryCenterTop.Frame.CenterY,
                        frameLaundryCenterTop.Frame.Width, frameLaundryCenterTop.Frame.Height, false, Transparency);

                    d.DrawBitmap(frameLaundryCenter.Texture,
                        x + frameLaundryCenter.Frame.CenterX,
                        y + frameLaundryCenter.Frame.CenterY + frameLaundryCenter.Frame.CenterY*2 + 16,
                        frameLaundryCenter.Frame.Width, frameLaundryCenter.Frame.Height, false, Transparency);
                    }
                    else if (type == 4 || type == 5) {
                    d.DrawBitmap(frameBlock.Texture,
                            x + frameBlock.Frame.CenterX,
                            y + frameBlock.Frame.CenterY,
                            frameBlock.Frame.Width, frameBlock.Frame.Height, false, Transparency);
                    }
                    else
                    { 
                        if (editorAnimIcon != null && editorAnimIcon.Frames.Count != 0)
                        {
                            var frameIcon = editorAnimIcon.Frames[0];
                            d.DrawBitmap(frameIcon.Texture, x + frameIcon.Frame.CenterX, y + frameIcon.Frame.CenterY,
                                frameIcon.Frame.Width, frameIcon.Frame.Height, false, Transparency);
                        }
                    }



            }
        }

        public override string GetObjectName()
        {
            return "LaundroMobile";
        }
    }
}
