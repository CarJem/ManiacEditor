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
    public class Button : EntityRenderer
    {

        public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            int type = (int)entity.attributesMap["type"].ValueVar;
            bool fliph = false;
            bool flipv = false;
            int animID = 0;
            if (type == 0 || type == 1)
            {
                animID = 0;
            }
            if (type == 2 || type == 3)
            {
                animID = 1;
            }
            if (type == 3)
            {
                fliph = true;
            }
            if (type == 1)
            {
                flipv = true;
            }
            var editorAnim = e.EditorInstance.EditorEntity_ini.LoadAnimation2("Button", d, animID, -1, fliph, flipv, false);
            var editorAnim2 = e.EditorInstance.EditorEntity_ini.LoadAnimation2("Button", d, animID, 1, fliph, flipv, false);
            var editorAnim3 = e.EditorInstance.EditorEntity_ini.LoadAnimation2("Button", d, animID, 2, fliph, flipv, false);
            if (editorAnim2 != null && editorAnim2.Frames.Count != 0)
            {
                var frame2 = editorAnim2.Frames[Animation.index];

                    d.DrawBitmap(frame2.Texture,
                        x + frame2.Frame.CenterX - (fliph ? (frame2.Frame.Width - editorAnim2.Frames[0].Frame.Width - 7) : 0),
                        y + frame2.Frame.CenterY + (flipv ? (frame2.Frame.Height - editorAnim2.Frames[0].Frame.Height - 7) : 0),
                        frame2.Frame.Width, frame2.Frame.Height, false, Transparency);
            }
            if (editorAnim3 != null && editorAnim3.Frames.Count != 0)
            {
                var frame3 = editorAnim3.Frames[Animation.index];

                d.DrawBitmap(frame3.Texture,
                    x + frame3.Frame.CenterX - (fliph ? (frame3.Frame.Width - editorAnim3.Frames[0].Frame.Width - 7) : 0),
                    y + frame3.Frame.CenterY + (flipv ? (frame3.Frame.Height - editorAnim3.Frames[0].Frame.Height - 7) : 0),
                    frame3.Frame.Width, frame3.Frame.Height, false, Transparency);
            }
            if (editorAnim != null && editorAnim.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[Animation.index];
                d.DrawBitmap(frame.Texture,
                    x + frame.Frame.CenterX - (fliph ? (frame.Frame.Width - editorAnim.Frames[0].Frame.Width + 9) : 0),
                    y + frame.Frame.CenterY + (flipv ? (frame.Frame.Height - editorAnim.Frames[0].Frame.Height + 9) : 0),
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);



            }
        }

        public override string GetObjectName()
        {
            return "Button";
        }
    }
}
