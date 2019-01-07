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
    public class MSPanel : EntityRenderer
    {

        public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            var editorAnim = e.EditorInstance.EditorEntity_ini.LoadAnimation2("MSPanel", d, 0, 0, false, false, false);
            var editorAnimPanel = e.EditorInstance.EditorEntity_ini.LoadAnimation2("MSPanel", d, 1, -1, false, false, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnimPanel != null && editorAnimPanel.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[0];
                var framePanel = editorAnimPanel.Frames[Animation.index];

                Animation.ProcessAnimation(framePanel.Entry.FrameSpeed, framePanel.Entry.Frames.Count, framePanel.Frame.Duration);

                d.DrawBitmap(frame.Texture,
                    x + frame.Frame.CenterX,
                    y + frame.Frame.CenterY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
                d.DrawBitmap(framePanel.Texture,
                    x + framePanel.Frame.CenterX,
                    y + framePanel.Frame.CenterY,
                    framePanel.Frame.Width, framePanel.Frame.Height, false, Transparency);
            }
        }

        public override string GetObjectName()
        {
            return "MSPanel";
        }
    }
}
