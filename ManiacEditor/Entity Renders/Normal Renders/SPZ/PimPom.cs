using System;
using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class PimPom : EntityRenderer
    {
        Boolean boolState = true;
        public override void Draw(GraphicsHandler d, SceneEntity entity, Classes.Edit.Scene.Sets.EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            int type = (int)entity.attributesMap["type"].ValueEnum;
            int color = (int)entity.attributesMap["color"].ValueUInt8;
            int direction = (int)entity.attributesMap["direction"].ValueUInt8;
            int angle = (int)entity.attributesMap["angle"].ValueInt32;
            int length = (int)entity.attributesMap["length"].ValueUInt8;
            int gap = (int)entity.attributesMap["gap"].ValueUInt8;
            bool fliph = false;
            bool flipv = false;
            int animID;
            int frameID;
            switch (type)
            {
                case 0:
                    animID = 0;
                    switch (color)
                    {
                        case 0:
                            frameID = 0;
                            break;
                        case 1:
                            frameID = 1;
                            break;
                        case 2:
                            frameID = 2;
                            break;
                        default:
                            frameID = 0;
                            break;
                    }
                    break;
                case 1:
                    animID = 1;
                    frameID = 0;
                    break;
                case 2:
                    animID = 2;
                    frameID = 0;
                    break;
                case 3:
                    animID = 3;
                    frameID = 0;
                    break;
                default:
                    animID = 0;
                    frameID = 0;
                    break;

            }
            if (direction == 1)
            {
                fliph = true;
            }
            switch (angle)
            {
                case 0:
                    break;
                case 1:
                    animID = 2;
                    fliph = true;
                    break;
                case 2:
                    animID = 3;
                    flipv = true;
                    break;
                case 3:
                    animID = 2;
                    flipv = true;
                    fliph = true;
                    break;
                default:
                    break;
            }
            var editorAnim = Editor.Instance.EntityDrawing.LoadAnimation2("PimPom", d.DevicePanel, animID, frameID, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && animID >= 0)
            {
                var frame = editorAnim.Frames[Animation.index];

                //Animation.ProcessAnimation(frame.Entry.SpeedMultiplyer, frame.Entry.Frames.Count, frame.Frame.Delay);
                if (length != 0 && angle == 0)
                {
                    var value = entity.attributesMap["length"].ValueUInt8;
                    if (editorAnim != null && editorAnim.Frames.Count != 0)
                    {
                        bool wEven = value % 2 == 0;
                        for (int xx = 0; xx <= value; ++xx)
                        {
                            d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame),
                                (x - 3) + (wEven ? frame.Frame.PivotX : -frame.Frame.Width) + (-value / 2 + xx) * (frame.Frame.Width + (gap*2)),
                                y + frame.Frame.PivotY,
                                frame.Frame.Width, frame.Frame.Height, false, Transparency);
                        }
                    }
                }
                else if (length != 0 && angle == 3)
                {
                    var value = entity.attributesMap["length"].ValueUInt8;
                    if (editorAnim != null && editorAnim.Frames.Count != 0)
                    {
                        bool wEven = value % 2 == 0;
                        for (int xx = 0; xx <= value; ++xx)
                        {
                            d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame),
                                x + (wEven ? frame.Frame.PivotX : -frame.Frame.Width) + (-value / 2 + xx) * (frame.Frame.Width),
                                y + (wEven ? frame.Frame.PivotY : -frame.Frame.Height) + (-value / 2 + xx) * (frame.Frame.Height),
                                frame.Frame.Width, frame.Frame.Height, false, Transparency);
                        }
                    }
                }
                else if (length != 0 && angle == 1)
                {
                    var value = entity.attributesMap["length"].ValueUInt8;
                    if (editorAnim != null && editorAnim.Frames.Count != 0)
                    {
                        bool wEven = value % 2 == 0;
                        for (int xx = 0; xx <= value; ++xx)
                        {
                            d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame),
                                (x - frame.Frame.Width) - (wEven ? frame.Frame.PivotX : -frame.Frame.Width) - (-value / 2 + xx) * (frame.Frame.Width),
                                y + (wEven ? frame.Frame.PivotY : -frame.Frame.Height) + (-value / 2 + xx) * (frame.Frame.Height - 2),
                                frame.Frame.Width, frame.Frame.Height, false, Transparency);
                        }
                    }
                }
                else if (length != 0 && angle == 2)
                {
                    var value = entity.attributesMap["length"].ValueUInt8;
                    if (editorAnim != null && editorAnim.Frames.Count != 0)
                    {
                        bool wEven = value % 2 == 0;
                        wEven = boolState;
                        for (int xx = 0; xx <= value; ++xx)
                        {
                            d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame),
                                (x + 3) + frame.Frame.PivotX,
                                y + (wEven ? frame.Frame.PivotY : -frame.Frame.Height) + (-value / 2 + xx) * (frame.Frame.Height + (gap * 2)),
                                frame.Frame.Width, frame.Frame.Height, false, Transparency);
                        }
                    }
                }
                else {
                    d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame),
                        x + frame.Frame.PivotX - (fliph ? (frame.Frame.Width - editorAnim.Frames[0].Frame.Width) : 0),
                        y + frame.Frame.PivotY + (flipv ? (frame.Frame.Height - editorAnim.Frames[0].Frame.Height) : 0),
                        frame.Frame.Width, frame.Frame.Height, false, Transparency);
                }


            }
        }

        public override bool isObjectOnScreen(GraphicsHandler d, SceneEntity entity, Classes.Edit.Scene.Sets.EditorEntity e, int x, int y, int Transparency)
        {
            int length = (int)entity.attributesMap["length"].ValueUInt8 + 1;
            int gap = (int)entity.attributesMap["gap"].ValueUInt8;
            int bounds = (48 * length+gap);

            return d.IsObjectOnScreen(x - bounds / 2, y - bounds / 2, bounds, bounds);
        }

        public override string GetObjectName()
        {
            return "PimPom";
        }
    }
}
