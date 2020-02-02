using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class FlameSpring : EntityRenderer
    {

        public override void Draw(Structures.EntityRenderProp properties)
        {
            Classes.Core.Draw.GraphicsHandler d = properties.Graphics;
            SceneEntity entity = properties.Object; 
            Classes.Core.Scene.Sets.EditorEntity e = properties.EditorObject;
            int x = properties.X;
            int y = properties.Y;
            int Transparency = properties.Transparency;
            int index = properties.Index;
            int previousChildCount = properties.PreviousChildCount;
            int platformAngle = properties.PlatformAngle;
            Methods.Entities.EntityAnimator Animation = properties.Animations;
            bool selected  = properties.isSelected;
            int type = (int)entity.attributesMap["type"].ValueEnum;
            bool fliph = false;
            bool flipv = false;
            int valveType;
            int animID;
            switch (type)
            {
                case 0:
                    animID = 0;
                    valveType = 0;
                    break;
                case 1:
                    animID = 0;
                    valveType = 1;
                    break;
                case 2:
                    animID = 0;
                    valveType = 2;
                    break;
                case 3:
                    animID = 2;
                    valveType = 0;
                    break;
                case 4:
                    animID = 2;
                    valveType = 1;
                    break;
                case 5:
                    animID = 2;
                    valveType = 2;
                    break;
                default:
                    animID = 0;
                    valveType = 0;
                    break;
            }
            var editorAnim = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("FlameSpring", d.DevicePanel, 0, animID, fliph, flipv, false);
            var nozzelA = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("FlameSpring", d.DevicePanel, 1, 0, fliph, flipv, false);
            var nozzelB = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("FlameSpring", d.DevicePanel, 1, 1, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && nozzelA != null && nozzelA.Frames.Count != 0 && nozzelB != null && nozzelB.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[Animation.index];
                var headA = nozzelA.Frames[0];
                var headB = nozzelB.Frames[0];

                Animation.ProcessAnimation(frame.Entry.SpeedMultiplyer, frame.Entry.Frames.Count, frame.Frame.Delay);

                d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(frame),
                    x + frame.Frame.PivotX,
                    y + frame.Frame.PivotY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);

                if (valveType == 2 || valveType == 0)
                {
                    d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(headA),
                        x - 21 - headA.Frame.PivotX,
                        y - 12 - headA.Frame.PivotY,
                        headA.Frame.Width, headA.Frame.Height, false, Transparency);
                }
                if (valveType == 1 || valveType == 0)
                {
                    d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(headB),
                        x + 12 + headB.Frame.PivotX,
                        y - 12 - headB.Frame.PivotY,
                        headB.Frame.Width, headB.Frame.Height, false, Transparency);
                }
            }
        }

        public override string GetObjectName()
        {
            return "FlameSpring";
        }
    }
}
