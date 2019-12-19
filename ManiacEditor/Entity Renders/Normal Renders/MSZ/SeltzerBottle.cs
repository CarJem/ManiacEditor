﻿using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class SeltzerBottle : EntityRenderer
    {

        public override void Draw(GraphicsHandler d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            bool fliph = false;
            bool flipv = false;
            int direction = (int)entity.attributesMap["direction"].ValueUInt8;
            if (direction == 1)
            {
                fliph = true;
            }
            var editorAnim = Editor.Instance.EntityDrawing.LoadAnimation2("Seltzer", d.DevicePanel, 0, -1, fliph, flipv, false);
            var editorAnim2 = Editor.Instance.EntityDrawing.LoadAnimation2("Seltzer", d.DevicePanel, 1, -1, fliph, flipv, false);
            var editorAnim3 = Editor.Instance.EntityDrawing.LoadAnimation2("Seltzer", d.DevicePanel, 0, 5, fliph, flipv, false);
            var editorAnim4 = Editor.Instance.EntityDrawing.LoadAnimation2("Seltzer", d.DevicePanel, 0, 4, false, false, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnim2 != null && editorAnim2.Frames.Count != 0 && editorAnim3 != null && editorAnim3.Frames.Count != 0 && editorAnim4 != null && editorAnim4.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[0];
                var frame2 = editorAnim2.Frames[0];
                var frame3 = editorAnim.Frames[1];
                var frame4 = editorAnim.Frames[2];
                var frame5 = editorAnim.Frames[3];
                var frame6 = editorAnim4.Frames[0];
                var frame7 = editorAnim3.Frames[0];

                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame),
                    x + frame.Frame.PivotX,
                    y + frame.Frame.PivotY,
                    frame.Frame.Width, frame.Frame.Height, false, 45);
                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame7),
                    x + frame7.Frame.PivotX + (fliph ? 10 : 0),
                    y + frame7.Frame.PivotY,
                    frame7.Frame.Width, frame7.Frame.Height, false, Transparency);
                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame2),
                    x + frame2.Frame.PivotX,
                    y + frame2.Frame.PivotY,
                    frame2.Frame.Width, frame2.Frame.Height, false, Transparency);
                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame3),
                    x + frame3.Frame.PivotX,
                    y + frame3.Frame.PivotY,
                    frame3.Frame.Width, frame3.Frame.Height, false, Transparency);
                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame4),
                    x + frame4.Frame.PivotX,
                    y + frame4.Frame.PivotY,
                    frame4.Frame.Width, frame4.Frame.Height, false, Transparency);
                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame5),
                    x + frame5.Frame.PivotX,
                    y + frame5.Frame.PivotY,
                    frame5.Frame.Width, frame5.Frame.Height, false, Transparency);
                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame6),
                    x + frame6.Frame.PivotX,
                    y + frame6.Frame.PivotY,
                    frame6.Frame.Width, frame6.Frame.Height, false, Transparency);
            }
        }

        public override string GetObjectName()
        {
            return "SeltzerBottle";
        }
    }
}