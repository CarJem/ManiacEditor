using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ManiacEditor;
using Microsoft.Xna.Framework;
using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class ParallaxSprite : EntityRenderer
    {

        public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency)
        {
            bool fliph = false;
            bool flipv = false;
            int aniID = (int)entity.attributesMap["aniID"].ValueUInt8;
            string editorAnimFile = Editor.Instance.SelectedZone.Replace("\\","") + "Parallax";
            var editorAnim = e.LoadAnimation2("EditorIcons2", d, 0, 12, fliph, flipv, false);
            if (Properties.EditorState.Default.ShowParallaxSprites)
            {
                editorAnim = e.LoadAnimation2(editorAnimFile, d, aniID, -1, fliph, flipv, false);
            }
            if (editorAnim != null && editorAnim.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[e.index];

                d.DrawBitmap(frame.Texture,
                    x + frame.Frame.CenterX - (fliph ? (frame.Frame.Width - editorAnim.Frames[0].Frame.Width) : 0),
                    y + frame.Frame.CenterY + (flipv ? (frame.Frame.Height - editorAnim.Frames[0].Frame.Height) : 0),
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }
        }

        public override string GetObjectName()
        {
            return "ParallaxSprite";
        }
    }
}
