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
    public class Jellygnite : EntityRenderer
    {

        public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            int direction = (int)entity.attributesMap["direction"].ValueUInt8;
            bool fliph = false;
            bool flipv = false;


            if (direction == 1)
            {
                fliph = true;
            }
            if (direction == 2)
            {
                flipv = true;
            }
            if (direction == 3)
            {
                flipv = true;
                fliph = true;
            }

            var editorAnim = e.EditorInstance.EditorEntity_ini.LoadAnimation2("Jellygnite", d, 0, 0, fliph, flipv, false);
            var editorAnimFront = e.EditorInstance.EditorEntity_ini.LoadAnimation2("Jellygnite", d, 3, 0, fliph, flipv, false);
            var editorAnimBack = e.EditorInstance.EditorEntity_ini.LoadAnimation2("Jellygnite", d, 5, 0, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnimFront != null && editorAnimFront.Frames.Count != 0 && editorAnimBack != null && editorAnimBack.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[0];
                var frameFront = editorAnimFront.Frames[0];
                var frameBack = editorAnimBack.Frames[0];

                d.DrawBitmap(frame.Texture,
                    x + frame.Frame.CenterX,
                    y + frame.Frame.CenterY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);

                for (int i = 0; i < 4; i++)
                {
                    d.DrawBitmap(frameFront.Texture,
                        x + frameFront.Frame.CenterX + 12,
                        y + frameFront.Frame.CenterY + 6 + 6 * i,
                        frameFront.Frame.Width, frameFront.Frame.Height, false, Transparency);
                    d.DrawBitmap(frameFront.Texture,
                        x + frameFront.Frame.CenterX - 12,
                        y + frameFront.Frame.CenterY + 6 + 6 * i,
                        frameFront.Frame.Width, frameFront.Frame.Height, false, Transparency);
                    d.DrawBitmap(frameBack.Texture,
                        x + frameBack.Frame.CenterX,
                        y + frameBack.Frame.CenterY + 6 + 6 * i,
                        frameBack.Frame.Width, frameBack.Frame.Height, false, Transparency);
                }
             }
        }

        public override string GetObjectName()
        {
            return "Jellygnite";
        }
    }
}
