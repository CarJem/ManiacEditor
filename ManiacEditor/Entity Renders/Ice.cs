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
    public class Ice : EntityRenderer
    {
        ItemBox itemBox = new ItemBox();
        Spikes spikes = new Spikes();
        IceSpring iceSpring = new IceSpring();
        public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
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
            var editorAnim = EditorEntity_ini.LoadAnimation2("Ice", d, animID, frameID, fliph, flipv, false);
            var editorAnimContents = EditorEntity_ini.LoadAnimation2("Ice", d, 9, -1, fliph, flipv, false);
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
                        d.DrawBitmap(frameContents.Texture,
                            x + frameContents.Frame.CenterX - (fliph ? (frameContents.Frame.Width - editorAnimContents.Frames[0].Frame.Width) : 0),
                            y + frameContents.Frame.CenterY + (flipv ? (frameContents.Frame.Height - editorAnimContents.Frames[0].Frame.Height) : 0),
                            frameContents.Frame.Width, frameContents.Frame.Height, false, Transparency);
                        break;
                    case 2:
                        frameContents = editorAnimContents.Frames[1];
                        d.DrawBitmap(frameContents.Texture,
                            x + frameContents.Frame.CenterX - (fliph ? (frameContents.Frame.Width - editorAnimContents.Frames[0].Frame.Width) : 0),
                            y + frameContents.Frame.CenterY + (flipv ? (frameContents.Frame.Height - editorAnimContents.Frames[0].Frame.Height) : 0),
                            frameContents.Frame.Width, frameContents.Frame.Height, false, Transparency);
                        break;
                    case 3:
                        frameContents = editorAnimContents.Frames[2];
                        d.DrawBitmap(frameContents.Texture,
                            x + frameContents.Frame.CenterX - (fliph ? (frameContents.Frame.Width - editorAnimContents.Frames[0].Frame.Width) : 0),
                            y + frameContents.Frame.CenterY + (flipv ? (frameContents.Frame.Height - editorAnimContents.Frames[0].Frame.Height) : 0),
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

                d.DrawBitmap(frame.Texture,
                    x + frame.Frame.CenterX - (fliph ? (frame.Frame.Width - editorAnim.Frames[0].Frame.Width) : 0),
                    y + frame.Frame.CenterY + (flipv ? (frame.Frame.Height - editorAnim.Frames[0].Frame.Height) : 0),
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }
        }

        public override string GetObjectName()
        {
            return "Ice";
        }
    }
}
