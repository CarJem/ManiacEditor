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


            var editorAnim = e.EditorInstance.EditorEntity_ini.LoadAnimation2("HonkyTonk", d, 0, 1, fliph, flipv, true, rotation);
            var editorAnim2 = e.EditorInstance.EditorEntity_ini.LoadAnimation2("HonkyTonk", d, 0, 2, fliph, flipv, true, rotation);
            var editorAnim3 = e.EditorInstance.EditorEntity_ini.LoadAnimation2("HonkyTonk", d, 0, 3, fliph, flipv, true, rotation);
            var editorAnim4 = e.EditorInstance.EditorEntity_ini.LoadAnimation2("HonkyTonk", d, 0, 4, fliph, flipv, true, rotation);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnim2 != null && editorAnim2.Frames.Count != 0 && editorAnim3 != null && editorAnim3.Frames.Count != 0 && editorAnim4 != null && editorAnim4.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[0];
                var frame2 = editorAnim2.Frames[0];
                var frame3 = editorAnim3.Frames[0];
                var frame4 = editorAnim4.Frames[0];


                d.DrawBitmap(frame2.Texture,
                    x - frame2.ImageWidth/2,
                    y - frame2.ImageHeight/2,
                    frame2.ImageWidth, frame2.ImageHeight, false, Transparency);
                d.DrawBitmap(frame.Texture,
                    x - frame.ImageWidth/2,
                    y - frame.ImageHeight/2,
                    frame.ImageWidth, frame.ImageHeight, false, Transparency);

            }
        }

        public override string GetObjectName()
        {
            return "Honkytonk";
        }
    }
}
