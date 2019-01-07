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
    public class LRZFireball : EntityRenderer
    {

        public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            bool fliph = false;
            bool flipv = false;
            int rotation = (int)(entity.attributesMap["rotation"].ValueInt32 / 1.42);

            var editorAnimFocus = e.EditorInstance.EditorEntity_ini.LoadAnimation2("LRZFireball", d, 0, 0, fliph, flipv, false);
            var editorAnim = e.EditorInstance.EditorEntity_ini.LoadAnimation2("LRZFireball", d, 1, 0, fliph, flipv, false, rotation);


            //Duds
            var editorAnim2 = e.EditorInstance.EditorEntity_ini.LoadAnimation2("LRZFireball", d, 0, 2, fliph, flipv, false, rotation);
            var editorAnim3 = e.EditorInstance.EditorEntity_ini.LoadAnimation2("LRZFireball", d, 0, 3, fliph, flipv, false, rotation);
            var editorAnim4 = e.EditorInstance.EditorEntity_ini.LoadAnimation2("LRZFireball", d, 0, 4, fliph, flipv, false, rotation);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnim2 != null && editorAnim2.Frames.Count != 0 && editorAnim3 != null && editorAnim3.Frames.Count != 0 && editorAnim4 != null && editorAnim4.Frames.Count != 0 && editorAnimFocus != null)
            {
                var frame = editorAnim.Frames[0];
                //var frameFocus = editorAnimFocus.Frames[0];
                //var frame2 = editorAnim2.Frames[0];
                //var frame3 = editorAnim3.Frames[0];
                //var frame4 = editorAnim4.Frames[0];

                //Animation.ProcessAnimation(framePropel.Entry.FrameSpeed, framePropel.Entry.Frames.Count, framePropel.Frame.Duration, 5);

                /*d.DrawBitmap(frameFocus.Texture,
                    x + frameFocus.Frame.CenterX,
                    y + frameFocus.Frame.CenterY,
                    frameFocus.Frame.Width, frameFocus.Frame.Height, false, Transparency);*/

                d.DrawBitmap(frame.Texture,
                    x + frame.Frame.CenterX,
                    y + frame.Frame.CenterY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);

            }
        }

        public override string GetObjectName()
        {
            return "LRZFireball";
        }
    }
}
