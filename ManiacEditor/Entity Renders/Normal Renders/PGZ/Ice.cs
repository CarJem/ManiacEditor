using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class Ice : EntityRenderer
    {
        ItemBox itemBox = new ItemBox();
        Spikes spikes = new Spikes();
        IceSpring iceSpring = new IceSpring();
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
            int type = (int)entity.attributesMap["type"].ValueUInt8;
            int size = (int)entity.attributesMap["size"].ValueUInt8;
            bool fliph = false;
            bool flipv = false;
            int animID = 0;
            int frameID = 0;
            switch (size)
            {
                case 0:
                    animID = 0;
                    frameID = 0;
                    break;
                case 1:
                    animID = 0;
                    frameID = 1;
                    break;
            }
            var editorAnim = Controls.Editor.MainEditor.Instance.EntityDrawing.LoadAnimation2("Ice", d.DevicePanel, animID, frameID, fliph, flipv, false);
            var editorAnimContents = Controls.Editor.MainEditor.Instance.EntityDrawing.LoadAnimation2("Ice", d.DevicePanel, 9, -1, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnimContents != null && editorAnimContents.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[animID];
                var frameContents = editorAnimContents.Frames[Animation.index];

                switch (type)
                {
                    case 0:
                        break;
                    case 1:
                        frameContents = editorAnimContents.Frames[0];
                        d.DrawBitmap(new Classes.Editor.Draw.GraphicsHandler.GraphicsInfo(frameContents),
                            x + frameContents.Frame.PivotX - (fliph ? (frameContents.Frame.Width - editorAnimContents.Frames[0].Frame.Width) : 0),
                            y + frameContents.Frame.PivotY + (flipv ? (frameContents.Frame.Height - editorAnimContents.Frames[0].Frame.Height) : 0),
                            frameContents.Frame.Width, frameContents.Frame.Height, false, Transparency);
                        break;
                    case 2:
                        frameContents = editorAnimContents.Frames[1];
                        d.DrawBitmap(new Classes.Editor.Draw.GraphicsHandler.GraphicsInfo(frameContents),
                            x + frameContents.Frame.PivotX - (fliph ? (frameContents.Frame.Width - editorAnimContents.Frames[0].Frame.Width) : 0),
                            y + frameContents.Frame.PivotY + (flipv ? (frameContents.Frame.Height - editorAnimContents.Frames[0].Frame.Height) : 0),
                            frameContents.Frame.Width, frameContents.Frame.Height, false, Transparency);
                        break;
                    case 3:
                        frameContents = editorAnimContents.Frames[2];
                        d.DrawBitmap(new Classes.Editor.Draw.GraphicsHandler.GraphicsInfo(frameContents),
                            x + frameContents.Frame.PivotX - (fliph ? (frameContents.Frame.Width - editorAnimContents.Frames[0].Frame.Width) : 0),
                            y + frameContents.Frame.PivotY + (flipv ? (frameContents.Frame.Height - editorAnimContents.Frames[0].Frame.Height) : 0),
                            frameContents.Frame.Width, frameContents.Frame.Height, false, Transparency);
                        break;
                    case 4:
                        spikes.IceDraw(d, entity, e, x, y, Transparency);
                        break;
                    case 5:
                        itemBox.IceDraw(d, entity, e, x, y, Transparency, 0);
                        break;
                    case 6:
                        itemBox.IceDraw(d, entity, e, x, y , Transparency, 1);
                        break;
                    case 7:
                        itemBox.IceDraw(d, entity, e, x, y, Transparency, 2);
                        break;
                    case 8:
                        itemBox.IceDraw(d, entity, e, x, y, Transparency, 3);
                        break;
                    case 9:
                        itemBox.IceDraw(d, entity, e, x, y, Transparency, 4);
                        break;
                    case 10:
                        itemBox.IceDraw(d, entity, e, x, y, Transparency, 5);
                        break;
                    case 11:
                        itemBox.IceDraw(d, entity, e, x, y, Transparency, 6);
                        break;
                    case 12:
                        itemBox.IceDraw(d, entity, e, x, y, Transparency, 7);
                        break;
                    case 13:
                        itemBox.IceDraw(d, entity, e, x, y, Transparency, 10);
                        break;
                    case 14:
                        itemBox.IceDraw(d, entity, e, x, y, Transparency, 11);
                        break;
                    case 15:
                        itemBox.IceDraw(d, entity, e, x, y, Transparency, 12);
                        break;
                    case 16:
                        itemBox.IceDraw(d, entity, e, x, y, Transparency, 13);
                        break;
                    case 17:
                        itemBox.IceDraw(d, entity, e, x, y, Transparency, 14);
                        break;
                    case 18:
                        iceSpring.IceDraw(d, entity, e, x, y, Transparency);
                        break;
                    case 19:
                        itemBox.IceDraw(d, entity, e, x, y, Transparency, 16);
                        break;
                    case 20:
                        itemBox.IceDraw(d, entity, e, x, y, Transparency, 17);
                        break;
                    default:
                        itemBox.IceDraw(d, entity, e, x, y, Transparency, -1);
                        break;
                }

                d.DrawBitmap(new Classes.Editor.Draw.GraphicsHandler.GraphicsInfo(frame),
                    x + frame.Frame.PivotX - (fliph ? (frame.Frame.Width - editorAnim.Frames[0].Frame.Width) : 0),
                    y + frame.Frame.PivotY + (flipv ? (frame.Frame.Height - editorAnim.Frames[0].Frame.Height) : 0),
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }
        }

        public override string GetObjectName()
        {
            return "Ice";
        }
    }
}
