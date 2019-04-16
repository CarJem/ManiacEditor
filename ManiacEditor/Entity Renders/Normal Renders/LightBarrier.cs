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
    public class LightBarrier : EntityRenderer
    {

        public override void Draw(GraphicsHandler d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            bool fliph = false;
            bool flipv = false;
            bool enabled = entity.attributesMap["enabled"].ValueBool;
            int size = (int)entity.attributesMap["size"].ValueVar;
            var editorAnim = Editor.Instance.EntityDrawing.LoadAnimation2("LightBarrier", d.DevicePanel, 0, -1, fliph, flipv, false);
            var editorAnim2 = Editor.Instance.EntityDrawing.LoadAnimation2("LightBarrier", d.DevicePanel, 0, -1, fliph, true, false);
            var editorAnim3 = Editor.Instance.EntityDrawing.LoadAnimation2("LightBarrier", d.DevicePanel, 0, 1, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnim2 != null && editorAnim2.Frames.Count != 0 && editorAnim3 != null && editorAnim3.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[0];
                var frame2 = editorAnim2.Frames[0];
                var frame3 = editorAnim3.Frames[0];
                int y_start = y + (size / 2) + frame.Frame.PivotY;
                int y_end = y - (size / 2);

                if (enabled == true)
                {
                    int repeat = 0;
                    int lengthMemory = size;
                    int lengthLeft = size;
                    bool finalLoop = false;
                    int i = 0;
                    int sprite_height = frame3.Frame.Height;


                    while (lengthLeft > sprite_height)
                    {
                        repeat++;
                        lengthLeft = lengthLeft - sprite_height;
                    }
                    d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame3),
                        x + frame3.Frame.PivotX - (fliph ? (frame3.Frame.Width - editorAnim3.Frames[0].Frame.Width) : 0),
                        y_end - (i * sprite_height) + (flipv ? (frame3.Frame.Height - editorAnim3.Frames[0].Frame.Height) : 0),
                        frame3.Frame.Width, lengthMemory, false, 128);
                    for (i = 1; i < repeat + 1; i++)
                    {
                        if (i == repeat)
                        {
                            finalLoop = true;
                        }
                        d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame3),
                            x + frame3.Frame.PivotX - (fliph ? (frame3.Frame.Width - editorAnim3.Frames[0].Frame.Width) : 0),
                                y_end + (i * sprite_height) + (flipv ? (frame3.Frame.Height - editorAnim3.Frames[0].Frame.Height) : 0),
                                frame3.Frame.Width, (finalLoop ? lengthLeft : frame3.Frame.Height), false, 128);
                    }
                }



                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame),
                    x + frame.Frame.PivotX + (fliph ? (frame.Frame.Width - editorAnim.Frames[0].Frame.Width * 2) : 0),
                    y + (size/2) + frame.Frame.PivotY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame2),
                    x + frame2.Frame.PivotX + (fliph ? (frame2.Frame.Width - editorAnim2.Frames[0].Frame.Width * 2) : 0),
                    y - (size/2),
                    frame2.Frame.Width, frame2.Frame.Height, false, Transparency);
            }
        }

        public override string GetObjectName()
        {
            return "LightBarrier";
        }
    }
}
