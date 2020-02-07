using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class WaterGush : EntityRenderer
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
            var length = (int)(entity.attributesMap["length"].ValueUInt32);
            int orientation = (int)(entity.attributesMap["orientation"].ValueUInt8);
            int animID = 0;
            int animID2 = 2;
            bool fliph = false;
            bool flipv = false;
            switch (orientation)
            {
                case 0:
                    animID = 0;
                    animID2 = 2;
                    break;
                case 1:
                    animID = 1;
                    animID2 = 3;
                    break;
                case 2:
                    animID = 1;
                    animID2 = 3;
                    fliph = true;
                    break;
                case 3:
                    animID = 0;
                    animID2 = 2;
                    flipv = true;
                    break;
            }
            var editorAnim = Controls.Editor.MainEditor.Instance.EntityDrawing.LoadAnimation2("WaterGush", d.DevicePanel, animID, -1, fliph, flipv, false);
            var editorAnimGush = Controls.Editor.MainEditor.Instance.EntityDrawing.LoadAnimation2("WaterGush", d.DevicePanel, animID2, -1, false, false, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnimGush != null && editorAnimGush.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[Animation.index];
                var frameGush = editorAnimGush.Frames[Animation.index];

                Animation.ProcessAnimation(frame.Entry.SpeedMultiplyer, frame.Entry.Frames.Count, frame.Frame.Delay);

                switch (orientation)
                {
                    case 0:
                        for (int i = -length + 1; i <= 0; ++i)
                        {
                            d.DrawBitmap(new Classes.Editor.Draw.GraphicsHandler.GraphicsInfo(frame), x + frame.Frame.PivotX, y + frame.Frame.PivotY + i * frame.Frame.Height,
                                frame.Frame.Width, frame.Frame.Height, false, Transparency);
                        }
                        d.DrawBitmap(new Classes.Editor.Draw.GraphicsHandler.GraphicsInfo(frameGush), x + frameGush.Frame.PivotX, y + frameGush.Frame.PivotY - length * frame.Frame.Height,
                            frameGush.Frame.Width, frameGush.Frame.Height, false, Transparency);
                        break;
                    case 1:
                        for (int i = -length + 1; i <= 0; ++i)
                        {
                            d.DrawBitmap(new Classes.Editor.Draw.GraphicsHandler.GraphicsInfo(frame), x - frame.Frame.PivotX - i * frame.Frame.Width, y + frame.Frame.PivotY,
                                frame.Frame.Width, frame.Frame.Height, false, Transparency);
                        }
                        d.DrawBitmap(new Classes.Editor.Draw.GraphicsHandler.GraphicsInfo(frameGush), x + frameGush.Frame.PivotX + length * frame.Frame.Width, y + frameGush.Frame.PivotY,
                            frameGush.Frame.Width, frameGush.Frame.Height, false, Transparency);
                        break;
                    case 2:
                        for (int i = 0; i < length; ++i)
                        {
                            d.DrawBitmap(new Classes.Editor.Draw.GraphicsHandler.GraphicsInfo(frame), x + frame.Frame.PivotX - frame.Frame.Width - i * frame.Frame.Width, y + frame.Frame.PivotY,
                                frame.Frame.Width, frame.Frame.Height, false, Transparency);
                        }
                        d.DrawBitmap(new Classes.Editor.Draw.GraphicsHandler.GraphicsInfo(frameGush), x + frameGush.Frame.PivotX - length * frame.Frame.Width, y + frameGush.Frame.PivotY,
                            frameGush.Frame.Width, frameGush.Frame.Height, false, Transparency);
                        break;
                    case 3:
                        for (int i = -length + 1; i <= 0; ++i)
                        {
                            d.DrawBitmap(new Classes.Editor.Draw.GraphicsHandler.GraphicsInfo(frame), x + frame.Frame.PivotX, y + frame.Frame.PivotY + frame.Frame.Height - i * frame.Frame.Height,
                                frame.Frame.Width, frame.Frame.Height, false, Transparency);
                        }
                        d.DrawBitmap(new Classes.Editor.Draw.GraphicsHandler.GraphicsInfo(frameGush), x + frameGush.Frame.PivotX, y + frameGush.Frame.PivotY + length * frame.Frame.Height,
                            frameGush.Frame.Width, frameGush.Frame.Height, false, Transparency);
                        break;
                }

            }
        }

        public override string GetObjectName()
        {
            return "WaterGush";
        }
    }
}
