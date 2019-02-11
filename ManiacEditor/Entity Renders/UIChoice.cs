using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;
using ManiacEditor;
using Microsoft.Xna.Framework;
using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class UIChoice : EntityRenderer
    {
        public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            string text = "Text" + e.EditorInstance.CurrentLanguage;
            int arrowWidth = (int)entity.attributesMap["arrowWidth"].ValueVar;
            if (arrowWidth != 0) arrowWidth /= 2;
            int frameID = (int)entity.attributesMap["frameID"].ValueVar;
            int listID = (int)entity.attributesMap["listID"].ValueVar;
            bool auxIcon = entity.attributesMap["auxIcon"].ValueBool;
            int auxframeID = (int)entity.attributesMap["auxFrameID"].ValueVar;
            int auxlistID = (int)entity.attributesMap["auxListID"].ValueVar;
            var editorAnim = e.EditorInstance.EditorEntity_ini.LoadAnimation(text, d, listID, frameID, false, false, false);
            var leftArrow = e.EditorInstance.EditorEntity_ini.LoadAnimation("UIElements", d, 2, 0, false, false, false);
            var rightArrow = e.EditorInstance.EditorEntity_ini.LoadAnimation("UIElements", d, 2, 1, false, false, false);
            int width = (int)entity.attributesMap["size"].ValuePosition.X.High;
            int height = (int)entity.attributesMap["size"].ValuePosition.Y.High;
            double alignmentVal = 0;
            int align = (int)entity.attributesMap["align"].ValueVar;
            switch (align)
            {
                case 0:
                    alignmentVal = -(width / 2) + 16;
                    break;
                case 1:
                    alignmentVal = (22 / 2);
                    break;
            }
            var editorAnimIcon = e.EditorInstance.EditorEntity_ini.LoadAnimation("SaveSelect", d, auxlistID, auxframeID, false, false, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[Animation.index];
                e.DrawUIButtonBack(d, x, y, width, height, frame.Frame.Width, frame.Frame.Height);
                d.DrawBitmap(frame.Texture, x + frame.Frame.PivotX + (int)alignmentVal, y + frame.Frame.PivotY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }
            if (leftArrow != null && leftArrow.Frames.Count != 0)
            {
                var frame = leftArrow.Frames[Animation.index];
                d.DrawBitmap(frame.Texture, x + frame.Frame.PivotX - arrowWidth + (int)alignmentVal, y + frame.Frame.PivotY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }
            if (rightArrow != null && rightArrow.Frames.Count != 0)
            {
                var frame = rightArrow.Frames[Animation.index];
                d.DrawBitmap(frame.Texture, x + frame.Frame.PivotX + arrowWidth + (int)alignmentVal, y + frame.Frame.PivotY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }
            if (auxIcon)
            {
                if (editorAnimIcon != null && editorAnimIcon.Frames.Count != 0)
                {
                    var frame = editorAnimIcon.Frames[Animation.index];
                    d.DrawBitmap(frame.Texture, x + frame.Frame.PivotX + (int)alignmentVal, y + frame.Frame.PivotY,
                        frame.Frame.Width, frame.Frame.Height, false, Transparency);
                }
            }


        }

        public override string GetObjectName()
        {
            return "UIChoice";
        }
    }
}
