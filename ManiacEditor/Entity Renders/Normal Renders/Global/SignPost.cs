﻿using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class SignPost : EntityRenderer
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
            var editorAnim = Controls.Editor.MainEditor.Instance.EntityDrawing.LoadAnimation2("SignPost", d.DevicePanel, 0, -1, false, false, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[Animation.index];
                Animation.ProcessAnimation(1, frame.Entry.Frames.Count, frame.Frame.Delay);
                d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frame), x + frame.Frame.PivotX, y + frame.Frame.PivotY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }
            if (Methods.Runtime.GameHandler.SelectedGameVersion == "1.3")
            {
                editorAnim = Controls.Editor.MainEditor.Instance.EntityDrawing.LoadAnimation2("SignPost", d.DevicePanel, 4, -1, false, false, false);
                if (editorAnim != null && editorAnim.Frames.Count != 0)
                {
                    for (int i = 0; i < editorAnim.Frames.Count; ++i)
                    {
                        if (i == 1)
                            continue;
                        var frame = editorAnim.Frames[i];
                        d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frame), x + frame.Frame.PivotX, y + frame.Frame.PivotY,
                            frame.Frame.Width, frame.Frame.Height, false, Transparency);
                    }
                }
            }
            else
            {
                editorAnim = Controls.Editor.MainEditor.Instance.EntityDrawing.LoadAnimation2("SignPost", d.DevicePanel, 6, -1, false, false, false);
                if (editorAnim != null && editorAnim.Frames.Count != 0)
                {
                    for (int i = 0; i < editorAnim.Frames.Count; ++i)
                    {
                        if (i == 1)
                            continue;
                        var frame = editorAnim.Frames[i];
                        d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frame), x + frame.Frame.PivotX, y + frame.Frame.PivotY,
                            frame.Frame.Width, frame.Frame.Height, false, Transparency);
                    }
                }
            }

        }

        public override string GetObjectName()
        {
            return "SignPost";
        }
    }
}
