using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class BallCannon : EntityRenderer
    {

        public override void Draw(GraphicsHandler d, SceneEntity entity, Classes.Editor.Scene.Sets.EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            int x2 = x;
            int y2 = y;
            bool flipv = false;
            bool fliph = false;
            int angle = (int)entity.attributesMap["angle"].ValueEnum;
            int type = (int)entity.attributesMap["type"].ValueUInt8;
            int rotation = 0;
            int rotation2 = 0;
            int CorkState = 0;
            if (type == 0)
            {
                switch (angle)
                {
                    case 0:
                        rotation = 90;
                        rotation2 = 180;
                        y += 16;
                        x += 8;
                        y2 += 24;
                        x2 += 0;
                        break;
                    case 1:
                        rotation = 180;
                        rotation2 = 270;
                        y += 24;
                        x -= 0;
                        y2 += 16;
                        x2 += -8;
                        break;
                    case 2:
                        rotation = 270;
                        y += 16;
                        x -= 8;
                        y2 += 8;
                        x2 += 0;
                        break;
                    case 3:
                        rotation2 = 90;
                        y += 8;
                        x += 0;
                        y2 += 16;
                        x2 += 8;
                        break;
                    case 4:
                        rotation2 = 90;
                        rotation = 180;
                        y += 24;
                        x += 0;
                        y2 += 16;
                        x2 += 8;
                        break;
                    case 5:
                        rotation = 270;
                        rotation2 = 180;
                        y += 16;
                        x -= 8;
                        y2 += 24;
                        x2 += 0;
                        break;
                    case 6:
                        rotation2 = 270;
                        y += 8;
                        x += 0;
                        y2 += 16;
                        x2 -= 8;
                        break;
                    case 7:
                        rotation2 = 0;
                        rotation = 90;
                        y += 16;
                        x += 8;
                        y2 += 8;
                        x2 += 0;
                        break;
                    default:
                        rotation = 90;
                        rotation2 = 180;
                        y += 16;
                        x += 8;
                        y2 += 24;
                        x2 += 0;
                        break;
                }
            }
            else
            {
                switch (type)
                {
                    case 0:
                        break;
                    case 1:
                        CorkState = 3;
                        break;
                    case 2:
                        CorkState = 4;
                        break;
                }
            }

            var editorAnim = Interfaces.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("BallCannon", d.DevicePanel, 0, -1, fliph, flipv, true, rotation);
            var editorAnimHolo = Interfaces.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("BallCannon", d.DevicePanel, 0, -1, fliph, flipv, true, rotation2);
            var editorAnimCork = Interfaces.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("BallCannon", d.DevicePanel, CorkState, 0, fliph, flipv, true);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnimHolo != null && editorAnimHolo.Frames.Count != 0 && editorAnimCork != null && editorAnimCork.Frames.Count != 0)
            {
                if (type == 1)
                {
                    var frame = editorAnimCork.Frames[0];

                    d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame),
                        x + frame.Frame.PivotX,
                        y + frame.Frame.PivotY,
                        frame.Frame.Height, frame.Frame.Height, false, Transparency);
                }
                else if (type == 2)
                {
                    var frame = editorAnimCork.Frames[0];

                    d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame),
                        x + frame.Frame.PivotX,
                        y + frame.Frame.PivotY,
                        frame.Frame.Height, frame.Frame.Height, false, Transparency);
                }
                else
                {
                    var frame = editorAnim.Frames[Animation.index];
                    var frame3 = editorAnimHolo.Frames[Animation.index];

                    if (selected)
                    {
                        d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame3),
                            x2 + frame3.Frame.PivotX,
                            y2 + frame3.Frame.PivotY,
                            frame3.Frame.Height, frame3.Frame.Height, false, Transparency - 125);
                    }

                    d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame),
                        x + frame.Frame.PivotX,
                        y + frame.Frame.PivotY,
                        frame.Frame.Height, frame.Frame.Height, false, Transparency);
                }


            }
        }

        public override string GetObjectName()
        {
            return "BallCannon";
        }
    }
}
