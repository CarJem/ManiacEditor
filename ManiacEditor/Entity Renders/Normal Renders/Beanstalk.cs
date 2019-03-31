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
    public class Beanstalk : EntityRenderer
    {

        public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            int direction = (int)entity.attributesMap["direction"].ValueUInt8;
            int type = (int)entity.attributesMap["type"].ValueUInt8;
            int animID = 0;
            int frameID = 0;
            bool plantType = false;
            switch (type)
            {
                case 0:
                    animID = 4;
                    frameID = 9;
                    break;
                case 1:
                    animID = 0;
                    frameID = 0;
                    break;
                case 2:
                    animID = 2;
                    frameID = 2;
                    break;
                case 3:
                    animID = 3;
                    frameID = -1;
                    plantType = true;
                    break;
            }
            bool fliph = false;
            bool flipv = false;
            if (direction == 1)
            {
                fliph = true;
            }
            var editorAnimNode = Editor.Instance.EntityDrawing.LoadAnimation2("Beanstalk", d, 0, 0, fliph, flipv, false);
            var editorAnimType = Editor.Instance.EntityDrawing.LoadAnimation2("Beanstalk", d, animID, frameID, fliph, flipv, false);
            if (editorAnimNode != null && editorAnimNode.Frames.Count != 0 && editorAnimType != null && editorAnimType.Frames.Count != 0)
            {
                var frame = editorAnimNode.Frames[0];
                var frameHead = editorAnimType.Frames[0];

                if (plantType == true)
                {
                    frameHead = editorAnimType.Frames[Animation.index];
                    Animation.ProcessAnimation(frameHead.Entry.SpeedMultiplyer, frameHead.Entry.Frames.Count, frameHead.Frame.Delay);
                }


                d.DrawBitmap(frame.Texture,
                    x + frame.Frame.PivotX - (fliph ? (frame.Frame.Width - editorAnimNode.Frames[0].Frame.Width) : 0),
                    y + frame.Frame.PivotY + (flipv ? (frame.Frame.Height - editorAnimNode.Frames[0].Frame.Height) : 0),
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
                d.DrawBitmap(frameHead.Texture,
                    x + (plantType ? (fliph ? frameHead.Frame.PivotX*2 : frameHead.Frame.PivotX) : (fliph ? -frameHead.Frame.Width : frameHead.Frame.PivotX)),
                    y + frameHead.Frame.PivotY,
                    frameHead.Frame.Width, frameHead.Frame.Height, false, Transparency);


            }
        }

        public override string GetObjectName()
        {
            return "Beanstalk";
        }
    }
}
