﻿using RSDKv5;
using SystemColors = System.Drawing.Color;

namespace ManiacEditor.Entity_Renders
{
    public class GenericTrigger : EntityRenderer
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
            var widthPixels = (int)(entity.attributesMap["size"].ValueVector2.X.High) * 2;
            var heightPixels = (int)(entity.attributesMap["size"].ValueVector2.Y.High) * 2;
            var width = (int)widthPixels / 16;
            var height = (int)heightPixels / 16;

            //Draw Icon
            var editorAnim = Controls.Editor.MainEditor.Instance.EntityDrawing.LoadAnimation2("EditorIcons2", d.DevicePanel, 0, 5, false, false, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0)
            {
                var frame2 = editorAnim.Frames[Animation.index];

                d.DrawBitmap(new Classes.Editor.Draw.GraphicsHandler.GraphicsInfo(frame2),
                    x + frame2.Frame.PivotX,
                    y + frame2.Frame.PivotY,
                    frame2.Frame.Width, frame2.Frame.Height, false, Transparency);

                editorAnim = Controls.Editor.MainEditor.Instance.EntityDrawing.LoadAnimation2("EditorAssets", d.DevicePanel, 0, 1, false, false, false);

                if (width != 0 && height != 0)
                {
                    int x1 = x + widthPixels / -2;
                    int x2 = x + widthPixels / 2 - 1;
                    int y1 = y + heightPixels / -2;
                    int y2 = y + heightPixels / 2 - 1;


                    d.DrawLine(x1, y1, x1, y2, SystemColors.White);
                    d.DrawLine(x1, y1, x2, y1, SystemColors.White);
                    d.DrawLine(x2, y2, x1, y2, SystemColors.White);
                    d.DrawLine(x2, y2, x2, y1, SystemColors.White);

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
                            d.DrawBitmap(new Classes.Editor.Draw.GraphicsHandler.GraphicsInfo(frame),
                                (x + widthPixels / (right ? 2 : -2)) - (right ? frame.Frame.Width : 0),
                                (y + heightPixels / (bottom ? 2 : -2) - (bottom ? frame.Frame.Height : 0)),
                                frame.Frame.Width, frame.Frame.Height, false, Transparency);

                        }
                    }
                }
            }


        }

        public override string GetObjectName()
        {
            return "GenericTrigger";
        }
    }
}
