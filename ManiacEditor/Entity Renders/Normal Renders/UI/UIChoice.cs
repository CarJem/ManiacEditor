﻿using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class UIChoice : EntityRenderer
    {
        public override void Draw(GraphicsHandler d, SceneEntity entity, Classes.Editor.Scene.Sets.EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            string text = "Text" + Classes.Editor.SolutionState.CurrentLanguage;
            int arrowWidth = (int)entity.attributesMap["arrowWidth"].ValueEnum;
            if (arrowWidth != 0) arrowWidth /= 2;
            int frameID = (int)entity.attributesMap["frameID"].ValueEnum;
            int listID = (int)entity.attributesMap["listID"].ValueEnum;
            bool auxIcon = entity.attributesMap["auxIcon"].ValueBool;
            bool noText = entity.attributesMap["noText"].ValueBool;
            int auxframeID = (int)entity.attributesMap["auxFrameID"].ValueEnum;
            int auxlistID = (int)entity.attributesMap["auxListID"].ValueEnum;
            var editorAnim = Editor.Instance.EntityDrawing.LoadAnimation(text, d.DevicePanel, listID, frameID, false, false, false);
            var leftArrow = Editor.Instance.EntityDrawing.LoadAnimation("UIElements", d.DevicePanel, 2, 0, false, false, false);
            var rightArrow = Editor.Instance.EntityDrawing.LoadAnimation("UIElements", d.DevicePanel, 2, 1, false, false, false);
            int width = (int)entity.attributesMap["size"].ValueVector2.X.High;
            int height = (int)entity.attributesMap["size"].ValueVector2.Y.High;
            double alignmentVal = 0;
            int align = (int)entity.attributesMap["align"].ValueEnum;
            switch (align)
            {
                case 0:
                    alignmentVal = -(width / 2) + 16;
                    break;
                case 1:
                    alignmentVal = (22 / 2);
                    break;
            }
            var editorAnimIcon = Editor.Instance.EntityDrawing.LoadAnimation("SaveSelect", d.DevicePanel, auxlistID, auxframeID, false, false, false);
            e.DrawUIButtonBack(d, x, y, width, height, 0, 0, Transparency);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && noText == false)
            {
                var frame = editorAnim.Frames[Animation.index];
                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame), x + frame.Frame.PivotX + (int)alignmentVal, y + frame.Frame.PivotY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }
            if (leftArrow != null && leftArrow.Frames.Count != 0)
            {
                var frame = leftArrow.Frames[Animation.index];
                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame), x + frame.Frame.PivotX - arrowWidth + (int)alignmentVal, y + frame.Frame.PivotY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }
            if (rightArrow != null && rightArrow.Frames.Count != 0)
            {
                var frame = rightArrow.Frames[Animation.index];
                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame), x + frame.Frame.PivotX + arrowWidth + (int)alignmentVal, y + frame.Frame.PivotY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }
            if (auxIcon)
            {
                if (editorAnimIcon != null && editorAnimIcon.Frames.Count != 0)
                {
                    var frame = editorAnimIcon.Frames[Animation.index];
                    d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame), x + frame.Frame.PivotX + (int)alignmentVal, y + frame.Frame.PivotY,
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
