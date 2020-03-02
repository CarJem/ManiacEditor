﻿using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class UIVsCharSelector : EntityRenderer
    {

        public override void Draw(Structures.EntityRenderProp properties)
        {
            Methods.Draw.GraphicsHandler d = properties.Graphics;
            SceneEntity entity = properties.Object; 
            Classes.Scene.Sets.EditorEntity e = properties.EditorObject;
            int x = properties.X;
            int y = properties.Y;
            int Transparency = properties.Transparency;
            int index = properties.Index;
            int previousChildCount = properties.PreviousChildCount;
            int platformAngle = properties.PlatformAngle;
            Methods.Entities.EntityAnimator Animation = properties.Animations;
            bool selected  = properties.isSelected;
            string text = "Text" + Methods.Editor.SolutionState.CurrentLanguage;
            int playerID = (int)entity.attributesMap["playerID"].ValueUInt8;
            int player = 8;
            switch (playerID)
            {
                case 0:
                    player = 8;
                    break;
                case 1:
                    player = 9;
                    break;
                case 2:
                    player = 10;
                    break;
                case 3:
                    player = 11;
                    break;

            }
            var editorAnimWaiting = Controls.Editor.MainEditor.Instance.EntityDrawing.LoadAnimation(text, d.DevicePanel, 12, 7, false, false, false);
            var editorAnimBackground = Controls.Editor.MainEditor.Instance.EntityDrawing.LoadAnimation("SaveSelect", d.DevicePanel, 14, 7, false, false, false);
            var editorAnimPlayerText = Controls.Editor.MainEditor.Instance.EntityDrawing.LoadAnimation(text, d.DevicePanel, 12, player, false, false, false);
            var editorAnimFrame = Controls.Editor.MainEditor.Instance.EntityDrawing.LoadAnimation("EditorUIRender", d.DevicePanel, 1, 0, false, false, false);

            d.DrawRectangle(x - 48, y - 48, x + 48, y + 48, System.Drawing.Color.FromArgb(128, 255, 255, 255));

            if (editorAnimFrame != null && editorAnimFrame.Frames.Count != 0)
            {
                var frame = editorAnimFrame.Frames[Animation.index];
                d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frame), x + frame.Frame.PivotX, y + frame.Frame.PivotY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }

            if (editorAnimBackground != null && editorAnimBackground.Frames.Count != 0)
            {
                var frame = editorAnimBackground.Frames[Animation.index];
                d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frame), x + frame.Frame.PivotX, y + frame.Frame.PivotY - 8,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }

            if (editorAnimWaiting != null && editorAnimWaiting.Frames.Count != 0)
            {
                var frame = editorAnimWaiting.Frames[Animation.index];
                d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frame), x + frame.Frame.PivotX, y + frame.Frame.PivotY - 8,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }

            if (editorAnimPlayerText != null && editorAnimPlayerText.Frames.Count != 0)
            {
                var frame = editorAnimPlayerText.Frames[Animation.index];
                d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frame), x + frame.Frame.PivotX + 36, y + frame.Frame.PivotY - 38,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }





        }

        public override string GetObjectName()
        {
            return "UIVsCharSelector";
        }
    }
}
