using System;
using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class PimPom : EntityRenderer
    {
        Boolean boolState = true;
        public override void Draw(Structures.EntityRenderProp Properties)
        {
            DevicePanel d = Properties.Graphics;

            Classes.Scene.EditorEntity e = Properties.EditorObject;
            int x = Properties.DrawX;
            int y = Properties.DrawY;
            int Transparency = Properties.Transparency;

            bool fliph = false;
            bool flipv = false;

            int type = (int)e.attributesMap["type"].ValueEnum;
            int color = (int)e.attributesMap["color"].ValueUInt8;
            int direction = (int)e.attributesMap["direction"].ValueUInt8;
            int angle = (int)e.attributesMap["angle"].ValueInt32;
            int length = (int)e.attributesMap["length"].ValueUInt8;
            int gap = (int)e.attributesMap["gap"].ValueUInt8;
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
            var Animation = LoadAnimation("PimPom", d, animID, frameID);
            if (length != 0 && angle == 0) DrawAngle0(d, Animation, x, y, Transparency, fliph, flipv, length, gap);
            else if (length != 0 && angle == 3) DrawAngle3(d, Animation, x, y, Transparency, fliph, flipv, length);
            else if (length != 0 && angle == 1) DrawAngle1(d, Animation, x, y, Transparency, fliph, flipv, length);
            else if (length != 0 && angle == 2) DrawAngle2(d, Animation, x, y, Transparency, fliph, flipv, length, gap);
            else DrawDefault(d, Animation, x, y, Transparency, fliph, flipv);
        }


        private void DrawDefault(DevicePanel d, Methods.Drawing.ObjectDrawing.EditorAnimation Frame, int x, int y, int Transparency, bool fliph, bool flipv)
        {
            DrawTexturePivotNormal(d, Frame, Frame.RequestedAnimID, Frame.RequestedFrameID, x, y, Transparency, fliph, flipv);
        }

        private void DrawAngle0(DevicePanel d, Methods.Drawing.ObjectDrawing.EditorAnimation Frame, int x, int y, int Transparency, bool fliph, bool flipv, int value, int gap)
        {
            bool wEven = value % 2 == 0;
            for (int xx = 0; xx <= value; ++xx)
            {
                int posX = (x - 3) + (wEven ? Frame.RequestedFrame.PivotX : -Frame.RequestedFrame.Width) + (-value / 2 + xx) * (Frame.RequestedFrame.Width + (gap * 2));
                int posY = y + Frame.RequestedFrame.PivotY;
                DrawTexture(d, Frame, Frame.RequestedAnimID, Frame.RequestedFrameID, posX, posY, Transparency, fliph, flipv);
            }
        }

        private void DrawAngle1(DevicePanel d, Methods.Drawing.ObjectDrawing.EditorAnimation Frame, int x, int y, int Transparency, bool fliph, bool flipv, int value)
        {
            bool wEven = value % 2 == 0;
            for (int xx = 0; xx <= value; ++xx)
            {
                int posX = (x - Frame.RequestedFrame.Width) - (wEven ? Frame.RequestedFrame.PivotX : -Frame.RequestedFrame.Width) - (-value / 2 + xx) * (Frame.RequestedFrame.Width);
                int posY = y + (wEven ? Frame.RequestedFrame.PivotY : -Frame.RequestedFrame.Height) + (-value / 2 + xx) * (Frame.RequestedFrame.Height - 2);
                DrawTexture(d, Frame, Frame.RequestedAnimID, Frame.RequestedFrameID, posX, posY, Transparency, fliph, flipv);
            }
        }

        private void DrawAngle2(DevicePanel d, Methods.Drawing.ObjectDrawing.EditorAnimation Frame, int x, int y, int Transparency, bool fliph, bool flipv, int value, int gap)
        {
            bool wEven = value % 2 == 0;
            wEven = boolState;
            for (int xx = 0; xx <= value; ++xx)
            {
                int posX = (x + 3) + Frame.RequestedFrame.PivotX;
                int posY = y + (wEven ? Frame.RequestedFrame.PivotY : -Frame.RequestedFrame.Height) + (-value / 2 + xx) * (Frame.RequestedFrame.Height + (gap * 2));
                DrawTexture(d, Frame, Frame.RequestedAnimID, Frame.RequestedFrameID, posX, posY, Transparency, fliph, flipv);
            }
        }

        private void DrawAngle3(DevicePanel d, Methods.Drawing.ObjectDrawing.EditorAnimation Frame, int x, int y, int Transparency, bool fliph, bool flipv, int value)
        {
            bool wEven = value % 2 == 0;
            for (int xx = 0; xx <= value; ++xx)
            {
                int posX = x + (wEven ? Frame.RequestedFrame.PivotX : -Frame.RequestedFrame.Width) + (-value / 2 + xx) * (Frame.RequestedFrame.Width);
                int posY = y + (wEven ? Frame.RequestedFrame.PivotY : -Frame.RequestedFrame.Height) + (-value / 2 + xx) * (Frame.RequestedFrame.Height);
                DrawTexture(d, Frame, Frame.RequestedAnimID, Frame.RequestedFrameID, posX, posY, Transparency, fliph, flipv);
            }
        }

        public override bool isObjectOnScreen(DevicePanel d, Classes.Scene.EditorEntity entity, int x, int y, int Transparency)
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
