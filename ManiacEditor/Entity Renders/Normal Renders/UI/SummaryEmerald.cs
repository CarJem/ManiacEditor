﻿using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class SummaryEmerald : EntityRenderer
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
            int emeraldID = (int)entity.attributesMap["emeraldID"].ValueEnum;
            var editorAnim = Controls.Editor.MainEditor.Instance.EntityDrawing.LoadAnimation("SummaryEmerald", d.DevicePanel, 0, emeraldID, false, false, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[Animation.index];
                //Animation.ProcessAnimation(frame.Entry.SpeedMultiplyer, frame.Entry.Frames.Count, frame.Frame.Delay);
                d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frame), x + frame.Frame.PivotX, y + frame.Frame.PivotY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }
        }

        public override string GetObjectName()
        {
            return "SummaryEmerald";
        }
    }
}
