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
    public class HandLauncher : EntityRenderer
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
            var editorAnim = e.EditorInstance.EditorEntity_ini.LoadAnimation2("HandLauncher", d, 0, -1, fliph, flipv, false);
            var editorAnimHand = e.EditorInstance.EditorEntity_ini.LoadAnimation2("HandLauncher", d, 1, 0, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnimHand != null && editorAnimHand.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[Animation.index];
                var frameHand = editorAnimHand.Frames[0];

                Animation.ProcessAnimation(frame.Entry.SpeedMultiplyer, frame.Entry.Frames.Count, frame.Frame.Delay);

                d.DrawBitmap(frame.Texture,
                    x + frame.Frame.PivotX - (fliph ? (frame.Frame.Width - editorAnim.Frames[0].Frame.Width) : 0),
                    y + frame.Frame.PivotY + (flipv ? (frame.Frame.Height - editorAnim.Frames[0].Frame.Height) : 0),
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);

                d.DrawBitmap(frameHand.Texture,
                    x + frameHand.Frame.PivotX - (fliph ? (frameHand.Frame.Width - editorAnimHand.Frames[0].Frame.Width) : 0),
                    y + frameHand.Frame.PivotY + (flipv ? (frameHand.Frame.Height - editorAnimHand.Frames[0].Frame.Height) : 0),
                    frameHand.Frame.Width, frameHand.Frame.Height, false, Transparency);
            }
        }

        public override string GetObjectName()
        {
            return "HandLauncher";
        }
    }
}
