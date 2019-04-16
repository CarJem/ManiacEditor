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
    public class SignPost : EntityRenderer
    {

        public override void Draw(GraphicsHandler d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            var editorAnim = Editor.Instance.EntityDrawing.LoadAnimation2("SignPost", d.DevicePanel, 0, -1, false, false, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[Animation.index];
                Animation.ProcessAnimation(1, frame.Entry.Frames.Count, frame.Frame.Delay);
                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame), x + frame.Frame.PivotX, y + frame.Frame.PivotY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }
            if (EditorInGame.SelectedGameVersion == "1.3")
            {
                editorAnim = Editor.Instance.EntityDrawing.LoadAnimation2("SignPost", d.DevicePanel, 4, -1, false, false, false);
                if (editorAnim != null && editorAnim.Frames.Count != 0)
                {
                    for (int i = 0; i < editorAnim.Frames.Count; ++i)
                    {
                        if (i == 1)
                            continue;
                        var frame = editorAnim.Frames[i];
                        d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame), x + frame.Frame.PivotX, y + frame.Frame.PivotY,
                            frame.Frame.Width, frame.Frame.Height, false, Transparency);
                    }
                }
            }
            else
            {
                editorAnim = Editor.Instance.EntityDrawing.LoadAnimation2("SignPost", d.DevicePanel, 6, -1, false, false, false);
                if (editorAnim != null && editorAnim.Frames.Count != 0)
                {
                    for (int i = 0; i < editorAnim.Frames.Count; ++i)
                    {
                        if (i == 1)
                            continue;
                        var frame = editorAnim.Frames[i];
                        d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame), x + frame.Frame.PivotX, y + frame.Frame.PivotY,
                            frame.Frame.Width, frame.Frame.Height, false, Transparency);
                    }
                }
            }

        }

        public override string GetObjectName()
        {
            return "SignPost";
        }
    }
}
