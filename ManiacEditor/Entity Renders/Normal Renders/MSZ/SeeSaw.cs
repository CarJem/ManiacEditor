﻿using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class SeeSaw : EntityRenderer
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
            if (Classes.Editor.Solution.Entities.SetupObject != "MMZSetup")
            {
                int side = (int)entity.attributesMap["side"].ValueUInt8;
                bool fliph = false;
                switch (side)
                {
                    case 0:
                        fliph = false;
                        break;
                    case 1:
                        fliph = true;
                        break;
                    default:
                        fliph = false;
                        break;
                }
                var editorAnim = Controls.Editor.MainEditor.Instance.EntityDrawing.LoadAnimation2("SeeSaw", d.DevicePanel, 0, 0, false, false, false);
                var editorAnim2 = Controls.Editor.MainEditor.Instance.EntityDrawing.LoadAnimation2("SeeSaw", d.DevicePanel, 1, 0, false, false, false);
                var editorAnim3 = Controls.Editor.MainEditor.Instance.EntityDrawing.LoadAnimation2("SeeSaw", d.DevicePanel, 2, 0, false, false, false);
                if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnim2 != null && editorAnim2.Frames.Count != 0 && editorAnim3 != null && editorAnim3.Frames.Count != 0)
                {
                    var frame = editorAnim.Frames[0];
                    var frame2 = editorAnim2.Frames[0];
                    var frame3 = editorAnim3.Frames[0];

                    d.DrawBitmap(new Classes.Editor.Draw.GraphicsHandler.GraphicsInfo(frame3),
                        x + frame3.Frame.PivotX - (fliph ? -35 : 35),
                        y + frame3.Frame.PivotY - 15,
                        frame3.Frame.Width, frame3.Frame.Height, false, Transparency);

                    d.DrawBitmap(new Classes.Editor.Draw.GraphicsHandler.GraphicsInfo(frame2),
                        x + frame2.Frame.PivotX,
                        y + frame2.Frame.PivotY,
                        frame2.Frame.Width, frame2.Frame.Height, false, Transparency);

                    d.DrawBitmap(new Classes.Editor.Draw.GraphicsHandler.GraphicsInfo(frame),
                        x + frame.Frame.PivotX,
                        y + frame.Frame.PivotY,
                        frame.Frame.Width, frame.Frame.Height, false, Transparency);


                }
            }

        }

        public override string GetObjectName()
        {
            return "SeeSaw";
        }
    }
}
