﻿using RSDKv5;


namespace ManiacEditor.Entity_Renders
{
    public class LaunchSpring : EntityRenderer
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
            int angle = (int)entity.attributesMap["angle"].ValueInt32;
            int type = (int)entity.attributesMap["type"].ValueUInt8;
            int rotation = 0;
            switch (angle)
            {
                case 0:
                    rotation = 0;
                    break;
                case 1:
                    rotation = 45;
                    break;
                case 2:
                    rotation = 90;
                    break;
                case 3:
                    rotation = 135;
                    break;
                case 4:
                   rotation = 180;
                   break;
                case 5:
                    rotation = 225;
                    break;
                case 6:
                    rotation = 270;
                    break;
                case 7:
                    rotation = 315;
                    break;
            }


            var editorAnim = Controls.Editor.MainEditor.Instance.EntityDrawing.LoadAnimation2("LaunchSpring", d.DevicePanel, 0, -1, false, true, false, rotation, true, false, Methods.Entities.EntityDrawing.Flag.FullEngineRotation);
            var editorAnim2 = Controls.Editor.MainEditor.Instance.EntityDrawing.LoadAnimation2("LaunchSpring", d.DevicePanel, 0, -1, true, true, false, rotation, true, false, Methods.Entities.EntityDrawing.Flag.FullEngineRotation);
            var editorAnim3 = Controls.Editor.MainEditor.Instance.EntityDrawing.LoadAnimation2("LaunchSpring", d.DevicePanel, 1, -1, false, false, false);
            var editorAnim4 = Controls.Editor.MainEditor.Instance.EntityDrawing.LoadAnimation2("LaunchSpring", d.DevicePanel, 2, -1, false, false, false, rotation, true, false, Methods.Entities.EntityDrawing.Flag.FullEngineRotation);


            if (editorAnim != null && editorAnim2 != null && editorAnim3 != null && editorAnim4 != null && editorAnim.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[0];
                var frame2 = editorAnim2.Frames[0];
                var frame3 = editorAnim3.Frames[0];
                var frame4 = editorAnim4.Frames[1];
                switch (angle)
                {
                    case 0:
                        d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frame4),
                        x + frame4.Frame.PivotX,
                        y + frame4.Frame.PivotY - 8,
                        frame4.ImageWidth, frame4.ImageHeight, false, Transparency);
                        //Universal Center Screw
                        d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frame3),
                            x + frame3.Frame.PivotX,
                            y + frame3.Frame.PivotY,
                            frame3.Frame.Width, frame3.Frame.Height, false, Transparency);
                        //Launcher
                        d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frame),
                            x + frame.Frame.PivotX - 20,
                            y + frame.Frame.PivotY,
                            frame.ImageWidth, frame.ImageHeight, false, Transparency);
                        d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frame2),
                            x + frame2.Frame.PivotX + 20,
                            y + frame2.Frame.PivotY + (type == 0 ? 46 : 0),
                            frame2.ImageWidth, frame2.ImageHeight, false, Transparency);
                        break;
                    case 1:

                        d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frame4),
                        x + frame4.Frame.PivotX - 9,
                        y + frame4.Frame.PivotY - 23,
                        frame4.ImageWidth, frame4.ImageHeight, false, Transparency);
                        //Universal Center Screw
                        d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frame3),
                            x + frame3.Frame.PivotX,
                            y + frame3.Frame.PivotY,
                            frame3.Frame.Width, frame3.Frame.Height, false, Transparency);
                        //Launcher
                        d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frame),
                            x + frame.Frame.PivotX + 3,
                            y + frame.Frame.PivotY - 8,
                            frame.ImageWidth, frame.ImageHeight, false, Transparency);
                        d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frame2),
                            x + frame2.Frame.PivotX + 30 - (type == 0 ? 32 : 0),
                            y + frame2.Frame.PivotY + 20 + (type == 0 ? 33 : 0),
                            frame2.ImageWidth, frame2.ImageHeight, false, Transparency);
                        break;
                    case 2:
                        d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frame4),
                        x + frame4.Frame.PivotX + 8,
                        y + frame4.Frame.PivotY + 0,
                        frame4.ImageWidth, frame4.ImageHeight, false, Transparency);
                        //Universal Center Screw
                        d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frame3),
                            x + frame3.Frame.PivotX,
                            y + frame3.Frame.PivotY,
                            frame3.Frame.Width, frame3.Frame.Height, false, Transparency);
                        //Launcher
                        d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frame),
                            x + frame.Frame.PivotX + 23,
                            y + frame.Frame.PivotY + 3,
                            frame.ImageWidth, frame.ImageHeight, false, Transparency);
                        d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frame2),
                            x + frame2.Frame.PivotX + 23 - (type == 0 ? 47 : 0),
                            y + frame2.Frame.PivotY + 43,
                            frame2.ImageWidth, frame2.ImageHeight, false, Transparency);
                        break;
                    case 3:
                        d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frame4),
                        x + frame4.Frame.PivotX - 8,
                        y + frame4.Frame.PivotY - 8,
                        frame4.ImageWidth, frame4.ImageHeight, false, Transparency);
                        //Universal Center Screw
                        d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frame3),
                            x + frame3.Frame.PivotX,
                            y + frame3.Frame.PivotY,
                            frame3.Frame.Width, frame3.Frame.Height, false, Transparency);
                        //Launcher
                        d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frame),
                            x + frame.Frame.PivotX + 31 - (type == 0 ? 32 : 0),
                            y + frame.Frame.PivotY + 26 - (type == 0 ? 32 : 0),
                            frame.ImageWidth, frame.ImageHeight, false, Transparency);
                        d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frame2),
                            x + frame2.Frame.PivotX + 4,
                            y + frame2.Frame.PivotY + 54,
                            frame2.ImageWidth, frame2.ImageHeight, false, Transparency);
                        break;
                    case 4:
                                        
                        d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frame4),
                        x + frame4.Frame.PivotX + 2,
                        y + frame4.Frame.PivotY + 8,
                        frame4.ImageWidth, frame4.ImageHeight, false, Transparency);
                        //Universal Center Screw
                        d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frame3),
                            x + frame3.Frame.PivotX,
                            y + frame3.Frame.PivotY,
                            frame3.Frame.Width, frame3.Frame.Height, false, Transparency);
                        //Launcher
                        d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frame),
                            x + frame.Frame.PivotX + 20,
                            y + frame.Frame.PivotY + 47 - (type == 0 ? 47 : 0),
                            frame.ImageWidth, frame.ImageHeight, false, Transparency);
                        d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frame2),
                            x + frame2.Frame.PivotX - 20,
                            y + frame2.Frame.PivotY + 47,
                            frame2.ImageWidth, frame2.ImageHeight, false, Transparency);
                        break;
                    case 5:
                        d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frame4),
                        x + frame4.Frame.PivotX -24,
                        y + frame4.Frame.PivotY -8,
                        frame4.ImageWidth, frame4.ImageHeight, false, Transparency);
                        //Universal Center Screw
                        d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frame3),
                            x + frame3.Frame.PivotX,
                            y + frame3.Frame.PivotY,
                            frame3.Frame.Width, frame3.Frame.Height, false, Transparency);
                        //Launcher
                        d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frame),
                            x + frame.Frame.PivotX -2 + (type == 0 ? 32 : 0),
                            y + frame.Frame.PivotY + 54 - (type == 0 ? 33 : 0),
                            frame.ImageWidth, frame.ImageHeight, false, Transparency);
                        d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frame2),
                            x + frame2.Frame.PivotX - 31,
                            y + frame2.Frame.PivotY + 27,
                            frame2.ImageWidth, frame2.ImageHeight, false, Transparency);
                        break;
                    case 6:

                        d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frame4),
                        x + frame4.Frame.PivotX - 8,
                        y + frame4.Frame.PivotY,
                        frame4.ImageWidth, frame4.ImageHeight, false, Transparency);
                        //Universal Center Screw
                        d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frame3),
                            x + frame3.Frame.PivotX,
                            y + frame3.Frame.PivotY,
                            frame3.Frame.Width, frame3.Frame.Height, false, Transparency);
                        //Launcher
                        d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frame),
                            x + frame.Frame.PivotX - 24 + (type == 0 ? 47 : 0),
                            y + frame.Frame.PivotY + 42,
                            frame.ImageWidth, frame.ImageHeight, false, Transparency);
                        d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frame2),
                            x + frame2.Frame.PivotX -24,
                            y + frame2.Frame.PivotY + 3,
                            frame2.ImageWidth, frame2.ImageHeight, false, Transparency);
                        break;
                    case 7:
                        d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frame4),
                        x + frame4.Frame.PivotX - 24,
                        y + frame4.Frame.PivotY - 24,
                        frame4.ImageWidth, frame4.ImageHeight, false, Transparency);
                        //Universal Center Screw
                        d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frame3),
                            x + frame3.Frame.PivotX,
                            y + frame3.Frame.PivotY,
                            frame3.Frame.Width, frame3.Frame.Height, false, Transparency);
                        //Launcher
                        d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frame),
                            x + frame.Frame.PivotX - 32 + (type == 0 ? 34 : 0),
                            y + frame.Frame.PivotY + 20 + (type == 0 ? 34 : 0),
                            frame.ImageWidth, frame.ImageHeight, false, Transparency);
                        d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frame2),
                            x + frame2.Frame.PivotX - 3,
                            y + frame2.Frame.PivotY - 8,
                            frame2.ImageWidth, frame2.ImageHeight, false, Transparency);
                        break;

                }

            }

        }




        public override string GetObjectName()
        {
            return "LaunchSpring";
        }
    }
}