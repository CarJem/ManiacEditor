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
    public class UIText : EntityRenderer
    {

        public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            
            string text = entity.attributesMap["text"].ValueString;
            bool selectable = entity.attributesMap["selectable"].ValueBool;
            bool highlighted = entity.attributesMap["highlighted"].ValueBool;
            int spacingAmount = 0;
            foreach(char symb in text)
            {
                int frameID = GetFrameID(symb, e.EditorInstance.LevelSelectChar);
                int listID = (highlighted ? 1 : 0);
                var editorAnim = e.EditorInstance.EditorEntity_ini.LoadAnimation("Text", d, listID, frameID, false, false, false);
                if (editorAnim != null && editorAnim.Frames.Count != 0)
                {
                    var frame = editorAnim.Frames[Animation.index];
                    //Animation.ProcessAnimation(frame.Entry.FrameSpeed, frame.Entry.Frames.Count, frame.Frame.Duration);
                    d.DrawBitmap(frame.Texture, x + frame.Frame.CenterX + spacingAmount, y + frame.Frame.CenterY,
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
            return "UIText";
        }
    }
}
