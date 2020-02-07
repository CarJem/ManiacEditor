﻿using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class BallCannon : EntityRenderer
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
            int x2 = x;
            int y2 = y;
            bool flipv = false;
            bool fliph = false;
            int angle = (int)entity.attributesMap["angle"].ValueEnum;
            int type = (int)entity.attributesMap["type"].ValueUInt8;
            int rotation = 0;
            int rotation2 = 0;
            int CorkState = 0;
            if (type == 0)
            {
                switch (angle)
                {
                    case 0:
                        rotation = 90;
                        rotation2 = 180;
                        y += 16;
                        x += 8;
                        y2 += 24;
                        x2 += 0;
                        break;
                    case 1:
                        rotation = 180;
                        rotation2 = 270;
                        y += 24;
                        x -= 0;
                        y2 += 16;
                        x2 += -8;
                        break;
                    case 2:
                        rotation = 270;
                        y += 16;
                        x -= 8;
                        y2 += 8;
                        x2 += 0;
                        break;
                    case 3:
                        rotation2 = 90;
                        y += 8;
                        x += 0;
                        y2 += 16;
                        x2 += 8;
                        break;
                    case 4:
                        rotation2 = 90;
                        rotation = 180;
                        y += 24;
                        x += 0;
                        y2 += 16;
                        x2 += 8;
                        break;
                    case 5:
                        rotation = 270;
                        rotation2 = 180;
                        y += 16;
                        x -= 8;
                        y2 += 24;
                        x2 += 0;
                        break;
                    case 6:
                        rotation2 = 270;
                        y += 8;
                        x += 0;
                        y2 += 16;
                        x2 -= 8;
                        break;
                    case 7:
                        rotation2 = 0;
                        rotation = 90;
                        y += 16;
                        x += 8;
                        y2 += 8;
                        x2 += 0;
                        break;
                    default:
                        rotation = 90;
                        rotation2 = 180;
                        y += 16;
                        x += 8;
                        y2 += 24;
                        x2 += 0;
                        break;
                }
            }
            else
            {
                switch (type)
                {
                    case 0:
                        break;
                    case 1:
                        CorkState = 3;
                        break;
                    case 2:
                        CorkState = 4;
                        break;
                }
            }

            var editorAnim = Controls.Editor.MainEditor.Instance.EntityDrawing.LoadAnimation2("BallCannon", d.DevicePanel, 0, -1, fliph, flipv, true, rotation);
            var editorAnimHolo = Controls.Editor.MainEditor.Instance.EntityDrawing.LoadAnimation2("BallCannon", d.DevicePanel, 0, -1, fliph, flipv, true, rotation2);
            var editorAnimCork = Controls.Editor.MainEditor.Instance.EntityDrawing.LoadAnimation2("BallCannon", d.DevicePanel, CorkState, 0, fliph, flipv, true);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnimHolo != null && editorAnimHolo.Frames.Count != 0 && editorAnimCork != null && editorAnimCork.Frames.Count != 0)
            {
                if (type == 1)
                {
                    var frame = editorAnimCork.Frames[0];

                    d.DrawBitmap(new Classes.Editor.Draw.GraphicsHandler.GraphicsInfo(frame),
                        x + frame.Frame.PivotX,
                        y + frame.Frame.PivotY,
                        frame.Frame.Height, frame.Frame.Height, false, Transparency);
                }
                else if (type == 2)
                {
                    var frame = editorAnimCork.Frames[0];

                    d.DrawBitmap(new Classes.Editor.Draw.GraphicsHandler.GraphicsInfo(frame),
                        x + frame.Frame.PivotX,
                        y + frame.Frame.PivotY,
                        frame.Frame.Height, frame.Frame.Height, false, Transparency);
                }
                else
                {
                    var frame = editorAnim.Frames[Animation.index];
                    var frame3 = editorAnimHolo.Frames[Animation.index];

                    if (selected)
                    {
                        d.DrawBitmap(new Classes.Editor.Draw.GraphicsHandler.GraphicsInfo(frame3),
                            x2 + frame3.Frame.PivotX,
                            y2 + frame3.Frame.PivotY,
                            frame3.Frame.Height, frame3.Frame.Height, false, Transparency - 125);
                    }

                    d.DrawBitmap(new Classes.Editor.Draw.GraphicsHandler.GraphicsInfo(frame),
                        x + frame.Frame.PivotX,
                        y + frame.Frame.PivotY,
                        frame.Frame.Height, frame.Frame.Height, false, Transparency);
                }


            }
        }

        public override string GetObjectName()
        {
            return "BallCannon";
        }
    }
}
