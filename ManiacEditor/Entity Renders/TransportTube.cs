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
    public class TransportTube : EntityRenderer
    {

        public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            int type = (int)entity.attributesMap["type"].ValueUInt8;
            int dirMask = (int)entity.attributesMap["dirMask"].ValueUInt8;
            var editorAnim = e.EditorInstance.EditorEntity_ini.LoadAnimation2("TransportTube", d, 0, 0, false, false, false);
            var upAnim = e.EditorInstance.EditorEntity_ini.LoadAnimation2("TransportTubes", d, 0, 0, false, false, false);
            var downAnim = e.EditorInstance.EditorEntity_ini.LoadAnimation2("TransportTubes", d, 0, 1, false, false, false);
            var rightAnim = e.EditorInstance.EditorEntity_ini.LoadAnimation2("TransportTubes", d, 0, 2, false, false, false);
            var leftAnim = e.EditorInstance.EditorEntity_ini.LoadAnimation2("TransportTubes", d, 0, 3, false, false, false);
            var upleftAnim = e.EditorInstance.EditorEntity_ini.LoadAnimation2("TransportTubes", d, 0, 4, false, false, false);
            var downleftAnim = e.EditorInstance.EditorEntity_ini.LoadAnimation2("TransportTubes", d, 0, 5, false, false, false);
            var uprightAnim = e.EditorInstance.EditorEntity_ini.LoadAnimation2("TransportTubes", d, 0, 6, false, false, false);
            var downrightAnim = e.EditorInstance.EditorEntity_ini.LoadAnimation2("TransportTubes", d, 0, 7, false, false, false);
            bool showUp = false, showDown = false, showLeft = false, showRight = false, showUpLeft = false, showDownLeft = false, showUpRight = false, showDownRight = false;
            if (type == 0)
            {
                switch (dirMask)
                {
                    case 0:
                        break;
                    case 1:
                        break;
                    case 2:
                        break;
                    case 3:
                        break;
                    case 4:
                        break;
                    case 5:
                        showLeft = true;
                        showUp = true;
                        break;
                    case 6:
                        showLeft = true;
                        showDown = true;
                        break;
                    case 7:
                        break;
                    case 8:
                        break;
                    case 9:
                        showUp = true;
                        showRight = true;
                        break;
                    case 10:
                        showDown = true;
                        showRight = true;
                        break;
                    case 18:
                        showDown = true;
                        showUpRight = true;
                        break;
                    case 20:
                        showLeft = true;
                        showUpRight = true;
                        break;
                    case 40:
                        showUpLeft = true;
                        showRight = true;
                        break;
                    case 65:
                        showUp = true;
                        showDownRight = true;
                        break;
                    case 68:
                        showLeft = true;
                        showDownRight = true;
                        break;
                    case 129:
                        showUp = true;
                        showDownLeft = true;
                        break;
                    case 136:
                        showRight = true;
                        showDownLeft = true;
                        break;
                }
            }


            if (editorAnim != null && editorAnim.Frames.Count != 0 && upAnim != null && upAnim.Frames.Count != 0 && downAnim != null && downAnim.Frames.Count != 0 && rightAnim != null && rightAnim.Frames.Count != 0 && leftAnim != null && leftAnim.Frames.Count != 0 && uprightAnim != null && uprightAnim.Frames.Count != 0 && downrightAnim != null && downrightAnim.Frames.Count != 0 && upleftAnim != null && upleftAnim.Frames.Count != 0 && downleftAnim != null && downleftAnim.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[Animation.index];
                var frame2 = upAnim.Frames[Animation.index];
                var frame3 = downAnim.Frames[Animation.index];
                var frame4 = rightAnim.Frames[Animation.index];
                var frame5 = leftAnim.Frames[Animation.index];
                var frame6 = uprightAnim.Frames[Animation.index];
                var frame7 = downrightAnim.Frames[Animation.index];
                var frame8 = upleftAnim.Frames[Animation.index];
                var frame9 = downleftAnim.Frames[Animation.index];


                d.DrawBitmap(frame.Texture,
                    x + frame.Frame.CenterX,
                    y + frame.Frame.CenterY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
                if (showUp == true)
                {
                    d.DrawBitmap(frame2.Texture,
                        x + frame2.Frame.CenterX,
                        y + frame2.Frame.CenterY,
                        frame2.Frame.Width, frame2.Frame.Height, false, Transparency);
                }
                if (showDown == true)
                {
                    d.DrawBitmap(frame3.Texture,
                        x + frame3.Frame.CenterX,
                        y + frame3.Frame.CenterY,
                        frame3.Frame.Width, frame3.Frame.Height, false, Transparency);
                }
                if (showRight == true)
                {
                    d.DrawBitmap(frame4.Texture,
                        x + frame4.Frame.CenterX,
                        y + frame4.Frame.CenterY,
                        frame4.Frame.Width, frame4.Frame.Height, false, Transparency);
                }
                if (showLeft == true)
                {
                    d.DrawBitmap(frame5.Texture,
                        x + frame5.Frame.CenterX,
                        y + frame5.Frame.CenterY,
                        frame5.Frame.Width, frame5.Frame.Height, false, Transparency);
                }
                if (showUpRight == true)
                {
                    d.DrawBitmap(frame6.Texture,
                        x + frame6.Frame.CenterX,
                        y + frame6.Frame.CenterY,
                        frame6.Frame.Width, frame6.Frame.Height, false, Transparency);
                }
                if (showDownRight == true)
                {
                    d.DrawBitmap(frame7.Texture,
                        x + frame7.Frame.CenterX,
                        y + frame7.Frame.CenterY,
                        frame7.Frame.Width, frame7.Frame.Height, false, Transparency);
                }
                if (showUpLeft == true)
                {
                    d.DrawBitmap(frame8.Texture,
                        x + frame8.Frame.CenterX,
                        y + frame8.Frame.CenterY,
                        frame8.Frame.Width, frame8.Frame.Height, false, Transparency);
                }
                if (showDownLeft == true)
                {
                    d.DrawBitmap(frame9.Texture,
                        x + frame9.Frame.CenterX,
                        y + frame9.Frame.CenterY,
                        frame9.Frame.Width, frame9.Frame.Height, false, Transparency);
                }
            }
        }

        public override string GetObjectName()
        {
            return "TransportTube";
        }
    }
}
