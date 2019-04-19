﻿using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class SeeSaw : EntityRenderer
    {

        public override void Draw(GraphicsHandler d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            if (Editor.Instance.Entities.SetupObject != "MMZSetup")
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
                var editorAnim = Editor.Instance.EntityDrawing.LoadAnimation2("SeeSaw", d.DevicePanel, 0, 0, false, false, false);
                var editorAnim2 = Editor.Instance.EntityDrawing.LoadAnimation2("SeeSaw", d.DevicePanel, 1, 0, false, false, false);
                var editorAnim3 = Editor.Instance.EntityDrawing.LoadAnimation2("SeeSaw", d.DevicePanel, 2, 0, false, false, false);
                if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnim2 != null && editorAnim2.Frames.Count != 0 && editorAnim3 != null && editorAnim3.Frames.Count != 0)
                {
                    var frame = editorAnim.Frames[0];
                    var frame2 = editorAnim2.Frames[0];
                    var frame3 = editorAnim3.Frames[0];

                    d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame3),
                        x + frame3.Frame.PivotX - (fliph ? -35 : 35),
                        y + frame3.Frame.PivotY - 15,
                        frame3.Frame.Width, frame3.Frame.Height, false, Transparency);

                    d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame2),
                        x + frame2.Frame.PivotX,
                        y + frame2.Frame.PivotY,
                        frame2.Frame.Width, frame2.Frame.Height, false, Transparency);

                    d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame),
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
