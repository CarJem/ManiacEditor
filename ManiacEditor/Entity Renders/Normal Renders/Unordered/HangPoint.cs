﻿using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class HangPoint : EntityRenderer
    {

        public override void Draw(GraphicsHandler d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            int length = (int)entity.attributesMap["length"].ValueEnum;
            int i = 0;
            bool fliph = false;
            bool flipv = false;
            var editorAnim = Editor.Instance.EntityDrawing.LoadAnimation2("HangPoint", d.DevicePanel, 0, 0, fliph, flipv, false);
            var editorAnim2 = Editor.Instance.EntityDrawing.LoadAnimation2("HangPoint", d.DevicePanel, 0, 1, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnim2 != null && editorAnim2.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[0];
                var frame2 = editorAnim2.Frames[0];
                int lengthMemory = length;

                //Animation.ProcessAnimation(frame.Entry.SpeedMultiplyer, frame.Entry.Frames.Count, frame.Frame.Delay);
                    int repeat = 0;
                    int lengthLeft = length;
                    bool finalLoop = false;
                    while (lengthLeft > 256)
                    {
                        repeat++;
                        lengthLeft = lengthLeft - 256;
                    }
                    d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame2),
                        x + frame2.Frame.PivotX - (fliph ? (frame2.Frame.Width - editorAnim2.Frames[0].Frame.Width) : 0),
                        y - (i * 256) + (flipv ? (frame2.Frame.Height - editorAnim2.Frames[0].Frame.Height) : 0),
                        frame2.Frame.Width, lengthMemory, false, Transparency);
                    for (i = 1; i < repeat+1; i++)
                    {
                    if (i == repeat)
                    {
                        finalLoop = true;
                    }
                        d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame2),
                            x + frame2.Frame.PivotX - (fliph ? (frame2.Frame.Width - editorAnim2.Frames[0].Frame.Width) : 0),
                                y + (i * 256) + (flipv ? (frame2.Frame.Height - editorAnim2.Frames[0].Frame.Height) : 0),
                                frame2.Frame.Width, (finalLoop ? lengthLeft : frame2.Frame.Height), false, Transparency);
                    }
                    d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame),
                        x + frame.Frame.PivotX - (fliph ? (frame.Frame.Width - editorAnim.Frames[0].Frame.Width) : 0),
                        y + length + (flipv ? (frame.Frame.Height - editorAnim.Frames[0].Frame.Height) : 0),
                            frame.Frame.Width, frame.Frame.Height, false, Transparency);

            }
        }

        public override string GetObjectName()
        {
            return "HangPoint";
        }
    }
}