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
    public class TwistedTubes : EntityRenderer
    {

        public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            bool fliph = false;
            bool flipv = false;
            int height = (int)entity.attributesMap["height"].ValueUInt8;
            var editorAnim = e.EditorInstance.EditorEntity_ini.LoadAnimation2("TwistedTubes", d, 0, 1, fliph, flipv, false);
            var editorAnim2 = e.EditorInstance.EditorEntity_ini.LoadAnimation2("TwistedTubes", d, 0, 0, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnim2 != null && editorAnim2.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[Animation.index];
                var frame2 = editorAnim2.Frames[Animation.index];


                //ProcessAnimation(frame.Entry.SpeedMultiplyer, frame.Entry.Frames.Count, frame.Frame.Delay);

                d.DrawBitmap(frame2.Texture,
                    x + frame2.Frame.PivotX - (fliph ? (frame2.Frame.Width - editorAnim2.Frames[0].Frame.Width) : 0),
                    y + frame2.Frame.PivotY + (flipv ? (frame2.Frame.Height - editorAnim2.Frames[0].Frame.Height) : 0),
                    frame2.Frame.Width, frame.Frame.Height, false, Transparency);
                for (int i = 0; i < height * 2; i++)
                {
                    d.DrawBitmap(frame.Texture,
                        x + frame.Frame.PivotX - (fliph ? (frame.Frame.Width - editorAnim.Frames[1].Frame.Width) : 0),
                        y + (i * 32) + frame.Frame.PivotY + 64 + (flipv ? (frame.Frame.Height - editorAnim.Frames[1].Frame.Height) : 0),
                        frame.Frame.Width, frame.Frame.Height, false, Transparency);
                }

            }
        }

        public override string GetObjectName()
        {
            return "TwistedTubes";
        }
    }
}
