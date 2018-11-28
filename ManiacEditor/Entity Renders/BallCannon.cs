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
    public class BallCannon : EntityRenderer
    {

        public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            int x2 = x;
            int y2 = y;
            bool flipv = false;
            bool fliph = false;
            int angle = (int)entity.attributesMap["angle"].ValueVar;
            int type = (int)entity.attributesMap["type"].ValueUInt8;
            int rotation = 0;
            int rotation2 = 0;
            int CorkState = 0;
            if (type == 0)
            {
                switch (angle)
                {
                    case 0:
                        rotation = 90;
                        rotation2 = 180;
                        y += 16;
                        y2 += 16;
                        break;
                    case 1:
                        rotation = 180;
                        rotation2 = 270;
                        y += 16;
                        y2 += 16;
                        x2 -= 16;
                        break;
                    case 2:
                        rotation = 270;
                        y += 16;
                        x -= 16;
                        break;
                    case 3:
                        rotation2 = 90;
                        y2 += 16;
                        break;
                    case 4:
                        rotation2 = 90;
                        y2 += 16;
                        rotation = 180;
                        y += 16;
                        break;
                    case 5:
                        rotation = 270;
                        rotation2 = 180;
                        y2 += 16;
                        y += 16;
                        x -= 16;
                        break;
                    case 6:
                        rotation2 = 270;
                        y2 += 16;
                        x2 -= 16;
                        break;
                    case 7:
                        rotation2 = 0;
                        rotation = 90;
                        y += 16;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch (type)
                {
                    case 0:
                        break;
                    case 1:
                        CorkState = 3;
                        break;
                    case 2:
                        CorkState = 4;
                        break;
                }
            }

            var editorAnim = EditorEntity_ini.LoadAnimation2("BallCannon", d, 0, -1, fliph, flipv, false, rotation);
            var editorAnimHolo = EditorEntity_ini.LoadAnimation2("BallCannon", d, 0, -1, fliph, flipv, false, rotation2);
            var editorAnimCork = EditorEntity_ini.LoadAnimation2("BallCannon", d, CorkState, 0, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnimHolo != null && editorAnimHolo.Frames.Count != 0 && editorAnimCork != null && editorAnimCork.Frames.Count != 0)
            {
                if (type == 1)
                {
                    var frame = editorAnimCork.Frames[0];

                    d.DrawBitmap(frame.Texture,
                        x + frame.Frame.CenterX,
                        y + frame.Frame.CenterY,
                        frame.Frame.Height, frame.Frame.Height, false, Transparency);
                }
                else if (type == 2)
                {
                    var frame = editorAnimCork.Frames[0];

                    d.DrawBitmap(frame.Texture,
                        x + frame.Frame.CenterX,
                        y + frame.Frame.CenterY,
                        frame.Frame.Height, frame.Frame.Height, false, Transparency);
                }
                else
                {
                    var frame = editorAnim.Frames[Animation.index];
                    var frame3 = editorAnimHolo.Frames[Animation.index];

                    if (selected)
                    {
                        d.DrawBitmap(frame3.Texture,
                            x2 + frame3.Frame.CenterX,
                            y2 + frame3.Frame.CenterY,
                            frame3.Frame.Height, frame3.Frame.Height, false, 125);
                    }

                    d.DrawBitmap(frame.Texture,
                        x + frame.Frame.CenterX,
                        y + frame.Frame.CenterY,
                        frame.Frame.Height, frame.Frame.Height, false, Transparency);
                }


            }
        }

        public override string GetObjectName()
        {
            return "BallCannon";
        }
    }
}
