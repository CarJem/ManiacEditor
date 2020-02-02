using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class Decoration : EntityRenderer
    {

        public override void Draw(Classes.Core.Draw.GraphicsHandler d, SceneEntity entity, Classes.Core.Scene.Sets.EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
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
            
            var editorAnim = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("Decoration", d.DevicePanel, type, -1, fliph, flipv, false);
            if (type == 2)
            {
                editorAnim = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("Decoration", d.DevicePanel, type, -1, fliph, flipv, false);
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
                        d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(frame), (x + frame.Frame.RelCenterX(fliph) + offsetX * xx) - (offsetX * repeatX / 2), (y + frame.Frame.RelCenterY(flipv) + offsetY * yy) - (offsetY * repeatY / 2),
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
