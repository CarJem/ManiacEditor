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
    public class UIResPicker : EntityRenderer
    {
        public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            string text = "Text" + e.EditorInstance.CurrentLanguage;
            int arrowWidth = (int)entity.attributesMap["arrowWidth"].ValueVar;
            if (arrowWidth != 0) arrowWidth /= 2;
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

			e.DrawUIButtonBack(d, x, y, width, height, 0, 0);
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


        }

        public override string GetObjectName()
        {
            return "UIResPicker";
        }
    }
}
