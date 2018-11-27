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
    public class InvisibleBlock : EntityRenderer
    {

        public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            var width = (int)(entity.attributesMap["width"].ValueUInt8);
            var height = (int)(entity.attributesMap["height"].ValueUInt8);
            var editorAnim = EditorEntity_ini.LoadAnimation2("ItemBox", d, 2, 10, false, false, false);

                if (editorAnim != null && editorAnim.Frames.Count != 0)
                {
                    var frame = editorAnim.Frames[Animation.index];
                    Animation.ProcessAnimation(frame.Entry.FrameSpeed, frame.Entry.Frames.Count, frame.Frame.Duration);
                    bool wEven = width % 2 == 0;
                    bool hEven = height % 2 == 0;
                    for (int xx = 0; xx <= width; ++xx)
                    {
                        for (int yy = 0; yy <= height; ++yy)
                        {
                            d.DrawBitmap(frame.Texture,
                                x + (wEven ? frame.Frame.CenterX : -frame.Frame.Width) + (-width / 2 + xx) * frame.Frame.Width,
                                y + (hEven ? frame.Frame.CenterY : -frame.Frame.Height) + (-height / 2 + yy) * frame.Frame.Height,
                                frame.Frame.Width, frame.Frame.Height, false, Transparency);
                        }
                    }
                }

        }

        public override string GetObjectName()
        {
            return "InvisibleBlock";
        }
    }
}
