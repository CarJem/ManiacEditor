﻿using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class UIHeading : EntityRenderer
    {

        public override void Draw(Structures.EntityRenderProp properties)
        {
            Classes.Editor.Draw.GraphicsHandler d = properties.Graphics;
            SceneEntity entity = properties.Object; 
            Classes.Editor.Scene.Sets.EditorEntity e = properties.EditorObject;
            int x = properties.X;
            int y = properties.Y;
            int Transparency = properties.Transparency;
            int index = properties.Index;
            int previousChildCount = properties.PreviousChildCount;
            int platformAngle = properties.PlatformAngle;
            Methods.Entities.EntityAnimator Animation = properties.Animations;
            bool selected  = properties.isSelected;
            string text = "Headings" + Classes.Editor.SolutionState.CurrentLanguage;
            int listID = (int)entity.attributesMap["headingID"].ValueEnum;
            var editorAnim = Controls.Editor.MainEditor.Instance.EntityDrawing.LoadAnimation(text, d.DevicePanel, listID, 0, false, false, false);
            var editorAnimBar = Controls.Editor.MainEditor.Instance.EntityDrawing.LoadAnimation("UIElements", d.DevicePanel, 0, 0, false, false, false);
            if (editorAnimBar != null && editorAnimBar.Frames.Count != 0)
            {
                var frame = editorAnimBar.Frames[Animation.index];
                //Animation.ProcessAnimation(frame.Entry.SpeedMultiplyer, frame.Entry.Frames.Count, frame.Frame.Delay);
                d.DrawBitmap(new Classes.Editor.Draw.GraphicsHandler.GraphicsInfo(frame), x + frame.Frame.PivotX, y + frame.Frame.PivotY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }
            if (editorAnim != null && editorAnim.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[Animation.index];
                //Animation.ProcessAnimation(frame.Entry.SpeedMultiplyer, frame.Entry.Frames.Count, frame.Frame.Delay);
                d.DrawBitmap(new Classes.Editor.Draw.GraphicsHandler.GraphicsInfo(frame), x + frame.Frame.PivotX, y + frame.Frame.PivotY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }


        }

        public override string GetObjectName()
        {
            return "UIHeading";
        }
    }
}
