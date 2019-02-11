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
    public class DoorTrigger : EntityRenderer
    {

        public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            int orientation = entity.attributesMap["orientation"].ValueUInt8;
            bool fliph = false;
            bool flipv = false;
            int AnimID_2 = 1;
            int frameID = 0;
            int frameID_2 = 0;
            int offsetX = 0;
            int offsetY = 0;
            switch (orientation)
            {
                case 0:
                    frameID = 0;
                    frameID_2 = 0;
                    AnimID_2 = 1;
                    offsetX = 0;
                    offsetY = 0;
                    break;
                case 1:
                    frameID = 0;
                    frameID_2 = 0;
                    AnimID_2 = 1;
                    fliph = true;
                    offsetX = -23;
                    offsetY = 0;
                    break;
                case 2:
                    frameID = 1;
                    AnimID_2 = 2;
                    offsetX = 0;
                    offsetY = 0;
                    break;
                case 3:
                    frameID = 1;
                    AnimID_2 = 2;
                    flipv = true;
                    offsetX = 0;
                    offsetY = 0;
                    break;
            }
            var editorAnim = e.EditorInstance.EditorEntity_ini.LoadAnimation2("DoorTrigger", d, 0, frameID, fliph, flipv, false);
            var editorAnim2 = e.EditorInstance.EditorEntity_ini.LoadAnimation2("DoorTrigger", d, AnimID_2, frameID_2, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && frameID >= 0 && editorAnim2 != null && editorAnim2.Frames.Count != 0 && frameID >= 0)
            {
                var frame = editorAnim.Frames[Animation.index];
                var frame2 = editorAnim2.Frames[Animation.index];

                Animation.ProcessAnimation(frame2.Entry.SpeedMultiplyer, frame2.Entry.Frames.Count, frame2.Frame.Delay);

                d.DrawBitmap(frame.Texture,
                    x + (fliph ? -frame.Frame.PivotX : frame.Frame.PivotX) - (fliph ? (frame.Frame.Width - editorAnim.Frames[0].Frame.Width) : 0) + offsetX,
                    y + (flipv ? -frame.Frame.PivotY : frame.Frame.PivotY) + (flipv ? (frame.Frame.Height - frame.Frame.Height - editorAnim.Frames[0].Frame.Height) : 0) + offsetY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);

                d.DrawBitmap(frame2.Texture,
                    x + (fliph ? -frame2.Frame.PivotX : frame2.Frame.PivotX) - (fliph ? (frame2.Frame.Width - editorAnim2.Frames[0].Frame.Width) : 0) + offsetX,
                    y + (flipv ? -frame2.Frame.PivotY : frame2.Frame.PivotY) + (flipv ? (frame2.Frame.Height - frame2.Frame.Height - editorAnim2.Frames[0].Frame.Height) : 0) + offsetY,
                    frame2.Frame.Width, frame2.Frame.Height, false, Transparency);
            }
        }

        public override string GetObjectName()
        {
            return "DoorTrigger";
        }
    }
}
