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
            var editorAnimFrame = e.EditorInstance.EntityDrawing.LoadAnimation("EditorUIRender", d, 0, 0, false, false, false);
            var editorAnimBackground = e.EditorInstance.EntityDrawing.LoadAnimation("SaveSelect", d, 10, -1, false, false, false);

            if (editorAnimFrame != null && editorAnimFrame.Frames.Count != 0)
            {
                var frame = editorAnimFrame.Frames[0];
                d.DrawBitmap(frame.Texture, x + frame.Frame.PivotX, y + frame.Frame.PivotY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }

            if (editorAnimBackground != null && editorAnimBackground.Frames.Count != 0)
            {
                var frame = editorAnimBackground.Frames[Animation.index];
                Animation.ProcessAnimation(frame.Entry.SpeedMultiplyer, frame.Entry.Frames.Count, frame.Frame.Delay);
                d.DrawBitmap(frame.Texture, x + frame.Frame.PivotX - 107, y + frame.Frame.PivotY,
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
                int frameID = GetFrameID(symb, e.EditorInstance.UIModes.MenuChar);
                var editorAnim2 = e.EditorInstance.EntityDrawing.LoadAnimation("UIElements", d, listID, frameID, false, false, false);
                if (editorAnim2 != null && editorAnim2.Frames.Count != 0)
                {
                    var frame = editorAnim2.Frames[0];
                    d.DrawBitmap(frame.Texture, text_X + spacingAmount, text_Y + frame.Frame.PivotY - text_YAdjust,
                        frame.Frame.Width, frame.Frame.Height, false, Transparency);
                    spacingAmount = spacingAmount + frame.Frame.Width;
                }
            }
            spacingAmount = 0;
            foreach (char symb in text2)
            {
                int frameID = GetFrameID(symb, e.EditorInstance.UIModes.MenuChar);
                var editorAnim2 = e.EditorInstance.EntityDrawing.LoadAnimation("UIElements", d, listID, frameID, false, false, false);
                if (editorAnim2 != null && editorAnim2.Frames.Count != 0)
                {
                    var frame = editorAnim2.Frames[0];
                    d.DrawBitmap(frame.Texture, text_X + 32 + spacingAmount, text_Y + frame.Frame.PivotY + 28 - text_YAdjust,
                        frame.Frame.Width, frame.Frame.Height, false, Transparency);
                    spacingAmount = spacingAmount + frame.Frame.Width;
                }
            }


        }

        public int GetFrameID(char letter, char[] arry)
        {
            char[] symArray = arry;
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
