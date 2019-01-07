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
    public class FlameSpring : EntityRenderer
    {

        public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            int type = (int)entity.attributesMap["type"].ValueVar;
            bool fliph = false;
            bool flipv = false;
            int valveType;
            int animID;
            switch (type)
            {
                case 0:
                    animID = 0;
                    valveType = 0;
                    break;
                case 1:
                    animID = 0;
                    valveType = 1;
                    break;
                case 2:
                    animID = 0;
                    valveType = 2;
                    break;
                case 3:
                    animID = 2;
                    valveType = 0;
                    break;
                case 4:
                    animID = 2;
                    valveType = 1;
                    break;
                case 5:
                    animID = 2;
                    valveType = 2;
                    break;
                default:
                    animID = 0;
                    valveType = 0;
                    break;
            }
            var editorAnim = e.EditorInstance.EditorEntity_ini.LoadAnimation2("FlameSpring", d, 0, animID, fliph, flipv, false);
            var nozzelA = e.EditorInstance.EditorEntity_ini.LoadAnimation2("FlameSpring", d, 1, 0, fliph, flipv, false);
            var nozzelB = e.EditorInstance.EditorEntity_ini.LoadAnimation2("FlameSpring", d, 1, 1, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && nozzelA != null && nozzelA.Frames.Count != 0 && nozzelB != null && nozzelB.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[Animation.index];
                var headA = nozzelA.Frames[0];
                var headB = nozzelB.Frames[0];

                Animation.ProcessAnimation(frame.Entry.FrameSpeed, frame.Entry.Frames.Count, frame.Frame.Duration);

                d.DrawBitmap(frame.Texture,
                    x + frame.Frame.CenterX,
                    y + frame.Frame.CenterY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);

                if (valveType == 2 || valveType == 0)
                {
                    d.DrawBitmap(headA.Texture,
                        x - 21 - headA.Frame.CenterX,
                        y - 12 - headA.Frame.CenterY,
                        headA.Frame.Width, headA.Frame.Height, false, Transparency);
                }
                if (valveType == 1 || valveType == 0)
                {
                    d.DrawBitmap(headB.Texture,
                        x + 12 + headB.Frame.CenterX,
                        y - 12 - headB.Frame.CenterY,
                        headB.Frame.Width, headB.Frame.Height, false, Transparency);
                }
            }
        }

        public override string GetObjectName()
        {
            return "FlameSpring";
        }
    }
}
