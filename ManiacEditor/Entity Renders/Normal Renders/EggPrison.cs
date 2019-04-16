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
    public class EggPrison : EntityRenderer
    {

        public override void Draw(GraphicsHandler d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            int type = (int)entity.attributesMap["type"].ValueVar;
            bool fliph = false;
            bool flipv = false;
            var editorAnim = Editor.Instance.EntityDrawing.LoadAnimation2("EggPrison", d.DevicePanel, 0, -1, fliph, flipv, false);
            var editorAnim2 = Editor.Instance.EntityDrawing.LoadAnimation2("EggPrison", d.DevicePanel, 1, -1, fliph, flipv, false);
            var editorAnim3 = Editor.Instance.EntityDrawing.LoadAnimation2("EggPrison", d.DevicePanel, 2, -1, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnim2 != null && editorAnim2.Frames.Count != 0 && editorAnim3 != null && editorAnim3.Frames.Count != 0)
            {
                    var frame = editorAnim.Frames[0];
                    var frame2 = editorAnim2.Frames[0];
                    var frame3 = editorAnim3.Frames[0];

                    d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame),
                        x + frame.Frame.PivotX - (fliph ? (frame.Frame.Width - editorAnim.Frames[0].Frame.Width) : 0),
                        y + frame.Frame.PivotY + (flipv ? (frame.Frame.Height - editorAnim.Frames[0].Frame.Height) : 0),
                        frame.Frame.Width, frame.Frame.Height, false, Transparency);
                    d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame2),
                        x + frame2.Frame.PivotX - (fliph ? (frame2.Frame.Width - editorAnim2.Frames[0].Frame.Width) : 0),
                        y + frame2.Frame.PivotY + (flipv ? (frame2.Frame.Height - editorAnim2.Frames[0].Frame.Height) : 0),
                        frame2.Frame.Width, frame2.Frame.Height, false, Transparency);
                    d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame3),
                        x + frame3.Frame.PivotX - (fliph ? (frame3.Frame.Width - editorAnim3.Frames[0].Frame.Width) : 0),
                        y + frame3.Frame.PivotY + (flipv ? (frame3.Frame.Height - editorAnim3.Frames[0].Frame.Height) : 0),
                        frame3.Frame.Width, frame3.Frame.Height, false, Transparency);

            }
        }

        public override string GetObjectName()
        {
            return "EggPrison";
        }
    }
}
