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
    public class UIButton : EntityRenderer
    {
        public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            string text = "Text" + e.EditorInstance.CurrentLanguage;
            int frameID = (int)entity.attributesMap["frameID"].ValueVar;
            int listID = (int)entity.attributesMap["listID"].ValueVar;
            int align = (int)entity.attributesMap["align"].ValueVar;
            int width = (int)entity.attributesMap["size"].ValuePosition.X.High;
            int height = (int)entity.attributesMap["size"].ValuePosition.Y.High;
            bool invisible = entity.attributesMap["invisible"].ValueBool;
            double alignmentVal = 0;
            var editorAnim = e.EditorInstance.EditorEntity_ini.LoadAnimation(text, d, listID, frameID, false, false, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && !invisible)
            {
                var frame = editorAnim.Frames[0];
                switch (align)
                {
                    case 0:
                        alignmentVal = -((width / 2)) - frame.Frame.CenterY;
                        break;
                    default:
                        alignmentVal = frame.Frame.CenterX + (22 / 2);
                        break;
                }
                e.DrawUIButtonBack(d, x, y, width, height, frame.Frame.Width, frame.Frame.Height);
                d.DrawBitmap(frame.Texture, x + (int)alignmentVal, y + frame.Frame.CenterY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }


        }

        public override string GetObjectName()
        {
            return "UIButton";
        }
    }
}
