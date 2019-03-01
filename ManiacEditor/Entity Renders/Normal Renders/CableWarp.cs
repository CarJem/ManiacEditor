using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ManiacEditor;
using Microsoft.Xna.Framework;
using RSDKv5;
using SharpDX.Direct3D9;

namespace ManiacEditor.Entity_Renders
{
    public class CableWarp : EntityRenderer
    {

        public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            int type = (int)entity.attributesMap["type"].ValueVar;
            bool fliph = false;
            bool flipv = false;
            int animID;
            int frameID;
            if (type == 2)
            {
                animID = 2;
                frameID = -1;
            }
            else
            {
                animID = 0;
                frameID = 0;
            }
            var editorAnim = e.EditorInstance.EditorEntity_ini.LoadAnimation2("CableWarp", d, animID, frameID, fliph, flipv, false);
            var editorAnim2 = e.EditorInstance.EditorEntity_ini.LoadAnimation2("PlaneSwitch", d, 0, 5, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && animID >= 0 && editorAnim2 != null && editorAnim2.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[Animation.index];
                var frame2 = editorAnim2.Frames[0];

                Animation.ProcessAnimation(frame.Entry.SpeedMultiplyer, frame.Entry.Frames.Count, frame.Frame.Delay);

                if (type != 2)
                {
                    d.DrawBitmap(frame.Texture,
                        x + frame.Frame.PivotX - (fliph ? (frame.Frame.Width - editorAnim.Frames[0].Frame.Width) : 0),
                        y + frame.Frame.PivotY + (flipv ? (frame.Frame.Height - editorAnim.Frames[0].Frame.Height) : 0),
                        frame.Frame.Width, frame.Frame.Height, false, Transparency);
                }
                else
                {
                    d.DrawBitmap(frame2.Texture,
    x + frame2.Frame.PivotX - (fliph ? (frame2.Frame.Width - editorAnim2.Frames[0].Frame.Width) : 0),
    y + frame2.Frame.PivotY + (flipv ? (frame2.Frame.Height - editorAnim2.Frames[0].Frame.Height) : 0),
    frame2.Frame.Width, frame2.Frame.Height, false, Transparency);
                }

            }
        }

        public override string GetObjectName()
        {
            return "CableWarp";
        }
    }
}
