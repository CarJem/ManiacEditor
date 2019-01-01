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
    public class UITAZoneModule : EntityRenderer
    {

        public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            var editorAnimFrame = EditorEntity_ini.LoadAnimation("EditorUIRender", d, 0, 0, false, false, false);
            var editorAnimBackground = EditorEntity_ini.LoadAnimation("SaveSelect", d, 10, -1, false, false, false);

            if (editorAnimFrame != null && editorAnimFrame.Frames.Count != 0)
            {
                var frame = editorAnimFrame.Frames[0];
                d.DrawBitmap(frame.Texture, x + frame.Frame.CenterX, y + frame.Frame.CenterY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }

            if (editorAnimBackground != null && editorAnimBackground.Frames.Count != 0)
            {
                var frame = editorAnimBackground.Frames[Animation.index];
                Animation.ProcessAnimation(frame.Entry.FrameSpeed, frame.Entry.Frames.Count, frame.Frame.Duration);
                d.DrawBitmap(frame.Texture, x + frame.Frame.CenterX - 107, y + frame.Frame.CenterY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }

            string text1 = entity.attributesMap["text1"].ValueString;
            string text2 = entity.attributesMap["text2"].ValueString;

            int listID = 3;
            int text_X = x - 46;
            int text_YAdjust = (text2 != "" ? 14 : 0);
            int text_Y = y - 8;

            int spacingAmount = 0;
            foreach (char symb in text1)
            {
                int frameID = GetFrameID(symb);
                var editorAnim2 = EditorEntity_ini.LoadAnimation("UIElements", d, listID, frameID, false, false, false);
                if (editorAnim2 != null && editorAnim2.Frames.Count != 0)
                {
                    var frame = editorAnim2.Frames[0];
                    d.DrawBitmap(frame.Texture, text_X + spacingAmount, text_Y + frame.Frame.CenterY - text_YAdjust,
                        frame.Frame.Width, frame.Frame.Height, false, Transparency);
                    spacingAmount = spacingAmount + frame.Frame.Width;
                }
            }
            spacingAmount = 0;
            foreach (char symb in text2)
            {
                int frameID = GetFrameID(symb);
                var editorAnim2 = EditorEntity_ini.LoadAnimation("UIElements", d, listID, frameID, false, false, false);
                if (editorAnim2 != null && editorAnim2.Frames.Count != 0)
                {
                    var frame = editorAnim2.Frames[0];
                    d.DrawBitmap(frame.Texture, text_X + 32 + spacingAmount, text_Y + frame.Frame.CenterY + 28 - text_YAdjust,
                        frame.Frame.Width, frame.Frame.Height, false, Transparency);
                    spacingAmount = spacingAmount + frame.Frame.Width;
                }
            }


        }

        public int GetFrameID(char letter)
        {
            char[] symArray = Editor.Instance.MenuChar;
            int position = 0;
            foreach (char sym in symArray)
            {
                if (sym == letter) return position;
                position++;
            }
            return position;
        }

        public override string GetObjectName()
        {
            return "UITAZoneModule";
        }
    }
}
