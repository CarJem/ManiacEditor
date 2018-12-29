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
    public class UICharButton : EntityRenderer
    {

        public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {

            //int frameID = (int)entity.attributesMap["listID"].ValueVar;
            int characterID = (int)entity.attributesMap["characterID"].ValueUInt8;
            if (characterID >= 3) characterID++;
            string text = "Text" + Editor.Instance.CurrentLanguage;
            var editorAnim = EditorEntity_ini.LoadAnimation("SaveSelect", d, 0, 0 , false, false, false);
            var editorAnimBorder = EditorEntity_ini.LoadAnimation("SaveSelect", d, 1, characterID, false, false, false);
            var editorAnimBackground = EditorEntity_ini.LoadAnimation("SaveSelect", d, 1, characterID, false, false, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[Animation.index];
                d.DrawBitmap(frame.Texture, x + frame.Frame.CenterX, y + frame.Frame.CenterY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }
            if (editorAnimBorder != null && editorAnimBorder.Frames.Count != 0)
            {
                var frame = editorAnimBorder.Frames[Animation.index];
                d.DrawBitmap(frame.Texture, x + frame.Frame.CenterX, y + frame.Frame.CenterY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }
            if (editorAnimBackground != null && editorAnimBackground.Frames.Count != 0)
            {
                var frame = editorAnimBackground.Frames[Animation.index];
                d.DrawBitmap(frame.Texture, x + frame.Frame.CenterX, y + frame.Frame.CenterY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }


        }

        public override string GetObjectName()
        {
            return "UICharButton";
        }
    }
}
