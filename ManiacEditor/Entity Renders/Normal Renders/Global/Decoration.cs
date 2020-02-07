using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class Decoration : EntityRenderer
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
            bool flipv = false;
            bool fliph = false;
            var type = entity.attributesMap["type"].ValueUInt8;
            var direction = entity.attributesMap["direction"].ValueUInt8;
            var repeatSpacing = entity.attributesMap["repeatSpacing"].ValueVector2;
            var repeatTimes = entity.attributesMap["repeatTimes"].ValueVector2;
            var rotSpeed = entity.attributesMap["rotSpeed"].ValueEnum;

            int offsetX = (int)repeatSpacing.X.High;
            int repeatX = (int)repeatTimes.X.High;
            int offsetY = (int)repeatSpacing.Y.High;
            int repeatY = (int)repeatTimes.Y.High;

            switch (direction)
            {               
                case 0:
                    break;
                case 1:
                    fliph = true;
                    break;
                case 2:
                    flipv = true;
                    break;
                case 3:
                    flipv = true;
                    fliph = true;
                    break;
            }
            
            var editorAnim = Controls.Editor.MainEditor.Instance.EntityDrawing.LoadAnimation2("Decoration", d.DevicePanel, type, -1, fliph, flipv, false);
            if (type == 2)
            {
                editorAnim = Controls.Editor.MainEditor.Instance.EntityDrawing.LoadAnimation2("Decoration", d.DevicePanel, type, -1, fliph, flipv, false);
            }

            if (editorAnim != null && editorAnim.Frames.Count != 0)
            {
                if (index >= editorAnim.Frames.Count)
                    index = 0;
                var frame = editorAnim.Frames[Animation.index];
                Animation.ProcessAnimation(frame.Entry.SpeedMultiplyer, frame.Entry.Frames.Count, frame.Frame.Delay);

                for (int yy = 0; yy <= repeatY; yy++)
                {
                    for (int xx = 0; xx <= repeatX; xx++)
                    {
                        d.DrawBitmap(new Classes.Editor.Draw.GraphicsHandler.GraphicsInfo(frame), (x + frame.Frame.RelCenterX(fliph) + offsetX * xx) - (offsetX * repeatX / 2), (y + frame.Frame.RelCenterY(flipv) + offsetY * yy) - (offsetY * repeatY / 2),
                            frame.Frame.Width, frame.Frame.Height, false, Transparency);
                    }
                }

            }
        }

        public override string GetObjectName()
        {
            return "Decoration";
        }
    }
}
