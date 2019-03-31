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
    public class FBZMissile : EntityRenderer
    {

        public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            bool fliph = false;
            bool flipv = false;
            int type = (int)entity.attributesMap["type"].ValueVar;
            int frameID = 0;
            if (type == 2)
            {
                frameID = 3;
            }
            if (type == 1)
            {
                frameID = 1;
            }
            var editorAnim = Editor.Instance.EntityDrawing.LoadAnimation2("Missile", d, frameID, -1, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[Animation.index];
                Animation.ProcessAnimation(frame.Entry.SpeedMultiplyer, frame.Entry.Frames.Count, frame.Frame.Delay);

                d.DrawBitmap(frame.Texture,
                    x + frame.Frame.PivotX + (fliph ? (frame.Frame.Width - editorAnim.Frames[0].Frame.Width * 2) : 0),
                    y + frame.Frame.PivotY + (flipv ? (frame.Frame.Height - editorAnim.Frames[0].Frame.Height) : 0),
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }
        }

        public override string GetObjectName()
        {
            return "FBZMissile";
        }
    }
}
