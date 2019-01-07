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
    public class PopcornMachine : EntityRenderer
    {

        public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            //int type = (int)entity.attributesMap["type"].ValueUInt8;
            int type = (int)entity.attributesMap["type"].ValueUInt8;
            int height = (int)entity.attributesMap["height"].ValueUInt8;
            bool fliph = false;
            bool flipv = false;
            var editorAnim = e.EditorInstance.EditorEntity_ini.LoadAnimation2("PopcornMachine", d, 0, 0, fliph, flipv, false);
            var editorAnim2 = e.EditorInstance.EditorEntity_ini.LoadAnimation2("PopcornMachine", d, 0, 1, fliph, flipv, false);
            var editorAnim3 = e.EditorInstance.EditorEntity_ini.LoadAnimation2("PopcornMachine", d, 0, 2, false, false, false);
            var editorAnim4 = e.EditorInstance.EditorEntity_ini.LoadAnimation2("PopcornMachine", d, 0, 3, false, false, false);
            var editorAnim5 = e.EditorInstance.EditorEntity_ini.LoadAnimation2("PopcornMachine", d, 0, 6, false, false, false);
            var editorAnim6 = e.EditorInstance.EditorEntity_ini.LoadAnimation2("PopcornMachine", d, 0, 7, false, false, false);
            var editorAnim7 = e.EditorInstance.EditorEntity_ini.LoadAnimation2("PopcornMachine", d, 0, 4, false, false, false);
            var editorAnim8 = e.EditorInstance.EditorEntity_ini.LoadAnimation2("PopcornMachine", d, 0, 5, false, false, false);
            var editorAnim9 = e.EditorInstance.EditorEntity_ini.LoadAnimation2("PopcornMachine", d, 0, 9, false, false, false);
            var editorAnim10 = e.EditorInstance.EditorEntity_ini.LoadAnimation2("PopcornMachine", d, 0, 10, false, false, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnim2 != null && editorAnim2.Frames.Count != 0 && editorAnim3 != null && editorAnim3.Frames.Count != 0 && editorAnim4 != null && editorAnim4.Frames.Count != 0 && editorAnim5 != null && editorAnim5.Frames.Count != 0 && editorAnim6 != null && editorAnim6.Frames.Count != 0 && editorAnim7 != null && editorAnim7.Frames.Count != 0 && editorAnim8 != null && editorAnim8.Frames.Count != 0 && editorAnim9 != null && editorAnim9.Frames.Count != 0 && editorAnim10 != null && editorAnim10.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[Animation.index];
                var frame2 = editorAnim2.Frames[Animation.index];
                var frame3 = editorAnim3.Frames[Animation.index];
                var frame4 = editorAnim4.Frames[Animation.index];
                var frame5 = editorAnim5.Frames[Animation.index];
                var frame6 = editorAnim6.Frames[Animation.index];
                var frame7 = editorAnim7.Frames[Animation.index];
                var frame8 = editorAnim8.Frames[Animation.index];
                var frame9 = editorAnim9.Frames[Animation.index];
                var frame10 = editorAnim10.Frames[Animation.index];

                //ProcessAnimation(frame.Entry.FrameSpeed, frame.Entry.Frames.Count, frame.Frame.Duration);
                d.DrawBitmap(frame.Texture,
                        x + frame.Frame.CenterX - (fliph ? (frame.Frame.Width - editorAnim.Frames[0].Frame.Width) : 0),
                        y + frame.Frame.CenterY + (flipv ? (frame.Frame.Height - editorAnim.Frames[0].Frame.Height) : 0),
                        frame.Frame.Width, frame.Frame.Height, false, Transparency);
                d.DrawBitmap(frame2.Texture,
                        x + frame2.Frame.CenterX - (fliph ? (frame2.Frame.Width - editorAnim2.Frames[0].Frame.Width) : 0),
                        y + frame2.Frame.CenterY + (flipv ? (frame2.Frame.Height - editorAnim2.Frames[0].Frame.Height) : 0),
                        frame2.Frame.Width, frame2.Frame.Height, false, Transparency);
                if (type == 0 || type == 2)
                {
                    d.DrawBitmap(frame3.Texture,
                        x + frame3.Frame.CenterX - (fliph ? (frame3.Frame.Width - editorAnim3.Frames[0].Frame.Width) : 0),
                        y + frame3.Frame.CenterY + (flipv ? (frame3.Frame.Height - editorAnim3.Frames[0].Frame.Height) : 0),
                        frame3.Frame.Width, frame3.Frame.Height, false, Transparency);
                }
                if (type == 1 || type == 2)
                {
                    d.DrawBitmap(frame4.Texture,
                        x + frame4.Frame.CenterX - (fliph ? (frame4.Frame.Width - editorAnim3.Frames[0].Frame.Width) : 0),
                        y + frame4.Frame.CenterY + (flipv ? (frame4.Frame.Height - editorAnim3.Frames[0].Frame.Height) : 0),
                        frame4.Frame.Width, frame4.Frame.Height, false, Transparency);
                }
                for (int i = 0; i <= height; i++)
                {
                    d.DrawBitmap(frame5.Texture,
                        x + frame5.Frame.CenterX - (fliph ? (frame5.Frame.Width - editorAnim3.Frames[0].Frame.Width) : 0),
                        y - 208 - (i * 160) + frame5.Frame.CenterY + (flipv ? (frame5.Frame.Height - editorAnim3.Frames[0].Frame.Height) : 0),
                        frame5.Frame.Width, frame5.Frame.Height, false, Transparency);
                    d.DrawBitmap(frame6.Texture,
                        x + frame6.Frame.CenterX - (fliph ? (frame6.Frame.Width - editorAnim3.Frames[0].Frame.Width) : 0),
                        y - 208 - (i * 160) + frame6.Frame.CenterY + (flipv ? (frame6.Frame.Height - editorAnim3.Frames[0].Frame.Height) : 0),
                        frame6.Frame.Width, frame6.Frame.Height, false, Transparency);
                }
                d.DrawBitmap(frame7.Texture,
                    x + frame7.Frame.CenterX - (fliph ? (frame7.Frame.Width - editorAnim3.Frames[0].Frame.Width) : 0),
                    y - 208 - (height * 160) + frame7.Frame.CenterY + (flipv ? (frame7.Frame.Height - editorAnim3.Frames[0].Frame.Height) : 0),
                    frame7.Frame.Width, frame7.Frame.Height, false, Transparency);
                d.DrawBitmap(frame8.Texture,
                    x + frame8.Frame.CenterX - (fliph ? (frame8.Frame.Width - editorAnim3.Frames[0].Frame.Width) : 0),
                    y - 208 - (height * 160) + frame8.Frame.CenterY + (flipv ? (frame8.Frame.Height - editorAnim3.Frames[0].Frame.Height) : 0),
                    frame8.Frame.Width, frame8.Frame.Height, false, Transparency);
                d.DrawBitmap(frame9.Texture,
                    x + frame9.Frame.CenterX - (fliph ? (frame9.Frame.Width - editorAnim9.Frames[0].Frame.Width) : 0),
                    y - 95 + frame9.Frame.CenterY + (flipv ? (frame9.Frame.Height - editorAnim9.Frames[0].Frame.Height) : 0),
                    frame9.Frame.Width, frame9.Frame.Height, false, Transparency);
                d.DrawBitmap(frame10.Texture,
                    x + frame10.Frame.CenterX - (fliph ? (frame10.Frame.Width - editorAnim10.Frames[0].Frame.Width) : 0),
                    y - 64 + frame10.Frame.CenterY + (flipv ? (frame10.Frame.Height - editorAnim10.Frames[0].Frame.Height) : 0),
                    frame10.Frame.Width, frame10.Frame.Height, false, Transparency);




            }
        }

        public override bool isObjectOnScreen(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency)
        {
            //TO-DO: Improve
            int type = (int)entity.attributesMap["type"].ValueUInt8;
            int height = (int)entity.attributesMap["height"].ValueUInt8;

            int boundsX = 256;
            int boundsY = 160*height + 96;
            return d.IsObjectOnScreen(x - boundsX * height, y - boundsY * height, boundsX * height, boundsY * height);
        }

        public override string GetObjectName()
        {
            return "PopcornMachine";
        }
    }
}
