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
    public class EggLoco : EntityRenderer
    {

        public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            bool fliph = false;
            bool flipv = false;
            var editorAnim = e.EditorInstance.EditorEntity_ini.LoadAnimation2("Train", d, 0, -1, fliph, flipv, false);
            var editorAnim2 = e.EditorInstance.EditorEntity_ini.LoadAnimation2("Train", d, 1, -1, fliph, flipv, false);
            var editorAnim3 = e.EditorInstance.EditorEntity_ini.LoadAnimation2("Train", d, 2, -1, fliph, flipv, false);
            var editorAnim4 = e.EditorInstance.EditorEntity_ini.LoadAnimation2("Train", d, 5, 0, false, false, false);
            var editorAnim5 = e.EditorInstance.EditorEntity_ini.LoadAnimation2("Train", d, 7, -1, fliph, flipv, false);
            var editorAnim6 = e.EditorInstance.EditorEntity_ini.LoadAnimation2("Train", d, 6, 0, false, false, false);
            var editorAnim7 = e.EditorInstance.EditorEntity_ini.LoadAnimation2("Train", d, 3, -1, fliph, flipv, false);
            var editorAnim8 = e.EditorInstance.EditorEntity_ini.LoadAnimation2("Train", d, 4, -1, fliph, flipv, false);
            var editorAnim9 = e.EditorInstance.EditorEntity_ini.LoadAnimation2("Train", d, 10, -1, fliph, flipv, false);
            var editorAnim10 = e.EditorInstance.EditorEntity_ini.LoadAnimation2("Train", d, 12, 0, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnim2 != null && editorAnim2.Frames.Count != 0 && editorAnim3 != null && editorAnim3.Frames.Count != 0 && editorAnim4 != null && editorAnim4.Frames.Count != 0 && editorAnim5 != null && editorAnim5.Frames.Count != 0 && editorAnim6 != null && editorAnim6.Frames.Count != 0 && editorAnim7 != null && editorAnim7.Frames.Count != 0 && editorAnim8 != null && editorAnim8.Frames.Count != 0 && editorAnim9 != null && editorAnim9.Frames.Count != 0 && editorAnim10 != null && editorAnim10.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[0];
                var frame2 = editorAnim2.Frames[0];
                var frame3 = editorAnim2.Frames[1];
                var frame4 = editorAnim3.Frames[Animation.index2];
                var frame5 = editorAnim4.Frames[0];
                var frame6 = editorAnim5.Frames[Animation.index];
                var frame7 = editorAnim6.Frames[0];
                var frame8 = editorAnim7.Frames[0];
                var frame9 = editorAnim7.Frames[1];
                var frame10 = editorAnim8.Frames[0];
                var frame11 = editorAnim8.Frames[1];
                var frame12 = editorAnim9.Frames[Animation.index3];
                var frame13 = editorAnim10.Frames[0];

                Animation.ProcessAnimation(frame6.Entry.SpeedMultiplyer, frame6.Entry.Frames.Count, frame6.Frame.Delay);
                Animation.ProcessAnimation2(frame4.Entry.SpeedMultiplyer, frame4.Entry.Frames.Count, frame4.Frame.Delay);
                Animation.ProcessAnimation3(frame12.Entry.SpeedMultiplyer, frame12.Entry.Frames.Count, frame12.Frame.Delay);

                //Eggman
                d.DrawBitmap(frame12.Texture,
                    x + frame12.Frame.PivotX,
                    y + frame12.Frame.PivotY,
                    frame12.Frame.Width, frame12.Frame.Height, false, Transparency);
                d.DrawBitmap(frame13.Texture,
                    x + frame13.Frame.PivotX,
                    y + frame13.Frame.PivotY,
                    frame13.Frame.Width, frame13.Frame.Height, false, Transparency);

                d.DrawBitmap(frame.Texture,
                    x + frame.Frame.PivotX,
                    y + frame.Frame.PivotY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);

                d.DrawBitmap(frame2.Texture,
                    x + frame2.Frame.PivotX - 104,
                    y + frame2.Frame.PivotY + 56,
                    frame2.Frame.Width, frame2.Frame.Height, false, Transparency);
                d.DrawBitmap(frame2.Texture,
                    x + frame2.Frame.PivotX - 30,
                    y + frame2.Frame.PivotY + 56,
                    frame2.Frame.Width, frame2.Frame.Height, false, Transparency);

                d.DrawBitmap(frame3.Texture,
                    x + frame3.Frame.PivotX - 104,
                    y + frame3.Frame.PivotY + 56,
                    frame3.Frame.Width, frame3.Frame.Height, false, Transparency);
                d.DrawBitmap(frame3.Texture,
                    x + frame3.Frame.PivotX - 30,
                    y + frame3.Frame.PivotY + 56,
                    frame3.Frame.Width, frame3.Frame.Height, false, Transparency);

                d.DrawBitmap(frame4.Texture,
                    x + frame4.Frame.PivotX + 22,
                    y + frame4.Frame.PivotY + 74,
                    frame4.Frame.Width, frame4.Frame.Height, false, Transparency);
                d.DrawBitmap(frame4.Texture,
                    x + frame4.Frame.PivotX + 84,
                    y + frame4.Frame.PivotY + 74,
                    frame4.Frame.Width, frame4.Frame.Height, false, Transparency);

                d.DrawBitmap(frame5.Texture,
                    x + frame5.Frame.PivotX,
                    y + frame5.Frame.PivotY,
                    frame5.Frame.Width, frame5.Frame.Height, false, Transparency);
                d.DrawBitmap(frame6.Texture,
                    x + frame6.Frame.PivotX,
                    y + frame6.Frame.PivotY,
                    frame6.Frame.Width, frame6.Frame.Height, false, Transparency);
                d.DrawBitmap(frame7.Texture,
                    x + frame7.Frame.PivotX,
                    y + frame7.Frame.PivotY,
                    frame7.Frame.Width, frame7.Frame.Height, false, Transparency);

                d.DrawBitmap(frame9.Texture,
                    x + frame9.Frame.PivotX - 46,
                    y + frame9.Frame.PivotY + 54,
                    frame9.Frame.Width, frame9.Frame.Height, false, Transparency);
                d.DrawBitmap(frame8.Texture,
                    x + frame8.Frame.PivotX - 122,
                    y + frame8.Frame.PivotY + 54,
                    frame8.Frame.Width, frame8.Frame.Height, false, Transparency);

                d.DrawBitmap(frame10.Texture,
                    x + frame10.Frame.PivotX + 7,
                    y + frame10.Frame.PivotY,
                    frame10.Frame.Width, frame10.Frame.Height, false, Transparency);
                d.DrawBitmap(frame11.Texture,
                    x + frame11.Frame.PivotX,
                    y + frame11.Frame.PivotY,
                    frame11.Frame.Width, frame11.Frame.Height, false, Transparency);




            }
        }

        public override string GetObjectName()
        {
            return "EggLoco";
        }
    }
}
