﻿using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class BouncePlant : EntityRenderer
    {

        public override void Draw(GraphicsHandler d, SceneEntity entity, Classes.Edit.Scene.Sets.EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            int direction = (int)entity.attributesMap["direction"].ValueUInt8;
            bool fliph = false;
            bool flipv = false;
            if (direction == 0)
            {
                fliph = true;
            }
            var editorAnim = Editor.Instance.EntityDrawing.LoadAnimation2("Plants", d.DevicePanel, 1, -1, fliph, flipv, false);
            x += (fliph ? 42 : -42);
            y += -42;
            if (editorAnim != null && editorAnim.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[0];

                for (int i = 0; i < 8; i++)
                {
                    d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame),
                        x + frame.Frame.PivotX + (fliph ? -(12 * i) : (12 * i)),
                        y + frame.Frame.PivotY + 12*i,
                        frame.Frame.Width, frame.Frame.Height, false, Transparency);
                }

            }
        }

        public override string GetObjectName()
        {
            return "BouncePlant";
        }
    }
}
