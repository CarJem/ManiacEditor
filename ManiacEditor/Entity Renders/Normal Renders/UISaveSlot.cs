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
    public class UISaveSlot : EntityRenderer
    {

        public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {

            //int frameID = (int)entity.attributesMap["listID"].ValueVar;
            int type = (int)entity.attributesMap["type"].ValueVar;
            string text = "Text" + e.EditorInstance.UIModes.CurrentLanguage;
            var editorAnim = e.EditorInstance.EntityDrawing.LoadAnimation("SaveSelect", d, 0, 0, false, false, false);
            var editorAnimBorder = e.EditorInstance.EntityDrawing.LoadAnimation("SaveSelect", d, 0, 1, false, false, false);
            var editorAnimBackground = e.EditorInstance.EntityDrawing.LoadAnimation("SaveSelect", d, 0, 2, false, false, false);
            var editorAnimText = e.EditorInstance.EntityDrawing.LoadAnimation(text, d, 2, 0, false, false, false);
            var editorAnimNoSave = e.EditorInstance.EntityDrawing.LoadAnimation(text, d, 2, 2, false, false, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[Animation.index];
                d.DrawBitmap(frame.Texture, x + frame.Frame.PivotX, y + frame.Frame.PivotY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }
            if (editorAnimBorder != null && editorAnimBorder.Frames.Count != 0)
            {
                var frame = editorAnimBorder.Frames[Animation.index];
                d.DrawBitmap(frame.Texture, x + frame.Frame.PivotX, y + frame.Frame.PivotY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }
            if (editorAnimBackground != null && editorAnimBackground.Frames.Count != 0)
            {
                var frame = editorAnimBackground.Frames[Animation.index];
                d.DrawBitmap(frame.Texture, x + frame.Frame.PivotX, y + frame.Frame.PivotY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }
            if (editorAnimText != null && editorAnimText.Frames.Count != 0)
            {
                var frame = editorAnimText.Frames[Animation.index];
                d.DrawBitmap(frame.Texture, x + frame.Frame.PivotX, y + frame.Frame.PivotY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }
            if (editorAnimNoSave != null && editorAnimNoSave.Frames.Count != 0 && type == 1)
            {
                var frame = editorAnimNoSave.Frames[Animation.index];
                d.DrawBitmap(frame.Texture, x + frame.Frame.PivotX, y + frame.Frame.PivotY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }


        }

        public override string GetObjectName()
        {
            return "UISaveSlot";
        }
    }
}
