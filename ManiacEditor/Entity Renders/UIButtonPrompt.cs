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
    public class UIButtonPrompt : EntityRenderer
    {

        public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            string text = "Text" + Editor.Instance.CurrentLanguage;
            int frameID = (int)entity.attributesMap["promptID"].ValueVar;
            int listID = (int)entity.attributesMap["buttonID"].ValueVar;
            var editorAnim = EditorEntity_ini.LoadAnimation("Buttons", d, listID, frameID, false, false, false);
            var editorAnimButton = EditorEntity_ini.LoadAnimation(text, d, 0, 0, false, false, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnimButton != null && editorAnimButton.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[Animation.index];
                var frameBackground = editorAnimButton.Frames[Animation.index];
                d.DrawBitmap(frame.Texture, x + frame.Frame.CenterX, y + frame.Frame.CenterY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
                d.DrawBitmap(frameBackground.Texture, x + frameBackground.Frame.CenterX, y + frameBackground.Frame.CenterY,
                    frameBackground.Frame.Width, frameBackground.Frame.Height, false, Transparency);

            }


        }

        public override string GetObjectName()
        {
            return "UIButtonPrompt";
        }
    }
}
