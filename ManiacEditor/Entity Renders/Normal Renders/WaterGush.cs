using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ManiacEditor;
using Microsoft.Xna.Framework;
using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class WaterGush : EntityRenderer
    {

        public override void Draw(GraphicsHandler d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
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
            var editorAnim = Editor.Instance.EntityDrawing.LoadAnimation2("WaterGush", d.DevicePanel, animID, -1, fliph, flipv, false);
            var editorAnimGush = Editor.Instance.EntityDrawing.LoadAnimation2("WaterGush", d.DevicePanel, animID2, -1, false, false, false);
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
                            d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame), x + frame.Frame.PivotX, y + frame.Frame.PivotY + i * frame.Frame.Height,
                                frame.Frame.Width, frame.Frame.Height, false, Transparency);
                        }
                        d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frameGush), x + frameGush.Frame.PivotX, y + frameGush.Frame.PivotY - length * frame.Frame.Height,
                            frameGush.Frame.Width, frameGush.Frame.Height, false, Transparency);
                        break;
                    case 1:
                        for (int i = -length + 1; i <= 0; ++i)
                        {
                            d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame), x - frame.Frame.PivotX - i * frame.Frame.Width, y + frame.Frame.PivotY,
                                frame.Frame.Width, frame.Frame.Height, false, Transparency);
                        }
                        d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frameGush), x + frameGush.Frame.PivotX + length * frame.Frame.Width, y + frameGush.Frame.PivotY,
                            frameGush.Frame.Width, frameGush.Frame.Height, false, Transparency);
                        break;
                    case 2:
                        for (int i = 0; i < length; ++i)
                        {
                            d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame), x + frame.Frame.PivotX - frame.Frame.Width - i * frame.Frame.Width, y + frame.Frame.PivotY,
                                frame.Frame.Width, frame.Frame.Height, false, Transparency);
                        }
                        d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frameGush), x + frameGush.Frame.PivotX - length * frame.Frame.Width, y + frameGush.Frame.PivotY,
                            frameGush.Frame.Width, frameGush.Frame.Height, false, Transparency);
                        break;
                    case 3:
                        for (int i = -length + 1; i <= 0; ++i)
                        {
                            d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame), x + frame.Frame.PivotX, y + frame.Frame.PivotY + frame.Frame.Height - i * frame.Frame.Height,
                                frame.Frame.Width, frame.Frame.Height, false, Transparency);
                        }
                        d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frameGush), x + frameGush.Frame.PivotX, y + frameGush.Frame.PivotY + length * frame.Frame.Height,
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
