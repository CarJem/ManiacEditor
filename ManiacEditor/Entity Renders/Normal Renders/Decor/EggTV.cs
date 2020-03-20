﻿using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class EggTV : EntityRenderer
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
            var widthPixels = (int)(entity.attributesMap["size"].ValueVector2.X.High);
            var heightPixels = (int)(entity.attributesMap["size"].ValueVector2.Y.High);
            var width = (int)widthPixels / 16;
            var height = (int)heightPixels / 16;

            var editorAnim = Controls.Editor.MainEditor.Instance.EntityDrawing.LoadAnimation2("EditorAssets", d.DevicePanel, 0, 1, false, false, false);

            if (width != 0 && height != 0)
            {
                // draw corners
                for (int i = 0; i < 4; i++)
                {
                    bool right = (i & 1) > 0;
                    bool bottom = (i & 2) > 0;

                    editorAnim = Controls.Editor.MainEditor.Instance.EntityDrawing.LoadAnimation2("EditorAssets", d.DevicePanel, 0, 1, right, bottom, false);
                    if (editorAnim != null && editorAnim.Frames.Count != 0)
                    {
                        var frame = editorAnim.Frames[Animation.index];
                        Animation.ProcessAnimation(frame.Entry.SpeedMultiplyer, frame.Entry.Frames.Count, frame.Frame.Delay);
                        d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frame),
                            (x + widthPixels / (right ? 2 : -2)) - (right ? frame.Frame.Width : 0),
                            (y + heightPixels / (bottom ? 2 : -2) - (bottom ? frame.Frame.Height : 0)),
                            frame.Frame.Width, frame.Frame.Height, false, Transparency);

                    }
                }

                // draw top and bottom
                for (int i = 0; i < 2; i++)
                {
                    bool bottom = (i & 1) > 0;

                    editorAnim = Controls.Editor.MainEditor.Instance.EntityDrawing.LoadAnimation2("EditorAssets", d.DevicePanel, 0, 2, false, bottom, false);
                    if (editorAnim != null && editorAnim.Frames.Count != 0)
                    {
                        var frame = editorAnim.Frames[Animation.index];
                        Animation.ProcessAnimation(frame.Entry.SpeedMultiplyer, frame.Entry.Frames.Count, frame.Frame.Delay);
                        bool wEven = width % 2 == 0;
                        for (int j = 1; j < width; j++)
                            d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frame),
                                (x + (wEven ? frame.Frame.PivotX : -frame.Frame.Width) + (-width / 2 + j) * frame.Frame.Width),
                                (y + heightPixels / (bottom ? 2 : -2) - (bottom ? frame.Frame.Height : 0)),
                                frame.Frame.Width, frame.Frame.Height, false, Transparency);
                    }
                }

                // draw sides
                for (int i = 0; i < 2; i++)
                {
                    bool right = (i & 1) > 0;

                    editorAnim = Controls.Editor.MainEditor.Instance.EntityDrawing.LoadAnimation2("EditorAssets", d.DevicePanel, 0, 3, right, false, false);
                    if (editorAnim != null && editorAnim.Frames.Count != 0)
                    {
                        var frame = editorAnim.Frames[Animation.index];
                        Animation.ProcessAnimation(frame.Entry.SpeedMultiplyer, frame.Entry.Frames.Count, frame.Frame.Delay);
                        bool hEven = height % 2 == 0;
                        for (int j = 1; j < height; j++)
                            d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frame),
                                (x + widthPixels / (right ? 2 : -2)) - (right ? frame.Frame.Width : 0),
                                (y + (hEven ? frame.Frame.PivotY : -frame.Frame.Height) + (-height / 2 + j) * frame.Frame.Height),
                                frame.Frame.Width, frame.Frame.Height, false, Transparency);
                    }
                }
            }
        }

        public override string GetObjectName()
        {
            return "EggTV";
        }
    }
}