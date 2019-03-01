using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using ManiacEditor;
using Microsoft.Xna.Framework;
using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class UIOptionPanel : EntityRenderer
    {
        public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            string text = "Text" + e.EditorInstance.CurrentLanguage;
            int topListID = (int)entity.attributesMap["topListID"].ValueVar;
            int topFrameID = (int)entity.attributesMap["topFrameID"].ValueVar;
            int botListID = (int)entity.attributesMap["botListID"].ValueVar;
            int botFrameID = (int)entity.attributesMap["botFrameID"].ValueVar;
            int panelSize = (int)entity.attributesMap["panelSize"].ValueVar;
            bool botAlignRight = entity.attributesMap["botAlignRight"].ValueBool;
            bool botHidden = entity.attributesMap["botHidden"].ValueBool;

            panelSize /= 2;

            int top = x - 212;
            int left = y - panelSize;
            int bottom = x + 212;
            int right = y + panelSize;

            d.DrawRectangle(top, left, bottom, right, System.Drawing.Color.FromArgb(255, 49, 162, 247));

            d.DrawRectangle(top, left, x - 6, left+24, System.Drawing.Color.Black);
            e.DrawTriangle(d, x - 6, left+12, 10, 24, 10, 10, 1);

            if (!botHidden)
            {
                d.DrawRectangle(bottom, right, x - 6, right - 24, System.Drawing.Color.Black);
                e.DrawTriangle(d, x - 6, right - 12, 24, 24, 10, 10, 0);
            }


            var editorAnimTop = e.EditorInstance.EditorEntity_ini.LoadAnimation(text, d, topListID, topFrameID, false, false, false);
            var editorAnimBot = e.EditorInstance.EditorEntity_ini.LoadAnimation(text, d, botListID, botFrameID, false, false, false);
            if (editorAnimTop != null && editorAnimTop.Frames.Count != 0)
            {
                var frame = editorAnimTop.Frames[Animation.index];

                int topX = top + 68;
                int topY = left + 12;

                d.DrawBitmap(frame.Texture, topX + frame.Frame.PivotX, topY + frame.Frame.PivotY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }
            if (editorAnimBot != null && editorAnimBot.Frames.Count != 0 && !botHidden)
            {
                var frame = editorAnimBot.Frames[Animation.index];

                int botX = x + (botAlignRight ? frame.Frame.Width - 6 : 0);
                int botY = right - 12;

                d.DrawBitmap(frame.Texture, botX + frame.Frame.PivotX, botY + frame.Frame.PivotY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }


        }

        public override string GetObjectName()
        {
            return "UIOptionPanel";
        }
    }
}
