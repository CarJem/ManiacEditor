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
    public class Honkytonk : EntityRenderer
    {

        public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            bool fliph = false;
            bool flipv = false;
            int angle = (int)entity.attributesMap["angle"].ValueVar;
            int rotation = (int)(angle / -0.71);
            int offsetX = (int)Math.Cos(angle / -0.71);
            int offsetY = (int)Math.Sin(angle / -0.71);


            var editorAnim = EditorEntity_ini.LoadAnimation2("HonkyTonk", d, 0, 1, fliph, flipv, false, rotation);
            if (editorAnim != null && editorAnim.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[0];

                d.DrawBitmap(frame.Texture,
                    x - frame.Frame.Width / 2,
                    y - frame.Frame.Height / 2,
                    frame.Frame.Width*3, frame.Frame.Height*3, false, Transparency);

            }
        }

        public override string GetObjectName()
        {
            return "Honkytonk";
        }
    }
}
