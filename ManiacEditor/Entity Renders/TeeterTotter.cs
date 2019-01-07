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
    public class TeeterTotter : EntityRenderer
    {

        public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            var value = entity.attributesMap["length"].ValueUInt32 / 2;
            var editorAnim = e.EditorInstance.EditorEntity_ini.LoadAnimation2("TeeterTotter", d, 0, 0, false, false, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[0];
                for (int i = -(int)value; i < value + 1; ++i)
                {
                    d.DrawBitmap(frame.Texture, x + frame.Frame.CenterX + (i * (frame.Frame.Width + 2)),
                        y + frame.Frame.CenterY, frame.Frame.Width, frame.Frame.Height, false, Transparency);
                }
            }
        }

        public override string GetObjectName()
        {
            return "TeeterTotter";
        }
    }
}
