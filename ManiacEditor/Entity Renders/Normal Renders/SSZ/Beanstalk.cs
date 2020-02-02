using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class Beanstalk : EntityRenderer
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
            int direction = (int)entity.attributesMap["direction"].ValueUInt8;
            int type = (int)entity.attributesMap["type"].ValueUInt8;
            int animID = 0;
            int frameID = 0;
            bool plantType = false;
            switch (type)
            {
                case 0:
                    animID = 4;
                    frameID = 9;
                    break;
                case 1:
                    animID = 0;
                    frameID = 0;
                    break;
                case 2:
                    animID = 2;
                    frameID = 2;
                    break;
                case 3:
                    animID = 3;
                    frameID = -1;
                    plantType = true;
                    break;
            }
            bool fliph = false;
            bool flipv = false;
            if (direction == 1)
            {
                fliph = true;
            }
            var editorAnimNode = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("Beanstalk", d.DevicePanel, 0, 0, fliph, flipv, false);
            var editorAnimType = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("Beanstalk", d.DevicePanel, animID, frameID, fliph, flipv, false);
            if (editorAnimNode != null && editorAnimNode.Frames.Count != 0 && editorAnimType != null && editorAnimType.Frames.Count != 0)
            {
                var frame = editorAnimNode.Frames[0];
                var frameHead = editorAnimType.Frames[0];

                if (plantType == true)
                {
                    frameHead = editorAnimType.Frames[Animation.index];
                    Animation.ProcessAnimation(frameHead.Entry.SpeedMultiplyer, frameHead.Entry.Frames.Count, frameHead.Frame.Delay);
                }


                d.DrawBitmap(new Classes.Editor.Draw.GraphicsHandler.GraphicsInfo(frame),
                    x + frame.Frame.PivotX - (fliph ? (frame.Frame.Width - editorAnimNode.Frames[0].Frame.Width) : 0),
                    y + frame.Frame.PivotY + (flipv ? (frame.Frame.Height - editorAnimNode.Frames[0].Frame.Height) : 0),
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
                d.DrawBitmap(new Classes.Editor.Draw.GraphicsHandler.GraphicsInfo(frameHead),
                    x + (plantType ? (fliph ? frameHead.Frame.PivotX*2 : frameHead.Frame.PivotX) : (fliph ? -frameHead.Frame.Width : frameHead.Frame.PivotX)),
                    y + frameHead.Frame.PivotY,
                    frameHead.Frame.Width, frameHead.Frame.Height, false, Transparency);


            }
        }

        public override string GetObjectName()
        {
            return "Beanstalk";
        }
    }
}
